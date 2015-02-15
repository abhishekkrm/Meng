/*

Copyright (c) 2004-2009 Bo Peng. All rights reserved.

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
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Threading;
using Quilt;
using Quilt.HostDetector;

namespace Quilt.HostDetector.NATCheck
{
    class NATCheck
    {
        #region static fields
        //should be user specified
        static public string StunServer_Name = "stunserver.org";

        static public int StunServer_Port = 3478;

        static public string TCP_NATCheck_Server = "localhost";

        static public int TCP_NATCheck_Port = 3478;

        static public int TCP_NATCheck_ListenPort = 3479;

        #endregion

        #region public fields

        public IPEndPoint stunServerEP;
        public IPEndPoint tcpServerEP;
        public TcpListener tcpListener;
        public IPAddress localAddress;

        //NATCheck Results
        public NATTYPE natType;
        public Direction.DIRECTION udpCheckResult;
        public Direction.DIRECTION tcpCheckResult;
        public IPEndPoint publicEP;

        #endregion

        #region constructors

        /// <summary>
        /// default constructor
        /// </summary>
        public NATCheck()
        {
            try
            {
                NetworkInterface[] nis = NetworkInterface.GetAllNetworkInterfaces();
                UnicastIPAddressInformationCollection ips = nis[0].GetIPProperties().UnicastAddresses;
                this.localAddress = ips[0].Address;
                this.stunServerEP = new IPEndPoint(Dns.GetHostEntry(NATCheck.StunServer_Name).AddressList[0], NATCheck.StunServer_Port);

                this.tcpServerEP = new IPEndPoint(Dns.GetHostEntry(NATCheck.TCP_NATCheck_Server).AddressList[0], NATCheck.TCP_NATCheck_Port);

                IPEndPoint tcpListenEP = new IPEndPoint(this.localAddress, NATCheck.TCP_NATCheck_ListenPort);
                this.tcpListener = new TcpListener(tcpListenEP);

                this.natType = NATTYPE.OPEN_INTERNET;
                this.tcpCheckResult = Direction.DIRECTION.BIDIRECTION;
                this.udpCheckResult = Direction.DIRECTION.BIDIRECTION;

                this.publicEP = null;
            }
            catch (Exception x)
            {
                Console.WriteLine("Error Code: " + x.Message + "\n");
            }
        }

        public NATCheck(NetworkInterface ni)
        {
            UnicastIPAddressInformationCollection ips = ni.GetIPProperties().UnicastAddresses;
            this.localAddress = ips[0].Address;

            this.stunServerEP = new IPEndPoint(Dns.GetHostEntry(NATCheck.StunServer_Name).AddressList[0], NATCheck.StunServer_Port);

            this.tcpServerEP = new IPEndPoint(Dns.GetHostEntry(NATCheck.TCP_NATCheck_Server).AddressList[0], NATCheck.TCP_NATCheck_Port);

            IPEndPoint tcpListenEP = new IPEndPoint(this.localAddress, NATCheck.TCP_NATCheck_ListenPort);
            this.tcpListener = new TcpListener(tcpListenEP);

            this.natType = NATTYPE.OPEN_INTERNET;
            this.tcpCheckResult = Direction.DIRECTION.BIDIRECTION;
            this.udpCheckResult = Direction.DIRECTION.BIDIRECTION;

            this.publicEP = new IPEndPoint(localAddress, 0);
        }

        #endregion

        #region public methods

        /// <summary>
        /// NATCheck enter
        /// </summary>
        public void StartCheck()
        {
            Console.WriteLine("NATCheck Started...\n");
            UDPChecker();
            TCPChecker();
        }

        public void UDPChecker()
        {
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                //bind to any port
                IPEndPoint bindEP = new IPEndPoint(this.localAddress, 0);
                sock.Bind(bindEP);
                UdpQuery(this.stunServerEP, sock);
            }
            catch
            {
                this.udpCheckResult = Direction.DIRECTION.UNKNOWN;
            }
        }

        public void TCPChecker()
        {
            Socket sock = null;
            try
            {
                IPEndPoint tcpEP = new IPEndPoint(this.localAddress, 0);
                TcpClient tcpClient = new TcpClient(tcpEP);
                //should be TcpDetectSvrs
                tcpClient.Connect(this.tcpServerEP);
                if (tcpClient.Connected)
                {
                    Timer timer = new Timer(new TimerCallback(Stop), null, 10000, Timeout.Infinite);
                    tcpListener.Start();
                    sock = tcpListener.AcceptSocket();
                }
                else
                {
                    this.tcpCheckResult = (udpCheckResult == Direction.DIRECTION.BIDIRECTION_TRAVERSE) ? Direction.DIRECTION.UNIDIRECTION : udpCheckResult;
                    return;
                }
            }
            catch
            {
                this.tcpCheckResult = (udpCheckResult == Direction.DIRECTION.BIDIRECTION_TRAVERSE) ? Direction.DIRECTION.UNIDIRECTION : udpCheckResult;
                return;
            }

            if (sock != null && this.tcpServerEP.Address == ((IPEndPoint)sock.RemoteEndPoint).Address)
            {
                this.tcpCheckResult = Direction.DIRECTION.BIDIRECTION;
            }
            else
            {
                this.tcpCheckResult = Direction.DIRECTION.UNIDIRECTION;
            }
        }

        #endregion

        #region private methods

        /// <summary>
        /// Gets NAT info from NATChecker server.
        /// </summary>
        /// <param name="host">NATChecker server name or IP.</param>
        /// <param name="port">NATChecker server port. Default port is 3478.</param>
        /// <param name="socket">UDP socket to use.</param>
        /// <returns>Returns UDP netwrok info.</returns>
        /// <exception cref="Exception">Throws exception if unexpected error happens.</exception>
        private void UdpQuery(IPEndPoint udpEP, Socket socket)
        {
            if (udpEP.Equals(null))
            {
                throw new ArgumentNullException("host");
            }
            if (socket.Equals(null))
            {
                throw new ArgumentNullException("socket");
            }
            if (udpEP.Port < 1)
            {
                throw new ArgumentException("Port value must be >= 1 !");
            }
            if (socket.ProtocolType != ProtocolType.Udp)
            {
                throw new ArgumentException("Socket must be UDP socket !");
            }

            IPEndPoint remoteEndPoint = udpEP;

            socket.ReceiveTimeout = 10000;
            socket.SendTimeout = 10000;

            /*
                In test I, the client sends a NATChecker Binding Request to a server, without any flags set in the
                CHANGE-REQUEST attribute, and without the RESPONSE-ADDRESS attribute. This causes the server 
                to send the response back to the address and port that the request came from.
            
                In test II, the client sends a Binding Request with both the "change IP" and "change port" flags
                from the CHANGE-REQUEST attribute set.  
              
                In test III, the client sends a Binding Request with only the "change port" flag set.
            */             

            // Test I
            CheckMessage testI = new CheckMessage();
            testI.Type = MessageType.BindingRequest;
            CheckMessage testIresponse = DoTransaction(testI, socket, remoteEndPoint);

            // UDP blocked.
            if (testIresponse == null)
            {
                this.natType = NATTYPE.UDP_BLOCKED;
                this.udpCheckResult = Direction.DIRECTION.UNKNOWN;
                this.publicEP = null;
            }
            else
            {
                // Test II
                CheckMessage testII = new CheckMessage();
                testII.Type = MessageType.BindingRequest;
                testII.ChangeRequest = new ChangeRequest(true, true);

                // Same IP?
                if (socket.LocalEndPoint.Equals(testIresponse.MappedAddress))
                {
                    CheckMessage testIIResponse = DoTransaction(testII, socket, remoteEndPoint);
                    // Open Internet.
                    if (testIIResponse != null)
                    {
                        this.natType = NATTYPE.OPEN_INTERNET;
                        this.udpCheckResult = Direction.DIRECTION.BIDIRECTION;
                        this.publicEP = testIresponse.MappedAddress;
                    }
                    // Symmetric UDP firewall.
                    else
                    {
                        this.natType = NATTYPE.SYMUDP_FIREWALL;
                        this.udpCheckResult = Direction.DIRECTION.UNIDIRECTION;
                        this.publicEP = testIresponse.MappedAddress;
                    }
                }
                // NAT
                else
                {
                    CheckMessage testIIResponse = DoTransaction(testII, socket, remoteEndPoint);
                    // Full cone NAT.
                    if (testIIResponse != null)
                    {
                        this.natType = NATTYPE.FULL_CONE;
                        this.udpCheckResult = Direction.DIRECTION.BIDIRECTION_TRAVERSE;
                        this.publicEP = testIresponse.MappedAddress;
                    }
                    else
                    {
                        /*
                            If no response is received, it performs test I again, but this time, does so to 
                            the address and port from the CHANGED-ADDRESS attribute from the response to test I.
                        */

                        // Test I(II)
                        CheckMessage testi = new CheckMessage();
                        testi.Type = MessageType.BindingRequest;

                        CheckMessage testiResponse = DoTransaction(testi, socket, testIresponse.ChangedAddress);
                        if (testiResponse == null)
                        {
                            throw new Exception("NATChecker Test I(II) did not get resonse!\n");
                        }
                        else
                        {
                            // Symmetric NAT
                            if (!testiResponse.MappedAddress.Equals(testIresponse.MappedAddress))
                            {
                                this.natType = NATTYPE.SYMMETRIC;
                                this.udpCheckResult = Direction.DIRECTION.UNIDIRECTION;
                                this.publicEP = testIresponse.MappedAddress;
                            }
                            else
                            {
                                // Test III
                                CheckMessage testIII = new CheckMessage();
                                testIII.Type = MessageType.BindingRequest;
                                testIII.ChangeRequest = new ChangeRequest(false, true);

                                CheckMessage testIIIResponse = DoTransaction(testIII, socket, testIresponse.ChangedAddress);
                                // Restricted
                                if (testIIIResponse != null)
                                {
                                    this.natType = NATTYPE.RESTRICTED_CONE;
                                    this.udpCheckResult = Direction.DIRECTION.BIDIRECTION_TRAVERSE;
                                    this.publicEP = testIresponse.MappedAddress;
                                }
                                // Port restricted
                                else
                                {
                                    this.natType = NATTYPE.PORT_RESTRICTED_CONE;
                                    this.udpCheckResult = Direction.DIRECTION.BIDIRECTION_TRAVERSE;
                                    this.publicEP = testIresponse.MappedAddress;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Does NATChecker transaction. Returns transaction response or null if transaction failed.
        /// </summary>
        /// <param name="request">NATChecker message.</param>
        /// <param name="socket">Socket to use for send/receive.</param>
        /// <param name="remoteEndPoint">Remote end point.</param>
        /// <returns>Returns transaction response or null if transaction failed.</returns>
        private static CheckMessage DoTransaction(CheckMessage request, Socket socket, IPEndPoint remoteEndPoint)
        {
            byte[] requestBytes = request.ToByteData();
            DateTime startTime = DateTime.Now;
            // We do it only 2 sec and retransmit with 100 ms.
            while (startTime.AddSeconds(2) > DateTime.Now)
            {
                try
                {
                    socket.SendTo(requestBytes, remoteEndPoint);

                    // We got response.
                    if (socket.Poll(100, SelectMode.SelectRead))
                    {
                        byte[] receiveBuffer = new byte[512];
                        socket.Receive(receiveBuffer);

                        // Parse message
                        CheckMessage response = new CheckMessage();
                        response.Parse(receiveBuffer);

                        // Check that transaction ID matches or not response what we want.
                        if (request.TransactionID.Equals(response.TransactionID))
                            return response;
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine("Error Code: " + x.Message + "\n");
                }
            }
            return null;
        }

        private void Stop(Object info)
        {
            this.tcpListener.Stop();
        }

        #endregion
    }
}
