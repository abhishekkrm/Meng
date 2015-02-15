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

#include "Overlapped.h"
#include "Buffer.h"
#include "Header.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			public ref class Channel sealed : public IChannel
			{
			public:

				Channel(ICore^ core, String^ id, String ^name, __int32 reference, IntPtr incomingpipe, IntPtr outgoingpipe, 
					IntPtr incomingmemory, IntPtr outgoingmemory, IntPtr incomingbuffer, IntPtr outgoingbuffer, IChannel^ channelclient,
					bool isincomingpipeconnected, bool isoutgoingpipeconnected);
				~Channel();

				void Incoming();
				void Outgoing();

				virtual void Handle(ChannelObject message) = IChannel::Handle;
				
			private:

				value class Request
				{
				public:
					
					Request(long from, long to, QS::_core_c_::Core::ChannelObject object) : from(from), to(to), object(object)
					{
					}

					long from, to;
					QS::_core_c_::Core::ChannelObject object;
				};

				ICore^ core;
				IChannel^ channelclient;
				String ^id, ^name;
				__int32 reference;
				bool isincomingpipeconnected, isoutgoingpipeconnected, isincomingpipereceiving, isoutgoingpipesending,
					issending;
				IntPtr incomingpipe, outgoingpipe, incomingmemory, outgoingmemory;
				Buffer *incomingbuffer, *outgoingbuffer;
				char *incomingcontrol, *outgoingcontrol;
				MyOverlapped *incomingoverlapped, *outgoingoverlapped;
				System::Collections::Generic::Queue<QS::_core_c_::Core::ChannelObject> ^outgoingobjects;
				long registered;
				cli::array<Request,1>^ requests;
				__int32 scheduled, completed;

				void Outgoing1(ChannelObject message);
				void Outgoing2();
				void Outgoing3();

				void Incoming1();
				void Incoming2();
				void Incoming3();
			};
		}
	}
}
