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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("E376CC26889A407AA36196C130F28B3B", "MapReduce")]
    [QS.Fx.Base.Synchronization(
        QS.Fx.Base.SynchronizationOption.Asynchronous | 
        QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class MapReduce_ : QS.Fx.Inspection.Inspectable, 
        QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IMap2Client_
    {
        #region Constructor

        public MapReduce_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Map", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMap2_> _mapreference,
            [QS.Fx.Reflection.Parameter("Reduce", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReduce2_> _reducereference,
            [QS.Fx.Reflection.Parameter("Concurrency", QS.Fx.Reflection.ParameterClass.Value)]
            int _concurrency)
        {
            this._mycontext = _mycontext;
            this._mapreference = _mapreference;
            this._reducereference = _reducereference;
            this._concurrency = _concurrency;
            this._mapproxy = this._mapreference.Dereference(this._mycontext);
            this._mapendpoint = this._mycontext.DualInterface<QS._qss_x_.Experiment_.Interface_.IMap2_, QS._qss_x_.Experiment_.Interface_.IMap2Client_>(this);
            this._mapconnection = this._mapendpoint.Connect(this._mapproxy.Map);
            this._reducers = new Reduce_[this._concurrency];
            for (int _i = 0; _i < this._concurrency; _i++)
                this._reducers[_i] = new Reduce_(this, _i, this._reducereference);
            this._reducerindex = 0;

            // TODO: here we initiate some maps...
            for (int _i = 0; _i < 100; _i++)
                this._mapendpoint.Interface.Map("");
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IMap2_> _mapreference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReduce2_> _reducereference;
        [QS.Fx.Base.Inspectable]
        private int _concurrency;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IMap2_ _mapproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Experiment_.Interface_.IMap2_, QS._qss_x_.Experiment_.Interface_.IMap2Client_> _mapendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _mapconnection;
        [QS.Fx.Base.Inspectable]
        private Reduce_[] _reducers;
        [QS.Fx.Base.Inspectable]
        private int _reducerindex;

        #endregion

        #region Class Reduce_

        private sealed class Reduce_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Experiment_.Interface_.IReduce2Client_
        {
            #region Constructor

            public Reduce_(MapReduce_ _owner, int _index, QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IReduce2_> _reducereference)
            {
                this._owner = _owner;
                this._index = _index;
                this._reduceproxy = _reducereference.Dereference(this._owner._mycontext);
                this._reduceendpoint = this._owner._mycontext.DualInterface<QS._qss_x_.Experiment_.Interface_.IReduce2_, QS._qss_x_.Experiment_.Interface_.IReduce2Client_>(this);
                this._reduceconnection = this._reduceendpoint.Connect(this._reduceproxy.Reduce);
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            public MapReduce_ _owner;
            [QS.Fx.Base.Inspectable]
            public int _index;
            [QS.Fx.Base.Inspectable]
            public QS._qss_x_.Experiment_.Object_.IReduce2_ _reduceproxy;
            [QS.Fx.Base.Inspectable]
            public QS.Fx.Endpoint.Internal.IDualInterface<QS._qss_x_.Experiment_.Interface_.IReduce2_, QS._qss_x_.Experiment_.Interface_.IReduce2Client_> _reduceendpoint;
            [QS.Fx.Base.Inspectable]
            public QS.Fx.Endpoint.IConnection _reduceconnection;

            #endregion

            #region IReduce2Client_ Members

            [QS.Fx.Base.Synchronization(
                QS.Fx.Base.SynchronizationOption.Asynchronous |
                QS.Fx.Base.SynchronizationOption.Multithreaded |
                QS.Fx.Base.SynchronizationOption.Serialized)]
            void QS._qss_x_.Experiment_.Interface_.IReduce2Client_.Reduced(QS._qss_x_.Experiment_.Value_.MapReduce_Dict_ _reduced)
            {
                int _index = (this._index + 1 ) % this._owner._reducers.Length;
                if (_index > 0)
                {
                    this._owner._reducers[_index]._reduceendpoint.Interface.Reduce(_reduced);
                    this._owner._reducers[_index]._reduceendpoint.Interface.Reduce(null);
                }
                else
                    this._owner._Completed(_reduced);
            }

            #endregion
        }

        #endregion

        #region IMap2Client_ Members

        [QS.Fx.Base.Synchronization(
            QS.Fx.Base.SynchronizationOption.Asynchronous | 
            QS.Fx.Base.SynchronizationOption.Multithreaded | 
            QS.Fx.Base.SynchronizationOption.Serialized)]
        void QS._qss_x_.Experiment_.Interface_.IMap2Client_.Mapped(QS._qss_x_.Experiment_.Value_.MapReduce_Dict_ _mapped)
        {
            this._reducers[this._reducerindex]._reduceendpoint.Interface.Reduce(_mapped);
            this._reducerindex = (this._reducerindex + 1) % this._reducers.Length;

            // TODO: here we initiate some more maps...

            if (false) // TODO: detect when no more maps left to initiate
            {
                this._reducers[0]._reduceendpoint.Interface.Reduce(null); // start flushing in the first reducer
            }
        }

        #endregion

        #region _Completed

        private void _Completed(QS._qss_x_.Experiment_.Value_.MapReduce_Dict_ _result)
        {
            // TODO: consume the result
        }

        #endregion
    }
}
