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

namespace QS._qss_c_.Multicasting3
{
    [QS.Fx.Serialization.ClassID(ClassID.Multicasting3_MessageID)]
    public class MessageID : QS.Fx.Serialization.ISerializable, Aggregation1_.IAggregationKey
    {
        public MessageID()
        {
        }

        public MessageID(Base3_.InSequence<Base3_.GroupID> groupView, uint messageSeqNo) 
            : this(groupView.Object, groupView.SeqNo, messageSeqNo)
        {
        }

        public MessageID(Base3_.GroupID groupID, uint viewSeqNo, uint messageSeqNo)
        {
            this.groupID = groupID;
            this.viewSeqNo = viewSeqNo;
            this.messageSeqNo = messageSeqNo;
        }

        #region Accessors

        public Base3_.GroupID GroupID
        {
            get { return groupID; }
        }

        public uint ViewSeqNo
        {
            get { return viewSeqNo; }
        }

        public uint MessageSeqNo
        {
            get { return messageSeqNo; }
        }

        #endregion

        private Base3_.GroupID groupID;
        private uint viewSeqNo, messageSeqNo;

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return groupID.SerializableInfo.Extend((ushort)ClassID.Multicasting3_MessageID, (ushort)(2 * sizeof(uint)), 0, 0); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            groupID.SerializeTo(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                *((uint*)headerptr) = viewSeqNo;
                *((uint*) (headerptr + sizeof(uint))) = messageSeqNo;
            }
            header.consume(2 * sizeof(uint));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            groupID = new QS._qss_c_.Base3_.GroupID();
            groupID.DeserializeFrom(ref header, ref data);
            fixed (byte* arrayptr = header.Array)
            {
                byte* headerptr = arrayptr + header.Offset;
                viewSeqNo = *((uint*)headerptr);
                messageSeqNo = *((uint*) (headerptr + sizeof(uint)));
            }
            header.consume(2 * sizeof(uint));
        }

        #endregion

        #region IKnownClass Members

        ClassID QS._qss_c_.Base3_.IKnownClass.ClassID
        {
            get { return ClassID.Multicasting3_MessageID; }
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            return (obj is MessageID) && groupID.Equals(((MessageID)obj).groupID) && viewSeqNo.Equals(((MessageID)obj).viewSeqNo)
                && messageSeqNo.Equals(((MessageID)obj).messageSeqNo);
        }

        public override int GetHashCode()
        {
            return groupID.GetHashCode() ^ viewSeqNo.GetHashCode() ^ messageSeqNo.GetHashCode();
        }

        public override string ToString()
        {
            return "<" + groupID.ToString() + ":" + viewSeqNo.ToString() + ", " + messageSeqNo.ToString() + ">";
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            MessageID anotherGuy = obj as MessageID;
            if (anotherGuy != null)
            {
                int result = groupID.CompareTo(anotherGuy.groupID);
                if (result == 0)
                {
                    result = viewSeqNo.CompareTo(anotherGuy.viewSeqNo);
                    if (result == 0)
                    {
                        return messageSeqNo.CompareTo(anotherGuy.messageSeqNo);
                    }
                    else
                        return result;
                }
                else
                    return result;
            }
            else
                throw new ArgumentException();
        }

        #endregion

		#region IStringSerializable Members

		ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort)ClassID.Multicasting3_MessageID; }
		}

		string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get
			{
				StringBuilder s = new StringBuilder(((QS.Fx.Serialization.IStringSerializable) groupID).AsString);
				s.Append(",");
				s.Append(viewSeqNo.ToString());
				s.Append(",");
				s.Append(messageSeqNo.ToString());
				return s.ToString();
			}

			set
			{
				int separator1 = value.IndexOf(",");
				int separator2 = value.IndexOf(",", separator1 + 1);
				groupID = new QS._qss_c_.Base3_.GroupID(value.Substring(0, separator1));
				viewSeqNo = System.Convert.ToUInt32(value.Substring(separator1 + 1, separator2 - separator1 - 1));
				messageSeqNo = System.Convert.ToUInt32(value.Substring(separator2 + 1));				
			}
		}

		#endregion
	}
}
