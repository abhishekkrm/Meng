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
using System.Windows.Forms;

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_ValueObject_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_ValueObject_(object _o)
            : base(false)
        {
            this._o = _o;
        }

        #endregion

        #region Fields

        // private Element_Parameter_ _parameter;
        private object _o;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Object

        public object _Object
        {
            get { return this._o; }
        }

        #endregion

        #region _Serialize

        public object _Serialize()
        {
            return this._o;
        }

        #endregion

//        #region _Parameter
//
//        public Element_Parameter_ _Parameter
//        {
//            get { return this._parameter; }
//        }
//
//        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Nodes.Clear();
            StringBuilder _ss = new StringBuilder();
            if (_o != null)
            {
                _ss.Append("\"");
                _ss.Append(_o.ToString());
                _ss.Append("\"");
            }
            else
                _ss.Append("null");
            this.Text = _ss.ToString();
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
            }
        }

        #endregion

        #region _CreateComment

        public override string _CreateComment()
        {
            if (this._o != null)
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append("type = \"");
                _ss.Append(_o.GetType().FullName);
                _ss.Append("\"");
                _ss.AppendLine();
                _ss.Append("data = \"");
                _ss.Append(_o.ToString());
                _ss.Append("\"");
                _ss.AppendLine();
                return _ss.ToString();
            }
            else
                return null;

/*
            if (_owner != null)
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append("type = ");
                Type _t = _owner._UnderlyingType;
                if (_t != null)
                {
                    _ss.Append("\"");
                    _ss.Append(_t.FullName);
                    _ss.Append("\"");
                }
                else
                    _ss.Append("null");
                _ss.AppendLine();
                _ss.Append("data = ");
                object _o = _owner._Object;
                if (_o != null)
                {
                    _ss.Append("\"");
                    _ss.Append(_o.ToString());
                    _ss.Append("\"");
                }
                else
                    _ss.Append("null");
                _ss.AppendLine();
                return _ss.ToString();
            }
            else
                return null;
*/
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
        #region _RenameOk

        public override bool _RenameOk()
        {
            return !this._cloned;
        }

        #endregion

        #region _Rename

        public override void _Rename(string _s)
        {
            lock (this)
            {
                if (!this._cloned)
                {
                    object _o = null;
                    Type _type = _owner._UnderlyingType;
                    if (_type.Equals(typeof(string)))
                        _o = _s;
                    else
                    {
                        System.Reflection.ConstructorInfo _constructorinfo = _type.GetConstructor(new Type[] { typeof(string) });
                        if (_constructorinfo != null)
                            _o = _constructorinfo.Invoke(new object[] { _s });
                        else
                        {
                            System.Reflection.MethodInfo _methodinfo = _type.GetMethod("Parse",
                                 System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, null, new Type[] { typeof(string) }, null);
                            if ((_methodinfo != null) && (_methodinfo.ReturnType.Equals(_type)))
                                _o = _methodinfo.Invoke(null, new object[] { _s });
                            else
                                throw new Exception("Sorry, I don't know how to make a value of this type out of a System.String.");
                        }
                    }
                    if (_o != null)
                    {
                        this._owner._Object = _o;
                    }
                }
            }
        }

        #endregion
*/
    }
}
