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

namespace QS._qss_x_.Administrator_
{
    public sealed class Request
    {
        #region Constructor

        public Request(string from, string to, RequestType type)
        {
            this.from = from;
            this.to = to;
            this.type = type;
        }

        #endregion

        #region Fields

        private string from, to;
        private RequestType type;

        #endregion

        #region Accessors

        public string From
        {
            get { return from; }
        }

        public string To
        {
            get { return to; }
        }

        public RequestType Type
        {
            get { return type; }
        }

        #endregion

        #region Resolve

        public Request[] Resolve()
        {
            List<Request> resolved = new List<Request>();
            int separator = from.LastIndexOf(Path.DirectorySeparatorChar);
            if (separator < 0)
                throw new Exception("Unsupported path: \"" + from + "\".");
            string fromfolder = from.Substring(0, separator);
            if (Directory.Exists(fromfolder))
            {
                string fromitems = from.Substring(separator + 1).Trim();
                if (fromitems.Length == 0)
                    fromitems = "*";
                foreach (string path in Directory.GetFiles(fromfolder, fromitems))
                    resolved.Add(new Request(path, to + Path.DirectorySeparatorChar + Path.GetFileName(path), type));
                Queue<KeyValuePair<string, string>> folders = new Queue<KeyValuePair<string, string>>();
                foreach (string path in Directory.GetDirectories(fromfolder, fromitems))
                {
                    separator = path.LastIndexOf(Path.DirectorySeparatorChar);
                    string _target = to + Path.DirectorySeparatorChar + path.Substring(separator + 1);
                    folders.Enqueue(new KeyValuePair<string, string>(path, _target));
                }
                while (folders.Count > 0)
                {
                    KeyValuePair<string, string> element = folders.Dequeue();
                    string _from = element.Key;
                    string _to = element.Value;
                    foreach (string path in Directory.GetFiles(_from))
                        resolved.Add(new Request(path, _to + Path.DirectorySeparatorChar + Path.GetFileName(path), type));
                    foreach (string path in Directory.GetDirectories(_from))
                    {
                        separator = path.LastIndexOf(Path.DirectorySeparatorChar);
                        string _target = _to + Path.DirectorySeparatorChar + path.Substring(separator + 1);
                        folders.Enqueue(new KeyValuePair<string, string>(path, _target));
                    }
                }
            }
            return resolved.ToArray();
        }

        #endregion
    }
}
