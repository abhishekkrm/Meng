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

#define PROFILE_CPU_UTIL
#define PROFILE_DATASET_SIZE
#define PROFILE_LOG_TO_SERVICE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.IO;


namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("8A8FA861A83648B183EC66A00C738EA2")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    
    public sealed class ReverseIndexClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public ReverseIndexClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
            string _folder_path,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch_size,
            [QS.Fx.Reflection.Parameter("number of files to analyze (~300MB / 10,000 files)", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_files,
            [QS.Fx.Reflection.Parameter("ratio of links to return", QS.Fx.Reflection.ParameterClass.Value)]
            double _num_links,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReverseIndex_> _workreference)
        {
            this._mycontext = _mycontext;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IReverseIndex_,
                    QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);

            string[] _html_files = Directory.GetFiles(_folder_path);

            List<string[]> _batches = new List<string[]>();
            List<int> _starts = new List<int>();
            string[] _batch = new string[_batch_size];
            int _batch_index = 0;
            int _count = 0;

            foreach (string _html_file in _html_files)
            {
                if (_count++ >= _num_files)
                {
                    break;
                }
                if (_batch_index == _batch_size)
                {
                    _batches.Add(_batch);
                    _starts.Add(_count - _batch_size -1);
                    _batch_index = 0;
                    _batch = new string[_batch_size];
                }
                _batch[_batch_index++] = _html_file;
            }
            if (_batch_index > 0)
            {
                string[] _tmp = new string[_batch_index];
                Array.Copy(_batch, _tmp, _batch_index);
                _batches.Add(_tmp);
                _starts.Add(_count - _tmp.Length-1);
                
            }
#if PROFILE_DATASET_SIZE
            _input_size = 0;
            foreach (string[] _b in _batches)
            {
                foreach (string _file in _b)
                {
                    FileInfo _fi = new FileInfo(_file);
                    _input_size += _fi.Length;
                    
                }
            }
#endif
            this._workendpoint.Interface._Set_Size(_num_links);
            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if PROFILE_CPU_UTIL
            this._totalcpuusage_counter.NextValue();
            this._proc_gc_counter.NextValue();
            this._gc_counter.NextValue();
            this._mycontext.Platform.Scheduler.Schedule(
                new QS.Fx.Base.Event(
                    new QS.Fx.Base.ContextCallback(
                        delegate(object _o)
                        {
                            this._mycontext.Platform.AlarmClock.Schedule(0.1, new QS.Fx.Clock.AlarmCallback(this._Statistics), null);
                        }), null));
#endif
            this._begun = true;
            this._starttime = this._mycontext.Platform.Clock.Time;

            for (int _i = 0; _i < _batches.Count; _i++)
                this._workendpoint.Interface._Work(_batches[_i],_starts[_i]);


            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReverseIndex_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IReverseIndex_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IReverseIndex_,
                QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;

        
        [QS.Fx.Base.Inspectable]
        private double _starttime;
        [QS.Fx.Base.Inspectable]
        private double _stoptime;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private bool _begun;
        [QS.Fx.Base.Inspectable]
        private bool _ended;
#if PROFILE_DATASET_SIZE
        [QS.Fx.Base.Inspectable]
        private Int64 _input_size;
#endif


#if PROFILE_CPU_UTIL
        private PerformanceCounter _totalcpuusage_counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private PerformanceCounter _gc_counter = new PerformanceCounter(".NET CLR Memory", "% Time in GC", "_Global_");
        private PerformanceCounter _proc_gc_counter = new PerformanceCounter(".NET CLR Memory", "% Time in GC", "liveobjects");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _totalcpuusage_samples = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _gc_samples = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _proc_gc_samples = new QS._qss_c_.Statistics_.Samples2D();
#endif

        #endregion

#if PROFILE_CPU_UTIL
        private void _Statistics(QS.Fx.Clock.IAlarm _alarmref)
        {
            if (!this._ended)
            {
                if (this._begun)
                {
                    double _t = this._mycontext.Platform.Clock.Time;
                    this._totalcpuusage_samples.Add(_t, (double)this._totalcpuusage_counter.NextValue());
                    this._proc_gc_samples.Add(_t, (double)this._proc_gc_counter.NextValue());
                    this._gc_samples.Add(_t, (double)this._gc_counter.NextValue());
                }
                _alarmref.Reschedule();
            }
        }
#endif

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReverseIndexClient_ Members

        private double GetAvg(QS._qss_c_.Statistics_.Samples2D _samples)
        {
            double _total = 0;
            foreach (QS._core_e_.Data.XY _sample in _samples.Samples)
                _total += _sample.y;
            _total = _total / ((double)_samples.Samples.Length);
            return _total;
        }

        private double[] GetY(QS._core_e_.Data.XY[] _xy)
        {
            double[] _y = new double[_xy.Length];
            for (int i = 0; i < _xy.Length; i++)
            {
                _y[i] = _xy[i].y;
            }
            return _y;
        }

        void QS._qss_x_.Experiment_.Interface_.IReverseIndexClient_._Done(IDictionary<string, IList<
#if !USE_LINK_ID
            string
#else
            int
#endif
            >> _links)
        {
            this._stoptime = _mycontext.Platform.Clock.Time;
            this._ended = true;
            this._duration = this._stoptime - this._starttime;
            this._mycontext.Platform.Logger.Log("Duration : " + this._duration.ToString());

#if PROFILE_CPU_UTIL
            
            this._mycontext.Platform.Logger.Log("Cpu Usage : " + GetAvg(this._totalcpuusage_samples).ToString());
            this._mycontext.Platform.Logger.Log("Total % Time in GC : " + GetAvg(this._gc_samples).ToString());
            this._mycontext.Platform.Logger.Log("Proc % Time in GC : " + GetAvg(this._proc_gc_samples).ToString());
#endif
#if PROFILE_LOG_TO_SERVICE
            QS._qss_x_.Experiment_.Utility_.Stat_Logger _logger = new QS._qss_x_.Experiment_.Utility_.Stat_Logger();
            IList<double[]> _data = new List<double[]>();
            _data.Add(GetY(this._totalcpuusage_samples.Samples));
            _data.Add(GetY(this._gc_samples.Samples));
            _data.Add(GetY(this._proc_gc_samples.Samples));
            string[] _col_names = new string[] { "CPU Usage", "Total % Time in GC", "Proc % Time in GC" };
            _logger.Log("ReverseIndex "+DateTime.Now.ToString(), _col_names,_data,"");

#endif
            StringBuilder _s = new StringBuilder();
            _s.AppendLine("Results:\n");
#if !USE_LINK_ID
            KeyValuePair<string, IList<string>>[] _link_lists = new KeyValuePair<string, IList<string>>[_links.Count];
#else
            KeyValuePair<string, IList<int>>[] _link_lists = new KeyValuePair<string, IList<int>>[_links.Count];
#endif
            _links.CopyTo(_link_lists, 0);

            #if !USE_LINK_ID
            Array.Sort<KeyValuePair<string, IList<string>>>(
                _link_lists,
                new Comparison<KeyValuePair<string, IList<string>>>(
                    delegate(KeyValuePair<string, IList<string>> _x, KeyValuePair<string, IList<string>> _y)
                    {
                        return _x.Value.Count - _y.Value.Count;
                    }));
#else
            Array.Sort<KeyValuePair<string, IList<int>>>(
                _link_lists,
                new Comparison<KeyValuePair<string, IList<int>>>(
                    delegate(KeyValuePair<string, IList<int>> _x, KeyValuePair<string, IList<int>> _y)
                    {
                        return _x.Value.Count - _y.Value.Count;
                    }));
#endif

            Array.Reverse(_link_lists);

            int _count = 0;
            
            foreach (KeyValuePair<string, IList<
#if !USE_LINK_ID
                string
#else
                int
#endif
                >> _lists in _link_lists)
            {
                if (_count++ > 15)
                {
                    break;
                }
                _s.Append(_lists.Key);
                _s.Append(" ");
                _s.AppendLine(_lists.Value.Count.ToString());
            }
            this._mycontext.Platform.Logger.Log(_s.ToString());


        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
