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

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._c_property)]
    [XmlType("Property")]
    [QS.Fx.Printing.Printable("Property", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Property
    {
        #region Constructor

        public Property(int id, string name, string comment, QS.Fx.Reflection.Xml.ValueClass valueclass, string initialvalue, PropertyDependencyClass dependencyclass, 
            PropertyBinding[] sources, PropertyBinding[] destinations)
        {
            this.id = id;
            this.name = name;
            this.comment = comment;
            this.valueclass = valueclass;
            this.initialvalue = initialvalue;
            this.dependencyclass = dependencyclass;
            this.sources = sources;
            this.destinations = destinations;
        }

        public Property()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("id")]
        private int id;
        [QS.Fx.Printing.Printable("name")]
        private string name;
        [QS.Fx.Printing.Printable("comment")]
        private string comment;
        [QS.Fx.Printing.Printable("valueclass")]
        private QS.Fx.Reflection.Xml.ValueClass valueclass;
        [QS.Fx.Printing.Printable("initialvalue")]
        private string initialvalue;
        [QS.Fx.Printing.Printable("dependencyclass")]
        private PropertyDependencyClass dependencyclass;
        [QS.Fx.Printing.Printable("sources")]
        private PropertyBinding[] sources;
        [QS.Fx.Printing.Printable("destinations")]
        private PropertyBinding[] destinations;

        #endregion

        #region Accessors

        [XmlAttribute("id")]
        public int Id
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [XmlElement("Name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [XmlElement("Comment")]
        public string Comment
        {
            get { return this.comment; }
            set { this.comment = value; }
        }

        [XmlElement("DependencyClass")]
        public PropertyDependencyClass DependencyClass
        {
            get { return this.dependencyclass; }
            set { this.dependencyclass = value; }
        }

        [XmlElement("ValueClass")]
        public QS.Fx.Reflection.Xml.ValueClass ValueClass
        {
            get { return this.valueclass; }
            set { this.valueclass = value; }
        }

        [XmlElement("InitialValue")]
        public string InitialValue
        {
            get { return this.initialvalue; }
            set { this.initialvalue = value; }
        }

        [XmlElement("Source")]
        public PropertyBinding[] Sources
        {
            get { return this.sources; }
            set { this.sources = value; }
        }

        [XmlElement("Destination")]
        public PropertyBinding[] Destinations
        {
            get { return this.destinations; }
            set { this.destinations = value; }
        }

        #endregion
    }
}
