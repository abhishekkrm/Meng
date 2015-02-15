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

#define SHUTDOWN_EVERYTHING_WHEN_COMPLETED

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("4F37872E73CE497CA8D40FC61FD46CF4")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class SerializerClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.ISerializerClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public SerializerClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("serializer", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.ISerializer_> _serializerreference,
            [QS.Fx.Reflection.Parameter("items", QS.Fx.Reflection.ParameterClass.Value)] 
            int _items,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("ready", QS.Fx.Reflection.ParameterClass.Value)] 
            double _ready,
            [QS.Fx.Reflection.Parameter("verbose", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _verbose,
            [QS.Fx.Reflection.Parameter("output", QS.Fx.Reflection.ParameterClass.Value)] 
            string _output)
        {
            this._mycontext = _mycontext;
            this._items = _items;
            this._count = _count;
            this._ready = _ready;
            this._verbose = _verbose;
            this._output = _output;
            this._serializerreference = _serializerreference;
            this._serializerproxy = this._serializerreference.Dereference(this._mycontext);
            this._serializerendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.ISerializer_,
                    QS._qss_x_.Experiment_.Interface_.ISerializerClient_>(this);
            this._serializerconnection = this._serializerendpoint.Connect(this._serializerproxy._Serializer);
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._myobject = new Dictionary<int, string>();
            for (int _i = 0; _i < this._items; _i++)
                this._myobject.Add(_i, _i.ToString());
            this._mycontext.Platform.Scheduler.Schedule
            (
                new QS.Fx.Base.Event
                (
                    new QS.Fx.Base.ContextCallback
                    (
                        delegate(object _o)
                        {
                            this._mycontext.Platform.Logger.Log("Initializing");
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
                            ThreadPool.QueueUserWorkItem(new WaitCallback(
                                delegate (object _oo)
                                {
                                    this._cpuusagecounter.NextValue();
                                    this._starttime = this._clock.Time;
                                    for (int _i = 0; _i < _count; _i++)
                                        this._serializerendpoint.Interface._Serialize(this._myobject);
                                    this._serializerendpoint.Interface._Done();
                                }));
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
        private int _items;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private double _ready;
        [QS.Fx.Base.Inspectable]
        private bool _verbose;
        [QS.Fx.Base.Inspectable]
        private string _output;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.ISerializer_> _serializerreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.ISerializer_ _serializerproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.ISerializer_,
                QS._qss_x_.Experiment_.Interface_.ISerializerClient_> _serializerendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _serializerconnection;
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
        [QS.Fx.Base.Inspectable]
        private IDictionary<int, string> _myobject;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializerClient_ Members

        void QS._qss_x_.Experiment_.Interface_.ISerializerClient_._Serialized(int _seqno, ArraySegment<byte> _segment)
        {
            if (this._verbose)
            {
                this._mycontext.Platform.Scheduler.Schedule(new QS.Fx.Base.Event(new QS.Fx.Base.ContextCallback(
                    delegate(object _o) { this._mycontext.Platform.Logger.Log("Serialized : " + _seqno.ToString()); })));
            }
        }

        void QS._qss_x_.Experiment_.Interface_.ISerializerClient_._Done()
        {
            this._stoptime = this._clock.Time;
            this._duration = this._stoptime - this._starttime;
            this._cpuusage = this._cpuusagecounter.NextValue();
            this._processingtime = this._duration / ((double)this._count);
#if SHUTDOWN_EVERYTHING_WHEN_COMPLETED
            string _myline =
                (QS._qss_x_.Object_.Context_._HasOptimizations ? "+" : "-") + "\t" +
                this._items.ToString() + "\t" +
                this._count.ToString() + "\t" +
                QS.Fx.Object.Runtime.NumberOfReplicas.ToString() + "\t" +
                QS.Fx.Object.Runtime.CommandLine + "\t" +
                this._duration.ToString() + "\t" +
                this._cpuusage.ToString() + "\t" +
                this._processingtime.ToString() + "\t\t\t" +
                (IntPtr.Size == 4 ? "x86" : "x64") + "\t" +
                DateTime.Now.ToString();
            using (StreamWriter _writer = new StreamWriter(this._output, true))
            {
                _writer.WriteLine(_myline);
                _writer.Flush();
            }
            Process.GetCurrentProcess().Kill();
#else


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
                        }
                    ),
                    null
                )
            );
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
