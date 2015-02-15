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

// #define DEBUG_GenericServer

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Threading;

namespace QS._qss_c_.Membership2.Servers
{
    public enum Type
    {
        REGULAR, OVERLAPPING, NONE
    }

    [QS.Fx.Base.Inspectable]
    public abstract class GenericServer : QS.Fx.Inspection.Inspectable, System.IDisposable
    {
        private const int initialRequestQueueSize = 10;
        private const double default_aggregationInterval = 5.0; // for now, just dumb aggregation based on delay in response from the server

        public GenericServer(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock,
            Base3_.ISenderCollection<Base3_.IReliableSerializableSender> senderCollection, FailureDetection_.IFailureDetector failureDetectionServer,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> instanceSenders)
        {
            failureDetectionServer.OnChange += new QS._qss_c_.FailureDetection_.ChangeCallback(failureDetectionServer_OnChange);

            this.logger = logger;
            demultiplexer.register((uint)QS.ReservedObjectID.Membership_CentralizedServer, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));
            this.alarmClock = alarmClock;
            processingAlarmCallback = new QS.Fx.Clock.AlarmCallback(this.processingCallback);
            
            // this.senderCollection = senderCollection;

            this.instanceSenders = instanceSenders;

            requestQueue = new Queue<QS._qss_c_.Membership2.Protocol.IMembershipChangeRequest>(initialRequestQueueSize);
            // notificationQueue = new Queue<ICollection<KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification>>>();

            addressPool = new Allocation.AddressPool();
			requestAggregationInterval = default_aggregationInterval;
		}

        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> instanceSenders;

        void failureDetectionServer_OnChange(IEnumerable<QS._qss_c_.FailureDetection_.Change> changes)
        {
            lock (requestQueue)
            {
                bool detected_crashes = false;

                foreach (FailureDetection_.Change change in changes)
                {
                    if (change.Action == FailureDetection_.Action.CRASHED)
                    {
                        if (change.InstanceID != null)
                        {
                            detected_crashes = true;

// #if DEBUG_GenericServer
                            logger.Log(this, "__OnChange: " + change.ToString());
// #endif

                            requestQueue.Enqueue(new Protocol.MembershipChangeRequest(change.InstanceID, true));

//                        if (registryOfNotifications.ContainsKey(change.InstanceID))
//                        {
//                            foreach (Base3.IAsynchronousOperation asynchronousOperation in registryOfNotifications[change.InstanceID])
//                               asynchronousOperation.Cancel();
//                        }
                        }
                    }
                }

                if (requestQueue.Count > 0)
                {
                    if (detected_crashes)
                    {
                        if (alarmRef != null)
                        {
                            alarmRef.Cancel();
                            alarmRef = null;
                        }

                        processingCallback(null);
                    }
                    else
                    {
                        if (alarmRef == null)
                            alarmRef = alarmClock.Schedule(requestAggregationInterval, processingAlarmCallback, null);
                    }
                }
            }
        }

        protected QS.Fx.Logging.ILogger logger;
        protected Allocation.IAddressPool addressPool;

		public double RequestAggregationInterval
		{
			get { return requestAggregationInterval; }
			set { requestAggregationInterval = value; }
		}	

		private double requestAggregationInterval;
		private Queue<Protocol.IMembershipChangeRequest> requestQueue;
        private QS.Fx.Clock.AlarmCallback processingAlarmCallback;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IAlarm alarmRef = null;
//        private Base3.ISenderCollection<Base3.IReliableSerializableSender> senderCollection;
//        private Queue<ICollection<KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification>>> notificationQueue;

//        private System.Collections.Generic.IDictionary<QS._core_c_.Base3.InstanceID, System.Collections.ObjectModel.Collection<Base3.IAsynchronousOperation>> registryOfNotifications = 
//            new System.Collections.Generic.Dictionary<QS._core_c_.Base3.InstanceID, System.Collections.ObjectModel.Collection<Base3.IAsynchronousOperation>>();

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Protocol.MembershipChangeRequest request = receivedObject as Protocol.MembershipChangeRequest;

#if DEBUG_GenericServer
            logger.Log(this, "Received request from " + sourceIID.Address.ToString() + ":\n" + request.ToString());
#else
            logger.Log(this, "Received request from " + sourceIID.Address.ToString());
#endif

            if (request.Crashed || (request.ToJoin.Count > 0) || (request.ToLeave.Count > 0))
            {
                lock (requestQueue)
                {
                    requestQueue.Enqueue(request);
                    if (alarmRef == null)
                        alarmRef = alarmClock.Schedule(requestAggregationInterval, processingAlarmCallback, null);
                }
            }

            // this.processRequest(new Protocol.MembershipChangeRequest[] { request });

            return null;
        }

        protected abstract void processRequest(ICollection<Protocol.IMembershipChangeRequest> request,
            ref Components_1_.AutomaticCollection<QS._core_c_.Base3.InstanceID, Protocol.Notification> notifications);

        private void processingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            try
            {
// #if DEBUG_GenericServer
                logger.Log(this, "__Processing Callback");
// #endif

                lock (this)
                {

                    ICollection<Membership2.Protocol.IMembershipChangeRequest> aggregatedRequests;
                    lock (requestQueue)
                    {
                        this.alarmRef = null; // fix

                        aggregatedRequests =
                            Membership2.Protocol.MembershipChangeRequest.AggregateCollection(
							// (System.Collections.Generic.ICollection<QS.CMS.Membership2.Protocol.IMembershipChangeRequest>) 
							requestQueue);
                        requestQueue.Clear();
                    }

#if DEBUG_GenericServer
                    logger.Log(this, "\n\nProcessing Requests:\n\n" +
                        Helpers.CollectionHelper.ToStringSeparated<Protocol.IMembershipChangeRequest>(aggregatedRequests, "\n") + "\n\n");
#endif

                    Components_1_.AutomaticCollection<QS._core_c_.Base3.InstanceID, Protocol.Notification> notifications =
                        new Components_1_.AutomaticCollection<QS._core_c_.Base3.InstanceID, Protocol.Notification>(
                            new Base3_.Constructor<Protocol.Notification>(Protocol.Notification.Create));

// #if DEBUG_GenericServer
//                    logger.Log(this, "\n\nConfiguration Before:\n\n" + this.ToString());
// #endif

                    this.processRequest(aggregatedRequests, ref notifications);

#if DEBUG_GenericServer
//                    logger.Log(this, "\n\nConfiguration Afterwards:\n\n" + this.ToString() + "\n\n");

                    StringBuilder s = new StringBuilder("\n\nNotifications to send out:\n\n");
                    foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification> notificationToSend in notifications.Collection)
                        s.AppendLine(notificationToSend.Key.ToString() + " ---------->-- " + notificationToSend.Value.ToString());
                    s.AppendLine("\n");
                    logger.Log(this, s.ToString());
#endif

                    // lock (notificationQueue)
                    // {
                    //    notificationQueue.Enqueue(new List<KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification>>(notifications.Collection));
                    // }

                    // for now, we just send it using unreliable sender, don't bother to retransmit and don't care bout failures
                    // TODO: Need to fix this.
                    foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Protocol.Notification> some_notification in notifications.Collection)
                    {
                        try
                        {
//                            QS.Fx.Network.NetworkAddress receiverAddress = some_notification.Key.Address;
                            QS.Fx.Serialization.ISerializable notification = some_notification.Value;

                            instanceSenders[some_notification.Key].BeginSend((uint)ReservedObjectID.Membership_ClientAgent, notification, null, null);

//                            Base3.IAsynchronousOperation notificationAsyncOperation = senderCollection[receiverAddress].BeginSend(
//                                (uint)ReservedObjectID.Membership_ClientAgent, notification, 
//                                new Base3.AsynchronousOperationCallback(notificationCompletionCallback), notification);
//                            if (!registryOfNotifications.ContainsKey(some_notification.Key))
//                                registryOfNotifications[some_notification.Key] =
//                                    new System.Collections.ObjectModel.Collection<QS.CMS.Base3.IAsynchronousOperation>();
//                            registryOfNotifications[some_notification.Key].Add(notificationAsyncOperation);

// #if DEBUG_GenericServer
                            logger.Log(this, "Sent notification to " + some_notification.Key.ToString());
// #endif
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, "Cannot send notification : " + exc.ToString());
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                logger.Log(this, "__ProcessingCallback : " + exc.ToString());
            }
        }

//        private void notificationCompletionCallback(Base3.IAsynchronousOperation asynchronousOperation)
//        {
//            lock (this)
//            {
//                Protocol.Notification notification = (Protocol.Notification) asynchronousOperation.AsyncState;
//                if (registryOfNotifications.ContainsKey(notification.ReceiverIID))
//                    registryOfNotifications[notification.ReceiverIID].Remove(asynchronousOperation);
//                senderCollection[notification.ReceiverIID.Address].EndSend(asynchronousOperation);
//            }
//        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                if (alarmRef != null)
                    alarmRef.Cancel();
                alarmRef = null;
            }
        }

        #endregion
    }
}
