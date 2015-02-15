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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Centralized_CC_SVR, "Centralized Communication Channel Server",
        "A centralized server for a shared collection of named shared checkpointed communication channels based on a centralized server implementation.")]
    public sealed class Centralized_CC_SVR 
        : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>, QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public Centralized_CC_SVR(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("root", QS.Fx.Reflection.ParameterClass.Value)] 
                string _rootfolder,
            [QS.Fx.Reflection.Parameter("deserializer", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer)
        {
            this._mycontext = _mycontext;
            this._rootfolder = _rootfolder;
            this._deserializer = _deserializer;
            this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private string _rootfolder;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Centralized_CC_.Controller_SVR _controller;

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>
            QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
        {
            lock (this)
            {
                if (this._controller == null)
                {
                    this._controller = 
                        new QS._qss_x_.Centralized_CC_.Controller_SVR
                        (
                            _mycontext,
                            this._rootfolder, 
                            this._deserializer
                        );
                }

                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    new QS._qss_x_.Component_.Classes_.Factory_2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>
                    (
                        _mycontext,
                        new QS._qss_x_.Interface_.Reference<
                            QS._qss_x_.Interface_.Classes_.IFactory2<
                                QS.Fx.Endpoint.Classes.IDualInterface<
                                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>>
                        (
                            this._controller
                        )
                    ),
                    "controller",
                    QS._qss_x_.Reflection_.Library.ObjectClassOf
                    (
                        typeof(QS._qss_x_.Object_.Classes_.IFactory2<
                            QS.Fx.Endpoint.Classes.IDualInterface<
                                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>,
                                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>>>)
                    )
                );
            }
        }

        #endregion
    }
}
