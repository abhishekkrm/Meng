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
	/// <summary>
	/// Summary description for WorkerWithTimeout.
	/// </summary>
	public abstract class WorkerWithTimeout : IDisposable
	{
		public WorkerWithTimeout(QS.Fx.Logging.ILogger logger, TimeSpan completionTimeout)
		{
			this.logger = logger;
			this.completed = new AutoResetEvent(false);
			this.completionDeadline = DateTime.Now + completionTimeout;

			thread = new Thread(new ThreadStart(this.mainloop));
		}

		protected void start()
		{
			thread.Start();
		}

		private DateTime completionDeadline;
		private QS.Fx.Logging.ILogger logger;
		private AutoResetEvent completed;
		private Thread thread;

		protected abstract void work();

		private void mainloop()
		{
			try
			{
				this.work();
			}
			catch (Exception exc)
			{
				logger.Log(this, exc.ToString());
			}

			completed.Set();
		}

		#region IDisposable Members

		public void Dispose()
		{
			DateTime currentTime = DateTime.Now;
			TimeSpan timeLeft = (currentTime < completionDeadline) ? (completionDeadline - currentTime) : TimeSpan.Zero;
			if (!completed.WaitOne(timeLeft, false))
				logger.Log(null, "AsynchronousWorker Aborted : Completion Timeout Expired");
			thread.Join(100);
			if (thread.IsAlive)
				thread.Abort();
		}

		#endregion
	}
}
