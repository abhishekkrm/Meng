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

namespace QS._qss_c_.FailureDetection_
{
    public enum Action : byte
    {
        STARTED, CRASHED
    }

    #region Class Change

    [QS.Fx.Serialization.ClassID(ClassID.FailureDetection_Change)]
    public class Change : QS.Fx.Serialization.ISerializable
    {
        public Change()
        {
        }

        public Change(QS._core_c_.Base3.InstanceID instanceID, Action action)
        {
            this.instanceID = instanceID;
            this.action = action;
        }

        private QS._core_c_.Base3.InstanceID instanceID;
        private Action action;

        public QS._core_c_.Base3.InstanceID InstanceID
        {
            get { return instanceID; }
        }

        public Action Action
        {
            get { return action; }
        }

        public override string ToString()
        {
            return action.ToString() + ":" + instanceID.ToString();
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return instanceID.SerializableInfo.Extend((ushort) ClassID.FailureDetection_Change, (ushort) sizeof(byte), 0, 0); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            instanceID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                *(arrayptr + header.Offset) = (byte) action;
            }
            header.consume(sizeof(byte));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            instanceID = new QS._core_c_.Base3.InstanceID();
            instanceID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                action = (Action) (*(arrayptr + header.Offset));
            }
            header.consume(sizeof(byte));
        }

        #endregion
    }

    #endregion
}
