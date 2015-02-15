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

namespace QS._qss_e_.Tests_.Test100
{
/*
	/// <summary>
	/// A simple scattering test used with experiment 002.
	/// </summary>
	public class MainApp : System.IDisposable
	{
        private const uint numberOfMulticasts = 10;

        public MainApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
		{
			try
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

				demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
				rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);

				CMS.Components.Sequencer.register_serializable();
				CMS.Base2.StringWrapper.register_serializable();

				objectContainer = new CMS.Components.IdentifiableObjectContainer(100);

				scatterer = new CMS.Scattering.Scatterer(rootSender, platform.Logger);

				multicastingSender = new QS.CMS.Scattering.RetransmittingScatterer(platform.Logger, 
					demultiplexer, scatterer, rootSender, objectContainer, platform.AlarmClock, 0.1, false);

				demultiplexer.register(1000, new CMS.Base2.ReceiveCallback(this.receiveCallback));
			}
			catch (Exception exc)
			{
				platform.Logger.Log(this, "Exception thrown in the constructor: " + exc.ToString());
				throw new Exception("Cannot create QS.TMS.Tests.Test100.MainApp", exc);
			}
		}

		private QS.Fx.Network.NetworkAddress[] destinationAddresses;
		private uint lastNumSent, totalToSend;

		public void multicast(IPAddress[] destinationIPAddresses)
		{
			try
			{
//				System.Text.StringBuilder foo = new System.Text.StringBuilder();
//				foo.Append("Sending multicast to:\n");
//				foreach (IPAddress ipaddr in destinationIPAddresses)
//					foo.Append(" --> " + ipaddr.ToString() + "\n");
//				platform.Logger.Log(this, foo.ToString()); 

				destinationAddresses = new QS.Fx.Network.NetworkAddress[destinationIPAddresses.Length];
				for (uint ind = 0; ind < destinationIPAddresses.Length; ind++)
					destinationAddresses[ind] = new QS.Fx.Network.NetworkAddress(destinationIPAddresses[ind], 12022);

				this.lastNumSent = 0;
				this.totalToSend = numberOfMulticasts;

				sendOne();
			}
			catch (Exception exc)
			{
				platform.Logger.Log(this, "Exception thrown in \"multicast\": " + exc.ToString()); 
				throw new Exception("Cannot invoke QS.TMS.Tests.Test100.MainApp.multicast", exc);
			}
		}
	
		private void sendOne()
		{
			lastNumSent++;
			CMS.Base2.IBase2Serializable serializableObject = new CMS.Base2.StringWrapper("Kaziun_" + lastNumSent);
			CMS.Scattering.IScatterSet addressSet = new CMS.Scattering.ScatterSet(destinationAddresses);
				
			multicastingSender.multicast(1000, addressSet, CMS.Components.Sequencer.wrap(serializableObject), 
				new CMS.Scattering.CompletionCallback(this.completionCallback));
		}

		private void completionCallback(bool succeeded, System.Exception exception)
		{
			if (succeeded)
			{
                if (lastNumSent < totalToSend)
                {
                    sendOne();
                }
                else
                {
                    platform.Logger.Log("All multicasts delivered successfully.");
                    completed.Set();
                }
            }
			else
				platform.Logger.Log("Error after " + lastNumSent + " multicasts: " + exception.ToString());
		}

        private System.Threading.AutoResetEvent completed = new System.Threading.AutoResetEvent(false);

        public void synchronize()
        {
            if (!completed.WaitOne(TimeSpan.FromSeconds(5), false))
                throw new Exception("Operation failed: not all multicasts were delivered on time");
        }

        private CMS.Platform.IPlatform platform;
		private CMS.Base2.IDemultiplexer demultiplexer;
		private CMS.Base2.RootSender rootSender;
		private CMS.Base2.IIdentifiableObjectContainer objectContainer;
		private CMS.Scattering.IScatterer scatterer;
		private CMS.Scattering.RetransmittingScatterer multicastingSender;

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS.CMS.Base2.IBase2Serializable wrappedObject)
		{
			CMS.Base2.IBase2Serializable serializableObject = 
				((CMS.Components.Sequencer.IWrappedObject) wrappedObject).SerializableObject;

			platform.Logger.Log("Receive_Callback : \"" + serializableObject.ToString() + 
				"\" from " + sourceAddress.ToString());

			return null;
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
*/
}
