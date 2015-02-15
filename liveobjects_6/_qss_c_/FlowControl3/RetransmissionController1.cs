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

// #define DEBUG_CollectingStatistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.FlowControl3
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class RetransmissionController1 : QS.Fx.Inspection.Inspectable, IRetransmissionController, QS._core_c_.Diagnostics2.IModule
	{
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

		#region Class Configuration

		public class Configuration : QS.Fx.Inspection.Inspectable
		{
			#region Defaults

			private const double default_initialEstimateOfCompletionTime			    = 0.010;
			private const double default_completionTimeSmoothingFactor			= 0.950;
			private const double default_retransmissionTimeoutMultipler				= 5.000;
            private const double default_minimumRetransmissionTimeout            = 0.010;
            private const double default_maximumRetransmissionTimeout           = 1.000;

			#endregion

			#region Constructors

			public readonly static Configuration DefaultConfiguration = new Configuration();

			public Configuration() : this(default_initialEstimateOfCompletionTime, 
				default_completionTimeSmoothingFactor, default_retransmissionTimeoutMultipler)
			{
			}

            public Configuration(double initialEstimateOfCompletionTime,
                double completionTimeSmoothingFactor, double retransmissionTimeoutMultipler)
                : this(initialEstimateOfCompletionTime, completionTimeSmoothingFactor, retransmissionTimeoutMultipler,
                    default_minimumRetransmissionTimeout, default_maximumRetransmissionTimeout)
            {
            }

			public Configuration(double initialEstimateOfCompletionTime, 
				double completionTimeSmoothingFactor, double retransmissionTimeoutMultipler, 
                double minimumRetransmissionTimeout, double maximumRetransmissionTimeout)
			{
				this.InitialEstimateOfCompletionTime = initialEstimateOfCompletionTime;
				this.CompletionTimeSmoothingFactor = completionTimeSmoothingFactor;
				this.RetransmissionTimeoutMultipler = retransmissionTimeoutMultipler;
                this.MinimumRetransmissionTimeout = minimumRetransmissionTimeout;
                this.MaximumRetransmissionTimeout = maximumRetransmissionTimeout;
			}

			#endregion

			#region Fields

			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double InitialEstimateOfCompletionTime;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double CompletionTimeSmoothingFactor;
			[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
			public readonly double RetransmissionTimeoutMultipler;
            [QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
            public readonly double MinimumRetransmissionTimeout;
            [QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
            public readonly double MaximumRetransmissionTimeout;

			#endregion
		}

		#endregion

		public RetransmissionController1() : this(null)
		{
		}

		public RetransmissionController1(IRetransmittingSender controlledObject) : this(controlledObject, Configuration.DefaultConfiguration)
		{
		}

		public RetransmissionController1(IRetransmittingSender controlledObject, Configuration configuration)
		{
			this.controlledObject = controlledObject;
			this.configuration = configuration;
			this.smoothedOutCompletionTime = configuration.InitialEstimateOfCompletionTime;
            this.minimumTimeout = configuration.MinimumRetransmissionTimeout;
            this.maximumTimeout = configuration.MaximumRetransmissionTimeout;

            QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
		}
		
		private IRetransmittingSender controlledObject;
		private Configuration configuration;
        private double minimumTimeout, maximumTimeout;

		[QS.Fx.Base.Inspectable("Smoothed Out Completion Time", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private double smoothedOutCompletionTime;

#if DEBUG_CollectingStatistics
        [QS.CMS.Diagnostics.Component("Completion Times")]
        [QS.CMS.QS._core_c_.Diagnostics2.Property("CompletionTimes")]
		private Statistics.Samples completionTimes = new QS.CMS.Statistics.Samples();
		// private Statistics.Samples retransmissionTimeouts = new QS.CMS.Statistics.Samples();
        [QS.CMS.Diagnostics.Component("Retransmission Timeouts")]
        [QS.CMS.QS._core_c_.Diagnostics2.Property("RetransmissionTimeouts")]
		private Statistics.Samples retransmissionTimeouts = new QS.CMS.Statistics.Samples();
#endif

		#region IRetransmissionController Members

		void IRetransmissionController.completed(double completionTime, int nretransmissions)
		{
			lock (this)
			{
#if DEBUG_CollectingStatistics
				completionTimes.addSample(completionTime);
#endif

//				if (nretransmissions == 0)
//				{
					smoothedOutCompletionTime += (1 - configuration.CompletionTimeSmoothingFactor) * (completionTime - smoothedOutCompletionTime);
					
                    double retransmissionTimeout = smoothedOutCompletionTime * configuration.RetransmissionTimeoutMultipler;
                    if (retransmissionTimeout < minimumTimeout)
                        retransmissionTimeout = minimumTimeout;
                    else if (retransmissionTimeout > maximumTimeout)
                        retransmissionTimeout = maximumTimeout;

					controlledObject.RetransmissionTimeout = retransmissionTimeout;

#if DEBUG_CollectingStatistics
					retransmissionTimeouts.addSample(retransmissionTimeout);
#endif
//				}
			}
		}

		IRetransmittingSender IRetransmissionController.ControlledObject
		{
			set { controlledObject = value; }
		}

        double IRetransmissionController.MinimumTimeout
        {
            get { return minimumTimeout; }
            set 
            { 
                minimumTimeout = value;
            }
        }

        double IRetransmissionController.MaximumTimeout
        {
            get { return maximumTimeout; }
            set 
            { 
                maximumTimeout = value;
            }
        }

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get
			{
				System.Collections.Generic.IList<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();

#if DEBUG_CollectingStatistics
				statistics.Add(new QS.CMS.Components.Attribute("CompletionTimes", completionTimes.DataSet));
				statistics.Add(new QS.CMS.Components.Attribute("Retransmission Timeouts", retransmissionTimeouts.DataSet));
#endif
				return statistics;
			}
		}

		#endregion
	}
}
