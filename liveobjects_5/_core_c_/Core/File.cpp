/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

Redistribution and use in source and binary forms,
with or without modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above
   copyright notice, this list of conditions and the following
   disclaimer in the documentation and/or other materials provided
   with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S)
AND ALL OTHER CONTRIBUTORS AND ANY EXPRESS OR IMPLIED WARRANTIES,
INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE ABOVE COPYRIGHT HOLDER(S) OR ANY OTHER
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF
USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
SUCH DAMAGE.

*/

#include "stdafx.h"
#include <winsock2.h>
#include <vcclr.h>

#include "File.h"
#include "Overlapped.h"

using namespace System;
using namespace System::Collections;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			#pragma region Constructor and destructor

			File::File(IntPtr completion_port, __int32 reference, String ^ filename) 
			{
				initialize(completion_port, reference, filename, 
					FileMode::OpenOrCreate, FileAccess::ReadWrite, FileShare::None, FileFlagsAndAttributes::None);
			}

			File::File(IntPtr completion_port, __int32 reference, String ^ filename, 
				FileMode f_mode, FileAccess f_access, FileShare f_share, FileFlagsAndAttributes f_flagsAndAttributes)
			{
				initialize(completion_port, reference, filename, f_mode, f_access, f_share, f_flagsAndAttributes);
			}

			void File::initialize(IntPtr completion_port, __int32 reference, String ^ filename, 
				FileMode f_mode, FileAccess f_access, FileShare f_share, FileFlagsAndAttributes f_flagsAndAttributes)
			{
				this->completion_port = completion_port;
				this->filename = filename;
				this->reference = reference;
				this->operations = gcnew Dictionary<int, Operation>(); 
				this->asynchronousOperationCallback = gcnew QS::Fx::Base::IOCompletionCallback(this, &File::AsynchronousOperationCallback);

				DWORD dwDesiredAccess = 0, dwShareMode = 0, dwCreationDisposition = 0, 
					dwFlagsAndAttributes = FILE_ATTRIBUTE_NORMAL | FILE_FLAG_OVERLAPPED;

				switch (f_mode)
				{
				case FileMode::CreateNew:
					dwCreationDisposition = CREATE_NEW;
					break;

				case FileMode::Create:
					dwCreationDisposition = CREATE_ALWAYS;
					break;

				case FileMode::Open:
					dwCreationDisposition = OPEN_EXISTING;
					break;

				case FileMode::OpenOrCreate:
					dwCreationDisposition = OPEN_ALWAYS;
					break;

				case FileMode::Truncate:
					dwCreationDisposition = TRUNCATE_EXISTING;
					break;

				case FileMode::Append:
					throw gcnew Exception("Attribute FileMode::Append is not supported.");
				}

				switch (f_access)
				{
				case FileAccess::Read:
					dwDesiredAccess = GENERIC_READ;
					break;

				case FileAccess::Write:
					dwDesiredAccess = GENERIC_WRITE;
					break;

				case FileAccess::ReadWrite:
					dwDesiredAccess = GENERIC_READ | GENERIC_WRITE;
					break;
				}

				switch (f_share)
				{
				case FileShare::None:
					dwShareMode = 0;
					break;

				case FileShare::Read:
					dwShareMode = FILE_SHARE_READ;
					break;

				case FileShare::Write:
					dwShareMode = FILE_SHARE_WRITE;
					break;

				case FileShare::ReadWrite:
					dwShareMode = FILE_SHARE_READ | FILE_SHARE_WRITE;
					break;

				case FileShare::Inheritable:
					throw gcnew Exception("Attribute FileShare::Inheritable is not supported.");

				case FileShare::Delete:
					throw gcnew Exception("Attribute FileShare::Delete is not supported.");
				}

				if ((f_flagsAndAttributes & FileFlagsAndAttributes::WriteThrough) == FileFlagsAndAttributes::WriteThrough)
					dwFlagsAndAttributes |= FILE_FLAG_WRITE_THROUGH;

				pin_ptr<const wchar_t> pinned_filename = PtrToStringChars(filename);
				file_handle = (IntPtr) CreateFile(((LPCWSTR) ((wchar_t *) pinned_filename)), 
					dwDesiredAccess, dwShareMode, NULL, dwCreationDisposition, dwFlagsAndAttributes, NULL);

				if ((HANDLE) file_handle == INVALID_HANDLE_VALUE)
					throw gcnew Exception("Could not open file \"" + filename + "\".");

				if (CreateIoCompletionPort(
					(HANDLE) file_handle, (HANDLE) completion_port, 0, 0) != (HANDLE) completion_port)
					throw gcnew Exception("Could not associate file \"" + filename + "\" with the completion port.");
			}

			File::~File() 
			{
				if (!Interlocked::CompareExchange(invalidated, 1, 0))
				{
					Monitor::Enter(this);
					try
					{
						if (operations->Count == 0 && !closed)
						{
							CloseHandle((HANDLE) file_handle);
							closed = true;
						}
					}
					finally
					{
						Monitor::Exit(this);
					}
				}
			}

			#pragma endregion

			#pragma region Writing

			IAsyncResult^ File::BeginWrite(Int64 position, ArraySegment<byte> data, AsyncCallback^ callback, Object^ state)
			{
				AsynchronousOperation^ asynchronousOperation = gcnew AsynchronousOperation(callback, state);
				Write(position, data, this->asynchronousOperationCallback, asynchronousOperation);				
				return asynchronousOperation;
			}

			int File::EndWrite(IAsyncResult^ result)
			{
				return dynamic_cast<AsynchronousOperation^>(result)->NumberOfBytesTransmitted;
			}

			void File::Write(Int64 position, ArraySegment<byte> data, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context)
			{
				GCHandle handle = GCHandle::Alloc(data.Array, GCHandleType::Pinned);				
				IntPtr address = (IntPtr)((LPVOID)(((DWORD) ((LPVOID) handle.AddrOfPinnedObject())) + data.Offset));
				_Write(position, address, data.Count, handle, callback, context);
			}

			void File::Write(Int64 offset, IntPtr buffer, Int32 count, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context)
			{
				_Write(offset, buffer, count, nullptr, callback, context);
			}

			void File::_Write(Int64 offset, IntPtr buffer, Int32 count, Object^ handleobj, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context)
			{
				HANDLE handle = this->handle();				
				int operation_seqno = ++lastoperation_seqno;

				Monitor::Enter(this);
				try
				{
					operations->Add(operation_seqno, Operation(Operation::Type::Write, callback, context, handleobj));
				}
				finally
				{
					Monitor::Exit(this);
				}

				MyOverlapped *poverlapped = new MyOverlapped(MyOverlapped::FileIO, reference, operation_seqno);
		
				__int64 debug_address = (__int64) ((void *) poverlapped);
				__int32 debug_count = (__int32) sizeof(MyOverlapped);

				// System::Windows::Forms::MessageBox::Show("debug_address = " + debug_address.ToString() + "\ndebug_count = " + debug_count.ToString() + "\n");

				// poverlapped->overlapped.Offset = *((int*) &offset);
				// poverlapped->overlapped.OffsetHigh = *(((int*) &offset) + 1);
				*((Int64*) &poverlapped->overlapped.Offset) = offset;

				DWORD nwritten;
				if (!WriteFile(handle, (LPVOID) buffer, (DWORD) count, &nwritten, (LPOVERLAPPED) poverlapped))
				{
					// if (callback != nullptr)
					//	callback(false, 0, nullptr, context);
				}
			}

			void File::Write(Int64 offset, Object^ obj, Int32 count)
			{
				GCHandle handle = GCHandle::Alloc(obj, GCHandleType::Pinned);
				IntPtr address = handle.AddrOfPinnedObject();
				_Write(offset, address, count, handle, nullptr, nullptr);
			}

//			void File::UnpinCallback(bool succeeded, Exception^ exception, Object ^context)
//			{
//				((GCHandle) context).Free();
//			}

			#pragma endregion

			#pragma region Completion

			void File::CompletedFileIO(MyOverlapped *poverlapped, unsigned int ntransmitted, bool succeeded)
			{
				int operation_seqno = poverlapped->bufferno;
				Operation operation;
				bool operation_located;

				Monitor::Enter(this);
				try
				{
					operation_located = operations->TryGetValue(operation_seqno, operation);
					if (operation_located)
						operations->Remove(operation_seqno);

					if (invalidated == 1 && operations->Count == 0 && !closed)
					{
						CloseHandle((HANDLE) file_handle);
						closed = true;
					}
				}
				finally 
				{
					Monitor::Exit(this);
				}
						
				try
				{
					if (succeeded)
						delete poverlapped;
				}
				catch (Exception^ exc)
				{
					// throw;
				}

				if (operation_located)
				{
					if (operation.handle != nullptr)
					{
						((GCHandle) (operation.handle)).Free();
						operation.handle = nullptr;
					}

					if (operation.callback != nullptr)
						operation.callback(succeeded, ntransmitted, nullptr, operation.context);
				}
				else
				{
					// TODO: Do something about this..........????????????????????
				}
			}
			
			#pragma endregion

			#pragma region Reading

			IAsyncResult^ File::BeginRead(Int64 position, ArraySegment<byte> data, AsyncCallback^ callback, Object^ state)
			{
				AsynchronousOperation^ asynchronousOperation = gcnew AsynchronousOperation(callback, state);
				Read(position, data, this->asynchronousOperationCallback, asynchronousOperation);				
				return asynchronousOperation;
			}

			int File::EndRead(IAsyncResult^ result)
			{
				return dynamic_cast<AsynchronousOperation^>(result)->NumberOfBytesTransmitted;
			}

			void File::Read(Int64 position, ArraySegment<byte> data, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context)
			{
				GCHandle handle = GCHandle::Alloc(data.Array, GCHandleType::Pinned);				
				IntPtr address = (IntPtr)((LPVOID)(((DWORD) ((LPVOID) handle.AddrOfPinnedObject())) + data.Offset));
				_Read(position, address, data.Count, handle, callback, context);
			}

			void File::_Read(Int64 offset, IntPtr buffer, Int32 count, Object^ handleobj, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context)
			{
				HANDLE handle = this->handle();				
				int operation_seqno = ++lastoperation_seqno;

				Monitor::Enter(this);
				try
				{
					operations->Add(operation_seqno, Operation(Operation::Type::Read, callback, context, handleobj));
				}
				finally
				{
					Monitor::Exit(this);
				}

				MyOverlapped *poverlapped = new MyOverlapped(MyOverlapped::FileIO, reference, operation_seqno);
		
				// poverlapped->overlapped.Offset = *((int*) &offset);
				// poverlapped->overlapped.OffsetHigh = *(((int*) &offset) + 1);
				*((Int64*) &poverlapped->overlapped.Offset) = offset;

				DWORD nwritten;
				ReadFile(handle, (LPVOID) buffer, (DWORD) count, &nwritten, (LPOVERLAPPED) poverlapped);
			}

			#pragma endregion

			#pragma region Accessing file properties

			Int64 File::Length::get()
			{						
				DWORD filesize; 
				if ((filesize = GetFileSize(this->handle(), NULL)) == 0xFFFFFFFF)
					throw gcnew Exception("Coult not get file size, GetLastError returned " + ((int)GetLastError()).ToString() + ".");
				
				return (Int64) filesize;
			}

			#pragma endregion
		}
	}
}
