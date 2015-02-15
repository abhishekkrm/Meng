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

namespace Quilt.HostDetector.ICMPTest
{
    public class ICMPTest
    {
        #region private fields

        private MyRouter[] _route;
        public IPAddress _groupAddress = null;
        private IPAddress _localAddress = null;
        private IPAddress _blockAddress = null;
        // index pointing to _blockAddress
        private int _index = 0;

        #endregion

        #region public fields

        public class MyRouter
        {
            public MyRouter(string addr, bool opened)
            {
                this._address = addr;
                this._opened = opened;
            }

            public string _address = null;
            public bool _opened = false;
        }

        #endregion

        #region static public fields

        static public int _RECVTT = 5000; // socket receive timeout: 5s

        static public int _TRY = 4;

        #endregion

        #region Constructor

        public ICMPTest(Queue<string> hostQueue, IPAddress localAddr)
        {
            try
            {
                //all routers supporting PIM will join this group
                this._groupAddress = IPAddress.Parse("224.0.0.13");

                this._localAddress = localAddr;

                // deep copy
                if (hostQueue != null)
                {
                    Queue<string> routequeue = new Queue<string>(hostQueue);
                    int i = 0;
                    //set the blockaddress to be the first router as default
                    if (routequeue.Count > 0)
                    {
                        this._blockAddress = IPAddress.Parse(routequeue.Peek());
                    }
                    
                    this._route = new MyRouter[routequeue.Count];
                    //copy the hostqueue to route list
                    foreach (string temp in routequeue)
                    {
                        this._route[i++] = new MyRouter(temp, false);
                    }
                }
                else
                {
                    throw new Exception("hostQueue is empty!");
                }
            }
            catch (Exception exc)
            {
                throw new Exception("ICMPTest: ICMPTest Exception " + exc.Message);
            }
        }

        #endregion

        #region Start

        public void Start()
        {
            try
            {
                long rtt = 0;
                Dictionary<string, bool> returnDict;
                Traceroute.Traceroute.PingAdvance(this._localAddress.ToString(), _groupAddress.ToString(), 5000, out rtt, out returnDict);

                foreach (MyRouter router in _route)
                {
                    if (returnDict.ContainsKey(router._address))
                    {
                        router._opened = true;
                    }
                }
            }
            catch(Exception x)
            {
                Console.WriteLine("Error code: " + x.Message);
            }

            // find the first router that doesn't support IPMC on the path
            for (int i = 0; i < this._route.Length; i++)
            {
                if (this._route[i]._opened == false)
                {
                    this._blockAddress = IPAddress.Parse(this._route[i]._address);
                    this._index = i;
                    break;
                }
            }
        }

        #endregion

        #region Public Attributes

        public IPAddress BlockedAddress
        {
            get { return this._blockAddress; }
        }

        public int Index
        {
            get { return this._index; }
        }

        #endregion
    }
}
