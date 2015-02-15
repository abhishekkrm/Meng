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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.LocalFolder, "LocalFolder", "A folder object based on a local filesystem folder.")]
    public sealed class LocalFolder : QS.Fx.Inspection.Inspectable, 
        QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public LocalFolder(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("path", QS.Fx.Reflection.ParameterClass.Value)] string _path,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader) 
        {
            this._path = _path;

            this._folderendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);

            this._folderendpoint.OnConnect += new QS.Fx.Base.Callback(this._FolderConnectCallback);
            this._folderendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._FolderDisconnectCallback);

            this._loaderendpoint = _mycontext.ImportedInterface<
                QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();

            if (_loader != null)
                this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
            else
                throw new Exception("Cannot run without a loader attached.");
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("path")]
        private string _path;

        [QS.Fx.Base.Inspectable("folderendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folderendpoint;

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;

        [QS.Fx.Base.Inspectable("loaderconnection")]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        #endregion

        #region IDictionary<string,IObject> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return _folderendpoint; }
        }

        #endregion

        #region _FolderConnectCallback

        private void _FolderConnectCallback()
        {
        }

        #endregion

        #region _FolderDisconnectCallback

        private void _FolderDisconnectCallback()
        {
        }

        #endregion

        #region IDictionary<string,IObject> Members

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            lock (this)
            {
                List<string> _keys = new List<string>();
                if (Directory.Exists(_path))
                {
                    foreach (string _filename in Directory.GetFiles(_path, "*.liveobject", SearchOption.TopDirectoryOnly))
                    {
                        string _key = Path.GetFileNameWithoutExtension(_filename);
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
                        if (((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>) this).TryGetObject(_key, out _object))
                            _keys.Add(_key);
                    }
                }
                return _keys;
            }
        }

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            lock (this)
            {
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                if (Directory.Exists(_path))
                {
                    foreach (string _filename in Directory.GetFiles(_path, "*.liveobject", SearchOption.TopDirectoryOnly))
                    {
                        string _key = Path.GetFileNameWithoutExtension(_filename);
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
                        if (((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>) this).TryGetObject(_key, out _object))
                            _objects.Add(_object);
                    }
                } 
                return _objects;
            }
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
            return ((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)this).TryGetObject(_key, out _object);
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            try
            {
                _object = ((QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)this).GetObject(_key);
                return true;
            }
            catch (Exception)
            {
                _object = null;
                return false;
            }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            lock (this)
            {
                string _filename = _path + Path.DirectorySeparatorChar + _key + ".liveobject";
                if (!File.Exists(_filename))
                    throw new Exception("No object with key \"" + _key + "\" exists in this folder.");
                string _objectxml;
                using (StreamReader _streamreader = new StreamReader(_filename))
                {
                    _objectxml = _streamreader.ReadToEnd();
                }
                return _loaderendpoint.Interface.Load(_objectxml);
            }
        }

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new NotImplementedException();
        }

        #region IDictionary<string,IObject>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            throw new NotImplementedException();
        }

        #endregion

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            throw new NotImplementedException();
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
