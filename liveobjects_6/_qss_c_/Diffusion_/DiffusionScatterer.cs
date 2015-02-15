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

namespace QS._qss_c_.Diffusion_
{
	/// <summary>
	/// Summary description for MulticastingSender.
	/// </summary>

/*
	public class DiffusionScatterer : IScatterer
	{
		public DiffusionScatterer(IRoutingAlgorithm routingAlgorithm, QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock, double gossipingInterval)
		{
			this.routingStructureType = routingStructureType;
			this.alarmClock = alarmClock;
			this.gossipingInterval = gossipingInterval;
		}

		private System.Type routingStructureType;
		private double gossipingInterval;
		private QS.Fx.QS.Fx.Clock.IAlarmClock alarmClock;

		#region Request

		private class Request
		{
			public Request(uint destinationLOID, IAddressSet destinationAddressSet, DiffusionScatterer diffusionScatterer, IMessage message)
			{
				this.destinationLOID = destinationLOID;
				this.destinationAddressSet = destinationAddressSet;
				this.message = message;
				this.diffusionScatterer = diffusionScatterer;

				diffusionAgent = new DiffusionAgent(destinationAddressSet, 666666666, 
					RoutingStructure.instantiate(diffusionScatterer.routingStructureType, destinationAddressSet.Count), 
					new DiffusionAgent.ForwardingCallback(this.forwardingCallback), diffusionScatterer.alarmClock, 
					diffusionScatterer.gossipingInterval);

			}

			private uint destinationLOID;
			private IAddressSet destinationAddressSet;
			private IMessage message;
			private DiffusionScatterer diffusionScatterer;

			private DiffusionAgent diffusionAgent;

			private void forwardingCallback(QS.Fx.Network.NetworkAddress networkAddress)
			{
				// .................
			}



		}

		#endregion

		#region IScatterer Members

		public void send(uint destinationLOID, IAddressSet destinationAddressSet, IMessage message)
		{
			Request request = new Request(destinationLOID, destinationAddressSet, this, message);

			// ...............



		}

		#endregion
	}
*/	
}
