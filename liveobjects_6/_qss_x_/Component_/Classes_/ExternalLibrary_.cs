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
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.ExternalLibrary, "External Library", "An external library containing type and object definitions.")]
    public sealed class ExternalLibrary_<
        [QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass> 
        : QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>, 
        QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>
        where ObjectClass : QS.Fx.Object.Classes.IObject
    {
        #region Constructor

        public ExternalLibrary_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("ID", QS.Fx.Reflection.ParameterClass.Value)] string _id,
            [QS.Fx.Reflection.Parameter("Url", QS.Fx.Reflection.ParameterClass.Value)] string _url)
        {
            this._id = _id;
            this._url = _url;
            this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>(this);
        }

        #endregion

        #region Fields

        private string _id, _url;
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>> _endpoint;

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>>
            QS._qss_x_.Object_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion

        #region IFactory<IObject> Members

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> 
            QS._qss_x_.Interface_.Classes_.IFactory<QS.Fx.Object.Classes.IObject>.Create()
        {
            return null;
/*
            lock (this)
            {
                return QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(
                    new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._ExternalLibrary_CreateCallback_),
                    typeof(object), this._id,
                    QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(ObjectClass)),
                    new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[]
                    {
                        new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_name, this._id),
                    }));
            }
*/ 
        }

        #endregion

        #region _ExternalLibrary_CreateCallback_

        private QS.Fx.Object.Classes.IObject _ExternalLibrary_CreateCallback_()
        {
            lock (this)
            {
                throw new NotImplementedException();
/*
                if (this._localquicksilver == null)
                    this._localquicksilver = new QS._qss_x_.QuickSilver_.QuickSilver_(this._configuration);
                return (QS.Fx.Object.Classes.IObject)new QS._qss_x_.QuickSilver_.QuickSilver_1_(this._localquicksilver);
*/ 
            }
        }

        #endregion
    }
}
