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
    [QS.Fx.Printing.Printable("Objects", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._XmlObjects)]
    [XmlInclude(typeof(ReferenceObject))]
    [XmlInclude(typeof(CompositeObject))]
    [XmlType(TypeName = "Objects")]
    public abstract class Objects
    {
        #region Constructors

        protected Objects(ObjectClass objectclass, Object[] objects)
        {
            this.objectclass = objectclass;
            this.objects = objects;
        }

        protected Objects()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("objectclass")]
        private ObjectClass objectclass;

        [QS.Fx.Printing.Printable("objects")]
        private Object[] objects;

        #endregion

        #region Accessors

        [XmlElement("ObjectClass")]
        public ObjectClass ObjectClass
        {
            get { return this.objectclass; }
            set { this.objectclass = value; }
        }

        [XmlElement("Object")]
        public Object[] ObjectArray
        {
            get { return this.objects; }
            set { this.objects = value; }
        }

        #endregion
    }
}
