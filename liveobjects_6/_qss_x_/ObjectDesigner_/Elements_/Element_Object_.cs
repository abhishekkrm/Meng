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

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_Object_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Object_(
            string _id, QS.Fx.Attributes.IAttributes _attributes, Category_ _category, Element_Parameter_ _binding,
            Element_Environment_ _environment_1, Element_Environment_ _environment_2, 
            QS.Fx.Reflection.IComponentClass _componentclass, Element_ObjectClass_ _objectclass, Element_From_ _from, 
            IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Component_> _components,
            IList<QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_> _connections,            
            IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_> _endpoints)
            : base(false)
        {
            this._id = _id;
            this._attributes = _attributes;
            if (this._attributes != null)
            {
                QS.Fx.Attributes.IAttribute _nameattribute;
                if (this._attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                    this._name = _nameattribute.Value;
            }
            this._category = _category;
            switch (_category)
            {
                case Category_.Composite_:
                    this._categorylabel = "Composite Object";
                    break;
                case Category_.Predefined_:
                    this._categorylabel = "Predefined Object";
                    break;
                case Category_.Repository_:
                    this._categorylabel = "Repository Object";
                    break;
                case Category_.Parameter_:
                    this._categorylabel = "Parameter Object";
                    break;
                default:
                    throw new NotImplementedException();
            }
            this._binding = _binding;
            this._environment_1 = _environment_1;
            this._environment_2 = _environment_2;
            this._componentclass = _componentclass;
            this._objectclass = _objectclass;
            this._from = _from;
            this._components = _components;
            this._connections = _connections;
            this._endpoints = _endpoints;
            this._reference = new Element_Reference_(this);
        }

        #endregion

        #region Fields

        private Element_Environment_ _environment_1, _environment_2;
        private QS.Fx.Reflection.IComponentClass _componentclass;
        private Element_ObjectClass_ _objectclass;
        private Element_From_ _from;
        private Element_Parameter_ _binding;
        private IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Component_> _components;
        private IList<QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_> _connections;
        private QS.Fx.Attributes.IAttributes _attributes;
        private IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_> _endpoints;
        private Category_ _category;
        private string _categorylabel, _id, _name;
        private int _num_e, _num_p;
        private PointF _o_p, _o_q, _o_s, _from_s;
        private Graphics _g;
        // private SizeF _s1, _s2, _s3;
        private float _from_x, _from_y, _connections_start_x, _components_start_y;
        private Element_Reference_ _reference;
        private bool _expanded;

        #endregion

        #region Static Fields

        private static Pen _pen1 = new Pen(Color.Black, 1);
        private static Pen _pen2 = new Pen(Color.Yellow, 5);

        #endregion

        #region Category_

        public enum Category_
        {
            Predefined_, Composite_, Repository_, Parameter_
        }

        #endregion

        #region Constants

        private const float _c_marginwidth = 10;
        private const float _c_portspacing = 10;
        private const float _c_textheight = 20;
        private const float _c_textheight_small = 8;
        private const float _c_textwidth = 300;
        private const float _c_textspacing = 2;
        private const float _c_fontsize = 10;
        private const float _c_fontsize_small = 5;
        private const float _c_parameterspacing_x = 50;
        private const float _c_parameterspacing_y = 50;
        private const float _c_fromspacing_x = 50;
        private const float _c_fromspacing_y = 50;
        private const float _c_bounddistance = 5;
        private const float _c_stickingout = 0.1f;
        private const float _c_internalmargin = 10;
        private const float _c_componentspacing = 20;
        private const float _c_wirespacing = 20;
        private const float _c_hihglightedlinewidth = 10;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Reference

        public Element_Reference_ _Reference
        {
            get { return this._reference; }
        }

        #endregion

        #region _ObjectClass

        public Element_ObjectClass_ _ObjectClass
        {
            get { return _objectclass; }
        }

        #endregion

        #region _ID

        public string _ID
        {
            get { return this._id; }
        }

        #endregion

        #region _Attributes

        public QS.Fx.Attributes.IAttributes _Attributes
        {
            get { return _attributes; }
        }

        #endregion

        #region _Serialize

        public QS.Fx.Reflection.Xml.Object _Serialize()
        {
            List<QS.Fx.Reflection.Xml.Attribute> _xml_attributes = new List<QS.Fx.Reflection.Xml.Attribute>();
            if (this._attributes != null)
            {
                foreach (QS.Fx.Attributes.IAttribute _attribute in this._attributes)
                    _xml_attributes.Add(new QS.Fx.Reflection.Xml.Attribute(_attribute.Class.ID.ToString(), _attribute.Value));
            }

            QS.Fx.Reflection.Xml.ObjectClass _xml_objectclass = null;
            if (this._objectclass != null)
                _xml_objectclass = this._objectclass._Serialize();

            List<QS.Fx.Reflection.Xml.Parameter> _xml_parameters = new List<QS.Fx.Reflection.Xml.Parameter>();
            if (this._environment_1 != null)
                _xml_parameters.AddRange(this._environment_1._Serialize());
            if (this._environment_2 != null)
                _xml_parameters.AddRange(this._environment_2._Serialize());

            switch (this._category)
            {
                case Category_.Predefined_:
                    {
                        return new QS.Fx.Reflection.Xml.ReferenceObject(
                            this._id, _xml_attributes.ToArray(), _xml_objectclass, null, _xml_parameters.ToArray(), null);
                    }
                    break;

                case Category_.Repository_:
                    {
                        QS.Fx.Reflection.Xml.Object _xml_from = null;
                        if ((this._from != null) && (this._from._Object != null))
                            _xml_from = this._from._Object._Serialize();

                        return new QS.Fx.Reflection.Xml.ReferenceObject(
                            this._id, 
                            _xml_attributes.ToArray(), 
                            _xml_objectclass, 
                            null,
                            null,
                            _xml_from);
                    }
                    break;

                case Category_.Composite_:
                    {
                        List<QS.Fx.Reflection.Xml.CompositeObject.Component> _xml_components =
                            new List<QS.Fx.Reflection.Xml.CompositeObject.Component>();
                        if (this._components != null)
                        {
                            foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Component_ _component in this._components.Values)
                                _xml_components.Add(_component._Serialize());
                        }

                        List<QS.Fx.Reflection.Xml.CompositeObject.Endpoint> _xml_endpoints =
                            new List<QS.Fx.Reflection.Xml.CompositeObject.Endpoint>();
                        if (this._endpoints != null)
                        {
                            foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                                _xml_endpoints.Add(_endpoint._Serialize());
                        }

                        List<QS.Fx.Reflection.Xml.CompositeObject.Connection> _xml_connections =
                            new List<QS.Fx.Reflection.Xml.CompositeObject.Connection>();
                        if (this._connections != null)
                        {
                            foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_ _connection in this._connections)
                                _xml_connections.Add(_connection._Serialize());
                        }

                        return new QS.Fx.Reflection.Xml.CompositeObject(this._id, _xml_attributes.ToArray(), _xml_objectclass, null,
                            _xml_parameters.ToArray(), _xml_components.ToArray(), _xml_endpoints.ToArray(), _xml_connections.ToArray());
                    }
                    break;

                case Category_.Parameter_:
                    {
                        return new QS.Fx.Reflection.Xml.ReferenceObject("@" + this._id, null, _xml_objectclass, null, null, null);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            string _label = this._categorylabel;
            if (this._attributes != null)
            {
                QS.Fx.Attributes.IAttribute _nameattribute;
                if (this._attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                    _label += " \"" + _nameattribute.Value + "\"";
            }
            this.Text = _label;
            this.Nodes.Clear();
            if (_objectclass != null)
            {
                this.Nodes.Add(_objectclass);
                _objectclass._Rebuild();
            }
            if (_from != null)
            {
                this.Nodes.Add(_from);
                _from._Rebuild();
            }
            if (_environment_1 != null)
            {
                foreach (Element_Parameter_ _p in this._environment_1._Parameters.Values)
                {
                    this.Nodes.Add(_p);
                    _p._Rebuild();
                }
            }
            if (_environment_2 != null)
            {
                foreach (Element_Parameter_ _p in this._environment_2._Parameters.Values)
                {
                    this.Nodes.Add(_p);
                    _p._Rebuild();
                }
            }
            if (_endpoints != null)
            {
                foreach (Element_Endpoint_ _e in this._endpoints.Values)
                {
                    this.Nodes.Add(_e);
                    _e._Rebuild();
                }
            }
            if (_components != null)
            {
                foreach (Element_Component_ _c in this._components.Values)
                {
                    this.Nodes.Add(_c);
                    _c._Rebuild();
                }
            }
            this._AdjustTreeNodeAppearance();
        }

        #endregion

        #region _CreateComment

        public override string _CreateComment()
        {
            StringBuilder _ss = new StringBuilder();
            _ss.AppendLine("id = \"" + this._id + "\"");
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

        #region _DoubleClick

        public override void _DoubleClick()
        {
            lock (this)
            {
                this._expanded = !this._expanded;
            }
        }

        #endregion

        #region _Recalculate_1

        public override void _Recalculate_1()
        {
            List<Element_Object_> _objects = new List<Element_Object_>();
            this._num_p = 0;
            float _margin_x = _c_portspacing;
            if ((this._environment_2 != null) && (this._environment_2._Parameters != null) && (this._environment_2._Parameters.Count > 0))
            {
                foreach (Element_Parameter_ _parameter in this._environment_2._Parameters.Values)
                {
                    if ((_parameter._ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _parameter._ValueClass._Correct &&
                        QS._qss_x_.Reflection_.Library._IsAnObjectReference(_parameter._ValueClass._Reflected_ValueClass))
                    {
                        _parameter._Recalculate_1();
                        _margin_x += _parameter._S.X + _c_portspacing;
                        this._num_p++;
                        if (_parameter._Value != null)
                            _objects.Add((Element_Object_) _parameter._Value);
                    }
                }
            }
            this._num_e = 0;
            float _margin_y = _c_portspacing;
            if ((this._endpoints != null) && (this._endpoints.Count > 0))
            {
                foreach (Element_Endpoint_ _endpoint in this._endpoints.Values)
                {
                    _endpoint._Recalculate_1();
                    _margin_y += _endpoint._S.Y + _c_portspacing;
                    this._num_e++;
                }
            }
            if (this._from != null)
            {
                _from._Recalculate_1();
                _margin_y = Math.Max(_margin_y, _from._S.Y + 2 * _c_portspacing);
            }
            this._reference._Recalculate_1();
            _margin_x = Math.Max(_margin_x, _reference._S.X + 2 * _c_portspacing);
            float _text_x = 2 * _c_internalmargin + _c_textwidth;
            float _text_y = 2 * _c_internalmargin + _c_textheight + _c_textspacing;
            if (this._id != null)
            {
                switch (_category)
                {
                    case Category_.Predefined_:
                        _text_y += _c_textheight_small + _c_textspacing;
                        break;
                    case Category_.Repository_:
                        _text_y += _c_textheight + _c_textspacing;
                        break;
                    default:
                        break;
                }
            }
            if (this._name != null)
                _text_y += _c_textheight + _c_textspacing;
            // SizeF _ss = _g.MeasureString(this._categorylabel, new Font(FontFamily.GenericSansSerif, _Text_Size, FontStyle.Bold));
            // float _font = _ss.
            float _components_x = 0;
            float _components_y = 0;
            if (this._expanded)
            {
                if ((this._components != null) && (this._components.Count > 0))
                {
                    foreach (Element_Component_ _component in this._components.Values)
                    {
                        _component._Recalculate_1();
                        _components_x = Math.Max(_components_x, _component._S.X);
                        _components_y += _component._S.Y + _c_componentspacing;
                    }
                    foreach (Element_Component_ _component in this._components.Values)
                    {
                        _component._S = new PointF(_components_x, _component._S.Y);
                    }
                }
                if ((this._connections != null) && (this._connections.Count > 0))
                {
                    this._connections_start_x = _components_x + _c_wirespacing;
                    _components_x += (_connections.Count + 1) * _c_wirespacing;
                }
                _components_x += 2 * _c_internalmargin + (2 * _num_e + 1) * _c_wirespacing;
                _components_y += _c_componentspacing + (_num_e + 1) * _c_wirespacing;
            }
            this._components_start_y = _text_y + _c_componentspacing;
            float _body_x = Math.Max(_text_x, _components_x);
            float _body_y = _text_y + _components_y;
            this._o_s = new PointF(2 * _c_marginwidth + Math.Max(_body_x, _margin_x), 2 * _c_marginwidth + Math.Max(_body_y, _margin_y));
            this._from_x = 0;
            this._from_y = 0;
            float _objects_x = 0;
            float _objects_y = 0;
            if (this._expanded)
            {
                if (this._from != null)
                {
                    if (this._from._Object != null)
                    {
                        this._from._Object._Recalculate_1();
                        this._from_x = _c_fromspacing_x + this._from._Object._S.X - this._from._Object._o_s.X / 2;
                        this._from_y = _c_fromspacing_y + this._from._Object._S.Y + this._o_s.Y;
                    }
                }
                bool _first = true;
                foreach (Element_Object_ _o in _objects)
                {
                    if (_first)
                        _first = false;
                    else
                        _objects_x += _c_parameterspacing_x;
                    _o._Recalculate_1();
                    _objects_x += _o._S.X;
                    _objects_y = Math.Max(_objects_y, _o._S.Y);
                }
                if (_objects_y > 0)
                    _objects_y += _c_parameterspacing_y;
            }
            this._s = new PointF(this._from_x + Math.Max(this._o_s.X, _objects_x), Math.Max(this._from_y, this._o_s.Y + _objects_y));
        }

        #endregion

        #region _Recalculate_2

        public override void _Recalculate_2()
        {
            List<Element_Object_> _objects = new List<Element_Object_>();
            this._q.X = this._p.X + this._s.X;
            this._q.Y = this._p.Y + this._s.Y;
            this._o_p = new PointF(this._from_x + (this._s.X - this._from_x - this._o_s.X) / 2, 0);
            this._o_q = new PointF(this._from_x + (this._s.X - this._from_x + this._o_s.X) / 2, this._o_s.Y);
            _reference._P = new PointF(this._p.X + this._o_p.X + (this._o_s.X - _reference._S.X) / 2, this._p.Y + this._o_p.Y);
            _reference._Recalculate_2();
            float _my_x, _my_y;
            if (this._expanded)
            {
                _my_x = this._p.X + this._o_p.X + _c_marginwidth + _c_internalmargin;
                _my_y = this._p.Y + this._o_p.Y + _c_marginwidth + _c_internalmargin + _components_start_y;
                if (this._components != null)
                {
                    foreach (Element_Component_ _component in this._components.Values)
                    {
                        _component._P = new PointF(_my_x, _my_y);
                        _component._Recalculate_2();
                        _my_y += _component._S.Y + _c_componentspacing;
                    }
                }
            }
            _my_x = this._p.X + this._o_p.X + _c_marginwidth;
            List<Element_Parameter_> _parameters = new List<Element_Parameter_>();
            if ((this._environment_2 != null) && (this._environment_2._Parameters != null))
            {
                bool _isfirstparameter = true;
                foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._environment_2._Parameters.Values)
                {
                    if ((_parameter._ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _parameter._ValueClass._Correct &&
                        QS._qss_x_.Reflection_.Library._IsAnObjectReference(_parameter._ValueClass._Reflected_ValueClass))
                    {
                        if (_isfirstparameter)
                            _isfirstparameter = false;
                        else
                            _my_x += _c_portspacing;
                        _parameter._P = new PointF(_my_x, this._p.Y + this._o_q.Y - _parameter._S.Y);
                        _my_x += _parameter._S.X;
                        _parameters.Add(_parameter);
                        if (_parameter._Value != null)
                            _objects.Add((Element_Object_) _parameter._Value);
                    }
                }
            }
            float _my_x_shift = ((this._p.X + this._o_q.X - _c_marginwidth) - _my_x) / 2;
            foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in _parameters)
            {
                _parameter._P = new PointF(_parameter._P.X + _my_x_shift, _parameter._P.Y);
                _parameter._Recalculate_2();
            }
            _my_y = this._p.Y + this._o_p.Y + _c_marginwidth;
            if (this._endpoints != null)
            {
                bool _isfirstendpoint = true;
                foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                {
                    if (_isfirstendpoint)
                        _isfirstendpoint = false;
                    else
                        _my_y += _c_portspacing;
                    _endpoint._P = new PointF(this._p.X + this._o_q.X - _endpoint._S.X, _my_y);
                    _my_y += _endpoint._S.Y;
                }
                float _my_y_shift = ((this._p.Y + this._o_q.Y - _c_marginwidth) - _my_y) / 2;
                foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                {
                    _endpoint._P = new PointF(_endpoint._P.X, _endpoint._P.Y + _my_y_shift);
                    _endpoint._Recalculate_2();
            }
            }
            _my_x = this._p.X + this._from_x;
            _my_y = this._p.Y + this._o_q.Y + _c_parameterspacing_y;
            if (this._expanded)
            {
                foreach (Element_Object_ _o in _objects)
                {
                    _o._P = new PointF(_my_x, _my_y);
                    _o._Recalculate_2();
                    _my_x = _o._Q.X + _c_parameterspacing_x;
                }
            }
            if (this._from != null)
            {
                this._from._P = new PointF(this._p.X + this._o_p.X, this._p.Y + this._o_p.Y + (this._o_s.Y - this._from._S.Y) / 2);
                this._from._Recalculate_2();
                if (this._expanded)
                {
                    if (this._from._Object != null)
                    {
                        this._from._Object._P = new PointF(this._p.X, this._p.Y + _c_fromspacing_y + this._o_s.Y);
                        this._from._Object._Recalculate_2();
                    }
                }
            }
        }

        #endregion

        #region _Draw

        public override void _Draw(Graphics _g, PointF _p0, PointF _q0, float _z)
        {
            this._g = _g;
            _g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                this._CalculateCoordinates(_g, _p0, _z);
                PointF _o_pp = new PointF(_pp.X + this._o_p.X * _z, _pp.Y + this._o_p.Y * _z);
                PointF _o_ps = new PointF(this._o_s.X * _z, this._o_s.Y * _z);
                PointF _o_pq = new PointF(_pp.X + this._o_q.X * _z, _pp.Y + this._o_q.Y * _z);
                RectangleF _o_rec = new RectangleF(_o_pp.X, _o_pp.Y, _o_ps.X, _o_ps.Y);
                if (this._selected)
                {
                    Pen _boundpen = new Pen(Color.Black, 1 * _z);
                    _boundpen.DashStyle = DashStyle.Dot;
                    _g.DrawRectangle(_boundpen, _pp.X - _c_bounddistance * _z, _pp.Y - _c_bounddistance * _z,
                        _ps.X + 2 * _c_bounddistance * _z, _ps.Y + 2 * _c_bounddistance * _z);
                }
                GraphicsPath _gp = _RoundedRectangle(_o_pp.X, _o_pp.Y, _o_ps.X, _o_ps.Y, 2 * _c_marginwidth * _z);
                switch (this._category)
                {
                    case Category_.Predefined_:
                    case Category_.Repository_:
                        {
                            Color _gradientbrushstartingcolor = Color.FromArgb(225, 225, 225);
                            Brush _gradientbrush = new LinearGradientBrush(
                                _o_rec, _gradientbrushstartingcolor, Color.White, LinearGradientMode.Horizontal);
                            _g.FillPath(_gradientbrush, _gp);
                        }
                        break;

                    case Category_.Composite_:
                        {
                            if (!this._expanded)
                            {
                                Color _gradientbrushstartingcolor = Color.FromArgb(225, 225, 225);
                                Brush _gradientbrush = new LinearGradientBrush(
                                    _o_rec, _gradientbrushstartingcolor, Color.White, LinearGradientMode.Horizontal);
                                _g.FillPath(_gradientbrush, _gp);
                            }
                        }
                        break;

                    case Category_.Parameter_:
                        {
                            _g.FillPath(Brushes.Ivory, _gp);
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
                if (this._selected)
                    _g.DrawPath(new Pen(Brushes.Yellow, 5 * _z), _gp);
                _g.DrawPath(new Pen(Brushes.Black, 2 * _z), _gp);
                float _my_x = _o_pp.X + (_c_marginwidth + _c_internalmargin) * _z;
                float _my_y = _o_pp.Y + (_c_marginwidth + _c_internalmargin) * _z;
                _g.DrawString((this._expanded ? "[-] " : "[+] ") + this._categorylabel,
                    new Font(FontFamily.GenericSansSerif, _c_fontsize * +_z, FontStyle.Bold), this._correct ? Brushes.Black : Brushes.Red,
                    new RectangleF(_my_x, _my_y, _c_textwidth * _z, _c_textheight * _z));
                _my_y += (_c_textheight + _c_textspacing) * _z;
                if (this._id != null)
                {
                    switch (_category)
                    {
                        case Category_.Predefined_:
                            {
                                _g.DrawString(this._id,
                                    new Font(FontFamily.GenericSansSerif, _c_fontsize_small * _z, FontStyle.Regular), Brushes.Gray,
                                    new RectangleF(_my_x, _my_y, _c_textwidth * _z, _c_textheight_small * _z));
                                _my_y += (_c_textheight_small + _c_textspacing) * _z;
                            }
                            break;
                        case Category_.Repository_:
                            {
                                _g.DrawString(this._id,
                                    new Font(FontFamily.GenericSansSerif, _c_fontsize * _z, FontStyle.Regular), Brushes.Blue,
                                    new RectangleF(_my_x, _my_y, _c_textwidth * _z, _c_textheight * _z));
                                _my_y += (_c_textheight + _c_textspacing) * _z;
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (this._name != null)
                {
                    _g.DrawString("\"" + this._name + "\"",
                        new Font(FontFamily.GenericSansSerif, _c_fontsize * _z, FontStyle.Regular), Brushes.Green,
                        new RectangleF(_my_x, _my_y, _c_textwidth * _z, _c_textheight * _z));
                    _my_y += (_c_textheight + _c_textspacing) * _z;
                }
                _reference._Draw(_g, _p0, _q0, _z);
                if ((this._environment_2 != null) && (this._environment_2._Parameters != null))
                {
                    foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._environment_2._Parameters.Values)
                    {
                        if ((_parameter._ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _parameter._ValueClass._Correct &&
                            QS._qss_x_.Reflection_.Library._IsAnObjectReference(_parameter._ValueClass._Reflected_ValueClass))
                        {
                            _parameter._Draw(_g, _p0, _q0, _z);
                            if (this._expanded)
                            {
                                if (_parameter._Value != null)
                                {
                                    Element_Object_ _o = (Element_Object_)_parameter._Value;
                                    _o._Draw(_g, _p0, _q0, _z);
                                    Element_Reference_ _ref = _o._Reference;
                                    PointF _pp_p1 = new PointF((_parameter._PP.X + _parameter._PQ.X) / 2, _parameter._PQ.Y);
                                    PointF _pp_p4 = new PointF((_ref._PP.X + _ref._PQ.X) / 2, _ref._PP.Y);
                                    PointF _pp_p2 = new PointF(_pp_p1.X, (1 - _c_stickingout) * _pp_p1.Y + _c_stickingout * _pp_p4.Y);
                                    PointF _pp_p3 = new PointF(_pp_p4.X, _c_stickingout * _pp_p1.Y + (1 - _c_stickingout) * _pp_p4.Y);
                                    PointF[] _pppp = new PointF[] { _pp_p1, _pp_p2, _pp_p3, _pp_p4 };
                                    Pen _parameterpen;
                                    if (_parameter._Selected)
                                    {
                                        _parameterpen = new Pen(Brushes.Cyan, 10 * _z);
                                        _parameterpen.DashStyle = DashStyle.Solid;
                                        _g.DrawLines(_parameterpen, _pppp);
                                    }
                                    _parameterpen = new Pen(Brushes.Black, 3 * _z);
                                    _parameterpen.DashStyle = DashStyle.Dot;
                                    _g.DrawLines(_parameterpen, _pppp);
                                }
                            }
                        }
                    }
                }
                if (this._expanded)
                {
                    if (this._components != null)
                    {
                        foreach (Element_Component_ _component in this._components.Values)
                            _component._Draw(_g, _p0, _q0, _z);
                    }
                }
                if (this._endpoints != null)
                {
                    int _e_index = 0;
                    int _e_count = _endpoints.Count;
                    float _lowest_wire = float.NegativeInfinity;
                    foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                    {
                        _endpoint._Draw(_g, _p0, _q0, _z);
                        if (this._expanded && (this._category == Category_.Composite_) && (_endpoint._Backend != null))
                        {
                            PointF _t0 = new PointF(_endpoint._PP.X, (_endpoint._PP.Y + _endpoint._PQ.Y) / 2);
                            PointF _tn = new PointF(_endpoint._Backend._PQ.X, (_endpoint._Backend._PP.Y + _endpoint._Backend._PQ.Y) / 2);
                            PointF[] _tt;
                            PointF _t1 = new PointF(_t0.X - (_e_index + 1) * _c_wirespacing * _z, _t0.Y);
                            PointF _t2 = new PointF(_t1.X, _tn.Y);
                            _tt = new PointF[] { _t0, _t1, _t2, _tn };

                            GraphicsPath _gp2 = new GraphicsPath();
                            _gp2.AddLines(_tt);
                            if (_endpoint._Highlighted || _endpoint._Backend._Highlighted)
                                _g.DrawPath(new Pen(Brushes.Cyan, _c_hihglightedlinewidth * _z), _gp2);
                            _g.DrawPath(new Pen(Brushes.Black, 3 * _z), _gp2);
                        }
                        _e_index++;
                    }
                }
                if (this._connections != null)
                {
                    if (this._expanded)
                    {
                        int _c_index = 0;
                        int _c_count = this._connections.Count;
                        foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_ _cc in this._connections)
                        {
                            PointF _t0 = new PointF(_cc._E1._PQ.X, (_cc._E1._PP.Y + _cc._E1._PQ.Y) / 2);
                            PointF _t1 = new PointF(
                                this._pp.X + (this._o_p.X + _c_marginwidth + _c_internalmargin + this._connections_start_x + _c_index * _c_wirespacing) * _z,
                                _t0.Y);
                            PointF _t3 = new PointF(_cc._E2._PQ.X, (_cc._E2._PP.Y + _cc._E2._PQ.Y) / 2);
                            PointF _t2 = new PointF(_t1.X, _t3.Y);
                            GraphicsPath _gp2 = new GraphicsPath();
                            _gp2.AddLines(new PointF[] { _t0, _t1, _t2, _t3 });
                            if (_cc._Highlighted)
                                _g.DrawPath(new Pen(Brushes.Cyan, _c_hihglightedlinewidth * _z), _gp2);
                            _g.DrawPath(new Pen(Brushes.Black, 3 * _z), _gp2);
                            _c_index++;
                        }
                    }
                }
                if (this._from != null)
                {
                    this._from._Draw(_g, _p0, _q0, _z);
                    if (this._expanded)
                    {
                        Element_Object_ _from_o = this._from._Object;
                        if (_from_o != null)
                        {
                            _from_o._Draw(_g, _p0, _q0, _z);
                            Element_Reference_ _from_ref = _from_o._Reference;
                            PointF _pp_p1 = new PointF(_from._PP.X, (_from._PP.Y + _from._PQ.Y) / 2);
                            PointF _pp_p4 = new PointF((_from_ref._PP.X + _from_ref._PQ.X) / 2, _from_ref._PP.Y);
                            PointF _pp_p2 = new PointF((1 - _c_stickingout) * _pp_p1.X + _c_stickingout * _pp_p4.X, _pp_p1.Y);
                            PointF _pp_p3 = new PointF(_pp_p4.X, _c_stickingout * _pp_p1.Y + (1 - _c_stickingout) * _pp_p4.Y);
                            PointF[] _pppp = new PointF[] { _pp_p1, _pp_p2, _pp_p3, _pp_p4 };
                            Pen _frompen;
                            if (_from._Selected)
                            {
                                _frompen = new Pen(Brushes.Cyan, 10 * _z);
                                _frompen.DashStyle = DashStyle.Solid;
                                _g.DrawLines(_frompen, _pppp);
                            }
                            _frompen = new Pen(Brushes.Black, 3 * _z);
                            _frompen.DashStyle = DashStyle.Dot;
                            _g.DrawLines(_frompen, _pppp);
                            string _from_ss = "from";
                            Font _from_ss_font = new Font(FontFamily.GenericSansSerif, _c_fontsize * _z, FontStyle.Regular);
                            SizeF _from_ss_size = _g.MeasureString(_from_ss, _from_ss_font);
                            _g.DrawString(_from_ss, _from_ss_font, Brushes.Black,
                                _pp_p1.X - _from_ss_size.Width, _pp_p1.Y - _from_ss_size.Height);
                        }
                    }
                }
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not render object.", _exc);
            }
        }

        #endregion

        #region _Click

        public override Element_ _Click(PointF _m)
        {
            if ((_m.X >= this._p.X) && (_m.X <= this._q.X) && (_m.Y >= this._p.Y) && (_m.Y <= this._q.Y))
            {
                Element_ _element;
                if (this._from != null)
                {
                    _element = this._from._Click(_m);
                    if (_element != null)
                        return _element;
                    if (this._expanded && (this._from._Object != null))
                    {
                        _element = this._from._Object._Click(_m);
                        if (_element != null)
                            return _element;
                    }
                }
                if ((this._environment_2 != null) && (this._environment_2._Parameters != null))
                {
                    foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._environment_2._Parameters.Values)
                    {
                        if ((_parameter._ParameterClass == QS.Fx.Reflection.ParameterClass.Value) && _parameter._ValueClass._Correct &&
                            QS._qss_x_.Reflection_.Library._IsAnObjectReference(_parameter._ValueClass._Reflected_ValueClass))
                        {
                            _element = _parameter._Click(_m);
                            if (_element != null)
                                return _element;
                            if (this._expanded && (_parameter._Value != null))
                            {
                                Element_Object_ _o = (Element_Object_) _parameter._Value;
                                _element = _o._Click(_m);
                                if (_element != null)
                                    return _element;
                            }
                        }
                    }
                }
                if (this._endpoints != null)
                {
                    foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                    {
                        _element = _endpoint._Click(_m);
                        if (_element != null)
                            return _element;
                    }
                }
                if (this._expanded && (this._components != null))
                {
                    foreach (Element_Component_ _component in this._components.Values)
                    {
                        _element = _component._Click(_m);
                        if (_element != null)
                            return _element;
                    }
                }
                if ((_m.X >= (this._p.X + this._o_p.X)) && (_m.X <= (this._p.X + this._o_q.X)) &&
                    (_m.Y >= (this._p.Y + this._o_p.Y)) && (_m.Y <= (this._p.Y + this._o_q.Y)))
                {
                    return this;
                }
            }
            return null;
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
                bool _allow_parameters = true;
                bool _use_from = false;
                bool _use_binding = false;
                bool _use_components = false;
                bool _allow_connections = false;
                switch (this._category)
                {
                    case Category_.Predefined_:
                        {
                        }
                        break;
                    case Category_.Repository_:
                        {
                            _allow_parameters = false;
                            _use_from = true;
                        }
                        break;
                    case Category_.Composite_:
                        {
                            _use_components = true;
                            _allow_connections = true;
                        }
                        break;
                    case Category_.Parameter_:
                        {
                            _allow_parameters = false;
                            _use_binding = true;
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if ((this._environment_1 != null) && (this._environment_1._Parameters != null))
                {
                    if (_allow_parameters)
                    {
                        foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._environment_1._Parameters.Values)
                        {
                            _parameter._Validate();
                            if (!_parameter._Correct)
                            {
                                this._correct = false;
                                if (_ss == null)
                                    _ss = new StringBuilder();
                                _ss.AppendLine("Error in parameter \"" + _parameter._ID + "\".");
                            }
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("This object should not define any parameters.");
                    }
                }
                if ((this._environment_2 != null) && (this._environment_2._Parameters != null))
                {
                    if (_allow_parameters)
                    {
                        foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._environment_2._Parameters.Values)
                        {
                            _parameter._Validate();
                            if (!_parameter._Correct)
                            {
                                this._correct = false;
                                if (_ss == null)
                                    _ss = new StringBuilder();
                                _ss.AppendLine("Error in parameter \"" + _parameter._ID + "\".");
                            }
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("This object should not define any parameters.");
                    }
                }
                switch (this._category)
                {
                    case Category_.Predefined_:
                        {
                        }
                        break;
                    case Category_.Repository_:
                        {
                            _allow_parameters = false;
                            _use_from = true;
                        }
                        break;
                    case Category_.Composite_:
                        {
                            _use_components = true;
                            _allow_connections = true;
                        }
                        break;
                    case Category_.Parameter_:
                        {
                            _allow_parameters = false;
                            _use_binding = true;
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                if (this._objectclass != null)
                {
                    this._objectclass._Validate();
                    if (this._objectclass._Correct)
                    {
                        _EndpointsOn();
                    }
                    else
                    {
                        _EndpointsOff();
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Error in the object class definition.");
                    }
                }
                else
                {
                    _EndpointsOff();
                    this._correct = false;
                    if (_ss == null)
                        _ss = new StringBuilder();
                    _ss.AppendLine("Missing object class specification.");
                }
                if (this._from != null)
                {
                    if (_use_from)
                    {
                        this._from._Validate();
                        if (!this._from._Correct)
                        {
                            this._correct = false;
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine("Error in the \"from\" clause.");
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Cannot define a \"from\" clause in this type of objects.");
                    }
                }
                else
                {
                    if (_use_from)
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Missing \"from\" clause.");
                    }
                }
                if (this._binding != null)
                {
                    if (_use_binding)
                    {
                        this._binding._Validate();
                        if (!this._binding._Correct)
                        {
                            this._correct = false;
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine("Error in the underlying parameter \"" + this._binding._ID + "\".");
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("This type of object cannot be bound to a parameter.");
                    }
                }
                else
                {
                    if (_use_binding)
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Missing binding to the underlying parameter.");
                    }
                }
                if (this._components != null)
                {
                    if (_use_components)
                    {
                        foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Component_ _component in this._components.Values)
                        {
                            _component._Validate();
                            if (!_component._Correct)
                            {
                                this._correct = false;
                                if (_ss == null)
                                    _ss = new StringBuilder();
                                _ss.AppendLine("Error in component \"" + _component._ID + "\".");
                            }
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Objects of this type cannot define components.");
                    }
                }
                else
                {
                    if (_use_components)
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("No components have been defined.");
                    }
                }
                if (this._endpoints != null)
                {
                    foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_ _endpoint in this._endpoints.Values)
                    {
                        _endpoint._Validate();
                        if (!_endpoint._Correct)
                        {
                            this._correct = false;
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine("Error in endpoint \"" + _endpoint._ID + "\".");
                        }
                    }
                }
                if (this._connections != null)
                {
                    if (_allow_connections)
                    {
                        foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Connection_ _connection in this._connections)
                        {
                            _connection._Validate();
                            if (!_connection._Correct)
                            {
                                this._correct = false;
                                if (_ss == null)
                                    _ss = new StringBuilder();
                                _ss.AppendLine("Error in the connection between endpoints \"" +
                                    _connection._E1._ID + "\" and \"" + _connection._E2._ID + "\".");
                            }
                        }
                    }
                    else
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Objects of this type cannot define connections.");
                    }
                }
                if (_ss != null)
                    this._error = _ss.ToString();
                this._reference._Validate();
            }
        }

        #endregion

        #region _DropOk

        public override bool _DropOk(QS._qss_x_.ObjectDesigner_.Elements_.Category_ _category)
        {
            lock (this)
            {
                if (!this._automatic && (_category == QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_))
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            {
                                switch (this._category)
                                {
                                    case Category_.Composite_:
                                    case Category_.Repository_:
                                        return true;

                                    case Category_.Parameter_:
                                    case Category_.Predefined_:
                                        return false;

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                            break;

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
                if (!this._automatic && (_category == QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_))
                {
                    switch (_category)
                    {
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_:
                            {
                                switch (this._category)
                                {
                                    case Category_.Composite_:
                                    case Category_.Repository_:
                                        {
                                            this._EndpointsOff();
                                            this._objectclass = ((Element_ObjectClass_)_element);
                                            this._objectclass._Validate();
                                            if (this._objectclass._Correct)
                                                this._EndpointsOn();
                                        }
                                        break;

                                    default:
                                        throw new NotImplementedException();
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _EndpointsOn

        private void _EndpointsOn()
        {
            if (this._endpoints == null)
            {
                this._endpoints = this._objectclass._EndpointsOf(this);
            }
        }

        #endregion

        #region _EndpointsOff

        private void _EndpointsOff()
        {
            if (this._endpoints != null)
            {
                foreach (Element_Endpoint_ _endpoint in this._endpoints.Values)
                {
                    if (_endpoint._Connection != null)
                    {
                        // should disconnect somehow........
                    }
                    // should disconnect from backend........
                }
                this._endpoints = null;
            }
        }

        #endregion

        #region _RoundedRectangle

        private GraphicsPath _RoundedRectangle(float _x1, float _y1, float _sx, float _sy, float _dd)
        {
            GraphicsPath _gp = new GraphicsPath();
            RectangleF _a = new RectangleF(_x1, _y1, _dd, _dd);
            _gp.AddArc(_a, 180, 90);
            _a.X = _x1 + _sx - _dd;
            _gp.AddArc(_a, 270, 90);
            _a.Y = _y1 + _sy - _dd;
            _gp.AddArc(_a, 0, 90);
            _a.X = _x1;
            _gp.AddArc(_a, 90, 90);
            _gp.CloseFigure();
            return _gp;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
        public Element_Object_(Element_Object_ _original)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Accessors

        public Element_From_ _From
        {
            get { return _from; }
            set { _from = value; }
        }

        public IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_> _Parameters
        {
            get { return _parameters; }
        }

        public IDictionary<string, QS._qss_x_.ObjectDesigner_.Elements_.Element_Endpoint_> _Endpoints
        {
            get { return _endpoints; }
        }

        #endregion

        #region _InitializeOrReinitializeAfterChange

        public override void _InitializeOrReinitializeAfterChange()
        {
            switch (this._category)
            {
                #region Repository Object

                case Category_.Repository_:
                    {
                        this._endpoints = new Dictionary<string, Elements_.Element_Endpoint_>();
                        foreach (Elements_.Element_Endpoint_ _endpoint in _objectclass._Endpoints.Values)
                            _endpoints.Add(_endpoint._ID, new Elements_.Element_Endpoint_(_object, _endpoint._ID,
                                new Elements_.Element_EndpointClass_(_endpoint._EndpointClass)));
                    }
                    break;

                #endregion

                #region Predefined Object

                case Category_.Predefined_:
                    {
                    }
                    break;

                #endregion

                #region Parameter Object

                case Category_.Parameter_:
                    {
                    }
                    break;

                #endregion

                #region Composite Object

                case Category_.Composite_:
                    {
                    }
                    break;

                #endregion

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion
*/ 
    }
}
