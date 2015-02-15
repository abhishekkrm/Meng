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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("6A43557322A84BFAACB07DC66E7CE09C")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Benchmark_09)]
    public sealed class Benchmark_09_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IBenchmark_, QS._qss_x_.Experiment_.Interface_.IBenchmark_, QS.Fx.Replication.IReplicated<Benchmark_09_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Benchmark_09_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("mode (1 = binary, 2 = xml, 3 = internal)", QS.Fx.Reflection.ParameterClass.Value)]
            int _mode,
            [QS.Fx.Reflection.Parameter("size", QS.Fx.Reflection.ParameterClass.Value)]
            int _size,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
            : this(_mycontext)
        {
            this._mode = _mode;
            this._size = _size;
            this._count = _count;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmark_>(this);
        }

        public Benchmark_09_(QS.Fx.Object.IContext _mycontext)
            : this()
        {
            this._mycontext = _mycontext;
        }

        public Benchmark_09_()
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
        private int _mode;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _size;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private IDictionary<int, string> _elements;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private MemoryStream _memorystream;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private BinaryFormatter _binaryformatter;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private XmlSerializer _xmlserializer;

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
                this._elements = new Dictionary<int, string>(this._size);
                for (int _i = 1; _i <= this._size; _i++)
                    this._elements.Add(_i, "element(" + _i.ToString() + ")");
                switch (this._mode)
                {
                    case 1:
                        {
                            this._binaryformatter = new BinaryFormatter();
                            this._memorystream = new MemoryStream();
                        }
                        break;

                    case 2:
                        {
                            this._xmlserializer = new XmlSerializer(typeof(IDictionary<int, string>));
                            this._memorystream = new MemoryStream();
                        }
                        break;

                    case 3:
                    default:
                        throw new NotImplementedException();
                }
            }
            switch (this._mode)
            {
                case 1:
                    {
                        for (int _j = 0; _j < this._count; _j++)
                        {
                            this._binaryformatter.Serialize(this._memorystream, this._elements);
                            this._memorystream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                    break;

                case 2:
                    {
                        for (int _j = 0; _j < this._count; _j++)
                        {
                            this._xmlserializer.Serialize(this._memorystream, this._elements);
                            this._memorystream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                    break;

                case 3:
                default:
                    throw new NotImplementedException();
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Done()
        {
            this._benchmarkendpoint.Interface._Done(true);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Benchmark_09_> Members

        void QS.Fx.Replication.IReplicated<Benchmark_09_>.Export(Benchmark_09_ _other)
        {
            _other._mode = this._mode;
            _other._size = this._size;
            _other._count = this._count;
        }

        void QS.Fx.Replication.IReplicated<Benchmark_09_>.Import(Benchmark_09_ _other)
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
