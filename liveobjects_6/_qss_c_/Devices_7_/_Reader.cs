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

namespace QS._qss_c_.Devices_7_
{
/*
    public class Reader : IPacket, IEnumerator<IPacket>, IEnumerable<IPacket>, IReader
    {        
        public Reader(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress listeningAddress, int mtu, QS.Fx.Logging.IEventLogger log)
        {
            this.interfaceAddress = interfaceAddress;
            this.listeningAddress = listeningAddress;
            this.mtu = mtu;
            this.buffer = new ArraySegment<byte>(new byte[mtu], 0, mtu);
            this.log = log;
            this.receiveCallback = receiveCallback;

            this.socket = Devices6.Sockets.CreateReceiverUDPSocket(interfaceAddress, ref this.listeningAddress);
            this.receiveAsyncCallback = new AsyncCallback(this.ReceiveCallback);
        }

        private static EndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);

        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Socket socket;
        private ArraySegment<byte> buffer;
        private int mtu, nreceived;
        private IPPacketInformation packetInfo;
        private EndPoint endpoint;
        private SocketFlags flags;
        private QS.Fx.Logging.IEventLogger log;
        private AsyncCallback receiveAsyncCallback;
        private IAsyncResult asynchronousResult;
        private bool waiting, available, started;
        private ReceiveCallback receiveCallback;

        #region Error

        private void Error(string s)
        {
            log.Log(new Logging.Events.Error(double.NaN, null, null, s));
        }

        #endregion

        #region IReader Members

        void IReader.Wait()
        {
            if (waiting)
                throw new Exception("Aready waiting.");

            do
            {
                try
                {
                    endpoint = remote_endpoint;
                    flags = SocketFlags.None;
                    waiting = true;
                    asynchronousResult = socket.BeginReceiveMessageFrom(
                        buffer.Array, buffer.Offset, buffer.Count, flags, ref endpoint, asynchronousCallback, asynchronousState);
                }
                catch (SocketException exc)
                {
                    waiting = false;
                    this.Error("Could not initiate asynchronous receive: " + exc.SocketErrorCode.ToString());
                }
            }
            while (!waiting);
        }

        #endregion

        #region ReceiveCallback

        private void ReceiveCallback(IAsyncResult asynchronousResult)
        {
            if (asynchronousResult != null)
            {
                if ((nreceived = socket.EndReceiveMessageFrom(asynchronousResult, ref flags, ref endpoint, out packetinfo)) > 0)
                {
                    available = true;
                }
                else
                {
                    this.Error("Could not finalize asynchronous receive.");

                    available = Get();                    
                }
            }
            else
                this.Error("Receive callback invoked on an empty request.");

            if (available)
            {                
                waiting = started = false;

                

                // notify somebody........
            }
        }

        #endregion

        #region Get

        private bool Get()
        {
            while (socket.Available > 0)
            {
                try
                {
                    endPoint = remote_endpoint;
                    flags = SocketFlags.None;

                    if ((nreceived = socket.ReceiveMessageFrom(
                        buffer.Array, buffer.Offset, buffer.Count, ref flags, ref endpoint, out packetinfo)) > 0)
                    {
                        return true;
                    }
                    else
                        this.Error("Could not complete synchronous receive.");
                }
                catch (SocketException exc)
                {
                    this.Error("Could not complete synchronous receive: " + exc.SocketErrorCode.ToString());
                }
            }

            return false;
        }

        #endregion

        #region IPacket Members

        IPEndPoint IPacket.Origin
        {
            get { return ((IPEndPoint) endpoint); }
        }

        IPAddress IPacket.Interface
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.NetworkAddress IPacket.Address
        {
            get { return listeningAddress; }
        }

        ArraySegment<byte> IPacket.Data
        {
            get { return new ArraySegment<byte>(buffer.Array, buffer.Offset, nreceived); }
        }

        #endregion

        #region IEnumerator<IPacket> Members

        IPacket IEnumerator<IPacket>.Current
        {
            get 
            { 
                if (!started)
                    throw new Exception("Not started.");

                if (!available)
                    throw new Exception("Already finished.");

                return this;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return ((IEnumerator<IPacket>)this).Current; }
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            if (started)
            {
                available = Get();
                return available;
            }
            else
            {
                started = true;
                return true;
            }
        }

        void System.Collections.IEnumerator.Reset()
        {
            if (started)
                throw new NotSupportedException("Cannot reset the iterator once the iteration has started.");
        }

        #endregion

        #region IEnumerable<Packet> Members

        IEnumerator<IPacket> IEnumerable<IPacket>.GetEnumerator()
        {
            return this;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<IPacket>)this).GetEnumerator();
        }

        #endregion
    }
*/ 
}
