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

namespace QS.Fx.Reflection
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class LibraryAttribute : System.Attribute
    {
        public LibraryAttribute(string _id) : this(_id, string.Empty, string.Empty, null)
        {
        }

        public LibraryAttribute(string _id, string _name) : this(_id, _name, string.Empty, null)
        {
        }

        public LibraryAttribute(string _id, string _name, string _description) : this(_id, _name, _description, null)
        {
        }

        public LibraryAttribute(string _id, string _name, string _description, Type _component)
        {
            int p = _id.IndexOf('`');
            if (p >= 0 && p < _id.Length)
            {
                this._id = (p > 0) ? new QS.Fx.Base.ID(_id.Substring(0, p)) : QS.Fx.Base.ID._0;
                this._version = (p < (_id.Length - 1)) ? Convert.ToUInt64(_id.Substring(p + 1)) : 0;
            }
            else
            {
                this._id = new QS.Fx.Base.ID(_id);
                this._version = 0;
            }
            this._name = _name;
            this._description = _description;
            this._component = _component;
        }

        private QS.Fx.Base.ID _id;
        private string _name, _description;
        private ulong _version;
        private Type _component;

        public QS.Fx.Base.ID ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public ulong Version
        {
            get { return _version; }
            set { _version = value; }
        }

        public Type Component
        {
            get { return _component; }
            set { _component = value; }
        }
    }
}
