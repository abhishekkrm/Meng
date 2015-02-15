using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QS.Fx.Object
{
    class FileConsole : QS.Fx.Logging.IConsole
    {
        private StreamWriter writer;

        public FileConsole(string filename)
        {
            writer = new StreamWriter(filename, true);
        }

        #region IConsole Members

        public void Log(string s)
        {
            lock (this) // necessary? not sure
            {
                writer.WriteLine(s);
                writer.Flush();
            }
        }

        #endregion
    }
}
