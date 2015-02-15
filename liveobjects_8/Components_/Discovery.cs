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

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass("015B9FC183DB4d8cA834C04C31E52BB1", "Discovery for STUN")]
    public sealed class Discovery<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IDiscovery<IdentifierClass, AddressClass>,
        QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>,
        QS.Fx.Interface.Classes.IMembershipChannelClient<
        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>, QS.Fx.Serialization.IStringSerializable
        where AddressClass : class, QS.Fx.Serialization.ISerializable, IEquatable<AddressClass>, QS.Fx.Serialization.IStringSerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Discovery
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("membership", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IMembershipChannel<
                    QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>> _membership_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug)
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery.Constructor");
#endif

            if (_membership_reference == null)
                _mycontext.Error("Membership channel reference cannot be NULL.");

            this._discovery_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDiscoveryClient<IdentifierClass, AddressClass>,
                    QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>>(this);

            this._discovery_endpoint.OnConnected += new QS.Fx.Base.Callback(Discovery_Connected);


            this._membership_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IMembershipChannel<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>,
                    QS.Fx.Interface.Classes.IMembershipChannelClient<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>>(this);
            this._membership_connection = this._membership_endpoint.Connect(_membership_reference.Dereference(_mycontext).Membership);

            this._membership_endpoint.OnConnected += new QS.Fx.Base.Callback(Membership_OnConnected);

            this.incarnation = new QS.Fx.Base.Incarnation();
            this.name = new QS.Fx.Base.Name();
            this.memberdict = new Dictionary<string, QS.Fx.Value.Member<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDiscoveryClient<IdentifierClass, AddressClass>,
                    QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>> _discovery_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannel<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannelClient<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>> _membership_endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _membership_connection;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation incarnation;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Name name;

        [QS.Fx.Base.Inspectable]
        
        private Dictionary<string, 
            QS.Fx.Value.Member<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>> memberdict;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Discovery_Connected

        private void Discovery_Connected()
        {
            this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.Discovery_Connected))); 
        }

        private void Discovery_Connected(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery.Discovery Connected");
#endif
        }

        #endregion

        #region Membership_OnConnected

        private void Membership_OnConnected()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery.Membership Connected");
#endif
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDiscovery<IdentifierClass,AddressClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IDiscoveryClient<IdentifierClass, AddressClass>, QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>> QS.Fx.Object.Classes.IDiscovery<IdentifierClass, AddressClass>.Discovery
        {
            get { return this._discovery_endpoint; }
        }

        #endregion

        #region IDiscoveryOps<IdentifierClass,AddressClass> Members

        void QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>.Register(IdentifierClass id, QS.Fx.Value.Classes.IAddressCollection<AddressClass> addrs)
        {
            QS.Fx.Value.Member<
                IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass> member = 
                new QS.Fx.Value.Member<
                    IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>(id, true, this.incarnation, this.name, addrs.Addresses.ToArray());
            this._membership_endpoint.Interface.Member(member);
        }

        void QS.Fx.Interface.Classes.IDiscoveryOps<IdentifierClass, AddressClass>.Lookup(IdentifierClass id)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<IdentifierClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this.MemberLookup), id));
        }

        #endregion

        #region IMembershipChannelClient<Incarnation,IMember<IdentifierClass,Incarnation,Name,AddressClass>> Members

        void QS.Fx.Interface.Classes.IMembershipChannelClient<
            QS.Fx.Base.Incarnation,
            QS.Fx.Value.Classes.IMember<
                IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>>.Membership(QS.Fx.Value.Classes.IMembership<QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass>> membership)
        {
            foreach (QS.Fx.Value.Member<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass> member in membership.Members)
            {
                lock (memberdict)
                {
                    QS.Fx.Value.Member<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass> oldmember;
                    if (!memberdict.TryGetValue(member.Identifier.ToString(), out oldmember))
                    {
                        memberdict.Add(member.Identifier.ToString(), member);
                    }
                    else
                    {
                        if (oldmember.Incarnation.CompareTo(member.Incarnation) < 0)
                        {
                            oldmember = member;
                        }
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region MemberLookup

        private void MemberLookup(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<IdentifierClass>)_event;
            IdentifierClass id = _event_._Object;

            AddressClass[] addresses = null;

            lock (memberdict)
            {
                QS.Fx.Value.Member<IdentifierClass, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, AddressClass> member;
                if (memberdict.TryGetValue(id.ToString(), out member))
                {
                    addresses = member.Addresses;
                }
            }

            if (addresses != null)
            {
                AddressArray<AddressClass> addrs = new AddressArray<AddressClass>(addresses);
                this._discovery_endpoint.Interface.Found(id, addrs);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        private class AddressArray<AddressClass>
            : QS.Fx.Value.Classes.IAddressCollection<AddressClass>
        {
            public AddressArray(AddressClass[] addresses)
            {
                this.addresses = addresses;
            }

            private AddressClass[] addresses;
        
            #region IAddressCollection<AddressClass> Members

            IEnumerable<AddressClass>  QS.Fx.Value.Classes.IAddressCollection<AddressClass>.Addresses
            {
	            get { return this.addresses; }
            }

            #endregion
        }
    }
}
