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
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.Folder_
{
    public abstract class Folder_
        : QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        protected Folder_(QS.Fx.Object.IContext _mycontext)
        {
            this._myendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);
            this._myconstructors = new Dictionary<string, QS._qss_x_.Object_.IConstructor_>();
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _myendpoint;
        
        protected IDictionary<string, QS._qss_x_.Object_.IConstructor_> _myconstructors;

        #endregion

        #region IDictionary<string,IObject> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._myendpoint; }
        }

        #endregion

        #region IDictionary<string,IObject> Members

        IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
        {
            lock (this)
            {
                return this._myconstructors.Keys;
            }
        }

        IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
        {
            List<QS._qss_x_.Object_.IConstructor_> _constructors = new List<QS._qss_x_.Object_.IConstructor_>();
            lock (this)
            {
                _constructors.AddRange(this._myconstructors.Values);
            }
            List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = 
                new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
            foreach (QS._qss_x_.Object_.IConstructor_ _constructor in _constructors)
                _objects.Add(_constructor.Create);
            return _objects;
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
        {
            lock (this)
            {
                return this._myconstructors.ContainsKey(_key);
            }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
        {
            QS._qss_x_.Object_.IConstructor_ _constructor;
            lock (this)
            {
                if (!this._myconstructors.TryGetValue(_key, out _constructor))
                    throw new Exception("No object with id \"" + _key + "\" could be found in this folder.");
            }
            return _constructor.Create;
        }

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
            string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            QS._qss_x_.Object_.IConstructor_ _constructor;
            lock (this)
            {
                if (!this._myconstructors.TryGetValue(_key, out _constructor))
                {
                    _object = null;
                    return false;
                }
            }
            _object = _constructor.Create;
            return true;
        }

        int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
        {
            lock (this)
            {
                return this._myconstructors.Count;
            }
        }

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            throw new NotSupportedException();
        }

        #region IDictionary<string,IObject>.Remove

        void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Remove(
            string _key)
        {
            throw new NotImplementedException();
        }

        #endregion

        bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
        {
            return true;
        }

        #endregion
    }
}
