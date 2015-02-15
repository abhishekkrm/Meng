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

// ﻿#define DEBUG_CollectTimings

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class WindowController1 : QS.Fx.Inspection.Inspectable, IWindowController
	{
		#region Class Configuration

		public class Configuration : QS.Fx.Inspection.Inspectable
		{
			#region Defaults

			private const double default_retransmissionRateSmoothingFactor		= 0.950;
//			private const double default_minimumDelayOfWindowSizeChange		= 0.001;
//			private const double default_maximumDelayOfWindowSizeChange	= 0.010;
			private const double default_retransmissionAllowanceThreshold		= 0.100;
			private const double default_retransmissionAlert								= 0.500;
			private const double default_negligibleRetransmissionThreshold		= 0.020;
			private const double default_maximumMultiplicativeScalingFactor		= 1.100;
			private const double default_failureWindowShrinkingFactor				= 0.900;
//-			private const uint default_minimumRequestsBeforeChange				= 10;
//			private const double default_roundtripsRequiredBeforeChange			= 5;
			private const double default_minimumNumberOfSamplesToChange	= 20;

			#endregion

			#region Constructors

			public readonly static Configuration DefaultConfiguration = new Configuration();

			public Configuration() : this(default_retransmissionRateSmoothingFactor,
//				default_minimumDelayOfWindowSizeChange, default_maximumDelayOfWindowSizeChange,
				default_retransmissionAllowanceThreshold, default_retransmissionAlert, 
				default_negligibleRetransmissionThreshold, default_maximumMultiplicativeScalingFactor,
				default_failureWindowShrinkingFactor, 
//-				default_minimumRequestsBeforeChange
//				default_roundtripsRequiredBeforeChange
				default_minimumNumberOfSamplesToChange)
			{
			}

			public Configuration(double retransmissionRateSmoothingFactor,
//				double minimumDelayOfWindowSizeChange, double maximumDelayOfWindowSizeChange,
				double retransmissionAllowanceThreshold, double retransmissionAlert, 
				double negligibleRetransmissionThreshold, double maximumMultiplicativeScalingFactor,
				double failureWindowShrinkingFactor, 
//-				uint minimumRequestsBeforeChange
//				double roundtripsRequiredBeforeChange
				double minimumNumberOfSamplesToChange)
			{
				this.RetransmissionRateSmoothingFactor = retransmissionRateSmoothingFactor;
//				this.MinimumDelayOfWindowSizeChange = minimumDelayOfWindowSizeChange;
//				this.MaximumDelayOfWindowSizeChange = maximumDelayOfWindowSizeChange;
				this.RetransmissionAllowanceThreshold = retransmissionAllowanceThreshold;
				this.RetransmissionAlert = retransmissionAlert;
				this.NegligibleRetransmissionThreshold = negligibleRetransmissionThreshold;
				this.MaximumMultiplicativeScalingFactor = maximumMultiplicativeScalingFactor;
				this.FailureWindowShrinkingFactor = failureWindowShrinkingFactor;
//-				this.MinimumRequestsBeforeChange = minimumRequestsBeforeChange;
//				this.RoundtripsRequiredBeforeChange = roundtripsRequiredBeforeChange;
				this.MinimumNumberOfSamplesToChange = minimumNumberOfSamplesToChange;
			}

			#endregion

			#region Fields

			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double RetransmissionRateSmoothingFactor;
//			[TMS.Inspection.Inspectable(TMS.Inspection.AttributeAccess.ReadOnly)]
//			public readonly double MinimumDelayOfWindowSizeChange;
//			[TMS.Inspection.Inspectable(TMS.Inspection.AttributeAccess.ReadOnly)]
//			public readonly double MaximumDelayOfWindowSizeChange;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double RetransmissionAllowanceThreshold;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double RetransmissionAlert;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double NegligibleRetransmissionThreshold;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double MaximumMultiplicativeScalingFactor;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double FailureWindowShrinkingFactor;
//-			[TMS.Inspection.Inspectable(TMS.Inspection.AttributeAccess.ReadOnly)]
//-			public readonly uint MinimumRequestsBeforeChange;
//			[TMS.Inspection.Inspectable(TMS.Inspection.AttributeAccess.ReadOnly)]
//			public readonly double RoundtripsRequiredBeforeChange;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double MinimumNumberOfSamplesToChange;

			#endregion
		}

		#endregion

		public WindowController1() : this(null, null)
		{
		}

		public WindowController1(QS.Fx.Clock.IClock clock, IWindowSender controlledSender) 
			: this(clock, controlledSender, Configuration.DefaultConfiguration)
		{
		}

		public WindowController1(QS.Fx.Clock.IClock clock, IWindowSender controlledSender, Configuration configuration)
		{
			this.clock = clock;
			this.controlledSender = controlledSender;
			this.configuration = configuration;

			this.multiplicativeScalingFactor = configuration.MaximumMultiplicativeScalingFactor;
		}

		private IWindowSender controlledSender;
		private QS.Fx.Clock.IClock clock;
		private Configuration configuration;

		[QS.Fx.Base.Inspectable("Smoothed Out Retransmission Factor", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double retransmissionFactor = 0;

//-		private uint minimumChangeSeqNo = 0;
		private double timeChanged = 0;
		private int samplesCollectedSinceChange = 0;
		private double multiplicativeScalingFactor;

//-		private uint maximumSentSeqNo = 0;

//		private double estimatedCompletionTime;

//		public double RTT
//		{
//			set { estimatedCompletionTime = value; }
//		}

#if DEBUG_CollectTimings
        [QS.CMS.Diagnostics.Component("Retransmission Factors")]
		private Statistics.Samples retransmissionFactors = new QS.CMS.Statistics.Samples();
        [QS.CMS.Diagnostics.Component("Window Size Samples")]
		private System.Collections.Generic.List<TMS.Data.XY> windowSizeSamples = new List<QS.TMS.Data.XY>();
#endif

		#region IWindowController Members

//-		void IWindowController.sending(uint requestSeqNo)
//-		{
//-			if (requestSeqNo > maximumSentSeqNo)
//-				maximumSentSeqNo = requestSeqNo;
//-		}

//-		void IWindowController.completed(uint requestSeqNo, int nretransmissions)
		void IWindowController.completed(Base3_.ITimedOperation request, int nretransmissions)
		{
			lock (this)
			{
				retransmissionFactor += (1 - configuration.RetransmissionRateSmoothingFactor) * (nretransmissions - retransmissionFactor);

#if DEBUG_CollectTimings
				retransmissionFactors.addSample(retransmissionFactor);
#endif

				double currentTime = clock.Time;

				if (request.StartingTime > timeChanged)
					samplesCollectedSinceChange++;

//				double minimumcentium_intervalciumcentium = 3 * smoothedOutCompletionTime;
//				double minimumcentium_intervalciumcentium = 
//					configuration.RoundtripsRequiredBeforeChange * estimatedCompletionTime;

//				if (minimumcentium_intervalciumcentium < minimumDelayOfWindowSizeChange)
//					minimumcentium_intervalciumcentium = minimumDelayOfWindowSizeChange;
//				else if (minimumcentium_intervalciumcentium > maximumDelayOfWindowSizeChange)
//					minimumcentium_intervalciumcentium = maximumDelayOfWindowSizeChange;			

				int newSize = 0;

//-				if (requestSeqNo > minimumChangeSeqNo) 
//				if (currentTime - timeChanged > minimumcentium_intervalciumcentium)
				if (samplesCollectedSinceChange > configuration.MinimumNumberOfSamplesToChange)
				{
					int currentWindowSize = controlledSender.WindowSize; // outgoingController.Concurrency;

					if (retransmissionFactor > configuration.RetransmissionAllowanceThreshold)
					{
						if (retransmissionFactor > configuration.RetransmissionAlert)
							newSize = (int)Math.Floor(currentWindowSize * configuration.FailureWindowShrinkingFactor);
						else
							newSize = currentWindowSize - 1;

						multiplicativeScalingFactor = 1 + 1 / ((double)newSize);
						if (multiplicativeScalingFactor > configuration.MaximumMultiplicativeScalingFactor)
							multiplicativeScalingFactor = configuration.MaximumMultiplicativeScalingFactor;
					}
					else if (retransmissionFactor < configuration.NegligibleRetransmissionThreshold)
					{
						newSize = (int) Math.Ceiling(currentWindowSize * multiplicativeScalingFactor);
					}
				}

				if (newSize > 0)
				{
					controlledSender.WindowSize = newSize;
					timeChanged = currentTime;
//-					minimumChangeSeqNo = maximumSentSeqNo + configuration.MinimumRequestsBeforeChange;
					samplesCollectedSinceChange = 0;

#if DEBUG_CollectTimings
					windowSizeSamples.Add(new TMS.Data.XY(currentTime, newSize));
#endif
				}
			}
		}

		IWindowSender IWindowController.ControlledObject
		{
			set { controlledSender = value; }
		}

		QS.Fx.Clock.IClock IWindowController.Clock
		{
			set { clock = value; }
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
				System.Collections.Generic.IList<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
#if DEBUG_CollectTimings
				statistics.Add(new QS.CMS.Components.Attribute("RetransmissionFactors", retransmissionFactors.DataSet));
				statistics.Add(new QS.CMS.Components.Attribute("WindowSizes", new QS.TMS.Data.XYSeries(windowSizeSamples.ToArray())));
#endif
				return statistics;
			}
		}

		#endregion
	}
}
