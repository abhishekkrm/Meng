/*
Copyright (c) 2008-2009 Chuck Sakoda. All rights reserved.
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
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace QS._qss_x_.Experiment_.Utility_
{
    public class CoreUtil_ : PerformanceCounters_
    {
        public CoreUtil_(QS.Fx.Object.IContext _mycontext, int _num_cores)
            : base(_mycontext)
        {
            _cores = new PerformanceCounter_[_num_cores];
            for (int i = 0; i < _num_cores; i++)
            {
                _cores[i] =this.Add("Processor", "% Processor Time", i.ToString(),true);
            }
            _total = this.Add("Processor", "% Processor Time", "_Total");
        }

        PerformanceCounter_[] _cores;
        PerformanceCounter_ _total;




        //public void CopyStats(double _duration)
        //{

        //    StringBuilder _sb = new StringBuilder();
        //    _sb.Append(_duration + ",");
        //    _sb.Append(_total.Average + ",");
        //    _sb.Append(_global_gc.Average + ",");
        //    _sb.Append(_local_gc.Average);



        //    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));

        //}

    }
}

