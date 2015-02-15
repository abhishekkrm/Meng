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

namespace QS._qss_c_.Membership_3_.Server
{
/*
    public class CentralizedGMS
    {
        public CentralizedGMS(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, Base3.IDemultiplexer demultiplexer, Base.IAlarmClock alarmClock, TimeSpan batchingInterval,
            Base3.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3.IReliableSerializableSender> reliableSenders)
        {
            this.eventLogger = eventLogger;
            this.logger = logger;
            this.alarmClock = alarmClock;
            this.batchingInterval = batchingInterval;
            this.reliableSenders = reliableSenders;

            configuration = new QS.CMS.Membership3.ServerState.Configuration(logger, eventLogger);

            demultiplexer.register((uint)ReservedObjectID.Membership3_GMS, new QS.CMS.Base3.ReceiveCallback(this.ReceiveCallback));
        }

        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS.Fx.Logging.ILogger logger;
        private Base.IAlarmClock alarmClock;
        private Queue<KeyValuePair<QS._core_c_.Base3.InstanceID, Requests.Request>> requestQueue = 
            new Queue<KeyValuePair<QS._core_c_.Base3.InstanceID, QS.CMS.Membership3.Requests.Request>>();
        private bool waiting;
        private Base.IAlarmRef alarmRef;
        private TimeSpan batchingInterval;
        private ServerState.Configuration configuration;
        private Base3.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3.IReliableSerializableSender> reliableSenders;

        #region ProcessQueue

        private void ProcessQueue()
        {
            ServerState.Configuration.Update configurationChange = new QS.CMS.Membership3.ServerState.Configuration.Update(configuration);

            Queue<KeyValuePair<QS._core_c_.Base3.InstanceID, Responses.Response>> responses =
                new Queue<KeyValuePair<QS._core_c_.Base3.InstanceID, QS.CMS.Membership3.Responses.Response>>();

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Requests.Request> element in requestQueue)
            {
                QS._core_c_.Base3.InstanceID requestorAddress = element.Key;
                Requests.Request request = element.Value;

                logger.Log(this, "__________Update(" + requestorAddress.ToString() + ", " + QS.Fx.Printing.Printable.ToString(request) + ")");

                Responses.Response response;
                try
                {
                    switch (request.RequestType)
                    {
                        case QS.CMS.Membership3.Requests.RequestType.Open:
                        {
                            Base3.GroupID groupID;
                            Interface.IGroupType groupType;
                            Interface.IGroupAttributes groupAttributes;

                            configurationChange.Open(
                                requestorAddress, ((Requests.Open)request).GroupSpecification, 
                                ((Requests.Open)request).OpeningMode, ((Requests.Open)request).AccessMode, 
                                ((Requests.Open)request).GroupType, ((Requests.Open)request).GroupAttributes,
                                out groupID, out groupType, out groupAttributes);

                            response = new Responses.Opened(request.SequenceNo, groupID, groupType, groupAttributes);
                        }
                        break;

                        case QS.CMS.Membership3.Requests.RequestType.Close:
                        {
                            configurationChange.Close(requestorAddress, ((Requests.Close)request).GroupID);
                            response = new Responses.Succeeded(request.SequenceNo);
                        }
                        break;

                        case QS.CMS.Membership3.Requests.RequestType.Crash:
                        {
                            configurationChange.Crash(requestorAddress, ((Requests.Crash)request).DeadAddresses);
                            response = new Responses.Succeeded(request.SequenceNo);
                        }
                        break;

                        case QS.CMS.Membership3.Requests.RequestType.Resync:
                        {
                            configurationChange.Resync(requestorAddress);
                            response = new Responses.Succeeded(request.SequenceNo);
                        }
                        break;

                        default:
                            throw new Exception("Unknown request type.");
                    }
                }
                catch (Exception exc)
                {
                    response = new Responses.Error(request.SequenceNo, exc);
                }

                responses.Enqueue(new KeyValuePair<QS._core_c_.Base3.InstanceID, Responses.Response>(requestorAddress, response));
            }

            requestQueue.Clear();

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Responses.Response> element in responses)
                ((Base3.IReliableSerializableSender)reliableSenders[element.Key]).send(
                    (uint)ReservedObjectID.Membership3_Client_ResponseChannel, element.Value);

            IDictionary<QS._core_c_.Base3.InstanceID, Notifications.Notification> notifications;
            configurationChange.Commit(out notifications);

            foreach (KeyValuePair<QS._core_c_.Base3.InstanceID, Notifications.Notification> element in notifications)
                ((Base3.IReliableSerializableSender)reliableSenders[element.Key]).send(
                    (uint)ReservedObjectID.Membership3_Client_NotificationChannel, element.Value);
        }

        #endregion

        #region BatchingCallback

        private void BatchingCallback(Base.IAlarmRef alarmRef)
        {
            lock (this)
            {
                waiting = false;
                if (requestQueue.Count > 0)
                    ProcessQueue();
            }
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID senderAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Requests.Request request = (Requests.Request)receivedObject;

            lock (this)
            {
                requestQueue.Enqueue(new KeyValuePair<QS._core_c_.Base3.InstanceID, Requests.Request>(senderAddress, request));
                if (!waiting)
                {
                    waiting = true;
                    if (alarmRef != null)
                        alarmRef.reschedule(batchingInterval.TotalSeconds);
                    else
                        alarmRef = alarmClock.scheduleAnAlarm(batchingInterval.TotalSeconds, new QS.CMS.Base.AlarmCallback(BatchingCallback), null);

                    // ProcessQueue();
                }
            }

            return null;
        }

        #endregion
    }
*/ 
}
