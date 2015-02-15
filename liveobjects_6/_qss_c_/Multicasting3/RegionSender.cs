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

namespace QS._qss_c_.Multicasting3
{
	public abstract class RegionSender<C>
		: QS.Fx.Inspection.Inspectable, IRegionSender where C : IRegionSink
	{
		public RegionSender(QS.Fx.Logging.ILogger logger, Membership2.Controllers.IMembershipController membershipController)
		{
			this.logger = logger;
			this.membershipController = membershipController;

			inspectable_sendersProxy = new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RegionID, C>("Senders", senders,
				new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RegionID, C>.ConversionCallback(Base3_.RegionID.FromString));
		}

		protected QS.Fx.Logging.ILogger logger;
		protected Membership2.Controllers.IMembershipController membershipController;

		[QS.Fx.Base.Inspectable("Senders", QS.Fx.Base.AttributeAccess.ReadOnly)]
		protected QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RegionID, C> inspectable_sendersProxy;

		protected System.Collections.Generic.IDictionary<Base3_.RegionID, C> senders =
			new System.Collections.Generic.Dictionary<Base3_.RegionID, C>();

		protected abstract C createSender(Base3_.RegionID regionID);

		#region IRegionSender Members

		IRegionSink IRegionSender.this[QS._qss_c_.Base3_.RegionID regionID]
		{
			get 
			{ 
				C sender;
				lock (senders)
				{
					try
					{
						sender = senders[regionID];
					}
					catch (Exception)
					{						
						senders.Add(regionID, sender = createSender(regionID));
					}
				}
				return sender;
			}
		}

		#endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get { throw new System.NotImplementedException(); }
		}

#endregion
	}
}
