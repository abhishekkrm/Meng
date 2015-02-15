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

namespace QS._qss_c_.Virtualization_
{
	/// <summary>
	/// Summary description for VirtualSubnetClient.
	/// </summary>
/* 
	public abstract class VirtualSubnetClient : Devices2.ISendingDevice
	{
		public VirtualSubnetClient(VirtualSubnet virtualSubnet)		
		{
			this.virtualSubnet = virtualSubnet;
			virtualSubnet.registerClient(new Devices2.OnReceiveCallback(this.receiveCallback), ref localAddress);
		}

		private VirtualSubnet virtualSubnet;
		private IPAddress localAddress;

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			Base2.IBlockOfData blockOfData)
		{
			// ..........

		}

		#region ISendingDevice Members

		public uint MTU
		{
			get
			{				
				return virtualSubnet.MTU;
			}
		}

		public void sendto(QS.Fx.Network.NetworkAddress destinationAddress, QS.CMS.Base2.IBlockOfData blockOfData)
		{
			virtualSubnet.send(localAddress, destinationAddress, blockOfData);
		}

		#endregion
	}
*/	
}
