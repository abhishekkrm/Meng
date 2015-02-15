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
	namespace _core_x_
	{
		namespace Uplink
		{
			public class Buffer sealed
			{
			public:

				__inline void initialize(long capacity, HANDLE control)
				{
					m_capacity = capacity;
					m_control = control;
				}
				
				__inline HANDLE control()
				{
					return m_control;
				}

				__inline long free()
				{
					return m_capacity + m_nread - m_nwritten;
				}

				__inline void copy(char *bytes, long count)
				{
					long offset = m_nwritten % m_capacity;
					long n = m_capacity - offset;
					if (count > n)
					{
						CopyMemory(m_data + offset, bytes, n);
						CopyMemory(m_data, bytes + n, count - n);
					}
					else
						CopyMemory(m_data + offset, bytes, count);
					do
						n = m_nwritten;
					while (InterlockedCompareExchange(&m_nwritten, n + count, n) != n);
				}

				__inline bool reading()
				{
					return (InterlockedExchange(&m_reading, 1) != 0);
				}

			private:

				long m_capacity;
				HANDLE m_control;
				volatile long m_nwritten;
				volatile long m_nread;
				volatile long m_reading;

#pragma warning(disable:4200)
				char m_data[0];
#pragma warning(default:4200)
			};
		}
	}
}
