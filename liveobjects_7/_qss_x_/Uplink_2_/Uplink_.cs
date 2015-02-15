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
using System.Runtime.InteropServices;

namespace QS._qss_x_.Uplink_2_
{
    public sealed class Uplink_ : 
        QS._core_c_.Core.IChannel,
        IDisposable
    {
        #region Constructor

        public Uplink_(QS._core_c_.Core.ICore _core, int _portno, QS._core_c_.Core.IChannel _channelclient)
        {
            this._core = _core;
            this._portno = _portno;
            this._channelclient = _channelclient;
            NetTcpBinding _binding = new NetTcpBinding();
            _binding.Security.Mode = SecurityMode.Transport;
            _binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            EndpointAddress _endpointaddress = new EndpointAddress("net.tcp://localhost:" + _portno.ToString() + "/controller");
            this._channelfactory = new ChannelFactory<IUplinkController_>(_binding, _endpointaddress);
            this._controller = _channelfactory.CreateChannel();
            Process _myprocess = Process.GetCurrentProcess();
            Random _random = new Random();
            this._id = DateTime.Now.ToString("yyyyMMddHHmmssfffffff") + _random.Next(1000000).ToString("000000") +
                _random.Next(1000000).ToString("000000") + _myprocess.Id.ToString("0000000");
            StringBuilder _ss = new StringBuilder();
            _ss.Append("[");
            _ss.Append(_myprocess.Id.ToString());
            _ss.Append("] ");
            _ss.Append(Environment.CommandLine);
            this._name = _ss.ToString();
            this._controller._Connect(this._id, this._name);
            ManualResetEvent _connected = new ManualResetEvent(false);
            _core.Schedule(
                new QS.Fx.Base.Event<ManualResetEvent>(
                    new QS.Fx.Base.ContextCallback<ManualResetEvent>(this._ConnectCallback), _connected));
            _connected.WaitOne();
        }

        #endregion

        #region Destructor

        ~Uplink_()
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
                    ManualResetEvent _disconnected = new ManualResetEvent(false);
                    _core.Schedule(
                        new QS.Fx.Base.Event<ManualResetEvent>(
                            new QS.Fx.Base.ContextCallback<ManualResetEvent>(this._DisconnectCallback), _disconnected));
                    _disconnected.WaitOne();
                    this._controller._Disconnect(this._id);
                    _channelfactory.Close();
                }
            }
        }

        #endregion

        #region Fields

        private QS._core_c_.Core.ICore _core;
        private int _disposed, _portno;
        private ChannelFactory<IUplinkController_> _channelfactory;
        private IUplinkController_ _controller;
        private string _id, _name;
        private QS._core_c_.Core.IChannel _channel, _channelclient;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IChannel Members

        void QS._core_c_.Core.IChannel.Handle(QS._core_c_.Core.ChannelObject _message)
        {
            this._channel.Handle(_message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ConnectCallback

        private void _ConnectCallback(ManualResetEvent _connected)
        {
            this._channel = _core.Open(this._id, this._name, this._channelclient);
            _connected.Set();
        }

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback(ManualResetEvent _disconnected)
        {
            _core.Close(this._id);
            _disconnected.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
