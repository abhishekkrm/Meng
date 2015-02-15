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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Scenarios_
{
    public sealed class Scenario_Foo_ : QS._qss_x_.Simulations_.IScenario_
    {
        #region _Create

        QS._qss_x_.Simulations_.ITask_[] QS._qss_x_.Simulations_.IScenario_._Create(int _nnodes, IDictionary<string, QS.Fx.Reflection.Xml.Parameter> _parameters)
        {
            List<QS._qss_x_.Simulations_.ITask_> _tasks = new List<QS._qss_x_.Simulations_.ITask_>();
            if (_nnodes < 2)
                throw new Exception("Not enough nodes.");

            double _mttb = (double)_parameters["MTTB"].Value;
            double _mttr = (double)_parameters["MTTR"].Value;
            double _mttf = (double)_parameters["MTTF"].Value;

            _tasks.Add(new QS._qss_x_.Simulations_.Task_("controller", 0, double.PositiveInfinity, 0, _controller()));
            
            for (int _i = 1; _i <= _nnodes; _i++)
                _tasks.Add(new QS._qss_x_.Simulations_.Task_(
                    ("node_" + _i.ToString("00")), _mttb, _mttf, _mttr, _node()));
            
            return _tasks.ToArray();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _controller

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _controller()
        {
            return _membership();
        }

        #endregion

        #region _node

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _node()
        {
            return _tree("controller");
        }

        #endregion

        #region _aggregation

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _aggregation(string _controller)
        {
/*
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.AggregationComponent,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("group", _group(_controller)),
                QS.Fx.Reflection.Parameter.Value<double>("rate", 1d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", 0.2d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", 1d),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", true));
*/
            throw new NotImplementedException();
        }

        #endregion

        #region _tree

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _tree(string _controller)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.TreeAggregator,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("group", _group(_controller)),
                QS.Fx.Reflection.Parameter.Value<int>("fanout", 2),
                QS.Fx.Reflection.Parameter.Value<double>("rate", 1d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", 0.2d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", 1d),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", true));
        }

        #endregion

        #region _ring

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _ring(string _controller)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.RingAggregator,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("group", _group(_controller)),
                QS.Fx.Reflection.Parameter.Value<double>("rate", 1d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", 0.2d),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", 1d),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", true));
        }

        #endregion

        #region _group

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _group(string _controller)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Group,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ValueClass("IncarnationClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Incarnation>()),
                QS.Fx.Reflection.Parameter.ValueClass("NameClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Name>()),
                QS.Fx.Reflection.Parameter.ValueClass("AddressClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Address>()),
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("membership", _membership(_controller)),
                QS.Fx.Reflection.Parameter.Object("transport", _transport()),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", false));
        }

        #endregion

        #region _membership

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _membership(string _controller)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.MembershipChannel,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ValueClass("IncarnationClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Incarnation>()),
                QS.Fx.Reflection.Parameter.ValueClass("NameClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Name>()),
                QS.Fx.Reflection.Parameter.ValueClass("AddressClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Address>()),
                QS.Fx.Reflection.Parameter.Object("transport", _transport()),
                QS.Fx.Reflection.Parameter.Value<QS.Fx.Base.Address>("address", new QS.Fx.Base.Address(_controller, 1000)),
                QS.Fx.Reflection.Parameter.Object("memory", null),
                QS.Fx.Reflection.Parameter.Value<double>("batching", 0d),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", false));
        }

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _membership()
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.MembershipChannel,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ValueClass("IncarnationClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Incarnation>()),
                QS.Fx.Reflection.Parameter.ValueClass("NameClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Name>()),
                QS.Fx.Reflection.Parameter.ValueClass("AddressClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Address>()),
                QS.Fx.Reflection.Parameter.Object("transport", _transport("0.0.0.0/0:1000")),
                QS.Fx.Reflection.Parameter.Value<QS.Fx.Base.Address>("address", null),
                QS.Fx.Reflection.Parameter.Object("memory", _loopback()),
                QS.Fx.Reflection.Parameter.Value<double>("batching", 0.5d),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", true));

        }

        #endregion

        #region _transport

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _transport(string _address)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.UnreliableTransport,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.Value<string>("address", _address),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", false));
        }

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _transport()
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.UnreliableTransport,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.Value<string>("address", "0.0.0.0/0:0"),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", false));
        }

        #endregion

        #region _loopback

        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _loopback()
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.LocalCommunicationChannel,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.ValueClass("CheckpointClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.Value<string>("address", null),
                QS.Fx.Reflection.Parameter.Value<bool>("debugging", false));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
