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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("E64976E863BF4170828A8366747E2C32")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    public sealed class PCA_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IPCA_,
        QS._qss_x_.Experiment_.Interface_.IPCA_, QS.Fx.Replication.IReplicated<PCA_>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PCA_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPCAClient_,
                    QS._qss_x_.Experiment_.Interface_.IPCA_>(this);
        }

        public PCA_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPCAClient_,
                QS._qss_x_.Experiment_.Interface_.IPCA_> _workendpoint;

        [QS.Fx.Base.Inspectable]
        private IDictionary<int, double> _mean_result = new Dictionary<int, double>();
        [QS.Fx.Base.Inspectable]
        private IDictionary<RowCol, double> _covariance_result = new Dictionary<RowCol, double>();
        [QS.Fx.Base.Inspectable]
        private bool _mean_step = false;
        [QS.Fx.Base.Inspectable]
        private bool _covariance_step = false;


        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    


        #region IPCA_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPCAClient_,
                QS._qss_x_.Experiment_.Interface_.IPCA_>
                    QS._qss_x_.Experiment_.Object_.IPCA_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    


        #region RowCol
        [QS.Fx.Reflection.ValueClass("3567EDAF7262460eA962FC9F587AE818")]
        public struct RowCol
        {
            public RowCol(int row, int col)
            {
                this.row = row;
                this.col = col;
            }

            public static bool operator ==(RowCol _c1, RowCol _c2)
            {
                if (_c1.col == _c2.col && _c1.row == _c2.row)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public static bool operator !=(RowCol _c1, RowCol _c2)
            {
                if (_c1==_c2)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public override string ToString()
            {
                return "(" + row.ToString() + ", " + col.ToString() + ")";
            }


            public int row, col;
        }

        #endregion

        #region CovarianceRequests_ 

        
    
 


        [QS.Fx.Reflection.ValueClass("D7C001564CB64847826F9672B34DC045")]
        public struct CovarianceRequests_
        {
            
            public CovarianceRequests_(int _size)
            {
                this._size = _size;
                _rows = new Dictionary<int, double[]>();
                _means = new Dictionary<int, double>();
                _requests = new List<RowCol>();
            }

            public void Add(int _row_i, int _col_j, double[] _row, double[] _col, double _row_mean, double _col_mean)
            {
                if (!_rows.ContainsKey(_row_i))
                {
                    _rows[_row_i] = _row;
                    _means[_row_i] = _row_mean;
                }
                if (!_rows.ContainsKey(_col_j))
                {
                    _rows[_col_j] = _col;
                    _means[_col_j] = _col_mean;
                }
                _requests.Add(new RowCol(_row_i, _col_j));
            }

            public IList<RowCol> Requests
            {
                get
                {
                    return this._requests;
                }
            }


            public double ApplyMeanAndMultiply(RowCol _rc)
            {
                return this.ApplyMeanAndMultiply(_rc.row, _rc.col);
            }

            public double ApplyMeanAndMultiply(int row, int col)
            {

                // now we wish to find the covariance matrix entry (_row,_col)
                // this requires row _row B, and col _col of B*, which is actually
                // row _col of B.
                double _sum = 0;
                for (int i = 0; i < _size; i++)
                {
                    _sum += (_rows[row][i] - _means[row]) * (_rows[col][i] - _means[col]);
                }
                _sum /= _size;

                return _sum;
            }

            IDictionary<int, double[]> _rows;
            IDictionary<int, double> _means;
            IList<RowCol> _requests;
int _size;
            
        }

        #endregion


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPCA_ Members


        #region Mean
        
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPCA_._Work_Mean(double[] _rows, int _start_row, int _size)
        {
            if (_covariance_step)
                throw new Exception("Started a mean step while covariance step already running");

            _mean_step = true;

            

            for (int _row = 0; _row < _rows.Length; _row++)
            {
                double _sum = 0;
                for (int i = 0; i < _size; i++)
                {
                    _sum += _rows[(_row*_size) + i];
                }
                _sum /= _size;
                _mean_result[_row + _start_row] = _sum;
            }
        }


        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPCA_._Done_Mean()
        {

            this._workendpoint.Interface._Done_Mean(_mean_result);
            _mean_step = false;

        }

        #endregion

        #region Covariance

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPCA_._Work_Covariance(CovarianceRequests_ _reqs)
        {

            // Each call to covariance is specified some rectangular area of values to be calculated
            // delimited by the coordinates (_start_x, _start_y) and (_end_x, _end_y)

            // Covaraiance Matrix = (1 / _size) * (B dot B*)
            
            if (_mean_step)
                throw new Exception("Started a covariance step while mean step already running");

            _covariance_step = true;

            

            foreach (RowCol _req in _reqs.Requests)
            {
                _covariance_result[_req] = _reqs.ApplyMeanAndMultiply(_req);
            }

        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPCA_._Done_Covariance()
        {

            this._workendpoint.Interface._Done_Covariance(_covariance_result);
            _covariance_step = false;

        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<PCA_> Members

        void QS.Fx.Replication.IReplicated<PCA_>.Export(PCA_ _other)
        {
            _other._mean_result.Clear();
            _other._covariance_result.Clear();
        }

        void QS.Fx.Replication.IReplicated<PCA_>.Import(PCA_ _other)
        {

            if (_mean_step)
            {
                foreach (KeyValuePair<int, double> _element in _other._mean_result)
                {
                    if (_mean_result.ContainsKey(_element.Key))
                        throw new Exception("Already have a average for row " + _element.Key.ToString());

                    _mean_result[_element.Key] = _element.Value;

                }
                _other._mean_result.Clear();
                return;
            }
         
            if (_covariance_step)
            {
                foreach (KeyValuePair<RowCol, double> _element in _other._covariance_result)
                {
                    if (_covariance_result.ContainsKey(_element.Key))
                        throw new Exception("Already have a covariance value for " + _element.Key.ToString());

                    _covariance_result[_element.Key] = _element.Value;

                }
                _other._covariance_result.Clear();
                return;
            }

            throw new Exception("Tried to import when not in a defined step");
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
