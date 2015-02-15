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
using System.Xml.Serialization;

namespace QS.Fx.Reflection.Xml
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._XmlObjectClass)]
    [XmlType(TypeName = "ObjectClass")]
    public sealed class ObjectClass : Class
    {
        #region Constructors

        public ObjectClass(string _id, Parameter[] _parameters, ObjectConstraint[] _objectconstraints, EndpointConstraint[] _endpointconstraints)
            : base(_id, _parameters)
        {
            this._objectconstraints = _objectconstraints;
            this._endpointconstraints = _endpointconstraints;
        }

        public ObjectClass() : base()
        {
        }

        #endregion

        [QS.Fx.Printing.Printable("objectconstraints")]
        private ObjectConstraint[] _objectconstraints;

        [QS.Fx.Printing.Printable("endpointconstraints")]
        private EndpointConstraint[] _endpointconstraints;

        [XmlElement("ObjectConstraint")]
        public ObjectConstraint[] ObjectConstraints
        {
            get { return _objectconstraints; }
            set { _objectconstraints = value; }
        }

        [XmlElement("EndpointConstraint")]
        public EndpointConstraint[] EndpointConstraints
        {
            get { return _endpointconstraints; }
            set { _endpointconstraints = value; }
        }
    }
}
