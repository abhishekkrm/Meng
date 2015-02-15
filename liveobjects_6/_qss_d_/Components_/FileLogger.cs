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
using System.IO;
using System.Text;

namespace QS._qss_d_.Components_
{
	public class FileLogger : QS.Fx.Logging.ILogger
	{
		public FileLogger() : this(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + "." + 
            DateTime.Now.ToString("yyyyMMddHHmm") + "." + System.Diagnostics.Process.GetCurrentProcess().Id.ToString() + ".errorlog")
		{
		}

		public FileLogger(string filename)
		{
			this.filename = filename;
		}

		private string filename;

		#region ILogger Members

		void QS.Fx.Logging.ILogger.Clear()
		{
			lock (this)
			{
				try
				{
					File.Delete(filename);
				}
				catch (Exception)
				{
				}
			}
		}

        void QS.Fx.Logging.ILogger.Log(object source, string message)
		{
			StringBuilder s = new StringBuilder();
			s.Append("[");
			s.Append(DateTime.Now.ToString());
			s.Append("] ");
			if (source != null)
			{
				s.Append(source.ToString());
				s.Append(" : ");
			}
			s.Append(message);
			((QS.Fx.Logging.IConsole)this).Log(s.ToString());
		}

		#endregion

		#region IConsole Members

        void QS.Fx.Logging.IConsole.Log(string s)
		{
			lock (this)
			{
				try
				{
					using (StreamWriter writer = new StreamWriter(filename, true))
					{
						writer.WriteLine(s);
						writer.Flush();
					}
				}
				catch (Exception)
				{
				}
			}
		}

/*
        void QS.Fx.Logging.IConsole.write(string s)
        {
            lock (this)
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filename, true))
                    {
                        writer.Write(s);
                        writer.Flush();
                    }
                }
                catch (Exception)
                {
                }
            }
        }
*/

		#endregion
	}
}
