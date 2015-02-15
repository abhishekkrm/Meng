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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TcpTransport, "TcpTransport")]
    public sealed class TcpTransport_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : 
        QS.Fx.Object.Classes.ITransport<string, MessageClass>,
        QS.Fx.Interface.Classes.ITransport<string, MessageClass>,
        IDisposable
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        unsafe public TcpTransport_([QS.Fx.Reflection.Parameter("Address", QS.Fx.Reflection.ParameterClass.Value)] string _address_specification)
        {
            lock (this)
            {
                this._transport_endpoint =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransportClient<string, MessageClass>,
                        QS.Fx.Interface.Classes.ITransport<string, MessageClass>>(this);
                this._transport_endpoint.OnConnect += new QS.Fx.Base.Callback(this._TransportEndpointConnect);
                this._transport_endpoint.OnConnected += new QS.Fx.Base.Callback(this._TransportEndpointConnected);
                this._transport_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._TransportEndpointDisconnect);
                if (_address_specification == null)
                    throw new Exception("Address cannot be null.");
                bool _random_port;
                int _separator = _address_specification.IndexOf(':');
                if (_separator >= 0 && _separator < _address_specification.Length)
                    this._port = Convert.ToInt32(_address_specification.Substring(_separator + 1));
                else
                {
                    this._port = 0;
                    _separator = _address_specification.Length;
                }
                _random_port = this._port == 0;
                QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(_address_specification.Substring(0, _separator));
                bool _found = false;
                foreach (IPAddress _ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                {
                    if (_ipaddress.AddressFamily == AddressFamily.InterNetwork && _subnet.contains(_ipaddress))
                    {
                        this._ipaddress = _ipaddress;
                        _found = true;
                        break;
                    }
                }
                if (!_found)
                    throw new Exception("Cannot find any network adapter connected to network " + _subnet.ToString() + ".");
                int _countdown = 1000;
                Random _random = new Random();
                do
                {
                    if (_random_port)
                        this._port = 50000 + _random.Next(10000);
                    try
                    {
                        this._mainsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        this._mainsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                        this._mainsocket.Bind(new IPEndPoint(this._ipaddress, this._port));
                        break;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            this._mainsocket.Close();
                        }
                        catch (Exception)
                        {
                        }
                        this._mainsocket = null;
                    }
                }
                while (!_random_port && --_countdown > 0);
                if (this._mainsocket == null)
                    throw new Exception("Cound not connect the main socket.");
                this._mainsocket.Listen(5);
/-----*
                ulong _u1;
                byte[] _bytes = this._ipaddress.GetAddressBytes();
                if (_bytes.Length != 4)
                    throw new Exception("Address is not 4 bytes long.");
                fixed (byte* _pbytes = _bytes)
                {
                    *((int*)&_u1) = _port;
                    ((byte*)&_u1)[4] = _pbytes[3];
                    ((byte*)&_u1)[5] = _pbytes[2];
                    ((byte*)&_u1)[6] = _pbytes[1];
                    ((byte*)&_u1)[7] = _pbytes[0];
                }
                ulong _u2 = (ulong)(DateTime.Now.Ticks - (new DateTime(2000, 1, 1)).Ticks);
                this._address = new QS.Fx.Base.Unsigned_128(_u1, _u2);
*-----/
                this._address = this._ipaddress + ":" + this._port;
                this._mainsocket.BeginAccept(this._AcceptCallback, null);
            }
        }

        #endregion

        #region Destructor

        ~TcpTransport_()
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
                    this._Disconnect();
            }
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
            lock (this)
            {
                if (this._mainsocket != null)
                {
                    try
                    {
                        this._mainsocket.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
                this._mainsocket = null;
                this._disconnected = true;
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<string, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<string, MessageClass>>
                _transport_endpoint;
        private IPAddress _ipaddress;
        private int _port;
        private string _address;
        private Socket _mainsocket;
        private int _disposed;
        private bool _disconnected;
        private IDictionary<string, CommunicationChannel_> _channels = new Dictionary<string, CommunicationChannel_>();

        #endregion

        #region ITransport<string,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.ITransportClient<string, MessageClass>,
            QS.Fx.Interface.Classes.ITransport<string, MessageClass>>
                QS.Fx.Object.Classes.ITransport<string, MessageClass>.Transport
        {
            get { return this._transport_endpoint; }
        }

        #endregion

        #region _TransportEndpointConnect

        private void _TransportEndpointConnect()
        {
        }

        #endregion

        #region _TransportEndpointConnected

        private void _TransportEndpointConnected()
        {
            this._transport_endpoint.Interface.Address(this._address);
        }

        #endregion

        #region _TransportEndpointDisconnect

        private void _TransportEndpointDisconnect()
        {
        }

        #endregion

        #region _AcceptCallback

        private void _AcceptCallback(IAsyncResult _result)
        {
            lock (this)
            {
                if (!this._disconnected)
                {
                    try
                    {
                        Initialization_ _initialization = new Initialization_(this, this._mainsocket.EndAccept(_result));                        
                        this._mainsocket.BeginAccept(this._AcceptCallback, null);
                    }
                    catch (Exception _exc)
                    {
                        this._Disconnect();
                    }
                }
            }
        }

        #endregion

        #region ITransport<string,MessageClass> Members

        void QS.Fx.Interface.Classes.ITransport<string, MessageClass>.Connect(string _address)
        {
            lock (this)
            {
                CommunicationChannel_ _channel;
                if (!this._channels.TryGetValue(_address, out _channel))
                {
                    _channel = new CommunicationChannel_(this, _address);
                    this._channels.Add(_address, _channel);
                }
                _channel._Outgoing();
            }
        }

        #endregion

        #region Class Initialization_

        private sealed class Initialization_
        {
            public Initialization_(TcpTransport_<MessageClass> _transport, Socket _socket)
            {
                this._transport = _transport;
                this._socket = _socket;
                this._endpoint = (IPEndPoint) _socket.RemoteEndPoint;
                this._socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this._InitializationCallback), this);                        
            }

            private TcpTransport_<MessageClass> _transport;
            private Socket _socket;
            private IPEndPoint _endpoint;
            private byte[] _buffer = new byte[8];

            private void _InitializationCallback(IAsyncResult _result)
            {
                SocketError _errorcode;
                int _nreceived = this.socket.EndReceive(_result, out _errorcode);
                if ((_errorcode == SocketError.Success) && (_nreceived == _buffer.Length))
                {

                }
            }
        }

        #endregion

        #region Class CommunicationChannel_

        private sealed class CommunicationChannel_
        {
            #region Constructor

            public CommunicationChannel_(TcpTransport_<MessageClass> _transport, string _address)
            {
                this._transport = _transport;
                this._address = _address;
            }

            #endregion

            #region Fields

            private TcpTransport_<MessageClass> _transport;
            private string _address;
            private Socket _outgoingsocket;

            #endregion

            #region _Outgoing

            public void _Outgoing()
            {
                if (_outgoingsocket != null)
                    throw new Exception("Outgoing socket already exists.");
                this._outgoingsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _outgoingsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                _outgoingsocket.Bind(new IPEndPoint(this._transport._ipaddress, 0));
            }

            #endregion
        }

        #endregion
    }
*/
}
