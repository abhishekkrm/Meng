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
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace QS._qss_e_.Tests_.Test005
{
/*
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable, CMS.Base.IClient
	{
		#region Initialization

		private const uint defaultBaseUDPPortNo		= 13666;
		private const uint defaultBaseTCPPortNo		= 14666;

		private static QS.GMS.GroupId testingGroupID = new QS.GMS.GroupId(100);

		public MainApp(QS.CMS.Components.AttributeSet args, QS.CMS.Base.IReadableLogger logger)
		{
			this.logger = logger;
			localCMS = new QS.CMS.Base2.CMSWrapper(logger, new QS.Fx.Network.NetworkAddress((string) args["base"]));

			QS.Fx.Network.NetworkAddress networkAddressOfTheGMSServer = new QS.Fx.Network.NetworkAddress((string) args["gms"]);
			localGMS = new QS.GMS.ClientServer.ClientGMS(
				networkAddressOfTheGMSServer.HostIPAddress, networkAddressOfTheGMSServer.PortNumber, localCMS);
			localFDClient = new GMS.ClientServer.ClientFD(localCMS);

			localCMS.Demultiplexer.register(this, new CMS.Dispatchers.DirectDispatcher(new QS.CMS.Base.OnReceive(this.receiveCallback)));

			logger.Log(null, "requesting to join a group");

			localGMS.joinGroup(testingGroupID, this.LocalObjectID, new QS.GMS.ViewChangeUpcall(this.viewChangeCallback));

			multicasting = args.contains("multicast");
			
			if (multicasting)
			{
				intervalBetweenMulticasts = TimeSpan.FromMilliseconds(Convert.ToDouble((string) args["multicast"]));			
				finished = false;
				checkAgain = new AutoResetEvent(false);
				mythread = new Thread(new ThreadStart(mainloop));
				mythread.Start();
			}
		}

		#endregion

		#region Sender main loop

		private void mainloop()
		{
			uint message_seqno = 1;

			while (!finished)
			{
				QS.CMS.Base.IAddress address = new QS.CMS.Base.GroupAddress(testingGroupID);
				QS.CMS.Base.IMessage message = new QS.CMS.Base.AnyMessage("foo(" + message_seqno.ToString() + ")");

				message_seqno++;

				logger.Log(null, "Sending to : " + address.ToString() + " message \"" + message.ToString() + "\"");

				try
				{
					// vsSender
					(localCMS.Senders[1]).send(this, address, message, new QS.CMS.Base.SendCallback(this.sendCallback));
				}
				catch (Exception exc)
				{
					logger.Log(null, "sending failed : " + exc.ToString());
				}

				checkAgain.WaitOne(intervalBetweenMulticasts, false);
			}
		}

		#endregion

		#region Callbacks

		private void receiveCallback(CMS.Base.IAddress sourceAddress, CMS.Base.IMessage message)
		{
			logger.Log(null, "Received from " + sourceAddress.ToString() + " message \"" + message.ToString() + "\"");
		}

		private void viewChangeCallback(QS.GMS.GroupId gid, QS.GMS.IView view)
		{
			// Debug.Assert(false, "view change callback on the client");

			logger.Log(this, "View Change -> " + gid.ToString() + " . " + view.ToString());
		}

		private void sendCallback(QS.CMS.Base.IMessageReference messageRef, bool success, System.Exception exception)
		{
			logger.Log(this, "SendCallback for " + messageRef.ToString() + ", success : " + success.ToString());
		}

		#endregion

		private QS.Fx.Logging.ILogger logger;
		private QS.CMS.Base.ICMS localCMS;
		private GMS.ClientServer.ClientFD localFDClient;
		private QS.GMS.IGMS localGMS;  

		private bool finished, multicasting;
		private TimeSpan intervalBetweenMulticasts;
		private AutoResetEvent checkAgain;
		private Thread mythread;

		#region IDisposable Members

		public void Dispose()
		{
			if (multicasting)
			{
				finished = true;
				checkAgain.Set();
				mythread.Join(TimeSpan.FromSeconds(3));
				if (mythread.IsAlive)
					mythread.Abort();
			}

			localCMS.shutdown();
		}

		#endregion	

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.xTest_Test005;
			}
		}

		#endregion
	}
*/ 
}
