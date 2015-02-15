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
    [QS.Fx.Printing.Printable("Parameter", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [XmlInclude(typeof(ValueClass))]
    [XmlInclude(typeof(InterfaceClass))]
    [XmlInclude(typeof(EndpointClass))]
    [XmlInclude(typeof(ObjectClass))]
    [XmlInclude(typeof(ReferenceObject))]
    [XmlInclude(typeof(CompositeObject))]
    [XmlInclude(typeof(Objects))]
    // [XmlInclude(typeof(TemplateObject))]
    [XmlInclude(typeof(Parameters))]    
    [XmlInclude(typeof(QS.Fx.Configuration.Configuration))]
    [XmlInclude(typeof(QS.Fx.Base.ID))]
    [XmlInclude(typeof(QS.Fx.Base.UINT32))]
    [XmlInclude(typeof(QS.Fx.Base.UINT64))]
    [XmlInclude(typeof(QS.Fx.Base.UINT128))]
    [XmlInclude(typeof(QS.Fx.Base.Identifier))]
    [XmlInclude(typeof(QS.Fx.Base.Incarnation))]
    [XmlInclude(typeof(QS.Fx.Base.Index))]
    [XmlInclude(typeof(QS.Fx.Base.Address))]
    [XmlInclude(typeof(QS.Fx.Base.Name))]
    [XmlInclude(typeof(QS.Fx.Value.Properties))]
    [XmlInclude(typeof(QS.Fx.Value.Property))]
    [XmlType(TypeName = "Parameter")]
    public sealed class Parameter
    {
        public Parameter(string _id, object _value)
        {
            this._id = _id;
            this._value = _value;
        }

        public Parameter()
        {
        }

        private string _id;
        private object _value;

        [XmlAttribute("id")]
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        [XmlElement("Value")]
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}
