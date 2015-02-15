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

#include "Exception.h"

namespace QS
{
	namespace Fx
	{
		namespace Unmanaged
		{
			// NOTE: This class represents the requests that may be exchanged between the application and the host.
			// Normally, it would always be passed by value.

			class __declspec(dllexport) Message sealed
			{
			public:

				// NOTE: These are the possible types of requests.

				enum __declspec(dllexport) Type : unsigned char
				{
					// NOTE: Data transmission; should eventually be followed by a matching response to free the memory.

					Data = 1,

					// NOTE: A message to log; also to be eventually followed by a matching response to free the memory.

					Log = 2
				};

				__inline Message(Type _type, bool _respond, void *_data, unsigned __int32 _size, unsigned __int32 _channel, unsigned __int64 _cookie) 
					: type(_type), respond(_respond), isresponse(false), data(_data), size(_size), channel(_channel), cookie(_cookie)
				{
				}

				__inline static Message ResponseTo(Message message) 			
				{
					if (message.isresponse)
						throw new Exception(_T("Cannot produce a response to a response."));

					if (!message.respond)
						throw new Exception(_T("Cannot produce a response to a request for which response is not expected."));

					Message response = message;
					response.respond = false;
					response.isresponse = true;
					return response;
				}

				__inline static Message Response(Type _type, void *_data, unsigned __int32 _size, unsigned __int32 _channel, unsigned __int64 _cookie)					
				{
					Message response;
					response.type = _type;
					response.respond = false;
					response.isresponse = true;
					response.data = _data;
					response.size = _size;
					response.channel = _channel;
					response.cookie = _cookie;
					return response;
				}

				__inline Message()
				{
				}

				// message type
				Type type;

				// this field indicates whether a response should follow this request
				bool respond;

				// indicates whether this message is a response; false = request, true = response
				bool isresponse;

				// data is implicitly assumed to be owned by the sender of the message
				void *data;

				// the size of the data block
				unsigned __int32 size;

				// channel number, wherever applicable
				unsigned __int32 channel;

				// must be copied to the response; can be used to identify the message
				unsigned __int64 cookie;
			};
		}
	}
}
