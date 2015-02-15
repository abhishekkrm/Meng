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

namespace QS._qss_c_.FailureDetection_.Centralized
{
    public class Subscriber : IFailureDetector
    {
        public Subscriber(QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer)
        {
            this.logger = logger;
            demultiplexer.register((uint)ReservedObjectID.FailureDetection_Subscriber, new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback));
        }

        private QS.Fx.Logging.ILogger logger;
        private event ChangeCallback onChange;

        private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
            QS._core_c_.Base3.InstanceID crashedIID = (QS._core_c_.Base3.InstanceID) receivedObject;
            if (onChange != null)
                onChange(new Change[] { new Change(crashedIID, Action.CRASHED) });

            return null;
        }

        #region IFailureDetector Members

        event ChangeCallback IFailureDetector.OnChange
        {
            add { onChange += value; }
            remove { onChange -= value; }
        }

        #endregion
    }
}
