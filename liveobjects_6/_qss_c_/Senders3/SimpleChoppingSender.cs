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

// #define DEBUG_SimpleChoppingSender

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders3
{
    public class SimpleChoppingSender :
        Base3_.ISenderCollection<QS.Fx.Network.NetworkAddress , Base3_.IReliableSerializableSender>,
        Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>,
        Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender>
    {
        private const int MaximumChunk = 10000;

        public SimpleChoppingSender(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer,
            Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingSenderCollection,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> underlyingInstanceSenderCollection)
        {
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.underlyingInstanceSenderCollection = underlyingInstanceSenderCollection;
            this.underlyingSenderCollection = underlyingSenderCollection;

            demultiplexer.register((uint)QS.ReservedObjectID.SimpleChoppingSender, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> underlyingInstanceSenderCollection;
        private Base3_.ISenderCollection<Base3_.IReliableSerializableSender> underlyingSenderCollection;
        private IDictionary<QS.Fx.Network.NetworkAddress, Sender> senders = new Dictionary<QS.Fx.Network.NetworkAddress, Sender>();
        private IDictionary<QS._core_c_.Base3.InstanceID, Sender> instance_senders = new Dictionary<QS._core_c_.Base3.InstanceID, Sender>();
        private IDictionary<QS._core_c_.Base3.InstanceID, IDictionary<uint, ReceivedChunks>> received =
            new Dictionary<QS._core_c_.Base3.InstanceID, IDictionary<uint, ReceivedChunks>>();

        #region Class ReceivedChunks

        private class ReceivedChunks
        {
            public ReceivedChunks(int nchunks)
            {
                this.chunks = new Chunk[nchunks];
            }

            public int nreceived;
            public Chunk[] chunks;

            public void Add(Chunk chunk)
            {
                if (chunks[chunk.index] == null)
                {
                    chunks[chunk.index] = chunk;
                    nreceived++;
                }
            }

            public bool Ready
            {
                get { return nreceived == chunks.Length; }
            }
        }

        #endregion

        #region ReceiveCallback

        unsafe private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable data)
        {
            IDictionary<uint, ReceivedChunks> chunkmap;
            if (!received.TryGetValue(address, out chunkmap))
                received.Add(address, chunkmap = new Dictionary<uint, ReceivedChunks>());

            Chunk chunk = ((Chunk) data);
            ReceivedChunks partial;
            if (!chunkmap.TryGetValue(chunk.seqno, out partial))
                chunkmap.Add(chunk.seqno, partial = new ReceivedChunks((int) chunk.count));

#if DEBUG_SimpleChoppingSender
            logger.Log(this, "Receiving (" + chunk.seqno.ToString() + ") - Chunk " + (chunk.index + 1).ToString() + " / " + chunk.count.ToString());
#endif

            partial.Add(chunk);

            if (partial.Ready)
            {
                uint destinationLOID;
                int count;
                fixed (byte* pfirst = partial.chunks[0].bytes)
                {
                    destinationLOID  = *((uint*)pfirst);
                    count = (int)(*((uint*)(pfirst + sizeof(uint))));
                }

#if DEBUG_SimpleChoppingSender
                logger.Log(this, "Receiving (" + chunk.seqno.ToString() + ", " + count.ToString() + " bytes)");
#endif

                byte[] contiguous_bytes = new byte[count];
                int offset = 0;
                for (int ind = 0; ind < partial.chunks.Length; ind++)
                {
                    Buffer.BlockCopy(partial.chunks[ind].bytes, 0, contiguous_bytes, offset, partial.chunks[ind].bytes.Length);
                    offset += partial.chunks[ind].bytes.Length;
                }

                chunkmap.Remove(chunk.seqno);
                
                QS.Fx.Serialization.ISerializable obj = QS._core_c_.Base3.Serializer.FromSegment(
                    new ArraySegment<byte>(contiguous_bytes, 2 * sizeof(uint), count - 2 * sizeof(uint)));

                demultiplexer.dispatch(destinationLOID, address, obj);
            }

            return null;
        }

        #endregion

        #region Class Chunk

        [QS.Fx.Serialization.ClassID(ClassID.SimpleChoppingSender_Chunk)]
        public class Chunk : QS.Fx.Serialization.ISerializable
        {
            public const int HeaderOverhead = 4 * sizeof(uint);

            public Chunk()
            {
            }

            public Chunk(uint seqno, uint index, uint count, byte[] bytes)
            {
                this.seqno = seqno;
                this.index = index;
                this.count = count;
                this.bytes = bytes;
            }

            public uint seqno, index, count;
            public byte[] bytes;

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    return new QS.Fx.Serialization.SerializableInfo(
                        (ushort)ClassID.SimpleChoppingSender_Chunk, HeaderOverhead, HeaderOverhead + bytes.Length, 1);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* parray = header.Array)
                {
                    byte* pheader = parray + header.Offset;
                    *((uint*)(pheader)) = index;
                    *((uint*)(pheader + sizeof(uint))) = count;
                    *((uint*)(pheader + 2 * sizeof(uint))) = (uint) bytes.Length;
                    *((uint*)(pheader + 3 * sizeof(uint))) = seqno;
                }
                header.consume(HeaderOverhead);
                data.Add(new QS.Fx.Base.Block(bytes));
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* parray = header.Array)
                {
                    byte* pheader = parray + header.Offset;
                    index = *((uint*)(pheader));
                    count = *((uint*)(pheader + sizeof(uint)));
                    bytes = new byte[*((uint*)(pheader + 2 * sizeof(uint)))];
                    seqno = *((uint*)(pheader + 3 * sizeof(uint)));
                }
                header.consume(HeaderOverhead);
                Buffer.BlockCopy(data.Array, data.Offset, bytes, 0, bytes.Length);
                data.consume(bytes.Length);
            }

            #endregion
        }

        #endregion

        #region Class Sender

        private class Sender : Base3_.IReliableSerializableSender
        {
            public Sender(SimpleChoppingSender owner, Base3_.IReliableSerializableSender underlyingSender)
            {
                this.owner = owner;
                this.underlyingSender = underlyingSender;
                this.maximumChunkSize = Math.Min(MaximumChunk, underlyingSender.MTU - Chunk.HeaderOverhead);
            }

            private SimpleChoppingSender owner;
            private Base3_.IReliableSerializableSender underlyingSender;
            private int maximumChunkSize;
            private uint lastseqno;

            #region IReliableSerializableSender Members

            unsafe QS._qss_c_.Base3_.IAsynchronousOperation QS._qss_c_.Base3_.IReliableSerializableSender.BeginSend(
                uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, 
                object asynchronousState)
            {
                IList<QS.Fx.Base.Block> segments = QS._core_c_.Base3.Serializer.ToSegments(data);
                int count = 2 * sizeof(uint);
                foreach (QS.Fx.Base.Block segment in segments)
                    count += (int) segment.size;

                uint seqno = ++lastseqno;

#if DEBUG_SimpleChoppingSender
                owner.logger.Log(this, "Sending (" + seqno.ToString() + ", " + count.ToString() + " bytes)");
#endif

                byte[] contiguous_bytes = new byte[count];
                fixed (byte* pcontiguous_bytes = contiguous_bytes)
                {
                    *((uint*) pcontiguous_bytes) = destinationLOID;
                    *((uint*) (pcontiguous_bytes + sizeof(uint))) = (uint) count;
                }

                int offset = 2 * sizeof(uint);
                foreach (QS.Fx.Base.Block segment in segments)
                {
                    if ((segment.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && segment.buffer != null)
                        Buffer.BlockCopy(segment.buffer, (int) segment.offset, contiguous_bytes, offset, (int) segment.size);
                    else
                        throw new Exception("Unmanaged memory not supported here.");

                    offset += (int) segment.size;
                }

                int nchunks = (int) Math.Ceiling(((double)count) / ((double)maximumChunkSize));
                byte[][] chunks = new byte[nchunks][];
                for (int ind = 0; ind < nchunks; ind++)
                {
                    chunks[ind] = new byte[Math.Min(maximumChunkSize, count - ind * maximumChunkSize)];
                    Buffer.BlockCopy(contiguous_bytes, ind * maximumChunkSize, chunks[ind], 0, chunks[ind].Length);
                }

                int index = 0;
                foreach (byte[] chunk in chunks)
                {
#if DEBUG_SimpleChoppingSender
                    owner.logger.Log(this, "Sending (" + seqno.ToString() + ") - Chunk " + (index + 1).ToString() + " / " + nchunks.ToString());
#endif

                    underlyingSender.BeginSend(
                        (uint)ReservedObjectID.SimpleChoppingSender, new Chunk(seqno, (uint)(index++), (uint)nchunks, chunk), null, null);
                }

                return null;
            }

            void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
            {                
            }

            #endregion

            #region ISerializableSender Members

            QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
            {
                get { throw new NotSupportedException(); }
            }

            void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                ((QS._qss_c_.Base3_.IReliableSerializableSender)this).BeginSend(destinationLOID, data, null, null);
            }

            int QS._qss_c_.Base3_.ISerializableSender.MTU
            {
                get { return int.MaxValue; }
            }

            #endregion

            #region IComparable Members

            int IComparable.CompareTo(object obj)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        #region GetSender

        private Sender GetSender(QS.Fx.Network.NetworkAddress address)
        {
            lock (senders)
            {
                if (senders.ContainsKey(address))
                    return senders[address];
                else
                {
                    Sender sender = new Sender(this, underlyingSenderCollection[address]);
                    senders.Add(address, sender);
                    return sender;
                }
            }
        }

        private Sender GetSender(QS._core_c_.Base3.InstanceID address)
        {
            lock (instance_senders)
            {
                if (instance_senders.ContainsKey(address))
                    return instance_senders[address];
                else
                {
                    Sender sender = new Sender(this, underlyingInstanceSenderCollection[address]);
                    instance_senders.Add(address, sender);
                    return sender;
                }
            }
        }

        #endregion

        #region ISenderCollection<InstanceID,IReliableSerializableSender> Members

        Base3_.IReliableSerializableSender 
            Base3_.ISenderCollection<QS.Fx.Network.NetworkAddress , Base3_.IReliableSerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region IAttributeCollection Members

        IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ISenderCollection<ISerializableSender> Members

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion

        #region ISenderCollection<InstanceID,IReliableSerializableSender> Members

        QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS._core_c_.Base3.InstanceID destinationAddress]
        {
            get { return GetSender(destinationAddress); }
        }

        #endregion
    }
}
