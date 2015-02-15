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

namespace QS._qss_c_.Receivers_1_
{
    public class Receiver1 : IReceiverClass
    {
        public Receiver1(QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        private QS.Fx.Logging.ILogger logger;

        #region Class Receiver

        private class Receiver : IReceiver
        {
            public Receiver(Receiver1 owner,
                QS._core_c_.Base3.InstanceID sourceAddress, QS._qss_c_.Base3_.IAddressCollection peerAddresses)
            {
                this.owner = owner;
                this.sourceAddress = sourceAddress;
                this.peerAddresses = peerAddresses;
            }

            private Receiver1 owner;
            private QS._core_c_.Base3.InstanceID sourceAddress;
            private QS._qss_c_.Base3_.IAddressCollection peerAddresses;

            // .......................

            #region IReceiver Members

            void IReceiver.Receive(uint sequenceNo, QS._core_c_.Base3.Message message)
            {
                
                // ............................................................
                
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        #region IReceiverClass Members

        IReceiver IReceiverClass.Create(
            QS._core_c_.Base3.InstanceID sourceAddress, QS._qss_c_.Base3_.IAddressCollection peerAddresses)
        {
            return new Receiver(this, sourceAddress, peerAddresses);
        }

        #endregion
    }
}
