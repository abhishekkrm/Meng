/*

Copyright (c) 2010 Colin Barth. All rights reserved.

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

using System;
using System.Collections.Generic;
using System.Text;
/*
 * Notes:
 * - When connecting Jared's component to DAClient, need to make sure that after connected, don't continue to try and connect on
 * subsequent group reconfigurations. But can't add a onconnectedCallback
 * - Need to know when I am a leader, but don't have access to group reconfigurations.
 * - Ring seems to only check for correct configuration if token has no message.
 */
namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DAProtocolStackManager, "Delegation Protocol stack Manager")]
    public sealed class DAProtocolStackManager :
        QS._qss_x_.Properties_.Component_.Ring_1_<QS._qss_x_.Properties_.Value_.AggregateToken_>,
        QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Object.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient,
        QS.Fx.Interface.Classes.IAggUpdater<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>
    {
        #region Constructor

        public DAProtocolStackManager(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("agg_dataflow_channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlowExposed<
                QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>> agg_dataflow_reference,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IGroup<
                QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> groupReference,
            [QS.Fx.Reflection.Parameter("parent_daclient", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlow> parent_client_reference,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            [QS.Fx.Reflection.Parameter("MTTA", QS.Fx.Reflection.ParameterClass.Value)]
            double _mtta,
            [QS.Fx.Reflection.Parameter("MTTB", QS.Fx.Reflection.ParameterClass.Value)]
            double _mttb,
            [QS.Fx.Reflection.Parameter("name", QS.Fx.Reflection.ParameterClass.Value)]
            string name,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, groupReference, _rate, _mtta, _mttb, name, _debug)
        {
            this._mycontext = _mycontext;

            if (name == null)
                name = "";
            this.debug_identifier = name;

            this.daClientConnected = false;
            this.parent_daclient_reference = parent_client_reference;
            this.agg_dataflow_reference = agg_dataflow_reference;
            this.aggDataflowProxy = this.agg_dataflow_reference.Dereference(_mycontext);
            this.clientEndpoint = _mycontext.DualInterface<
               QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow>(this);
            this.clientEndpoint.OnConnect += new QS.Fx.Base.Callback(
                   delegate
                   {
                       this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.clientEndpoint_OnConnect)));
                   });
            this.clientEndpoint.OnConnected += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.clientEndpoint_OnConnected)));
                    });
            this.clientEndpoint.OnDisconnect += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.clientEndpoint_OnDisconnect)));
                    });
            this.aggDataflowEndpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient>(this);
            this.aggDataflowEndpoint.OnConnect += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggDataflowEndpoint_OnConnect)));
                    });
            this.aggDataflowEndpoint.OnConnected += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggDataflowEndpoint_OnConnected)));
                    });
            this.aggDataflowEndpoint.OnDisconnect += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggDataflowEndpoint_OnDisconnect)));
                    });
            this.connectionToAggDataFlow = this.aggDataflowEndpoint.Connect(this.aggDataflowProxy.DataFlow);

            this.aggAggregatorEndpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IAggUpdaterClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>,
                QS.Fx.Interface.Classes.IAggUpdater<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>>(this);
            this.aggAggregatorEndpoint.OnConnect += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggAggregatorEndpoint_OnConnect)));
                    });
            this.aggAggregatorEndpoint.OnConnected += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggAggregatorEndpoint_OnConnected)));
                    });
            this.aggAggregatorEndpoint.OnDisconnect += new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.aggAggregatorEndpoint_OnDisconnect)));
                    });
            this.connectionToAggAggregator = this.aggAggregatorEndpoint.Connect(this.aggDataflowProxy.AggUpdater);
        }

        #endregion

        #region Fields

        //For debugging:
        private string debug_identifier;

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private bool daClientConnected;
        //The endpoint that the component who retrieved the protocol stack manager connects to.
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow> clientEndpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlowExposed<
        QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>> agg_dataflow_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IDataFlowExposed<
            QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable> aggDataflowProxy;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient> aggDataflowEndpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlow> parent_daclient_reference;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow> aggDataflowClientEndpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.IAggUpdaterClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>,
           QS.Fx.Interface.Classes.IAggUpdater<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>> aggAggregatorEndpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IDataFlow parentDAClientProxy;
        private QS.Fx.Endpoint.IConnection connectionToAggDataFlow;
        private QS.Fx.Endpoint.IConnection connectionToAggAggregator;
        private QS.Fx.Endpoint.IConnection connectionToParentDAClient;
        private QS.Fx.Base.Incarnation currIncarnation;
        private QS._qss_x_.Properties_.Value_.Round_ currRound;
        private QS.Fx.Base.Index currIndex;
        private QS.Fx.Clock.IAlarm _alarm1;
        private bool _IsCurrLeader;
        
        #endregion

        #region IDataFlow Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow> QS.Fx.Object.Classes.IDataFlow.DataFlow
        {
            get { return this.clientEndpoint; }
        }
        #endregion

        #region IDataFlow Members

        void QS.Fx.Interface.Classes.IDataFlow.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " received message from DAClient. ");
#endif
            if (this.aggDataflowEndpoint.IsConnected)
            {
                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<int, long, QS.Fx.Serialization.ISerializable>(new QS._qss_x_.Properties_.Base_.EventCallback_(this.fromDAClientToStack), id, version, value));
            }
        }

        #endregion

        #region IDataFlowClient Members

        void QS.Fx.Interface.Classes.IDataFlowClient.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " received message from aggregator. ");
#endif
            if (this.aggDataflowEndpoint.IsConnected)
            {
                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<int, long, QS.Fx.Serialization.ISerializable>(new QS._qss_x_.Properties_.Base_.EventCallback_(this.fromStackToDAClient), id, version, value));
            }
        }

        #endregion

        #region IAggUpdater Members

        void QS.Fx.Interface.Classes.IAggUpdater<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>.updated(
            QS._qss_x_.Properties_.Value_.Round_ round, QS.Fx.Serialization.ISerializable message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " received updated token. ");
#endif
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>(
                           new QS._qss_x_.Properties_.Base_.EventCallback_(this.processAggUpdated), round, message));
        }

        #endregion

        #region ring_members
 
        protected override void _Ring_Membership(QS.Fx.Value.Classes.IMembership<QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership)
        {
            this.processMembershipChange(_membership);
        }

        protected override void _Ring_Incoming(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.AggregateToken_ _message)
        {
            this.processRingIncoming(_identifier, _message);
        }

        #endregion

        private void fromDAClientToStack(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " passing message from DAClient to Aggregator. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable> _event2 =
                (QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable>)_event;
            int id = _event2._Object1;
            long version = _event2._Object2;
            QS.Fx.Serialization.ISerializable value = _event2._Object3;
            this.aggDataflowEndpoint.Interface.Send(id, version, value);
        }

        private void fromStackToDAClient(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " passing message from Aggregator to DAClient. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable> _event2 =
                (QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable>)_event;
            int id = _event2._Object1;
            long version = _event2._Object2;
            QS.Fx.Serialization.ISerializable value = _event2._Object3;
            this.clientEndpoint.Interface.Send(id, version, value);
        }

        private void processRingIncoming(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.AggregateToken_ msg)
        {
            QS._qss_x_.Properties_.Value_.Round_ round = msg.Round;
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + "processRingIncoming: received message from ring with incarnation " + round.Incarnation.String + " and index " + round.Index.String);
#endif
            //If message not from the current incarnation, ignore it.
            if (this.currIncarnation.Equals(round.Incarnation))
            {
                //If message not from predecessor, ignore it.
                if (this._Predecessor.Identifier.Equals(_identifier))
                {
                    //If I am the leader, proceed to next aggregation step.
                    if (this._IsLeader)
                    {
                        this.currIndex = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this.currIndex).Incremented;
                        this.currRound = new QS._qss_x_.Properties_.Value_.Round_(this.currIncarnation, this.currIndex);
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processRingIncoming: sending to aggregator for finalization. ");
#endif
                        this.aggAggregatorEndpoint.Interface.finalize(round, msg.Payload);
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processRingIncoming: starting timer for next token round. ");
#endif
                        this._alarm1 = this._platform.AlarmClock.Schedule
                                    (
                                        (0.2),
                                        new QS.Fx.Clock.AlarmCallback
                                        (
                                            delegate(QS.Fx.Clock.IAlarm _alarm)
                                            {
                                                if ((_alarm1 != null) && !_alarm1.Cancelled && ReferenceEquals(_alarm1, _alarm))
                                                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<
                                                        QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>(this.startTokenPassing, this.currRound, msg.Payload));
                                            }
                                        ),
                                        null
                                    );
                    }
                    //Otherwise, pass to aggregation client. 
                    else
                    {
                        if (this.currIndex.Equals(round.Index))
                        {
                            this.currIndex = ((QS.Fx.Base.IIncrementable<QS.Fx.Base.Index>)this.currIndex).Incremented;
                            this.currRound = round;
                            this.aggAggregatorEndpoint.Interface.update(this.currRound, msg.Payload);
                        }
                        else
                        {
#if VERBOSE
                            if (this._logger != null)
                                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + "processRingIncoming: expected index " + this.currIndex.String + " but received " + round.Index.String);
#endif
                        }
                    }
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processRingIncoming: message not from predecessor. ");
#endif
                }
            }
            else
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processRingIncoming: Expecting incarnation " + this.currIncarnation.String + " but got " + round.Incarnation.String + ". ");
#endif
            }
        }

        private void processMembershipChange(QS.Fx.Value.Classes.IMembership<QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership)
        {
            this.currIncarnation = _membership.Incarnation;
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Received new membership with incarnation " + this.currIncarnation.String);
#endif
            //If I'm a member, process, otherwise underlying ring will deal with making me a member again.
            if (this._IsMember)
            {
                this.currIncarnation = _membership.Incarnation;
                this.currIndex = new QS.Fx.Base.Index(0);
                this.currRound = new QS._qss_x_.Properties_.Value_.Round_(this.currIncarnation, this.currIndex);
#if VERBOSE
                if (this._logger != null)
                    if(this._Predecessor != null && this._Successor != null)
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Successor: " + this._Successor.Identifier.String + " Predecessor: " + this._Predecessor.Identifier.String);
                    else if(this._Successor != null)
                         this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Successor: " + this._Successor.Identifier.String + " Predecessor is null");
                    else if(this._Predecessor != null)
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Successor is null " + " Predecessor: " + this._Predecessor.Identifier.String);
                    else
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Singleton");
#endif
                //If I am now the leader, process accordingly.
                if (this._IsLeader)
                {
                    if (this._IsCurrLeader)
                    {
                        if (_alarm1 != null)
                        {
                            this._alarm1.Cancel();
                            this._alarm1 = null;
                        }
                    }
                    //If no parent client, I am the root.
                    if (this.parent_daclient_reference == null)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DAProtocolStackManager " + this.debug_identifier + ": IS ROOT. Node ID: " + this._Identifier);
#endif
                    }
                    //If I wasn't already the leader, need to make connections to parent client.
                    else if (!this._IsCurrLeader)
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DAProtocolStackManager " + this.debug_identifier + ": BECOMING LEADER. Node ID: " + this._Identifier);
#endif
                        //this.parentDAClientProxy = this.parent_daclient_reference.Dereference(this._mycontext);
                        //this.connectionToParentDAClient = this.aggDataflowProxy.DataFlowClient.Connect(this.parentDAClientProxy.DataFlow);
                        //this.daClientConnected = true;

                        //this.parentDAClientProxy = this.parent_daclient_reference.Dereference(this._mycontext);
                        this.connectionToParentDAClient = this.aggDataflowProxy.DataFlowClient.Connect(this.parent_daclient_reference.Dereference(this._mycontext).DataFlow);
                        this.daClientConnected = true;
                    }
                    else
                    {
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("DAProtocolStackManager " + this.debug_identifier + ": STILL LEADER. Node ID: " + this._Identifier);
#endif
                    }
                    this._IsCurrLeader = true;
                    //I am the leader, so re-start aggregation with a new round.
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + ": Starting token passing by sending to aggregator.");
#endif
                    this.aggAggregatorEndpoint.Interface.update(this.currRound, null);
                }
                //Otherwise, if I was the leader, stop being the leader.
                else if (this._IsCurrLeader)
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("DAProtocolStackManager " + this.debug_identifier + ": NO LONGER LEADER. Node ID: " + this._Identifier);
#endif
                    if (_alarm1 != null)
                    {
                        this._alarm1.Cancel();
                        this._alarm1 = null;
                    }
                    this._IsCurrLeader = false;
                    if (this.connectionToParentDAClient != null)
                    {
                        this.connectionToParentDAClient.Dispose();
                    }
                    this.daClientConnected = false;
                    this.parentDAClientProxy = null;
                }
            }
        }

        private void processAggUpdated(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processing agg updated. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable> _event2 =
               (QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>)_event;
            QS._qss_x_.Properties_.Value_.Round_ round = _event2._Object1;
            QS.Fx.Serialization.ISerializable message = _event2._Object2;
            //If the update is from the current incarnation, process. Otherwise drop it.
            if (round.Incarnation.Equals(this.currIncarnation))
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processAggUpdated: passing updated token to successor with ID " + this._Successor.Identifier.String + ". ");
#endif
                QS._qss_x_.Properties_.Value_.AggregateToken_ token = new QS._qss_x_.Properties_.Value_.AggregateToken_(round, message);
                this._Ring_Outgoing(this._Successor.Identifier, token);
            }
            else
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " processAggUpdated: Expecting incarnation " + this.currIncarnation.String + " but got " + round.Incarnation.String + ". ");
#endif
            }
        }

        private void startTokenPassing(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " startTokenPassing");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable> _event2 =
             (QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Serialization.ISerializable>)_event;
            QS._qss_x_.Properties_.Value_.Round_ round = _event2._Object1;
            QS.Fx.Serialization.ISerializable message = _event2._Object2;

            //If the final value is from the current incarnation, process. Otherwise drop it.
            if (round.Incarnation.Equals(this.currIncarnation))
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " startTokenPassing: passing token to aggregator. ");
#endif
                this.aggAggregatorEndpoint.Interface.update(this.currRound, message);
            }
            else
            {
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " startTokenPassing: Expecting incarnation " + this.currIncarnation.String + " but got " + round.Incarnation.String + ". ");
#endif
            }
        }

        #region aggDataFlow_Connect

        private void aggDataflowEndpoint_OnConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " connecting agg dataflow endpoint.");
#endif
        }

        #endregion

        #region aggDataFlow_Connected

        private void aggDataflowEndpoint_OnConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " connected agg dataflow endpoint.");
#endif
        }

        #endregion

        #region aggDataFlow_Disconnect

        private void aggDataflowEndpoint_OnDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " disconnecting agg dataflow endpoint.");
#endif
        }

        #endregion

        #region aggDataFlowClient_Connect

        private void aggDataflowClientEndpoint_OnConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " connecting agg dataflow endpoint client endpoint to next DAClient.");
#endif
        }

        #endregion

        #region aggDataFlowClient_Connected

        private void aggDataflowClientEndpoint_OnConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " connected agg dataflow endpoint client endpoint to next DAClient.");
#endif
        }

        #endregion

        #region aggDataFlowClient_Disconnect

        private void aggDataflowClientEndpoint_OnDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " disconnecting agg dataflow endpoint client endpoint to next DAClient.");
#endif
        }

        #endregion

        #region aggAggregator_Connect

        private void aggAggregatorEndpoint_OnConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Connecting to Aggregator Client.");
#endif
        }

        #endregion

        #region aggAggregator_Connected

        private void aggAggregatorEndpoint_OnConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Connected to Aggregator Client.");
#endif
        }

        #endregion

        #region aggAggregator_Disconnect

        private void aggAggregatorEndpoint_OnDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Disconnecting from Aggregator Client.");
#endif
        }

        #endregion

        #region client_Connect

        private void clientEndpoint_OnConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Connecting to Aggregator Client.");
#endif
        }

        #endregion

        #region client_Connected

        private void clientEndpoint_OnConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Connected to Aggregator Client.");
#endif
        }

        #endregion

        #region client_Disconnect

        private void clientEndpoint_OnDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Disconnecting from Aggregator Client.");
#endif
            this.connectionToAggDataFlow.Dispose();
            this.connectionToAggAggregator.Dispose();
            if (this.connectionToParentDAClient != null)
                this.connectionToParentDAClient.Dispose();
        }

        #endregion

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAProtocolStackManager " + this.debug_identifier + " : Disposing");
#endif
            base._Dispose();
        }
    }
}
