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

#define WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("9DCB042DB7C64DC18763F6BB6BB42145")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_Serializer)]
    public sealed class Serializer_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.ISerializer_, QS._qss_x_.Experiment_.Interface_.ISerializer_, QS.Fx.Replication.IReplicated<Serializer_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        static Serializer_()
        {
            if (File.Exists(_filename))
                File.Delete(_filename);
        }

        public Serializer_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("ours", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _ours,
            [QS.Fx.Reflection.Parameter("sched", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _sched,
            [QS.Fx.Reflection.Parameter("lock", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _lock,
            [QS.Fx.Reflection.Parameter("async", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _async) : this(_mycontext)
        {
            this._ours = _ours;
            this._sched = _sched;
            this._lock = _lock;
            this._async = _async;
        }

        public Serializer_(QS.Fx.Object.IContext _mycontext) : this()
        {
            this._mycontext = _mycontext;
            this._serializerendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.ISerializerClient_,
                    QS._qss_x_.Experiment_.Interface_.ISerializer_>(this);
            this._serializecallback = new WaitCallback(this._Serialize);
            this._writecallback = new AsyncCallback(this._WriteCallback);
            this._finished = new ManualResetEvent(false);
        }

        public Serializer_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.ISerializerClient_,
                QS._qss_x_.Experiment_.Interface_.ISerializer_> _serializerendpoint;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _ours;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _sched;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _lock;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _async;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _issubordinate;
        [QS.Fx.Base.Inspectable]
        private Queue<ArraySegment<byte>> _segments = new Queue<ArraySegment<byte>>();
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private BinaryFormatter _formatter;
        [QS.Fx.Base.Inspectable]
        private int _seqno;
        [QS.Fx.Base.Inspectable]
        private FileStream _tempfile;
        [QS.Fx.Base.Inspectable]
        private WaitCallback _serializecallback;
        [QS.Fx.Base.Inspectable]
        private AsyncCallback _writecallback;
        [QS.Fx.Base.Inspectable]
        private int _pending;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _finished;
        [QS.Fx.Base.Inspectable]
        private bool _writing;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializer_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.ISerializerClient_,
                QS._qss_x_.Experiment_.Interface_.ISerializer_>
                    QS._qss_x_.Experiment_.Object_.ISerializer_._Serializer
        {
            get { return this._serializerendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializer_ Members

        private static string _filename = @"C:\temp.dat";

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.ISerializer_._Serialize(object _object)
        {
            if (_ours)
            {
                if (!this._initialized)
                {
                    this._formatter = new BinaryFormatter();
                    this._initialized = true;
                }
                MemoryStream _stream = new MemoryStream();
                this._formatter.Serialize(_stream, _object);
                ArraySegment<byte> _newsegment = new ArraySegment<byte>(_stream.GetBuffer(), 0, (int)_stream.Length);
                if (this._issubordinate)
                    this._segments.Enqueue(_newsegment);
                else
                {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                    if (this._tempfile == null)
                        this._tempfile = File.Create(_filename);
#endif
                    if (this._segments.Count > 0)
                    {
                        foreach (ArraySegment<byte> _segment in this._segments)
                        {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                            this._tempfile.Write(_segment.Array, _segment.Offset, _segment.Count);
#else
                        this._serializerendpoint.Interface._Serialized((++this._seqno), _segment);
#endif
                        }
                        this._segments.Clear();
                    }
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                    this._tempfile.Write(_newsegment.Array, _newsegment.Offset, _newsegment.Count);
#else
                this._serializerendpoint.Interface._Serialized((++this._seqno), _newsegment);
#endif
                }
            }
            else
            {
                Interlocked.Increment(ref this._pending);
                if (this._sched)
                {
                    if (!ThreadPool.QueueUserWorkItem(this._serializecallback, _object))
                    {
                        throw new Exception("Could not queue the work item!");
                    }
                }
                else
                    this._Serialize(_object);
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.ISerializer_._Done()
        {
            if (this._ours)
            {
                if (this._segments.Count > 0)
                {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                    if (this._tempfile == null)
                        this._tempfile = File.Create(_filename);
#endif
                    foreach (ArraySegment<byte> _segment in this._segments)
                    {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                        this._tempfile.Write(_segment.Array, _segment.Offset, _segment.Count);
#else
                        this._serializerendpoint.Interface._Serialized((++this._seqno), _segment);
#endif
                    }
                    this._segments.Clear();
                }
            }
            else
            {
                this._finished.WaitOne();
            }
            this._serializerendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Serialize

        private void _Serialize(object _object)
        {
            BinaryFormatter _formatter = new BinaryFormatter();
            MemoryStream _stream = new MemoryStream();
            _formatter.Serialize(_stream, _object);
            ArraySegment<byte> _newsegment = new ArraySegment<byte>(_stream.GetBuffer(), 0, (int) _stream.Length);
            if (this._lock)
            {
                Monitor.Enter(this);
            }
            try
            {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                if (this._tempfile == null)
                    this._tempfile = File.Create(_filename);
                if (this._async)
                {
                    this._segments.Enqueue(_newsegment);
                    if (!this._writing)
                    {
                        this._writing = true;
                        ArraySegment<byte> _segment = this._segments.Dequeue();
                        this._tempfile.BeginWrite(_segment.Array, _segment.Offset, _segment.Count, this._writecallback, null);
                    }
                }
                else
                {
                    if (this._segments.Count > 0)
                    {
                        foreach (ArraySegment<byte> _segment in this._segments)
                            this._tempfile.Write(_segment.Array, _segment.Offset, _segment.Count);
                        this._segments.Clear();
                    }
                    this._tempfile.Write(_newsegment.Array, _newsegment.Offset, _newsegment.Count);
                }
#else
                if (this._segments.Count > 0)
                {
                    foreach (ArraySegment<byte> _segment in this._segments)
                        this._serializerendpoint.Interface._Serialized((++this._seqno), _segment);
                    this._segments.Clear();
                }

                this._serializerendpoint.Interface._Serialized((++this._seqno), _newsegment);
#endif
            }
            finally
            {
                if (this._lock)
                {
                    Monitor.Exit(this);
                }
            }
            if (!this._async)
            {
                if (Interlocked.Decrement(ref this._pending) <= 0)
                    this._finished.Set();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _WriteCallback

        private void _WriteCallback(IAsyncResult _asyncresult)
        {
            lock (this)
            {
                this._tempfile.EndWrite(_asyncresult);
                if (this._segments.Count > 0)
                {
                    ArraySegment<byte> _segment = this._segments.Dequeue();
                    this._tempfile.BeginWrite(_segment.Array, _segment.Offset, _segment.Count, this._writecallback, null);
                }
                else
                    this._writing = false;
            }
            if (Interlocked.Decrement(ref this._pending) <= 0)
                this._finished.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Serializer_> Members

        void QS.Fx.Replication.IReplicated<Serializer_>.Export(Serializer_ _other)
        {
            _other._issubordinate = true;
        }

        void QS.Fx.Replication.IReplicated<Serializer_>.Import(Serializer_ _other)
        {
            if (this._issubordinate)
            {
                if (_other._segments.Count > 0)
                {
                    foreach (ArraySegment<byte> _segment in _other._segments)
                        this._segments.Enqueue(_segment);
                    _other._segments.Clear();
                }
            }
            else
            {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                if (this._tempfile == null)
                    this._tempfile = File.Create(_filename);
#endif
                if (this._segments.Count > 0)
                {
                    foreach (ArraySegment<byte> _segment in this._segments)
                    {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                        this._tempfile.Write(_segment.Array, _segment.Offset, _segment.Count);
#else
                        this._serializerendpoint.Interface._Serialized((++this._seqno), _segment);
#endif
                    }
                    this._segments.Clear();
                }
                if (_other._segments.Count > 0)
                {
                    foreach (ArraySegment<byte> _segment in _other._segments)
                    {
#if WRITE_SEGMENTS_TO_A_TEMP_FILE_INSTEAD_OF_RESPONDING
                        this._tempfile.Write(_segment.Array, _segment.Offset, _segment.Count);
#else
                        this._serializerendpoint.Interface._Serialized((++this._seqno), _segment);
#endif
                    }
                    _other._segments.Clear();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { throw new NotImplementedException(); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            throw new NotImplementedException();
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
