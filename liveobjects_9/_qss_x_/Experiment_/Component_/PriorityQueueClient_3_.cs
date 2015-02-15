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
    [QS.Fx.Reflection.ComponentClass("E61E6D32875B4939917143E104FBFDD1")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class PriorityQueueClient_3_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PriorityQueueClient_3_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("queue", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPriorityQueue_> _queuereference,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)] 
            int _concurrency,
            [QS.Fx.Reflection.Parameter("locking", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _locking,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("variability", QS.Fx.Reflection.ParameterClass.Value)] 
            double _variability)
        {
            this._mycontext = _mycontext;
            this._concurrency = _concurrency;
            this._locking = _locking;
            this._count = _count;
            this._variability = _variability;
            this._queuereference = _queuereference;
            this._queueproxy = this._queuereference.Dereference(this._mycontext);
            this._queueendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_,
                    QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_>(this);
            this._queueconnection = this._queueendpoint.Connect(this._queueproxy._Queue);
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._threads = new Thread[this._concurrency];
            this._benchmarksinitializing = this._concurrency;
            for (int _i = 0; _i < this._concurrency; _i++)
            {
                this._threads[_i] = new Thread(new ParameterizedThreadStart(this._ThreadCallback));
                this._threads[_i].Start(_i);
            }
            this._mycontext.Platform.Scheduler.Schedule
            (
                new QS.Fx.Base.Event
                (
                    new QS.Fx.Base.ContextCallback
                    (
                        delegate(object _o)
                        {
                            this._mycontext.Platform.Logger.Log("Initializing");
                            this._benchmarksinitialized.WaitOne();
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
                            this._benchmarksworking = this._concurrency;
                            this._cpuusagecounter.NextValue();
                            this._starttime = this._clock.Time;
                            this._benchmarkscanbegin.Set();
                            this._benchmarkscomplete.WaitOne();
                            this._queueendpoint.Interface._Done();                            
                        }
                    ),
                    null
                )
            );
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
        private QS._qss_x_.Experiment_.Object_.IPriorityQueue_ _queueproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPriorityQueue_,
                QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_> _queueendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _queueconnection;
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private int _benchmarksinitializing;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _benchmarksinitialized = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _benchmarkscanbegin = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _benchmarksworking;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _benchmarkscomplete = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double _starttime;
        [QS.Fx.Base.Inspectable]
        private double _stoptime;
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

        #region _ThreadCallback

        private void _ThreadCallback(object _o)
        {
            int _index = (int)_o;
            Random _random = new Random();
            double[] _timestamps = new double[this._count];
            for (int _i = 0; _i < this._count; _i++)
                _timestamps[_i] = (((double)_i) / ((double) this._count)) + (_random.NextDouble() * this._variability);
            if (Interlocked.Decrement(ref this._benchmarksinitializing) == 0)
                this._benchmarksinitialized.Set();
            QS._qss_x_.Experiment_.Interface_.IPriorityQueue_ _queueinterface = this._queueendpoint.Interface;
            this._benchmarkscanbegin.WaitOne();
            if (this._locking)
            {
                for (int _i = 0; _i < this._count; _i++)
                {
                    lock (this)
                    {
                        _queueinterface._Enqueue(_timestamps[_i]);
                    }
                }
            }
            else
            {
                for (int _i = 0; _i < this._count; _i++)
                    _queueinterface._Enqueue(_timestamps[_i]);
            }
            if (Interlocked.Decrement(ref this._benchmarksworking) == 0)
                this._benchmarkscomplete.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPriorityQueueClient_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Importing()
        {
        }

        void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Done()
        {
            this._stoptime = this._clock.Time;
            this._duration = this._stoptime - this._starttime;
            this._cpuusage = this._cpuusagecounter.NextValue();
            this._processingtime = this._duration / ((double)this._count);
            this._mycontext.Platform.Scheduler.Schedule
            (
                new QS.Fx.Base.Event
                (
                    new QS.Fx.Base.ContextCallback
                    (
                        delegate(object _o)
                        {
                            this._mycontext.Platform.Logger.Log("Completion Time (s): " + this._duration.ToString());
                            this._mycontext.Platform.Logger.Log("Processing Time (μs): " + (this._processingtime * ((double)1000000)).ToString());
                            this._mycontext.Platform.Logger.Log("CPU Usage (%): " + this._cpuusage.ToString());
                        }
                    ),
                    null
                )
            );
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
