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

// #define DEBUG_RegionContext

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings4
{
    public class RegionContext : IContext
    {
        public RegionContext(Base3_.RVID rvid, uint controlChannel, uint forwardingChannel, uint acknowledgementChannel,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection, QS.Fx.Logging.ILogger logger)
        {
            this.rvid = rvid;
            this.senderCollection = senderCollection;
            this.forwardingChannel = forwardingChannel;
            this.controlChannel = controlChannel;
            this.acknowledgementChannel = acknowledgementChannel;
            this.logger = logger;
        }

        private Base3_.RVID rvid;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection;
        private uint controlChannel, forwardingChannel, acknowledgementChannel;
        private QS.Fx.Logging.ILogger logger;

        #region IContext Members

        void IContext.Forward(QS._core_c_.Base3.InstanceID senderAddress, uint sequenceNo, 
            QS._core_c_.Base3.Message message, IEnumerable<QS._core_c_.Base3.InstanceID> destinationAddresses)
        {
#if DEBUG_RegionContext
            logger.Log(this, "__Forward(" + rvid.ToString() + ", " + senderAddress + ") message " +
                sequenceNo.ToString() + " to " + destinationAddresses.ToString());
#endif

            Multicasting5.ForwardingRV forwardingRV = 
                new QS._qss_c_.Multicasting5.ForwardingRV(rvid, senderAddress, sequenceNo, message);

            foreach (QS._core_c_.Base3.InstanceID destinationAddress in destinationAddresses)
                ((Base3_.IReliableSerializableSender)senderCollection[destinationAddress]).BeginSend(
                    forwardingChannel, forwardingRV, null, null);
        }

        string IContext.Name
        {
            get { return rvid.ToString(); }
        }

        QS._core_c_.Base3.Message IContext.Wrap(QS._core_c_.Base3.Message message)
        {
            return new QS._core_c_.Base3.Message(controlChannel, new ObjectRV(rvid, message));
        }

        void IContext.Acknowledge(QS._core_c_.Base3.InstanceID address, QS._core_c_.Base3.Message acknowledgementObject)
        {
#if DEBUG_RegionContext
            logger.Log(this, "__Acknowledge(" + rvid.ToString() + ", " + address + ") : " + acknowledgementObject.ToString());
#endif

            ((Base3_.IReliableSerializableSender)senderCollection[address]).BeginSend(
                acknowledgementChannel, new ObjectRV(rvid, acknowledgementObject), null, null);
        }

        #endregion
    }
}
