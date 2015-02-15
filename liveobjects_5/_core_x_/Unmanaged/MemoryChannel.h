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

#include "../../../liveobjects_2/Message.h"

namespace QS
{
	namespace _core_x_
	{
		namespace Unmanaged
		{
			class __declspec(dllexport) MemoryChannel sealed
			{
			public:

				static MemoryChannel *Create(int nrequests, HANDLE handle)
				{
					MemoryChannel *pchannel = (MemoryChannel *) malloc(sizeof(MemoryChannel) + nrequests * sizeof(QS::Fx::Unmanaged::Message));

					pchannel->blocked = false;
					pchannel->handle = handle;
					pchannel->nrequests = nrequests;
					pchannel->nenqueued = 0;
					pchannel->ndequeued = 0;

					return pchannel;
				}

				static void Delete(MemoryChannel *pchannel)
				{
					free((void *) pchannel);
				}

				__inline bool Enqueue(QS::Fx::Unmanaged::Message request)
				{
					if (ndequeued > nenqueued - nrequests)
					{
						requests[nenqueued % nrequests] = request;
						nenqueued++;
						return true;
					}
					else
						return false;
				}

				__inline bool Dequeue(QS::Fx::Unmanaged::Message *prequest)
				{
					if (nenqueued > ndequeued)
					{
						*prequest = requests[ndequeued % nrequests];
						ndequeued++;
						return true;
					}
					else
						return false;
				}

				__inline bool IsEmpty()
				{
					return ndequeued == nenqueued;
				}

				__inline bool IsFull()
				{
					return nenqueued == ndequeued + nrequests;
				}

				__inline bool Peek(QS::Fx::Unmanaged::Message *prequest)
				{
					if (nenqueued > ndequeued)
					{
						*prequest = requests[ndequeued % nrequests];
						return true;
					}
					else
						return false;
				}

				HANDLE handle;
				bool blocked;

			private:

				volatile int nrequests, nenqueued, ndequeued;
			#pragma warning(disable:4200)
				QS::Fx::Unmanaged::Message requests[0];	
			#pragma warning(default:4200)
			};
		}
	}
}
