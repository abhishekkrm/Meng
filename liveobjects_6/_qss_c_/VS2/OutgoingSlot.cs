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

namespace QS._qss_c_.VS_2_
{
	/// <summary>
	/// Summary description for OutgoingSlot.
	/// </summary>
/* 
	public class OutgoingSlot : OutgoingMessageRef	
	{
		public OutgoingSlot(uint senderLOID, 
			GMS.GroupId groupID, Base.IMessage message, Base.SendCallback sendCallback) 
			: base(senderLOID, groupID, message, sendCallback)
		{
			currentPhase = Senders.FBCASTSender.ProtocolPhase.INITIALLY_SENDING_MESSAGE;
		}

		public QS.Fx.QS.Fx.Clock.IAlarm alarmRef;

		protected override void initializeAfterRegistering()
		{
			receiverStates = new Collections.Hashtable((uint) view.theView.NumberOfSubViews);

			for (uint ind = 0; ind < view.theView.NumberOfSubViews; ind++)
			{
				QS.Fx.Network.NetworkAddress receiverAddress = new QS.Fx.Network.NetworkAddress(view.theView[ind].IPAddress, view.theView[ind].PortNumber);
				receiverStates[receiverAddress] = new ReceiverState(receiverAddress);
			}
		}

//		protected override bool stable()
//		{
//			return currentPhase >= Senders.FBCASTSender.ProtocolPhase.MESSAGE_DELIVERED;
//		}

		public void disposeOfMessage()
		{
			message = null;
		}

		public Senders.FBCASTSender.ProtocolPhase CurrentPhase
		{
			set
			{
				currentPhase = value;
			}

			get
			{
				return currentPhase;
			}
		}

		public void resetACKs()
		{
			foreach (ReceiverState receiverState in receiverStates.Values)
			{
				receiverState.acknowledged = false;
			}
		}

		public System.Collections.ICollection Pending
		{
			get
			{
				System.Collections.ArrayList pending = new System.Collections.ArrayList();				
				object[] rStates = receiverStates.Values;
				foreach (ReceiverState receiverState in rStates)
				{
					if (!((Group.View.ReceiverState) view.receiverStates[receiverState.receiverAddress]).failed 
						&& !receiverState.acknowledged)
					{
						pending.Add(receiverState.receiverAddress);
					}
				}
				return pending;
			}
		}

		public bool markAsConfirmed(QS.Fx.Network.NetworkAddress networkAddress)
		{
			bool alreadyConfirmed = ((ReceiverState) receiverStates[networkAddress]).acknowledged;
			((ReceiverState) receiverStates[networkAddress]).acknowledged = true;
			return !alreadyConfirmed;
		}

		// TODO: Calls for a more efficient implementation... but for now, let's just make it work
		public bool AllConfirmed
		{
			get
			{
				return Pending.Count == 0;
			}
		}

		private class ReceiverState
		{
			public ReceiverState(QS.Fx.Network.NetworkAddress receiverAddress)
			{
				this.receiverAddress = receiverAddress;
				acknowledged = false;
			}

			public QS.Fx.Network.NetworkAddress receiverAddress;
			public bool acknowledged;
		}

		private Senders.FBCASTSender.ProtocolPhase currentPhase;
		private Collections.Hashtable receiverStates = null; 
	}
*/	
}
