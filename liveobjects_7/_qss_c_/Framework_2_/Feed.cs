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

namespace QS._qss_c_.Framework_2_
{
    public sealed class Feed : IChannel
    {
        #region Constructor

        public Feed(GroupRef groupRef, Group group, bool ishybrid, OutgoingCallback outgoingCallback,
            QS.Fx.Base.ContextCallback<SendRequest> sendingCallback, QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback)
        {
            this.groupRef = groupRef;
            this.group = group;
            this.ishybrid = ishybrid;
            this.outgoingCallback = outgoingCallback;
            this.sendingCallback = sendingCallback;
            this.completionCallback = completionCallback;
        }

        #endregion

        #region Fields

        private GroupRef groupRef;
        private Group group;
        private OutgoingCallback outgoingCallback;
        private QS.Fx.Base.ContextCallback<SendRequest> sendingCallback;
        private QS._core_c_.Base6.CompletionCallback<SendRequest> completionCallback;
        private bool ishybrid;

        #endregion

        #region Getting Messages

        public void GetObjects(uint maxmessages, out bool hasmore)
        {
            outgoingCallback(this, maxmessages, out hasmore);
        }

        #endregion

        #region IChannel Members

        public void Send(QS.Fx.Serialization.ISerializable message)
        {
            Send(message, null, null);
        }

        public void Send(QS.Fx.Serialization.ISerializable message, CompletionCallback callback, object context)
        {
            SendRequest request = new SendRequest(
                message, (uint)ReservedObjectID.Framework2_Group, group, groupRef, callback, context, sendingCallback, completionCallback);
            
            if (ishybrid)
                group.HybridSend(request);
            else
                group.Send(request);
        }

        #endregion
    }
}
