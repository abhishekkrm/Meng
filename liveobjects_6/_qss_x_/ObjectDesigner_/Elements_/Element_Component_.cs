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
    public sealed class Element_Component_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Component_(string _id, Element_Object_ _object) : base(false)
        {
            this._id = _id;
            this._object = _object;
        }

        #endregion

        #region Fields

        private string _id;
        private Element_Object_ _object;

        #endregion

        #region Constants

        private const float _c_fontsize = 10;
        private const float _c_textheight = 20;
        private const float _c_textwidth = 100;
        private const float _c_textspacing = 2;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ID

        public string _ID
        {
            get { return _id; }
        }

        #endregion

        #region _Object

        public Element_Object_ _Object
        {
            get { return _object; }
        }

        #endregion

        #region Serialize

        public QS.Fx.Reflection.Xml.CompositeObject.Component _Serialize()
        {
            QS.Fx.Reflection.Xml.Object _xml_object = null;
            if (this._object != null)
                _xml_object = this._object._Serialize();
            return new QS.Fx.Reflection.Xml.CompositeObject.Component(this._id, _xml_object);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Text = "Component \"" + _id + "\"";
            this.Nodes.Clear();
            if (this._object != null)
            {
                this.Nodes.Add(this._object);
                this._object._Rebuild();
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
                if (this._object != null)
                {
                    this._object._Validate();
                    if (!this._object._Correct)
                    {
                        this._correct = false;
                        this._error = "Error in the object definition.";
                    }
                }
                else
                {
                    this._correct = false;
                    this._error = "Missing object definition.";
                }
            }
        }

        #endregion

        #region _Recalculate_1

        public override void _Recalculate_1()
        {
            this._object._Recalculate_1();
            this._s = new System.Drawing.PointF(
                Math.Max(_c_textwidth, this._object._S.X),
                _c_textheight + _c_textspacing + this._object._S.Y);
        }

        #endregion

        #region _Recalculate_2

        public override void _Recalculate_2()
        {
            this._q.X = this._p.X + this._s.X;
            this._q.Y = this._p.Y + this._s.Y;
            this._object._P = new System.Drawing.PointF(this._p.X, this._p.Y + _c_textheight + _c_textspacing);
            this._object._Recalculate_2();
        }

        #endregion

        #region _Draw

        public override void _Draw(Graphics _g, PointF _p0, PointF _q0, float _z)
        {
            this._CalculateCoordinates(_g, _p0, _z);
            if (this._selected)
                _g.DrawRectangle(new Pen(Brushes.Yellow, 5 * _z), _pp.X, _pp.Y, _ps.X, _ps.Y);
            _g.DrawString("Component \"" + this._id + "\"",
                new Font(FontFamily.GenericSansSerif, _c_fontsize * +_z, FontStyle.Italic), Brushes.DarkGray, _pp.X, _pp.Y);
            _object._Draw(_g, _p0, _q0, _z);
        }

        #endregion

        #region _Click

        public override Element_ _Click(PointF _m)
        {
            if ((_m.X >= this._p.X) && (_m.X <= this._q.X) && (_m.Y >= this._p.Y) && (_m.Y <= this._q.Y))
            {
                Element_ _element = this._object._Click(_m);
                if (_element != null)
                    return _element;
                else
                    return this;
            }
            else
                return null;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
