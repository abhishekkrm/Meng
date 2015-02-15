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
    [XmlType("Range")]
    public sealed class Range : Elements
    {
        #region Constructors

        public Range(Expression _from, Expression _to)
        {
            this._from = _from;
            this._to = _to;
        }

        public Range()
        {
        }

        #endregion

        #region Fields

        private Expression _from, _to;

        #endregion

        #region Accessors

        [XmlElement("From")]
        public Expression From
        {
            get { return _from; }
            set { _from = value; }
        }

        [XmlElement("To")]
        public Expression To
        {
            get { return _to; }
            set { _to = value; }
        }

        #endregion

        #region Overridden from Elements

        public override ElementsCategory ElementsCategory
        {
            get { return ElementsCategory.Range; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Range)
            {
                Range other = (Range) obj;
                return _from.Equals(other._from) && _to.Equals(other._to);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _from.GetHashCode() ^ _to.GetHashCode();
        }

        #endregion
*/
    }
}
