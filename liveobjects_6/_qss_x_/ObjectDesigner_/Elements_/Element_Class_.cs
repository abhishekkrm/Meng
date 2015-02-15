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

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public abstract class Element_Class_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        protected Element_Class_(
            string _id, QS.Fx.Attributes.IAttributes _attributes, Category_ _category, Element_Parameter_ _binding,
            Element_Environment_ _environment, bool _automatic)
            : base(_automatic)
        {
            this._id = _id;
            this._attributes = _attributes;
            this._category = _category;
            this._binding = _binding;
            this._environment = _environment;
        }

        protected Element_Class_(Element_Class_ _other) 
            : this(_other._id, _other._attributes, _other._category, _other._binding, new Element_Environment_(_other._environment), true)
        {
        }

        #endregion

        #region Fields

        protected string _id;
        protected QS.Fx.Attributes.IAttributes _attributes;
        protected Category_ _category;
        protected Element_Parameter_ _binding;
        protected Element_Environment_ _environment;

        #endregion

        #region Category_

        public enum Category_
        {
            Predefined_, Parameter_
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Recalculate_1

        public override void _Recalculate_1()
        {
        }

        #endregion

        #region _Recalculate_2

        public override void _Recalculate_2()
        {

        }

        #endregion

        #region _Validate

        public override void _Validate()
        {
            lock (this)
            {
                this._correct = true;
                this._error = null;
                switch (this._category)
                {
                    case Category_.Predefined_:
                        {
                            if (this._environment != null)
                            {
                                StringBuilder _ss = null;
                                this._environment._Validate();
                                if (!this._environment._Correct)
                                {
                                    this._correct = false;
                                    if (_ss == null)
                                        _ss = new StringBuilder();
                                    _ss.AppendLine("Error in one of the parameters.");
                                }
                                if (_ss != null)
                                    this._error = _ss.ToString();
                            }
                        }
                        break;

                    case Category_.Parameter_:
                        {
                            this._binding = null;
                            Element_Environment_ _context = this._environment._Context;
                            while (_context != null)
                            {
                                if (_context._Parameters.TryGetValue(this._id, out this._binding))
                                    break;
                                else
                                    _context = _context._Context;
                            }
                            if (this._binding == null)
                            {
                                this._correct = false;
                                this._error = "Parameter \"" + this._id + "\" is not defined in this context.";
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        #region _Rebuild

        public override void _Rebuild()
        {
            string _label = "Class";
            QS.Fx.Attributes.IAttribute _nameattribute = null;
            if (this._attributes != null)
            {
                if (!this._attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                    _nameattribute = null;
            }
            if ((_nameattribute != null) && (_nameattribute.Value != null))
                _label += " \"" + _nameattribute.Value + "\"";
            else
                if (this._id != null)
                    _label += " " + _id;
                else
                    _label += " (unnamed)";
            this.Text = _label;
            this.Nodes.Clear();
            if ((this._environment != null) && (this._environment._Parameters != null))
            {
                foreach (Element_Parameter_ _p in this._environment._Parameters.Values)
                {
                    this.Nodes.Add(_p);
                    _p._Rebuild();
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

        #region _Highlight

        public override IEnumerable<Element_> _Highlight()
        {
            lock (this)
            {
                List<Element_> _highlighted = new List<Element_>();
                if (this._binding != null)
                {
                    _highlighted.Add(this._binding);
                }
                return (_highlighted.Count > 0) ? _highlighted : null;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
