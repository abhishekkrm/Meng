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
    public class GCUtil_ : PerformanceCounters_
    {
        public GCUtil_(QS.Fx.Object.IContext _mycontext)
            : this(_mycontext, -1)
        {
            
        }

        public GCUtil_(QS.Fx.Object.IContext _mycontext, int _num_replicas)
            : base(_mycontext)
        {
            this._mycontext = _mycontext;


            if (_num_replicas == -1)
            {
                _num_replicas = LiveobjectInstances(".NET CLR Memory").Count -1;
            }

            _gen_0 = new PerformanceCounter_[_num_replicas + 1];
            _gen_1 = new PerformanceCounter_[_num_replicas + 1];
            _gen_2 = new PerformanceCounter_[_num_replicas + 1];
            _time_in_gc = new PerformanceCounter_[_num_replicas + 1];
            for (int i = 0; i < _num_replicas + 1; i++)
            {
                string _num = string.Empty;
                if (i > 0)
                {
                    _num = "#" + i;
                }
                _gen_0[i] = this.Add(".NET CLR Memory", "# Gen 0 Collections", "liveobjects" + _num);
                _gen_1[i] = this.Add(".NET CLR Memory", "# Gen 1 Collections", "liveobjects" + _num);
                _gen_2[i] = this.Add(".NET CLR Memory", "# Gen 2 Collections", "liveobjects" + _num);
                _time_in_gc[i] = this.Add(".NET CLR Memory", "% Time in GC", "liveobjects" + _num,false,true);
            }
        }

        [QS.Fx.Base.Inspectable]
        PerformanceCounter_[] _gen_0, _gen_1, _gen_2, _time_in_gc;
        QS.Fx.Object.IContext _mycontext;

        public void PrintAvg()
        {
            if (_mycontext != null)
            {
                _mycontext.Platform.Logger.Log(_gen_0[0].Name + ": " + _gen_0[0].Max.ToString());
                _mycontext.Platform.Logger.Log(_gen_1[0].Name + ": " + _gen_1[0].Max.ToString());
                _mycontext.Platform.Logger.Log(_gen_2[0].Name + ": " + _gen_2[0].Max.ToString());
                _mycontext.Platform.Logger.Log(_time_in_gc[0].Name + ": " + _time_in_gc[0].DuplicateAvg.ToString());
                int _num_rep = _gen_0.Length;
                if (_num_rep > 1)
                {
                    double _sum = 0;
                    for (int i = 1; i < _num_rep; i++)
                    {
                        _sum += _gen_0[i].Max;
                    }
                    _sum /= _num_rep - 1;
                    _mycontext.Platform.Logger.Log("Remote Replica Gen 0 Collections: "+ _sum.ToString());
                    _sum = 0;
                    for (int i = 1; i < _num_rep; i++)
                    {
                        _sum += _gen_1[i].Max;
                    }
                    _sum /= _num_rep - 1;
                    _mycontext.Platform.Logger.Log("Remote Replica Gen 1 Collections: " + _sum.ToString());
                    _sum = 0;
                    for (int i = 1; i < _num_rep; i++)
                    {
                        _sum += _gen_2[i].Max;
                    }
                    _sum /= _num_rep - 1;
                    _mycontext.Platform.Logger.Log("Remote Replica Gen 2 Collections: " + _sum.ToString());
                    _sum = 0;
                    for (int i = 1; i < _num_rep; i++)
                    {
                        _sum += _time_in_gc[i].DuplicateAvg;
                    }
                    _sum /= _num_rep - 1;
                    _mycontext.Platform.Logger.Log("Remote Replica Time in GC: " + _sum.ToString());
                }
            }
        }
    }
}
