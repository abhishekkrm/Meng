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

namespace QS._qss_e_.Tests_.Test102
{
/*
	/// <summary>
	/// This test runs a GMS server on a node, to be used with experiment 003.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
		{
			IPAddress localIPAddress;
			if (args.contains("base"))
				localIPAddress = IPAddress.Parse((string) args["base"]);
			else if (args.contains("subnet"))
			{
				localIPAddress = CMS.Devices2.Network.AnyAddressOn(
					new CMS.Base.Subnet((string) args["subnet"]), platform);
			}
			else 
				localIPAddress = platform.NICs[0];
			QS.Fx.Network.NetworkAddress localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, 11001);

			GMS.ClientServer.DirtyFactoryLoader.loadFactories();

			cms = new CMS.Base2.CMSWrapper(platform, localAddress);			
			gms = new GMS.ClientServer.ServerGMS(cms);
		}

		private CMS.Base2.CMSWrapper cms;
		private GMS.ClientServer.ServerGMS gms; 

		#region IDisposable Members

		public void Dispose()
		{
			cms.shutdown();
		}

		#endregion
	}
*/ 
}
