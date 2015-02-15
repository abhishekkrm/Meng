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
    [QS.Fx.Serialization.ClassID(ClassID.RegionSig)]
    public class RegionSig : System.IComparable<RegionSig>, System.IComparable, QS.Fx.Serialization.ISerializable
    {
        public RegionSig(GroupID[] groupIDs)
        {
            this.groupIDs = (GroupID[]) groupIDs.Clone();
            Array.Sort<GroupID>(this.groupIDs);
        }

        public RegionSig()
        {
        }

        private GroupID[] groupIDs;

        public bool CorrespondsToNoGroups
        {
            get { return groupIDs.Length == 0; }
        }

        public RegionSig CalculateModifiedSig(IList<GroupID> groupsToAdd, IList<GroupID> groupsToLeave)
        {
            return this.CalculateModifiedSig(
                QS._core_c_.Helpers.CollectionHelper.List2Array<GroupID>(groupsToAdd), QS._core_c_.Helpers.CollectionHelper.List2Array<GroupID>(groupsToLeave));
        }

        public RegionSig CalculateModifiedSig(GroupID[] groupsToAdd, GroupID[] groupsToLeave)
        {
            System.Collections.Generic.List<GroupID> newIDs = new List<GroupID>(groupIDs.Length + groupsToAdd.Length - groupsToLeave.Length);
            newIDs.AddRange(groupIDs);
            foreach (GroupID groupID in groupsToLeave)
                newIDs.RemoveAt(newIDs.BinarySearch(groupID));
            newIDs.AddRange(groupsToAdd);
            RegionSig resultSig = new RegionSig();
            resultSig.groupIDs = newIDs.ToArray();
            Array.Sort<GroupID>(resultSig.groupIDs);
            return resultSig;
        }

        public GroupID[] GroupIDs
        {
            get { return groupIDs; }
        }

        #region IComparable<RegionSig> Members

        public int CompareTo(RegionSig other)
        {
            int result = groupIDs.Length.CompareTo(other.groupIDs.Length);
            if (result == 0)
            {
                for (int ind = 0; ind < groupIDs.Length; ind++)
                {
                    result = groupIDs[ind].CompareTo(other.groupIDs[ind]);
                    if (result != 0)
                        return result;
                }
                return 0;
            }
            else 
                return result;
        }

        public bool Equals(RegionSig other)
        {
            return ((System.IComparable<RegionSig>) this).CompareTo(other) == 0;
        }

        #endregion

        public override string ToString()
        {
            return Convert.ArrayToString<GroupID>("RegionSig", groupIDs);
        }

        public override int GetHashCode()
        {
            int hashCode = groupIDs.Length.GetHashCode();
            for (int ind = 0; ind < groupIDs.Length; ind++)
                hashCode = hashCode ^ groupIDs[ind].GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return ((System.IComparable)this).CompareTo(obj) == 0;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            RegionSig other = obj as RegionSig;
            if (other != null)
                return ((System.IComparable<RegionSig>)this).CompareTo(other);
            else
                throw new Exception("Cannot compare with object of incompatible type.");
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return SerializationHelper.SerializableInfoOfArray(groupIDs.Length, sizeof(uint)); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            SerializationHelper.SerializeArray<GroupID>(groupIDs, ref header, ref data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            SerializationHelper.DeserializeArray<GroupID>(out groupIDs, ref header, ref data);
        }

        #endregion
    }
}
