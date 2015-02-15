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

// #define START_MANUALLY
// #define LAUNCH_WORKERS_IN_A_DEBUGGER

#define PROFILE_1
#define PROFILE_MESSAGES

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace QS._qss_x_.Runtime_
{
    public sealed class RemoteContext_ : QS.Fx.Inspection.Inspectable, IRemoteContext_, IDisposable
    {
        #region Constructor

        public RemoteContext_(QS.Fx.Object.IRuntimeContext _runtimecontext, int _nworkers, string _externals)
        {
            this._runtimecontext = _runtimecontext;
            this._nworkers = _nworkers;
            this._externals = _externals;
            List<Client_> _clients = new List<Client_>();
            if (this._nworkers > 0)
                for (int _i = 0; _i < this._nworkers; _i++)
                    _clients.Add(new Client_(this, false, null));
            if (this._externals != null)
                foreach (string _address in _externals.Split(','))
                    _clients.Add(new Client_(this, true, _address));
            if (_clients.Count > 0)
            {
                this._clients = _clients.ToArray();
                foreach (Client_ _client in this._clients)
                    _client._Wait();
                this._isenabled = true;
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IRuntimeContext _runtimecontext;
        [QS.Fx.Base.Inspectable]
        private int _nworkers;
        [QS.Fx.Base.Inspectable]
        private string _externals;
        [QS.Fx.Base.Inspectable]
        private bool _isenabled;
        [QS.Fx.Base.Inspectable]
        private Client_[] _clients;
#if PROFILE_1
        [QS.Fx.Base.Inspectable]
        QS._qss_c_.Statistics_.Samples2D DeserializationSamples
        {
            get
            {
                QS._qss_c_.Statistics_.Samples2D _s = new QS._qss_c_.Statistics_.Samples2D();
                foreach (Client_ _c in _clients)
                {
                    foreach (QS._core_e_.Data.XY _xy in _c._serializingseries.Samples)
                    {
                        _s.Add(_xy.x, _xy.y);
                    }
                }
                return _s;
            }
        }
        [QS.Fx.Base.Inspectable]
        QS._qss_c_.Statistics_.Samples2D DeserializationSizeSamples
        {
            get
            {
                QS._qss_c_.Statistics_.Samples2D _s = new QS._qss_c_.Statistics_.Samples2D();
                foreach (Client_ _c in _clients)
                {
                    foreach (QS._core_e_.Data.XY _xy in _c._serializingseries_objectsize.Samples)
                    {
                        _s.Add(_xy.x, _xy.y);
                    }
                }
                return _s;
            }
        }
#endif
        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this._clients != null)
                foreach (Client_ _client in this._clients)
                    ((IDisposable)_client).Dispose();
        }

        #endregion

        #region IRemoteContext_ Members

        IClient_[] IRemoteContext_._Clients
        {
            get
            {
                IClient_[] _clients = new IClient_[this._clients.Length];
                for (int _i = 0; _i < _clients.Length; _i++)
                    _clients[_i] = this._clients[_i];
                return _clients;
            }
        }

        #endregion

        #region Client_

        private sealed class Client_ : QS.Fx.Inspection.Inspectable, IDisposable, IClient_, QS.Fx.Base.IEvent, QS.Fx.Base.IHandler
        {
            #region Constructor

            public Client_(RemoteContext_ _remotecontext, bool _isexternal, string _address)
            {
                this._remotecontext = _remotecontext;
                this._scheduler = this._remotecontext._runtimecontext.Platform.Scheduler;
                this._clock = this._remotecontext._runtimecontext.Platform.Clock;
                this._isexternal = _isexternal;
                this._address = _address;
                if (!this._isexternal)
                {
                    this._initializationsocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this._initializationsocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
                    this._initializationsocket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
                    this._initializationsocket.Listen(1);
                    this._ipaddress = ((IPEndPoint)this._initializationsocket.LocalEndPoint).Address;
                    this._port = ((IPEndPoint)this._initializationsocket.LocalEndPoint).Port;
                    this._initializationsocket.BeginAccept(this._Accept, null);
#if START_MANUALLY
                    Form _form = new Form();
                    _form.Text = "Launch Worker";
                    TextBox _textbox = new TextBox();
                    _textbox.Text = "-a -m -o:" + this._ipaddress.ToString() + ":" + this._port.ToString();
                    _textbox.Dock = DockStyle.Fill;
                    _form.Controls.Add(_textbox);
                    _form.ShowDialog();
#else
                    this._process = new Process();
#if LAUNCH_WORKERS_IN_A_DEBUGGER
                    const string _devenv = @"Microsoft Visual Studio 9.0\Common7\IDE\devenv.exe";
                    string _filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), _devenv);
                    if (!File.Exists(_filename))
                    {
                        _filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + " (x86)", _devenv);
                        if (!File.Exists(_filename))
                            throw new Exception("Cannot locate devenv.exe!");
                    }
                    this._process.StartInfo.FileName = _filename;
                    this._process.StartInfo.Arguments = "/debugexe " + Process.GetCurrentProcess().MainModule.FileName + 
                        " -a -m -o:" + this._ipaddress.ToString() + ":" + this._port.ToString();
#else
                    this._process.StartInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                    
                    string _serialization_opt = (QS.Fx.Object.Runtime.SerializationType == 3) ? "internal":  "binary";
                    this._process.StartInfo.Arguments = "-a -m:1 -o:" + this._ipaddress.ToString() + ":" + this._port.ToString() + " -serialization:"+_serialization_opt;
#endif
                    this._process.StartInfo.CreateNoWindow = false;
                    this._process.StartInfo.UseShellExecute = true;
                    this._process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    if (!this._process.Start())
                        throw new Exception("Could not start a new worker process.");
                    //this._process.ProcessorAffinity = new IntPtr(0x1);
#endif
                }

                this._InitializeInspection();
            }

            #endregion

            #region Destructor

            ~Client_()
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
                        try
                        {
                            if (this._process != null)
                                this._process.Kill();
                        }
                        catch (Exception)
                        {
                        }

                        try
                        {
                            if (this._socket != null)
                                this._socket.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private RemoteContext_ _remotecontext;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Scheduling.IScheduler _scheduler;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IClock _clock;
            [QS.Fx.Base.Inspectable]
            private int _disposed;
            [QS.Fx.Base.Inspectable]
            private int _schedule;
            [QS.Fx.Base.Inspectable]
            private int _operating;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.IEvent _root;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.IEvent _from;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.IEvent _to;
            [QS.Fx.Base.Inspectable]
            private bool _isexternal;
            [QS.Fx.Base.Inspectable]
            private ManualResetEvent _ready = new ManualResetEvent(false);
            [QS.Fx.Base.Inspectable]
            private bool _isready;
            [QS.Fx.Base.Inspectable]
            private string _address;
            [QS.Fx.Base.Inspectable]
            private Process _process;
            [QS.Fx.Base.Inspectable]
            private Socket _initializationsocket;
            [QS.Fx.Base.Inspectable]
            private Socket _socket;
            [QS.Fx.Base.Inspectable]
            private IPAddress _ipaddress;
            [QS.Fx.Base.Inspectable]
            private int _port;
            [QS.Fx.Base.Inspectable]
            private bool _incoming;
            [QS.Fx.Base.Inspectable]
            private byte[] _incomingheader;
            [QS.Fx.Base.Inspectable]
            private byte[] _incomingdata;
            [QS.Fx.Base.Inspectable]
            private int _incomingcount;
            [QS.Fx.Base.Inspectable]
            private int _incomingsize;
            [QS.Fx.Base.Inspectable]
            private SerializationType_ _incomingtype;
            [QS.Fx.Base.Inspectable]
            private System.Runtime.Serialization.IFormatter _formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            [QS.Fx.Base.Inspectable]
            private int _maxsize = 100;
            [QS.Fx.Base.Inspectable]
            private int _lastid = 0;
            [QS.Fx.Base.Inspectable]
            private int _incomingcount1;
            [QS.Fx.Base.Inspectable]
            private int _incomingcount2;
#if PROFILE_MESSAGES
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _incomingseries = new QS._qss_c_.Statistics_.Samples2D();
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _outgoingseries = new QS._qss_c_.Statistics_.Samples2D();
            [QS.Fx.Base.Inspectable]
            public QS._qss_c_.Statistics_.Samples2D _serializingseries = new QS._qss_c_.Statistics_.Samples2D();
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _schedulingseries = new QS._qss_c_.Statistics_.Samples2D();
            [QS.Fx.Base.Inspectable]
            public QS._qss_c_.Statistics_.Samples2D _serializingseries_objectsize = new QS._qss_c_.Statistics_.Samples2D();
            [QS.Fx.Base.Inspectable]
            public IDictionary<int, Incoming_> _incoming_index = new Dictionary<int, Incoming_>();

            public struct Incoming_
            {
                public Incoming_(int size, SerializationType_ _type)
                {
                    this._size = size;
                    this._type = _type;
                }
                int _size;
                SerializationType_ _type;
            }
#endif

            private IDictionary<int, Object_> _objects = new Dictionary<int, Object_>();
            private IDictionary<int, object> _incomingobjects = new Dictionary<int, object>();

            private QS.Fx.Base.IEvent _next;

            #endregion

            #region Inspection

            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_> __inspectable_objects;
            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<int, object> __inspectable_incomingobjects;

            private void _InitializeInspection()
            {
                this.__inspectable_objects =
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_>("_objects", this._objects,
                        new QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_>.ConversionCallback(Convert.ToInt32));
                this.__inspectable_incomingobjects =
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<int, object>("_incomingobjects", this._incomingobjects,
                        new QS._qss_e_.Inspection_.DictionaryWrapper1<int, object>.ConversionCallback(Convert.ToInt32));
            }

            #endregion

            #region _Accept

            private unsafe void _Accept(IAsyncResult _result)
            {
                this._socket = this._initializationsocket.EndAccept(_result);
                this._initializationsocket.Close();
                this._isready = true;
                this._ready.Set();
                this._incomingheader = new byte[sizeof(int) + sizeof(byte)];
                this._incomingcount = 0;
                this._incomingsize = this._incomingheader.Length;
                this._socket.BeginReceive(this._incomingheader, 0, this._incomingsize, SocketFlags.None, new AsyncCallback(this._Receive), null);
            }

            #endregion

            #region _Wait

            public void _Wait()
            {
                this._ready.WaitOne();
            }

            #endregion

            #region _Receive (1)

            private unsafe void _Receive(IAsyncResult _result)
            {
                if (this._disposed == 0)
                {
                    SocketError _error;
                    int _count = this._socket.EndReceive(_result, out _error);
                    if (_error == SocketError.Success)
                    {
                        if (_count < 0)
                            throw new Exception("Received less than zero bytes!");
                        this._incomingcount += _count;
                        if (this._incomingcount < this._incomingsize)
                            this._socket.BeginReceive((this._incoming ? this._incomingdata : this._incomingheader),
                                this._incomingcount, this._incomingsize - this._incomingcount, SocketFlags.None, new AsyncCallback(this._Receive), null);
                        else
                        {
                            this._incomingcount = 0;
                            if (this._incoming)
                            {
                                int _index = (++this._incomingcount1);
                                byte[] _data = this._incomingdata;
                                int _size = this._incomingsize;
                                this._incoming = false;
                                this._incomingsize = this._incomingheader.Length;
                                this._incomingdata = null;
                                SerializationType_ _type = this._incomingtype;

#if PROFILE_MESSAGES
                                this._incomingseries.Add(this._clock.Time, (double)_size);
#endif

                                this._socket.BeginReceive(this._incomingheader, 0, this._incomingsize, SocketFlags.None, new AsyncCallback(this._Receive), null);
                                if (_size < 1000)
                                    this._Receive(_index, _data, _type);
                                else
                                    this._Schedule(
                                        new QS.Fx.Base.Event<int, byte[], SerializationType_>(
                                            new QS.Fx.Base.ContextCallback<int, byte[], SerializationType_>(this._Receive), _index, _data, _type));
                                
                            }
                            else
                            {
                                this._incoming = true;
                                fixed (byte* _pheader = this._incomingheader)
                                {
                                    this._incomingtype = (SerializationType_) (*_pheader);
                                    this._incomingsize = *((int*)(_pheader + sizeof(byte)));
                                    
                                    
                                }
                                if (this._incomingsize > 0)
                                {
                                    this._incomingdata = new byte[this._incomingsize];
                                    this._socket.BeginReceive(this._incomingdata, 0, this._incomingsize, SocketFlags.None, new AsyncCallback(this._Receive), null);
                                }
                                else
                                    throw new Exception("Could not receive, the initial header indicates that the message is empty, disconnecting the channel.");
                            }
                        }
                    }
                }
            }

            #endregion

            #region _Receive (2)

            private unsafe void _Receive(int _index, byte[] _data, SerializationType_ _type)
            {
#if PROFILE_MESSAGES
                int _data_len = _data.Length;
                double _t1 = this._clock.Time;
                bool _add = false;
                if (_data_len > 187)
                {
                    _add = true;
                }
                //{
                //    Random _r = new Random();
                //    using (FileStream _fs = new FileStream(@"C:\Users\Public\obj-bytestream-" + _r.Next(10000), FileMode.CreateNew))
                //    {
                //        _fs.Write(_data, 0, _data.Length);
                //    }6
                //}
                

#endif

                object _object;

                switch (_type)
                {
                    case SerializationType_._Binary:
                        {
                            if (_add)
                            {
                                int b;
                            }
                            MemoryStream _memory = new MemoryStream(_data);
                            _object = this._formatter.Deserialize(_memory);
                        }
                        break;

                    case SerializationType_._Internal:
                        {
                            ushort _incoming_classid;
                            uint _incoming_headersize;
                            fixed (byte* _pdata = _data)
                            {
                                _incoming_classid = *((ushort *) _pdata);
                                _incoming_headersize = *((uint *) (_pdata + sizeof(ushort)));
                            }
                            QS.Fx.Serialization.ISerializable _message = QS.Fx.Serialization.Serializer.Internal.CreateObject(_incoming_classid);
                            QS.Fx.Base.ConsumableBlock _incoming_header =
                                new QS.Fx.Base.ConsumableBlock(_data, sizeof(uint) + sizeof(ushort), _incoming_headersize);
                            QS.Fx.Base.ConsumableBlock _incoming_data =
                                new QS.Fx.Base.ConsumableBlock(_data, (uint) (sizeof(uint) + sizeof(ushort) + _incoming_headersize), 
                                    (uint) (_data.Length - sizeof(ushort) - sizeof(uint) - _incoming_headersize));
                            _message.DeserializeFrom(ref _incoming_header, ref _incoming_data);
                            _object = _message;
                        }
                        break;

                    case SerializationType_._Xml:
                        throw new NotImplementedException();

                    case SerializationType_._Undefined:
                    default:
                        throw new NotSupportedException();
                }

#if PROFILE_MESSAGES
                double _t3 = 0;
                if (_add)
                {
                    _t3 = this._clock.Time;
                    this._serializingseries.Add(_t1, _t3 - _t1);
                }
#endif

                this._Schedule(new QS.Fx.Base.Event<int, object>(new QS.Fx.Base.ContextCallback<int, object>(this._Receive), _index, _object));

#if PROFILE_MESSAGES

                if (_add)
                {
                    double _t2 = this._clock.Time;
                    double _td = _t2 - _t3;
                    this._serializingseries_objectsize.Add(_t1, _data_len);
                    this._schedulingseries.Add(_t1, _td);
                }
#endif

            }

            #endregion

            #region _Receive (3)

            private void _Receive(int _index, object _object)
            {
                if (this._disposed == 0)
                {
                    if (_index > this._incomingcount2)
                    {
                        if (_index == (this._incomingcount2 + 1))
                        {
                            _Receive(_object);
                            while (true)
                            {
                                _index++;
                                if (this._incomingobjects.TryGetValue(_index, out _object))
                                {
                                    this._incomingobjects.Remove(_index);
                                    _Receive(_object);
                                }
                                else
                                    break;
                            }
                            this._incomingcount2 = _index - 1;
                        }
                        else
                            this._incomingobjects.Add(_index, _object);
                    }
                    else
                        throw new Exception("Duplicate index!");
                }
            }

            #endregion

            #region _Receive (4)

            private void _Receive(object _object)
            {
                if (_object is QS._qss_x_.Runtime_.IOperation_)
                {
                    QS._qss_x_.Runtime_.IOperation_ _operation = (QS._qss_x_.Runtime_.IOperation_)_object;
                    int _id = _operation._Id;
                    switch (_operation._Type)
                    {
                        case OperationType_._Ready:
                            {
                                Object_ _wrapperobject;
                                if (!this._objects.TryGetValue(_id, out _wrapperobject))
                                    throw new Exception("Cannot locate object with id \"" + _id.ToString() + "\".");
                                Ready_ _ready = (Ready_)_operation;
                                _wrapperobject._Ready(_ready._Count, _ready._Overhead);
                            }
                            break;

                        case OperationType_._Result:
                            {
                                Object_ _wrapperobject;
                                if (!this._objects.TryGetValue(_id, out _wrapperobject))
                                    throw new Exception("Cannot locate object with id \"" + _id.ToString() + "\".");
                                _wrapperobject._Result(((Result_)_operation)._SequenceNo, ((Result_)_operation)._Object);
                            }
                            break;

                        case OperationType_._Create:
                        case OperationType_._Delete:
                        case OperationType_._Import:
                        case OperationType_._Export:
                        case OperationType_._Invoke:
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                    throw new NotImplementedException();
            }

            #endregion

            #region _Send

            private void _Send(object _object)
            {
                MemoryStream _memory = new MemoryStream(_maxsize);
                this._formatter.Serialize(_memory, _object);
                int _size = (int)_memory.Length;
                if (_size > _maxsize)
                    _maxsize = _size;
#if PROFILE_MESSAGES
                this._outgoingseries.Add(this._clock.Time, (double)_size);
#endif
                byte[] _header = BitConverter.GetBytes(_size);
                this._socket.BeginSend(_header, 0, _header.Length, SocketFlags.None, null, null);
                byte[] _data = _memory.GetBuffer();
                this._socket.BeginSend(_data, 0, _size, SocketFlags.None, null, null);
            }

            #endregion

            #region _Schedule

            private void _Schedule(QS.Fx.Base.IEvent _event)
            {
                if (this._disposed == 0)
                {
                    if (_event.Next != null)
                        throw new Exception("Cannot enqueue an event that links to another event.");
                    QS.Fx.Base.IEvent _myroot;
                    do
                    {
                        _myroot = this._root;
                        _event.Next = _myroot;
                    }
                    while (Interlocked.CompareExchange<QS.Fx.Base.IEvent>(ref this._root, _event, _myroot) != _myroot);
                    this._schedule = 1;
                    if ((this._operating == 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0))
                        this._scheduler.Schedule(this);
                }
            }

            #endregion

            #region IClient_ Members

            IObject_ IClient_._Object(string _class)
            {
                int _id = Interlocked.Increment(ref this._lastid);
                Object_ _object = new Object_(this, _id, _class);
                this._Schedule(new Create_(this, _object, _id, _class));
                return _object;
            }

            #endregion

            #region IHandler Members

            void QS.Fx.Base.IHandler.Handle(QS.Fx.Base.IEvent _event)
            {
                IOperation_ _operation = (IOperation_)_event;
                switch (_operation._Type)
                {
                    case OperationType_._Create:
                        {
                            int _id = _operation._Id;
                            Object_ _wrapperobject = (Object_)_operation._Context;
                            if (this._objects.ContainsKey(_id))
                                throw new Exception("Already registered object with id \"" + _id.ToString() + "\".");
                            this._objects.Add(_id, _wrapperobject);
                            this._Send(_event);
                        }
                        break;

                    case OperationType_._Delete:
                        {
                            int _id = _operation._Id;
                            if (!this._objects.ContainsKey(_id))
                                throw new Exception("Object with id \"" + _id.ToString() + "\" is not registered.");
                            this._objects.Remove(_id);
                            this._Send(_event);
                        }
                        break;

                    case OperationType_._Import:
                    case OperationType_._Export:
                    case OperationType_._Invoke:
                        {
                            this._Send(_event);
                        }
                        break;

                    case OperationType_._Ready:
                    case OperationType_._Result:
                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion

            #region IEvent Members

            void QS.Fx.Base.IEvent.Handle()
            {
                double _t1 = _clock.Time;
                int _handled = 0;
                bool _postpone = false;
                if (this._disposed == 0)
                {
                    do
                    {
                        while (!_postpone && ((this._schedule != 0) || (this._from != null)))
                        {
                            bool _more = (this._schedule != 0);
                            this._schedule = 0;
                            if (_more)
                            {
                                QS.Fx.Base.IEvent _event = Interlocked.Exchange<QS.Fx.Base.IEvent>(ref this._root, null);
                                if (_event != null)
                                {
                                    QS.Fx.Base.IEvent _s1 = null;
                                    QS.Fx.Base.IEvent _s2 = null;
                                    while (_event != null)
                                    {
                                        QS.Fx.Base.IEvent _next = _event.Next;
                                        if (_s1 != null)
                                            _event.Next = _s1;
                                        else
                                        {
                                            _event.Next = null;
                                            _s2 = _event;
                                        }
                                        _s1 = _event;
                                        _event = _next;
                                    }
                                    if (_s1 != null)
                                    {
                                        if (this._to != null)
                                            this._to.Next = _s1;
                                        else
                                            this._from = _s1;
                                        this._to = _s2;
                                    }
                                }
                            }
                            while (!_postpone && (this._from != null))
                            {
                                QS.Fx.Base.IEvent _event = this._from;
                                this._from = _event.Next;
                                _event.Next = null;
                                if (this._from == null)
                                    this._to = null;
                                _event.Handle();
                                _handled++;
                                if (_handled >= QS.Fx.Object.Runtime.MaximumNumberOfEventsToHandleSequentially)
                                    _postpone = true;
                                double _elapsed = _clock.Time - _t1;
                                if (_elapsed >= QS.Fx.Object.Runtime.MaximumAmountOfTimeToHandleEventsSequentially)
                                    _postpone = true;
                            }
                        }
                        if (_postpone)
                            this._scheduler.Schedule(this);
                        else
                            this._operating = 0;
                    }
                    while (!_postpone && (this._schedule != 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0));
                }
            }

            QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
            {
                get { return this._next; }
                set { this._next = value; }
            }

            QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
            {
                get { return QS.Fx.Base.SynchronizationOption.Multithreaded; }
            }

            #endregion

            #region Object_

            private sealed class Object_ : QS.Fx.Inspection.Inspectable, IDisposable, IObject_
            {
                #region Constructor

                public Object_(Client_ _client, int _id, string _class)
                {
                    this._client = _client;
                    this._id = _id;
                    this._class = _class;
                }

                #endregion

                #region Destructor

                ~Object_()
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
                            this._client._Schedule(new Delete_(this._client, this, this._id));
                        }
                    }
                }

                #endregion

                #region Fields

                [QS.Fx.Base.Inspectable]
                private Client_ _client;
                [QS.Fx.Base.Inspectable]
                private int _id;
                [QS.Fx.Base.Inspectable]
                private string _class;
                [QS.Fx.Base.Inspectable]
                private int _disposed;
                [QS.Fx.Base.Inspectable]
                private QS.Fx.Base.ContextCallback<int, double> _ready;
                [QS.Fx.Base.Inspectable]
                private QS.Fx.Base.ContextCallback<int, QS.Fx.Object.Classes.IObject> _result;

                #endregion

                #region _Import

                void IObject_._Import(QS.Fx.Object.Classes.IObject _object)
                {
                    this._client._Schedule(new Import_(this._client, this, this._id, _object));
                }

                #endregion

                #region _Invoke

                void IObject_._Invoke(QS.Fx.Base.IEvent _event)
                {
                    this._client._Schedule(new Invoke_(this._client, this, this._id, _event));
                }

                #endregion

                #region _Ready

                void IObject_._Ready(QS.Fx.Base.ContextCallback<int, double> _callback)
                {
                    if (Interlocked.CompareExchange<QS.Fx.Base.ContextCallback<int, double>>(ref this._ready, _callback, null) != null)
                        throw new Exception("Cannot register more than one ready callback!");
                }

                public void _Ready(int _count, double _overhead)
                {
                    QS.Fx.Base.ContextCallback<int, double> _callback = this._ready;
                    if (_callback == null)
                        throw new Exception("No result callback has been registered!");
                    try
                    {
                        _callback(_count, _overhead);
                    }
                    catch (Exception)
                    {
                    }
                }

                #endregion

                #region _Export

                void IObject_._Export(int _sequenceno)
                {
                    this._client._Schedule(new Export_(this._client, this, this._id, _sequenceno));
                }

                #endregion

                #region _Result

                void IObject_._Result(QS.Fx.Base.ContextCallback<int, QS.Fx.Object.Classes.IObject> _callback)
                {
                    if (Interlocked.CompareExchange<QS.Fx.Base.ContextCallback<int, QS.Fx.Object.Classes.IObject>>(ref this._result, _callback, null) != null)
                        throw new Exception("Cannot register more than one result callback!");
                }

                public void _Result(int _sequenceno, QS.Fx.Object.Classes.IObject _object)
                {
                    QS.Fx.Base.ContextCallback<int, QS.Fx.Object.Classes.IObject> _callback = this._result;
                    if (_callback == null)
                        throw new Exception("No result callback has been registered!");
                    try
                    {
                        _callback(_sequenceno, _object);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Caught exception executing callback on object", e);
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion
    }
}
