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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_set)]
    [XmlType("Set")]
    [QS.Fx.Printing.Printable("Set", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Set<
        [QS.Fx.Reflection.Parameter("MemberClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MemberClass>
        : QS.Fx.Value.Classes.ISet<MemberClass>
    /*
            where ElementClass : class, QS.Fx.Serialization.ISerializable
    */
    {
        #region Constructor

        public Set(IEnumerable<MemberClass> members)
        {
            this.members = new List<MemberClass>(members);
        }

        public Set()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("members")]
        private List<MemberClass> members;

        #endregion

        #region Accessors

        [XmlElement("Member")]
        public MemberClass[] Members
        {
            get { return (this.members != null) ? this.members.ToArray() : null; }
            set { this.members = (value != null) ? new List<MemberClass>(value) : null; }
        }

        #endregion

        #region ISet<MemberClass> Members

        [XmlIgnore]
        IEnumerable<MemberClass> QS.Fx.Value.Classes.ISet<MemberClass>.Members
        {
            get { return this.members; }
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
