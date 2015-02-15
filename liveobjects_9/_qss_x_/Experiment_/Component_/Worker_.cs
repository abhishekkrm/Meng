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
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("D4D7A5AEF1D642CE8CF4395D5FD2D6DE")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_Worker)]
    public sealed class Worker_ : QS.Fx.Inspection.Inspectable, 
        QS._qss_x_.Experiment_.Object_.IWorker_, QS._qss_x_.Experiment_.Interface_.IWorker_, QS.Fx.Replication.IReplicated<Worker_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Worker_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IWorkerClient_,
                    QS._qss_x_.Experiment_.Interface_.IWorker_>(this);
            this._name = "_";
            this._seqno = 0;
        }

        public Worker_()
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
            QS._qss_x_.Experiment_.Interface_.IWorkerClient_, 
                QS._qss_x_.Experiment_.Interface_.IWorker_> _workendpoint;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private StringBuilder _s = new StringBuilder();
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private List<int> _n = new List<int>();
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private string _name;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _seqno;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWorker_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWorkerClient_,
                QS._qss_x_.Experiment_.Interface_.IWorker_>
                    QS._qss_x_.Experiment_.Object_.IWorker_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWorker_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IWorker_._Work(string s)
        {
            if (_s.Length > 0)
                _s.Append(", ");
            _s.Append(s);
            this._n.Add(Convert.ToInt32(s));
            Thread.Sleep(0);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IWorker_._Done()
        {
            this._s.AppendLine();
            this._s.AppendLine("NUMBERS:");
            this._s.AppendLine();
            this._n.Sort();
            int _x = 0;
            bool _ok = true;
            foreach (int _y in this._n)
            {
                this._s.Append(_y.ToString());
                this._s.Append(" ");
                if (_y != (_x + 1))
                {
                    this._s.AppendLine();
                    this._s.AppendLine("********** ERROR: expecting " + (_x + 1).ToString() + " but got " + _y.ToString() + ".");
                    _ok = false;
                    break;
                }
                _x = _y;
            }
            if (_ok)
            {
                this._s.AppendLine();
                this._s.AppendLine("----------> ALL OK.");
            }
            this._workendpoint.Interface._Done(_s.ToString());
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Worker_> Members

        void QS.Fx.Replication.IReplicated<Worker_>.Export(Worker_ _other)
        {
            _other._name = this._name + "." + (++this._seqno).ToString();
            _other._seqno = 0;
        }

        void QS.Fx.Replication.IReplicated<Worker_>.Import(Worker_ _other)
        {
            _s.AppendLine();
            _s.AppendLine(_other._name);
            _s.AppendLine("{");
            using (StringReader _r = new StringReader(_other._s.ToString()))
            {
                string _line;
                while ((_line = _r.ReadLine()) != null)
                {
                    _s.Append("    ");
                    _s.AppendLine(_line);
                }
            }
            _s.AppendLine();
            _s.AppendLine("}");
            _other._s.Length = 0;
            this._n.AddRange(_other._n);
            _other._n.Clear();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort) QS.ClassID.Experiment_Component_Worker, 3 * sizeof(int), 3 * sizeof(int) + _s.Length + _name.Length, 2);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                *((int*) _pbuffer) = this._s.Length;
                *((int*) (_pbuffer + sizeof(int))) = this._name.Length;
                *((int*) (_pbuffer + 2 * sizeof(int))) = this._seqno;
            }
            _header.consume(3 * sizeof(int));
            _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(this._s.ToString())));
            _data.Add(new QS.Fx.Base.Block(Encoding.ASCII.GetBytes(this._name)));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            int _slength, _namelength;
            fixed (byte* _parray = _header.Array)
            {
                byte* _pbuffer = _parray + _header.Offset;
                _slength = *((int*)_pbuffer);
                _namelength = *((int*)(_pbuffer + sizeof(int)));
                this._seqno = *((int*)(_pbuffer + 2 * sizeof(int)));
            }
            _header.consume(3 * sizeof(int));
            this._s = new StringBuilder(Encoding.ASCII.GetString(_data.Array, _data.Offset, _slength));
            _data.consume(_slength);
            this._name = Encoding.ASCII.GetString(_data.Array, _data.Offset, _namelength);
            _data.consume(_namelength);
        }

        #endregion
    }
}
