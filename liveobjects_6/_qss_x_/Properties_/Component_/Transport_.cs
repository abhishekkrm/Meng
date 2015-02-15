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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    public abstract class Transport_<MessageClass, AddressClass, TransportClass, ChannelClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>
        where TransportClass : Transport_<MessageClass, AddressClass, TransportClass, ChannelClass>
        where ChannelClass : Transport_<MessageClass, AddressClass, TransportClass, ChannelClass>.CommunicationChannel_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Transport_
        (
            QS.Fx.Object.IContext _mycontext,
            bool _debug
        )
        : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_.Constructor");
#endif

            this._transport_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
                    QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>>(this);
            this._transport_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            this._transport_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
                QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>> _transport_endpoint;
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Address _address;
        [QS.Fx.Base.Inspectable]
        protected IDictionary<AddressClass, ChannelClass> _channels = new Dictionary<AddressClass, ChannelClass>();
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Base.Address> _connecting = new Queue<QS.Fx.Base.Address>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransport<AddressClass,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<QS.Fx.Base.Address, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>>
                QS.Fx.Object.Classes.ITransport<QS.Fx.Base.Address, MessageClass>.Transport
        {
            get { return this._transport_endpoint; }
        }

        #endregion

        #region ITransport<QS.Fx.Base.Address,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransport<QS.Fx.Base.Address, MessageClass>.Connect(QS.Fx.Base.Address _address)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Address>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._ConnectChannel), _address));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Dispose");
#endif

            lock (this)
            {
                if (this._transport_endpoint.IsConnected)
                    this._transport_endpoint.Disconnect();
                foreach (ChannelClass _channel in this._channels.Values)
                    ((IDisposable) _channel).Dispose();

                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Start");
#endif

            base._Start();
        }

        protected void _Start(QS.Fx.Base.Address _address)
        {
            lock (this)
            {
                this._address = _address;
                this._initialized = true;
                if (this._transport_endpoint.IsConnected)
                {
                    this._notified = true;
                    this._transport_endpoint.Interface.Address(_address);
                }
                this._ConnectChannel();
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Stop");
#endif

            lock (this)
            {
                this._initialized = false;

                base._Stop();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Connect ");
#endif

            lock (this)
            {
                if (this._initialized && !this._notified && this._transport_endpoint.IsConnected)
                {
                    this._notified = true;
                    this._transport_endpoint.Interface.Address(_address);
                }
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Disconnect");
#endif

            lock (this)
            {
                this._notified = false;
            }
        }

        #endregion

        #region _ConnectChannel

        private void _ConnectChannel(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Base.Address _address = ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Address>) _event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._ConnectChannel " + QS.Fx.Printing.Printable.ToString(_address));
#endif

            lock (this)
            {
                this._connecting.Enqueue(_address);
                if (this._initialized)
                    _ConnectChannel();               
            }
        }

        private void _ConnectChannel()
        {
            while (this._connecting.Count > 0)
            {
                QS.Fx.Base.Address _address = this._connecting.Dequeue();
                if (this._transport_endpoint.IsConnected)
                {
                    AddressClass _networkaddress = _Address(_address);
                    ChannelClass _channel;
                    if (!this._channels.TryGetValue(_networkaddress, out _channel))
                    {
                        _channel = _Channel(_networkaddress);
                        this._channels.Add(_networkaddress, _channel);
                        if ((this._platform != null) && (_channel is QS._qss_x_.Platform_.IApplication))
                            ((QS._qss_x_.Platform_.IApplication) _channel).Start(this._platform, null);
                    }
                    this._transport_endpoint.Interface.Connected(_address, _channel._Reference);
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.Transport_._ConnectChannel : IGNORING " + _address.ToString());
#endif
                }
            }
        }

        #endregion

        #region _Address

        protected abstract AddressClass _Address(QS.Fx.Base.Address _address);

        #endregion

        #region _Channel

        protected abstract ChannelClass _Channel(AddressClass _networkaddress);

        #endregion

        #region _Receive

        protected void _Receive(AddressClass _networkaddress, MessageClass _message)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<AddressClass, MessageClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Receive),
                    _networkaddress,
                    _message));
        }

        private void _Receive(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<AddressClass, MessageClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<AddressClass, MessageClass>)_event;
            AddressClass _networkaddress = _event_._Object1;
            MessageClass _message = _event_._Object2;

#if INFO
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Receive " + _networkaddress.ToString() + "\n\n" + QS.Fx.Printing.Printable.ToString(_message) + "\n\n");
#endif

            lock (this)
            {
                ChannelClass _channel;
                if (!this._channels.TryGetValue(_networkaddress, out _channel))
                {
                    _channel = this._Channel(_networkaddress);
                    this._channels.Add(_networkaddress, _channel);
                    if ((this._platform != null) && (_channel is QS._qss_x_.Platform_.IApplication))
                        ((QS._qss_x_.Platform_.IApplication)_channel).Start(this._platform, null);
                    this._transport_endpoint.Interface.Connected(new QS.Fx.Base.Address(_networkaddress.ToString()), _channel._Reference);
                }
                _channel._Incoming(_message);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        #region _Connected

        protected void _Connected(AddressClass _networkaddress, ChannelClass _channel)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<AddressClass,ChannelClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connected),
                    _networkaddress,
                    _channel));
        }

        private void _Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<AddressClass,ChannelClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<AddressClass,ChannelClass>)_event;
            AddressClass _networkaddress = _event_._Object1;
            ChannelClass _channel= _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Transport_._Connected " + _networkaddress.ToString() + "\n\n" + QS.Fx.Printing.Printable.ToString(_channel) + "\n\n");
#endif

            lock (this)
            {
                //ChannelClass _existing_channel;
                if (!this._channels.ContainsKey(_networkaddress))
                {
                    this._channels.Add(_networkaddress, _channel);
                    if ((this._platform != null) && (_channel is QS._qss_x_.Platform_.IApplication))
                        ((QS._qss_x_.Platform_.IApplication)_channel).Start(this._platform, null);
                    this._transport_endpoint.Interface.Connected(new QS.Fx.Base.Address(_networkaddress.ToString()), _channel._Reference);
                }
                else
                {
                    throw new Exception("Component_.Transport_._Connected: Found an existing channel for this address");
                }
 
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class CommunicationChannel_

        public abstract class CommunicationChannel_
            : QS._qss_x_.Properties_.Component_.Base_, 
            QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>,
            QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Constructor

            public CommunicationChannel_(QS.Fx.Object.IContext _mycontext, TransportClass _transport, AddressClass _networkaddress) 
                : base(_mycontext, _transport._debug)
            {
                this._transport = _transport;
                this._networkaddress = _networkaddress;
                this._id = _networkaddress.ToString();
                this._attributes = new QS.Fx.Attributes.Attributes(this);
                this._objectclass = QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>();
                this._endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>,
                    QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>>(this);
                this._endpoint.OnConnected +=
                    new QS.Fx.Base.Callback(
                        delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
                this._endpoint.OnDisconnect +=
                    new QS.Fx.Base.Callback(
                        delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            protected TransportClass _transport;
            [QS.Fx.Base.Inspectable]
            protected AddressClass _networkaddress;

            [QS.Fx.Base.Inspectable]
            private string _id;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Attributes.IAttributes _attributes;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Reflection.IObjectClass _objectclass;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>, 
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> _endpoint;
            [QS.Fx.Base.Inspectable]
            private bool _initialized;
            [QS.Fx.Base.Inspectable]
            private bool _connected;
            [QS.Fx.Base.Inspectable]
            private bool _sending;
            [QS.Fx.Base.Inspectable]
            private bool _receiving;
            [QS.Fx.Base.Inspectable]
            private Queue<MessageClass> _outgoing = new Queue<MessageClass>();
            [QS.Fx.Base.Inspectable]
            private Queue<MessageClass> _incoming = new Queue<MessageClass>();

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Reference

            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>> _Reference
            {
                get
                {
                    return 
                        QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>>.Create(
                            this, this._id, this._attributes, this._objectclass);
                }
            }

            #endregion

            #region ICommunicationChannel<MessageClass> Members

            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>, 
                QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>> 
                    QS.Fx.Object.Classes.ICommunicationChannel<MessageClass>.Channel
            {
                get { return this._endpoint; }
            }

            #endregion

            #region ICommunicationChannel<MessageClass> Members

            void QS.Fx.Interface.Classes.ICommunicationChannel<MessageClass>.Message(MessageClass _message)
            {
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<MessageClass>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Outgoing), _message));
            }

            #endregion

            #region _Incoming

            public void _Incoming(MessageClass _message)
            {
                this._Enqueue(
                    new QS._qss_x_.Properties_.Base_.Event_<MessageClass>(
                        new QS._qss_x_.Properties_.Base_.EventCallback_(this._Incoming), _message));
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Initialize

            protected override void _Initialize()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Initialize");
#endif

                base._Initialize();
            }

            #endregion

            #region _Dispose

            protected override void _Dispose()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Dispose");
#endif

                if (this._endpoint.IsConnected)
                    this._endpoint.Disconnect();

                base._Dispose();
            }

            #endregion

            #region _Start

            protected override void _Start()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Start");
#endif

                base._Start();

                lock (this)
                {
                    this._initialized = true;
                    if (this._sending)
                        this._Outgoing();
                }
            }

            #endregion

            #region _Stop

            protected override void _Stop()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Stop");
#endif

                base._Stop();
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Connect

            private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Connect");
#endif

                lock (this)
                {
                    this._connected = true;
                    if (this._receiving)
                        this._Incoming();
                }
            }

            #endregion

            #region _Disconnect

            private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Disconnect");
#endif

                lock (this)
                {
                    this._connected = false;
                    this._incoming.Clear();
                }
            }

            #endregion

            #region _Outgoing

            private void _Outgoing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                MessageClass _message = ((QS._qss_x_.Properties_.Base_.IEvent_<MessageClass>) _event)._Object;

#if INFO
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Outgoing " + QS.Fx.Printing.Printable.ToString(_message));
#endif

                lock (this)
                {
                    this._sending = true;
                    this._outgoing.Enqueue(_message);
                    if (this._initialized)
                        this._Outgoing();
                }
            }

            private void _Outgoing()
            {
                while (this._outgoing.Count > 0)
                {
                    MessageClass _message = this._outgoing.Dequeue();
                    this._Outgoing(_message);
                }
            }

            protected abstract void _Outgoing(MessageClass _message);

            #endregion

            #region _Incoming

            private void _Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
            {
                MessageClass _message = ((QS._qss_x_.Properties_.Base_.IEvent_<MessageClass>) _event)._Object;

#if INFO
                if (this._logger != null)
                    this._logger.Log("Component_.Transport_.CommunicationChannel_._Incoming " + QS.Fx.Printing.Printable.ToString(_message));
#endif

                lock (this)
                {
                    this._receiving = true;
                    this._incoming.Enqueue(_message);
                    if (this._connected)
                        this._Incoming();
                }
            }

            private void _Incoming()
            {
                if (this._endpoint.IsConnected)
                {
                    while (this._incoming.Count > 0)
                        this._endpoint.Interface.Message(_incoming.Dequeue());
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
