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

namespace QS._qss_e_.Tests_.Test104
{
/*
	/// <summary>
	/// This is a simple test used with experiment 005.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		private const uint port_number = 12022;

		public MainApp(CMS.Platform.IPlatform platform, QS.CMS.Components.AttributeSet args)
		{
			this.platform = platform;

			QS.CMS.Base2.Serializer.CommonSerializer.registerClasses(platform.Logger);

			IPAddress localIPAddress;
			if (args.contains("base"))
				localIPAddress = IPAddress.Parse((string) args["base"]);
			else if (args.contains("subnet"))
				localIPAddress = CMS.Devices2.Network.AnyAddressOn(
					new CMS.Base.Subnet((string) args["subnet"]), platform);
			else 
				localIPAddress = platform.NICs[0];
			QS.Fx.Network.NetworkAddress localAddress = new QS.Fx.Network.NetworkAddress(localIPAddress, (int) port_number);			

			platform.Logger.Log(null, "Base Address Chosen : " + localAddress.ToString());

			IPAddress gmsIPAddress = IPAddress.Parse((string) args["gms"]);
			IPAddress allocationServerIPAddress = IPAddress.Parse((string) args["allocsrv"]);
			
			demultiplexer = new CMS.Base2.Demultiplexer(platform.Logger);
			rootSender = new QS.CMS.Base2.RootSender(localAddress, platform.UDPDevice, demultiplexer, platform.Logger);
			scatterer = new CMS.Scattering.Scatterer(rootSender, platform.Logger);

			masterContainer = new CMS.Base2.MasterIOC();

			retransmittingScatterer = new QS.CMS.Scattering.RetransmittingScatterer(platform.Logger, 
			    demultiplexer, scatterer, rootSender, masterContainer, platform.AlarmClock, 
			    System.Convert.ToDouble(args["timeout"]), true);

            simpleRS = new QS.CMS.Scattering.SimpleRS(rootSender, scatterer, platform.AlarmClock,
                demultiplexer, platform.Logger, 1000, System.Convert.ToDouble(args["timeout"]));

            CMS.Base2.Serializer.CommonSerializer.registerClass(ClassID.BlockOfData, typeof(CMS.Base2.BlockOfData));
			CMS.Base2.StringWrapper.register_serializable();

			demultiplexer.register(1000, new CMS.Base2.ReceiveCallback(this.receiveCallback));

			if (localIPAddress.Equals(gmsIPAddress))
			{
				platform.Logger.writeLine("Starting up a fake membership service on a local node.");

				membershipServer = new QS.CMS.Membership.Server(platform.Logger, retransmittingScatterer);
			}

			if (localIPAddress.Equals(allocationServerIPAddress))
			{
				platform.Logger.writeLine("Starting up a multicast address allocation server on a local node.");

				allocationServer = new CMS.IPMulticast.AllocationServer(demultiplexer, platform.Logger);
			}

			CMS.Components.Sequencer.register_serializable();

			CMS.Base.Serializer.Get.register(QS.ClassID.Mahesh_CSGMSImmutableView, 
				new CMS.Base.CreateSerializable(GMS.ClientServer.ImmutableView.factory));
			// GMS.ClientServer.DirtyFactoryLoader.loadFactories();

			membershipClient = new QS.CMS.Membership.Client(platform.Logger, demultiplexer);

			rpcProxy = new CMS.RPC2.RPCProxy(demultiplexer, rootSender, platform.Logger, masterContainer, 
				platform.AlarmClock);

			allocationClient = new CMS.Allocation.AllocationClient(new CMS.Base.ObjectAddress(
				allocationServerIPAddress, (int) port_number, (uint) QS.ReservedObjectID.IPMulticast_AllocationServer), 
				rpcProxy, demultiplexer, platform.Logger);

			viewController = new QS.CMS.VS5.SimpleVC2(
                platform.Logger, masterContainer, simpleRS, demultiplexer, allocationClient, localAddress, platform.UDPDevice, rootSender);

			viewController.linkToGMS(membershipClient);
		}

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, CMS.Base2.IBase2Serializable serializableObject)
		{
			platform.Logger.Log(this, "ReceiveCallback : " + serializableObject.ToString());
			return null;
		}

		private GMS.GroupId groupID;
		private GMS.ClientServer.ImmutableView membershipView;

		public void initializeExperiment(GMS.GroupId groupID, byte[] membershipViewAsBytes,
			uint numberOfMulticasts, uint numberOfConcurrentMulticasts)
		{
			this.numberOfMulticasts = numberOfMulticasts;
			this.numberOfConcurrentMulticasts = numberOfConcurrentMulticasts;

			this.resultsReady = new System.Threading.AutoResetEvent(false);

			this.groupID = groupID;
			membershipView = (GMS.ClientServer.ImmutableView)
				CMS.Base2.CompatibilitySOWrapper.ByteArray2Object(membershipViewAsBytes);

			membershipServer.distributeVCNotification(groupID, membershipView);

			platform.AlarmClock.scheduleAnAlarm(
				3.0, new CMS.Base.AlarmCallback(this.alarmCallback), null);
		}

		private void change_membership()
		{
			GMS.ClientServer.ImmutableSubView[] subviews = 
				new QS.GMS.ClientServer.ImmutableSubView[membershipView.NumberOfSubViews / 2];
			for (uint ind = 0 ;ind < subviews.Length; ind++)
				subviews[ind] = (QS.GMS.ClientServer.ImmutableSubView) membershipView[ind];
			GMS.ClientServer.ImmutableView anotherView = 
				new QS.GMS.ClientServer.ImmutableView(membershipView.SeqNo + 1, subviews);

			membershipServer.distributeVCNotification(groupID, anotherView);
		}

		private void alarmCallback(CMS.Base.IAlarmRef alarmRef)
		{
			completionCallback = new QS.CMS.Scattering.Callback(this.completion_upcall);
			sometingToSend = new CMS.Base2.StringWrapper("A nice little message.");
			
			startingTimes = new double[numberOfMulticasts];
			finishingTimes = new double[numberOfMulticasts];

			numStarted = numComplete = 0;

			experimentBeginning = QS.CMS.Base2.PreciseClock.Clock.Time;
				
			lock (this)
			{
				for (uint ind = 0; ind < numberOfConcurrentMulticasts; ind++)
					multicast_ingroup();
			}
		}

		private CMS.Base2.IBase2Serializable sometingToSend;
		private double experimentBeginning;
		private double[] startingTimes, finishingTimes;
		private uint numberOfMulticasts, numberOfConcurrentMulticasts, numStarted, numComplete;
		private void multicast_ingroup()
		{
			startingTimes[numStarted++] = QS.CMS.Base2.PreciseClock.Clock.Time;
			viewController.send(groupID, sometingToSend, completionCallback);
		}

		private CMS.Scattering.Callback completionCallback;
		private void completion_upcall(bool succeeded, System.Exception exception)
		{
			lock (this)
			{
				if (numComplete < numberOfMulticasts)
				{
					finishingTimes[numComplete++] = QS.CMS.Base2.PreciseClock.Clock.Time;

					if (numStarted < numberOfMulticasts)
					{
						multicast_ingroup();
					}
					else if (numComplete == numberOfMulticasts)
					{
						results = new Results(startingTimes, finishingTimes);
						resultsReady.Set();
					}
				}
			}

            // platform.Logger.writeLine("Completion_Upcall: " + numComplete);
        }

		private System.Threading.AutoResetEvent resultsReady;
		private Results results = null;
		[Serializable]
		public class Results
		{
			public Results()
			{
			}

			public Results(double[] startingTimes, double[] finishingTimes)
			{
				this.startingTimes = startingTimes;
				this.finishingTimes = finishingTimes;
			}

			public double[] startingTimes, finishingTimes;

			public override string ToString()
			{
				System.Text.StringBuilder output = new System.Text.StringBuilder();
				output.Append("Starting and Ack Times:\n\n");
				for (uint ind = 0; ind < startingTimes.Length; ind++)
					output.Append(startingTimes[ind].ToString() + "\t" + finishingTimes[ind].ToString() + "\n");
				return output.ToString();
			}
		}

		public Results collectResults()
		{
			platform.Logger.writeLine("CollectResults_Enter");
			resultsReady.WaitOne();
			platform.Logger.writeLine("CollectResults_Leave");
			return results;
		}

		private CMS.Platform.IPlatform platform;
		private CMS.Base2.IDemultiplexer demultiplexer;
		private CMS.Base2.RootSender rootSender;
		private CMS.Base2.IMasterIOC masterContainer;
		private CMS.Scattering.IScatterer scatterer;
		private CMS.Scattering.IRetransmittingScatterer retransmittingScatterer, simpleRS;
		private CMS.VS5.SimpleVC2 viewController;
		private CMS.RPC2.RPCProxy rpcProxy;
		private CMS.Allocation.AllocationClient allocationClient;
		private CMS.IPMulticast.AllocationServer allocationServer;

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
