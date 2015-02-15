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

// #define REPORT_TRANSFERS_VERBOSE
// #define REPORT_THE_BEGINNING_OF_IMPORTING
// #define REPORT_THE_END_OF_PROCESSING
#define TRANSFER_ENABLED
// #define NO_OPTIMIZATION_IN_THE_SCHEDULER_CODE
#define DEBUG
//#define PROFILE
// #define PROFILE_2
// #define VERBOSE_IMPORT
//#define PROFILE_ENQUEUE
//#define PROFILE_DEQUEUE
#define CHECK_REPLICATE
#define CHECK_HANDLE
#define LEAVE_REPLICAS
//#define PROFILE_REPLICAS
//#define PROFILE_IMPORTING

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace QS._qss_x_.Object_
{
    public sealed class Context_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.IContext, QS._qss_x_.Object_.IInternal_, IDisposable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        internal Context_(QS.Fx.Platform.IPlatform _platform, ErrorHandling_ _errorhandling, QS._qss_x_.Registry_._Registry _registry,
            QS.Fx.Base.SynchronizationOption _defaultsynchronizationoption, QS.Fx.Base.SynchronizationOption _synchronizationoption)
        {
            this._serializationoption = QS.Fx.Object.Runtime.SerializationType;
            this._errorhandling = _errorhandling;
            this._defaultsynchronizationoption = _defaultsynchronizationoption;
            this._synchronizationoption = QS.Fx.Base.Synchronization.CombineOptions(this._defaultsynchronizationoption, _synchronizationoption);
            this._replicated = ((this._synchronizationoption & QS.Fx.Base.SynchronizationOption.Replicated) == QS.Fx.Base.SynchronizationOption.Replicated);
            this._platform = _platform;
            if (this._platform == null)
                throw new Exception("Platform is NULL.");
            this._clock = this._platform.Clock;
            this._toprocess =
                ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.ToProcess) == QS.Fx.Replication.Policy.ToProcess);
            this._tomachine =
                ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.ToMachine) == QS.Fx.Replication.Policy.ToMachine);
            this._tonetwork =
                ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.ToNetwork) == QS.Fx.Replication.Policy.ToNetwork);
            this._replicated = this._replicated & (this._toprocess | this._tomachine | this._tonetwork);
            if (this._toprocess)
            {
                this._numprocess = QS.Fx.Object.Runtime.NumberOfReplicas;
                if (this._numprocess == 0)
                    this._numprocess = QS.Fx.Object.Runtime.Concurrency;
            }
            if (this._tomachine)
                this._nummachine = QS.Fx.Object.Runtime.NumberOfWorkers;

            this._aggressive =
                ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.Aggressive) == QS.Fx.Replication.Policy.Aggressive);
            this._bypass =
                ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.Bypass) == QS.Fx.Replication.Policy.Bypass);
            this._transferenabled = ((QS.Fx.Object.Runtime.Replication & QS.Fx.Replication.Policy.Transfer) == QS.Fx.Replication.Policy.Transfer);
            this._transfertimeout1 = QS.Fx.Object.Runtime.TransferTimeout;
            this._transfertimeout2 = Math.Min(0.1f, this._transfertimeout1 / 10.0f); 
            this._queue1 = new Queue_(this, _platform, QS.Fx.Base.SynchronizationOption.Singlethreaded);
            this._queue2 = new Queue_(this, _platform, QS.Fx.Base.SynchronizationOption.Multithreaded | (this._replicated ? QS.Fx.Base.SynchronizationOption.Replicated : QS.Fx.Base.SynchronizationOption.None));
#if DEBUG
            this._logger = new QS._qss_c_.Base3_.Logger(_platform.Clock, true, null);
#endif
            if (_registry != null)
                this._registry = _registry;
            else
            {
                this._registry = new QS._qss_x_.Registry_._Registry(this);
                this._registry._Initialize();
            }
            if (QS.Fx.Object.Runtime.FlowControlEnabled)
            {
                this._maxevents = 5;
                this._batching = QS.Fx.Object.Runtime.FlowControlInterval;
            }
            else
            {
                this._maxevents = 100000000;
                this._batching = 100000000;
            }
        }

        internal Context_(QS.Fx.Platform.IPlatform _platform, ErrorHandling_ _errorhandling,
            QS.Fx.Base.SynchronizationOption _defaultsynchronizationoption, QS.Fx.Base.SynchronizationOption _synchronizationoption)
            : this(_platform, _errorhandling, null, _defaultsynchronizationoption, _synchronizationoption)
        {
        }

        internal Context_(Context_ _mycontext, QS.Fx.Base.SynchronizationOption _synchronizationoption)
            : this(_mycontext._platform, _mycontext._errorhandling, _mycontext._registry, _mycontext._defaultsynchronizationoption, _synchronizationoption)
        {
        }

        #endregion

        #region Destructor

        ~Context_()
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
                    ((IDisposable)this._queue1).Dispose();
                    ((IDisposable)this._queue2).Dispose();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Debugging Stuff

        public static bool _HasOptimizations
        {
            get
            {
#if NO_OPTIMIZATION_IN_THE_SCHEDULER_CODE
                return false;
#else
                return true;
#endif
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private bool _transferenabled;
        [QS.Fx.Base.Inspectable]
        private double _transfertimeout1 = 1;
        [QS.Fx.Base.Inspectable]
        private double _transfertimeout2 = 0.1;
        [QS.Fx.Base.Inspectable]
        private int _serializationoption;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.SynchronizationOption _defaultsynchronizationoption;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.SynchronizationOption _synchronizationoption;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Registry_._Registry _registry;
        [QS.Fx.Base.Inspectable]
        private Queue_ _queue1;
        [QS.Fx.Base.Inspectable]
        private Queue_ _queue2;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Platform.IPlatform _platform;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private int _disposed;
#if DEBUG
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Logging.ILogger _logger;
#endif
        [QS.Fx.Base.Inspectable]
        private ErrorHandling_ _errorhandling;

        [QS.Fx.Base.Inspectable]
        private bool _replicated;
        [QS.Fx.Base.Inspectable]
        private int _concurrency;
        [QS.Fx.Base.Inspectable]
        private string _class;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IObject _object;
        [QS.Fx.Base.Inspectable]
        private Type _underlyingtype;
        [QS.Fx.Base.Inspectable]
        private bool _preparedforreplication;
        [QS.Fx.Base.Inspectable]
        private System.Reflection.ConstructorInfo _constructor;
        [QS.Fx.Base.Inspectable]
        private System.Reflection.MethodInfo _exportmethod;
        [QS.Fx.Base.Inspectable]
        private System.Reflection.MethodInfo _importmethod;
        [QS.Fx.Base.Inspectable]
        private int _numreplicas;
        [QS.Fx.Base.Inspectable]
        private int _maxreplicas;
        [QS.Fx.Base.Inspectable]
        private int _replicaindex;
        [QS.Fx.Base.Inspectable]
#if PROFILE_REPLICAS
        public Replica_[] _replicas;
#else
        private Replica_[] _replicas;
#endif
        [QS.Fx.Base.Inspectable]
        private bool _toprocess;
        [QS.Fx.Base.Inspectable]
        private bool _tomachine;
        [QS.Fx.Base.Inspectable]
        private bool _tonetwork;
        [QS.Fx.Base.Inspectable]
        private int _numprocess;
        [QS.Fx.Base.Inspectable]
        private int _nummachine;
        [QS.Fx.Base.Inspectable]
        private int _numnetwork;
        [QS.Fx.Base.Inspectable]
        private bool _aggressive;
        [QS.Fx.Base.Inspectable]
        private bool _bypass;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Runtime_.IRemoteContext_ _remotecontext;
        [QS.Fx.Base.Inspectable]
        private int _minevents = 3;
        [QS.Fx.Base.Inspectable]
        private int _maxevents;
        [QS.Fx.Base.Inspectable]
        private double _batching;
        [QS.Fx.Base.Inspectable]
        private int _inreplicate;
#if PROFILE_IMPORTING
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _importingseries = new QS._qss_c_.Statistics_.Samples2D();
#endif
        [QS.Fx.Base.Inspectable]
        private bool _isimporting;
        [QS.Fx.Base.Inspectable]
        private Replica_.Transfer_ _transferred;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ErrorHandling_

        public enum ErrorHandling_
        {
            Log = 0, Exception = 1, Halt = 2
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IInternal Members

        QS.Fx.Base.SynchronizationOption QS.Fx.Object.IInternal.SynchronizationOption
        {
            get { return this._synchronizationoption; }
        }

        void QS.Fx.Object.IInternal.Enqueue(QS.Fx.Base.IEvent _event)
        {
            if ((_event.SynchronizationOption & QS.Fx.Base.SynchronizationOption.Concurrent) != QS.Fx.Base.SynchronizationOption.Concurrent)
            {
                if ((_event.SynchronizationOption & QS.Fx.Base.SynchronizationOption.Multithreaded) != QS.Fx.Base.SynchronizationOption.Multithreaded)
                {
                    if (this._replicated)
                        throw new Exception("Replicated objects cannot have singlethreaded code.");
                    else
                        this._queue1._Schedule(_event);
                }
                else
                {
                    bool _schedulenow = true;
                    if (this._replicated && this._bypass && (this._numreplicas > 0) &&
                        ((_event.SynchronizationOption & QS.Fx.Base.SynchronizationOption.Replicated) == QS.Fx.Base.SynchronizationOption.Replicated))
                    {
                        int _concurrency = this._concurrency;
                        if ((_concurrency > 1) && Interlocked.CompareExchange(ref this._concurrency, _concurrency + 1, _concurrency) == _concurrency)
                        {
                            if (this._numreplicas > 0)
                            {
                                int _replicaindex = (Interlocked.Increment(ref this._replicaindex) - 1) % this._numreplicas;
                                _event.Next = null;
                                this._replicas[_replicaindex]._Schedule(_event);
                                _schedulenow = false;
                            }
                            else
                                throw new Exception("Race condition in the scheduler; the number of object replicas has dropped to 0 while concurrency level is still greater than 0.");
                        }
                    }
                    if (_schedulenow)
                        this._queue2._Schedule(_event);
                }
            }
            else
                this._platform.Scheduler.Schedule(_event);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IInternal_

        string IInternal_._Class
        {
            get { return this._class; }
            set
            {
                if (Interlocked.CompareExchange<string>(ref this._class, value, null) != null)
                    throw new Exception("Cannot register more than one reference with a single context.");
            }
        }

        QS.Fx.Object.Classes.IObject IInternal_._Object
        {
            get { return this._object; }
            set
            {
                if (Interlocked.CompareExchange<QS.Fx.Object.Classes.IObject>(ref this._object, value, null) != null)
                    throw new Exception("Cannot register more than one object with a single context.");
            }
        }

        QS._qss_x_.Runtime_.IRemoteContext_ IInternal_._RemoteContext
        {
            get { return this._remotecontext; }
            set
            {
                if (Interlocked.CompareExchange<QS._qss_x_.Runtime_.IRemoteContext_>(ref this._remotecontext, value, null) != null)
                    throw new Exception("Cannot register more than one remote context with a single context.");
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Registry

        internal QS._qss_x_.Registry_._Registry _Registry
        {
            get { return this._registry; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IContext Members

        QS.Fx.Base.SynchronizationOption QS.Fx.Object.IContext.SynchronizationOption
        {
            get { return this._synchronizationoption; }
        }

        void QS.Fx.Object.IContext.Enqueue(QS.Fx.Base.IEvent _event)
        {
            if ((_event.SynchronizationOption & QS.Fx.Base.SynchronizationOption.Multithreaded) != QS.Fx.Base.SynchronizationOption.Multithreaded)
                this._queue1._Schedule(_event);
            else
                this._queue2._Schedule(_event);
        }

        QS.Fx.Platform.IPlatform QS.Fx.Object.IContext.Platform
        {
            get { return this._platform; }
        }

        void QS.Fx.Object.IContext.Error(string s, Exception e)
        {
            switch (this._errorhandling)
            {
                case ErrorHandling_.Exception:
                    {
                        if (s != null)
                            if (e != null)
                                throw new Exception(s, e);
                            else
                                throw new Exception(s);
                        else
                            if (e != null)
                                throw new Exception("Exception", e);
                            else
                                throw new Exception("Exception");
                    }
                    break;

                case ErrorHandling_.Log:
                    {
                        StringBuilder _sb = new StringBuilder();
                        if (s != null)
                        {
                            _sb.AppendLine(s);
                            _sb.AppendLine();
                        }
                        if (e != null)
                        {
                            bool _isfirst = true;
                            do
                            {
                                _sb.AppendLine();
                                if (_isfirst)
                                    _isfirst = false;
                                else
                                {
                                    _sb.AppendLine(new string('-', 80));
                                    _sb.AppendLine();
                                }
                                _sb.AppendLine(e.ToString());
                                e = e.InnerException;
                            }
                            while (e != null);
                        }
                        this._logger.Log(_sb.ToString());
                    }
                    break;

                case ErrorHandling_.Halt:
                    {
                        (new QS._qss_x_.Base1_.ExceptionForm("Exception", s, e)).Show();
                        if (s != null)
                            if (e != null)
                                throw new Exception(s, e);
                            else
                                throw new Exception(s);
                        else
                            if (e != null)
                                throw new Exception("Exception", e);
                            else
                                throw new Exception("Exception");
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void QS.Fx.Object.IContext.Error(string s)
        {
            ((QS.Fx.Object.IContext)this).Error(s, null);
        }

        void QS.Fx.Object.IContext.Error(Exception e)
        {
            ((QS.Fx.Object.IContext)this).Error(null, e);
        }

        bool QS.Fx.Object.IContext.CanCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            if (_proxy != null)
            {
                if (_proxy is ObjectClass)
                    return true;
                else
                {
                    QS.Fx.Reflection.IObjectClass _objectclass = QS._qss_x_.Reflection_.Library.ObjectClassOfComponent(_proxy.GetType());
                    QS.Fx.Reflection.IObjectClass _resultclass = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                    return _objectclass.IsSubtypeOf(_resultclass);
                }
            }
            else
                return true;
        }

        ObjectClass QS.Fx.Object.IContext.SafeCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            if (_proxy != null)
            {
                ObjectClass _result = _proxy as ObjectClass;
                if (_result != null)
                    return _result;
                else
                {
                    QS.Fx.Reflection.IObjectClass _objectclass = QS._qss_x_.Reflection_.Library.ObjectClassOfComponent(_proxy.GetType());
                    QS.Fx.Reflection.IObjectClass _resultclass = QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass));
                    if (_objectclass.IsSubtypeOf(_resultclass))
                        return (ObjectClass)QS._qss_x_.Reflection_.Internal_._internal._cast_object(_proxy, _objectclass.UnderlyingType, typeof(ObjectClass), false);
                    else
                        return null;
                }
            }
            else
                return null;
        }

        ObjectClass QS.Fx.Object.IContext.UnsafeCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            if (_proxy != null)
            {
                ObjectClass _result = _proxy as ObjectClass;
                if (_result != null)
                    return _result;
                else
                {
                    QS.Fx.Reflection.IObjectClass _objectclass = QS._qss_x_.Reflection_.Library.ObjectClassOfComponent(_proxy.GetType());
                    return (ObjectClass)QS._qss_x_.Reflection_.Internal_._internal._cast_object(_proxy, _objectclass.UnderlyingType, typeof(ObjectClass), false);
                }
            }
            else
                return null;
        }

        QS.Fx.Endpoint.Internal.IExportedInterface<I> QS.Fx.Object.IContext.ExportedInterface<I>(I _exportedinterface)
        {
            if ((this._synchronizationoption & QS.Fx.Base.SynchronizationOption.Asynchronous) == QS.Fx.Base.SynchronizationOption.Asynchronous)
                _exportedinterface = QS._qss_x_.Reflection_.Internal_._internal._inject_interceptor<I>(_exportedinterface, this);
            return new QS._qss_x_.Endpoint_.Internal_.ExportedInterface_<I>(_exportedinterface);
        }

        QS.Fx.Endpoint.Internal.IImportedInterface<I> QS.Fx.Object.IContext.ImportedInterface<I>()
        {
            return new QS._qss_x_.Endpoint_.Internal_.ImportedInterface_<I>();
        }

        QS.Fx.Endpoint.Internal.IDualInterface<I, J> QS.Fx.Object.IContext.DualInterface<I, J>(J _exportedinterface)
        {
            if ((this._synchronizationoption & QS.Fx.Base.SynchronizationOption.Asynchronous) == QS.Fx.Base.SynchronizationOption.Asynchronous)
                _exportedinterface = QS._qss_x_.Reflection_.Internal_._internal._inject_interceptor<J>(_exportedinterface, this);
            return new QS._qss_x_.Endpoint_.Internal_.DualInterface_<I, J>(_exportedinterface);
        }

        QS.Fx.Endpoint.Internal.IExportedUI QS.Fx.Object.IContext.ExportedUI(System.Windows.Forms.Control _ui)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ExportedUI_(_ui);
        }

        QS.Fx.Endpoint.Internal.IImportedUI QS.Fx.Object.IContext.ImportedUI(System.Windows.Forms.Control _containerui)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ImportedUI_(_containerui);
        }

        QS.Fx.Endpoint.Internal.IExportedUI_X QS.Fx.Object.IContext.ExportedUI_X(
            QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback,
            QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback,
            QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ExportedUI_X_(_repositioncallback, _updatecallback, _drawcallback);
        }

        QS.Fx.Endpoint.Internal.IImportedUI_X QS.Fx.Object.IContext.ImportedUI_X
        (
#if XNA
            Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService _graphicsdevice, 
#endif
QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback
        )
        {
            return new QS._qss_x_.Endpoint_.Internal_.ImportedUI_X_
            (
#if XNA
                _graphicsdevice, 
#endif
_contentcallback
            );
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Dummy_

        internal sealed class Dummy_ : QS.Fx.Replication.IReplicated<Dummy_>
        {
            public Dummy_()
            {
            }

            void QS.Fx.Replication.IReplicated<Dummy_>.Import(Dummy_ other)
            {
            }

            void QS.Fx.Replication.IReplicated<Dummy_>.Export(Dummy_ other)
            {
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _PrepareForReplication

        private void _PrepareForReplication()
        {
            if (this._object == null)
                throw new Exception("Cannot spawn replicas; no object registered with this context!");
            this._underlyingtype = this._object.GetType();
            this._constructor = this._underlyingtype.GetConstructor(new Type[] { typeof(QS.Fx.Object.IContext) });
            if (this._constructor == null)
                throw new Exception("Cannot spawn replicas; there is no suitable constructor in type \"" + this._underlyingtype.ToString() + "\".");
            Type _replicatedtype = typeof(QS.Fx.Replication.IReplicated<Dummy_>).GetGenericTypeDefinition().MakeGenericType(this._underlyingtype);
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
            this._numreplicas = 0;
            this._maxreplicas = this._numprocess + this._nummachine + this._numnetwork;
            if (this._tomachine || this._tonetwork)
            {
                if (this._remotecontext == null)
                    throw new Exception("Remote context not set.");
            }
            this._replicas = new Replica_[this._maxreplicas];
            this._preparedforreplication = true;
        }

        #endregion

        #region _CreateNewReplica

        private void _CreateNewReplica()
        {
            QS.Fx.Object.IContext _replicacontext = new ReplicaContext_(this);
            QS.Fx.Object.Classes.IObject _newobject = (QS.Fx.Object.Classes.IObject) this._constructor.Invoke(new object[] { _replicacontext });
            if (_newobject == null)
                throw new Exception("Cannot spawn replica; the constructor returned null.");
            this._exportmethod.Invoke(this._object, new object[] { _newobject });
            Queue_ _newqueue = new Queue_(
                this,
                this._platform,
                QS.Fx.Base.SynchronizationOption.Asynchronous |
                QS.Fx.Base.SynchronizationOption.Multithreaded |
                QS.Fx.Base.SynchronizationOption.Serialized);
            bool _isremote = this._numreplicas >= this._numprocess;
            // bool _isnetwork = this._numreplicas >= (this._numprocess + this._nummachine);
            QS._qss_x_.Runtime_.IObject_ _remoteobject = null;
            if (_isremote)
            {
                QS._qss_x_.Runtime_.IClient_ _client = this._remotecontext._Clients[(this._numreplicas - this._numprocess) % this._remotecontext._Clients.Length];
                _remoteobject = _client._Object(this._class);
            }
            int _replicaindex = this._numreplicas;
            int _parentindex = (_replicaindex / 2) - 1;
            Replica_ _parentreplica = null;
            if (_parentindex >= 0)
                _parentreplica = this._replicas[_parentindex];
            Replica_ _newreplica = new Replica_(this, _newobject, _newqueue, _isremote, _remoteobject, _parentreplica);
            this._replicas[_replicaindex] = _newreplica;
            Interlocked.Increment(ref this._numreplicas);
        }

        #endregion

        #region _Replicate

        private void _Replicate(QS.Fx.Base.IEvent _event, out bool _handled, out bool _blocked)
        {
#if CHECK_REPLICATE
            if (Interlocked.Increment(ref this._inreplicate) != 1)
                throw new Exception("Race condition in Context_._Replicate (on enter)");
#endif
            if ((_event.SynchronizationOption & QS.Fx.Base.SynchronizationOption.Replicated) == QS.Fx.Base.SynchronizationOption.Replicated)
            {
                this._isimporting = false;
                if (Interlocked.Increment(ref this._concurrency) == 1)
                {
                    if (!this._preparedforreplication)
                        this._PrepareForReplication();
                    while (this._numreplicas < this._maxreplicas)
                        _CreateNewReplica();
                }
                if (this._numreplicas < 1)
                    throw new Exception("Cannot schedule event; no replicas have been spawned.");
                int _replicaindex;
                _handled = false;
                for (int _i = 0; !_handled && (_i < this._numreplicas); _i++)
                {
                    _replicaindex = (Interlocked.Increment(ref this._replicaindex) - 1) % this._numreplicas;
                    _handled = this._replicas[_replicaindex]._Schedule(_event);
                }
                _blocked = !_handled;
                if (_blocked)
                    Interlocked.Decrement(ref this._concurrency);
            }
            else
            {
#if TRANSFER_ENABLED
                while (this._transferred != null)
                {
#if REPORT_TRANSFERS_VERBOSE
                    this._platform.Scheduler.Schedule(
                        new QS.Fx.Base.Event(
                            new QS.Fx.Base.ContextCallback(
                                delegate(object _o)
                                {
                                    this._platform.Logger.Log("incoming transfers at master replica.");
                                })));
#endif
                    Replica_.Transfer_ _transferred = Interlocked.Exchange(ref this._transferred, null);
                    while (_transferred != null)
                    {
                        this._importmethod.Invoke(this._object, new object[] { _transferred._transportedobject });
                        _transferred = _transferred._nexttransfer;
                        Interlocked.Decrement(ref this._concurrency);
                    }
                }
#endif
                _handled = false;
                if (this._concurrency > 0)
                {
                    _blocked = true;
                    if (!this._isimporting)
                    {
#if REPORT_THE_BEGINNING_OF_IMPORTING
                        this._platform.Scheduler.Schedule(
                            new QS.Fx.Base.Event(
                                new QS.Fx.Base.ContextCallback(
                                    delegate(object _o) { this._platform.Logger.Log("beginning to import with " + 
                                        this._concurrency.ToString() + " events in " + this._numreplicas.ToString() + " replicas."); })));
#endif
                        this._isimporting = true;
                        for (int _i = 0; _i < this._numreplicas; _i++)
                        {
                            Replica_ _replica = this._replicas[_i];
                            if (_replica._IsRemote)
                                _replica._Export();
                        }
                    }
                }
                else
                {
#if REPORT_THE_END_OF_PROCESSING
                    this._platform.Scheduler.Schedule(
                        new QS.Fx.Base.Event(
                            new QS.Fx.Base.ContextCallback(
                                delegate(object _o)
                                {
                                    this._platform.Logger.Log("end of processing with " + this._numreplicas.ToString() + " replicas.");
                                })));
#endif
                    this._isimporting = true;
                    if (this._transferred != null)
                        throw new Exception("There is still unconsumed transferred state on the master replica.");
                    for (int _i = 0; _i < this._numreplicas; _i++)
                    {
                        Replica_ _replica = this._replicas[_i];
                        if ((_replica != null) && (!_replica._IsRemote || _replica._Export()))
                        {
#if LEAVE_REPLICAS
                            _replica._IsCompleted = true;
#else
                            this._replicas[_i] = null;
#endif
                            if (_replica._Transferred != null)
                                throw new Exception("There is still unconsumed transferred state on working replica ( " + _i.ToString() + ").");
#if VERBOSE_IMPORT
                            this._platform.Logger.Log("import ( " + _i.ToString() + " )");
#endif
#if PROFILE_IMPORTING
                            double _importing_t1 = this._clock.Time;
#endif
                            this._importmethod.Invoke(this._object, new object[] { _replica._Object });
#if PROFILE_IMPORTING
                            double _importing_t2 = this._clock.Time;
                            double _importing_td = _importing_t2 - _importing_t1;
                            this._importingseries.Add(_importing_t1, _importing_td);
#endif
                        }
                    }
                    while ((this._numreplicas > 0) && ((this._replicas[this._numreplicas - 1] == null) || (this._replicas[this._numreplicas - 1]._IsCompleted)))
                        this._numreplicas--;
                    _blocked = (this._numreplicas > 0);
                }
            }
#if CHECK_REPLICATE
            if (Interlocked.Decrement(ref this._inreplicate) != 0)
                throw new Exception("Race condition in Context_._Replicate (on leave)");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Replica_
        // Make this class internal so I can get to the stats inside it programmatically -cms
#if PROFILE_REPLICAS
        internal
#else
        private
#endif
        sealed class Replica_ : QS.Fx.Inspection.Inspectable, System.IDisposable, QS.Fx.Internal.I000011
        {
            #region Constructor

            public Replica_(Context_ _context, QS.Fx.Object.Classes.IObject _object, Queue_ _queue, bool _isremote, QS._qss_x_.Runtime_.IObject_ _remoteobject, 
                Replica_ _parentreplica)
            {
                this._parentreplica = _parentreplica;
                this._context = _context;
                this._clock = this._context._platform.Clock;
                this._transfertime = this._clock.Time + this._context._transfertimeout1;
                this._object = _object;
                this._queue = _queue;
                this._isremote = _isremote;
                this._remoteobject = _remoteobject;
                this._minevents = this._context._minevents;
                this._maxevents = this._context._maxevents;
                this._batching = this._context._batching;
                if (_isremote)
                {
                    this._remoteobject._Import(this._object);
                    this._remoteobject._Ready(new QS.Fx.Base.ContextCallback<int, double>(this._Ready));
                    this._remoteobject._Result(new QS.Fx.Base.ContextCallback<int, QS.Fx.Object.Classes.IObject>(this._Result));
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (this._isremote)
                {
                    ((IDisposable)this._remoteobject).Dispose();
                }
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private Context_ _context;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IClock _clock;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Object.Classes.IObject _object;
            [QS.Fx.Base.Inspectable]
            private Queue_ _queue;
            [QS.Fx.Base.Inspectable]
            private bool _isremote;
            [QS.Fx.Base.Inspectable]
            private QS._qss_x_.Runtime_.IObject_ _remoteobject;
            [QS.Fx.Base.Inspectable]
            private int _exporting;
            [QS.Fx.Base.Inspectable]
            private int _sequenceno;
            [QS.Fx.Base.Inspectable]
            private bool _exported;
            [QS.Fx.Base.Inspectable]
            private int _scheduled;
            [QS.Fx.Base.Inspectable]
            private bool _iscompleted;
            [QS.Fx.Base.Inspectable]
            private int _minevents;
            [QS.Fx.Base.Inspectable]
            private int _maxevents;
            [QS.Fx.Base.Inspectable]
            private int _unavailable;
            [QS.Fx.Base.Inspectable]
            private double _overhead;
            [QS.Fx.Base.Inspectable]
            private double _batching;
#if PROFILE_REPLICAS
            [QS.Fx.Base.Inspectable]
            private const int _num_events = 120000;
            [QS.Fx.Base.Inspectable]
            public QS._qss_e_.Data_.FastStatistics2D_ _schedule_times = new QS._qss_e_.Data_.FastStatistics2D_(_num_events);
            //[QS.Fx.Base.Inspectable]
            //private QS._qss_c_.Statistics_.Samples2D _numscheduled = new QS._qss_c_.Statistics_.Samples2D();
#endif
            [QS.Fx.Base.Inspectable]
            private double _transfertime;
            [QS.Fx.Base.Inspectable]
            private Replica_ _parentreplica;
            [QS.Fx.Base.Inspectable]
            private Transfer_ _transferred;

            #endregion

            #region _Schedule

            public bool _Schedule(QS.Fx.Base.IEvent _event)
            {
                if (this._scheduled < this._maxevents)
                {
                    int _numscheduled = Interlocked.Increment(ref this._scheduled);
                    // unsafe with bypass
#if PROFILE_REPLICAS
                    double _t1 = this._clock.Time;
                    //this._numscheduled.Add(_t1, (double) _numscheduled);
#endif
                    _event.Next = null;
                    if (this._isremote)
                        this._remoteobject._Invoke(_event);
                    else
                    {
                        QS.Fx.Internal.I000010 _eventinternal = (QS.Fx.Internal.I000010)_event;
                        if (_eventinternal == null)
                            throw new Exception("Cannot handle a replicated event; missing internal interfaces.");
                        _eventinternal.x(this._object, this);
                        this._queue._Schedule(_event);
                    }
                    // unsafe with bypass
#if PROFILE_REPLICAS
                    double _t2 = this._clock.Time;
                    this._schedule_times.Add(_t1, _t2 - _t1);
#endif
                    return true;
                }
                else
                {
                    this._unavailable = 1;
                    return false;
                }
            }

            #endregion

            #region _Object

            public QS.Fx.Object.Classes.IObject _Object
            {
                get { return this._object; }
            }

            #endregion

            #region _RemoteObject

            public QS._qss_x_.Runtime_.IObject_ _RemoteObject
            {
                get { return this._remoteobject; }
            }

            #endregion

            #region _IsRemote

            public bool _IsRemote
            {
                get { return this._isremote; }
            }

            #endregion

            #region _IsCompleted

            public bool _IsCompleted
            {
                get { return this._iscompleted; }
                set { this._iscompleted = value; }
            }

            #endregion

            #region _Transferred

            public Transfer_ _Transferred
            {
                get { return this._transferred; }
            }

            #endregion

            #region _Export

            public bool _Export()
            {
                if (this._isremote)
                {
                    if ((this._exporting == 0) && (Interlocked.CompareExchange(ref this._exporting, 1, 0) == 0))
                    {
                        int _sequenceno = Interlocked.Increment(ref this._sequenceno);
                        this._remoteobject._Export(_sequenceno);
                    }
                    return this._exported;
                }
                else
                    return true;
            }

            #endregion

            #region _Result

            private void _Result(int _sequenceno, QS.Fx.Object.Classes.IObject _object)
            {
                if (this._object != null)
                    this._context._importmethod.Invoke(this._object, new object[] { _object });
                else
                    this._object = _object;
                if (_sequenceno >= this._sequenceno)
                    this._exported = true;
                this._context._queue2._Resume();
            }

            #endregion

            #region _Ready

            private void _Ready(int _count, double _overhead)
            {
                this._overhead = _overhead;
                int _maxevents = (int)Math.Floor(this._batching / _overhead);
                if (_maxevents < this._minevents)
                    _maxevents = this._minevents;
                this._maxevents = _maxevents;
                if (_count > 0)
                {
                    int _old_scheduled, _new_scheduled;
                    do
                    {
                        _old_scheduled = this._scheduled;
                        _new_scheduled = _old_scheduled - _count;
                    }
                    while (Interlocked.CompareExchange(ref this._scheduled, _new_scheduled, _old_scheduled) != _old_scheduled);
#if PROFILE_REPLICAS
                    //this._numscheduled.Add(this._clock.Time, (double) _new_scheduled);
#endif
                    int _old_concurrency, _new_concurrency;
                    do
                    {
                        _old_concurrency = this._context._concurrency;
                        _new_concurrency = _old_concurrency - _count;
                    }
                    while (Interlocked.CompareExchange(ref this._context._concurrency, _new_concurrency, _old_concurrency) != _old_concurrency);
                    bool _resume1 = (_new_concurrency == 0);
                    bool _resume2 = (Interlocked.Exchange(ref this._unavailable, 0) == 1);
                    if (_resume1 || _resume2)
                        this._context._queue2._Resume();
                }
            }

            #endregion

            #region Class Transfer

            public sealed class Transfer_ : QS.Fx.Base.IEvent
            {
                #region Constructor

                public Transfer_(Replica_ _sourcereplica, Replica_ _targetreplica, QS.Fx.Object.Classes.IObject _transportedobject)
                {
                    this._sourcereplica = _sourcereplica;
                    this._targetreplica = _targetreplica;
                    this._transportedobject = _transportedobject;
                }

                #endregion

                #region Fields

                public Replica_ _sourcereplica;
                public Replica_ _targetreplica;
                public QS.Fx.Object.Classes.IObject _transportedobject;
                public Transfer_  _nexttransfer;
                public QS.Fx.Base.IEvent _nextevent;

                #endregion

                #region IEvent Members

                void QS.Fx.Base.IEvent.Handle()
                {
                    ((QS.Fx.Internal.I000011) this._targetreplica).x();
                }

                QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
                {
                    get { return this._nextevent; }
                    set { this._nextevent = value; }
                }

                QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
                {
                    get { return QS.Fx.Base.SynchronizationOption.Multithreaded; }
                }

                #endregion
            }

            #endregion

            #region I000011 Members

            void QS.Fx.Internal.I000011.x()
            {
                bool _resume3 = false;
#if TRANSFER_ENABLED
                while (this._transferred != null)
                {
#if REPORT_TRANSFERS_VERBOSE
                    this._context._platform.Scheduler.Schedule(
                        new QS.Fx.Base.Event(
                            new QS.Fx.Base.ContextCallback(
                                delegate(object _o) { this._context._platform.Logger.Log("incoming transfers at replica " + this._sequenceno.ToString() + "."); })));
#endif
                    Transfer_ _transferred = Interlocked.Exchange(ref this._transferred, null);
                    while (_transferred != null)
                    {
                        this._context._importmethod.Invoke(this._object, new object[] { _transferred._transportedobject });
                        _transferred = _transferred._nexttransfer;
                        Interlocked.Decrement(ref this._context._concurrency);
                    }
                }
                if (this._context._transferenabled)
                {
                    double _time = this._clock.Time;
                    if (_time > this._transfertime)
                    {
#if REPORT_TRANSFERS_VERBOSE
                        this._context._platform.Scheduler.Schedule(
                            new QS.Fx.Base.Event(
                                new QS.Fx.Base.ContextCallback(
                                    delegate(object _o) { this._context._platform.Logger.Log("transfer timeout at replica " + this._sequenceno.ToString() + "."); })));
#endif
                        Context_ _context = this._context;
                        Replica_ _parentreplica = this._parentreplica;
                        if (_parentreplica != null)
                        {
                            if (_parentreplica._transferred == null)
                            {
#if REPORT_TRANSFERS_VERBOSE
                                this._context._platform.Scheduler.Schedule(
                                    new QS.Fx.Base.Event(
                                        new QS.Fx.Base.ContextCallback(
                                            delegate(object _o) { this._context._platform.Logger.Log("transfer from replica " + this._sequenceno.ToString() + 
                                                " to replica " + _parentreplica._sequenceno.ToString() + "."); })));
#endif
                                Interlocked.Increment(ref this._context._concurrency);
                                QS.Fx.Object.IContext _replicacontext = new ReplicaContext_(this._context);
                                QS.Fx.Object.Classes.IObject _transportedobject = (QS.Fx.Object.Classes.IObject)this._context._constructor.Invoke(new object[] { _replicacontext });
                                if (_transportedobject == null)
                                    throw new Exception("Cannot spawn transport replica; the constructor returned null.");
                                this._context._exportmethod.Invoke(this._object, new object[] { _transportedobject });
                                this._context._importmethod.Invoke(_transportedobject, new object[] { this._object });
                                Transfer_ _transfer = new Transfer_(this, _parentreplica, _transportedobject);
                                Transfer_ _transferredroot;
                                do
                                {
                                    _transferredroot = _parentreplica._transferred;
                                    _transfer._nexttransfer = _transferredroot;
                                }
                                while (Interlocked.CompareExchange<Transfer_>(ref _parentreplica._transferred, _transfer, _transferredroot) != _transferredroot);
                                Interlocked.Increment(ref _parentreplica._scheduled);
                                Interlocked.Increment(ref _context._concurrency);
                                _parentreplica._queue._Schedule(_transfer);
                                _time = this._clock.Time;
                                this._transfertime = _time + this._context._transfertimeout1;
                            }
                            else
                                this._transfertime = _time + this._context._transfertimeout2;
                        }
                        else
                        {
                            if (_context._transferred == null)
                            {
#if REPORT_TRANSFERS_VERBOSE
                                this._context._platform.Scheduler.Schedule(
                                    new QS.Fx.Base.Event(
                                        new QS.Fx.Base.ContextCallback(
                                            delegate(object _o)
                                            {
                                                this._context._platform.Logger.Log("transfer from replica " + this._sequenceno.ToString() + " to master.");
                                            })));
#endif
                                Interlocked.Increment(ref _context._concurrency);
                                QS.Fx.Object.IContext _replicacontext = new ReplicaContext_(this._context);
                                QS.Fx.Object.Classes.IObject _transportedobject = (QS.Fx.Object.Classes.IObject)this._context._constructor.Invoke(new object[] { _replicacontext });
                                if (_transportedobject == null)
                                    throw new Exception("Cannot spawn transport replica; the constructor returned null.");
                                this._context._exportmethod.Invoke(this._object, new object[] { _transportedobject });
                                this._context._importmethod.Invoke(_transportedobject, new object[] { this._object });
                                Transfer_ _transfer = new Transfer_(this, null, _transportedobject);
                                Transfer_ _transferredroot;
                                do
                                {
                                    _transferredroot = _context._transferred;
                                    _transfer._nexttransfer = _transferredroot;
                                }
                                while (Interlocked.CompareExchange<Transfer_>(ref _context._transferred, _transfer, _transferredroot) != _transferredroot);
                                _resume3 = true;
                                _time = this._clock.Time;
                                this._transfertime = _time + this._context._transfertimeout1;
                            }
                            else
                                this._transfertime = _time + this._context._transfertimeout2;
                        }
                    }
                }
#endif
                Interlocked.Decrement(ref this._scheduled);
                bool _resume1 = (Interlocked.Decrement(ref this._context._concurrency) == 0);
                bool _resume2 = (Interlocked.Exchange(ref this._unavailable, 0) == 1);
                if (_resume1 || _resume2 || _resume3)
                    this._context._queue2._Resume();
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Queue_
#if PROFILE_REPLICAS
        internal
#else
        private 
#endif
 sealed class Queue_ : QS.Fx.Inspection.Inspectable, System.IDisposable, QS.Fx.Base.IEvent
        {
            #region Constructor

            public Queue_(Context_ _context, QS.Fx.Platform.IPlatform _platform, QS.Fx.Base.SynchronizationOption _synchronizationoption)
            {
                this._synchronizationoption = _synchronizationoption;
                this._replicated = ((this._synchronizationoption & QS.Fx.Base.SynchronizationOption.Replicated) == QS.Fx.Base.SynchronizationOption.Replicated);
                this._platform = _platform;
                this._scheduler = _platform.Scheduler;
                this._context = _context;
                this._clock = _platform.Clock;
            }

            #endregion

            #region Destructor

            ~Queue_()
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
            private Context_ _context;
            [QS.Fx.Base.Inspectable]
            private bool _replicated;
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
            private QS.Fx.Base.SynchronizationOption _synchronizationoption;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Platform.IPlatform _platform;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Scheduling.IScheduler _scheduler;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Clock.IClock _clock;
#if PROFILE_ENQUEUE

            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Data_.FastStatistics2D_ _statistics_enqueue = new QS._qss_e_.Data_.FastStatistics2D_(100000);
                //"enqueue times", "times it takes to enqueue", "time", "s", "time in seconds", "time", "s", "time in seconds");
#endif
#if PROFILE_DEQUEUE
            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Data_.FastStatistics2D_ _statistics_dequeue = new QS._qss_e_.Data_.FastStatistics2D_(100000);
            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Data_.FastStatistics2D_ _statistics_dequeue_replicated = new QS._qss_e_.Data_.FastStatistics2D_(100000);
            [QS.Fx.Base.Inspectable]
            private QS._qss_e_.Data_.FastStatistics2D_ _statistics_dequeue_handled = new QS._qss_e_.Data_.FastStatistics2D_(100000);
#endif
#if PROFILE
#if PROFILE_2
            [QS.Fx.Base.Inspectable]
            private QS._qss_c_.Statistics_.Samples2D _statistics_busy = new QS._qss_c_.Statistics_.Samples2D(
                "busy", "busy or not", "time", "s", "time in seconds", "busy", "0/1", "0 = free, 1 = busy");
#endif
#endif
            [QS.Fx.Base.Inspectable]
            private int _inhandle;

            private QS.Fx.Base.IEvent _next;

            #endregion

            #region _Schedule


#if NO_OPTIMIZATION_IN_THE_SCHEDULER_CODE
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
#endif
            public void _Schedule(QS.Fx.Base.IEvent _event)
            {
#if PROFILE_ENQUEUE
                double _t1 = _clock.Time;
#endif
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
#if PROFILE_ENQUEUE
                double _t2 = _clock.Time;
                
                    this._statistics_enqueue.Add(_t1, _t2 - _t1);
                
#endif
            }

            #endregion

            #region _Resume

#if NO_OPTIMIZATION_IN_THE_SCHEDULER_CODE
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
#endif
            public void _Resume()
            {
                if (this._disposed == 0)
                {
                    this._schedule = 1;
                    int _operating = Interlocked.Exchange(ref this._operating, 1);
                    if ((_operating == 0) || (_operating == -2))
                        this._scheduler.Schedule(this);
                }
            }

            #endregion

            #region IEvent Members

#if NO_OPTIMIZATION_IN_THE_SCHEDULER_CODE
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoOptimization)]
#endif
            void QS.Fx.Base.IEvent.Handle()
            {
                double _t1 = _clock.Time;
#if PROFILE
#if PROFILE_2
                _statistics_busy.Add(_t1 - 0.000000001, 0);
                _statistics_busy.Add(_t1, 1);
#endif
#endif

                int _handled = 0;
                int _replicated = 0;
                bool _postpone = false;
                if (this._disposed == 0)
                {
                    do
                    {
                        if (this._operating != 1)
                            throw new Exception("Race condition in Context_.Queue_._Handle (should not enter, operating = " + this._operating.ToString() + ")");
#if CHECK_HANDLE
                        if (Interlocked.Increment(ref this._inhandle) != 1)
                            throw new Exception("Race condition in Context_.Queue_._Handle (main logic enter)");
#endif

                        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                        while ((this._operating == 1) && !_postpone && ((this._schedule != 0) || (this._from != null)))
                        {
                            bool _more = (this._schedule != 0); // there may have been new events on the queue
                            this._schedule = 0;
                            if (_more)
                            {
                                #region pull events from the queue, invert the order, and append after the existing ones on the todo list

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

                                #endregion
                            }
                            while ((this._operating == 1) && !_postpone && (this._from != null))
                            {
                                QS.Fx.Base.IEvent _event = this._from;
                                QS.Fx.Base.IEvent _next = _event.Next;
                                bool _used = false;
                                bool _isblocked = false;
                                if (this._replicated)
                                {
                                    Interlocked.Exchange(ref this._operating, -1); // tentatively set the queue as blocked (-1) while trying to replicate event
                                    this._context._Replicate(_event, out _used, out _isblocked);
                                    _replicated++;
                                    if (!_isblocked)
                                        Interlocked.Exchange(ref this._operating, 1);
                                    if (_used)
                                    {
                                        this._from = _next;
                                        if (this._from == null)
                                            this._to = null;
                                    }
                                }
                                if ((this._operating == 1) && !_used && !_isblocked)
                                {
                                    #region since non-blocked, process any unconsumed events right now, potentially postpone

                                    _event.Next = null;
                                    this._from = _next;
                                    if (this._from == null)
                                        this._to = null;
                                    _event.Handle();
                                    _handled++;
                                    if (_handled >= QS.Fx.Object.Runtime.MaximumNumberOfEventsToHandleSequentially)
                                        _postpone = true;

                                    #endregion
                                }
                                if ((this._operating == 1) && !_postpone)
                                {
                                    double _elapsed = _clock.Time - _t1;
                                    if (_elapsed >= QS.Fx.Object.Runtime.MaximumAmountOfTimeToHandleEventsSequentially)
                                        _postpone = true;
                                }
                            }
                        }

                        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

#if CHECK_HANDLE
                        if (Interlocked.Decrement(ref this._inhandle) != 0)
                            throw new Exception("Race condition in Context_.Queue_._Handle (main logic leave)");
#endif
                        if (_postpone)
                        {
                            if (this._operating == 1)
                                this._scheduler.Schedule(this);
                            else
                                throw new Exception("Race condition in Context_.Queue_._Handle (postpone = " +
                                    _postpone.ToString() + ", operating = " + this._operating.ToString() + ")");
                        }
                        else
                        {
                            bool _retry = true;
                            if (this._operating == -1)
                            {
                                if (Interlocked.CompareExchange(ref this._operating, -2, -1) == -1)
                                    _retry = false;
                            }
                            if (_retry && (this._operating == 1))
                            {
                                if (Interlocked.CompareExchange(ref this._operating, 0, 1) != 1)
                                    throw new Exception("Race condition in Context_.Queue_._Handle (trying to set operating 1 -> 0)");
                            }
                        }
                    }
                    while (!_postpone && (this._schedule != 0) && (this._operating == 0) && (Interlocked.CompareExchange(ref this._operating, 1, 0) == 0));
                }
#if PROFILE_DEQUEUE
                double _t2 = _clock.Time;
                this._statistics_dequeue_handled.Add(_t1, _handled);
                this._statistics_dequeue_replicated.Add(_t1, _replicated);
                this._statistics_dequeue.Add(_t1, _t2 - _t1);
#endif
#if PROFILE
#if PROFILE_2
                _statistics_busy.Add(_t2 - 0.000000001, 1);
                _statistics_busy.Add(_t2, 0);
#endif
#endif
            }

            QS.Fx.Base.IEvent QS.Fx.Base.IEvent.Next
            {
                get { return this._next; }
                set { this._next = value; }
            }

            QS.Fx.Base.SynchronizationOption QS.Fx.Base.IEvent.SynchronizationOption
            {
                get { return this._synchronizationoption; }
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
