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

// #define DEBUG_ChoppingSender

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Senders3
{
    public class ChoppingSender : Base3_.SenderClass<Base3_.IReliableSerializableSender>,
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender>
    {
        public ChoppingSender(Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingSenderCollection,            
            QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer) : this(underlyingSenderCollection, null, logger, demultiplexer)
        {
        }

        public ChoppingSender(Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingSenderCollection,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> underlyingInstanceSenderCollection,
            QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer)
        {
            this.underlyingSenderCollection = underlyingSenderCollection;
            this.underlyingInstanceSenderCollection = underlyingInstanceSenderCollection;
            this.logger = logger;
            this.demultiplexer = demultiplexer;

            demultiplexer.register((uint)ReservedObjectID.ChoppingSender, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));

            assembledChunks = new System.Collections.Generic.Dictionary<uint, ChunkSet>();
        }

        private Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingSenderCollection;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> underlyingInstanceSenderCollection;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;

        private System.Collections.Generic.IDictionary<uint, ChunkSet> assembledChunks;

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
                    Sender sender = new Sender(this, address);
                    senders[address] = sender;
                    return sender;
                }
            }
        }

        #region ISenderCollection<InstanceID,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get 
            {
                if (underlyingInstanceSenderCollection != null)
                    return GetSender(destinationAddress);
                else
                    throw new NotSupportedException("No underlying instance senders defined.");
            }
        }

        #endregion

        #region Class ChunkSet

        private class ChunkSet
        {
            public ChunkSet()
            {
                chunks = new List<Chunk>();
                bytesCollected = 0;
                bytesNeeded = int.MaxValue;
            }

            private List<Chunk> chunks;
            private int bytesCollected, bytesNeeded;

            public void addChunk(Chunk chunk)
            {
                while (chunk.ChunkNo >= chunks.Count)
                    chunks.Add(null);

                if (chunks[chunk.ChunkNo] == null)
                {
                    chunks[chunk.ChunkNo] = chunk;
                    bytesCollected += chunk.ChunkSize;

                    if (chunk.ChunkNo == 0)
                        bytesNeeded = (int) chunk.ObjectSize;
                }
            }

            public bool Ready
            {
                get { return bytesCollected == bytesNeeded; }
            }

            public QS._core_c_.Base3.Message Message
            {
                get
                {
                    QS.Fx.Base.ConsumableBlock segment = new QS.Fx.Base.ConsumableBlock((uint) bytesNeeded);
                    foreach (Chunk chunk in chunks)
                        chunk.ChunkSegments.SerializeTo(ref segment);
                    segment.reset();

                    QS.Fx.Serialization.ISerializable assembledObject = QS._core_c_.Base3.Serializer.CreateObject(chunks[0].ObjectClassID);

                    QS.Fx.Base.ConsumableBlock header =
                        new QS.Fx.Base.ConsumableBlock(segment.Array, (uint) segment.Offset, (uint) chunks[0].HeaderSize);
                    segment.consume((int) chunks[0].HeaderSize);

                    assembledObject.DeserializeFrom(ref header, ref segment);

                    return new QS._core_c_.Base3.Message(chunks[0].DestinationLOID, assembledObject);
                }
            }
        }

        #endregion

		protected override Base3_.IReliableSerializableSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, destinationAddress);
        }

        #region Receive Callback

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_ChoppingSender
            logger.Log(this, "__ReceiveCallback: " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

            Chunk chunk = receivedObject as Chunk;
            if (chunk != null)
            {
                lock (this)
                {
                    ChunkSet chunkSet;
                    if (!assembledChunks.ContainsKey(chunk.SeqNo))
                        assembledChunks[chunk.SeqNo] = (chunkSet = new ChunkSet());
                    else
                        chunkSet = assembledChunks[chunk.SeqNo];

                    chunkSet.addChunk(chunk);
                    if (chunkSet.Ready)
                    {
                        QS._core_c_.Base3.Message message = chunkSet.Message;
                        assembledChunks.Remove(chunk.SeqNo);

                        demultiplexer.dispatch(message.destinationLOID, sourceIID, message.transmittedObject);
                    }
                }
            }
            else
            {
                logger.Log(this, "__ReceiveCallback : Unknown object received : " + 
					QS._core_c_.Helpers.ToString.ObjectRef(receivedObject));
            }

            return null;
        }

        #endregion

        #region Class Chunk

        [QS.Fx.Serialization.ClassID(ClassID.ChoppingSender_Chunk)]
        public class Chunk : QS.Fx.Serialization.ISerializable
        {
            public Chunk()
            {
            }

            public Chunk(uint seqno, uint destinationLOID, ushort classID, uint headerSize, uint objectSize, Base3_.Segments chunk)
            {
                this.seqno = seqno;
                this.destinationLOID = destinationLOID;
                this.classID = classID;
                this.headerSize = headerSize;
                this.objectSize = objectSize;
                this.chunkno = 0;
                this.chunk = chunk;
            }

            public Chunk(uint seqno, ushort chunkno, Base3_.Segments chunk)
            {
                this.seqno = seqno;
/*
                this.destinationLOID = 0;
                this.classID = 0;
                this.objectSize = objectSize;
*/
                this.chunkno = chunkno;
                this.chunk = chunk;
            }

            private uint seqno, destinationLOID, objectSize, headerSize;
            private ushort classID, chunkno;
            private Base3_.Segments chunk;

            #region Accessors

	        public uint SeqNo 
	        {
                get { return seqno; }
	        }

            public ushort ChunkNo
            {
                get { return chunkno; }
            }

            public ushort ChunkSize
            {
                get { return chunk.SegmentSize; }
            }

            public uint HeaderSize
            {
                get { return headerSize; }
            }

            public uint ObjectSize
            {
                get { return objectSize; }
            }

            public uint DestinationLOID
            {
                get { return destinationLOID; }
            }

            public ushort ObjectClassID
            {
                get { return classID; }
            }

            public Base3_.Segments ChunkSegments
            {
                get { return chunk; }
            }

            #endregion

            private const int headerOverhead = 4 * sizeof(uint) + 2 * sizeof(ushort);
            public static int HeaderOverhead
            {
                get { return headerOverhead; }
            }

            private const int appendOverhead = sizeof(uint) + sizeof(ushort);
            public static int AppendOverhead
            {
                get { return appendOverhead; }
            }

            #region ISerializable Members

            public QS.Fx.Serialization.SerializableInfo SerializableInfo
            {
                get 
                { 
                    return chunk.SerializableInfo.Extend(
                        (ushort) ClassID.ChoppingSender_Chunk, (ushort) ((chunkno > 0) ? appendOverhead : headerOverhead), 0, 0);
                }
            }

            public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    *((uint *)headerptr) = seqno;
                    *((ushort *)(headerptr + sizeof(uint))) = chunkno;

                    if (chunkno > 0)
                        header.consume(appendOverhead);
                    else
                    {
                        *((uint*)(headerptr + sizeof(uint) + sizeof(ushort))) = destinationLOID;
                        *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort))) = objectSize;
                        *((ushort*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))) = classID;
                        *((uint*)(headerptr + 3 * sizeof(uint) + 2 * sizeof(ushort))) = headerSize;

                        header.consume(headerOverhead);
                    }
                }

                chunk.SerializeTo(ref header, ref data);
            }

            public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* arrayptr = header.Array)
                {
                    byte* headerptr = arrayptr + header.Offset;
                    seqno = *((uint*)headerptr);
                    chunkno = *((ushort*)(headerptr + sizeof(uint)));

                    if (chunkno > 0)
                        header.consume(appendOverhead);
                    else
                    {
                        destinationLOID = *((uint*)(headerptr + sizeof(uint) + sizeof(ushort)));
                        objectSize = *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort)));
                        classID = *((ushort*)(headerptr + 3 * sizeof(uint) + sizeof(ushort)));
                        headerSize  = *((uint*)(headerptr + 3 * sizeof(uint) + 2 * sizeof(ushort)));

                        header.consume(headerOverhead);
                    }
                }

                chunk = new QS._qss_c_.Base3_.Segments();
                chunk.DeserializeFrom(ref header, ref data);
            }

            #endregion

            public override string ToString()
            {
                return "(seqno:" + seqno.ToString() + ", chunkno:" + chunkno.ToString() + ((chunkno == 0) ? (", headerSize:" + 
                    headerSize.ToString() + ", objectSize:" + objectSize.ToString() + ", classID:" + classID.ToString() + ", destinationLOID:" + 
                    destinationLOID.ToString()) : ("")) + ", chunk:{" + chunk.ToString() + "})";
            }
        }

        #endregion

        #region Class Sender

        public class Sender : Base3_.IReliableSerializableSender
        {
            public Sender(ChoppingSender owner, QS._core_c_.Base3.InstanceID destinationAddress)
            {
                this.owner = owner;
                this.underlyingSender = owner.underlyingInstanceSenderCollection[destinationAddress];

                owner.logger.Log(this, 
                    "__ChoppingSender(" + destinationAddress.ToString() + ") : Underlying MTU = " + underlyingSender.MTU.ToString());
            }

            public Sender(ChoppingSender owner, QS.Fx.Network.NetworkAddress destinationAddress)
            {
                this.owner = owner;
                this.underlyingSender = owner.underlyingSenderCollection[destinationAddress];
            }

            private ChoppingSender owner;
            private Base3_.IReliableSerializableSender underlyingSender;

            private uint lastused_seqno = 0;

            #region IReliableSerializableSender Members

            QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
                if (data.SerializableInfo.Size <= underlyingSender.MTU)
                {
                    underlyingSender.BeginSend(destinationLOID, data, null, null);
                }
                else
                {
                    Choppable choppable = new Choppable(data);

                    lock (this)
                    {
                        uint my_seqno = ++lastused_seqno;
                        Chunk chunk = new Chunk(my_seqno, destinationLOID, data.SerializableInfo.ClassID, (uint) data.SerializableInfo.HeaderSize, 
                            (uint) data.SerializableInfo.Size, choppable.ChopOff(underlyingSender.MTU - Chunk.HeaderOverhead));
                        underlyingSender.BeginSend((ushort)ReservedObjectID.ChoppingSender, chunk, null, null);

                        Base3_.Segments choppedSegments;
                        ushort chunkno = 0;
                        while ((choppedSegments = choppable.ChopOff(underlyingSender.MTU - Chunk.AppendOverhead)) != null)
                        {
                            chunk = new Chunk(my_seqno, ++chunkno, choppedSegments);
                            underlyingSender.BeginSend((ushort)ReservedObjectID.ChoppingSender, chunk, null, null);
                        }
                    }
                }

                return null;
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {
                throw new NotImplementedException();
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { return underlyingSender.Address; }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((QS._qss_c_.Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { return int.MaxValue; } // needs to be changed
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}
