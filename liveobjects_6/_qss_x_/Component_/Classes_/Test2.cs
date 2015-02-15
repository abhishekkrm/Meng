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
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Test2, "Test2")]
    public sealed class Test2
        : QS._qss_x_.Object_.Classes_.ITest2, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>
    {
        #region Constructor

        public Test2
        (
            QS.Fx.Object.IContext _mycontext
        )
        {
            this._channel = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>>(this);
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>> _channel;

        #endregion

        #region ITest2 Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>> 
            QS._qss_x_.Object_.Classes_.ITest2.Channel
        {
            get { return this._channel; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<string,string> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>.Send(string _message)
        {
            System.Windows.Forms.MessageBox.Show(
                (_message != null) ? _message : "(null)",
                "Send in Test2", 
                System.Windows.Forms.MessageBoxButtons.OK, 
                System.Windows.Forms.MessageBoxIcon.Information); 
        }

        #endregion
    }
}
