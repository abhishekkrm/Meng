/* Copyright (c) 2009 Krzysztof Ostrowski. All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Scenarios_
{
    public sealed class Scenario_Dissemination_ : QS._qss_x_.Simulations_.IScenario_
    {
        #region _Create

        QS._qss_x_.Simulations_.ITask_[] QS._qss_x_.Simulations_.IScenario_._Create(int _nnodes, IDictionary<string, QS.Fx.Reflection.Xml.Parameter> _parameters)
        {
            List<QS._qss_x_.Simulations_.ITask_> _tasks = new List<QS._qss_x_.Simulations_.ITask_>();
            if (_nnodes < 2)
                throw new Exception("Not enough nodes.");

            double _nodemttb = (double)_parameters["node.mttb"].Value;
            double _nodemttr = (double)_parameters["node.mttr"].Value;
            double _nodemttf = (double)_parameters["node.mttf"].Value;
            int _nodetreefanout = (int)_parameters["node.tree.fanout"].Value;
            double _nodetreerate = (double)_parameters["node.tree.rate"].Value;
            double _nodetreemtta = (double)_parameters["node.tree.mtta"].Value;
            double _nodetreemttb = (double)_parameters["node.tree.mttb"].Value;
            bool _nodetreedebug = (bool)_parameters["node.tree.debug"].Value;
            bool _nodegroupdebug = (bool)_parameters["node.group.debug"].Value;
            bool _nodemembershipdebug = (bool)_parameters["node.membership.debug"].Value;
            int _groupsize = (int)_parameters["group.size"].Value;
            double _controllermembershipbatching = (double)_parameters["controller.membership.batching"].Value;
            bool _controllermembershipdebug = (bool)_parameters["controller.membership.debug"].Value;

            const string _controllername = "controller";

            int _baseport = (int)_parameters["controller.baseport"].Value;
            int _numMembershipChannels = 0;

            //_nnodes -= 1;

            // assume membership services run on independent nodes
            int _numMembershipLevels = (int)(Math.Ceiling(Math.Log(_nnodes, _groupsize)));
            for (int level = 1; level <= _numMembershipLevels; level++)
            {
                int _numGroups = (int) Math.Ceiling((double)_nnodes / Math.Pow(_groupsize, level));
                for (int i = 0; i < _numGroups; i++)
                {
                    _numMembershipChannels++;
                    _tasks.Add
                    (
                        new QS._qss_x_.Simulations_.Task_
                        (
                            _controllername + _numMembershipChannels.ToString("00"),
                            0,
                            double.PositiveInfinity,
                            0,
                            Scenario_._Membership
                            (
                                Scenario_._Transport("0.0.0.0/0:" + (_baseport + _numMembershipChannels).ToString(), false),
                                null,
                                Scenario_._Loopback(false),
                                _controllermembershipbatching,
                                _controllermembershipdebug
                            )
                        )
                    );
                }
            }

            for (int _node = 0; _node < _nnodes; _node++)
            {
                int _numLevels = GetNumLevelsForNode(_node, _groupsize, _numMembershipLevels);
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _prevAgent = null;
                int _membershipNumber;
                for (int _level = _numLevels; _level > 1; _level--)
                {
                    _membershipNumber = GetMembershipNumber(_node, _nnodes, _groupsize, _level);
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _agent = Scenario_._LocalHierarchicalAgent(
                        Scenario_._UnreliableTreeDisseminationChannel
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
                                    new QS.Fx.Base.Address(_controllername + _membershipNumber.ToString("00"), 1000 + _membershipNumber),
                                    null,
                                    0,
                                    _nodemembershipdebug
                                ),
                                Scenario_._Transport("0.0.0.0/0:0", false),
                                _nodegroupdebug
                            )
                        ),
                        _prevAgent,
                        false
                    );

                    _prevAgent = _agent;
                }

                _membershipNumber = GetMembershipNumber(_node, _nnodes, _groupsize, 1);
                _tasks.Add
                (
                    new QS._qss_x_.Simulations_.Task_
                    (
                        ("node_" + _node.ToString("00")),
                        _nodemttb,
                        _nodemttf,
                        _nodemttr,
                        Scenario_._LocalHierarchicalAgent(
                            Scenario_._UnreliableTreeDisseminationChannel
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
                                        new QS.Fx.Base.Address(_controllername + _membershipNumber.ToString("00"), 1000 + _membershipNumber),
                                        null,
                                        0,
                                        _nodemembershipdebug
                                    ),
                                    Scenario_._Transport("0.0.0.0/0:0", false),
                                    _nodegroupdebug
                                )
                            ),
                            _prevAgent,
                            true
                        )
                    )
                );
            }

            /* Original Dissemination, single membership
            _tasks.Add
            (
                new QS._qss_x_.Simulations_.Task_
                (
                    _controllername,
                    0,
                    double.PositiveInfinity,
                    0,
                    Scenario_._Membership
                    (
                        Scenario_._Transport("0.0.0.0/0:1000", false),
                        null,
                        Scenario_._Loopback(false),
                        _controllermembershipbatching,
                        _controllermembershipdebug
                    )
                )
            );

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
                        Scenario_._UnreliableTreeDisseminationChannel
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
             * */

            return _tasks.ToArray();
        }

        #endregion

        int GetNumLevelsForNode(int _node, int _groupsize, int _numTotalLevels)
        {
            int i;
            for (i = 1; i < _numTotalLevels; i++)
            {
                if (_node % ((int)Math.Pow(_groupsize, i)) != 0)
                    return i;
            }

            return i;
        }

        int GetMembershipNumber(int _node, int _nnodes, int _groupsize, int _level)
        {
            int _channel = 0, i;
            for (i = 1; i < _level; i++)
            {
                _channel += (int)Math.Ceiling((double)_nnodes / Math.Pow(_groupsize, i));
            }

            _channel += 1 + (int)Math.Floor((double)_node / Math.Pow(_groupsize, i));
            return _channel;
        }
    }
}
