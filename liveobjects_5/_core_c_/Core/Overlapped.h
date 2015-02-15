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

#include <winsock2.h>
#pragma comment(lib,"ws2_32.lib")

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			public class MyOverlapped
			{
			public:

				static const int DefaultNumberOfSenderBuffers = 10;

				enum Operation : int
				{
					Send, Receive, FileIO, PipeConnect, PipeRead, PipeWrite // Initialization = 0, Callback, , Shutdown
				};

				MyOverlapped(Operation operation, __int32 reference, int bufferno)
				{
					ZeroMemory(this, sizeof(MyOverlapped));

					this->operation = operation;
					this->reference = reference;
					this->bufferno = bufferno;

					if (operation == Send)
					{
						nsenderbuffers = DefaultNumberOfSenderBuffers;
						senderbuffers = new WSABUF[nsenderbuffers];
					}
				}
				
				WSAOVERLAPPED overlapped; // OVERLAPPED overlapped;
				Operation operation;
				__int32 reference;
				WSABUF wsabuf;
				WSABUF *senderbuffers;
				int nsenderbuffers;
				struct sockaddr_in remote_address;
				int remote_address_length;
				__int32 bufferno;
			};
		}
	}
}
