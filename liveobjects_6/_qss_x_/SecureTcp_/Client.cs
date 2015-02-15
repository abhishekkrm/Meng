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
    public abstract class Client : Peer
    {
        #region Constructor

        protected Client(QS.Fx.Object.IContext _mycontext, string _address, string _network) : base(_mycontext)
        {
            this.address = _address;
            this.network = _network;
            string _localname = Dns.GetHostName();
            IPHostEntry _localhostentry;
            try
            {
                _localhostentry = Dns.GetHostEntry(_localname);
            }
            catch (Exception exc)
            {
                throw new Exception("Cannot resolve local host name \"" + _localname + "\".", exc);
            }
            QS._qss_c_.Base1_.Subnet _subnet = new QS._qss_c_.Base1_.Subnet(this.network);
            bool _found = false;
            foreach (IPAddress _ip in _localhostentry.AddressList)
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
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string address;
        [QS.Fx.Base.Inspectable]
        private string network;
        [QS.Fx.Base.Inspectable]
        private IPAddress localaddress;

#if LINUX_NO_WCF
        [QS.Fx.Base.Inspectable]
        private Socket initsocket;

        private XmlSerializer _serializer = new XmlSerializer(typeof(object[]));
#endif

        #endregion

        #region _Connect

        protected void _Connect()
        {
            lock (this)
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                _socket.Bind(new IPEndPoint(this.localaddress, 0));
                int _myport = ((IPEndPoint) _socket.LocalEndPoint).Port;
                string _serveripaddress;
                int _serverportno;
                long _connectionid;
                byte[] _iv, _key;

#if !LINUX_NO_WCF
                WSHttpBinding _binding = new WSHttpBinding();
#if DEBUG_DISABLE_SECURITY
                _binding.Security.Mode = SecurityMode.None;
#else
                _binding.Security.Mode = SecurityMode.Message;
                _binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
#endif
                ChannelFactory<IServer> _channelfactory = new ChannelFactory<IServer>(_binding, new EndpointAddress(this.address));
                IServer _controller = _channelfactory.CreateChannel();
                using ((IDisposable)_controller)
                {
                    _controller.Connect(this.localaddress.ToString(), _myport, out _serveripaddress, out _serverportno, out _connectionid, out _iv, out _key);
                }

#else

                // note this is currently synchronous, because the last call was too (??)
                initsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                initsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                if(!this.address.ToLower().StartsWith(@"http://"))
                    throw new Exception("Improperly formatted server address");
                string[] _tmp = this.address.Substring(7).Split(':');
                IPAddress[] _addrs = Dns.GetHostAddresses(_tmp[0]);
                IPAddress _addr = null;
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
                byte[] _buf = new byte[2048];
                initsocket.Connect(new IPEndPoint(_addr, Convert.ToInt32(_tmp[1])));
                MemoryStream _stream = new MemoryStream();
                _serializer.Serialize(_stream, new object[] { this.localaddress.ToString(), _myport });
                int _nsend = initsocket.Send(_stream.ToArray());
                if (_nsend <= 0)
                {
                    throw new Exception("Failed to send connection info");
                }
                int _nrecv = initsocket.Receive(_buf);
                if (_nrecv > 0)
                {

                    _stream = new MemoryStream(_buf);
                    object[] _params = (object[])_serializer.Deserialize(_stream);
                    _serveripaddress = (string)_params[0];
                    _serverportno = (int)_params[1];
                    _connectionid = (long)_params[2];
                    _iv = (byte[])_params[3];
                    _key = (byte[])_params[4];
                }
                else
                {
                    throw new Exception("Received 0 bytes ?");
                }

                initsocket.Disconnect(false);

#endif
                ICryptoTransform _encryptor, _decryptor;
                using (SymmetricAlgorithm _symmetricalgorithm = SymmetricAlgorithm.Create())
                {
                    _encryptor = _symmetricalgorithm.CreateEncryptor(_key, _iv);
                    _decryptor = _symmetricalgorithm.CreateDecryptor(_key, _iv);
                }
                _socket.Connect(IPAddress.Parse(_serveripaddress), _serverportno);
                this._Configure(_connectionid, _encryptor, _decryptor);
                this._Connect(_socket);
            }
        }

        #endregion
    }
}
