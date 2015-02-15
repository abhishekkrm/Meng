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
using System.Diagnostics;

namespace QS._qss_c_.Diffusion_
{
	/// <summary>
	/// Summary description for GossipingObject.
	/// </summary>

/*
	public class DiffusionAgent : IDisposable
	{
		public delegate void ForwardingCallback(QS.Fx.Network.NetworkAddress forwardingAddress);

		public DiffusionAgent(IAddressSet addressSet, QS.Fx.Network.NetworkAddress localAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, IRoutingStructure routingStructure,
			ForwardingCallback forwardingCallback, 
			QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, double interval)
		{
			Debug.Assert(addressSet.Count == routingStructure.Size);

			lock (this)
			{
				this.addressSet = addressSet;
				this.forwardingCallback = forwardingCallback;
				this.localIndex = localIndex;
				this.routingStructure = routingStructure;

				uint[] incoming_indexes = routingStructure.incomingAt(localIndex, 66666);
				uint[] outgoing_indexes = routingStructure.outgoingAt(localIndex, 66666);
				incomingPeers = new Collections.LinkableHashSet((uint) incoming_indexes.Length);
				outgoingPeers = new Collections.LinkableHashSet((uint) outgoing_indexes.Length);
				forwardingSet = new Collections.LinkableHashSet((uint) 
					(incoming_indexes.Length + outgoing_indexes.Length));

				foreach (uint index in incoming_indexes)
				{
					GenericPeer peer = new IncomingPeer(index);
					incomingPeers.insert(peer);
					forwardingSet.insert(peer);
				}

				foreach (uint index in outgoing_indexes)
				{
					GenericPeer peer = new OutgoingPeer(index);
					incomingPeers.insert(peer);
					forwardingSet.insert(peer);
				}


				// .........................................................

				this.alarmRef = alarmClock.Schedule(interval, 
					new QS.Fx.QS.Fx.Clock.AlarmCallback(this.alarmCallback), null);
			}
		}

		private QS.Fx.QS.Fx.Clock.IAlarm alarmRef;
		private ForwardingCallback forwardingCallback;
		private IAddressSet addressSet;
		private QS.Fx.Network.NetworkAddress localAddress, destinationAddress;
		private uint localIndex, destinationIndex;
		private IRoutingStructure routingStructure;
		private Collections.ILinkableHashSet incomingPeers, outgoingPeers, forwardingSet;

		#region Class GenericPeer

		private abstract class GenericPeer : Collections.GenericLinkable
		{
			public GenericPeer(uint index)
			{
				this.index = index;
			}

			private uint index;

			public uint IndexOf
			{
				get
				{
					return index;
				}
			}

			public override object Contents
			{
				get
				{
					return index;
				}
			}
		}

		#endregion

		#region Class IncomingPeer

		private class IncomingPeer : GenericPeer
		{
			public IncomingPeer(uint index) : base(index)
			{
			}
		}

		#endregion

		#region Class OutgoingPeer

		private class OutgoingPeer : GenericPeer
		{
			public OutgoingPeer(uint index) : base(index)
			{
			}
		}

		#endregion

		private void alarmCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
		{
			lock (this)
			{
				foreach (GenericPeer peer in forwardingSet)
					this.forwardingCallback(addressSet[peer.IndexOf]);

				// ..............
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			lock (this)
			{
				alarmRef.Cancel();
			}
		}

		#endregion
	}
*/	
}
