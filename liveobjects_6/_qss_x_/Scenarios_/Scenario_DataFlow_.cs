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
    public sealed class Scenario_DataFlow_ : QS._qss_x_.Simulations_.IScenario_
    {
        QS._qss_x_.Simulations_.ITask_[] QS._qss_x_.Simulations_.IScenario_._Create(int _nnodes, IDictionary<string, QS.Fx.Reflection.Xml.Parameter> _parameters)
        {
            double _nodetreerate = (double)_parameters["node.tree.rate"].Value;
            double _nodetreemtta = (double)_parameters["node.tree.mtta"].Value;
            double _nodetreemttb = (double)_parameters["node.tree.mttb"].Value;

            List<QS._qss_x_.Simulations_.ITask_> _tasks = new List<QS._qss_x_.Simulations_.ITask_>();
            if (_nnodes < 3)
                throw new Exception("Not enough nodes.");
//This test makes a heirarchy with height = 1:

            String file_root = "dataflows\\mship_root.ruleset";
            String file_flow = "dataflows\\mship_flow.ruleset";

            String path_root = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, file_root);
            String path_flow = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, file_flow);

            String rules_root = getLines(path_root);
            String rules_flow = getLines(path_flow);

            if (rules_root.Equals("") || rules_flow.Equals(""))
                ;

            const string _rootmembershipprefix = "rootmembership";
            const string _rootdelegationprefix = "rootdelegation";
            const string _membershipprefix1_1 = "membership1_1";
            const string _delegationprefix1_1 = "delegation1_1";
            const string _membershipprefix1_2 = "membership1_2";
            const string _delegationprefix1_2 = "delegation1_2";
            const string _membershipprefix1_3 = "membership1_3";
            const string _delegationprefix1_3 = "delegation1_3";
            const string _membershipprefix2_1 = "membership2_1";
            const string _delegationprefix2_1 = "delegation2_1";
            const string client1_1_1Prefix = "c1_1_1";
            const string client1_1_2Prefix = "c1_1_2";
            const string client1_1_3Prefix = "c1_1_3";
            const string client1_1_4Prefix = "c1_1_4";
            const string client1_2_1Prefix = "c1_2_1";
            const string client1_2_2Prefix = "c1_2_2";
            const string client1_2_3Prefix = "c1_2_3";
            const string client1_3_1Prefix = "c1_3_1";
            const string client1_3_2Prefix = "c1_3_2";
            const string client1_3_3Prefix = "c1_3_3";

            #region membershipchannels

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> root_mc = Scenario_._Membership(
                Scenario_._Transport("0.0.0.0/0:" + 50014, false),
                null,
                Scenario_._Loopback(false),
                0,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> mc1_1 = Scenario_._Membership(
                Scenario_._Transport("0.0.0.0/0:" + 50010, false),
                null,
                Scenario_._Loopback(false),
                0,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> mc1_2 = Scenario_._Membership(
                Scenario_._Transport("0.0.0.0/0:" + 50011, false),
                null,
                Scenario_._Loopback(false),
                0,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> mc1_3 = Scenario_._Membership(
                Scenario_._Transport("0.0.0.0/0:" + 50012, false),
                null,
                Scenario_._Loopback(false),
                0,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> mc2_1 = Scenario_._Membership(
                Scenario_._Transport("0.0.0.0/0:" + 50013, false),
                null,
                Scenario_._Loopback(false),
                0,
                true);
#endregion

            #region rootclient
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> rootClient = Scenario_._DelegationClient(
              new QS.Fx.Base.Identifier(new Guid()),
              Scenario_._DelegationChannel(
                 Scenario_._Loader(Scenario_._Library()),
                 Scenario_._Transport("0.0.0.0/0", false),
                 null,
                 new QS.Fx.Base.Address(_rootdelegationprefix, 50024),
                 false),
                 "ROOT",
              true);
            #endregion

            #region parentclient
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> parentClient1 = Scenario_._DelegationClient(
               new QS.Fx.Base.Identifier(new Guid()),
               Scenario_._DelegationChannel(
                  Scenario_._Loader(Scenario_._Library()),
                  Scenario_._Transport("0.0.0.0/0", false),
                  null,
                  new QS.Fx.Base.Address(_delegationprefix2_1, 50023),
                  false),
                  "PARENT",
               true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> parentClient2 = Scenario_._DelegationClient(
               new QS.Fx.Base.Identifier(new Guid()),
               Scenario_._DelegationChannel(
                  Scenario_._Loader(Scenario_._Library()),
                  Scenario_._Transport("0.0.0.0/0", false),
                  null,
                  new QS.Fx.Base.Address(_delegationprefix2_1, 50023),
                  false),
                  "PARENT",
               true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> parentClient3 = Scenario_._DelegationClient(
               new QS.Fx.Base.Identifier(new Guid()),
               Scenario_._DelegationChannel(
                  Scenario_._Loader(Scenario_._Library()),
                  Scenario_._Transport("0.0.0.0/0", false),
                  null,
                  new QS.Fx.Base.Address(_delegationprefix2_1, 50023),
                  false),
                  "PARENT",
               true);
            #endregion

            #region protocolStacks
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> stack1_1 = Scenario_._DAProtocolStack(
                Scenario_._DataFlow(rules_flow, "LEAF"),
                Scenario_._Group(
                    Scenario_._Membership(
                        Scenario_._Transport("0.0.0.0/0", false),
                        new QS.Fx.Base.Address(_membershipprefix1_1, 50010),
                        null,
                        0,
                        false),
                    Scenario_._Transport("0.0.0.0/0", false),
                    false
                ),
                parentClient1,
                _nodetreerate,
                _nodetreemtta,
                _nodetreemttb,
                "LEAF",
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> stack1_2 = Scenario_._DAProtocolStack(
                Scenario_._DataFlow(rules_flow, "LEAF"),
                Scenario_._Group(
                    Scenario_._Membership(
                        Scenario_._Transport("0.0.0.0/0", false),
                        new QS.Fx.Base.Address(_membershipprefix1_2, 50011),
                        null,
                        0,
                        false),
                    Scenario_._Transport("0.0.0.0/0", false),
                    false
                ),
                parentClient2,
                _nodetreerate,
                _nodetreemtta,
                _nodetreemttb,
                "LEAF",
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> stack1_3 = Scenario_._DAProtocolStack(
                Scenario_._DataFlow(rules_flow, "LEAF"),
                Scenario_._Group(
                    Scenario_._Membership(
                        Scenario_._Transport("0.0.0.0/0", false),
                        new QS.Fx.Base.Address(_membershipprefix1_3, 50012),
                        null,
                        0,
                        false),
                    Scenario_._Transport("0.0.0.0/0", false),
                    false
                ),
                parentClient3,
                _nodetreerate,
                _nodetreemtta,
                _nodetreemttb,
                "LEAF",
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> stack2_1 = Scenario_._DAProtocolStack(
                Scenario_._DataFlow(rules_flow, "PARENT"),
                Scenario_._Group(
                    Scenario_._Membership(
                        Scenario_._Transport("0.0.0.0/0", false),
                        new QS.Fx.Base.Address(_membershipprefix2_1, 50013),
                        null,
                        0,
                        false),
                    Scenario_._Transport("0.0.0.0/0", false),
                    false
                ),
                rootClient,
                _nodetreerate,
                _nodetreemtta,
                _nodetreemttb,
                "PARENT",
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> root_stack = Scenario_._DAProtocolStack(
                Scenario_._DataFlow(rules_root, "ROOT"),
                Scenario_._Group(
                    Scenario_._Membership(
                        Scenario_._Transport("0.0.0.0/0", false),
                        new QS.Fx.Base.Address(_rootmembershipprefix, 50014),
                        null,
                        0,
                        false),
                    Scenario_._Transport("0.0.0.0/0", false),
                    false
                ),
                null,
                _nodetreerate,
                _nodetreemtta,
                _nodetreemttb,
                "ROOT",
                true);
#endregion

            #region DAs
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> root_da = Scenario_._DelegationChannel(
                Scenario_._Loader(Scenario_._Library()),
                Scenario_._Transport("0.0.0.0/0:" + 50024, false),
                root_stack,
                null,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> da1_1 = Scenario_._DelegationChannel(
                Scenario_._Loader(Scenario_._Library()),
                Scenario_._Transport("0.0.0.0/0:" + 50020, false),
                stack1_1,
                null,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> da1_2 = Scenario_._DelegationChannel(
                Scenario_._Loader(Scenario_._Library()),
                Scenario_._Transport("0.0.0.0/0:" + 50021, false),
                stack1_2,
                null,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> da1_3 = Scenario_._DelegationChannel(
                Scenario_._Loader(Scenario_._Library()),
                Scenario_._Transport("0.0.0.0/0:" + 50022, false),
                stack1_3,
                null,
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> da2_1 = Scenario_._DelegationChannel(
                Scenario_._Loader(Scenario_._Library()),
                Scenario_._Transport("0.0.0.0/0:" + 50023, false),
                stack2_1,
                null,
                true);
#endregion

            #region nodes
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_1_1 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_1, 50020),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_1_2 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_1, 50020),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_1_3 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_1, 50020),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_1_4 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_1, 50020),
                   false),
                   "LEAF",
                true),
                true);
            
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_2_1 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_2, 50021),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_2_2 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_2, 50021),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_2_3 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_2, 50021),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_3_1 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_3, 50022),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_3_2 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_3, 50022),
                   false),
                   "LEAF",
                true),
                true);

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> n1_3_3 = Scenario_._TestDataFlowClient(
                Scenario_._DelegationClient(
                new QS.Fx.Base.Identifier(new Guid()),
                Scenario_._DelegationChannel(
                   Scenario_._Loader(Scenario_._Library()),
                   Scenario_._Transport("0.0.0.0/0", false),
                   null,
                   new QS.Fx.Base.Address(_delegationprefix1_3, 50022),
                   false),
                   "LEAF",
                true),
                true);
             
#endregion

            #region tasks
            //Start root da and ma
            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                       (_rootdelegationprefix,
                           0,
                           double.PositiveInfinity,
                           0,
                           root_da));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_rootmembershipprefix,
                            0,
                            double.PositiveInfinity,
                            0,
                           root_mc));
            //Start parent da and ma
            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                       (_delegationprefix2_1,
                           0,
                           double.PositiveInfinity,
                           0,
                           da2_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_membershipprefix2_1,
                            0,
                            double.PositiveInfinity,
                            0,
                            mc2_1));
            //Start group 1
            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        ( _delegationprefix1_1,
                            0,
                            double.PositiveInfinity,
                            0,
                            da1_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_membershipprefix1_1,
                            0,
                            double.PositiveInfinity,
                            0,
                            mc1_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_1_1Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_1_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_1_2Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_1_2));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_1_3Prefix,
                            0,
                            10.0,
                            45,
                            n1_1_3));
           
           /* _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_1_4Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_1_4));*/
            //Start second group
            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_delegationprefix1_2,
                            0,
                            double.PositiveInfinity,
                            0,
                            da1_2));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_membershipprefix1_2,
                            0,
                            double.PositiveInfinity,
                            0,
                            mc1_2));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_2_1Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_2_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_2_2Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_2_2));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_2_3Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_2_3));
            //Start third group.
            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_delegationprefix1_3,
                            0,
                            double.PositiveInfinity,
                            0,
                            da1_3));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (_membershipprefix1_3,
                            0,
                            double.PositiveInfinity,
                            0,
                            mc1_3));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_3_1Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_3_1));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_3_2Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_3_2));

            _tasks.Add(new QS._qss_x_.Simulations_.Task_
                        (client1_3_3Prefix,
                            0,
                            double.PositiveInfinity,
                            0,
                            n1_3_3));
             
            #endregion


            /*            double _nodemttb = (double)_parameters["node.mttb"].Value;
            double _nodemttr = (double)_parameters["node.mttr"].Value;
            double _nodemttf = (double)_parameters["node.mttf"].Value;
            int _fanout = (int)_parameters["da.fanout"].Value;

            //Ring size hardcoded for now.
            int ringSize = 3;
            //Group size includes members + DA + membership server.
            int totalGroupSize = ringSize + 2;

*/
 /*           int size = 0;
            int height = 0;

            while (size < _nnodes)
            {
                height++;
                int cnt = (int)Math.Pow(_fanout, height);
                size = cnt + 2 * ((cnt - 1) / (_fanout - 1));
            }

            if (size != _nnodes)
                throw new Exception("Invalid number of nodes for fanout = " + _fanout);
*/
/*
            double _nodetreerate = (double)_parameters["node.tree.rate"].Value;
            double _nodetreemtta = (double)_parameters["node.tree.mtta"].Value;
            double _nodetreemttb = (double)_parameters["node.tree.mttb"].Value;
            bool _nodetreedebug = (bool)_parameters["node.tree.debug"].Value;
            bool _nodegroupdebug = (bool)_parameters["node.group.debug"].Value;
            bool _nodemembershipdebug = (bool)_parameters["node.membership.debug"].Value;
            double _controllermembershipbatching = (double)_parameters["controller.membership.batching"].Value;
            bool _controllermembershipdebug = (bool)_parameters["controller.membership.debug"].Value;
*/            

            // each DA comes with a MS
            // go one level of the tree at a time
                // until <remaining> <= level*2

/*
            int start_port = 1000;
            const string _controllerprefix = "controller";
            string _controllername = "";
            int nodegroup_size = 3;
            int group_index = 0;
            int port = 1000;

            for (int _i = 0; _i < _nnodes; _i++)
            {
                if (_i % nodegroup_size == 0)
                {
                    _controllername = _controllerprefix + group_index.ToString("00");
                    port = start_port + group_index;
                    group_index++;

                    _tasks.Add
                    (
                        new QS._qss_x_.Simulations_.Task_
                        (
                            _controllername,
                            0,
                            double.PositiveInfinity,
                            0,
                            Scenario_._DelegationChannel
                            (
                                Scenario_._Loader
                                (
                                    Scenario_._Library()
                                ),
                                Scenario_._Transport("0.0.0.0/0:" + port, false),
                                Scenario_._DataFlow(),   // protocol stack
                                null,
                                true
                            )
                        )
                    );
                }
                else
                {
                    _tasks.Add
                    (
                        new QS._qss_x_.Simulations_.Task_
                        (
                            "client" + _i.ToString("00"),
                            0,
                            double.PositiveInfinity,
                            0,
                            Scenario_._DelegationClient
                            (
                                new QS.Fx.Base.Identifier(new Guid()),
                                Scenario_._DelegationChannel
                                (
                                    Scenario_._Loader
                                    (
                                        Scenario_._Library()
                                    ),
                                    Scenario_._Transport("0.0.0.0/0:" + port, false),
                                    Scenario_._Library(),   // protocol stack
                                    new QS.Fx.Base.Address(_controllername, port),
                                    true
                                ),
                                true
                            )
                        )
                    );
                }
            }
   
             
            /*
              * 
              * 
              * Old from here on out
            for (int _i = 1; _i <= _nnodes; _i++)
            {
                _tasks.Add
                (
                    new QS._qss_x_.Simulations_.Task_
                    (
                        ("node_" + _i.ToString("00")),
                        _nodemttb,
                        _nodemttf,
                        _nodemttr,
                        Scenario_._Tree
                        (
                            _nodetreefanout,
                            _nodetreerate,
                            _nodetreemtta,
                            _nodetreemttb,
                            _nodetreedebug,
                            Scenario_._Group
                            (
                                Scenario_._Membership
                                (
                                    Scenario_._Transport("0.0.0.0/0:0", false),
                                    new QS.Fx.Base.Address(_controllername, 1000),
                                    null,
                                    0,
                                    _nodemembershipdebug
                                ),
                                Scenario_._Transport("0.0.0.0/0:0", false),
                                _nodegroupdebug
                            )
                        )
                    )
                );
            }
           */ 
            return _tasks.ToArray();
        }

        private string getLines(string path)
        {
            String result = "";
            StreamReader SR;
            SR = File.OpenText(path);

            String line = SR.ReadLine();
            while (line != null)
            {
                result += line + "\n";
                line = SR.ReadLine();
            }

            return result;
        }
    }
}
