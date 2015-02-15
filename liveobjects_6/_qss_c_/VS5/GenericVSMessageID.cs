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

namespace QS._qss_c_.VS5
{
	/// <summary>
	/// Summary description for GenericVSMessageID.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.VSMessageID)]
	public class GenericVSMessageID : IVSMessageID
	{
		public static void register_serializable()
		{
			QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(ClassID.VSMessageID, typeof(GenericVSMessageID));
		}

		public GenericVSMessageID()
		{
		}

		public GenericVSMessageID(GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo)
		{
			this.groupID = groupID;
			this.viewSeqNo = viewSeqNo;
			this.withinViewSeqNo = withinViewSeqNo;
		}

		private GMS.GroupId groupID;
		private uint viewSeqNo, withinViewSeqNo;

		public QS._qss_c_.Base2_.ContainerClass ContainerClass
		{
			get
			{
				return QS._qss_c_.Base2_.ContainerClass.VSViewController;
			}
		}

		#region IVSMessageID Members

		public GMS.GroupId GroupID
		{
			get
			{
				return groupID;
			}
		}

		public uint ViewSeqNo
		{
			get
			{
				return viewSeqNo;
			}
		}

		public uint WithinViewSeqNo
		{
			get
			{
				return withinViewSeqNo;
			}
		}

		#endregion		

		#region ISerializable Members

		public virtual uint Size
		{
			get
			{
				return QS._core_c_.Base2.SizeOf.Int32 + 2 * QS._core_c_.Base2.SizeOf.UInt32;
			}
		}

		public virtual void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			groupID = new GMS.GroupId(QS._core_c_.Base2.Serializer.loadInt32(blockOfData));
			viewSeqNo = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
			withinViewSeqNo = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
		}

		public virtual void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS._core_c_.Base2.Serializer.saveInt32(groupID.ID, blockOfData);
			QS._core_c_.Base2.Serializer.saveUInt32(viewSeqNo, blockOfData);
			QS._core_c_.Base2.Serializer.saveUInt32(withinViewSeqNo, blockOfData);
		}

		public virtual QS.ClassID ClassID
		{
			get
			{
				return ClassID.VSMessageID;
			}
		}

		#endregion

		#region System.Object Overrides 

		public override bool Equals(object obj)
		{
			return (obj is GenericVSMessageID) ? (groupID.Equals(((GenericVSMessageID) obj).groupID) &&
				viewSeqNo.Equals(((GenericVSMessageID) obj).viewSeqNo) &&
				withinViewSeqNo.Equals(((GenericVSMessageID) obj).withinViewSeqNo)) : false;
		}

		public override int GetHashCode()
		{
			return groupID.GetHashCode() ^ viewSeqNo.GetHashCode() ^ withinViewSeqNo.GetHashCode();
		}

		public override string ToString()
		{
			return "[" + groupID.ToString() + ":" + viewSeqNo.ToString() + ":" + withinViewSeqNo.ToString() + "]";
		}

		#endregion
	
		#region IComparable Members

		public virtual int CompareTo(object obj)
		{
			if (obj is GenericVSMessageID)
			{
				GenericVSMessageID anotherGuy = (GenericVSMessageID) obj;
				int result = groupID.CompareTo(anotherGuy.groupID);
				if (result == 0)
				{
					result = viewSeqNo.CompareTo(anotherGuy.viewSeqNo);
					return (result == 0) ? (withinViewSeqNo.CompareTo(anotherGuy.withinViewSeqNo)) : result;
				}
				else return result;
			}
			else
				throw new Exception("Attempted to compare to and object of incompatible type.");
		}

		#endregion
	}
}
