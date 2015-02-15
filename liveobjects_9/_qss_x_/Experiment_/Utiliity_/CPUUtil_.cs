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
    public class CPUUtil_ : PerformanceCounters_
    {
        public CPUUtil_(QS.Fx.Object.IContext _mycontext):base(_mycontext)
        {
            _proc  = this.Add("Processor", "% Processor Time", "_Total");
            _global_gc =this.Add(".NET CLR Memory", "% Time in GC", "_Global_",false,true);
            _local_gc = this.Add(".NET CLR Memory", "% Time in GC", "liveobjects",false,true);
            this._mycontext = _mycontext;

        }
        
        
        PerformanceCounter_ _proc, _local_gc, _global_gc;
        private QS.Fx.Object.IContext _mycontext;
        
        public void PrintAvg()
        {
            if (_mycontext != null)
            {
                _mycontext.Platform.Logger.Log(_proc.Name + ": " + _proc.Average.ToString());
                _mycontext.Platform.Logger.Log(_local_gc.Name + ": " + _local_gc.DuplicateAvg.ToString());
                _mycontext.Platform.Logger.Log(_global_gc.Name + ": " + _global_gc.DuplicateAvg.ToString());
            }
        }

        public void CopyStats(double _duration)
        {

            StringBuilder _sb = new StringBuilder();
            _sb.Append(_duration + ",");
            _sb.Append(_proc.Average + ",");
            _sb.Append(_global_gc.Average + ",");
            _sb.Append(_local_gc.Average);



            ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
           
        }

    }
}
