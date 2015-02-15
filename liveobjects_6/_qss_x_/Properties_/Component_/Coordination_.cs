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
using System.Threading;

namespace QS._qss_x_.Properties_.Component_
{
/*
    public sealed class Coordination_ 
        : QS._qss_x_.Properties_.Component_.Properties_
/-*
        , QS.Fx.Interface.Classes.IRingClient<QS.Fx.Base.Incarnation, QS.Fx.Base.Index, QS.Fx.Serialization.ISerializable>
 
        QS._qss_x_.Properties_.Object_.IProperties_, 
        QS._qss_x_.Properties_.Interface_.IGroupClient_
*-/ 
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Coordination_
        (
/-*
            QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IGroup_> _group_reference,
            QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IProperties_> _properties_reference
*-/ 
            bool _debug
        )
            : base(_debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_.Constructor");
#endif

/-*
            this._group_reference = _group_reference;

//            if (_group_reference == null)
//                throw new Exception("Cannot run with group NULL.");
//            if (_properties_reference == null)
//                throw new Exception("Cannot run with properties NULL.");

            this._group_endpoint =
                _mycontext.DualInterface<
                    QS._qss_x_.Properties_.Interface_.IGroup_,
                    QS._qss_x_.Properties_.Interface_.IGroupClient_>(this);
            this._group_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Connect)));
                    });
            this._group_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Disconnect)));
                    });

            this._top_dispatch = new QS._qss_x_.Properties_.Base_.Dispatch_(new QS._qss_x_.Properties_.Base_.PropertiesCallback_(this._Top_Dispatch_Pass_1));
            this._top_endpoint =
                _mycontext.DualInterface<
                    QS._qss_x_.Properties_.Interface_.IProperties_,
                    QS._qss_x_.Properties_.Interface_.IProperties_>(this._top_dispatch);
            this._top_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Top_Connect)));
                    });
            this._top_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Top_Disconnect)));
                    });

            this._bottom_dispatch = new QS._qss_x_.Properties_.Base_.Dispatch_(new QS._qss_x_.Properties_.Base_.PropertiesCallback_(this._Bottom_Dispatch_Pass_1));
            this._bottom_endpoint =
                _mycontext.DualInterface<
                    QS._qss_x_.Properties_.Interface_.IProperties_,
                    QS._qss_x_.Properties_.Interface_.IProperties_>(this._bottom_dispatch);
            this._bottom_endpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Bottom_Connect)));
                    });
            this._bottom_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Bottom_Disconnect)));
                    });
*-/
        }

        #endregion

        #region Fields

/-*
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IGroup_> _group_reference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Object_.IGroup_ _group_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IGroup_,
            QS._qss_x_.Properties_.Interface_.IGroupClient_> _group_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _group_connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IProperties_> _properties_reference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Object_.IProperties_ _properties_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_,
            QS._qss_x_.Properties_.Interface_.IProperties_> _properties_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _properties_connection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.Dispatch_ _top_dispatch;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_,
            QS._qss_x_.Properties_.Interface_.IProperties_> _top_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.Dispatch_ _bottom_dispatch;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_,
            QS._qss_x_.Properties_.Interface_.IProperties_> _bottom_endpoint;
*-/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@


        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
/-*
                this._group_object = _group_reference.Object;
                this._group_connection = this._group_endpoint.Connect(this._group_object.Group);
*-/
            }
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Dispose");
#endif

            lock (this)
            {
/-*
                if ((this._group_object != null) && (this._group_object is IDisposable))
                    ((IDisposable)this._group_object).Dispose();

                if ((this._properties_object != null) && (this._properties_object is IDisposable))
                    ((IDisposable) this._properties_object).Dispose();
*-/
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Start");
#endif

            base._Start();

            lock (this)
            {
/-*
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Start(this._platform, null);

                if ((this._properties_object != null) && (this._properties_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._properties_object).Start(this._platform, null);
*-/ 
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Stop");
#endif

            lock (this)
            {
/-*
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Stop();

                if ((this._properties_object != null) && (this._properties_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._properties_object).Stop();
*-/ 
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

/-*
        #region _Top_Dispatch_Pass_1

        private void _Top_Dispatch_Pass_1(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS._qss_x_.Properties_.Base_.Notification_>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Top_Dispatch_Pass_2),
                    new QS._qss_x_.Properties_.Base_.Notification_(_id, _version, _value)));
        }

        #endregion

        #region _Top_Dispatch_Pass_2

        private void _Top_Dispatch_Pass_2(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.Notification_ _notification =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Base_.Notification_>)_event)._Object;

            this._Top_Receive(_notification._ID, _notification._Version, _notification._Value);
        }

        #endregion

        #region _Bottom_Dispatch_Pass_1

        private void _Bottom_Dispatch_Pass_1(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS._qss_x_.Properties_.Base_.Notification_>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Bottom_Dispatch_Pass_2),
                    new QS._qss_x_.Properties_.Base_.Notification_(_id, _version, _value)));
        }

        #endregion

        #region _Bottom_Dispatch_Pass_2

        private void _Bottom_Dispatch_Pass_2(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.Notification_ _notification =
                ((QS._qss_x_.Properties_.Base_.IEvent_<QS._qss_x_.Properties_.Base_.Notification_>)_event)._Object;

            this._Bottom_Receive(_notification._ID, _notification._Version, _notification._Value);
        }

        #endregion

        #region _Group_Connect

        protected virtual void _Group_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Group_Connect");
#endif
        }

        #endregion

        #region _Group_Disconnect

        protected virtual void _Group_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Group_Disconnect");
#endif
        }

        #endregion

        #region _Top_Connect

        protected virtual void _Top_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Top_Connect");
#endif
        }

        #endregion

        #region _Top_Disconnect

        protected virtual void _Top_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Top_Disconnect");
#endif
        }

        #endregion

        #region _Top_Receive

        protected virtual void _Top_Receive(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_._Top_Receive");
#endif
        }

        #endregion

        #region _Bottom_Connect

        protected virtual void _Bottom_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Bottom_Connect");
#endif
        }

        #endregion

        #region _Bottom_Disconnect

        protected virtual void _Bottom_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Bottom_Disconnect");
#endif
        }

        #endregion

        #region _Bottom_Receive

        protected virtual void _Bottom_Receive(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_._Bottom_Receive");
#endif
        }

        #endregion
 
        #region _Top_Initialize

        protected void _Top_Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Top_Initialize");
#endif

            lock (this)
            {
                this._properties_object = this._properties_reference.Object;
                if (this._properties_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication) this._properties_object).Start(this._platform, null);
                this._properties_endpoint = this._properties_object.Properties;
                this._properties_connection = this._top_endpoint.Connect(this._properties_endpoint);
            }
        }

        #endregion

        #region _Top_Dispose

        protected void _Top_Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Coordination_._Top_Dispose");
#endif

            lock (this)
            {
                this._properties_connection.Dispose();
                this._properties_connection = null;
                this._properties_endpoint = null;
                if (this._properties_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication) this._properties_object).Stop();
                if (this._properties_object is IDisposable)
                    ((IDisposable) this._properties_object).Dispose();
                this._properties_object = null;
            }
        }

        #endregion

        #region _Top_Send

        protected void _Top_Send(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_._Top_Send");
#endif

            this._top_endpoint.Interface.Value(_id, _version, _value);
        }

        #endregion

        #region _Bottom_Send

        protected void _Bottom_Send(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.Aggregator_._Bottom_Send");
#endif

            this._bottom_endpoint.Interface.Value(_id, _version, _value);
        }

        #endregion
*-/
    }
*/
}
