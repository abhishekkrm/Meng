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

namespace QS._qss_c_._OldFx.Services.Base
{
    public static class Addressing
    {
        public const string Scheme = "quicksilver";

        public static Uri EncodeUri(QS._core_c_.Base3.InstanceID address, string path)
        {
            return new UriBuilder(Scheme, address.Address.HostIPAddress.ToString(), address.Address.PortNumber, 
                "/" + address.Incarnation.SeqNo.ToString() + "/" + path).Uri;
        }

        public static void DecodeUri(Uri uri, out QS._core_c_.Base3.InstanceID address, out string path)
        {
            DecodeUri(uri, null, out address, out path);
        }

        public static void DecodeUri(Uri uri, QS._qss_c_.Base1_.Subnet[] subnets, out QS._core_c_.Base3.InstanceID address, out string path)
        {
            System.Net.IPAddress ipAddress;
            if (!System.Net.IPAddress.TryParse(uri.Host, out ipAddress))
            {
                System.Net.IPAddress[] addresses = System.Net.Dns.GetHostAddresses(uri.Host);
                if (subnets == null)
                {
                    if (addresses.Length != 1)
                        throw new ArgumentException();
                    else
                        ipAddress = addresses[0];
                }
                else
                {
                    bool found = false;
                    foreach (System.Net.IPAddress candidate_address in addresses)
                    {
                        foreach (QS._qss_c_.Base1_.Subnet subnet in subnets)
                        {
                            if (subnet.contains(candidate_address))
                            {
                                found = true;
                                ipAddress = candidate_address;
                                break;
                            }                            
                        }
                        if (found)
                            break;
                    }
                    if (!found)
                        throw new ArgumentException();
                }
            }
            if (uri.AbsolutePath[0] != '/')
                throw new ArgumentException();
            int separator_position = uri.AbsolutePath.IndexOf('/', 1);
            if (separator_position < 0 || separator_position >= uri.AbsolutePath.Length)
                throw new ArgumentException();
            if (uri.AbsolutePath.IndexOf('/', separator_position + 1) > 0)
                throw new ArgumentException();
            address = new QS._core_c_.Base3.InstanceID(new QS.Fx.Network.NetworkAddress(ipAddress, uri.Port), 
                Convert.ToUInt32(uri.AbsolutePath.Substring(1, separator_position - 1)));
            path = uri.AbsolutePath.Substring(separator_position + 1);
        }
    }
}
