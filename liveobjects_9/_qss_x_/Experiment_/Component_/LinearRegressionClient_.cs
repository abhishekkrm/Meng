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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("9C6F47AAAD0A46E2B7D6E578E50EC3E5")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class LinearRegressionClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public LinearRegressionClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("cycle", QS.Fx.Reflection.ParameterClass.Value)] 
            int _cycle,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("batch", QS.Fx.Reflection.ParameterClass.Value)] 
            int _batch,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.ILinearRegression_> _workreference)
        {
            this._mycontext = _mycontext;
            this._cycle = _cycle;
            this._count = _count;
            this._batch = _batch;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.ILinearRegression_,
                    QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            this._data = new QS._core_e_.Data.XY[this._count];
            Random _random = new Random();
            for (int _i = 0; _i < this._count; _i++)
                this._data[_i] = new QS._core_e_.Data.XY(_random.NextDouble(), _random.NextDouble());
            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            this._timestamp = _mycontext.Platform.Clock.Time;
            this._Work();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private int _cycle;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private int _batch;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.ILinearRegression_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.ILinearRegression_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.ILinearRegression_,
                QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private double _timestamp;
        [QS.Fx.Base.Inspectable]
        private double _duration;
        [QS.Fx.Base.Inspectable]
        private int _done;
        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.XY[] _data;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ILinearRegressionClient_ Members

        void QS._qss_x_.Experiment_.Interface_.ILinearRegressionClient_._Done(double _a, double _b)
        {
            if ((++this._done) < this._cycle)
                this._Work();
            else
            {
                this._duration = _mycontext.Platform.Clock.Time - this._timestamp;
                this._mycontext.Platform.Logger.Log("Duration : " + this._duration.ToString());
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Work

        private void _Work()
        {
            int _k = 0;
            while (true)
            {
                int _n = this._count - _k;
                if (_n > 0)
                {
                    if (_n > this._batch)
                        _n = this._batch;
                    this._workendpoint.Interface._Work(new ArraySegment<QS._core_e_.Data.XY>(this._data, _k, _n));
                    _k += _n;
                }
                else
                    break;
            }
            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
