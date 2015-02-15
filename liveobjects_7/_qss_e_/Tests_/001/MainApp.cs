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
using System.Diagnostics;
using System.Threading;

namespace QS._qss_e_.Tests_.Test001
{
	/// <summary>
	/// Summary description for Test001.
	/// </summary>
	public class MainApp : Base_1_.ControlledApplication //, System.IDisposable
	{
        public MainApp(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			this.platform = platform;
		
			IPAddress localAddress = args.contains("base") ? IPAddress.Parse((string) args["base"]) : platform.NICs[0];
			platform.Logger.Log(null, "Base Address : " + localAddress.ToString());

			if (args.contains("listen"))
			{
				QS.Fx.Network.NetworkAddress listeningAtAddress = new QS.Fx.Network.NetworkAddress((string) args["listen"]);
				QS._qss_c_.Devices_2_.IListener listener = platform.UDPDevice.listenAt(
					localAddress, listeningAtAddress, new QS._qss_c_.Devices_2_.OnReceiveCallback(this.receiveCallback));

				platform.Logger.Log(null, "Listening : " + listener.Address.ToString());
			}
			else if (args.contains("sendto"))
			{
				QS.Fx.Network.NetworkAddress sourceAddress = new QS.Fx.Network.NetworkAddress(localAddress, 0);
				QS.Fx.Network.NetworkAddress destinationAddress = new QS.Fx.Network.NetworkAddress((string) args["sendto"]);

				for (uint ind = 0; ind < Convert.ToUInt32((string) args["count"]); ind++)
				{
					byte[] someData = BitConverter.GetBytes(ind);
					platform.UDPDevice.sendto(sourceAddress, destinationAddress, 
						new QS._core_c_.Base2.BlockOfData(someData, 0, (uint) someData.Length));
				}
			}	

			platform.Logger.Log(this, "initial phase completed");
		}

        private QS._qss_c_.Platform_.IPlatform platform;
        // private AutoResetEvent completed = new AutoResetEvent(false);
        private System.Exception exception = null;

        private uint nreceived = 0;
        private uint nawaiting = uint.MaxValue;

		#region IDisposable Members

		public override void Dispose()
		{
		}

		#endregion

        public void synchronize(uint nawaiting)
        {
            bool already_done = false;
            lock (this)
            {
                this.nawaiting = nawaiting;
                already_done = nreceived >= nawaiting;
            }

            if (already_done)
                applicationController.upcall("transmission_completed", QS._core_c_.Components.AttributeSet.None);
            else
            {
                // completed.WaitOne();
            }

            if (exception != null)
                throw new Exception("Operation failed.", exception);
        }

        private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
			QS._core_c_.Base2.IBlockOfData blockOfData)
		{
            try
            {
                if (!(blockOfData.SizeOfData == System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint))))
                    throw new Exception("The received block of data is " + blockOfData.SizeOfData.ToString() +
                        " bytes long while it was expected to be only " +
                        System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint)).ToString() + " bytes long.");

                platform.Logger.Log(null, "Received : " + BitConverter.ToUInt32(blockOfData.Buffer,
                    (int)blockOfData.OffsetWithinBuffer).ToString() + " from " + sourceAddress.ToString() + " at " + 
                    destinationAddress.ToString());

                lock (this)
                {
                    nreceived++;

                    if (nreceived >= nawaiting)
                    {
                        // completed.Set();
                        applicationController.upcall("transmission_completed", QS._core_c_.Components.AttributeSet.None);
                    }
                }
            }
            catch (Exception exc)
            {
                this.exception = exc;

                // completed.Set();
                applicationController.upcall("transmission_completed", QS._core_c_.Components.AttributeSet.None);
            }
        }
	}
}
