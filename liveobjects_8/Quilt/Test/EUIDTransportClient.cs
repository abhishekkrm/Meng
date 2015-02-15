/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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
using System.Linq;
using System.Text;

using QS.Fx.Value;
using QS.Fx.Serialization;

namespace Quilt.Test
{
    [QS.Fx.Reflection.ComponentClass("DC610F96540C45a78B050D4ECDF88762", "EUIDTransportClient")]
    public sealed class EUIDTransportClient:
        QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, QS.Fx.Serialization.ISerializable>
    {
        #region Constructors

        public EUIDTransportClient(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("EUIDTransport", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.ITransport<EUIDAddress, ISerializable>>
            _transport_object_reference)
            : base(_mycontext, true)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransportClient.Constructor");
#endif
            this._mycontext = _mycontext;

            if (_transport_object_reference != null)
            {
                this._transport_endpt =
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.ITransport<EUIDAddress, ISerializable>,
                        QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, ISerializable>>(this);

                this._transport_endpt.OnConnected += new QS.Fx.Base.Callback(this._Transport_ConnectedCallback);
                this._transport_endpt.OnDisconnect += new QS.Fx.Base.Callback(this._Transport_DisconnectCallback);

                this._transport_conn = this._transport_endpt.Connect(_transport_object_reference.Dereference(_mycontext).Transport);

            }

        }

        #endregion

        #region Field

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ITransport<EUIDAddress, ISerializable>,
            QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, ISerializable>> _transport_endpt;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _transport_conn;

        [QS.Fx.Base.Inspectable]
        private EUIDAddress _self_euid;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Transport_ConnectedCallback

        private void _Transport_ConnectedCallback()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransportClient.Transport Connected");
#endif
            
        }

        #endregion

        #region _Transport_DisconnectCallback

        private void _Transport_DisconnectCallback()
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransportClient.Transport Disconnect");
#endif

        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ITransportClient<EUIDAddress,ISerializable> Members

        void QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, ISerializable>.Address(EUIDAddress address)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransportClient.ITransportClient.Address" + address.String);
#endif

            this._self_euid = address;
        }

        void QS.Fx.Interface.Classes.ITransportClient<EUIDAddress, ISerializable>.Connected(EUIDAddress address, QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICommunicationChannel<ISerializable>> channel)
        {

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.EUIDTransportClient.ITransportClient.Connected" + address.String);
#endif
            
        }

        #endregion
    }
}
