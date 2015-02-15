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

namespace QS.Fx.Object.ConstraintClasses
{
    [QS.Fx.Reflection.ObjectConstraintClass(QS.Fx.Reflection.ObjectConstraintClasses.Secure)]
    public sealed class Secure : QS.Fx.Reflection.IObjectConstraint<Secure>
    {
        #region Constructor

        public Secure()
        {
        }

        #endregion

        #region Fields

        private int _level;

        #endregion

        #region IObjectConstraint<Some> Members

        void QS.Fx.Reflection.IObjectConstraint.Initialize(string _spec, QS.Fx.Reflection.IObjectClass _class)
        {
            this._level = Int32.Parse(_spec);
        }

        bool QS.Fx.Reflection.IObjectConstraint<Secure>.Weaker(Secure _other)
        {
            return this._level <= _other._level;
        }

        #endregion

        #region Overridden System.ToString

        public override string ToString()
        {
            return _level.ToString();
        }

        #endregion
    }
}
