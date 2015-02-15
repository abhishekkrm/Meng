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
    [QS.Fx.Reflection.ComponentClass("6EBB3D5D4897408A90A5C43E57C29D32")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Benchmark_08)]
    public sealed class Benchmark_08_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IBenchmark_, QS._qss_x_.Experiment_.Interface_.IBenchmark_, QS.Fx.Replication.IReplicated<Benchmark_08_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Benchmark_08_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("reuse", QS.Fx.Reflection.ParameterClass.Value)]
            bool _reuse,
            [QS.Fx.Reflection.Parameter("length", QS.Fx.Reflection.ParameterClass.Value)]
            int _length,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
            : this(_mycontext)
        {
            if (((_length % _count) != 0) && ((_count % _length) != 0))
                throw new Exception("Between the length and the count, one must be a multiple of the other.");
            this._reuse = _reuse;
            this._length = _length;
            this._count = _count;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmark_>(this);
        }

        public Benchmark_08_(QS.Fx.Object.IContext _mycontext)
            : this()
        {
            this._mycontext = _mycontext;
        }

        public Benchmark_08_()
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
            QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                QS._qss_x_.Experiment_.Interface_.IBenchmark_> _benchmarkendpoint;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _imported;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _reuse;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _length;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private IDictionary<int, int> _elements;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _n1;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _n2;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _n3;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _c1;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _c2;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _cycle;

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
                if (this._reuse)
                    this._elements = new Dictionary<int, int>(this._length);
                else
                    this._elements = new Dictionary<int, int>();
                if (this._length <= this._count)
                {
                    this._n1 = this._length;
                    this._n2 = this._count / this._n1;
                    this._n3 = 1;
                    this._c1 = true;
                    this._c2 = false;
                }
                else
                {
                    this._n1 = this._count;
                    this._n2 = 1;
                    this._n3 = this._length / this._n1;
                    this._c1 = false;
                    this._c2 = true;
                }
            }
            int _x = this._elements.Count;
            for (int _i = 0; _i < this._n2; _i++)
            {
                for (int _j = 0; _j < this._n1; _j++)
                    this._elements.Add(_x++, 0);
                if (this._c1)
                {
                    if (this._reuse)
                        this._elements.Clear();
                    else
                        this._elements = new Dictionary<int, int>();
                }
            }
            this._cycle++;
            if (this._cycle >= this._n3)
            {
                this._cycle = 0;
                if (this._c2)
                {
                    if (this._reuse)
                        this._elements.Clear();
                    else
                        this._elements = new Dictionary<int, int>();
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

        #region IReplicated<Benchmark_08_> Members

        void QS.Fx.Replication.IReplicated<Benchmark_08_>.Export(Benchmark_08_ _other)
        {
            _other._reuse = this._reuse;
            _other._length = this._length;
            _other._count = this._count;
        }

        void QS.Fx.Replication.IReplicated<Benchmark_08_>.Import(Benchmark_08_ _other)
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
