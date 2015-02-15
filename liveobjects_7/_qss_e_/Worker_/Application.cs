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
using System.Diagnostics;
using System.IO;
using System.Net;

namespace QS._qss_e_.Worker_
{
    public class Application : IDisposable
    {
        public Application(string executable, string working_path, 
            IPAddress local_address, IPAddress controller_address, int console_portno, int controller_portno, QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;

            if (!Directory.Exists(working_path))
                Directory.CreateDirectory(working_path);

            process = new Process();
            process.StartInfo.WorkingDirectory = working_path;
            process.StartInfo.FileName = executable;
            process.StartInfo.Arguments = typeof(Runtime_.RemoteAgent).ToString() + " -here" + " -base:" + local_address.ToString() + 
                " -logfile:" + working_path + "\\messagelog.txt" + " -sendlog:" + controller_address.ToString() + ":" + console_portno.ToString() + 
                " -rsync:" + controller_address.ToString() + ":" + controller_portno.ToString();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            logger.Log(this, "Starting process... " + process.StartInfo.Arguments);

            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(this.ExitingCallback);
            process.Start();
        }

        private QS.Fx.Logging.ILogger logger;
        private Process process;

        #region ExitingCallback

        private void ExitingCallback(object sender, EventArgs e)
        {
            logger.Log(this, "Process exited.");
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            logger.Log(this, "Disposing.........");            
        }

        #endregion
    }
}
