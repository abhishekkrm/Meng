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

#define VERBOSE
#define STATISTICS

using System;
using System.Collections.Generic;
using System.Text;

#if NO
namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.RingAggregator, "Properties Framework Ring Aggregator")]
    public sealed class RingAggregator_<[QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass>
        : QS._qss_x_.Properties_.Component_.Ring_<MessageClass>,
        QS.Fx.Object.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>,
        QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public RingAggregator_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            [QS.Fx.Reflection.Parameter("MTTA", QS.Fx.Reflection.ParameterClass.Value)]
            double _mtta,
            [QS.Fx.Reflection.Parameter("MTTB", QS.Fx.Reflection.ParameterClass.Value)]
            double _mttb,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _group_reference, _rate, _mtta, _mttb, _debug) 
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_.Constructor");
#endif

            this._aggregator_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>,
                    QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>,
            QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>> _aggregator_endpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IAggregator Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IAggregatorClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>,
            QS.Fx.Interface.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>>
                QS.Fx.Object.Classes.IAggregator<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, MessageClass>.Aggregator
        {
            get { return this._aggregator_endpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected unsafe override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Dispose");
#endif

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Start");
#endif

            base._Start();
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Stop");
#endif

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/*
        #region _Process_0

        private void _Process_0()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Process_0 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif
        }

        #endregion

        #region _Process_1

        private void _Process_1()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Process_1 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.Token_(this._membership.Incarnation, this._index);

             // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }
        
        #endregion

        #region _Process_2

        private void _Process_2()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Process_2 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif

            this._outgoing_token = new QS._qss_x_.Properties_.Value_.Token_(this._membership.Incarnation, this._index);
        }

        #endregion

        #region _Process_3

        private void _Process_3()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.RingAggregator_._Process_3 ( "
                    + QS.Fx.Printing.Printable.ToString(this._membership.Incarnation) + ", " + QS.Fx.Printing.Printable.ToString(this._index) + " )");
#endif
        }

        #endregion
*/

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ 
    }
}
#endif
