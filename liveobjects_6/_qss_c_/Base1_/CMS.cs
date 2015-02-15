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

namespace QS._qss_c_.Base1_
{
	/// <summary>
	/// Aaa...
	/// </summary>
	/// 

/*
	public class CMS : ICMS, Components.IRequestSerializer
	{
		private const uint defaultBaseUDPPort											= 11000;
		private const uint defaultBaseTCPPort												= 12000;
		private const uint defaultMaximumNumberOfObjectIDs					= 100;
		private const uint defaultAnticipatedNumberOfVSGroups				= 20;
		private const uint defaultAnticipatedNumberOfIPGroups					= 100;

		public CMS()
		{
			initialize(null, defaultBaseUDPPort, defaultBaseTCPPort, defaultMaximumNumberOfObjectIDs, defaultAnticipatedNumberOfVSGroups, defaultAnticipatedNumberOfIPGroups); 
		}

		public CMS(uint baseUDPPort, uint maximumNumberOfObjectIDs)
		{
			initialize(null, baseUDPPort, defaultBaseTCPPort, maximumNumberOfObjectIDs, defaultAnticipatedNumberOfVSGroups, defaultAnticipatedNumberOfIPGroups);
		}

		public CMS(IConsole console, uint baseUDPPort, uint maximumNumberOfObjectIDs)
		{
			initialize(new Logger(null, false, console), baseUDPPort, defaultBaseTCPPort, maximumNumberOfObjectIDs, defaultAnticipatedNumberOfVSGroups, defaultAnticipatedNumberOfIPGroups);
		}

		public CMS(uint baseUDPPort, uint baseTCPPort, uint maximumNumberOfObjectIDs, uint anticipatedNumberOfVSGroups, uint anticipatedNumberOfIPGroups)
		{
			initialize(null, baseUDPPort, baseTCPPort, maximumNumberOfObjectIDs, anticipatedNumberOfVSGroups, anticipatedNumberOfIPGroups);
		}

		public CMS(Base.IReadableLogger logger, uint baseUDPPort, uint baseTCPPort, uint maximumNumberOfObjectIDs, uint anticipatedNumberOfVSGroups, uint anticipatedNumberOfIPGroups)
		{
			initialize(logger, baseUDPPort, baseTCPPort, maximumNumberOfObjectIDs, anticipatedNumberOfVSGroups, anticipatedNumberOfIPGroups);
		}

		private void initialize(Base.IReadableLogger logger, uint baseUDPPort, uint baseTCPPort, uint maximumNumberOfObjectIDs, uint anticipatedNumberOfVSGroups, uint anticipatedNumberOfIPGroups)
		{
//			logger.Log(this, "Class QS.CMS.Base.CMS is obsolete, please use QS.CMS.Base2.CMSWrapper instead.");

			this.logger = (logger != null) ? logger : (new Logger(null, true));

			Serializer.Get.register(ClassID.AnyMessage, AnyMessage.Factory);

//			internalWorker = new Components.Worker(new Components.WorkerCallback(this.workerCallback));

			demultiplexer = new SimpleDemultiplexer(maximumNumberOfObjectIDs);
			alarmClock = new PQAlarmClock(new Collections.BHeap(100, 2), logger);

			udpDevice = new Devices.UDPCommunicationsDevice(logger, true, (int) baseUDPPort, anticipatedNumberOfIPGroups);
			faultyDevice = new Devices.FaultyDevice(udpDevice, logger, alarmClock, 0.05, 0.01, 50);

			baseSender = new Senders.BaseSender(faultyDevice, udpDevice, new Devices.IReceivingDevice[] { udpDevice }, demultiplexer, logger);
			reliableSender = new Senders.ReliableSender(baseSender, demultiplexer, alarmClock, 10, 10, TimeSpan.FromMilliseconds(100), logger, false);
			
			tcpDevice = new Devices.TCPCommunicationsDevice(
                "CMS_TCP", Devices2.Network.LocalAddresses[0], logger, true, (int) baseTCPPort, 10);
			ultrareliableSender = new Senders.ReliableSender(new Senders.BaseSender(udpDevice, udpDevice, new Devices.IReceivingDevice[] {}, 
				demultiplexer, logger), demultiplexer, alarmClock, 10, 10, TimeSpan.FromMilliseconds(5), logger, false, 
				(uint) ReservedObjectID.UltrareliableSender);

			localAddress = new QS.Fx.Network.NetworkAddress(udpDevice.IPAddress, udpDevice.PortNumber);

			this.logger.Log(null, "Local_Address " + localAddress.ToString());

			multicastingDevice = new Multicasting.DirectMulticastingDevice(baseSender, alarmClock, logger);
			flushingDevice = new Flushing.N2FlushingDevice(logger, reliableSender, demultiplexer);
			vsSender = new VS3.VSSender(logger, localAddress, multicastingDevice, demultiplexer, reliableSender, reliableSender, flushingDevice);			 

//			internalWorker.startup();
		}

		public void join(IPAddress groupAddress, int listeningPortNo)
		{
			udpDevice.join(groupAddress, listeningPortNo);
		}

		public void leave(IPAddress groupAddress)
		{
			udpDevice.leave(groupAddress);
		}

		public void shutdown()
		{
			alarmClock.shutdown();
//			viewController.shutdown();
			udpDevice.shutdown();
			tcpDevice.shutdown();
//			internalWorker.cleanup();
		}

		public VS3.VSSender VSSender
		{
			get
			{
				return vsSender;
			}
		}

		public QS.Fx.Network.NetworkAddress Address
		{
			get
			{
				return localAddress;
			}
		}

		public IReadableLogger Logger
		{
			get
			{
				return this.logger;
			}
		}

		public IConsole Console
		{
			set
			{
				logger.Console = value;
			}
		}

//		private void workerCallback(Collections.ILinkable request)
//		{
//			((Components.IAsynchronousRequest) request).process();
//		}

		private IReadableLogger logger; //  = new Logger(true);
		private IDemultiplexer demultiplexer;
		private Devices.UDPCommunicationsDevice udpDevice;
		private Devices.FaultyDevice faultyDevice;
		private Senders.BaseSender baseSender;
		private Senders.ReliableSender reliableSender;
		private Base.PQAlarmClock alarmClock;
		private QS.Fx.Network.NetworkAddress localAddress;
		private Multicasting.IMulticastingDevice multicastingDevice;		
		private Flushing.IFlushingDevice flushingDevice;
		private GMS.IGMS theGMS = null;
		private VS3.VSSender vsSender;
		private Devices.TCPCommunicationsDevice tcpDevice;
		private ISender ultrareliableSender;

//		private Senders.SimpleSender2 sender2;

		#region ICMS Members

		public void linkToGMS(GMS.IGMS theGMS)
		{
			this.theGMS = theGMS;

			GMS.ViewChangeGoAhead viewChangeGoAhead = theGMS.linkCMSToGMS(new GMS.ViewChangeRequest(vsSender.viewChangeRequest),
				new GMS.ViewChangeAllDone(vsSender.viewChangeAllDone), new GMS.ViewChangeCleanup(vsSender.viewChangeCleanup));

			Debug.Assert((viewChangeGoAhead != null), "viewChangeGoAhead is null");
			vsSender.registerGMSCallbacks(viewChangeGoAhead);
		}

		public ISender[] Senders
		{
			get
			{
				return new ISender[] { ultrareliableSender }; // baseSender, fbcastSender, ..
			}
		}

		public IDemultiplexer Demultiplexer
		{
			get
			{
				return demultiplexer;
			}
		}

		#endregion

//		private Components.Worker internalWorker;

		#region IRequestSerializer Members

		public void enqueue(Components.IAsynchronousRequest request)
		{
//			internalWorker.enqueue(request);
		}

		#endregion
	}
*/
}
