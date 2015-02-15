/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.Collections;

namespace Quilt.Core
{
    public class BloomFilter
    {
        private BitArray hashbits;
        private int numKeys;
        private int[] hashKeys;

        public BloomFilter(int tableSize, int nKeys)
        {
            numKeys = nKeys;
            hashKeys = new int[numKeys];
            hashbits = new BitArray(tableSize);
        }

        private int HashString(string s)
        {
            int hash = 0;

            for (int i = 0; i < s.Length; i++)
            {
                hash += s[i];
                hash += (hash << 10);
                hash ^= (hash >> 6);
            }
            hash += (hash << 3);
            hash ^= (hash >> 11);
            hash += (hash << 15);
            return hash;
        }

        private void CreateHashes(string str)
        {
            int hash1 = str.GetHashCode();
            int hash2 = HashString(str);

            hashKeys[0] = Math.Abs(hash1 % hashbits.Count);
            if (numKeys > 1)
            {
                for (int i = 1; i < numKeys; i++)
                {
                    hashKeys[i] = Math.Abs((hash1 + (i * hash2))
                        % hashbits.Count);
                }
            }
        }

        public bool Test(string str)
        {
            CreateHashes(str);
            // Test each hash key.  Return false if any 
            //  one of the bits is not set.
            foreach (int hash in hashKeys)
            {
                if (!hashbits[hash])
                    return false;
            }
            // All bits set.  The item is there.
            return true;
        }

        public bool Add(string str)
        {
            // Initially assume that the item is in the table
            bool rslt = true;
            CreateHashes(str);
            foreach (int hash in hashKeys)
            {
                if (!hashbits[hash])
                {
                    // One of the bits wasn't set, so show that
                    // the item wasn't in the table, and set that bit.
                    rslt = false;
                    hashbits[hash] = true;
                }
            }
            return rslt;
        }
    }
}