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

namespace QS._qss_c_.RPC2
{
/*
	/// <summary>
	/// Summary description for RPCClient.
	/// </summary>
	public class RPCProxy : IRPCProxy
	{
		private const double retransmissionTimeout = 1;

		private static void register_serializable()
		{
			Base2.WrappedIO.register_serializable();
			Base2.AddressedObject.register_serializable();
		}

		public RPCProxy(Base2.IDemultiplexer demultiplexer, Base2.IBlockSender underlyingSender,
            QS.Fx.Logging.ILogger logger, Base2.IIdentifiableObjectContainer objectContainer, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock) 
			: this(demultiplexer, underlyingSender, logger, objectContainer, alarmClock,
				(uint) ReservedObjectID.RPCProxy_RequestChannel, (uint) ReservedObjectID.RPCProxy_ResponseChannel)
		{
		}

		private RPCProxy(Base2.IDemultiplexer demultiplexer, Base2.IBlockSender underlyingSender,
            QS.Fx.Logging.ILogger logger, Base2.IIdentifiableObjectContainer objectContainer, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, 
			uint req_loid, uint res_loid)
		{
			register_serializable();

			this.demultiplexer = demultiplexer;
			this.req_loid = req_loid;
			this.res_loid = res_loid;
			this.underlyingSender = underlyingSender;
			this.logger = logger;
			this.objectContainer = objectContainer;
			this.alarmClock = alarmClock;
			retransmissionAlarmCallback = new QS.Fx.QS.Fx.Clock.AlarmCallback(this.retransmissionCallback);

			demultiplexer.register(this.req_loid, new Base2.ReceiveCallback(this.requestCallback)); 
			demultiplexer.register(this.res_loid, new Base2.ReceiveCallback(this.respondCallback)); 
		}

		private Base2.IDemultiplexer demultiplexer;
		private uint req_loid, res_loid;
		private Base2.IBlockSender underlyingSender;
		private QS.Fx.Logging.ILogger logger;
		private Base2.IIdentifiableObjectContainer objectContainer;
        private QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.QS.Fx.Clock.AlarmCallback retransmissionAlarmCallback;

		private Base2.IBase2Serializable requestCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable serializableObject)
		{
			Components.Sequencer.IWrappedObject wrapper = (Components.Sequencer.IWrappedObject) serializableObject;
			Base2.AddressedObject addressedObject = (Base2.AddressedObject) wrapper.SerializableObject;

			Base2.IBase2Serializable responseObject = demultiplexer.demultiplex(
				addressedObject.DestinationLOID, sourceAddress, destinationAddress, addressedObject.SerializableObject);

			wrapper.SerializableObject = (responseObject != null) ? responseObject : Base2.NullObject.Object;

			underlyingSender.send(this.res_loid, sourceAddress, wrapper);

			return null;
		}

		private Base2.IBase2Serializable respondCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable serializableObject)
		{
			logger.Log(this, "__respondCallback");

			Components.Sequencer.IWrappedObject reswrapper = (Components.Sequencer.IWrappedObject) serializableObject;
			Components.Sequencer.IWrappedObject reqwrapper = 
				(Components.Sequencer.IWrappedObject) objectContainer.remove(reswrapper);
			if (reqwrapper != null)
			{
				Request request = (Request) reqwrapper.SerializableObject;
				request.alarmRef.Cancel();
				request.retransmissionCancelled = true;
				
				request.callback(request.contextObject, 
					(reswrapper.SerializableObject is Base2.NullObject) ? null : reswrapper.SerializableObject);
			}
			else
			{
				logger.Log(this, "__respondCallback : cannot find the request");
			}

			return null;
		}

        private void retransmissionCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
		{
			logger.Log(this, "__retransmissionCallback");

			Request request = (Request) alarmRef.Context;
			if (!request.retransmissionCancelled)
			{
				underlyingSender.send(this.req_loid, request.calleeAddress, request.wrapper);
				alarmRef.Reschedule();
			}
		}

		#region Class Request

		[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
		private class Request : Base2.AddressedObject
		{
			public Request(Base.ObjectAddress calleeAddress, Base2.IBase2Serializable argumentObject, 
				RPCCallback callback, object contextObject) 
				: base(calleeAddress.LocalObjectID, argumentObject)
			{
				this.calleeAddress = calleeAddress;
				this.callback = callback;
				this.contextObject = contextObject;
			}

			public Base.ObjectAddress calleeAddress;
			public RPCCallback callback;
			public object contextObject;
			
			public Components.Sequencer.IWrappedObject wrapper;
            public QS.Fx.QS.Fx.Clock.IAlarm alarmRef;
			public bool retransmissionCancelled = false;
		}

		#endregion

		#region IProxy Members

		public void call(Base.ObjectAddress calleeAddress, Base2.IBase2Serializable argumentObject, 
			RPCCallback callback, object contextObject)
		{
			Request request = new Request(calleeAddress, argumentObject, callback, contextObject);
			request.wrapper = Components.Sequencer.wrap(request);
			objectContainer.insert(request.wrapper);
			request.alarmRef = alarmClock.Schedule(retransmissionTimeout, retransmissionAlarmCallback, request);
			underlyingSender.send(this.req_loid, calleeAddress, request.wrapper);
		}

		#endregion
	}
*/
}
