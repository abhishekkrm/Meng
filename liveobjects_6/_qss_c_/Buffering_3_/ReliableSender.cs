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
    public class ReliableSender : Base3_.SenderClass<QS._qss_c_.Base3_.ISerializableSender>
    {
        public ReliableSender(QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection,
            Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> acknowledgementSenderCollection, Base3_.IDemultiplexer demultiplexer, 
            IControllerClass controllerClass, QS.Fx.Clock.IAlarmClock alarmClock, int windowSize, double retransmissionTimeout)
        {
            this.logger = logger;
            this.underlyingCollection = underlyingCollection;
            this.acknowledgementSenderCollection = acknowledgementSenderCollection;
            this.demultiplexer = demultiplexer;
            this.controllerClass = controllerClass;
            this.alarmClock = alarmClock;
            this.windowSize = windowSize;
            this.retransmissionTimeout = retransmissionTimeout;

            demultiplexer.register((uint)QS.ReservedObjectID.ReliableSender_MessageChannel, 
                new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));
            demultiplexer.register((uint)QS.ReservedObjectID.ReliableSender_AcknowledgementChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.acknowledgementCallback));
        }

        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection, acknowledgementSenderCollection;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private IControllerClass controllerClass;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private int windowSize;
        private double retransmissionTimeout;

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Sender.IncomingRequest incomingRequest = receivedObject as Sender.IncomingRequest;
            if (incomingRequest != null)
            {


/*
                acknowledgementSenderCollection[sourceAddress].send(
                    QS.ReservedObjectID.ReliableSender_AcknowledgementChannel, new Components.SeqNo(incomingRequest.SeqNo));
*/
            }

            return null;
        }

		private QS.Fx.Serialization.ISerializable acknowledgementCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Components_1_.SeqNo seqNo = receivedObject as Components_1_.SeqNo;
            if (seqNo != null)
            {
                



            }

            return null;
        }

		protected override QS._qss_c_.Base3_.ISerializableSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, destinationAddress);
        }

        public class Sender : Base3_.SerializableSender
        {
            public Sender(ReliableSender owner, QS.Fx.Network.NetworkAddress destinationAddress) : base(owner.SenderCollection[destinationAddress])
            {
                this.owner = owner;
                this.controller = owner.controllerClass.CreateController(underlyingSender.MTU);
                this.outgoingWindow = new FlowControl_1_.OutgoingWindow((uint) owner.windowSize);
                alarmCallback = new QS.Fx.Clock.AlarmCallback(this.retransmissionCallback);
            }

            private ReliableSender owner;
            private IController controller;
            private FlowControl_1_.IOutgoingWindow outgoingWindow;
            private QS.Fx.Clock.AlarmCallback alarmCallback;

            private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                
            }

            #region QS.CMS.Base3.ISerializableSender Members

            public override void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                if (outgoingWindow.hasSpace())
                {
                    OutgoingRequest request = new OutgoingRequest(destinationLOID, data);
                    uint seqno = outgoingWindow.append(request);
                    request.SeqNo = seqno;
                    request.alarmRef = owner.alarmClock.Schedule(owner.retransmissionTimeout, alarmCallback, seqno);

                    underlyingSender.send((uint) QS.ReservedObjectID.ReliableSender_MessageChannel, request);
                }
            }

            public override int MTU
            {
                get { return controller.MTU; }
            }

            #endregion

            #region Class OutgoingRequest

            private class OutgoingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
            {
                public OutgoingRequest(uint destinationLOID, QS.Fx.Serialization.ISerializable data) : base(new QS._core_c_.Base3.Message(destinationLOID, data))
                {
                }

                public QS.Fx.Clock.IAlarm alarmRef;

                public override ClassID ClassID
                {
                    get { return QS.ClassID.ReliableSender_OurMessage; }
                }
            }

            #endregion

            #region Class IncomingRequest

            [QS.Fx.Serialization.ClassID(QS.ClassID.ReliableSender_OurMessage)]
            public class IncomingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
            {
                public IncomingRequest()
                {
                }
            }

            #endregion
        }
    }
}
