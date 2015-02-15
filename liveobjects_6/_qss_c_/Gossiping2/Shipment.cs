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

namespace QS._qss_c_.Gossiping2
{
    public interface IShipment : QS.Fx.Serialization.ISerializable
    {
        void Add(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.Message message);
        void Add(IShipment otherShipment);
        
        IEnumerable<QS._core_c_.Base3.InstanceID> Sources
        {
            get;
        }

        IEnumerable<QS._core_c_.Base3.Message> this[QS._core_c_.Base3.InstanceID sourceAddress]
        {
            get;
        }
    }

    [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_Shipment)]
    public class Shipment : Components_2_.SerializableDictOf<QS._core_c_.Base3.InstanceID, Components_2_.MessageCollection>, IShipment
    {
        public Shipment()
        {
        }

        #region IShipment members

        void IShipment.Add(QS._core_c_.Base3.InstanceID senderAddress, QS._core_c_.Base3.Message message)
        {
            this.GetCreate(senderAddress).Add(message);
        }

        void IShipment.Add(IShipment otherShipment)
        {
            foreach (QS._core_c_.Base3.InstanceID sourceAddress in otherShipment.Sources)
                GetCreate(sourceAddress).Add(otherShipment[sourceAddress]);
        }

        IEnumerable<QS._core_c_.Base3.InstanceID> IShipment.Sources
        {
            get { return this.Dictionary.Keys; }
        }

        IEnumerable<QS._core_c_.Base3.Message> IShipment.this[QS._core_c_.Base3.InstanceID senderAddress]
        {
            get { return this[senderAddress]; }
        }

        #endregion

        public override ClassID ClassID
        {
            get { return ClassID.Gossiping2_Shipment; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            bool separate = false;
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Components_2_.MessageCollection> element in this.Dictionary)
            {
                if (separate)
                    s.Append(", ");
                else
                    separate = true;

                s.Append("From ");
                s.Append(element.Key.ToString());
                s.Append(" : { ");
                s.Append(element.Value.ToString());
                s.Append(" }");
            }
            return s.ToString();
        }
    }
}
