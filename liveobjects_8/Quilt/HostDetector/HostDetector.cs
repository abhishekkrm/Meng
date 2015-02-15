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

#define GRADIENT

using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Quilt.HostDetector.ICMPTest;
using Quilt.HostDetector.Traceroute;
using Quilt.HostDetector.NATCheck;

namespace Quilt.HostDetector
{
    public class HostDetector
    {
        #region private fields

        private bool _is_experiment = false;

        /// <summary>
        /// keep all results detected now
        /// </summary>
        private Dictionary<string, DetectResult> resultDict;

        /// <summary>
        /// keep the route from host to a DNS server
        /// </summary>
        private Queue<string> route;

        /// <summary>
        /// keep the Publicaddress of private address in the last NAT
        /// </summary>
        private IPEndPoint publicEP;

        //private String _result_file;

        private string _result_file;

        private Callback _call_back;
        private AsyncCallback _check_callback;
        private CHECK_HANDLER _check_handler;

        #endregion

        #region public fields

        static public string FileName = "C:/cachefile.txt";

        public delegate void CHECK_HANDLER(string nic_addr);

        public delegate void Callback(string nic_addr, DetectResult result);

        #endregion

        #region constructor

        /// <summary>
        /// detect host network with default servers
        /// </summary>
        //public HostDetector(Callback call_back, string cache_path)
        public HostDetector(Callback call_back, bool is_experiment)
        {
            this._is_experiment = is_experiment;

            try
            {
                route = new Queue<string>();

                this.resultDict = new Dictionary<string, DetectResult>();

                //this._result_file = cache_path + FileName;
                this._result_file = HostDetector.FileName;

                //load results in disc cache to resultDict
                FileStream fs = new FileStream(this._result_file, FileMode.OpenOrCreate, FileAccess.Read);
                StreamReader sr = new StreamReader(this._result_file);
                string rstring = this.SyncCacheRead(sr, fs);

                this._Unmarshal_resultDict(rstring);

                if (call_back == null) throw new Exception("Call back missing!!\n");
                this._call_back = call_back;
                this._check_callback = new AsyncCallback(CheckHandlerCallback);
                this._check_handler = new CHECK_HANDLER(CheckHandler);
            }
            catch (Exception x)
            {
                Console.WriteLine("Error Code: " + x.Message + "\n");
            }
        }

        #endregion

        #region public methods

        public Queue<string> Route
        {
            get { return route; }
        }

        public void Start(string ni)
        {
            try
            {
                // Return if no NIC is available
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    resultDict.Clear();
                    Console.WriteLine("The network is not available, please try again later.\n");
                    return;
                }
                this._check_handler.BeginInvoke(ni, _check_callback, ni);
            }
            catch
            {
            }
        }
        #endregion

        #region private methods

        private string SyncCacheRead(StreamReader sr, FileStream fs)
        {
            string rstring;
            try
            {
                rstring = sr.ReadToEnd();
            }
            catch (Exception x)
            {
                Thread.Sleep(1000000);
                rstring = this.SyncCacheRead(sr, fs);
            }
            sr.Close();
            fs.Close();
            return rstring;
        }

        private void SyncCacheWrite(StreamWriter sw, string result)
        {
            try
            {
                sw.Write(result);
                sw.Close();
            }
            catch (Exception x)
            {
                Thread.Sleep(500000);
                this.SyncCacheWrite(sw, result);
            }
            sw.Close();
        }


        private void CheckHandlerCallback(IAsyncResult ir)
        {
            if (ir == null)
                throw new ArgumentNullException("CheckHandlerCallback: ir\n");
            string nic_addr = ir.AsyncState as string;

            this._check_handler.EndInvoke(ir);


            //call callback with euids
            DetectResult result;
            if (resultDict.TryGetValue(nic_addr, out result))
            {
                this._call_back(nic_addr, result);
            }
        }

        private void CheckHandler(string ni)
        {
            //return all active EUIDs
            if (ni == "0.0.0.0")
            {
                //CheckAll();
            }
            else
            {
                Check(ni);
            }

            //detection finished, flush results to disc cache
            StreamWriter sw = new StreamWriter(this._result_file, false);
            this.SyncCacheWrite(sw, this._Marshal_resultDict(this.resultDict));
        }

        /// <summary>
        /// start detecting
        /// </summary>
        //private void CheckAll()
        //{
        //    Console.WriteLine("HostNetworkDetect Started...\n");
        //    try
        //    {
        //        List<string> tempList = new List<string>();
        //        tempList.Clear();
        //        result.Clear();

        //        //Check if any network connection is available, if not, for each ip address assigned
        //        //to this nic, store all dnss, then clear the adapter dict, wait for an interval time to
        //        //continue checking later
        //        if (!NetworkInterface.GetIsNetworkAvailable())
        //        {
        //            foreach (KeyValuePair<string, Adapter> kvp in adapterDict)
        //            {
        //                //get the ips assigned to this interface
        //                UnicastIPAddressInformationCollection ips =
        //                    kvp.Value.adapter.GetIPProperties().UnicastAddresses;
        //                //get dnss for this interface
        //                IPAddressCollection dnss = kvp.Value.adapter.GetIPProperties().DnsAddresses;

        //                foreach (UnicastIPAddressInformation ip in ips)
        //                {
        //                    Queue<string> dnsQueue = new Queue<string>();
        //                    foreach (IPAddress dns in dnss)
        //                        dnsQueue.Enqueue(dns.ToString());
        //                }
        //            }
        //            adapterDict.Clear();
        //            resultDict.Clear();
        //            Console.WriteLine("No NetworkInterface Available, please try later.\n");
        //            return;
        //        }

        //        //get all networkinterfaces
        //        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

        //        foreach (KeyValuePair<string, Adapter> kvp in adapterDict)
        //        {
        //            Adapter ap = kvp.Value;
        //            ap.isChecked = false;
        //        }

        //        //check all available networkinterfaces
        //        foreach (NetworkInterface adapter in adapters)
        //        {
        //            if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        //            {
        //                if (adapter.OperationalStatus == OperationalStatus.Up)
        //                {
        //                    Adapter ap;
        //                    if (!adapterDict.TryGetValue(adapter.Name, out ap))
        //                    {
        //                        ap = new Adapter(true, adapter);

        //                        //get host ip assigned to the current adapter 
        //                        UnicastIPAddressInformationCollection ips =
        //                            adapter.GetIPProperties().UnicastAddresses;
        //                        IPAddress ip = ips.First().Address;
        //                        IPAddress mask = ips.First().IPv4Mask;

        //                        IPAddressCollection dnss = adapter.GetIPProperties().DnsAddresses;
        //                        Queue<string> dnsQueue = new Queue<string>();
        //                        foreach (IPAddress dns in dnss)
        //                            dnsQueue.Enqueue(dns.ToString());

        //                        //get the first dns address
        //                        IPAddress dns1 = dnss.First();
        //                        long rtt;
        //                        //if the dns server is reachable within 500ms' timeout
        //                        Traceroute.Traceroute.PingTest(dns1.ToString(), 500, out rtt);

        //                        if (rtt < 500)
        //                        {
        //                            Queue<string> hostQueue;
        //                            //Traceroute to the current DNS server
        //                            Traceroute.Traceroute.Tracert(dnss.First().ToString(), out hostQueue);

        //                            if (IsSameSubnet(mask.ToString(), ip.ToString(), hostQueue.First()))
        //                            {
        //                                Console.WriteLine("Current Interface active!\n");
        //                                adapterDict.Add(adapter.Name, ap);
        //                                DetectResult detectResult = new DetectResult();

        //                                //not use localhost now, for test
        //                                tcpServerEP = new IPEndPoint(ip, HostDetector.tcpServerPort);

        //                                //start Checking the NAT type if there is any on the route from host to Check Server
        //                                NATCheck.NATCheck natChecker =
        //                                    new NATCheck.NATCheck(ap.adapter, StunServerEP, tcpServerEP);
        //                                natChecker.StartCheck();

        //                                //get the results of checking TCP/UDP connections from NAT Checker
        //                                detectResult.natType = natChecker.natType;
        //                                detectResult.udpResult = natChecker.udpCheckResult;
        //                                detectResult.tcpResult = natChecker.tcpCheckResult;

        //                                //get the Publicendpoint detected by the NATChecker
        //                                publicEP = natChecker.publicEP;
        //                                detectResult.publicAddress = publicEP.Address;

        //                                //if natChecker detected an address mapping, change the address of the last NAT
        //                                //on the route
        //                                if (natChecker.natType != NATTYPE.OPEN_INTERNET
        //                                    && natChecker.natType != NATTYPE.UDP_BLOCKED
        //                                    && natChecker.natType != NATTYPE.SYMUDP_FIREWALL)
        //                                    CreateRoute(publicEP, hostQueue);
        //                                else
        //                                    route = hostQueue;

        //                                //get the route from host to a DNS server with a distance of more than 1 hop
        //                                //and changed NAT address
        //                                detectResult.route = route;

        //                                //start ICMPSnooping
        //                                ICMPTest.ICMPTest icmpTest =
        //                                    new ICMPTest.ICMPTest(route);
        //                               icmpTest.Start();

        //                                //get the result of the first router that bans IPMC
        //                                detectResult.blockedAddress = icmpTest.BlockedAddress;

        //                                //add the detect result of the current interface into result list
        //                                resultDict.Add(ap.adapter.Name, detectResult);
        //                            }
        //                            else
        //                                Console.WriteLine("Other Interface active!\n");
        //                        }
        //                    }
        //                    else
        //                        ap.isChecked = true; //this nic has been detected already
        //                }
        //                else //the current networkinterface is down, remove the previous result of this nic
        //                {
        //                    if (adapterDict.ContainsKey(adapter.Name))
        //                    {
        //                        UnicastIPAddressInformationCollection ips =
        //                            adapterDict[adapter.Name].adapter.GetIPProperties().UnicastAddresses;
        //                        IPAddressCollection dnss = adapterDict[adapter.Name].adapter.GetIPProperties().DnsAddresses;

        //                        foreach (UnicastIPAddressInformation ip in ips)
        //                        {
        //                            Queue<string> dnsQueue = new Queue<string>();
        //                            foreach (IPAddress dns in dnss)
        //                            {
        //                                dnsQueue.Enqueue(dns.ToString());
        //                            }
        //                        }
        //                        adapterDict.Remove(adapter.Name);
        //                        resultDict.Remove(adapter.Name);
        //                    }
        //                }
        //            }
        //            Console.WriteLine("HostNetworkDetect Finished...\n");
        //        }// end of foreach adapter

        //        foreach (KeyValuePair<string, Adapter> kvp in adapterDict)
        //        {
        //            if (!kvp.Value.isChecked)
        //            {
        //                UnicastIPAddressInformationCollection ips =
        //                            kvp.Value.adapter.GetIPProperties().UnicastAddresses;
        //                IPAddressCollection dnss = kvp.Value.adapter.GetIPProperties().DnsAddresses;

        //                foreach (UnicastIPAddressInformation ip in ips)
        //                {
        //                    Queue<string> dnsQueue = new Queue<string>();
        //                    foreach (IPAddress dns in dnss)
        //                        dnsQueue.Enqueue(dns.ToString());
        //                }
        //                tempList.Add(kvp.Key);
        //            }
        //        }

        //        foreach (string str in tempList)
        //        {
        //            adapterDict.Remove(str);
        //            resultDict.Remove(str);
        //        }
        //        result = new Dictionary<string, DetectResult>(resultDict);
        //    }
        //    catch (Exception x)
        //    {
        //        Console.WriteLine("Error code:" + x.Message + "\n");
        //    }
        //}

        /// <summary>
        /// start detecting with given networkinterface
        /// </summary>
        /// <param name="ni"></param>
        private void Check(string nic_addr)
        {
            Console.WriteLine("HostNetworkDetect Started" + nic_addr);

            try
            {
                // Find the NIC having the given address
                NetworkInterface ni = null;
                NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface nic in nics)
                {
                    UnicastIPAddressInformationCollection nic_ips = nic.GetIPProperties().UnicastAddresses;
                    foreach (UnicastIPAddressInformation ip_info in nic_ips)
                    {
                        if (ip_info.Address.ToString() == nic_addr)
                        {
                            ni = nic;
                        }
                    }
                }

                //the given address is invalid
                if (ni == null)
                {
                    throw new Exception("HostDetector.Check(string), wrong address!");
                }

                //if the ni found here is available
                if (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        UnicastIPAddressInformationCollection ips = ni.GetIPProperties().UnicastAddresses;
                        if (ips.Count == 0)
                        {
                            throw new Exception("HostDetector.Check ips empty! " + nic_addr);
                        }

                        IPAddress ip = ips.First().Address;
                        IPAddress mask = ips.First().IPv4Mask;

                        // Get DNS servers

                        IPAddressCollection dnss = ni.GetIPProperties().DnsAddresses;

                        IPAddress dns1;
                        try
                        {
                            if (dnss.Count > 0 && !_is_experiment)
                            {
                                Queue<string> dnsQueue = new Queue<string>();
                                foreach (IPAddress dns in dnss)
                                {
                                    dnsQueue.Enqueue(dns.ToString());
                                }

                                dns1 = dnss.First();
                            }
                            else
                            {                                

#if GRADIENT
                                IPAddress[] list = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                                dns1 = list.First();
                                // For Gradient experiment
                                // pick itself as dns server
                                foreach (IPAddress _ipaddress in list)
                                {
                                    if (_ipaddress.AddressFamily == AddressFamily.InterNetwork && 
                                        (_ipaddress.ToString().Contains("10.0.") || (_ipaddress.ToString().Contains("10.1."))))
                                    {
                                        dns1 = _ipaddress;
                                        break;
                                    }
                                }
#else
                                string dnsname = "cs16";
                                IPAddress[] addrlist = Dns.GetHostAddresses(dnsname);

                                if (addrlist.Count() > 0)
                                {
                                    dns1 = addrlist.First();
                                }
                                else
                                {
                                    throw new Exception("HostDetector.Chekc unable to resolve dns address!");
                                }
                                //dns1 = IPAddress.Parse("10.1.4.3");
#endif
                            }
                        }
                        catch (Exception exc)
                        {
                            throw new Exception("HostDetector.Check dns" + exc.Message);
                        }
                        //try another traceroute destination, like google.com
                        //IPHostEntry tracedstEt = Dns.Resolve("google.com");
                        //IPAddress tracedst = tracedstEt.AddressList[0];
                        Console.WriteLine("DNS set" + dns1.ToString());

                        // ping the target dns server
                        long rtt;

#if GRADIENT
                        rtt = 1;
#else
                        try
                        {
                            Traceroute.Traceroute.PingTest(dns1.ToString(), 500, out rtt);
                        }
                        catch (Exception exc)
                        {
                            throw new Exception("HostDetector.Check tracert" + exc.Message);
                        }
#endif

                        //if the dns server is reachable within 500ms' timeout
                        if (rtt < 500)
                        {
                            Queue<string> hostQueue;
#if GRADIENT
                            hostQueue = new Queue<string>();
                            hostQueue.Enqueue(dns1.ToString());
#else
                            
                            //Traceroute to the current DNS server
                            Traceroute.Traceroute.Tracert(dns1.ToString(), out hostQueue);
#endif
                            DetectResult detectResult;
                            bool detect = false;
                            if (this.resultDict.TryGetValue(nic_addr, out detectResult))// this nic has been detected before
                            {
                                foreach (string routeaddr in hostQueue)
                                {
                                    if (!detectResult.route.Contains(routeaddr))// though nic_addr is the same, but in different environment
                                    {
                                        detect = true;
                                        this.resultDict.Remove(nic_addr);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                detect = true;
                            }

                            if (detect) //should detect
                            {
                                detectResult = new DetectResult();
                                //start Checking the NAT type if there is any on the route from host to Check Server

                                NATCheck.NATCheck natChecker = new NATCheck.NATCheck(ni); 
                                if (!_is_experiment)
                                {
                                    natChecker.StartCheck();
                                }
                                else
                                {
                                    // experiment, use the default value
                                }

                                //get the results of checking TCP/UDP connections from NAT Checker
                                detectResult.natType = natChecker.natType;
                                detectResult.udpResult = natChecker.udpCheckResult;
                                detectResult.tcpResult = natChecker.tcpCheckResult;
                                //get the Publicendpoint detected by the NATChecker
                                publicEP = natChecker.publicEP;
                                detectResult.publicAddress = publicEP.Address;
                                //if natChecker detected an address mapping, change the address of the last NAT
                                //on the route
                                if (natChecker.natType != NATTYPE.OPEN_INTERNET
                                    && natChecker.natType != NATTYPE.UDP_BLOCKED
                                    && natChecker.natType != NATTYPE.SYMUDP_FIREWALL)
                                    CreateRoute(publicEP, hostQueue);
                                else
                                    route = hostQueue;

                                //get the route from host to a DNS server with a distance of more than 1 hop
                                //and changed NAT address
                                detectResult.route = route;

                                //start ICMPSnooping
                                //UnicastIPAddressInformationCollection ips = ni.GetIPProperties().UnicastAddresses;
                                ICMPTest.ICMPTest icmpTest =
                                    new ICMPTest.ICMPTest(route, ips[0].Address);

                                if (_is_experiment)
                                {
                                    try
                                    {
#if GRADIENT
                                        string id = System.Net.Dns.GetHostName();

                                        StreamReader map = new StreamReader("c:/map.txt");
                                        string line;
                                        Dictionary<string, string> map_dict = new Dictionary<string, string>();
                                        while (null != (line = map.ReadLine()))
                                        {
                                            char[] set = { ' ' };
                                            string[] elems = line.Split(set, StringSplitOptions.RemoveEmptyEntries);

                                            map_dict.Add(elems[3], elems[0]);
                                        }
                                        map.Close();

                                        try
                                        {
                                            while (route.Count > 0)
                                            {
                                                route.Dequeue();
                                            }

                                            string replace;
                                            if (!map_dict.TryGetValue(id, out replace))
                                            {
                                                replace = id;
                                            }
                                            
                                            route.Enqueue(replace);

                                            // Set as local ip
                                            detectResult.blockedAddress = dns1;
                                        }
                                        catch (Exception exc)
                                        {
                                            throw new Exception("Quilt.PubsubApp.PubSubClient.Constructor " + exc);
                                        }

#else
                                        //read local file
                                        StreamReader hosts = new StreamReader("c:\\hosts");
                                        string line;
                                        Dictionary<string, string> map = new Dictionary<string, string>();
                                        while (null != (line = hosts.ReadLine()))
                                        {
                                            string[] elems = line.Split(' ', '\t');
                                            map.Add(elems[0], elems[1].Split('-')[0]);
                                        }

                                        int num = route.Count;
                                        for (int i = 0; i < num; i++)
                                        {
                                            string name = map[route.Dequeue()];
                                            //if (i < (num - 1))
                                            {
                                                route.Enqueue(name);
                                            }
                                        }
                                        detectResult.route = route;

                                        detectResult.index = 0;
                                        foreach (string rt in route)
                                        {
                                            if (rt.Contains("ipmc") || rt.Contains("dns"))
                                            {
                                                detectResult.index++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }

                                        hosts.Close();
#endif
                                    }
                                    catch (Exception exc)
                                    {
                                        throw new Exception("HostDetector.Check translate" + exc.Message);
                                    }
                                }
                                else
                                {
                                    icmpTest.Start();

                                    //get the result of the first router that bans IPMC
                                    detectResult.blockedAddress = icmpTest.BlockedAddress;
                                    detectResult.index = icmpTest.Index;
                                }

                                //add the detect result of the current interface into result list
                                this.resultDict.Add(nic_addr, detectResult);
                                
                            }
                            else
                            {
                                if (!this.resultDict.ContainsKey(nic_addr))
                                {
                                    throw new Exception("No result record!");
                                }
                            }
                        }
                        else
                        {
                            //remove outdated result since the nic is not active currently
                            this.resultDict.Remove(nic_addr);
                        }
                    }
                    // If the nic is not active
                    else
                    {
                        //remove outdated result since the nic is not active currently
                        this.resultDict.Remove(nic_addr);
                    }
                }
                else
                {
                    throw new Exception("HostDetector.Check(string), loopback address!");
                }

                Console.WriteLine("HostNetworkDetect (with NetworkInterface) Finished...\n");
            }
            catch (Exception x)
            {
                //Console.WriteLine("Error code:" + x.Message + "\n");
                throw new Exception(x.Message);
            }
        }

        private string _Marshal_resultDict(Dictionary<string, DetectResult> dict)
        {
            string s = "";
            DetectResult dr;
            // marshal structure:
            // @nic_addr1|index|udpResult|tcpResult|natType|publicAddress|blockedAddress|routeaddr1$routeaddr2$...routeaddri$
            // @nic_addr2...
            foreach (string nic in dict.Keys)
            {
                if (!dict.TryGetValue(nic, out dr))
                {
                    throw new Exception("empty result!\n");
                }
                else
                {
                    s += "@" + nic + "|" + dr.index.ToString() + "|"
                           + ((int)dr.udpResult).ToString() + "|" + ((int)dr.tcpResult).ToString() + "|"
                           + ((int)dr.natType).ToString() + "|" + dr.publicAddress.ToString() + "|"
                           + dr.blockedAddress.ToString() + "|";
                    foreach (string addr in dr.route.ToArray())
                    {
                        if (addr != "")
                        {
                            s += "$" + addr;
                        }
                    }
                }
            }
            return s;
        }

        private void _Unmarshal_resultDict(string s)
        {
            string[] nics = s.Split('@');
            foreach (string nic in nics)
            {
                if (nic != "")
                {
                    string[] fields = nic.Split('|');
                    DetectResult dr = new DetectResult();
                    dr.index = Int32.Parse(fields[1]);
                    dr.udpResult = (Direction.DIRECTION)Int32.Parse(fields[2]);
                    dr.tcpResult = (Direction.DIRECTION)Int32.Parse(fields[3]);
                    dr.natType = (NATTYPE)Int32.Parse(fields[4]);
                    dr.publicAddress = IPAddress.Parse(fields[5]);
                    dr.blockedAddress = IPAddress.Parse(fields[6]);
                    string[] addrs = fields[7].Split('$');
                    dr.route = new Queue<string>();
                    foreach (string addr in addrs)
                    {
                        if (addr != "")
                        {
                            dr.route.Enqueue(addr);
                        }
                    }
                    this.resultDict.Add(fields[0], dr);
                }
            }
        }

        /// <summary>
        /// convert ip from string to number
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>return the number : type int</returns>
        private int GetIPNum(string ip)
        {
            int num = 0;
            string[] ips = ip.Split('.');
            for (int i = 0; i < ips.GetLength(0); i++)
                num = (num + Int32.Parse(ips[i])) * 256;
            return num;
        }

        /// <summary>
        /// check if the IP is public 
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns>return type : boolean</returns>
        private bool IsPublicIP(string ip)
        {
            //keep the private ip ranges
            int aBegin = GetIPNum("10.0.0.0");
            int aEnd = GetIPNum("10.255.255.255");
            int bBegin = GetIPNum("172.16.0.0");
            int bEnd = GetIPNum("172.31.255.255");
            int cBegin = GetIPNum("192.168.0.0");
            int cEnd = GetIPNum("192.168.255.255");
            int num = GetIPNum(ip);
            if ((num >= aBegin && num <= aEnd)
                || (num >= bBegin && num <= bEnd)
                || (num >= cBegin && num <= cEnd))
                return false;
            return true;
        }

        /// <summary>
        /// modify the route, change back the Publicaddress by the last NAT on the route
        /// </summary>
        /// <param name="publicEP">the endpoint containing the public address of the last NAT</param>
        /// <param name="hostQueue">the result of traceroute</param>
        private void CreateRoute(IPEndPoint publicEP, Queue<string> hostQueue)
        {
            try
            {
                string tmp = null;
                route = new Queue<string>();
                //get the enumerator of hostQueue
                Queue<string>.Enumerator en = hostQueue.GetEnumerator();

                if (!en.MoveNext())
                {
                    Console.WriteLine("Empty hostQueue!...\n");
                    return;
                }
                tmp = en.Current;
                while (en.MoveNext())
                {
                    if (!IsPublicIP(tmp) && IsPublicIP(en.Current))
                    {
                        route.Enqueue(publicEP.Address.ToString());
                        do
                            route.Enqueue(en.Current);
                        while (en.MoveNext());
                        break;
                    }
                    else
                    {
                        route.Enqueue(tmp);
                        tmp = en.Current;
                    }
                }

                if (IsPublicIP(tmp))
                    route.Enqueue(tmp);
                else
                    route.Enqueue(publicEP.Address.ToString());
            }
            catch (Exception x)
            {
                Console.WriteLine("CreateRoute Error Code: " + x.Message + "\n");
            }
        }

        /// <summary>
        /// check if the two addresses are in the same subnet
        /// </summary>
        /// <param name="hostSubnet">host subnet</param>
        /// <param name="hostAddr">host address</param>
        /// <param name="anotherAddr">the comparing address</param>
        /// <returns>return type : boolean</returns>
        /*private bool IsSameSubnet(string hostSubnet, string hostAddr, string anotherAddr)
        {
            string[] subnets = hostSubnet.Split('.');
            string[] hosts = hostAddr.Split('.');
            string[] anothers = anotherAddr.Split('.');

            for (int pos = 0; pos < 4; pos++)
            {
                int sub = int.Parse(subnets[pos]);
                int host = int.Parse(hosts[pos]);
                int another = int.Parse(anothers[pos]);

                if ((sub & host) != (sub & another))
                    return false;
            }
            return true;
        }*/

        #endregion


    }
}
