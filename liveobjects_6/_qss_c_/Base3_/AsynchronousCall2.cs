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
	public class AsynchronousCall2<C> : AsynchronousCall1<C>
	{
//		public AsynchronousCall2(System.Delegate processingCallback, object[] arguments,
//			AsyncCallback completionCallback, object state) 
//			: this(processingCallback, arguments, completionCallback, state, false)
//		{
//		}

		public AsynchronousCall2(System.Delegate processingCallback, object[] arguments, 
			AsyncCallback completionCallback, object state, bool scheduleAutomatically) : base(completionCallback, state)
		{
			this.processingCallback = processingCallback;
			this.arguments = arguments;

			if (!typeof(C).IsAssignableFrom(processingCallback.Method.ReturnType))
				throw new ArgumentException("Return type of the delegate provided is " +
					processingCallback.Method.ReturnType.Name + ", but it should be " + typeof(C).Name + ".");

			if (scheduleAutomatically)
				System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(internalCallback));
		}

		private System.Delegate processingCallback;
		private object[] arguments;

		public void Invoke()
		{
			C operation_result;
			System.Exception thrown_exception = null;

			try
			{
				operation_result = (C)processingCallback.DynamicInvoke(arguments);
			}
			catch (Exception exc)
			{
				operation_result = default(C);
				thrown_exception = exc;
			}

			Completed(operation_result, thrown_exception);
		}

		#region Internal Processing

		private void internalCallback(object o)
		{
			Invoke();
		}

		#endregion
	}
}
