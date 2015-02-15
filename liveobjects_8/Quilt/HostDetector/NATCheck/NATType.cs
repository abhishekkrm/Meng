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

namespace Quilt.HostDetector.NATCheck
{
    public enum NATTYPE
    {
        /// <summary>
        /// UDP is always blocked.
        /// </summary>
        UDP_BLOCKED,
        /// <summary>
        /// No NAT, public IP, no firewall.
        /// </summary>
        OPEN_INTERNET,
        /// <summary>
        /// No NAT, public IP, but symmetric UDP firewall.
        /// </summary>
        SYMUDP_FIREWALL,
        /// <summary>
        /// A full cone NAT is one where all requests from the same internal IP address and port are 
        /// mapped to the same external IP address and port. Furthermore, any external host can send 
        /// a packet to the internal host, by sending a packet to the mapped external address.
        /// </summary>
        FULL_CONE,
        /// <summary>
        /// A restricted cone NAT is one where all requests from the same internal IP address and 
        /// port are mapped to the same external IP address and port. Unlike a full cone NAT, an external
        /// host (with IP address X) can send a packet to the internal host only if the internal host 
        /// had previously sent a packet to IP address X.
        /// </summary>
        RESTRICTED_CONE,
        /// <summary>
        /// A port restricted cone NAT is like a restricted cone NAT, but the restriction 
        /// includes port numbers. Specifically, an external host can send a packet, with source IP
        /// address X and source port P, to the internal host only if the internal host had previously 
        /// sent a packet to IP address X and port P.
        /// </summary>
        PORT_RESTRICTED_CONE,
        /// <summary>
        /// A symmetric NAT is one where all requests from the same internal IP address and port, 
        /// to a specific destination IP address and port, are mapped to the same external IP address and
        /// port.  If the same host sends a packet with the same source address and port, but to 
        /// a different destination, a different mapping is used. Furthermore, only the external host that
        /// receives a packet can send a UDP packet back to the internal host.
        /// </summary>
        SYMMETRIC
    }
}
