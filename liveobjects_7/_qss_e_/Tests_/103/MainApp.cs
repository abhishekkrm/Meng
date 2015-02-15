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
using System.Net;

namespace QS._qss_e_.Tests_.Test103
{
/*
	/// <summary>
	/// Test to be used by experiment 004.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
		{
			this.platform = platform;

			IPAddress localIPAddress;
			if (args.contains("base"))
				localIPAddress = IPAddress.Parse((string) args["base"]);
			else if (args.contains("subnet"))
				localIPAddress = CMS.Devices2.Network.AnyAddressOn(new CMS.Base.Subnet((string) args["subnet"]), platform);
			else 
				localIPAddress = platform.NICs[0];
			QS.Fx.Network.NetworkAddress localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, 12022);

			platform.Logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

			IPAddress gmsIPAddress = IPAddress.Parse((string) args["gms"]);
			QS.Fx.Network.NetworkAddress gmsAddress = new QS.Fx.Network.NetworkAddress(gmsIPAddress, 11001);

			demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
			rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);
			scatterer = new CMS.Scattering.Scatterer(rootSender, platform.Logger);
			objectContainer = new CMS.Components.IdentifiableObjectContainer(100);
			retransmittingScatterer = new QS.CMS.Scattering.RetransmittingScatterer(platform.Logger, 
				demultiplexer, scatterer, rootSender, objectContainer, platform.AlarmClock, 0.1, false);

			CMS.Base2.Serializer.CommonSerializer.registerClass(ClassID.BlockOfData, typeof(CMS.Base2.BlockOfData));

			// demultiplexer.register(1000, new CMS.Base2.ReceiveCallback(this.receiveCallback));

			if (localIPAddress.Equals(gmsIPAddress))
			{
				platform.Logger.writeLine("Starting up a fake membership server on a local node.");

				membershipServer = new QS.CMS.Membership.Server(platform.Logger, retransmittingScatterer);
			}

			CMS.Components.Sequencer.register_serializable();

			CMS.Base.Serializer.Get.register(QS.ClassID.Mahesh_CSGMSImmutableView, 
				new CMS.Base.CreateSerializable(GMS.ClientServer.ImmutableView.factory));
			// GMS.ClientServer.DirtyFactoryLoader.loadFactories();

			membershipClient = new QS.CMS.Membership.Client(platform.Logger, demultiplexer);
			membershipClient.linkCMSToGMS(new GMS.ViewChangeRequest(this.viewChangeRequestCallback), null, null);
		}

		private void viewChangeRequestCallback(GMS.GroupId groupID, GMS.IView membershipView)
		{
			platform.Logger.Log(this, "ViewChangeRequestCallback : " + groupID.ToString() + ":" +
				membershipView.SeqNo.ToString());
		}

		public void distributeVCNotification(GMS.GroupId groupID, byte[] membershipViewAsBytes)
		{
			GMS.ClientServer.ImmutableView membershipView = (GMS.ClientServer.ImmutableView)
				CMS.Base2.CompatibilitySOWrapper.ByteArray2Object(membershipViewAsBytes);

			membershipServer.distributeVCNotification(groupID, membershipView);
		}

		private CMS.Platform.IPlatform platform;
		private CMS.Base2.IDemultiplexer demultiplexer;
		private CMS.Base2.RootSender rootSender;
		private CMS.Base2.IIdentifiableObjectContainer objectContainer;
		private CMS.Scattering.IScatterer scatterer;
		private CMS.Scattering.RetransmittingScatterer retransmittingScatterer;

		private CMS.Membership.Server membershipServer = null;
		private CMS.Membership.Client membershipClient = null;

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
*/ 
}
