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

using System;
using System.Threading;

namespace QS._qss_c_.Components_1_
{
	public interface IObjectPool
	{
		IObjectWrapper AllocateObject
		{
			get;
		}
	}

	public class ObjectPool : IObjectPool
	{
		public ObjectPool(uint initialSize, AllocateCallback allocateCallback)
		{
			this.initialSize = initialSize;
			// this.maximumSize = maximumSize;

			this.allocateCallback = allocateCallback;

			objectWrappers = new ObjectWrapper[initialSize];
			for (int ind = (int) initialSize - 1; ind >= 0; ind--)
			{
				objectWrappers[ind] = new ObjectWrapper(allocateCallback(), this);
				objectWrappers[ind].Next = (ind < (initialSize - 1)) ? objectWrappers[ind + 1] : null;
			}
			firstFreeWrapper = objectWrappers[0];
		}

		private uint initialSize; // , maximumSize;
		private AllocateCallback allocateCallback;

		private ObjectWrapper[] objectWrappers;		
		private ObjectWrapper firstFreeWrapper;
		
		#region Class ObjectWrapper

		private class ObjectWrapper : Collections_1_.GenericLinkable, IObjectWrapper
		{
			public ObjectWrapper(object wrappedObject, ObjectPool objectPool)
			{
				this.wrappedObject = wrappedObject;
				this.objectPool = objectPool;
			}

			private object wrappedObject;
			private ObjectPool objectPool;

			#region IObjectWrapper Members

			public object WrappedObject
			{
				get
				{
					return wrappedObject;
				}
			}

			public void ReleaseObject()
			{
				objectPool.release(this);
			}

			#endregion
		}

		#endregion

		private void release(IObjectWrapper objectWrapper)
		{
			lock (this)
			{
				bool shouldPulse = firstFreeWrapper == null;
				((ObjectWrapper) objectWrapper).Next = firstFreeWrapper;
				firstFreeWrapper = (ObjectWrapper) objectWrapper;

				if (shouldPulse)
					Monitor.Pulse(this);
			}
		}

		#region IObjectPool Members

		public IObjectWrapper AllocateObject
		{
			get
			{
				IObjectWrapper result = null;

				lock (this)
				{
					while (firstFreeWrapper == null)
						Monitor.Wait(this);

					result = firstFreeWrapper;
					firstFreeWrapper = (ObjectWrapper) firstFreeWrapper.Next;
				}

				return result;
			}
		}

		#endregion
	}

	public interface IObjectWrapper
	{
		object WrappedObject
		{
			get;
		}

		void ReleaseObject();
	}

	public delegate object AllocateCallback();
}
