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

namespace QS._qss_c_.Membership_3_.Client
{
/*
    public class Client : Interface.IMembershipService
    {
        public Client(Base3.IReliableSerializableSender senderToGMS, FailureDetection.DummyFD failureDetector,
            QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, Base3.IDemultiplexer demultiplexer)
        {
            this.senderToGMS = senderToGMS;
            this.failureDetector = failureDetector;
            this.logger = logger;
            this.eventLogger = eventLogger;

            demultiplexer.register(
                (uint)ReservedObjectID.Membership3_Client_ResponseChannel, new QS.CMS.Base3.ReceiveCallback(this.ResponseCallback));
            demultiplexer.register(
                (uint)ReservedObjectID.Membership3_Client_NotificationChannel, new QS.CMS.Base3.ReceiveCallback(this.NotificationCallback));

            configuration = new QS.CMS.Membership3.ClientState.Configuration(logger, eventLogger);
        }

        // private static readonly TimeSpan DefaultBatchingInterval = TimeSpan.FromMilliseconds(100);
        //
        // public Client(Base.IAlarmClock alarmClock) : this(alarmClock, DefaultBatchingInterval)
        // { 
        // }
        //
        // public Client(Base.IAlarmClock alarmClock, TimeSpan batchingInterval)
        // {
        //    this.alarmClock = alarmClock;
        //    this.batchingInterval = batchingInterval.TotalSeconds;
        // }
        //
        // private Base.IAlarmClock alarmClock;
        // private bool waiting;
        // private Base.IAlarmRef alarmRef;
        // private double batchingInterval;
        // private Queue<Request> outgoingQueue = new Queue<RequestState>();

        private int lastUsedSequenceNo;
        private IDictionary<int, RequestState> pendingRequests = new Dictionary<int, RequestState>();
        private Base3.IReliableSerializableSender senderToGMS;
        private FailureDetection.DummyFD failureDetector;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private ClientState.Configuration configuration;

        #region ProcessNotification

        private void ProcessNotification(Notifications.Notification notification)
        {
            logger.Log(this, "__ProcessNotification: " + QS.Fx.Printing.Printable.ToString(notification));

            configuration.Update(notification);
        }

        #endregion

        #region ReceiveCallbacks

        private QS.Fx.Serialization.ISerializable ResponseCallback(QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Responses.Response response = (Responses.Response)receivedObject;
            RequestState requestState = null;
            lock (this)
            {
                if (pendingRequests.TryGetValue(response.SequenceNo, out requestState))
                    pendingRequests.Remove(response.SequenceNo);
            }
            if (requestState != null)
                requestState.Completed(response);

            return null;
        }

        private QS.Fx.Serialization.ISerializable NotificationCallback(QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            ProcessNotification((Notifications.Notification)receivedObject);

            return null;
        }

        #endregion

        #region Internals

        private IAsyncResult BeginRequest(Requests.Request request, AsyncCallback asyncCallback, object asyncState)
        {
            RequestState requestState = new RequestState(request, asyncCallback, asyncState);
            pendingRequests.Add(request.SequenceNo, requestState);

            // outgoingQueue.Enqueue(request);
            //
            // if (!waiting)
            // {
            //    if (alarmRef == null)
            //        alarmRef = alarmClock.scheduleAnAlarm(batchingInterval, new QS.CMS.Base.AlarmCallback(this.BatchingCallback), null);
            //    else
            //        alarmRef.reschedule(batchingInterval);
            //    waiting = true;
            //
            // Submit();
            // }

            senderToGMS.send((uint)ReservedObjectID.Membership3_GMS, request);

            return requestState;
        }

        // private void BatchingCallback(Base.IAlarmRef alarmRef)
        // {
        //     lock (this)
        //     {
        //         if (outgoingQueue.Count > 0)
        //         {
        //             alarmRef.reschedule(batchingInterval);
        //             Submit();
        //         }
        //         else
        //         {
        //             waiting = false;
        //         }
        //     }
        // }
        // 
        // private void Submit()
        // {
        //     // ...........
        // }

        private Responses.Response EndRequest(IAsyncResult asyncResult)
        {
            return ((RequestState)asyncResult).Response;
        }

        #endregion

        #region IMembershipService Members

        IAsyncResult QS.CMS.Membership3.Interface.IMembershipService.BeginOpen(
            QS.CMS.Membership3.Expressions.Expression groupSpecification, 
            QS.CMS.Membership3.Interface.OpeningMode openingMode, QS.CMS.Membership3.Interface.AccessMode accessMode, 
            QS.CMS.Membership3.Interface.IGroupType groupType, QS.CMS.Membership3.Interface.IGroupAttributes groupAttributes, 
            AsyncCallback asyncCallback, object asyncState)
        {
            lock (this)
            {
                return BeginRequest(new QS.CMS.Membership3.Requests.Open(++lastUsedSequenceNo,
                        groupSpecification, openingMode, accessMode, groupType, groupAttributes), asyncCallback, asyncState);
            }
        }

        QS.CMS.Membership3.Interface.IGroupRef QS.CMS.Membership3.Interface.IMembershipService.EndOpen(IAsyncResult asyncResult)
        {
            RequestState requestState = (RequestState)asyncResult;
            Requests.Open request = (Requests.Open) requestState.Request;

            Responses.Response response = EndRequest(requestState);
            switch (response.ResponseType)
            {
                case QS.CMS.Membership3.Responses.ResponseType.Opened:
                {
                    Responses.Opened opened = (Responses.Opened) response;
                    return new GroupRef(this, 
                        opened.GroupID, request.GroupSpecification.ToString(), request.AccessMode, opened.GroupType, opened.GroupAttributes);
                }

                case QS.CMS.Membership3.Responses.ResponseType.Exception:
                    throw ((Responses.Error)response).Exception;

                default:
                    throw new Exception("Unrecognized response.");
            }
        }

        QS.CMS.Membership3.Interface.IGroupRef QS.CMS.Membership3.Interface.IMembershipService.Open(
            QS.CMS.Membership3.Expressions.Expression groupSpecification,
            QS.CMS.Membership3.Interface.OpeningMode openingMode, QS.CMS.Membership3.Interface.AccessMode accessMode,
            QS.CMS.Membership3.Interface.IGroupType groupType, QS.CMS.Membership3.Interface.IGroupAttributes groupAttributes)
        {
            IAsyncResult asyncResult = ((Interface.IMembershipService) this).BeginOpen(
                groupSpecification, openingMode, accessMode, groupType, groupAttributes, null, null);

            asyncResult.AsyncWaitHandle.WaitOne();
            return ((Interface.IMembershipService) this).EndOpen(asyncResult);
        }

        #endregion

        #region Requests against an existing group 
        
        public void Close(GroupRef groupRef)
        {
            lock (this)
            {
                BeginRequest(new QS.CMS.Membership3.Requests.Close(++lastUsedSequenceNo, groupRef.GroupID), null, null);
            }
        }

        #endregion
    }
*/ 
}
