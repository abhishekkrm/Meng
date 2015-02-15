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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DictionaryClient)]
    public sealed class DictionaryClient<
        [QS.Fx.Reflection.Parameter("KeyClass", QS.Fx.Reflection.ParameterClass.ValueClass)] KeyClass,
        [QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass>
        : QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>>,
        QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>
        where KeyClass : class
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        #region Constructor

        public DictionaryClient(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("dictionary", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<KeyClass, ObjectClass>> _dictionary)
        {
            if (_dictionary == null)
                throw new Exception("Dictionary reference cannot be null.");

            this._dictionaryendpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>, 
                    QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>>(this);

            this._dictionaryconnection =
                ((QS.Fx.Endpoint.Classes.IEndpoint)this._dictionaryendpoint).Connect(_dictionary.Dereference(_mycontext).Endpoint);

            this._serviceendpoint = 
                _mycontext.ExportedInterface<QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>>(
                    this._dictionaryendpoint.Interface);
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>,
            QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>> _dictionaryendpoint;

        private QS.Fx.Endpoint.IConnection _dictionaryconnection;

        private QS.Fx.Endpoint.Internal.IExportedInterface<
            QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>> _serviceendpoint;

        #endregion

        #region IService<IDictionary<KeyClass,ObjectClass>> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>> 
            QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.IDictionary<KeyClass, ObjectClass>>.Endpoint
        {
            get { return this._serviceendpoint; }
        }

        #endregion

        #region IDictionaryClient<KeyClass,ObjectClass> Members

        void QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>.Ready()
        {
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>.Added(KeyClass _key, QS.Fx.Object.IReference<ObjectClass> _object)
        {
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<KeyClass, ObjectClass>.Removed(
            KeyClass _key)
        {
        }

        #endregion
    }
}
