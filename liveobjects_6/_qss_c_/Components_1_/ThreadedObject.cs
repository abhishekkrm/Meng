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

using System.Threading;

namespace QS._qss_c_.Components_1_
{
	public interface IThreadedObject : IDisposable
	{
		void Start();

		bool IsCompleted
		{
			get;
		}

		WaitHandle Completed
		{
			get;
		}
	}

	public abstract class ThreadedObject : QS.Fx.Inspection.Inspectable, IThreadedObject
	{
		private const double timeoutOnShutdown = 1;

		public ThreadedObject()
		{
			mythread = new Thread(new ThreadStart(Work));
			isCompleted = false;
			completed = new ManualResetEvent(false);
		}

		private Thread mythread;
		private ManualResetEvent completed;
		private bool isCompleted;

		protected abstract void Work();

		#region IThreadedObject Members

		void IThreadedObject.Start()
		{
			lock (this)
			{
				if (mythread.ThreadState == ThreadState.Unstarted)
					mythread.Start();
				else
					throw new Exception("Already started.");
			}
		}

		[QS.Fx.Base.Inspectable]
		bool IThreadedObject.IsCompleted
		{
			get { return isCompleted; }
		}

		WaitHandle IThreadedObject.Completed
		{
			get { return completed; }
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			lock (this)
			{
				isCompleted = true;
				completed.Set();
			}

			if (!mythread.Join(TimeSpan.FromSeconds(timeoutOnShutdown)))
			{
				mythread.Abort();
                if (!mythread.Join(TimeSpan.FromSeconds(timeoutOnShutdown)))
                {
                    mythread.Abort();
                    mythread.Join(TimeSpan.FromSeconds(timeoutOnShutdown));
                    throw new Exception("Cannot dispose threaded object: Cannot abort the worker thread.");
                }
			}
		}

		#endregion
	}
}
