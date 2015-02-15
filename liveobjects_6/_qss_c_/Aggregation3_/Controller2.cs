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

using System.Threading;

namespace QS._qss_c_.Aggregation3_
{
/*
	public class Controller2 : Controller
	{
		public Controller2()
		{
		}

		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private IList<QS._core_c_.Base3.InstanceID> incomingAddresses;
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private QS._core_c_.Base3.InstanceID outgoingAddress;
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private bool submitted;
		[TMS.Inspection.Inspectable(QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private bool acknowledgementsCollected;

		private QS.Fx.Serialization.ISerializable aggregatedObject;

		#region Checking Completion

		private void CheckCompletion()
		{
			bool was_not_completed = !completed;
			completed = submitted && acknowledgementsCollected;
			bool completed_now = was_not_completed && completed;

			Monitor.Exit(this);

			if (completed_now)
			{
				if (outgoingAddress != null)
					send(outgoingAddress, Base2.NullObject.Object);
				else
					invokeCallback();

				remove();
			}
		}

		#endregion

		#region Overriden methods from Controller

		protected override void initialize(IList<QS._core_c_.Base3.InstanceID> incomingAddresses, QS._core_c_.Base3.InstanceID outgoingAddress)
		{
			this.incomingAddresses = incomingAddresses;
			this.outgoingAddress = outgoingAddress;

			submitted = false;
			acknowledgementsCollected = (incomingAddresses.Count == 0);
		}

		protected override void submit(QS.Fx.Serialization.ISerializable data)
		{
			Monitor.Enter(this);

			if (!submitted)
			{

				submitted = true;
				this.CheckCompletion();
			}
			else
			{
				Monitor.Exit(this);
			}
		}

		protected override void aggregate()
		{
		}

		protected override void receive(QS.CMS.QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message)
		{
			Monitor.Enter(this);

			if (!acknowledgementsCollected && incomingAddresses.Remove(address) && incomingAddresses.Count == 0)
			{
				acknowledgementsCollected = true;
				this.CheckCompletion();
			}
			else
			{
				Monitor.Exit(this);
			}
		}

		#endregion
	}
*/
}
