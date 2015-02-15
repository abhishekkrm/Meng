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

// #include "IListener.h"
// #include "IListenerController.h"
// #include "ReceiveCallback.h"
#include "ReceiveBuffer.h"

using namespace System;
using namespace System::Threading;
using namespace System::Runtime::InteropServices;
using namespace System::Net;
using namespace System::Net::Sockets;
using namespace System::Collections::Generic;

// newly enabled
// #define DEBUG_CollectBytesWaiting
// #define DEBUG_CollectReceiveTypes
#define DEBUG_LogOverheads

#define OPTION_USE_BUFFER_MANAGER

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			[QS::_core_c_::Diagnostics::ComponentContainerAttribute]
			[QS::Fx::Base::Inspectable]
			public ref class Listener sealed : public QS::Fx::Inspection::Inspectable, public IListener, public QS::_core_c_::Diagnostics2::IModule
			{
			public:

				#pragma region Constructor and destructor

				Listener(IListenerController^ owner, unsigned __int32 reference, QS::_core_c_::Core::Address^ address, ::Socket^ socket, 
					QS::Fx::Network::ReceiveCallback^ callback, Object^ context, int bufferSize, int numberOfBuffers, bool drainSynchronously,
					QS::_core_c_::Statistics::IStatisticsController^ statisticsController, bool high_priority, int myadfbuffersize)
				{
					this->disposed = false;

					this->parameters = gcnew QS::_core_x_::Base::Parameters();

//					if (drainSynchronously && numberOfBuffers > 1)
//						throw gcnew Exception("Synchronous draining of sockets should not be used with multiple receive buffers.");

					diagnosticsContainer = gcnew QS::_core_c_::Diagnostics2::Container();

					this->statisticsController = statisticsController;
					this->owner = owner;
					this->address = address;
					this->callback = callback;
					this->context = context;
					this->active = false;
					this->socket = socket;
					this->high_priority = high_priority;
					this->myadfbuffersize = myadfbuffersize;

					this->numberOfBuffers = numberOfBuffers;
					this->bufferSize = bufferSize;
					this->drainSynchronously = drainSynchronously;

#if defined(OPTION_USE_BUFFER_MANAGER)
				
					buffercontroller = gcnew ReceiveBuffer::Controller(reference, bufferSize);

#else

					this->buffers = gcnew array<array<unsigned char>^>(numberOfBuffers);
					this->bufferHandles = gcnew array<GCHandle>(numberOfBuffers);
					this->bufferAddresses = gcnew array<IntPtr>(numberOfBuffers);

					for (int ind = 0; ind < numberOfBuffers; ind++)
					{
						this->buffers[ind] = gcnew array<unsigned char>(bufferSize);
						this->bufferHandles[ind] = GCHandle::Alloc(this->buffers[ind], GCHandleType::Pinned);
						this->bufferAddresses[ind] = this->bufferHandles[ind].AddrOfPinnedObject();
					}

#endif

#if defined(DEBUG_CollectBytesWaiting)
					this->timeseries_bytesWaiting = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_CollectReceiveTypes)
					this->timeseries_receiveTypes = gcnew List<QS::_core_e_::Data::XY>();
#endif

#if defined(DEBUG_LogOverheads)
					this->ts_ProcessingOverheads = statisticsController->Allocate2D("processing overheads", "", "time", "s", "", "overhead", "s", "");
#endif

					this->parameters->RegisterLocal(this);
					QS::_core_c_::Diagnostics2::Helper::RegisterLocal(diagnosticsContainer, this);
				}

				~Listener()
				{
					this->disposed = true;
					owner->Unregister(this);
					socket->Close();
				}

				#pragma endregion

				#pragma region Accessors

				property bool Disposed
				{
					bool get() { return this->disposed; }
				}

				property bool HighPriority
				{
					bool	get() { return high_priority; }
				}

				virtual property QS::_core_c_::Core::Address^ CombinedAddress
				{
					QS::_core_c_::Core::Address^ get() = QS::_core_c_::Core::IListener::CombinedAddress::get
					{ 
						return address; 
					}
				}

				virtual property System::Net::IPAddress^ InterfaceAddress
				{
					System::Net::IPAddress^ get() = QS::Fx::Network::IListener::InterfaceAddress::get
					{
						return address->NIC;
					}
				}

				virtual property QS::Fx::Network::NetworkAddress^ NetworkAddress
				{
					QS::Fx::Network::NetworkAddress^ get() = QS::Fx::Network::IListener::Address::get
					{
						return gcnew QS::Fx::Network::NetworkAddress(address->IPAddress, address->PortNumber);
					}
				}

				property Address^ CoreAddress
				{
					Address^ get() { return address; }
				}

				property Object^ Context
				{
					Object^ get() { return context; }
				}

				property int BufferSize
				{
					int get() { return bufferSize; }
				}

				property ::Socket^ Socket
				{
					::Socket^ get() { return socket; }
				}

				property QS::Fx::Network::ReceiveCallback^ Callback
				{
					QS::Fx::Network::ReceiveCallback^ get() { return callback; }
				}

				property bool Active
				{
					bool get() { return active; }
				}

				property int NumberOfBuffers
				{
					int get() { return numberOfBuffers; }
				}

				property bool DrainSynchronously
				{
					bool get() { return drainSynchronously; }
				}

#if defined(OPTION_USE_BUFFER_MANAGER)

				property ReceiveBuffer::Controller^ BufferController
				{
					ReceiveBuffer::Controller^ get() { return buffercontroller; }
				}

#else

				property array<array<unsigned char>^>^ Buffers
				{
					array<array<unsigned char>^>^ get() { return buffers; }
				}

				property array<IntPtr>^ BufferAddresses
				{
					array<IntPtr>^ get() { return bufferAddresses; }
				}

#endif

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::ListenerInfo::Parameters::AdfBufferSize)]
				property int AdfBufferSize
				{
					int get() { return myadfbuffersize; }

					void set(int n)
					{
						if (n > 0)
						{
							socket->SetSocketOption(SocketOptionLevel::Socket, SocketOptionName::ReceiveBuffer, n);
							myadfbuffersize = n;
						}
					}
				}

				#pragma endregion

				#pragma region IListener Members

				virtual void Start();
				virtual void Stop();

				#pragma endregion

				#pragma region QS.Fx.Base.IParametrized Members

				virtual property QS::Fx::Base::IParameters^ Parameters
				{
					QS::Fx::Base::IParameters^ get() = QS::Fx::Base::IParametrized::Parameters::get
					{
						return this->parameters;
					}
				}
				
				#pragma endregion

				#pragma region Various inline functions for logging and tracing

#if defined(DEBUG_CollectBytesWaiting)
				__inline void LogWaiting(double time, int bytesWaiting)
				{
					timeseries_bytesWaiting->Add(QS::_core_e_::Data::XY(time, (double)(bytesWaiting)));
				}
#endif

#if defined(DEBUG_CollectReceiveTypes)
				enum class ReceiveType : int
				{
					Synchronous, Asynchronous
				};

				__inline void LogReceive(double time, ReceiveType receiveType)
				{
					timeseries_receiveTypes->Add(QS::_core_e_::Data::XY(time, (double)((int) receiveType)));
				}
#endif

#if defined(DEBUG_LogOverheads)
				__inline void LogOverhead(double time, double overhead)
				{
					ts_ProcessingOverheads->Add(time, overhead);
				}
#endif

				#pragma endregion

				#pragma region Diagnostics.IModule Members

				virtual property QS::_core_c_::Diagnostics2::IComponent^ DiagnosticsComponent
				{
					QS::_core_c_::Diagnostics2::IComponent^ get() = QS::_core_c_::Diagnostics2::IModule::Component::get
					{
						return diagnosticsContainer; 
					}
				}

				#pragma endregion

			private:

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::ListenerInfo::Parameters::HighPriority)]
				bool high_priority;

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::ListenerInfo::Parameters::DrainSynchronously)]
				bool drainSynchronously;			

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::ListenerInfo::Parameters::BufferSize, QS::Fx::Base::ParameterAccess::Readable)]
				int bufferSize;

				[QS::Fx::Base::ParameterAttribute(QS::_core_c_::Core::ListenerInfo::Parameters::NumberOfBuffers, QS::Fx::Base::ParameterAccess::Readable)]
				int numberOfBuffers;

#if defined(OPTION_USE_BUFFER_MANAGER)
				
				ReceiveBuffer::Controller^ buffercontroller;

#else		

				// Changed to support multiple buffers
				array<array<unsigned char>^>^ buffers;
				array<GCHandle>^ bufferHandles;
				array<IntPtr>^ bufferAddresses;

#endif

				QS::_core_x_::Base::Parameters^ parameters;
				QS::_core_c_::Diagnostics2::Container^ diagnosticsContainer;
				IListenerController^ owner;
				QS::_core_c_::Core::Address^ address;
				::Socket^ socket;
				bool active, disposed;
				QS::Fx::Network::ReceiveCallback^ callback;
				Object^ context;
				QS::_core_c_::Statistics::IStatisticsController^ statisticsController;
				int myadfbuffersize;

				#pragma region DEBUG_CollectBytesWaiting

#if defined(DEBUG_CollectBytesWaiting)
				List<QS::_core_e_::Data::XY>^ timeseries_bytesWaiting;

				[QS::_core_c_::Diagnostics::Component("Bytes Waiting")]
				[QS::_core_c_::Diagnostics2::Property("BytesWaiting")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_BytesWaiting
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						return gcnew QS::_core_e_::Data::XYSeries(timeseries_bytesWaiting->ToArray());
					}
				}
#endif

				#pragma endregion

				#pragma region DEBUG_CollectReceiveTypes

#if defined(DEBUG_CollectReceiveTypes)
				List<QS::_core_e_::Data::XY>^ timeseries_receiveTypes;

				[QS::_core_c_::Diagnostics::Component("Receive Types")]
				[QS::_core_c_::Diagnostics2::Property("ReceiveTypes")]
				property QS::_core_e_::Data::IDataSet^ TimeSeries_ReceiveTypes
				{
					QS::_core_e_::Data::IDataSet^ get()
					{
						return gcnew QS::_core_e_::Data::XYSeries(timeseries_receiveTypes->ToArray());
					}
				}
#endif

				#pragma endregion

				#pragma region DEBUG_LogOverheads

#if defined(DEBUG_LogOverheads)

				[QS::_core_c_::Diagnostics2::Property("ProcessingOverheads")]
				QS::_core_c_::Statistics::ISamples2D^ ts_ProcessingOverheads;

//				[QS::_core_c_::Diagnostics::Component("Processing Overheads")]
//				[QS::_core_c_::Diagnostics2::Property("ProcessingOverheads")]
//				property QS::_core_e_::Data::IDataSet^ TimeSeries_ProcessingOverheads
//				{
//					QS::_core_e_::Data::IDataSet^ get()
//					{
//						return gcnew QS::_core_e_::Data::XYSeries(timeseries_processingOverheads->ToArray());
//					}
//				}

#endif

				#pragma endregion
			};
		}
	}
}
