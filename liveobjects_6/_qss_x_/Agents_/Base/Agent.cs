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

// #define DEBUG_LogIncomingMessages
// #define DEBUG_LogControlObjectStateUponMessageReceipt
#define DEBUG_CollectDetailedProfilingInformation

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Agents_.Base
{
    public sealed class Agent : QS.Fx.Inspection.Inspectable, IAgent, IDriverContext, IProtocolPeerContext
    {
#if DEBUG_CollectDetailedProfilingInformation
        public static QS.Fx.Clock.IClock ProfilingClock;
        public static double sum_aggregate, sum_disseminate;
        public static int num_aggregate, num_disseminate;
#endif

        #region Constants

        public static double DefaultRate = 10;

        #endregion

        #region Constructor

        internal Agent(Container container, QS._qss_x_.Base1_.AgentID id, QS._qss_x_.Base1_.AgentIncarnation incarnation, AgentAttributes attributes)
        {
            this.container = container;
            this.id = id;
            this.incarnation = incarnation;
            this.attributes = attributes;

            _InitializeInspection();
        }

        #endregion

        #region Fields

        internal Container container;

        [QS.Fx.Base.Inspectable]
        internal QS.Fx.Logging.ILogger logger;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentID id;
        [QS.Fx.Base.Inspectable]
        internal QS._qss_x_.Base1_.AgentIncarnation incarnation;
        [QS.Fx.Base.Inspectable]
        internal AgentAttributes attributes;
        [QS.Fx.Base.Inspectable]
        internal IConfiguration configuration;
        [QS.Fx.Base.Inspectable]
        internal bool isregular;
        [QS.Fx.Base.Inspectable]
        internal IDriver driver;
        [QS.Fx.Base.Inspectable]
        internal IAgent bottomagent;
        [QS.Fx.Base.Inspectable]
        internal Plan plan;
        [QS.Fx.Base.Inspectable]
        internal uint last_aggregation_round, last_dissemination_round, maximum_round;
        [QS.Fx.Base.Inspectable]
        internal double rate = DefaultRate;

        internal IDictionary<QS._qss_x_.Base1_.SessionID, Session> sessions = new Dictionary<QS._qss_x_.Base1_.SessionID, Session>();

        #endregion

        #region Inspection

        [QS.Fx.Base.Inspectable("_sessions")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_x_.Base1_.SessionID, Session> __inspectable_sessions;

        private void _InitializeInspection()
        {
            __inspectable_sessions =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_x_.Base1_.SessionID, Session>("_sessions", sessions,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_x_.Base1_.SessionID, Session>.ConversionCallback(
                        QS._qss_x_.Base1_.SessionID.FromString));
        }

        #endregion

        #region IAgent.ID

        QS._qss_x_.Base1_.AgentID IAgent.ID
        {
            get { return id; }
        }

        #endregion

        #region IAgent.Incarnation

        QS._qss_x_.Base1_.AgentIncarnation IAgent.Incarnation
        {
            get { return incarnation; }
        }

        #endregion

        #region IAgent.Initialize

        void IAgent.Initialize(QS.Fx.Logging.ILogger logger, IAgent bottomagent)
        {
            this.logger = logger;
            this.bottomagent = bottomagent;
        }

        #endregion

        #region IAgent.Register

        void IAgent.Register(QS._qss_x_.Base1_.SessionID sessionid, string sessionname, IAgent upperagent)
        {
            logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                "} _Register : " + sessionid.ToString() + " \"" + sessionname + 
                "\" with upper agent : { " + upperagent.ID.ToString() + " # " + upperagent.Incarnation.ToString() + " }");

            Session session;
            if (sessions.TryGetValue(sessionid, out session))
            {
                System.Diagnostics.Debug.Assert(session.upperagent == null, "Not Implemented");

                if (session.parentid != null && session.parentid.Equals(upperagent.ID) &&
                    ((IComparable<QS._qss_x_.Base1_.AgentIncarnation>)session.parentincarnation).CompareTo(upperagent.Incarnation) > 0)
                {
                    System.Diagnostics.Debug.Assert(false, "Unhandled: registering an obsolete agent with old incarnation number.");
                }

                session.upperagent = upperagent;

                // ........................................................................................................................................................
            }
            else
            {
                session = new Session(sessionid, sessionname, upperagent, bottomagent, null);
                sessions.Add(sessionid, session);
            }

            session.parentid = upperagent.ID;
            session.parentincarnation = upperagent.Incarnation;

            // session.parentupdated = true;
            logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                "} _Register : not implemented : not marking parent as updated in session " + sessionid.ToString() + ".");
        }

        #endregion

        #region IAgent.Reconfigure

        void IAgent.Reconfigure(IConfiguration configuration)
        {
            if (driver != null)
                driver.Dispose();

            this.configuration = configuration;
            this.isregular = configuration.IsRegular;

            StringBuilder s = new StringBuilder();
            foreach (IMember _member in configuration.Members)
                s.AppendLine((_member != null) ? (" [ " + _member.Category.ToString().PadRight(10) + " ] "+ _member.ID.ToString()) : "nullmember");

            logger.Log("Reconfiguring agent : id = " + id.ToString() + ", incarnation = " + incarnation.ToString() + "\n" +
                "configuration incarnation = " + configuration.Incarnation.ToString() + "\n" +
                "localindex = " + configuration.LocalIndex.ToString() + "\n" +
                "members = \n" + s.ToString() + "\n");

            if (configuration.Members.Length > 1)
                driver = new Components.Driver1(); 
            else
                driver = new Components.Driver0();

            _RecalculatePlan();

            driver.Initialize(this);
        }

        #endregion

        #region IAgent.Receive

        void IAgent.Receive(uint configuration_incarnation, uint sender_member_index, QS.Fx.Serialization.ISerializable message)
        {
#if DEBUG_LogIncomingMessages
#if DEBUG_LogControlObjectStateUponMessageReceipt
            StringBuilder s = new StringBuilder();
            foreach (Session _session in sessions.Values)
                s.AppendLine("Session(" + _session.id.ToString() + ") :\n" + QS.Fx.Printing.Printable.ToString(_session.controlobject));
#endif
            logger.Log(
                "Agent[" + id.ToString() + " # " + incarnation.ToString() + "].Receive : configuration_incarnation = " + 
                configuration_incarnation.ToString() + ", sender_member_index = " + sender_member_index.ToString() + "\n" +
                QS.Fx.Printing.Printable.ToString(message)
#if DEBUG_LogControlObjectStateUponMessageReceipt
                    + "\n\nSTATE:\n\n" + s.ToString()
#endif
                );
#endif
            driver.Receive(configuration_incarnation, sender_member_index, message);
        }

        #endregion
    
        #region IAgent.StartSession

        void IAgent.StartSession(QS._qss_x_.Base1_.SessionID sessionid, string sessionname, IProtocol protocol)
        {
            logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                "} _StartSession : " + sessionid.ToString() + " \"" + sessionname + "\"");

            Session session;
            if (sessions.TryGetValue(sessionid, out session))
            {
                // System.Diagnostics.Debug.Assert(session.bottomagent == null, "Not Implemented");
                session.bottomagent = bottomagent;
                session.protocol = protocol;
            }
            else
            {
                session = new Session(sessionid, sessionname, null, bottomagent, protocol);
                sessions.Add(sessionid, session);
            }

            if ((attributes & AgentAttributes.Local) == AgentAttributes.Local)
            {
                System.Diagnostics.Debug.Assert(bottomagent == null);

                Connection _connection;
                if (!container.connections.TryGetValue(sessionid.Topic, out _connection))
                {
                    System.Diagnostics.Debug.Assert(false, "No connections");
                    throw new Exception("Session has been installed, but could not locate any client connections.");
                }
                // session.connection = _connection;

                object _endpoint = _connection.client.Endpoint;
                if (!protocol.Interface.IsAssignableFrom(_endpoint.GetType()))
                {
                    System.Diagnostics.Debug.Assert(false, "Wrong endpoint");
                }

                IProtocolBind _binding = protocol.Bind(_endpoint);
                session.controlobject = _binding;
                _binding.Initialize(logger);

                logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() + "} : created bind");
            }
            else
            {
                if (bottomagent != null)
                    bottomagent.Register(sessionid, sessionname, this);

                if ((attributes & AgentAttributes.Global) == AgentAttributes.Global)
                {
                    IProtocolRoot _root = protocol.Root();
                    session.controlobject = _root;
                    _root.Initialize(logger);

                    logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() + "} : created root");
                }
                else
                {
                    IProtocolPeer _peer = protocol.Peer(this);
                    session.controlobject = _peer;
                    _peer.Initialize(logger);

                    logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() + "} : created peer");
                }

                IProtocolControl _lowerobj = ((Agent) bottomagent).sessions[sessionid].controlobject;
                if (_lowerobj != null)
                {
                    session.controlobject.ConnectLower(_lowerobj);
                    _lowerobj.ConnectUpper(session.controlobject);
                }
            }

            if ((attributes & AgentAttributes.Global) != AgentAttributes.Global)
            {
                if (session.upperagent != null)
                {
                    IProtocolControl _upperobj = ((Agent)session.upperagent).sessions[sessionid].controlobject;
                    if (_upperobj != null)
                    {
                        session.controlobject.ConnectUpper(_upperobj);
                        _upperobj.ConnectLower(session.controlobject);
                    }
                }
            }

            if (configuration != null)
                _RecalculatePlan();
        }

        #endregion

        #region IAgent.StopSession

        void IAgent.StopSession(QS._qss_x_.Base1_.SessionID sessionid)
        {
            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // _RecalculatePlan();
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (driver != null)
                driver.Dispose();
        }

        #endregion

        #region IDriverContext Members

        QS._qss_x_.Base1_.AgentID IDriverContext.ID
        {
            get { return id; }
        }

        QS._qss_x_.Base1_.AgentIncarnation IDriverContext.Incarnation
        {
            get { return incarnation; }
        }

        IConfiguration IDriverContext.Configuration
        {
            get { return configuration; }
        }

        double IDriverContext.Rate
        {
            get { return rate; }
        }

        QS.Fx.Clock.IClock IDriverContext.Clock
        {
            get { return container.context.Clock; }
        }

        QS.Fx.Clock.IAlarmClock IDriverContext.AlarmClock
        {
            get { return container.context.AlarmClock; }
        }

        QS.Fx.Logging.ILogger IDriverContext.Logger
        {
            get { return logger; }
        }

        void IDriverContext.Aggregate(uint round, out QS.Fx.Serialization.ISerializable outgoing)
        {
            AggregationToken _outgoing = new AggregationToken();
            _Aggregate(round, null, _outgoing);
            outgoing = _outgoing;
        }

        void IDriverContext.Aggregate(
            uint round, QS.Fx.Serialization.ISerializable[] incoming, out QS.Fx.Serialization.ISerializable outgoing)
        {
            AggregationToken[] _incoming = new AggregationToken[incoming.Length];
            Array.Copy(incoming, _incoming, incoming.Length);
            AggregationToken _outgoing = new AggregationToken();
            _Aggregate(round, _incoming, _outgoing);
            outgoing = _outgoing;
        }

        void IDriverContext.Aggregate(uint round, QS.Fx.Serialization.ISerializable[] incoming)
        {
            AggregationToken[] _incoming = new AggregationToken[incoming.Length];
            Array.Copy(incoming, _incoming, incoming.Length);
            _Aggregate(round, _incoming, null);
        }

        void IDriverContext.Disseminate(uint round, int noutgoing, out QS.Fx.Serialization.ISerializable[] outgoing)
        {
            DisseminationToken[] _outgoing = new DisseminationToken[noutgoing];
            _Disseminate(round, null, _outgoing);
            outgoing = (QS.Fx.Serialization.ISerializable[]) _outgoing;
        }

        void IDriverContext.Disseminate(
            uint round, QS.Fx.Serialization.ISerializable incoming, int noutgoing, out QS.Fx.Serialization.ISerializable[] outgoing)
        {
            DisseminationToken[] _outgoing = new DisseminationToken[noutgoing];
            _Disseminate(round, (DisseminationToken) incoming, _outgoing);
            outgoing = (QS.Fx.Serialization.ISerializable[]) _outgoing;
        }

        void IDriverContext.Disseminate(uint round, QS.Fx.Serialization.ISerializable incoming)
        {
            _Disseminate(round, (DisseminationToken) incoming, null);
        }

        #endregion

        #region IDriverContext.Round

        uint IDriverContext.Round
        {
            get { return maximum_round; }
        }

        #endregion

        #region _RecalculatePlan

        private void _RecalculatePlan()
        {
            plan = null;
        }

        #endregion

        #region _Aggregate

        private void _Aggregate(uint round, AggregationToken[] _incoming, AggregationToken outgoing)
        {
            AggregationToken incoming;
            if (_incoming != null)
            {
                System.Diagnostics.Debug.Assert(_incoming.Length == 1, "Not Implemented");
                incoming = _incoming[0];
            }
            else
                incoming = null;

            bool round_is_newer = round > this.last_aggregation_round;
            this.maximum_round = Math.Max(this.maximum_round, round);
            if (incoming != null)
            {
                this.maximum_round = Math.Max(this.maximum_round, incoming.maximumround);
                this.maximum_round = Math.Max(this.maximum_round, incoming.disseminationround);
            }
            this.last_aggregation_round = Math.Max(this.last_aggregation_round, round);            
            uint _disseminationround;
            if (incoming != null)
                _disseminationround = Math.Min(incoming.disseminationround, this.last_dissemination_round);
            else
                _disseminationround = this.last_dissemination_round;

            ParentInfo[] _parents1 = null, _parents;
            List<ParentInfo> _parents2 = null;

            if (incoming != null)
                _parents1 = incoming.parents;

            if (outgoing != null)
            {
                outgoing.disseminationround = _disseminationround;
                outgoing.maximumround = this.maximum_round;
            }

            if (plan != null)
            {
                #region Creating _parents

                if (plan.sessions != null)
                {
                    foreach (Session _session in plan.sessions)
                    {
                        if (_session.parentupdated)
                        {
                            if (_parents2 == null)
                                _parents2 = new List<ParentInfo>();
                            _parents2.Add(new ParentInfo(_session.id, _session.parentid, _session.parentincarnation));
                        }
                    }
                }

                if (_parents1 != null)
                {
                    if (_parents2 != null)
                    {
                        _parents = new ParentInfo[_parents1.Length + _parents2.Count];
                        Array.Copy(_parents1, 0, _parents, 0, _parents1.Length);
                        _parents2.CopyTo(_parents, _parents1.Length);
                    }
                    else
                        _parents = _parents1;
                }
                else
                {
                    if (_parents2 != null)
                        _parents = _parents2.ToArray();
                    else
                        _parents = null;
                }

                #endregion

                if (outgoing != null)
                {
                    outgoing.parents = _parents;

                    if (incoming != null)
                    {
                        #region incoming and outgoing

                        if (round_is_newer && incoming.isvalid && (incoming.fingerprint == plan.fingerprint))
                        {
                            outgoing.isvalid = true;
                            outgoing.fingerprint = plan.fingerprint;

                            if (plan.sessions != null && incoming.tokens != null)
                            {
                                System.Diagnostics.Debug.Assert(incoming.tokens.Length == plan.sessions.Length,
                                    "The number of sessions in the plan and the incoming token don't match.\nPlan:\n" +
                                    QS.Fx.Printing.Printable.ToString(plan) + "\nIncoming token:\n" + QS.Fx.Printing.Printable.ToString(incoming));

                                outgoing.tokens = new QS.Fx.Serialization.ISerializable[plan.sessions.Length];

                                for (int ind = 0; ind < plan.sessions.Length; ind++)
                                {
                                    Session _session = plan.sessions[ind];
                                    if (incoming.tokens[ind] != null && _session.controlobject != null)
                                    {
                                        IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                        System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                        double __time1 = ProfilingClock.Time;
#endif

                                        bool __result = _peerobj.Aggregate(
                                            new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), incoming.tokens[ind], out outgoing.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                        double __time2 = ProfilingClock.Time;
                                        sum_aggregate += __time2 - __time1;
                                        num_aggregate++;
#endif                                        
                                        
                                        if (__result)
                                            continue;
                                    }

                                    outgoing.tokens[ind] = null;
                                    logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() + 
                                        "} can't aggregate : either the incoming protocol token or the local controlobject is null");
                                }
                            }
                            else
                                outgoing.tokens = null;
                        }
                        else
                            outgoing.isvalid = false;

                        #endregion
                    }
                    else
                    {
                        #region only outgoing

                        if (round_is_newer)
                        {
                            outgoing.isvalid = true;
                            outgoing.fingerprint = plan.fingerprint;

                            if (plan.sessions != null)
                            {
                                outgoing.tokens = new QS.Fx.Serialization.ISerializable[plan.sessions.Length];
                                for (int ind = 0; ind < plan.sessions.Length; ind++)
                                {
                                    Session _session = plan.sessions[ind];
                                    if (_session.controlobject != null)
                                    {
                                        IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                        System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                        double __time1 = ProfilingClock.Time;
#endif

                                        bool __result = _peerobj.Aggregate(
                                            new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), out outgoing.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                        double __time2 = ProfilingClock.Time;
                                        sum_aggregate += __time2 - __time1;
                                        num_aggregate++;
#endif                                        

                                        if (__result)
                                            continue;
                                    }

                                    outgoing.tokens[ind] = null;
                                    logger.Log(
                                        "{ " + id.ToString() + " # " + incarnation.ToString() + "} can't aggregate : controlobject is null");
                                }
                            }
                            else
                                outgoing.tokens = null;
                        }
                        else
                        {
                            outgoing.isvalid = false;
                            outgoing.fingerprint = 0;
                            outgoing.tokens = null;
                        }

                        #endregion
                    }
                }
                else
                {
                    #region only incoming

                    System.Diagnostics.Debug.Assert(incoming != null);

                    #region Processing parents

                    if (incoming.parents != null)
                    {
                        foreach (ParentInfo _parentinfo in incoming.parents)
                        {
                            Session _session;
                            if (sessions.TryGetValue(_parentinfo.sessionid, out _session))
                            {
                                logger.Log(
                                    "Not implemented: replacing parent in session " + _session.id.ToString() + ".");                                
                            }
                        }
                    }

                    #endregion

                    if (incoming.isvalid && incoming.fingerprint == plan.fingerprint && _disseminationround >= plan.roundno)
                    {
                        if (!plan.confirmed)
                        {
                            plan.confirmed = true;
                            logger.Log("Agent " + id.ToString() + " determine that the plan has been confirmed.");
                        }

                        if (plan.sessions != null && incoming.tokens != null)
                        {
                            System.Diagnostics.Debug.Assert(incoming.tokens.Length == plan.sessions.Length,
                                "The number of sessions in the plan and the incoming token don't match.\nPlan:\n" +
                                QS.Fx.Printing.Printable.ToString(plan) + "\nIncoming token:\n" + QS.Fx.Printing.Printable.ToString(incoming));

                            for (int ind = 0; ind < plan.sessions.Length; ind++)
                            {
                                Session _session = plan.sessions[ind];
                                if (incoming.tokens[ind] != null && _session.controlobject != null)
                                {
                                    IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                    System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                    double __time1 = ProfilingClock.Time;
#endif

                                    bool __result = _peerobj.Aggregate(new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), incoming.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                    double __time2 = ProfilingClock.Time;
                                    sum_aggregate += __time2 - __time1;
                                    num_aggregate++;
#endif                                        

                                    if (__result)
                                        continue;
                                }

                                logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                                    "} can't aggregate : either the incoming protocol token or the local controlobject is null");
                            }
                        }
                    }
                    else
                    {
                        if (plan.confirmed)
                        {
                            plan.confirmed = false;
                            logger.Log("Agent " + id.ToString() + " determine that the plan is no longer confirmed.");
                        }
                    }

                    #endregion
                }
            }
            else
            {
                if (outgoing != null)
                    outgoing.isvalid = false;
            }
        }

        #endregion

        #region _Disseminate

        private void _Disseminate(uint round, DisseminationToken incoming, DisseminationToken[] _outgoing)
        {
            DisseminationToken outgoing;
            if (_outgoing != null)
            {
                System.Diagnostics.Debug.Assert(_outgoing.Length == 1, "Not Implemented");
                outgoing = new DisseminationToken();
                _outgoing[0] = outgoing;
            }
            else
                outgoing = null;

            bool round_is_newer = round > this.last_dissemination_round;
            this.maximum_round = Math.Max(this.maximum_round, round);
            if (incoming != null)
                this.maximum_round = Math.Max(this.maximum_round, incoming.aggregationround);
            this.last_dissemination_round = Math.Max(this.last_dissemination_round, round);
            uint _aggregationround;
            if (incoming != null)
                _aggregationround = Math.Min(this.last_aggregation_round, incoming.aggregationround);
            else
                _aggregationround = this.last_aggregation_round;

            if (incoming != null)
            {
                if ((plan == null) && (incoming.planinfo == null))
                {
//                    System.Diagnostics.Debug.Assert(false,
//                        "_Disseminate : Both local and incoming plan are null; at agent " + id.ToString() + "#" + incarnation.ToString() +
//                        ", upon receipt of:\n" + QS.Fx.Printing.Printable.ToString(incoming));

                    // erase the incoming info to prevent incomplete aggregation

                    incoming.fingerprint = 0;
                    incoming.tokens = null;
                    incoming.recalculateplan = true;

                    logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() + "} received and empty plan and has no plan either; ignoring it");
                }
                else
                {
                    if (incoming.planinfo != null)
                    {
                        Session[] _sessions = new Session[incoming.planinfo.sessions.Length];
                        for (int ind = 0; ind < incoming.planinfo.sessions.Length; ind++)
                        {
                            Session _session;
                            if (!sessions.TryGetValue(incoming.planinfo.sessions[ind], out _session))
                                _session = new Session(incoming.planinfo.sessions[ind], null, null, null, null);
                            _sessions[ind] = _session;
                        }
                        plan = new Plan(configuration.Incarnation, incoming.planinfo.round, false, _sessions);

                        logger.Log("Agent " + id.ToString() + " accepted a plan:\n" + QS.Fx.Printing.Printable.ToString(plan));
                    }
                    else
                    {
                        if (incoming.recalculateplan)
                        {
                            plan = null;
                            logger.Log("Agent " + id.ToString() + " is throwing away the local plan:\n" + QS.Fx.Printing.Printable.ToString(plan));
                        }
                    }

                    if (plan != null)
                    {
                        System.Diagnostics.Debug.Assert(plan.fingerprint == incoming.fingerprint,
                            "Plan fingerprints don't match.\nExisting plan:\n" + QS.Fx.Printing.Printable.ToString(plan) + "\nIncoming message:\n" +
                            QS.Fx.Printing.Printable.ToString(incoming));
                    }

                    if (outgoing == null)
                    {
                        if (plan != null && plan.sessions != null && incoming.tokens != null)
                        {
                            System.Diagnostics.Debug.Assert(incoming.tokens.Length == plan.sessions.Length,
                                "The number of sessions in the plan and the incoming token don't match.\nPlan:\n" + 
                                QS.Fx.Printing.Printable.ToString(plan) + "\nIncoming token:\n" + QS.Fx.Printing.Printable.ToString(incoming));

                            for (int ind = 0; ind < plan.sessions.Length; ind++)
                            {
                                Session _session = plan.sessions[ind];
                                if (incoming.tokens[ind] != null && _session.controlobject != null)
                                {
                                    IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                    System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                    double __time1 = ProfilingClock.Time;
#endif

                                    bool __result = _peerobj.Disseminate(new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), incoming.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                    double __time2 = ProfilingClock.Time;
                                    sum_disseminate += __time2 - __time1;
                                    num_disseminate++;
#endif                                        

                                    if (__result)
                                        continue;
                                }

                                logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                                    "} can't disseminate : either the incoming protocol token or the local controlobject is null");
                            }
                        }
                    }
                }
            }

            if (outgoing != null)
            {
                outgoing.aggregationround = _aggregationround;

                if (incoming != null)
                {
                    outgoing.fingerprint = incoming.fingerprint;
                    outgoing.planinfo = incoming.planinfo;
                    outgoing.recalculateplan = incoming.recalculateplan;

                    if (plan != null && plan.sessions != null && incoming.tokens != null)
                    {
                        System.Diagnostics.Debug.Assert(incoming.tokens.Length == plan.sessions.Length,
                            "The number of sessions in the plan and the incoming token don't match.\nPlan:\n" +
                            QS.Fx.Printing.Printable.ToString(plan) + "\nIncoming token:\n" + QS.Fx.Printing.Printable.ToString(incoming));

                        outgoing.tokens = new QS.Fx.Serialization.ISerializable[plan.sessions.Length];

                        for (int ind = 0; ind < plan.sessions.Length; ind++)
                        {
                            Session _session = plan.sessions[ind];
                            if (incoming.tokens[ind] != null && _session.controlobject != null)
                            {
                                IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                double __time1 = ProfilingClock.Time;
#endif

                                bool __result = _peerobj.Disseminate(
                                    new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), incoming.tokens[ind], out outgoing.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                double __time2 = ProfilingClock.Time;
                                sum_disseminate += __time2 - __time1;
                                num_disseminate++;
#endif                                        

                                if (__result)
                                    continue;
                            }

                            outgoing.tokens[ind] = null;
                            logger.Log("{ " + id.ToString() + " # " + incarnation.ToString() +
                                "} can't disseminate : either the incoming protocol token or the local controlobject is null");
                        }
                    }
                    else
                    {
                        outgoing.tokens = null;
                    }
                }
                else
                {
                    if (plan == null)
                    {
                        Session[] _sessions = new Session[sessions.Count];
                        sessions.Values.CopyTo(_sessions, 0);
                        plan = new Plan(configuration.Incarnation, round, false, _sessions);

                        logger.Log("Agent " + id.ToString() + " created a new plan:\n" + QS.Fx.Printing.Printable.ToString(plan));
                    }

                    System.Diagnostics.Debug.Assert(plan != null);

                    outgoing.fingerprint = plan.fingerprint;
                    if (!plan.confirmed)
                    {
                        if (plan.roundno == 0)
                            plan.roundno = round;
                        outgoing.planinfo = plan.info;
                    }

                    if (plan.sessions != null)
                    {
                        outgoing.tokens = new QS.Fx.Serialization.ISerializable[plan.sessions.Length];
                        for (int ind = 0; ind < plan.sessions.Length; ind++)
                        {
                            Session _session = plan.sessions[ind];
                            if (_session.controlobject != null)
                            {
                                IProtocolPeer _peerobj = _session.controlobject as IProtocolPeer;
                                System.Diagnostics.Debug.Assert(_peerobj != null);

#if DEBUG_CollectDetailedProfilingInformation
                                double __time1 = ProfilingClock.Time;
#endif

                                bool __result = _peerobj.Disseminate(new QS._qss_x_.Runtime_1_.VERSION(configuration.Incarnation, round), out outgoing.tokens[ind]);

#if DEBUG_CollectDetailedProfilingInformation
                                double __time2 = ProfilingClock.Time;
                                sum_disseminate += __time2 - __time1;
                                num_disseminate++;
#endif                                        

                                if (__result)
                                    continue;
                            }

                            outgoing.tokens[ind] = null;
                            logger.Log(
                                "{ " + id.ToString() + " # " + incarnation.ToString() + "} can't disseminate : controlobject is null");
                        }
                    }
                    else
                        outgoing.tokens = null;                        
                }
            }
        }

        #endregion

        #region IProtocolPeerContext Members

        bool IProtocolPeerContext.IsRegular
        {
            get { return isregular; }
            set 
            { 
                isregular = value;
                // TODO: Should report this to the GMS...
            }
        }

        #endregion
    }
}
