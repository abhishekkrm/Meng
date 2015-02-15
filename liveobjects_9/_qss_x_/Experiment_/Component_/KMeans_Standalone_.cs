/*
Copyright (c) 2008-2009 Chuck Sakoda. All rights reserved.
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
//#define PROFILE_LOCKWAITING
//#define LOCKWAITING_SUM
//#define PROFILE_COREUTIL
#define CPUUTIL
//#define GCUTIL
//#define LOCKINGUTIL

using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("765C06F3D2FF46d08C62322185B47B3D", "KMeans_Standalone_")]
    class KMeans_Standalone_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject
    {

        #region Constructor

        public KMeans_Standalone_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("k value", QS.Fx.Reflection.ParameterClass.Value)]
            int _k,
            [QS.Fx.Reflection.Parameter("number of points", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_points,
            [QS.Fx.Reflection.Parameter("decimal places of precision", QS.Fx.Reflection.ParameterClass.Value)]
            int _precision,
            [QS.Fx.Reflection.Parameter("# of iterations", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_of_iterations,
            [QS.Fx.Reflection.Parameter("# threads", QS.Fx.Reflection.ParameterClass.Value)] 
            int _num_threads)
        {
            this._mycontext = _mycontext;
            this._clock = this._mycontext.Platform.Clock;
            this._num_threads = _num_threads;
#if LOCKINGUTIL
            this._lockingutil = new QS._qss_x_.Experiment_.Utility_.LockingUtil_(_mycontext);
#endif
#if GCUTIL
            this._gcutil = new QS._qss_x_.Experiment_.Utility_.GCUtil_(_mycontext); 
#endif
#if CPUUTIL
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);
#endif
#if COREUTIL
            this._coreutil = new QS._qss_x_.Experiment_.Utility_.CoreUtil_(_mycontext, 8); 
#endif
#if PROFILE_DATASET_SIZE
            _input_size = 0;
            int c = 0;
            foreach (string _file in Directory.GetFiles(_folder, "*.txt", SearchOption.AllDirectories))
            {
                if (c++ >= _count)
                    break;

                FileInfo _fi = new FileInfo(_file);
                _input_size += _fi.Length;


            }
#endif
#if PROFILE_LOCKWAITING
            this._lockwaiting_times = new QS._qss_c_.Statistics_.Samples2D();
#if LOCKWAITING_SUM
            this._lockwaiting_sums = new QS._qss_e_.Data_.FastStatistics_(this._num_threads);
#endif
#endif
            this._k = _k;
            for (int i = 0; i < _k; i++)
            {
                this._clustering.Add(i, new List<Point3D_>());
            }
            this._thread_times = new QS._qss_e_.Data_.FastStatistics2D_(this._num_threads);

            Thread[] _t = new Thread[_num_threads];

            for (int i = 0; i < _num_threads; i++)
            {
                _t[i] = new Thread(new ParameterizedThreadStart(this._Work));
            }


            this._clock = _mycontext.Platform.Clock;

            if (_num_of_iterations != null)
            {
                this._iterations = _num_of_iterations;
            }
            else
            {
                this._iterations = 0;
            }

            int _batch_size = (int)Math.Ceiling(((double)(_num_points) / _num_threads));
            Point3D_[][] _batches;
            _batches = new Point3D_[_num_threads][];



            Random _random = new Random();

            for (int i = 0; i < _num_threads; i++)
            {
                int _size = ((_batch_size * (i)) < _num_points) ? _batch_size : (_num_points - (_batch_size * (i - 1)));
                _batches[i] = new Point3D_[_size];

                for (int j = 0; j < _size; j++)
                {
                    _batches[i][j] = new Point3D_(_random.Next(1, 6) * _random.NextDouble(), _random.Next(1, 6) * _random.NextDouble(), _random.Next(1, 6) * _random.NextDouble());
                }
            }



            // sample random initial means
            this._means = new Point3D_[_k];

            for (int i = 0; i < this._k; i++)
            {
                _means[i] = _batches[_random.Next(0, _batches.Length)][_random.Next(0, _batch_size)];
            }



            #region Start Threads

            for (int i = 0; i < _num_threads; i++)
            {
                _t[i].Start(new object[] { _batches[i] });
            }

            #endregion


        }

        #endregion



        #region Fields
        [QS.Fx.Base.Inspectable]
        private int _k;
        [QS.Fx.Base.Inspectable]
        private int _iterations;
        [QS.Fx.Base.Inspectable]
        private int _init = 0;
        [QS.Fx.Base.Inspectable]
        private int _id = -1;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _num_threads;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _iter_wait = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _iter_wait_3 = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        int _iter_wait_2 = 0;
        [QS.Fx.Base.Inspectable]
        private int _num_waiting = 0;
        [QS.Fx.Base.Inspectable]
        private ManualResetEvent _started = new ManualResetEvent(false);
        [QS.Fx.Base.Inspectable]
        private double _begin, _end;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
#if PROFILE_COREUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CoreUtil_ _coreutil; 
#endif
#if CPUUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
#endif
#if GCUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.GCUtil_ _gcutil;
#endif
#if LOCKINGUTIL
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.LockingUtil_ _lockingutil;
#endif
        [QS.Fx.Base.Inspectable]
        private Point3D_[] _means;
        [QS.Fx.Base.Inspectable]
        private IDictionary<int, List<Point3D_>> _clustering = new Dictionary<int, List<Point3D_>>();
        [QS.Fx.Base.Inspectable]
        private int _done = 0;
#if PROFILE_LOCKWAITING
        [QS.Fx.Base.Inspectable]
        private int _lw_unit = 1000000000; // nanoseconds
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _lockwaiting_times;
        //[QS.Fx.Base.Inspectable]
        //private bool _is_merged_lwtimes = false;
        //private QS._qss_c_.Statistics_.Samples2D _merged_lwtimes;
        //[QS.Fx.Base.Inspectable]
        //public QS._qss_c_.Statistics_.Samples2D _Merged_LockWaiting_times
        //{
        //    get
        //    {
        //        if (!_is_merged_lwtimes)
        //        {
        //            QS._qss_x_.Experiment_.Utiliity_.Sample2D_Merger_ _m = new QS._qss_x_.Experiment_.Utiliity_.Sample2D_Merger_();
        //            _merged_lwtimes = _m._Merge_Samples(_lockwaiting_times);
        //            _is_merged_lwtimes = true;
        //        }
        //        return _merged_lwtimes;
        //    }

        //}
#if LOCKWAITING_SUM
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics_ _lockwaiting_sums;
#endif
#endif
        [QS.Fx.Base.Inspectable]
        private QS._qss_e_.Data_.FastStatistics2D_ _thread_times;


        #endregion


        public struct Point3D_
        {
            public Point3D_(double x, double y, double z)
            {
                this._x = x;
                this._y = y;
                this._z = z;
            }

            public double _x, _y, _z;

            public bool EqualsWithPrecision(Point3D_ c2, int _precision)
            {
                if (Math.Round(_x, _precision) == Math.Round(c2._x, _precision) && Math.Round(_y, _precision) == Math.Round(c2._y, _precision) && Math.Round(_z, _precision) == Math.Round(c2._z, _precision))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            public static bool operator ==(Point3D_ c1, Point3D_ c2)
            {
                if (c1._x == c2._x && c1._y == c2._y && c1._z == c2._z)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool operator !=(Point3D_ c1, Point3D_ c2)
            {
                if (c1 == c2)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }



        private double _Distance(Point3D_ _a, Point3D_ _b)
        {
            // sqrt( sum( (p_i - q_i)^2, 1, n ) )

            double _sum = Math.Pow(_a._x - _b._x, 2) + Math.Pow(_a._y - _b._y, 2) + Math.Pow(_a._z - _b._z, 2);
            return Math.Sqrt(_sum);
        }


        void _Work(object _o)
        {

            object[] o = (object[])_o;
            Point3D_[] _data = (Point3D_[])o[0];
            int _thread_id = Interlocked.Increment(ref _id);




            #region Wait for all threads to start before working

            if (Interlocked.Increment(ref this._init) == this._num_threads)
            {
                MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

#if PROFILE_COREUTIL
                _coreutil.Start(); 
#endif
#if CPUUTIL
                this._cpuutil.Start();
#endif
#if LOCKINGUTIL
                this._lockingutil.Start();
#endif
#if GCUTIL
                this._gcutil.Start(); 
#endif
                this._begin = this._clock.Time;
                _started.Set();
            }
            else
            {
                _started.WaitOne();
            }

            #endregion

            double _begin_thread = this._clock.Time;


#if PROFILE_1
            double _timestamp = double.NaN;
            if (this._clock != null)
                _timestamp = this._clock.Time;
#endif
#if DEBUG_MSG
            _log.Log("      Entering Work");
#endif
            for (int _iter = 0; _iter < this._iterations; _iter++)
            {

                if (Interlocked.Increment(ref this._iter_wait_2) == this._num_threads)
                {
                    this._iter_wait_3.Reset();
                    this._num_waiting = 0;
                    this._iter_wait.Set();
                }
                else
                {
                    this._iter_wait.WaitOne();
                }
                try
                {
                    // iterate over each data point we've received
                    for (int j = 0; j < _data.Length; j++)
                    {
                        double _min = double.PositiveInfinity;
                        int _min_id = -1;

                        // calculate the distance from this point to each mean
                        for (int i = 0; i < _means.Length; i++)
                        {
                            double _d = _Distance(_means[i], _data[j]);
                            if (_d < _min)
                            {
                                _min = _d;
                                _min_id = i;
                            }
                        }

                        // cluster this point with the appropriate mean (min distance)
                        lock (this._clustering)
                        {
                            try
                            {
                                _clustering[_min_id].Add(_data[j]);
                            }
                            catch (KeyNotFoundException _e)
                            {
                                _clustering[_min_id] = new List<Point3D_>();
                                _clustering[_min_id].Add(_data[j]);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("F", e);
                }


                if (Interlocked.Increment(ref this._num_waiting) == _num_threads)
                {
                    _means = new Point3D_[this._k];

                    // iterate over each k value and find the new mean of the points
                    // clustered into this k-value
                    for (int j = 0; j < _clustering.Count; j++)
                    {
                        double _sum_x, _sum_y, _sum_z;
                        _sum_x = _sum_y = _sum_z = 0;
                        List<Point3D_> _element = _clustering[j];

                        // find the average for this particular k-value
                        for (int i = 0; i < _element.Count; i++)
                        {
                            _sum_x += _element[i]._x;
                            _sum_y += _element[i]._y;
                            _sum_z += _element[i]._z;
                        }
                        _sum_x /= _element.Count;
                        _sum_y /= _element.Count;
                        _sum_z /= _element.Count;

                        _means[j] = new Point3D_(_sum_x, _sum_y, _sum_z);

                    }


                    // reset for the next set of Work calls.
                    foreach (KeyValuePair<int, List<Point3D_>> _k in _clustering)
                    {
                        _k.Value.Clear();
                    }
                    Interlocked.Exchange(ref this._iter_wait_2, 0);
                    
                    this._iter_wait.Reset();
                    this._iter_wait_3.Set();
                    
                }
                else
                {
                            this._iter_wait_3.WaitOne();
                        
                    

                }
                

            }

            double _end_thread = this._clock.Time;

            #region Report Per Thread profiling
#if PROFILE_LOCKWAITING
#if LOCKWAITING_SUM
            this._mycontext.Platform.Logger.Log("Lockwaiting Sum: " + _sum_lockwait);
            this._lockwaiting_sums.Log(_sum_lockwait);
#endif
#endif
            this._thread_times.Add(_begin_thread, _end_thread);
            this._mycontext.Platform.Logger.Log("Thread Length: " + (_end_thread - _begin_thread).ToString());

#if PROFILE_LOCKWAITING
            //QS._core_e_.Data.XYSeries _xys = new QS._core_e_.Data.XYSeries(this._lockwaiting_times[_thread_id].Samples);
            //QS._core_e_.Data.XY[] _AddUpY = ((QS._core_e_.Data.XYSeries)_xys.AddUp_Y).Data;
            
            //this._mycontext.Platform.Logger.Log("Time spent waiting for locks: " + _AddUpY[_AddUpY.Length-1].y);
#endif

            #endregion

            #region Report Whole Experiment Profiling (last thread to complete)

            if (Interlocked.Increment(ref this._done) == this._num_threads)
            {
                this._end = _mycontext.Platform.Clock.Time;
                double _duration = this._end - this._begin;
                this._mycontext.Platform.Logger.Log("Duration: " + (this._end - this._begin));
                

#if PROFILE_COREUTIL
                this._coreutil.Stop();
                this._coreutil.PrintAvg(); 
#endif
#if CPUUTIL
                this._cpuutil.Stop();
                this._cpuutil.PrintAvg();
                
#endif
#if GCUTIL
                this._gcutil.Stop();
                this._gcutil.PrintAvg();
                //this._gcutil.CopyStats(_duration);
#endif
#if LOCKINGUTIL
                this._lockingutil.Stop();
                this._lockingutil.PrintAvg();
                this._lockingutil.CopyStats(_duration);
#endif


#if PROFILE_1
            double _totalcpuusage = 0;
            foreach (QS._core_e_.Data.XY _sample in this._totalcpuusage_samples.Samples)
                _totalcpuusage += _sample.y;
            _totalcpuusage = _totalcpuusage / ((double)_totalcpuusage_samples.Samples.Length);
            this._mycontext.Platform.Logger.Log("Cpu Usage : " + _totalcpuusage.ToString());
#endif

            }

            #endregion

        }


    }
}

