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

// ﻿#define DEBUG_LogReceiveRates

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class RegionSender1 : RegionSender<RegionSender1.Sender>, QS._qss_e_.Base_1_.IStatisticsCollector
	{
		public RegionSender1(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, 
			Membership2.Controllers.IMembershipController membershipController,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, int initialWindowSize)
			: base(logger, membershipController)
		{
			this.underlyingSenderCollection = underlyingSenderCollection;
			this.clock = clock;
			this.initialWindowSize = initialWindowSize;
		}

		private QS.Fx.Clock.IClock clock;
		private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection;
		private int initialWindowSize;

		#region Accessors

		public int InitialWindowSize
		{
			set { initialWindowSize = value; }
		}

		#endregion

		#region Class Sender

		public class Sender 
			: QS.Fx.Inspection.Inspectable, IRegionSink, FlowControl3.IWindowSender, QS._qss_e_.Base_1_.IStatisticsCollector
		{
			public Sender(RegionSender1 owner, Base3_.RegionID regionID)
			{
				this.owner = owner;
				this.regionID = regionID;

				// for now, just statically assign this
				regionController = owner.membershipController.lookupRegion(regionID);
				underlyingSender = owner.underlyingSenderCollection[regionController.Address];

				windowSize = owner.initialWindowSize;
				messagesInTransit = 0;

				inspectableQueueProxy = new QS._qss_e_.Inspection_.CollectionWrapper<IRegionRequest>(
					// (System.Collections.Generic.ICollection<QS.CMS.Multicasting3.IRegionRequest>) 
					outgoingQueue);

				windowController = new FlowControl3.WindowController1(owner.clock, this);
			}

			private FlowControl3.WindowController1 windowController;

			private RegionSender1 owner;
			private Base3_.RegionID regionID;
			[QS.Fx.Base.Inspectable("_regionController", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private Membership2.Controllers.IRegionController regionController;
			private QS._qss_c_.Base3_.ISerializableSender underlyingSender;

			[QS.Fx.Base.Inspectable("Window Size", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private int windowSize;
			[QS.Fx.Base.Inspectable("Number of Messages Currently in Transit", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private int messagesInTransit;
			private System.Collections.Generic.Queue<IRegionRequest> outgoingQueue = new Queue<IRegionRequest>();

			[QS.Fx.Base.Inspectable("Outgoing Queue", QS.Fx.Base.AttributeAccess.ReadOnly)]
			private QS._qss_e_.Inspection_.CollectionWrapper<IRegionRequest> inspectableQueueProxy;

#if DEBUG_LogReceiveRates
            [QS.CMS.Diagnostics.Component("Logged Receive Rates")]
			private QS.CMS.Statistics.Samples loggedReceiveRates = new QS.CMS.Statistics.Samples();
#endif

            [QS._core_c_.Diagnostics.Component("Receive Rate Estimate")]
			private FlowControl3.SmoothingEstimator receiveRate = new FlowControl3.SmoothingEstimator(0.9, 1000);

			#region Internal Processing

			private void process_internally(IRegionRequest request, bool submit, bool release)
			{
				System.Collections.Generic.Queue<IRegionRequest> toSend = new Queue<IRegionRequest>();
				lock (this)
				{
					if (submit)
					{
						if (release)
						{
							toSend.Enqueue(request);
						}
						else if (messagesInTransit < windowSize)
						{
							messagesInTransit++;
							toSend.Enqueue(request);
						}
						else
						{
							outgoingQueue.Enqueue(request);
						}
					}
					else
					{
						messagesInTransit--;
						while (messagesInTransit < windowSize && outgoingQueue.Count > 0)
						{
							toSend.Enqueue(outgoingQueue.Dequeue());
							messagesInTransit++;
						}
					}
				}

				foreach (IRegionRequest requestToSend in toSend)
				{
					underlyingSender.send(requestToSend.DestinationLOID, requestToSend.Data);
					requestToSend.Completed();
				}
			}

			#endregion

			#region IRegionSink Members

			void IRegionSink.Submit(IRegionRequest request)
			{
				process_internally(request, true, false);
			}

			void IRegionSink.Release(IRegionRequest request)
			{
				// windowController.RTT = request.Duration;
				((FlowControl3.IWindowController) windowController).completed(request, request.NumberOfRetransmissions);
				process_internally(null, false, true);
			}

			void IRegionSink.Resubmit(IRegionRequest request)
			{
				process_internally(request, true, true);
			}

			double IRegionSink.ReceivingRate
			{
				set
				{
					((FlowControl3.IEstimator)receiveRate).AddSample(value);

#if DEBUG_LogReceiveRates
					loggedReceiveRates.addSample(value);
#endif
				}
			}

			#endregion

			#region IWindowSender Members

			int QS._qss_c_.FlowControl3.IWindowSender.WindowSize
			{
				get { return windowSize; }
				set
				{
					// in general, should lock and do something special
					windowSize = value;
				}
			}

			#endregion

			#region IStatisticsCollector Members

			IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
			{
				get { return ((QS._qss_e_.Base_1_.IStatisticsCollector)receiveRate).Statistics; }
			}

			#endregion
		}

		#endregion

		protected override Sender createSender(QS._qss_c_.Base3_.RegionID regionID)
		{
			return new Sender(this, regionID);
		}

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> s = new List<QS._core_c_.Components.Attribute>();
				foreach (System.Collections.Generic.KeyValuePair<Base3_.RegionID, Sender> element in senders)
				{
					s.Add(new QS._core_c_.Components.Attribute(element.Key.ToString(),
						new QS._core_c_.Components.AttributeSet(((QS._qss_e_.Base_1_.IStatisticsCollector)element.Value).Statistics)));
				}
				return s;
			}
		}

		#endregion
	}
}
