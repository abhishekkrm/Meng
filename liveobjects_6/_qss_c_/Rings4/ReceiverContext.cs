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

namespace QS._qss_c_.Rings4
{
    public class ReceiverContext : IReceiverContext
    {
        public ReceiverContext(
            QS._core_c_.Base3.InstanceID senderAddress, IContext collectionContext, uint controlChannel,
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection)
        {
            this.senderAddress = senderAddress;
            this.collectionContext = collectionContext;
            this.controlChannel = controlChannel;
            this.senderCollection = senderCollection;
        }

        private QS._core_c_.Base3.InstanceID senderAddress;
        private IContext collectionContext;
        private uint controlChannel;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, Base3_.IReliableSerializableSender> senderCollection;

        #region IReceiverContext Members

        string IReceiverContext.Name
        {
            get { return senderAddress.ToString() + "-" + collectionContext.Name; }
        }

/*
        QS.CMS.Base3.Message IReceiverContext.Wrap(QS.Fx.Serialization.ISerializable dataObject)
        {
            return collectionContext.Wrap(new Base3.Message(controlChannel, new ControlReq(senderAddress, dataObject)));
        }
*/

        void IReceiverContext.SendControl(QS._core_c_.Base3.InstanceID receiverAddress, QS.Fx.Serialization.ISerializable dataObject)
        {
            QS._core_c_.Base3.Message message = 
                collectionContext.Wrap(new QS._core_c_.Base3.Message(controlChannel, new ControlReq(senderAddress, dataObject)));
            ((Base3_.IReliableSerializableSender)senderCollection[receiverAddress]).BeginSend(
                message.destinationLOID, message.transmittedObject, null, null);
        }

        void IReceiverContext.Forward(
            uint sequenceNo, QS._core_c_.Base3.Message message, IEnumerable<QS._core_c_.Base3.InstanceID> destinationAddresses)
        {
            collectionContext.Forward(senderAddress, sequenceNo, message, destinationAddresses);
        }

        void IReceiverContext.Acknowledge(QS._core_c_.Base3.Message acknowledgementObject)
        {
            collectionContext.Acknowledge(senderAddress, acknowledgementObject);
        }

        #endregion
    }
}
