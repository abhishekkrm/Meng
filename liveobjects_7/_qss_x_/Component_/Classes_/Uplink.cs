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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Uplink, "Uplink",
        "An object that manages a connection from this local process to the controller in a system service on the local machine.")]
    public sealed class Uplink
        : QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>, QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public Uplink(QS.Fx.Object.IContext _mycontext, [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)] string _address)
        {
            this._mycontext = _mycontext;
            this._address = _address;
            this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private string _address;
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;
        private QS._qss_x_.Uplink_.IUplink _uplink;

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>
            QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
        {
            lock (this)
            {
                if (this._uplink == null)
                    this._uplink = new QS._qss_x_.Uplink_.Uplink(this._address);

                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create
                (
                    new Uplink_(_mycontext, this._uplink),
                    "uplink",
                    new QS.Fx.Attributes.Attributes
                    (
                        new QS.Fx.Attributes.IAttribute[]
                        {
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, "Uplink"),
                            new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, 
                                "An object that manages a connection from this local process to the controller in a system service on the local machine."),
                        }
                    ),
                    QS._qss_x_.Reflection_.Library.ObjectClassOf
                    (
                        typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)
                    )
                );
            }
        }

        #endregion

        #region Class Uplink_

        private sealed class Uplink_ : 
            QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>, 
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>
        {
            #region Constructor

            public Uplink_(QS.Fx.Object.IContext _mycontext, QS._qss_x_.Uplink_.IUplink _uplink)
            {
                this._uplink = _uplink;
                this._endpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>(this);
            }

            #endregion

            #region Fields

            private QS._qss_x_.Uplink_.IUplink _uplink;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _endpoint;

            #endregion

            #region IDictionary<string,IObject> Members

            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
                QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Endpoint
            {
                get { return this._endpoint; }
            }

            #endregion

            #region IDictionary<string,IObject> Members

            IEnumerable<string> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Keys()
            {
                throw new NotSupportedException();
            }

            IEnumerable<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Objects()
            {
                throw new NotSupportedException();
            }

            bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.ContainsKey(string _key)
            {
                throw new NotSupportedException();
            }

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.GetObject(string _key)
            {
                throw new NotSupportedException();
            }

            bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.TryGetObject(string _key, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
            {
                throw new NotSupportedException();
            }

            void QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Add(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
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

            int QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.Count()
            {
                throw new NotSupportedException();
            }

            bool QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>.IsReadOnly()
            {
                return true;
            }

            #endregion
        }

        #endregion
    }
}
