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

//#define PROFILE_1
//#define PROFILE_DATASET_SIZE
//#define PROFILE_SUBMIT
#define COREUTIL
//#define CPUUTIL
//#define THREADUTIL
//#define TIMELINE
//#define MEMALLOCUTIL
//#define REPLICA_GCUTIL
//#define ALL_UTIL
//#define PAIR

#if ALL_UTIL
#define MEMALLOCUTIL
#define COREUTIL
#define REPLICA_GCUTIL
#endif

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("C26AA56F9C334764A78348BE1A7FCFCE")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class WordCountClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IWordCountClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WordCountClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
            string _folder,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch_size,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWordCount_> _workreference)
        {
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IWordCount_,
                    QS._qss_x_.Experiment_.Interface_.IWordCountClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
#if REPLICA_GCUTIL
            this._replica_gcutil = new QS._qss_x_.Experiment_.Utility_.GCUtil_(_mycontext); 
#endif
#if MEMALLOCUTIL
            this._memalloc_util = new QS._qss_x_.Experiment_.Utility_.MemoryAllocUtil_(_mycontext); 
#endif
#if CPUUTIL
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext); 
#endif
#if THREADUTIL
            this._threadutil = new QS._qss_x_.Experiment_.Utility_.ThreadUtil_(_mycontext,0,true); 
#endif
#if COREUTIL
            this._coreutil = new QS._qss_x_.Experiment_.Utility_.CoreUtil_(_mycontext, 8); 
#endif
#if PROFILE_1
            this._totalcpuusage_counter.NextValue();
            this._mycontext.Platform.Scheduler.Schedule(
                new QS.Fx.Base.Event(
                    new QS.Fx.Base.ContextCallback(
                        delegate(object _o)
                        {
                            this._mycontext.Platform.AlarmClock.Schedule(0.1, new QS.Fx.Clock.AlarmCallback(this._Statistics), null);
                        }), null));
#endif
#if PROFILE_DATASET_SIZE
            _input_size = 0;
            int c = 0;
            foreach (string _file in Directory.GetFiles(_folder, "*.txt", SearchOption.AllDirectories))
            {
                if (c++ >= _count)
                    break;

                FileInfo _fi = new FileInfo(_file);
                _input_size += _fi.Length;


            }
#endif
#if ALL_UTIL
            this._allutil = new QS._qss_x_.Experiment_.Utility_.PerformanceCounters_(_mycontext);
#if COREUTIL
            this._allutil.Add(_coreutil);
#endif
#if REPLICA_GCUTIL
            this._allutil.Add(this._replica_gcutil);
#endif
#if MEMALLOCUTIL
            this._allutil.Add(this._memalloc_util);
#endif
#endif




            int _num_batches = (int)Math.Ceiling((double)_count / _batch_size);
            string[] _paths = Directory.GetFiles(_folder, "*.txt", SearchOption.AllDirectories);
            string[][] _batches = new string[_num_batches][];



            for (int i = 0; i < _num_batches; i++)
            {
                int _size = ((_batch_size * (i + 1)) < _count) ? _batch_size : (_count - (_batch_size * i));
                _batches[i] = new string[_size];
                for (int j = 0; j < _size; j++)
                {
                    _batches[i][j] = _paths[(_batch_size * i) + j];
                }
            }



            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
#if CPUUTIL
            this._cpuutil.Start();
#endif
#if THREADUTIL
            this._threadutil.Start();
#endif
            #if !ALL_UTIL
#if COREUTIL
            this._coreutil.Start(); 
#endif
#if REPLICA_GCUTIL
            this._replica_gcutil.Start(); 
#endif
#if MEMALLOCUTIL
            this._memalloc_util.Start(); 
#endif
#else
            this._allutil.Start();
#endif
            this._begun = true;
            this._starttime = _mycontext.Platform.Clock.Time;
#if TIMELINE
            this._begin_submit = this._clock.Time;
#endif
            for (int i = 0; i < _num_batches; i++)
            {

#if PROFILE_SUBMIT
                double _t1 = this._clock.Time;
#endif
                this._workendpoint.Interface._Work2(_batches[i]);
#if PROFILE_SUBMIT
                double _t2 = this._clock.Time;
                this._submit_times.Add(_t1, _t2 - _t1);
#endif
            }
#if TIMELINE
            this._end_submit = this._clock.Time;
#endif
            //while (_count > 0)
            //{
            //    foreach (string _path in Directory.GetFiles(_folder, "*.txt", SearchOption.AllDirectories))
            //    {
            //        if (_count-- > 0)
            //            this._workendpoint.Interface._Work(_path);
            //        else
            //            break;
            //    }
            //}

            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

#if TIMELINE
        [QS.Fx.Base.Inspectable]
        double _begin_submit;
        [QS.Fx.Base.Inspectable]
        double _end_submit;
#endif

#if ALL_UTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.PerformanceCounters_ _allutil;
#endif
#if PROFILE_SUBMIT
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _submit_times = new QS._qss_c_.Statistics_.Samples2D();
#endif
#if COREUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CoreUtil_ _coreutil; 
#endif
#if CPUUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil; 
#endif
#if THREADUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.ThreadUtil_ _threadutil; 
#endif
#if REPLICA_GCUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.GCUtil_ _replica_gcutil; 
#endif
#if MEMALLOCUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.MemoryAllocUtil_ _memalloc_util; 
#endif
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWordCount_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IWordCount_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWordCount_,
                QS._qss_x_.Experiment_.Interface_.IWordCountClient_> _workendpoint;
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
        [QS.Fx.Base.Inspectable]
        private Int64 _output_size;
#endif
#if PROFILE_1
        private PerformanceCounter _totalcpuusage_counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _totalcpuusage_samples = new QS._qss_c_.Statistics_.Samples2D();
#endif
#if TIMELINE
        [QS.Fx.Base.Inspectable]
        QS._qss_x_.Experiment_.Utility_.TimelineCopyPrep_ _prep; 
#endif

        #endregion

        #region _Statistics

#if PROFILE_1
        private void _Statistics(QS.Fx.Clock.IAlarm _alarmref)
        {
            if (!this._ended)
            {
                if (this._begun)
                    this._totalcpuusage_samples.Add(this._clock.Time, (double)this._totalcpuusage_counter.NextValue());
                _alarmref.Reschedule();
            }
        }
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWordCountClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IWordCountClient_._Done(

            IDictionary<string, int> 

 _result)
        {
            this._stoptime = _mycontext.Platform.Clock.Time;
            this._ended = true;
            this._duration = this._stoptime - this._starttime;
#if CPUUTIL
            this._cpuutil.Stop();
            this._cpuutil.PrintAvg();
            this._cpuutil.CopyStats(this._duration);
#endif
#if !ALL_UTIL
#if REPLICA_GCUTIL

            this._replica_gcutil.Stop();
            this._replica_gcutil.PrintAvg();
            //this._replica_gcutil.CopyStats(this._duration);

#endif
#if MEMALLOCUTIL
            
            this._memalloc_util.Stop();
            this._memalloc_util.PrintAvg();
            //this._replica_gcutil.CopyStats(this._duration);

#endif
#if COREUTIL

            this._coreutil.Stop();
            this._coreutil.PrintAvg(); 

#endif
#else
            this._allutil.Stop();
            this._allutil.PrintAvg();
#endif
#if THREADUTIL
            this._threadutil.Stop();
          //  this._coreutil.PrintAvg(); 
#endif
#if TIMELINE
           _prep = new QS._qss_x_.Experiment_.Utility_.TimelineCopyPrep_(this._starttime, this._stoptime);
            QS._qss_x_.Experiment_.Component_.WordCount_ _wc = (QS._qss_x_.Experiment_.Component_.WordCount_)this._workproxy;


            _prep.Add(this._begin_submit, this._end_submit - this._begin_submit);
            _prep.Add(this._begin_submit, _wc._import_times.Samples[_wc._import_times.Samples.Length - 1].x - this._begin_submit);
            _prep.Add(_wc._import_times);
            _prep.Copy();
#endif


            this._mycontext.Platform.Logger.Log("Duration : " + this._duration.ToString());
            StringBuilder _s = new StringBuilder();
            _s.AppendLine("Results:\n");
            KeyValuePair<string, int>[] _wordcounts = new KeyValuePair<string, int>[_result.Count];
            _result.CopyTo(_wordcounts, 0);
            Array.Sort<KeyValuePair<string, int>>(
                _wordcounts,
                new Comparison<KeyValuePair<string, int>>(
                    delegate(KeyValuePair<string, int> _x, KeyValuePair<string, int> _y)
                    {
                        return -_x.Value.CompareTo(_y.Value);
                    }));
            int _c = 10;
            int i = 0;
     
       foreach (KeyValuePair<string, int> _wordcount in _wordcounts)
            {
                if(i++ == _c) 
                    break;
                _s.Append(_wordcount.Key);
                _s.Append(" ");
                _s.AppendLine(_wordcount.Value.ToString());
            }
            this._mycontext.Platform.Logger.Log(_s.ToString());



#if PROFILE_1
            double _totalcpuusage = 0;
            foreach (QS._core_e_.Data.XY _sample in this._totalcpuusage_samples.Samples)
                _totalcpuusage += _sample.y;
            _totalcpuusage = _totalcpuusage / ((double)_totalcpuusage_samples.Samples.Length);
            this._mycontext.Platform.Logger.Log("Cpu Usage : " + _totalcpuusage.ToString());
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
