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

namespace QS._qss_c_.Senders8
{
    public class ScatteringSource<AddressClass> : IScatteringSource<AddressClass>, ISink<ISource<AddressClass>> 
        where AddressClass : QS.Fx.Serialization.ISerializable
    {
        public ScatteringSource() : this(null)
        {
        }

        public ScatteringSource(ISink<IScatteringSource<AddressClass>> sink)
        {
            uplinkChannel = sink.Register(this);
        }

        private Queue<Channel> pendingQueue = new Queue<Channel>();
        private IChannel uplinkChannel;

        private void Signaled(Channel channel)
        {
            lock (this)
            {
                pendingQueue.Enqueue(channel);
            }

            if (uplinkChannel != null)
                uplinkChannel.Signal();
        }

        #region IScatteringSource<AddressClass> Members

        bool IScatteringSource<AddressClass>.Ready
        {
            get { return pendingQueue.Count > 0; }
        }

        Nullable<QS._qss_c_.Base3_.Addressed<AddressClass, QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>>> 
            IScatteringSource<AddressClass>.Get(uint maximumSize)
        {
            lock (this)
            {
                while (pendingQueue.Count > 0)
                {
                    Channel channel = pendingQueue.Dequeue();

                    lock (channel)
                    {
                        while (channel.Source.Ready)
                        {
                            Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> request = channel.Source.Get(maximumSize);
                            if (request != null)
                            {
                                pendingQueue.Enqueue(channel);
                                return new Base3_.Addressed<AddressClass, Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>>(channel.Address, request);
                            }
                        }

                        channel.Cleared();
                    }                    
                }

                return null;
            }
        }

        #endregion

        #region Class Channel

        private class Channel : IChannel
        {
            public Channel(ScatteringSource<AddressClass> owner, ISource<AddressClass> source) : this(owner, source.Address, source)
            {
            }

            public Channel(ScatteringSource<AddressClass> owner, AddressClass destinationAddress, ISource source)
            {
                this.owner = owner;
                this.destinationAddress = destinationAddress;
                this.source = source;
            }

            private ScatteringSource<AddressClass> owner;
            private AddressClass destinationAddress;
            private ISource source;
            private bool signaled = false;

            public ISource Source
            {
                get { return source; }
            }

            public AddressClass Address
            {
                get { return destinationAddress; }
            }

            public void Cleared()
            {
                signaled = false;
            }

            #region IChannel Members

            void IChannel.Signal()
            {
                bool signaled_now;
                lock (this)
                {
                    if (signaled_now = !signaled)
                        signaled = true;
                }

                if (signaled_now)
                    owner.Signaled(this);
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

        #endregion

        #region ISink<ISource<AddressClass>> Members

        IChannel ISink<ISource<AddressClass>>.Register(ISource<AddressClass> source)
        {
            return new Channel(this, source);
        }

        #endregion
    }
}
