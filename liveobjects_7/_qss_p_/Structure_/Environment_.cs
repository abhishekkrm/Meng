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
    public sealed class Environment_ : QS.Fx.Inspection.Inspectable, IEnvironment_
    {
        #region Constructor

        public Environment_(IEnvironment_ _environment)
        {
            this._environment = _environment;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IEnvironment_ _environment;
        [QS.Fx.Base.Inspectable]
        private IDictionary<string, IDeclaration_> _declarations = new Dictionary<string, IDeclaration_>();

        #endregion

        #region IEnvironment_ Members

        IEnvironment_ IEnvironment_._Environment
        {
            get { return this._environment; }
        }

        void IEnvironment_._Add(IDeclaration_ _declaration)
        {
            if (!this._declarations.ContainsKey(_declaration._Identifier))
                this._declarations.Add(_declaration._Identifier, _declaration);
            else
                throw new Exception("Identifier \"" + _declaration._Identifier + "\" has already been defined.");
        }

        IDeclaration_ IEnvironment_._Get(string _identifier)
        {
            IDeclaration_ _declaration;
            if (this._declarations.TryGetValue(_identifier, out _declaration))
                return _declaration;
            else
            {
                if (this._environment != null)
                    return this._environment._Get(_identifier);
                else
                    throw new Exception("Identifier \"" + _identifier + "\" has not been defined yet.");
            }
        }

        IDeclaration_ IEnvironment_._Get(string _identifier, Declaration_.Type_ _type)
        {
            IDeclaration_ _declaration = ((IEnvironment_)this)._Get(_identifier);
            if (!_declaration._Type.Equals(_type))
                throw new Exception("Declared element \"" + _identifier + "\" is not the type of an element expected in this context.");
            return _declaration;
        }

        DeclarationClass IEnvironment_._Get<DeclarationClass>(string _identifier)
        {
            IDeclaration_ _declaration = ((IEnvironment_)this)._Get(_identifier);
            return (DeclarationClass)_declaration;
        }

        DeclarationClass IEnvironment_._Get<DeclarationClass>(string _identifier, Declaration_.Type_ _type)
        {
            IDeclaration_ _declaration = ((IEnvironment_) this)._Get(_identifier, _type);
            return (DeclarationClass) _declaration;
        }

        bool IEnvironment_._Get(string _identifier, out IDeclaration_ _declaration)
        {
            if (this._declarations.TryGetValue(_identifier, out _declaration))
                return true;
            else
            {
                if (this._environment != null)
                    return this._environment._Get(_identifier, out _declaration);
                else
                    return false;
            }
        }

        bool IEnvironment_._Get(string _identifier, Declaration_.Type_ _type, out IDeclaration_ _declaration)
        {
            return ((IEnvironment_)this)._Get(_identifier, out _declaration) && (_declaration._Type.Equals(_type));
        }

        bool IEnvironment_._Get<DeclarationClass>(string _identifier, out DeclarationClass _declaration)
        {
            IDeclaration_ __declaration;
            if (((IEnvironment_)this)._Get(_identifier, out __declaration))
            {
                _declaration = (DeclarationClass)__declaration;
                return true;
            }
            else
            {
                _declaration = default(DeclarationClass);
                return false;
            }
        }

        bool IEnvironment_._Get<DeclarationClass>(string _identifier, Declaration_.Type_ _type, out DeclarationClass _declaration)
        {
            IDeclaration_ __declaration;
            if (((IEnvironment_)this)._Get(_identifier, _type, out __declaration))
            {
                _declaration = (DeclarationClass)__declaration;
                return true;
            }
            else
            {
                _declaration = default(DeclarationClass);
                return false;
            }
        }

        #endregion
    }
}
