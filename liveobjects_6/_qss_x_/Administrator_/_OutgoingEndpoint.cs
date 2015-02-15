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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Text;

namespace QS._qss_x_.Administrator_
{
/*
    public sealed class OutgoingEndpoint : IDisposable
    {
        public OutgoingEndpoint(Socket socket, ICryptoTransform encryptor)
        {
            this.socket = socket;
            this.encryptor = encryptor;
        }

        private const int BUFFERSIZE = 1000;

        private Socket socket;
        private ICryptoTransform encryptor;
        private Queue<KeyValuePair<string, string>> tosend = new Queue<KeyValuePair<string, string>>();
        private bool sending;

        public void Send(string from, string to)
        {
            lock (this)
            {
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
                        tosend.Enqueue(new KeyValuePair<string, string>(path, to + Path.DirectorySeparatorChar + Path.GetFileName(path)));
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
                            tosend.Enqueue(new KeyValuePair<string, string>(path, _to + Path.DirectorySeparatorChar + Path.GetFileName(path)));
                        foreach (string path in Directory.GetDirectories(_from))
                        {
                            separator = path.LastIndexOf(Path.DirectorySeparatorChar);
                            string _target = _to + Path.DirectorySeparatorChar + path.Substring(separator + 1);
                            folders.Enqueue(new KeyValuePair<string, string>(path, _target));
                        }
                    }
                }
                if (tosend.Count > 0 && !sending)
                {
                    sending = true;
                    _Send();
                }
            }
        }

        private void _Send()
        {            
            KeyValuePair<string, string> element = tosend.Dequeue();
            string source = element.Key;
            string destination = element.Value;            

            MemoryStream memorystream = new MemoryStream();
            CryptoStream cryptostream = new CryptoStream(memorystream, encryptor, CryptoStreamMode.Write);
            byte[] buffer = new byte[BUFFERSIZE];
            using (FileStream filestream = new FileStream(source, FileMode.Open, FileAccess.Read))
            {
                int count;
                while ((count = filestream.Read(buffer, 0, buffer.Length)) > 0)
                    cryptostream.Write(buffer, 0, count);
            }
            cryptostream.Close();
            memorystream.Close();
            byte[] filedata = memorystream.ToArray();
            byte[] sourcebytes = Encoding.Unicode.GetBytes(source);
            byte[] destinationbytes = Encoding.Unicode.GetBytes(destination);

            IList<ArraySegment<byte>> data = new List<ArraySegment<byte>>();
            data.Add(new ArraySegment<byte>(BitConverter.GetBytes(true)));
            data.Add(new ArraySegment<byte>(BitConverter.GetBytes((int) sourcebytes.Length)));
            data.Add(new ArraySegment<byte>(sourcebytes));
            data.Add(new ArraySegment<byte>(BitConverter.GetBytes((int) destinationbytes.Length)));
            data.Add(new ArraySegment<byte>(destinationbytes));
            data.Add(new ArraySegment<byte>(BitConverter.GetBytes((int) filedata.Length)));
            if (filedata.Length > 0)
                data.Add(new ArraySegment<byte>(filedata));

            socket.BeginSend(data, SocketFlags.None, new AsyncCallback(this._SendCallback), data);
        }

        private void _SendCallback(IAsyncResult result)
        {
            lock (this)
            {
                IList<ArraySegment<byte>> data = (IList<ArraySegment<byte>>) result.AsyncState;
                int count = 0;
                foreach (ArraySegment<byte> segment in data)
                    count += segment.Count;

                SocketError socketerror;
                int nsent = socket.EndSend(result, out socketerror);
                if (socketerror != SocketError.Success)
                {
                    // ..........
                }
            }
        }

        void IDisposable.Dispose()
        {
        }
    }
*/ 
}
