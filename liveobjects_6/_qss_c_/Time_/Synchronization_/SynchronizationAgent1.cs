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

// #define DEBUG_SynchronizationAgent1

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Time_.Synchronization_
{
    public class SynchronizationAgent1
    {
        #region Class Client

        public class Client
        {
            public Client(IPAddress localAddress, QS.Fx.Network.NetworkAddress agentAddress)
                : this(localAddress, agentAddress, QS._core_c_.Base2.PreciseClock.Clock)
            {
            }

            public Client(IPAddress localAddress, QS.Fx.Network.NetworkAddress agentAddress, QS.Fx.Clock.IClock clock)
            {
                this.clock = clock;
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listeningSocket.Bind(new IPEndPoint(localAddress, 0));
                portno = ((IPEndPoint)listeningSocket.LocalEndPoint).Port;
                senderRemote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

#if DEBUG_SynchronizationAgent1
                Console.WriteLine("Listening at " + ((IPEndPoint)listeningSocket.LocalEndPoint).Address.ToString() + ":" +
                    ((IPEndPoint)listeningSocket.LocalEndPoint).Port.ToString());
#endif

                sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sendingSocket.Bind(new IPEndPoint(localAddress, 0));
                sendingSocket.Connect(new IPEndPoint(agentAddress.HostIPAddress, agentAddress.PortNumber));

#if DEBUG_SynchronizationAgent1
                Console.WriteLine("Sending from " + ((IPEndPoint)sendingSocket.LocalEndPoint).Address.ToString() + ":" +
                    ((IPEndPoint)sendingSocket.LocalEndPoint).Port.ToString());
#endif
            }

            private Socket listeningSocket, sendingSocket;
            private int portno;
            private byte[] buffer1 = new byte[sizeof(uint) + sizeof(int)];
            private byte[] buffer2 = new byte[sizeof(uint) + sizeof(int) + sizeof(double)];
            private uint seqno;
            private EndPoint senderRemote;
            private QS.Fx.Clock.IClock clock;
            private double t1, t2, t3;
            private AutoResetEvent sendCompleted = new AutoResetEvent(false), receiveCompleted = new AutoResetEvent(false);            

            public unsafe double Synchronize(int nsamples)
            {
                double min_1to2 = double.MaxValue, min_2to1 = double.MaxValue;
                int samples = 0;
                while (samples  < nsamples)
                {
                    lock (this)
                    {
                        fixed (byte* pbuffer = buffer1)
                        {
                            *((uint*)pbuffer) = ++seqno;
                            *((int*)(pbuffer + sizeof(uint))) = portno;
                        }

                        listeningSocket.BeginReceiveFrom(
                            buffer2, 0, buffer2.Length, SocketFlags.None, ref senderRemote, new AsyncCallback(this.ReceiveCallback), null);

                        t1 = clock.Time;

                        sendingSocket.BeginSend(buffer1, 0, buffer1.Length, SocketFlags.None, new AsyncCallback(this.SendCallback), null);

                        bool sendOK = sendCompleted.WaitOne(1, true);
                        bool receivedOK = receiveCompleted.WaitOne(10, true);

                        if (sendOK && receivedOK)
                        {
                            double time_1to2 = t2 - t1, time_2to1 = t3 - t2;
                            if (time_1to2 < min_1to2)
                                min_1to2 = time_1to2;
                            if (time_2to1 < min_2to1)
                                min_2to1 = time_2to1;
                            samples++;
                        }
                    }
                }

                return (min_1to2 - min_2to1) / 2;
            }

            #region ReceiveCallback

            private unsafe void ReceiveCallback(System.IAsyncResult asyncResult)
            {
                lock (this)
                {
                    double now = clock.Time, received_time;
                    uint received_seqno;
                    int nreceived = listeningSocket.EndReceiveFrom(asyncResult, ref senderRemote);
                    if (nreceived >= sizeof(uint) + sizeof(int) + sizeof(double))
                    {
                        fixed (byte* pbuffer = buffer2)
                        {
                            received_seqno = *((uint*)pbuffer);
                            received_time = *((double*)(pbuffer + sizeof(uint) + sizeof(int)));
                        }

                        if (received_seqno == seqno)
                        {
                            t2 = received_time;
                            t3 = now;
                            receiveCompleted.Set();
                        }
                    }
                }
            }

            #endregion

            #region SendCallback

            private void SendCallback(System.IAsyncResult asyncResult)
            {
                sendingSocket.EndSend(asyncResult);
                sendCompleted.Set();
            }

            #endregion
        }

        #endregion

        public SynchronizationAgent1(QS.Fx.Network.NetworkAddress listeningAddress) : this(listeningAddress, QS._core_c_.Base2.PreciseClock.Clock)
        {
        }

        public SynchronizationAgent1(QS.Fx.Network.NetworkAddress listeningAddress, QS.Fx.Clock.IClock clock)
        {
            this.clock = clock;
            listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listeningSocket.Bind(new IPEndPoint(listeningAddress.HostIPAddress, listeningAddress.PortNumber));
            this.listeningAddress = new QS.Fx.Network.NetworkAddress(
                ((IPEndPoint)listeningSocket.LocalEndPoint).Address, ((IPEndPoint)listeningSocket.LocalEndPoint).Port);
            senderRemote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));            
            listeningSocket.BeginReceiveFrom(
                buffer, 0, buffer.Length, SocketFlags.None, ref senderRemote, new AsyncCallback(this.ReceiveCallback), null);

            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendingSocket.Bind(new IPEndPoint(listeningAddress.HostIPAddress, 0));            
        }

        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Socket listeningSocket, sendingSocket;
        private byte[] buffer = new byte[sizeof(uint) + sizeof(int) + sizeof(double)];
        private EndPoint senderRemote;
        private QS.Fx.Clock.IClock clock;

        public QS.Fx.Network.NetworkAddress Address
        {
            get { return listeningAddress; }
        }

        #region ReceiveCallback

        private unsafe void ReceiveCallback(System.IAsyncResult asyncResult)
        {
            int nreceived = listeningSocket.EndReceiveFrom(asyncResult, ref senderRemote);

#if DEBUG_SynchronizationAgent1
            Console.WriteLine("Received " + nreceived.ToString() + " from " + ((IPEndPoint)senderRemote).Address.ToString() + ":" +
                ((IPEndPoint)senderRemote).Port.ToString());
#endif

            if (nreceived >= sizeof(uint))
            {
                fixed (byte* pbuffer = buffer)
                {
                    ((IPEndPoint)senderRemote).Port = *((int*)(pbuffer + sizeof(uint)));
                    *((double*)(pbuffer + sizeof(uint) + sizeof(int))) = clock.Time;
                }

#if DEBUG_SynchronizationAgent1
                Console.WriteLine("Responding to " + ((IPEndPoint)senderRemote).Address.ToString() + ":" +
                    ((IPEndPoint)senderRemote).Port.ToString());
#endif
                
                sendingSocket.BeginSendTo(buffer, 0, buffer.Length, SocketFlags.None, senderRemote, 
                    new AsyncCallback(this.SendCallback), null);
            }            
        }

        #endregion

        #region SendCallback
        
        private void SendCallback(System.IAsyncResult asyncResult)
        {
            sendingSocket.EndSendTo(asyncResult);
            listeningSocket.BeginReceiveFrom(
                buffer, 0, buffer.Length, SocketFlags.None, ref senderRemote, new AsyncCallback(this.ReceiveCallback), null);
        }

        #endregion
    }
}
