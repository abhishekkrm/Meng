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

namespace QS._qss_x_.Properties_.Base_
{
    public sealed class Property_ : QS.Fx.Inspection.Inspectable, IProperty_
    {
        #region Constructor

        public Property_
        (
            QS.Fx.Base.Index _index,
            QS.Fx.Base.Identifier _identifier,
            string _name,
            string _comment,
            QS.Fx.Reflection.IValueClass _valueclass,
            QS.Fx.Value.Classes.IPropertyVersion _version,
            QS.Fx.Serialization.ISerializable _value
/*
            IProperty_[] _dependencies
*/
        )
        {
            this._index = _index;
            this._identifier = _identifier;
            this._name = _name;
            this._comment = _comment;
            this._valueclass = _valueclass;
            this._isoutdated = false;
            this._isdefined = (_version != null);
            this._isupdated = this._isdefined;
            this._version = _version;
            this._value = _value;

/*
            this._forward = new List<IProperty_>();
            this._dependencies = _dependencies;
*/
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _index;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private string _name;
        [QS.Fx.Base.Inspectable]
        private string _comment;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Reflection.IValueClass _valueclass;
        [QS.Fx.Base.Inspectable]
        private bool _isdefined;
        [QS.Fx.Base.Inspectable]
        private bool _isoutdated;
        [QS.Fx.Base.Inspectable]
        private bool _isupdated;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IPropertyVersion _version;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Serialization.ISerializable _value;

/*
        [QS.Fx.Base.Inspectable]
        private List<IProperty_> _forward;
        [QS.Fx.Base.Inspectable]
        private IProperty_[] _dependencies;
*/

        #endregion

        #region IProperty_ Members

        QS.Fx.Base.Index IProperty_._Index
        {
            get { return this._index; }
        }

        QS.Fx.Base.Identifier IProperty_._Identifier
        {
            get { return this._identifier; }
        }

        string IProperty_._Name
        {
            get { return this._name; }
        }

        string IProperty_._Comment
        {
            get { return this._comment; }
        }

        bool IProperty_._IsDefined
        {
            get { return this._isdefined; }
            set { this._isdefined = value; }
        }

        bool IProperty_._IsOutdated
        {
            get { return this._isoutdated; }
            set { this._isoutdated = value; }
        }

        bool IProperty_._IsUpdated
        {
            get { return this._isupdated; }
            set { this._isupdated = value; }
        }

        QS.Fx.Value.Classes.IPropertyVersion IProperty_._Version
        {
            get { return this._version; }
            set { this._version = value; }
        }

        QS.Fx.Serialization.ISerializable IProperty_._Value
        {
            get { return this._value; }
            set { this._value = value; }
        }

        bool IProperty_._Update(QS.Fx.Value.Classes.IPropertyVersion _version, QS.Fx.Serialization.ISerializable _value)
        {
            if ((_version != null) && ((this._version == null) || (((IComparable<QS.Fx.Value.Classes.IPropertyVersion>) this._version).CompareTo(_version) < 0)))
            {
                this._version = _version;
                this._isdefined = true;
                this._isoutdated = false;
                bool _updated = (_value != null) ? ((this._value == null) || !_value.Equals(this._value)) : (this._value != null);
                this._value = _value;
                if (_updated)
                    this._isupdated = true;
                return _updated;
            }
            else
                return false;
        }



/*
        QS._qss_x_.Properties_.Base_.PropertyValue_ IProperty_._Value
        {
            get 
            {
                if (this._isoutdated)
                {
                    this._Recalculate();
                    this._isoutdated = false;
                }
                return this._value; 
            }
            
            set 
            {
                if (!this._isdefined || (!value._Incarnation.Equals(this._value._Incarnation) && !value._Index.Equals(this._value._Index)))
                {
                    this._value = value;
                    this._isdefined = true;
                    this._isoutdated = false;
                    this._isupdated = true;
                    foreach (IProperty_ _property in this._forward)
                        _property._Refresh();
                }
            }
        }

        void IProperty_._Forward(IProperty_ _property)
        {
            this._forward.Add(_property);
        }

        void IProperty_._Refresh()
        {
            this._isoutdated = true;
            this._isupdated = true;
        }

*/

        #endregion

/*
        #region _Recalculate

        private void _Recalculate()
        {
             // @@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion
*/
    }
}
