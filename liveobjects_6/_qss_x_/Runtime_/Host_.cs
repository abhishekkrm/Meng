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

// #define DEBUG_MESSAGES
#define PROFILE_1
#define KEEP_OBJECT_FOR_PROFILING
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;

namespace QS._qss_x_.Runtime_
{
    public sealed class Host_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, IDisposable, QS.Fx.Base.IHandler
    {
        #region Constructor

        public Host_(QS.Fx.Object.IContext _mycontext, string _address)
        {
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._address = _address;
            this._ipaddress = IPAddress.Parse(_address.Substring(0, _address.IndexOf(':')));
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, false);
            this._socket.Bind(new IPEndPoint(this._ipaddress, 0));
            this._port = ((IPEndPoint)this._socket.LocalEndPoint).Port;
            this._socket.Connect(this._ipaddress, Convert.ToInt32(_address.Substring(_address.IndexOf(':') + 1)));
            this._incomingheader = BitConverter.GetBytes((int)0);
            this._incomingcount = 0;
            this._incomingsize = this._incomingheader.Length;
            this._socket.BeginReceive(this._incomingheader, this._incomingcount, this._incomingsize, SocketFlags.None, new AsyncCallback(this._Receive), null);
            this._InitializeInspection();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private string _address;
        [QS.Fx.Base.Inspectable]
        private IPAddress _ipaddress;
        [QS.Fx.Base.Inspectable]
        private int _port;
        [QS.Fx.Base.Inspectable]
        private Socket _socket;
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
        private int _maxsize = 100;
        [QS.Fx.Base.Inspectable]
        private System.Runtime.Serialization.IFormatter _formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        [QS.Fx.Base.Inspectable]
        private double _batching = 0.1;
        [QS.Fx.Base.Inspectable]
        private double _checkpointing = 1000;
#if PROFILE_1
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _serialization_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _serializiation_sizes = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D[] ObjectStats
        {
            get
            {
                List<QS._qss_c_.Statistics_.Samples2D> _t_list = new List<QS._qss_c_.Statistics_.Samples2D>();
                foreach (Object_ _o in _objects.Values)
                {
                    foreach (QS._qss_c_.Statistics_.Samples2D _s in _o.Stats)
                        _t_list.Add(_s);
                }
                return _t_list.ToArray();
            }
        }
#endif
        private IDictionary<int, Object_> _objects = new Dictionary<int, Object_>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_> __inspectable_objects;

        private void _InitializeInspection()
        
        {
            
            __inspectable_objects =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_>("_objects", _objects,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<int, Object_>.ConversionCallback(Convert.ToInt32));
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            try
            {
                if (this._socket != null)
                    this._socket.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region _Receive (1)

        private void _Receive(IAsyncResult _result)
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
                        this._mycontext.Enqueue(new QS.Fx.Base.Event<byte[]>(new QS.Fx.Base.ContextCallback<byte[]>(this._Receive), this._incomingdata));
                        this._incoming = false;
                        this._incomingsize = this._incomingheader.Length;
                        this._incomingdata = null;
                        this._socket.BeginReceive(this._incomingheader, 0, this._incomingsize, SocketFlags.None, new AsyncCallback(this._Receive), null);
                    }
                    else
                    {
                        this._incoming = true;
                        this._incomingsize = BitConverter.ToInt32(this._incomingheader, 0);
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

        #endregion

        #region _Receive (2)

        private void _Receive(byte[] _data)
        {
            MemoryStream _memory = new MemoryStream(_data);
            object _object = this._formatter.Deserialize(_memory);
            if (_object is QS._qss_x_.Runtime_.IOperation_)
            {
#if DEBUG_MESSAGES
                this._mycontext.Platform.Logger.Log("Incoming : " + QS.Fx.Printing.Printable.ToString(_object));
#endif
                QS._qss_x_.Runtime_.IOperation_ _operation = (QS._qss_x_.Runtime_.IOperation_)_object;
                int _id = _operation._Id;
                switch (_operation._Type)
                {
                    case OperationType_._Create:
                        {
                            if (this._objects.ContainsKey(_id))
                                throw new Exception("Cannot create object with id \"" + _id + "\" because another object with this key already exists.");
                            Object_ _wrapperobject = new Object_(this, _id);
                            this._objects.Add(_id, _wrapperobject);
                            _wrapperobject._Schedule(_operation);
                        }
                        break;

                    case OperationType_._Delete:
                        {
                            Object_ _wrapperobject;
                            if (this._objects.TryGetValue(_id, out _wrapperobject))
                            {
                                this._objects.Remove(_id);
                                try
                                {
                                    ((IDisposable)_wrapperobject).Dispose();
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                        break;

                    case OperationType_._Import:
                    case OperationType_._Export:
                    case OperationType_._Invoke:
                        {
                            Object_ _wrapperobject;
                            if (!this._objects.TryGetValue(_id, out _wrapperobject))
                                throw new Exception("Cannot import into object with id \"" + _id + "\" because no such object exists.");
                            _wrapperobject._Schedule(_operation);
                        }
                        break;

                    case OperationType_._Ready:
                    case OperationType_._Result:
                    default:
                        throw new NotImplementedException();
                }
            }
            else
                throw new NotImplementedException();
        }

        #endregion

        #region _Send

        private unsafe void _Send(QS._qss_x_.Runtime_.IOperation_ _operation)
        {
#if DEBUG_MESSAGES
            this._mycontext.Platform.Logger.Log("Outgoing : " + QS.Fx.Printing.Printable.ToString(_operation));
#endif

            switch (_operation._Serialization)
            {
                case SerializationType_._Binary:
                    {
#if PROFILE_1
                        double _t1 = this._clock.Time;
#endif
                        int _size;
                        byte[] _header, _data;
                        MemoryStream _memory = new MemoryStream(this._maxsize);
                        this._formatter.Serialize(_memory, _operation);
                        _size = (int) _memory.Length;
                        _header = new byte[sizeof(int) + sizeof(byte)];
                        fixed (byte *_pheader = _header)
                        {
                            *_pheader = (byte) SerializationType_._Binary;
                            *((int *)(_pheader + sizeof(byte))) = _size;
                        }
                        _data = _memory.GetBuffer();
#if PROFILE_1
                        if (_size > 187)
                        {
                            double _t2 = this._clock.Time;
                            this._serializiation_sizes.Add(_t1, _size);
                            this._serialization_times.Add(_t1, _t2 - _t1);
                        }
#endif
                        if (_size > this._maxsize)
                            this._maxsize = _size;
                        this._socket.BeginSend(_header, 0, _header.Length, SocketFlags.None, null, null);
                        this._socket.BeginSend(_data, 0, _size, SocketFlags.None, null, null);
                    }
                    break;

                case SerializationType_._Internal:
                    {
                        QS.Fx.Serialization.ISerializable _message = (QS.Fx.Serialization.ISerializable) _operation;
                        QS.Fx.Serialization.SerializableInfo _info = _message.SerializableInfo;
                        QS.Fx.Base.ConsumableBlock _header = new QS.Fx.Base.ConsumableBlock((uint)(_info.HeaderSize + 2 * sizeof(uint) + sizeof(ushort) + sizeof(byte)));
                        IList<QS.Fx.Base.Block> _blocks = new List<QS.Fx.Base.Block>(_info.NumberOfBuffers + 1);
                        _blocks.Add(_header.Block);
                        fixed (byte* _headerptr_0 = _header.Array)
                        {
                            byte* _headerptr = _headerptr_0 + _header.Offset;
                            *_headerptr = (byte) SerializationType_._Internal;
                            _headerptr += sizeof(byte);
                            *((uint*)_headerptr) = (uint) (_info.Size + sizeof(ushort) + sizeof(uint));
                            _headerptr += sizeof(uint);
                            *((ushort*)_headerptr) = _info.ClassID;
                            _headerptr += sizeof(ushort);
                            *((uint*)_headerptr) = (uint)_info.HeaderSize;
                        }
                        _header.consume(2 * sizeof(uint) + sizeof(ushort) + sizeof(byte));
                        _message.SerializeTo(ref _header, ref _blocks);
                        IList<ArraySegment<byte>> _buffers = new List<ArraySegment<byte>>();
                        foreach (QS.Fx.Base.Block _block in _blocks)
                        {
                            if (_block.type != QS.Fx.Base.Block.Type.Managed)
                                throw new NotSupportedException("Cannot serialize unmanaged memory.");
                            _buffers.Add(new ArraySegment<byte>(_block.buffer, (int) _block.offset, (int) _block.size));
                        }
                        this._socket.BeginSend(_buffers, SocketFlags.None, null, null);
                    }
                    break;

                case SerializationType_._Xml:
                    throw new NotImplementedException();

                case SerializationType_._Undefined:
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region IHandler Members

        void QS.Fx.Base.IHandler.Handle(QS.Fx.Base.IEvent _event)
        {
            QS._qss_x_.Runtime_.IOperation_ _operation = (QS._qss_x_.Runtime_.IOperation_)_event;
            switch (_operation._Type)
            {
                case OperationType_._Ready:
                case OperationType_._Result:
                    {
                        this._Send(_operation);
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

        #endregion

        #region Object_

        private sealed class Object_ : QS.Fx.Inspection.Inspectable, IDisposable, QS.Fx.Base.IEvent, QS.Fx.Base.IHandler
        {
            #region Constructor

            public Object_(Host_ _host, int _id)
            {
                this._host = _host;
                this._clock = this._host._clock;
                this._alarmclock = this._host._mycontext.Platform.AlarmClock;
                this._scheduler = this._host._mycontext.Platform.Scheduler;
                this._id = _id;
                this._batching = this._host._batching;
                this._checkpointing = this._host._checkpointing;
                this._prevtime = this._clock.Time;
                this._nexttime = this._prevtime + this._batching;

                
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
                    }
                }
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private Host_ _host;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IClock _clock;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IAlarmClock _alarmclock;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Scheduling.IScheduler _scheduler;
            [QS.Fx.Base.Inspectable]
            private int _id;
            [QS.Fx.Base.Inspectable]
            private string _class;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Object.Classes.IObject _object;
            [QS.Fx.Base.Inspectable]
            private Type _underlyingtype;
            [QS.Fx.Base.Inspectable]
            private System.Reflection.ConstructorInfo _constructor;
            [QS.Fx.Base.Inspectable]
            private System.Reflection.MethodInfo _exportmethod;
            [QS.Fx.Base.Inspectable]
            private System.Reflection.MethodInfo _importmethod;
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
            private int _numready;
            [QS.Fx.Base.Inspectable]
            private double _prevtime;
            [QS.Fx.Base.Inspectable]
            private double _nexttime;
            [QS.Fx.Base.Inspectable]
            private double _batching;
            [QS.Fx.Base.Inspectable]
            private double _overhead = -1;
            [QS.Fx.Base.Inspectable]
            private int _reporting;
            [QS.Fx.Base.Inspectable]
            private double _checkpointing;
            [QS.Fx.Base.Inspectable]
            private int _sequenceno;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IAlarm _checkpointalarm;
            [QS.Fx.Base.Inspectable]
            private int _isdirty;
#if PROFILE_1
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _export_times = new QS._qss_c_.Statistics_.Samples2D("Export Times");
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _import_times = new QS._qss_c_.Statistics_.Samples2D("Import Times");
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _invoke_times = new QS._qss_c_.Statistics_.Samples2D("Invoke Times");
            [QS.Fx.Base.Inspectable]
            public QS._qss_c_.Statistics_.Samples2D[] Stats
            {
                get
                {
                    return new QS._qss_c_.Statistics_.Samples2D[] { _export_times, _import_times, _invoke_times };
                }
            }
#endif
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Object.Classes.IObject _kept_object;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Inspection.Inspectable InpectableObject
            {
                get {
                        return (QS.Fx.Inspection.Inspectable)_kept_object;
                    
                }
                
            }

            private QS.Fx.Base.IEvent _next;

            #endregion

            #region _Handle

            

            public void _Handle(IOperation_ _operation)
            {
                switch (_operation._Type)
                {
                    case OperationType_._Create:
                        {
                            this._class = ((Create_)_operation)._Class;
                        }
                        break;

                    case OperationType_._Import:
                        {
#if PROFILE_1
                            double _t1 = this._clock.Time;
#endif
                            QS.Fx.Object.Classes.IObject _receivedobject = ((Import_)_operation)._Object;
                            this._underlyingtype = _receivedobject.GetType();
                            this._constructor = this._underlyingtype.GetConstructor(new Type[] { typeof(QS.Fx.Object.IContext) });
                            if (this._constructor == null)
                                throw new Exception("Cannot spawn replicas; there is no suitable constructor in type \"" + this._underlyingtype.ToString() + "\".");
                            Type _replicatedtype = typeof(QS.Fx.Replication.IReplicated<QS._qss_x_.Object_.Context_.Dummy_>).GetGenericTypeDefinition().MakeGenericType(this._underlyingtype);
                            if (_replicatedtype == null)
                                throw new Exception("Cannot spawn replicas; cannot instantiate the IReplicated interface template.");
                            if (!_replicatedtype.IsAssignableFrom(this._underlyingtype))
                                throw new Exception("Cannot spawn replicas; object type does not derive from IReplicated.");
                            this._importmethod = _replicatedtype.GetMethod("Import");
                            if (this._importmethod == null)
                                throw new Exception("Cannot spawn replicas; cannot find the import method.");
                            this._exportmethod = _replicatedtype.GetMethod("Export");
                            if (this._exportmethod == null)
                                throw new Exception("Cannot spawn replicas; cannot find the export method.");
                            QS.Fx.Object.IContext _replicacontext = new QS._qss_x_.Object_.ReplicaContext_(this._host._mycontext);
                            this._object = (QS.Fx.Object.Classes.IObject) this._constructor.Invoke(new object[] { _replicacontext });
                            if (this._object == null)
                                throw new Exception("Cannot spawn replica; the constructor returned null.");
                            this._exportmethod.Invoke(_receivedobject, new object[] { this._object });
#if PROFILE_1
                            double _t2 = this._clock.Time;
                            this._import_times.Add(_t1, _t2 - _t1);
#endif
                        }
                        break;

                    case OperationType_._Invoke:
                        {
#if PROFILE_1
                            double _t1 = this._clock.Time;
#endif
                            QS.Fx.Base.IEvent _event = ((Invoke_)_operation)._Event;
                            QS.Fx.Internal.I000010 _eventinternal = (QS.Fx.Internal.I000010)_event;
                            if (_eventinternal == null)
                                throw new Exception("Cannot handle a replicated event; missing internal interfaces.");
                            _eventinternal.x(this._object, null);
                            _event.Handle();
                            this._numready++;
                            this._Ready();
                            this._Checkpoint();
#if PROFILE_1
                            double _t2 = this._clock.Time;
                            this._invoke_times.Add(_t1, _t2 - _t1);
#endif
                        }
                        break;

                    case OperationType_._Ready:
                        {
                            this._Ready();
                        }
                        break;

                    case OperationType_._Export:
                        {
#if PROFILE_1
                            double _t1 = this._clock.Time;
#endif
                            if (this._object != null)
                            {
                                int _sequenceno = ((Export_)_operation)._SequenceNo;
                                if (_sequenceno > 0)
                                {
                                    if (_sequenceno != (this._sequenceno + 1))
                                        throw new Exception("Cannot export; wrong sequence number!");
                                    this._sequenceno = _sequenceno;
                                }
                                QS.Fx.Object.Classes.IObject _resultobject = this._object;
                                if (_sequenceno > 0)
                                {
#if KEEP_OBJECT_FOR_PROFILING
                                    this._kept_object = this._object;
#endif
                                    this._object = null;



                                }
                                else
                                {
                                    this._object = (QS.Fx.Object.Classes.IObject)this._constructor.Invoke(null);
                                    if (this._object == null)
                                        throw new Exception("Cannot create the result object; the constructor returned null.");
                                    this._exportmethod.Invoke(_resultobject, new object[] { this._object });
                                }
                                this._isdirty = 0;
                                this._host._mycontext.Enqueue(new Ready_(this._host, this, this._id, this._numready, this._overhead));
                                this._numready = 0;
                                this._host._mycontext.Enqueue(new Result_(this._host, this, this._id, _sequenceno, _resultobject));
#if PROFILE_1
                                double _t2 = this._clock.Time;
                                this._export_times.Add(_t1, _t2 - _t1);
#endif
                            }
                        }
                        break;

                    case OperationType_._Delete:
                    case OperationType_._Result:
                    default:
                        throw new NotImplementedException();
                }
            }

            #endregion

            #region _Ready

            private void _Ready()
            {
                double _timestamp = this._clock.Time;
                double _remaining = this._nexttime - _timestamp;
                if (_remaining > 0)
                {
                    if (Interlocked.Exchange(ref this._reporting, 1) == 0)
                        this._host._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Ready), _remaining));
                }
                else
                {
                    this._reporting = 0;
                    double _overhead = (_timestamp - this._prevtime) / ((double)this._numready);
                    this._prevtime = _timestamp;
                    this._nexttime = this._prevtime + this._batching;
                    if (this._overhead > 0)
                        _overhead = 0.5 * this._overhead + 0.5 * _overhead;
                    this._overhead = _overhead;
                    this._host._mycontext.Enqueue(new Ready_(this._host, this, this._id, this._numready, this._overhead));
                    this._numready = 0;
                }
            }

            private void _Ready(object _o)
            {
                this._alarmclock.Schedule((double)_o, new QS.Fx.Clock.AlarmCallback(this._Ready), null);
            }

            private void _Ready(QS.Fx.Clock.IAlarm _alarmref)
            {
                this._Schedule(new Ready_(null, null, 0, 0, 0));
            }

            #endregion

            #region _Checkpoint

            private void _Checkpoint()
            {
                if ((this._isdirty == 0) && (Interlocked.Exchange(ref this._isdirty, 1) == 0))
                    this._host._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Checkpoint), null));
            }

            private void _Checkpoint(object _o)
            {
                this._checkpointalarm = this._alarmclock.Schedule(this._checkpointing, new QS.Fx.Clock.AlarmCallback(this._Checkpoint), null);
            }

            private void _Checkpoint(QS.Fx.Clock.IAlarm _alarmref)
            {
                this._Schedule(new Export_(null, null, 0, -1));
            }

            #endregion

            #region _Schedule

            public void _Schedule(IOperation_ _operation)
            {
                _operation._Handler = this;
                if (this._disposed == 0)
                {
                    if (_operation.Next != null)
                        throw new Exception("Cannot enqueue an event that links to another event.");
                    QS.Fx.Base.IEvent _myroot;
                    do
                    {
                        _myroot = this._root;
                        _operation.Next = _myroot;
                    }
                    while (Interlocked.CompareExchange<QS.Fx.Base.IEvent>(ref this._root, _operation, _myroot) != _myroot);
                    this._schedule = 1;
                    if ((this._operating == 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0))
                        this._scheduler.Schedule(this);
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

            #region IHandler Members

            void QS.Fx.Base.IHandler.Handle(QS.Fx.Base.IEvent _event)
            {
                this._Handle((IOperation_)_event);
            }

            #endregion
        }

        #endregion
    }
}
