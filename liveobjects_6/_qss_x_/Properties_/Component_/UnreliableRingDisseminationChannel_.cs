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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UnreliableRingDisseminationChannel,
        "UnreliableRingDisseminationChannel", "Unreliable Ring Dissemination.")]
    public sealed class UnreliableRingDisseminationChannel_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS._qss_x_.Properties_.Component_.Ring_<QS.Fx.Serialization.ISerializable>,
            QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public UnreliableRingDisseminationChannel_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            [QS.Fx.Reflection.Parameter("fanout", QS.Fx.Reflection.ParameterClass.Value)]
            int _fanout,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            [QS.Fx.Reflection.Parameter("MTTA", QS.Fx.Reflection.ParameterClass.Value)]
            double _mtta,
            [QS.Fx.Reflection.Parameter("MTTB", QS.Fx.Reflection.ParameterClass.Value)]
            double _mttb,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _group_reference, _rate, _mtta, _mttb, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.UnreliableDisseminationChannel_.Constructor");
#endif
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
        }


        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channel_endpoint;

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
            this._Send_Ring(null, _message);
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

        #region _Ring_Incoming

        protected override void _Ring_Incoming(QS.Fx.Base.Identifier _identifier, QS.Fx.Serialization.ISerializable _message)
        {
            if (_message != null && _message is MessageClass)
            {
                // forward message up to the client
                if (this._channel_endpoint.IsConnected)
                {
                    this._channel_endpoint.Interface.Receive((MessageClass)_message);
                }

                // also forward message to neighbors
                this._Send_Ring(_identifier, (MessageClass)_message);
            }
        }

        #endregion

        #region _Send_Ring

        private void _Send_Ring(QS.Fx.Base.Identifier _source, MessageClass _message)
        {
            lock (this)
            {
                if (this._Successor != null && (_source == null || this._Successor.Identifier.CompareTo(_source) != 0))
                {
                    this._Ring_Outgoing(this._Successor.Identifier, _message);
                }
            }
        }

        #endregion
    }

}
