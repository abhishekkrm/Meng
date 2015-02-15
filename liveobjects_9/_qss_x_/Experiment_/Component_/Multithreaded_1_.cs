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
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("4553505C77614A0D919CC1688E5C2152")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class Multithreaded_1_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Multithreaded_1_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)]
            int _concurrency,
            [QS.Fx.Reflection.Parameter("memsize", QS.Fx.Reflection.ParameterClass.Value)]
            int _memsize,
            [QS.Fx.Reflection.Parameter("iterations", QS.Fx.Reflection.ParameterClass.Value)]
            int _iterations,
            [QS.Fx.Reflection.Parameter("duration", QS.Fx.Reflection.ParameterClass.Value)]
            double _duration,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
        {
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._concurrency = _concurrency;
            this._memsize = _memsize;
            this._iterations = _iterations;
            this._duration = _duration;
            this._count = _count;
            this._threads = new Thread[this._concurrency];
            for (int _i = 0; _i < this._concurrency; _i++)
                (this._threads[_i] = new Thread(new ThreadStart(this._Work))).Start();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        private int _concurrency;
        [QS.Fx.Base.Inspectable]
        private int _memsize;
        [QS.Fx.Base.Inspectable]
        private int _iterations;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private Thread[] _threads;
        [QS.Fx.Base.Inspectable]
        private int _numstarted;
        [QS.Fx.Base.Inspectable]
        private int _numstopped;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _started = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _stopped = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private double _starttime;
        [QS.Fx.Base.Inspectable]
        private double _stoptime;
        [QS.Fx.Base.Inspectable]
        private PerformanceCounter _totalcpuusage_counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _totalcpuusage_samples = new QS._qss_c_.Statistics_.Samples2D();
        [QS.Fx.Base.Inspectable]
        private double _totalcpuusage;
        [QS.Fx.Base.Inspectable]
        private int _completed;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Work

        private void _Work()
        {
            byte[] _data0 = new byte[1000];
            for (int _i = 0; _i < this._count; _i++)
            {
                if (Interlocked.Increment(ref this._numstarted) == this._concurrency)
                {
                    this._stopped.Reset();
                    this._numstopped = 0;
                    if (_i == 0)
                        MessageBox.Show("Are you ready to start the experiment?", "Ready?", 
                            MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    this._totalcpuusage_counter.NextValue();
                    this._starttime = this._clock.Time;
                    this._stoptime = this._starttime + this._duration;
                    this._started.Set();
                }
                else
                    this._started.WaitOne();
                double _time;
                do
                {
                    byte[] _data;
                    if (this._memsize > 0)
                        _data = new byte[this._memsize];
                    else
                        _data = _data0;
                    _data[0] = (byte) (_i % 256);
                    for (int _j = 0; _j < this._count; _j++)
                        _data0[_j % _data0.Length] = (byte) (_j % 256);
                    _time = this._clock.Time;
                }
                while (_time < this._stoptime);
                if (Interlocked.Increment(ref this._numstopped) == this._concurrency)
                {
                    this._totalcpuusage_samples.Add(this._clock.Time, this._totalcpuusage_counter.NextValue());
                    this._started.Reset();
                    this._numstarted = 0;
                    this._stopped.Set();
                }
                else
                    this._stopped.WaitOne();
            }
            if (Interlocked.Exchange(ref this._completed, 1) == 0)
            {
                for (int _i = 0; _i < this._totalcpuusage_samples.Samples.Length; _i++)
                    this._totalcpuusage += this._totalcpuusage_samples.Samples[_i].y;
                this._totalcpuusage /= (double)this._totalcpuusage_samples.Samples.Length;
                this._mycontext.Platform.Logger.Log("CPU usage : " + this._totalcpuusage.ToString());
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
