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

namespace QS._qss_x_.Language_.Structure_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Property : Variable
    {
        #region Constructors

        public Property(Protocol _protocol, ValueType _valuetype, string _name, Expression _initialvalue, 
            Placement _placement, PropertyAttributes _attributes, string _comment) 
            : base(_valuetype, _name, ((_attributes & PropertyAttributes.Const) == PropertyAttributes.Const))
        {
            this._protocol = _protocol;
            this._initialvalue = _initialvalue;
            this._placement = _placement;
            this._comment = _comment;
            this._attributes = _attributes;
        }

        #endregion

        #region Fields

        // [QS.Fx.Printing.Printable]
        private Protocol _protocol;
        [QS.Fx.Printing.Printable]
        private Expression _initialvalue;
        [QS.Fx.Printing.Printable]
        private Placement _placement;
        [QS.Fx.Printing.Printable]
        private string _comment;
        [QS.Fx.Printing.Printable]
        private IList<OnUpdate> _updatehandlers = new List<OnUpdate>();
        [QS.Fx.Printing.Printable]
        private PropertyAttributes _attributes;

        #endregion

        #region Overridden from Variable

        public override VariableCategory VariableCategory
        {
            get { return VariableCategory.Property; }
        }

        #endregion

        #region Interface

        public Protocol Protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        public Expression InitialValue
        {
            get { return _initialvalue; }
        }

        public Placement Placement
        {
            get { return _placement; }
            set { _placement = value; }
        }

        public PropertyAttributes Attributes
        {
            get { return _attributes; }
            set 
            { 
                _attributes = value;
                _isconstant = ((_attributes & PropertyAttributes.Const) == PropertyAttributes.Const);
            }
        }

        public string Comment
        {
            get { return _comment; }
        }

        public void AddHandler(OnUpdate _handler)
        {
            _updatehandlers.Add(_handler);
        }

        public IEnumerable<OnUpdate> Handlers
        {
            get { return _updatehandlers; }
        }

        #endregion
    }
}
