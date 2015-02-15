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


//#define PROFILE_ADD

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("52ABE30CF73F4c59B75EC6E1DEFB4093", "EmptyDictionary_")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded |
        QS.Fx.Base.SynchronizationOption.Replicated)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Experiment_Component_EmptyDictionary)]
    [Serializable]
    public sealed class EmptyDictionary_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Object_.IDictionary_,
        QS._qss_x_.Experiment_.Interface_.IDictionary_, QS.Fx.Replication.IReplicated<EmptyDictionary_>,
        QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public EmptyDictionary_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("# Iterations", QS.Fx.Reflection.ParameterClass.Value)]
            int _num_iterations
        ,
            [QS.Fx.Reflection.Parameter("Count",QS.Fx.Reflection.ParameterClass.Value)]
            int _count)
        {
            this._mycontext = _mycontext;
            this._workendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                    QS._qss_x_.Experiment_.Interface_.IDictionary_>(this);
            this._num_iterations = _num_iterations;
            this._clock = _mycontext.Platform.Clock;
            this._count = _count;
#if PROFILE_ADD
            this._add_samples = new QS._qss_e_.Data_.FastStatistics2D_(_count);
            this._replica_add_samples = new List<QS._qss_e_.Data_.FastStatistics2D_>();
#endif
            this._mycontext.Platform.Logger.Log("Pointless iterations per add call: " + _num_iterations);
            
        }

        public EmptyDictionary_()
        {

        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields
        [QS.Fx.Base.Inspectable]
        private int _count;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IClock _clock;
        [QS.Fx.Base.Inspectable]
        [NonSerialized]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IDictionaryClient_,
                QS._qss_x_.Experiment_.Interface_.IDictionary_> _workendpoint;

        [QS.Fx.Base.Inspectable]
        private int _added = 0;
        [QS.Fx.Base.Inspectable]
        private int _num_iterations = -1;

        [QS.Fx.Base.Inspectable]
        public QS._qss_e_.Data_.FastStatistics2D_ ReplicaSchedSamples
        {
            get
            {
                QS._qss_e_.Data_.StatisticsInternalHelper_ _h = new QS._qss_e_.Data_.StatisticsInternalHelper_(_mycontext);
                return _h._ReplicaStats(0);
            }
        }
#if PROFILE_ADD
        [QS.Fx.Base.Inspectable]
        
        private QS._qss_e_.Data_.FastStatistics2D_ _add_samples;
        [QS.Fx.Base.Inspectable]
        public IList<QS._qss_e_.Data_.FastStatistics2D_> _replica_add_samples;
#endif
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS._qss_x_.Experiment_.Interface_.IDictionaryClient_, QS._qss_x_.Experiment_.Interface_.IDictionary_> QS._qss_x_.Experiment_.Object_.IDictionary_._Work
        {
            get { return this._workendpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IDictionary_ Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Add(string _key, string _value)
        {
#if PROFILE_ADD
            double _t1 = this._clock.Time;
#endif
            if (_num_iterations < 0)
            {
                throw new Exception("Didn't set # iterations on replicas");
            }
            _added++;
            for (int i = 0; i < _num_iterations; i++)
            {
                _added++;
            }
            _added -= _num_iterations;
#if PROFILE_ADD
            double _t2 = this._clock.Time;
            this._add_samples.Add(_t1, _t2 - _t1);
#endif
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Get(string _key)
        {
            this._workendpoint.Interface._Got(string.Empty, string.Empty);
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Clear()
        {
            this._workendpoint.Interface._Cleared();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Count()
        {
            this._workendpoint.Interface._Counted(_added);
        }
        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._DumpStats()
        {
#if PROFILE_ADD
            if (this._replica_add_samples.Count == 8)
            {
                double _min_x = double.MaxValue;
                double _max_x = 0;
                foreach (QS._qss_e_.Data_.FastStatistics2D_ _s in this._replica_add_samples)
                {
                    foreach (QS._core_e_.Data.XY _xy in _s.Samples)
                    {
                        if (_xy.x + _xy.y > _max_x)
                        {
                            _max_x = _xy.x + _xy.y;
                        }
                        if (_xy.x < _min_x)
                        {
                            _min_x = _xy.x;
                        }
                    }
                }
                this._mycontext.Platform.Logger.Log("Add duration: " + (_max_x - _min_x).ToString());
            }
#endif
        }
        #region NotImplemented

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddFromFile(string _path, int start, int length)
        {
            throw new NotImplementedException();

        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Aggregated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._Set_Ratio(double _ratio)
        {
            throw new NotImplementedException();
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded | QS.Fx.Base.SynchronizationOption.Replicated)]
        void QS._qss_x_.Experiment_.Interface_.IDictionary_._AddMultiple(string[] _key, string[] _value)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IReplicated<Dictionary_> Members

        void QS.Fx.Replication.IReplicated<EmptyDictionary_>.Export(EmptyDictionary_ _other)
        {
            _other._count = this._count;
            _other._added = 0;
            _other._num_iterations = this._num_iterations;
#if PROFILE_ADD
            _other._clock = this._clock;
            _other._add_samples = new QS._qss_e_.Data_.FastStatistics2D_(_count);
#endif
            //_other._dict.Clear();
        }



        void QS.Fx.Replication.IReplicated<EmptyDictionary_>.Import(EmptyDictionary_ _other)
        {
            this._added += _other._added;
#if PROFILE_ADD
            this._replica_add_samples.Add(_other._add_samples);
            
#endif

        }



        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                throw new NotImplementedException();
            }

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
