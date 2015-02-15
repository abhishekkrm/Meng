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

#define DEBUG_MembershipServer

using System;

namespace QS._qss_c_.Membership_1_
{
/*
	/// <summary>
	/// Summary description for Server.
	/// </summary>
	public class Server
	{
		public Server(QS.Fx.Logging.ILogger logger, Scattering.IRetransmittingScatterer retransmittingScatterer)
		{
			this.logger = logger;
			this.retransmittingScatterer = retransmittingScatterer;
		}

		private QS.Fx.Logging.ILogger logger;
		private Scattering.IRetransmittingScatterer retransmittingScatterer;
		private uint internalSeqNo = 0;

		public void distributeVCNotification(GMS.GroupId groupID, GMS.ClientServer.ImmutableView membershipView)
		{
			GMS.ClientServer.WholeViewMessage wholeViewMessage = 
				new GMS.ClientServer.WholeViewMessage(groupID, membershipView, ++internalSeqNo);
			System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
			wholeViewMessage.save(memoryStream);
			Base2.IBase2Serializable transmittedObject = new Base2.BlockOfData(memoryStream);				
			
			QS.Fx.Network.NetworkAddress[] destinations = new QS.Fx.Network.NetworkAddress[membershipView.NumberOfSubViews];
			for (uint ind = 0; ind < destinations.Length; ind++)
				destinations[ind] = VS5.GenericVSView.SubView2NetworkAddress(membershipView[ind]);

			Scattering.IScatterSet scatterSet = new Scattering.ScatterSet(destinations);
#if DEBUG_MembershipServer
//			scatterSet.Logger = this.logger;
#endif

			retransmittingScatterer.multicast((uint) ReservedObjectID.Membership_Client, scatterSet, 
				Components.Sequencer.wrap(transmittedObject), 
				new Scattering.Callback(this.scatteringCompletionCallback));
		}

		private void scatteringCompletionCallback(bool success, System.Exception exception)
		{
			logger.Log(this, "_ScatteringCompletionCallback: success = " + success.ToString());
		}

		#region Class ViewChangeNotification

/-*
		[System.Serializable]
		public class ViewChangeNotification
		{
			public ViewChangeNotification()
			{
			}

			public ViewChangeNotification(GMS.GroupId groupID, GMS.ClientServer.ImmutableView membershipView)
			{
				this.groupID = groupID;
				this.membershipView = membershipView;
			}

			private GMS.GroupId groupID;
			private GMS.ClientServer.ImmutableView membershipView;

			public GMS.GroupId GroupID
			{
				set
				{
					groupID = value;
				}

				get
				{
					return groupID;
				}
			}

			public GMS.ClientServer.ImmutableView MembershipView
			{
				set
				{
					membershipView = value;
				}

				get
				{
					return membershipView;
				}
			}
		}
*-/

		#endregion
	}
*/ 
}
