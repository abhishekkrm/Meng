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
    public sealed class Metadata_ : QS.Fx.Inspection.Inspectable, IMetadata_
    {
        #region Constructor

        public Metadata_(string _identifier, string _version, string _name, string _comment)
        {
            this._identifier = _identifier;
            this._version = _version;
            this._name = _name;
            this._comment = _comment;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private string _identifier;
        [QS.Fx.Base.Inspectable]
        private string _version;
        [QS.Fx.Base.Inspectable]
        private string _name;
        [QS.Fx.Base.Inspectable]
        private string _comment;

        #endregion

        #region IMetadata_ Members

        string IMetadata_._Identifier
        {
            get { return this._identifier; }
        }

        string IMetadata_._Version
        {
            get { return this._version; }
        }

        string IMetadata_._Name
        {
            get { return this._name; }
        }

        string IMetadata_._Comment
        {
            get { return this._comment; }
        }

        #endregion

        #region IElement_ Members

        void IElement_._Print(QS._qss_p_.Printing_.IPrinter_ _printer)
        {
            switch (_printer._Type)            
            {
                case QS._qss_p_.Printing_.Printer_.Type_._Source:
                    {
                        if (this._identifier != null)
                        {
                            _printer._Print("@");
                            _printer._Print(this._identifier);
                            if (this._version != null)
                            {
                                _printer._Print(",");
                                _printer._Print(this._version);
                            }
                            _printer._Print(" ");
                        }
                        _printer._Print(this._name);
                    }
                    break;

                case QS._qss_p_.Printing_.Printer_.Type_._Compiled:
                    {
                        _printer._Print("\"");
                        _printer._Print(this._identifier);
                        _printer._Print("`");
                        _printer._Print(this._version);
                        _printer._Print("\", \"");
                        _printer._Print(this._name);
                        _printer._Print("\"");
                        if (this._comment != null)
                        {
                            _printer._Print(", \"");
                            _printer._Print(this._comment);
                            _printer._Print("\"");
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _Declare

        public static void _Declare(QS._qss_p_.Printing_.IPrinter_ _printer, IMetadata_ _metadata)
        {
            if (_metadata != null)
            {
                if ((_metadata._Comment != null) && (_metadata._Comment.Length > 0))
                {
                    _printer._Print("/// <summary>\n/// ");
                    _printer._Print(_metadata._Comment);
                    _printer._Print("\n/// </summary>\n");
                }
            }
        }

        #endregion
    }
}
