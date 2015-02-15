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

// #define DEBUG_LazySender
// #define Calculate_Statistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Buffering_3_
{
    public class LazySender : Base3_.SenderClass<IBufferingSender>, Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>
    {
        public LazySender(uint destinationLOID, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection,
            IControllerClass controllerClass, QS.Fx.Clock.IAlarmClock alarmClock, TimeSpan flushingInterval, QS.Fx.Logging.ILogger logger)
        {
            this.controllerClass = controllerClass;
            this.underlyingCollection = underlyingCollection;
            this.destinationLOID = destinationLOID;
            this.flushingInterval = flushingInterval;
            this.logger = logger;
            this.alarmClock = alarmClock;
        }

        private IControllerClass controllerClass;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection;
        private QS.Fx.Logging.ILogger logger;
        private uint destinationLOID;
        private TimeSpan flushingInterval;
        private QS.Fx.Clock.IAlarmClock alarmClock;

#if Calculate_Statistics
		Statistics.Samples messageCountSamples = new QS.CMS.Statistics.Samples();

		public TMS.Data.IDataSet MessageCountSamples
		{
			get { return messageCountSamples.DataSet; }
		}
#endif

        public TimeSpan FlushingInterval
        {
            get { return flushingInterval; }
            set { flushingInterval = value; }
        }

		protected override IBufferingSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, destinationAddress);
        }

        public class Sender : Base3_.SerializableSender, IBufferingSender
        {
            public Sender(LazySender owner, QS.Fx.Network.NetworkAddress destinationAddress) : base(owner.underlyingCollection[destinationAddress])
            {
                this.owner = owner;
                this.controller = owner.controllerClass.CreateController(underlyingSender.MTU);
                this.myAlarmCallback = new QS.Fx.Clock.AlarmCallback(flushingCallback);
            }

            private LazySender owner;
            private IController controller;
            private QS.Fx.Clock.AlarmCallback myAlarmCallback;
            private QS.Fx.Clock.IAlarm alarmRef = null;

            private void flushingCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
#if DEBUG_LazySender
                logger.Log(this, "__________FlushingCallback");
#endif

                flush();
            }

            #region IBufferingSender Members

            public void flush()
            {
                lock (this)
                {
                    controller.flush();
					while (controller.ReadyQueue.Count > 0)
					{
						IMessageCollection messageCollection = controller.ReadyQueue.Dequeue();
#if Calculate_Statistics
						owner.messageCountSamples.addSample(messageCollection.Count);
#endif
						underlyingSender.send(owner.destinationLOID, messageCollection);
					}

					if (alarmRef != null)
                    {
                        alarmRef.Cancel();
                        alarmRef = null;
                    }
                }
            }

            #endregion

            #region QS.CMS.Base3.ISerializableSender Members

            public override void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
#if DEBUG_LazySender
                logger.Log(this, "__________Send");
#endif

                lock (this)
                {
                    controller.append(destinationLOID, data);
					while (controller.ReadyQueue.Count > 0)
					{
						IMessageCollection messageCollection = controller.ReadyQueue.Dequeue();
#if Calculate_Statistics
						owner.messageCountSamples.addSample(messageCollection.Count);
#endif
						underlyingSender.send(owner.destinationLOID, messageCollection);
					}

					if (alarmRef == null && !controller.Empty)
                        alarmRef = owner.alarmClock.Schedule(owner.flushingInterval.TotalSeconds, myAlarmCallback, null);
                }
            }

            public override int MTU
            {
                get { return controller.MTU; }
            }

            #endregion
        }

        #region ISenderClass<ISerializableSender> Members

        QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.SenderCollection
        {
            get 
			{
				return new Base3_.SenderCollectionCast<Buffering_3_.IBufferingSender, QS._qss_c_.Base3_.ISerializableSender>(
					((Base3_.ISenderClass<IBufferingSender>)this).SenderCollection);
			}
        }

        QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
        {
            return this.createSender(destinationAddress);
        }

        #endregion
    }
}
