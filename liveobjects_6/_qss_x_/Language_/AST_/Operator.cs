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
    public abstract class Operator
    {
        #region Constructors

        protected Operator(ValueType _valuetype, List<ValueType> _parametertypes)
        {
            this._valuetype = _valuetype;
            this._parametertypes = _parametertypes;
        }

        protected Operator()
        {
        }

        #endregion

        #region Fields

        private ValueType _valuetype;
        private List<ValueType> _parametertypes;

        #endregion

        #region Accessors

        [XmlIgnore]
        public abstract OperatorCategory OperatorCategory
        {
            get;
        }

        [XmlElement("ValueType")]
        public ValueType ValueType
        {
            get { return _valuetype; }
            set { _valuetype = value; }
        }

        [XmlElement("ParameterType")]
        public ValueType[] ParameterTypes
        {
            get { return (_parametertypes != null) ? _parametertypes.ToArray() : null; }
            set { _parametertypes = ((value != null) ? new List<ValueType>(value) : new List<ValueType>()); }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Operator)
            {
                Operator other = (Operator) obj;
                return
                    (((_valuetype == null) && (other._valuetype == null)) ||
                    ((_valuetype != null) && (other._valuetype != null)) && _valuetype.Equals(other._valuetype)) &&                    
                    (((_parametertypes == null) && (other._parametertypes == null)) ||
                    ((_parametertypes != null) && (other._parametertypes != null)) && _parametertypes.Equals(other._parametertypes));
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return ((_valuetype != null) ? _valuetype.GetHashCode() : 0) ^ ((_parametertypes != null) ? _parametertypes.GetHashCode() : 0);
        }

        #endregion
*/ 
    }
}
