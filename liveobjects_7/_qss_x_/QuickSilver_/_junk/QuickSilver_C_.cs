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

namespace QS._qss_x_.QuickSilver_
{
/*
    public sealed class QuickSilver_C_ : IDisposable
    {
        #region Constants

        private const string _c_configuration_uplink_port = "uplink_port";

        #endregion

        #region Constructor

        private QuickSilver_C_(QS.Fx.Configuration.IConfiguration _configuration)
        {
            this._configuration = _configuration;
            QS.Fx.Configuration.IParameter _parameter;            
            int _uplink_port =
                (this._configuration.TryGetParameter(_c_configuration_uplink_port, out _parameter) && (_parameter.Value.Length > 0)) ?
                    Convert.ToInt32(_parameter.Value) :
                    60666;            
        }

        #endregion

        #region Destructor

        ~QuickSilver_C_()
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

        private QS.Fx.Configuration.IConfiguration _configuration;
        private int _disposed;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Object_

        public sealed class Object_
            : QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
        {
            #region Constants

            private const string _c_key_QSM = "QSM";

            #endregion

            #region Constructor

            private Object_(QuickSilver_C_ _myquicksilver_c_)
            {
                this._myquicksilver_c_ = _myquicksilver_c_;
                this._myconstructors = 
                    new Dictionary<string, QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();

//                this._myconstructors.Add(
//                    _c_key_QSM,
//                    new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(
//                        this._ConstructorCallback_QSM_));

                this._myendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);
            }

            #endregion

            #region Fields

            private QuickSilver_C_ _myquicksilver_c_;
            private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _myendpoint;
            private IDictionary<string, QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _myconstructors;

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _ConstructorCallback_QSM_

//            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _ConstructorCallback_QSM_()
//            {
//                lock (this)
//                {
//                    return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
//                    (
//                        new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._Object_CreateCallback_),
//                        typeof(Object_),
//                        "QSM",
//                        QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)),
//                        new QS.Fx.Attributes.Attributes
//                        (
//                            new QS.Fx.Attributes.IAttribute[] 
//                            {
//                                new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "QSM"),
//                            }
//                        )
//                    );
//                }
//            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

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
                List<QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _constructors =
                    new List<QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
                lock (this)
                {
                    _constructors.AddRange(this._myconstructors.Values);
                }
                List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _objects = 
                    new List<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();
                foreach (QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor in _constructors)
                    _objects.Add(_constructor());
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
                QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor;
                lock (this)
                {
                    if (!this._myconstructors.TryGetValue(_key, out _constructor))
                        throw new Exception("No object with id \"" + _key + "\" could be found in this folder.");
                }
                return _constructor();
            }

            bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(
                string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
            {
                QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _constructor;
                lock (this)
                {
                    if (!this._myconstructors.TryGetValue(_key, out _constructor))
                    {
                        _object = null;
                        return false;
                    }
                }
                _object = _constructor();
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

            bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
            {
                return true;
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region Class Factory_

            [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.QuickSilver_C_,
                "QuickSilverClient", "A factory that creates proxies to interact with the single in-process shared client of QuickSilver.")]
            public sealed class Factory_
                : QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>,
                QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
            {
                #region Constructor

                public Factory_(
                    [QS.Fx.Reflection.Parameter("Configuration", QS.Fx.Reflection.ParameterClass.Value)] 
                        QS.Fx.Configuration.IConfiguration _configuration)
                {
                    this._myendpoint =
                        _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
                    this._configuration = _configuration;
                }

                #endregion

                #region Fields

                private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _myendpoint;
                private QS.Fx.Configuration.IConfiguration _configuration;
                private QuickSilver_C_ _myquicksilver_c_;

                #endregion

                // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                #region IFactory<IObject> Members

                QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>
                    QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
                {
                    get { return this._myendpoint; }
                }

                #endregion

                #region IFactory<IObject> Members

                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>
                    QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
                {
                    lock (this)
                    {
                        return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(
                            new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._Object_CreateCallback_),
                            typeof(Object_),
                            "QuickSilverClient",
                            QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)),
                            new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[] 
                            {
                                new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "QuickSilverClient"),
                            }));
                    }
                }

                #endregion

                #region _Object_CreateCallback_

                private QS.Fx.Object.Classes.IObject _Object_CreateCallback_()
                {
                    lock (this)
                    {
                        if (this._myquicksilver_c_ == null)
                            this._myquicksilver_c_ = new QuickSilver_C_(this._configuration);
                        return (QS.Fx.Object.Classes.IObject) new Object_(this._myquicksilver_c_);
                    }
                }

                #endregion

                // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            }

            #endregion
        }

        #endregion
    }
*/
}
