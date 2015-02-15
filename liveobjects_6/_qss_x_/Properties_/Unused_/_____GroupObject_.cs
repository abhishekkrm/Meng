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

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Group, "Group", "An implementation of a group.")]
    public sealed class GroupObject_<
        [QS.Fx.Reflection.Parameter("VersionClass", QS.Fx.Reflection.ParameterClass.ValueClass)] VersionClass,
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : IGroupObject_,
        IGroupObject_internals_,
        IMembershipClient_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>, 
        ITransportClient_<AddressClass, string>
/-*
        where VersionClass : class, QS.Fx.Serialization.ISerializable
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable
        where AddressClass : class, QS.Fx.Serialization.ISerializable
*-/ 
    {
        #region Constructor

        public GroupObject_
        (
            [QS.Fx.Reflection.Parameter("membership", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<IMembershipObject_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>> 
                    _membership_object_reference,

            [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<ITransportObject_<AddressClass, string>> 
                    _transport_object_reference
        )
        {           
            if (_membership_object_reference != null)
            {
                this._membership_endpoint = 
                    _mycontext.DualInterface<
                        IMembership_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>,
                        IMembershipClient_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>>(this);

                this._membership_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Membership_ConnectedCallback);
                this._membership_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Membership_DisconnectCallback);
                
                this._membership_connection = this._membership_endpoint.Connect(_membership_object_reference.Object.Membership);
            }

            if (_transport_object_reference != null)
            {
                this._transport_endpoint = 
                    _mycontext.DualInterface<
                        ITransport_<AddressClass, string>, 
                        ITransportClient_<AddressClass, string>>(this);

                this._transport_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Transport_ConnectedCallback);
                this._transport_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Transport_DisconnectCallback);

                this._transport_connection = this._transport_endpoint.Connect(_transport_object_reference.Object.Transport);
            }
        }

        #endregion

        #region Fields

        private GroupObject_UI_ _ui;
        private QS.Fx.Endpoint.Internal.IExportedUI _ui_endpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            IMembership_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>,
            IMembershipClient_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>> 
                _membership_endpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            ITransport_<AddressClass, string>, 
            ITransportClient_<AddressClass, string>> 
                _transport_endpoint;
        private QS.Fx.Endpoint.IConnection _membership_connection, _transport_connection;
        private Queue<string> _log = new Queue<string>();

        #endregion

        #region IGroupObject_ Members

        QS.Fx.Endpoint.Classes.IExportedUI IGroupObject_._UI
        {
            get 
            {
                lock (this)
                {
                    if (this._ui == null)
                        this._ui = new GroupObject_UI_(this);
                    if (this._ui_endpoint == null)
                        this._ui_endpoint = _mycontext.ExportedUI(this._ui);
                    return this._ui_endpoint;
                }
            }
        }

        #endregion

        #region IGroupObject_internals_ Members

        IEnumerable<string> IGroupObject_internals_._Messages()
        {
            lock (this)
            {
                IEnumerable<string> _messages = _log;
                _log = new Queue<string>();
                return _messages;
            }
        }

        #endregion
        
        #region _Refresh

        private void _Refresh()
        {
            IGroupObject_UI_ _ui = this._ui;
            if (_ui != null)
                _ui._Refresh();
        }

        #endregion

        #region _Membership_ConnectedCallback

        private void _Membership_ConnectedCallback()
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.AppendLine("____________________CONNECT(membership)");
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion

        #region _Membership_DisconnectCallback

        private void _Membership_DisconnectCallback()
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.AppendLine("____________________DISCONNECT(membership)");
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion

        #region _Transport_ConnectedCallback

        private void _Transport_ConnectedCallback()
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.AppendLine("____________________CONNECT(transport)");
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion

        #region _Transport_DisconnectCallback

        private void _Transport_DisconnectCallback()
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.AppendLine("____________________DISCONNECT(transport)");
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion

        #region ITransportClient_<AddressClass,string> Members

        void ITransportClient_<AddressClass, string>.Receive(AddressClass _address, string _message)
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.Append("____________________MESSAGE(");
                _s.Append(QS.Fx.Printing.Printable.ToString(_address));
                _s.AppendLine(")");
                _s.AppendLine(_message);
                _s.AppendLine();
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion

        #region IMembershipClient_<VersionClass,IdentifierClass,IncarnationClass,AddressClass> Members

        void IMembershipClient_<VersionClass, IdentifierClass, IncarnationClass, AddressClass>._MembershipView(
            IMembershipView_<VersionClass, IdentifierClass, IncarnationClass, AddressClass> _membership)
        {
            lock (this)
            {
                StringBuilder _s = new StringBuilder();
                _s.AppendLine("____________________MEMBERSHIP");
                _s.AppendLine(QS.Fx.Printing.Printable.ToString(_membership));
                _s.AppendLine();
                this._log.Enqueue(_s.ToString());
            }
            this._Refresh();
        }

        #endregion
    }
*/ 
}
