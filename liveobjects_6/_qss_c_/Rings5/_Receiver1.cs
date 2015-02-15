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

namespace QS._qss_c_.Rings5
{
    /// <summary>
    /// This is a receiver that forms a part of a group in which all members are caching all of the received messages and use push/pull
    /// to fill the gaps, acknowledging the receipt of data jointly.
    /// </summary>
    public class _Receiver1 : _IReceiverClass
    {
        public _Receiver1(QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        private QS.Fx.Logging.ILogger logger;

        #region Class Receiver

        private class Receiver : _IReceiver
        {
            public Receiver(_Receiver1 owner, _IReceiverContext context)
            {
                this.owner = owner;
                this.context = context;
            }

            private _Receiver1 owner;
            private _IReceiverContext context;

            #region IReceiver Members

            void _IReceiver.Receive(uint sequenceNo, QS._core_c_.Base3.Message message)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void _IReceiver.Process(out IAsynchronousCall<QS.Fx.Serialization.ISerializable> outgoingCall)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void _IReceiver.Process(IAsynchronousCall<QS.Fx.Serialization.ISerializable> incomingCall, out IAsynchronousCall<QS.Fx.Serialization.ISerializable> outgoingCall)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            void _IReceiver.Process(IAsynchronousCall<QS.Fx.Serialization.ISerializable> incomingCall)
            {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
        }

        #endregion

        #region IReceiverClass Members

        _IReceiver _IReceiverClass.CreateReceiver(_IReceiverContext context)
        {
            return new Receiver(this, context);
        }

        #endregion
    }
}
