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
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Threading;
//using Point3D_ = QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("D22AF1F0C7524DABB252BF23B753809B")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class KMeansClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IKMeansClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public KMeansClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("number of points", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_points,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch_size,
            [QS.Fx.Reflection.Parameter("decimal places of precision", QS.Fx.Reflection.ParameterClass.Value)]
            int _precision,
            [QS.Fx.Reflection.Parameter("# of iterations", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_of_iterations,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IKMeans_> _workreference)
        {
            this._mycontext = _mycontext;
            this._precision = _precision;

            this._batch_size = _batch_size;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IKMeans_,
                    QS._qss_x_.Experiment_.Interface_.IKMeansClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._cpuutil = new QS._qss_x_.Experiment_.Utility_.CPUUtil_(_mycontext);

            if (_num_of_iterations != null)
            {
                this._iterations = _num_of_iterations;
            }
            else
            {
                this._iterations = 0;
            }

            while (_num_points % _batch_size != 0)
            {
                _batch_size++;
            }

            _batches = new QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[_num_points / _batch_size][];


            QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[] _batch = null;
            int _batch_index = 0;
            int _count = 0;
            Random _random = new Random();
            for (int i = 0; i < _num_points; i++)
            {
                if (i % _batch_size == 0)
                {
                    _count = 0;
                    _batch = new QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[_batch_size];
                    _batches[_batch_index++] = _batch;
                }
                _batch[_count++] = new QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_(_random.Next(1, 6) * _random.NextDouble(), _random.Next(1, 6) * _random.NextDouble(), _random.Next(1, 6) * _random.NextDouble());
            }
             
            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            // sample random initial means
            this._means = new QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[_k];

            for (int i = 0; i < this._k; i++)
            {
                _means[i] = _batches[_random.Next(0, this._batches.Length)][_random.Next(0, _batch_size)];
            }

            this._cpuutil.Start();
            this._start = this._mycontext.Platform.Clock.Time;
            // submit work
            for (int i = 0; i < this._batches.Length; i++)
            {
                this._workendpoint.Interface._Work(_means, this._batches[i]);
            }
           
            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Utility_.CPUUtil_ _cpuutil;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IKMeans_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IKMeans_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IKMeans_,
                QS._qss_x_.Experiment_.Interface_.IKMeansClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;


        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[] _means;
        [QS.Fx.Base.Inspectable]
        private double _start, _end;
        [QS.Fx.Base.Inspectable]
        private int _k;
        [QS.Fx.Base.Inspectable]
        private int _batch_size;
        [QS.Fx.Base.Inspectable]
        private int _resubmit_count = 0;
        [QS.Fx.Base.Inspectable]
        private int _precision;
        [QS.Fx.Base.Inspectable]
        private int _iterations;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[][] _batches;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IKMeansClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IKMeansClient_._Done(Array __new_means)
        {
            QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[] _new_means = (QS._qss_x_.Experiment_.Component_.KMeans_.Point3D_[])__new_means;

            if (this._iterations == 0)
            {

                bool _resubmit = false;


                for (int i = 0; i < this._k; i++)
                {
                    if (!_new_means[i].EqualsWithPrecision(_means[i], _precision))
                    {
                        _resubmit = true;
                        break;

                    }
                }
                if (_resubmit)
                {
                    //this._mycontext.Platform.Logger.Log("Resubmitting");
                    _resubmit_count++;
                    _means = _new_means;
                    for (int i = 0; i < this._batches.Length; i++)
                    {
                        this._workendpoint.Interface._Work(_means, this._batches[i]);
                    }
                    this._workendpoint.Interface._Done();
                }

                else
                {
                    
                    this._end = this._mycontext.Platform.Clock.Time;
                    this._cpuutil.Stop();
                    this._cpuutil.PrintAvg();
                    
                    double _duration = this._end - this._start;
                    this._cpuutil.CopyStats(_duration);
                    this._mycontext.Platform.Logger.Log("Duration: " + _duration);
                    this._mycontext.Platform.Logger.Log("Iterations (resubmissions): " + _resubmit_count);

                    
                }
            }
            else
            {
                if (_resubmit_count == _iterations)
                {
                    this._end = this._mycontext.Platform.Clock.Time;
                    this._cpuutil.Stop();
                    this._cpuutil.PrintAvg();
                    double _duration = this._end - this._start;
                    //Clipboard.SetText(_duration.ToString());
                    this._cpuutil.CopyStats(_duration);
                    
                    this._mycontext.Platform.Logger.Log("Duration: " + _duration);
                    this._mycontext.Platform.Logger.Log("Iterations (resubmissions): " + _resubmit_count);
                }
                else
                {
                    _resubmit_count++;
                    _means = _new_means;
                    for (int i = 0; i < this._batches.Length; i++)
                    {
                        this._workendpoint.Interface._Work(_means, this._batches[i]);
                    }
                    this._workendpoint.Interface._Done();
                }
            }
        }

        void QS._qss_x_.Experiment_.Interface_.IKMeansClient_._Set_K(int _k)
        {
            this._k = _k;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
