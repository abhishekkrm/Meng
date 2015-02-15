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

// #define DEBUG_Logging

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

namespace QS._qss_c_.Clocks_2_
{
    public class SynchronizationServer
    {
        private const int BUFFERSIZE = 20;
        private const int NUMBUFFERS = 1000;

        public SynchronizationServer(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress multicastAddress, QS.Fx.Clock.IClock clock)
        {
            this.interfaceAddress = interfaceAddress;
            this.multicastAddress = multicastAddress;
            this.clock = clock;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);            
            socket.Bind(new IPEndPoint(interfaceAddress, 0));

            if (multicastAddress.IsMulticastAddress)
            {
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
                socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastLoopback, false);
            }
            socket.Connect(multicastAddress.HostIPAddress, multicastAddress.PortNumber);

            mycallback = new AsyncCallback(this.SendCallback);
            buffers = new byte[NUMBUFFERS][];
            handles = new GCHandle[buffers.Length];
            addresses = new IntPtr[buffers.Length];
            for (int ind = 0; ind < buffers.Length; ind++)
            {
                buffers[ind] = new byte[BUFFERSIZE];
                handles[ind] = GCHandle.Alloc(buffers[ind], GCHandleType.Pinned);
                addresses[ind] = handles[ind].AddrOfPinnedObject();
            }
        }

        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress multicastAddress;
        private QS.Fx.Clock.IClock clock;
        private Socket socket;
        private AsyncCallback mycallback;
        private byte[][] buffers;
        private GCHandle[] handles;
        private IntPtr[] addresses;
        private int nsent;
        private ManualResetEvent done = new ManualResetEvent(false);

        #region Send

        public void Send()
        {
            SendOne(0);
            done.WaitOne();
        }

        private unsafe void SendOne(int ind)
        {
            fixed (byte* pbuffer = buffers[ind])
            {
                *((double*)pbuffer) = clock.Time;
            }


            SocketError errorcode;
            socket.BeginSend(buffers[ind], 0, buffers[ind].Length, SocketFlags.None, out errorcode, mycallback, null);
            if (errorcode != SocketError.Success)
                throw new Exception("Cannot send: " + errorcode.ToString());
        }

        #endregion

        #region SendCallback

        private void SendCallback(IAsyncResult result)
        {
            SocketError errorcode;
            socket.EndSend(result, out errorcode);
            if (errorcode != SocketError.Success)
                Console.WriteLine("Cannot send: " + errorcode.ToString());

            nsent++;
            if (nsent < NUMBUFFERS)
                SendOne(nsent);
            else
                done.Set();
        }

        #endregion
    }
}
