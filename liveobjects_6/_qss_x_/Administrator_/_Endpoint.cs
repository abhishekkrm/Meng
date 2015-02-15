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
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace QS._qss_x_.Administrator_
{
/*
    public sealed class Endpoint
    {

        public Endpoint(Socket socket, byte[] key, RequestCallback requestcallback)
        {
            this.socket = socket;
            this.key = key;
            this.requestcallback = requestcallback;
            symmetricalgorithm = SymmetricAlgorithm.Create();

        }

        private Socket socket;
        private byte[] key;
        private RequestCallback requestcallback;
        private SymmetricAlgorithm symmetricalgorithm;
        private int lastoutgoing;
        private Queue<Message> outgoing;
        private bool issending;
        private IDictionary<int, Message> pending = new Dictionary<int, Message>();

        public void Request(int operation, byte[][] arguments, ResponseCallback responsecallback, object context)
        {
            lock (this)
            {
                Message message = new Message(
                    ++lastoutgoing, 0, synchronous ? MessageType.Request : MessageType.Event, operation, arguments_in);
                outgoing.Enqueue(message);
                _Outgoing();
                if (synchronous)
                    pending.Add(message.SeqNo, message);
            }
        }

        private void _Outgoing()
        {
            if (!issending && outgoing.Count > 0)
            {
                Message message = outgoing.Dequeue();
                issending = true;
                byte[] initializationvector;
                ICryptoTransform cryptotransform;
                lock (symmetricalgorithm)
                {
                    symmetricalgorithm.GenerateIV();
                    initializationvector = symmetricalgorithm.IV;
                    cryptotransform = symmetricalgorithm.CreateEncryptor(key, initializationvector);
                }
                MemoryStream memorystream = new MemoryStream();
                CryptoStream cryptostream = new CryptoStream(memorystream, cryptotransform, CryptoStreamMode.Write);
                message.Serialize(cryptostream);
                cryptostream.Close();
                memorystream.Close();
                byte[] encryptedmessage = memorystream.ToArray();
                byte[] encryptedmessagesize = BitConverter.GetBytes((int)encryptedmessage.Length);
                List<ArraySegment<byte>> tosend = new List<ArraySegment<byte>>();
                tosend.Add(new ArraySegment<byte>(encryptedmessagesize));
                tosend.Add(new ArraySegment<byte>(encryptedmessage));
                socket.Send(tosend);                
            }
        }
    }
*/ 
}
