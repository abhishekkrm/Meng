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

namespace QS._qss_e_.Tests_.Test101
{
/*
	/// <summary>
	/// A test to be used as a pair with test 102 with experiment 003 to catch some basic bugs in the GMS service.
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
			if (localIPAddress.Equals(gmsIPAddress))
			{
				platform.Logger.writeLine("Starting up a GMS server on a local node.");

				GMS.ClientServer.DirtyFactoryLoader.loadFactories();
				serverCMS = new CMS.Base2.CMSWrapper(platform, gmsAddress);			
				serverGMS = new GMS.ClientServer.ServerGMS(serverCMS);
			}

			platform.Logger.writeLine("Connecting to GMS server at " + gmsAddress.ToString() + ".");
			
			clientCMS = new QS.CMS.Base2.CMSWrapper(platform, new QS.Fx.Network.NetworkAddress(localIPAddress, 11002));
			clientGMS = new QS.GMS.ClientServer.ClientGMS(gmsAddress.HostIPAddress, gmsAddress.PortNumber, clientCMS);
			localFDClient = new GMS.ClientServer.ClientFD(clientCMS);

/-*
			demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
			rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);

			// ........

			scatterer = new CMS.Scattering.Scatterer(rootSender);

			retransmittingScatterer = new QS.CMS.Scattering.RetransmittingScatterer(platform.Logger, 
				demultiplexer, scatterer, rootSender, null, platform.AlarmClock, 0.1, false);

			demultiplexer.register(1000, new CMS.Base2.ReceiveCallback(this.receiveCallback));
*-/
		}

		private CMS.Base2.CMSWrapper clientCMS, serverCMS = null;
		private GMS.ClientServer.ClientGMS clientGMS;
		private GMS.ClientServer.ServerGMS serverGMS = null; 
		private GMS.ClientServer.ClientFD localFDClient;

		private CMS.Platform.IPlatform platform;

/-*
		private CMS.Base2.IDemultiplexer demultiplexer;
		private CMS.Base2.RootSender rootSender;
		private CMS.Scattering.IScatterer scatterer;
		private CMS.Scattering.RetransmittingScatterer retransmittingScatterer;
*-/

		public void join(GMS.GroupId groupID)
		{
			clientGMS.joinGroup(groupID, 100, new QS.GMS.ViewChangeUpcall(this.viewChangeCallback));
		}

		private void viewChangeCallback(GMS.GroupId groupID, GMS.IView membershipView)
		{
			platform.Logger.writeLine("View Change Callback : Group " + groupID.ToString() + ", View " + 
				membershipView.SeqNo.ToString() + ".");
		}

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS.CMS.Base2.IBase2Serializable wrappedObject)
		{
			
			// ........................................

		}

		#region IDisposable Members

		public void Dispose()
		{
			clientCMS.shutdown();

			if (serverCMS != null)
				serverCMS.shutdown();
		}

		#endregion
	}
*/ 
}
