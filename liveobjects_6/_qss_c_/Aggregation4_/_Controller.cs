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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation4_
{
/*
	[TMS.Inspection.Inspectable]
	public abstract class Controller<IncomingClass, OutgoingClass, NotificationClass> 
		: TMS.Inspection.Inspectable, IAggregationController, IAsyncResult
		where IncomingClass : new()
		where OutgoingClass : new()
		where NotificationClass : QS.Fx.Serialization.ISerializable
	{
		public Controller(IAggregationContext context)
		{
			this.context = context;
			timeCreated = context.Clock.Time;
		}

		#region System.IDisposable Members

		public virtual void Dispose()
		{
		}

		#endregion

		private IAggregationContext context;
		private IChannelController channelController;
		private int seqNo;
		private AsyncCallback completionCallback;
		private object asynchronousState;
		private bool isCompleted = false;
		private System.Threading.ManualResetEvent completionWaitHandle = null;

		protected Aggregation3.IAggregatable aggregatedObject = null;
		protected double timeCreated;
		protected QS.Fx.Serialization.ISerializable dataObject = null;
		protected System.Collections.Generic.IDictionary<QS.Fx.Network.NetworkAddress, IncomingClass> incomingPeers;
		protected OutgoingClass outgoingPeer;

		#region Struct SendReq

		protected struct SendReq
		{
			public SendReq(QS.Fx.Network.NetworkAddress address, QS.Fx.Serialization.ISerializable serializableObject)
			{
				this.Address = address;
				this.SerializableObject = serializableObject;
			}

			public QS.Fx.Network.NetworkAddress Address;
			public QS.Fx.Serialization.ISerializable SerializableObject;
		}

		#endregion

		#region Sending

		private void Send(SendReq sendreq, Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> senders)
		{
			Base3.Message message = channelController.CreateMessage(this, sendreq.SerializableObject);
			senders[sendreq.Address].send(message.destinationLOID, message.transmittedObject);
		}

		private void Send(System.Collections.Generic.IList<SendReq> notifications,
			System.Collections.Generic.IList<SendReq> forwardings)
		{
			if (notifications != null && notifications.Count > 0)
			{
				foreach (SendReq sendreq in notifications)
					send(sendreq, context.NotificationSenders);
			}

			if (forwardings != null && forwardings.Count > 0)
			{
				foreach (SendReq sendreq in forwardings)
					send(sendreq, context.ForwardingSenders);
			}
		}

		#endregion

		#region Struct ActionContext

		protected struct ActionContext
		{
			public static ActionContext Create
			{
				get
				{
					ActionContext context = new ActionContext();
					context.Initialize();
					return context;
				}
			}

			private void Initialize()
			{
				notifications = new List<SendReq>();
				forwardings = new List<SendReq>();
				invokeCallback = removeThyself = false;
			}

			private System.Collections.Generic.List<SendReq> notifications, forwardings;
			private bool invokeCallback, removeThyself;

			#region Accessors

			public System.Collections.Generic.IList<SendReq> Notifications
			{
				get { return notifications; }
			}

			public System.Collections.Generic.IList<SendReq> Forwardings
			{
				get { return forwardings; }
			}

			public bool InvokeCallback
			{
				get { return invokeCallback; }
				set { invokeCallback = value; }
			}
			
			public bool RemoveThyself
			{
				get { return finalizeCleanup; }
				set { removeThyself = value; }
			}

			#endregion
		}

		#endregion

		#region Internal Processing

		private void ReceivedData(QS.Fx.Serialization.ISerializable data, 
			QS.CMS.Aggregation3.IAggregatable toAggregate, bool forwarded)
		{
			ActionContext actionContext = ActionContext.Create;

			lock (this)
			{
				// ...........
			}

			HandleContext(actionContext);
		}

		private void ReceivedNotification(QS.Fx.Network.NetworkAddress sourceAddress, N notification)
		{
			ActionContext actionContext = ActionContext.Create;

			lock (this)
			{
				// ...........

			}

			HandleContext(actionContext);
		}

		private void HandleContext(ActionContext actionContext)
		{
			Send(actionContext.Notifications, actionContext.Forwardings);

			if (actionContext.InvokeCallback)
				completionCallback(this);

			if (actionContext.RemoveThyself)
				RemoveThyself();
		}

		protected void SetCompleted(ActionContext actionContext)
		{
			isCompleted = true;
			if (completionWaitHandle != null)
				completionWaitHandle.Set();

			if (completionCallback != null)
			{
				actionContext.InvokeCallback = true;
			}
			else
			{
				actionContext.RemoveThyself = true;
			}
		}

		private void RemoveThyself()
		{
			channelController.RemoveCompleted(this);
		}

		#endregion

		#region IAggregationController Members

		void IAggregationController.Initialize(
			IChannelController channelController, int seqNo,
			System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> incomingAddresses, 
			QS.Fx.Network.NetworkAddress outgoingAddress)
		{
			lock (this)
			{
				this.channelController = channelController;
				this.seqNo = seqNo;

				incomingPeers = new System.Collections.Generic.Dictionary<QS.Fx.Network.NetworkAddress, IncomingClass>(
					incomingAddresses.Count);
				foreach (QS.Fx.Network.NetworkAddress incomingAddress in incomingAddresses)
					incomingPeers[incomingAddress] = new IncomingClass(incomingAddress);

				// ...........
			}
		}

		void IAggregationController.ReplaceIncoming(QS.Fx.Network.NetworkAddress deadAddress,
			System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> replacementAddresses)
		{
			throw new NotImplementedException();
		}

		void IAggregationController.ReplaceOutgoing(QS.Fx.Network.NetworkAddress deadAddress, QS.Fx.Network.NetworkAddress replacementAddress)
		{
			throw new NotImplementedException();
		}

		void IAggregationController.Receive(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
		{
			if (receivedObject is N)
			{
				ReceivedNotification(sourceAddress, (N) receivedObject);
			}
			else
			{
				ReceivedData(receivedObject, null, true);
			}
		}

		#endregion

		#region IAggregation Members

		IAsyncResult IAggregation.BeginAggregate(AsyncCallback completionCallback, object asynchronousState)
		{
			this.completionCallback = completionCallback;
			this.asynchronousState = asynchronousState;

			return this;
		}

		void IAggregation.EndAggregate(IAsyncResult asynchronousResult)
		{
			RemoveThyself();
		}

		void IAggregation.Submit(QS.Fx.Serialization.ISerializable data, QS.CMS.Aggregation3.IAggregatable toAggregate)
		{
			ReceivedData(data, toAggregate, false);
		}

		IChannel IAggregation.Channel
		{
			get { return channelController; }
		}

		int IAggregation.SeqNo
		{
			get { return seqNo; }
		}

		QS.CMS.Aggregation3.IAggregatable IAggregation.AggregatedObject
		{
			get { return aggregatedObject; }
		}

		#endregion

		#region IAsyncResult Members

		object IAsyncResult.AsyncState
		{
			get { return asynchronousState; }
		}

		System.Threading.WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get 
			{
				lock (this)
				{
					if (completionWaitHandle == null)
						completionWaitHandle = new System.Threading.ManualResetEvent(isCompleted);
				}

				return completionWaitHandle;
			}
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get { return false; }
		}

		bool IAsyncResult.IsCompleted
		{
			get { return isCompleted; }
		}

		#endregion
	}
*/
}
