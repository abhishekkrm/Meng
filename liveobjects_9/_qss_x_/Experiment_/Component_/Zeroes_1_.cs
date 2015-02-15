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
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Zeroes_1)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Zeroes_1_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Zeroes_1_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("id", QS.Fx.Reflection.ParameterClass.Value)] 
            string _id,
            [QS.Fx.Reflection.Parameter("length", QS.Fx.Reflection.ParameterClass.Value)] 
            long _length,
            [QS.Fx.Reflection.Parameter("block", QS.Fx.Reflection.ParameterClass.Value)] 
            int _block,
            [QS.Fx.Reflection.Parameter("next", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>> _next,
            [QS.Fx.Reflection.Parameter("temp", QS.Fx.Reflection.ParameterClass.Value)] 
            string _temp,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
            this._mycontext = _mycontext;
            this._id = _id;
            this._length = _length;
            this._block = _block;
            this._next = _next;
            this._temp = _temp;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Zeroes_1_(" + this._id + ")._Constructor");
#endif

            this._next_endpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>>();

            this._chunk = new byte[this._block];
            Array.Clear(this._chunk, 0, this._chunk.Length);
            ThreadPool.QueueUserWorkItem(new WaitCallback(this._ProcessingCallback), null);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _id;
        [QS.Fx.Base.Inspectable]
        private long _length;
        [QS.Fx.Base.Inspectable]
        private string _temp;
        [QS.Fx.Base.Inspectable]
        private int _block;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>> _next;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block> _next_proxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>> _next_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _next_connection;
        [QS.Fx.Base.Inspectable]
        private byte[] _chunk;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Base.Block> _outgoing = new Queue<QS.Fx.Base.Block>();
        [QS.Fx.Base.Inspectable]
        private long _read;

#if PROFILE
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_read =
            new QS._qss_c_.Statistics_.Samples2D(
                "read", "reading progress", "time", "s", "elapsed time in seconds", "read", "bytes", "number of bytes read");
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
            base._Initialize();

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Zeroes_1_(" + this._id + ")._Initialize");
#endif

            lock (this)
            {
                lock (this._outgoing)
                {
                    this._next_proxy = this._next.Dereference(this._mycontext);
                    this._next_connection = this._next_endpoint.Connect(this._next_proxy.Channel);
                    while (this._outgoing.Count > 0)
                        this._next_endpoint.Interface.Message(this._outgoing.Dequeue());
                }
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Zeroes_1_(" + this._id + ")._Dispose");
#endif

            lock (this)
            {
                lock (this._outgoing)
                {
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
                this._logger.Log("Zeroes_1_(" + this._id + ")._Connect");
#endif
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Zeroes_1_(" + this._id + ")._Disconnect");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ProcessingCallback

        private void _ProcessingCallback(object _o)
        {
            Monitor.Enter(this);
            Monitor.Enter(this._outgoing);
            bool _has1 = true;
            bool _has2 = true;
            try
            {
                int _count = (int)Math.Min((long)this._block, (this._length - this._read));
                this._read += (long)_count;
                this._outgoing.Enqueue(new QS.Fx.Base.Block(this._chunk, 0, (uint)_count));

#if PROFILE
                double _time = this._platform.Clock.Time;
                _statistics_read.Add(_time, (double)this._read);
#endif

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Zeroes_1_(" + this._id + ")._ProcessingCallback(" + this._read.ToString() + ")");
#endif

                if (this._read < this._length)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this._ProcessingCallback), null);

                Monitor.Exit(this);
                _has1 = false;

                if (this._next_endpoint.IsConnected)
                {
                    while (this._outgoing.Count > 0)
                        this._next_endpoint.Interface.Message(this._outgoing.Dequeue());
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      
    }
}
