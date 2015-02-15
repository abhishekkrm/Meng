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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders4
{
	public class ActiveMessageSink : IActiveMessageSink, IPassiveMessageSource
	{
		public ActiveMessageSink(Base3_.NoArgumentCallback signalCallback)
		{
			this.signalCallback = signalCallback;
		}

		private System.Collections.Generic.Queue<Channel> waitingChannels = new Queue<Channel>();
		private Base3_.NoArgumentCallback signalCallback;

		#region IPassiveMessageSource Members

		QS.Fx.Serialization.ISerializable IPassiveMessageSource.NextMessage
		{
			get 
			{
				return null;
			}
		}

		#endregion

		#region Internal Processing

		private void signalChannel(Channel channel)
		{
			lock (this)
			{
				if (!channel.Waiting)
				{
					waitingChannels.Enqueue(channel);
					channel.Waiting = true;

					signalCallback();
				}
			}
		}

		private void unregisterChannel(Channel channel)
		{
		}

		#endregion

		#region IActiveMessageSink Members

		IActiveSinkChannel IActiveMessageSink.Register(GetMessageCallback getMessageCallback)
		{
			return new Channel(this, getMessageCallback);
		}

		#endregion

		#region Class Channel

		private class Channel : IActiveSinkChannel
		{
			public Channel(ActiveMessageSink owner, GetMessageCallback getMessageCallback)
			{
				this.owner = owner;
				this.getMessageCallback = getMessageCallback;
			}

			private ActiveMessageSink owner;
			private GetMessageCallback getMessageCallback;
			private bool waiting = false;

			#region Accessors

			public IEnumerator<ISendRequest> GetRequests
			{
				get { return getMessageCallback(); }
			}

			public bool Waiting
			{
				get { return waiting; }
				set { waiting = value; }
			}

			#endregion

			#region IActiveSinkChannel Members

			void IActiveSinkChannel.Signal()
			{
				if (!waiting)
					owner.signalChannel(this);
			}

			#endregion

			#region System.IDisposable Members

			void IDisposable.Dispose()
			{
				owner.unregisterChannel(this);
			}

			#endregion
		}

		#endregion
	}
}
