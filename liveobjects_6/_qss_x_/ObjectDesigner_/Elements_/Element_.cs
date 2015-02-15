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
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public abstract class Element_ : TreeNode
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        protected Element_(bool _automatic) : base()
        {
            this._automatic = _automatic;
            this._AdjustTreeNodeAppearance();
        }

        #endregion

        #region Fields

        protected bool _correct, _automatic, _highlighted, _selected;
        protected string _error;
        protected PointF _p, _q, _s, _pp, _pq, _ps;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Correct ***********

        public bool _Correct
        {
            get { return this._correct; }
/*
            set
            {
                lock (this)
                {
                    _correct = value;
                    _AdjustTreeNodeAppearance();
                }
            }
*/
        }

        #endregion

        #region _Error

        public string _Error
        {
            get { return this._error; }
        }

        #endregion

        #region _Highlighted

        public bool _Highlighted
        {
            get { return _highlighted; }
            set
            {
                lock (this)
                {
                    _highlighted = value;
                    _AdjustTreeNodeAppearance();
                }
            }
        }

        #endregion

        #region _Selected

        public bool _Selected
        {
            get { return _selected; }
            set
            {
                lock (this)
                {
                    _selected = value;
                    _AdjustTreeNodeAppearance();
                }
            }
        }

        #endregion

        #region _P, _Q, _S, _PP, _PQ, _PS

        public PointF _P
        {
            get { return _p; }
            set { _p = value; }
        }

        public PointF _Q
        {
            get { return _q; }
            set { _q = value; }
        }

        public PointF _S
        {
            get { return _s; }
            set { _s = value; }
        }

        public PointF _PP
        {
            get { return _pp; }
        }

        public PointF _PQ
        {
            get { return _pq; }
        }

        public PointF _PS
        {
            get { return _ps; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Click

        public virtual Element_ _Click(PointF _m)
        {
            return null;
        }

        #endregion

        #region _DropOk

        public virtual bool _DropOk(Elements_.Category_ _category)
        {
            return false;
        }

        #endregion

        #region _Drop

        public virtual void _Drop(Elements_.Category_ _category, Elements_.Element_ _element)
        {
        }

        #endregion

        #region _CreateComment

        public virtual string _CreateComment()
        {
            return null;
        }

        #endregion

        #region _Recalculate

        public virtual void _Recalculate_1()
        {
        }

        public virtual void _Recalculate_2()
        {
        }

        #endregion

        #region _Draw

        public virtual void _Draw(Graphics _g, PointF _p0, PointF _q0, float _z)
        {
        }

        #endregion

        #region _Highlight

        public virtual IEnumerable<Elements_.Element_> _Highlight()
        {
            return null;
        }

        #endregion

        #region _DoubleClick

        public virtual void _DoubleClick()
        {
        }

        #endregion

        #region _Rebuild

        public virtual void _Rebuild()
        {
        }

        #endregion

        #region _Validate

        public virtual void _Validate()
        {
        }

        #endregion

        #region _Menu

        public virtual IEnumerable<Element_Action_> _Menu()
        {
            return new Element_Action_[]
            {
                new Element_Action_(
                    "Recalculate", new QS.Fx.Base.ContextCallback(this._RecalculateCallback), null)
            };
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _AdjustTreeNodeAppearance

        protected virtual void _AdjustTreeNodeAppearance()
        {
            this.BackColor = _selected ? (_highlighted ? Color.GreenYellow : Color.Yellow) : (_highlighted ? Color.Cyan : Color.White);
            this.ForeColor = _automatic ? Color.Gray : (_correct ? Color.Black : Color.Red);
            bool _isobject = (this is Element_Object_) || (this is Element_ValueObject_);
            this.NodeFont = new Font(FontFamily.GenericSansSerif, 10, _isobject ? FontStyle.Bold : FontStyle.Regular);
            StringBuilder _ss = null;
            string _comment = this._CreateComment();
            if (_comment != null)
            {
                if (_ss == null)
                    _ss = new StringBuilder();
                _ss.AppendLine(_comment);
            }
            string _error = this._Error;
            if (_error != null)
            {
                if (_ss == null)
                    _ss = new StringBuilder();
                _ss.AppendLine("errors:");
                _ss.AppendLine(_error);
            }
            this.ToolTipText = (_ss != null) ? _ss.ToString() : string.Empty;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CalculateCoordinates

        protected void _CalculateCoordinates(Graphics _g, PointF _p0, float _z)
        {
            RectangleF _b = _g.VisibleClipBounds;
            this._pp = new PointF(_b.Left + (this._p.X - _p0.X) * _z, _b.Top + (this._p.Y - _p0.Y) * _z);
            this._pq = new PointF(_b.Left + (this._q.X - _p0.X) * _z, _b.Top + (this._q.Y - _p0.Y) * _z);
            this._ps = new PointF(this._s.X * _z, this._s.Y * _z);
        }

        #endregion

        #region _Overlapping

        protected bool _Overlapping(PointF _pc, PointF _qc)
        {
            return ((_pc.X <= this._q.X) && (_qc.X >= this._p.X) && (_pc.Y <= this._q.Y) && (_qc.Y >= this._p.Y));
        }

        #endregion

        #region _RefreshCallback

        private void _RecalculateCallback(object _context)
        {
            this._Validate();
            this.TreeView.BeginUpdate();
            this._Rebuild();
            this.TreeView.EndUpdate();
            this._Recalculate_1();
            this._Recalculate_2();
            this.TreeView.Refresh();
        }

        #endregion

        #region _GenerateTypeCode

        protected void _GenerateTypeCode(Type _type, StringBuilder _ss)
        {
            this._GenerateTypeCode(_type, _ss, 0);
        }

        private void _GenerateTypeCode(Type _type, StringBuilder _ss, int _level)
        {
            if (_type.IsNested)
                _GenerateTypeCode(_type.DeclaringType, _ss, _level);
            else
            {
                for (int _i = 0; _i < _level; _i++)
                    _ss.Append("    ");
                _ss.Append(_type.Namespace);
            }
            _ss.Append(".");
            string _name = _type.Name;
            if (_name.Contains("`"))
                _name = _name.Substring(0, _name.IndexOf('`'));
            _ss.Append(_name);
            if (_type.IsGenericType)
            {
                _ss.Append("<\n");
                bool _isfirst = true;
                foreach (Type _type2 in _type.GetGenericArguments())
                {
                    if (_isfirst)
                        _isfirst = false;
                    else
                        _ss.Append(",\n");
                    _GenerateTypeCode(_type2, _ss, _level + 1);
                }
                _ss.Append(">");
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
        #region Fields **********

        protected Element_ _owner;

        #endregion

        #region Accessors *****

        public Element_ _Owner
        {
            get { return this._owner; }
            set { this._owner = value; }
        }

        #endregion

        #region _RenameOk

        public virtual bool _RenameOk()
        {
            return false;
        }

        #endregion

        #region _Rename

        public virtual void _Rename(string _s)
        {
        }

        #endregion
*/ 
    }
}
