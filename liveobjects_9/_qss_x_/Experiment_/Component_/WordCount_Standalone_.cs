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
//#define CPUUTIL
//#define GCUTIL
//#define LOCKINGUTIL
//#define SORT
#define PAIR
#define COREUTIL

#if COREUTIL
#define PROFILE_COREUTIL
#endif
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
    [QS.Fx.Reflection.ComponentClass("7F9EE6451B22466b86F37E52D4D7FE76", "WordCount_Standalone_")]
    class WordCount_Standalone_ : QS.Fx.Inspection.Inspectable , QS.Fx.Object.Classes.IObject
    {

        #region Constructor

        public WordCount_Standalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
            string _folder,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
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

            #region Batch Paths




            int _batch_size = (int)Math.Ceiling((double)_count / _num_threads);

            string[][] _batches = new string[_num_threads][];

            string[] _paths = Directory.GetFiles(_folder, "*.txt", SearchOption.AllDirectories);

            if (_paths.Length < _count)
            {
                string[] _tmp = new string[_count];
                Array.Copy(_paths, _tmp, _paths.Length);
                Array.Copy(_paths, 0, _tmp, _paths.Length, _count - _paths.Length);
                _paths = _tmp;
            }

            for (int i = 0; i < _num_threads; i++)
            {
                int _size = ((_batch_size * (i + 1)) < _count) ? _batch_size : (_count - (_batch_size * i));
                _batches[i] = new string[_size];
                for (int j = 0; j < _size; j++)
                {
                    _batches[i][j] = _paths[(_batch_size * i) + j];
                }
            }


            #endregion




            #region Start Threads

            for (int i = 0; i < _num_threads; i++)
            {
                _t[i].Start(new object[] { _batches[i] });
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
        private IDictionary<string, int> _result = new Dictionary<string, int>();
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

        #region Word_

        private struct Word_
        {
            public Word_(int _offset, int _count)
            {
                this._offset = _offset;
                this._count = _count;
            }

            public int _offset, _count;
        }

        #endregion

        void _Work(object _o)
        {

            object[] o = (object[])_o;
            string[] _paths = (string[])o[0];
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
            #region Main Work Loop

#if PROFILE_LOCKWAITING
#if LOCKWAITING_SUM
            double _sum_lockwait = 0;
#endif
#endif
            for (int k = 0; k< _paths.Length; k++)
            {

                string _path = _paths[k];
#if SORT
                string _text;
                using (StreamReader _reader = new StreamReader(_path))
                    _text = _reader.ReadToEnd();
                int _i = 0;
                int _n = _text.Length;
                List<Word_> _words = new List<Word_>();
                while (_i < _n)
                {
                    while ((_i < _n) && !char.IsLetter(_text[_i]))
                        _i++;
                    if (_i < _n)
                    {
                        int _j = _i;
                        while ((_j < _n) && char.IsLetter(_text[_j]))
                            _j++;
                        _words.Add(new Word_(_i, _j - _i));
                        _i = _j;
                    }
                    else
                        break;
                }
                _words.Sort(
                    new Comparison<Word_>(
                        delegate(Word_ _x, Word_ _y)
                        {
                            int _count = _x._count;
                            if (_y._count < _count)
                                _count = _y._count;
                            for (int _k = 0; _k < _count; _k++)
                            {
                                char _xc = _text[_x._offset + _k];
                                char _yc = _text[_y._offset + _k];
                                if (_xc < _yc)
                                    return -1;
                                if (_xc > _yc)
                                    return 1;
                            }
                            if (_x._count < _y._count)
                                return -1;
                            if (_x._count > _y._count)
                                return 1;
                            return 0;
                        }));
                Word_ _a = new Word_(0, 0);
                int _c = 0;
                foreach (Word_ _b in _words)
                {
                    bool _isnew = false;
                    if (_a._count == _b._count)
                        for (int _k = 0; (!_isnew && (_k < _a._count)); _k++)
                            _isnew = (_text[_a._offset + _k] != _text[_b._offset + _k]);
                    else
                        _isnew = true;
                    if (_isnew)
                    {
                        if (_c > 0)
                        {
                            string _w = _text.Substring(_a._offset, _a._count);
                            int _cc;
#if PROFILE_LOCKWAITING
                            double _t1 = this._clock.Time;
#endif
                            lock (this._result)
                            {
#if PROFILE_LOCKWAITING
                                double _t2 = this._clock.Time;
#if LOCKWAITING_SUM
                                _sum_lockwait += _t2 - _t1;
#endif
                                double _d = (_t2 - _t1) * _lw_unit;
                                if(_d >= 0)
                                    this._lockwaiting_times.Add(_t1, _d);
#endif
                                if (!_result.TryGetValue(_w, out _cc))
                                    _cc = 0;
                                _result[_w] = _c + _cc;
                            }
                        }
                        _a = _b;
                        _c = 0;
                    }
                    _c++;
                }
                if (_c > 0)
                {
                    string _w = _text.Substring(_a._offset, _a._count);
                    int _cc;
#if PROFILE_LOCKWAITING
                    double _t1 = this._clock.Time;
#endif
                    lock (this._result)
                    {
#if PROFILE_LOCKWAITING
                        double _t2 = this._clock.Time;
#if LOCKWAITING_SUM
                        _sum_lockwait += _t2 - _t1;
#endif
                        double _d = (_t2 - _t1) * _lw_unit;
                        if (_d >= 0)
                            this._lockwaiting_times.Add(_t1, _d);
#endif
                        if (!_result.TryGetValue(_w, out _cc))
                            _cc = 0;
                        _result[_w] = _c + _cc;
                    }
                }

#if PROFILE_1
            if (this._clock != null)
                this._profile_1.Log(this._clock.Time - _timestamp);
#endif
#else
                TextReader _r = new StreamReader(new FileStream(_path, FileMode.Open));
                string _rte = _r.ReadToEnd();
                _r.Close();

#if PAIR
            string _last_word = null;
            string _curr_word;
#endif

                string[] _l = _rte.Split('\n');


                for (int i = 0; i < _l.Length; i++)
                {


                    string[] words = _l[i].Trim().Split(' ');
                    for (int j = 0; j < words.Length; j++)
                    {

                        //StringBuilder _sb = new StringBuilder();
                        //char[] _a = words[j].ToCharArray();

                        //for (int k = 0; k < _a.Length; k++)
                        //{
                        //    if (!char.IsPunctuation(_a[k]))
                        //    {
                        //        _sb.Append(_a[k]);
                        //    }
                        //}
                        string _word = words[j];
#if PAIR
                    if (_last_word == null)
                    {
                        // this is the first word
                        _last_word = _word;
                        continue;
                    }
                    else
                    {
                        _curr_word = _word;
                    }

                    int _count;
                    string _word_pair = _last_word + " " + _curr_word;
                    lock (this._result)
                    {
                        if (this._result.TryGetValue(_word_pair, out  _count))
                        {
                            this._result[_word_pair] = _count + 1;
                        }
                        else
                        {
                            this._result.Add(_word_pair, 1);
                        }
                    }
                    _last_word = _curr_word;
#else
                        int _val = 0;
                        lock (this._result)
                        {
                            if (_result.TryGetValue(_word, out _val))
                            {
                                _result[_word] = _val + 1;
                            }
                            else
                            {
                                _result.Add(_word, 1);
                            }
                        }
#endif

                    }

                }
#endif
            }

            #endregion

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

                //this._cpuutil.CopyStats(_duration);
                this._mycontext.Platform.Logger.Log("Duration : " + _duration.ToString());
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
                    if (i++ == _c)
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

        }

    }
}

