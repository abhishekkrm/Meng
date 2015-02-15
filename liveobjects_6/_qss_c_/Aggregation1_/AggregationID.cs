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

// #define Using_VS2005Beta1

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation1_
{
    [QS.Fx.Serialization.ClassID(ClassID.AggregationID)]
    public class AggregationID : System.IComparable<AggregationID>, QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable
    {
        public AggregationID()
        {
        }

        public AggregationID(IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress)
        {
            this.aggregationKey = aggregationKey;
            this.rootAddress = rootAddress;
        }

        private IAggregationKey aggregationKey;
        private QS._core_c_.Base3.InstanceID rootAddress;

        #region Accessors

        public IAggregationKey AggregationKey
        {
            get { return aggregationKey; }
        }

        public QS._core_c_.Base3.InstanceID RootAddress
        {
            get { return rootAddress; }
        }

        #endregion

        // ........................................................

        #region System.Object Overrides

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("AggregationID(");
            s.Append(RootAddress.ToString());
            s.Append(", ");
            s.Append(AggregationKey.ToString());
            s.Append(")");
            return s.ToString();
        }

        #endregion

        #region IComparable<AggregationID> Members

        int IComparable<AggregationID>.CompareTo(AggregationID other)
        {
            int result = AggregationKey.CompareTo(other.AggregationKey);
            return (result != 0) ? result : ((System.IComparable<QS._core_c_.Base3.InstanceID>)RootAddress).CompareTo(other.RootAddress);
        }

#if Using_VS2005Beta1
        bool IComparable<AggregationID>.Equals(AggregationID other)
        {
            return AggregationKey.Equals(other.AggregationKey) && RootAddress.Equals(other.RootAddress);
        }
#endif

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return aggregationKey.SerializableInfo.CombineWith(rootAddress.SerializableInfo).Extend((ushort)ClassID.AggregationID, sizeof(ushort), 0, 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) ((Base3_.IKnownClass) aggregationKey).ClassID;
            }
            header.consume(sizeof(ushort));
            aggregationKey.SerializeTo(ref header, ref data);
            rootAddress.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            ushort classID;
            fixed (byte* arrayptr = header.Array)
            {
                classID = *((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            aggregationKey = (IAggregationKey)QS._core_c_.Base3.Serializer.CreateObject(classID);
            aggregationKey.DeserializeFrom(ref header, ref data);
            rootAddress = new QS._core_c_.Base3.InstanceID();
            rootAddress.DeserializeFrom(ref header, ref data);
        }

        #endregion

		#region IStringSerializable Members

		ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort)ClassID.AggregationID; }
		}

		string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get
			{
				StringBuilder s = new StringBuilder(((QS.Fx.Serialization.IStringSerializable) rootAddress).AsString);
				s.Append(",");
				s.Append(QS._core_c_.Base3.Serializer.ToString(aggregationKey));
				return s.ToString();
			}

			set
			{
				int separator = value.IndexOf(",");
				rootAddress = new QS._core_c_.Base3.InstanceID();
				((QS.Fx.Serialization.IStringSerializable)rootAddress).AsString = value.Substring(0, separator);
				aggregationKey = QS._core_c_.Base3.Serializer.FromString(value.Substring(separator + 1)) as IAggregationKey;
			}
		}

		#endregion
	}
}
