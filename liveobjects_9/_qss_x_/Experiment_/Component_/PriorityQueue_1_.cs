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

// #define REPORT_IMPORTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.CompilerServices;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("F4357DF1D52246EA8AE18A408260F900")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS._qss_x_.Reflection_.Internal]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    [QS.Fx.Serialization.ClassID(QS.ClassID.PriorityQueue_1)]
    public sealed class PriorityQueue_1_ : QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Experiment_.Object_.IPriorityQueue_, QS._qss_x_.Experiment_.Interface_.IPriorityQueue_, QS.Fx.Replication.IReplicated<PriorityQueue_1_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public PriorityQueue_1_(QS.Fx.Object.IContext _mycontext)        
        {
            this._name = "master";
            this._ismaster = true;
            this._mycontext = _mycontext;
            this._benchmarkendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IPriorityQueueClient_,
                    QS._qss_x_.Experiment_.Interface_.IPriorityQueue_>(this);
        }

        public PriorityQueue_1_()
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
        private QS._core_c_.Core.SplayTree<double, QS._core_c_.Core.Alarm> _splaytree = new QS._core_c_.Core.SplayTree<double, QS._core_c_.Core.Alarm>();
        private Random _random;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private string _name;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _ismaster;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private int _seqno;
        [QS.Fx.Base.Inspectable]
        [QS.Fx.Printing.Printable]
        private bool _importing;

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
            this._warmup = _warmup;
            this._random = new Random();
            for (int _i = 0; _i < this._warmup; _i++)
            {
                double _time = _random.NextDouble();
                QS._core_c_.Core.Alarm _alarm = new QS._core_c_.Core.Alarm(null, _time, 0, null, null);
                ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(_alarm);
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time1, double _time2)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(double _time1, double _time2, double _time3, double _time4)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time3, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time4, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time3, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time4, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time5, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time6, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time7, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time8, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time3, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time4, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time5, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time6, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time7, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time8, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time9, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time10, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time11, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time12, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time13, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time14, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time15, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time16, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Enqueue(
            double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16,
            double _time17, double _time18, double _time19, double _time20, double _time21, double _time22, double _time23, double _time24,
            double _time25, double _time26, double _time27, double _time28, double _time29, double _time30, double _time31, double _time32)
        {
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time3, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time4, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time5, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time6, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time7, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time8, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time9, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time10, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time11, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time12, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time13, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time14, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time15, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time16, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time17, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time18, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time19, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time20, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time21, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time22, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time23, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time24, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time25, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time26, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time27, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time28, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time29, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time30, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time31, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time32, 0, null, null));
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
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time1, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time2, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time3, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time4, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time5, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time6, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time7, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time8, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time9, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time10, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time11, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time12, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time13, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time14, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time15, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time16, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time17, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time18, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time19, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time20, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time21, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time22, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time23, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time24, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time25, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time26, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time27, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time28, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time29, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time30, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time31, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time32, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time33, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time34, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time35, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time36, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time37, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time38, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time39, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time40, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time41, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time42, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time43, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time44, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time45, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time46, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time47, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time48, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time49, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time50, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time52, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time52, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time53, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time54, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time55, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time56, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time57, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time58, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time59, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time60, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time61, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time62, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time63, 0, null, null));
            ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Enqueue(new QS._core_c_.Core.Alarm(null, _time64, 0, null, null));
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Dequeue(int _count)
        {
            for (int _k = 0; _k < _count; _k++)
                ((QS._core_c_.Core.IPriorityQueue<QS._core_c_.Core.Alarm>)_splaytree).Dequeue();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IPriorityQueue_._Done()
        {
            this._benchmarkendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<PriorityQueue_> Members

        void QS.Fx.Replication.IReplicated<PriorityQueue_1_>.Export(PriorityQueue_1_ _other)
        {
            this._seqno++;
            _other._name = ((this._ismaster) ? "copy" : this._name) + "." + this._seqno.ToString();
            _other._ismaster = false;
        }

        void QS.Fx.Replication.IReplicated<PriorityQueue_1_>.Import(PriorityQueue_1_ _other)
        {
            if (this._ismaster && !this._importing)
            {
                this._importing = true;
                this._benchmarkendpoint.Interface._Importing();
            }
#if REPORT_IMPORTS
            this._mycontext.Platform.Scheduler.Schedule(
                new QS.Fx.Base.Event(
                    new QS.Fx.Base.ContextCallback(
                        delegate(object _o) { this._mycontext.Platform.Logger.Log("import ( " + this._name + " <= " + _other._name + " ) "); })));
#endif
            QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm> _t1 = (QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm>) this._splaytree;
            QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm> _t2 = (QS._core_c_.Core.IBST<double, QS._core_c_.Core.Alarm>) _other._splaytree;
            if (_t2.Count < _t1.Count)
                _t1.Merge(_t2);
            else
            {
                _t2.Merge(_t1);
                this._splaytree = _other._splaytree;
            }
            _other._splaytree = new QS._core_c_.Core.SplayTree<double, QS._core_c_.Core.Alarm>();
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
