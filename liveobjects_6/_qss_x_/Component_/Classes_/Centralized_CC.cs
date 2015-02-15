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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Centralized_CC, "Centralized Communication Channel Client", 
        "A client of a shared collection of named shared checkpointed communication channels based on a centralized server implementation.")]
    public sealed class Centralized_CC 
        : QS.Fx.Inspection.Inspectable, 
        QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>, QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public Centralized_CC(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("connection", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> _connection,
            [QS.Fx.Reflection.Parameter("deserializer", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer)
        {
            this._mycontext = _mycontext;
            this._connection = _connection;
            this._deserializer = _deserializer;
            this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Channel_.Message_.Centralized_CC.IMessage>> _connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Centralized_CC_.Controller _controller;

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
                    this._controller = new QS._qss_x_.Centralized_CC_.Controller(_mycontext, this._connection, this._deserializer);

                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    this._controller.Dictionary, 
                    "channels",
                    new QS.Fx.Attributes.Attributes
                    (
                        new QS.Fx.Attributes.IAttribute[]
                        {
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "Communication System"),
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, "A simple multicast-capable communication system."),
                        }
                    ),
                    QS._qss_x_.Reflection_.Library.ObjectClassOf
                    (
                        typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)
                    )
                );

/*
                return QS.Fx.Object.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    new QS.Fx.Component.Classes.Service<QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
                    (
                        new QS.Fx.Interface.Reference<QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
                        (
                            this._controller
                        )
                    ),
                    "controller",
                    QS.Fx.Reflection.Library.ObjectClassOf
                    (
                        typeof(QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>)
                    )
                );
*/
            }
        }

        #endregion
    }
}
