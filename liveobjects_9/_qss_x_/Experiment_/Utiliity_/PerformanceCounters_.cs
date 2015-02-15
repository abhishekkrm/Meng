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
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Experiment_.Utility_
{
    public class PerformanceCounters_ : QS.Fx.Inspection.Inspectable
    {
        public PerformanceCounters_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._counters = new List<PerformanceCounter_>();
            this._begun = false;
            this._ended = false;

        }

        public PerformanceCounters_(QS.Fx.Object.IContext _mycontext, double _interval)
            : this(_mycontext)
        {
            this._timer_interval_seconds = _interval;
        }


        public IList<string> LiveobjectInstances(string _category)
        {

            IList<string> _ret = new List<string>();
            

            PerformanceCounterCategory[] _a = PerformanceCounterCategory.GetCategories();

            for (int i = 0; i < _a.Length; i++)
            {
                if (_a[i].CategoryName == _category)
                {
                    string[] _b = _a[i].GetInstanceNames();
                    foreach (string b in _b)
                    {
                        if (b.Contains("liveobjects"))
                        {
                            _ret.Add(b);
                        }
                    }
                }
            }
            
            return _ret;
        }

        public void Start()
        {
            foreach (PerformanceCounter_ _pc in _counters)
            {
                _pc.Init();
            }
            this._begun = true;
            this._timer = new System.Timers.Timer(_timer_interval_seconds * 1000);
            this._timer.AutoReset = true;
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(this._Statistics);
            this._timer.Start();
            //this._mycontext.Platform.Scheduler.Schedule(
            //    new QS.Fx.Base.Event(
            //        new QS.Fx.Base.ContextCallback(
            //            delegate(object _o)
            //            {
            //                this._mycontext.Platform.AlarmClock.Schedule(0.1, new QS.Fx.Clock.AlarmCallback(this._Statistics), null);
            //            }), null));
        }

        [QS.Fx.Base.Inspectable]
        public bool Started
        {
            get
            {
                return this._begun;
            }
        }

        private double _timer_interval_seconds = 1;
        private int _sample_count = 0;
        public void PrintAvg()
        {
            if (_mycontext != null)
            {
                foreach (PerformanceCounter_ _pc in _counters)
                {
                    this._mycontext.Platform.Logger.Log(_pc.Name + ": " + _pc.Average.ToString());
                }
            }
        }

        public void Stop()
        {
            this._ended = true;
            this._timer.Stop();
        }

        private void _Statistics(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!this._ended)
            {
                if (this._begun)
                {
                    double _t;
                    if (this._mycontext == null)
                        _t = _sample_count * _timer_interval_seconds;
                    else
                        _t = this._mycontext.Platform.Clock.Time;
                    foreach (PerformanceCounter_ _pc in _counters)
                    {
                        _pc.Sample(_t);
                    }
                }
            }
        }

        private IList<PerformanceCounter_> Counters
        {
            get
            {

                return this._counters;
            }
        }

        public void Add(PerformanceCounters_ _pcs)
        {
            foreach (PerformanceCounter_ _pc in _pcs.Counters)
            {
                _counters.Add(_pc);
            }
        }

        public PerformanceCounter_ Add(string category, string counter, string instance)
        {
            PerformanceCounter_ _pc = new PerformanceCounter_(category, counter, instance);
            _counters.Add(_pc);
            return _pc;
        }

        public PerformanceCounter_ Add(string category, string counter, string instance, bool _sortable)
        {
            PerformanceCounter_ _pc = new PerformanceCounter_(category, counter, instance, _sortable, false);
            _counters.Add(_pc);
            return _pc;
        }
        public PerformanceCounter_ Add(string category, string counter, string instance, bool _sortable, bool _duplicate)
        {
            PerformanceCounter_ _pc = new PerformanceCounter_(category, counter, instance, _sortable, _duplicate);
            _counters.Add(_pc);
            return _pc;
        }
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D[] Statistics
        {
            get
            {
                QS._qss_c_.Statistics_.Samples2D[] _ret = new QS._qss_c_.Statistics_.Samples2D[_counters.Count];
                for (int i = 0; i < _counters.Count; i++)
                {
                    _ret[i] = _counters[i].Samples;
                }
                return _ret;
            }
        }

        [QS.Fx.Base.Inspectable]
        public string SortedStatisticsAvg
        {
            get
            {
                try
                {
                    double[][] _sorted;
                    double[] _averages;
                    IList<QS._qss_c_.Statistics_.Samples2D> _ret;
                    SortStats(out _sorted, out _averages, out _ret);
                    StringBuilder _sb = new StringBuilder();
                    //_sb.Append("Sample #,Time,");
                    //for (int i = 0; i < _ret.Count; i++)
                    //{
                    //    _sb.Append("Sorted" + i.ToString() + ",");
                    //}
                    //_sb.AppendLine();

                    //// paste averages
                    //_sb.Append("AVG,AVG,");
                    for (int i = 0; i < _averages.Length; i++)
                    {

                        _sb.Append(_averages[i] + ",");


                    }
                    _sb.AppendLine();

                    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
                    return "Copied stats to clipboard";
                }
                catch (Exception e)
                {
                    return "F-";
                }
            }
        }


        private bool SortStats(out double[][] _sorted, out double[] _averages, out IList<QS._qss_c_.Statistics_.Samples2D> _ret)
        {
          _ret = new List<QS._qss_c_.Statistics_.Samples2D>();
            for (int i = 0; i < _counters.Count; i++)
            {
                if (_counters[i].Sortable)
                {
                    _ret.Add(_counters[i].Samples);
                }
            }
            //QS._qss_c_.Statistics_.Samples2D[] _ret = new QS._qss_c_.Statistics_.Samples2D[__ret.Count];
            //for (int i = 0; i < _ret.Length; i++)
            //{
            //    _ret[i] = __ret[i];
            //}
            _sorted = new double[_ret[0].Samples.Length][];
            for (int i = 0; i < _ret[0].Samples.Length; i++)
            {
                _sorted[i] = new double[_ret.Count];

                for (int j = 0; j < _ret.Count; j++)
                {
                    _sorted[i][j] = _ret[j].Samples[i].y;
                }

                Array.Sort(_sorted[i]);
                Array.Reverse(_sorted[i]);
            }

           _averages = new double[_ret.Count];
            for (int i = 0; i < _averages.Length; i++)
            {
                double _sum = 0;
                for (int j = 0; j < _sorted.Length; j++)
                {
                    _sum += _sorted[j][i];
                }
                _sum /= _sorted.Length;
                _averages[i] = _sum;
            }
            return true;
        }

        [QS.Fx.Base.Inspectable]
        public string SortedStatistics
        {
            get
            {
                try
                {
                    double[][] _sorted;
                     double[] _averages;
                     IList<QS._qss_c_.Statistics_.Samples2D> _ret;
                     SortStats(out _sorted,out _averages, out _ret);
                                        StringBuilder _sb = new StringBuilder();
                    _sb.Append("Sample #,Time,");
                    for (int i = 0; i < _ret.Count; i++)
                    {
                        _sb.Append("Sorted" + i.ToString() + ",");
                    }
                    _sb.AppendLine();

                    // paste sorted stats

                    for (int i = 0; i < _sorted.Length; i++)
                    {
                        _sb.Append(i.ToString() + "," + _ret[0].Samples[i].x + ",");
                        for (int j = 0; j < _ret.Count; j++)
                        {
                            _sb.Append(_sorted[i][j] + ",");
                        }
                        _sb.AppendLine();
                    }

                    // paste averages
                    _sb.Append("AVG,AVG,");
                    for (int i = 0; i < _averages.Length; i++)
                    {
                    
                            _sb.Append(_averages[i] + ",");
                        
                        
                    }
                    _sb.AppendLine();

                    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
                    return "Copied stats to clipboard";
                }
                catch (Exception e)
                {
                    return "F-";
                }

            }
        }

        [QS.Fx.Base.Inspectable]
        public string CopyDuplicateAvgs
        {
            get
            {
                try
                {
                    IList<PerformanceCounter_> _samples = new List<PerformanceCounter_>();
                    for (int i = 0; i < _counters.Count; i++)
                    {
                        if (_counters[i].TrimDuplicates)
                        {
                            _samples.Add(_counters[i]);
                        }
                    }
                    StringBuilder _sb = new StringBuilder();
                    //_sb.Append("Sample #,Time,");
                    foreach (PerformanceCounter_ _pc in _samples)
                    {
                        _sb.Append(_pc.Name + ",");
                    }
                    _sb.AppendLine();

                    //int _c = _counters[0].Samples.Samples.Length;

                    foreach (PerformanceCounter_ _pc in _samples)
                    {
                        _sb.Append(_pc.DuplicateAvg + ",");

                    }
                    _sb.AppendLine();


                    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
                    return "Copied stats to clipboard";

                }
                catch (Exception e)
                {
                    return "F-";
                }
                    
            }
        }

        [QS.Fx.Base.Inspectable]
        public string CopyStatstoClipboard
        {
            get
            {
                try
                {
                    IList<QS._core_e_.Data.XY[]> _samples = new List<QS._core_e_.Data.XY[]>();
                    for (int i = 0; i < _counters.Count; i++)
                    {
                            _samples.Add(_counters[i].Samples.Samples);
                        
                    }
                    StringBuilder _sb = new StringBuilder();
                    _sb.Append("Sample #,Time,");
                    foreach(PerformanceCounter_ _pc in this._counters) {
                        _sb.Append(_pc.Name+ ",");
                    }
                    _sb.AppendLine();

                    int _c = _counters[0].Samples.Samples.Length;
                    
                    for (int i = 0; i < _c; i++)
                    {
                        _sb.Append(i.ToString() + "," + _samples[0][0].x + ",");
                        foreach (QS._core_e_.Data.XY[] _xy in _samples)
                        {
                            _sb.Append(_xy[i].y + ",");
                        }
                        _sb.AppendLine();
                    }

                    ClipboardThread_ _t = new ClipboardThread_(_sb.ToString().Replace(',', '\t'));
                    return "Copied stats to clipboard";
                }
                catch (Exception e)
                {
                    return "F-";
                }
            }
        }

        bool _ended;
        bool _begun;
        QS.Fx.Object.IContext _mycontext;
        IList<PerformanceCounter_> _counters;
        System.Timers.Timer _timer;

    }
}
