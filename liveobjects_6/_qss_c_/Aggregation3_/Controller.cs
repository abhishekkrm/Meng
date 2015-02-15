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

// #define DEBUG_AggregationController

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation3_
{
	public abstract class Controller
        : QS.Fx.Inspection.Inspectable, IController, IAsyncResult, QS.Fx.Logging.ILogger, Aggregation1_.IAggregationController
	{
		public Controller()
		{
		}

		public delegate void SendCallback(Controller sender, QS._core_c_.Base3.InstanceID receiverAddress, QS.Fx.Serialization.ISerializable message);
		public delegate void RemoveCallback(Controller removedController);

		public void preinitialize(IChannel channel, int seqno, SendCallback sendCallback, RemoveCallback removeCallback,
			IList<QS._core_c_.Base3.InstanceID> incomingAddresses, QS._core_c_.Base3.InstanceID outgoingAddress, QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;

			this.channel = channel;
			this.seqno = seqno;
			this.sendCallback = sendCallback;
			this.removeCallback = removeCallback;

			initialize(incomingAddresses, outgoingAddress);
		}

		protected IChannel channel;
		[QS.Fx.Base.Inspectable("SeqNo", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected int seqno;
		[QS.Fx.Base.Inspectable("Completed", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected bool completed = false;

		private AsyncCallback completionCallback = null;
		private object asynchronousState;
		private SendCallback sendCallback;
		private RemoveCallback removeCallback;

		[QS.Fx.Base.Inspectable("HasCompletionCallback")]
		public bool HasCompletionCallback
		{
			get { return completionCallback != null; }
		}

#if DEBUG_AggregationController
		[TMS.Inspection.Inspectable("Operation Log")]
		protected QS.Fx.Logging.ILogger operation_log = new Base.Logger(true, null, false, string.Empty);
#endif

		protected QS.Fx.Logging.ILogger logger;

		#region Methods to Use

		protected void send(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message)
		{
#if DEBUG_AggregationController
			operation_log.logMessage(this, "Send : " + address.ToString());
#endif

			sendCallback(this, address, new Agent.Message(channel.ChannelID, seqno, message));
		}

		protected void collected(QS.Fx.Serialization.ISerializable data)
		{
		}

		protected void invokeCallback()
		{
			if (completionCallback != null)
				completionCallback(this);
			else
			{
#if DEBUG_AggregationController
				((QS.Fx.Logging.ILogger) this).logMessage(this, "Cannot invoke callback: callback is NULL.");
#endif
			}
		}

		protected void remove()
		{
			removeCallback(this);
		}

		#endregion

		#region Methods to Override

		protected abstract void initialize(IList<QS._core_c_.Base3.InstanceID> incomingAddresses, QS._core_c_.Base3.InstanceID outgoingAddress);

		protected abstract void receive(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message);

		protected abstract void aggregate();

		protected abstract void submit(QS.Fx.Serialization.ISerializable data, IAggregatable toAggregate);

		#endregion

		public void receiveCallback(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message)
		{
#if DEBUG_AggregationController
			operation_log.logMessage(this, "Receive " + address.ToString() + ", " + message.ToString());
#endif

			receive(address, message);
		}

		#region IController Members

		public virtual IAggregatable AggregatedObject
		{
			get { return null; }
		}

		IChannel IController.Channel
		{
			get { return channel; }
		}

		int IController.SeqNo
		{
			get { return seqno; }
		}

		IAsyncResult IController.BeginAggregate(AsyncCallback completionCallback, object asynchronousState)
		{
#if DEBUG_AggregationController
			operation_log.logMessage(this, "BeginAggregate");
#endif

			if (completionCallback == null)
				throw new Exception("No callback has been provided.");

			lock (this)
			{
				if (this.completionCallback != null)
					throw new Exception("Aggregation on this controller has already been initiated.");

				this.completionCallback = completionCallback;
				this.asynchronousState = asynchronousState;
			}

			aggregate();

			return this;
		}

		void IController.Submit(QS.Fx.Serialization.ISerializable data, IAggregatable toAggregate)
		{
#if DEBUG_AggregationController
			operation_log.logMessage(this, "Submit");
#endif

			submit(data, toAggregate);
		}

		#endregion

		#region IAsyncResult Members

		object IAsyncResult.AsyncState
		{
			get { return asynchronousState; }
		}

		System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get { throw new NotSupportedException(); }
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get { return false; }
		}

		bool IAsyncResult.IsCompleted
		{
			get { return completed; }
		}

		#endregion

		#region ILogger Members

		void QS.Fx.Logging.ILogger.Clear()
		{
			throw new NotImplementedException();
		}

        void QS.Fx.Logging.ILogger.Log(object source, string message)
		{
			logger.Log(source, seqno.ToString() + " " + message);
		}

		#endregion

		#region IConsole Members

        void QS.Fx.Logging.IConsole.Log(string s)
		{
			((QS.Fx.Logging.ILogger)this).Log(null, s);
		}

		#endregion

		#region IAggregationController Members

		QS._qss_c_.Aggregation1_.AggregationID QS._qss_c_.Aggregation1_.IAggregationController.ID
		{
			get 
			{
				Base3_.ViewID viewID = channel.ChannelID.GroupID as Base3_.ViewID;
				if (viewID == null)
					throw new NotSupportedException("This controller is not backward compatible with this type of group.");

				return new Aggregation1_.AggregationID(new Multicasting3.MessageID(
					viewID.GroupID, viewID.ViewSeqNo, (uint) seqno), channel.ChannelID.RootAddress); 
			}
		}

		IAggregatable QS._qss_c_.Aggregation1_.IAggregationController.AggregatedObject
		{
			get { return this.AggregatedObject; }
		}

		#endregion
	}
}
