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

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_Value_ // : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Value_(Category_ _category, Element_ValueClass_ _valueclass, Type _underlyingtype, object _object)
            : base()
        {
            this._category = _category;
            this._valueclass = _valueclass;
            this._object = _object;
            this._underlyingtype = _underlyingtype;

/*
            if (this._category == Category_.Value_)
                this._m_valueobject = new Element_ValueObject_(this);
            lock (typeof(Element_Value_))
            {
                if (_object_reference_valueclass == null)
                {
                    _object_reference_valueclass =
                        QS._qss_x_.Reflection_.Library.LocalLibrary.GetValueClass(QS.Fx.Reflection.ValueClasses._Object).Instantiate(
                            new QS.Fx.Reflection.IParameter[]
                            {
                                new QS.Fx.Reflection.Parameter("ObjectClass", null, QS.Fx.Reflection.ParameterClass.ObjectClass, null, null,
                                    QS._qss_x_.Reflection_.Library.LocalLibrary.GetObjectClass(QS.Fx.Reflection.ObjectClasses.Object))
                            });
                }
            }
*/
        }

        #endregion

        #region Fields

        private Element_ValueClass_ _valueclass;
        private Category_ _category;
        private object _object;
        private Type _underlyingtype;
        private Element_Value_ _original;
        private Element_ValueObject_ _m_valueobject;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*


        public Element_Value_(Element_Value_ _original)
        {
            this._cloned = true;
            this._original = _original;
            this._valueclass = new Element_ValueClass_(_original._valueclass);
            this._category = _original._category;
            this._underlyingtype = _original._underlyingtype;
            if (this._category == Category_.Value_)
                this._m_valueobject = new Element_ValueObject_(this);
            if (_original._object != null)
            {
                switch (this._category)
                {
                    case Category_.Object_:
                        this._object = new Element_Object_((Element_Object_)_original._object);
                        break;
                    case Category_.Value_:
                        this._object = _original._object;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
                this._object = null;
        }

        #endregion


        #region Static Fields

        private static QS.Fx.Reflection.IValueClass _object_reference_valueclass;

        #endregion


        #region Accessors

        public Element_ValueClass_ _ValueClass
        {
            get { return this._valueclass; }
        }

        public Category_ _Category
        {
            get { return this._category; }
        }

        public object _Object
        {
            get { return this._object; }
            set { this._object = value; }
        }

        public Type _UnderlyingType
        {
            get { return this._underlyingtype; }
        }

        #endregion

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Text = "Value";
            this.Nodes.Clear();
            this.Nodes.Add(_valueclass);
            _valueclass._Rebuild();
            switch (_category)
            {
                case Category_.Object_:
                    {
                        if ((_object != null) && (_object is Element_))
                        {
                            this.Nodes.Add((Element_)_object);
                            ((Element_)_object)._Rebuild();
                        }
                    }
                    break;

                case Category_.Value_:
                    {
                        if (_m_valueobject != null)
                        {
                            this.Nodes.Add(this._m_valueobject);
                            this._m_valueobject._Rebuild();
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            this._AdjustTreeNodeAppearance();
        }

        #endregion

        #region _Validate

        public override void _Validate()
        {
            lock (this)
            {
                this._correct = true;
                this._error = null;
                StringBuilder _ss = null;
                if (this._valueclass != null)
                {
                    this._valueclass._Validate();
                    if (!this._valueclass._Correct)
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Error in the value class specification.");
                    }
                    QS.Fx.Reflection.IValueClass _reflection_valueclass = this._valueclass._reflection_ValueClass;
                    switch (_category)
                    {
                        case Category_.Object_:
                            {
                                QS.Fx.Reflection.IObjectClass _reflection_objectclass;
                                if (QS._qss_x_.Reflection_.Library._IsAnObjectReference(_reflection_valueclass, out _reflection_objectclass))
                                {
                                    if (this._object != null)
                                    {
                                        Element_Object_ _valueobject = (Element_Object_) this._object;
                                        _valueobject._Validate();
                                        if (!_valueobject._Correct)
                                        {
                                            this._correct = false;
                                            if (_ss == null)
                                                _ss = new StringBuilder();
                                            _ss.AppendLine("Error in the definition of the object passed as a value.");
                                        }
                                        if (_valueobject._ObjectClass != null)
                                        {
// TODO: Fix this...............................................................................................................................................................................................................
                                            QS.Fx.Reflection.IObjectClass _o_objectclass = _valueobject._ObjectClass._reflection_ObjectClass;
                                            if (!_o_objectclass.IsSubtypeOf(_reflection_objectclass))
                                            {
                                                this._correct = false;
                                                if (_ss == null)
                                                    _ss = new StringBuilder();
                                                _ss.AppendLine("The class of the object passed as a value is not a subtype of the class of the object expected by this parameter.");
                                            }
                                        }
                                        else
                                        {
                                            this._correct = false;
                                            if (_ss == null)
                                                _ss = new StringBuilder();
                                            _ss.AppendLine("The class of the object passed as a value appears to be undefined.");
                                        }
                                    }
                                }
                                else
                                {
                                    this._correct = false;
                                    if (_ss == null)
                                        _ss = new StringBuilder();
                                    _ss.AppendLine("The value is expected to be an object reference, but it is not.");
                                }
                            }
                            break;

                        case Category_.Value_:
                            {
                                if (this._underlyingtype != null)
                                {
                                    QS.Fx.Reflection.IValueClass _vc = QS._qss_x_.Reflection_.Library.ValueClassOf(this._underlyingtype);
                                    if (!_vc.IsSubtypeOf(_reflection_valueclass))
                                    {
                                        this._correct = false;
                                        if (_ss == null)
                                            _ss = new StringBuilder();
                                        _ss.AppendLine("The .NET type of the value does not match the specified value class.");
                                    }
                                    if (this._object != null)
                                    {
                                        if (!this._underlyingtype.IsAssignableFrom(this._object.GetType()))
                                        {
                                            this._correct = false;
                                            if (_ss == null)
                                                _ss = new StringBuilder();
                                            _ss.AppendLine("The .NET type of the actual data object does not match the expected .NET value type.");
                                        }
                                    }
                                }
                                else
                                {
                                    this._correct = false;
                                    if (_ss == null)
                                        _ss = new StringBuilder();
                                    _ss.AppendLine("The value is not an object reference, but its .NET type is undefined.");
                                }
                                if (this._m_valueobject != null)
                                    _m_valueobject._Validate();
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    this._correct = false;
                    if (_ss == null)
                        _ss = new StringBuilder();
                    _ss.AppendLine("Missing value class specification.");
                }
                if (_ss != null)
                    this._error = _ss.ToString();
            }
        }

        #endregion

        #region _DropOk

        public override bool _DropOk(QS._qss_x_.ObjectDesigner_.Elements_.Category_ _category)
        {
            lock (this)
            {
                if (!this._cloned)
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ValueClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.InterfaceClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.EndpointClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            return false;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                            return this._category == Category_.Object_;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.OrdinaryObject_:
                            return this._category == Category_.Value_;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                    return false;
            }
        }

        #endregion

        #region _Drop

        public override void _Drop(QS._qss_x_.ObjectDesigner_.Elements_.Category_ _category, Element_ _element)
        {
            lock (this)
            {
                if (!this._cloned)
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ValueClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.InterfaceClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.EndpointClass_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                            {
                                if (this._category == Category_.Object_)
                                {
                                    this._object = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.OrdinaryObject_:
                            {
                                if (this._category == Category_.Value_)
                                {
                                    this._object = _element;
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region Serialize

        public object _Serialize()
        {
            switch (this._category)
            {
                case Category_.Object_:
                    {
                        if (this._object != null)
                        {
                            Element_Object_ _object = (Element_Object_)this._object;
                            return _object._Serialize();
                        }
                        else
                            return new QS.Fx.Reflection.Xml.ReferenceObject(null, null, null, null, null);
                    }
                    break;

                case Category_.Value_:
                    {
                        return this._object;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
*/ 
    }
}
