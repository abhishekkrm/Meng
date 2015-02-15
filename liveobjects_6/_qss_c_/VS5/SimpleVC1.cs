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
using System.Threading;

namespace QS._qss_c_.VS5
{
/*
	/// <summary>
	/// Summary description for SimpleVC.
	/// </summary>
	public class SimpleVC1 : GenericVSViewController
	{
		public SimpleVC1(QS.Fx.Logging.ILogger logger, Scattering.IRetransmittingScatterer underlyingMulticastingSender,
			Base2.IDemultiplexer demultiplexer) : base(logger, demultiplexer)
		{
			this.underlyingMulticastingSender = underlyingMulticastingSender;
		}

		private Scattering.IRetransmittingScatterer underlyingMulticastingSender;

		protected override IVSGroup createGroup(GMS.GroupId groupID, GMS.IView membershipView, QS.Fx.Logging.ILogger logger)
		{
			return new Group(groupID, membershipView, this, logger);
		}

		#region Class Group

		private class Group : GenericVSGroup
		{
			public Group(GMS.GroupId groupID, GMS.IView membershipView, SimpleVC1 encapsulatingVC, QS.Fx.Logging.ILogger logger)
				: base(groupID, membershipView, logger)
			{
				this.encapsulatingVC = encapsulatingVC;
			}

			private SimpleVC1 encapsulatingVC;

			protected override IVSView createView(QS.GMS.IView membershipView, bool initiallySuspended, QS.Fx.Logging.ILogger logger)
			{
				return new View(membershipView, initiallySuspended, logger);
			}

			protected override void multicastWrappedObject(GMS.GroupId groupID, uint viewSeqNo, 
				uint withinViewSeqNo, Base2.IBase2Serializable serializableObject, Scattering.IScatterSet scatterSet,
				Scattering.CompletionCallback completionCallback)
			{
				Monitor.Exit(currentView);
				Monitor.Exit(this);

				// encapsulatingVC.underlyingMulticastingSender.multicast(encapsulatingVC.LocalObjectID, 
				// 	scatterSet, new GenericVSMessage((GenericVSMessageID) messageID, serializableObject), null);
			}
		}

		#endregion	
	
		#region Class View

		private class View : GenericVSView
		{
			public View(GMS.IView membershipView, bool initiallySuspended, QS.Fx.Logging.ILogger logger) 
				: base(membershipView, initiallySuspended, logger)
			{
			}

			// ...........

			protected override IVSRCCDescriptor createRCCDescriptor(QS.Fx.Network.NetworkAddress sourceAddress)
			{
				return null;
			}

			protected override void distributeFlushingReport(bool initial, uint numberOfMessagesSent, 
				ReceiverReport[] receiverReports, Scattering.IScatterSet destinationSet)
			{
			}
		}

		#endregion
	}
*/
}
