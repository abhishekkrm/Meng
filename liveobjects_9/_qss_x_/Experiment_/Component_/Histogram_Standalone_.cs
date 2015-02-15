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
//#define PROFILE_LOCKWAITING
//#define LOCKWAITING_SUM
//#define PROFILE_COREUTIL
#define CPUUTIL
//#define GCUTIL
//#define LOCKINGUTIL

using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("CEA20C9BC33D47c697F1018BC2048631", "Histogram_Standalone_")]
    class Histogram_Standalone_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {

        #region Constructor

        public Histogram_Standalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("# of frames", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_of_frames,
            [QS.Fx.Reflection.Parameter("repeat",QS.Fx.Reflection.ParameterClass.Value)]
            int _repeat,
            [QS.Fx.Reflection.Parameter("cache",QS.Fx.Reflection.ParameterClass.Value)]
            bool _cache,
            [QS.Fx.Reflection.Parameter("# threads", QS.Fx.Reflection.ParameterClass.Value)] 
            int _num_threads)
            
        {
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._num_threads = _num_threads;
#if LOCKINGUTIL
            this._lockingutil = new QS._qss_x_.Experiment_.Utility_.LockingUtil_(_mycontext);
#endif
#if GCUTIL
            this._gcutil = new QS._qss_x_.Experiment_.Utility_.GCUtil_(_mycontext); 
#endif
#if CPUUTIL
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
#endif
#if COREUTIL
            this._coreutil = new QS._qss_x_.Experiment_.Utility_.CoreUtil_(_mycontext, 8); 
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
#if PROFILE_LOCKWAITING
            this._lockwaiting_times = new QS._qss_c_.Statistics_.Samples2D();
#if LOCKWAITING_SUM
            this._lockwaiting_sums = new QS._qss_e_.Data_.FastStatistics_(this._num_threads);
#endif
#endif

            this._thread_times = new QS._qss_e_.Data_.FastStatistics2D_(this._num_threads);

            Thread[] _t = new Thread[_num_threads];

            for (int i = 0; i < _num_threads; i++)
            {
                _t[i] = new Thread(new ParameterizedThreadStart(this._Work));
            }

           Random _random = new Random();

            this._clock = _mycontext.Platform.Clock;
            
            string _cache_path = @"C:\Users\Public\histogramcache.dat";
            
            int _batch_size = (int)Math.Ceiling(((double)(1024*1024 *(Int64)_num_of_frames) / _num_threads));
            int _num_batches = _num_threads;
            bool _generate_batches = false;
            if (_cache)
            {
                if (!File.Exists(_cache_path))
                {
                    _generate_batches = true;
                }
            }
            else
            {
                _generate_batches = true;
            }

            byte[][] _batches;
            
            if (_generate_batches)
            {
                 _batches= new byte[_num_batches][];
                for (int i = 0; i < _num_batches; i++)
                {
                    int _len;
                    if (i == _num_batches - 1)
                    {
                        _len = (1024 * 1024 * _num_of_frames) - (i * _batch_size);

                    }
                    else
                    {
                        _len = _batch_size;

                    }
                    _batches[i] = new byte[_len];
                    _random.NextBytes(_batches[i]);

                }
                if (_cache)
                {
                    // save batches
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    MemoryStream _ms = new MemoryStream();
                    _bf.Serialize(_ms, _batches);
                    using (FileStream _fs = new FileStream(_cache_path, FileMode.Create))
                    {
                        _fs.Write(_ms.ToArray(), 0, (int)_ms.Length);
                    }
                    
                }
            }
            else
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter _bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                using(FileStream _fs = new FileStream(_cache_path,FileMode.Open)) {
                    _batches = (byte[][])_bf.Deserialize(_fs);
                }
            }
            //_repeat /= _num_threads;
            if (_repeat <= 0)
                _repeat = 1;



            #region Start Threads

            for (int i = 0; i < _num_threads; i++)
            {
                _t[i].Start(new object[] { _batches[i], _repeat });
            }

            #endregion


        }

        #endregion



        #region Fields

        [QS.Fx.Base.Inspectable]
        private int _init = 0;
        [QS.Fx.Base.Inspectable]
        private int _id = -1;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _started = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private double _begin, _end;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
#if PROFILE_COREUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CoreUtil_ _coreutil; 
#endif
#if CPUUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
#endif
#if GCUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.GCUtil_ _gcutil;
#endif
#if LOCKINGUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.LockingUtil_ _lockingutil;
#endif
        [QS.Fx.Base.Inspectable]
        private int[] _histogram = new int[256];
        [QS.Fx.Base.Inspectable]
        private int _done = 0;
#if PROFILE_LOCKWAITING
        [QS.Fx.Base.Inspectable]
        private int _lw_unit = 1000000000; // nanoseconds
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _lockwaiting_times;
        //[QS.Fx.Base.Inspectable]
        //private bool _is_merged_lwtimes = false;
        //private QS._qss_c_.Statistics_.Samples2D _merged_lwtimes;
        //[QS.Fx.Base.Inspectable]
        //public QS._qss_c_.Statistics_.Samples2D _Merged_LockWaiting_times
        //{
        //    get
        //    {
        //        if (!_is_merged_lwtimes)
        //        {
        //            QS._qss_x_.Experiment_.Utiliity_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utiliity_.Sample2D_Merger_();
        //            _merged_lwtimes = _m._Merge_Samples(_lockwaiting_times);
        //            _is_merged_lwtimes = true;
        //        }
        //        return _merged_lwtimes;
        //    }

        //}
#if LOCKWAITING_SUM
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics_ _lockwaiting_sums;
#endif
#endif
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics2D_ _thread_times;


        #endregion

        
        void _Work(object _o)
        {

            object[] o = (object[])_o;
            byte[] _bitmap = (byte[])o[0];
            int _repeat = (int)o[1];
            int _thread_id = Interlocked.Increment(ref _id);




            #region Wait for all threads to start before working

            if (Interlocked.Increment(ref this._init) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if PROFILE_COREUTIL
                _coreutil.Start(); 
#endif
#if CPUUTIL
                this._cpuutil.Start();
#endif
#if LOCKINGUTIL
                this._lockingutil.Start();
#endif
#if GCUTIL
                this._gcutil.Start(); 
#endif
                this._begin = this._clock.Time;
                _started.Set();
            }
            else
            {
                _started.WaitOne();
            }

            #endregion

            double _begin_thread = this._clock.Time;


#if PROFILE_1
            double _timestamp = double.NaN;
            if (this._clock != null)
                _timestamp = this._clock.Time;
#endif
            for (int j = 0; j < _repeat; j++)
            {
                lock (this._histogram)
                {
                    for (int _i = 0; _i < _bitmap.Length; _i++)
                        this._histogram[(int)_bitmap[_i]]++;
                }
            }
            double _end_thread = this._clock.Time;

            #region Report Per Thread profiling
#if PROFILE_LOCKWAITING
#if LOCKWAITING_SUM
            this._mycontext.Platform.Logger.Log("Lockwaiting Sum: " + _sum_lockwait);
            this._lockwaiting_sums.Log(_sum_lockwait);
#endif
#endif
            this._thread_times.Add(_begin_thread, _end_thread);
            this._mycontext.Platform.Logger.Log("Thread Length: " + (_end_thread - _begin_thread).ToString());

#if PROFILE_LOCKWAITING
            //QS._core_e_.Data.XYSeries _xys = new QS._core_e_.Data.XYSeries(this._lockwaiting_times[_thread_id].Samples);
            //QS._core_e_.Data.XY[] _AddUpY = ((QS._core_e_.Data.XYSeries)_xys.AddUp_Y).Data;
            
            //this._mycontext.Platform.Logger.Log("Time spent waiting for locks: " + _AddUpY[_AddUpY.Length-1].y);
#endif

            #endregion

            #region Report Whole Experiment Profiling (last thread to complete)

            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {
                this._end = _mycontext.Platform.Clock.Time;
                double _duration = this._end - this._begin;
                this._mycontext.Platform.Logger.Log("Duration: " + (this._end - this._begin));
                StringBuilder _s = new StringBuilder();
                for (int _i = 0; _i < _histogram.Length; _i++)
                {
                    _s.Append(_histogram[_i].ToString());
                    _s.Append(" ");
                }

                MessageBox.Show(_s.ToString());

#if PROFILE_COREUTIL
                this._coreutil.Stop();
                this._coreutil.PrintAvg(); 
#endif
#if CPUUTIL
                this._cpuutil.Stop();
                this._cpuutil.PrintAvg();
                this._cpuutil.CopyStats(_duration);
#endif
#if GCUTIL
                this._gcutil.Stop();
                this._gcutil.PrintAvg();
                //this._gcutil.CopyStats(_duration);
#endif
#if LOCKINGUTIL
                this._lockingutil.Stop();
                this._lockingutil.PrintAvg();
                this._lockingutil.CopyStats(_duration);
#endif


#if PROFILE_1
            double _totalcpuusage = 0;
            foreach (QS._core_e_.Data.XY _sample in this._totalcpuusage_samples.Samples)
                _totalcpuusage += _sample.y;
            _totalcpuusage = _totalcpuusage / ((double)_totalcpuusage_samples.Samples.Length);
            this._mycontext.Platform.Logger.Log("Cpu Usage : " + _totalcpuusage.ToString());
#endif

            }

            #endregion

        }


    }
}

