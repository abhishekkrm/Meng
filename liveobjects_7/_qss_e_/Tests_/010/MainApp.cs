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

namespace QS._qss_e_.Tests_.Test010
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			IPAddress localAddress = 
				QS._qss_c_.Devices_2_.Network.AnyAddressOn(new QS._qss_c_.Base1_.Subnet((string) args["base"]));
			QS._qss_d_.Service_2_.IClient serviceClient = new QS._qss_d_.Service_2_.Client(
				localAddress, null, QS._qss_d_.Base_.Win32Config.DefaultCryptographicKeyFile);
			
			using (QS._qss_d_.Service_2_.IServiceRef serviceRef = serviceClient.connectTo(
				new QS.Fx.Network.NetworkAddress(IPAddress.Parse((string) args["proxy"]), 
				(int) QS._qss_d_.Base_.Win32Config.DefaultMainTCPServicePortNo), 
					   platform.Logger, TimeSpan.FromSeconds(30)))
			{
				QS._qss_d_.Service_2_.DeploymentRequest[] requests = new QS._qss_d_.Service_2_.DeploymentRequest[] {
					new QS._qss_d_.Service_2_.DeploymentRequest("C:\\.testing\\QuickSilver.dll", 
						"C:\\.testing\\applications\\QuickSilver.dll"),
					new QS._qss_d_.Service_2_.DeploymentRequest("C:\\.testing\\QuickSilver_Service.exe", 
						"C:\\.testing\\applications\\QuickSilver_Service.exe")};

				string operation_log = serviceRef.deploy(requests, 
					new IPAddress[] 
					{ 
						IPAddress.Parse("128.84.223.71"), 
						IPAddress.Parse("128.84.223.72"), 
						IPAddress.Parse("128.84.223.73"), 
						IPAddress.Parse("128.84.223.74"), 
						IPAddress.Parse("128.84.223.75"), 
						IPAddress.Parse("128.84.223.76"), 
						IPAddress.Parse("128.84.223.77"), 
						IPAddress.Parse("128.84.223.78"), 
						IPAddress.Parse("128.84.223.79"), 
						IPAddress.Parse("128.84.223.80"), 
						IPAddress.Parse("128.84.223.81"), 
						IPAddress.Parse("128.84.223.82"), 
						IPAddress.Parse("128.84.223.83"), 
						IPAddress.Parse("128.84.223.84"), 
						IPAddress.Parse("128.84.223.85"), 
						IPAddress.Parse("128.84.223.200"), 
						IPAddress.Parse("128.84.223.209"), 
						IPAddress.Parse("128.84.223.210"), 
						IPAddress.Parse("128.84.223.211"), 
						// IPAddress.Parse("128.84.223.212"), 
						IPAddress.Parse("128.84.223.213"), 
						IPAddress.Parse("128.84.223.214"), 
						IPAddress.Parse("128.84.223.215"), 
						IPAddress.Parse("128.84.223.216")
					}, 
					TimeSpan.FromSeconds(20)); 
				platform.Logger.Log("Operation Log: " + operation_log);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
