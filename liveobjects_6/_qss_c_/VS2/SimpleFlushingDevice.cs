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

#define DEBUG_SimpleFlushingDevice

using System;

namespace QS._qss_c_.VS_2_
{
	/// <summary>
	/// Summary description for SimpleFlushingDevice.
	/// </summary>
/* 
	public class SimpleFlushingDevice : IFlushingDevice, Base.IClient
	{
		public SimpleFlushingDevice(Base.ISender underlyingReliableSender, Base.IDemultiplexer demultiplexer, 
			IViewController viewController, QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;

			this.underlyingReliableSender = underlyingReliableSender;
			demultiplexer.register(this, new Dispatchers.DirectDispatcher(new Base.OnReceive(this.receiveCallback)));
			this.viewController = viewController;

//			this.flushingViews = new Collections.Hashtable(20);
		
		}

		private Base.ISender underlyingReliableSender;
		private QS.Fx.Logging.ILogger logger;
		private IViewController viewController;

//		private Collections.Hashtable flushingViews;


		private void receiveCallback(Base.IAddress source, Base.IMessage message)
		{
			if (!(source is Base.ObjectAddress))
				throw new Exception("unknown source address type");
			QS.Fx.Network.NetworkAddress sourceAddress = (QS.Fx.Network.NetworkAddress) source;

			if (!(message is Base.AnyMessage))
				throw new Exception("received message is not wrapped within QS.CMS.Base.AnyMessage class");

			object msg = ((Base.AnyMessage) message).Contents;

			if (msg is FlushingReport)
			{
				FlushingReport flushingReport = (FlushingReport) msg;

#if DEBUG_SimpleFlushingDevice
				logger.Log(this, "received a flushing report from " + source.ToString());
#endif

//				lock (this)
//				{
//					FlushingView flushingView = lookupView(flushingReport.viewAddress);
//					flushingView.receivedReports[sourceAddress] = flushingReport;
//				}


				ForwardingRequest[] fwdreqs = 
					viewController.flushingReportArrived((QS.Fx.Network.NetworkAddress) source, (FlushingReport) msg);
				
				if (fwdreqs != null)
					forward(fwdreqs);
			}
			else
				throw new Exception("received a message of unknown type, cannot process");
		}

		public void ignoreCrashed(Base.ViewAddress viewAddress, Collections.ISet deadAddresses)
		{
//			lock (this)
//			{
//				Collections.IDictionaryEntry dic_en = flushingViews.lookup(viewAddress);
//				if (dic_en != null)
//				{
//					FlushingView flushingView = (FlushingView) dic_en.Value;
//
//					foreach (QS.Fx.Network.NetworkAddress networkAddress in deadAddresses)
//					{
//						if (flushingView.receivedReports.lookup(networkAddress) != null) // oh this is so nasty
//							flushingView.receivedReports.remove(networkAddress);
//					}
//				}
//			}			
		}

		public void initiateFlushing(QS.Fx.Network.NetworkAddress[] liveAddresses, FlushingReport flushingReport, 
			ForwardingRequest[] forwardingRequests)
		{
#if DEBUG_SimpleFlushingDevice
			logger.Log(this, "initiate flushing : report = " + flushingReport.ToString() + ", with " + 
				forwardingRequests.Length.ToString() + " forwarding requests.");
#endif

//			lock (this)
//			{
//				FlushingView flushingView = lookupView(flushingReport.viewAddress);
//				flushingView.flushingReport = flushingReport;
//			}


//			foreach (FlushingReport.ReceivedMessages rmsg in flushingReport.receivedMessages)
			foreach (QS.Fx.Network.NetworkAddress networkAddress in liveAddresses)
			{
#if DEBUG_SimpleFlushingDevice
				logger.Log(this, "sending a local flushing report to " + networkAddress.ToString()); // rmsg...
#endif

				underlyingReliableSender.send(this, new Base.ObjectAddress(networkAddress, this.LocalObjectID), // rmsg...
					new Base.AnyMessage(flushingReport), null);
			}

			if (forwardingRequests != null)
				forward(forwardingRequests);
		}

		public void forward(ForwardingRequest[] forwardingRequests)
		{
			foreach (ForwardingRequest forwardingReq in forwardingRequests)
			{
				foreach (ForwardingRequest.MessageRef messageRef in forwardingReq.messageRefs)
				{
#if DEBUG_SimpleFlushingDevice
					logger.Log(this, "forwarding a message to " + forwardingReq.forwardingAddress.ToString() + 
						", message : " + messageRef.ToString());
#endif

					messageRef.vsSender.forward(forwardingReq.forwardingAddress, messageRef);
				}
			}
		}

//		private FlushingView lookupView(Base.ViewAddress viewAddress)
//		{
//			FlushingView flushingView = null;
//
//			Collections.IDictionaryEntry dic_en = flushingViews.lookupOrCreate(viewAddress);
//			if (dic_en.Value == null)
//			{
//				flushingView = new FlushingView();
//				dic_en.Value = flushingView;
//			}
//			else
//				flushingView = (FlushingView) dic_en.Value;
//
//			return flushingView;
//		}
//
//		private class FlushingView
//		{
//			public FlushingView()
//			{
//				receivedReports = new Collections.Hashtable(10); // TODO: We need to put something meaningful here!
//			}
//
//			public FlushingReport flushingReport;
//			public Collections.Hashtable receivedReports;
//		}

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.SimpleFlushingDevice;
			}
		}

		#endregion
	}
*/	
}
