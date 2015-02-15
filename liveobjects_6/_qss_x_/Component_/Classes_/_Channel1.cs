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
using System.ServiceModel;
using System.Runtime.Serialization;

namespace QS._qss_x_.Component_.Classes_
{
/*
    public static class Channel1
    {
        #region Interface IClient

        [ServiceContract]
        public interface IClient
        {
            [OperationContract(IsOneWay = true)]
            void Initialize(byte[] _checkpoint);

            [OperationContract(IsOneWay = false)]
            byte[] Checkpoint();

            [OperationContract(IsOneWay = true)]
            void Receive(byte[] _message);
        }

        #endregion

        #region Class Channel

        [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Channel1, "Channel1", "")]
        public sealed class Channel<
            [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
            [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
             : QS.TMS.Inspection.Inspectable, IClient,
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
            where MessageClass : class, QS.Fx.Serialization.ISerializable
            where CheckpointClass : class, QS.Fx.Serialization.ISerializable
        {
            #region Constructor

            public Channel(
                [QS.Fx.Reflection.Parameter("serveraddress", QS.Fx.Reflection.ParameterClass.Value)] string _serveraddress,
                [QS.Fx.Reflection.Parameter("channelid", QS.Fx.Reflection.ParameterClass.Value)] string _channelid)
            {
                this._serveraddress = _serveraddress;
                this._channelid = _channelid;

                WSHttpBinding _binding = new WSHttpBinding();
                _binding.Security.Mode = SecurityMode.Message;
                _binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
                this._channelfactory = new ChannelFactory<IServer>(_binding, new EndpointAddress(this._serveraddress));
                this._controller = _channelfactory.CreateChannel();

                this._channelendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>(this);

                this._channelendpoint.OnConnect += new QS.Fx.Base.Callback(this._ChannelConnectCallback);
                this._channelendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ChannelDisconnectCallback);
            }

            #endregion

            #region Fields

            [QS.TMS.Inspection.Inspectable("address")]
            private string _serveraddress;

            [QS.TMS.Inspection.Inspectable("channelid")]
            private string _channelid;

            private ChannelFactory<IServer> _channelfactory;
            private IServer _controller;

            [QS.TMS.Inspection.Inspectable("endpoint")]
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _channelendpoint;

            #endregion

            #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

            QS.Fx.Endpoint.Classes.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>>
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
            {
                get { return this._channelendpoint; }
            }

            #endregion

            #region _ChannelConnectCallback

            private void _ChannelConnectCallback()
            {
            }

            #endregion

            #region _ChannelDisconnectCallback

            private void _ChannelDisconnectCallback()
            {
            }

            #endregion

            #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

            void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
            {
            }

            #endregion

            #region IClient Members

            void IClient.Initialize(byte[] _checkpoint)
            {
                throw new NotImplementedException();
            }

            byte[] IClient.Checkpoint()
            {
                throw new NotImplementedException();
            }

            void IClient.Receive(byte[] _message)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion

        #region Interface IServer

        [ServiceContract(CallbackContract = typeof(IClient))]
        public interface IServer
        {
            [OperationContract(IsOneWay = true)]
            void Send(byte[] _message);
        }

        #endregion

        #region Class Controller

        [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Channel1Controller, "Channel1Controller", "")]
        public sealed class Controller : QS.TMS.Inspection.Inspectable, IServer, QS.Fx.Object.Classes.IObject
        {
            #region Constructor

            public Controller(
                [QS.Fx.Reflection.Parameter("serveraddress", QS.Fx.Reflection.ParameterClass.Value)] string _serveraddress)
            {
                this._serveraddress = _serveraddress;
            }

            #endregion

            #region Fields

            [QS.TMS.Inspection.Inspectable("address")]
            private string _serveraddress;

            private IClient client;

            #endregion

            #region IServer Members

            void IServer.Send(byte[] _message)
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        #endregion
    }
*/
}
