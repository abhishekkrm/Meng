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
    [QS.Fx.Base.Inspectable]
	public class Logger : QS.Fx.Inspection.Inspectable, QS._core_c_.Base.IReadableLogger
	{
		public Logger(QS.Fx.Clock.IClock clock) : this(clock, true, null)
		{
		}

        public Logger(QS.Fx.Clock.IClock clock, bool buffering, QS.Fx.Logging.IConsole underlyingConsole)
		{
			this.clock = clock;
			if (buffering)
				logentries = new List<LogEntry>();
			this.underlyingConsole = underlyingConsole;
		}

		private QS.Fx.Clock.IClock clock;
        private QS.Fx.Logging.IConsole underlyingConsole;
		private System.Collections.Generic.List<LogEntry> logentries;

        public QS.Fx.Clock.IClock Clock
        {
            get { return clock; }
            set { clock = value; }
        }

		#region Struct LogEntry

		private struct LogEntry
		{
			public LogEntry(object source, string message, double timestamp)
			{
				this.source = source;
				this.message = message;
				this.timestamp = timestamp;
			}

			private object source;
			private string message;
			private double timestamp;

			public void AppendTo(StringBuilder s)
			{
				s.Append("[");
				s.Append(timestamp.ToString("000000.000000000"));
				s.Append("] ");
				if (source != null)
				{
					s.Append(source.GetType().Name);
					s.Append(" : ");
				}
				s.Append(message);
			}

			public override string ToString()
			{
				StringBuilder s = new StringBuilder();
				AppendTo(s);
				return s.ToString();
			}
		}

		#endregion

		#region ILogger Members

		public void Clear()
		{
			lock (this)
			{
				if (logentries != null)
					logentries.Clear();
				else
					throw new Exception("Cannot clear: not buffering.");
			}
		}

		public void Log(object source, string message)
		{
			LogEntry logentry = new LogEntry(source, message, (clock != null) ? clock.Time : 0);

			lock (this)
			{
				if (logentries != null)
					logentries.Add(logentry);

				if (underlyingConsole != null)
					underlyingConsole.Log(logentry.ToString());
			}
		}

		#endregion

		#region IConsole Members

/*
        void QS.Fx.Logging.IConsole.write(string s)
        {
            ((QS.Fx.Logging.ILogger)this).logMessage(null, s); // for now
        }
*/

		public void Log(string s)
		{
			((QS.Fx.Logging.ILogger) this).Log(null, s);			
		}

		#endregion

		#region IOutputReader Members

		// string QS.CMS.Base.IOutputReader.CurrentContents
		[QS.Fx.Base.Inspectable]
		public string CurrentContents
		{
			get 
			{
				lock (this)
				{
					StringBuilder s = new StringBuilder();
					foreach (LogEntry logentry in logentries)
					{
						logentry.AppendTo(s);
						s.Append("\n");
					}
					return s.ToString();
				}
			}
		}

		uint QS._core_c_.Base.IOutputReader.NumberOfMessages
		{
            get 
            {
                lock (this)
                {
                    return (uint)logentries.Count;
                }
            }
		}

		string QS._core_c_.Base.IOutputReader.this[uint indexOfAMessage]
		{
			get 
            {
                lock (this)
                {
                    StringBuilder ss = new StringBuilder();
                    logentries[(int)indexOfAMessage].AppendTo(ss);
                    return ss.ToString();
                }
            }
		}

		string[] QS._core_c_.Base.IOutputReader.rangeAsArray(uint indexOfTheFirstMessage, uint numberOfMessages)
		{
            lock (this)
            {
                string[] result = new string[numberOfMessages];
                for (int i = 0; i < numberOfMessages; i++)
                {
                    StringBuilder ss = new StringBuilder();
                    logentries[((int)indexOfTheFirstMessage) + i].AppendTo(ss);
                    result[i] = ss.ToString();
                }
                return result;
            }
		}

		string QS._core_c_.Base.IOutputReader.rangeAsString(uint indexOfTheFirstMessage, uint numberOfMessages)
		{
            lock (this)
            {
                StringBuilder ss = new StringBuilder();
                for (int i = 0; i < numberOfMessages; i++)
                    logentries[((int)indexOfTheFirstMessage) + i].AppendTo(ss);
                return ss.ToString();
            }
		}

		QS.Fx.Logging.IConsole QS._core_c_.Base.IOutputReader.Console
		{
			get { return underlyingConsole; }
			set
			{
				lock (this)
				{
					if (value != underlyingConsole)
					{
						underlyingConsole = value;

                        if (underlyingConsole != null)
                        {
                            foreach (LogEntry logentry in logentries)
                            {
                                StringBuilder s = new StringBuilder();
                                logentry.AppendTo(s);
                                underlyingConsole.Log(s.ToString());
                            }
                        }
					}
				}
			}
		}

		#endregion
	}
}
