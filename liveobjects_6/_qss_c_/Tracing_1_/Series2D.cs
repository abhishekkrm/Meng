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

namespace QS._qss_c_.Tracing_1_
{
/*
    public class Series2D : ITrace
    {
        public Series2D(string name, string description, 
            string x_name, string x_units, string x_description, string y_name, string y_units, string y_description) 
            : this(name, description, new Axis(x_name, x_units, x_description), new Axis(y_name, y_units, y_description))
        {
        }

        public Series2D(string name, string description, Axis x, Axis y)
        {
            this.name = name;
            this.description = description;            
            axes[0] = x;
            axes[1] = y;
        }

        private string name, description;            
        private Axis[] axes = new Axis[2];
        private int nsamples;

        private void Flush()
        {
        }

        public void Add(double x, double y)
        {
            // TODO: Implement................



            Flush();
        }

        #region ITrace Members

        string ITrace.Name
        {
            get { return name; }
        }

        string ITrace.Description
        {
            get { return description; }
        }

        int ITrace.Size
        {
            get { return nsamples; }
        }

        Axis[] ITrace.Axes
        {
            get { return axes; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Flush();
        }

        #endregion
    }
*/ 
}
