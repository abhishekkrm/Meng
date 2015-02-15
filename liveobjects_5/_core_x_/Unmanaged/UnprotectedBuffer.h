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
#include <windows.h>

namespace QS
{
	namespace _core_x_
	{
		namespace Unmanaged
		{
			class __declspec(dllexport) UnprotectedBuffer sealed
			{
			public:

				__inline UnprotectedBuffer()
				{
					nrequests = 0;
					requests = NULL;
					nenqueued = 0;
					ndequeued = 0;
				}

				__inline void Enqueue(QS::Fx::Unmanaged::Message request)
				{
					if (nrequests > 0)
					{
						if (nenqueued == ndequeued + nrequests)
						{
							int _nrequests = 2 * nrequests;
							QS::Fx::Unmanaged::Message *_requests = new QS::Fx::Unmanaged::Message[_nrequests];

							int _n1 = ndequeued % nrequests;
							int _n2 = nrequests - _n1;
							if (_n2 > 0)
								CopyMemory(_requests, requests + _n1, _n2 * sizeof(QS::Fx::Unmanaged::Message));
							if (_n1 > 0)
								CopyMemory(_requests + _n2, requests, _n1 * sizeof(QS::Fx::Unmanaged::Message)); 
							
							delete requests;
							requests = _requests;
							nrequests = _nrequests;
							nenqueued -= ndequeued;
							ndequeued = 0;
						}
					}
					else
					{
						nrequests = 1000;
						requests = new QS::Fx::Unmanaged::Message[nrequests];
					}

					requests[nenqueued % nrequests] = request;
					nenqueued++;
				}

				__inline bool Dequeue(QS::Fx::Unmanaged::Message *prequest)
				{
					bool nonempty = nenqueued > ndequeued;		
					if (nonempty)
					{
						*prequest = requests[ndequeued % nrequests];
						ndequeued++;
					}
					return nonempty;
				}

				__inline bool IsEmpty()
				{
					return ndequeued == nenqueued;
				}

				__inline bool Peek(QS::Fx::Unmanaged::Message *prequest)
				{
					bool nonempty = nenqueued > ndequeued;		
					if (nonempty)
						*prequest = requests[ndequeued % nrequests];
					return nonempty;
				}

			private:

				int nrequests, nenqueued, ndequeued;
				QS::Fx::Unmanaged::Message *requests;
			};
		}
	}
}
