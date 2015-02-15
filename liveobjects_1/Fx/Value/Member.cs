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
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses._s_member_2)]
    [XmlType("Member")]
    [QS.Fx.Printing.Printable("Member", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Member<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("NameClass", QS.Fx.Reflection.ParameterClass.ValueClass)] NameClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>
    {
        #region Constructor

        public Member(IdentifierClass identifier, bool operational, IncarnationClass incarnation, NameClass name, AddressClass[] addresses)
        {
            this.identifier = identifier;
            this.operational = operational;
            this.incarnation = incarnation;
            this.name = name;
            this.addresses = addresses;
        }

        public Member()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("identifier")]
        private IdentifierClass identifier;
        [QS.Fx.Printing.Printable("operational")]
        private bool operational;
        [QS.Fx.Printing.Printable("incarnation")]
        private IncarnationClass incarnation;
        [QS.Fx.Printing.Printable("name")]
        private NameClass name;
        [QS.Fx.Printing.Printable("addresses")]
        private AddressClass[] addresses;

        #endregion

        #region Accessors

        [XmlElement("Identifier")]
        public IdentifierClass Identifier
        {
            get { return this.identifier; }
            set { this.identifier = value; }
        }

        [XmlElement("Operational")]
        public bool Operational
        {
            get { return this.operational; }
            set { this.operational = value; }
        }

        [XmlElement("Incarnation")]
        public IncarnationClass Incarnation
        {
            get { return this.incarnation; }
            set { this.incarnation = value; }
        }

        [XmlElement("Name")]
        public NameClass Name
        {
            get { return this.name; }
            set { this.name = value; }
        }


        [XmlElement("Address")]
        public AddressClass[] Addresses
        {
            get { return this.addresses; }
            set { this.addresses = value; }
        }

        #endregion

        #region IMember<IdentifierClass,IncarnationClass,NameClass,AddressClass> Members

        [XmlIgnore]
        IdentifierClass QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>.Identifier
        {
            get { return this.identifier; }
        }

        [XmlIgnore]
        IncarnationClass QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>.Incarnation
        {
            get { return this.incarnation; }
        }

        [XmlIgnore]
        NameClass QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass>.Name
        {
            get { return this.name; }
        }

        [XmlIgnore]
        bool QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>.Operational
        {
            get { return this.operational; }
        }

        [XmlIgnore]
        IEnumerable<AddressClass> QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>.Addresses
        {
            get { return this.addresses; }
        }

        #endregion
    }
}
