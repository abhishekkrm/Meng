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
    [QS.Fx.Reflection.ComponentClass("4BA720B39CCF4BBFB1383C2E421AC747")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class MatrixMultiplyClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MatrixMultiplyClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count,
            [QS.Fx.Reflection.Parameter("size", QS.Fx.Reflection.ParameterClass.Value)] 
            int _size,
            [QS.Fx.Reflection.Parameter("verify", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _verify,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMatrixMultiply_> _workreference)
        {
            this._mycontext = _mycontext;
            this._count = _count;
            this._size = _size;
            this._verify = _verify;
            this._workreference = _workreference;
            this._workproxy = this._workreference.Dereference(this._mycontext);
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_,
                    QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_>(this);
            this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
            Random _random = new Random();
            double [][] _matrices = new double[_count][];
            for (int _i = 0; _i < _count; _i++)
            {
                double[] _matrix = new double[_size * _size];
                for (int _j = 0; _j < _matrix.Length; _j++)
                    _matrix[_j] = (double) _random.Next(2);
                _matrices[_i] = _matrix;
            }
            if (this._verify)
            {
                double[] _temp = new double[_size * _size];
                bool _owns = false;
                this._result = _matrices[0];
                for (int _z = 1; _z < _count; _z++)
                {
                    double[] _matrix = _matrices[_z];
                    for (int _i = 0; _i < _size; _i++)
                    {
                        for (int _j = 0; _j < _size; _j++)
                        {
                            double _s = 0;
                            for (int _k = 0; _k < _size; _k++)
                                _s += this._result[_i * _size + _k] * _matrix[_k * _size + _j];
                            _temp[_i * _size + _j] = _s;
                        }
                    }
                    if (_owns)
                    {
                        double[] _m = this._result;
                        this._result = _temp;
                        _temp = _m;
                    }
                    else
                    {
                        this._result = _temp;
                        _temp = new double[_size * _size];
                        _owns = true;
                    }
                }
            }
            MessageBox.Show("Are you ready to start the experiment?", "Ready?", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            this._timestamp = _mycontext.Platform.Clock.Time;
            foreach (double[] _matrix in _matrices)
                this._workendpoint.Interface._Work(_matrix, _size);
            this._workendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        private int _size;
        [QS.Fx.Base.Inspectable]
        private bool _verify;
        [QS.Fx.Base.Inspectable]
        private double[] _result;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMatrixMultiply_> _workreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IMatrixMultiply_ _workproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_,
                QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workconnection;
        [QS.Fx.Base.Inspectable]
        private double _timestamp;
        [QS.Fx.Base.Inspectable]
        private double _duration;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMatrixMultiplyClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_._Done(double[] _matrix)
        {
            this._duration = _mycontext.Platform.Clock.Time - this._timestamp;
            this._mycontext.Platform.Logger.Log("Duration : " + this._duration.ToString());
            if (this._verify)
            {
                bool _same = true;
                for (int _i = 0; _same && (_i < (this._size * this._size)); _i++)
                    _same = (this._result[_i] == _matrix[_i]);
                this._mycontext.Platform.Logger.Log("Correct : " + _same.ToString());
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
