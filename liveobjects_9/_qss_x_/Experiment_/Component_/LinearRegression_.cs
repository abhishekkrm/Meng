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
    [QS.Fx.Reflection.ComponentClass("5A0008BAC3E84E87B8CE8E2F8FFF9674")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [Serializable]
    public sealed class LinearRegression_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.ILinearRegression_,
        QS._qss_x_.Experiment_.Interface_.ILinearRegression_, QS.Fx.Replication.IReplicated<LinearRegression_>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public LinearRegression_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_,
                    QS._qss_x_.Experiment_.Interface_.ILinearRegression_>(this);
        }

        public LinearRegression_()
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
            QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_,
                QS._qss_x_.Experiment_.Interface_.ILinearRegression_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private double _num;
        [QS.Fx.Base.Inspectable]
        private double _sumx;
        [QS.Fx.Base.Inspectable]
        private double _sumxx;
        [QS.Fx.Base.Inspectable]
        private double _sumy;
        [QS.Fx.Base.Inspectable]
        private double _sumxy;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ILinearRegression_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_,
                QS._qss_x_.Experiment_.Interface_.ILinearRegression_>
                    QS._qss_x_.Experiment_.Object_.ILinearRegression_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ILinearRegression_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.ILinearRegression_._Work(ArraySegment<QS._core_e_.Data.XY> _data)
        {
            QS.Fx.Serialization.ISerializable _b;
            
            
            this._num += _data.Count;
            for (int _k = 0; _k < _data.Count; _k++)
            {
                QS._core_e_.Data.XY _point = _data.Array[_data.Offset + _k];
                this._sumx += _point.x;
                this._sumy += _point.y;
                this._sumxx += _point.x * _point.x;
                this._sumxy += _point.x * _point.y;
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.ILinearRegression_._Done()
        {
            double _a = (this._num * this._sumxy - this._sumx * this._sumy) / (this._num * this._sumxx - this._sumx * this._sumx);
            double _b = (this._sumy - _a * this._sumx) / this._num;
            this._num = 0;
            this._sumx = 0;
            this._sumy = 0;
            this._sumxx = 0;
            this._sumxy = 0;
            this._workendpoint.Interface._Done(_a, _b);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<LinearRegression_> Members

        void QS.Fx.Replication.IReplicated<LinearRegression_>.Export(LinearRegression_ _other)
        {
        }

        void QS.Fx.Replication.IReplicated<LinearRegression_>.Import(LinearRegression_ _other)
        {
            this._num += _other._num;
            this._sumx += _other._sumx;
            this._sumy += _other._sumy;
            this._sumxx += _other._sumxx;
            this._sumxy += _other._sumxy;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
