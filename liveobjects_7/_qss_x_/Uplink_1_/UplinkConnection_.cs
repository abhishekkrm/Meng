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
    public sealed class UplinkConnection_ : QS._core_c_.Core.IChannel, IDisposable,
        QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>,
        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>
    {
        #region Constructor

        public UplinkConnection_(
            QS.Fx.Object.IContext _mycontext,
            QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver, 
            UplinkController_ _controller)
        {
            this._myquicksilver = _myquicksilver;
            this._controller = _controller;
            this._endpoint = 
                _mycontext.DualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>>(this);
            this._endpoint.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
        }

        #endregion

        #region Destructor

        ~UplinkConnection_()
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

        private QS._qss_x_.QuickSilver_.QuickSilver_ _myquicksilver;
        private int _disposed;
        private UplinkController_ _controller;
        private QS._core_c_.Core.IChannel _channelclient;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>,
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>> _endpoint;
        private Queue<UplinkObject_> _outgoing = new Queue<UplinkObject_>();
        private bool _outgoingconnected;
        private UplinkObject_ _lastincomingobject;
        private Stack<UplinkObject_> _incoming = new Stack<UplinkObject_>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICommunicationChannel<ISerializable> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS.Fx.Serialization.ISerializable>, 
            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>> 
        QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>._Channel
        {
            get { return this._endpoint; }
        }

        #endregion

        #region _Connect

        public void _Connect(QS._core_c_.Core.IChannel _channelclient)
        {
            this._channelclient = _channelclient;
        }

        #endregion

        #region _ConnectedCallback

        private void _ConnectedCallback()
        {
            this._outgoingconnected = true;
            this._myquicksilver._Core.Schedule(
                new QS.Fx.Base.Event<object>(
                    new QS.Fx.Base.ContextCallback<object>(this._ConnectedCallback), null));
        }

        private void _ConnectedCallback(object _o)
        {
            if (this._outgoingconnected)
            {
                while (this._outgoing.Count > 0)
                    this._endpoint.Interface._Receive(this._outgoing.Dequeue());
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IChannel Members

        void QS._core_c_.Core.IChannel.Handle(QS._core_c_.Core.ChannelObject _message)
        {
            if (this._outgoingconnected)
            {
                while (this._outgoing.Count > 0)
                    this._endpoint.Interface._Receive(this._outgoing.Dequeue());
                this._endpoint.Interface._Receive(new UplinkObject_(_message));
            }
            else
                this._outgoing.Enqueue(new UplinkObject_(_message));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICommunicationChannel<ISerializable> Members

        void QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS.Fx.Serialization.ISerializable>._Send(
            QS.Fx.Serialization.ISerializable _message)
        {
            UplinkObject_ _incomingobject = (UplinkObject_) _message;
            UplinkObject_ _o;
            do
            {
                _o = this._lastincomingobject;
                _incomingobject._link = _o;
            }
            while (!ReferenceEquals(Interlocked.CompareExchange<UplinkObject_>(ref this._lastincomingobject, _incomingobject, _o), _o));
            this._myquicksilver._Core.Schedule(
                new QS.Fx.Base.Event<object>(
                    new QS.Fx.Base.ContextCallback<object>(this._SendCallback), null));

        }

        private void _SendCallback(object _x)
        {
            UplinkObject_ _o = Interlocked.Exchange<UplinkObject_>(ref this._lastincomingobject, null);
            while (_o != null)
            {
                this._incoming.Push(_o);
                _o = _o._link;
            }
            while (this._incoming.Count > 0)
                this._channelclient.Handle(this._incoming.Pop()._channelobject);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
