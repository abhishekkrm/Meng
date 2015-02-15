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
    public static class Scenario_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _DataFlow
        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _DataFlow(string rules, string name)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.DataFlow,
                QS.Fx.Reflection.Parameter.Value<string>("rules", rules),
                QS.Fx.Reflection.Parameter.Value<string>("name", name));
        }

        #endregion

        #region _DelegationClient
        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _DelegationClient(
            QS.Fx.Base.Identifier _identifier,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _delegationchannel,
            string name,
            bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.DAClient,
                QS.Fx.Reflection.Parameter.Value<QS.Fx.Base.Identifier>("identifier", _identifier),
                QS.Fx.Reflection.Parameter.Object("delegationchannel", _delegationchannel),
                QS.Fx.Reflection.Parameter.Value<string>("name", name),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }
        #endregion

        #region TestDataFlowClient
        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _TestDataFlowClient(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _dataflowchannel,
            bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.TestDataFlowClient2,
                QS.Fx.Reflection.Parameter.Object("daclient_reference", _dataflowchannel),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }
        #endregion

        #region _DelegationChannel
        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _DelegationChannel(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _loader,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _transport,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _protocolstack, 
            QS.Fx.Base.Address _address,
            bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.DelegationChannel_1,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ObjectClass("ObjectClass", QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IObject>()),
                QS.Fx.Reflection.Parameter.Object("xmlloader", _loader),
                QS.Fx.Reflection.Parameter.Object("transport", _transport),
                QS.Fx.Reflection.Parameter.Object("protocolstack", _protocolstack),
                QS.Fx.Reflection.Parameter.Value<QS.Fx.Base.Address>("address", _address),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }
        #endregion
        
        #region DAProtocolStack
        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _DAProtocolStack(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> agg_dataflow_reference,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> groupReference,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> parent_client_reference,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            double _mtta,
            double _mttb,
            string name,
            bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.DAProtocolStackManager,
                QS.Fx.Reflection.Parameter.Object("agg_dataflow_channel", agg_dataflow_reference),
                QS.Fx.Reflection.Parameter.Object("group", groupReference),
                QS.Fx.Reflection.Parameter.Object("parent_daclient", parent_client_reference),
                QS.Fx.Reflection.Parameter.Value<double>("rate", _rate),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", _mtta),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", _mttb),
                QS.Fx.Reflection.Parameter.Value<string>("name", name),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }
        #endregion


        #region _Tree

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Tree(
            int _fanout, double _rate, double _mtta, double _mttb, bool _debug, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _group)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Tree_0_,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("group", _group),
                QS.Fx.Reflection.Parameter.Value<int>("fanout", _fanout),
                QS.Fx.Reflection.Parameter.Value<double>("rate", _rate),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", _mtta),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", _mttb),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _UnreliableTreeDisseminationChannel

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _UnreliableTreeDisseminationChannel(
            int _fanout, double _rate, double _mtta, double _mttb, bool _debug, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _group)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.UnreliableTreeDisseminationChannel,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Value.Classes.IText>()),
                QS.Fx.Reflection.Parameter.ValueClass("CheckpointClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Value.Classes.IText>()),
                QS.Fx.Reflection.Parameter.Object("group", _group),
                QS.Fx.Reflection.Parameter.Value<int>("fanout", _fanout),
                QS.Fx.Reflection.Parameter.Value<double>("rate", _rate),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", _mtta),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", _mttb),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _LocalHierarchicalAgent

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _LocalHierarchicalAgent(
            //double _rate, double _mtta, double _mttb, bool _debug,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _policy,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _higheragent,
            bool _loopback)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.LocalHierarchicalAgent,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Value.Classes.IText>()),
                QS.Fx.Reflection.Parameter.ValueClass("CheckpointClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Value.Classes.IText>()),
                QS.Fx.Reflection.Parameter.Object("policy", _policy),
                QS.Fx.Reflection.Parameter.Object("higher_agent", _higheragent),
                QS.Fx.Reflection.Parameter.Value<bool>("loopback", _loopback)//,
                //QS.Fx.Reflection.Parameter.Value<double>("rate", _rate),
                //QS.Fx.Reflection.Parameter.Value<double>("MTTA", _mtta),
                //QS.Fx.Reflection.Parameter.Value<double>("MTTB", _mttb),
                //QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug)
                );
        }

        #endregion

        #region _Ring

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Ring(
            double _rate, double _mtta, double _mttb, bool _debug, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _group)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Ring_0_,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("group", _group),
                QS.Fx.Reflection.Parameter.Value<double>("rate", _rate),
                QS.Fx.Reflection.Parameter.Value<double>("MTTA", _mtta),
                QS.Fx.Reflection.Parameter.Value<double>("MTTB", _mttb),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _Group

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Group(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _membership, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _transport, bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Group,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ValueClass("IncarnationClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Incarnation>()),
                QS.Fx.Reflection.Parameter.ValueClass("NameClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Name>()),
                QS.Fx.Reflection.Parameter.ValueClass("AddressClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Address>()),
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Serialization.ISerializable>()),
                QS.Fx.Reflection.Parameter.Object("membership", _membership),
                QS.Fx.Reflection.Parameter.Object("transport", _transport),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _Membership

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Membership(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _transport, QS.Fx.Base.Address _address,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _memory, double _batching, bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.MembershipChannel,
                QS.Fx.Reflection.Parameter.ValueClass("IdentifierClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Identifier>()),
                QS.Fx.Reflection.Parameter.ValueClass("IncarnationClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Incarnation>()),
                QS.Fx.Reflection.Parameter.ValueClass("NameClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Name>()),
                QS.Fx.Reflection.Parameter.ValueClass("AddressClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass<QS.Fx.Base.Address>()),
                QS.Fx.Reflection.Parameter.Object("transport", _transport),
                QS.Fx.Reflection.Parameter.Value<QS.Fx.Base.Address>("address", _address),
                QS.Fx.Reflection.Parameter.Object("memory", _memory),
                QS.Fx.Reflection.Parameter.Value<double>("batching", _batching),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _Transport

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Transport(string _address, bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.UnreliableTransport,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.Value<string>("address", _address),
                QS.Fx.Reflection.Parameter.Value<bool>("debug", _debug));
        }

        #endregion

        #region _Loopback

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Loopback(bool _debug)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.LocalCommunicationChannel,
                QS.Fx.Reflection.Parameter.ValueClass("MessageClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.ValueClass("CheckpointClass", QS.Fx.Reflection.Library.LocalLibrary.ValueClass(QS.Fx.Reflection.ValueClasses.ISerializable)),
                QS.Fx.Reflection.Parameter.Value<string>("address", null),
                QS.Fx.Reflection.Parameter.Value<bool>("debugging", _debug));
        }

        #endregion


        #region _Loader

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Loader(
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _library)
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Loader,
                QS.Fx.Reflection.Parameter.Object("library", _library));
        }

        #endregion

        #region _Library

        public static QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _Library()
        {
            return QS.Fx.Reflection.Library.LocalLibrary.Object(
                QS.Fx.Reflection.ComponentClasses.Library);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
