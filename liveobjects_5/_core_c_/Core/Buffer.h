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

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			public class Buffer sealed
			{
			public:

				__inline void initialize(long capacity)
				{
					m_capacity = capacity;
					m_read = 0;
					m_written = 0;
					m_writing = 1;
					m_reading = 0;
				}

				__inline long free()
				{
					return m_capacity + m_read - m_written;
				}

				__inline void write(char *bytes, long count)
				{
					long offset = m_written % m_capacity;
					long n = m_capacity - offset;
					if (count > n)
					{
						CopyMemory(m_data + offset, bytes, n);
						CopyMemory(m_data, bytes + n, count - n);
					}
					else
						CopyMemory(m_data + offset, bytes, count);
					m_written = m_written + count;
				}

				__inline bool isempty()
				{
					return m_read >= m_written;
				}

				__inline bool reading()
				{
					return InterlockedExchange(&m_reading, 1);
				}

				__inline void cannotwrite()
				{
					m_writing = 0;
				}

				__inline long written()
				{
					return m_written;
				}

				__inline bool writing()
				{
					return InterlockedExchange(&m_writing, 1);
				}

				__inline void cannotread()
				{
					m_reading = 0;
				}

				__inline void read(long offset, char *bytes, long count)
				{
					offset = offset % m_capacity;
					long n = m_capacity - offset;
					if (count > n)
					{
						CopyMemory(bytes, m_data + offset, n);
						CopyMemory(bytes + n, m_data, count - n);
					}
					else
						CopyMemory(bytes, m_data + offset, count);
				}

				__inline void read(long count)
				{
					m_read = m_read + count;
				}

			private:

				__int64 m_capacity;

				volatile long m_written;
				volatile long m_read;
				volatile long m_writing;
				volatile long m_reading;

#pragma warning(disable:4200)
				char m_data[0];
#pragma warning(default:4200)
			};
		}
	}
}
