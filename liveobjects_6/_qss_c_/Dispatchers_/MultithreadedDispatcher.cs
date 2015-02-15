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

namespace QS._qss_c_.Dispatchers_
{
	/// <summary>
	/// Aaa...
	/// </summary>
	public class MultithreadedDispatcher : Base1_.IDispatcher
	{
		public MultithreadedDispatcher(Base1_.OnReceive callback)
		{
			this.callback = callback;
		}

		private Base1_.OnReceive callback;

		private class Task
		{
			public Task(Base1_.IAddress sourceAddress, QS._core_c_.Base.IMessage message)
			{
				this.sourceAddress = sourceAddress;
				this.message = message;
			}

			public Base1_.IAddress sourceAddress;
			public QS._core_c_.Base.IMessage message;
		}

		private void invokeCallback(object taskObject)
		{
			Task task = (Task) taskObject;
			callback(task.sourceAddress, task.message);
		}

		#region IDispatcher Members

		public void dispatch(QS._qss_c_.Base1_.IAddress sourceAddress, QS._core_c_.Base.IMessage message)
		{
			ThreadPool.QueueUserWorkItem(
				new WaitCallback(invokeCallback), new Task(sourceAddress, message));
		}

		#endregion
	}
}
