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
	/// Summary description for GenericVS3PMID.
	/// </summary>
	public enum ProtocolPhase : byte
	{
		TRANSMITTING_DATA, /* ALLOW_DELIVERY, */ DELIVERED_EVERYWHERE, GARBAGE_COLLECTION
	}

	public interface IVSMPPMID : IVSMessageID
	{
		ProtocolPhase Phase
		{
			get;
		}
	}

	[QS.Fx.Serialization.ClassID(QS.ClassID.GenericVSMPPMID)]
	public class GenericVSMPPMID : GenericVSMessageID, IVSMPPMID
	{
		public GenericVSMPPMID(GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo, ProtocolPhase phase)
			: base(groupID, viewSeqNo, withinViewSeqNo)
		{
			this.phase = phase;
		}

		public GenericVSMPPMID() : base()
		{
		}

		private ProtocolPhase phase;

		public ProtocolPhase Phase
		{
			set
			{
				phase = value;
			}

			get
			{
				return phase;
			}
		}

		#region Base2.ISerializable Overrides

		public override ClassID ClassID
		{
			get
			{
				return QS.ClassID.GenericVSMPPMID;
			}
		}

		public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS._core_c_.Base2.Serializer.saveByte((byte) phase, blockOfData);
			base.save(blockOfData);
		}


		public override uint Size
		{
			get
			{
				return base.Size + QS._core_c_.Base2.SizeOf.Byte;
			}
		}

		public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			phase = (ProtocolPhase) QS._core_c_.Base2.Serializer.loadByte(blockOfData);
			base.load (blockOfData);
		}

		#endregion

		#region System.Object and Comparable Overrides

		public override int CompareTo(object obj)
		{
			int result = phase.CompareTo(((GenericVSMPPMID) obj).phase);
			return (result != 0) ? result : base.CompareTo(obj);
		}

		public override bool Equals(object obj)
		{
			return phase == ((GenericVSMPPMID) obj).phase && base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return phase.GetHashCode() ^ base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString() + "/" + phase.ToString();
		}

		#endregion
	}
}
