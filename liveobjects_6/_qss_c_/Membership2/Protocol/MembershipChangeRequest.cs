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

namespace QS._qss_c_.Membership2.Protocol
{
    [QS.Fx.Serialization.ClassID(ClassID.MembershipChangeRequest)]
    public class MembershipChangeRequest : IMembershipChangeRequest, QS.Fx.Serialization.ISerializable
    {
        public MembershipChangeRequest()
        {
        }

        public MembershipChangeRequest(QS._core_c_.Base3.InstanceID instanceID, bool crashed) : this(instanceID, crashed, null, null)
        {
        }

        public MembershipChangeRequest(QS._core_c_.Base3.InstanceID instanceID, bool crashed,
            IList<QS._qss_c_.Base3_.GroupID> groupsToJoin, IList<QS._qss_c_.Base3_.GroupID> groupsToLeave)
        {
            this.instanceID = instanceID;
            this.crashed = crashed;
            this.toJoin = new List<Base3_.GroupID>();
            if (groupsToJoin != null)
                toJoin.AddRange(groupsToJoin);
            this.toLeave = new List<Base3_.GroupID>();
            if (groupsToLeave != null)
                toLeave.AddRange(groupsToLeave);
        }

        private QS._core_c_.Base3.InstanceID instanceID;
        private bool crashed;
        private List<Base3_.GroupID> toJoin, toLeave;

        #region IMembershipChangeRequest Members

        public QS._core_c_.Base3.InstanceID InstanceID
        {
            get { return instanceID; }
        }

        public bool Crashed
        {
            get { return crashed; }
        }

        public IList<QS._qss_c_.Base3_.GroupID> ToJoin
        {
            get { return toJoin; }
        }

        public IList<QS._qss_c_.Base3_.GroupID> ToLeave
        {
            get { return toLeave; }
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                return instanceID.SerializableInfo.CombineWith(Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.GroupID>(toJoin)).CombineWith(
                    Base3_.SerializationHelper.SerializableInfoOfListOfFixed<Base3_.GroupID>(toLeave)).Extend((ushort) ClassID.MembershipChangeRequest, sizeof(bool), 0, 0);                
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            instanceID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                *((bool*)(arrayptr + header.Offset)) = crashed;
            }
            header.consume(sizeof(bool));
            if (!crashed)
            {
                Base3_.SerializationHelper.SerializeCollection<Base3_.GroupID>(toJoin, ref header, ref data);
                Base3_.SerializationHelper.SerializeCollection<Base3_.GroupID>(toLeave, ref header, ref data);
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            instanceID = new QS._core_c_.Base3.InstanceID();
            instanceID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                crashed = *((bool*)(arrayptr + header.Offset));
            }
            header.consume(sizeof(bool));
            if (!crashed)
            {
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.GroupID>, Base3_.GroupID>(out toJoin,
                    Base3_.Constructors<Base3_.GroupID>.ListConstructor, ref header, ref data);
                Base3_.SerializationHelper.DeserializeCollection<List<Base3_.GroupID>, Base3_.GroupID>(out toLeave,
                    Base3_.Constructors<Base3_.GroupID>.ListConstructor, ref header, ref data);
            }
        }

        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder("MembershipChangeRequest : " + instanceID.ToString() + " ");
            if (crashed)
                s.Append("CRASHED");
            else
            {
                s.Append("ToJoin: ");
                s.Append(QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.GroupID>(toJoin, ","));
                s.Append(" ToLeave: ");
                s.Append(QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<Base3_.GroupID>(toLeave, ","));
            }
            return s.ToString();
        }

        public static ICollection<IMembershipChangeRequest> AggregateCollection(
            IEnumerable<IMembershipChangeRequest> requestsToAggregate)
        {
            Dictionary<QS._core_c_.Base3.InstanceID,Protocol.IMembershipChangeRequest> aggregatedRequests =
                new Dictionary<QS._core_c_.Base3.InstanceID, IMembershipChangeRequest>();
            foreach (MembershipChangeRequest request in requestsToAggregate)
            {
                if (aggregatedRequests.ContainsKey(request.instanceID))
                {
                    aggregatedRequests[request.instanceID] = 
                        aggregatedRequests[request.instanceID].AggregateWith(request);
                }
                else
                    aggregatedRequests.Add(request.instanceID, request);
            }

            return aggregatedRequests.Values;
        }

        public IMembershipChangeRequest AggregateWith(IMembershipChangeRequest anotherGuy)
        {
            if (this.Crashed || anotherGuy.Crashed)
                return anotherGuy;
            else
            {
                foreach (Base3_.GroupID groupID in anotherGuy.ToLeave)
                {
                    if (toJoin.Contains(groupID))
                        toJoin.Remove(groupID);
                    if (!toLeave.Contains(groupID))
                        toLeave.Add(groupID);
                }

                foreach (Base3_.GroupID groupID in anotherGuy.ToJoin)
                {
                    if (toLeave.Contains(groupID))
                        toLeave.Remove(groupID);
                    if (!toJoin.Contains(groupID))
                        toJoin.Add(groupID);
                }

                return this;
            }
        }
    }
}
