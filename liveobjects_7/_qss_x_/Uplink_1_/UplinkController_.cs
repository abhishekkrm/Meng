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
using System.Threading;
using System.Security.Cryptography;
using System.ServiceModel.Description;
using System.Diagnostics;
using System.Security.Principal;
using System.Security.Cryptography.X509Certificates;
using System.IO.Compression;

namespace QS._qss_x_.Uplink_1_
{
    public sealed class UplinkController_ : IDisposable,
        QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>,
        QS._qss_x_.Interface_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>
    {
        #region Constructor

        public UplinkController_(
            QS.Fx.Object.IContext _mycontext, 
            QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver,
            string _address, string _network, int _portno, QS._core_c_.Core.IChannelController _channelclientcontroller)
        {
            this._mycontext = _mycontext;
            this._myquicksilver = _myquicksilver;
            this._channelclientcontroller = _channelclientcontroller;
            this._endpoint =
                _mycontext.ExportedInterface<
                    QS._qss_x_.Interface_.Classes_.IFactory2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>>(this);
            this._underlyingchannelcontroller =
                new QS._qss_x_.Component_.Classes_.SecureTcpChannelController<QS.Fx.Serialization.ISerializable>
                (
                    _mycontext,
                    _address, 
                    _network, 
                    _portno,
                    QS._qss_x_.Object_.Reference<
                        QS._qss_x_.Object_.Classes_.IFactory2<
                            QS.Fx.Endpoint.Classes.IDualInterface<
                                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>>.Create
                                (
                                    this, 
                                    "UplinkController", 
                                    new QS.Fx.Attributes.Attributes
                                    (
                                        new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "Uplink Controller")
                                    ), 
                                    QS._qss_x_.Reflection_.Library.ObjectClassOf
                                    (
                                        typeof(QS._qss_x_.Object_.Classes_.IFactory2<
                                            QS.Fx.Endpoint.Classes.IDualInterface<
                                                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                                                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>)
                                    )
                                )
                );
        }

        #endregion

        #region Destructor

        ~UplinkController_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                }
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver;
        private int _disposed, _seqno;
        private QS._core_c_.Core.IChannelController _channelclientcontroller;
        private QS.Fx.Object.Classes.IObject _underlyingchannelcontroller;
        private QS.Fx.Endpoint.Internal.IExportedInterface<
            QS._qss_x_.Interface_.Classes_.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>> _endpoint;
        private ICollection<UplinkConnection_> _connections = new System.Collections.ObjectModel.Collection<UplinkConnection_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IFactory2<IDualInterface<ICommunicationChannelClient<ISerializable>,ICommunicationChannel<ISerializable>>> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<
            QS._qss_x_.Interface_.Classes_.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>> 
        QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>, 
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion

        #region IFactory2<IDualInterface<ICommunicationChannelClient<ISerializable>,ICommunicationChannel<ISerializable>>> Members

        QS.Fx.Endpoint.IReference<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>
        QS._qss_x_.Interface_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>.Create()
        {
            UplinkConnection_ _connection = new UplinkConnection_(_mycontext, this._myquicksilver, this);
            ManualResetEvent _done = new ManualResetEvent(false);
            this._myquicksilver._Core.Schedule(
                new QS.Fx.Base.Event<UplinkConnection_, ManualResetEvent>(
                    new QS.Fx.Base.ContextCallback<UplinkConnection_, ManualResetEvent>(this._ConnectionCallback),
                    _connection,
                    _done));
            _done.WaitOne();
            return new QS._qss_x_.Endpoint_.Reference<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>>(
                        ((QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>) _connection)._Channel);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ConnectionCallback

        private void _ConnectionCallback(UplinkConnection_ _connection, ManualResetEvent _done)
        {
            string _id = (++this._seqno).ToString();
            string _name = _id;
            QS._core_c_.Core.IChannel _channel = this._channelclientcontroller.Open(_id, _name, _connection);
            _connection._Connect(_channel);
            this._connections.Add(_connection);
            _done.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
