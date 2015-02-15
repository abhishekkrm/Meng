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
using System.Net.NetworkInformation;

namespace Quilt.HostDetector.Traceroute
{
    public class Traceroute
    {
        #region public methods

        public static void Tracert(string ipAddr, out Queue<string> hostQueue)
        {
            try
            {

                Console.WriteLine("TraceRoute Started...\n");
                byte[] data = new byte[1024];
                int recv, timestart, timestop;

                // Transform address
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ipAddr), 0);
                EndPoint ep = (EndPoint)iep;
                string temphostname = Dns.GetHostEntry(iep.Address).HostName;

                // Make ICMP packet
                ICMP packet = new ICMP();
                packet.Type = 0x08;
                packet.Code = 0x00;
                packet.Checksum = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 2, 2);
                data = Encoding.ASCII.GetBytes("Traceroute Packet");
                Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
                packet.MessageSize = data.Length + 4;
                int packetsize = packet.MessageSize + 4;
                UInt16 chcksum = packet.getChecksum();
                packet.Checksum = chcksum;

                // Create traceroute raw socket
                Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                host.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                // Create output host queue
                hostQueue = new Queue<string>();

                int badcount = 0;
                for (int i = 1; i < 5; i++)
                {
                    // Set TTL from 1 to 50??
                    host.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, i);
                    timestart = Environment.TickCount;
                    host.SendTo(packet.getBytes(), packetsize, SocketFlags.None, iep);
                    try
                    {
                        data = new byte[1024];
                        recv = host.ReceiveFrom(data, ref ep);
                        timestop = Environment.TickCount;
                        ICMP response = new ICMP(data, recv);

                        if (response.Type == 11) // Intermediate result
                        {
                            IPEndPoint ipEp = ep as IPEndPoint;
                            hostQueue.Enqueue(ipEp.Address.ToString());
                        }
                        if (response.Type == 0) // End of traceroute
                        {
                            if (i > 1)
                            {
                                IPEndPoint ipEp = ep as IPEndPoint;
                                hostQueue.Enqueue(ipEp.Address.ToString());

                                Console.WriteLine("{0} reached in {1} hops, {2}ms.\n", ep.ToString(), i, timestop - timestart);

                                foreach (string s in hostQueue)
                                {
                                    Console.WriteLine("hop: " + s + "\n");
                                }

                                break;
                            }
                            // if it's only one hop between host and dns server, change the domain name, and try a larger range
                            // for example, if domain name is "cs.cornell.edu", change it into "cornell.edu"
                            // if not, traceroute ends.
                            else if (temphostname != ipAddr && temphostname != "")
                            {
                                string[] namepieces = temphostname.Split('.');
                                if (namepieces.GetLength(0) - 1 == 0)
                                {
                                    Console.WriteLine("Error: Trace is too short.\n");
                                    break;
                                }
                                temphostname = null;
                                for (int j = 1; j < namepieces.GetLength(0) - 1; j++)
                                    temphostname += namepieces[j] + ".";
                                temphostname += namepieces[namepieces.GetLength(0) - 1];
                                IPHostEntry tempep = Dns.GetHostEntry(temphostname);
                                iep = new IPEndPoint(tempep.AddressList[0], 0);
                                hostQueue.Clear();
                                i = 0;
                            }
                            else
                            {
                                // TODO

                                // Pick a random IP/ or traceroute www.cornell.edu

                                // limit to 4
                            }
                        }
                        badcount = 0;
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("hop {0}: No response from remote host \n", i);
                        badcount++;
                        if (badcount == 5)
                        {
                            Console.WriteLine("Unable to reach remote host \n");
                            break;
                        }
                    }
                }
                host.Close();
            }
            catch (Exception exc)
            {
                throw new Exception("Traceoute: Tracert exception " + exc.Message);
            }
        }

        public static void PingTest(string ipAddr, int timeoutMS, out long rtt)
        {
            Ping pingsender = new Ping();
            PingOptions option = new PingOptions();
            option.DontFragment = true;
            PingReply reply = pingsender.Send(ipAddr, timeoutMS);
            if (reply.Status == IPStatus.Success)
            {
                rtt = reply.RoundtripTime;
            }
            else
            {
                rtt = timeoutMS;
            }
        }

        public static void PingAdvance(string localip, string ipAddr, int timeoutMS, out long rtt, out Dictionary<string, bool> returnDic)
        {
            try
            {
                rtt = 123;
                Console.WriteLine("TraceRoute Started...\n");
                byte[] data = new byte[1024];
                int recv, timestart, timestop;

                // Transform address
                IPEndPoint iep = new IPEndPoint(IPAddress.Parse(ipAddr), 0);
                EndPoint ep = (EndPoint)iep;
                string temphostname = Dns.GetHostEntry(iep.Address).HostName;

                // Make ICMP packet
                ICMP packet = new ICMP();
                packet.Type = 0x08;
                packet.Code = 0x00;
                packet.Checksum = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(1), 0, packet.Message, 2, 2);
                data = Encoding.ASCII.GetBytes("Traceroute Packet");
                Buffer.BlockCopy(data, 0, packet.Message, 4, data.Length);
                packet.MessageSize = data.Length + 4;
                int packetsize = packet.MessageSize + 4;
                UInt16 chcksum = packet.getChecksum();
                packet.Checksum = chcksum;

                // Create traceroute raw socket
                Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                host.Bind((EndPoint)(new IPEndPoint(IPAddress.Parse(localip), 10000)));
                host.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                // Create output host queue
                returnDic = new Dictionary<string, bool>();
                timestart = Environment.TickCount;
                host.SendTo(packet.getBytes(), packetsize, SocketFlags.None, iep);
                
                try
                {
                    while (true)
                    {
                        data = new byte[1024];
                        recv = host.ReceiveFrom(data, ref ep);
                        timestop = Environment.TickCount;

                        if ((timestop - timestart) / TimeSpan.TicksPerMillisecond > timeoutMS)
                        {
                            break;
                        }

                        ICMP response = new ICMP(data, recv);

                        {
                            IPEndPoint ipEp = ep as IPEndPoint;
                            string addr = ipEp.Address.ToString();

                            if (!returnDic.ContainsKey(addr))
                            {
                                returnDic.Add(addr, true);
                            }
                        }

                    }
                }
                catch (SocketException)
                {
                    Console.WriteLine("PingTest No response from remote host");
                }

                host.Close();
            }
            catch (Exception exc)
            {
                throw new Exception("Traceoute: Tracert exception " + exc.Message);
            }
        }

        #endregion
    }
}
