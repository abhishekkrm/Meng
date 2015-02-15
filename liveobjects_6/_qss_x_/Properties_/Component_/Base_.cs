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
#define LOGGER
#define LOGGER_FORWARD

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
    public abstract class Base_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Platform_.IApplication
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        protected Base_(QS.Fx.Object.IContext _mycontext, bool _debug)
        {
            this._mycontext = _mycontext;
            this._debug = _debug;
            if (this._debug)
            {
#if LOGGER
                this._temporary_logger = new QS._qss_c_.Base3_.Logger(null, true, null);
                this._logger = _temporary_logger;
#endif
            }

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_.Constructor");
#endif
            ((QS._qss_x_.Platform_.IApplication)this).Start(this._mycontext.Platform, null);
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Initialize)));
        }

        #endregion

        #region Destructor

        ~Base_()
        {
            this._Dispose(false);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private int _initialized;
        [QS.Fx.Base.Inspectable]
        private int _disposed;
        [QS.Fx.Base.Inspectable]
        private int _schedule;
        [QS.Fx.Base.Inspectable]
        private int _operating;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.IEvent_ _root;
        [QS.Fx.Base.Inspectable]
        private Stack<QS._qss_x_.Properties_.Base_.IEvent_> _todo = new Stack<QS._qss_x_.Properties_.Base_.IEvent_>();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Base3_.Logger _temporary_logger;

        private QS.Fx.Scheduling.IScheduler _scheduler;
        private AsyncCallback _dequeue;

        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        protected bool _debug;
        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Platform.IPlatform _platform;
        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Logging.ILogger _logger;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform _platform, QS._qss_x_.Platform_.IApplicationContext _context)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_.{IApplication}.Start");
#endif

            if (_platform == null)
                _mycontext.Error("Platform is NULL.");

            if ((this._platform != null) && (ReferenceEquals(this._platform, _platform)))
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.Base_.{IApplication}.Start => Component already initalized with the smae platform; igoring...");
#endif
            }
            else
            {
                if (this._initialized < 4)
                {
                    if (Interlocked.CompareExchange(ref this._initialized, 1, 0) == 0)
                    {
                        this._platform = _platform;
                        if (this._debug)
                        {
#if LOGGER
                            string[] _previously_logged =
                                ((QS._core_c_.Base.IReadableLogger)this._temporary_logger).rangeAsArray(
                                    0,
                                    ((QS._core_c_.Base.IReadableLogger)this._temporary_logger).NumberOfMessages);
                            this._temporary_logger = null;
#if LOGGER_FORWARD
                            this._logger = _platform.Logger;
#else
                        this._logger = new QS._qss_c_.Base3_.Logger(this._platform.Clock, true, null);
#endif
                            if (this._logger == null)
                                _mycontext.Error("Logger is NULL.");
                            foreach (string _sss in _previously_logged)
                                this._logger.Log("{backlog} " + _sss);
#endif
                        }
                        this._scheduler = _platform.Scheduler;
                        if (this._scheduler == null)
                            _mycontext.Error("Scheduler is NULL.");
                        this._dequeue = new AsyncCallback(this._Dequeue);
                        if (Interlocked.CompareExchange(ref this._initialized, 2, 1) != 1)
                            _mycontext.Error("Already initialized.");
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Start)));
                    }
                    else
                        _mycontext.Error("Already initializing.");
                }
            }
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_.{IApplication}.Stop");
#endif

            if (this._initialized == 3)
                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Stop)));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        private void _Initialize(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            this._Initialize();
        }

        protected virtual void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Initialize");
#endif
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                this._initialized = 4;
                if (_disposemanagedresources)
                    this._Dispose();
            }
        }

        protected virtual void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Dispose");
#endif
        }

        #endregion

        #region _Start

        private void _Start(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            if (this._initialized < 4)
            {
                if (Interlocked.CompareExchange(ref this._initialized, 3, 2) == 2)
                    this._Start();
            }
        }

        protected virtual void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Start");
#endif
        }

        #endregion

        #region _Stop

        private void _Stop(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            if (this._initialized < 4)
                this._Stop();
        }

        protected virtual void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Stop");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Enqueue

        protected void _Enqueue(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Enqueue\n\n" + QS.Fx.Printing.Printable.ToString(_event) + "\n");
#endif

            if (this._initialized < 4)
            {
                if (_event._Next != null)
                    _mycontext.Error("Cannot enqueue an event that links to another event.");
                QS._qss_x_.Properties_.Base_.IEvent_ _myroot;
                do
                {
                    _myroot = this._root;
                    _event._Next = _myroot;
                }
                while (Interlocked.CompareExchange<QS._qss_x_.Properties_.Base_.IEvent_>(ref this._root, _event, _myroot) != _myroot);
                this._schedule = 1;
                if ((this._initialized > 1) && (this._operating == 0) && (Interlocked.Exchange(ref this._operating, 1) == 0))
                    this._scheduler.BeginExecute(this._dequeue, null);
            }
        }

        #endregion

        #region _Dequeue

        private void _Dequeue(IAsyncResult _o)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Base_._Dequeue");
#endif

            if (this._initialized < 4)
            {
                this._operating = 0;
                while (this._schedule > 0)
                {
                    this._operating = 1;
                    this._schedule = 0;
                    QS._qss_x_.Properties_.Base_.IEvent_ _event = Interlocked.Exchange<QS._qss_x_.Properties_.Base_.IEvent_>(ref this._root, null);
                    if (_event != null)
                    {
                        do
                        {
                            QS._qss_x_.Properties_.Base_.IEvent_ _next = _event._Next;
                            _event._Next = null;
                            _todo.Push(_event);
                            _event = _next;
                        }
                        while (_event != null);
                        while (_todo.Count > 0)
                        {
                            _event = _todo.Pop();
                            _event._Callback(_event);
                        }
                    }
                    this._operating = 0;
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
