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

//#define PERIODICALLY_CHECK_THREADSTATE

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Timers;
using System.Diagnostics;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("02ACFD5E6D2F46A7BD97FC58FFA53C2E")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class Multithreaded_2_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Multithreaded_2_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)]
            int _concurrency,
            [QS.Fx.Reflection.Parameter("itemsize", QS.Fx.Reflection.ParameterClass.Value)]
            int _itemsize,
            [QS.Fx.Reflection.Parameter("numitems", QS.Fx.Reflection.ParameterClass.Value)]
            int _numitems,
            [QS.Fx.Reflection.Parameter("iterations", QS.Fx.Reflection.ParameterClass.Value)]
            int _iterations,
            [QS.Fx.Reflection.Parameter("duration", QS.Fx.Reflection.ParameterClass.Value)]
            double _duration,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
        {

#if PERIODICALLY_CHECK_THREADSTATE
            this._threadstate_outpath = @"C:\Users\chuck\Desktop\threadstate.txt";

#endif
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._concurrency = _concurrency;
            this._itemsize = _itemsize;
            this._numitems = _numitems;
            this._iterations = _iterations;
            this._duration = _duration;
            this._count = _count;
            this._threads = new Thread[this._concurrency];

            //ProcessThreadCollection _ptc = Process.GetCurrentProcess().Threads;
            //int _sc = _ptc.Count;
            //IList<int> _l = new List<int>();
            //for (int _j = 0; _j < _sc; _j++)
            //{
            //    _l.Add(_ptc[_j].Id);
            //}

            for (int _i = 0; _i < this._concurrency; _i++)
            {
                (this._threads[_i] = new Thread(new ThreadStart(this._Work))).Start();

            }

            //_ptc = Process.GetCurrentProcess().Threads;
            //_sc = _ptc.Count;
            //IList<int> _l2 = new List<int>();
            //for (int _j = 0; _j < _sc; _j++)
            //{
            //    _l2.Add(_ptc[_j].Id);
            //}


#if PERIODICALLY_CHECK_THREADSTATE
            
            this._threadstate_samples = new IList<System.Threading.ThreadState>[this._concurrency];
            
            for (int i = 0; i < this._concurrency; i++)
            {
                this._threadstate_samples[i] = new List<System.Threading.ThreadState>();
            }

            
            this._threadstate_timer = new System.Timers.Timer();
            this._threadstate_timer.Interval = 100;
            this._threadstate_timer.AutoReset = true;
            this._threadstate_timer.Elapsed += new ElapsedEventHandler(_threadstate_timer_Elapsed);

#endif
        }
#if PERIODICALLY_CHECK_THREADSTATE

        void _setup_phys()
        {
            _phys_count = Convert.ToInt32(Process.GetCurrentProcess().Threads.Count.ToString());
            for (int i = 0; i < _phys_count; i++)
            {
                _phys_threads.Add(Process.GetCurrentProcess().Threads[i]);
            }
            this._phys_threadstate_samples = new IList<System.Diagnostics.ThreadState>[_phys_count];
            this._phys_threadwaitreason_samples = new IList<string>[_phys_count];
            for (int i = 0; i < _phys_count; i++)
            {
                this._phys_threadstate_samples[i] = new List<System.Diagnostics.ThreadState>();
                this._phys_threadwaitreason_samples[i] = new List<string>();
            }

        }
        void _threadstate_timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            for (int i = 0; i < this._concurrency; i++)
            {
                System.Threading.ThreadState _state = _threads[i].ThreadState;

                this._threadstate_samples[i].Add(_state);
                

            }
            for (int i = 0; i < _phys_count;i++ )
            {
                this._phys_threadstate_samples[i].Add(_phys_threads[i].ThreadState);
                try
                {
                    string _reason = this._phys_threads[i].WaitReason.ToString();
                    this._phys_threadwaitreason_samples[i].Add(_reason);
                }
                catch (Exception ex)
                {
                    this._phys_threadwaitreason_samples[i].Add("NotWaiting");
                }
            }
        }
#endif

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
        private int _itemsize;
        [QS.Fx.Base.Inspectable]
        private int _numitems;
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

#if PERIODICALLY_CHECK_THREADSTATE
        [QS.Fx.Base.Inspectable]
        private int _phys_count =-1;
        [QS.Fx.Base.Inspectable]
        private System.Timers.Timer _threadstate_timer;
        [QS.Fx.Base.Inspectable]
        private IList<System.Threading.ThreadState>[] _threadstate_samples;
        [QS.Fx.Base.Inspectable]
        private IList<System.Diagnostics.ThreadState>[] _phys_threadstate_samples;
        [QS.Fx.Base.Inspectable]
        private IList<string>[] _phys_threadwaitreason_samples;
        [QS.Fx.Base.Inspectable]
        private IList<ProcessThread> _phys_threads = new List<ProcessThread>();
        [QS.Fx.Base.Inspectable]
        private string _threadstate_outpath;
#endif

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Work

        private void _Work()
        {
            bool _is = System.Runtime.GCSettings.IsServerGC;

            for (int _i = 0; _i < this._count; _i++)
            {
                if (Interlocked.Increment(ref this._numstarted) == this._concurrency)
                {

                    this._stopped.Reset();

                    this._numstopped = 0;
                    if (_i == 0)
                        MessageBox.Show("Are you ready to start the experiment?", "Ready?",
                            MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    int c = Process.GetCurrentProcess().Threads.Count;
#if PERIODICALLY_CHECK_THREADSTATE

                    if (_phys_count == -1)
                    {
                        _setup_phys();
                        this._threadstate_timer.Start();
                    } 
                    
#endif
                    this._totalcpuusage_counter.NextValue();
                    this._starttime = this._clock.Time;
                    this._stoptime = this._starttime + this._duration;
                    this._started.Set();
                }
                else
                    this._started.WaitOne();
                IDictionary<int, byte[]> _items = null;
                int _j = 0;
                int _jmax = this._numitems;
                double _time;
                int _size = this._itemsize;
                QS.Fx.Clock.IClock _clock = this._clock;
                double _stop = this._stoptime;
                int _maxk = this._iterations;
                int _c = 0;
                do
                {
                    _j++;
                    if (_j > _jmax)
                    {
                        _j = 0;
                        _items = null;
                    }
                    else
                    {
                        if (_items == null)
                            _items = new Dictionary<int, byte[]>();
                        _items.Add(_j, new byte[_size]);
                    }
                    for (int _k = 0; _k < _maxk; _k++)
                        _c++;
                    _time = _clock.Time;
                }
                while (_time < _stop);
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
#if PERIODICALLY_CHECK_THREADSTATE
                this._threadstate_timer.Stop();
                using (TextWriter _tw = new StreamWriter(new FileStream(_threadstate_outpath, FileMode.Create)))
                {
                    _tw.Write("Sample #\t");
                    for (int _kk = 0; _kk < this._concurrency; _kk++)
                    {
                        _tw.Write("LTS" + _kk.ToString() + "\t");
                    }
                    for (int _kk = 0; _kk < _phys_count; _kk++)
                    {
                        _tw.Write("PTS" + _kk + "\t");
                    }
                    for (int _kk = 0; _kk < _phys_count; _kk++)
                    {
                        _tw.Write("PTWR" + _kk + "\t");
                    }
                    _tw.WriteLine();
                    for (int _kk = 0; _kk < _threadstate_samples[0].Count; _kk++)
                    {
                        _tw.Write(_kk.ToString() + "\t");
                        foreach (IList<System.Threading.ThreadState> _state in _threadstate_samples)
                        {
                            _tw.Write(_state[_kk].ToString() + "\t");
                        }
                        foreach (IList<System.Diagnostics.ThreadState> _state in _phys_threadstate_samples)
                        {
                            _tw.Write(_state[_kk].ToString() + "\t");
                        }
                        foreach (IList<string> _state in _phys_threadwaitreason_samples)
                        {
                            _tw.Write(_state[_kk] + "\t");
                        }
                        _tw.WriteLine();
                    }
                }
#endif
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
