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

namespace QS._qss_c_.Aggregation3_
{
	[QS.Fx.Serialization.ClassID(ClassID.Aggregation3_ChannelID)]
	public class ChannelID : QS.Fx.Serialization.ISerializable
	{
		public ChannelID()
		{
		}

		public ChannelID(IGroupID groupID, QS._core_c_.Base3.InstanceID rootAddress)
		{
			this.groupID = groupID;
			this.rootAddress = rootAddress;
		}

		private IGroupID groupID;
		private QS._core_c_.Base3.InstanceID rootAddress;

		public IGroupID GroupID
		{
			get { return groupID; }
		}

		public QS._core_c_.Base3.InstanceID RootAddress
		{
			get { return rootAddress; }
		}

		#region ISerializable Members

		QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
		{
			get
			{
				return groupID.SerializableInfo.CombineWith(rootAddress.SerializableInfo).Extend(
					(ushort)ClassID.Aggregation3_ChannelID, (ushort)sizeof(ushort), 0, 0);
			}
		}

		unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				*((ushort*)(arrayptr + header.Offset)) = groupID.SerializableInfo.ClassID;
			}
			header.consume(sizeof(ushort));
			groupID.SerializeTo(ref header, ref data);
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
			groupID = QS._core_c_.Base3.Serializer.CreateObject(classID) as IGroupID;
			groupID.DeserializeFrom(ref header, ref data);
			rootAddress = new QS._core_c_.Base3.InstanceID();
			rootAddress.DeserializeFrom(ref header, ref data);
		}

		#endregion

		public override string ToString()
		{
			return groupID.ToString() + "," + rootAddress.ToString();
		}
	}
}
