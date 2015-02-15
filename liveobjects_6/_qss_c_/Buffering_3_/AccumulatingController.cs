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

namespace QS._qss_c_.Buffering_3_
{
    public class AccumulatingController : IController
    {
        public AccumulatingController(int maximumSize)
        {
            this.maximumSize = maximumSize;
            outgoingCollection = new OutgoingCollection();
            readyQueue = new Queue<IMessageCollection>();
        }

        private int maximumSize;
        private OutgoingCollection outgoingCollection;
        private System.Collections.Generic.Queue<IMessageCollection> readyQueue;

        public static IControllerClass ControllerClass
        {
            get
            {
                return controllerClass;
            }
        }

        #region ControllerClass

        private static Class controllerClass = new Class();
        private class Class : IControllerClass
        {
            public Class()
            {
            }

            #region IControllerClass Members

            public IController CreateController(int maximumSize)
            {
                return new AccumulatingController(maximumSize);
            }

            public IEnumerable<QS._core_c_.Base3.Message> GetMessages(QS.Fx.Serialization.ISerializable messageCollection)
            {
                IncomingCollection incomingCollection = messageCollection as IncomingCollection;
                if (incomingCollection != null)
                {
                    return incomingCollection.Messages;
                }
                else
                    throw new ArgumentException("Expected " + typeof(IncomingCollection).Name + ", received " + 
                        ((messageCollection != null) ? messageCollection.GetType().Name : "(null)"));
            }

            #endregion
        }

        #endregion

        #region Buffering3.IController Members

        public bool Empty
        {
            get {return outgoingCollection.empty; }
        }

		public int CurrentCapacity
		{
			get { return maximumSize - outgoingCollection.CurrentSize - 6; }
		}

        public void append(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
        {
            if (data.SerializableInfo.Size > this.CurrentCapacity)
                flush();

            outgoingCollection.append(destinationLOID, data);
        }

        public void flush()
        {
            if (!outgoingCollection.empty)
            {
                readyQueue.Enqueue(outgoingCollection);
                outgoingCollection = new OutgoingCollection();
            }
        }

        public System.Collections.Generic.Queue<IMessageCollection> ReadyQueue
        {
            get { return readyQueue; }
        }

        public int MTU
        {
            get { return maximumSize - 8; }
        }

        #endregion

        #region Class OutgoingCollection

        private class OutgoingCollection : IMessageCollection
        {
            public OutgoingCollection()
            {
                messages = new System.Collections.Generic.List<QS._core_c_.Base3.Message>();
                headerSize = 2;
                size = 2;
                numberOfBuffers = 0;

                empty = true;
            }

            #region IMessageCollection Members

            public int Count
            {
                get
                {
                    return messages.Count;
                }
            }

            #endregion

            public int CurrentSize
            {
                get
                {
                    return size;
                }
            }

            private System.Collections.Generic.IList<QS._core_c_.Base3.Message> messages;
            private ushort headerSize;
            private int size, numberOfBuffers;

            public bool empty;

            public void append(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                empty = false;

                QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;
                headerSize += (ushort) (info.HeaderSize + 6);
                size += info.Size + 6;
                numberOfBuffers += info.NumberOfBuffers;
                messages.Add(new QS._core_c_.Base3.Message(destinationLOID, data));
            }

            #region QS.Fx.Serialization.ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get
                {
                    return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.AccumulatingController_MessageCollection, headerSize, size, numberOfBuffers);
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    *((ushort *)headerptr) = (ushort) messages.Count;
                    header.consume(2);
                    foreach (QS._core_c_.Base3.Message message in messages)
                    {
                        headerptr = arrayptr + header.Offset;
                        *((ushort*)headerptr) = message.transmittedObject.SerializableInfo.ClassID;
                        *((uint*)(headerptr + 2)) = message.destinationLOID;
                        header.consume(6);
                        message.transmittedObject.SerializeTo(ref header, ref data);
                    }
                }
            }

            public void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new Exception("Operation not permitted in this context.");
            }

            #endregion
        }

        #endregion

        #region Class IncomingCollection

        [QS.Fx.Serialization.ClassID(QS.ClassID.AccumulatingController_MessageCollection)]
        public class IncomingCollection : QS.Fx.Serialization.ISerializable
        {
            public IncomingCollection()
            {
            }

            public System.Collections.Generic.IList<QS._core_c_.Base3.Message> Messages
            {
                get { return messages; }
            }

            private System.Collections.Generic.IList<QS._core_c_.Base3.Message> messages = null;

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get { throw new Exception("Operation not permitted in this context."); }
            }

            public void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                throw new Exception("Operation not permitted in this context.");
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    ushort nmessages = *((ushort*)headerptr);
                    messages = new System.Collections.Generic.List<QS._core_c_.Base3.Message>(nmessages);
                    header.consume(2);
                    for (int ind = 0; ind < nmessages; ind++)
                    {
                        headerptr = arrayptr + header.Offset;
                        ushort classID = *((ushort*)headerptr);
                        QS._core_c_.Base3.Message message = new QS._core_c_.Base3.Message(*((uint*)(headerptr + 2)), QS._core_c_.Base3.Serializer.CreateObject(classID));
                        header.consume(6);
                        message.transmittedObject.DeserializeFrom(ref header, ref data);
                        messages.Add(message);
                    }
                }
            }

            #endregion

            public override string ToString()
            {
                System.Text.StringBuilder s = new StringBuilder();
                s.AppendLine("\n\nAccumulatingController.IncomingCollection with " + messages.Count.ToString() + " messages:");
                foreach (QS._core_c_.Base3.Message message in messages)
                    s.AppendLine(message.destinationLOID.ToString() + "\t: " + message.transmittedObject.ToString());
                s.AppendLine();
                return s.ToString();
            }
        }

        #endregion
    }
}
