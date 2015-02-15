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

namespace QS._qss_x_.Reflection_
{
    internal sealed class ObjectConstraintClass : Class<QS.Fx.Reflection.IObjectConstraintClass, ObjectConstraintClass>, QS.Fx.Reflection.IObjectConstraintClass
    {
        public ObjectConstraintClass
        (
            Library.Namespace_ _namespace, 
            QS.Fx.Base.ID _id, 
            ulong _incarnation, 
            string _name, 
            string _comment, 
            Type _type,
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, 
            IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters,
            System.Reflection.ConstructorInfo _constructor,
            Type _type2
        )
        : base(_namespace, _id, _incarnation, _name, _comment, _type, _classparameters, _openparameters)
        {
            this._constructor = _constructor;
            this._type2 = _type2;
            this._weakermethod = _type2.GetMethod("Weaker", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (this._weakermethod == null)
                throw new Exception("Cannot find the \"Weaker\" method in type \"" + _type.FullName + "\".");
        }

        private System.Reflection.ConstructorInfo _constructor;
        private Type _type2;
        private System.Reflection.MethodInfo _weakermethod;

        QS.Fx.Reflection.IObjectConstraint QS.Fx.Reflection.IObjectConstraintClass.CreateConstraint()
        {
            QS.Fx.Reflection.IObjectConstraint _constraint = (QS.Fx.Reflection.IObjectConstraint) this._constructor.Invoke(new object[0]);
            return _constraint;
        }

        public bool WeakerThan(QS.Fx.Reflection.IObjectConstraint _constraint1, QS.Fx.Reflection.IObjectConstraint _constraint2)
        {
            return (bool) _weakermethod.Invoke(_constraint1, new object[] { _constraint2 });
        }

        protected override ObjectConstraintClass _Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            throw new NotImplementedException();
        }
    }
}
