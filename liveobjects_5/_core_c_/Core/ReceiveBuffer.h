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
			public ref class ReceiveBuffer sealed : public QS::Fx::Base::IBlockControl
			{
			public:

				ref class Controller sealed 
				{
				public:

					Controller(unsigned __int32 id, int buffersize);
					~Controller();

					ReceiveBuffer^ Allocate();
					void Release(ReceiveBuffer^ buffer);

					ReceiveBuffer^ Lookup(__int32 id);

				private:

					unsigned __int32 id;
					int buffersize, nused, nfree, lastid;
					ReceiveBuffer ^head, ^tail;
					IDictionary<__int32, ReceiveBuffer^>^ buffers;
				};

				property __int32 ID
				{
					__int32 get() { return id; }
				}

				property array<unsigned char>^ Buffer
				{
					array<unsigned char>^ get() { return buffer; }
				}

				property IntPtr Address
				{
					IntPtr get() { return address; }
				}

				property __int32 NumReferences
				{
					__int32 get() { return nreferences; }
					void set(__int32 nreferences) { this->nreferences = nreferences; }
				}

/*
				void AddReference();
				void Release();
*/

				virtual void AddReference() = QS::Fx::Base::IBlockControl::AddReference
				{
					nreferences++;
				}

				virtual void Release() = QS::Fx::Base::IBlockControl::Release
				{
					nreferences--;
				}

				virtual property unsigned __int64 Identifier 
				{
					unsigned __int64 get() = QS::Fx::Base::IBlockControl::Identifier::get
					{
						unsigned __int64 longid;
						((__int32 *) &longid)[0] = this->ownerid;
						((__int32 *) &longid)[1] = this->id;
						return longid;
					}
				}

			private:

				ReceiveBuffer(Controller^ controller, __int32 ownerid, __int32 id, int size, int nreferences);
				~ReceiveBuffer();

				Controller^ controller;
				int size, nreferences;
				__int32 ownerid, id;
				array<unsigned char>^ buffer;
				GCHandle handle;
				IntPtr address;
				ReceiveBuffer^ next;
			};
		}
	}
}
