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

// #define DEBUG_InstanceSender

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_c_.Senders6
{
    public class InstanceSender : Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>, 
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender>
    {
        public InstanceSender(QS.Fx.Logging.ILogger logger, // QS._core_c_.Base3.InstanceID localIID,
            FailureDetection_.IFailureDetector failureDetector, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection,
            Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingReliableSenderCollection)
        {
            this.logger = logger;
//            this.localIID = localIID;
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.underlyingReliableSenderCollection = underlyingReliableSenderCollection;

            if (failureDetector != null)
                failureDetector.OnChange += new QS._qss_c_.FailureDetection_.ChangeCallback(failureDetector_OnChange);
            else
            {
                if (underlyingReliableSenderCollection != null)
                    throw new ArgumentException("Cannot instantiate reliable instance sender without failure detector.");
            }
        }

        void failureDetector_OnChange(IEnumerable<QS._qss_c_.FailureDetection_.Change> changes)
        {
            lock (senders)
            {
                foreach (FailureDetection_.Change change in changes)
                {
                    if (change.Action == QS._qss_c_.FailureDetection_.Action.CRASHED && senders.ContainsKey(change.InstanceID))
                    {
                        senders[change.InstanceID].crashed();
                        senders.Remove(change.InstanceID);
                    }
                }
            }
        }

        private QS.Fx.Logging.ILogger logger;
//        private QS._core_c_.Base3.InstanceID localIID;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
        private Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingReliableSenderCollection;
        private System.Collections.Generic.IDictionary<QS._core_c_.Base3.InstanceID, Sender> senders = 
            new System.Collections.Generic.Dictionary<QS._core_c_.Base3.InstanceID, Sender>();

        private Sender GetSender(QS._core_c_.Base3.InstanceID address)
        {
            lock (senders)
            {
                if (senders.ContainsKey(address))
                    return senders[address];
                else
                {
                    Sender sender = new Sender(address, ((underlyingSenderCollection != null) ? underlyingSenderCollection[address.Address] : null),
                        ((underlyingReliableSenderCollection != null) ? underlyingReliableSenderCollection[address.Address] : null));
                    senders[address] = sender;
                    return sender;
                }
            }
        }

        #region Class Message

        [QS.Fx.Serialization.ClassID(ClassID.Senders6_InstanceSender_Message)]
        internal class Message : QS.Fx.Serialization.ISerializable, Base3_.IAsynchronousOperation
        {
            public override string ToString()
            {
                return "(ICID:" + incarnation.ToString() + ";LOID:" + destinationLOID.ToString() + ";DATA:" + dataObject.ToString() + ")";
            }

            public const int HeaderOverhead = sizeof(ushort) + 2 * sizeof(uint); 

            public Message()
            {
            }

            public Message(InstanceSender.Sender owner, QS._core_c_.Base3.Incarnation incarnation, uint destinationLOID, QS.Fx.Serialization.ISerializable dataObject,
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) : this(incarnation, destinationLOID, dataObject)
            {
                this.owner = owner;
                this.completionCallback = completionCallback;
                this.asynchronousState = asynchronousState;
            }

            public Message(QS._core_c_.Base3.Incarnation incarnation, uint destinationLOID, QS.Fx.Serialization.ISerializable dataObject)
            {
                this.incarnation = incarnation;
                this.destinationLOID = destinationLOID;
                this.dataObject = dataObject;
            }

            private QS._core_c_.Base3.Incarnation incarnation;
            private uint destinationLOID;
            private QS.Fx.Serialization.ISerializable dataObject;
            private Base3_.AsynchronousOperationCallback completionCallback;
            private object asynchronousState;
            private Base3_.IAsynchronousOperation asynchronousOperation;
            private InstanceSender.Sender owner;

            #region Accessors

            public QS._core_c_.Base3.Incarnation Incarnation
            {
                get { return incarnation; }
            }

            public uint DestinationLOID
            {
                get { return destinationLOID; }
            }

            public QS.Fx.Serialization.ISerializable DataObject
            {
                get { return dataObject; }
            }

            public object AsynchronousState
            {
                get { return asynchronousState; }
            }

            public Base3_.AsynchronousOperationCallback CompletionCallback
            {
                get { return completionCallback; }
            }

            public Base3_.IAsynchronousOperation AsynchronousOperation
            {
                get { return asynchronousOperation; }
                set { asynchronousOperation = value; }
            }	

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { return dataObject.SerializableInfo.Extend((ushort)ClassID.Senders6_InstanceSender_Message, HeaderOverhead, 0, 0); }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    *((uint*) headerptr) = incarnation.SeqNo;
                    *((uint*)(headerptr + sizeof(uint))) = destinationLOID;
                    *((ushort*)(headerptr + 2 * sizeof(uint))) = dataObject.SerializableInfo.ClassID;
                }
                header.consume(HeaderOverhead);
                dataObject.SerializeTo(ref header, ref data);
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                ushort classID;
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    incarnation = new QS._core_c_.Base3.Incarnation(*((uint*)headerptr));
                    destinationLOID = *((uint*)(headerptr + sizeof(uint)));
                    classID = *((ushort*)(headerptr + 2 * sizeof(uint)));
                }
                header.consume(HeaderOverhead);
                dataObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
                dataObject.DeserializeFrom(ref header, ref data);
            }

            #endregion

            #region IAsynchronousOperation Members

            void QS._qss_c_.Base3_.IAsynchronousOperation.Cancel()
            {
                owner.cancel(this);
            }

            void QS._qss_c_.Base3_.IAsynchronousOperation.Ignore()
            {
                lock (this)
                {
                    completionCallback = null;
                }
            }

            bool QS._qss_c_.Base3_.IAsynchronousOperation.Cancelled
            {
                get { return asynchronousOperation.Cancelled; }
            }

            #endregion

            #region IAsyncResult Members

            object IAsyncResult.AsyncState
            {
                get { return asynchronousState; }
            }

            System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
            {
                get { return asynchronousOperation.AsyncWaitHandle; }
            }

            bool IAsyncResult.CompletedSynchronously
            {
                get { return asynchronousOperation.CompletedSynchronously; }
            }

            bool IAsyncResult.IsCompleted
            {
                get { return asynchronousOperation.IsCompleted; }
            }

            #endregion
        }

        #endregion

        #region Class Sender

        internal class Sender : QS._qss_c_.Base3_.ISerializableSender, Base3_.IReliableSerializableSender
        {
            public Sender(QS._core_c_.Base3.InstanceID instanceID, 
                QS._qss_c_.Base3_.ISerializableSender underlyingSender, Base3_.IReliableSerializableSender underlyingReliableSender)
            {
                this.instanceID = instanceID;
                this.underlyingSender = underlyingSender;
                this.underlyingReliableSender = underlyingReliableSender;

                myMTU = (underlyingSender != null) ? underlyingSender.MTU : int.MaxValue;
                if (underlyingReliableSender != null && underlyingReliableSender.MTU < myMTU)
                    myMTU = underlyingReliableSender.MTU;
                myMTU -= Message.HeaderOverhead;
            }

            private QS._core_c_.Base3.InstanceID instanceID;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
            private Base3_.IReliableSerializableSender underlyingReliableSender;
            private int myMTU;

            // not very efficient, some quick hack for now
            private System.Collections.ObjectModel.Collection<Message> pending = new System.Collections.ObjectModel.Collection<Message>();

            public void crashed()
            {
                lock (pending)
                {
                    foreach (Message message in pending)
                    {
                        lock (message)
                        {
                            message.AsynchronousOperation.Cancel();
                        }
                    }

                    pending.Clear();
                }
            }

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return instanceID.Address; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                if (underlyingSender != null)
                    underlyingSender.send((uint)ReservedObjectID.Senders6_InstanceReceiver, new Message(instanceID.Incarnation, destinationLOID, data));
                else
                    throw new NotSupportedException("Regular channel is not supported on this sender: no underlying regular sender attached.");
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { return myMTU; }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
                QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                if (underlyingReliableSender != null)
                {
                    Message message = new Message(this, instanceID.Incarnation, destinationLOID, data, completionCallback, asynchronousState);
                    lock (pending)
                    {
                        lock (message)
                        {
                            message.AsynchronousOperation = underlyingReliableSender.BeginSend((uint)ReservedObjectID.Senders6_InstanceReceiver,
                                message, new Base3_.AsynchronousOperationCallback(this.completionCallback), message);                            
                            if (message.AsynchronousOperation == null)
                                throw new Exception("Internal error: Could not send message.");
                        }

                        pending.Add(message);
                    }

                    return message;
                }
                else
                    throw new NotSupportedException("Reliable channel is not supported on this sender: no underlying reliable sender attached.");
            }

            private void completionCallback(Base3_.IAsynchronousOperation asynchronousOperation)
            {
                Message message = (Message) asynchronousOperation.AsyncState;
                lock (pending)
                {
                    pending.Remove(message);

                    lock (message)
                    {
                        if (message.CompletionCallback != null)
                            message.CompletionCallback(message);
                    }
                }
            }

            public void cancel(Message message)
            {
                lock (pending)
                {
                    pending.Remove(message);

                    lock (message)
                    {
                        message.AsynchronousOperation.Cancel();
                    }
                }
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
                if (underlyingReliableSender != null)
                {
                    Message message = (Message) asynchronousOperation;
                    underlyingReliableSender.EndSend(message.AsynchronousOperation);
                }
                else
                    throw new NotSupportedException("Reliable channel is not supported on this sender: no underlying reliable sender attached.");
            }

            #endregion
        }

        #endregion

        #region ISenderCollection<InstanceID,ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get 
            { 
                foreach (QS._core_c_.Base3.InstanceID address in senders.Keys)
                    yield return ((QS.Fx.Serialization.IStringSerializable) address).AsString;
            }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get 
            { 
                QS._core_c_.Base3.InstanceID address = new QS._core_c_.Base3.InstanceID();
                ((QS.Fx.Serialization.IStringSerializable) address).AsString = attributeName;
                return new QS.Fx.Inspection.ScalarAttribute(attributeName, GetSender(address));
            }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Instance Senders"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
        }

        #endregion

        #region ISenderCollection<InstanceID,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get
            {
                if (underlyingReliableSenderCollection != null)
                    return GetSender(destinationAddress);
                else
                    throw new NotSupportedException("Reliable sending is not suppored, no underlying reliable transport layer attached.");
            }
        }

        #endregion
    }
}
