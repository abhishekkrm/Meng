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

namespace QS._qss_c_.Gossiping_1_
{
/*
	public class GossipingSender1 : GossipingSender<GossipingSender1.Sender>, Base.IClient
	{
		public GossipingSender1(Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> underlyingSenderCollection,
			Base3.IDemultiplexer demultiplexer) : base()
		{
			this.underlyingSenderCollection = underlyingSenderCollection;
			demultiplexer.register(((Base.IClient)this).LocalObjectID, new QS.CMS.Base3.ReceiveCallback(receiveCallback));
		}

		private Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> underlyingSenderCollection;

		protected override Sender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return new Sender(this, destinationAddress);
		}

		private QS.Fx.Serialization.ISerializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
		{
			Base3.Message.Wrapper message = receivedObject as Base3.Message.Wrapper;
			if (message != null)
			{




			}
			else
			{


			}

			return null;
		}

		#region Class Sender

		public new class Sender : GossipingSender<Sender>.Sender
		{
			public Sender(GossipingSender1 owner, QS.Fx.Network.NetworkAddress destinationAddress) : base(destinationAddress)
			{
				this.owner = owner;
				underlyingSender = owner.underlyingSenderCollection[destinationAddress];
			}

			private GossipingSender1 owner;
			private QS.CMS.Base3.ISerializableSender underlyingSender;
			private int lastused_seqno = 0;
			private System.Collections.Generic.IDictionary<int, Request> requests =
				new System.Collections.Generic.SortedDictionary<int, Request>();

			#region Class Request

			public class Request : Base3.AsynchronousOperation
			{
				public Request(Sender owner, Base3.Message message, int seqno,
					QS.CMS.Base3.AsynchronousOperationCallback completionCallback, object asynchronousState)
					: base(completionCallback, asynchronousState)
				{
					this.owner = owner;
					this.message = new QS.CMS.Senders3.ReliableSender.Sender.OutgoingRequest(seqno, message);
					this.seqno = seqno;
				}

				private Sender owner;
				private Senders3.ReliableSender.Sender.OutgoingRequest message;

				public void send()
				{

				}

				public override void Unregister()
				{
					throw new NotImplementedException();
				}

				#region ISerializable Members

				QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
				{
					get { throw new global::System.NotImplementedException(); }
				}

				void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
				{
					
				}

				void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
				{
					throw new NotImplementedException();
				}

				#endregion
			}

			#endregion

			public override QS.CMS.Base3.IAsynchronousOperation BeginSend(
				uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
				QS.CMS.Base3.AsynchronousOperationCallback completionCallback, object asynchronousState)
			{
				Base3.Message message = new QS.CMS.Base3.Message(destinationLOID, data);
				Request request;
				lock (this)
				{
					int seqno = ++lastused_seqno;
					request = new Request(this, message, seqno, completionCallback, asynchronousState);
					requests.Add(seqno, request);
				}

				request.send();

				return request;
			}

			public override void EndSend(QS.CMS.Base3.IAsynchronousOperation asynchronousOperation)
			{
				throw new NotImplementedException();
			}

			public override void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
			{
				underlyingSender.send(destinationLOID, data);
			}

			public override int MTU
			{
				get { throw new global::System.NotImplementedException(); }
			}
		}

		#endregion

		#region IClient Members

		uint QS.CMS.Base.IClient.LocalObjectID
		{
			get { return (uint) ReservedObjectID.Gossiping_GossipingSender; }
		}

		#endregion
	}
*/
}
