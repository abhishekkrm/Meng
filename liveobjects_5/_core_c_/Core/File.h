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

#pragma once

// #include "..\\..\\stdafx.h"
// #include <winsock2.h>
// #pragma comment(lib,"ws2_32.lib")
#include "Overlapped.h"

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace System::Runtime::InteropServices;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			public ref class File sealed : public QS::_core_c_::Core::IFile, System::IDisposable
			{
			public:

				File(IntPtr completion_port, __int32 reference, String ^ filename);
				File(IntPtr completion_port, __int32 reference, String ^ filename, 
					FileMode mode, FileAccess access, FileShare share, FileFlagsAndAttributes flagsAndAttributes);
				~File();

				#pragma region IFile Members

				virtual void Write(Int64 offset, IntPtr buffer, Int32 count, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context) = QS::_core_c_::Core::IFile::Write;

				virtual void Write(Int64 offset, Object^ obj, Int32 count);

				virtual void Write(Int64 position, ArraySegment<byte> data, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context) = QS::_core_c_::Core::IFile::Write;

				virtual void Read(Int64 position, ArraySegment<byte> data, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context) = QS::_core_c_::Core::IFile::Read;

				virtual IAsyncResult^ BeginWrite(Int64 position, ArraySegment<byte> data, AsyncCallback^ callback, Object^ state) = QS::_core_c_::Core::IFile::BeginWrite;
				
				virtual int EndWrite(IAsyncResult^ result) = QS::_core_c_::Core::IFile::EndWrite;

				virtual IAsyncResult^ BeginRead(Int64 position, ArraySegment<byte> data, AsyncCallback^ callback, Object^ state) = QS::_core_c_::Core::IFile::BeginRead;
				
				virtual int EndRead(IAsyncResult^ result) = QS::_core_c_::Core::IFile::EndRead;

				virtual property String^ Name
				{
					String^ get() = QS::_core_c_::Core::IFile::Name::get
					{ 
						return filename; 
					}
				}

				virtual property Int64 Length
				{
					Int64 get() = QS::_core_c_::Core::IFile::Length::get;
				}

				#pragma endregion

				void CompletedFileIO(MyOverlapped *poverlapped, unsigned int ntransmitted, bool succeeded);

			private:

				#pragma region Class AsynchronousOperation

				ref class AsynchronousOperation sealed : IAsyncResult
				{
				public:

					AsynchronousOperation(AsyncCallback^ callback, Object^ state)
					{
						this->callback = callback;
						this->state = state;
					}

					void Completed(bool success, unsigned int ntransmitted, Exception^ error)
					{
						completed = true;
						this->ntransmitted = ntransmitted;
						
						if (callback != nullptr)
							callback(this);
					}

					#pragma region IAsyncResult Members

					virtual property bool IsCompleted 
					{
						bool get() = IAsyncResult::IsCompleted::get
						{
							return this->completed;
						}
					}

					virtual property Object^ AsyncState 
					{
						Object^ get() = IAsyncResult::AsyncState::get
						{
							return this->state;
						}
					}

					virtual property WaitHandle^ AsyncWaitHandle 
					{
						WaitHandle^ get() = IAsyncResult::AsyncWaitHandle::get
						{
							throw gcnew NotSupportedException(
								"Cannot return wait handle, this is a single-threaded platform that does not support blocking operations.");
						}
					}

					virtual property bool CompletedSynchronously 
					{
						bool get() = IAsyncResult::CompletedSynchronously::get
						{
							return false;
						}
					}

					#pragma endregion

					property int NumberOfBytesTransmitted
					{
						int get() { return ntransmitted; }
					}

				private:

					bool completed;
					AsyncCallback^ callback;
					Object^ state;
					int ntransmitted;
				};

				#pragma endregion

				#pragma region Struct Operation

				value struct Operation
				{
				public:

					enum class Type : int
					{
						Write, Read
					};

					Operation(Type type, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context, Object^ handle)
					{
						this->type = type;
						this->callback = callback;
						this->context = context;
						this->handle = handle;
					};

					Type type;
					QS::Fx::Base::IOCompletionCallback^ callback;
					Object ^context, ^handle;
				};

				#pragma endregion

				String^ filename;
				__int32 reference;
				IntPtr completion_port;
				int invalidated;
				bool closed;
				IntPtr file_handle;
				int lastoperation_seqno;
				System::Collections::Generic::IDictionary<int, Operation>^ operations;
				QS::Fx::Base::IOCompletionCallback^ asynchronousOperationCallback;

				void initialize(IntPtr completion_port, __int32 reference, String ^ filename, 
						FileMode mode, FileAccess access, FileShare share, FileFlagsAndAttributes flagsAndAttributes);

				void _Write(Int64 offset, IntPtr buffer, Int32 count, Object^ handle, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context);
				void _Read(Int64 offset, IntPtr buffer, Int32 count, Object^ handle, QS::Fx::Base::IOCompletionCallback^ callback, Object^ context);

				__inline HANDLE handle() 
				{
					HANDLE handle = (HANDLE) this->file_handle;
					if (invalidated)
						throw gcnew Exception("File is invalid and cannot be accessed.");
					return handle;
				}

//				void UnpinCallback(bool succeeded, Exception^ exception, Object ^context);

				void AsynchronousOperationCallback(bool success, unsigned int ntransmitted, Exception^ error, Object^ context)
				{
					dynamic_cast<AsynchronousOperation^>(context)->Completed(success, ntransmitted, error);
				}
			};
		}
	}
}
