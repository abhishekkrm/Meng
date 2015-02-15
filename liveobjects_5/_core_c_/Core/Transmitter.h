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

// #include "ISender.h"
// #include "AsynchronousSend.h"
// #include "IIOController.h"
// #include "ITransmitter.h"
// #include "ITransmitterController.h"
// #include "ErrorCallback.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

#define DEBUG_TagOutgoingStreams
#define DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads
#define DEBUG_ProvideDetailedProfilingInformationForTransmissionCompletionOverheads

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			[QS::Fx::Base::Inspectable]
			public ref class Transmitter sealed : public QS::Fx::Inspection::Inspectable, public ITransmitter
			{
			public:

				#pragma region TaggingOutgoingStreams

				const static bool TaggingOutgoingStreams = 
#if defined(DEBUG_TagOutgoingStreams)
					true
#else
					false
#endif
					;

				#pragma endregion

				Transmitter(ITransmitterController^ owner, QS::Fx::Logging::ILogger^ logger, QS::Fx::Clock::IClock^ clock, ::Socket^ socket, void *poverlapped, ErrorCallback^ errorCallback, 
					int streamid, bool high_priority)
				{
					this->logger = logger;
					this->clock = clock;
					this->streamid = streamid;
					this->owner = owner;
					this->socket = socket;
					this->poverlapped = poverlapped;
					this->errorCallback = errorCallback;
					this->high_priority = high_priority;

					bufferHandles = gcnew Queue<GCHandle>();

#if defined(DEBUG_TagOutgoingStreams)
					miniheader = new unsigned char[8];
					((int*) miniheader)[0] = streamid;
#endif
				}

				void Transmit(QS::Fx::Network::AsynchronousSend request, int seqno);
				void Completed();

				property ::Socket^ Socket
				{
					::Socket^ get() { return socket; }
				}

				property bool HighPriority
				{
					bool	get() { return high_priority; }
				}

				property ITransmitterController^ Controller
				{
					ITransmitterController^ get() { return owner; }
				}

			private:

				int streamid;
				bool high_priority;

#if defined(DEBUG_TagOutgoingStreams)
				unsigned char *miniheader; 
#endif

				ITransmitterController^ owner;
				QS::Fx::Clock::IClock^ clock;
				QS::Fx::Logging::ILogger^ logger;
				[QS::Fx::Inspection::Ignore]
				::Socket^ socket;
				[QS::Fx::Inspection::Ignore]
				void *poverlapped;
				QS::Fx::Network::Data data;
				QS::Fx::Base::ContextCallback^ completionCallback;
				Object^ context;
				[QS::Fx::Inspection::Ignore]
				Queue<GCHandle>^ bufferHandles; 
				ErrorCallback^ errorCallback;

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionOverheads)
				int nsamples;
				double last_checked, cumulated_d1, cumulated_d2, cumulated_d3, cumulated_d4;
#endif

#if defined(DEBUG_ProvideDetailedProfilingInformationForTransmissionCompletionOverheads)
				int completion_nsamples;
				double completion_last_checked, completion_cumulated_d1, completion_cumulated_d2, completion_cumulated_d3;
#endif
			};
		}
	}
}
