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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Component_.Classes_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.CentralizedCommunicationChannelServer,
        "CentralizedCommunicationChannelServer", "This is a server for simple, totally ordered communication channels.")]
    public sealed class CentralizedCommunicationChannelServer<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS.TMS.Inspection.Inspectable, 
            QS.Fx.Object.Classes.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>,
            QS.Fx.Interface.Classes.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public CentralizedCommunicationChannelServer()
        {
            this.endpoint = 
                _mycontext.ExportedInterface<
                    QS.Fx.Interface.Classes.IFactory2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>, 
                            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>>(this);
        }

        #endregion

        #region Fields

        [QS.TMS.Inspection.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<
            QS.Fx.Interface.Classes.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>> endpoint;

        [QS.TMS.Inspection.Inspectable]
        private ICollection<
            QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>> connections = new
                        System.Collections.ObjectModel.Collection<
                            QS.Fx.Endpoint.Internal.IDualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>();

        #endregion

        #region IFactory2<IDualInterface<ICommunicationChannelClient<MessageClass>,ICommunicationChannel<MessageClass>>> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<
            QS.Fx.Interface.Classes.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>, 
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>> 
            QS.Fx.Object.Classes.IFactory2
                <QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>, 
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>.Endpoint
        {
            get { return this.endpoint; }
        }

        #endregion

        #region IFactory2<IDualInterface<ICommunicationChannelClient<MessageClass>,ICommunicationChannel<MessageClass>>> Members

        QS.Fx.Endpoint.IReference<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>, 
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>> 
            QS.Fx.Interface.Classes.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>, 
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>.Create()
        {
            lock (this)
            {
                QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> connection = 
                            _mycontext.DualInterface<
                                QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>(this);
                
                this.connections.Add(connection);
                
                return new QS.Fx.Endpoint.Reference<
                    QS.Fx.Endpoint.Classes.IDualInterface<
                        QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>>(connection);
            }
        }

        #endregion

        #region ICommunicationChannel<MessageClass> Members

        void QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>.Send(MessageClass _message)
        {
            lock (this)
            {
                foreach (QS.Fx.Endpoint.Internal.IDualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannelClient<MessageClass>,
                        QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> connection in this.connections)
                {
                    if (connection.IsConnected)
                        connection.Interface.Receive(_message);
                }
            }
        }

        #endregion
    }
*/ 
}
