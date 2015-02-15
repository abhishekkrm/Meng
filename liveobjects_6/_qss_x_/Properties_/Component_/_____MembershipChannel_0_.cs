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

// #define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS._qss_x_.Properties_.Component_.Classes_._MembershipChannel_0)]
    public sealed class MembershipChannel_0_<
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IMembershipChannel<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>,
        QS.Fx.Interface.Classes.IMembershipChannel<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>
        where AddressClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public MembershipChannel_0_
        (
/-*
            [QS.Fx.Reflection.Parameter("incarnation", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Value.Classes.IMembership<QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>
            _membership
*-/
        )
        {
            this._membership_endpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IMembershipChannelClient<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>,
                    QS.Fx.Interface.Classes.IMembershipChannel<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>>(this);
            this._membership_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
/-*
            List<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>> _members =
                new List<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>();
            this._membership =
                new QS.Fx.Value.Membership<
                    QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<
                        QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>(
                            new QS.Fx.Base.Incarnation(0U), false, _members.ToArray());
*-/
        }

        #endregion

        #region Fields
 
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>>
                _membership_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMembership<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>> _membership;

        #endregion

        #region IMembershipChannel Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>>
        QS.Fx.Object.Classes.IMembershipChannel<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>.Membership
        {
            get { return this._membership_endpoint; }
        }

        #endregion

        #region IMembershipChannel Members

        void QS.Fx.Interface.Classes.IMembershipChannel<
            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass>>.Member(
                QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, AddressClass> _member)
        {
        }

        #endregion

        #region _Connect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            try
            {
                if (_membership_endpoint.IsConnected)
                    _membership_endpoint.Interface.Membership(_membership);
            }
            catch (Exception)
            {
                try
                {
                    _membership_endpoint.Disconnect();
                }
                catch (Exception)
                {
                }
            }
        }

        #endregion
    }
*/
}
