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

namespace QS._qss_x_.Agents_.Base
{
    internal sealed class Container  : QS.Fx.Inspection.Inspectable, IContainer
    {
        #region Constructor

        internal Container(IContainerContext context)
        {
            this.context = context;
        }

        #endregion

        #region Fields

        internal IContainerContext context;

        [QS.Fx.Base.Inspectable]
        internal IDictionary<QS._qss_x_.Base1_.QualifiedID, Agent> agents = new Dictionary<QS._qss_x_.Base1_.QualifiedID, Agent>();

        [QS.Fx.Base.Inspectable]
        internal IDictionary<QS._qss_x_.Base1_.QualifiedID, Connection> connections = new Dictionary<QS._qss_x_.Base1_.QualifiedID, Connection>();

        #endregion

        #region IContainer.Install

        IAgent IContainer.Install(QS._qss_x_.Base1_.AgentID id, QS._qss_x_.Base1_.AgentIncarnation incarnation, AgentAttributes attributes,
            QS.Fx.Logging.ILogger logger)
        {
            context.Logger.Log("Installing agent : id = " + id.ToString() + ", incarnation = " + incarnation.ToString() +
                " attributes = " + attributes.ToString());

            Agent _agent = new Agent(this, id, incarnation, attributes);
            agents.Add(id.Superdomain, _agent);

            if ((attributes & AgentAttributes.Local) != AgentAttributes.Local)
            {
                Agent _bottomagent;
                if (!agents.TryGetValue(id.Subdomain, out _bottomagent))
                    throw new Exception("Can't attach a bottom agent, no agent with upper id = " + id.Subdomain.ToString() + " was found.");
                ((IAgent)_agent).Initialize(logger, _bottomagent);
            }
            else
            {
                ((IAgent)_agent).Initialize(logger, null);
            }

            return _agent;
        }

        #endregion

        #region IContainer.Uninstall

        void IContainer.Uninstall(QS._qss_x_.Base1_.AgentID id)
        {
            context.Logger.Log("Uninstalling agent : id = " + id.ToString());

            Agent _agent;
            if (!agents.TryGetValue(id.Superdomain, out _agent))
                throw new Exception("No agent with upper id = " + id.Superdomain.ToString() + " could be found in this container.");

            if (!_agent.id.Equals(id))
                throw new Exception("The agent with upper id = " + id.Superdomain.ToString() + 
                    " currently installed in the container has a different lower id than the agent to be removed.");

            ((IAgent)_agent).Dispose();
        }

        #endregion 

        #region IContainer.Connect

        IConnection IContainer.Connect(QS._qss_x_.Base1_.QualifiedID topicid, IClient client)
        {
            lock (this)
            {
                Connection connection = new Connection(this, topicid, client);
                if (connections.ContainsKey(topicid))
                {
                    System.Diagnostics.Debug.Assert(false, "Not Implemented");
                    throw new Exception("At this moment, only one local client application per topic per container is supported.");
                }

                connections.Add(topicid, connection);

                return connection;
            }
        }

        #endregion

        #region Disconnect

        internal void Disconnect(Connection connection)
        {
            lock (this)
            {
                connections.Remove(connection.topicid);
                ((IDisposable)connection).Dispose();

                // ........................................................................................................................................................................

                System.Diagnostics.Debug.Assert(false, "Not Implemented");
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
