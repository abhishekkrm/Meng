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

// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Batching_
{
    // TODO: Add synchronization......................

    public sealed class BatchingSink : QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>
    {
        public BatchingSink(QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Logging.ILogger logger, 
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> underlyingSink, int capacity)
        {
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.logger = logger;
            this.underlyingSink = underlyingSink;
            this.capacity = capacity;
            this.completionCallback = new QS._core_c_.Base6.CompletionCallback<object>(this.CompletionCallback);
            this.maximumConsumed = capacity - HeaderOverhead;
        }

        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock; 
        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> underlyingSink;
        private int capacity, maximumConsumed;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> callbacks =
            new Queue<QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>();
        private bool registered;
        private Queue<Chunk> pending = new Queue<Chunk>();
        private QS._core_c_.Base6.CompletionCallback<object> completionCallback;
        
        private Queue<Chunk> chunks = new Queue<Chunk>();
        private Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> workingQueue = new Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>();

        private const int ChunkOverhead = sizeof(uint) + sizeof(ushort);
        private const int HeaderOverhead = sizeof(ushort);

        #region GetObjectsCallback

        private void GetObjectsCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> outgoing,
            int maximumNumberOfObjects,
#if UseEnhancedRateControl
            int maximumNumberOfBytes, 
#endif
            out int numberOfObjectsReturned,
#if UseEnhancedRateControl    
            out int numberOfBytesReturned,
#endif
            out bool moreObjectsAvailable)
        {
            // TODO: Implement enhanced rate control

            numberOfObjectsReturned = 0;
#if UseEnhancedRateControl    
            numberOfBytesReturned = 0;
#endif
            moreObjectsAvailable = true;

            while (moreObjectsAvailable && numberOfObjectsReturned < maximumNumberOfObjects) // && numberOfBytesReturned < maximumNumberOfBytes)
            {
                int consumed = 0, consumed_header = 0, consumed_buffers = 0, nelements = 0;
                bool wrapped = false;

                while (moreObjectsAvailable && !wrapped)
                {
                    if (pending.Count > 0)
                    {
                        if (consumed + pending.Peek().info.Size + ChunkOverhead <= maximumConsumed)
                        {
                            Chunk chunk = pending.Dequeue();
                            chunks.Enqueue(chunk);
                            consumed += chunk.info.Size + ChunkOverhead;
                            consumed_header += chunk.info.HeaderSize + ChunkOverhead;
                            consumed_buffers += chunk.info.NumberOfBuffers;
                            nelements++;
                        }
                        else
                        {
                            outgoing.Enqueue(new Request(this, chunks, consumed, consumed_header, consumed_buffers, nelements));
                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += .................................................HERE
                            wrapped = true;
                            chunks = new Queue<Chunk>();
                        }
                    }
                    else if (callbacks.Count > 0)
                    {
                        // TODO: Implement enhanced rate control

                        QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> callback = callbacks.Dequeue();
                        int nreturned;
#if UseEnhancedRateControl
                        int nbytesreturned;
#endif
                        bool more;
                        callback(workingQueue, 1, 
#if UseEnhancedRateControl
                            int.MaxValue, // FIX THIS .............................................................................................HERE
#endif
                            out nreturned, 
#if UseEnhancedRateControl                            
                            out nbytesreturned, 
#endif                            
                            out more);

                        if (more)
                            callbacks.Enqueue(callback);

                        if (nreturned > 0)
                        {
                            foreach (QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> message in workingQueue)
                                pending.Enqueue(new Chunk(message));
                            workingQueue.Clear();
                        }
                    }
                    else
                    {
                        moreObjectsAvailable = registered = false;

                        if (nelements > 0)
                        {
                            outgoing.Enqueue(new Request(this, chunks, consumed, consumed_header, consumed_buffers, nelements));
                            numberOfObjectsReturned++;
                            // numberOfBytesReturned += .................................................HERE
                            wrapped = true;
                            chunks = new Queue<Chunk>();
                        }
                    }
                }
            }
        }

        #endregion

        #region Callback

        private void CompletionCallback(bool succeeded, System.Exception exception, object context)
        {
            Request request = (Request) context;

            foreach (Chunk chunk in request.chunks)
            {
                if (chunk.request.CompletionCallback != null)
                    chunk.request.CompletionCallback(succeeded, exception, chunk.request.Context);
            }
        }

        #endregion

        #region Struct Chunk

        public struct Chunk
        {
            public Chunk(QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request)
            {
                this.request = request;
                this.info = request.Argument.transmittedObject.SerializableInfo;
            }

            public QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message> request;
            public QS.Fx.Serialization.SerializableInfo info;
        }

        #endregion

        #region Class Request

        public class Request : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>, QS.Fx.Serialization.ISerializable
        {
            public Request(BatchingSink owner, Queue<Chunk> chunks, int consumed, int consumed_header, int consumed_buffers, int nelements)
            {
                this.chunks = chunks;
                this.owner = owner;
                this.consumed_header = consumed_header;
                this.consumed_buffers = consumed_buffers;
                this.consumed = consumed;
                this.nelements = nelements;
            }

            private BatchingSink owner;
            private int consumed, consumed_header, consumed_buffers, nelements;
            
            public Queue<Chunk> chunks;

            #region IAsynchronous<Message,object> Members

            QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
            {
                get { return new QS._core_c_.Base3.Message((uint)ReservedObjectID.Batching_Unpacker, this); }
            }

            object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
            {
                get { return this; }
            }

            QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
            {
                get { return owner.completionCallback; }
            }

            #endregion

            #region ISerializable Members

            QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get 
                {
                    return new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Batching_Unpacker_Request,
                        consumed_header + HeaderOverhead, consumed + HeaderOverhead, consumed_buffers);
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* parray = header.Array)
                {
                    byte* pheader = parray + header.Offset;
                    *((ushort*)pheader) = (ushort) nelements;
                    header.consume(sizeof(ushort));                    

                    foreach (Chunk chunk in chunks)
                    {
                        pheader = parray + header.Offset;
                        *((uint*)pheader) = chunk.request.Argument.destinationLOID;
                        *((ushort*)(pheader + sizeof(uint))) = chunk.info.ClassID;
                        header.consume(sizeof(uint) + sizeof(ushort));

                        chunk.request.Argument.transmittedObject.SerializeTo(ref header, ref data);
                    }
                }
            }

            void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                throw new NotSupportedException();
            }

            #endregion
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        void QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> getObjectsCallback)
        {
            callbacks.Enqueue(getObjectsCallback);
            if (!registered)
                underlyingSink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this.GetObjectsCallback));
        }

        int QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
