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

#define DEBUG_SimpleRS

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Scattering_1_
{
    public class SimpleRS : IRetransmittingScatterer
    {
        public SimpleRS(Base2_.IBlockSender underlyingSender, IScatterer underlyingScatterer, QS.Fx.Clock.IAlarmClock alarmClock, 
            Base2_.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger, uint initialWindowSize, double retransmissionTimeout)
        {
            this.underlyingSender = underlyingSender;
            this.logger = logger;
            this.underlyingScatterer = underlyingScatterer;
            this.alarmClock = alarmClock;
            this.demultiplexer = demultiplexer;
            this.retransmissionTimeout = retransmissionTimeout;
            outgoingWindow = new FlowControl_1_.OutgoingWindow(initialWindowSize);
            outgoingQueue = new Collections_1_.RawQueue();
            this.alarmCallback = new QS.Fx.Clock.AlarmCallback(retransmissionCallback);

            QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.SimpleRS_Message, typeof(SimpleRS.Message));
            QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.SimpleRS_Ack, typeof(SimpleRS.Acknowledgement));

            demultiplexer.register((uint)ReservedObjectID.SimpleRS_Messages, new QS._qss_c_.Base2_.ReceiveCallback(this.messageCallback));
            demultiplexer.register((uint)ReservedObjectID.SimpleRS_Acknowledgements, new QS._qss_c_.Base2_.ReceiveCallback(this.acknowledgementCallback));
        }

        private Base2_.IBlockSender underlyingSender;
        private QS.Fx.Logging.ILogger logger;
        private IScatterer underlyingScatterer;
        private Base2_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private double retransmissionTimeout;
        private FlowControl_1_.IOutgoingWindow outgoingWindow;
        private Collections_1_.IRawQueue outgoingQueue;
        private QS.Fx.Clock.AlarmCallback alarmCallback;

        #region Class Message

        [QS.Fx.Serialization.ClassID(QS.ClassID.SimpleRS_Message)]
        public class Message : QS._core_c_.Base2.IBase2Serializable
        {
            public Message()
            {
            }

            public Message(uint seqno, QS._core_c_.Base2.IBase2Serializable message)
            {
                this.seqno = seqno;
                this.message = message;
            }

            public uint seqno;
            public QS._core_c_.Base2.IBase2Serializable message;

            #region Base2.ISerializable Members

            public uint Size
            {
                get
                {
                    return QS._core_c_.Base2.SizeOf.UInt32 + QS._core_c_.Base2.SizeOf.UInt16 + message.Size;
                }
            }

            public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
            {
                seqno = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
                message = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject((QS.ClassID) QS._core_c_.Base2.Serializer.loadUInt16(blockOfData));
                message.load(blockOfData);
            }

            public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
            {
                QS._core_c_.Base2.Serializer.saveUInt32(seqno, blockOfData);
                QS._core_c_.Base2.Serializer.saveUInt16((ushort) message.ClassID, blockOfData);
                message.save(blockOfData);
            }

            public QS.ClassID ClassID
            {
                get
                {
                    return ClassID.SimpleRS_Message;
                }
            }            

            #endregion
        }

        #endregion

        #region Class Acknowledgement

        [QS.Fx.Serialization.ClassID(QS.ClassID.SimpleRS_Ack)]
        public class Acknowledgement : QS._core_c_.Base2.IBase2Serializable
        {
            public Acknowledgement()
            {
            }

            public Acknowledgement(uint seqno)
            {
                this.seqno = seqno;
            }

            public uint seqno;

            #region Base2.ISerializable Members

            public uint Size
            {
                get
                {
                    return QS._core_c_.Base2.SizeOf.UInt32;
                }
            }

            public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
            {
                seqno = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
            }

            public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
            {
                QS._core_c_.Base2.Serializer.saveUInt32(seqno, blockOfData);
            }

            public QS.ClassID ClassID
            {
                get
                {
                    return ClassID.SimpleRS_Ack;
                }
            }

            #endregion
        }

        #endregion

        private QS._core_c_.Base2.IBase2Serializable messageCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            Message message = (Message)serializableObject;
            underlyingSender.send((uint) ReservedObjectID.SimpleRS_Acknowledgements, sourceAddress, new Acknowledgement(message.seqno));
            RetransmittingScatterer.WrappedMessage wrappedGuy = (RetransmittingScatterer.WrappedMessage) message.message;

            return this.demultiplexer.demultiplex(wrappedGuy.DestinationLOID, sourceAddress, destinationAddress, wrappedGuy.Message);
        }

        private QS._core_c_.Base2.IBase2Serializable acknowledgementCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            Acknowledgement ack = (Acknowledgement) serializableObject;            
            lock (this)
            {
#if DEBUG_SimpleRS
                // logger.Log(this, "Acknowledged " + ack.seqno.ToString() + " by host " + sourceAddress.ToString());
#endif

                IRetransmittingScattererRequest request =
                    (IRetransmittingScattererRequest) outgoingWindow.lookup(ack.seqno);
                if (request != null)
                {
                    if (request.AddressSet.acknowledged(sourceAddress))
                    {
                        request.AlarmRef.Cancel();
                        outgoingWindow.remove(ack.seqno);

                        while (outgoingWindow.hasSpace() && outgoingQueue.size() > 0)
                        {
                            request = (IRetransmittingScattererRequest)outgoingQueue.dequeue();
                            processOutgoing(request);
                        }
                    }
                }
                else
                {
#if DEBUG_SimpleRS
                    // logger.Log(this, "Acknowledgement " + ack.seqno.ToString() + " is left unmatched.");
#endif
                }
            }
            
            return null;
        }

        private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            uint seqno = (uint) alarmRef.Context;

#if DEBUG_SimpleRS
            // logger.Log(this, "Retransmitting " + seqno.ToString());
#endif

            lock (this)
            {
                IRetransmittingScattererRequest request = (IRetransmittingScattererRequest)outgoingWindow.lookup(seqno);
                if (request != null)
                {
                    underlyingScatterer.scatter(request.AddressSet.ScatterAddress, (uint) ReservedObjectID.SimpleRS_Messages, new Message(seqno, request));
                    alarmRef.Reschedule();
                }
            }
        }

        private void processOutgoing(IRetransmittingScattererRequest request)
        {
            uint seqno = outgoingWindow.append(request);

#if DEBUG_SimpleRS
            // logger.Log(this, "Sending out as " + seqno.ToString() + " object " + request.ToString());
#endif

            underlyingScatterer.scatter(request.AddressSet.ScatterAddress, (uint) ReservedObjectID.SimpleRS_Messages, new Message(seqno, request));
            request.AlarmRef = alarmClock.Schedule(retransmissionTimeout, alarmCallback, seqno);
        }

        #region IRetransmittingScatterer Members

		public void multicast(uint destinationLOID, Scattering_1_.IScatterSet destinationAddressSet, Base2_.IIdentifiableObject message, 
            CompletionCallback completionCallback)
        {
            throw new NotImplementedException();
        }

		public void multicast(IRetransmittingScattererRequest request)
        {
            if ((request is Collections_1_.ILinkable) && (request is RetransmittingScatterer.WrappedMessage))
            {
                lock (this)
                {
                    if (outgoingWindow.hasSpace())
                    {
                        processOutgoing(request);
                    }
                    else
                        outgoingQueue.enqueue((Collections_1_.ILinkable)request);
                }
            }
            else
                throw new Exception("Scatter request must be of type QS.CMS.Collections.ILinkable and QS.CMS.RetransmittingScatterer.WrappedMessage");
        }

		public uint MTU
		{
			get
            {
                throw new NotImplementedException();
            }
		}

		public double RetransmissionTimeout
		{
			get
            {
                throw new NotImplementedException();
            }

			set
            {
                throw new NotImplementedException();
            }
        }

		public Base2_.IIdentifiableObjectContainer IOContainer
		{
			set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
