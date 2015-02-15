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

namespace QS._qss_p_.Structure_
{
    public sealed class Declaration_Reference_ : Declaration_, IDeclaration_, IDeclaration_Reference_, IElement_, IUnresolved_,
        IValue_, IValue_Reference_, 
        IValueClass_, IValueClass_Reference_, 
        IInterfaceClass_, IInterfaceClass_Reference_,
        IOperatorClass_, IOperatorClass_Reference_,
        IObjectClass_, IObjectClass_Reference_
    {
        #region Constructor

        public Declaration_Reference_(IEnvironment_ _context, string _identifier, Declaration_.Type_ _type, IEnumerable<IDeclaration_> _parameters) 
            : base(null, null, _type, null)
        {
            this._context = _context;
            this._identifier = _identifier;
            if (_parameters != null)
                this._parameters = (new List<IDeclaration_>(_parameters)).ToArray();
        }

        public Declaration_Reference_(IEnvironment_ _context, string _identifier, IEnumerable<IDeclaration_> _parameters)
            : this(_context, _identifier, Declaration_.Type_._Unresolved, _parameters)
        {
        }

        public Declaration_Reference_(IDeclaration_ _base, IEnumerable<IDeclaration_> _parameters)
            : base(null, null, _base._Type, null)
        {
            this._base = _base;
            if (_parameters != null)
                this._parameters = (new List<IDeclaration_>(_parameters)).ToArray();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IEnvironment_ _context;
        [QS.Fx.Base.Inspectable]
        private string _identifier;
        [QS.Fx.Base.Inspectable]
        private IDeclaration_ _base;
        [QS.Fx.Base.Inspectable]
        private IDeclaration_[] _parameters;

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        this._base._Print(_printer);
                        Declaration_._PrintParameters(_printer, this._parameters);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region IDeclaration_Reference_ Members

        IEnvironment_ IDeclaration_Reference_._Context
        {
            get { return this._context; }
        }

        string IDeclaration_Reference_._Identifier
        {
            get { return this._identifier; }
        }

        IDeclaration_ IDeclaration_Reference_._Base
        {
            get { return this._base; }
        }

        IDeclaration_[] IDeclaration_Reference_._Parameters
        {
            get { return this._parameters; }
        }

        #endregion

        #region IUnresolved_ Members

        void IUnresolved_._Resolve()
        {
            lock (this)
            {
                if (this._base == null)
                    this._base = this._context._Get(this._identifier);
                if (this._base is IUnresolved_)
                    ((IUnresolved_) this._base)._Resolve();
                this._Type(this._base._Type);                
            }
        }

        #endregion

        #region IValueClass_ Members

        ValueClass_.Type_ IValueClass_._Type
        {
            get { return ValueClass_.Type_._Reference; }
        }

        #endregion

        #region IValueClass_Reference_ Members

        IValueClass_ IValueClass_Reference_._Base
        {
            get { return (IValueClass_) this._base; }
        }

        #endregion

        #region IInterfaceClass_ Members

        InterfaceClass_.Type_ IInterfaceClass_._Type
        {
            get { return InterfaceClass_.Type_._Reference; }
        }

        #endregion

        #region IInterfaceClass_Reference_ Members

        IInterfaceClass_ IInterfaceClass_Reference_._Base
        {
            get { return (IInterfaceClass_) this._base; }
        }

        #endregion

        #region IOperatorClass_Reference_ Members

        IOperatorClass_ IOperatorClass_Reference_._Base
        {
            get { return (IOperatorClass_) this._base; }
        }

        #endregion

        #region IValue_Reference_ Members

        IValue_ IValue_Reference_._Base
        {
            get { return (IValue_) this._base; }
        }

        #endregion

        #region IValue_ Members ******************************************

        Value_.Type_ IValue_._Type
        {
            get { return Value_.Type_._Reference; }
        }

        IValueClass_ IValue_._ValueClass
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IOperatorClass_ Members ******************************************

        OperatorClass_.Type_ IOperatorClass_._Type
        {
            get { return OperatorClass_.Type_._Reference; }
        }

        IValue_[] IOperatorClass_._Arguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IValue_[] IOperatorClass_._Results
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region IObjectClass_ Members ******************************************************

        ObjectClass_.Type_ IObjectClass_._Type
        {
            get { return ObjectClass_.Type_._Reference; }
        }

        IEndpoint_[] IObjectClass_._Endpoints
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IObjectClass_Reference_ Members

        IObjectClass_ IObjectClass_Reference_._Base
        {
            get { return (IObjectClass_) this._base; }
        }

        #endregion
    }
}
