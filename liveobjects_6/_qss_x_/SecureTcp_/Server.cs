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

// #define DEBUG_REPORT_DISCONNECT
#define DEBUG_DISABLE_SECURITY
//#define LINUX_NO_WCF
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

namespace QS._qss_x_.SecureTcp_
{
    
#if !LINUX_NO_WCF
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
#endif
    public abstract class Server : QS.Fx.Inspection.Inspectable, IDisposable
#if !LINUX_NO_WCF
        , IServer
#endif
    {
        #region Constructor

        protected Server(QS.Fx.Object.IContext _mycontext, string _address, string _network, int _mainport)
        {
            this._mycontext = _mycontext;
            this.address = _address;
            this.network = _network;
            this.logger = _mycontext.Platform.Logger;
            this.mainport = _mainport;
            QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(this.network);
            bool _found = false;
            foreach (IPAddress _ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && _subnet.contains(_ip))
                {
                    this.localaddress = _ip;
                    _found = true;
                    break;
                }
            }
            if (!_found)
                throw new Exception("Cannot find any local ip address on the requested subnet " + _subnet.ToString() + ".");
            this.mainsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.mainsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
            this.mainsocket.Bind(new IPEndPoint(this.localaddress, this.mainport));
            this.mainsocket.Listen(5);
            this.symmetricalgorithm = SymmetricAlgorithm.Create();
            this.random = new Random();
            this.connections = new Dictionary<QS.Fx.Base.ID, Connection>();

#if LINUX_NO_WCF
            this.logger.Log("Using non-WCF communication to establish initial connection.");
            this.initsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.initsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
            if(!this.address.ToLower().StartsWith(@"http://")) 
                throw new Exception("Improperly formatted server address");

            string[] _addr_split = this.address.Substring(7).Split(':');
            IPAddress[] _addrs = Dns.GetHostAddresses(_addr_split[0]);
            IPAddress _addr =  null;
            foreach (IPAddress _a in _addrs)
            {
                if (_a.AddressFamily == AddressFamily.InterNetwork)
                {
                    _addr = _a;
                    break;
                }
            }
            if (_addr == null)
            {
                throw new Exception("Could not find a suitable interface on this system to listen on for initial connection.  We could only find non-IPv4 adapters.");
            }
           
            IPEndPoint _endpt = new IPEndPoint(_addr, Convert.ToInt32(_addr_split[1]));
            this.logger.Log("Binding to " + _endpt.ToString());
            this.initsocket.Bind(_endpt);
            this.initsocket.Listen(5);
            this.logger.Log("Begin accept on " + _endpt.ToString());
            this.initsocket.BeginAccept(this._WCFConnectCallback, null);
#else
            this.servicehost = new ServiceHost(this);
            WSHttpBinding _binding = new WSHttpBinding();
#if DEBUG_DISABLE_SECURITY
            _binding.Security.Mode = SecurityMode.None;
#else
            _binding.Security.Mode = SecurityMode.Message;
            _binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
#endif
            this.servicehost.AddServiceEndpoint(typeof(IServer), _binding, this.address);
            this.servicehost.Open();
#endif

            this.mainsocket.BeginAccept(this._AcceptCallback, null);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;


        private QS.Fx.Logging.ILogger logger;
        [QS.Fx.Base.Inspectable]
        private string address;
        [QS.Fx.Base.Inspectable]
        private string network;
        [QS.Fx.Base.Inspectable]
        private int mainport;
        [QS.Fx.Base.Inspectable]
        private IPAddress localaddress;
        [QS.Fx.Base.Inspectable]
        private bool disposed;
#if LINUX_NO_WCF
        private Socket initsocket;
        private XmlSerializer _serializer = new XmlSerializer(typeof(object[]));
#endif
        private Socket mainsocket;
        private SymmetricAlgorithm symmetricalgorithm;
        private Random random;
        private ServiceHost servicehost;
        private IDictionary<QS.Fx.Base.ID, Connection> connections;

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("connections")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Connection> __inspectable_connections
        {
            get
            {
                return new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Connection>("connections", connections,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Connection>.ConversionCallback(delegate(string s) { return new QS.Fx.Base.ID(s); }));
            }
        }

        #endregion

        #region IServer Members

#if !LINUX_NO_WCF
        void IServer.Connect(string _client_ipaddress, int _client_portno,
            out string _server_ipaddress, out int _server_portno, out long _connection_id, out byte[] _iv, out byte[] _key)
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (!identity.IsAuthenticated)
                throw new Exception("The user has not been authenticated.");
            if (identity.IsGuest || identity.IsAnonymous)
                throw new Exception("the user is a guest or anonymous.");
            lock (this)
            {
                _server_ipaddress = this.localaddress.ToString();
                _server_portno = this.mainport;
                byte[] _bytes = BitConverter.GetBytes(0L);
                this.random.NextBytes(_bytes);
                _connection_id = BitConverter.ToInt64(_bytes, 0);
                ICryptoTransform _encryptor, _decryptor;
                lock (this.symmetricalgorithm)
                {
                    this.symmetricalgorithm.GenerateIV();
                    this.symmetricalgorithm.GenerateKey();
                    _iv = this.symmetricalgorithm.IV;
                    _key = this.symmetricalgorithm.Key;
                    _encryptor = this.symmetricalgorithm.CreateEncryptor(_key, _iv);
                    _decryptor = this.symmetricalgorithm.CreateDecryptor(_key, _iv);
                }
                QS.Fx.Base.ID _id = _CompressAddress(IPAddress.Parse(_client_ipaddress), _client_portno);
                Connection _connection = this._NewConnection();
                this.connections.Add(_id, _connection);
                _connection.Configure(_id, _connection_id, _encryptor, _decryptor, new QS.Fx.Base.ContextCallback<QS.Fx.Base.ID>(this._DisconnectCallback));
                _connection._Connected();
            }
        }
#else
        private void _WCFConnectCallback(IAsyncResult result)
        {
            
            lock (this)
            {
                if (!disposed)
                {
                    try
                    {
                        Socket _newsocket = this.initsocket.EndAccept(result);
                        IPEndPoint _remoteendpoint = (IPEndPoint)_newsocket.RemoteEndPoint;
                        this.logger.Log("Received non-WCF callback from "+_remoteendpoint.ToString());
                        
                        string _server_ipaddress;
                        int _server_portno;
                        long _connection_id;
                        byte[] _iv;
                        byte[] _key;



                        _server_ipaddress = this.localaddress.ToString();
                        _server_portno = this.mainport;
                        byte[] _bytes = BitConverter.GetBytes(0L);
                        this.random.NextBytes(_bytes);
                        _connection_id = BitConverter.ToInt64(_bytes, 0);
                        ICryptoTransform _encryptor, _decryptor;
                        lock (this.symmetricalgorithm)
                        {
                            this.symmetricalgorithm.GenerateIV();
                            this.symmetricalgorithm.GenerateKey();
                            _iv = this.symmetricalgorithm.IV;
                            _key = this.symmetricalgorithm.Key;
                            _encryptor = this.symmetricalgorithm.CreateEncryptor(_key, _iv);
                            _decryptor = this.symmetricalgorithm.CreateDecryptor(_key, _iv);
                        }

                        byte[] _buf = new byte[1024];
                        int _nrecv = _newsocket.Receive(_buf);
                        if (_nrecv <= 0)
                        {
                            throw new Exception("Failed to get connection info from client");
                        }
                        object[] _tmp = (object[])_serializer.Deserialize(new MemoryStream(_buf));
                        string _client_ipaddress = (string)_tmp[0];
                        int _client_portno = (int)_tmp[1];
                        
                        QS.Fx.Base.ID _id = _CompressAddress(IPAddress.Parse(_client_ipaddress), _client_portno);
                        _mycontext.Platform.Logger.Log("Adding ID: "+_id.ToString());
                        Connection _connection = this._NewConnection();
                        this.connections.Add(_id, _connection);
                        _connection.Configure(_id, _connection_id, _encryptor, _decryptor, new QS.Fx.Base.ContextCallback<QS.Fx.Base.ID>(this._DisconnectCallback));
                        _connection._Connected();

                        // send msg
                        
                        


                        object[] _toserialize = new object[] { _server_ipaddress, _server_portno, _connection_id, _iv, _key };
                        
                        MemoryStream _mem = new MemoryStream();
                        _serializer.Serialize(_mem,_toserialize);
                        
                        int _nsend = _newsocket.Send(_mem.ToArray(), 0, (int)_mem.Length, SocketFlags.None);
                        if (_nsend < _mem.Length)
                        {
                            throw new Exception("Failed to send entire initialization message");
                        }



    
                        this.initsocket.BeginAccept(this._WCFConnectCallback, null);
                    }
                    catch (Exception exc)
                    {
                    }
                }
            }
        }
#endif

        #endregion

        #region _CompressAddress

        private static QS.Fx.Base.ID _CompressAddress(IPAddress _ipaddress, int _portno)
        {
            byte[] _u1 = new byte[8];
            byte[] _u2 = new byte[8];
            _ipaddress.GetAddressBytes().CopyTo(_u1, 0);
            BitConverter.GetBytes(_portno).CopyTo(_u2, 0);            
            return new QS.Fx.Base.ID(new QS.Fx.Base.Int128(BitConverter.ToUInt64(_u1, 0), BitConverter.ToUInt64(_u2, 0)));
        }

        #endregion

        #region _NewConnection

        protected abstract Connection _NewConnection();

        #endregion

        #region _AcceptCallback

        private void _AcceptCallback(IAsyncResult result)
        {
            this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._AcceptCallback_0_), result));
        }

        private void _AcceptCallback_0_(object  _o)
        {
            IAsyncResult result = (IAsyncResult)_o;
            lock (this)
            {
                if (!disposed)
                {
                    try
                    {
                        Socket _newsocket = this.mainsocket.EndAccept(result);
                        IPEndPoint _remoteendpoint = (IPEndPoint)_newsocket.RemoteEndPoint;
                        QS.Fx.Base.ID _id = _CompressAddress(_remoteendpoint.Address, _remoteendpoint.Port);
                        Connection _connection;
                        if (this.connections.TryGetValue(_id, out _connection))
                        {
                            try
                            {
                                _connection.Connect(_newsocket);
                            }
                            catch (Exception exc)
                            {
                                this.connections.Remove(_id);
                                _Disconnected(_connection);
                            }
                        }
                        this.mainsocket.BeginAccept(this._AcceptCallback, null);
                    }
                    catch (Exception exc)
                    {
                        this._Exception(exc);
                    }
                }
            }
        }

        #endregion

        #region _Exception

        protected abstract void _Exception(Exception _exc);

        #endregion

        #region _Disconnected

        protected abstract void _Disconnected(Connection _connection);

        #endregion

        #region _DisconnectCallback

        private void _DisconnectCallback(QS.Fx.Base.ID _id)
        {
            lock (this)
            {
                Connection _connection;
                if (this.connections.TryGetValue(_id, out _connection))
                {
                    this.connections.Remove(_id);
                    _Disconnected(_connection);
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                this.disposed = true;
                if (this.mainsocket != null)
                {
                    try
                    {
                        this.mainsocket.Close();
                    }
                    catch (Exception)
                    {
                    }
                    this.mainsocket = null;
                }
                if (this.servicehost != null)
                    this.servicehost.Close();
                foreach (Connection connection in this.connections.Values)
                {
                    try
                    {
                        ((IDisposable) connection).Dispose();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        #endregion

        #region Class Connection

        protected abstract class Connection : Peer, IDisposable
        {
            #region Constructor

            protected Connection(QS.Fx.Object.IContext _mycontext) : base(_mycontext)
            {
                this._mycontext = _mycontext;
            }

            #endregion

            #region Configure

            public void Configure(QS.Fx.Base.ID _id, long _connection_id, ICryptoTransform _encryptor, ICryptoTransform _decryptor, 
                QS.Fx.Base.ContextCallback<QS.Fx.Base.ID> _disconnect_callback)
            {
                this.id = _id;
                this.disconnect_callback = _disconnect_callback;
                this.timer = new Timer(new TimerCallback(this._TimeoutCallback), null, TimeSpan.FromMinutes(1), TimeSpan.Zero);
                this._Configure(_connection_id, _encryptor, _decryptor); 
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IContext _mycontext;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.ID id;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.ContextCallback<QS.Fx.Base.ID> disconnect_callback;

            private Timer timer;

            #endregion

            #region _Connect

            public void Connect(Socket _socket)
            {
                this.timer.Dispose();
                this.timer = null;
                this._Connect(_socket);
            }

            public abstract void _Connected();

            #endregion

            #region _TimeoutCallback

            private void _TimeoutCallback(object o)
            {
                this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._TimeoutCallback_0_), o));
            }

            private void _TimeoutCallback_0_(object o)
            {
                lock (this)
                {
                    if (this.timer != null)
                        this._Disconnect();
                }
            }

            #endregion

            #region _Exception

            protected override void _Exception(Exception _exception)
            {
                this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Exception_0_)));
            }

            private void _Exception_0_(object _o)
            {
#if DEBUG_REPORT_DISCONNECT
                System.Windows.Forms.MessageBox.Show(_exception.ToString(), "Exception",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
#endif
                disconnect_callback(this.id);                
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                this._Disconnect();
            }

            #endregion
        }

        #endregion
    }
}
