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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Base3_
{
	public class AsynchronousRequest2<C> : IAsyncResult, IAsynchronousRequest<C>
	{
		public AsynchronousRequest2(C argument)
		{
			this.argument = argument;
		}

		private C argument;
		private bool succeeded, completed = false;
		private System.Exception exception = null;
		private System.Threading.ManualResetEvent completionEvent = null;

		#region IAsynchronousRequest<C> Members

		public C Argument
		{
			get { return argument; }
		}

		private void SetCompletion(bool succeeded, Exception exception)
		{
			lock (this)
			{
				completed = true;
				this.succeeded = succeeded;
				this.exception = exception;

				if (completionEvent != null)
					completionEvent.Set();
			}

			CompletionCallback();
		}

		protected virtual void CompletionCallback()
		{
		}

		void IAsynchronousRequest<C>.Completed()
		{
			SetCompletion(true, null);
		}

		void IAsynchronousRequest<C>.Failed(Exception exception)
		{
			SetCompletion(false, exception);
		}

		#endregion

		#region IAsyncResult Members

		public virtual object AsyncState
		{
			get { return null; }
		}

		System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get
			{
				lock (this)
				{
					if (completionEvent == null)
						completionEvent = new System.Threading.ManualResetEvent(completed);
					return completionEvent;
				}
			}
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get { return false; }
		}

		bool IAsyncResult.IsCompleted
		{
			get { return completed; }
		}

		#endregion
	}
}
