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
using System.IO;
using System.Threading;

namespace QS._qss_d_.Base_
{
	/// <summary>
	/// Summary description for OutputReader.
	/// </summary>
	public class OutputReader
	{
		public OutputReader(StreamReader streamReader, TimeSpan outputPollingInterval)
		{
			this.outputPollingInterval = outputPollingInterval;
			this.streamReader = streamReader;
			
			// output = new System.Collections.ArrayList();
            output = new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, true, null);

			controllingThread = new Thread(new ThreadStart(mainloop));
			shuttingDown = false;
			shutdownEvent = new AutoResetEvent(false);

			controllingThread.Start();
		}

		public QS._core_c_.Base.IOutputReader Output
		{
			get
			{
				return output;
			}
		}

		private const uint maximumShutdownWaitingTimeInSeconds = 1;			
		public void shutdown()
		{
			shuttingDown = true;
			shutdownEvent.Set();

			controllingThread.Join(
				TimeSpan.FromSeconds(maximumShutdownWaitingTimeInSeconds));
			if (controllingThread.IsAlive)
				controllingThread.Abort();
		}

		public string CurrentOutput // obsolete
		{
			get
			{
				return output.CurrentContents;
//				string result = "";
//				lock (output)
//				{
//					foreach (string s in output)
//						result += s + "\n";
//				}
//				return result;
			}
		}

		void mainloop()
		{
			while (!shuttingDown)
			{
				while (true)
				{
					string str = streamReader.ReadLine();
					if (str != null)
					{
						// lock (output)
						// {
						// output.Add(str);
						output.Log(null, str);
						// }
					}
					else
						break;
				}

				shutdownEvent.WaitOne(outputPollingInterval, false);
			}	
		}

		private Thread controllingThread;
		private bool shuttingDown;
		private AutoResetEvent shutdownEvent;
		private StreamReader streamReader;
		private TimeSpan outputPollingInterval;
		// private System.Collections.ArrayList output;
		private QS._core_c_.Base.IReadableLogger output;
	}
}
