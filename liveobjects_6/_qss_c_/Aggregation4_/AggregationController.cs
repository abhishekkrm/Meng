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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Aggregation4_
{
	public abstract class AggregationController : QS.Fx.Inspection.Inspectable, 
		Base1_.IClient, IControllerClass, QS._qss_e_.Base_1_.IStatisticsCollector, IAggregationContext		
	{
		public AggregationController(
			uint controllerCollectionLOID, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> notificationSenders,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> forwardingSenders)
		{
			this.controllerCollectionLOID = controllerCollectionLOID;
			this.alarmClock = alarmClock;
			this.clock = clock;
			this.notificationSenders = notificationSenders;
			this.forwardingSenders = forwardingSenders;
		}

		#region Accessors

		public Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> NotificationSenders
		{
			set { notificationSenders = value; }
			get { return notificationSenders; }
		}

		public Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> ForwardingSenders
		{
			set { forwardingSenders = value; }
			get { return forwardingSenders; }
		}

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get { return alarmClock; }
		}

		public QS.Fx.Clock.IClock Clock
		{
			get { return clock; }
		}

		#endregion

		protected QS.Fx.Clock.IAlarmClock alarmClock;
		protected QS.Fx.Clock.IClock clock;
		protected Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> notificationSenders, forwardingSenders;
		protected uint controllerCollectionLOID;

		#region IClient Members

		uint QS._qss_c_.Base1_.IClient.LocalObjectID
		{
			get { return controllerCollectionLOID; }
		}

		#endregion

		#region IControllerClass Members

		public abstract IAggregationController CreateController();

		#endregion

		#region IStatisticsCollector Members

		public virtual IList<QS._core_c_.Components.Attribute> Statistics
		{
            get { return Helpers_.ListOf<QS._core_c_.Components.Attribute>.Nothing; }
		}

		#endregion
	}
}
