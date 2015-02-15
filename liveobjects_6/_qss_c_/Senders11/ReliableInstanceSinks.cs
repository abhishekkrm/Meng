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

namespace QS._qss_c_.Senders11
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class ReliableInstanceSinks : QS.Fx.Inspection.Inspectable, 
        Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        public ReliableInstanceSinks(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger, uint dataChannel,
            QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock,
            Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks)
        {
            this.logger = logger;
            this.eventLogger = eventLogger;
            this.dataChannel = dataChannel;
            this.localAddress = localAddress;
            this.alarmClock = alarmClock;
            this.clock = clock;
            this.downstreamSinks = downstreamSinks;
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private uint dataChannel;
        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Base6_.ICollectionOf<QS.Fx.Network.NetworkAddress, QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> downstreamSinks;
        [QS._core_c_.Diagnostics.ComponentCollection]
        [QS.Fx.Base.Inspectable("Sinks")]
        private IDictionary<QS._core_c_.Base3.InstanceID, ReliableInstanceSink> sinks = new Dictionary<QS._core_c_.Base3.InstanceID, ReliableInstanceSink>();

        public void Acknowledge(QS._core_c_.Base3.InstanceID sourceAddress, IList<Base1_.Range<uint>> acknowledgements)
        {
            ReliableInstanceSink sink;
            lock (this)
            {
                if (!sinks.TryGetValue(sourceAddress, out sink))
                    throw new Exception("Sink with address " + sourceAddress.ToString() + " does not exist.");
            }

            sink.Acknowledge(acknowledgements);
        }

        #region ICollectionOf<InstanceID,ISink<IAsynchronous<Message>>> Members

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> 
            QS._qss_c_.Base6_.ICollectionOf<QS._core_c_.Base3.InstanceID, 
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>>.this[QS._core_c_.Base3.InstanceID address]
        {
            get 
            {
                lock (this)
                {
                    ReliableInstanceSink sink;
                    if (!sinks.TryGetValue(address, out sink))
                    {
                        sinks.Add(address, sink = new ReliableInstanceSink(
                            logger, eventLogger, dataChannel, localAddress, address, alarmClock, clock, downstreamSinks[address.Address]));

                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register(address.ToString(), ((QS._core_c_.Diagnostics2.IModule)sink).Component);
                    }

                    return sink;                   
                }                
            }
        }

        #endregion
    }
}
