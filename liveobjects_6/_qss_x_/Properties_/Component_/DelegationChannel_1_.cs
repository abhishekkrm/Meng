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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.DelegationChannel_1, "New Properties Framework Delegation Channel")]
    class DelegationChannel_1_<[QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("ObjectClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] ObjectClass>
        : QS._qss_x_.Properties_.Component_.Channel_<
            QS._qss_x_.Properties_.Value_.DelegationControl_, QS._qss_x_.Properties_.Value_.DelegationControl_, 
            DelegationChannel_<IdentifierClass, ObjectClass>.Connection_>,
        QS.Fx.Object.Classes.IDelegationChannel<IdentifierClass, ObjectClass>,
        QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>
        where ObjectClass : class, QS.Fx.Object.Classes.IObject
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public DelegationChannel_1_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("xmlloader", QS.Fx.Reflection.ParameterClass.Value)]
           QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<
            QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> loader_reference,
            [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<
                    QS.Fx.Base.Address,
                    QS.Fx.Serialization.ISerializable>> _transport_reference,
            [QS.Fx.Reflection.Parameter("protocolstack", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<ObjectClass> protocolStack,
            [QS.Fx.Reflection.Parameter("address", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Base.Address _address,       
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _transport_reference, _address, (_address == null), _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_.Constructor");
#endif
            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
            this._loaderendpoint.Connect(loader_reference.Dereference(_mycontext).Endpoint);
            this.protocolStack = protocolStack;
            this._delegation_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>,
                    QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>>(this);
            this._delegation_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Connect)));
                    });
            this._delegation_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Disconnect)));
                    });
        }

        #endregion
            
        #region Fields

        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>,
            QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>> _delegation_endpoint;

        private QS.Fx.Object.IReference<ObjectClass> protocolStack;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDelegationChannel<IdentifierClass,ObjectClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IDelegationChannelClient<IdentifierClass, ObjectClass>, 
            QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>> 
                QS.Fx.Object.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Delegation
        {
            get { return this._delegation_endpoint; }
        }

        #endregion

        #region IDelegationChannel<IdentifierClass,ObjectClass> Members

        void QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Register(IdentifierClass _id)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Register), _id));
        }

        void QS.Fx.Interface.Classes.IDelegationChannel<IdentifierClass, ObjectClass>.Unregister(IdentifierClass _id)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Delegation_Unregister), _id));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Delegation_Connect

        private void _Delegation_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Delegation_Connect");
#endif
        }

        #endregion

        #region _Delegation_Disconnect

        private void _Delegation_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Delegation_Disconnect");
#endif
        }

        #endregion

        #region _Delegation_Register

        private void _Delegation_Register(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            IdentifierClass _identifier = ((QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Delegation_Register ( " + _identifier.ToString() + " )");
#endif

            this._Channel_Outgoing
            (
                new QS._qss_x_.Properties_.Value_.DelegationControl_
                (
                    QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Register_,
                    _identifier,
                    null
                )
            );
        }

        #endregion

        #region _Delegation_Unregister

        private void _Delegation_Unregister(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            IdentifierClass _identifier = ((QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Delegation_Unregister ( " + _identifier.ToString() + " )");
#endif

            this._Channel_Outgoing
            (
                new QS._qss_x_.Properties_.Value_.DelegationControl_
                (
                    QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Unregister_,
                    _identifier,
                    null
                )
            );
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Channel_Incoming

        protected override void _Channel_Incoming(QS._qss_x_.Properties_.Value_.DelegationControl_ _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Channel_Incoming  : " + QS.Fx.Printing.Printable.ToString(_message));
#endif

            switch (_message._Operation)
            {
                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Delegate_:
                    {
                        if (this._delegation_endpoint.IsConnected)
                        {
                            string objectxml = _message._Object;
                            QS.Fx.Object.IReference<ObjectClass> _objectref =
                                (QS.Fx.Object.IReference<ObjectClass>)this._loaderendpoint.Interface.Load(objectxml);
                           /* using (StringReader reader = new StringReader(objectxml))
                            {
                                try
                                {
                                    QS.Fx.Reflection.Xml.Root root = (QS.Fx.Reflection.Xml.Root)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Deserialize(reader);
                                    QS.Fx.Reflection.Xml.CompositeObject obj = (QS.Fx.Reflection.Xml.CompositeObject)root.Object;
                                    QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IObject>.Create(
                                    _objectref = (QS.Fx.Object.IReference<ObjectClass>)root.Object;
                                }
                                catch (Exception _exc)
                                {
                                    throw new Exception("Could not add object to the folder because the object could not be serialized.\n", _exc);
                                }
                            }*/
                            this._delegation_endpoint.Interface.Delegate((IdentifierClass) _message._Identifier, _objectref);
                        }
                    }
                    break;

                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Undelegate_:
                    {
                        this._delegation_endpoint.Interface.Undelegate((IdentifierClass) _message._Identifier);
                    }
                    break;

                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Register_:
                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Unregister_:
                    throw new NotSupportedException();

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connection_Connected

        protected override void _Connection_Connected(DelegationChannel_<IdentifierClass, ObjectClass>.Connection_ _connection)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Connection_Connected  : " + QS.Fx.Printing.Printable.ToString(_connection._Address));
#endif

        }

        #endregion

        #region _Connection_Disconnecting

        protected override void _Connection_Disconnecting(DelegationChannel_<IdentifierClass, ObjectClass>.Connection_ _connection)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Connection_Disconnecting  : " + QS.Fx.Printing.Printable.ToString(_connection._Address));
#endif

        }

        #endregion

        #region _Connection_Incoming

        protected override void _Connection_Incoming(
            DelegationChannel_<IdentifierClass, ObjectClass>.Connection_ _connection, QS._qss_x_.Properties_.Value_.DelegationControl_ _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.DelegationChannel_._Connection_Incoming  : " + QS.Fx.Printing.Printable.ToString(_message));
#endif

            switch (_message._Operation)
            {
                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Register_:
                    {
                        StringBuilder sb = new StringBuilder();
                        using (StringWriter writer = new StringWriter(sb))
                        {
                            try
                            {
                                (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(protocolStack.Serialize));
                            }
                            catch (Exception _exc)
                            {
                                throw new Exception("Could not add object to the folder because the object could not be serialized.\n", _exc);
                            }
                        }
                        string _objectxml = sb.ToString();

                        this._Connection_Outgoing
                        (
                            _connection,
                            new QS._qss_x_.Properties_.Value_.DelegationControl_
                            (
                                QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Delegate_,
                                _message._Identifier,
                                _objectxml
                            )
                        );
                    }
                    break;

                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Unregister_:
                    {
                        this._Connection_Outgoing
                        (
                            _connection,
                            new QS._qss_x_.Properties_.Value_.DelegationControl_
                            (
                                QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Undelegate_,
                                _message._Identifier,
                                null
                            )
                        );
                    }
                    break;

                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Delegate_:
                case QS._qss_x_.Properties_.Value_.DelegationControl_.Operation_.Undelegate_:
                    throw new NotSupportedException();

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class Connection_

        public new sealed class Connection_
            : QS._qss_x_.Properties_.Component_.Channel_<
                QS._qss_x_.Properties_.Value_.DelegationControl_, QS._qss_x_.Properties_.Value_.DelegationControl_, 
                DelegationChannel_<IdentifierClass, ObjectClass>.Connection_>.Connection_
        {
            #region Constructor

            public Connection_()
                : base()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            private bool _initialized;

            #endregion

            #region Accessors

            public bool _Initialized
            {
                get { return this._initialized; }
                set { this._initialized = value; }
            }

            #endregion
        }

        #endregion     

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
