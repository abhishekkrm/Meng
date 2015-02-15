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

using QS.Fx.Object.Classes;
using QS.Fx.Value;
using QS.Fx.Base;
using QS.Fx.Serialization;

using Quilt.Transmitter;
using Quilt.Core;

namespace Quilt.Bootstrap
{
    // Boostrap component of Quilt, residing in QuiltPeer, having acess of Transmitter
    public sealed class Bootstrap :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IBootstrap<BootstrapMember, BootstrapMembership>,
        QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>
    {
        #region Constructor

        public Bootstrap(
            QS.Fx.Object.IContext _mycontext,
            Transmitter.Transmitter _transmitter,
            EUIDAddress _bootstrap_server)
            : base(_mycontext, true)
        {
            this._mycontext = _mycontext;
            this._transmitter = _transmitter;
            this._bootstrap_server = _bootstrap_server;

            this._bootstrap_endpt = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>,
                QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>>(this);
        }

        #endregion

        #region Field

        private QS.Fx.Object.IContext _mycontext;
        private Transmitter.Transmitter _transmitter;
        public EUIDAddress _bootstrap_server;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>,
            QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>> _bootstrap_endpt;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IBootstrap<BootstrapMember,PatchInfo> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IBootstrapClient<BootstrapMembership>, 
            QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>> IBootstrap<BootstrapMember, BootstrapMembership>.Bootstrap
        {
            get { return this._bootstrap_endpt; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IBootstrapOps<BootstrapMember> Members

        void QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>.Join(string group_id, BootstrapMember self)
        {
            // Send a BootstrapJoin message to bootstrap server
            BootstrapJoin join_msg = new BootstrapJoin(new Name(group_id), self);
            _transmitter.SendMessage(_bootstrap_server, join_msg);
        }

        void QS.Fx.Interface.Classes.IBootstrapOps<BootstrapMember>.Leave(string group_id, string self_id)
        {
            BootstrapLeave leave_msg = new BootstrapLeave(new Name(group_id), new Name(self_id));
            _transmitter.SendMessage(_bootstrap_server, leave_msg);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ProcessBootstrapMsg
        
        /// <summary>
        /// Processing BootstrapMembership messages
        /// Called by the QuiltPeer, since this Bootstrap does not register its callback to transmitter
        /// </summary>
        public void ProcessBootstrapMsg(EUIDAddress remote_euid, ISerializable message)
        {
            if (message.SerializableInfo.ClassID == (ushort)QS.ClassID.BootstrapMembership)
            {
                // Membership Message
                BootstrapMembership membership_msg = message as BootstrapMembership;
                this._bootstrap_endpt.Interface.BootstrapMembership(membership_msg);
            }
        }

        #endregion
    }
}
