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

namespace QS._qss_c_.Base6_
{
    public struct Int32x2
    {
        public Int32x2(int int1, int int2)
        {
            this.int1 = int1;
            this.int2 = int2;
        }

        public Int32x2(long value)
        {
            unsafe
            {
                int* pvalue = (int*)(&value);
                this.int2 = *pvalue;
                this.int1 = *(pvalue + 1);
            }
        }

        private int int1, int2;

        public static explicit operator long(Int32x2 obj)
        {
            long value;
            unsafe
            {
                int* pvalue = (int*)(&value);
                *pvalue = obj.int2;
                *(pvalue + 1) = obj.int1;
            }            
            return value;
        }

        public static explicit operator Int32x2(long value)
        {
            return new Int32x2(value);
        }

        public int Int1
        {
            get { return int1; }
            set { int1 = value; }
        }	
        
        public int Int2
        {   
            get { return int2; }
            set { int2 = value; }
        }

        public override string ToString()
        {
            return int1.ToString() + ":" + int2.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is Int32x2)
            {
                Int32x2 other = (Int32x2) obj;
                return int1 == other.int1 && int2 == other.int2;
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return int1 ^ int2;
        }
    }
}
