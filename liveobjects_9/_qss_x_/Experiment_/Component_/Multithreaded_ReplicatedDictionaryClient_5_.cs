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
//#define PROFILE_ENQUEUE_TIMES

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
    [QS.Fx.Reflection.ComponentClass("D7F93E55959E43088B117D9C08D61B46", "Multithreaded_ReplicatedDictionaryClient_5_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Multithreaded_ReplicatedDictionaryClient_5_ : QS.Fx.Inspection.Inspectable,
        QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IDictionaryClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Multithreaded_ReplicatedDictionaryClient_5_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("# threads", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_threads,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference)
        {
            this._mycontext = _mycontext;
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
            this._workreference = _workreference;
            this._num_threads = _num_threads;
            this._clock = this._mycontext.Platform.Clock;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionary_,
                    QS._qss_x_.Experiment_.Interface_.IDictionaryClient_>(this);
            this._count = _count;
#if PROFILE_ENQUEUE_TIMES
            this._enqueue_times = new QS._qss_e_.Data_.FastStatistics2D_(_count);
#endif
            this._workendpoint.OnConnected += new QS.Fx.Base.Callback(_workendpoint_OnConnected);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
        }

        private void _Work()
        {
            int size = this._count;
            int _id = Interlocked.Increment(ref this._id);
            if (Interlocked.Increment(ref this._started) == this._num_threads)
            {


                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if PROFILE_ENQUEUE_TIMES
            double _t1 = this._clock.Time;
            int _submitted = 0;
#endif
                this._begin = this._clock.Time;
                _start.Set();
            }
            else
            {
                _start.WaitOne();
            }

            int _base = _id * (_count / _num_threads);

            for (int i = 0; i < _count / _num_threads; i++)
            {
#if PROFILE_ENQUEUE_TIMES
                
                double _add_begin = this._clock.Time;
#endif
                this._workendpoint.Interface._Add((_base+i).ToString(), string.Empty);
#if PROFILE_ENQUEUE_TIMES
                double _add_end = this._clock.Time;
                this._enqueue_times.Add(_add_begin, _add_end - _add_begin);
#endif
            }

            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {

                this._workendpoint.Interface._Get((_count - 1).ToString());

#if PROFILE_ENQUEUE_TIMES
            double _t2 = this._clock.Time;
#endif





//#if PROFILE_ENQUEUE_TIMES
//            double _start_min = _t1;
//            double _end_max = _t2;
//            this._mycontext.Platform.Logger.Log("Overall time to enqueue: " + (_end_max - _start_min).ToString());
            
//#endif
                this._workendpoint.Interface._Count();
            }
        }

        void _workendpoint_OnConnected()
        {

            Thread[] _threads = new Thread[_num_threads];
            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i] = new Thread(new ThreadStart(this._Work));
            }

            for (int i = 0; i < _num_threads; i++)
            {
                _threads[i].Start();
            }

            

        }

        #endregion
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Queue Latency Statistics

#if PROFILE_ENQUEUE_TIMES
        [QS.Fx.Base.Inspectable]
        public QS._core_e_.Data.IDataSet TimeInFirstQueue
        {
            get
            {
                QS._core_e_.Data.XY[] _xys = new QS._core_e_.Data.XY[_count];
                QS._qss_x_.Experiment_.Component_.EmptyDictionary_ _ed = (QS._qss_x_.Experiment_.Component_.EmptyDictionary_)_workproxy;
                
                QS._qss_e_.Data_.FastStatistics2D_ _sched_samp = _ed.ReplicaSchedSamples;

                for (int i = 0; i < _count; i++)
                {
                    _xys[i] = new QS._core_e_.Data.XY(i,_sched_samp.Samples[i].x - (_enqueue_times.Samples[i].x + _enqueue_times.Samples[i].y));
                }

                return new QS._core_e_.Data.XYSeries(_xys.ToArray());
            }
        }

        [QS.Fx.Base.Inspectable]
        public QS._core_e_.Data.IDataSet TimeInSecondQueue
        {
            get
            {
                QS._core_e_.Data.XY[] _xys = new QS._core_e_.Data.XY[_count];
                QS._qss_x_.Experiment_.Component_.EmptyDictionary_ _ed = (QS._qss_x_.Experiment_.Component_.EmptyDictionary_)_workproxy;

                QS._qss_e_.Data_.FastStatistics2D_ _sched_samp = _ed.ReplicaSchedSamples;
                QS._qss_e_.Data_.FastStatistics2D_ _add_samp = _ed._replica_add_samples[0];
                for (int i = 0; i < _count; i++)
                {
                    _xys[i] = new QS._core_e_.Data.XY(i, _add_samp.Samples[i].x - ( _sched_samp.Samples[i].x + _sched_samp.Samples[i].y));
                }

                return new QS._core_e_.Data.XYSeries(_xys.ToArray());
            }
        }
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        #region Fields
        [QS.Fx.Base.Inspectable]
        private int _done = 0;
        [QS.Fx.Base.Inspectable]
        private int _id = -1;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _start = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _started = 0;
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
#if PROFILE_ENQUEUE_TIMES
        [QS.Fx.Base.Inspectable]
        QS._qss_e_.Data_.FastStatistics2D_ _enqueue_times;
#endif
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
