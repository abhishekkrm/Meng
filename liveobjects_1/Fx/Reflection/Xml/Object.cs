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
    [QS.Fx.Printing.Printable("Object", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._XmlObject)]
    [XmlInclude(typeof(ReferenceObject))]
    [XmlInclude(typeof(CompositeObject))]
    [XmlType(TypeName = "Object")]
    public abstract class Object
    {
        #region Constructors

        protected Object(string _id, Attribute[] _attributes, ObjectClass _objectclass, Object _authority, Parameter[] _parameters)
        {
            this._id = _id;
            this._attributes = _attributes;
            this._objectclass = _objectclass;
            this._authority = _authority;
            this._parameters = _parameters;
        }

        protected Object()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("id")]
        private string _id;

        private Attribute[] _attributes;

        [QS.Fx.Printing.Printable("objectclass")]
        private ObjectClass _objectclass;

        [QS.Fx.Printing.Printable("parameters")]
        private Parameter[] _parameters;

        [QS.Fx.Printing.Printable("authority")]
        private Object _authority;

        #endregion

        #region Accessors

        [XmlAttribute("id")]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlElement("Attribute")]
        public Attribute[] Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        [XmlElement("ObjectClass")]
        public ObjectClass ObjectClass
        {
            get { return _objectclass; }
            set { _objectclass = value; }
        }

        [XmlElement("Authority")]
        public Object Authority
        {
            get { return _authority; }
            set { _authority = value; }
        }

        [XmlElement("Parameter")]
        public Parameter[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        #endregion
    }
}
