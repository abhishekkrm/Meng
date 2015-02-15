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
using System.Diagnostics;
using System.Threading;

namespace QS._qss_x_.Registry_
{
    internal sealed class _Registry : QS.Fx.Inspection.Inspectable,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        IDisposable
    {
        #region _Shutdown

        public static void _Shutdown()
        {
            lock (typeof(_Registry))
            {
                foreach (_Registry _registry in _registries)
                    ((IDisposable) _registry).Dispose();
            }
        }

        private static readonly List<_Registry> _registries = new List<_Registry>();

        #endregion

        #region Constants

        // Optional components to load should be in folder ..\registry, relative to the executable.
        private static readonly string _DEFAULT_REGISTRY_FOLDER =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + "registry";

        #endregion

        #region Contructor

        internal _Registry(QS.Fx.Object.IContext _mycontext) : this(_mycontext, _DEFAULT_REGISTRY_FOLDER)
        {
        }

        internal _Registry(QS.Fx.Object.IContext _mycontext, string _registry_folder)
        {
            this._mycontext = _mycontext;
            this._registry_folder = _registry_folder;
            lock (typeof(_Registry))
            {
                _registries.Add(this);
            }
        }

        #endregion

        #region Destructor

        ~_Registry()
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
                    lock (this)
                    {
                        if (this._objects != null)
                        {
                            foreach (_Object _object in this._objects.Values)
                            {
                                try
                                {
                                    ((IDisposable)_object).Dispose();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            this._objects.Clear();
                            this._objects = null;
                        }
                    }
                }
            }
        }

        #endregion

        #region Fields

        private IDictionary<string, _Object> _objects = new Dictionary<string, _Object>();
        private int _disposed;
        private QS.Fx.Object.IContext _mycontext;
        private string _registry_folder;

        #endregion

        #region _Initialize

        internal void _Initialize()
        {
            lock (this)
            {
                if (Directory.Exists(_registry_folder))
                {
                    using (QS._qss_x_.Component_.Classes_.Loader _loader = new QS._qss_x_.Component_.Classes_.Loader(_mycontext,
                        QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                            QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                    {
                        QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject> _interface;
                        using (QS._qss_x_.Component_.Classes_.Service<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>.Connect(
                            _mycontext, _loader, out _interface))
                        {
                            string[] _dd = Directory.GetDirectories(_registry_folder, "*", SearchOption.TopDirectoryOnly);
                            foreach (string _directoryname in _dd)
                            {
                                string _id = _directoryname.Substring(_directoryname.LastIndexOfAny(new char[] { '\\', '/' }) + 1);
                                string[] _ff = Directory.GetFiles(_directoryname, "*.liveobject", SearchOption.TopDirectoryOnly);
                                if (_ff.Length != 1)
                                    throw new Exception("Invalid registry entry at key \"" + _id + "\", there should be exatly one live object file under that key.");
                                string _filename = _ff[0];
                                try
                                {
                                    string _objectxml;
                                    using (StreamReader _reader = new StreamReader(_filename))
                                    {
                                        _objectxml = _reader.ReadToEnd();
                                    }
                                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = _interface.Load(_objectxml);
                                    if (_objectref != null)
                                    {
                                        if (!_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Factory)))
                                            throw new Exception("The object is not a factory object type.");
                                        _Object _object = new _Object(_mycontext, _id, _objectref);
                                        _objects.Add(_id, _object);
                                    }
                                }
                                catch (Exception _exc)
                                {
                                    throw new Exception("Object installed at key \"" + _id + "\" cannot be loaded to the registry.", _exc);
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region IDictionary<string, IObject> Members

        #region IDictionary<string,IObject>.Keys

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            lock (this)
            {
                return this._objects.Keys;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.Objects

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            lock (this)
            {
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                foreach (KeyValuePair<string, _Object> _element in this._objects)
                    _objects.Add(_element.Value.Reference);
                return _objects;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.ContainsKey

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            lock (this)
            {
                return this._objects.ContainsKey(_key);
            }
        }

        #endregion

        #region IDictionary<string,IObject>.GetObject

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            lock (this)
            {
                _Object _object;
                if (this._objects.TryGetValue(_key, out _object))
                    return _object.Reference;
                else
                    throw new Exception("No object named \"" + _key + "\" exists in the registry.");
            }
        }

        #endregion

        #region IDictionary<string,IObject>.TryGetObject

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
            string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            lock (this)
            {
                _Object _o;
                if (this._objects.TryGetValue(_key, out _o))
                {
                    _object = _o.Reference;
                    return true;
                }
                else
                {
                    _object = null;
                    return false;
                }
            }
        }

        #endregion

        #region IDictionary<string,IObject>.Add

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new Exception("Objects cannot be dynamically added to the registry using this programmatic interface");
        }

        #endregion

        #region IDictionary<string,IObject>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDictionary<string,IObject>.Count

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            lock (this)
            {
                return this._objects.Count;
            }
        }

        #endregion

        #region IDictionary<string,IObject>.IsReadOnly

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return true;
        }

        #endregion

        #endregion

        #region Class _Object

        private sealed class _Object : QS.Fx.Inspection.Inspectable, IDisposable
        {
            #region Constructor

            public _Object(QS.Fx.Object.IContext _mycontext, string _id, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
            {
                this._mycontext = _mycontext;
                this._id = _id;
                this._objectref = _objectref;
            }

            #endregion

            #region Destructor

            ~_Object()
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
                        lock (this)
                        {
                            if (this._connection != null)
                            {
                                try
                                {
                                    this._connection.Dispose();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            this._connection = null;
                            this._endpoint = null;
                            if ((this._factoryobject != null) && (this._factoryobject is IDisposable))
                            {
                                try
                                {
                                    ((IDisposable)this._factoryobject).Dispose();
                                }
                                catch (Exception)
                                {
                                }
                            }
                            this._factoryobject = null;
                            this._objectref = null;
                        }
                    }
                }
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IContext _mycontext;
            private string _id;
            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
            private QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject> _factoryobject;
            private QS.Fx.Endpoint.Internal.IImportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;
            private QS.Fx.Endpoint.IConnection _connection;
            private int _disposed;

            #endregion

            #region Reference

            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> Reference
            {
                get
                {
                    lock (this)
                    {
                        if (this._factoryobject == null)
                        {
                            this._factoryobject = this._objectref.CastTo<QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>().Dereference(_mycontext);
                            this._endpoint = _mycontext.ImportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>();
                            this._connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._endpoint).Connect(this._factoryobject.Endpoint);
                        }
                        return this._endpoint.Interface.Create();
                    }
                }
            }

            #endregion
        }

        #endregion

        #region REGISTRY

        //public static _Registry REGISTRY
        //{
        //    get
        //    {
        //        lock (typeof(_Registry))
        //        {
        //            if (_REGISTRY == null)
        //                throw new Exception("The registry has not been initialized yet.");
        //            return _REGISTRY;
        //        }
        //    }
        //}

        //private static _Registry _REGISTRY;

        //public static void _INITIALIZE(QS.Fx.Object.IContext _mycontext)
        //{
        //    lock (typeof(_Registry))
        //    {
        //        if (_REGISTRY == null)
        //            _REGISTRY = new _Registry(_mycontext, _DEFAULT_REGISTRY_FOLDER);
        //        else
        //            throw new Exception("The registry has already been initialized once.");
        //    }
        //}

        //public static void _SHUTDOWN()
        //{
        //    lock (typeof(_Registry))
        //    {
        //        if (_REGISTRY != null)
        //        {
        //            try
        //            {
        //                ((IDisposable)_REGISTRY).Dispose();
        //            }
        //            catch (Exception)
        //            {
        //            }
        //            _REGISTRY = null;
        //        }
        //    }
        //}

        #endregion
    }
}
