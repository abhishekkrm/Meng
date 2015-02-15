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
    [QS.Fx.Serialization.ClassID(ClassID.Gossiping2_RegionAggregated)]
    public class RegionAggregated : QS.Fx.Serialization.ISerializable
    {
        public RegionAggregated()
        {
        }

        public RegionAggregated(Base3_.RegionID regionID)
        {
            this.regionID = regionID;
        }

        public RegionAggregated(Base3_.RegionID regionID, Aggregation3_.IAggregatable aggregatedObject) : this(regionID)
        {
            this.aggregatedObjects.Add(aggregatedObject);
        }

        public RegionAggregated(Base3_.RegionID regionID, IEnumerable<Aggregation3_.IAggregatable> aggregatedObjects) : this(regionID)
        {
            this.aggregatedObjects.AddRange(aggregatedObjects);
        }

        private Base3_.RegionID regionID;
        private List<Aggregation3_.IAggregatable> aggregatedObjects = new List<QS._qss_c_.Aggregation3_.IAggregatable>();

        public Base3_.RegionID RegionID
        {
            get { return regionID; }
        }

        public IList<Aggregation3_.IAggregatable> AggregatedObjects
        {
            get { return aggregatedObjects; }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = regionID.SerializableInfo.Extend(
                    (ushort)ClassID.Gossiping2_RegionAggregated, (ushort) sizeof(ushort), 0, 0);
                foreach (Aggregation3_.IAggregatable aggregatedObject in aggregatedObjects)
                {
                    info.HeaderSize += (ushort) sizeof(ushort);
                    info.Size += sizeof(ushort);
                    info.AddAnother(aggregatedObject.SerializableInfo);
                }
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            regionID.SerializeTo(ref header, ref data);          
            fixed (byte* arrayptr = header.Array)
            {
                *((ushort*)(arrayptr + header.Offset)) = (ushort) aggregatedObjects.Count;
            }
            header.consume(sizeof(ushort));
            foreach (Aggregation3_.IAggregatable aggregatedObject in aggregatedObjects)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    *((ushort*)(arrayptr + header.Offset)) = aggregatedObject.SerializableInfo.ClassID;
                }
                header.consume(sizeof(ushort));
                aggregatedObject.SerializeTo(ref header, ref data);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            regionID = new QS._qss_c_.Base3_.RegionID();
            regionID.DeserializeFrom(ref header, ref data);
            int nobjects;
            fixed (byte* arrayptr = header.Array)
            {
                nobjects = (int) *((ushort*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(ushort));
            while (nobjects-- > 0)
            {
                ushort classID;
                fixed (byte* arrayptr = header.Array)
                {
                    classID = *((ushort*)(arrayptr + header.Offset));
                }
                header.consume(sizeof(ushort));
                Aggregation3_.IAggregatable aggregatedObject = (Aggregation3_.IAggregatable)QS._core_c_.Base3.Serializer.CreateObject(classID);
                aggregatedObject.DeserializeFrom(ref header, ref data);
                aggregatedObjects.Add(aggregatedObject);
            }
        }

        #endregion

        public override string ToString()
        {
            return "{ " + regionID.ToString() + " : " + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Aggregation3_.IAggregatable>(
                aggregatedObjects, ",") + " }";
        }
    }
}
