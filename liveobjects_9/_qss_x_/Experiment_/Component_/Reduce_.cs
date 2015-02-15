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
//#define PROFILE
//#define PAIR
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QS._qss_x_.Experiment_.Value_;

using System.Diagnostics;
namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("AC3CAD10030A4bd68B58649DD0FF5983","MapReduce_Reduce")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public class Reduce_: QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Interface_.IReduce_, QS._qss_x_.Experiment_.Object_.IReduce_
    {
        public Reduce_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Directory",QS.Fx.Reflection.ParameterClass.Value)] string _dir,
            [QS.Fx.Reflection.Parameter("Batch Size", QS.Fx.Reflection.ParameterClass.Value)] int _batch_size,
            [QS.Fx.Reflection.Parameter("Out File",QS.Fx.Reflection.ParameterClass.Value)] string _out_file)
        {
            this._mycontext = _mycontext;
            this._platform = _mycontext.Platform;            
            this._dir = _dir;
            this._batch_size = _batch_size;
            if (_out_file != null)
                this._out_file = _out_file;
            else
                this._out_file = null;

            _map_endpt = _mycontext.DualInterface< QS._qss_x_.Experiment_.Interface_.IMap_,QS._qss_x_.Experiment_.Interface_.IReduce_>(this);
            _map_endpt.OnConnected += new QS.Fx.Base.Callback(_map_endpt_OnConnected);
            //_map_endpt.Connect(_map.Dereference(_mycontext).Mapper);

            _platform.Logger.Log("Batch size = " + _batch_size);

            _total = new Dictionary<string,
#if PAIR
                IDictionary<string,int>
#else
                int
#endif
                >();
            
            
        }

        void _map_endpt_OnConnected()
        {
            _files_to_process = new Queue<string>();
            foreach (string file in Directory.GetFiles(_dir))
            {
                _files_to_process.Enqueue(file);
            }
            _num_files = _files_to_process.Count;
            _start_time = this._platform.Clock.Time;
            for (int i = 0; i < _batch_size; i++)
            {
                try
                {
                    _map_endpt.Interface.Map(_files_to_process.Dequeue());
                }
                catch (InvalidOperationException e)
                {
                    // Queue empty
                    break;
                }
            }
        }

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        private string _out_file;
        private Queue<string> _files_to_process;
        private int _batch_size;
        private Stopwatch _watch;
        private string _dir;
        private int _num_files;
        private int _num_files_completed = 0;
        private IDictionary<string, 
#if PAIR
            IDictionary<string,int>
#else

            int
#endif
            > _total;
        private QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Experiment_.Interface_.IMap_, QS._qss_x_.Experiment_.Interface_.IReduce_> _map_endpt;
        private QS.Fx.Platform.IPlatform _platform;
        [QS.Fx.Base.Inspectable]
        double _start_time;
        [QS.Fx.Base.Inspectable]
        double _end_time;
        [QS.Fx.Base.Inspectable]
        double _expr_time;
#if PROFILE
        
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _lock_waiting =
            new QS._qss_c_.Statistics_.Samples2D(
                "lock wait time", "time spent waiting for reduce lock", "time", "x", "reduce call starting at time x", "time", "s", "time spent waiting for reduce lock");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _reducing_time =
            new QS._qss_c_.Statistics_.Samples2D(
                "processing time", "time to merge results", "time", "s", "reduce call starting at time x", "time", "s", "time to merge results");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _reduce_complete =
            new QS._qss_c_.Statistics_.Samples2D(
                "completed reduces", "time reduce are completed", "time", "s", "reduce call ending at time x", "completed", "#", "# completed");
        
#endif

        #endregion

        #region IReduce_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Concurrent)]
        void QS._qss_x_.Experiment_.Interface_.IReduce_.ConcurrentReduce(MapReduce_Dict_ _mapped)
        {
#if PROFILE
            double _id = this._platform.Clock.Time;
            double _begin_lock = this._platform.Clock.Time;
#endif
            lock (this._total)
            {
#if PROFILE
                double _end_lock=this._platform.Clock.Time;
                _lock_waiting.Add(_id, _end_lock - _begin_lock);
                double _begin_proc = this._platform.Clock.Time;
#endif

                

                Reduce(_mapped);

                if (_files_to_process.Count > 0)
                {
                    _map_endpt.Interface.Map(_files_to_process.Dequeue());
                }
                
                if (_num_files_completed == _num_files)
                {
                    this._end_time = this._platform.Clock.Time;
                    this._expr_time = this._end_time - this._start_time;
                    _mycontext.Platform.Logger.Log("Elapsed time: " +this._expr_time);
                    if (_out_file != null)
                    {
                        TextWriter _tw = new StreamWriter(new FileStream(_out_file, FileMode.Create));
                        _tw.WriteLine(this._expr_time);
                        _tw.Close();
                        Process.GetCurrentProcess().Kill();
                    }
                }
#if PROFILE
                double _end_proc = this._platform.Clock.Time;
                _reducing_time.Add(_id, _end_proc - _begin_proc);
                double _end_time = this._platform.Clock.Time;
                _reduce_complete.Add(_end_time, 1);
#endif
            }

        }


        private void Reduce(MapReduce_Dict_ _mapped)
        {
#if PAIR
            foreach(KeyValuePair<string, Dictionary<string,int>> _first_word in _mapped.Dictionary) {
                IDictionary<string, int> _t;
                if(!_total.TryGetValue(_first_word.Key,out _t)) {
                    _t = new Dictionary<string, int>();
                    _total.Add(_first_word.Key, _t);
                }
                foreach (string _second_word in _first_word.Value.Keys)
                {
                    int _val;
                    if (_t.TryGetValue(_second_word, out _val))
                    {
                        _t[_second_word] = _val + _first_word.Value[_second_word];
                    }
                    else
                    {
                        _t.Add(_second_word, _first_word.Value[_second_word]);
                    }
                }
            }
#else
            foreach (KeyValuePair<string, int> _map in _mapped.Dictionary)
            {
                int old_val;
                if (_total.TryGetValue(_map.Key, out old_val))
                {
                    _total[_map.Key] = _map.Value + old_val;
                }
                else
                {
                    _total.Add(_map);
                }
            }
            
#endif
            _num_files_completed++;
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
        void QS._qss_x_.Experiment_.Interface_.IReduce_.SerializedReduce(MapReduce_Dict_ _mapped, double _begin_time)
        {
#if PROFILE
            double _end_time = this._platform.Clock.Time;
            double _id = this._platform.Clock.Time;
            this._lock_waiting.Add(_id, _end_time - _begin_time);
            double _begin_proc = this._platform.Clock.Time;
#endif

            

            Reduce(_mapped);

            if (_files_to_process.Count > 0)
            {
                _map_endpt.Interface.Map(_files_to_process.Dequeue());
            }
            
            
            if (_num_files_completed == _num_files)
            {

                this._end_time = this._platform.Clock.Time;
                this._expr_time = this._end_time - this._start_time;
                _mycontext.Platform.Logger.Log("Elapsed time: " + this._expr_time);
                if (_out_file != null)
                {
                    TextWriter _tw = new StreamWriter(new FileStream(_out_file, FileMode.Create));
                    _tw.WriteLine(this._expr_time);
                    _tw.Close();
                    Process.GetCurrentProcess().Kill();
                }
            }
#if PROFILE
            double _end_proc = this._platform.Clock.Time;
            this._reducing_time.Add(_id, _end_proc - _begin_proc);
            double __end_time = this._platform.Clock.Time;
            _reduce_complete.Add(__end_time, 1);
#endif
        }


        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Experiment_.Interface_.IMap_, QS._qss_x_.Experiment_.Interface_.IReduce_> QS._qss_x_.Experiment_.Object_.IReduce_.Reducer
        {
            get { return this._map_endpt; }
        }

        #endregion
    }
}

