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

// #define VERBOSE
#define PROFILE

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Encryptor_1)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Encryptor_1_  
        : QS._qss_x_.Properties_.Component_.Base_, 
        QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Encryptor_1_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("id", QS.Fx.Reflection.ParameterClass.Value)] 
            string _id,
            [QS.Fx.Reflection.Parameter("mode", QS.Fx.Reflection.ParameterClass.Value)] 
            string _mode,
            [QS.Fx.Reflection.Parameter("type", QS.Fx.Reflection.ParameterClass.Value)] 
            string _type,
            [QS.Fx.Reflection.Parameter("iv", QS.Fx.Reflection.ParameterClass.Value)] 
            byte[] _iv,
            [QS.Fx.Reflection.Parameter("key", QS.Fx.Reflection.ParameterClass.Value)] 
            byte[] _key,
            [QS.Fx.Reflection.Parameter("next", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>> _next,
            [QS.Fx.Reflection.Parameter("async", QS.Fx.Reflection.ParameterClass.Value)] 
            string _async,
            [QS.Fx.Reflection.Parameter("temp", QS.Fx.Reflection.ParameterClass.Value)] 
            string _temp,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
        : base(_mycontext, _debug)
        {
            this._mycontext = _mycontext;
            this._id = _id;
            _mode = _mode.ToLower();
            this._mode = _mode;
            _type = _type.ToLower();
            this._type = _type;
            this._iv = _iv;
            this._key = _key;
            this._next = _next;
            this._temp = _temp;
            _async = _async.ToLower();
            this._async = _async;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Encryptor_1_(" + this._id + ")._Constructor");
#endif

            if (_async.Equals("no"))
                this._async_mode = 0;
            else if (_async.Equals("threadpool"))
                this._async_mode = 1;
            else if (_async.Equals("liveobjects"))
                this._async_mode = 2;
            else
                throw new NotImplementedException();

            this._endpoint = _mycontext.ExportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>>(this);
            this._next_endpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>>();

            if (_type.Equals("rijndael"))
                this._algorithm = new RijndaelManaged();
            else if (_type.Equals("aes"))
                this._algorithm = new AesCryptoServiceProvider();
            else if (_type.Equals("des"))
                this._algorithm = new DESCryptoServiceProvider();
            else if (_type.Equals("3des"))
                this._algorithm = new TripleDESCryptoServiceProvider();
            else if (_type.Equals("rc2"))
                this._algorithm = new RC2CryptoServiceProvider();
            else
                throw new NotImplementedException();

            if (_mode.Equals("encrypt"))
                this._transform = _algorithm.CreateEncryptor(this._key, this._iv);
            else if (_mode.Equals("decrypt"))
                this._transform = _algorithm.CreateDecryptor(this._key, this._iv);
            else
                throw new NotImplementedException();

            this._outstream = new MemoryStream();
            this._instream = new CryptoStream(this._outstream, this._transform, CryptoStreamMode.Write);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _id;
        [QS.Fx.Base.Inspectable]
        private string _temp;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>> _endpoint;
        [QS.Fx.Base.Inspectable]
        private string _mode;
        [QS.Fx.Base.Inspectable]
        private string _type;
        [QS.Fx.Base.Inspectable]
        private byte[] _iv;
        [QS.Fx.Base.Inspectable]
        private byte[] _key;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>> _next;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block> _next_proxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>> _next_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _next_connection;
        [QS.Fx.Base.Inspectable]
        private SymmetricAlgorithm _algorithm;
        [QS.Fx.Base.Inspectable]
        private ICryptoTransform _transform;
        [QS.Fx.Base.Inspectable]
        private CryptoStream _instream;
        [QS.Fx.Base.Inspectable]
        private MemoryStream _outstream;
        [QS.Fx.Base.Inspectable]
        private long _read;
        [QS.Fx.Base.Inspectable]
        private long _written;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Base.Block> _outgoing = new Queue<QS.Fx.Base.Block>();
        [QS.Fx.Base.Inspectable]
        private string _async;
        [QS.Fx.Base.Inspectable]
        private int _async_mode;

#if PROFILE
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_read =
            new QS._qss_c_.Statistics_.Samples2D(
                "read", "reading progress", "time", "s", "elapsed time in seconds", "read", "bytes", "number of bytes read");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_written =
            new QS._qss_c_.Statistics_.Samples2D(
                "written", "writing progress", "time", "s", "elapsed time in seconds", "written", "bytes", "number of bytes written");
#endif

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #endregion

        #region IUnidirectionalChannel<byte[]> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<
            QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>>
                QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>.Channel
        {
            get { return this._endpoint; }
        }

        #endregion

        #region ICommunicationChannel<byte[]> Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
        void QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>.Message(QS.Fx.Base.Block _message)
        {
            this._Handle(_message);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
            base._Initialize();

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Encryptor_1_(" + this._id + ")._Initialize");
#endif

            lock (this)
            {
                lock (this._outgoing)
                {
                    this._next_proxy = this._next.Dereference(this._mycontext);
                    this._next_connection = this._next_endpoint.Connect(this._next_proxy.Channel);
                    while (this._outgoing.Count > 0)
                    {
                        QS.Fx.Base.Block _chunk = this._outgoing.Dequeue();
                        this._written += (long)_chunk.size;

#if PROFILE
                        double _time = this._platform.Clock.Time;
                        _statistics_written.Add(_time, (double)this._written);
#endif

                        this._next_endpoint.Interface.Message(_chunk);
                    }
                }
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Encryptor_1_(" + this._id + ")._Dispose");
#endif

            lock (this)
            {
                lock (this._outgoing)
                {
                    try
                    {
                        this._instream.Flush();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        this._instream.FlushFinalBlock();
                    }
                    catch (Exception)
                    {
                    }

                    if (this._outstream.Length > 0)
                        this._outgoing.Enqueue(new QS.Fx.Base.Block(this._outstream.ToArray()));

                    try
                    {
                        this._instream.Close();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        this._instream.Dispose();
                    }
                    catch (Exception)
                    {
                    }

                    this._instream = null;

                    this._outstream.Close();

                    this._outstream = null;

                    if (this._next_endpoint.IsConnected)
                    {
                        while (this._outgoing.Count > 0)
                        {
                            QS.Fx.Base.Block _chunk = this._outgoing.Dequeue();
                            this._written += (long)_chunk.size;

#if PROFILE
                            double _time = this._platform.Clock.Time;
                            _statistics_written.Add(_time, (double)this._written);
#endif

                            this._next_endpoint.Interface.Message(_chunk);
                        }
                    }

                    if (this._next_endpoint.IsConnected)
                        this._next_endpoint.Disconnect();
                    if ((this._next_proxy != null) && (this._next_proxy is IDisposable))
                        ((IDisposable)this._next_proxy).Dispose();

#if PROFILE
                    if (!Directory.Exists(this._temp))
                        Directory.CreateDirectory(this._temp);

                    using (StreamWriter _w = new StreamWriter(Path.Combine(this._temp, "read.txt"), false))
                    {
                        foreach (QS._core_e_.Data.XY _sample in this._statistics_read.Samples)
                        {
                            _w.Write(_sample.x.ToString());
                            _w.Write("\t");
                            _w.WriteLine(_sample.y.ToString());
                        }
                    }

                    using (StreamWriter _w = new StreamWriter(Path.Combine(this._temp, "written.txt"), false))
                    {
                        foreach (QS._core_e_.Data.XY _sample in this._statistics_written.Samples)
                        {
                            _w.Write(_sample.x.ToString());
                            _w.Write("\t");
                            _w.WriteLine(_sample.y.ToString());
                        }
                    }
#endif
                }

                base._Dispose();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        private void _Connect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Encryptor_1_(" + this._id + ")._Connect");
#endif
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Encryptor_1_(" + this._id + ")._Disconnect");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Handle

        private void _Handle(QS.Fx.Base.Block _chunk)
        {
            Monitor.Enter(this);
            bool _has1 = true;
            bool _has2 = false;
            try
            {
                if ((_chunk.size > 0) && (this._instream != null))
                {
                    this._instream.Write(_chunk.buffer, (int)_chunk.offset, (int)_chunk.size);
                    this._instream.Flush();
                    this._read += (long)_chunk.size;

#if PROFILE
                    double _time = this._platform.Clock.Time;
                    _statistics_read.Add(_time, (double)this._read);
#endif
                }

                Monitor.Enter(this._outgoing);
                _has2 = true;

                if (this._outstream != null)
                {
                    if (this._outstream.Length > 0)
                    {
                        this._outgoing.Enqueue(new QS.Fx.Base.Block(this._outstream.ToArray()));
                        this._outstream.Seek(0, SeekOrigin.Begin);
                        this._outstream.SetLength(0);
                    }
                }

                Monitor.Exit(this);
                _has1 = false;

                switch (this._async_mode)
                {
                    case 0:
                        this._Handle_0();
                        break;
                    case 1:
                        ThreadPool.QueueUserWorkItem(new WaitCallback(this._Handle_1));
                        break;
                    case 2:
                        this._mycontext.Enqueue(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(this._Handle_1)));
                        break;
                    default:
                        throw new NotImplementedException();
                }

                Monitor.Exit(this._outgoing);
                _has2 = false;
            }
            finally
            {
                if (_has1)
                    Monitor.Exit(this);
                if (_has2)
                    Monitor.Exit(this._outgoing);
            }
        }

        #endregion

        #region _Handle_0

        private void _Handle_0()
        {
            lock (this._outgoing)
            {
                if (this._next_endpoint.IsConnected)
                {
                    while (this._outgoing.Count > 0)
                    {
                        QS.Fx.Base.Block _chunk = this._outgoing.Dequeue();
                        this._written += (long)_chunk.size;

#if PROFILE
                        double _time = this._platform.Clock.Time;
                        _statistics_written.Add(_time, (double)this._written);
#endif

                        this._next_endpoint.Interface.Message(_chunk);
                    }
                }
            }
        }

        #endregion

        #region _Handle_1

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
        private void _Handle_1(object _o)
        {
            this._Handle_0();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      
    }
}
