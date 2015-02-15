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

namespace QS._qss_c_.Base2_
{
/*
	/// <summary>
	/// Summary description for CMS.
	/// </summary>
	public class CMS : ICMS
	{
		public CMS(Platform.IPlatform platform, QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Network.NetworkAddress gmsAddress,
			QS.Fx.Network.NetworkAddress allocationServerAddress)
		{
			this.platform = platform;
			this.demultiplexer = new Demultiplexer(platform.Logger);
			this.rootSender = new RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);			

			QS.CMS.Base2.Serializer.CommonSerializer.registerClasses(platform.Logger);

			wrapped_oldCMS = new QS.CMS.Base2.CMSWrapper(platform, rootSender.RootAddress);			

			QS.GMS.ClientServer.DirtyFactoryLoader.loadFactories();

			clientGMS = new QS.GMS.ClientServer.ClientGMS(gmsAddress.HostIPAddress, gmsAddress.PortNumber, wrapped_oldCMS);
			clientFD = new GMS.ClientServer.ClientFD(wrapped_oldCMS);

			scatterer = new Scattering.Scatterer(rootSender, platform.Logger);

			masterContainer = new MasterIOC();

			retransmittingScatterer = new QS.CMS.Scattering.RetransmittingScatterer(platform.Logger, 
				demultiplexer, scatterer, rootSender, masterContainer, platform.AlarmClock, 0.01, true);

			// Base2.Serializer.CommonSerializer.registerClass(ClassID.BlockOfData, typeof(BlockOfData));
			// StringWrapper.register_serializable();

			rpcProxy = new RPC2.RPCProxy(demultiplexer, rootSender, platform.Logger, masterContainer, platform.AlarmClock);

			allocationClient = new Allocation.AllocationClient(new Base.ObjectAddress(allocationServerAddress, 
				(uint) QS.ReservedObjectID.IPMulticast_AllocationServer), rpcProxy, demultiplexer, platform.Logger);

			viewController = new QS.CMS.VS5.SimpleVC2(platform.Logger, masterContainer, 
				retransmittingScatterer, demultiplexer, allocationClient, localAddress, platform.UDPDevice, rootSender);
			viewController.linkToGMS(clientGMS);
		}

		private Platform.IPlatform platform;
		private IDemultiplexer demultiplexer;
		private RootSender rootSender;
		private CMSWrapper wrapped_oldCMS;
		private GMS.ClientServer.ClientGMS clientGMS;
		private GMS.ClientServer.ClientFD clientFD;
		private IMasterIOC masterContainer;
		private Scattering.IScatterer scatterer;
		private Scattering.RetransmittingScatterer retransmittingScatterer;
		private VS5.SimpleVC2 viewController;
		private RPC2.RPCProxy rpcProxy;
		private Allocation.AllocationClient allocationClient;

		#region System.IDisposable Members

		public void Dispose()
		{

		}

		#endregion

		#region ICMS Members

		public QS.CMS.Platform.IPlatform Platform
		{
			get
			{
				return platform;
			}
		}

		public ISerializer Serializer
		{
			get
			{
				return Base2.Serializer.CommonSerializer;
			}
		}

		public IDemultiplexer Demultiplexer
		{
			get
			{
				return demultiplexer;
			}
		}

		public IBlockSender RootSender
		{
			get
			{
				return rootSender;
			}
		}

		public QS.GMS.IGMS GMS
		{
			get
			{
				return clientGMS;
			}
		}

		public QS.CMS.VS5.IVSSender GroupSender
		{
			get
			{
				return viewController;
			}
		}

		#endregion
	}
*/ 
}
