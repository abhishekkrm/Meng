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

namespace QS._core_c_.Base
{
	/// <summary>
	/// Aaa...
	/// </summary>
	[System.Serializable] 
    [QS.Fx.Base.Inspectable]
	public class Logger : QS.Fx.Inspection.Inspectable, IReadableLogger, QS.Fx.Diagnostics.IDiagnosticsComponent
	{
        public static QS.Fx.Clock.IClock DefaultClock
        {
            get { return defaultClock; }
            set { defaultClock = value; }
        }

        private static QS.Fx.Clock.IClock defaultClock;

        public static Logger StandardConsole
        {
            get { return new Logger(QS._core_c_.Base2.PreciseClock.Clock, false, new ConsoleWrapper()); }
        }

		private const TimestampingMode default_timestampingMode = TimestampingMode.CPUTIME;

		public enum TimestampingMode
		{
			NONE, REALTIME, CPUTIME
		}

		#region IConsole Members

        public void Log(string s)
		{
			this.Log(null, s);
		}

		#endregion

        public Logger(QS.Fx.Clock.IClock clock) : this(clock, true)
        {
        }

        public Logger(QS.Fx.Clock.IClock clock, bool buffering)
            : this(clock, buffering, null, true, null)
		{
		}

        public Logger(QS.Fx.Clock.IClock clock, bool buffering, QS.Fx.Logging.IConsole console)
            : this(clock, buffering, console, true, null)
		{
		}

        public Logger(QS.Fx.Clock.IClock clock, bool buffering, QS.Fx.Logging.IConsole console, bool timestamping, string prefixString)
			: this(clock, buffering, console, timestamping ? default_timestampingMode : TimestampingMode.NONE, prefixString)
		{
		}

        public Logger(QS.Fx.Clock.IClock clock, bool buffering, QS.Fx.Logging.IConsole console, TimestampingMode timestampingMode, string prefixString)
		{
			this.buffering = buffering;
			this.console = console;
			this.timestampingMode = timestampingMode;
			this.prefixString = prefixString;
            this.clock = clock;

			if (buffering)
			{
				// completeLogContents = "";
				logContents = new System.Collections.ArrayList(100);
			}
		}

        private QS.Fx.Clock.IClock clock;
        private bool enabled = true;
        private QS.Fx.Logging.IConsole console = null;		
		private bool buffering;
		private System.Collections.ArrayList logContents;
		private TimestampingMode timestampingMode;
		private string prefixString;

		private bool synchronized = true;
		private bool numbering = false;
		private int lastused_seqno = 0;

		// private string completeLogContents = null;

        public bool Buffering
        {
            get { return buffering; }
            set { buffering = value;  }
        }

		public TimestampingMode TimestampingModeOf
		{
            get { return timestampingMode; }
			set { timestampingMode = value; }
		}

        public string PrefixString
        {
            get { return prefixString; }
            set { prefixString = value; }
        }

        public string[] Messages
        {
            get { return (string[]) logContents.ToArray(typeof(string)); }
            set 
            {
                foreach (string msg in value)
                    logContents.Add(msg);
            }
        }

		#region ILogger Members

		public void Clear()
		{
			if (buffering)
			{
				lock (this)
				{
					logContents.Clear();
				}
			}
		}

		private System.Object mylock = new Object();
        public void Log(object source, string message)
		{
            if (enabled)
            {
                int seqno = 0;
                if (synchronized)
                {
                    System.Threading.Monitor.Enter(mylock);
                    if (numbering)
                        seqno = ++lastused_seqno;
                }

                try
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    if (prefixString != null)
                        sb.Append(prefixString);

                    if (synchronized && numbering)
                    {
                        sb.Append("<");
                        sb.Append(seqno.ToString());
                        sb.Append("> ");
                    }

                    switch (timestampingMode)
                    {
                        case TimestampingMode.REALTIME:
                            {
                                sb.Append("[");
                                sb.Append(DateTime.Now.ToString());
                                sb.Append("] ");
                            }
                            break;

                        case TimestampingMode.CPUTIME:
                            {
                                QS.Fx.Clock.IClock useclock = (clock != null ? clock : defaultClock);
                                if (useclock != null)
                                {
                                    sb.Append("[");
                                    sb.Append(useclock.Time.ToString("000000.000000000"));
                                    sb.Append("] ");
                                }
                            }
                            break;

                        // case TimestampingMode.NONE:
                        default:
                            break;
                    }

                    if (source != null)
                    {
                        sb.Append(source.GetType().Name);
                        sb.Append(" : ");
                    }

                    sb.Append(message);

                    string line = sb.ToString();

                    if (buffering)
                    {
                        lock (logContents)
                        {
                            logContents.Add(line + "\n");
                        }
                    }

                    if (console != null)
                    {
                        try
                        {
                            console.Log(line);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                finally
                {
                    if (synchronized)
                        System.Threading.Monitor.Exit(mylock);
                }
            }
		}

		#endregion

		#region IOutputReader Members

        [System.Xml.Serialization.XmlIgnore]
        public QS.Fx.Logging.IConsole Console
		{
			get
			{
				return console;
			}

			set
			{
				lock (this)
				{
					console = value;
					if (console != null)
					{
						console.Log(this.CurrentContents);
					}
				}
			}
		}

        [System.Xml.Serialization.XmlIgnore]
        public uint NumberOfMessages
        {
			get
			{
				if (!buffering)
					throw new Exception("this logger is not buffering messages");

				uint result = 0;
				lock (logContents)
				{
					result = (uint) logContents.Count;
				}
				return result;
			}
		}

        // [System.Xml.Serialization.XmlIgnore]
        public string this[uint indexOfAMessage]
        {
			get
			{
				if (!buffering)
					throw new Exception("this logger is not buffering messages");

				string result = null;
				lock (logContents)
				{
					result = (string) logContents[(int) indexOfAMessage];
				}

				return result;
			}
		}

		public string rangeAsString(uint indexOfTheFirstMessage, uint numberOfMessages)
		{
			string[] rangeArray = this.rangeAsArray(indexOfTheFirstMessage, numberOfMessages);
			string result = "";
			for (uint ind = 0; ind < rangeArray.Length; ind++)
				result += rangeArray[ind];
			return result;
		}

		public string[] rangeAsArray(uint indexOfTheFirstMessage, uint numberOfMessages)
		{
			if (!buffering)
				throw new Exception("this logger is not buffering messages");

			string[] result = null;
			lock (logContents)
			{
				uint numberAvailable = ((uint) logContents.Count) - indexOfTheFirstMessage;
				if (numberOfMessages > numberAvailable)
					numberOfMessages = numberAvailable;

				result = new string[numberOfMessages];

                for (int ind = 0; ind < numberOfMessages; ind++)
                    result[ind] = (string) logContents[((int) indexOfTheFirstMessage) + ind];

//				uint index = 0;
//				for (System.Collections.IEnumerator en = logContents.GetEnumerator(); 
//					en.MoveNext() && (index < numberOfMessages); result[index++] = (string) en.Current)
//					;
			}	
		
			return result;
		}

		[QS.Fx.Base.Inspectable("Current Contents")]
        [System.Xml.Serialization.XmlIgnore]
        public string CurrentContents
        {
			get
			{
				if (!buffering)
					throw new Exception("this logger is not buffering messages");

				string currentContents = "";
				lock (logContents)
				{
					for (System.Collections.IEnumerator en = logContents.GetEnumerator(); en.MoveNext();
						currentContents = currentContents + ((string) en.Current))
						;
				}
				return currentContents;
			}
		}

		#endregion

/*
		#region IScalarAttribute Members

		object QS.TMS.Inspection.IScalarAttribute.Value
		{
			get { return this.CurrentContents; }
		}

		#endregion

		#region IAttribute Members

		string QS.TMS.Inspection.IAttribute.Name
		{
			get { return "Current Log Contents"; }
		}

		QS.TMS.Inspection.AttributeClass QS.TMS.Inspection.IAttribute.AttributeClass
		{
			get { return QS.TMS.Inspection.AttributeClass.SCALAR; }
		}

		#endregion
*/

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.Logger; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            Clear();
        }

        #endregion
    }
}
