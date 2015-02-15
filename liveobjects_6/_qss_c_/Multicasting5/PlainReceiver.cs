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

namespace QS._qss_c_.Multicasting5
{
    [QS.Fx.Base.Inspectable]
    public class PlainReceiver : QS.Fx.Inspection.Inspectable
    {
        public PlainReceiver(QS._core_c_.Base3.InstanceID localIID, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer)
        {
            this.localIID = localIID;
            this.logger = logger;
            this.demultiplexer = demultiplexer;

            demultiplexer.register((uint)ReservedObjectID.Multicasting5_PlainReceiver, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        private QS._core_c_.Base3.InstanceID localIID;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Message message = receivedObject as Message;
            if (message != null)
            {
                ReceiveMessage(sourceIID, message);
            }
            else
                throw new Exception("Wrong object type.");

            return null;
        }

        #endregion

        #region Processing Messages

        private void ReceiveMessage(QS._core_c_.Base3.InstanceID sourceIID, Message message)
        {
            Acknowledgement acknowledgement = message.AcknowledgementTo;


        }

        #endregion
    }
}
