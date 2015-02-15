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

namespace QS._qss_x_.Applications_
{
/*
    public class Application_005 : QS.TMS.Inspection.Inspectable, Platform.IApplication
    {
        #region Scenario

        #region Class Scenario

        public class Scenario
        {
            public Scenario(Scope[] scopes)
            {
                this.scopes = scopes;
            }

            public Scope[] scopes;

            public class Scope
            {
                public Scope(string name, string[] connections, Topic[] topics)
                {
                    this.name = name;
                    this.connections = connections;
                    this.topics = topics;
                }

                public string name;
                public string[] connections;
                public Topic[] topics;

                public class Topic
                {
                    public Topic(string name)
                    {
                        this.name = name;
                    }

                    public string name;
                }
            }
        }

        #endregion

        public static Scenario GetScenario()
        {
            return new Scenario(
                new Scenario.Scope[] 
                {
/-*
                    new Scenario.Scope("node001", new string[0]),
                    new Scenario.Scope("node002", new string[] { "node001" }),
*-/
                    new Scenario.Scope(
                        "node001", 
                        new string[0], 
                        new Scenario.Scope.Topic[] 
                        { 
                            new Scenario.Scope.Topic("tA"), 
                            new Scenario.Scope.Topic("tB"), 
                            new Scenario.Scope.Topic("tC")
                        }),
                    new Scenario.Scope(
                        "node002", 
                        new string[0], 
                        new Scenario.Scope.Topic[] 
                        { 
                            new Scenario.Scope.Topic("tD") 
                        }),
                    new Scenario.Scope(
                        "node003", 
                        new string[] { "node001", "node002" },
                        new Scenario.Scope.Topic[] 
                        { 
                            new Scenario.Scope.Topic("tE") 
                        }),
                    new Scenario.Scope(
                        "node004", 
                        new string[] { "node003" },
                        new Scenario.Scope.Topic[0]),
                    new Scenario.Scope(
                        "node005", 
                        new string[] { "node003" },
                        new Scenario.Scope.Topic[0]),
                    new Scenario.Scope(
                        "node006", 
                        new string[] { "node003" },
                        new Scenario.Scope.Topic[0])
                });
        }

        #endregion

        #region Constructor

        public Application_005()
        {
        }

        #endregion

        #region Fields

        [TMS.Inspection.Inspectable] private Scenario scenario;
        [TMS.Inspection.Inspectable] private Scenario.Scope scenario_scope;
        [TMS.Inspection.Inspectable] private string hostname;
        [TMS.Inspection.Inspectable] private int portno;
        [TMS.Inspection.Inspectable] private QS.Fx.Service.Service service;

        #endregion

        #region IApplication Members

        #region Start

        void QS.Fx.Platform.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS.Fx.Platform.IApplicationContext context)
        {
            scenario = (Scenario) context.Arguments["scenario"];
            hostname = platform.Network.GetHostName();            
            
            for (int ind = 0; ind < scenario.scopes.Length; ind++)
            {
                if (scenario.scopes[ind].name.Equals(hostname))
                {
                    portno = 65501 + ind;
                    scenario_scope = scenario.scopes[ind];
                    break;
                }
            }
            if (scenario_scope == null || portno == 0)
                throw new Exception();
            
            List<QS.Fx.Service.Configuration.Connection> connections = new List<QS.Fx.Service.Configuration.Connection>();
            foreach (string _connection in scenario_scope.connections)
            {
                int connectionportno = 0;
                for (int ind = 0; ind < scenario.scopes.Length; ind++)
                {
                    if (scenario.scopes[ind].name.Equals(_connection))
                    {
                        connectionportno = 65501 + ind;
                        break;
                    }
                }
                if (connectionportno == 0)
                    throw new Exception();

                connections.Add(
                    new QS.Fx.Service.Configuration.Connection(_connection, "quicksilver://" + _connection + ":" + connectionportno));
            }

            List<Backbone.Controller.Configuration.Topic> bbcc_topics = new List<QS.Fx.Backbone.Controller.Configuration.Topic>();
            foreach (Scenario.Scope.Topic topic in scenario_scope.topics)
                bbcc_topics.Add(new QS.Fx.Backbone.Controller.Configuration.Topic(topic.name));

            Backbone.Controller.Configuration bbcc = new QS.Fx.Backbone.Controller.Configuration(bbcc_topics.ToArray());
            
            service = new QS.Fx.Service.Service(((QS.CMS.Base.IReadableLogger) platform.Logger), platform, 
                new QS.Fx.Service.Configuration(hostname, "0.0.0.0/0", portno, connections.ToArray(), bbcc));
        }

        #endregion

        #region Stop

        void QS.Fx.Platform.IApplication.Stop()
        {
            if (service != null)
                ((IDisposable) service).Dispose();
            service = null;
        }

        #endregion

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
*/
}
