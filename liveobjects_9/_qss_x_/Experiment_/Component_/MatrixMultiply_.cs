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
    [QS.Fx.Reflection.ComponentClass("A7933561D3294BF0895D9A0725BC6BC0")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    public sealed class MatrixMultiply_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IMatrixMultiply_,
        QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_, QS.Fx.Replication.IReplicated<MatrixMultiply_>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MatrixMultiply_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_,
                    QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_>(this);
        }

        public MatrixMultiply_()
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
            QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_,
                QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private double[] _matrix;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _size;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _owns;
        [QS.Fx.Base.Inspectable]
        private double[] _temp;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMatrixMultiply_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IMatrixMultiplyClient_,
                QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_>
                    QS._qss_x_.Experiment_.Object_.IMatrixMultiply_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IMatrixMultiply_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_._Work(double[] _matrix, int _size)
        {
            this._Multiply(_matrix, _size);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IMatrixMultiply_._Done()
        {
            this._workendpoint.Interface._Done(this._matrix);
            this._matrix = null;
            this._temp = null;
            this._size = 0;
            this._owns = false;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<MatrixMultiply_> Members

        void QS.Fx.Replication.IReplicated<MatrixMultiply_>.Export(MatrixMultiply_ _other)
        {
            _other._matrix = null;
            _other._temp = null;
            _other._size = 0;
            _other._owns = false;
        }

        void QS.Fx.Replication.IReplicated<MatrixMultiply_>.Import(MatrixMultiply_ _other)
        {
            this._Multiply(_other._matrix, _other._size);
            _other._owns = false;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _Multiply

        private void _Multiply(double[] _matrix, int _size)
        {
            if (_matrix != null)
            {
                if (_size > 0)
                {
                    if (this._matrix != null)
                    {
                        if (this._size == _size)
                        {
                            if (this._temp == null)
                                this._temp = new double[_size * _size];
                            for (int _i = 0; _i < _size; _i++)
                            {
                                for (int _j = 0; _j < _size; _j++)
                                {
                                    double _s = 0;
                                    for (int _k = 0; _k < _size; _k++)
                                        _s += this._matrix[_i * _size + _k] * _matrix[_k * _size + _j];
                                    this._temp[_i * _size + _j] = _s;
                                }
                            }
                            if (this._owns)
                            {
                                double[] _m = this._matrix;
                                this._matrix = this._temp;
                                this._temp = _m;
                            }
                            else
                            {
                                this._matrix = this._temp;
                                this._temp = null;
                                this._owns = true;
                            }
                        }
                        else
                            throw new Exception("Sizes do not match.");
                    }
                    else
                    {
                        this._matrix = _matrix;
                        this._size = _size;
                        this._owns = false;
                    }
                }
                else
                    throw new Exception("Size cannot be zero.");
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
