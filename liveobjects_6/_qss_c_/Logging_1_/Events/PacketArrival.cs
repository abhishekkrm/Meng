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

namespace QS._qss_c_.Logging_1_.Events
{
    [EventName("Packet Arrival")]
    public class PacketArrival : GenericEvent
    {
        public PacketArrival(double time, object source, QS._core_c_.Base3.InstanceID senderAddress, 
            QS._core_c_.Base3.InstanceID receiverAddress, QS._core_c_.Base3.Message message) : base(time, receiverAddress, source)
        {
            this.senderAddress = senderAddress;
            this.receiverAddress = receiverAddress;
            this.channel = message.destinationLOID;
            this.message = message.transmittedObject.ToString();
        }

        [EventProperty("Sender")]
        private QS._core_c_.Base3.InstanceID senderAddress;
        
        [EventProperty("Receiver")]
        private QS._core_c_.Base3.InstanceID receiverAddress;
        
        private uint channel;
        [EventProperty("Channel")]
        private string Channel
        {
            get
            {
                return Enum.IsDefined(typeof(ReservedObjectID), channel) ? ((ReservedObjectID)channel).ToString() : channel.ToString();
            }
        }

        [EventProperty("Contents")]
        private string message;

        public static readonly EventClass EventClass = new EventClass(typeof(PacketArrival));

        #region Overrides for Event1

        protected override QS.Fx.Logging.IEventClass ClassOf()
        {
            return EventClass;
        }

        protected override string DescriptionOf()
        {
            return "Packet Deserialized";
        }

        protected override object PropertyOf(string property)
        {
            return EventClass.PropertyOf(this, property);
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append(senderAddress.ToString());
            s.Append(" - ");
            s.Append(receiverAddress.ToString());
            s.Append(" [");
            s.Append(this.Channel);
            s.Append("] ");
            s.Append(message);                    
            return s.ToString();
        }

        #endregion
    }
}
