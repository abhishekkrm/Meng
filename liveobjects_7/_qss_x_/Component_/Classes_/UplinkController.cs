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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UplinkController, "Uplink Controller",
        "An object that manages connections from processes on the local machine.")]
    public sealed class UplinkController
        : QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>, QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public UplinkController(QS.Fx.Object.IContext _mycontext, [QS.Fx.Reflection.Parameter("capacity", QS.Fx.Reflection.ParameterClass.Value)] int _capacity)
        {
            this._capacity = _capacity;
            this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
        }

        #endregion

        #region Fields

        private int _capacity;
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;
        // private QS.Fx.Uplink.Controller _controller;

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
//                if (this._controller == null)
//                {
//                    this._controller = new QS.Fx.Uplink.Controller(this._capacity);
//                }

                return null;
/*
                return QS.Fx.Object.Reference<QS.Fx.Object.Classes.IObject>.Create(this._controller);
                    new QS.Fx.Component.Classes.Factory_2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.Centralized_CC.IMessage>,
                            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Channel.Message.Centralized_CC.IMessage>>>
                    (
                        new QS.Fx.Interface.Reference<
                            QS.Fx.Interface.Classes.IFactory2<
                                QS.Fx.Endpoint.Classes.IDualInterface<
                                    QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.Centralized_CC.IMessage>,
                                    QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Channel.Message.Centralized_CC.IMessage>>>>
                        (
                            this._controller
                        )
                    ),
                    "controller",
                    QS.Fx.Reflection.Library.ObjectClassOf
                    (
                        typeof(QS.Fx.Object.Classes.IFactory2<
                            QS.Fx.Endpoint.Classes.IDualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannelClient<QS.Fx.Channel.Message.Centralized_CC.IMessage>,
                                QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Channel.Message.Centralized_CC.IMessage>>>)
                    )
                );
*/ 
            }
        }

        #endregion
    }
}
