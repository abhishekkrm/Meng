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
    [XmlType("Sequence")]
    public sealed class Sequence : Action
    {
        #region Constructors

        public Sequence(List<Action> _actions)
        {
            this._actions = _actions;
        }

        public Sequence()
        {
        }

        #endregion

        #region Fields

        private List<Action> _actions;

        #endregion

        #region Accessors

        [XmlElement("Action")]
        public Action[] Actions
        {
            get { return (_actions != null) ? _actions.ToArray() : null; }
            set { _actions = (value != null) ? new List<Action>(value) : new List<Action>(); }
        }

/*
        [XmlIgnore]
        public List<Action> Actions
        {
            get { return _actions; }
            set { _actions = value; }
        }
*/

        #endregion

        #region Overridden from Action

        public override ActionCategory ActionCategory
        {
            get { return ActionCategory.Sequence; }
        }

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Sequence)
            {
                Sequence other = (Sequence)obj;
                return _actions.Equals(other._actions);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _actions.GetHashCode();
        }

        #endregion
*/
    }
}
