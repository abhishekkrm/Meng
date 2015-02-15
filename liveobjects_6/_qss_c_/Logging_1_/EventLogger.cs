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

namespace QS._qss_c_.Logging_1_
{
    public class EventLogger : QS.Fx.Logging.IEventLogger, IEventSource, QS.Fx.Diagnostics.IDiagnosticsComponent
/*
        , Base.IReadableLogger
*/ 
    {
        public EventLogger(QS.Fx.Clock.IClock clock, bool enabled)
        {
            this.clock = clock;
            this.enabled = enabled;
        }

        private QS.Fx.Clock.IClock clock;
        private bool enabled;
        private IList<QS.Fx.Logging.IEvent> events = new List<QS.Fx.Logging.IEvent>();
        private QS.Fx.Logging.IEventLogger chainedLogger;
/*
        private Base.IConsole console;
*/ 

        #region IEventLogger Members

        void QS.Fx.Logging.IEventLogger.Log(QS.Fx.Logging.IEvent eventToLog)
        {
            if (enabled)
            {
                lock (events)
                {
                    events.Add(eventToLog);
                    if (chainedLogger != null)
                        chainedLogger.Log(eventToLog);

/*
                    if (console != null)
                        console.writeLine(ToString(eventToLog));
*/ 
                }
            }
        }

        #endregion

        #region IDiagnosticsComponent Members

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
            lock (this)
            {
                events.Clear();
                events.Add(new Events.SimpleEvent1(clock.Time, null, this, "Cleared"));
            }            
        }

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.EventLogger; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return enabled; }
            set 
            {
                lock (this)
                {
                    enabled = value;                    
                    events.Add(new Events.SimpleEvent1(clock.Time, null, this, enabled ? "Enabled" : "Disabled"));
                }
            }
        }

        #endregion

        #region IEventSource Members

        QS.Fx.Logging.IEventLogger IEventSource.Logger
        {
            get { return chainedLogger; }
            set 
            {
                lock (this)
                {
                    if (value != chainedLogger)
                    {
                        chainedLogger = value;
                        if (chainedLogger != null)
                            foreach (QS.Fx.Logging.IEvent e in events)
                                chainedLogger.Log(e);
                    }
                }
            }
        }

        #endregion

/*
        #region Converting Event to String

        private static string ToString(IEvent e)
        {
            return e.ToString();
        }

        #endregion
 
        #region IOutputReader Members

        string QS.CMS.Base.IOutputReader.CurrentContents
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        uint QS.CMS.Base.IOutputReader.NumberOfMessages
        {
            get { return (uint) events.Count; }
        }

        string QS.CMS.Base.IOutputReader.this[uint indexOfAMessage]
        {
            get { return ToString(events[(int) indexOfAMessage]); }
        }

        string[] QS.CMS.Base.IOutputReader.rangeAsArray(uint indexOfTheFirstMessage, uint numberOfMessages)
        {
            string[] result = new string[numberOfMessages];
            for (int ind = 0; ind < numberOfMessages; ind++)
                result[ind] = ToString(events[(int) indexOfTheFirstMessage + ind]);
            return result;
        }

        string QS.CMS.Base.IOutputReader.rangeAsString(uint indexOfTheFirstMessage, uint numberOfMessages)
        {
            StringBuilder s = new StringBuilder();
            for (int ind = 0; ind < numberOfMessages; ind++)
                s.AppendLine(ToString(events[(int)indexOfTheFirstMessage + ind]));
            return s.ToString();
        }

        QS.Fx.Logging.IConsole QS.CMS.Base.IOutputReader.Console
        {
            get { return console; }
            set
            {
                lock (this)
                {
                    if (value != console)
                    {
                        console = value;
                        if (console != null)
                            foreach (IEvent e in events)
                                console.writeLine(ToString(e));
                    }
                }
            }
        }

        #endregion

        #region ILogger Members

        void QS.Fx.Logging.ILogger.clear()
        {
            ((QS.Fx.Diagnostics.IDiagnosticsComponent)this).ResetComponent();
        }

        void QS.Fx.Logging.ILogger.Log(object source, string message)
        {
            ((IEventLogger)this).Log(new Events.SimpleEvent(clock.Time, source, message));
        }

        #endregion

        #region IConsole Members

        void QS.Fx.Logging.IConsole.writeLine(string s)
        {
            ((QS.Fx.Logging.ILogger)this).logMessage(null, s);
        }

        #endregion
*/
    }
}
