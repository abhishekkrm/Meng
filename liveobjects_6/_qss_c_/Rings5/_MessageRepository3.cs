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

namespace QS._qss_c_.Rings5
{
/*
    public class _MessageRepository3 : IMessageRepository
    {
        private const uint DefaultCapacity = 5;

        public _MessageRepository3()
        {
            capacity = DefaultCapacity;
            messages = new Nullable<Base3.Message>[capacity];
        }

        private uint minimum = 1, maximum = 0, capacity;
        private Nullable<Base3.Message>[] messages;

        #region Internal Processing

        void IncreaseCapacity(uint desiredCapacity)
        {
            Nullable<Base3.Message>[] newMessages = new Nullable<Base3.Message>[desiredCapacity];
            for (uint seqno = minimum; seqno <= maximum; seqno++)
                newMessages[seqno % desiredCapacity] = messages[seqno % capacity];
            messages = newMessages;
            capacity = desiredCapacity;
        }

        void AdjustMaximum(uint sequenceNo)
        {
            uint desiredCapacity = sequenceNo - minimum + 1;
            if (desiredCapacity > capacity)
                IncreaseCapacity((uint) Math.Ceiling(((double) desiredCapacity) * 1.5));

            while (maximum < sequenceNo)
            {
                maximum++;
                messages[maximum % capacity] = null;
            }
        }

        #endregion

        #region IMessageRepository Members

        bool IMessageRepository.Add(uint sequenceNo, QS.CMS.Base3.Message message)
        {
            if (sequenceNo < minimum)
                return false;

            if (sequenceNo > maximum)
                AdjustMaximum(sequenceNo);

            if (messages[sequenceNo % capacity].HasValue)
                return false;
            else
            {
                messages[sequenceNo % capacity] = message;
                return true;
            }
        }

        uint IMessageRepository.MaximumSeen
        {
            get { return maximum; }            
            set { AdjustMaximum(value); }
        }

        uint IMessageRepository.MaximumClean
        {
            get { return minimum - 1; }

            set
            {
                uint new_minimum = value + 1;
                while (minimum < new_minimum)
                {
                    if (!messages[minimum % capacity].HasValue)
                        throw new Exception("Cannot cleanup message " + minimum.ToString() + ", message not in the repository.");
                    messages[minimum % capacity] = null;
                    minimum++;
                }
            }
        }

        Nullable<Base3.Message> IMessageRepository.Get(uint sequenceNo)
        {
            if (sequenceNo >= minimum && sequenceNo <= maximum && messages[sequenceNo % capacity].HasValue)
                return messages[sequenceNo % capacity].Value;
            else
                return null;
        }

        #endregion
    }
*/ 
}
