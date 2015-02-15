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

namespace QS._qss_x_.Backbone_.Scope
{
    public abstract class Scope : QS.Fx.Inspection.Inspectable, IScope
    {
        #region Constructor

        protected Scope(QS.Fx.Base.ID id, string name)
        {
            this.id = id;
            this.name = name;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Base.ID id;

        [QS.Fx.Base.Inspectable]
        protected string name;

        #endregion

        #region IComparable<IScope> Members

        int IComparable<IScope>.CompareTo(IScope other)
        {
            return ((IComparable<QS.Fx.Base.ID>) id).CompareTo(other.ID);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            IScope other = obj as IScope;
            if (other != null)
                return ((IComparable<QS.Fx.Base.ID>) id).CompareTo(other.ID);
            else
                throw new Exception("The argument of comparison is not a scope.");
        }

        #endregion

        #region IEquatable<IScope> Members

        bool IEquatable<IScope>.Equals(IScope other)
        {
            return ((IEquatable<QS.Fx.Base.ID>) id).Equals(other.ID);
        }

        #endregion

        #region System.Object Overrides

        public override bool Equals(object obj)
        {
            IScope other = obj as IScope;
            return (other != null) && ((IEquatable<QS.Fx.Base.ID>) id).Equals(other.ID);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return id.ToString();
        }

        #endregion

        #region IScope Members

        QS.Fx.Base.ID IScope.ID
        {
            get { return id; }
        }

        string IScope.Name
        {
            get { return name; }
        }

        ScopeType IScope.Type
        {
            get { return this.Type; }
        }

        #endregion

        #region Internal Interface

        internal QS.Fx.Base.ID ID
        {
            get { return id; }
        }

        internal string Name
        {
            get { return name; }
        }

        internal abstract ScopeType Type
        {
            get;
        }

        #endregion
    }
}
