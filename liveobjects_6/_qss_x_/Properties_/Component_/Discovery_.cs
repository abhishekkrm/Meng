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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Discovery, "Properties Framework Discovery Object")]
    public sealed class Discovery_<
        [QS.Fx.Reflection.Parameter("IdentifierClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IdentifierClass,
        [QS.Fx.Reflection.Parameter("IncarnationClass", QS.Fx.Reflection.ParameterClass.ValueClass)] IncarnationClass,
        [QS.Fx.Reflection.Parameter("NameClass", QS.Fx.Reflection.ParameterClass.ValueClass)] NameClass,
        [QS.Fx.Reflection.Parameter("AddressClass", QS.Fx.Reflection.ParameterClass.ValueClass)] AddressClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Object.Classes.IDiscovery<IdentifierClass, IncarnationClass, NameClass, AddressClass>,
        QS.Fx.Interface.Classes.IMembershipChannel<IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>
        where IdentifierClass : class, QS.Fx.Serialization.ISerializable, IEquatable<IdentifierClass>
        where IncarnationClass : class, QS.Fx.Serialization.ISerializable, QS.Fx.Base.IIncrementable<IncarnationClass>, IComparable<IncarnationClass>, new()
        where NameClass : class, QS.Fx.Serialization.ISerializable
        where AddressClass : class, QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Discovery_
        (
            QS.Fx.Object.IContext _mycontext,

            [QS.Fx.Reflection.Parameter("transport", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ITransport<
                    QS.Fx.Base.Address,
                    QS.Fx.Serialization.ISerializable>> _transport_reference,

            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_.Constructor");
#endif

            this._membership_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IMembershipChannel<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
                    QS.Fx.Interface.Classes.IMembershipChannel<
                        IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>(this);
            this._membership_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Connect)));
                    });
            this._membership_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Disconnect)));
                    });
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>
                    _membership_endpoint;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IDiscovery Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>,
            QS.Fx.Interface.Classes.IMembershipChannel<
                IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>>
        QS.Fx.Object.Classes.IDiscovery<IdentifierClass, IncarnationClass, NameClass, AddressClass>.Membership
        {
            get { return this._membership_endpoint; }
        }

        #endregion

        #region IMembershipChannel Members

        void QS.Fx.Interface.Classes.IMembershipChannel<
            IncarnationClass, QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>.Member(
                QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Membership_Member), _member));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
            }
        }
        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Dispose");
#endif

            lock (this)
            {
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Start");
#endif

            base._Start();

            lock (this)
            {
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Stop");
#endif

            lock (this)
            {
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Membership_Connect

        private void _Membership_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Membership_Connect");
#endif
        }

        #endregion

        #region _Membership_Disconnect

        private void _Membership_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Membership_Disconnect");
#endif
        }

        #endregion

        #region _Membership_Member

        private void _Membership_Member(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass> _member =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Value.Classes.IMember<IdentifierClass, IncarnationClass, NameClass, AddressClass>>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Discovery_._Member\n\n" + QS.Fx.Printing.Printable.ToString(_member) + "\n\n");
#endif

            lock (this)
            {
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
