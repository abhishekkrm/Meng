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
using System.Windows.Forms;
using System.ServiceModel;

namespace QS._qss_x_.Demo_.Component_
{

    [QS.Fx.Reflection.ComponentClass("DF81FF9A569442dbBEC4365FEFBA5C5C", "WebServiceRelay_")]
    public sealed class WebServiceRelay_<
         [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] 
        MessageClass,
         [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] 
        CheckpointClass> :
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
        QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class
        where CheckpointClass : class
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WebServiceRelay_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Web Service Address", QS.Fx.Reflection.ParameterClass.Value)] 
            string _svc_addr)
        {
            this._mycontext = _mycontext;

            // Create 
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
                >(this);

            // Connect to webservice w/o needing an App.config file
            System.ServiceModel.WSHttpBinding _binding = new WSHttpBinding();
            EndpointAddress _endpt = new EndpointAddress(_svc_addr);
            _logger = ChannelFactory<QS._qss_x_.Demo_.Interface_.ILog>.CreateChannel(_binding, _endpt);

            // Initialize the logger
            _logger.Init();

        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        QS._qss_x_.Demo_.Interface_.ILog _logger;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
                > _channelendpoint;

        #endregion




        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channelendpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            QS.Fx.Value.Classes.IText _t = _message as QS.Fx.Value.Classes.IText;
            if (_t != null)
            {
                this._logger.Log(_t.Text);
            }
            else
            {
                this._logger.Log(_message.ToString());
            }
        }

        #endregion
    }
}
