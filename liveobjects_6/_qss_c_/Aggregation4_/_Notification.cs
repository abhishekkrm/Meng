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

namespace QS._qss_c_.Aggregation4_
{
/*
	[QS.Fx.Serialization.ClassID(ClassID.Aggregation4_AggregationController2_Notification)]
	private class Notification : QS.Fx.Serialization.ISerializable
	{
		public enum Type : ushort
		{

//					ACK,							// this node has received data and does not need forwarding
//					SUBTREE_ACK,			// all subtree rooted at this node received data
//					CLEANUP,					// the whole tree has data and knows about it, we now do cleanup, top-down
//					FLOOD_SUBTREE

		}

		public Notification()
		{
		}

		public Notification(Type type)
		{
			this.type = type;
		}

		private Type type;

		#region Accessors

		public Type TypeOf
		{
			get { return type; }
		}

		#endregion

		#region ISerializable Members

		QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
		{
			get
			{
				return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Aggregation4_AggregationController2_Notification,
					sizeof(ushort), sizeof(ushort), 0);
			}
		}

		unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				byte* headerptr = arrayptr + header.Offset;
				*((ushort*)headerptr) = (ushort)type;
			}
			header.consume(sizeof(ushort));
		}

		unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				byte* headerptr = arrayptr + header.Offset;
				type = (Type)(*((ushort*)headerptr));
			}
			header.consume(sizeof(ushort));
		}

		#endregion

		public override string ToString()
		{
			return type.ToString();
		}
	}
*/
}
