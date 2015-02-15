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

namespace QS._qss_x_.Experiment_.Value_
{
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.ValueClass_02)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class ValueClass_02_
    {
        #region Constructor

        public ValueClass_02_(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public ValueClass_02_(string s)
        {
            string[] ss = s.Split(',');
            this.x = Convert.ToDouble(ss[0].TrimStart('(', ' '));
            this.y = Convert.ToDouble(ss[1].TrimEnd(' ', ')'));
        }

        public ValueClass_02_()
        {
        }

        #endregion

        #region Fields

        private double x;
        private double y;

        #endregion

        #region Accessors

        public double X
        {
            get { return this.x; }
            set { this.x = value; }
        }

        public double Y
        {
            get { return this.y; }
            set { this.y = value; }
        }

        #endregion

        #region Overridden from System.Object

        public override string ToString()
        {
            return "(" + this.x.ToString() + ", " + this.y.ToString() + ")";
        }

        #endregion
    }
}
