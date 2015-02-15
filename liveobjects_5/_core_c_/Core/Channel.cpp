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
#include "Channel.h"
#include "Overlapped.h"

using namespace System;
using namespace System::ComponentModel;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			#pragma region Constructor

			Channel::Channel(ICore^ core, String^ id, String ^name, __int32 reference, IntPtr incomingpipe, IntPtr outgoingpipe,
				IntPtr incomingmemory, IntPtr outgoingmemory, IntPtr incomingbuffer, IntPtr outgoingbuffer, IChannel^ channelclient,
				bool isincomingpipeconnected, bool isoutgoingpipeconnected)
			{
				this->core = core;
				this->id = id;
				this->name = name;
				this->channelclient = channelclient;
				this->reference = reference;
				this->incomingpipe = incomingpipe;
				this->outgoingpipe = outgoingpipe;
				this->incomingmemory = incomingmemory;
				this->outgoingmemory = outgoingmemory;
				this->incomingbuffer = (Buffer *) incomingbuffer.ToPointer();
				this->outgoingbuffer = (Buffer *) outgoingbuffer.ToPointer();
				this->isincomingpipeconnected = isincomingpipeconnected;
				this->isoutgoingpipeconnected = isoutgoingpipeconnected;
				this->incomingcontrol = new char[1];
				this->outgoingcontrol = new char[1];
				this->incomingoverlapped = new MyOverlapped(MyOverlapped::PipeRead, this->reference, 1);
				this->outgoingoverlapped = new MyOverlapped(MyOverlapped::PipeWrite, this->reference, 2);
				this->outgoingobjects = gcnew System::Collections::Generic::Queue<QS::_core_c_::Core::ChannelObject>();
				this->isincomingpipereceiving = false;
				this->isoutgoingpipesending = false;
				this->registered = 0;
			}

			#pragma endregion

			#pragma region Destructor

			Channel::~Channel()
			{
				if (!CloseHandle((HANDLE) this->incomingpipe))
					throw gcnew Exception("Could not close the incoming pipe.");
				if (!CloseHandle((HANDLE) this->outgoingpipe))
					throw gcnew Exception("Could not close the outgoing pipe.");
				delete this->incomingoverlapped;
				delete this->outgoingoverlapped;
				delete this->outgoingcontrol;
				delete this->incomingcontrol;
				if (!UnmapViewOfFile((void *) this->incomingbuffer))
					throw gcnew Exception("Cannot unmape the incoming buffer.");
				if (!UnmapViewOfFile((void *) this->outgoingbuffer))
					throw gcnew Exception("Cannot unmape the outgoing buffer.");
				if (!CloseHandle((HANDLE) this->incomingmemory))
					throw gcnew Exception("Could not close handle to the incoming buffer.");
				if (!CloseHandle((HANDLE) this->outgoingmemory))
					throw gcnew Exception("Could not close handle to the outgoing buffer.");
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Incoming

			void Channel::Incoming()
			{
				this->isincomingpipeconnected = true;
				this->Incoming1();
			}

			#pragma endregion

			#pragma region Outgoing

			void Channel::Outgoing()
			{
				this->isoutgoingpipeconnected = true;
				this->isoutgoingpipesending = false;
				this->Outgoing3();
				this->Incoming3();
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region IChannel Members

			void Channel::Handle(ChannelObject message)
			{
				this->core->Schedule(
					gcnew QS::Fx::Base::Event<ChannelObject>(
						gcnew QS::Fx::Base::ContextCallback<ChannelObject>(this, &QS::_core_c_::Core::Channel::Outgoing1), 
						message));
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Outgoing1

			void Channel::Outgoing1(ChannelObject message)
			{
				this->outgoingobjects->Enqueue(message);				
				if (!this->issending)
				{
					this->issending = true;
					this->Outgoing2();
				}
			}

			#pragma endregion

			#pragma region Outgoing2

			void Channel::Outgoing2()
			{
				while (true)
				{
					if (this->outgoingobjects->Count > 0)
					{
						ChannelObject message = this->outgoingobjects->Peek();
						if ((message.datalength + sizeof(Header)) > this->outgoingbuffer->free())
						{
							this->outgoingbuffer->cannotwrite();
							break;
						}
						else
						{
							Header header(message.operation, message.channel, message.connection, message.sequenceno, message.datalength);
							this->outgoingbuffer->write((char *) &header, sizeof(Header));
							for each (QS::Fx::Base::Block block in message.datablocks)
							{
								switch (block.type)
								{
								case QS::Fx::Base::Block::Type::Managed:
									{
										pin_ptr<cli::array<unsigned char>^> pblock = &block.buffer;							
										this->outgoingbuffer->write(((char *) pblock) + block.offset, block.size);
									}
									break;

								case QS::Fx::Base::Block::Type::Unmanaged:
								case QS::Fx::Base::Block::Type::Pinned:
									{
										this->outgoingbuffer->write(((char *) block.address.ToPointer()) + block.offset, block.size);
									}
									break;

								case QS::Fx::Base::Block::Type::None:
								default:
									throw gcnew NotImplementedException();
								}
							}
							this->outgoingobjects->Dequeue();
							if (this->outgoingobjects->Count == 0)
							{
								this->issending = false;
								break;
							}
						}
					}
					else
						this->issending = false;
				}
				this->Outgoing3();
			}

			#pragma endregion

			#pragma region Outgoing3

			void Channel::Outgoing3()
			{
				if (!this->outgoingbuffer->isempty() && !this->outgoingbuffer->reading() && this->isoutgoingpipeconnected && !this->isoutgoingpipesending)
				{
					ZeroMemory(&this->outgoingoverlapped->overlapped, sizeof(OVERLAPPED));	
					this->outgoingcontrol[0] = 1;
					if (!WriteFile((HANDLE) this->outgoingpipe, this->outgoingcontrol, 1, NULL, &this->outgoingoverlapped->overlapped))
					{
						int errorno = GetLastError();
						if (errorno != ERROR_IO_PENDING)
							throw gcnew Exception("Cannot initiate writing to the outgoing pipe (" + errorno.ToString() + ").");
						this->isoutgoingpipesending = true;
					}
				}
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

			#pragma region Incoming1

			void Channel::Incoming1()
			{
				do
				{
					if (this->isincomingpipereceiving)
					{
						if (!this->incomingcontrol[0])
							throw gcnew Exception("Unknown operation.");
						this->Incoming2();
					}
					else
						this->isincomingpipereceiving = true;
					ZeroMemory(&this->incomingoverlapped->overlapped, sizeof(OVERLAPPED));
					this->incomingcontrol[0] = 0;
				}
				while (ReadFile((HANDLE) this->incomingpipe, 
					this->incomingcontrol, 1, NULL, &this->incomingoverlapped->overlapped));
				int errorno = GetLastError();
				if (errorno != ERROR_IO_PENDING)
					throw gcnew Exception("Cannot initiate reading from the incoming pipe (" + errorno.ToString() + ").");
			}

			#pragma endregion

			#pragma region Incoming2

			void Channel::Incoming2()
			{
				while (true)
				{
					long written = this->incomingbuffer->written();
					if (written < this->registered + sizeof(Header))
					{
						this->incomingbuffer->cannotread();
						break;
					}
					else
					{
						Header header;
						this->incomingbuffer->read(this->registered, (char *) &header, sizeof(Header));
						this->registered += sizeof(Header);
						
						// ....................
					}
				}
				this->Incoming3();
			}

			#pragma endregion

			#pragma region Incoming3

			void Channel::Incoming3()
			{
				if (this->incomingbuffer->isempty() && !this->incomingbuffer->writing() && this->isoutgoingpipeconnected && !this->isoutgoingpipesending)
				{
					ZeroMemory(&this->outgoingoverlapped->overlapped, sizeof(OVERLAPPED));	
					this->outgoingcontrol[0] = 1;
					if (!WriteFile((HANDLE) this->outgoingpipe, this->outgoingcontrol, 1, NULL, &this->outgoingoverlapped->overlapped))
					{
						int errorno = GetLastError();
						if (errorno != ERROR_IO_PENDING)
							throw gcnew Exception("Cannot initiate writing to the outgoing pipe (" + errorno.ToString() + ").");
						this->isoutgoingpipesending = true;
					}
				}
			}

			#pragma endregion

			// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		}
	}
}
