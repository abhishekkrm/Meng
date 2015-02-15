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

namespace QS._qss_x_.Simulations_
{
    public sealed class Task_ : ITask_
    {
        public Task_(string _name, double _mttb, double _mttf, double _mttr, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            this._name = _name;
            this._mttb = _mttb;
            this._mttf = _mttf;
            this._mttr = _mttr;
            this._object = _object;
        }

        public Task_(string _name, double _mttb, double _mttf, double _mttr, string _objectxml)
        {
            this._name = _name;
            this._mttb = _mttb;
            this._mttf = _mttf;
            this._mttr = _mttr;
            this._objectxml = _objectxml;
        }

        private string _name;
        private double _mttb, _mttf, _mttr;
        private string _objectxml;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;

        string ITask_._Name
        {
            get { return this._name; }
        }

        double ITask_._MTTB
        {
            get { return this._mttb; }
        }

        double ITask_._MTTF
        {
            get { return this._mttf; }
        }

        double ITask_._MTTR
        {
            get { return this._mttr; }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> ITask_._Object
        {
            get { return this._object; }
        }

        string ITask_._ObjectXml
        {
            get { return this._objectxml; }
        }
    }
}
