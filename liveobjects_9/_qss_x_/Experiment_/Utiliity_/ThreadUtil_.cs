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
    public class ThreadUtil_ : PerformanceCounters_
    {

        public ThreadUtil_(QS.Fx.Object.IContext _mycontext)
            : this(_mycontext, 0, false)
        {

        }

        private const double _interval_in_seconds = .5;

        public ThreadUtil_(QS.Fx.Object.IContext _mycontext, int num_replicas, bool _wait_reasons)
            : base(_mycontext, _interval_in_seconds)
        {
            IList<string>[] _procs = new IList<string>[num_replicas + 1];
            for (int i = 0; i < _procs.Length; i++)
            {
                _procs[i] = new List<string>();
            }

            PerformanceCounterCategory[] _a = PerformanceCounterCategory.GetCategories();
            for (int i = 0; i < _a.Length; i++)
            {
                if (_a[i].CategoryName == "Thread")
                {
                    string[] _b = _a[i].GetInstanceNames();
                    foreach (string b in _b)
                    {
                        if (b.Contains("liveobjects"))
                        {
                            string[] _id = b.Split('#');
                            if (_id.Length != 2)
                            {
                                _procs[0].Add(b);
                            }
                            else
                            {
                                int _int_id = Convert.ToInt32(_id[1]);
                                if (_int_id < _procs.Length)
                                    _procs[_int_id].Add(b);
                            }
                        }
                    }
                }
            }
            if (_wait_reasons)
            {
                _wait = new List<PerformanceCounter_[]>(_procs.Length);
                for (int i = 0; i < _procs.Length; i++)
                {
                    _wait.Add(new PerformanceCounter_[_procs[i].Count]);
                    for (int j = 0; j < _procs[i].Count; j++)
                    {
                        _wait[i][j] = this.Add("Thread", "Thread Wait Reason", _procs[i][j]);
                    }
                }
            }
            _processes = new List<PerformanceCounter_[]>(_procs.Length);
            for (int i = 0; i < _procs.Length; i++)
            {
                _processes.Add(new PerformanceCounter_[_procs[i].Count]);
                for (int j = 0; j < _procs[i].Count; j++)
                {
                    _processes[i][j] = this.Add("Thread", "Thread State", _procs[i][j]);
                }
            }
        }

        [QS.Fx.Base.Inspectable]
        private IList<PerformanceCounter_[]> _processes;

        [QS.Fx.Base.Inspectable]
        private IList<PerformanceCounter_[]> _wait;

        private const int _num_thread_states = 7;
        private const int _inactive_thread_avg = 5;

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D[] Histogram
        {
            get
            {
                return _Histogram(false, _processes);
            }
        }

        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D[] Histogram_DropDeadThreads
        {
            get
            {
                return _Histogram(true, _processes);
            }
        }
        private QS._qss_c_.Statistics_.Samples2D[] _Histogram(bool _drop, IList<PerformanceCounter_[]> _pcs)
        {
            QS._qss_c_.Statistics_.Samples2D[] _ret = new QS._qss_c_.Statistics_.Samples2D[_pcs.Count];
            for (int i = 0; i < _pcs.Count; i++)
            {
                _ret[i] = new QS._qss_c_.Statistics_.Samples2D("Process " + i);
                int[] _count = new int[_num_thread_states];
                foreach (PerformanceCounter_ _c in _pcs[i])
                {
                    if (!(_c.Average == _inactive_thread_avg && _drop))
                    {
                        foreach (QS._core_e_.Data.XY _xy in _c.Samples.Samples)
                        {
                            _count[(int)_xy.y - 1]++;
                        }
                    }
                }
                for (int j = 0; j < _num_thread_states; j++)
                {
                    _ret[i].Add(j + 1, _count[j]);
                }
            }
            return _ret;
        }

        //public void CopyStats(double _duration)
        //{

        //    StringBuilder _sb = new StringBuilder();
        //    _sb.Append(_duration + ",");
        //    _sb.Append(_proc.Average + ",");
        //    _sb.Append(_global_gc.Average + ",");
        //    _sb.Append(_local_gc.Average);



        //    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));

        //}

    }
}
