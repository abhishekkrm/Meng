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
    [QS.Fx.Reflection.ComponentClass("564D5B472DB1415998607666314C114F")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class MembershipClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IMembershipClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MembershipClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("membership", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMembership_> _membershipreference,
            [QS.Fx.Reflection.Parameter("nnodes", QS.Fx.Reflection.ParameterClass.Value)] 
            int _nnodes,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("batch", QS.Fx.Reflection.ParameterClass.Value)] 
            int _batch,
            [QS.Fx.Reflection.Parameter("ready", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ready)
        {
            this._mycontext = _mycontext;
            this._nnodes = _nnodes;
            this._count = _count;
            this._batch = _batch;
            this._ready = _ready;
            this._membershipreference = _membershipreference;
            this._membershipproxy = this._membershipreference.Dereference(this._mycontext);
            this._membershipendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IMembership_,
                    QS._qss_x_.Experiment_.Interface_.IMembershipClient_>(this);
            this._membershipconnection = this._membershipendpoint.Connect(this._membershipproxy._Membership);
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter_1 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter_2 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._mycontext.Platform.Scheduler.Schedule
            (
                new QS.Fx.Base.Event
                (
                    new QS.Fx.Base.ContextCallback
                    (
                        delegate(object _o)
                        {
                            this._mycontext.Platform.Logger.Log("Initializing");
                            this._ids = new string[this._nnodes];
                            this._online = new bool[this._nnodes];
                            this._indexes = new int[this._batch];
                            for (int _k = 0; _k < this._nnodes; _k++)
                            {
                                const int _n = 64;
                                char[] _c = new char[_n];
                                for (int _kk = 0; _kk < _n; _kk++)
                                    _c[_kk] = (char)(((int)'a') + this._random.Next(26));
                                this._ids[_k] = new string(_c);
                                this._online[_k] = false;
                            }
                            for (int _k = 0; _k < this._batch; _k++)
                                this._indexes[_k] = this._random.Next(this._nnodes);
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
                            this._cpuusagecounter_1.NextValue();
                            this._cpuusagecounter_2.NextValue();
                            this._cpuusagecounter.NextValue();
                            this._starttime = this._clock.Time;
                            for (int _i = 0; _i < _count; _i++)
                            {
                                int _index = this._indexes[_i % this._batch];
                                if ((_index == 0) && (_i > 0))
                                    this._membershipendpoint.Interface._Done(false);
                                string _id = this._ids[_index];
                                bool _online = !this._online[_index];
                                this._online[_index] = _online;
                                this._membershipendpoint.Interface._Update(_id, (double) _i, _online);
                            }
                            this._membershipendpoint.Interface._Done(true);
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
        private int _nnodes;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private int _batch;
        [QS.Fx.Base.Inspectable]
        private double _ready;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMembership_> _membershipreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IMembership_ _membershipproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IMembership_,
                QS._qss_x_.Experiment_.Interface_.IMembershipClient_> _membershipendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _membershipconnection;
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
        [QS.Fx.Base.Inspectable]
        private string[] _ids;
        [QS.Fx.Base.Inspectable]
        private bool[] _online;
        [QS.Fx.Base.Inspectable]
        private int[] _indexes;
        [QS.Fx.Base.Inspectable]
        private Random _random = new Random();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMembershipClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IMembershipClient_._Done(bool _completed)
        {
            if (_completed)
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
