/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using QS.Fx.Value;
using QS.Fx.Value.Classes;
using QS.Fx.Base;

using Quilt.Bootstrap;
using Quilt.Core;
using Quilt.Transmitter;

namespace Quilt.Multicast
{
    public class GradientProtocol: IMulticast
    {

        #region Constructor

        public GradientProtocol()
        {

        }

        #endregion

        #region Fields

        public double _schedule_interval;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        //private 

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IMulticast Members

        double IMulticast.GetScheduleInterval()
        {
            return _schedule_interval;
        }

        void IMulticast.Schedule()
        {
            throw new NotImplementedException();
        }

        void IMulticast.PublishData(DataBuffer.Data data)
        {
            throw new NotImplementedException();
        }

        void IMulticast.Join(PatchInfo patch_info, List<BootstrapMember> members, Quilt.Transmitter.Transmitter transmitter)
        {
            throw new NotImplementedException();
        }

        void IMulticast.ProcessMessage(EUIDAddress remote_addr, QS.Fx.Serialization.ISerializable message)
        {
            throw new NotImplementedException();
        }

        void IMulticast.SetCallback(QuiltPeer.ReceivedData call_back)
        {
            throw new NotImplementedException();
        }

        void IMulticast.SetShareState(QuiltPeer.ShareState share_state)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
