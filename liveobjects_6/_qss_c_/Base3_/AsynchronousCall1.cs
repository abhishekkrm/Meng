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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
	public class AsynchronousCall1<C> : IAsynchronousCall<C>
	{
		public AsynchronousCall1(AsyncCallback completionCallback, object state)
		{
			this.completionCallback = completionCallback;
			this.state = state;
		}

		private AsyncCallback completionCallback;
		private object state;
		private C result;
		private System.Exception exception;
		private System.Threading.ManualResetEvent completion;
		private bool completed;

//		public void Completed(C operation_result)
//		{
//			Completed(operation_result, null);
//		}

		public void Completed(C operation_result, System.Exception thrown_exception)
		{
			lock (this)
			{
				this.result = operation_result;
				this.exception = thrown_exception;

				completed = true;
				if (completion != null)
					completion.Set();
			}

			if (completionCallback != null)
				completionCallback(this);
		}

		#region IAsynchronousCall<C> Members

		public C OperationResult
		{
			get 
			{
				lock (this)
				{
					if (!completed)
						throw new Exception("Operation has not completed yet.");
					if (exception != null)
						throw new Exception("Operation failed.", exception);
					return result;
				}
			}
		}

		#endregion

		#region IAsyncResult Members

		object IAsyncResult.AsyncState
		{
			get { return state; }
		}

		System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get 
			{
				lock (this)
				{
					if (completion == null)
						completion = new System.Threading.ManualResetEvent(completed);
					return completion;
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
