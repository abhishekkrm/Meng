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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_Parameter_ : Element_Port_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Parameter_(
            string _id, QS.Fx.Reflection.ParameterClass _parameterclass, Element_ValueClass_ _valueclass,
            QS.Fx.Attributes.IAttributes _attributes, Element_ _value, 
            Element_Environment_ _context, bool _automatic, QS.Fx.Reflection.IParameter _template_parameter)
            : base(Element_Port_.Category_.Parameter_, _automatic)
        {
            this._id = _id;
            this._parameterclass = _parameterclass;
            this._valueclass = _valueclass;
            this._attributes = _attributes;
            this._value = _value;
            this._context = _context;
            this._template_parameter = _template_parameter;
        }

        public Element_Parameter_(Element_Parameter_ _other) : base(Element_Port_.Category_.Parameter_, true)
        {
            this._id = _other._id;
            this._parameterclass = _other._parameterclass;
            this._valueclass = (_other._valueclass != null) ? new Element_ValueClass_(_other._valueclass) : null;
            this._attributes = _other._attributes;
            this._context = _other._context;
            this._template_parameter = _other._template_parameter;
            if (_other._value != null)
            {
                switch (this._parameterclass)
                {
                    case QS.Fx.Reflection.ParameterClass.ValueClass:
                        this._value = new Element_ValueClass_((Element_ValueClass_)_other._value);
                        break;
                    case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                        this._value = new Element_InterfaceClass_((Element_InterfaceClass_)_other._value);
                        break;
                    case QS.Fx.Reflection.ParameterClass.EndpointClass:
                        this._value = new Element_EndpointClass_((Element_EndpointClass_)_other._value);
                        break;
                    case QS.Fx.Reflection.ParameterClass.ObjectClass:
                        this._value = new Element_ObjectClass_((Element_ObjectClass_)_other._value);
                        break;
                    case QS.Fx.Reflection.ParameterClass.Value:
                        throw new NotImplementedException();
                }
            }
            else
                this._value = null;
        }

        #endregion

        #region Fields

        private string _id;
        private QS.Fx.Reflection.ParameterClass _parameterclass;
        private Element_ValueClass_ _valueclass;
        private QS.Fx.Attributes.IAttributes _attributes;
        private Element_ _value;
        private Element_Environment_ _context;
        private QS.Fx.Reflection.IParameter _template_parameter, _reflected_parameter;
        private bool _isobject;

//        private Element_Parameter_ _original;

        #endregion

        #region Static Fields

        private static readonly Type _objectreferencetype =
            typeof(QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Reflected_Parameter

        public QS.Fx.Reflection.IParameter _Reflected_Parameter
        {
            get { return this._reflected_parameter; }
        }

        #endregion

        #region _ID

        public string _ID
        {
            get { return _id; }
        }

        #endregion

        #region _ParameterClass

        public QS.Fx.Reflection.ParameterClass _ParameterClass
        {
            get { return _parameterclass; }
        }

        #endregion

        #region _ValueClass

        public Element_ValueClass_ _ValueClass
        {
            get { return this._valueclass; }
        }

        #endregion

        #region _Value

        public Element_ _Value
        {
            get { return _value; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Validate

        public override void _Validate()
        {
            lock (this)
            {
                this._correct = true;
                this._error = null;
                StringBuilder _ss = null;
                if (this._value != null)
                {
                    this._value._Validate();
                    if (!this._value._Correct)
                    {
                        this._reflected_parameter = null;
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Error in the value of parameter \"" + this._id + "\".");
                    }
                }
                if (true) // this._correct)
                {
                    switch (this._parameterclass)
                    {
                        #region ValueClass, InterfaceClass, EndpointClass, ObjectClass

                        case QS.Fx.Reflection.ParameterClass.ValueClass:
                        case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                        case QS.Fx.Reflection.ParameterClass.EndpointClass:
                        case QS.Fx.Reflection.ParameterClass.ObjectClass:
                            {
                                if (this._value != null)
                                {
                                    switch (this._parameterclass)
                                    {
                                        #region ValueClass

                                        case QS.Fx.Reflection.ParameterClass.ValueClass:
                                            {
                                                if (this._value is Element_ValueClass_)
                                                {
                                                    Element_ValueClass_ _valueclass = (Element_ValueClass_) this._value;
                                                    QS.Fx.Reflection.IValueClass _reflected_valueclass = _valueclass._Reflected_ValueClass;
                                                    this._reflected_parameter = new QS.Fx.Reflection.Parameter(
                                                        this._id, this._attributes, this._parameterclass, null, null, _reflected_valueclass);
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The class assigned to the type parameter \"" + this._id + 
                                                        "\" must be a value class.");
                                                }
                                            }
                                            break;

                                        #endregion

                                        #region InterfaceClass

                                        case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                                            {
                                                if (this._value is Element_InterfaceClass_)
                                                {
                                                    Element_InterfaceClass_ _interfaceclass = (Element_InterfaceClass_) this._value;
                                                    QS.Fx.Reflection.IInterfaceClass _reflected_interfaceclass = _interfaceclass._Reflected_InterfaceClass;
                                                    this._reflected_parameter = new QS.Fx.Reflection.Parameter(
                                                        this._id, this._attributes, this._parameterclass, null, null, _reflected_interfaceclass);
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The class assigned to the type parameter \"" + this._id + 
                                                        "\" must be an interface class.");
                                                }
                                            }
                                            break;

                                        #endregion

                                        #region EndpointClass

                                        case QS.Fx.Reflection.ParameterClass.EndpointClass:
                                            {
                                                if (this._value is Element_EndpointClass_)
                                                {
                                                    Element_EndpointClass_ _endpointclass = (Element_EndpointClass_) this._value;
                                                    QS.Fx.Reflection.IEndpointClass _reflected_endpointclass = _endpointclass._Reflected_EndpointClass;
                                                    this._reflected_parameter = new QS.Fx.Reflection.Parameter(
                                                        this._id, this._attributes, this._parameterclass, null, null, _reflected_endpointclass);
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The class assigned to the type parameter \"" + this._id +
                                                        "\" must be an endpoint class.");
                                                }
                                            }
                                            break;

                                        #endregion

                                        #region ObjectClass

                                        case QS.Fx.Reflection.ParameterClass.ObjectClass:
                                            {
                                                if (this._value is Element_ObjectClass_)
                                                {
                                                    Element_ObjectClass_ _objectclass = (Element_ObjectClass_)this._value;
                                                    QS.Fx.Reflection.IObjectClass _reflected_objectclass = _objectclass._Reflected_ObjectClass;
                                                    this._reflected_parameter = new QS.Fx.Reflection.Parameter(
                                                        this._id, this._attributes, this._parameterclass, null, null, _reflected_objectclass);
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The class assigned to the type parameter \"" + this._id +
                                                        "\" must be an object class.");
                                                }
                                            }
                                            break;

                                        #endregion
                                    }
                                }
                                else
                                {
                                    this._reflected_parameter = null;
                                    this._correct = false;
                                    if (_ss == null)
                                        _ss = new StringBuilder();
                                    _ss.AppendLine("The type parameter \"" + this._id + "\" cannot be left undefined.");
                                }
                            }
                            break;

                        #endregion

                        #region Value

                        case QS.Fx.Reflection.ParameterClass.Value:
                            {
                                if (this._valueclass != null)
                                {
                                    this._valueclass._Validate();
                                    if (this._valueclass._Correct)
                                    {
                                        QS.Fx.Reflection.IValueClass _reflected_valueclass = this._valueclass._Reflected_ValueClass;
                                        QS.Fx.Reflection.IObjectClass _reflected_objectclass;
                                        if (QS._qss_x_.Reflection_.Library._IsAnObjectReference(_reflected_valueclass, out _reflected_objectclass))
                                        {
                                            if (this._value != null)
                                            {
                                                if (this._value is Element_Object_)
                                                {
                                                    if (this._value._Correct)
                                                    {
                                                        Element_Object_ _o = (Element_Object_)this._value;
                                                        QS.Fx.Reflection.IObjectClass _o_reflected_objectclass = _o._ObjectClass._Reflected_ObjectClass;
                                                        if (!_o_reflected_objectclass.IsSubtypeOf(_reflected_objectclass))
                                                        {
                                                            this._correct = false;
                                                            if (_ss == null)
                                                                _ss = new StringBuilder();
                                                            _ss.AppendLine("The class of the object passed as a value is not a subtype of the class of the object expected by this parameter.");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The value of parameter \"" + this._id + "\" should be an object reference.");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (this._value != null)
                                            {
                                                if (this._value is Element_ValueObject_)
                                                {
                                                    Element_ValueObject_ _o = (Element_ValueObject_)this._value;
                                                    if (_o._Object != null)
                                                    {
                                                        Type _o_underlyingtype = _o._Object.GetType();
                                                        QS.Fx.Reflection.IValueClass _o_reflected_valueclass =
                                                            QS._qss_x_.Reflection_.Library.ValueClassOf(_o_underlyingtype);
                                                        if (!_o_reflected_valueclass.IsSubtypeOf(_reflected_valueclass))
                                                        {
                                                            this._correct = false;
                                                            if (_ss == null)
                                                                _ss = new StringBuilder();
                                                            _ss.AppendLine("The .NET type of the value does not match the specified value class.");
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    this._reflected_parameter = null;
                                                    this._correct = false;
                                                    if (_ss == null)
                                                        _ss = new StringBuilder();
                                                    _ss.AppendLine("The value of parameter \"" + this._id + "\" should be an ordinary object.");
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        this._reflected_parameter = null;
                                        this._correct = false;
                                        if (_ss == null)
                                            _ss = new StringBuilder();
                                        _ss.AppendLine("Error in the value class specification of the value parameter \"" + this._id + "\".");
                                    }
                                }
                                else
                                {
                                    this._reflected_parameter = null;
                                    this._correct = false;
                                    if (_ss == null)
                                        _ss = new StringBuilder();
                                    _ss.AppendLine("The value parameter \"" + this._id + "\" cannot have an undefined value class.");
                                }
                            }
                            break;

                        #endregion

                        default:
                            throw new NotImplementedException();
                    }
                }
                if (_ss != null)
                    this._error = _ss.ToString();
            }
        }

        #endregion

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Text = "Parameter \"" + _id + "\" : " + _parameterclass.ToString();
            this.Nodes.Clear();
            if (this._valueclass != null)
            {
                this.Nodes.Add(this._valueclass);
                this._valueclass._Rebuild();
            }
            if (_value != null)
            {
                this.Nodes.Add(_value);
                _value._Rebuild();
            }
            this._AdjustTreeNodeAppearance();
        }

        #endregion

        #region _Highlight

        public override IEnumerable<Element_> _Highlight()
        {
            lock (this)
            {
                List<Element_> _highlighted = new List<Element_>();
                if (this._value != null)
                {
                    _highlighted.Add(this._value);
                    if (this._value is Element_Object_)
                        _highlighted.Add(((Element_Object_) this._value)._Reference);
                }
                return (_highlighted.Count > 0) ? _highlighted : null;
            }
        }

        #endregion

        #region _CreateComment

        public override string _CreateComment()
        {
            StringBuilder _ss = new StringBuilder();
            _ss.AppendLine("id = \"" + this._id + "\"");
            _ss.AppendLine("class = " + this._parameterclass.ToString());
            if (this._attributes != null)
            {
                QS.Fx.Attributes.IAttribute _attribute;
                if (this._attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _attribute) && (_attribute.Value != null))
                    _ss.AppendLine("name = \"" + _attribute.Value + "\"");
                if (this._attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_comment, out _attribute) && (_attribute.Value != null))
                    _ss.AppendLine("comment = \"" + _attribute.Value + "\"");
            }
            return _ss.ToString();
        }

        #endregion

        #region Serialize

        public QS.Fx.Reflection.Xml.Parameter _Serialize()
        {
            object _o;
            switch (this._parameterclass)
            {
                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    {
                        Element_ValueClass_ _valueclass = (Element_ValueClass_)this._value;
                        _o = _valueclass._Serialize();
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    {
                        Element_InterfaceClass_ _interfaceclass = (Element_InterfaceClass_)this._value;
                        _o = _interfaceclass._Serialize();
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    {
                        Element_EndpointClass_ _endpointclass = (Element_EndpointClass_)this._value;
                        _o = _endpointclass._Serialize();
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    {
                        Element_ObjectClass_ _objectclass = (Element_ObjectClass_)this._value;
                        _o = _objectclass._Serialize();
                    }
                    break;

                case QS.Fx.Reflection.ParameterClass.Value:
                    {
                        if (this._value != null)
                        {
                            if (this._value is Element_Object_)
                                _o = ((Element_Object_)this._value)._Serialize();
                            else if (this._value is Element_ValueObject_)
                                _o = ((Element_ValueObject_)this._value)._Serialize();
                            else
                                throw new NotImplementedException();
                        }
                        else
                        {
                            if (QS._qss_x_.Reflection_.Library._IsAnObjectReference(this._valueclass._Reflected_ValueClass))
                                _o = new QS.Fx.Reflection.Xml.ReferenceObject(null, null, null, null, null, null);
                            else
                                _o = null;
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
            return new QS.Fx.Reflection.Xml.Parameter(this._id, _o);
        }

        #endregion

        #region _DropOk

        public override bool _DropOk(QS._qss_x_.ObjectDesigner_.Elements_.Category_ _category)
        {
            lock (this)
            {
                if (!this._automatic)
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ValueClass_:
                            return this._parameterclass == QS.Fx.Reflection.ParameterClass.ValueClass;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.InterfaceClass_:
                            return this._parameterclass == QS.Fx.Reflection.ParameterClass.InterfaceClass;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.EndpointClass_:
                            return this._parameterclass == QS.Fx.Reflection.ParameterClass.EndpointClass;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            return this._parameterclass == QS.Fx.Reflection.ParameterClass.ObjectClass;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.OrdinaryObject_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.Value)
                                {
                                    if (this._valueclass._Correct)
                                    {
                                        bool _object_expected = 
                                            QS._qss_x_.Reflection_.Library._IsAnObjectReference(this._valueclass._Reflected_ValueClass);
                                        bool _object_delivered = 
                                            (_category == QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_);
                                        return _object_expected == _object_delivered;
                                    }
                                    else
                                        return false;
                                }
                                else
                                    return false;
                            }
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
                if (!this._automatic)
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ValueClass_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.ValueClass)
                                {
                                    this._value = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.InterfaceClass_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.InterfaceClass)
                                {
                                    this._value = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.EndpointClass_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.EndpointClass)
                                {
                                    this._value = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.ObjectClass)
                                {
                                    this._value = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.Value)
                                {
                                    this._value = _element;
                                }
                            }
                            break;
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.OrdinaryObject_:
                            {
                                if (this._parameterclass == QS.Fx.Reflection.ParameterClass.Value)
                                {
                                    this._value = _element;
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

        #region _Menu

        public override IEnumerable<Element_Action_> _Menu()
        {
            List<Element_Action_> _menu = new List<Element_Action_>(base._Menu());
            if (this._parameterclass == QS.Fx.Reflection.ParameterClass.Value)
            {
                if ((this._valueclass != null) && (this._valueclass._Correct))
                {
                    if (!QS._qss_x_.Reflection_.Library._IsAnObjectReference(this._valueclass._Reflected_ValueClass))
                    {
                        _menu.Add(
                            new Element_Action_(
                                "Change Value", new QS.Fx.Base.ContextCallback(this._ChangeValueCallback), null));
                    }
                }
            }
            return _menu;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ChangeValueCallback

        private void _ChangeValueCallback(object _context)
        {
            ValueObjectForm_ _form = 
                new ValueObjectForm_(
                    this._valueclass._Reflected_ValueClass.UnderlyingType, 
                    (this._value != null) ? ((Elements_.Element_ValueObject_) this._Value)._Object : null);
            _form.Text = "Parameter \"" + this._id + "\"";
            _form.StartPosition = FormStartPosition.Manual;
            _form.Location = this.TreeView.PointToScreen(this.Bounds.Location);
            _form.ShowDialog();
            this._value = new Elements_.Element_ValueObject_(_form._Object);
            this._Validate();
            this._Rebuild();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
