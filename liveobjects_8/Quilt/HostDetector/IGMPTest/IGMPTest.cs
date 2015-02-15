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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Net.NetworkInformation;

namespace Quilt.HostDetector.IGMPTest
{
    public class IGMPTest
    {
        #region private fields

        private Queue<string> route;
        private IPAddress groupAddress = null;
        private IPAddress localAddress = null;
        private IPAddress blockAddress = null;
        // index pointing to blockAddress
        private int index;

        #endregion

        #region static public fields

        static public int LocalPort = 3483;

        static public int RECVTT = 120000; // socket receive timeout: 120000 milliseconds

        #endregion

        #region Constructor

        public IGMPTest(Queue<string> hostQueue)
        {
            groupAddress = IPAddress.Parse("224.0.0.1");
            NetworkInterface[] ni = NetworkInterface.GetAllNetworkInterfaces();
            UnicastIPAddressInformationCollection ips = ni[0].GetIPProperties().UnicastAddresses;
            localAddress = ips[0].Address;
            // deep copy
            route = new Queue<string>(hostQueue);
        }

        #endregion

        #region Start

        public void Start()
        {
            Console.WriteLine("IGMPTest Started...\n");
            int padByteCount = 0, sendCount = 1, recv = 0;
            int timestart = 0, timestop = 0;
            bool success = true;

            while (success && route.Count() > 0)
            {
                IPAddress routerAddress = IPAddress.Parse(route.Dequeue());
                IgmpHeader igmpHeader = new IgmpHeader();
                Ipv4Header ipv4Header = new Ipv4Header();
                ArrayList headerList = new ArrayList();
                IPEndPoint igmpDestination = new IPEndPoint(routerAddress, 0);
                IPEndPoint localEndpoint = new IPEndPoint(localAddress, 0);
                Socket igmpSocket = null;
                int timeout = RECVTT;
                byte[] igmpPacket, padBytes;
                byte[] data = new byte[1024];

                // Build the IPv4 header
                //Console.WriteLine("Building the IPv4 header...");
                ipv4Header.Version = 4;
                ipv4Header.Protocol = (byte)ProtocolType.Igmp;
                ipv4Header.Ttl = 20;
                ipv4Header.Offset = 0;
                ipv4Header.Length = 20;
                ipv4Header.TotalLength = (ushort)System.Convert.ToUInt16(IgmpHeader.IgmpHeaderLength + padByteCount);
                ipv4Header.SourceAddress = localAddress;
                ipv4Header.DestinationAddress = routerAddress;

                // Add the header to the list of headers
                //Console.WriteLine("Adding the header to the list of headers...");
                headerList.Add(ipv4Header);

                // Build the IGMP header
                //Console.WriteLine("Building the IGMP header...");
                igmpHeader.VersionType = IgmpHeader.IgmpMembershipQuery;
                igmpHeader.MaximumResponseTime = 100;
                igmpHeader.GroupAddress = groupAddress;

                // Allocate pad bytes as necessary
                //Console.WriteLine("Allocating pad bytes as necessary...");
                padBytes = new byte[padByteCount];

                // Add the IGMP header to the list of headers
                //Console.WriteLine("Adding the IGMP header to the list of headers...");
                headerList.Add(igmpHeader);

                // Build the packet
                //Console.WriteLine("Building the packet...");
                igmpPacket = igmpHeader.BuildPacket(headerList, padBytes);

                // Create the raw socket
                //Console.WriteLine("Creating the raw socket, Socket()...");

                try
                {
                    igmpSocket = new Socket(
                        AddressFamily.InterNetwork,
                        SocketType.Raw,
                        ProtocolType.Igmp
                        );

                    // Bind to a local interface
                    //Console.WriteLine("Binding to a local interface, Bind()...");
                    EndPoint temp_ep = (EndPoint)(new IPEndPoint(localAddress, LocalPort));
                    igmpSocket.Bind(temp_ep);

                    // Set the HeaderIncluded option
                    //Console.WriteLine("Setting the HeaderIncluded option...");
                    igmpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);

                    byte[] invalue = new byte[4] { 1, 0, 0, 0 };
                    byte[] outvalue = new byte[4];
                    igmpSocket.IOControl(IOControlCode.ReceiveAllIgmpMulticast, invalue, outvalue);
                    igmpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 30000);

                    // Send the packet the requested number of times
                    //Console.WriteLine("Send the packet the requested number of times...");
                    for (int i = 0; i < sendCount; i++)
                    {
                        Console.WriteLine("IGMPSnooping Destination...: " + igmpDestination.Address.ToString() + "\n");
                        int rc = igmpSocket.SendTo(igmpPacket, igmpDestination);
                        //Console.WriteLine("send {0} bytes to {1}", rc, igmpDestination.ToString());
                    }

                    EndPoint ep = (EndPoint)igmpDestination;
                    while (timeout > 0)
                    {
                        timestart = Environment.TickCount;
                        try
                        {
                            recv = igmpSocket.ReceiveFrom(data, ref ep);
                        }
                        catch (Exception x)
                        {
                            Console.WriteLine("IPMC Disabled...\n");
                            blockAddress = routerAddress;
                            throw new Exception("recv timeout\n");
                        }
                        timestop = Environment.TickCount;
                        timeout = timeout - (timestop - timestart);
                        //Console.WriteLine("IGMPSnooping Destination...: " + ep.AddressFamily.ToString());

                        //check if the router responds
                        if (recv > 0 && igmpDestination.Equals((IPEndPoint)ep))
                        {
                            recv = 0;
                            Console.WriteLine("IPMC Enabled...\n");
                            break;
                        }
                    }
                    if (timeout <= 0)
                    {
                        Console.WriteLine("IPMC Disabled...\n");
                        blockAddress = routerAddress;
                        success = false;
                    }
                    igmpSocket.Close();

                    index++;
                }
                catch (Exception x)
                {
                    success = false;
                    //Console.WriteLine("Error Code: " + x.Message + "\n"); 
                    igmpSocket.Close();
                }
            }
        }

        #endregion

        #region Public Attributes

        public IPAddress BlockedAddress
        {
            get { return blockAddress; }
        }

        public int Index
        {
            get { return index; }
        }

        #endregion
    }
}
