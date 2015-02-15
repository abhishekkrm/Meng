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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_.Multicast_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS._qss_x_.Properties_.Component_.Classes_.Multicast_._Application, "Properties Framework Multicast Application")]
    public sealed class Application_ 
        : QS._qss_x_.Properties_.Component_.ChannelClient_<string, string>, 
        QS.Fx.Object.Classes.IUI        
    {
        #region Constructor

        public Application_
        (
            [QS.Fx.Reflection.Parameter("sending", QS.Fx.Reflection.ParameterClass.Value)]
            bool _sending,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<string, string>> _channel_reference
        )
            : base(_channel_reference)
        {
            this._sending = _sending;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private bool _sending;
        
        private UI_ _ui;
        private QS.Fx.Endpoint.Internal.IExportedUI _ui_endpoint;

        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get 
            {
                lock (this)
                {
                    if (this._ui == null)
                    {
                        this._ui = new UI_();
                        this._ui_endpoint = _mycontext.ExportedUI(this._ui);
                    }
                    return this._ui_endpoint;
                }
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Multicast_.Application_._Dispose");
#endif

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
            base._Start();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Multicast_.Application_._Start");
#endif

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Multicast_.Application_._Stop");
#endif

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            base._Stop();
        }

        #endregion

        #region _Top_Connect

        protected override void _Top_Connect()
        {
            base._Top_Connect();

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Top_Disconnect

        protected override void _Top_Disconnect()
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            base._Top_Disconnect();
        }

        #endregion

        #region _Top_Checkpoint

        protected override string _Top_Checkpoint()
        {
            base._Top_Checkpoint();

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            return null;
        }

        #endregion

        #region _Top_Initialize

        protected override void _Top_Initialize(string _checkpoint)
        {
            base._Top_Initialize(_checkpoint);

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Top_Receive

        protected override void _Top_Receive(string _message)
        {
            base._Top_Receive(_message);

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion
    }
*/
}
