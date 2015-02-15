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

#define GRADIENT
#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass("802BA69E43AB4689B3BC45FB50E492A0", "TcpTransport2_")]
    public sealed class TcpTransport2_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Transport_<
            MessageClass, QS.Fx.Network.NetworkAddress, TcpTransport2_<MessageClass>, TcpTransport2_<MessageClass>.CommunicationChannel_>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor
        public TcpTransport2_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)]
            string _address,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TcpTransport2_.Constructor " + _address);
#endif

            int _i = _address.IndexOf(':');
            if (_i >= 0)
            {
                this._subnet = new QS._qss_c_.Base1_.Subnet(_address.Substring(0, _i));
                this._port = Convert.ToInt32(_address.Substring(_i + 1));
            }
            else
            {
                this._subnet = new QS._qss_c_.Base1_.Subnet(_address);
                Random r = new Random();
                this._port = 20000 + r.Next(20000);
            }
            if (_address.IndexOf('/') >= 0 && _address.Substring(0, _address.IndexOf('/')) == "127.0.0.1")
            {
                _use_localhost = true;
            }

            lock (this)
            {
                this._hostname = _platform.Network.GetHostName();
                this._networkinterface = null;
                foreach (QS.Fx.Network.INetworkInterface _nic in _platform.Network.Interfaces)
                {
                    if (_nic.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        if (this._subnet.contains(_nic.InterfaceAddress))
                        {
                            this._networkinterface = _nic;
                            break;
                        }
                    }
                }
                if (this._networkinterface == null)
                    throw new Exception("Could not locate any network adapter on the requested subnet " + this._subnet.ToString() + ".");

                IPAddress _listen_ip;
                if (_use_localhost)
                {
                    _listen_ip = IPAddress.Any;
                }
                else
                {
                    _listen_ip = this._networkinterface.InterfaceAddress;
                }

                try
                {
                    this._transport_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this._transport_socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    this._transport_socket.Bind(new IPEndPoint(_listen_ip, this._port));

#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.TcpTransport2_.Constructor bind on" + _listen_ip);
#endif
                }
                catch (Exception)
                {
                    throw new Exception("Failed to setup socket.");
                }
                this._transport_socket.Listen(5);

                this._transport_socket.BeginAccept(this._AcceptCallback, null);

#if GRADIENT
                // Not bootstrap
                //if (_port < 12000)
                {
                    string nid = System.Net.Dns.GetHostName();

                    if (_listen_ip.ToString().Contains("10.1."))
                    {
                        StreamReader map = new StreamReader("c:/map.txt");
                        string line;
                        Dictionary<string, string> map_dict = new Dictionary<string, string>();
                        while (null != (line = map.ReadLine()))
                        {
                            char[] set = { ' ' };
                            string[] elems = line.Split(set, StringSplitOptions.RemoveEmptyEntries);

                            map_dict.Add(elems[3], elems[0]);
                        }
                        map.Close();

                        this._id = map_dict[nid];
                    }
                    else
                    {
                        this._id = _port.ToString();
                    }

                    this._in_writer = new StreamWriter("c:/" + this._id + "_" + this._port + "_gradient_net_in.log");
                    this._out_writer = new StreamWriter("c:/" + this._id + "_" + this._port + "_gradient_net_out.log");
                }
#endif

                //this._listener =
                //    this._networkinterface.Listen(
                //        new QS._core_x_.Network.NetworkAddress(this._networkinterface.InterfaceAddress, this._port),
                //        new QS._core_x_.Network.ReceiveCallback(this._ReceiveCallback), null);
                this._networkaddress = new QS.Fx.Network.NetworkAddress(_listen_ip, this._port);
                this._port = this._networkaddress.PortNumber;

                this._Start(new QS.Fx.Base.Address(_listen_ip, this._port));
            }
        }

        #endregion

        #region Fields


        [QS.Fx.Base.Inspectable]
        private bool _use_localhost = false;
        [QS.Fx.Base.Inspectable]
        private string _hostname;
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base1_.Subnet _subnet;
        [QS.Fx.Base.Inspectable]
        private int _port;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.INetworkInterface _networkinterface;
        [QS.Fx.Base.Inspectable]
        private Socket _transport_socket;
        //private QS._core_x_.Network.IListener _listener;
        private IDictionary<string, SocketHandler_> sockets = new Dictionary<string, SocketHandler_>();
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.NetworkAddress _networkaddress;
        //[QS.Fx.Base.Inspectable]
        //private IDictionary<string, CommunicationChannel_> channels = new Dictionary<string, CommunicationChannel_>();

#if GRADIENT
        // Only for gradient
        public StreamWriter _in_writer;
        public StreamWriter _out_writer;
        public string _id;
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TcpTransport2_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TcpTransport2_._Dispose");
#endif

            lock (this.sockets)
            {
                foreach (SocketHandler_ s in sockets.Values)
                {
                    s._DisconnectCallback();

                }

                _transport_socket.Close(0);
                _transport_socket = null;

                base._Dispose();
            }

#if GRADIENT
            this._in_writer.Close();
            this._out_writer.Close();
#endif
        }

        #endregion

        #region _Start

        protected override void _Start()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TcpTransport2_._Start");
#endif

            lock (this)
            {
                base._Start();
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.TcpTransport2_._Stop");
#endif

            lock (this)
            {
                if (this._transport_socket != null)
                {
                    this._transport_socket.Close(0);
                    this._transport_socket = null;
                }
                base._Stop();
            }
        }

        #endregion

        #region _AcceptCallback

        private void _AcceptCallback(IAsyncResult _result)
        {
            lock (this)
            {
                if (this._transport_socket != null)
                {
                    try
                    {
                        Socket tmp = this._transport_socket.EndAccept(_result);
                        //tmp.SendBufferSize = 32 * 1024;
                        //tmp.ReceiveBufferSize = 32 * 1024;
                        sockets.Add(tmp.RemoteEndPoint.ToString(), new SocketHandler_(tmp, this));
                        QS.Fx.Network.NetworkAddress _addr;
                        _addr = new QS.Fx.Network.NetworkAddress(((System.Net.IPEndPoint)tmp.RemoteEndPoint).Address, ((System.Net.IPEndPoint)tmp.RemoteEndPoint).Port);

                        //lock (this.channels)
                        //{
                        //    if (this.channels.ContainsKey(_addr.ToString()))
                        //    {
                        //        throw new Exception("Got a new socket for a channel we already have");
                        //    }
                        //}

                        CommunicationChannel_ _tmp = this._Channel(_addr);
                        base._Connected(_addr, _tmp);
                        this._transport_socket.BeginAccept(this._AcceptCallback, null);



                    }
                    catch (SocketException se)
                    {
                        _mycontext.Platform.Logger.Log("Caught socket exception [3]: " + se.ToString());

#if VERBOSE
                        if (_in_writer != null)
                        {
                            _in_writer.WriteLine("Caught exception [3]: " + se.ToString());
                        }
#endif

                        // If accepted connection is closed by remote host, just continue
                        if (se.ErrorCode == 10054)
                        {
                            this._transport_socket.BeginAccept(this._AcceptCallback, null);
                        }
                    }
                    catch (Exception e)
                    {
                        _mycontext.Platform.Logger.Log("Caught exception [3]: " + e.ToString());

#if VERBOSE
                        if (_in_writer != null)
                        {
                            _in_writer.WriteLine("Caught exception [3]: " + e.ToString());
                        }
#endif

                        this._Dispose();
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Address

        protected override QS.Fx.Network.NetworkAddress _Address(QS.Fx.Base.Address _address)
        {
            string[] _x = _address.String.Split(':');
            if (_x.Length != 2)
                throw new Exception("Bad address format.");

            string _hostname = _x[0];
            int _port = Convert.ToInt32(_x[1]);




            System.Net.IPAddress _ipaddress = System.Net.IPAddress.None;

            // Added in case hostname is an IP
            string[] segs = _hostname.Split('.');
            if (segs.Length == 4 && System.Text.RegularExpressions.Regex.IsMatch(segs[3], @"^\d+$"))
            {
                _ipaddress = IPAddress.Parse(_hostname);
                return new QS.Fx.Network.NetworkAddress(_ipaddress, _port);
            }


            bool _found = false;
            foreach (System.Net.IPAddress _some_ipaddress in this._platform.Network.GetHostEntry(_hostname).AddressList)
            {
                if ((_some_ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork))// && this._subnet.contains(_some_ipaddress))
                {
                    _ipaddress = _some_ipaddress;
                    _found = true;
                    break;
                }
            }
            if (!_found)
                throw new Exception("Cannot locate host \"" + _hostname + "\" on subnet " + this._subnet.ToString() + ".");

            return new QS.Fx.Network.NetworkAddress(_ipaddress, _port);

        }

        #endregion

        #region _Channel

        /// <summary>
        /// Return a channel based on the network address, or create a new one
        /// which is not in the dictionary yet
        /// </summary>
        /// <param name="_networkaddress"></param>
        /// <returns></returns>
        protected override CommunicationChannel_ _Channel(QS.Fx.Network.NetworkAddress _networkaddress)
        {
            lock (this._channels)
            {
                CommunicationChannel_ tmp;
                if (!_channels.TryGetValue(_networkaddress, out tmp))
                {
                    tmp = new CommunicationChannel_(this, _networkaddress);
                    //_channels.Add(_networkaddress, tmp);
                }
                return tmp;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        #region Class SocketHandler_

        public sealed class SocketHandler_
        {
            #region Constructor

            public SocketHandler_(Socket _socket, TcpTransport2_<MessageClass> _transport)
            {
                if (_socket != null)
                {
                    this._socket = _socket;
                    _endpoint = (IPEndPoint)_socket.RemoteEndPoint;
                }

                incomingheader = BitConverter.GetBytes((int)0);
                incomingheader2 = new byte[sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)];
                this._transport = _transport;
                this.incomingcount = 0;
                this.incomingsize = incomingheader.Length;
                this._initialized = true;
                this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);


            }

            public SocketHandler_(QS.Fx.Network.NetworkAddress address, TcpTransport2_<MessageClass> _transport)
            {
                try
                {
                    incomingheader = BitConverter.GetBytes((int)0);
                    incomingheader2 = new byte[sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)];
                    this._transport = _transport;
                    this._endpoint = new IPEndPoint(address.HostIPAddress, address.PortNumber);
                    this.incomingcount = 0;
                    this.incomingsize = incomingheader.Length;
                    this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    //if (address.HostIPAddress == IPAddress.Parse("127.0.0.1"))
                    //{
                    //    this._socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), this._transport._port));
                    //}
                    //else
                    //{

                    // is this an illegitimate use of Bind?  Why always bind to the same IP?


                    this._socket.Bind(new IPEndPoint(this._transport._networkaddress.HostIPAddress, this._transport._port));
                    //}
                    this._socket.BeginConnect(new IPEndPoint(address.HostIPAddress, address.PortNumber), new AsyncCallback(this._ConnectCallback), null);

                    //this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                }
                catch (Exception e)
                {
                    _transport._mycontext.Platform.Logger.Log("Caught exception [4]: " + e.ToString());
                    this._DisconnectCallback();
                }
            }

            #endregion

            #region Fields

            TcpTransport2_<MessageClass> _transport;
            IPEndPoint _endpoint;
            Socket _socket;
            private int incomingcount, incomingsize;
            private byte[] incomingheader, incomingheader2, incomingpacket;
            private int incoming_seqno;
            private long connectionid = 0;
            private Queue<QS.Fx.Serialization.ISerializable> incoming = new Queue<QS.Fx.Serialization.ISerializable>();

            private int outgoingcount, outgoingsize;
            private IList<ArraySegment<byte>> outgoingpacket;
            private int outgoing_seqno;
            private Queue<QS.Fx.Serialization.ISerializable> outgoing = new Queue<QS.Fx.Serialization.ISerializable>();
            private bool _initialized = false;

            #endregion


            #region _ConnectCallback


            private void _ConnectCallback(IAsyncResult _result)
            {
                SocketError _errorcode;
                try
                {

                    this._socket.EndConnect(_result);

                }
                catch (SocketException e)
                {
                    _transport._mycontext.Platform.Logger.Log("Caught exception [5]: " + e.ToString());

                    this._socket = null;

                    return;
                    // connect fails

                    // maybe remove the channel from dictionary here
                    // alternative is to allow them to call connect again
                    // either way they can just call connect again to try, though they never know a connect attempt fails, 
                    // perhaps give them a channel and immediately disconnect it or something
                }
                if (this._socket.Connected)
                {
                    //this._socket.SendBufferSize = 32 * 1024;
                    //this._socket.ReceiveBufferSize = 32 * 1024;
                    this.incomingcount = 0;
                    this.incomingsize = incomingheader.Length;

                    this._initialized = true;
                    lock (this.outgoing)
                    {
                        while (this.outgoing.Count > 0)
                        {
                            this._SerializeAndSend();

                        }
                    }

                    if (this._socket != null)
                    {
                        this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                    }
                    //_socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), this);
                }

            }

            #endregion

            #region _DisconnectCallback

            public void _DisconnectCallback()
            {
                lock (this)
                {
                    CommunicationChannel_ tmp;
                    QS.Fx.Network.NetworkAddress addr = new QS.Fx.Network.NetworkAddress(_endpoint.Address, _endpoint.Port);
                    if (this._transport._channels.TryGetValue(addr, out tmp))
                    {

                        tmp.Disconnect();
                        this._transport._channels.Remove(addr);
                    }
                    if (_socket != null)
                    {
                        //_socket.LingerState = new LingerOption(true, 0);
                        //_socket.Shutdown(SocketShutdown.Both);
                        _socket.Close(0);

                        _socket = null;

                    }
                    this._transport.sockets.Remove(this._endpoint.ToString());
                }
            }
            #endregion


            #region _ReceiveCallback

            private void _ReceiveCallback(IAsyncResult result)
            {
                try
                {
                    lock (this.incoming)
                    {
                        SocketError _errorcode;
                        int _nreceived;

                        lock (this._socket)
                        {
                            _nreceived = this._socket.EndReceive(result, out _errorcode);
                        }
                        if (_errorcode != SocketError.Success)
                            throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                        if (_nreceived < 0)
                            throw new Exception("Received less than zero bytes?!");
                        this.incomingcount += _nreceived;
                        if (this.incomingcount < this.incomingsize)
                        {
                            if (this.incomingheader != null && this.incomingcount < this.incomingheader.Length && this.incomingsize <= this.incomingheader.Length)
                            {
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(this.incomingheader, this.incomingcount, this.incomingsize - this.incomingcount,
                                        SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                                }
                            }
                            else
                                throw new Exception("Receive buffer is null, or the size of it does not match the expected size of the incoming packet.");
                        }
                        else
                        {
                            this.incomingcount = 0;
                            this.incomingsize = BitConverter.ToInt32(this.incomingheader, 0);
                            if (this.incomingsize > 0)
                            {
                                incomingpacket = new byte[this.incomingsize];
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(incomingpacket, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                                }
                            }
                            else
                                throw new Exception("Could not receive, the initial header indicates that the message is empty, disconnecting the channel.");
                        }
                    }
                }
                catch (Exception exc)
                {
                    _transport._mycontext.Platform.Logger.Log("Caught exception [6]: " + exc.ToString());

#if VERBOSE
                    if (_transport._in_writer != null)
                    {
                        _transport._in_writer.WriteLine("Caught exception [6]: " + exc.ToString());
                    }
#endif

                    this._DisconnectCallback();
                    //try
                    //{
                    //    throw new Exception("Caught exception receiving, disconnecting.\r\n" + exc.ToString());
                    //}
                    //catch (Exception)
                    //{
                    //}
                }
            }

            #endregion

            #region _ReceiveCallback_2

            private unsafe void _ReceiveCallback_2(IAsyncResult result)
            {
                try
                {
                    lock (this.incoming)
                    {
                        SocketError _errorcode;
                        int _nreceived;
                        lock (this._socket)
                        {

                            _nreceived = this._socket.EndReceive(result, out _errorcode);
                        }
                        if (_errorcode != SocketError.Success)
                            throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                        if (_nreceived < 0)
                            throw new Exception("Received less than zero bytes?!");
                        this.incomingcount += _nreceived;
                        if (this.incomingcount < this.incomingsize)
                        {
                            if (this.incomingpacket != null && this.incomingcount < this.incomingpacket.Length && this.incomingsize <= this.incomingpacket.Length)
                            {
                                lock (this._socket)
                                {
                                    this._socket.BeginReceive(this.incomingpacket, this.incomingcount, this.incomingsize - this.incomingcount,
                                        SocketFlags.None, new AsyncCallback(this._ReceiveCallback_2), null);
                                }
                            }
                            else
                                throw new Exception("Receive buffer is null, or the size of it does not match the expected size of the incoming packet.");
                        }
                        else
                        {
                            MemoryStream memorystream = new MemoryStream(this.incomingpacket);
                            this.incomingpacket = null;
                            Stream _ins;
                            _ins = memorystream;

                            int _ndecrypted = 0;
                            while (_ndecrypted < incomingheader2.Length)
                            {
                                int _ndecrypted_now = _ins.Read(incomingheader2, _ndecrypted, incomingheader2.Length - _ndecrypted);
                                if (_ndecrypted_now < 0)
                                    throw new Exception("Decrypted less than zero bytes?!");
                                _ndecrypted += _ndecrypted_now;
                            }
                            long _incoming_header_connectionid;
                            int _incoming_header_seqno;
                            ushort _incoming_header_classid;
                            uint _incoming_header_headersize, _incoming_header_messagesize;
                            fixed (byte* headerptr = incomingheader2)
                            {
                                _incoming_header_connectionid = *((long*)(headerptr));
                                _incoming_header_seqno = (int)(*((uint*)(headerptr + sizeof(long))));
                                _incoming_header_classid = *((ushort*)(headerptr + sizeof(long) + sizeof(uint)));
                                _incoming_header_headersize = *((uint*)(headerptr + sizeof(long) + sizeof(uint) + sizeof(ushort)));
                                _incoming_header_messagesize = *((uint*)(headerptr + sizeof(long) + 2 * sizeof(uint) + sizeof(ushort)));
                            }
                            if (_incoming_header_connectionid != this.connectionid)
                                throw new Exception("Cannot receive, the received packet has connection id = " + _incoming_header_connectionid.ToString() +
                                    ", which is different from the local connectionid = " + this.connectionid.ToString() + " from " + _socket.RemoteEndPoint.ToString() +
                                " with sequence number " + _incoming_header_seqno + "and class id " + _incoming_header_classid);
                            this.incoming_seqno++;
                            if (_incoming_header_seqno != this.incoming_seqno)
                                throw new Exception("Cannot receive, the received packet has seqno = " + _incoming_header_seqno.ToString() +
                                    ", which is different from the local seqno = " + this.incoming_seqno.ToString());
                            // int length = (int)cryptostream.Length;
                            // if (_incoming_header_messagesize != length)
                            //    throw new Exception("Cannot receive, the encrypted stream has " + length.ToString() +
                            //        " bytes, but according to the header, the message was supposed to have " + _incoming_header_messagesize.ToString() + " bytes.");
                            byte[] _packet = new byte[_incoming_header_messagesize];
                            _ins.Read(_packet, 0, (int)_incoming_header_messagesize);

                            QS.Fx.Serialization.ISerializable message = QS._core_c_.Base3.Serializer.CreateObject(_incoming_header_classid);
                            QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock(_packet, 0, _incoming_header_headersize);
                            QS.Fx.Base.ConsumableBlock data = new QS.Fx.Base.ConsumableBlock(
                                _packet, _incoming_header_headersize, _incoming_header_messagesize - _incoming_header_headersize);
                            try
                            {
                                message.DeserializeFrom(ref header, ref data);
                            }
                            catch (Exception _exc)
                            {
                                throw new Exception("Could not deserialize the incoming message.", _exc);
                            }
                            //bool _this_locked = true;
                            if (!(message is MessageClass))
                                throw new Exception("Received a message of an unexpected type.");
#if INFO
                            if (this._transport._logger != null)
                                this._transport._logger.Log("Component_.TcpTransport2_._ReceiveCallback2 " + this._endpoint.ToString() + "\n\n" + QS.Fx.Printing.Printable.ToString(message) + "\n\n");
#endif

#if GRADIENT
                            double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            if (message.SerializableInfo.ClassID == (ushort)(QS.ClassID.TransmitterMsg))
                            {
                                if (((Quilt.Transmitter.TransmitterMsg)message).Message.SerializableInfo.ClassID == (ushort)(QS.ClassID.GradientData))
                                {
                                    Quilt.Multicast.GradientData rc_data = (Quilt.Multicast.GradientData)(((Quilt.Transmitter.TransmitterMsg)message).Message);
                                    this._transport._in_writer.WriteLine(cur + "\t" + rc_data._stream.String + "\t" + rc_data._data._serial_no + "\tIN");
                                    this._transport._in_writer.Flush();
                                }
                            }
                            //else if (((Quilt.Transmitter.TransmitterMsg)message).Message.SerializableInfo.ClassID == (ushort)(QS.ClassID.BootstrapMembership))
                            //{
                            //    Quilt.Bootstrap.BootstrapMembership memship = (Quilt.Bootstrap.BootstrapMembership)(((Quilt.Transmitter.TransmitterMsg)message).Message);
                            //    this._transport._in_writer.WriteLine(cur + "\tGroup\t" + memship.GroupName.String + "\tIN");
                            //    this._transport._in_writer.Flush();
                            //}
#endif

#if VERBOSE
                            //if (this._transport._logger != null)
                            //{
                            //    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                            //    this._transport._logger.Log(cur + "TCP Transport IN");
                            //}
#endif

                            this._transport._Receive(new QS.Fx.Network.NetworkAddress(this._endpoint.Address, (int)this._endpoint.Port), (MessageClass)message);

                            this.incomingcount = 0;
                            this.incomingsize = this.incomingheader.Length;
                            lock (this._socket)
                            {
                                this._socket.BeginReceive(this.incomingheader, 0, this.incomingsize, SocketFlags.None, new AsyncCallback(this._ReceiveCallback), null);
                            }

                        }
                    }
                }
                catch (Exception exc)
                {
                    _transport._mycontext.Platform.Logger.Log("Caught exception [1]: " + exc.ToString());

#if VERBOSE
                    if (_transport._in_writer != null)
                    {
                        _transport._in_writer.WriteLine("Caught exception [1]: " + exc.ToString());
                    }
#endif

                    this._DisconnectCallback();
                    //try
                    //{
                    //    throw new Exception("Caught exception trying to receive2, disconnecting.\r\n" + exc.ToString());
                    //}
                    //catch (Exception)
                    //{
                    //}
                }
            }

            #endregion


            #region _SerializeAndSend

            private unsafe void _SerializeAndSend()
            {
                if (_initialized && this._socket != null && this._socket.Connected)
                {
                    try
                    {
                        if (this.connectionid != 0)
                        {
                            throw new Exception("Connection ID is invalid, for connection to " + this._socket.RemoteEndPoint.ToString());
                        }
                        QS.Fx.Serialization.ISerializable message = outgoing.Dequeue();
                        QS.Fx.Serialization.SerializableInfo info = message.SerializableInfo;
                        QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint)(info.HeaderSize + sizeof(long) + 3 * sizeof(uint) + sizeof(ushort)));
                        IList<QS.Fx.Base.Block> blocks = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
                        blocks.Add(header.Block);
                        fixed (byte* headerptr = header.Array)
                        {
                            *((long*)(headerptr)) = this.connectionid;
                            *((uint*)(headerptr + sizeof(long))) = (uint)++outgoing_seqno;
                            *((ushort*)(headerptr + sizeof(long) + sizeof(uint))) = info.ClassID;
                            *((uint*)(headerptr + sizeof(long) + sizeof(uint) + sizeof(ushort))) = (uint)info.HeaderSize;
                            *((uint*)(headerptr + sizeof(long) + 2 * sizeof(uint) + sizeof(ushort))) = (uint)info.Size;
                        }
                        header.consume(sizeof(long) + 3 * sizeof(uint) + sizeof(ushort));
                        message.SerializeTo(ref header, ref blocks);
                        MemoryStream memorystream = new MemoryStream();
                        Stream _outs;

                        _outs = memorystream;

                        foreach (QS.Fx.Base.Block _block in blocks)
                            _outs.Write(_block.buffer, (int)_block.offset, (int)_block.size);

                        byte[] _packetdata = memorystream.ToArray();
                        int length = (int)_packetdata.Length;
                        this.outgoingpacket = new List<ArraySegment<byte>>();
                        byte[] lengthbytes = BitConverter.GetBytes(length);
                        this.outgoingpacket.Add(new ArraySegment<byte>(lengthbytes));
                        this.outgoingpacket.Add(new ArraySegment<byte>(memorystream.ToArray()));
                        this.outgoingsize = lengthbytes.Length + length;
                        this.outgoingcount = 0;
#if INFO
                        if (this._transport._logger != null)
                            this._transport._logger.Log("Component_.TcpTransport2_._SerializeAndSend " + this._endpoint.ToString() + "\n\n" + QS.Fx.Printing.Printable.ToString(message) + "\n\n");
#endif

#if GRADIENT
                        double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        if (message.SerializableInfo.ClassID == (ushort)(QS.ClassID.TransmitterMsg))
                        {
                            if (((Quilt.Transmitter.TransmitterMsg)message).Message.SerializableInfo.ClassID == (ushort)(QS.ClassID.GradientData))
                            {
                                Quilt.Multicast.GradientData data = (Quilt.Multicast.GradientData)(((Quilt.Transmitter.TransmitterMsg)message).Message);
                                this._transport._out_writer.WriteLine(cur + "\t" + data._data._serial_no + "\tOUT");
                                this._transport._out_writer.Flush();
                            }
                            else if (((Quilt.Transmitter.TransmitterMsg)message).Message.SerializableInfo.ClassID == (ushort)(QS.ClassID.BootstrapMembership))
                            {
                                Quilt.Bootstrap.BootstrapMembership memship = (Quilt.Bootstrap.BootstrapMembership)(((Quilt.Transmitter.TransmitterMsg)message).Message);
                                this._transport._out_writer.WriteLine(cur + "\tGroup\t" + memship.GroupName.String + "\tOUT\t" + _socket.RemoteEndPoint.ToString());
                                this._transport._out_writer.Flush();
                            }
                        }
#endif

#if VERBOSE
                        //if (this._transport._logger != null)
                        //{
                        //    double cur = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                        //    this._transport._logger.Log(cur + "TCP Transport OUT");
                        //}
#endif

                        if (_socket != null)
                        {
#if VERBOSE
                            //this.
#endif
                            _socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
                        }

                    }
                    catch (Exception exc)
                    {
                        _transport._mycontext.Platform.Logger.Log("Caught exception [2]: " + exc.ToString());
#if VERBOSE
                        if (_transport._out_writer != null)
                        {
                            _transport._out_writer.WriteLine("Caught exception [2]: " + exc.ToString());
                        }
#endif

                        _DisconnectCallback();
                    }
                }

            }

            #endregion
            #region _SendCallback

            private void _SendCallback(IAsyncResult result)
            {
                try
                {
                    lock (this.outgoing)
                    {
                        SocketError _errorcode;
                        int _ntransmitted;
                        if (this._socket != null)
                        {
                            lock (this._socket)
                            {
                                _ntransmitted = this._socket.EndSend(result, out _errorcode);
                            }

                            if (_errorcode != SocketError.Success)
                                throw new Exception("Could not transmit, a socket error has occurred: \"" + _errorcode.ToString() + "\".");
                            if (_ntransmitted < 0)
                                throw new Exception("Transmitted less than zero bytes?!");
                            this.outgoingcount += _ntransmitted;
                            if (this.outgoingcount < this.outgoingsize)
                            {
                                try
                                {
                                    while (_ntransmitted >= this.outgoingpacket[0].Count)
                                    {
                                        _ntransmitted -= this.outgoingpacket[0].Count;
                                        this.outgoingpacket.RemoveAt(0);
                                    }
                                    this.outgoingpacket[0] = new ArraySegment<byte>(
                                        this.outgoingpacket[0].Array, this.outgoingpacket[0].Offset + _ntransmitted, this.outgoingpacket[0].Count - _ntransmitted);
                                }
                                catch (Exception exc)
                                {
                                    throw new Exception("SendCallback outgoing packet error" + exc.Message);
                                }

                                //if (this._socket != null)
                                {
                                    lock (this._socket)
                                    {
                                        _socket.BeginSend(this.outgoingpacket, SocketFlags.None, new AsyncCallback(this._SendCallback), null);
                                    }
                                }
                            }
                            else
                            {
                                if (outgoing.Count > 0)
                                    _SerializeAndSend();
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    _DisconnectCallback();
                    //throw new Exception("Caught exception trying to sendcallback, disconnecting.\r\n" + exc.ToString());

                }
            }

            #endregion


            public unsafe void Send(MessageClass message)
            {

                lock (this.outgoing)
                {
                    bool pending = this.outgoing.Count > 0;
                    this.outgoing.Enqueue(message);

                    if (!pending)
                    {
                        this._SerializeAndSend();
                    }
                }

            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class CommunicationChannel_

        public new sealed class CommunicationChannel_
            : QS._qss_x_.Properties_.Component_.Transport_<
                MessageClass, QS.Fx.Network.NetworkAddress, TcpTransport2_<MessageClass>, CommunicationChannel_>.CommunicationChannel_
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Constructor

            public CommunicationChannel_(TcpTransport2_<MessageClass> _transport, QS.Fx.Network.NetworkAddress _networkaddress)
                : base(_transport._mycontext, _transport, _networkaddress)
            {
                if (this._sender == null)
                {
                    SocketHandler_ sock;
                    if (this._transport.sockets.TryGetValue(this._networkaddress.ToString(), out sock))
                    {
                        this._sender = sock;

                    }
                    else
                    {
                        this._sender = new SocketHandler_(this._networkaddress, this._transport);
                        this._transport.sockets.Add(this._networkaddress.ToString(), this._sender);

                    }
                }
            }

            #endregion

            #region Fields


            [QS.Fx.Base.Inspectable]
            private SocketHandler_ _sender;
            [QS.Fx.Base.Inspectable]
            private bool _disposed = false;

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            public void Disconnect()
            {
                this._Dispose();
            }

            #region _Initialize

            protected override void _Initialize()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.TcpTransport2_.CommunicationChannel_._Initialize");
#endif

                base._Initialize();
            }

            #endregion

            #region _Dispose

            protected override void _Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.TcpTransport2_.CommunicationChannel_._Dispose");
#endif

                    base._Dispose();
                }
            }

            #endregion

            #region _Start

            protected override void _Start()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.TcpTransport2_.CommunicationChannel_._Start");
#endif

                base._Start();
            }

            #endregion

            #region _Stop

            protected override void _Stop()
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.TcpTransport2_.CommunicationChannel_._Stop");
#endif

                base._Stop();
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Outgoing

            protected override unsafe void _Outgoing(MessageClass _message)
            {
                if (this._sender == null)
                {
                    throw new Exception("Sender is null, should have instantiated in constructor.");
                }
                //if (this._sender == null)
                //{
                //    SocketHandler_ sock;
                //    if (this._transport.sockets.TryGetValue(this._networkaddress.ToString(), out sock))
                //    {
                //        this._sender = sock;

                //    }
                //    else
                //    {
                //        this._sender = new SocketHandler_(this._networkaddress, this._transport);
                //        this._transport.sockets.Add(this._networkaddress.ToString(), this._sender);

                //    }
                //}



                this._sender.Send(_message);
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion
    }
}
