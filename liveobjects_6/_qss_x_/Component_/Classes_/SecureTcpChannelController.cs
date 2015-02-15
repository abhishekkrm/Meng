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
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SecureTcpChannelController,
        "SecureTcpChannelController", "This component provides a server for secure point-to-point connections from remote clients.")]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public sealed class SecureTcpChannelController<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.SecureTcp_.Server, 
            QS.Fx.Object.Classes.IObject
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public SecureTcpChannelController(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address,
            [QS.Fx.Reflection.Parameter("network", QS.Fx.Reflection.ParameterClass.Value)] string _network,
            [QS.Fx.Reflection.Parameter("mainport", QS.Fx.Reflection.ParameterClass.Value)] int _mainport,
            [QS.Fx.Reflection.Parameter("servicefactory", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IFactory2<
                    QS.Fx.Endpoint.Classes.IDualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>>> _endpointfactory)
            : base(_mycontext, _address, _network, _mainport)
        {
            this._mycontext = _mycontext;
            this._factory_reference = _endpointfactory;
            this.endpoint =
                _mycontext.ImportedInterface<
                    QS._qss_x_.Interface_.Classes_.IFactory2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>>>();
            lock (this)
            {
                this._factory_object = _endpointfactory.Dereference(_mycontext);
                this.connection = ((QS.Fx.Endpoint.Classes.IEndpoint) this.endpoint).Connect(_factory_object.Endpoint);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>>> _factory_reference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>> _factory_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<
            QS._qss_x_.Interface_.Classes_.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>>> endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection connection;

        #endregion

        #region _Exception

        protected override void _Exception(Exception _exc)
        {
            try
            {
                (new QS._qss_x_.Base1_.ExceptionForm(_exc)).Show();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region _NewConnection

        protected override Connection _NewConnection()
        {
            return new _Connection(_mycontext, this.endpoint.Interface.Create());
        }

        #endregion

        #region _Disconnected

        protected override void _Disconnected(Connection _connection)
        {
        }

        #endregion

        #region Class _Connection

        private sealed class _Connection : QS._qss_x_.SecureTcp_.Server.Connection, QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>
        {
            #region Constructor

            public _Connection(
                QS.Fx.Object.IContext _mycontext,
                QS.Fx.Endpoint.IReference<
                    QS.Fx.Endpoint.Classes.IDualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>>> _endpointref)
                : base(_mycontext)
            {
                this._mycontext = _mycontext;
                this.endpoint = _mycontext.DualInterface<QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>>(this);
                this.endpoint.OnConnect += new QS.Fx.Base.Callback(this._ConnectCallback);
                this.endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._DisconnectCallback);
                this._otherendpoint = _endpointref.Endpoint;
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IContext _mycontext;

            [QS.Fx.Base.Inspectable("endpoint")]
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>> endpoint;

            [QS.Fx.Base.Inspectable("otherendpoint")]
            private QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<MessageClass>> _otherendpoint;

            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.IConnection connection;

            #endregion

            #region _Connected

            public override void _Connected()
            {
                this.connection = ((QS.Fx.Endpoint.Classes.IEndpoint) this.endpoint).Connect(this._otherendpoint);
            }

            #endregion

            #region _ConnectCallback

            private void _ConnectCallback()
            {
            }

            #endregion

            #region _DisconnectCallback

            private void _DisconnectCallback()
            {
                this._Disconnect();
            }

            #endregion

            #region _Receive

            protected override void _Receive(QS.Fx.Serialization.ISerializable message)
            {
                MessageClass _message = message as MessageClass;
                if (_message != null)
                    this.endpoint.Interface._Send(_message);
                else
                    _Disconnect();
            }

            #endregion

            #region ICommunicationChannelClient<MessageClass> Members

            void QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<MessageClass>._Receive(MessageClass _message)
            {
                this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Receive_0_), _message));
            }

            private void _Receive_0_(object _o)
            {
                QS.Fx.Serialization.ISerializable _message = (QS.Fx.Serialization.ISerializable) _o;
                lock (this)
                {
                    this._Send(_message);
                }
            }

            #endregion

            #region _Exception

            protected override void _Exception(Exception _exception)
            {
                this.endpoint.Disconnect();
                base._Exception(_exception);
            }

            #endregion
        }

        #endregion
    }
}
