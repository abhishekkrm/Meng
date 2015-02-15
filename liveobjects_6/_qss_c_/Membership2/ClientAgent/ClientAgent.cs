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

// #define DEBUG_ClientAgent

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Membership2.ClientAgent
{
    public class ClientAgent : IClientAgent
    {
        public ClientAgent(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, QS._qss_c_.Base3_.ISerializableCaller serverCaller,
            QS.Fx.Network.NetworkAddress localAddress, Membership2.ClientState.IConfiguration membershipConfiguration, QS._core_c_.Base3.InstanceID instanceID)
        {
            this.logger = logger;
            demultiplexer.register((uint)ReservedObjectID.Membership_ClientAgent, new QS._qss_c_.Base3_.ReceiveCallback(this.notificationCallback));
            this.serverCaller = serverCaller;
            this.membershipConfiguration = membershipConfiguration;
            this.instanceID = instanceID;             

            completionAsyncCallback = new AsyncCallback(this.completionCallback);
        }
        
        private QS.Fx.Logging.ILogger logger;
        private Membership2.ClientState.IConfiguration membershipConfiguration;
        private QS._qss_c_.Base3_.ISerializableCaller serverCaller;        
        private QS._core_c_.Base3.InstanceID instanceID;
        private AsyncCallback completionAsyncCallback;
        private System.Collections.Generic.IDictionary<uint, Protocol.Notification> notificationQueue =
            new System.Collections.Generic.Dictionary<uint, Protocol.Notification>();
        private uint numberOfNotificationsProcessed = 0;

        #region IClientAgent Members

        public void ChangeMembership(IList<QS._qss_c_.Base3_.GroupID> groupsToJoin, IList<QS._qss_c_.Base3_.GroupID> groupsToLeave)
        {
            Protocol.MembershipChangeRequest request = 
                new QS._qss_c_.Membership2.Protocol.MembershipChangeRequest(instanceID, false, groupsToJoin, groupsToLeave);

#if DEBUG_ClientAgent
            logger.Log(this, "Sending request to " + serverCaller.Address.ToString() + ":\n" + request.ToString() + "\n");
#endif

            IAsyncResult asynchronousResult = serverCaller.BeginCall(
                (uint)QS.ReservedObjectID.Membership_CentralizedServer, request, completionAsyncCallback, request);
/*
            asynchronousResult.AsyncWaitHandle.WaitOne();
            serverCaller.EndCall(asynchronousResult);
*/
        }

        #endregion

        #region Callbacks

        private void completionCallback(IAsyncResult asyncResult)
        {
            Protocol.MembershipChangeRequest request = asyncResult.AsyncState as Protocol.MembershipChangeRequest;

#if DEBUG_ClientAgent
            logger.Log(this, "Completed request:\n" + request.ToString() + "\n");
#endif

            serverCaller.EndCall(asyncResult);
        }

		private QS.Fx.Serialization.ISerializable notificationCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_ClientAgent
            logger.Log(this, "__NotificationCallback : Received " + Helpers.ToString.ReceivedObject(sourceIID.Address, receivedObject));
#endif

            Protocol.Notification notification = receivedObject as Protocol.Notification;
            if (notification != null)
            {
                if (notification.ReceiverIID.Equals(instanceID))
                {
                    lock (this)
                    {
                        if (notification.SequenceNo > numberOfNotificationsProcessed)
                        {
                            notificationQueue[notification.SequenceNo] = notification;
                            while (notificationQueue.ContainsKey(numberOfNotificationsProcessed + 1))
                            {
                                numberOfNotificationsProcessed++;
                                Protocol.Notification notificationToConsume = notificationQueue[numberOfNotificationsProcessed];
                                notificationQueue.Remove(numberOfNotificationsProcessed);

// #if DEBUG_ClientAgent
                                logger.Log(this, "Processing notification #" + notification.SequenceNo.ToString() + ".");
// #endif
                                membershipConfiguration.Change(notificationToConsume);
                            }
                        }
                        else
                        {
// #if DEBUG_ClientAgent
                            logger.Log(this, "Notification with old SequenceNo received: received " + notification.SequenceNo.ToString() + 
                                ", expected "+ (numberOfNotificationsProcessed + 1).ToString());
// #endif
                        }
                    }
                }
                else
                {
// #if DEBUG_ClientAgent
                    logger.Log(this, "Notification for old incarnation received.");
// #endif
                }
            }

            return null;
        }

        #endregion
    }
}
