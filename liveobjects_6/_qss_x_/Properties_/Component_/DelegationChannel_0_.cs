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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DelegationChannel_0, "Properties Framework Delegation Channel (Local)")]
    public sealed class DelegationChannel_0_<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IDelegationChannel<IdentifierClass, ObjectClass>,
        QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public DelegationChannel_0_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("identifier", QS.Fx.Reflection.ParameterClass.Value)]
            IdentifierClass _identifier,
            [QS.Fx.Reflection.Parameter("object", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<ObjectClass> _object_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
        : base(_mycontext, _debug)
        {
            this._identifier = _identifier;
            this._object_reference = _object_reference;
            this._delegation_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>,
                    QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IdentifierClass _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<ObjectClass> _object_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>,
            QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>> _delegation_endpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDelegationChannel<IdentifierClass,ObjectClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>,
            QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>>
                QS.Fx.Object.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Delegation
        {
            get { return this._delegation_endpoint; }
        }

        #endregion

        #region IDelegationChannel<IdentifierClass,ObjectClass> Members

        void QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Register(IdentifierClass _id)
        {
            if (_id.Equals(this._identifier))
                this._delegation_endpoint.Interface.Delegate(this._identifier, this._object_reference);
        }

        void QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Unregister(IdentifierClass _id)
        {
            this._delegation_endpoint.Interface.Undelegate(_id);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
