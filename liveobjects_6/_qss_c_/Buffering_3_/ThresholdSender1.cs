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

// #define DEBUG_ThresholdSender

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Buffering_3_
{
	public class ThresholdSender1
		: Base3_.SenderClass<IBufferingSender>, Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>
	{
        public ThresholdSender1(uint destinationLOID, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection,
            IControllerClass controllerClass, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, double referenceInterval, double bufferingThreshold, 
            QS.Fx.Logging.ILogger logger)
        {
            this.destinationLOID = destinationLOID;
            this.underlyingCollection = underlyingCollection;
            this.controllerClass = controllerClass;
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.referenceInterval = referenceInterval;
            this.bufferingThreshold = bufferingThreshold;
        }

        private uint destinationLOID;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection;
        private IControllerClass controllerClass;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private double referenceInterval, bufferingThreshold;

		public void SetThreshold(double referenceInterval, double bufferingThreshold)
		{
			this.referenceInterval = referenceInterval;
			this.bufferingThreshold = bufferingThreshold;
		}

		protected override IBufferingSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, destinationAddress);
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

		public class Sender : Base3_.SerializableSender, IBufferingSender
        {
            public Sender(ThresholdSender1 owner, QS.Fx.Network.NetworkAddress destinationAddress) : base(owner.underlyingCollection[destinationAddress])
            {
                this.owner = owner;
                this.controller = owner.controllerClass.CreateController(underlyingSender.MTU);
                this.alarmCallback = new QS.Fx.Clock.AlarmCallback(this.checkingCallback);                
                this.referenceInterval = owner.referenceInterval;
                this.incrementUnit = referenceInterval / owner.bufferingThreshold;

                creditConsumed = 0;
            }

            private ThresholdSender1 owner;
            private IController controller;
            private double incrementUnit, referenceInterval, creditConsumed, lastTimeChecked;
            private QS.Fx.Clock.IAlarm alarmRef = null;
            private QS.Fx.Clock.AlarmCallback alarmCallback;

            #region IBufferingSender Members

            public void flush()
            {
                throw new ArgumentException("Flushing is not supported with this sender.");
            }

            #endregion

            private void checkingCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                lock (this)
                {
#if DEBUG_ThresholdSender
                logger.Log(this, "__________CheckingCallback");
#endif

                    double now = owner.clock.Time;
                    double timeCredit = now - lastTimeChecked;
                    lastTimeChecked = now;

                    creditConsumed -= timeCredit;
                    if (creditConsumed < 0)
                        creditConsumed = 0;

                    if (creditConsumed < referenceInterval)
                    {
                        controller.flush();
                        processQueue();
                    }

                    if (creditConsumed == 0)
                    {
                        this.alarmRef = null;

#if DEBUG_ThresholdSender
                    logger.Log(this, "__________AlarmCancelled");
#endif
                    }
                    else
                    {
                        alarmRef.Reschedule((creditConsumed > referenceInterval) ? creditConsumed : referenceInterval);

#if DEBUG_ThresholdSender
                    logger.Log(this, "__________ReschedulingAlarm, Timeout: " + alarmRef.Timeout.ToString());
#endif
                    }
                }
            }

            private void processQueue()
            {
                while (controller.ReadyQueue.Count > 0)
                {
                    IMessageCollection messageCollection = controller.ReadyQueue.Dequeue();
                    creditConsumed -= incrementUnit * messageCollection.Count;
                    underlyingSender.send(owner.destinationLOID, messageCollection);

                    if (creditConsumed < 0)
                        creditConsumed = 0;

#if DEBUG_ThresholdSender
                logger.Log(this, "__________ProcessedQueue(" + messageCollection.Count.ToString() + "), Credit: " + creditConsumed.ToString());
#endif
                }
            }

            #region ISerializableSender Members

            public override void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                lock (this)
                {
                    creditConsumed += incrementUnit;

                    if (creditConsumed < referenceInterval)
                    {
#if DEBUG_ThresholdSender
                    logger.Log(this, "__________SendingDirectly, Credit: " + creditConsumed.ToString());
#endif

                        underlyingSender.send(destinationLOID, data);
                        if (alarmRef == null)
                        {
                            alarmRef = owner.alarmClock.Schedule(
                                ((creditConsumed > referenceInterval) ? creditConsumed : referenceInterval), alarmCallback, null);
                            lastTimeChecked = owner.clock.Time;

#if DEBUG_ThresholdSender
                        logger.Log(this, "__________SchedulingAlarm, Timeout: " + alarmRef.Timeout.ToString());
#endif
                        }
                    }
                    else
                    {
#if DEBUG_ThresholdSender
                    logger.Log(this, "__________Buffering, Credit: " + creditConsumed.ToString());
#endif

                        controller.append(destinationLOID, data);
                        processQueue();

                        if (creditConsumed < referenceInterval)
                        {
                            controller.flush();
                            processQueue();
                        }
                    }
                }
            }

            public override int MTU
            {
                get { return controller.MTU; }
            }

            #endregion
        }
    }
}
