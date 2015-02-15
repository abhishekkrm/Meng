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

//#define VERBOSE
#define STATISTICS
#define FATALERRORS
// #define INCARNATIONFILE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
    public abstract class GroupClient_<MessageClass>
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>
        where MessageClass : QS.Fx.Serialization.ISerializable
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public GroupClient_
        (
            QS.Fx.Object.IContext _mycontext,
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference,
            bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_.Constructor");
#endif

            if (_group_reference == null)
                _mycontext.Error("Group reference cannot be NULL.");
            this._group_reference = _group_reference;
            this._group_endpoint =
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>,
                    QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>>(this);
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
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<
            QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_reference;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable> _group_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _group_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _group_connection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Identifier _identifier;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _incarnation;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Name _name;
#if INCARNATIONFILE
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Filesystem.IFile _incarnationfile;
#endif
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _group_incarnation;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _group_members;
        [QS.Fx.Base.Inspectable]
        private bool _ismember;
        [QS.Fx.Base.Inspectable]
        private bool _issingleton;
        [QS.Fx.Base.Inspectable]
        private bool _isleader;
        [QS.Fx.Base.Inspectable]
        private int _rank;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _leader;

        #endregion

        #region Accessors

        protected QS.Fx.Base.Identifier _Identifier
        {
            get { return this._identifier; }
        }

        protected QS.Fx.Base.Incarnation _Incarnation
        {
            get { return this._incarnation; }
        }

        protected QS.Fx.Base.Name _Name
        {
            get { return this._name; }
        }

        protected QS.Fx.Base.Incarnation _Group_Incarnation
        {
            get { return this._group_incarnation; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>[] _Group_Members
        {
            get { return this._group_members; }
        }

        protected bool _IsMember
        {
            get { return this._ismember; }
        }

        protected bool _IsSingleton
        {
            get { return this._issingleton; }
        }

        protected bool _IsLeader
        {
            get { return this._isleader; }
        }

        protected QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _Leader
        {
            get { return this._leader; }
        }

        protected int _Rank
        {
            get { return this._rank; }
        }

        protected QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IGroup<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>,
            QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> _Group_Endpoint
        {
            get { return _group_endpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IGroupClient<QS.Fx.Base.Identifier,QS.Fx.Base.Incarnation,QS.Fx.Base.Name,QS.Fx.Serialization.ISerializable> Members

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>.Membership(
            QS.Fx.Value.Classes.IMembership<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership)
        {
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<
                    QS.Fx.Value.Classes.IMembership<
                        QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>>(
                            new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Reconfigure), _membership));
        }

        void QS.Fx.Interface.Classes.IGroupClient<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>.Message(
            QS.Fx.Base.Identifier _identifier, QS.Fx.Serialization.ISerializable _message)
        {
            if (!(_message is MessageClass))
                _mycontext.Error("Received a message of an unknown type.");
            this._Enqueue(
                new QS._qss_x_.Properties_.Base_.Event_<QS.Fx.Base.Identifier, MessageClass>(
                    new QS._qss_x_.Properties_.Base_.EventCallback_(this._Group_Incoming), _identifier, (MessageClass) _message));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected unsafe override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Initialize");
#endif

            base._Initialize();

            lock (this)
            {
                if (this._platform != null)
                {
                    this._name = new QS.Fx.Base.Name(_platform.Network.GetHostName());
                    byte[] _a = _platform.Network.Interfaces[0].InterfaceAddress.GetAddressBytes();
                    ulong _n = 0;
                    for (int _i = 0; _i < 4; _i++)
                    {
                        byte _b = _a[_i];
                        _n = _n << 8;
                        if (_b >= 100)
                        {
                            byte _k = (byte)(_b / 100);
                            _n |= _k;
                            _b = (byte)(_b - _k * 100);
                        }
                        _n = _n << 4;
                        if (_b >= 10)
                        {
                            byte _k = (byte)(_b / 10);
                            _n |= _k;
                            _b = (byte)(_b - _k * 10);
                        }
                        _n = _n << 4;
                        if (_b >= 1)
                        {
                            byte _k = (byte)(_b);
                            _n |= _k;
                            _b = (byte)(_b - _k * 1);
                        }
                    }
                    this._identifier = new QS.Fx.Base.Identifier(_n, (ulong)(new Random()).Next());
                }
                else
                {
                    this._name = new QS.Fx.Base.Name("unnamed");
                    this._identifier = new QS.Fx.Base.Identifier(Guid.NewGuid());
                }
#if INCARNATIONFILE
                QS.Fx.Filesystem.IFolder _rootfolder = this._mycontext.Platform.Filesystem.Root;
                if (_rootfolder.FileExists("incarnation"))
                {
                    this._incarnationfile = _rootfolder.OpenFile(
                        "incarnation", System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None, QS.Fx.Filesystem.FileFlags.None);
                    byte[] _data = new byte[sizeof(uint)];
                    this._incarnationfile.BeginRead(0, new ArraySegment<byte>(_data), new AsyncCallback(this._ReadCallback), _data);
                }
                else
                {
#if VERBOSE
                    if (this._logger != null)
                        this._logger.Log("Component_.GroupClient_._Initialize : INCARNATION(1)");
#endif
                    this._incarnationfile = _rootfolder.OpenFile(
                        "incarnation", System.IO.FileMode.CreateNew, System.IO.FileAccess.Write, System.IO.FileShare.None, QS.Fx.Filesystem.FileFlags.None);
                    this._incarnation = new QS.Fx.Base.Incarnation(1U);
                    byte[] _data = new byte[sizeof(uint)];
                    fixed (byte *_pdata = _data)
                    {
                        *((uint *) _pdata) = 1;
                    }
                    this._incarnationfile.BeginWrite(0, new ArraySegment<byte>(_data), new AsyncCallback(this._WriteCallback), _data);
                }
#else
                this._incarnation = new QS.Fx.Base.Incarnation(1U);
                this._group_object = _group_reference.Dereference(_mycontext);
                if (this._group_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Start(this._platform, null);
                this._group_connection = this._group_endpoint.Connect(this._group_object.Group);
#endif
            }
        }

        #endregion

        #region _ReadCallback

#if INCARNATIONFILE
        private unsafe void _ReadCallback(IAsyncResult _result)
        {
            lock (this)
            {
                if (!_result.IsCompleted)
                    throw new Exception("Could not read from the incarnation file.");
                byte[] _data = (byte[]) _result.AsyncState;
                uint _incarnation;
                fixed (byte* _pdata = _data)
                {
                    _incarnation = (*((uint*)_pdata)) + 1;
                    *((uint*)_pdata) = _incarnation;
                }
                this._incarnation = new QS.Fx.Base.Incarnation(_incarnation);
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.GroupClient_._ReadCallback : INCARNATION(" + _incarnation.ToString() + ")");
#endif
                this._incarnationfile.BeginWrite(0, new ArraySegment<byte>(_data), new AsyncCallback(this._WriteCallback), _data);
            }
        }
#endif
        #endregion

        #region _WriteCallback

#if INCARNATIONFILE
        private unsafe void _WriteCallback(IAsyncResult _result)
        {
            lock (this)
            {
                if (!_result.IsCompleted)
                    throw new Exception("Could not write to the incarnation file.");
                this._incarnationfile.Dispose();
#if VERBOSE
                if (this._logger != null)
                    this._logger.Log("Component_.GroupClient_._WriteCallback : CONNECT");
#endif
                this._group_object = _group_reference.Dereference(_mycontext);
                if (this._group_object is QS._qss_x_.Platform_.IApplication)
                    ((QS._qss_x_.Platform_.IApplication)this._group_object).Start(this._platform, null);
                this._group_connection = this._group_endpoint.Connect(this._group_object.Group);
            }
        }
#endif

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Dispose");
#endif

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is IDisposable))
                    ((IDisposable) this._group_object).Dispose();
            }

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Start");
#endif

            base._Start();

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._group_object).Start(this._platform, null);
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Stop");
#endif

            lock (this)
            {
                if ((this._group_object != null) && (this._group_object is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication) this._group_object).Stop();
            }

            base._Stop();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Group_Connect

        private void _Group_Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Group_Connect");
#endif

            lock (this)
            {
                if (this._group_endpoint.IsConnected)
                    this._group_endpoint.Interface.Register(this._identifier, this._incarnation, this._name);
            }
        }

        #endregion

        #region _Group_Disconnect

        private void _Group_Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Group_Disconnect");
#endif
        }

        #endregion

        #region _Group_Incoming

        private void _Group_Incoming(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, MessageClass> _event_ =
                (QS._qss_x_.Properties_.Base_.IEvent_<QS.Fx.Base.Identifier, MessageClass>)_event;
            QS.Fx.Base.Identifier _identifier = _event_._Object1;
            MessageClass _message = _event_._Object2;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Group_Incoming ( " +
                    QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
#endif

            this._Group_Incoming(_identifier, _message);
        }

        protected abstract void _Group_Incoming(QS.Fx.Base.Identifier _identifier, MessageClass _message);

        #endregion

        #region _Group_Outgoing

        protected void _Group_Outgoing(QS.Fx.Base.Identifier _identifier, MessageClass _message)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Group_Outgoing ( " +
                    QS.Fx.Printing.Printable.ToString(_identifier) + " )\n\n" + QS.Fx.Printing.Printable.ToString(_message));
#endif

            if (this._group_endpoint.IsConnected)
                this._group_endpoint.Interface.Message(_identifier, _message);
        }

        #endregion

        #region _Group_Reconfigure

        private void _Group_Reconfigure(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
            QS.Fx.Value.Classes.IMembership<
                QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>> _membership =
                    ((QS._qss_x_.Properties_.Base_.IEvent_<
                        QS.Fx.Value.Classes.IMembership<
                            QS.Fx.Base.Incarnation, QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>>)_event)._Object;

#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_.GroupClient_._Group_Reconfigure\n\n" + QS.Fx.Printing.Printable.ToString(_membership) + "\n\n");
#endif

            lock (this)
            {
                this._group_incarnation = _membership.Incarnation;
                this._group_members = 
                    new List<QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name>>(_membership.Members).ToArray();
                this._ismember = false;
                this._rank = -1;
                for (int _ind = 0; _ind < this._group_members.Length; _ind++)
                {
                    QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _member = this._group_members[_ind];
                    if (_member.Identifier.Equals(this._identifier) && _member.Incarnation.Equals(this._incarnation))
                    {
                        this._ismember = true;
                        this._rank = _ind;
                    }
                }
                this._isleader = (this._rank == 0);
                this._issingleton = this._isleader && (this._group_members.Length == 1);
                if (this._group_members.Length > 0)
                    this._leader = this._group_members[0];
                else
                    this._leader = null;
#if VERBOSE
                if (this._ismember)
                {
                    if (this._logger != null)
                        this._logger.Log(
                            "Component_.GroupClient_._Reconfigure (enter)\n" +
                            "\n_rank = " + _rank.ToString() +
                            "\n_issingleton = " + _issingleton.ToString() +
                            "\n_isleader = " + _isleader.ToString() +
                            "\n_leader = " + QS.Fx.Printing.Printable.ToString(_leader) + "\n\n");
                }
                else
                {
                    if (this._logger != null)
                        this._logger.Log("Component_.GroupClient_._Reconfigure (leave)");
                }
#endif

                this._Group_Reconfigure();
            }
        }

        protected abstract void _Group_Reconfigure();

        #endregion
    }
}
