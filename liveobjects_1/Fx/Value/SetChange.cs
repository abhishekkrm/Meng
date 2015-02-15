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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_setchange)]
    [XmlType("SetChange")]
    [QS.Fx.Printing.Printable("SetChange", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class SetChange<
        [QS.Fx.Reflection.Parameter("MemberClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MemberClass>
        : QS.Fx.Value.Classes.ISetChange<MemberClass>
/*
        where ElementClass : class, QS.Fx.Serialization.ISerializable
*/
    {
        #region Constructor

        public SetChange(IEnumerable<MemberClass> add, IEnumerable<MemberClass> remove)
        {
            this.add = new List<MemberClass>(add);
            this.remove = new List<MemberClass>(remove);
        }

        public SetChange()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("add")]
        private List<MemberClass> add;
        [QS.Fx.Printing.Printable("remove")]
        private List<MemberClass> remove;

        #endregion

        #region Accessors

        [XmlElement("Add")]
        public MemberClass[] Add
        {
            get { return (this.add != null) ? this.add.ToArray() : null; }
            set { this.add = (value != null) ? new List<MemberClass>(value) : null; }
        }

        [XmlElement("Remove")]
        public MemberClass[] Remove
        {
            get { return (this.remove != null) ? this.remove.ToArray() : null; }
            set { this.remove = (value != null) ? new List<MemberClass>(value) : null; }
        }

        #endregion

        #region ISetChange<MemberClass> Members

        [XmlIgnore]
        IEnumerable<MemberClass> QS.Fx.Value.Classes.ISetChange<MemberClass>.Add
        {
            get { return this.add; }
        }

        [XmlIgnore]
        IEnumerable<MemberClass> QS.Fx.Value.Classes.ISetChange<MemberClass>.Remove
        {
            get { return this.remove; }
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
