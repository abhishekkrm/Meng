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
using System.Runtime.CompilerServices;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("DEA9C113A2D443198B0BE13D0D3B2498")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PriorityQueue_2)]
    public sealed class PriorityQueue_2_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IPriorityQueue_, QS._qss_x_.Experiment_.Interface_.IPriorityQueue_, QS.Fx.Replication.IReplicated<PriorityQueue_2_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PriorityQueue_2_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("warmup", QS.Fx.Reflection.ParameterClass.Value)]
            int _warmup) : this(_mycontext)
        {
            this._warmup = _warmup;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_,
                    QS._qss_x_.Experiment_.Interface_.IPriorityQueue_>(this);
            this._splaytree = new QS._core_c_.Core.SplayTree<double, QS._core_c_.Core.Alarm>();
            this._random = new Random();
            for (int _i = 0; _i < this._warmup; _i++)
            {
                double _time = this._random.NextDouble();
                QS._core_c_.Core.Alarm _alarm = new QS._core_c_.Core.Alarm(null, _time, 0, null, null);
                ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>) this._splaytree).Enqueue(_alarm);
            }
        }

        public PriorityQueue_2_(QS.Fx.Object.IContext _mycontext) : this()
        {
            this._mycontext = _mycontext;
        }

        public PriorityQueue_2_()
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
            QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_,
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_> _benchmarkendpoint;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private int _warmup;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _isreplica;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private Random _random;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Core.SplayTree<double, QS._core_c_.Core.Alarm> _splaytree;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Core.Alarm _todo1;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private QS._core_c_.Core.Alarm _todo2;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPriorityQueue_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_,
                QS._qss_x_.Experiment_.Interface_.IPriorityQueue_>
                    QS._qss_x_.Experiment_.Object_.IPriorityQueue_._Queue
        {
            get { return this._benchmarkendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IPriorityQueue_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Synchronous)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Warmup(int _warmup)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time)
        {
            if (this._isreplica)
            {
                QS._core_c_.Core.Alarm _newalarm = new QS._core_c_.Core.Alarm(null, _time, 0, null, null);
                QS._core_c_.Core.Alarm _oldalarm;
                QS._core_c_.Core.BST<double, QS._core_c_.Core.Alarm>.SearchResult _searchresult;
                this._splaytree.BinarySearch(_time, out _oldalarm, out _searchresult);
                ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>)_newalarm).Left = this._todo2;
                ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>)_newalarm).Right = null;
                ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>)_newalarm).Parent = _oldalarm;
                if (this._todo2 != null)
                    ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>) this._todo2).Right = _newalarm;
                else
                    this._todo1 = _newalarm;
                this._todo2 = _newalarm;
            }
            else
            {
                QS._core_c_.Core.Alarm _alarm = new QS._core_c_.Core.Alarm(null, _time, 0, null, null);
                ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)this._splaytree).Enqueue(_alarm);
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time1, double _time2)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time1, double _time2, double _time3, double _time4)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16)
        { 
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16,
            double _time17, double _time18, double _time19, double _time20, double _time21, double _time22, double _time23, double _time24,
            double _time25, double _time26, double _time27, double _time28, double _time29, double _time30, double _time31, double _time32)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16,
            double _time17, double _time18, double _time19, double _time20, double _time21, double _time22, double _time23, double _time24,
            double _time25, double _time26, double _time27, double _time28, double _time29, double _time30, double _time31, double _time32,
            double _time33, double _time34, double _time35, double _time36, double _time37, double _time38, double _time39, double _time40,
            double _time41, double _time42, double _time43, double _time44, double _time45, double _time46, double _time47, double _time48,
            double _time49, double _time50, double _time51, double _time52, double _time53, double _time54, double _time55, double _time56,
            double _time57, double _time58, double _time59, double _time60, double _time61, double _time62, double _time63, double _time64)
        { 
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Dequeue(int _count)
        {
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Done()
        {
            this._benchmarkendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<PriorityQueue_> Members

        void QS.Fx.Replication.IReplicated<PriorityQueue_2_>.Export(PriorityQueue_2_ _other)
        {
            _other._isreplica = true;
            _other._splaytree = this._splaytree;
        }

        void QS.Fx.Replication.IReplicated<PriorityQueue_2_>.Import(PriorityQueue_2_ _other)
        {
            if (this._isreplica || !_other._isreplica)
                throw new Exception("Import is nonly legal between a master and a child replica.");
            QS._core_c_.Core.Alarm _newalarm = _other._todo1;
            while (_newalarm != null)
            {
                QS._core_c_.Core.Alarm _nextalarm = ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>)_newalarm).Right;
                QS._core_c_.Core.Alarm _oldalarm = ((QS._core_c_.Core.IBSTNode<double, QS._core_c_.Core.Alarm>)_newalarm).Parent;

                //                 this._splaytree.InsertAt(_newalarm, _oldalarm);
            }

            //QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm> _t1 = (QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm>)this._splaytree;
            //QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm> _t2 = (QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm>)_other._splaytree;
            //if (_t2.Count < _t1.Count)
            //    _t1.Merge(_t2);
            //else
            //{
            //    _t2.Merge(_t1);
            //    this._splaytree = _other._splaytree;
            //}
            //_other._splaytree = null;
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
    }
}
