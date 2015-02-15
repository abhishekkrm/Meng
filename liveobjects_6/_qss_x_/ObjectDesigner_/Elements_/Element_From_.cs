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
    public sealed class Element_From_ : Element_Port_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_From_(Element_Object_ _object) : base(Element_Port_.Category_.From_, false)
        {
            this._object = _object;
        }

        #endregion

        #region Fields

        protected Element_Object_ _object;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Object

        public Element_Object_ _Object
        {
            get { return _object; }
//            set { _object = value; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Text = "From";
            this.Nodes.Clear();
            if (this._object != null)
            {
                this.Nodes.Add(this._object);
                this._object._Rebuild();
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
                if (this._object != null)
                {
                    _highlighted.Add(this._object);
                    _highlighted.Add(this._object._Reference);
                }
                return (_highlighted.Count > 0) ? _highlighted : null;
            }
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
                    if (this._object._Correct)
                    {
                        QS.Fx.Reflection.IObjectClass _m_objectclass = this._object._ObjectClass._Reflected_ObjectClass;
                        if (!QS._qss_x_.Reflection_.Library._IsAnObjectRepository(_m_objectclass))
                        {
                            this._correct = false;
                            this._error = "The object that is supposed to serve as a source repository does not look like a repository.";
                        }
                    }
                    else
                    {
                        this._correct = false;
                        this._error = "Error in the definition of the repository object.";
                    }
                }
                else
                {
                    this._correct = false;
                    this._error = "Missing definition of the repository object.";
                }
            }
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
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                            return true;

                        default:
                            return false;
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
                        case QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_:
                            {
                                this._object = (Element_Object_) _element;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
