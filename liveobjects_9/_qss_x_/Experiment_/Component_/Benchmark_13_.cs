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
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("E318FF4FD75A4DF394F8213FF3B23A16")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Benchmark_13)]
    public sealed class Benchmark_13_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IBenchmark_, QS._qss_x_.Experiment_.Interface_.IBenchmark_, QS.Fx.Replication.IReplicated<Benchmark_13_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Benchmark_13_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
            : this(_mycontext)
        {
            this._count = _count;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmark_>(this);
            this._port = 10000;
        }

        public Benchmark_13_(QS.Fx.Object.IContext _mycontext)
            : this()
        {
            this._mycontext = _mycontext;
        }

        public Benchmark_13_()
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
        private int _count;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private ServiceHost _servicehost;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private Foo _service;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _port;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _seqno;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private string _address;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private ChannelFactory<IFoo> _channelfactory;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private IFoo _proxy;

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

        #region Interface IFoo

        [ServiceContract]
        public interface IFoo
        {
            [OperationContract]
            void foo();
        }

        #endregion

        #region Class Foo

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        public sealed class Foo : IFoo, IDisposable
        {
            public Foo()
            {
            }

            void IFoo.foo()
            {
            }

            void IDisposable.Dispose()
            {
            }
        }

        #endregion

        #region _Close

        private void _Close()
        {
            if (this._servicehost != null)
            {
                this._servicehost.Close();
                if (this._servicehost is IDisposable)
                    ((IDisposable)this._servicehost).Dispose();
                this._servicehost = null;
            }
            if (this._service != null)
            {
                if (this._service is IDisposable)
                    ((IDisposable) this._service).Dispose();
                this._service = null;
            }
            if (this._proxy != null)
            {
                if (this._proxy is IDisposable)
                    ((IDisposable)this._proxy).Dispose();
                this._proxy = null;
            }
            if (this._channelfactory != null)
            {
                this._channelfactory.Close();
                if (this._channelfactory is IDisposable)
                    ((IDisposable)this._channelfactory).Dispose();
                this._channelfactory = null;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IBenchmark_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Work()
        {
            if (!this._initialized)
            {
                this._initialized = true;
                this._service = new Foo();
                this._address = "http://localhost:" + this._port.ToString() + "/foo";
                this._servicehost = new ServiceHost(this._service);
                this._servicehost.AddServiceEndpoint(typeof(IFoo), new WSHttpBinding(SecurityMode.None), this._address);
                this._servicehost.Open();
                this._channelfactory = new ChannelFactory<IFoo>(new WSHttpBinding(SecurityMode.None), this._address);
                this._proxy = this._channelfactory.CreateChannel();
            }
            for (int _i = 0; _i < this._count; _i++)
                this._proxy.foo();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Done()
        {
            this._Close();
            this._benchmarkendpoint.Interface._Done(true);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Benchmark_13_> Members

        void QS.Fx.Replication.IReplicated<Benchmark_13_>.Export(Benchmark_13_ _other)
        {
            _other._count = this._count;
            _other._port = 10000 + (++this._seqno);
        }

        void QS.Fx.Replication.IReplicated<Benchmark_13_>.Import(Benchmark_13_ _other)
        {
            _other._Close();
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
