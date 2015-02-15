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
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using CovarianceRequests_ = QS._qss_x_.Experiment_.Component_.PCA_.CovarianceRequests_;
using RowCol = QS._qss_x_.Experiment_.Component_.PCA_.RowCol;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("AF233986545146A693F55392369F272E")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class PCAClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IPCAClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PCAClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("size", QS.Fx.Reflection.ParameterClass.Value)]  // assumes _size X _size matrix.
            int _size,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)]  
            int _batch_size,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPCA_> _workreference)
        {
            this._mycontext = _mycontext;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPCA_,
                    QS._qss_x_.Experiment_.Interface_.IPCAClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._size = _size;
            this._batch_size = _batch_size;

            Random _random = new Random();

            _matrix = new double[_size][];
            for (int i = 0; i < _size; i++)
            {
                _matrix[i] = new double[_size];
                for (int j = 0; j < _size; j++)
                {
                    _matrix[i][j] = (double)_random.NextDouble() * _random.Next(2);
                }
            }

            List<double[]> _batches = new List<double[]>();
            int _count = 0;
            double[] _batch = new double[_batch_size * _size];
            int _batch_index = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_count++ == _batch_size -1)
                {
                    _batches.Add(_batch);
                    _batch = new double[_batch_size * _size];
                    _batch_index = 0;
                    _count = 0;
                }
                
                    Array.Copy(_matrix[i], 0, _batch, _batch_index, _matrix[i].Length);
                
                _batch_index += _matrix[i].Length;
            }


            if (_count < _batch_size)
            {
                Array.Resize<double>(ref _batch, _count);
                _batches.Add(_batch);
            }

            int _row_count = 0;

            for (int i = 0; i < _batches.Count; i++)
            {
                this._workendpoint.Interface._Work_Mean(_batches[i], _row_count, _size);
                _row_count += _batches[i].Length / _size ;
            }
            this._workendpoint.Interface._Done_Mean();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IPCA_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IPCA_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPCA_,
                QS._qss_x_.Experiment_.Interface_.IPCAClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;


        [QS.Fx.Base.Inspectable]
        private int _batch_size;
        [QS.Fx.Base.Inspectable]
        private int _size;
        [QS.Fx.Base.Inspectable]
        private double[][] _matrix;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPCAClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IPCAClient_._Done_Mean(IDictionary<int, double> _row_means)
        {

            


            List<CovarianceRequests_> _reqs = new List<CovarianceRequests_>();
            

            int _cov_batch_size = _batch_size * _batch_size;

            int _count = 0;
            CovarianceRequests_ _batch = new CovarianceRequests_(_size);

            for(int i=0;i<_size;i++) {
                for (int j = 0; j < _size; j++)
                {
                    if (_count++ >= _cov_batch_size)
                    {
                        _reqs.Add(_batch);
                        _batch = new CovarianceRequests_(_size);
                    }
                    _batch.Add(i, j, _matrix[i], _matrix[j], _row_means[i], _row_means[j]);
                }
            }

            foreach (CovarianceRequests_ _req in _reqs)
            {

                this._workendpoint.Interface._Work_Covariance(_req);
            }
            this._workendpoint.Interface._Done_Covariance();
        }

        void QS._qss_x_.Experiment_.Interface_.IPCAClient_._Done_Covariance(IDictionary<RowCol, double> _cov_matrix_dict)
        {

            StringBuilder _pretty_cov_matrix = new StringBuilder();
            double[] _cov_matrix = new double[_size * _size];
            foreach (KeyValuePair<RowCol, double> _entry in _cov_matrix_dict)
            {
                RowCol _rc = _entry.Key;
                _cov_matrix[_rc.col + _rc.row * _size] = _entry.Value;
            }


            for (int _row = 0; _row < _size; _row++)
            {
                for (int _col = 0; _col < _size; _col++)
                {
                    _pretty_cov_matrix.Append(_cov_matrix[_row * _size + _col].ToString("0.0"));
                }
                _pretty_cov_matrix.AppendLine();
            }

            this._mycontext.Platform.Logger.Log(_pretty_cov_matrix.ToString());

        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
