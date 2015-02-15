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
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_d_.Agent_2_
{
    public class SessionController : IDisposable, IChannel, ISessionController
    {
        public SessionController(Socket socket, EndSessionCallback endSessionCallback, IService service) : this(socket, endSessionCallback)
        {
            this.session = service.Connect(clientAddress, logger, this);
        }

        public SessionController(Socket socket, EndSessionCallback endSessionCallback, ISession session) : this(socket, endSessionCallback)
        {
            this.session = session;
        }

        public SessionController(Socket socket, EndSessionCallback endSessionCallback)
        {
            this.endSessionCallback = endSessionCallback;
            this.socket = socket;
            this.clientAddress = new QS.Fx.Network.NetworkAddress(((IPEndPoint)socket.RemoteEndPoint).Address, ((IPEndPoint)socket.RemoteEndPoint).Port);
            miniheader = new byte[4];
            remote_ep = new IPEndPoint(IPAddress.Any, 0);
            socket.BeginReceiveFrom(miniheader, 0, 4, SocketFlags.None, ref remote_ep, new AsyncCallback(this.ReceiveCallback), null);
        }

        private EndSessionCallback endSessionCallback;
        private QS.Fx.Network.NetworkAddress clientAddress;
        private Socket socket;
        private QS._core_c_.Base.IReadableLogger logger = new QS._core_c_.Base.Logger(QS._core_c_.Core.Clock.SharedClock, true);
        private byte[] miniheader;
        private EndPoint remote_ep;
        private ISession session;

        #region ReceiveCallback

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                if (!socket.Connected)
                    endSessionCallback(this);
                else
                {
                    int nreceived;
                    if ((nreceived = socket.EndReceiveFrom(result, ref remote_ep)) > 0)
                    {
                        if (nreceived < 4)
                            throw new Exception("Internal error, received a miniheader with less than sizeof(int) bytes.");

                        int messagesize = BitConverter.ToInt32(miniheader, 0);
                        byte[] message = new byte[messagesize];

                        int offset = 0;
                        do
                        {
                            int count = socket.Receive(message, offset, messagesize - offset, SocketFlags.None);
                            if (count > 0)
                                offset += count;
                            else
                                throw new Exception("Internal error, could not receive the entire message.");
                        }
                        while (offset < messagesize);

                        Message msg = (Message) (new XmlSerializer(typeof(Message))).Deserialize(new MemoryStream(message));

                        try
                        {
                            session.Receive(msg);
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, exc.ToString());
                        }
                    }

                    remote_ep = new IPEndPoint(IPAddress.Any, 0);
                    socket.BeginReceiveFrom(miniheader, 0, 4, SocketFlags.None, ref remote_ep, new AsyncCallback(this.ReceiveCallback), null);
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, exc.ToString());
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                socket.Disconnect(false);
            }
            catch (Exception)
            {
            }

            try
            {
                socket.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        public override string ToString()
        {
            return clientAddress.ToString();            
        }

        #region IChannel Members

        void IChannel.Send(Message message)
        {
            MemoryStream stream = new MemoryStream();
            (new XmlSerializer(typeof(Message))).Serialize(stream, message);
            byte[] bytes = stream.GetBuffer();
            int nbytes = (int) stream.Length;
            byte[] miniheader = BitConverter.GetBytes(nbytes);

            SendBytes(miniheader, miniheader.Length);
            SendBytes(bytes, nbytes);
        }

        private void SendBytes(byte[] bytes, int nbytes)
        {
            int offset = 0;
            do
            {
                int count = socket.Send(bytes, offset, nbytes - offset, SocketFlags.None);
                if (count > 0)
                    offset += count;
                else
                    throw new Exception("Internal error: could not send the entire message.");
            }
            while (offset < nbytes);
        }

        #endregion

        #region ISessionController Members

        QS.Fx.Logging.IConsole ISessionController.Console
        {
            get { return logger.Console; }
            set { logger.Console = value; }
        }

        QS.Fx.Network.NetworkAddress ISessionController.Address
        {
            get { return clientAddress; }
        }

        #endregion
    }
}
