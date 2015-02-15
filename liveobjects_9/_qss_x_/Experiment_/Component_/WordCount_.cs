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
//#define PROFILE_TIME_BTWN_WORKING
//#define PROFILE_WORKING
//#define PROFILE_WORKING_1
//#define PROFILE_IMPORT
//#define EXPORT_TIMELINE
//#define PROFILE_REPLICAS
//#define PROFILE_ADD
//#define PROFILE_SORT
//#define PROFILE_SPLIT
//#define PROFILE_NEW
//#define SORT
#define PAIR
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("2DDA0F3F650F4FFEB85EA9BC0253904A")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_WordCount)]
    public sealed class WordCount_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IWordCount_, QS._qss_x_.Experiment_.Interface_.IWordCount_, QS.Fx.Replication.IReplicated<WordCount_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WordCount_(QS.Fx.Object.IContext _mycontext)
        {

            this._mycontext = _mycontext;
            this._clock = _mycontext.Platform.Clock;

#if PROFILE_REPLICAS
            this._replica_times = new QS._qss_c_.Statistics_.Samples2D();
#endif
            QS._qss_x_.Object_.ReplicaContext_ _r = _mycontext as QS._qss_x_.Object_.ReplicaContext_;
            if (_r == null)
            {
                this._workendpoint = this._mycontext.DualInterface<
                    QS._qss_x_.Experiment_.Interface_.IWordCountClient_,
                        QS._qss_x_.Experiment_.Interface_.IWordCount_>(this);

                this._master = true;
#if PHASECPU
                this._workphase = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
                this._mergephase = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
#endif
#if PROFILE_ADD
                this._replica_addtimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
#if PROFILE_NEW
                this._replica_newtimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
#if PROFILE_SORT
                this._replica_sorttimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
#if PROFILE_1
            this._replica_addtimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
#if PROFILE_SPLIT
            this._replica_splittimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
#if PROFILE_TIME_BTWN_WORKING
            this._replica_diffs = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif
                #region Profiling

#if PROFILE_1
                this._replica_addtimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif

#if PROFILE_WORKING
            this._replica_worktimes = new List<QS._qss_c_.Statistics_.Samples2D>();
#endif

                #endregion
            }
            else
            {
#if PROFILE_1
                this._add_times = new QS._qss_c_.Statistics_.Samples2D();
#endif
                //if (!_master)
                //    this._mycontext.Platform.Logger.Log("T" + this._mycontext.Platform.Clock.Time.ToString());
            }

        }

        public WordCount_()
        {

        }


        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields



#if PROFILE_WORKING
        [QS.Fx.Base.Inspectable]
        static int _working = 0;
#endif
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [NonSerialized]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWordCountClient_,
                QS._qss_x_.Experiment_.Interface_.IWordCount_> _workendpoint;

        [QS.Fx.Base.Inspectable]
        private IDictionary<string, int> _result = new Dictionary<string, int>();

        [QS.Fx.Base.Inspectable]
        private bool _master = false;
        [QS.Fx.Base.Inspectable]
        private double _begin_import = -1;

        #region Profiling
#if REPLICA_GCUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.GCUtil_ _gcutil;
#endif
#if PROFILE_REPLICAS
        [QS.Fx.Base.Inspectable]
        private double _replica_start = -1;
        [QS.Fx.Base.Inspectable]
        private double _replica_end = -1;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _replica_times;
#endif
#if PROFILE_1
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _add_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_addtimes;
        private bool _is_merged_addtimes = false;
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _merged_addtimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_Addtimes
        {
            get
            {
                if (!_is_merged_addtimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                    _merged_addtimes = _m._Merge_Samples(_replica_addtimes);
                    _is_merged_addtimes = true;
                }
                return _merged_addtimes;
            }
        }
#endif
#if PROFILE_2
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _import_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _work_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _export_times = new QS._qss_c_.Statistics_.Samples2D();
        
#endif
#if PROFILE_WORKING
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _worktimes = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_worktimes;
        private bool _is_merged_worktimes = false;
        private QS._qss_c_.Statistics_.Samples2D _merged_worktimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_Worktimes
        {
            get
            {
                if (!_is_merged_worktimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                   _merged_worktimes = _m._Merge_Samples(_replica_worktimes);
                   _is_merged_worktimes = true;
                }
                return _merged_worktimes;
            }
        }
#endif

#if PROFILE_IMPORT

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _import_size = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        public QS._qss_c_.Statistics_.Samples2D _import_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        public double AverageKeySize
        {
            get
            {
                Int64 _c = 0;
                foreach (KeyValuePair<string,int> _s in _result)
                {
                    _c += _s.Key.Length;
                }
                return _c / (double)_result.Count;
            }
        }
#endif
#if PROFILE_ADD
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _add_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_addtimes;
#endif
#if PROFILE_SPLIT
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _split_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_splittimes;
        [QS.Fx.Base.Inspectable]
        public double SplitTimeAvg
        {
            get
            {
                return AvgListofSample2D(_replica_splittimes);
            }
        }
        private bool _is_merged_splittimes = false;
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _merged_splittimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_splittimes
        {
            get
            {
                if (!_is_merged_splittimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                    _merged_splittimes = _m._Merge_Samples(_replica_splittimes);
                    _is_merged_splittimes = true;
                }
                return _merged_splittimes;
            }
        }
#endif

#if PROFILE_SORT
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _sort_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_sorttimes;// = new List<QS._qss_c_.Statistics_.Samples2D>();
        private bool _is_merged_sorttimes = false;
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _merged_sorttimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_sorttimes
        {
            get
            {
                if (!_is_merged_sorttimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                    _merged_sorttimes = _m._Merge_Samples(_replica_sorttimes);
                    _is_merged_sorttimes = true;
                }
                return _merged_sorttimes;
            }
        }
#endif
        private double AvgListofSample2D(IList<QS._qss_c_.Statistics_.Samples2D> _list)
        {
            double _sum_y = 0;
            foreach (QS._qss_c_.Statistics_.Samples2D _s in _list)
            {

                foreach (QS._core_e_.Data.XY _xy in _s.Samples)
                {
                    _sum_y += _xy.y;
                }
            }
            _sum_y /= _list.Count;
            return _sum_y;
        }
#if PROFILE_NEW
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _new_times = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_newtimes;
        [QS.Fx.Base.Inspectable]
        public double NewTimeAvg
        {
            get
            {
                return AvgListofSample2D(_replica_newtimes);
            }
        }
        private bool _is_merged_newtimes = false;
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _merged_newtimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_newtimes
        {
            get
            {
                if (!_is_merged_newtimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                    _merged_newtimes = _m._Merge_Samples(_replica_newtimes);
                    _is_merged_newtimes = true;
                }
                return _merged_newtimes;
            }
        }
#endif
#if PROFILE_TIME_BTWN_WORKING
        [QS.Fx.Base.Inspectable]
        private double _last_work=-1;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _work_diff = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IList<QS._qss_c_.Statistics_.Samples2D> _replica_diffs;
        private bool _is_merged_difftimes = false;
        [NonSerialized]
        private QS._qss_c_.Statistics_.Samples2D _merged_difftimes;
        [QS.Fx.Base.Inspectable]
        public QS._qss_c_.Statistics_.Samples2D _Merged_difftimes
        {
            get
            {
                if (!_is_merged_difftimes)
                {
                    QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utility_.Sample2D_Merger_();
                    _merged_difftimes = _m._Merge_Samples(_replica_diffs);
                    _is_merged_difftimes = true;
                }
                return _merged_difftimes;
            }
        }
#endif
        #endregion

#if PHASECPU
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _workphase;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _mergephase;
        [QS.Fx.Base.Inspectable]
        private bool _import_cpu_started = false;
        [QS.Fx.Base.Inspectable]
        private bool _export_cpu_started = false;
#endif
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWordCount_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWordCountClient_,
                QS._qss_x_.Experiment_.Interface_.IWordCount_>
                    QS._qss_x_.Experiment_.Object_.IWordCount_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWordCount_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IWordCount_._Work(string _path)
        {
#if PROFILE_REPLICAS
            if (this._replica_start == -1)
            {
                this._replica_start = this._mycontext.Platform.Clock.Time;
                this._mycontext.Platform.Logger.Log("Start: " + _replica_start);
            }
#endif

#if SORT
#if PROFILE_TIME_BTWN_WORKING
            if (_last_work != -1)
            {
                this._work_diff.Add(_last_work, this._mycontext.Platform.Clock.Time - _last_work);
            }
#endif

#if PROFILE_WORKING_1
            Interlocked.Increment(ref _working);
            
            double _t1 = this._clock.Time;
            this._worktimes.Add(_t1, _working);
            
#endif
#if PROFILE_1
            double _t1 = this._mycontext.Platform.Clock.Time; 
            
#endif
#if PROFILE_SPLIT
            double _t1 = this._mycontext.Platform.Clock.Time; 
#endif
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

#if PROFILE_SPLIT
            double _t2= this._mycontext.Platform.Clock.Time;
            this._split_times.Add(_t1, _t2 - _t1);
#endif
#if PROFILE_SORT
            double _t1 = this._mycontext.Platform.Clock.Time;
#endif
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
#if PROFILE_SORT
            double _t2 = this._mycontext.Platform.Clock.Time;
            this._sort_times.Add(_t1, _t2 - _t1);
#endif
            Word_ _a = new Word_(0, 0);
            int _c = 0;
#if PROFILE_NEW
            double _t1 = this._mycontext.Platform.Clock.Time;
#endif
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
#if PROFILE_ADD
                        double _t1 = this._mycontext.Platform.Clock.Time;
#endif
                        if (!_result.TryGetValue(_w, out _cc))
                            _cc = 0;
                        _result[_w] = _c + _cc;
#if PROFILE_ADD
                        double _t2 = this._mycontext.Platform.Clock.Time;
                        this._add_times.Add(_t1, _t2 - _t1);
#endif
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
#if PROFILE_ADD
                double _t1 = this._mycontext.Platform.Clock.Time;
#endif
                if (!_result.TryGetValue(_w, out _cc))
                    _cc = 0;
                _result[_w] = _c + _cc;
#if PROFILE_ADD
                double _t2 = this._mycontext.Platform.Clock.Time;
                this._add_times.Add(_t1, _t2 - _t1);
#endif
            }
#if PROFILE_NEW
            double _t2 = this._mycontext.Platform.Clock.Time;
            this._new_times.Add(_t1, _t2 - _t1);
#endif

#if PROFILE_1
            double _t2 = this._mycontext.Platform.Clock.Time;
            this._add_times.Add(_t1, _t2 - _t1);
#endif
#if PROFILE_WORKING_1
            Interlocked.Decrement(ref _working);
#endif

#if PROFILE_TIME_BTWN_WORKING
            this._last_work = this._mycontext.Platform.Clock.Time;
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
                    if (this._result.TryGetValue(_word_pair, out  _count))
                    {
                        this._result[_word_pair] = _count + 1;
                    }
                    else
                    {
                        this._result.Add(_word_pair, 1);
                    }
                    _last_word = _curr_word;
#else
                    int _val = 0;
                    if (_result.TryGetValue(_word, out _val))
                    {
                        _result[_word] = _val + 1;
                    }
                    else
                    {
                        _result.Add(_word, 1);
                    }
#endif

                }

            }


#endif
#if PROFILE_REPLICAS
            this._replica_end = this._mycontext.Platform.Clock.Time;
#endif

        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IWordCount_._Work2(string[] _paths)
        {
#if PROFILE_WORKING_2
            Interlocked.Increment(ref _working);
            double _t1 = this._clock.Time;
            this._worktimes.Add(_t1, _working);
#endif

            for (int i = 0; i < _paths.Length; i++)
            {
                ((QS._qss_x_.Experiment_.Interface_.IWordCount_)this)._Work(_paths[i]);
            }

#if PROFILE_WORKING_2
            Interlocked.Decrement(ref _working);
#endif
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IWordCount_._Done()
        {
#if PHASECPU
            this._mergephase.Stop();
#endif
            this._workendpoint.Interface._Done(this._result);
#if PHASECPU
            this._workphase.PrintAvg();
            this._mergephase.PrintAvg();
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<WordCount_> Members

        void QS.Fx.Replication.IReplicated<WordCount_>.Export(WordCount_ _other)
        {
#if PROFILE_2
            double _t1=0;
            if (this._master)
            {
                _t1 = this._clock.Time;
            }
#endif
#if PHASECPU
            if (this._master && !_export_cpu_started)
            {
                _export_cpu_started = true;
                this._workphase.Start();
                
            }
#endif
            _other._master = false;
            _other._result.Clear();
            _other._clock = this._clock;

            #region Profiling

#if PROFILE_2
            if (this._master)
            {
                double _t2 = this._clock.Time;
                this._export_times.Add(_t1, _t2 - _t1);
            }
#endif

            #endregion
        }

        void QS.Fx.Replication.IReplicated<WordCount_>.Import(WordCount_ _other)
        {
#if PROFILE_IMPORT
            double _t1_import = this._clock.Time;
            this._import_size.Add(_t1_import, _other._result.Count);
#endif
#if PHASECPU
            if (this._master && !_import_cpu_started)
            {
                _import_cpu_started = true;
                this._workphase.Stop();
                this._mergephase.Start();
            }
#endif
            //if (this._master && _begin_import == -1)
            //{
            //    _begin_import = this._clock.Time;
            //}
#if PROFILE_2
            double _t1 = 0;
            if (this._master)
            {
                _t1 = this._clock.Time;
            }
#endif
            foreach (KeyValuePair<string, int> _element in _other._result)
            {
                int _count;
                if (!_result.TryGetValue(_element.Key, out _count))
                    _count = 0;
                _count += _element.Value;
                _result[_element.Key] = _count;
            }
            _other._result.Clear();

#if PROFILE_IMPORT
            this._import_times.Add(_t1_import, this._clock.Time - _t1_import);
#endif
#if PROFILE_2
            if (this._master)
            {
                double _t2 = this._clock.Time;
                this._import_times.Add(_t1, _t2 - _t1);
            }
#endif
#if PROFILE_WORKING
            if (this._master)
            {
                this._replica_worktimes.Add(_other._worktimes);
            }
#endif
#if PROFILE_1
            if (this._master)
            {
                this._replica_addtimes.Add(_other._add_times);

            }
            else
                this._add_times = _other._add_times;
#endif
#if PROFILE_REPLICAS
            this._replica_times.Add(_other._replica_start, _other._replica_end);
#endif
#if PROFILE_ADD
            try
            {
                this._replica_addtimes.Add(_other._add_times);
            }
            catch (Exception e)
            {

            }
#endif
#if PROFILE_SORT
            try
            {
                if (this._master)
                {
                    this._replica_sorttimes.Add(_other._sort_times);
                }
                else
                {
                    this._sort_times = _other._sort_times;
                }
                
            }
            catch (Exception e)
            {

            }
#endif

#if PROFILE_TIME_BTWN_WORKING
            try
            {
                if (this._master)
                    this._replica_diffs.Add(_other._work_diff);
                else
                    this._work_diff = _other._work_diff;
            }
            catch (Exception e)
            {

            }
#endif
#if PROFILE_NEW
            try
            {
                if (this._master)
                    this._replica_newtimes.Add(_other._new_times);
                else
                    this._new_times = _other._new_times;
            }
            catch (Exception e)
            {

            }
#endif
#if PROFILE_SPLIT
            try
            {
                if (this._master)
                    this._replica_splittimes.Add(_other._split_times);
                else
                    this._split_times = _other._split_times;
            }
            catch (Exception e)
            {

            }
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                int _count = this._result.Count;
                int _headersize = sizeof(int) * (2 * _count + 1);

#if PROFILE_REPLICAS
                _headersize += 2 * sizeof(double);
#endif
                int _totalsize = _headersize;
                foreach (KeyValuePair<string, int> _element in this._result)
                    _totalsize += _element.Key.Length;
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(
                   (ushort)QS.ClassID.Experiment_Component_WordCount, _headersize, _totalsize, _count);
#if PROFILE_SORT
                 _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._sort_times).SerializableInfo);
#endif
#if PROFILE_SPLIT
                 _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._split_times).SerializableInfo);
#endif
#if PROFILE_NEW
                 _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._new_times).SerializableInfo);
#endif
#if PROFILE_1
                 _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._add_times).SerializableInfo);
#endif
#if PROFILE_TIME_BTWN_WORKING
                 _info.AddAnother(((QS.Fx.Serialization.ISerializable)this._work_diff).SerializableInfo);
#endif
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            int _count = (this._result != null) ? this._result.Count : 0;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*)_pbuffer) = _count;
                _pbuffer += sizeof(int);
#if PROFILE_REPLICAS
                *((double*)_pbuffer) = _replica_start;
                _pbuffer += sizeof(double);
                *((double*)_pbuffer) = _replica_end;
                _pbuffer += sizeof(double);
#endif

                if (_count > 0)
                {

                    foreach (KeyValuePair<string, int> _element in this._result)
                    {
                        *((int*)_pbuffer) = _element.Key.Length;
                        _pbuffer += sizeof(int);
                        *((int*)_pbuffer) = _element.Value;
                        _pbuffer += sizeof(int);
                        _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(_element.Key)));
                    }

                }
                _header.consume(sizeof(int) * (2 * _count + 1));
#if PROFILE_REPLICAS
                _header.consume(sizeof(double) * 2);
#endif
            }


#if PROFILE_SORT
            ((QS.Fx.Serialization.ISerializable)this._sort_times).SerializeTo(ref _header, ref _data);
#endif
#if PROFILE_SPLIT
            ((QS.Fx.Serialization.ISerializable)this._split_times).SerializeTo(ref _header, ref _data);
#endif
#if PROFILE_NEW
            ((QS.Fx.Serialization.ISerializable)this._new_times).SerializeTo(ref _header, ref _data);
#endif

#if PROFILE_1
            ((QS.Fx.Serialization.ISerializable)this._add_times).SerializeTo(ref _header, ref _data);
#endif
#if PROFILE_TIME_BTWN_WORKING
            ((QS.Fx.Serialization.ISerializable)this._work_diff).SerializeTo(ref _header, ref _data);
#endif
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _count;
            int _c;
            int _ic = 0;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                _count = *((int*)_pbuffer);
                _c = _count;
                _pbuffer += sizeof(int);
#if PROFILE_REPLICAS
                this._replica_start = *((double*)_pbuffer);
                _pbuffer += sizeof(double);
                this._replica_end = *((double*)_pbuffer);
                _pbuffer += sizeof(double); 
#endif

                this._result = new Dictionary<string, int>(_count);
                while (_count-- > 0)
                {
                    int _length = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    int _value = *((int*)_pbuffer);
                    _pbuffer += sizeof(int);
                    string _key = Encoding.ASCII.GetString(_data.Array, _data.Offset, _length);
                    _data.consume(_length);
                    this._result[_key] = _value;
                }
                _header.consume(sizeof(int) * (2 * _c + 1));
#if PROFILE_REPLICAS
                _header.consume(sizeof(double) * 2);
#endif
            }

#if PROFILE_SORT
            ((QS.Fx.Serialization.ISerializable)this._sort_times).DeserializeFrom(ref _header, ref _data);
#endif
#if PROFILE_SPLIT
            ((QS.Fx.Serialization.ISerializable)this._split_times).DeserializeFrom(ref _header, ref _data);
#endif
#if PROFILE_NEW
            ((QS.Fx.Serialization.ISerializable)this._new_times).DeserializeFrom(ref _header, ref _data);
#endif
#if PROFILE_1
            ((QS.Fx.Serialization.ISerializable)this._add_times).DeserializeFrom(ref _header, ref _data);
#endif
#if PROFILE_TIME_BTWN_WORKING
            ((QS.Fx.Serialization.ISerializable)this._work_diff).DeserializeFrom(ref _header, ref _data);
#endif
        }

        #endregion
    }
}
