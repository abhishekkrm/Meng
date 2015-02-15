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

namespace QS.Fx.Attributes
{
    public sealed class AttributeClass : ObjectWithAttributes, IAttributeClass
    {
        #region Constructor

        public AttributeClass(QS.Fx.Base.ID _id, string _name, string _comment)
        {
            this._id = _id;
            this._name = _name;
            this._comment = _comment;
        }

        #endregion

        #region Fields

        private QS.Fx.Base.ID _id;
        private string _name, _comment;

        #endregion

        #region Attributes

        [QS.Fx.Attributes.Attribute(AttributeClasses.CLASSID_id)]
        private string _attribute_id
        {
            get { return _id.ToString(); }
        }

        [QS.Fx.Attributes.Attribute(AttributeClasses.CLASSID_name)]
        private string _attribute_name
        {
            get { return _name; }
        }

        [QS.Fx.Attributes.Attribute(AttributeClasses.CLASSID_comment)]
        private string _attribute_comment
        {
            get { return _comment; }
        }

        #endregion

        #region IAttributeClass Members

        QS.Fx.Base.ID IAttributeClass.ID
        {
            get { return _id; }
        }

        #endregion

        #region IComparable<IAttributeClass> Members

        int IComparable<IAttributeClass>.CompareTo(IAttributeClass other)
        {
            return ((IComparable<QS.Fx.Base.ID>) _id).CompareTo(other.ID);
        }

        #endregion

        #region IEquatable<IAttributeClass> Members

        bool IEquatable<IAttributeClass>.Equals(IAttributeClass other)
        {
            return (other != null) && ((IComparable<QS.Fx.Base.ID>) _id).Equals(other.ID);
        }

        #endregion

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            return ((IComparable<IAttributeClass>)this).CompareTo(obj as IAttributeClass);
        }

        #endregion

        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            return ((IEquatable<IAttributeClass>) this).Equals(obj as IAttributeClass);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _id.ToString();
        }

        #endregion
    }
}
