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

namespace QS._qss_c_.Receivers5
{
    [QS.Fx.Base.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class InstanceReceiverController : QS.Fx.Inspection.Inspectable, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public InstanceReceiverController(
            QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger,
            Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, uint messageChannel,
            IInstanceReceiverClass receiverClass)
        {
            this.localAddress = localAddress;
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.demultiplexer = demultiplexer;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.receiverClass = receiverClass;

            demultiplexer.register(messageChannel, // (uint)ReservedObjectID.Receivers5_InstanceReceiver_MessageChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.MessageReceiveCallback));
        }

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private Base3_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<QS._core_c_.Base3.InstanceID, ReceiverContext> contexts = new Dictionary<QS._core_c_.Base3.InstanceID, ReceiverContext>();
        private IInstanceReceiverClass receiverClass;

        #region Class ReceiverContext

        [QS._core_c_.Diagnostics.ComponentContainer]
        [QS.Fx.Base.Inspectable]
        private class ReceiverContext : QS.Fx.Inspection.Inspectable, IInstanceReceiverContext
        {
            public ReceiverContext(InstanceReceiverController owner, QS._core_c_.Base3.InstanceID sourceAddress)
            {
                this.owner = owner;
                this.sourceAddress = sourceAddress;
            }

            private InstanceReceiverController owner;
            private QS._core_c_.Base3.InstanceID sourceAddress;
            [QS._core_c_.Diagnostics.Component]
            private IInstanceReceiver receiver;

            #region Accessors

            public IInstanceReceiver Receiver
            {
                get { return receiver; }
                set { receiver = value; }
            }

            #endregion

            #region IInstanceReceiverContext Members

            QS._core_c_.Base3.InstanceID IInstanceReceiverContext.SourceAddress
            {
                get { return sourceAddress; }
            }

            QS.Fx.Clock.IAlarmClock IInstanceReceiverContext.AlarmClock
            {
                get { return owner.alarmClock; }
            }

            QS.Fx.Clock.IClock IInstanceReceiverContext.Clock
            {
                get { return owner.clock; }
            }

            Base3_.IDemultiplexer IInstanceReceiverContext.Demultiplexer
            {
                get { return owner.demultiplexer; }
            }

            #endregion
        }

        #endregion

        #region GetContext

        private ReceiverContext GetContext(QS._core_c_.Base3.InstanceID sourceAddress, bool canCreate)
        {
            ReceiverContext receiverContext;
            if (!contexts.TryGetValue(sourceAddress, out receiverContext))
            {
                if (!canCreate)
                    throw new Exception("Could not locate receiver for source " + sourceAddress.ToString());
                
                receiverContext = new ReceiverContext(this, sourceAddress);
                receiverContext.Receiver = receiverClass.Create(receiverContext);

                QS._core_c_.Diagnostics2.IModule module = receiverContext.Receiver as QS._core_c_.Diagnostics2.IModule;
                if (module != null)
                    ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(sourceAddress.ToString(), module.Component);

                contexts.Add(sourceAddress, receiverContext);
            }

            return receiverContext;
        }

        #endregion

        #region Receive Callbacks

        private QS.Fx.Serialization.ISerializable MessageReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Senders11.InstanceMessage message = receivedObject as Senders11.InstanceMessage;
            if (message == null)
                throw new Exception("Received a message of an incompatible type.");

            if (message.Address.Equals(localAddress))
                GetContext(sourceAddress, true).Receiver.Receive(message.SequenceNo, message.Message);

            return null;
        }

        #endregion
    }
}
