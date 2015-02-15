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
    [QS.Fx.Reflection.ComponentClass("37A4A20AE4684B3E8C0F50C2EF838314")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class BenchmarkClient_3_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public BenchmarkClient_3_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("benchmark", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IBenchmark_> _benchmarkreference,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)] 
            int _concurrency,
            [QS.Fx.Reflection.Parameter("locking", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _locking,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("ready", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ready,
            [QS.Fx.Reflection.Parameter("warmup", QS.Fx.Reflection.ParameterClass.Value)] 
            int _warmup)
        {
            this._mycontext = _mycontext;
            this._concurrency = _concurrency;
            this._locking = _locking;
            this._count = _count;
            this._ready = _ready;
            this._warmup = _warmup;
            this._benchmarkreference = _benchmarkreference;
            this._benchmarkproxy = this._benchmarkreference.Dereference(this._mycontext);
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmark_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_>(this);
            this._benchmarkconnection = this._benchmarkendpoint.Connect(this._benchmarkproxy._Benchmark);
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter_1 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter_2 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
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
                            if (this._ready > 0)
                            {
                                this._mycontext.Platform.Logger.Log("Initializing");
                                for (int _i = 0; _i < this._warmup; _i++)
                                    this._benchmarkendpoint.Interface._Work();
                                this._benchmarksinitialized.WaitOne();
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
                            this._benchmarksworking = this._concurrency;
                            this._cpuusagecounter_1.NextValue();
                            this._cpuusagecounter_2.NextValue();
                            this._cpuusagecounter.NextValue();
                            this._starttime = this._clock.Time;
                            this._benchmarkscanbegin.Set();
                            this._benchmarkscomplete.WaitOne();
                            this._benchmarkendpoint.Interface._Done();
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
        private double _ready;
        [QS.Fx.Base.Inspectable]
        private int _warmup;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IBenchmark_> _benchmarkreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IBenchmark_ _benchmarkproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IBenchmark_,
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_> _benchmarkendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _benchmarkconnection;
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
        private double _stoptime_0;
        [QS.Fx.Base.Inspectable]
        private double _stoptime;
        [QS.Fx.Base.Inspectable]
        private bool _imported;
        [QS.Fx.Base.Inspectable]
        private int _importedok;
        [QS.Fx.Base.Inspectable]
        private double _duration_1;
        [QS.Fx.Base.Inspectable]
        private double _duration_2;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter_1;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter_2;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage_1;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage_2;
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
            if (Interlocked.Decrement(ref this._benchmarksinitializing) == 0)
                this._benchmarksinitialized.Set();
            QS._qss_x_.Experiment_.Interface_.IBenchmark_ _benchmarkinterface = this._benchmarkendpoint.Interface;
            this._benchmarkscanbegin.WaitOne();
            if (this._locking)
            {
                for (int _i = 0; _i < this._count; _i++)
                {
                    lock (this)
                    {
                        _benchmarkinterface._Work();
                    }
                }
            }
            else
            {
                for (int _i = 0; _i < this._count; _i++)
                    _benchmarkinterface._Work();
            }
            if (Interlocked.Decrement(ref this._benchmarksworking) == 0)
                this._benchmarkscomplete.Set();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IBenchmarkClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_._Done(bool _imported)
        {
            if (_imported)
            {
                this._stoptime = this._clock.Time;
                this._duration = this._stoptime - this._starttime;
                this._cpuusage = this._cpuusagecounter.NextValue();
                this._processingtime = this._duration / ((double)this._count);
                if (this._imported)
                {
                    this._duration_2 = this._stoptime - this._stoptime_0;
                    this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                }
                this._mycontext.Platform.Scheduler.Schedule
                (
                    new QS.Fx.Base.Event
                    (
                        new QS.Fx.Base.ContextCallback
                        (
                            delegate(object _o)
                            {
                                this._mycontext.Platform.Logger.Log("Duration (s): " + this._duration.ToString());
                                this._mycontext.Platform.Logger.Log("Processing (μs): " + (this._processingtime * ((double)1000000)).ToString());
                                this._mycontext.Platform.Logger.Log("Utilization (%): " + this._cpuusage.ToString());
                                if (this._imported)
                                {
                                    this._mycontext.Platform.Logger.Log("Duration #1 (s): " + this._duration_1.ToString());
                                    this._mycontext.Platform.Logger.Log("Duration #2 (s): " + this._duration_2.ToString());
                                    this._mycontext.Platform.Logger.Log("Utilization #1 (%): " + this._cpuusage_1.ToString());
                                    this._mycontext.Platform.Logger.Log("Utilization #2 (%): " + this._cpuusage_2.ToString());
                                }
                            }
                        ),
                        null
                    )
                );
            }
            else
            {
                if (!this._imported)
                {
                    this._imported = true;
                    if (Interlocked.Exchange(ref this._importedok, 1) == 0)
                    {
                        this._stoptime_0 = this._clock.Time;
                        this._duration_1 = this._stoptime_0 - this._starttime;
                        this._cpuusage_1 = this._cpuusagecounter_1.NextValue();
                        this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
