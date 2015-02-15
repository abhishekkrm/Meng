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

namespace QS._qss_x_.Language_.AST_
{
    [XmlType("OnUpdate")]
    public sealed class OnUpdate : Trigger
    {
        #region Constructors

        public OnUpdate(string _property, string _from, string _to)
        {
            this._property = _property;
            this._from = _from;
            this._to = _to;
        }

        public OnUpdate(string _property) : this(_property, null, null)
        {
        }

        public OnUpdate()
        {
        }

        #endregion

        #region Fields

        private string _property, _from, _to;

        #endregion

        #region Accessors

        [XmlAttribute("Property")]
        public string Property
        {
            get { return _property; }
            set { _property = value; }
        }

        [XmlAttribute("From")]
        public string From
        {
            get { return _from; }
            set { _from = value; }
        }

        [XmlAttribute("To")]
        public string To
        {
            get { return _to; }
            set { _to = value; }
        }

        #endregion

        #region Overridden from Trigger

        public override TriggerCategory TriggerCategory
        {
            get { return TriggerCategory.Update; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is OnUpdate)
            {
                OnUpdate other = (OnUpdate)obj;
                return _property.Equals(other._property);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _property.GetHashCode();
        }

        #endregion
*/
    }
}
