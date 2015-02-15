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

namespace QS._qss_c_.Rings6
{
/*
    public class ForwardingBuckets : IForwardingBuckets
    {
        public ForwardingBuckets(Base3.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3.IReliableSerializableSender> senderCollection)
        {
            this.senderCollection = senderCollection;
        }

        private Base3.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3.IReliableSerializableSender> senderCollection;
        private uint epoch, ttl;
        private Bucket[] buckets;

        #region Class Bucket

        private class Bucket
        {
            public Bucket(ForwardingBuckets owner)
            {
                this.owner = owner;
            }

            private ForwardingBuckets owner;

        }

        #endregion

        #region IForwardingBuckets Members

        uint IForwardingBuckets.Epoch
        {
            get { return epoch; }
            set
            {
                if (ttl == 0)
                    throw new Exception("Cannot advance the epoch unless TTL > 0.");

                while (epoch < value)
                {
                    if (epoch >= ttl)
                    {
                        uint min_epoch = epoch + 1 - ttl;

                    }
                    
                }
            }
        }

        uint IForwardingBuckets.TTL
        {
            get { return ttl; }
            set
            {
                if (value < 1)
                    throw new Exception("Time to live value less than 1 not supported.");

                if (value > ttl)
                {
                    Bucket[] new_buckets = new Bucket[value];
                    if (buckets != null && ttl > 0)
                    {
                        uint min_epoch = (epoch >= ttl) ? (epoch + 1 - ttl) : 1;
                        for (uint ind = min_epoch; ind <= epoch; ind++)
                            new_buckets[(int)(ind % value)] = buckets[(int)(ind % ttl)];
                    }
                    buckets = new_buckets;
                    ttl = value;
                }
                else
                {
                    throw new NotSupportedException("For now, we can only increase this value.");
                }
            }
        }

        void IForwardingBuckets.Schedule(uint sequenceNo, QS.CMS.QS._core_c_.Base3.InstanceID destinationAddress)
        {
            if (epoch == 0)
                throw new Exception("Cannot schedule anything in epoch 0.");

            // ................
        }

        #endregion
    }
*/ 
}
