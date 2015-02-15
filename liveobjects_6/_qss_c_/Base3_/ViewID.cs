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
	[Serializable]
	[QS.Fx.Serialization.ClassID(QS.ClassID.ViewID)]
	public class ViewID : QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable, System.IComparable, Aggregation3_.IGroupID 
	{
		public ViewID()
		{
		}

		public ViewID(GroupID groupID, uint viewSeqNo)
		{
			this.groupID = groupID;
			this.viewSeqNo = viewSeqNo;
		}

		private GroupID groupID;
		private uint viewSeqNo;

		#region Accessors

		public GroupID GroupID
		{
			set { groupID = value; }
			get { return groupID; }
		}

		public uint ViewSeqNo
		{
			set { viewSeqNo = value; }
			get { return viewSeqNo; }
		}

		#endregion

		#region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
		{
			get { return groupID.SerializableInfo.Extend((ushort) QS.ClassID.ViewID, sizeof(uint), 0, 0); }
		}

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			groupID.SerializeTo(ref header, ref data);
			fixed (byte* arrayptr = header.Array)
			{
				*((uint*)(arrayptr + header.Offset)) = viewSeqNo;
			}
			header.consume(sizeof(uint));
		}

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
			groupID = new GroupID();
			groupID.DeserializeFrom(ref header, ref data);
			fixed (byte* arrayptr = header.Array)
			{
				viewSeqNo = *((uint*)(arrayptr + header.Offset));
			}
			header.consume(sizeof(uint));
		}

		#endregion

		#region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort) QS.ClassID.ViewID; }
		}

        string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get { return ((QS.Fx.Serialization.IStringSerializable)groupID).AsString + "," + viewSeqNo.ToString(); }
			set
			{
				int separator = value.IndexOf(",");
				groupID = new GroupID();
				groupID.AsString = value.Substring(0, separator);
				viewSeqNo = System.Convert.ToUInt32(value.Substring(separator + 1));
			}
		}

		#endregion

		#region Overrides from System.Object

		public override bool Equals(object obj)
		{
			return (obj is ViewID) && groupID.Equals(((ViewID)obj).groupID) && viewSeqNo.Equals(((ViewID)obj).viewSeqNo);
		}

		public override int GetHashCode()
		{
			return groupID.GetHashCode() ^ viewSeqNo.GetHashCode();
		}

		public override string ToString()
		{
			return "View(" + groupID.ToString() + ":" + viewSeqNo.ToString() + ")";
		}

		#endregion

		#region IComparable Members

		int IComparable.CompareTo(object obj)
		{
			if (obj is ViewID)
			{
				int comparegroup_result = groupID.CompareTo(((ViewID)obj).groupID);
				return (comparegroup_result != 0) ? comparegroup_result : viewSeqNo.CompareTo(((ViewID)obj).viewSeqNo);
			}
			else
				throw new ArgumentException("Cannot compare to object of type " + QS._core_c_.Helpers.ToString.ObjectRef(obj));
		}

		#endregion
	}
}
