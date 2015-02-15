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

using System.Threading;

namespace QS._qss_c_.Aggregation2_
{
/*
	public class AggregationAgent : IAggregationAgent
	{
		public AggregationAgent(QS.Fx.Logging.ILogger logger, Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> senderCollection,
			Base3.IDemultiplexer demultiplexer)
		{
			this.logger = logger;
			this.senderCollection = senderCollection;

			// this.demultiplexer = demultiplexer;
			demultiplexer.register((uint) ReservedObjectID.Aggregation2_AggregationAgent, new QS.CMS.Base3.ReceiveCallback(this.receiveCallback));
		}

		private QS.Fx.Logging.ILogger logger;
		private Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> senderCollection;
		// private Base3.IDemultiplexer demultiplexer;
		private System.Collections.Generic.IDictionary<AggregationClassID, IAggregationClass> aggregationClasses =
			new System.Collections.Generic.Dictionary<AggregationClassID, IAggregationClass>();
		private System.Collections.Generic.IDictionary<AggregationChannelID, ChannelController> channelControllers =
			new System.Collections.Generic.Dictionary<AggregationChannelID, ChannelController>();

		#region AggregationChannel

		private class ChannelController
		{
			public ChannelController(AggregationAgent owner, IAggregationChannel aggregationChannel)
			{
				this.owner = owner;
				this.aggregationChannel = aggregationChannel;
			}

			private AggregationAgent owner;
			private IAggregationChannel aggregationChannel;

			public void receive(QS.Fx.Network.NetworkAddress sourceAddress, int seqno)
			{
				// .................................................
			}
		}

		#endregion

		#region Receive Callback

		private QS.Fx.Serialization.ISerializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
		{
			AggregationMessage aggregationMessage = receivedObject as AggregationMessage;
			if (aggregationMessage != null)
			{
				AggregationChannelID aggregationChannelID = aggregationMessage.ChannelID;
				ChannelController channelController;
				lock (channelControllers)
				{
					if (channelControllers.ContainsKey(aggregationChannelID))
						channelController = channelControllers[aggregationChannelID];
					else
					{
						IAggregationClass aggregationClass = aggregationClasses[aggregationMessage.ClassID];
						channelController = new ChannelController(this, aggregationClass.GetChannel(aggregationChannelID, 
							new RemoveChannelCallback(this.removeChannel)));
						channelControllers[aggregationChannelID] = channelController;
					}
				}

				channelController.receive(sourceAddress, aggregationMessage.AggregationSeqNo);
			}
			else
				throw new Exception("Wrong message type: " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));

			return null;
		}

		#endregion

		#region Remove Channel Callback

		private void removeChannel(IAggregationChannel aggregationChannel)
		{
			lock (channelControllers)
			{
				channelControllers.Remove(aggregationChannel.ID);
			}
		}

		#endregion

		#region AggregationMessage

		public class AggregationMessage : QS.Fx.Serialization.ISerializable
		{
			public AggregationMessage()
			{
			}

			#region Accessors

			public AggregationClassID ClassID
			{
				get
				{
					// .................................................................................................
					return default(AggregationClassID);
				}
			}

			public AggregationChannelID ChannelID
			{
				get
				{
					// .................................................................................................
					return null;
				}
			}

			public int AggregationSeqNo
			{
				get
				{
					// ..........................................................
					return 0;
				}
			}

			#endregion

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get { throw new global::System.NotImplementedException(); }
			}

			void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				throw new NotImplementedException();
			}

			void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		#endregion

		#region Aggregation2.IAggregationAgent Members

		public void registerClass(IAggregationClass aggregationClass)
		{
			lock (aggregationClasses)
			{
				aggregationClasses.Add(aggregationClass.ID, aggregationClass);
			}
		}

		public void aggregate()
		{
		}

		public void submit()
		{
		}

//		public IAggregationController aggregate(IAggregationKey aggregationKey, AggregationCallback aggregationCallback)
//		{
//			// ..................................................................
//			return null;
//		}

//		public void submit(IAggregationKey aggregationKey)
//		{
//			// ..................................................................
//		}
//

		#endregion
	}
*/
}
