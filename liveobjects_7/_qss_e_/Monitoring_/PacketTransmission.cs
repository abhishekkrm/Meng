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

namespace QS._qss_e_.Monitoring_
{
    public class PacketTransmission
    {
        public PacketTransmission(
            QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
            QS.Fx.Network.NetworkAddress receiverAddress, bool succeeded, string error, double timeSent, double timeArrived, 
            string contents)
        {
            this.sourceAddress = sourceAddress;
            this.destinationAddress = destinationAddress;
            this.receiverAddress = receiverAddress;
            this.succeeded = succeeded;
            this.error = error;
            this.timeSent = timeSent;
            this.timeArrived = timeArrived;
            this.contents = contents;
        }

        private QS.Fx.Network.NetworkAddress sourceAddress, destinationAddress, receiverAddress;
        private bool succeeded;
        private double timeSent, timeArrived;
        private string error, contents;

        public QS.Fx.Network.NetworkAddress SourceAddress
        {
            get { return sourceAddress; }
        }

        public QS.Fx.Network.NetworkAddress DestinationAddress
        {
            get { return destinationAddress; }
        }

        public QS.Fx.Network.NetworkAddress ReceiverAddress
        {
            get { return receiverAddress; }
        }

        public bool Succeeded
        {
            get { return succeeded; }
        }

        public double TimeSent
        {
            get { return timeSent; }
        }

        public double TimeArrived
        {
            get { return timeArrived; }
        }

        public string Error
        {
            get { return error; }
        }

        public string Contents
        {
            get { return contents; }
        }
    }
}
