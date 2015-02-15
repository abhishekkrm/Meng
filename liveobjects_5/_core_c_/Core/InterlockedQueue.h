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
using namespace System::Collections::Generic;
using namespace System::Threading;

namespace QS
{
	namespace _core_c_
	{
		namespace Core
		{
			generic<typename C> where C : ref class, ISLLNode<C> public ref class InterlockedQueue sealed : public IQueue<C>
			{
			public:

				#pragma region Constructor and destructor

				InterlockedQueue()
				{
					throw gcnew NotImplementedException();
				}

				~InterlockedQueue()
				{
				}

				#pragma endregion

				#pragma region IQueue<C> Members

				virtual void Enqueue(C item) = IQueue<C>::Enqueue
				{
/*
					do
					{
						item->Next = root;
					}
					while (!ReferenceEquals(Interlocked::CompareExchange(root, item, item->Next), item->Next)); 
					Interlocked::Increment(count);					
*/
				}

				virtual C Dequeue() = IQueue<C>::Dequeue
				{
/*
					
					Interlocked::Decrement(count);
*/
					return C();
				}
        
				virtual property int Count
				{
					int get() = IQueue<C>::Count::get
					{
						return count;
					}
				}

				virtual property bool IsEmpty
				{
					bool get() = IQueue<C>::IsEmpty::get
					{
						return !count;
					}
				}

				#pragma endregion

			private:

				C root;
				int count;
			};
		}
	}
}
