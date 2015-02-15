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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_versioned)]
    [XmlType("Versioned")]
    [QS.Fx.Printing.Printable("Versioned", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Versioned<
        [QS.Fx.Reflection.Parameter("VersionClass", QS.Fx.Reflection.ParameterClass.ValueClass)] VersionClass,
        [QS.Fx.Reflection.Parameter("ValueClass", QS.Fx.Reflection.ParameterClass.ValueClass)] ValueClass>
        : QS.Fx.Value.Classes.IVersioned<VersionClass, ValueClass>
/*
        where VersionClass : class, QS.Fx.Serialization.ISerializable
        where ValueClass : class, QS.Fx.Serialization.ISerializable
*/
    {
        #region Constructor

        public Versioned(VersionClass version, ValueClass value)
        {
            this.version = version;
            this.value = value;
        }

        public Versioned()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("version")]
        private VersionClass version;
        [QS.Fx.Printing.Printable("value")]
        private ValueClass value;

        #endregion

        #region Accessors

        [XmlElement("Version")]
        public VersionClass Version
        {
            get { return this.version; }
            set { this.version = value; }
        }

        [XmlElement("Value")]
        public ValueClass Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        #endregion

        #region IVersioned<VersionClass,ValueClass> Members

        [XmlIgnore]
        VersionClass QS.Fx.Value.Classes.IVersioned<VersionClass, ValueClass>.Version
        {
            get { return this.version; }
        }

        [XmlIgnore]
        ValueClass QS.Fx.Value.Classes.IVersioned<VersionClass, ValueClass>.Value
        {
            get { return this.value; }
        }

        #endregion

/*
        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            { 
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
        }

        #endregion
*/
    }
}
