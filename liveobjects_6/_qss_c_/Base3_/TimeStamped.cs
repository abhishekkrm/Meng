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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    public abstract class TimeStamped<Class> : Base3_.KnownClass, QS.Fx.Serialization.ISerializable where Class : QS.Fx.Serialization.ISerializable, new()
    {
        public TimeStamped()
        {
        }

        public TimeStamped(System.DateTime timeStamp, Class dataObject)
        {
            this.timeStamp = timeStamp;
            this.dataObject = dataObject;
        }

        private System.DateTime timeStamp;
        private Class dataObject;

        public System.DateTime TimeStamp
        {
            get { return timeStamp; }
        }

        public Class Object
        {
            get { return dataObject; }
        }

        #region QS.Fx.Serialization.ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return dataObject.SerializableInfo.Extend((ushort) this.ClassID, sizeof(long), 0, 0); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
            {
                *((long*)(arrayptr + header.Offset)) = timeStamp.Ticks;
            }
            header.consume(sizeof(long));
            dataObject.SerializeTo(ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                timeStamp = new DateTime(*((long*)(arrayptr + header.Offset)));
            }
            header.consume(sizeof(long));
            dataObject = new Class();
            dataObject.DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
