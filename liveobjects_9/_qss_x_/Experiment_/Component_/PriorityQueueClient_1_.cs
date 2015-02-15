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
using System.Net.Mail;
using System.Net;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("7C91E0378F2E4AA1A14138C0324CFCDB")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class PriorityQueueClient_1_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PriorityQueueClient_1_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("queue", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPriorityQueue_> _queuereference,
            [QS.Fx.Reflection.Parameter("warmup", QS.Fx.Reflection.ParameterClass.Value)] 
            int _warmup,
            [QS.Fx.Reflection.Parameter("batch", QS.Fx.Reflection.ParameterClass.Value)] 
            int _batch,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("epoch", QS.Fx.Reflection.ParameterClass.Value)] 
            int _epoch,
            [QS.Fx.Reflection.Parameter("variability", QS.Fx.Reflection.ParameterClass.Value)] 
            double _variability,
            [QS.Fx.Reflection.Parameter("output", QS.Fx.Reflection.ParameterClass.Value)] 
            string _output,
            [QS.Fx.Reflection.Parameter("email", QS.Fx.Reflection.ParameterClass.Value)] 
            string _email)
        {
            this._mycontext = _mycontext;
            this._warmup = _warmup;
            this._batch = _batch;
            this._count = _count;
            this._epoch = _epoch;
            this._variability = _variability;
            this._output = _output;
            this._email = _email;
            if (this._email != null)
            {
                this._email = this._email.Trim();
                if (this._email.Length == 0)
                    this._email = null;
            }
            this._queuereference = _queuereference;
            this._queueproxy = this._queuereference.Dereference(this._mycontext);
            this._queueendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_,
                    QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_>(this);
            this._queueconnection = this._queueendpoint.Connect(this._queueproxy._Queue);
            this._clock = this._mycontext.Platform.Clock;
            this._cpuusagecounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter_1 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter_2 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusagecounter_3 = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            this._cpuusage = this._cpuusagecounter.NextValue();
            this._cpuusage_1 = this._cpuusagecounter_1.NextValue();
            this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
            this._cpuusage_3 = this._cpuusagecounter_3.NextValue();
            this._mycontext.Platform.Scheduler.Schedule
            (
                new QS.Fx.Base.Event
                (
                    new QS.Fx.Base.ContextCallback
                    (
                        delegate(object _o)
                        {
                            this._mycontext.Platform.Logger.Log("Warming up...");
                            this._queueendpoint.Interface._Warmup(this._warmup);
                            this._mycontext.Platform.Logger.Log("Preparing...");
                            this._random = new Random();
                            this._timestamps = new double[this._count];
                            for (int _i = 0; _i < this._count; _i++)
                                this._timestamps[_i] = (((double)_i) / ((double)this._count)) + (this._random.NextDouble() * this._variability);
                            this._mycontext.Platform.Logger.Log("Initializing...");
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
                            this._cpuusage_1 = this._cpuusagecounter_1.NextValue();
                            this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                            this._cpuusage_3 = this._cpuusagecounter_3.NextValue();
                            this._mycontext.Platform.Logger.Log("Enqueueing...");
                            this._epochok = (this._epoch > 0);
                            this._starttime = this._clock.Time;
                            if (this._batch == 1)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < this._count)
                                {
                                    this._queueendpoint.Interface._Enqueue(this._timestamps[_k]);
                                    _k++;
                                    _e++;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 2)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 1))
                                {
                                    this._queueendpoint.Interface._Enqueue(this._timestamps[_k], this._timestamps[_k + 1]);
                                    _k += 2;
                                    _e += 2;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 4)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 3))
                                {
                                    this._queueendpoint.Interface._Enqueue(this._timestamps[_k], this._timestamps[_k + 1], this._timestamps[_k + 2], this._timestamps[_k + 3]);
                                    _k += 4;
                                    _e += 4;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 8)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 7))
                                {
                                    this._queueendpoint.Interface._Enqueue(
                                        this._timestamps[_k], this._timestamps[_k + 1], this._timestamps[_k + 2], this._timestamps[_k + 3],
                                        this._timestamps[_k + 4], this._timestamps[_k + 5], this._timestamps[_k + 6], this._timestamps[_k + 7]);
                                    _k += 8;
                                    _e += 8;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 16)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 15))
                                {
                                    this._queueendpoint.Interface._Enqueue(
                                        this._timestamps[_k], this._timestamps[_k + 1], this._timestamps[_k + 2], this._timestamps[_k + 3],
                                        this._timestamps[_k + 4], this._timestamps[_k + 5], this._timestamps[_k + 6], this._timestamps[_k + 7],
                                        this._timestamps[_k + 8], this._timestamps[_k + 9], this._timestamps[_k + 10], this._timestamps[_k + 11],
                                        this._timestamps[_k + 12], this._timestamps[_k + 13], this._timestamps[_k + 14], this._timestamps[_k + 15]);
                                    _k += 16;
                                    _e += 16;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 32)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 31))
                                {
                                    this._queueendpoint.Interface._Enqueue(
                                        this._timestamps[_k], this._timestamps[_k + 1], this._timestamps[_k + 2], this._timestamps[_k + 3],
                                        this._timestamps[_k + 4], this._timestamps[_k + 5], this._timestamps[_k + 6], this._timestamps[_k + 7],
                                        this._timestamps[_k + 8], this._timestamps[_k + 9], this._timestamps[_k + 10], this._timestamps[_k + 11],
                                        this._timestamps[_k + 12], this._timestamps[_k + 13], this._timestamps[_k + 14], this._timestamps[_k + 15],
                                        this._timestamps[_k + 16], this._timestamps[_k + 17], this._timestamps[_k + 18], this._timestamps[_k + 19],
                                        this._timestamps[_k + 20], this._timestamps[_k + 21], this._timestamps[_k + 22], this._timestamps[_k + 23],
                                        this._timestamps[_k + 24], this._timestamps[_k + 25], this._timestamps[_k + 26], this._timestamps[_k + 27],
                                        this._timestamps[_k + 28], this._timestamps[_k + 29], this._timestamps[_k + 30], this._timestamps[_k + 31]);
                                    _k += 32;
                                    _e += 32;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else if (this._batch == 64)
                            {
                                int _k = 0;
                                int _e = 0;
                                while (_k < (this._count - 63))
                                {
                                    this._queueendpoint.Interface._Enqueue(
                                        this._timestamps[_k], this._timestamps[_k + 1], this._timestamps[_k + 2], this._timestamps[_k + 3],
                                        this._timestamps[_k + 4], this._timestamps[_k + 5], this._timestamps[_k + 6], this._timestamps[_k + 7],
                                        this._timestamps[_k + 8], this._timestamps[_k + 9], this._timestamps[_k + 10], this._timestamps[_k + 11],
                                        this._timestamps[_k + 12], this._timestamps[_k + 13], this._timestamps[_k + 14], this._timestamps[_k + 15],
                                        this._timestamps[_k + 16], this._timestamps[_k + 17], this._timestamps[_k + 18], this._timestamps[_k + 19],
                                        this._timestamps[_k + 20], this._timestamps[_k + 21], this._timestamps[_k + 22], this._timestamps[_k + 23],
                                        this._timestamps[_k + 24], this._timestamps[_k + 25], this._timestamps[_k + 26], this._timestamps[_k + 27],
                                        this._timestamps[_k + 28], this._timestamps[_k + 29], this._timestamps[_k + 30], this._timestamps[_k + 31],
                                        this._timestamps[_k + 32], this._timestamps[_k + 33], this._timestamps[_k + 34], this._timestamps[_k + 35],
                                        this._timestamps[_k + 36], this._timestamps[_k + 37], this._timestamps[_k + 38], this._timestamps[_k + 39],
                                        this._timestamps[_k + 40], this._timestamps[_k + 41], this._timestamps[_k + 42], this._timestamps[_k + 43],
                                        this._timestamps[_k + 44], this._timestamps[_k + 45], this._timestamps[_k + 46], this._timestamps[_k + 47],
                                        this._timestamps[_k + 48], this._timestamps[_k + 49], this._timestamps[_k + 50], this._timestamps[_k + 51],
                                        this._timestamps[_k + 52], this._timestamps[_k + 53], this._timestamps[_k + 54], this._timestamps[_k + 55],
                                        this._timestamps[_k + 56], this._timestamps[_k + 57], this._timestamps[_k + 58], this._timestamps[_k + 59],
                                        this._timestamps[_k + 60], this._timestamps[_k + 61], this._timestamps[_k + 62], this._timestamps[_k + 63]);
                                    _k += 64;
                                    _e += 64;
                                    if (this._epochok && (_e >= this._epoch))
                                    {
                                        this._queueendpoint.Interface._Dequeue(_e);
                                        _e = 0;
                                    }
                                }
                            }
                            else
                                throw new Exception("Unsupported batch size!");
                            this._queueendpoint.Interface._Done();
                            this._midtime_1 = this._clock.Time;
                            this._cpuusage_1 = this._cpuusagecounter_1.NextValue();
                            this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                            this._mycontext.Platform.Logger.Log("Working...");
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
        private string _output;
        [QS.Fx.Base.Inspectable]
        private string _email;
        [QS.Fx.Base.Inspectable]
        private int _warmup;
        [QS.Fx.Base.Inspectable]
        private bool _epochok;
        [QS.Fx.Base.Inspectable]
        private int _epoch;
        [QS.Fx.Base.Inspectable]
        private int _batch;
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
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private double _starttime;
        [QS.Fx.Base.Inspectable]
        private double _stoptime;
        [QS.Fx.Base.Inspectable]
        private double _midtime_1;
        [QS.Fx.Base.Inspectable]
        private double _midtime_2;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private double _duration_1;
        [QS.Fx.Base.Inspectable]
        private double _duration_2;
        [QS.Fx.Base.Inspectable]
        private double _duration_3;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter_1;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter_2;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _cpuusagecounter_3;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage_1;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage_2;
        [QS.Fx.Base.Inspectable]
        private double _cpuusage_3;
        [QS.Fx.Base.Inspectable]
        private double _processingtime;
        [QS.Fx.Base.Inspectable]
        private Random _random;
        [QS.Fx.Base.Inspectable]
        private double[] _timestamps;
        [QS.Fx.Base.Inspectable]
        private int _importing;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPriorityQueueClient_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Importing()
        {
            if (Interlocked.Exchange(ref this._importing, 1) == 0)
            {
                this._midtime_2 = this._clock.Time;
                this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                this._cpuusage_3 = this._cpuusagecounter_3.NextValue();
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_._Done()
        {
            this._stoptime = this._clock.Time;
            if (Interlocked.Exchange(ref this._importing, 1) == 0)
            {
                this._midtime_2 = this._stoptime;
                this._cpuusage_2 = this._cpuusagecounter_2.NextValue();
                this._cpuusage_3 = this._cpuusagecounter_3.NextValue();
            }
            this._cpuusage_3 = this._cpuusagecounter_3.NextValue();
            this._cpuusage = this._cpuusagecounter.NextValue();
            this._duration = this._stoptime - this._starttime;
            this._duration_1 = this._midtime_1 - this._starttime;
            this._duration_2 = this._midtime_2 - this._midtime_1;
            this._duration_3 = this._stoptime - this._midtime_2;
            this._processingtime = this._duration / ((double)this._count);
#if SHUTDOWN_EVERYTHING_WHEN_COMPLETED
            string _myline =
                (QS._qss_x_.Object_.Context_._HasOptimizations ? "+" : "-") + "\t" +
                this._variability.ToString() + "\t" +
                this._warmup.ToString() + "\t" +
                this._batch.ToString() + "\t" +
                this._count.ToString() + "\t" +
                this._epoch.ToString() + "\t" +
                QS.Fx.Object.Runtime.NumberOfReplicas.ToString() + "\t" +
                QS.Fx.Object.Runtime.CommandLine + "\t" +
                this._duration.ToString() + "\t" +
                this._cpuusage.ToString() + "\t" +
                (this._processingtime * 1000000).ToString() + "\t\t\t" +
                (IntPtr.Size == 4 ? "x86" : "x64") + "\t" +
                DateTime.Now.ToString() + "\t" +
                this._duration_1.ToString() + "\t" +
                this._duration_2.ToString() + "\t" +
                this._duration_3.ToString() + "\t" +
                this._cpuusage_1.ToString() + "\t" +
                this._cpuusage_2.ToString() + "\t" +
                this._cpuusage_3.ToString();
            using (StreamWriter _writer = new StreamWriter(this._output, true))
            {
                _writer.WriteLine(_myline);
                _writer.Flush();
            }
            //if (this._email != null)
            //{
            //    try
            //    {
            //        SmtpClient _smtpclient = new SmtpClient("mushroom1.straydog.cs.cornell.edu");
            //        _smtpclient.Credentials = new NetworkCredential("quicksilver", "", "straydog");
            //        MailMessage _mailmessage = new MailMessage(
            //            new MailAddress("quicksilver@straydog.cs.cornell.edu", "QuickSilver"),
            //            new MailAddress(this._email, this._email));
            //        _mailmessage.Subject = "Experiment Completed";
            //        _mailmessage.Body = _myline;
            //        _smtpclient.Send(_mailmessage);
            //    }
            //    catch (Exception _exc)
            //    {
            //    }
            //}
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
                            this._mycontext.Platform.Logger.Log("Completion Time (s): " + this._duration.ToString());
                            this._mycontext.Platform.Logger.Log("Processing Time (μs): " + (this._processingtime * ((double)1000000)).ToString());
                            this._mycontext.Platform.Logger.Log("CPU Usage (%): " + this._cpuusage.ToString());
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
