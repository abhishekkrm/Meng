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

#include "StdAfx.h"
#include "ReceiveBuffer.h"

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{			
			#pragma region ReceiveBuffer Constructor

			ReceiveBuffer::ReceiveBuffer(Controller^ controller, __int32 ownerid, __int32 id, int size, int nreferences)
			{
				this->controller = controller;
				this->ownerid = ownerid;
				this->id = id;
				this->size = size;
				this->nreferences = nreferences;
				this->next = nullptr;
				this->buffer = gcnew array<unsigned char>(size);
				this->handle = GCHandle::Alloc(this->buffer, GCHandleType::Pinned);
				this->address = this->handle.AddrOfPinnedObject();
			}

			#pragma endregion

			#pragma region ReceiveBuffer Destructor

			ReceiveBuffer::~ReceiveBuffer()
			{
				this->address = IntPtr::Zero;
				this->handle.Free();
				this->buffer = nullptr;
				this->size = 0;
			}

			#pragma endregion

			#pragma region ReceiveBuffer AddReference

/*
			void ReceiveBuffer::AddReference()
			{
				System::Diagnostics::Debug::Assert(nreferences > 0);
				System::Diagnostics::Debug::Assert(next == nullptr);

				nreferences++;
			}
*/

			#pragma endregion

			#pragma region ReceiveBuffer Release

/*
			void ReceiveBuffer::Release()
			{
				System::Diagnostics::Debug::Assert(nreferences > 0);
				System::Diagnostics::Debug::Assert(next == nullptr);

				nreferences--;

				if (nreferences == 0)
					controller->Release(this);
			}
*/

			#pragma endregion

			#pragma region ReceiveBuffer::Controller Constructor

			ReceiveBuffer::Controller::Controller(unsigned __int32 id, int buffersize)
			{
				this->id = id;
				this->buffersize = buffersize;
				this->nused = 0;
				this->nfree = 0;
				this->head = nullptr;
				this->tail = nullptr;
				this->lastid = 0;
				this->buffers = gcnew Dictionary<__int32, ReceiveBuffer^>();
			}

			#pragma endregion

			#pragma region ReceiveBuffer::Controller Destructor

			ReceiveBuffer::Controller::~Controller()
			{
			}

			#pragma endregion

			#pragma region ReceiveBuffer::Controller Allocate

			ReceiveBuffer^ ReceiveBuffer::Controller::Allocate()
			{
				nused++;

				if (head != nullptr)
				{
					nfree--;
					ReceiveBuffer^ buffer = head;
					head = head->next;
					if (head == nullptr)
						tail = nullptr;
					buffer->next = nullptr;
					
					System::Diagnostics::Debug::Assert(buffer->nreferences == 0);
					
					return buffer;
				}
				else
				{
					__int32 id = ++lastid;
					ReceiveBuffer^ buffer = gcnew ReceiveBuffer(this, this->id, id, this->buffersize, 0);
					buffers->Add(id, buffer);
					return buffer;
				}
			}

			#pragma endregion

			#pragma region ReceiveBuffer::Controller Release

			void ReceiveBuffer::Controller::Release(ReceiveBuffer^ buffer)
			{
				System::Diagnostics::Debug::Assert(buffer->nreferences == 0);
				System::Diagnostics::Debug::Assert(buffer->next == nullptr);

				nused--;
				nfree++;
				if (tail != nullptr)
					tail->next = buffer;
				else
					head = buffer;
				tail = buffer;
				buffer->next = nullptr;
			}

			#pragma endregion

			#pragma region ReceiveBuffer::Controller Lookup

			ReceiveBuffer^ ReceiveBuffer::Controller::Lookup(__int32 id)
			{
				return buffers[id];
			}

			#pragma endregion
		}
	}
}
