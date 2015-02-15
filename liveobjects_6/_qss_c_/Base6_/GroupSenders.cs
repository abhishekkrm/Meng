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

namespace QS._qss_c_.Base6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class GroupSenders : QS.Fx.Inspection.IAttributeCollection, Multicasting3.ISimpleSender
    {
        public GroupSenders(Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection, QS.Fx.Clock.IClock clock)
        {
            this.sinkCollection = sinkCollection;
            this.clock = clock;
        }

        private Base6_.ICollectionOf<Base3_.GroupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> sinkCollection;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<Base3_.GroupID, Sender> senders = new Dictionary<Base3_.GroupID, Sender>();
        private QS.Fx.Clock.IClock clock;

        #region Class Sender

        [QS._core_c_.Diagnostics.Component]
        private class Sender : Buffer, Multicasting3.IGroupSender
        {
            public Sender(Base3_.GroupID groupID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink, QS.Fx.Clock.IClock clock) : base(sink, clock)
            {
                this.groupID = groupID;
            }

            private Base3_.GroupID groupID;

            #region IGroupSender Members

            QS._qss_c_.Base3_.GroupID QS._qss_c_.Multicasting3.IGroupSender.GroupID
            {
                get { return groupID; }
            }

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Multicasting3.IGroupSender.BeginSend(
                uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
                QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                return ((Base3_.IReliableSerializableSender)this).BeginSend(
                    destinationLOID, data, completionCallback, asynchronousState);
            }

            void QS._qss_c_.Multicasting3.IGroupSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            int QS._qss_c_.Multicasting3.IGroupSender.MTU
            {
                get { throw new NotSupportedException(); }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                if (obj is Base3_.GroupID)
                    return groupID.CompareTo((Base3_.GroupID)obj);
                else if (obj is Multicasting3.IGroupSender)
                    return groupID.CompareTo(((Multicasting3.IGroupSender)obj).GroupID);
                else
                    throw new Exception("Uncomparable");
            }

            #endregion

            #region IComparable<GroupID> Members

            int IComparable<QS._qss_c_.Base3_.GroupID>.CompareTo(QS._qss_c_.Base3_.GroupID other)
            {
                return groupID.CompareTo(other);
            }

            #endregion

            #region IComparable<IGroupSender> Members

            int IComparable<QS._qss_c_.Multicasting3.IGroupSender>.CompareTo(QS._qss_c_.Multicasting3.IGroupSender other)
            {
                return groupID.CompareTo(other.GroupID);
            }

            #endregion
        }

        #endregion

        #region IGroupSenderClass<IGroupSender> Members

        QS._qss_c_.Multicasting3.IGroupSender QS._qss_c_.Multicasting3.IGroupSenderClass<QS._qss_c_.Multicasting3.IGroupSender>.this[QS._qss_c_.Base3_.GroupID groupID]
        {
            get
            {
                lock (this)
                {
                    if (senders.ContainsKey(groupID))
                        return senders[groupID];
                    else
                    {
                        Sender sender = new Sender(groupID, sinkCollection[groupID], clock);
                        senders[groupID] = sender;
                        return sender;
                    }
                }
            }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get
            {
                List<string> names = new List<string>();
                foreach (Base3_.GroupID address in senders.Keys)
                    names.Add(address.AsString);
                return names;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get
            {
                Base3_.GroupID address = new Base3_.GroupID();
                address.AsString = attributeName;
                return new QS.Fx.Inspection.ScalarAttribute(attributeName, senders[address]);
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Buffer Collection"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion

        #region IStatisticsCollector Members

        IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
        {
            get { return QS._qss_c_.Helpers_.ListOf<QS._core_c_.Components.Attribute>.Nothing;  }
        }

        #endregion

        #region IInspectable Members

        public QS.Fx.Inspection.IAttributeCollection Attributes
        {
            get { return this; }
        }

        #endregion
    }
}
