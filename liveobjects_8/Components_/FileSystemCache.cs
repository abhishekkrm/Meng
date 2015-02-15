/* Copyright (c) 2009 Jared Cantwell. All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using System.Windows.Forms;

namespace MapLibrary
{
#if XNA
    [QS.Fx.Reflection.ComponentClass("5E24DDE68A874ad1A27673FDCB9AB9A3", "Filesystem Cache", "Cache String/byte[] pairs to the filesystem for persistent storage")]
    class FileSystemCache : ICache, ICacheOps
    {

        #region Constructor
        public FileSystemCache(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Storage Root Path (default: c:/livebojects/cache/)", QS.Fx.Reflection.ParameterClass.Value)]
            String root) 
        {
            if (root != null)
            {
                // Create the path if it doesn't exist
                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);

                // Make sure the path ends in a trailing slash
                if (!(root.EndsWith("\\") || root.EndsWith("/")))
                    root += "\\";

                rootPath = root;
            }

            endpoint = _mycontext.ExportedInterface<ICacheOps>(this);
        }
        #endregion

        #region Private Fields

        private QS.Fx.Endpoint.Internal.IExportedInterface<ICacheOps> endpoint;
        // Trailing slash should always be there
        private String rootPath = "C:\\liveobjects\\cache\\";       // default path

        #endregion

        #region ICacheOps Members

        void ICacheOps.Add(string key, byte[] obj)
        {
            if(File.Exists(rootPath + key)) {
                try { File.Delete(rootPath + key); }
                catch(Exception e){}
            }

            using (FileStream file = new FileStream(rootPath + key, FileMode.OpenOrCreate, FileAccess.Write))
            {
                file.Write(obj, 0, obj.Length);
            }
        }

        bool ICacheOps.Contains(string key)
        {
            return File.Exists(rootPath + key);
        }

        bool ICacheOps.Remove(string key)
        {
            try
            {
                File.Delete(rootPath + key);
            }
            catch (Exception e) 
            {
                return false;
            }

            return true;
        }

        byte[] ICacheOps.Get(string key)
        {
            if (!File.Exists(rootPath + key))
                return null;

            using (FileStream fsSource = new FileStream(rootPath + key, FileMode.Open, FileAccess.Read))
            {
                // Read the source file into a byte array.
                byte[] bytes = new byte[fsSource.Length];
                int numBytesToRead = (int)fsSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                return bytes;    
            }
        }

        #endregion

        #region ICache Members

        QS.Fx.Endpoint.Classes.IExportedInterface<ICacheOps> ICache.Cache
        {
            get { return this.endpoint; }
        }

        #endregion
    }
#endif
}
