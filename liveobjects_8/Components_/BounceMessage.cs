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
using System.Linq;
using System.Text;

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("C6BF790B5F184f6688E1B20FB34255CC", "BounceMessage")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.BounceMessage)]
    public sealed class BounceMessage
        : QS.Fx.Serialization.ISerializable
    {
        #region Type

        public enum Type
        {
            TEST,
            BOUNCE,
            ACTIVE,
            PASSIVE,
            PUNCH,
            ALIVE
        }

        #endregion

        #region Constructor

        public BounceMessage(Type type, QS.Fx.Value.STUNAddress addr)
        {
            this.type = type;
            this.addr = addr;
            this.target_addr = addr;
        }

        public BounceMessage(Type type, QS.Fx.Value.STUNAddress addr, QS.Fx.Value.STUNAddress target_addr)
        {
            this.type = type;
            this.addr = addr;
            this.target_addr = target_addr;
        }

        public BounceMessage()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Value.STUNAddress addr;
        private QS.Fx.Value.STUNAddress target_addr;
        private Type type;

        #endregion

        #region Acess

        public Type TYPE
        {
            get { return this.type; }
        }

        public QS.Fx.Value.STUNAddress Addr
        {
            get { return this.addr; }
        }

        public QS.Fx.Value.STUNAddress TargetAddr
        {
            get { return this.target_addr; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.BounceMessage), sizeof(Type));
                info.AddAnother(((QS.Fx.Serialization.ISerializable)addr).SerializableInfo);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)target_addr).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader_0 = header.Array)
            {
                byte* _pheader = _pheader_0 + header.Offset;
                *((Type*)_pheader) = this.type;
            }
            //???Need fix this code???
            header.consume(sizeof(Type));
            if (this.addr != null)
                ((QS.Fx.Serialization.ISerializable)addr).SerializeTo(ref header, ref data);
            if (this.target_addr != null)
                ((QS.Fx.Serialization.ISerializable)target_addr).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader_0 = header.Array)
            {
                byte* _pheader = _pheader_0 + header.Offset;
                this.type = *((Type*)_pheader);
            }
            header.consume(sizeof(Type));
            this.addr = new STUNAddress();
            ((QS.Fx.Serialization.ISerializable)addr).DeserializeFrom(ref header, ref data);
            this.target_addr = new STUNAddress();
            ((QS.Fx.Serialization.ISerializable)target_addr).DeserializeFrom(ref header, ref data);
        }

        #endregion
    }
}
