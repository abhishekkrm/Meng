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

namespace QS._qss_c_.Senders5
{
	public class CollectingSink : ICollectingSink, ISource
	{
		public CollectingSink(ISimpleSink simpleSink)
		{
			this.simpleSink = simpleSink;
			simpleSink.Source = this;
		}

		private ISimpleSink simpleSink;
		private System.Collections.Generic.Queue<Channel> waitingChannels = new Queue<Channel>();
		private bool waiting = false;

		public ISimpleSink SimpleSink
		{
			get { return simpleSink; }
		}

		#region Internal Processing

		private void signalChannel(Channel channel)
		{
			lock (this)
			{
				if (!channel.Waiting)
				{
					waitingChannels.Enqueue(channel);
					channel.Waiting = true;

					if (!waiting)
					{
						waiting = true;
						simpleSink.Signal();
					}
				}
			}
		}

		private void unregisterChannel(Channel channel)
		{
		}

		#endregion

		#region ISource Members

		bool ISource.Ready
		{
			get { return waitingChannels.Count > 0; }
		}

		QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> ISource.Get(uint maximumSize)
		{
			Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message = null;
			lock (this)
			{				
				while (message == null && waitingChannels.Count > 0)
				{
					Channel channel = waitingChannels.Dequeue();
					while (message == null && channel.Source.Ready)
						message = channel.Source.Get(maximumSize);

					if (channel.Source.Ready)
						waitingChannels.Enqueue(channel);
					else
						channel.Waiting = false;
				}

				if (waitingChannels.Count == 0) // message == null
					waiting = false;
			}
			return message;
		}

		#endregion

		#region ISink Members

		IChannel ICollectingSink.Register(ISource source)
		{
			return new Channel(this, source);
		}

		#endregion

		#region Class Channel

		private class Channel : IChannel
		{
			public Channel(CollectingSink owner, ISource source)
			{
				this.owner = owner;
				this.source = source;
			}

			private CollectingSink owner;
			private ISource source;
			private bool waiting = false;

			#region Accessors

			public bool Waiting
			{
				get { return waiting; }
				set { waiting = value; }
			}

			public ISource Source
			{
				get { return source; }
			}

			#endregion

			#region IChannel Members

			void IGenericSink.Signal()
			{
				if (!waiting && source.Ready)
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
