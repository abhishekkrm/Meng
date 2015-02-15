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

namespace QS._qss_x_.ObjectDesigner_
{
    [QS.Fx.Reflection.ObjectClass("4FABAA281DEB4FB9AF77C6ED3C7B9A93`0", "_o_foo_2", "_o_foo_2")]
    public interface _o_foo_2 : QS.Fx.Object.Classes.IObject
    {
        [QS.Fx.Reflection.Endpoint("Channel1")]
        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Value.Classes.IText>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Value.Classes.IText>> Channel1
        {
            get;
        }

        [QS.Fx.Reflection.Endpoint("Channel2")]
        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Value.Classes.IText>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Value.Classes.IText>> Channel2
        {
            get;
        }
    }

    [QS.Fx.Reflection.ComponentClass("ED87EB02C72A4D158C499CBC4A69D55C`0", "_c_foo_2", "_c_foo_2")]
    public sealed class _c_foo_2 : _o_foo_2
    {
        public _c_foo_2(QS.Fx.Object.IContext _context)
        {
        }

        #region _o_foo_2 Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Value.Classes.IText>, QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Value.Classes.IText>> _o_foo_2.Channel1
        {
            get { throw new NotImplementedException(); }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Value.Classes.IText>, QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Value.Classes.IText>> _o_foo_2.Channel2
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
