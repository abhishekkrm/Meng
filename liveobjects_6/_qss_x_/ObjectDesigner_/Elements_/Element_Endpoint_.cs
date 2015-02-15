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
    public sealed class Element_Endpoint_ : Element_Port_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Endpoint_(Element_Object_ _object, string _id, Element_EndpointClass_ _endpointclass, bool _automatic)
            : base(Element_Port_.Category_.Endpoint_, _automatic)
        {
            this._object = _object;
            this._id = _id;
            this._endpointclass = _endpointclass;
        }

        #endregion

        #region Fields

        private Element_Object_ _object;
        private string _id;
        private Element_EndpointClass_ _endpointclass;
        private Element_Endpoint_ _frontend, _backend;
        private Element_Connection_ _connection;
//        private Element_Endpoint_ _original;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        
        #region _ID

        public string _ID
        {
            get { return _id; }
        }

        #endregion

        #region _Frontend

        public Element_Endpoint_ _Frontend
        {
            get { return _frontend; }
        }

        #endregion

        #region _Backend

        public Element_Endpoint_ _Backend
        {
            get { return _backend; }
        }

        #endregion

        #region _Connection

        public Element_Connection_ _Connection
        {
            get { return _connection; }
        }

        #endregion

        #region _Object

        public Element_Object_ _Object
        {
            get { return _object; }
        }

        #endregion

        #region _EndpointClass

        public Element_EndpointClass_ _EndpointClass
        {
            get { return _endpointclass; }
            set { _endpointclass = value; }
        }

        #endregion

        #region Serialize

        public QS.Fx.Reflection.Xml.CompositeObject.Endpoint _Serialize()
        {
            throw new NotImplementedException();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            this.Text = "Endpoint \"" + _id + "\"";
            this.Nodes.Clear();
            if (this._endpointclass != null)
            {
                this.Nodes.Add(this._endpointclass);
                this._endpointclass._Rebuild();
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
                if (this._connection != null)
                {
                    _highlighted.Add(this._connection);
                    if (!ReferenceEquals(this._connection._E1, this))
                        _highlighted.Add(this._connection._E1);
                    if (!ReferenceEquals(this._connection._E2, this))
                        _highlighted.Add(this._connection._E2);
                }
                if (this._frontend != null)
                    _highlighted.Add(this._frontend);
                if (this._backend != null)
                    _highlighted.Add(this._backend);
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
                StringBuilder _ss = null;
                if (this._id == null)
                {
                    this._correct = false;
                    if (_ss == null)
                        _ss = new StringBuilder();
                    _ss.AppendLine("Missing endpoint identifier.");
                }
                if (this._endpointclass != null)
                {
                    this._endpointclass._Validate();
                    if (!this._endpointclass._Correct)
                    {
                        this._correct = false;
                        if (_ss == null)
                            _ss = new StringBuilder();
                        _ss.AppendLine("Error in the endpoint class definition.");
                    }
                }
                else
                {
                    this._correct = false;
                    if (_ss == null)
                        _ss = new StringBuilder();
                    _ss.AppendLine("Missing endpoint class specification.");
                }
                if (this._backend != null)
                {
                    // TODO..................................... should check if forwarded correctly
                }
                if (_ss != null)
                    this._error = _ss.ToString();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
        public Element_Endpoint_(Element_Endpoint_ _original) : base(Element_Port_.Category_.Endpoint_)
        {
            this._cloned = true;
            this._original = _original;
            this._id = _original._id;
            this._object = _original._object;
            this._endpointclass = new Element_EndpointClass_(_original._endpointclass);
        }

        #endregion
*/ 
    }
}
