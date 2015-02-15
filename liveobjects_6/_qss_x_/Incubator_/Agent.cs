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

namespace QS._qss_x_.Incubator_
{
    public sealed class Agent : QS.Fx.Inspection.Inspectable
    {
        internal Agent(QS._qss_x_.Base1_.AgentID id, QS._qss_x_.Base1_.AgentIncarnation incarnation, bool isroot, bool islocal,
            Domain subdomain, Domain superdomain, Member member, Node node, QS._qss_c_.Base3_.Logger logger)
        {
            this.id = id;
            this.incarnation = incarnation;
            this.isroot = isroot;
            this.islocal = islocal;
            this.subdomain = subdomain;
            this.superdomain = superdomain;
            this.node = node;
            this.member = member;
            this.logger = logger;
        }

        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Base3_.Logger logger;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentID id;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentIncarnation incarnation;
        [QS.Fx.Base.Inspectable]
        internal bool isroot, islocal;
        [QS.Fx.Base.Inspectable]
        internal Domain subdomain, superdomain;
        [QS.Fx.Base.Inspectable]
        internal Node node;
        [QS.Fx.Base.Inspectable]
        internal Member member;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Agents_.Base.IAgent agent;
        [QS.Fx.Base.Inspectable]
        internal ICollection<Session> sessions = new System.Collections.ObjectModel.Collection<Session>();

        #region __Inspectable_Agent

        internal class __Inspectable_Agent : QS.Fx.Inspection.IAttributeCollection
        {
            internal __Inspectable_Agent(Agent agent, QS._qss_x_.Base1_.SessionID sessionid)
            {
                this.agent = agent;
                this.sessionid = sessionid;
            }

            private Agent agent;
            private QS._qss_x_.Base1_.SessionID sessionid;

            #region IAttributeCollection Members

            IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
            {
                get { return (agent.subdomain != null) ? (new string[] { "this", "children" }) : (new string[] { "this" }); }
            }

            QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
            {
                get 
                {
                    if (attributeName.Equals("this"))
                        return ((QS.Fx.Inspection.IAttribute) ((QS._qss_x_.Agents_.Base.Agent)agent.agent).sessions[sessionid].controlobject);
                    else if (attributeName.Equals("children"))
                    {
                        Agent[] _agents;
                        string _name;
                        if (agent.subdomain != null)
                        {
                            _name = "Members of " + agent.subdomain.id.ToString();
                            if (agent.subdomain.cluster != null && agent.subdomain.cluster.members != null)
                            {
                                _agents = new Agent[agent.subdomain.cluster.members.Length];
                                for (int ind = 0; ind < agent.subdomain.cluster.members.Length; ind++)
                                    _agents[ind] = agent.subdomain.cluster.members[ind].agent;
                            }
                            else
                            {
                                _agents = new Agent[] { agent.node.localagent };
                            }
                        }
                        else
                        {
                            _name = "Members";
                            _agents = new Agent[0];
                        }

                        return new __Inspectable_AgentCollection(_name, _agents, sessionid); 
                    }
                    else
                        throw new Exception();
                }
            }

            #endregion

            #region IAttribute Members

            string QS.Fx.Inspection.IAttribute.Name
            {
                get { return "AGENT(" + agent.id.ToString() + "#" + agent.incarnation.ToString() + "; " + sessionid.ToString() + ")"; }
            }

            QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
            {
                get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
            }

            #endregion
        }

        #endregion

        #region __Inspectable_AgentCollection

        internal class __Inspectable_AgentCollection : QS.Fx.Inspection.IAttributeCollection
        {
            internal __Inspectable_AgentCollection(string name, Agent[] agents, QS._qss_x_.Base1_.SessionID sessionid)
            {
                this.name = name;
                this.sessionid = sessionid;
                foreach (Agent _agent in agents)
                    this.agents.Add(_agent.id.ToString(), _agent);
            }

            private string name;
            private IDictionary<string, Agent> agents = new Dictionary<string, Agent>();
            private QS._qss_x_.Base1_.SessionID sessionid;

            #region IAttributeCollection Members

            IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
            {
                get { return agents.Keys; }
            }

            QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
            {
                get 
                {
                    return new __Inspectable_Agent(agents[attributeName], sessionid);
                }
            }

            #endregion

            #region IAttribute Members

            string QS.Fx.Inspection.IAttribute.Name
            {
                get { return name; }
            }

            QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
            {
                get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
            }

            #endregion
        }

        #endregion
    }
}
