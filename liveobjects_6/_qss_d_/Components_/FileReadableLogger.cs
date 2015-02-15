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
using System.IO;

namespace QS._qss_d_.Components_
{
    public class FileReadableLogger : QS._core_c_.Base.IReadableLogger
    {
        public FileReadableLogger(string filename)
        {
            this.filename = filename;
            if (File.Exists(filename))
            {
                using (StreamReader reader = new StreamReader(filename))
                {
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                            messages.Add(line);
                    }
                }
            }
        }

        private string filename;
        private List<string> messages = new List<string>();

        private void Append(string s)
        {
            if (s != null)
            {
                messages.Add(s);
                using (StreamWriter writer = new StreamWriter(filename, true))
                {
                    writer.WriteLine(s);
                    writer.Flush();
                }
            }
        }

        #region ILogger Members

        void QS.Fx.Logging.ILogger.Clear()
        {
            lock (this)
            {
                File.Delete(filename);
                messages.Clear();
            }
        }

        void QS.Fx.Logging.ILogger.Log(object source, string message)
        {
            lock (this)
            {
                Append(((source != null) ? source.ToString() : "") + message);
            }
        }

        #endregion

        #region IConsole Members

        void QS.Fx.Logging.IConsole.Log(string s)
        {
            lock (this)
            {
                Append(s);
            }
        }

        #endregion

        #region IOutputReader Members

        string QS._core_c_.Base.IOutputReader.CurrentContents
        {
            get 
            {
                lock (this)
                {
                    StringBuilder s = new StringBuilder();
                    foreach (string m in messages)
                        s.AppendLine(m);
                    return s.ToString();
                }
            }
        }

        uint QS._core_c_.Base.IOutputReader.NumberOfMessages
        {
            get { return (uint) messages.Count; }
        }

        string QS._core_c_.Base.IOutputReader.this[uint indexOfAMessage]
        {
            get { throw new NotImplementedException(); }
        }

        string[] QS._core_c_.Base.IOutputReader.rangeAsArray(uint indexOfTheFirstMessage, uint numberOfMessages)
        {
            throw new NotImplementedException();
        }

        string QS._core_c_.Base.IOutputReader.rangeAsString(uint indexOfTheFirstMessage, uint numberOfMessages)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Logging.IConsole QS._core_c_.Base.IOutputReader.Console
        {
            get { return null; }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
