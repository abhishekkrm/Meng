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

// #define DEBUG_LogCrashAndRebootRelatedEvents
#define DEBUG_CollectDetailedProfilingInformation

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Incubator_
{
    public sealed class Incubator : QS.Fx.Inspection.Inspectable, IDisposable, IIncubator, IApplicationContext
    {
#if DEBUG_CollectDetailedProfilingInformation

        [QS.Fx.Base.Inspectable]
        public double AverageAgentAggregationTime
        {
            get { return QS._qss_x_.Agents_.Base.Agent.sum_aggregate / QS._qss_x_.Agents_.Base.Agent.num_aggregate; }
        }

        [QS.Fx.Base.Inspectable]
        public double AverageAgentDisseminationTime
        {
            get { return QS._qss_x_.Agents_.Base.Agent.sum_disseminate / QS._qss_x_.Agents_.Base.Agent.num_disseminate; }
        }

        [QS.Fx.Base.Inspectable]
        public double AverageTokenSerializationTime
        {
            get { return Channel.sum_serialize / Channel.num_serialize; }
        }

#endif

        #region Constants

        private const int DefaultFanout = 5;
        private const int DefaultDepth = 1;
        private const double DefaultMTTF = 1000000000000;
        private const double DefaultMTTR = 60;
        private const double DefaultMinLatency = 0.009;
        private const double DefaultAvgLatency = 0.010;

        #endregion

        #region Constructor

//        public Incubator(QS.Fx.Clock.IClock physicalClock) : this(physicalClock, DefaultFanout, DefaultDepth)
//        {
//        }

        public Incubator(QS.Fx.Clock.IClock physicalClock, int fanout, int nnodes, double mttf, double mttr, double check, IApplication[] __applications,
            double minlatency, double maxlatency)
        {
#if DEBUG_CollectDetailedProfilingInformation
            QS._qss_x_.Agents_.Base.Agent.ProfilingClock = physicalClock;
            Channel.ProfilingClock = physicalClock;
#endif

            this.physicalclock = physicalClock;
            this.simulatedclock = new QS._qss_c_.Simulations_1_.SimulatedClock(mainlogger); // , new QS._qss_c_.Collections_4_.BinaryTree());
            this.mainlogger = new QS._qss_c_.Base3_.Logger(simulatedclock, true, null);
            this.fanout = fanout;
            this.nnodes = nnodes; //  (int)Math.Round(Math.Pow(fanout, depth));
            this.mttf = mttf;
            this.mttr = mttr;
            this.checkinterval = check;
            this.nextchecking = checkinterval;

            this.minlatency = minlatency;
            this.avglatency = (minlatency + maxlatency) / 2;

            mainlogger.Log(this, "Initializing the simulation: nnodes = " + nnodes.ToString() + ", fanout = " + fanout.ToString() +                 
                ", mttf = " + mttf.ToString() + ", mttr = " + mttr.ToString() + ", latency = " + minlatency.ToString() + " to " + maxlatency.ToString());

            
            for (int __application_index = 0; __application_index < __applications.Length; __application_index++)
                applications.Add("Application(" + (__application_index + 1).ToString("000") + ")", __applications[__application_index]);

            foreach (KeyValuePair<string, IApplication> application in applications)
                application.Value.Initialize(application.Key, nnodes, this);

            Stack<Agent> _toinstallsessions = new Stack<Agent>();

            string nameformat = "D" + (((int) Math.Floor(Math.Log10((double) nnodes))) + 1).ToString();
            nodes = new Node[nnodes];
            nodelogs = new QS._core_c_.Base.IOutputReader[nnodes];
            for (int ind = 0; ind < nnodes; ind++)
            {
                Node node = new Node(this, ind, "node" + (ind + 1).ToString(nameformat));
                nodes[ind] = node;
                nodelogs[ind] = node.logger;

                foreach (KeyValuePair<string, IApplication> application in applications)
                {
                    Client client = new Client(application.Key, nodes[ind], application.Value, application.Value.Clients[ind], null);
                    nodes[ind].clients.Add(client.applicationname, client);
                    client.client.Initialize(client.node.name, client.node);
                }

                node.localdomain = new Domain(
                    new QS._qss_x_.Base1_.QualifiedID(new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, 1UL)),
                        new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, (ulong)(ind + 1)))),
                    false, true, null, null, node, null, null, new QS._qss_c_.Base3_.Logger(simulatedclock, true, null)); 
                node.localagent = new Agent(
                    new QS._qss_x_.Base1_.AgentID(QS._qss_x_.Base1_.QualifiedID.Undefined, node.localdomain.id), 
                    new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { 1 }), false, true, null, node.localdomain, null, node, node.logger);
                node.maxincarnation = 1;

                _toinstallsessions.Push(node.localagent);

                node.localagent.agent = ((QS._qss_x_.Agents_.Base.IContainer) nodes[ind].container).Install(
                    nodes[ind].localagent.id, nodes[ind].localagent.incarnation, QS._qss_x_.Agents_.Base.AgentAttributes.Local,
                    nodes[ind].localagent.logger);
            }

            Queue<Domain> _qin = new Queue<Domain>();
            for (int ind = 0; ind < nnodes; ind++)
                _qin.Enqueue(nodes[ind].localdomain);

            int _nextlevel = 2;
            while (_qin.Count > 1)
            {
                bool _isroot = _qin.Count <= fanout;
                Queue<Domain> _qout = new Queue<Domain>();
                int _levelindex = 0;
                while (_qin.Count > 0)
                {
//                    if (_qin.Count < fanout)
//                        throw new Exception("Could not generate the initial domain tree, less than a fanout domain left to cluster together.");
                    Domain[] _subdomains = new Domain[Math.Min(_qin.Count, fanout)];
                    for (int ind = 0; ind < _subdomains.Length; ind++)
                        _subdomains[ind] = _qin.Dequeue();
                    
                    Domain _superdomain = new Domain(
                        new QS._qss_x_.Base1_.QualifiedID(new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, (ulong) _nextlevel)),
                            new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, (ulong)(++_levelindex)))),
                            _isroot, false, null, _subdomains, _subdomains[0].node, null, null, new QS._qss_c_.Base3_.Logger(simulatedclock, true, null));
                    _superdomain.cluster = new Cluster(_superdomain, 1, new Member[_subdomains.Length]);
                    _superdomain.maxclusterincarnation = _superdomain.cluster.incarnation;
                    _superdomain.node.remotedomains.Add(_superdomain);
                    _qout.Enqueue(_superdomain);
                    
                    for (int ind = 0; ind < _subdomains.Length; ind++)
                        _subdomains[ind].superdomain = _superdomain;

                    for (int ind = 0; ind < _subdomains.Length; ind++)
                    {
                        Agent _agent = new Agent(new QS._qss_x_.Base1_.AgentID(_subdomains[ind].id, _superdomain.id), 
                            new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { 1 }), false, false, _subdomains[ind], _superdomain,
                            null, _subdomains[ind].node, _subdomains[ind].logger);
                        _subdomains[ind].agent = _agent;
                        _subdomains[ind].maxincarnation = _agent.incarnation;
                        _agent.node.remoteagents.Add(_agent);
                        _agent.member = new Member(
                            _superdomain.cluster, _agent, QS._qss_x_.Agents_.Base.MemberCategory.Normal, (uint) ind, null);
                        _superdomain.cluster.members[ind] = _agent.member;

                        _toinstallsessions.Push(_agent);

                        _agent.agent = ((QS._qss_x_.Agents_.Base.IContainer) _agent.node.container).Install(
                            _agent.id, _agent.incarnation, QS._qss_x_.Agents_.Base.AgentAttributes.None, _agent.logger);
                    }

                    for (int ind = 0; ind < _subdomains.Length; ind++)
                    {
                        Channel[] _memberrefs = new Channel[_superdomain.cluster.members.Length];
                        Configuration _configuration = new Configuration(_superdomain.cluster.members[ind], _memberrefs);
                        _superdomain.cluster.members[ind].configuration = _configuration;
                        for (int _ind = 0; _ind < _subdomains.Length; _ind++)
                            _memberrefs[_ind] = new Channel(
                                this, _superdomain.cluster.members[ind], _superdomain.cluster.members[_ind]);

                        _superdomain.cluster.members[ind].agent.agent.Reconfigure(_configuration);
                    }
                }
                _qin = _qout;
                _nextlevel++;
            }

            rootdomain = _qin.Dequeue();
            rootdomain.agent = new Agent(new QS._qss_x_.Base1_.AgentID(rootdomain.id, QS._qss_x_.Base1_.QualifiedID.Undefined),
                new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { 1 }), true, false, rootdomain, null, null, rootdomain.node, rootdomain.logger);
            rootdomain.maxincarnation = rootdomain.agent.incarnation;
            rootdomain.node.remoteagents.Add(rootdomain.agent);

            _toinstallsessions.Push(rootdomain.agent);

            rootdomain.agent.agent = ((QS._qss_x_.Agents_.Base.IContainer)rootdomain.node.container).Install(
                rootdomain.agent.id, rootdomain.agent.incarnation, QS._qss_x_.Agents_.Base.AgentAttributes.Global, rootdomain.agent.logger);

            uint _sessionno = 0;
            foreach (KeyValuePair<string, IApplication> _application in applications)
            {
                Session _session = new Session(
                    new QS._qss_x_.Base1_.SessionID(
                        new QS._qss_x_.Base1_.QualifiedID(new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, 0UL)),
                            new QS.Fx.Base.ID(new QS.Fx.Base.Int128(0UL, ((ulong)(++_sessionno))))), 1), _application.Key, 
                                _application.Value.Protocol);

                _session.rootdomain = rootdomain;
                _session.rootagent = rootdomain.agent;

                sessions.Add(_session);

                for (int ind = 0; ind < nnodes; ind++)
                {
                    Client _client = nodes[ind].clients[_application.Key];
                    _client.topicid = _session.id.Topic;
                    _client.client.Start(((QS._qss_x_.Agents_.Base.IContainer) nodes[ind].container).Connect(_session.id.Topic, _client.client));
                }
            }

            while (_toinstallsessions.Count > 0)
            {
                Agent __agent = _toinstallsessions.Pop();
                foreach (Session __session in sessions)
                {
                    __agent.sessions.Add(__session);
                    __agent.agent.StartSession(__session.id, __session.name, __session.protocol);
                }
            }

            _InitializeInspection();

            mainlogger.Log(this, "Initialization complete.");

            thread = new Thread(new ThreadStart(this._ThreadCallback));
            thread.Start();
        }

        #endregion

        #region Fields

        internal Node[] nodes;
        internal QS._core_c_.Base.IOutputReader[] nodelogs;

        [QS.Fx.Base.Inspectable]
        internal QS.Fx.Clock.IClock physicalclock;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Base3_.Logger mainlogger;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Simulations_1_.SimulatedClock simulatedclock;
        [QS.Fx.Base.Inspectable]
        internal int fanout, nnodes; // depth, 
        [QS.Fx.Base.Inspectable]
        internal Domain rootdomain;
        [QS.Fx.Base.Inspectable]
        internal int nprocessed;
        [QS.Fx.Base.Inspectable]
        internal List<Session> sessions = new List<Session>();
        [QS.Fx.Base.Inspectable]
        internal double minlatency = DefaultMinLatency, avglatency = DefaultAvgLatency;
        [QS.Fx.Base.Inspectable]
        internal double mttf = DefaultMTTF, mttr = DefaultMTTR;
        [QS.Fx.Base.Inspectable]
        internal double simulation_speed = double.PositiveInfinity; // 20;
        [QS.Fx.Base.Inspectable]
        internal double checkinterval = 60, nextchecking;

        internal event QS.Fx.Base.Callback onUpdate;
        internal Thread thread;
        internal bool done, running;
        internal AutoResetEvent recheck = new AutoResetEvent(false);
        internal System.Random random = new System.Random();

        internal IDictionary<string, IApplication> applications = new Dictionary<string, IApplication>();

        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Statistics_.Samples2D _messagesizes = new QS._qss_c_.Statistics_.Samples2D(
            "Message Sizes", "the sizes of transmitted messages", "time", "s", "time when the message was delivered",
            "size", "bytes", "message size in bytes");

        [QS.Fx.Base.Inspectable]
        internal QS._qss_c_.Statistics_.Samples2D _receivingtimes = new QS._qss_c_.Statistics_.Samples2D(
            "Message Processing Overheads", "the times to process the transmitted message", "time", "s", "time when the message was delivered",
            "time", "s", "the time it took to process the message");

        #endregion

        #region Inspection

        private IDictionary<string, Node> _nodes;
        private IDictionary<string, QS._core_c_.Base.IOutputReader> _nodelogs;

        [QS.Fx.Base.Inspectable("_nodes")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node> __inspectable_nodes;
        [QS.Fx.Base.Inspectable("_nodelogs")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader> __inspectable_nodelogs;
        [QS.Fx.Base.Inspectable("_applications")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, IApplication> __inspectable_applications;

        private void _InitializeInspection()
        {
            _nodes = new Dictionary<string, Node>();
            _nodelogs = new Dictionary<string, QS._core_c_.Base.IOutputReader>();

            foreach (Node node in nodes)
            {
                _nodes.Add(node.name, node);
                _nodelogs.Add(node.name, node.logger);
            }

            __inspectable_nodes =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node>("_nodes", _nodes,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, Node>.ConversionCallback(
                        delegate(string s) { return s; }));

            __inspectable_nodelogs =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader>("_nodelogs", _nodelogs,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS._core_c_.Base.IOutputReader>.ConversionCallback(
                        delegate(string s) { return s; }));

            __inspectable_applications =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, IApplication>("_applications", applications,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, IApplication>.ConversionCallback(
                        delegate(string s) { return s; }));
        }

        #endregion

        #region Reconfigure

        internal void Reconfigure(Node node)
        {
            lock (this)
            {
                Queue<Agent> _notifyagents = new Queue<Agent>();

                if (node.isup)
                {
                    #region Repairing

#if DEBUG_LogCrashAndRebootRelatedEvents
                    mainlogger.Log("Repaired node " + node.name.ToString() + ".");
#endif

                    System.Diagnostics.Debug.Assert(node.localagent == null);
                    System.Diagnostics.Debug.Assert(node.remoteagents.Count == 0);

#if DEBUG_LogCrashAndRebootRelatedEvents
                    node.logger.Log("\n\n@@@@@@@@@@@@@@@@@@@@ REBOOT @@@@@@@@@@@@@@@@@@@@\n\n");
#endif

                    node.container = new QS._qss_x_.Agents_.Base.Container(node);

                    foreach (KeyValuePair<string, Client> _client in node.clients)
                        _client.Value.client.Start(((QS._qss_x_.Agents_.Base.IContainer) node.container).Connect(_client.Value.topicid, _client.Value.client));

                    node.localagent = new Agent(
                        new QS._qss_x_.Base1_.AgentID(QS._qss_x_.Base1_.QualifiedID.Undefined, node.localdomain.id),
                        new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { ++node.maxincarnation }), false, true, null, node.localdomain, null, node, node.logger);

                    node.localagent.agent = ((QS._qss_x_.Agents_.Base.IContainer) node.container).Install(
                        node.localagent.id, node.localagent.incarnation, QS._qss_x_.Agents_.Base.AgentAttributes.Local, node.localagent.logger);

                    Stack<Agent> _toinstallsessions = new Stack<Agent>();
                    _toinstallsessions.Push(node.localagent);

                    Domain _domain = node.localdomain;                    

                    while (true)
                    {
                        if (_domain.superdomain != null)
                        {
                            System.Diagnostics.Debug.Assert(_domain.superdomain != null);
                            System.Diagnostics.Debug.Assert(_domain.agent == null);

                            System.Diagnostics.Debug.Assert(_domain.maxincarnation.Versions.Length == 1);
                            _domain.maxincarnation = new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { _domain.maxincarnation.Versions[0] + 1 });
                            Agent _newagent = new Agent(new QS._qss_x_.Base1_.AgentID(_domain.id, _domain.superdomain.id),
                                _domain.maxincarnation, false, false, _domain, _domain.superdomain, null, node, _domain.logger);

                            _domain.agent = _newagent;
                            node.remoteagents.Add(_newagent);

                            _domain.agent.agent = ((QS._qss_x_.Agents_.Base.IContainer) node.container).Install(
                                _domain.agent.id, _domain.agent.incarnation, QS._qss_x_.Agents_.Base.AgentAttributes.None, _domain.agent.logger);

                            _toinstallsessions.Push(_newagent);

                            Cluster _cluster = _domain.superdomain.cluster;
                            if (_cluster != null)
                            {
                                Member[] _newmembers = new Member[_cluster.members.Length + 1];

                                System.Diagnostics.Debug.Assert(_cluster.incarnation == _domain.superdomain.maxclusterincarnation);
                                Cluster _newcluster = new Cluster(
                                    _domain.superdomain, ++_domain.superdomain.maxclusterincarnation, _newmembers);
                                _domain.superdomain.cluster = _newcluster;

                                for (int ind = 0; ind < _cluster.members.Length; ind++)
                                    _newmembers[ind] = _cluster.members[ind];
                                
                                Member _newmember = new Member(
                                    _newcluster, _newagent, QS._qss_x_.Agents_.Base.MemberCategory.Candidate, (uint) (_newmembers.Length - 1), null);
                                _domain.agent.member = _newmember;
                                _newmembers[_newmember.index] = _newmember;

                                for (int ind = 0; ind < _newmembers.Length; ind++)
                                {
                                    _newmembers[ind].cluster = _newcluster;

                                    Channel[] _newchannels = new Channel[_newmembers.Length];
                                    Configuration _newconfiguration = new Configuration(_newmembers[ind], _newchannels);
                                    _newmembers[ind].configuration = _newconfiguration;

                                    for (int _ind = 0; _ind < _newmembers.Length; _ind++)
                                        _newchannels[_ind] = new Channel(this, _newmembers[ind], _newmembers[_ind]);

                                    _newmembers[ind].agent.agent.Reconfigure(_newconfiguration);
                                }

                                break;
                            }
                            else
                            {
                                Member _newmember = new Member(null, _newagent, QS._qss_x_.Agents_.Base.MemberCategory.Normal, 0, null);
                                _domain.agent.member = _newmember;
                                
                                Configuration _newconfiguration = new Configuration(_newmember, new Channel[] { null });
                                _newmember.configuration = _newconfiguration;
                                
                                Cluster _newcluster = new Cluster(
                                    _domain.superdomain, ++_domain.superdomain.maxclusterincarnation, new Member[] { _newmember });
                                _domain.superdomain.cluster = _newcluster;
                                _newmember.cluster = _newcluster;

                                _domain.agent.agent.Reconfigure(_newconfiguration);

                                _domain = _domain.superdomain;
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.Assert(false, "Not Implemented");
                        }
                    }

                    while (_toinstallsessions.Count > 0)
                    {
                        Agent __agent = _toinstallsessions.Pop();
                        foreach (Session _session in sessions)
                        {
                            __agent.sessions.Add(_session);
                            __agent.agent.StartSession(_session.id, _session.name, _session.protocol);
                        }
                    }

                    #endregion
                }
                else
                {
                    #region Crashing

#if DEBUG_LogCrashAndRebootRelatedEvents
                    mainlogger.Log("Crashing node " + node.name.ToString() + ".");
#endif

                    foreach (KeyValuePair<string, Client> _client in node.clients)
                        _client.Value.client.Stop();

                    Domain _domain = node.localdomain;
                    while (true)
                    {
                        Agent _agent = _domain.agent;
                        if (!(ReferenceEquals(_agent.node, node) && node.remoteagents.Remove(_agent)))
                        {
                            System.Diagnostics.Debug.Assert(false, "A crashing agent is hosted on a different node.");
                        }

                        _domain.agent = null;
                        ((QS._qss_x_.Agents_.Base.IContainer) _agent.node.container).Uninstall(_agent.id);

                        _agent.node.remoteagents.Remove(_agent);
                        _domain.node.remotedomains.Remove(_domain);

                        if (ReferenceEquals(_domain, rootdomain))
                        {
                            System.Diagnostics.Debug.Assert(false, "Handling the crash of the entire system has not been implemented.");
                        }
                        else
                        {
                            Cluster _cluster = _domain.superdomain.cluster;
                            if (_cluster.members.Length > 1)
                            {
                                Member[] _newmembers = new Member[_cluster.members.Length - 1];
                                Cluster _newcluster = new Cluster(_cluster.domain, _cluster.incarnation + 1, _newmembers);
                                _domain.superdomain.cluster = _newcluster;
                                System.Diagnostics.Debug.Assert(_newcluster.incarnation == _domain.superdomain.maxclusterincarnation + 1);
                                _domain.superdomain.maxclusterincarnation = _newcluster.incarnation;

                                int _ind = 0;
                                for (int ind = 0; ind < _cluster.members.Length; ind++)
                                {
                                    if (!_cluster.members[ind].agent.id.Equals(_agent.id))
                                    {
                                        Member _newmember = new Member(_newcluster, _cluster.members[ind].agent, _cluster.members[ind].category, (uint)_ind, null);
                                        _newmember.agent.member = _newmember;
                                        _notifyagents.Enqueue(_newmember.agent);
                                        _newmembers[_ind++] = _newmember;
                                    }
                                }

                                for (int ind = 0; ind < _newcluster.members.Length; ind++)
                                {
                                    Channel[] _newmemberrefs = new Channel[_newmembers.Length];
                                    _newcluster.members[ind].configuration = new Configuration(_newcluster.members[ind], _newmemberrefs);
                                    for (int __ind = 0; __ind < _newcluster.members.Length; __ind++)
                                        _newmemberrefs[__ind] = new Channel(this, _newcluster.members[ind], _newcluster.members[__ind]);
                                }

                                break;
                            }
                            else
                            {
                                _domain = _domain.superdomain;
                                _domain.cluster = null;
                            }
                        }
                    }

                    foreach (Agent _agent in node.remoteagents)
                    {
                        Node _newnode = _agent.subdomain.cluster.members[0].agent.node;

#if DEBUG_LogCrashAndRebootRelatedEvents
                        mainlogger.Log("Redeploying agent " + _agent.id.ToString() + 
                            " from node " + node.name.ToString() + " to node " + _newnode.name.ToString());
#endif

                        ((QS._qss_x_.Agents_.Base.IContainer) node.container).Uninstall(_agent.id);

//                        Member _newmember;
//                        if (_agent.member != null)
//                        {
//                            _newmember = new Member(_agent.member.cluster, null, _agent.member.category,
//                                _agent.member.index, _agent.member.configuration);
//                            _newmember.cluster.members[_newmember.index] = _newmember;
//                        }
//                        else
//                            _newmember = null;

                        System.Diagnostics.Debug.Assert(_agent.incarnation.Versions.Length == 1);
                        QS._qss_x_.Base1_.AgentIncarnation _newincarnation =
                            new QS._qss_x_.Base1_.AgentIncarnation(new uint[] { _agent.incarnation.Versions[0] + 1 });

#if DEBUG_LogCrashAndRebootRelatedEvents
                        _agent.subdomain.logger.Log("@@@@@@@@@@@@@@@@@@@@ MIGRATING AGENT " +
                            _agent.id.ToString() + "#" + _agent.incarnation.ToString() + " FROM " + _agent.node.name + " TO " + _newnode.name +
                            " NEW INCARNATION " + _newincarnation.ToString() + " @@@@@@@@@@@@@@@@@@@@");
#endif

                        Agent _newagent = new Agent(_agent.id, _newincarnation, _agent.isroot, _agent.islocal,
                            _agent.subdomain, _agent.superdomain, _agent.member /* _newmember */, _newnode, _agent.subdomain.logger);

                        _newagent.subdomain.agent = _newagent;
                        _newagent.subdomain.maxincarnation = _newincarnation;
                        _newnode.remoteagents.Add(_newagent);

                        if (_newagent.member != null)
                        {
                            _newagent.member.agent = _newagent;

//                            foreach (Member _anothermember in _newagent.member.cluster.members)
//                            {
//                                if (!ReferenceEquals(_anothermember, _newagent.member))
//                                {
//                                    for (int ind = 0; ind < _anothermember.configuration.members.Length; ind++)
//                                    {
//                                        Channel _channel = _anothermember.configuration.members[ind];
//                                        if (ReferenceEquals(_channel.remotemember, _agent.member))
//                                        {
//                                            mainlogger.Log("Updating channel from agent " + _anothermember.agent.id.ToString() +
//                                                " to agent " + _newmember.agent.id.ToString() + ".");
//
//                                            _memberref.isdead = true;
//                                            _anothermember.configuration.members[ind] = new Channel(this, _anothermember, _newmember);
//                                            break;
//                                        }
//                                    }
//                                }
//                            }
                        }

                        _newagent.agent = ((QS._qss_x_.Agents_.Base.IContainer) _newnode.container).Install(_newagent.id, _newagent.incarnation, 
                            (_newagent.isroot ? QS._qss_x_.Agents_.Base.AgentAttributes.Global : QS._qss_x_.Agents_.Base.AgentAttributes.None), _newagent.logger);

                        if (_newagent.member != null)
                            _newagent.agent.Reconfigure(_newagent.member.configuration);

                        _newagent.sessions = _agent.sessions;
                        foreach (Session _session in _newagent.sessions)
                        {
                            if (ReferenceEquals(_session.rootagent, _agent))
                                _session.rootagent = _newagent;
                            _newagent.agent.StartSession(_session.id, _session.name, _session.protocol);
                        }
                    }

                    node.remoteagents.Clear();

                    ((QS._qss_x_.Agents_.Base.IContainer)node.container).Uninstall(node.localagent.id);
                    node.localagent = null;

                    ((IDisposable)node.container).Dispose();
                    node.container = null;

                    #endregion
                }

                while (_notifyagents.Count > 0)
                {
                    Agent _agent = _notifyagents.Dequeue();
                    _agent.agent.Reconfigure(_agent.member.configuration);
                }
            }
        }

        #endregion

        #region _ThreadCallback

        private void _ThreadCallback()
        {
            while (!done && simulatedclock.QueueSize > 0)
            {
                if (running)
                {
                    bool updated = false;
                    lock (this)
                    {
                        simulatedclock.advance();
                        nprocessed++;
                        if (!double.IsPositiveInfinity(simulation_speed))
                            recheck.WaitOne((int) Math.Round(1000.0 / simulation_speed), false);
                        if (simulatedclock.Time > nextchecking)
                        {
                            nextchecking = simulatedclock.Time + checkinterval;
                            running = false;
                            updated = true;
                        }
                    }
                    if (updated && onUpdate != null)
                        onUpdate();
                }
                else
                    recheck.WaitOne();
            }
        }

        #endregion

        #region IDisposable Members

        void  IDisposable.Dispose()
        {
            done = true;
            recheck.Set();
            thread.Join();
        }

        #endregion

        #region IIncubator Members

        bool IIncubator.Running
        {
            get { return running; }
        }

        void IIncubator.Start()
        {
            lock (this)
            {
                if (!running)
                {
                    running = true;
                    recheck.Set();
                }
            }
        }

        void IIncubator.Stop()
        {
            running = false;
        }

        double IIncubator.Time
        {
            get { return simulatedclock.Time; }
        }

        int IIncubator.Processed
        {
            get { return nprocessed; }
        }

        int IIncubator.Pending
        {
            get { return simulatedclock.QueueSize; }
        }

        event QS.Fx.Base.Callback IIncubator.OnUpdate
        {
            add { lock (this) { onUpdate += value; } }
            remove { lock (this) { onUpdate -= value; } }
        }

        #endregion

        #region IApplicationContext Members

        QS.Fx.Logging.ILogger IApplicationContext.Logger
        {
            get { return mainlogger; }
        }

        QS.Fx.Clock.IClock IApplicationContext.Clock
        {
            get { return simulatedclock; }
        }

        QS.Fx.Clock.IAlarmClock IApplicationContext.AlarmClock
        {
            get { return simulatedclock; }
        }

        #endregion
    }
}
