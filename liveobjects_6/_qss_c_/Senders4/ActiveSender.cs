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

namespace QS._qss_c_.Senders4
{
	public class ActiveSender
	{
		public ActiveSender()
		{
			// .............................................................................
		}

		// .............................................................................

		private class Sender
		{
			public Sender(ActiveSender owner)
			{
				this.owner = owner;
				consumeAsyncCallback = new AsyncCallback(consumeCallback);
				activeMessageSink = new ActiveMessageSink(new QS._qss_c_.Base3_.NoArgumentCallback(SignalCallback));

				// bufferingController = null; // <--should do something about it ! ...................is it used?
				// flowController = null; // <--should do something about it ! .......................is it used?
			}

			private ActiveSender owner;
			private bool waiting = false;
			// private FlowControl4.IFlowController flowController; ........................is it used?
			// private Buffering3.IController bufferingController; ............................is it used?
			private AsyncCallback consumeAsyncCallback;

			private ActiveMessageSink activeMessageSink;

			#region Internal Processing

			private void  SignalCallback()
			{
				if (!waiting)
				{
					waiting = true;
/*
					IAsyncResult asynchronousResult = flowController.BeginConsume(consumeAsyncCallback, null);
					if (asynchronousResult.CompletedSynchronously)
					{
						QS.Fx.Serialization.ISerializable data = ((IPassiveMessageSource)activeMessageSink).NextMessage;
						while (


						bufferingController.ReadyQueue.Count


					}
*/
				}
			}

			private void consumeCallback(IAsyncResult asynchronousResult)
			{
			}

			#endregion
		}
	}
}
