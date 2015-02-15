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

//#define CPUUTIL
#define PROFILE_ENQUEUE_TIMES

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Security.Cryptography;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("6D740AA124704f1eB14D4BBF00CD338D", "Multithreaded_ReplicatedDictionaryClient_4_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Multithreaded_ReplicatedDictionaryClient_4_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IDictionaryClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Multithreaded_ReplicatedDictionaryClient_4_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("get burst size", QS.Fx.Reflection.ParameterClass.Value)]
            int _get_burst_size,
            [QS.Fx.Reflection.Parameter("ratio add to get", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ratio_add_to_get,
            [QS.Fx.Reflection.Parameter("# of threads", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_threads,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference)
        {
            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._workreference = _workreference;
            this._clock = this._mycontext.Platform.Clock;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionary_,
                    QS._qss_x_.Experiment_.Interface_.IDictionaryClient_>(this);
            this._count = _count;
            this._get_burst_size = _get_burst_size;
            this._ratio_add_to_get = _ratio_add_to_get;
            this._num_threads = _num_threads;
            this._threads = new Thread[_num_threads];

#if PROFILE_ENQUEUE_TIMES
            this._thread_ends = new double[_num_threads];
            this._thread_starts = new double[_num_threads];
#endif

            
            
            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i] = new Thread(new ParameterizedThreadStart(this._ThreadWork));
                _threads[i].Name = "Dict Client Worker " + i;
            }

            this._workendpoint.OnConnected += new QS.Fx.Base.Callback(_workendpoint_OnConnected);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);

        }

        void _workendpoint_OnConnected()
        {
            int size = this._count / this._num_threads;

            for (int i = 0; i < this._num_threads; i++)
            {
                //_workers[i] = new Worker_(i * _individual_dict_size, _num_cycles, _cd, _num_adds_per_cycle, _num_gets_per_cycle, new ThreadStart(_Done));
                _threads[i].Start(new object[] { i * size, size, i });
            }
        }

        #endregion

        private void _ThreadWork(object _params)
        {
            object[] _o = (object[])_params;
            int _base = (int)_o[0];
            int _size = (int)_o[1];
            int _id = (int)_o[2];

            int _curr = _base;

            if (Interlocked.Increment(ref this._ready) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if CPUUTIL

            _cpuutil.Start();
#endif

                this._begin = this._clock.Time;
                _start.Set();
            }
            else
            {
                _start.WaitOne();
            }
#if PROFILE_ENQUEUE_TIMES
            _thread_starts[_id] = this._clock.Time;
            int _submitted = 0;
#endif

            //while (_curr < _base + _size)
            //{
//                for (int i = 0; i < _get_burst_size; i++)
//                {
//#if PROFILE_ENQUEUE_TIMES
//                    _submitted++;
//#endif
//                    this._workendpoint.Interface._Get((_curr - 1).ToString());

//                }

                for (int i = 0; i < _ratio_add_to_get * _get_burst_size; i++)
                {
#if PROFILE_ENQUEUE_TIMES
                    _submitted++;
#endif
                    this._workendpoint.Interface._Add(_curr.ToString(), string.Empty);

                    _curr++;
                    if (_curr >= _base + _size)
                        break;
                }
                
            //}

#if PROFILE_ENQUEUE_TIMES
            _thread_ends[_id] = this._clock.Time;
#endif




            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {
                this._workendpoint.Interface._Get((_curr - 1).ToString());
#if PROFILE_ENQUEUE_TIMES
                double _start_min = _thread_starts.Min();
                double _end_max = _thread_ends.Max();
                this._mycontext.Platform.Logger.Log("Overall time to enqueue: " + (_end_max - _start_min).ToString());
                for (int i = 0; i < _num_threads; i++)
                {
                    this._mycontext.Platform.Logger.Log("Thread[" + i + "] time to enqueue: " + (_thread_ends[i] - _thread_starts[i]).ToString());
                }
                this._mycontext.Platform.Logger.Log("Total events submitted: " + _submitted);
#endif
                this._workendpoint.Interface._Count();

            }

        }

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        #region Fields
#if PROFILE_ENQUEUE_TIMES
        [QS.Fx.Base.Inspectable]
        private double[] _thread_starts;
        [QS.Fx.Base.Inspectable]
        private double[] _thread_ends;
#endif
        [QS.Fx.Base.Inspectable]
        private double _ratio_add_to_get;
        
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private int _get_burst_size;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _start = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _done = 0;
        [QS.Fx.Base.Inspectable]
        private int _ready = 0;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IDictionary_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IDictionary_,
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double _begin;
        [QS.Fx.Base.Inspectable]
        private double _end;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicatedDictionaryClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Got(string key, string _vals)
        {
            


        }





        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Cleared()
        {

        }

        #endregion

        #region IDictionaryClient_ Members


        void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Counted(int _count)
        {
            this._mycontext.Platform.Logger.Log("Counted: " + _count);

            this._end = this._clock.Time;
#if CPUUTIL
                this._cpuutil.Stop();
#endif
            this._mycontext.Platform.Logger.Log("Done");
            this._mycontext.Platform.Logger.Log("Elapsed: " + (this._end - this._begin).ToString());

            this._workendpoint.Interface._DumpStats();
            QS._qss_x_.Experiment_.Utility_.ClipboardThread_ _t = new QS._qss_x_.Experiment_.Utility_.ClipboardThread_((this._end - this._begin).ToString());


        }

        #endregion
    }
}
