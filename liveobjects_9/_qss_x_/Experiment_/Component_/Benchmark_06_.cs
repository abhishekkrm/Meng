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
    [QS.Fx.Reflection.ComponentClass("9AF5F52BA306431CAC87A7ED0D0BA147")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Benchmark_06)]
    public sealed class Benchmark_06_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IBenchmark_, QS._qss_x_.Experiment_.Interface_.IBenchmark_, QS.Fx.Replication.IReplicated<Benchmark_06_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Benchmark_06_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)]
            int _count) : this(_mycontext)
        {
            this._count = _count;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IBenchmarkClient_,
                    QS._qss_x_.Experiment_.Interface_.IBenchmark_>(this);
        }

        public Benchmark_06_(QS.Fx.Object.IContext _mycontext) : this()
        {
            this._mycontext = _mycontext;
        }

        public Benchmark_06_()
        {
            this._receivecallback = new AsyncCallback(this._ReceiveCallback);
            foreach (IPAddress _ipaddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    this._ipaddress = _ipaddress;
                    break;
                }
            }
            if (this._ipaddress == null)
                throw new Exception("Cannot allocate any IP address.");
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
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private Socket _socket;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IPAddress _ipaddress;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private IPEndPoint _endpoint = new IPEndPoint(IPAddress.Any, 0);
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private AsyncCallback _receivecallback;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private byte[] _buffer = new byte[1];
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _count;

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
            for (int _i = 0; _i < this._count; _i++)
            {
                this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                this._endpoint.Address = this._ipaddress;
                this._endpoint.Port = 0;
                this._socket.Bind(this._endpoint);
                this._socket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, this._receivecallback, null);
                this._socket.Close();
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IBenchmark_._Done()
        {
            this._benchmarkendpoint.Interface._Done(true);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region _ReceiveCallback

        private void _ReceiveCallback(IAsyncResult _asyncresult)
        {
            // this._socket.EndReceive(_asyncresult);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Benchmark_06_> Members

        void QS.Fx.Replication.IReplicated<Benchmark_06_>.Export(Benchmark_06_ _other)
        {
            _other._count = this._count;
        }

        void QS.Fx.Replication.IReplicated<Benchmark_06_>.Import(Benchmark_06_ _other)
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
