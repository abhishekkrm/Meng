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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("B710D140823446998EA2821A0B34AE4F")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class PriorityQueueClient_2_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PriorityQueueClient_2_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("queue", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPriorityQueue_> _queuereference,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)] 
            int _concurrency,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("variability", QS.Fx.Reflection.ParameterClass.Value)] 
            double _variability)
        {
            this._mycontext = _mycontext;
            this._concurrency = _concurrency;
            this._count = _count;
            this._variability = _variability;
            this._queuereference = _queuereference;
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._threads = new Thread[this._concurrency];
            this._benchmarksinitializing = this._concurrency;
            this._queueclients = new Client_[this._concurrency];
            this._benchmarksimported = new int[this._concurrency];
            this._queueproxies = new QS._qss_x_.Experiment_.Object_.IPriorityQueue_[this._concurrency];
            this._queueendpoints = new QS.Fx.Endpoint.Internal.IDualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_, QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_>[this._concurrency];
            this._queueconnections = new QS.Fx.Endpoint.IConnection[this._concurrency];
            this._starttimes = new double[this._concurrency];
            this._stoptimes = new double[this._concurrency];
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                delegate(object _o)
                {
                    this._mycontext.Platform.Logger.Log("Initializing");
                })));
            for (int _i = 0; _i < this._concurrency; _i++)
            {
                this._threads[_i] = new Thread(new ParameterizedThreadStart(this._ThreadCallback));
                this._threads[_i].Start(_i);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _concurrency;
        [QS.Fx.Base.Inspectable]
        private bool _locking;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private double _variability;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPriorityQueue_> _queuereference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IPriorityQueue_[] _queueproxies;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPriorityQueue_,
                QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_>[] _queueendpoints;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection[] _queueconnections;
        [QS.Fx.Base.Inspectable]
        private Client_[] _queueclients;
        [QS.Fx.Base.Inspectable]
        private int[] _benchmarksimported;
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private int _benchmarksinitializing;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _benchmarkscanbegin = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _benchmarksworking;
        [QS.Fx.Base.Inspectable]
        private int _benchmarksimporting;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double[] _starttimes;
        [QS.Fx.Base.Inspectable]
        private double[] _stoptimes;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage;
        [QS.Fx.Base.Inspectable]
        private double _processingtime;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region Class Client_

        private sealed class Client_ : QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_
        {
            #region Constructor

            public Client_(PriorityQueueClient_2_ _owner, int _index)
            {
                this._owner = _owner;
                this._index = _index;
            }

            #endregion

            #region Fields

            private PriorityQueueClient_2_ _owner;
            private int _index;

            #endregion

            #region IPriorityQueueClient_ Members

            [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
            void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Importing()
            {
            }


            void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Done()
            {
                this._owner._Done(this._index);
            }

            #endregion
        }

        #endregion 

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _ThreadCallback

        private void _ThreadCallback(object _o)
        {
            int _index = (int)_o;
            this._queueclients[_index] = new Client_(this, _index);
            this._queueproxies[_index] = this._queuereference.Dereference(this._mycontext);
            this._queueendpoints[_index] = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_,
                    QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_>(this._queueclients[_index]);
            this._queueconnections[_index] = this._queueendpoints[_index].Connect(this._queueproxies[_index]._Queue);
            QS._qss_x_.Experiment_.Interface_.IPriorityQueue_ _queueinterface = this._queueendpoints[_index].Interface;
            Random _random = new Random();
            double[] _timestamps = new double[this._count];
            for (int _i = 0; _i < this._count; _i++)
                _timestamps[_i] = (((double)_i) / ((double)this._count)) + (_random.NextDouble() * this._variability);
            if (Interlocked.Decrement(ref this._benchmarksinitializing) == 0)
            {
                this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                    delegate(object _oo)
                    {
                        double _t1 = this._clock.Time;
                        double _t2;
                        this._cpuusage = this._cpuusagecounter.NextValue();
                        do
                        {
                            Thread.Sleep(100);
                            _t2 = this._clock.Time;
                            this._cpuusage = this._cpuusagecounter.NextValue();
                            if (this._cpuusage > 10)
                                _t1 = _t2;
                        }
                        while ((_t2 - _t1) < 3);
                        this._mycontext.Platform.Logger.Log("Initialized");
                        this._cpuusagecounter.NextValue();
                        this._benchmarksworking = this._concurrency;
                        this._benchmarksimporting = this._concurrency;
                        this._benchmarkscanbegin.Set();
                    })));
            }
            this._benchmarkscanbegin.WaitOne();
            this._starttimes[_index] = this._clock.Time;
            for (int _i = 0; _i < this._count; _i++)
                _queueinterface._Enqueue(_timestamps[_i]);
            _queueinterface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    


        #region _Done

        private void _Done(int _index)
        {
            if (Interlocked.Exchange(ref this._benchmarksimported[_index], 1) == 0)
            {
                this._stoptimes[_index] = this._clock.Time;
                if (Interlocked.Decrement(ref this._benchmarksworking) == 0)
                {
                    this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                        delegate(object _oo)
                        {
                            this._cpuusage = this._cpuusagecounter.NextValue();
                            this._duration = 0;
                            for (int _j = 0; _j < this._concurrency; _j++)
                                this._duration += (this._stoptimes[_j] - this._starttimes[_j]);
                            this._duration = this._duration / ((double)this._concurrency);
                            this._processingtime = this._duration / ((double)this._count);
                            this._mycontext.Platform.Logger.Log("Duration (s): " + this._duration.ToString());
                            this._mycontext.Platform.Logger.Log("Processing (μs): " + (this._processingtime * ((double)1000000)).ToString());
                            this._mycontext.Platform.Logger.Log("Utilization (%): " + this._cpuusage.ToString());
                        }),
                        null));
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
