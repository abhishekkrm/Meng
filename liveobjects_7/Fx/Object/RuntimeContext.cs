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
using System.IO;

namespace QS.Fx.Object
{
    public sealed class RuntimeContext : QS.Fx.Inspection.Inspectable, IRuntimeContext
    {
        #region Launcher

        public static QS._qss_c_.Base3_.Constructor<IRuntimeContext> Launcher(string _mytemp, int _concurrency, QS.Fx.Scheduling.Policy _policy, bool _pinthreads)
        {
            return (new Launcher_(_mytemp, _concurrency, _policy, _pinthreads))._Constructor;
        }

        private sealed class Launcher_
        {
            public Launcher_(string _mytemp, int _concurrency, QS.Fx.Scheduling.Policy _policy, bool _pinthreads)
            {
                this._mytemp = _mytemp;
                this._concurrency = _concurrency;
                this._policy = _policy;
                this._pinthreads = _pinthreads;
            }

            private string _mytemp;
            private int _concurrency;
            private QS.Fx.Scheduling.Policy _policy;
            private bool _pinthreads;

            public QS._qss_c_.Base3_.Constructor<IRuntimeContext> _Constructor
            {
                get { return new QS._qss_c_.Base3_.Constructor<IRuntimeContext>(this._ConstructorCallback); }
            }

            private IRuntimeContext _ConstructorCallback()
            {
                return new RuntimeContext(this._mytemp, this._concurrency, this._policy, this._pinthreads);
            }
        }

        #endregion

        #region Constructor

        public RuntimeContext(string _mytemp, int _concurrency, QS.Fx.Scheduling.Policy _policy, bool _pinthreads)
        {
            string _workingroot = Path.Combine(_mytemp, ".tmp1");
            string _fsroot = Path.Combine(_mytemp, ".tmp2");
            string _outputfile = Path.Combine(_mytemp, "quicksilver.txt");
            Directory.CreateDirectory(_workingroot);
            Directory.CreateDirectory(_fsroot);
            if (_concurrency > 0)
                this._queue = new QS.Fx.Scheduling.MultithreadedQueue(_concurrency, _policy, _pinthreads);
            else
                this._queue = new QS.Fx.Scheduling.SinglethreadedQueue();
            this._core = new QS._core_c_.Core.Core(_workingroot, this._queue, _pinthreads);

            QS.Fx.Logging.IConsole console = null;
            if (QS.Fx.Object.Runtime.LogFile != null)
                console = new QS.Fx.Object.FileConsole(QS.Fx.Object.Runtime.LogFile);
            this._logger = new QS._qss_c_.Base3_.Logger(this._core, true, console);

            this._platform = new QS._qss_x_.Platform_.PhysicalPlatform(_logger, null, _core, _fsroot);
            this._core.Start();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Scheduling.IQueue _queue;
        [QS.Fx.Base.Inspectable]
        private QS._core_c_.Base.IReadableLogger _logger;
        [QS.Fx.Base.Inspectable]
        private QS._core_c_.Core.ICore _core;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Platform_.PhysicalPlatform _platform;

        #endregion

        #region IContext Members

        QS.Fx.Platform.IPlatform IRuntimeContext.Platform
        {
            get { return this._platform; }
        }

        void IRuntimeContext.Start()
        {
            this._core.Start();
        }

        void IRuntimeContext.Stop()
        {
            this._core.Stop();
        }

        void IRuntimeContext.Release()
        {
            this._core.Stop();
            this._core.Dispose();
            if (this._queue is System.IDisposable)
                ((System.IDisposable)this._queue).Dispose();
        }

        QS.Fx.Logging.IConsole IRuntimeContext.Console
        {
            get { return this._logger.Console; }
            set { this._logger.Console = value; }
        }

        #endregion
    }
}
