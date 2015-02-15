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
using System.Net;

namespace QS._qss_x_.Administrator_
{
/*
    public sealed class Subnet
    {
        public Subnet(string s)
        {
            string[] _parts = s.Split('.');
            if (_parts.Length != 4)
                throw new Exception("Cannot parse subnet \"" + s + "\".");
            from = new byte[4];
            to = new byte[4];
            for (int k = 0; k < 4; k++)
            {
                if (_parts[k].Equals("*"))
                {
                    from[k] = 0;
                    to[k] = 255;
                }
                else
                {
                    int m = _parts[k].IndexOf('-');
                    if (m >= 0 && m < _parts[k].Length)
                    {
                        from[k] = Convert.ToByte(_parts[k].Substring(0, m));
                        to[k] = Convert.ToByte(_parts[k].Substring(m + 1));
                    }
                    else
                    {
                        from[k] = Convert.ToByte(_parts[k]);
                        to[k] = from[k];
                    }
                }
            }
        }

        private byte[] from, to;

        public bool Contains(IPAddress a)
        {
            byte[] b = a.GetAddressBytes();
            if (b.Length != 4)
                return false;
            for (int k = 0; k < 4; k++)
            {
                if (b[k] < from[k] || b[k] > to[k])
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            bool isfirst = true;
            for (int k = 0; k < 4; k++)
            {
                if (isfirst)
                    isfirst = false;
                else
                    s.Append(".");
                if (from[k] == to[k])
                    s.Append(from[k].ToString());
                else if (from[k] == 0 && to[k] == 255)
                    s.Append("*");
                else
                {
                    s.Append(from[k].ToString());
                    s.Append("-");
                    s.Append(to[k].ToString());
                }
            }
            return s.ToString();
        }
    }
*/ 
}
