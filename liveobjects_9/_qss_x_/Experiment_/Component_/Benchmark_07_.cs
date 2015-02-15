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
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("3003B12530C6481997C9359CCCB1C9D0")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Benchmark_07)]
    public sealed class Benchmark_07_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IBenchmark_, QS._qss_x_.Experiment_.Interface_.IBenchmark_, QS.Fx.Replication.IReplicated<Benchmark_07_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Benchmark_07_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("num", QS.Fx.Reflection.ParameterClass.Value)]
            int _num,
            [QS.Fx.Reflection.Parameter("size", QS.Fx.Reflection.ParameterClass.Value)]
            int _size,
            [QS.Fx.Reflection.Parameter("batch", QS.Fx.Reflection.ParameterClass.Value)]
            int _batch,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
            : this(_mycontext)
        {
            this._num = _num;
            this._size = _size;
            this._batch = _batch;
            this._count = _count;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmark_>(this);
        }

        public Benchmark_07_(QS.Fx.Object.IContext _mycontext)
            : this()
        {
            this._mycontext = _mycontext;
        }

        public Benchmark_07_()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _imported;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                QS._qss_x_.Experiment_.Interface_.IBenchmark_> _benchmarkendpoint;
        private Random _random;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private byte[][] _buffers;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _num;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _size;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _batch;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _offset;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int[] _selector1;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int[] _selector2;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int[] _indexes1;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int[] _indexes2;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _batching;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IBenchmark_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                QS._qss_x_.Experiment_.Interface_.IBenchmark_>
                    QS._qss_x_.Experiment_.Object_.IBenchmark_._Benchmark
        {
            get { return this._benchmarkendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IBenchmark_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Work()
        {
            if (!this._initialized)
            {
                this._initialized = true;
                this._buffers = new byte[this._num][];
                this._selector1 = new int[this._count];
                this._selector2 = new int[this._count];
                this._indexes1 = new int[this._count];
                this._indexes2 = new int[this._count];
                for (int _m = 0; _m < this._num; _m++)
                    this._buffers[_m] = new byte[this._size];                
                this._random = new Random();
                for (int _i = 0; _i < this._count; _i++)
                {
                    this._selector1[_i] = this._random.Next(this._num);
                    this._selector2[_i] = this._random.Next(this._num);
                    this._indexes1[_i] = this._random.Next(this._size);
                    this._indexes2[_i] = this._random.Next(this._size);
                }
                this._batching = (this._batch < this._count);
            }
            if (this._batching)
            {
                int _i1 = this._offset;
                int _i2 = (this._offset + this._batch - 1) % this._count;
                int _i3 = 0;
                int _i4 = -1;
                if (_i1 > _i2)
                {
                    _i4 = _i2;
                    _i2 = this._count - 1;
                }
                if (_i1 <= _i2)
                {
                    for (int _i = _i1; _i <= _i2; _i++)
                    {
                        int _s1 = this._selector1[_i];
                        int _s2 = this._selector2[_i];
                        int _p1 = this._indexes1[_i];
                        int _p2 = this._indexes2[_i];
                        byte _b = this._buffers[_s1][_p1];
                        this._buffers[_s1][_p1] = this._buffers[_s2][_p2];
                        this._buffers[_s2][_p2] = _b;
                    }
                }
                if (_i3 <= _i4)
                {
                    for (int _i = _i3; _i < _i4; _i++)
                    {
                        int _s1 = this._selector1[_i];
                        int _s2 = this._selector2[_i];
                        int _p1 = this._indexes1[_i];
                        int _p2 = this._indexes2[_i];
                        byte _b = this._buffers[_s1][_p1];
                        this._buffers[_s1][_p1] = this._buffers[_s2][_p2];
                        this._buffers[_s2][_p2] = _b;
                    }
                }
                this._offset = (this._offset + this._batch) % this._count;
            }
            else
            {
                for (int _i = 0; _i < this._count; _i++)
                {
                    int _s1 = this._selector1[_i];
                    int _s2 = this._selector2[_i];
                    int _p1 = this._indexes1[_i];
                    int _p2 = this._indexes2[_i];
                    byte _b = this._buffers[_s1][_p1];
                    this._buffers[_s1][_p1] = this._buffers[_s2][_p2];
                    this._buffers[_s2][_p2] = _b;
                }
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Done()
        {
            this._benchmarkendpoint.Interface._Done(true);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Benchmark_07_> Members

        void QS.Fx.Replication.IReplicated<Benchmark_07_>.Export(Benchmark_07_ _other)
        {
            _other._num = this._num;
            _other._size = this._size;
            _other._count = this._count;
            _other._batch = this._batch;
        }

        void QS.Fx.Replication.IReplicated<Benchmark_07_>.Import(Benchmark_07_ _other)
        {
            if (!this._imported)
            {
                this._imported = true;
                this._benchmarkendpoint.Interface._Done(false);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { throw new NotImplementedException(); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            throw new NotImplementedException();
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            throw new NotImplementedException();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
