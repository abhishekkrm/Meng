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

// #define DEBUG_InstanceReceiver

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders6
{
    public class InstanceReceiver
    {
        public InstanceReceiver(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID localIID, Base3_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            this.localIID = localIID;
            this.demultiplexer = demultiplexer;

            demultiplexer.register((uint)ReservedObjectID.Senders6_InstanceReceiver, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private QS._core_c_.Base3.InstanceID localIID;
        private Base3_.IDemultiplexer demultiplexer;

        #region Receive Callback

        private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            InstanceSender.Message message = receivedObject as InstanceSender.Message;
            if (message != null)
            {
                if (message.Incarnation.Equals(localIID.Incarnation))
                    demultiplexer.dispatch(message.DestinationLOID, sourceIID, message.DataObject);
                else
                {
#if DEBUG_InstanceReceiver
                    logger.Log(this, "Message meant for incarnation " + message.Incarnation.ToString() + ", but this is "+ 
                        localIID.Incarnation.ToString() + ", ignoring the message.\nMessage contents: " + receivedObject.ToString());
#endif
                }
            }
            else
                throw new Exception("Wrong message type.");

            return null;
        }

        #endregion
    }
}
