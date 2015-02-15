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
    /// <summary>
    /// Local client encapsulating a single instance of QSM. Multiple clients can be created independently, 
    /// although for best performance this should be avoided.
    /// </summary>
    public interface IClient : IDisposable
    {
        /// <summary>
        /// Obtain a reference to a group that can be used for sending or receiving. Multiple references to the same group are
        /// permitted. The local client maintains membership for as long as at least a single reference exists. This call is blocking.
        /// </summary>
        /// <param name="gid">Identifier of the group. Must be known upfront or obtained through a naming service. 
        /// In this version of QSM naming services are not provided.</param>
        /// <returns>A disposable reference to the group.</returns>
        QS._qss_c_.Framework_2_.IGroup Open(QS._qss_c_.Base3_.GroupID gid);

        /// <summary>
        /// Obtain a reference to a group that can be used for sending or receiving. Multiple references to the same group are
        /// permitted. The local client maintains membership for as long as at least a single reference exists. This call is blocking.
        /// </summary>
        /// <param name="gid">Identifier of the group. Must be known upfront or obtained through a naming service. 
        /// In this version of QSM naming services are not provided.</param>
        /// <param name="options">Options that control the way data flows through this attachment point.</param>
        /// <returns>A disposable reference to the group.</returns>
        QS._qss_c_.Framework_2_.IGroup Open(QS._qss_c_.Base3_.GroupID gid, GroupOptions options);

        /// <summary>
        /// An asynchronous, nonblocking version of the open call.
        /// </summary>
        /// <param name="gid">Identifier of the group. Must be known upfront or obtained through a naming service. 
        /// In this version of QSM naming services are not provided.</param>
        /// <param name="callback">Callback to be invoked when the reference is ready.</param>
        /// <param name="context">Context to pass to the callback.</param>
        /// <returns>A reference representing this asynchronous open request.</returns>
        IAsyncResult BeginOpen(QS._qss_c_.Base3_.GroupID gid, AsyncCallback callback, object context);

        /// <summary>
        /// An asynchronous, nonblocking version of the open call.
        /// </summary>
        /// <param name="gid">Identifier of the group. Must be known upfront or obtained through a naming service. 
        /// In this version of QSM naming services are not provided.</param>
        /// <param name="options">Options that control the way data flows through this attachment point.</param>
        /// <param name="callback">Callback to be invoked when the reference is ready.</param>
        /// <param name="context">Context to pass to the callback.</param>
        /// <returns>A reference representing this asynchronous open request.</returns>
        IAsyncResult BeginOpen(QS._qss_c_.Base3_.GroupID gid, GroupOptions options, AsyncCallback callback, object context);

        /// <summary>
        /// Completes the asynchronous open call.
        /// </summary>
        /// <param name="openreq">A reference representing an asynchronous open request.</param>
        /// <returns>A disposable reference to the group.</returns>
        QS._qss_c_.Framework_2_.IGroup EndOpen(IAsyncResult openreq);

        /// <summary>
        /// Accurate clock.
        /// </summary>
        QS.Fx.Clock.IClock Clock
        {
            get;
        }

//        /// <summary>
//        /// Gets or sets the maximum sending rate, in packets per second, imposed by this module on any given recipient.
//        /// </summary>
//        double MaximumRate
//        {
//            get;
//            set;
//        }
    }
}
