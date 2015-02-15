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
    public sealed class Attributes : IAttributes
    {
        #region Constructor

        public Attributes(params IAttribute[] _attributes)
        {
            foreach (IAttribute _attribute in _attributes)
                this._attributes.Add(_attribute.Class, _attribute);
        }

        public Attributes(object _object)
        {
            Type type = _object.GetType();
            foreach (System.Reflection.FieldInfo info in type.GetFields(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (info.FieldType.Equals(typeof(string)))
                {
                    foreach (AttributeAttribute _attribute in info.GetCustomAttributes(typeof(AttributeAttribute), true))
                    {
                        QS.Fx.Base.ID id = new QS.Fx.Base.ID(_attribute.Class);
                        IAttributeClass c;
                        if (AttributeClasses.GetAttributeClass(id, out c))
                        {
                            IAttribute a = new _AttributeFromField(c, _object, info);
                            _attributes.Add(c, a);
                        }
                    }
                }
            }
            foreach (System.Reflection.PropertyInfo info in type.GetProperties(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                if (info.PropertyType.Equals(typeof(string)))
                {
                    foreach (AttributeAttribute _attribute in info.GetCustomAttributes(typeof(AttributeAttribute), true))
                    {
                        QS.Fx.Base.ID id = new QS.Fx.Base.ID(_attribute.Class);
                        IAttributeClass c;
                        if (AttributeClasses.GetAttributeClass(id, out c))
                        {
                            IAttribute a = new _AttributeFromProperty(c, _object, info);
                            _attributes.Add(c, a);
                        }
                    }
                }
            }
        }

        #endregion

        #region Fields

        private IDictionary<IAttributeClass, IAttribute> _attributes = new Dictionary<IAttributeClass, IAttribute>();

        #endregion

        #region IAttributes Members

        bool IAttributes.Get(IAttributeClass _class, out IAttribute _attribute)
        {
            lock (this)
            {
                return _attributes.TryGetValue(_class, out _attribute);
            }
        }

        IAttribute IAttributes.Get(IAttributeClass _class)
        {
            lock (this)
            {
                IAttribute _attribute;
                if (!_attributes.TryGetValue(_class, out _attribute))
                    throw new Exception("Cannot attribute of class \"" + _class.ID.ToString() + "\".");
                return _attribute;
            }
        }

        #endregion

        #region IEnumerable<IAttribute> Members

        IEnumerator<IAttribute> IEnumerable<IAttribute>.GetEnumerator()
        {
            lock (this)
            {
                List<IAttribute> __attributes = new List<IAttribute>(_attributes.Values);
                return __attributes.GetEnumerator();
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                List<object> __attributes = new List<object>();
                foreach (Attribute _attribute in _attributes.Values)
                    __attributes.Add(_attribute);
                return __attributes.GetEnumerator();
            }
        }

        #endregion

        #region Class _AttributeFromField

        private class _AttributeFromField : IAttribute
        {
            #region Constructor

            public _AttributeFromField(IAttributeClass _class, object _object, System.Reflection.FieldInfo _fieldinfo)
            {
                this._class = _class;
                this._object = _object;
                this._fieldinfo = _fieldinfo;
            }

            #endregion

            #region Fields

            private IAttributeClass _class;
            private object _object;
            private System.Reflection.FieldInfo _fieldinfo;

            #endregion

            #region IAttribute Members

            IAttributeClass IAttribute.Class
            {
                get { return _class; }
            }

            string IAttribute.Value
            {
                get { return (string) _fieldinfo.GetValue(_object); }
            }

            #endregion
        }

        #endregion

        #region Class _AttributeFromProperty

        private class _AttributeFromProperty : IAttribute
        {
            #region Constructor

            public _AttributeFromProperty(IAttributeClass _class, object _object, System.Reflection.PropertyInfo _propertyinfo)
            {
                this._class = _class;
                this._object = _object;
                this._propertyinfo = _propertyinfo;
            }

            #endregion

            #region Fields

            private IAttributeClass _class;
            private object _object;
            private System.Reflection.PropertyInfo _propertyinfo;

            #endregion

            #region IAttribute Members

            IAttributeClass IAttribute.Class
            {
                get { return _class; }
            }

            string IAttribute.Value
            {
                get { return (string) _propertyinfo.GetValue(_object, null); }
            }

            #endregion
        }

        #endregion

        #region Serialize

        public static QS.Fx.Reflection.Xml.Attribute[] Serialize(QS.Fx.Attributes.IAttributes _attributes)
        {
            List<QS.Fx.Reflection.Xml.Attribute> _a = new List<QS.Fx.Reflection.Xml.Attribute>();
            if (_attributes != null)
            {
                foreach (QS.Fx.Attributes.IAttribute _attribute in _attributes)
                    _a.Add(new QS.Fx.Reflection.Xml.Attribute(_attribute.Class.ID.ToString(), _attribute.Value));
            }
            return _a.ToArray();
        }

        #endregion
    }
}
