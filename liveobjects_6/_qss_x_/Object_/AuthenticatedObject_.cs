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
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Threading;

namespace QS._qss_x_.Object_
{
    public sealed class AuthenticatedObject_
        : RealProxy, 
        QS.Fx.Interface.Classes.IAuthenticatingClient<QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public AuthenticatedObject_(
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref,
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authorityref)
            : base(_objectref.ObjectClass.UnderlyingType)
        {
            this._mycontext = _mycontext;
            this._objectref = _objectref;
            this._authorityref = _authorityref;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject>> _authorityref;
        private QS.Fx.Object.Classes.IObject _object;
        private QS.Fx.Object.Classes.IAuthenticating1<QS.Fx.Object.Classes.IObject> _authority;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAuthenticating<QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IAuthenticatingClient<QS.Fx.Object.Classes.IObject>> _authentication;
        private QS.Fx.Endpoint.IConnection _connection;
        private bool _ready, _authenticated;
        private ManualResetEvent _readyevent;

        #endregion

        #region Invoke

        public override System.Runtime.Remoting.Messaging.IMessage Invoke(System.Runtime.Remoting.Messaging.IMessage _message)
        {
            lock (this)
            {
                if (_authority == null)
                {
                    _authority = _authorityref.Dereference(_mycontext);
                    _authentication = _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.IAuthenticating<QS.Fx.Object.Classes.IObject>,
                        QS.Fx.Interface.Classes.IAuthenticatingClient<QS.Fx.Object.Classes.IObject>>(this);
                    _connection = _authentication.Connect(_authority.Authentication);
                }

                if (_object == null)
                {
                    _readyevent = new ManualResetEvent(false);
                    _authentication.Interface.Authenticate(_objectref);
                    _object = _objectref.Dereference(_mycontext);
                }

                if (!_ready)
                    _readyevent.WaitOne();

                if (!_authenticated)
                    throw new UnauthorizedAccessException();

                IMethodCallMessage _request = (IMethodCallMessage) _message;
                QS._qss_x_.Endpoint_.Internal_.Endpoint_ _endpoint = 
                    (QS._qss_x_.Endpoint_.Internal_.Endpoint_) 
                        _request.MethodBase.Invoke(_object, null);

                IMethodReturnMessage _response = new ReturnMessage(_endpoint, null, 0, null, _request);

                return _response;
            }
        }

        #endregion

        #region IAuthenticatingClient<IObject> Members

        void QS.Fx.Interface.Classes.IAuthenticatingClient<QS.Fx.Object.Classes.IObject>.Authenticated(bool authenticated)
        {
            this._authenticated = authenticated;
            this._ready = true;
            if (this._readyevent != null)
                this._readyevent.Set();
        }

        #endregion
    }
}
