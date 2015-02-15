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
    public class Console : QS.Fx.Logging.IEventLogger, QS.Fx.Diagnostics.IDiagnosticsComponent
    {
        public static Console MyConsole
        {
            get 
            {
                lock (typeof(Console))
                {
                    if (myConsole == null)
                        myConsole = new Console(
                            new QS._core_c_.Base.ConsoleWrapper(new QS._core_c_.Base.WriteLineCallback(System.Console.WriteLine)));
                }
                return myConsole; 
            }
        }

        private static Console myConsole;

        public Console(QS.Fx.Logging.IConsole console)
        {
            this.console = console;
        }

        private QS.Fx.Logging.IConsole console;

        #region QS.Fx.Logging.IEventLogger Members

        void QS.Fx.Logging.IEventLogger.Log(QS.Fx.Logging.IEvent eventToLog)
        {
            console.Log(QS.Fx.Printing.Printable.ToString(eventToLog));
        }

        #endregion

        #region IDiagnosticsComponent Members

        QS.Fx.Diagnostics.ComponentClass QS.Fx.Diagnostics.IDiagnosticsComponent.Class
        {
            get { return QS.Fx.Diagnostics.ComponentClass.EventLogger; }
        }

        bool QS.Fx.Diagnostics.IDiagnosticsComponent.Enabled
        {
            get { return true; }
            set
            {
                if (!value)
                    throw new Exception("This component is always enabled by default.");
            }
        }

        void QS.Fx.Diagnostics.IDiagnosticsComponent.ResetComponent()
        {
        }

        #endregion
    }
}
