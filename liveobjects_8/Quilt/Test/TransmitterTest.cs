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
using QS.Fx.Serialization;

using Quilt.Transmitter;

namespace Quilt.Test
{
    [QS.Fx.Reflection.ComponentClass("1719B17304CC444a8914408F22EAF2CA", "Quilt Transmitter Test")]
    public sealed class TransmitterTest:
        QS._qss_x_.Properties_.Component_.Base_
    {
        #region Constructor

        public TransmitterTest(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("EUIDTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>>
            _transport_object_reference,
            [QS.Fx.Reflection.Parameter("Opponent EUID", QS.Fx.Reflection.ParameterClass.Value)]
                string _opponent_euid)
            : base(_mycontext, true)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TransmitterTest.Constructor");
#endif
            this._mycontext = _mycontext;
            this._transport_object_reference = _transport_object_reference;
            this._opponent_euid = _opponent_euid;
            this._message_hander = MessageHandler;
        }

        #endregion

        #region Field

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        private string _opponent_euid;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, TransmitterMsg>> _transport_object_reference;
        private Transmitter.Transmitter _transmitter;
        private Transmitter.Transmitter.UpperMessageHandler _message_hander;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TransmitterTest._Initialize");
#endif

            this._transmitter = new Quilt.Transmitter.Transmitter(_mycontext, _transport_object_reference, _message_hander);

            base._Initialize();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MessageHandler

        private void MessageHandler(EUIDAddress remote_euid, ISerializable message)
        {
            if (message == null)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.TransmitterTest.MessageHandler Transmitter has been set up!");
#endif
                // Action
                if (_opponent_euid == "" || _opponent_euid == null)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.TransmitterTest.MessageHandler Wait!");
#endif
                }
                else
                {
                    EUIDAddress target = new EUIDAddress(_opponent_euid);
                    UnicodeText test_msg = new UnicodeText("This is a test message");
                    _transmitter.SendMessage(target, test_msg);
                }
            }
            else
            {
                if (message.SerializableInfo.ClassID == (ushort)(QS.ClassID.UnicodeText))
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.TransmitterTest.MessageHandler Received Message: " + ((UnicodeText)message).ToString());
#endif
                }
            }
        }

        #endregion

    }
}
