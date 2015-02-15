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

namespace QS._qss_x_.Uplink_2_
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public sealed class UplinkController_ : IUplinkController_, IDisposable
    {
        #region Constructor

        public UplinkController_(QS._core_c_.Core.ICore _core, int _portno, QS._core_c_.Core.IChannelController _channelclientcontroller)
        {
            this._core = _core;
            this._portno = _portno;
            this._channelclientcontroller = _channelclientcontroller;
            NetTcpBinding _binding = new NetTcpBinding();
            _binding.Security.Mode = SecurityMode.Transport;
            _binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            Uri _myuri = new Uri("net.tcp://localhost:" + this._portno.ToString() + "/controller");
            this._servicehost = new ServiceHost(this, _myuri);
            this._servicehost.AddServiceEndpoint(typeof(IUplinkController_), _binding, string.Empty);
            this._servicehost.Open();
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
                    _servicehost.Close();
                }
            }
        }

        #endregion

        #region Fields

        private QS._core_c_.Core.ICore _core;
        private int _disposed, _portno, _capacity_control, _capacity_data;
        private ServiceHost _servicehost;
        private IDictionary<string, UplinkConnection_> _connections = new Dictionary<string, UplinkConnection_>();
        private QS._core_c_.Core.IChannelController _channelclientcontroller;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IController_ Members

        void IUplinkController_._Connect(string _id, string _name)
        {
            Request_ _request;
            lock (this)
            {
                if (this._connections.ContainsKey(_id))
                    throw new Exception("Application with id \"" + _id + "\" is already connected.");
                UplinkConnection_ _connection = new UplinkConnection_(_id, _name);
                _connections.Add(_id, _connection);
                _request = new Request_(_id, _name, _connection);
                _core.Schedule(
                    new QS.Fx.Base.Event<Request_>(
                        new QS.Fx.Base.ContextCallback<Request_>(this._ConnectCallback), _request));
            }
            _request._done.WaitOne();
        }

        void IUplinkController_._Disconnect(string _id)
        {
            lock (this)
            {
                UplinkConnection_ _connection;
                if (!this._connections.TryGetValue(_id, out _connection))
                    throw new Exception("Application with id \"" + _id + "\" is not yet connected.");
                _connections.Remove(_id);
                ((IDisposable) _connection).Dispose();
                this._channelclientcontroller.Close(_id);
                Request_ _request = new Request_(_id, null, null);
                _core.Schedule(
                    new QS.Fx.Base.Event<Request_>(
                        new QS.Fx.Base.ContextCallback<Request_>(this._DisconnectCallback), _request));
                _request._done.WaitOne();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Request_

        private sealed class Request_
        {
            public Request_(string _id, string _name, UplinkConnection_ _connection)
            {
                this._connection = _connection;
                this._id = _id;
                this._name = _name;
                this._done = new ManualResetEvent(false);
            }

            public string _id, _name;
            public UplinkConnection_ _connection;
            public ManualResetEvent _done;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ConnectCallback

        private void _ConnectCallback(Request_ _request)
        {
            QS._core_c_.Core.IChannel _channel = _core.Open(_request._id, _request._name, _request._connection);
            QS._core_c_.Core.IChannel _channelclient = this._channelclientcontroller.Open(_request._id, _request._name, _channel);
            _request._connection._Connect(_channelclient);
            _request._done.Set();
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback(Request_ _request)
        {
            _core.Close(_request._id);
            _request._done.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
