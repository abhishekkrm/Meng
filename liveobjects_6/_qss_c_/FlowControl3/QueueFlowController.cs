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

namespace QS._qss_c_.FlowControl3
{
	public class QueueFlowController<C> : FlowController<C>, IWindowSender
	{
		public QueueFlowController(int initialWindowSize, ReadyCallback<C> readyCallback) : base(readyCallback)
		{
			this.windowSize = initialWindowSize;
		}

		private System.Collections.Generic.Queue<C> outgoingQueue = new Queue<C>();

		[QS.Fx.Base.Inspectable("Number of Requests In Transit", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private int inTransit = 0;
		[QS.Fx.Base.Inspectable("Window Size", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private int windowSize;

		#region IWindowSender Members

		int IWindowSender.WindowSize
		{
			get { return windowSize; }
			set
			{
				List<C> toSend = new List<C>();
				lock (this)
				{
					windowSize = value;
					while (inTransit < windowSize && outgoingQueue.Count > 0)
					{
						inTransit++;
						toSend.Add(outgoingQueue.Dequeue());
					}
				}
				if (toSend.Count > 0)
					readyCallback(toSend);
			}
		}

		#endregion

		#region IFlowController<C> Members

		public override void submit(C element, out IEnumerable<C> ready)
		{
			bool send_now;
			lock (this)
			{
				send_now = inTransit < windowSize;
				if (send_now)
					inTransit++;
				else
					outgoingQueue.Enqueue(element);
			}
			ready = send_now ? new C[] { element } : null;
		}

		public override void release(out IEnumerable<C> ready)
		{
			List<C> toSend = new List<C>();
			lock (this)
			{
				inTransit--;
				while (inTransit < windowSize && outgoingQueue.Count > 0)
				{
					inTransit++;
					toSend.Add(outgoingQueue.Dequeue());
				}
			}
			ready = toSend.Count > 0 ? toSend : null;
		}

		public override void resubmit(C element, out IEnumerable<C> ready)
		{
			ready = new C[] { element };
		}

		#endregion
	}
}
