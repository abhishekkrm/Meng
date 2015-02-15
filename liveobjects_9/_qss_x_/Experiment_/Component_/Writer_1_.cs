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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Writer_1)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Writer_1_
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IUnidirectionalChannel<QS.Fx.Base.Block>,
        QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Writer_1_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("id", QS.Fx.Reflection.ParameterClass.Value)] 
            string _id,
            [QS.Fx.Reflection.Parameter("file", QS.Fx.Reflection.ParameterClass.Value)] 
            string _file,
            [QS.Fx.Reflection.Parameter("temp", QS.Fx.Reflection.ParameterClass.Value)] 
            string _temp,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        )
            : base(_mycontext, _debug)
        {
            this._mycontext = _mycontext;
            this._id = _id;
            this._file = _file;
            this._temp = _temp;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Writer_1_(" + this._id + ")._Constructor");
#endif

            this._endpoint = _mycontext.ExportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _id;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS.Fx.Interface.Classes.ICommunicationChannel<QS.Fx.Base.Block>> _endpoint;
        [QS.Fx.Base.Inspectable]
        private string _file;
        [QS.Fx.Base.Inspectable]
        private string _temp;
        [QS.Fx.Base.Inspectable]
        private FileStream _outstream;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Base.Block> _incoming = new Queue<QS.Fx.Base.Block>();
        [QS.Fx.Base.Inspectable]
        private IAsyncResult _o;
        [QS.Fx.Base.Inspectable]
        private long _written;
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _writing;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Block _chunk;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _done = new ManualResetEvent(false);

#if PROFILE
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
                this._logger.Log("Writer_1_(" + this._id + ")._Initialize");
#endif

            lock (this)
            {
                this._outstream = new FileStream(Path.GetFullPath(this._file), FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
                this._initialized = true;
                if (this._incoming.Count > 0 && !this._writing)
                {
                    this._chunk = this._incoming.Dequeue();
                    this._o = this._outstream.BeginWrite(this._chunk.buffer, (int) this._chunk.offset, (int) this._chunk.size, new AsyncCallback(this._WriteCallback), null);
                    this._writing = true;
                }
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Writer_1_(" + this._id + ")._Dispose");
#endif

            lock (this)
            {
                this._outstream.Flush();
                this._outstream.Close();

#if PROFILE
                if (!Directory.Exists(this._temp))
                    Directory.CreateDirectory(this._temp);

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
                this._logger.Log("Writer_1_(" + this._id + ")._Connect");
#endif
        }

        #endregion

        #region _Disconnect

        private void _Disconnect()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Writer_1_(" + this._id + ")._Disconnect");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Handle

        private void _Handle(QS.Fx.Base.Block _chunk)
        {
            lock (this)
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Writer_1_(" + this._id + ")._Handle(" + _chunk.size.ToString() + ")");
#endif

                this._incoming.Enqueue(_chunk);
                if (this._initialized && !this._writing)
                {
                    this._chunk = this._incoming.Dequeue();
                    this._o = this._outstream.BeginWrite(this._chunk.buffer, (int) this._chunk.offset, (int) this._chunk.size, new AsyncCallback(this._WriteCallback), null);
                    this._writing = true;
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _WriteCallback

        private void _WriteCallback(IAsyncResult _o)
        {
            lock (this)
            {
                this._outstream.EndWrite(_o);
                this._written += (long) this._chunk.size;

#if PROFILE
                double _time = this._platform.Clock.Time;
                _statistics_written.Add(_time, (double) this._written);
#endif

#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Writer_1_(" + this._id + ")._WriteCallback(" + this._written.ToString() + ")");
#endif

                if (this._incoming.Count > 0)
                {
                    this._chunk = this._incoming.Dequeue();
                    this._o = this._outstream.BeginWrite(this._chunk.buffer, (int)this._chunk.offset, (int) this._chunk.size, new AsyncCallback(this._WriteCallback), null);
                    this._writing = true;
                }
                else
                {
                    this._chunk = new QS.Fx.Base.Block();
                    this._o = null;
                    this._writing = false;
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@      
    }
}
