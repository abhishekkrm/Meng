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

#define DEBUG_SimpleSender

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.VS6
{
    public class SimpleSender : Multicasting3.GroupSenderClass<Multicasting3.IGroupSender, SimpleSender.Sender>
    {
        public SimpleSender(QS.Fx.Logging.ILogger logger)
        {
			this.logger = logger;
		}

        private QS.Fx.Logging.ILogger logger;

		#region Receive Callback

		QS.Fx.Serialization.ISerializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
		{
#if DEBUG_SimpleSender
            logger.Log(this, "__ReceiveCallback : " + QS._core_c_.Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			// ..................................

			return null;
		}

		#endregion

        #region Class Sender

        public class Sender : Multicasting3.GroupSender
        {
            public Sender(SimpleSender owner, Base3_.GroupID groupID) : base(groupID)
            {
                this.owner = owner;
            }

            private SimpleSender owner;

			#region Class Request

			public class Request : Base3_.AsynchronousOperation
			{
				public Request(SimpleSender.Sender owner, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState) 
					: base(completionCallback, asynchronousState)
				{
					this.owner = owner;
				}

				private SimpleSender.Sender owner;
				private QS.Fx.Clock.IAlarm alarmRef = null;

				public override void Unregister()
				{
/*
					alarmRef.Cancel();
					owner.removeComplete(message.ID.MessageSeqNo);
*/
				}

				#region Accessors

				public QS.Fx.Clock.IAlarm AlarmRef
				{
					get { return alarmRef; }
					set { alarmRef = value; }
				}

				#endregion

				public void acknowledgementCallback()
				{
/*
#if DEBUG_SimpleSender
					owner.owner.logger.Log(this, "__AcknowledgementCallback: " + message.ToString());
#endif
					this.IsCompleted = true;
*/
				}
			}

			#endregion

            #region IGroupSender Members

            public override Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data,
                Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
            {
				Request request = null;

				lock (this)
				{
					// somehow get hold of IVSViewController, the controller is locked

					IVSViewController vsViewController = null;

					Scattering_1_.IScatterSet destinations;
					Multicasting3.MessageID messageID = vsViewController.RegisterOutgoing(out destinations);

					vsViewController.Release();


					// ....deliver a message using this information, with shrinking group of destinations in scatter set



					// ....

					request = new Request(this, completionCallback, asynchronousState);

					// ....

					// ....
				}



				return null;
            }

            public override void EndSend(Base3_.IAsynchronousOperation asynchronousOperation)
            {
            }

            public override int MTU
            {
                get { throw new System.NotImplementedException(); }
            }

            #endregion
        }

        #endregion

        protected override Sender createSender(Base3_.GroupID groupID)
        {
            return new Sender(this, groupID);
        }
    }
}
