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
using System.Runtime.InteropServices;

namespace QS._qss_c_.Clocks_2_
{
    public class SynchronizationClient : IDisposable
    {
        public SynchronizationClient(QS.Fx.Network.NetworkAddress multicastAddress, QS.Fx.Clock.IClock clock)
        {
            this.multicastAddress = multicastAddress;
            this.clock = clock;
            foreach (IPAddress interfaceAddress in Dns.GetHostAddresses(Dns.GetHostName()))
                responders.Add(new Responder(this, interfaceAddress));
        }

        private QS.Fx.Network.NetworkAddress multicastAddress;
        private QS.Fx.Clock.IClock clock;
        private IList<Responder> responders = new List<Responder>();

        #region Class Responder

        private class Responder : IDisposable
        {
            private const int NUMBUFFERS = 1000;
            private const int BUFFERSIZE = 20;

            public Responder(SynchronizationClient owner, IPAddress interfaceAddress)
            {
                this.owner = owner;
                this.interfaceAddress = interfaceAddress;

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                socket.Bind(new IPEndPoint(interfaceAddress, owner.multicastAddress.PortNumber));
                if (owner.multicastAddress.IsMulticastAddress)
                {
                    socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership,
                        new MulticastOption(owner.multicastAddress.HostIPAddress, interfaceAddress));
                }
                
                mycallback = new AsyncCallback(this.ReceiveCallback);
                buffers = new byte[NUMBUFFERS][];
                remoteeps = new EndPoint[buffers.Length];
                timediffs = new double[buffers.Length];

                for (int ind = 0; ind < buffers.Length; ind++)
                {
                    buffers[ind] = new byte[BUFFERSIZE];
                    remoteeps[ind] = new IPEndPoint(IPAddress.Any, 0);
                    timediffs[ind] = double.NaN;
                    socket.BeginReceiveFrom(buffers[ind], 0, buffers[ind].Length, SocketFlags.None, ref remoteeps[ind], mycallback, ind);
                }
            }

            private SynchronizationClient owner;
            private IPAddress interfaceAddress;
            private Socket socket;
            private AsyncCallback mycallback;
            private byte[][] buffers;
            private EndPoint[] remoteeps;
            private double[] timediffs;

            #region ReceiveCallback

            private void ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    double remotetime, localtime = owner.clock.Time;
                    int ind = (int)result.AsyncState;
                    int nreceived = socket.EndReceiveFrom(result, ref remoteeps[ind]);

#if DEBUG_Logging
                Console.WriteLine("received " + nreceived.ToString());
#endif

                    if (nreceived >= sizeof(double))
                    {
                        unsafe
                        {
                            fixed (byte* pbuffer = buffers[ind])
                            {
                                remotetime = *((double*)pbuffer);
                            }
                        }

                        timediffs[ind] = localtime - remotetime;
                    }
                }
                catch (Exception)
                {
                }
            }

            #endregion

            #region Delta

            public double Delta
            {
                get
                {
                    double delta = double.MaxValue;
                    for (int ind = 0; ind < buffers.Length; ind++)
                    {
                        if (timediffs[ind] < delta)
                            delta = timediffs[ind];
                    }
                    
                    return delta;
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
        }

        #endregion

        #region Delta

        public double Delta
        {
            get
            {
                double delta = double.PositiveInfinity;
                foreach (Responder responder in responders)
                {
                    double resdelta = responder.Delta;
                    if (resdelta < delta)
                        delta = resdelta;
                }

                return delta;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            foreach (Responder responder in responders)
                ((IDisposable)responder).Dispose();
        }

        #endregion
    }
}
