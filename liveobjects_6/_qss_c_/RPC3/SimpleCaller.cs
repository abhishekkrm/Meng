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

// #define DEBUG_SimpleCaller

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.RPC3
{
    public class SimpleCaller : Base3_.SenderClass<QS._qss_c_.Base3_.ISerializableCaller>
    {
        public SimpleCaller(QS.Fx.Logging.ILogger logger, Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senderCollection,
            Base3_.IDemultiplexer demultiplexer, QS.Fx.Clock.IAlarmClock alarmClock, double retransmissionTimeout)
        {
            this.logger = logger;
            this.senderCollection = senderCollection;
            this.alarmClock = alarmClock;
            this.demultiplexer = demultiplexer;
            this.retransmissionTimeout = retransmissionTimeout;

            demultiplexer.register((uint)ReservedObjectID.RPC3_SimpleCaller_RequestChannel, new Base3_.ReceiveCallback(requestCallback));
            demultiplexer.register((uint)ReservedObjectID.RPC3_SimpleCaller_ResponseChannel, new Base3_.ReceiveCallback(responseCallback));

            requests = new System.Collections.Generic.Dictionary<uint, OutgoingRequest>();
        }

        private QS.Fx.Logging.ILogger logger;
        private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> senderCollection;
        private Base3_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private double retransmissionTimeout;
        private int sequenceNo = 0;
        private System.Collections.Generic.IDictionary<uint, OutgoingRequest> requests;

        #region Request and Response Callbacks

		private QS.Fx.Serialization.ISerializable requestCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_SimpleCaller
            logger.Log(this, "__RequestCallback : " + sourceIID.ToString() + ", " + receivedObject.ToString());
#endif

            IncomingRequest request = (IncomingRequest) receivedObject;

            QS.Fx.Serialization.ISerializable responseObject = null;
            bool succeeded = true;
            System.Exception exception = null;

            try
            {
                responseObject = demultiplexer.dispatch(
                    request.Object.destinationLOID, sourceIID, request.Object.transmittedObject);
            }
            catch (Exception exc)
            {
                succeeded = false;
                exception = exc;
            }

            QS.Fx.Serialization.ISerializable response = new Response(request.SeqNo, responseObject, succeeded, exception);

#if DEBUG_SimpleCaller
            logger.Log(this, "...responding with : " + response.ToString());
#endif

            senderCollection[sourceIID.Address].send((uint)ReservedObjectID.RPC3_SimpleCaller_ResponseChannel, response);
            return null;
        }

		private QS.Fx.Serialization.ISerializable responseCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_SimpleCaller
            logger.Log(this, "__ResponseCallback : " + sourceIID.ToString() + ", " + receivedObject.ToString());
#endif

			try
			{
				Response response = (Response)receivedObject;
				OutgoingRequest request;
				uint seqno = response.SeqNo;

#if DEBUG_SimpleCaller
				logger.Log(this, "__Acknowledged : " + seqno.ToString());
#endif

				lock (requests)
				{
					request = requests[seqno];
					requests.Remove(seqno);
				}

				request.alarmRef.Cancel();

				request.succeeded = response.Object.OperationSucceeded;
				request.responseObject = response.Object.Object;
				request.exception = response.Object.Exception;
				request.completed = true;
				request.completionEvent.Set();

				if (request.callback != null)
					request.callback(request);
			}
#if DEBUG_SimpleCaller
			catch (Exception exc)
			{
				logger.Log(this, "__ResponseCallback: Cannot process response \"" + Helpers.ToString.ReceivedObject(sourceIID.Address, receivedObject) +
					"\".\n" + exc.ToString());
#else
            catch (Exception)
            {
#endif
			}

			return null;
        }

        #endregion

		protected override QS._qss_c_.Base3_.ISerializableCaller createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Caller(this, destinationAddress);
        }

        #region Requests and Responses

        #region Class OutgoingRequest

        private class OutgoingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>, IAsyncResult
        {
            public OutgoingRequest(uint destinationLOID, QS.Fx.Serialization.ISerializable data, AsyncCallback callback, object state) 
                : base(new QS._core_c_.Base3.Message(destinationLOID, data))
            {
                this.callback = callback;
                this.state = state;

                completed = false;
                completionEvent = new System.Threading.ManualResetEvent(false);
            }

            public QS.Fx.Clock.IAlarm alarmRef;
            public AsyncCallback callback;
            public object state;
            public System.Threading.ManualResetEvent completionEvent;
            public bool completed, succeeded;
            public QS.Fx.Serialization.ISerializable responseObject;
            public System.Exception exception = null;

            public override ClassID ClassID
            {
                get { return QS.ClassID.RPC3_SimpleCaller_Request; }
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get { return state; }
            }

            public System.Threading.WaitHandle AsyncWaitHandle
            {
                get { return completionEvent; }
            }

            public bool CompletedSynchronously
            {
                get { return false; }
            }

            public bool IsCompleted
            {
                get { return completed; }
            }

            #endregion
        }

        #endregion

        #region Class IncomingRequest

        [QS.Fx.Serialization.ClassID(QS.ClassID.RPC3_SimpleCaller_Request)]
        public class IncomingRequest : Base3_.InSequence<QS._core_c_.Base3.Message>
        {
            public IncomingRequest()
            {
            }
        }

        #endregion

        #region Class Response

        [QS.Fx.Serialization.ClassID(QS.ClassID.RPC3_SimpleCaller_Response)]
        public class Response : Base3_.InSequence<Base3_.Result>
        {
            public Response()
            {
            }

            public Response(uint seqno, QS.Fx.Serialization.ISerializable responseObject, bool succeeded, System.Exception exception)
                : base(seqno, new QS._qss_c_.Base3_.Result(succeeded, responseObject, exception))
            {
            }

            public override ClassID ClassID
            {
                get { return QS.ClassID.RPC3_SimpleCaller_Response; }
            }
        }

        #endregion

        #endregion

        #region Class Caller

        public class Caller : QS._qss_c_.Base3_.ISerializableCaller
        {
            public Caller(SimpleCaller owner, QS.Fx.Network.NetworkAddress address)
            {
                this.owner = owner;
                this.alarmCallback = new QS.Fx.Clock.AlarmCallback(this.retransmissionCallback);
                this.underlyingSender = owner.senderCollection[address];
            }

            private SimpleCaller owner;
            private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
            private QS.Fx.Clock.AlarmCallback alarmCallback;

            private void retransmissionCallback(QS.Fx.Clock.IAlarm alarmRef)
            {
                OutgoingRequest request = (OutgoingRequest)alarmRef.Context;

#if DEBUG_SimpleCaller
                owner.logger.Log(this, "__RetransmissionCallback");
#endif

                if (!request.completed)
                    underlyingSender.send((uint)QS.ReservedObjectID.RPC3_SimpleCaller_RequestChannel, request);
            }

            #region ISerializableCaller Members

            public QS.Fx.Network.NetworkAddress Address
            {
                get { return underlyingSender.Address; }
            }

            public QS.Fx.Serialization.ISerializable Call(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                IAsyncResult asyncResult = this.BeginCall(destinationLOID, data, null, null);
                asyncResult.AsyncWaitHandle.WaitOne();
                return this.EndCall(asyncResult);
            }

            public IAsyncResult BeginCall(uint destinationLOID, QS.Fx.Serialization.ISerializable data, AsyncCallback callback, object state)
            {
                OutgoingRequest request = new OutgoingRequest(destinationLOID, data, callback, state);
                uint seqno = (uint) System.Threading.Interlocked.Increment(ref owner.sequenceNo);
                request.SeqNo = seqno;

                lock (owner.requests)
                {
                    owner.requests.Add(seqno, request);
                }

                request.alarmRef = owner.alarmClock.Schedule(owner.retransmissionTimeout, alarmCallback, request);

                underlyingSender.send((uint)QS.ReservedObjectID.RPC3_SimpleCaller_RequestChannel, request);

                return request;
            }

            public QS.Fx.Serialization.ISerializable EndCall(IAsyncResult asyncResult)
            {
                OutgoingRequest request = (OutgoingRequest)asyncResult;

                if (!request.completed)
                    throw new Exception("Request has not completed yet!");
                if (!request.succeeded)
                    throw ((request.exception != null) ? request.exception : new Exception("Request failed for unknown reason."));

                return request.responseObject;
            }
            
            public int MTU
            {
                get { throw new System.NotImplementedException(); }
            }

            #endregion
        }

        #endregion
    }
}
