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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Test1, "Test1")]
    public sealed class Test1 
        : QS.Fx.Object.Classes.IObject, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>
    {
        #region Constructor

        public Test1
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("other", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ITest1> _other
        )
        {
            this._channel = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>>(this);
            this._other = _other.Dereference(_mycontext);
            this._connection = this._other.Channel.Connect(this._channel);
            this._channel.Interface.Send("Hello");
        }

        #endregion

        #region Fields

        private QS._qss_x_.Object_.Classes_.ITest1 _other;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<string, string>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>> _channel;
        private QS.Fx.Endpoint.IConnection _connection;

        #endregion

        #region ICheckpointedCommunicationChannelClient<string,string> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>.Receive(string _message)
        {
            System.Windows.Forms.MessageBox.Show(
                (_message != null) ? _message : "(null)",
                "Receive in Test1",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information); 
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>.Initialize(string _checkpoint)
        {
            System.Windows.Forms.MessageBox.Show(
                (_checkpoint != null) ? _checkpoint : "(null)",
                "Initialize in Test1",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information);
        }

        string QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<string, string>.Checkpoint()
        {
            return "(checkpoint)";
        }

        #endregion
    }
}
