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
    public interface IShipmentCollection : QS.Fx.Serialization.ISerializable
    {
        void Add(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.InstanceID destinationAddress, QS._core_c_.Base3.Message message);
        void Add(IShipmentCollection otherCollection);
        void Add(QS._core_c_.Base3.InstanceID destinationAddress, IShipment shipment);

        IEnumerable<QS._core_c_.Base3.InstanceID> Destinations
        {
            get;
        }

        IShipment this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get;
        }

        void Clear();

        IShipment Remove(QS._core_c_.Base3.InstanceID destinationAddress);
    }

    [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_ShipmentCollection)]
    public class ShipmentCollection : Components_2_.SerializableDictOf<QS._core_c_.Base3.InstanceID, Shipment>, IShipmentCollection
    {
        public ShipmentCollection() 
        {
        }

        #region IShipmentCollection Members

        void IShipmentCollection.Add(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.InstanceID destinationAddress, QS._core_c_.Base3.Message message)
        {
            ((IShipment)GetCreate(destinationAddress)).Add(sourceAddress, message);
        }

        void IShipmentCollection.Add(IShipmentCollection otherCollection)
        {
            foreach (QS._core_c_.Base3.InstanceID destinationAddress in otherCollection.Destinations)
                ((IShipmentCollection) this).Add(destinationAddress, otherCollection[destinationAddress]);
        }

        void IShipmentCollection.Add(QS._core_c_.Base3.InstanceID destinationAddress, IShipment shipment)
        {
            ((IShipment)GetCreate(destinationAddress)).Add(shipment);
        }

        IEnumerable<QS._core_c_.Base3.InstanceID> IShipmentCollection.Destinations
        {
            get { return this.Dictionary.Keys; }
        }

        IShipment IShipmentCollection.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return this[destinationAddress]; }
        }

        IShipment IShipmentCollection.Remove(QS._core_c_.Base3.InstanceID destinationAddress)
        {
            return base.Remove(destinationAddress);
        }

        #endregion

        public override ClassID ClassID
        {
            get { return ClassID.Gossiping2_ShipmentCollection; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            bool separate = false;
            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Shipment> element in this.Dictionary)
            {
                if (separate)
                    s.Append(", ");
                else
                    separate = true;

                s.Append("To ");
                s.Append(element.Key.ToString());
                s.Append(" : { ");
                s.Append(element.Value.ToString());
                s.Append(" }");
            }
            return s.ToString();
        }
    }
}
