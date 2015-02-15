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

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DAClient, "Delegation Channel Client")]
    public sealed class DAClient :
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDataFlow, QS.Fx.Object.Classes.IDataFlow, QS.Fx.Interface.Classes.IDataFlowClient
    {
        #region Constructor

        public DAClient (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("identifier", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Identifier _identifier,
            [QS.Fx.Reflection.Parameter("delegationchannel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>> _delegation_reference,
            [QS.Fx.Reflection.Parameter("name", QS.Fx.Reflection.ParameterClass.Value)]
            string name,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug )
            : base(_mycontext, _debug)
        {
            this._mycontext = _mycontext;

            if (name == null)
                name = "";
            this.debug_identifier = name;

            this._identifier = new QS.Fx.Base.Identifier(Guid.NewGuid());
            //this.delegated = false;
            this._delegation_reference = _delegation_reference;
            this._delegation_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>>(this);
            this._delegation_endpoint.OnConnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Connect)));
                    });
            this._delegation_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Connected)));
                    });
            this._delegation_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Disconnect)));
                    });
            this.daProxy = this._delegation_reference.Dereference(_mycontext);
            this.connectionToDA = this._delegation_endpoint.Connect(this.daProxy.Delegation);
            this.appEndpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDataFlowClient,
                QS.Fx.Interface.Classes.IDataFlow>(this);
            this.appEndpoint.OnConnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.appConnect)));
                    });
            this.appEndpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.appConnected)));
                    });
            this.appEndpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.appDisconnect)));
                    });
            this.protocolChannelEndpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDataFlow,
                QS.Fx.Interface.Classes.IDataFlowClient>(this);
            this.protocolChannelEndpoint.OnConnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.channelConnect)));
                    });
            this.protocolChannelEndpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.channelConnected)));
                    });
            this.protocolChannelEndpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.channelDisconnect)));
                    });
        }

        #endregion

        #region Fields
        //For debugging simulation.
        private string debug_identifier;

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>> _delegation_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject> _delegation_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>> _delegation_endpoint;
        private QS.Fx.Object.Classes.IDelegationChannel<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject> daProxy;
        private QS.Fx.Endpoint.IConnection connectionToDA;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> protocolStack;
        private QS.Fx.Object.Classes.IDataFlow protocolEndpoint;
        //private bool delegated;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDataFlowClient,
            QS.Fx.Interface.Classes.IDataFlow> appEndpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDataFlow,
            QS.Fx.Interface.Classes.IDataFlowClient> protocolChannelEndpoint;

        #endregion

        #region IDataFlow Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IDataFlowClient, QS.Fx.Interface.Classes.IDataFlow> QS.Fx.Object.Classes.IDataFlow.DataFlow
        {
            get { return this.appEndpoint; }
        }

        #endregion

        #region IDataFlow Members

        void QS.Fx.Interface.Classes.IDataFlow.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " received message from application. ");
#endif
            if (this.protocolChannelEndpoint.IsConnected)
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
                this._logger.Log("DAClient " + this.debug_identifier + " received message from DAProtocolStackManager. ");
#endif
            if (this.appEndpoint.IsConnected)
            {
                this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<int, long, QS.Fx.Serialization.ISerializable>(new QS._qss_x_.Properties_.Base_.EventCallback_(this.fromDAClientToApp), id, version, value));
            }
        }

        #endregion

        #region IDelegationChannelClient<Identifier,IObject> Members

        void QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>.Delegate(QS.Fx.Base.Identifier id, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> o)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " received delegation. ");
#endif
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(new QS._qss_x_.Properties_.Base_.EventCallback_(this.delegateProtocolStack), id, o));
        }

        void QS.Fx.Interface.Classes.IDelegationChannelClient<QS.Fx.Base.Identifier, QS.Fx.Object.Classes.IObject>.Undelegate(QS.Fx.Base.Identifier id)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DACLient " + this.debug_identifier + " received undelegate");
#endif
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.undelegateProtocolStack)));
        }

        #endregion

        private void fromDAClientToStack(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " passing message from application to stack. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable> _event2 =
                (QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable>)_event;
            int id = _event2._Object1;
            long version = _event2._Object2;
            QS.Fx.Serialization.ISerializable value = _event2._Object3;
            this.protocolChannelEndpoint.Interface.Send(id, version, value);

        }

        private void fromDAClientToApp(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " received passing message from stack to application. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable> _event2 =
                (QS._qss_x_.Properties_.Base_.IEvent_<int, long, QS.Fx.Serialization.ISerializable>)_event;
            int id = _event2._Object1;
            long version = _event2._Object2;
            QS.Fx.Serialization.ISerializable value = _event2._Object3;
            this.appEndpoint.Interface.Send(id, version, value);
        }

        private void delegateProtocolStack(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " handling connect protocol stack. ");
#endif
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _event2 =
               (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>)_event;
            this.protocolStack = _event2._Object2;
            if (this.appEndpoint.IsConnected)
                this.connectProtocolStack();
        }

        private void undelegateProtocolStack(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " handling undelegate protocol stack. ");
#endif
            this.protocolStack = null;
        }

        #region connectProtocolStack

        private void connectProtocolStack()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " Connecting protocol stack.");
#endif
            QS.Fx.Object.Classes.IObject o = this.protocolStack.Dereference(this._mycontext);
            this.protocolEndpoint = (QS.Fx.Object.Classes.IDataFlow)o;
            this.protocolChannelEndpoint.Connect(protocolEndpoint.DataFlow);
        }

        #endregion

        #region _Delegation_Connect

        private void _Delegation_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " connecting to delegation channel. ");
#endif
        }

        #endregion

        #region _Delegation_Connected

        private void _Delegation_Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " connected to delegation channel. ");
#endif

            lock (this)
            {
                if (this._delegation_endpoint.IsConnected)
                    this._delegation_endpoint.Interface.Register(this._identifier);
            }
        }

        #endregion

        #region _Delegation_Disconnect

        private void _Delegation_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " disconnecting from delegation channel. ");
#endif
        }

        #endregion

        #region appConnect

        private void appConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " connecting to application. ");
#endif
        }

        #endregion

        #region appConnected

        private void appConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " connected to application. ");
#endif
            if (this.protocolStack != null)
            {
                    this.connectProtocolStack();
            }
        }

        #endregion

        #region appDisconnect

        private void appDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " disconnecting from client.");
#endif
            lock (this)
            {
                if (this.protocolChannelEndpoint.IsConnected)
                {
                    this.protocolChannelEndpoint.Disconnect();
                    this._delegation_endpoint.Disconnect();
                    if(this.protocolEndpoint is IDisposable)
                        ((IDisposable)this.protocolEndpoint).Dispose();
                    this.protocolStack = null;
                }
            }
        }

        #endregion

        #region channelConnect

        private void channelConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DA Client " + this.debug_identifier + " connecting to protocol stack channel");
#endif
        }

        #endregion

        #region channelConnected

        private void channelConnected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DA Client " + this.debug_identifier + " connected to protocol stack channel");
#endif
        }

        #endregion

        #region channelDisconnect

        private void channelDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("DAClient " + this.debug_identifier + " disconnecting form protocol stack channel.");
#endif
        }

        #endregion
    }
}
