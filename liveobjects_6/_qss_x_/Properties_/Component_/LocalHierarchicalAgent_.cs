/*

Copyright (c) 2004-2009 Petko Nikolov. All rights reserved.

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
#define STATISTICS

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.LocalHierarchicalAgent,
        "LocalHierarchicalAgent", "LocalHierarchicalAgent")]
    public sealed class LocalHierarchicalAgent_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS._qss_x_.Properties_.Component_.Base_,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
            //QS.Fx.Object.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            //QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public LocalHierarchicalAgent_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("policy", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _dissemination_policy,
            [QS.Fx.Reflection.Parameter("higher_agent", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _higher_agent,
            [QS.Fx.Reflection.Parameter("loopback", QS.Fx.Reflection.ParameterClass.Value)]
            bool loopback
        )
            : base(_mycontext, true)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.LocalHierarchicalAgent_.Constructor");
#endif
            this.loopback = loopback;

            this._channel_endpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

            this._channel_endpoint.OnConnected += new QS.Fx.Base.Callback(
                delegate
                {
                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._ChannelConnectCallback)));
                });

            this._channel_endpoint.OnDisconnect += new QS.Fx.Base.Callback(
                delegate
                {
                    this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._ChannelDisconnectCallback)));
                });

            if (_dissemination_policy != null)
            {
                this._policy_class = new LocalHierarchicalAgent_<MessageClass, CheckpointClass>.PolicyChannel_(this);

                this._dissemination_policy = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this._policy_class);

                this._policy_connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._dissemination_policy).Connect(
                    _dissemination_policy.Dereference(_mycontext).Channel);
            }

            if (_higher_agent != null)
            {
                this._higher_agent_class = new LocalHierarchicalAgent_<MessageClass, CheckpointClass>.HigherAgentChannel_(this);

                this._higher_agent = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>>(this._higher_agent_class);

                this._higher_agent_connection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._higher_agent).Connect(
                    _higher_agent.Dereference(_mycontext).Channel);
            }
        }


        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_endpoint;

        [QS.Fx.Base.Inspectable("dissemination policy")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _dissemination_policy;

        private PolicyChannel_ _policy_class;

        private QS.Fx.Endpoint.IConnection _policy_connection;

        [QS.Fx.Base.Inspectable("higher agent")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>> _higher_agent;

        private HigherAgentChannel_ _higher_agent_class;

        private QS.Fx.Endpoint.IConnection _higher_agent_connection;

        [QS.Fx.Base.Inspectable("loopback")]
        private bool loopback;

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { return this._channel_endpoint; }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            // loopback to application?
            if (this.loopback)
                this._channel_endpoint.Interface.Receive(_message);

            // forward to higher agent, if such exists
            if (this._higher_agent != null && this._higher_agent.IsConnected)
                this._higher_agent.Interface.Send(_message);

            // forward to local dissemination policy
            if (this._dissemination_policy != null && this._dissemination_policy.IsConnected)
                this._dissemination_policy.Interface.Send(_message);
        }

        #endregion

        #region _ChannelConnectCallback

        private void _ChannelConnectCallback(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            this._channel_endpoint.Interface.Initialize(null);
        }

        #endregion

        #region _ChannelDisconnectCallback

        private void _ChannelDisconnectCallback(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        private class PolicyChannel_ : QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        {
            private LocalHierarchicalAgent_<MessageClass, CheckpointClass> lha;

            public PolicyChannel_(LocalHierarchicalAgent_<MessageClass, CheckpointClass> lha)
            {
                this.lha = lha;
            }

            #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
            {
                // forward to application / lower agent
                if (lha._channel_endpoint != null && lha._channel_endpoint.IsConnected)
                    lha._channel_endpoint.Interface.Receive(_message);

                // forward to higher agent, if such exists
                if (lha._higher_agent != null && lha._higher_agent.IsConnected)
                    lha._higher_agent.Interface.Send(_message);
            }

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
            {
            }

            CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
            {
                return null;
            }

            #endregion
        }

        private class HigherAgentChannel_ : QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>
        {
            private LocalHierarchicalAgent_<MessageClass, CheckpointClass> lha;

            public HigherAgentChannel_(LocalHierarchicalAgent_<MessageClass, CheckpointClass> lha)
            {
                this.lha = lha;
            }

            #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Receive(MessageClass _message)
            {
                // forward to application / lower agent
                if (lha._channel_endpoint != null && lha._channel_endpoint.IsConnected)
                    lha._channel_endpoint.Interface.Receive(_message);

                // forward to local dissemination policy
                if (lha._dissemination_policy != null && lha._dissemination_policy.IsConnected)
                    lha._dissemination_policy.Interface.Send(_message);
            }

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Initialize(CheckpointClass _checkpoint)
            {

            }

            CheckpointClass QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>.Checkpoint()
            {
                return null;
            }

            #endregion
        }

    }

}
