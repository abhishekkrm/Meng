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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_membership)]
    [XmlType("Membership")]
    [QS.Fx.Printing.Printable("Membership", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Membership<
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("MemberClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MemberClass>
        : QS.Fx.Value.Classes.IMembership<IncarnationClass, MemberClass>
    {
        #region Constructor

        public Membership(IncarnationClass incarnation, bool incremental, MemberClass[] members)
        {
            this.incarnation = incarnation;
            this.incremental = incremental;
            this.members = members;
        }

        public Membership()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("incarnation")]
        private IncarnationClass incarnation;
        [QS.Fx.Printing.Printable("incremental")]
        private bool incremental;
        [QS.Fx.Printing.Printable("members")]
        private MemberClass[] members;

        #endregion

        #region Accessors

        [XmlElement("Incarnation")]
        public IncarnationClass Incarnation
        {
            get { return this.incarnation; }
            set { this.incarnation = value; }
        }

        [XmlElement("Incremental")]
        public bool Incremental
        {
            get { return this.incremental; }
            set { this.incremental = value; }
        }

        [XmlElement("Member")]
        public MemberClass[] Members
        {
            get { return this.members; }
            set { this.members = value; }
        }

        #endregion

        #region IMembership<IncarnationClass,MemberClass> Members

        [XmlIgnore]
        IncarnationClass QS.Fx.Value.Classes.IMembership<IncarnationClass, MemberClass>.Incarnation
        {
            get { return this.incarnation; }
        }

        [XmlIgnore]
        bool QS.Fx.Value.Classes.IMembership<IncarnationClass, MemberClass>.Incremental
        {
            get { return this.incremental; }
        }

        [XmlIgnore]
        IEnumerable<MemberClass> QS.Fx.Value.Classes.IMembership<IncarnationClass, MemberClass>.Members
        {
            get { return this.members; }
        }

        #endregion
    }
}
