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
#define PROFILE
#define PROFILE_FINE_GRAINED_1
#define PROFILE_FINE_GRAINED_2
#define PROFILE_FINE_GRAINED_3
//#define PAIR

using System;
using QS._qss_x_.Experiment_.Value_;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("228ABDBDC8E54460B055FE785E8ED178", "MapReduce_Map")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public class Map_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Interface_.IMap_, QS._qss_x_.Experiment_.Object_.IMap_, IDisposable
    {
        #region Constructor

        public Map_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Reducer", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReduce_> _reduce,
            [QS.Fx.Reflection.Parameter("Concurrent?", QS.Fx.Reflection.ParameterClass.Value)] int _concurrent)
        {
#if PROFILE_FINE_GRAINED_1
            _profile_fine_grained_1 = new QS._qss_e_.Data_.FastStatistics_(1000);
#endif
#if PROFILE_FINE_GRAINED_2
            _profile_fine_grained_2 = new QS._qss_e_.Data_.FastStatistics_(1000000);
#endif
#if PROFILE_FINE_GRAINED_3
            _profile_fine_grained_3 = new QS._qss_e_.Data_.FastStatistics_(2000000);
#endif

            this._mycontext = _mycontext;
            this._concurrent = _concurrent;
            this._platform = _mycontext.Platform;
            string _c;
            switch (_concurrent)
            {
                case 0:
                    _c = "disabled";
                    break;
                case 1:
                    _c = "concurrent";
                    break;

                case 2:
                    _c = "serialized";
                    break;
                default:
                    throw new NotImplementedException();
            }
            _platform.Logger.Log("Reduce configured as " + _c);
            _reduce_endpt = _mycontext.DualInterface<QS._qss_x_.Experiment_.Interface_.IReduce_, QS._qss_x_.Experiment_.Interface_.IMap_>(this);
            this._reduce_obj = _reduce.Dereference(_mycontext);
            _conn = _reduce_endpt.Connect(_reduce_obj.Reducer);
        }

        #endregion

        #region Destructor

        ~Map_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
                if (_disposemanagedresources)
                    this._Dispose();
        }

        protected virtual void _Dispose()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        private int _disposed;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Experiment_.Interface_.IReduce_, QS._qss_x_.Experiment_.Interface_.IMap_> _reduce_endpt;
        private int _concurrent;
        private QS.Fx.Platform.IPlatform _platform;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IReduce_ _reduce_obj;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _conn;

#if PROFILE
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _document_read =
            new QS._qss_c_.Statistics_.Samples2D(
                "time reading file", "time reading the file", "time", "s", "map call starting at time x", "time", "s", "time to read the file");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _processing_time =
            new QS._qss_c_.Statistics_.Samples2D(
                "processing time", "time processing the file", "time", "s", "map call starting at time x", "time", "s", "time to process the file");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _map_complete =
            new QS._qss_c_.Statistics_.Samples2D(
                "completed maps", "time maps are completed", "time", "s", "map call ending at time x", "completed", "#", "# completed");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _read_complete =
            new QS._qss_c_.Statistics_.Samples2D(
                "completed reads", "time reads are completed", "time", "s", "read call ending at time x", "completed", "#", "# completed");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _alloc_time =
            new QS._qss_c_.Statistics_.Samples2D(
                "allocation time", "time allocating dictionary", "time", "s", "map call starting at time x", "time", "s", "time to alloc dictionary");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _total_time =
            new QS._qss_c_.Statistics_.Samples2D(
                "total time", "total map call time", "time", "s", "map call starting at time x", "time", "s", "total time");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _num_inner_allocs =
            new QS._qss_c_.Statistics_.Samples2D(
                "number of inner allocs", "", "time", "s", "map call starting at time x", "#", "", "number of inner allocs for this call");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples1D _inner_alloc_time =
            new QS._qss_c_.Statistics_.Samples1D("inner dict alloc time", "time", "s");
#endif

#if PROFILE_FINE_GRAINED_1
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics_ _profile_fine_grained_1;
#endif
#if PROFILE_FINE_GRAINED_2
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics_ _profile_fine_grained_2;
#endif
#if PROFILE_FINE_GRAINED_3
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics_ _profile_fine_grained_3;
#endif

        #endregion

        #region IMap_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Experiment_.Interface_.IReduce_, QS._qss_x_.Experiment_.Interface_.IMap_> QS._qss_x_.Experiment_.Object_.IMap_.Mapper
        {
            get { return this._reduce_endpt; }
        }

        #endregion

        #region IMap_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Concurrent)]
        void QS._qss_x_.Experiment_.Interface_.IMap_.Map(string path_to_map)
        {
#if PROFILE_FINE_GRAINED_1
            double profile_fine_grained_1_sample_from = _platform.Clock.Time;
#endif

#if PROFILE
            double _id = this._platform.Clock.Time;
            int _inner_allocs = 0;
            double _begin_total = this._platform.Clock.Time;
            double _begin_alloc = this._platform.Clock.Time;
#endif
            MapReduce_Dict_ _dict = new MapReduce_Dict_();

#if PROFILE
            double _end_alloc = this._platform.Clock.Time;
            this._alloc_time.Add(_id, _end_alloc - _begin_alloc);
            double _begin_read = this._platform.Clock.Time;
#endif

            TextReader _r = new StreamReader(new FileStream(path_to_map, FileMode.Open));
            string _rte = _r.ReadToEnd();
            _r.Close();

#if PROFILE
            double _end_read = this._platform.Clock.Time;
            _document_read.Add(_id, _end_read - _begin_read);
            _read_complete.Add(_end_read, 1);
            double _begin_proc = this._platform.Clock.Time;
#endif
#if PAIR
            string _last_word = null;
            string _curr_word;
#endif

            string[] _l = _rte.Split('\n');

            //foreach (string line in _rte.Split('\n'))
            for(int i = 0;i<_l.Length;i++)
            {
#if PROFILE_FINE_GRAINED_2
                double profile_fine_grained_2_sample_from = _platform.Clock.Time;
#endif

                string[] words = _l[i].Trim().ToLower().Split(' ');
                for(int j=0;j<words.Length;j++)
                //foreach (string word in words)
                {
#if PROFILE_FINE_GRAINED_3
                    double profile_fine_grained_3_sample_from = _platform.Clock.Time;
#endif

                    StringBuilder _sb = new StringBuilder();
                    char[] _a = words[j].ToCharArray();
                    //foreach (char _c in words[j].ToCharArray())
                    for(int k =0;k<_a.Length;k++) 
                    {
                        if (!char.IsPunctuation(_a[k]))
                        {
                            _sb.Append(_a[k]);
                        }
                    }
                    string _word = _sb.ToString();
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

                    Dictionary<string, int> _val;
                    if (_dict.Dictionary.TryGetValue(_last_word, out  _val))
                    {
                        int _v;
                        if (_val.TryGetValue(_curr_word, out _v))
                        {
                            _val[_curr_word] = _v + 1;
                        }
                        else
                        {
                            _val.Add(_curr_word, 1);
                        }
                    }
                    else
                    {
#if PROFILE
                        _inner_allocs++;
                        double _begin_inner_alloc = this._platform.Clock.Time;
#endif
                        _val = new Dictionary<string, int>();
#if PROFILE
                        double _end_inner_alloc = this._platform.Clock.Time;
                        _inner_alloc_time.Add(_end_inner_alloc - _begin_inner_alloc);
#endif
                        _val.Add(_curr_word, 1);
                        _dict.Dictionary.Add(_last_word, _val);
                    }
                    _last_word = _curr_word;
#else
                    int _val = 0;
                    if (_dict.Dictionary.TryGetValue(_word, out _val))
                    {
                        _dict.Dictionary[_word] = _val + 1;
                    }
                    else
                    {
                        _dict.Dictionary.Add(_word, 1);
                    }
#endif

#if PROFILE_FINE_GRAINED_3
                    this._profile_fine_grained_3.Log(_platform.Clock.Time - profile_fine_grained_3_sample_from);
#endif
                }

#if PROFILE_FINE_GRAINED_2
                this._profile_fine_grained_2.Log(_platform.Clock.Time - profile_fine_grained_2_sample_from);
#endif
            }
            double _context_queue_time = -1;
#if PROFILE
            double _end_proc = this._platform.Clock.Time;
            _processing_time.Add(_id, _end_proc - _begin_proc);

#endif

            if (_concurrent == 1)
            {
                _reduce_endpt.Interface.ConcurrentReduce(_dict);
            }
            else if (_concurrent == 2)
            {
#if PROFILE
                _context_queue_time = this._platform.Clock.Time;
#endif
                _reduce_endpt.Interface.SerializedReduce(_dict, _context_queue_time);
            }

#if PROFILE
            double _end_map = this._platform.Clock.Time;
            _map_complete.Add(_end_map, 1);
            double _end_total = this._platform.Clock.Time;
            _total_time.Add(_id, _end_total - _begin_total);
            _num_inner_allocs.Add(_id, _inner_allocs);
#endif

#if PROFILE_FINE_GRAINED_1
            this._profile_fine_grained_1.Log(_platform.Clock.Time - profile_fine_grained_1_sample_from);
#endif
        }

        #endregion
    }
}

