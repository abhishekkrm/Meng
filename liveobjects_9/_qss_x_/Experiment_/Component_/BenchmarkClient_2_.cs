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
    [QS.Fx.Reflection.ComponentClass("44C6825475E449D699D0D3E92C3261F1")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class BenchmarkClient_2_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public BenchmarkClient_2_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("benchmark", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IBenchmark_> _benchmarkreference,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)] 
            int _concurrency,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            long _count,
            [QS.Fx.Reflection.Parameter("ready", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ready,
            [QS.Fx.Reflection.Parameter("warmup", QS.Fx.Reflection.ParameterClass.Value)] 
            int _warmup)
        {
            this._mycontext = _mycontext;
            this._concurrency = _concurrency;
            this._count = _count;
            this._ready = _ready;
            this._warmup = _warmup;
            this._benchmarkreference = _benchmarkreference;
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._threads = new Thread[this._concurrency];
            this._benchmarksinitializing = this._concurrency;
            this._benchmarkclients = new Client_[this._concurrency];
            this._benchmarksimported = new int[this._concurrency];
            this._benchmarkproxies = new QS._qss_x_.Experiment_.Object_.IBenchmark_[this._concurrency];
            this._benchmarkendpoints = new QS.Fx.Endpoint.Internal.IDualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmark_,QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_>[this._concurrency];
            this._benchmarkconnections = new QS.Fx.Endpoint.IConnection[this._concurrency];
            this._starttimes = new double[this._concurrency];
            this._stoptimes = new double[this._concurrency];
            for (int _i = 0; _i < this._concurrency; _i++)
            {
                this._threads[_i] = new Thread(new ParameterizedThreadStart(this._ThreadCallback));
                this._threads[_i].Start(_i);
            }
            this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                delegate(object _o)
                {
                    this._mycontext.Platform.Logger.Log("Initializing");
                })));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _concurrency;
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private long _count;
        [QS.Fx.Base.Inspectable]
        private double _ready;
        [QS.Fx.Base.Inspectable]
        private int _warmup;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IBenchmark_> _benchmarkreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IBenchmark_[] _benchmarkproxies;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IBenchmark_,
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_>[] _benchmarkendpoints;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection[] _benchmarkconnections;
        [QS.Fx.Base.Inspectable]
        private Client_[] _benchmarkclients;
        [QS.Fx.Base.Inspectable]
        private int _benchmarksinitializing;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _benchmarkscanbegin = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private int _benchmarksworking;
        [QS.Fx.Base.Inspectable]
        private int[] _benchmarksimported;
        [QS.Fx.Base.Inspectable]
        private int _benchmarksimporting;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double[] _starttimes;
        [QS.Fx.Base.Inspectable]
        private double[] _stoptimes;
        [QS.Fx.Base.Inspectable]
        private bool _imported;
        [QS.Fx.Base.Inspectable]
        private int _importedok;
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

        private sealed class Client_ : QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_
        {
            #region Constructor

            public Client_(BenchmarkClient_2_ _owner, int _index)
            {
                this._owner = _owner;
                this._index = _index;
            }

            #endregion

            #region Fields

            private BenchmarkClient_2_ _owner;
            private int _index;

            #endregion

            #region IBenchmarkClient_ Members

            void QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_._Done(bool _imported)
            {
                this._owner._Done(this._index, _imported);
            }

            #endregion
        }

        #endregion 

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _ThreadCallback

        private void _ThreadCallback(object _o)
        {
            int _index = (int) _o;
            this._benchmarkclients[_index] = new Client_(this, _index);
            this._benchmarkproxies[_index] = this._benchmarkreference.Dereference(this._mycontext);
            this._benchmarkendpoints[_index] = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmark_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_>(this._benchmarkclients[_index]);
            this._benchmarkconnections[_index] = this._benchmarkendpoints[_index].Connect(this._benchmarkproxies[_index]._Benchmark);
            QS._qss_x_.Experiment_.Interface_.IBenchmark_ _benchmarkinterface = this._benchmarkendpoints[_index].Interface;
            for (int _i = 0; _i < this._warmup; _i++)
                _benchmarkinterface._Work();
            if (Interlocked.Decrement(ref this._benchmarksinitializing) == 0)
            {
                this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                    delegate(object _oo)
                    {
                        if (this._ready > 0)
                        {
                            double _t1 = this._clock.Time;
                            double _t2;
                            this._cpuusage = this._cpuusagecounter.NextValue();
                            do
                            {
                                Thread.Sleep(100);
                                _t2 = this._clock.Time;
                                this._cpuusage = this._cpuusagecounter.NextValue();
                                if (this._cpuusage > this._ready)
                                    _t1 = _t2;
                            }
                            while ((_t2 - _t1) < 3);
                        }
                        this._mycontext.Platform.Logger.Log("Initialized");
                        this._cpuusagecounter.NextValue();
                        this._benchmarksworking = this._concurrency;
                        this._benchmarksimporting = this._concurrency;
                        this._benchmarkscanbegin.Set();
                    })));
            }
            this._benchmarkscanbegin.WaitOne();
            this._starttimes[_index] = this._clock.Time;
            for (long _i = 0; _i < this._count; _i++)
                _benchmarkinterface._Work();
            _benchmarkinterface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Done

        private void _Done(int _index, bool _imported)
        {
            if (_imported)
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
                                this._processingtime = this._duration / (this._count);
                                this._mycontext.Platform.Logger.Log("Duration (s): " + this._duration.ToString());
                                this._mycontext.Platform.Logger.Log("Processing (μs): " + (this._processingtime * ((double)1000000)).ToString());
                                this._mycontext.Platform.Logger.Log("Utilization (%): " + this._cpuusage.ToString());
                            }),
                            null));
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
