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

// #define DEBUG_EnableLoggingForReceiveCallback
// #define DEBUG_EnableLoggingForConnectionOpeningAndClosing

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Node
{    
    public sealed class Node : QS.Fx.Inspection.Inspectable, INode, IConnectionControlContext, IControllerContext, IDisposable
    {
        #region Constructor

        public Node(string scopename, QS.Fx.Base.ID scopeid, QS.Fx.Platform.IPlatform platform, QS._qss_c_.Base1_.Subnet subnet, int portno)
        {
#if DEBUG_EnableLogging
            platform.Logger.Log("Initializing node(name = \"" + scopename + "\", id = " + scopeid.ToString() + ", port = " + portno.ToString() + ")");
#endif

            this.scopename = scopename;
            this.scopeid = scopeid;
            this.platform = platform;

            protocolstack = new ProtocolStack(platform, subnet, portno);
            protocolstack.Demultiplexer.register(
                (uint) ReservedObjectID.Fx_Backbone_Node, new QS._qss_c_.Base3_.ReceiveCallback(this._ReceiveCallback));

            _connections_inspectable = 
                new QS._qss_e_.Inspection_.DictionaryWrapper1<string, IConnectionControl>("_connections", connections,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, IConnectionControl>.ConversionCallback(delegate(string s) { return s; }));            
        }

        #endregion

        #region Accessors
        
        public IController Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
#if DEBUG_EnableLogging
            platform.Logger.Log("Disposing node(name = \"" + scopename + "\", id = " + scopeid.ToString() + ")");
#endif

            ((IDisposable)controller).Dispose();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable] private string scopename;
        [QS.Fx.Base.Inspectable] private QS.Fx.Base.ID scopeid;
        [QS.Fx.Base.Inspectable] private QS.Fx.Platform.IPlatform platform;
        [QS.Fx.Base.Inspectable] private IProtocolStack protocolstack;        
        [QS.Fx.Base.Inspectable] private IController controller;

        private IDictionary<string, IConnectionControl> connections = new Dictionary<string, IConnectionControl>();
        private IDictionary<QS.Fx.Base.ID, IConnectionControl> connections_byid = new Dictionary<QS.Fx.Base.ID, IConnectionControl>();
        private double timeout = 5;

        [QS.Fx.Base.Inspectable("_connections")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, IConnectionControl> _connections_inspectable;

        #endregion       
    
        #region INode Members

        IAsyncResult INode.BeginConnect(string name, Base1_.Address address, AsyncCallback callback, object cookie)
        {
            lock (this)
            {
                IConnectionControl connection;
                if (connections.TryGetValue(name, out connection))
                {
                    if (!connection.Address.Equals(address))
                        throw new Exception(
                            "Address mismatch: existing is " + connection.Address.ToString() + ", requesting " + address.ToString());
                }
                else
                    connection = _NewConnection(true, name, address);

                bool connect;
                Connections_2_.Request request = Connections_2_.Request.Create(connection, callback, cookie, out connect);

                if (connect)
                    _StartConnection(connection);

                return request;
            }            
        }

        private void _ReleaseConnectionCallback(Connections_2_.IConnectionControl _connection)
        {
            lock (this)
            {
                IConnectionControl connection = _connection as IConnectionControl;
                if (connection == null)
                    throw new Exception("Type mismatch.");

                connection.ReferenceCount--;
                if (connection.IsOutgoing && connection.ReferenceCount == 0)
                    _StopConnection(connection);
            }
        }

        IConnection INode.EndConnect(IAsyncResult result)
        {
            return Connections_2_.Request.GetConnection<Connection>(result);
        }

        void INode.Connect(string name, Base1_.Address address)
        {
            ((INode)this).BeginConnect(name, address, null, null);
        }

        IEnumerable<IConnection> INode.Connections
        {
            get
            {
                lock (this)
                {
                    IConnection[] _connections = new IConnection[connections.Count];
                    int index = 0;
                    foreach (IConnectionControl connection in connections.Values)
                        _connections[index++] = connection;
                    return _connections;
                }
            }
        }

        #endregion

        #region IControllerContext Members

        string IControllerContext.Name
        {
            get { return scopename; }
        }

        QS.Fx.Base.ID IControllerContext.ID
        {
            get { return scopeid; }
        }

        QS.Fx.Logging.ILogger IControllerContext.Logger
        {
            get { return platform.Logger; }
        }

        QS.Fx.Clock.IClock IControllerContext.Clock
        {
            get { return platform.Clock; }
        }

        QS.Fx.Clock.IAlarmClock IControllerContext.AlarmClock
        {
            get { return platform.AlarmClock; }
        }

        QS.Fx.Scheduling.IScheduler IControllerContext.Scheduler
        {
            get { return platform.Scheduler; }
        }

        #endregion

        #region _NewConnection

        private IConnectionControl _NewConnection(bool isoutgoing, string name, Base1_.Address address)
        {
#if DEBUG_EnableLogging
            platform.Logger.Log("_NewConnection(\"" + name + "\", " + address.ToString() + ")");
#endif

            IConnectionControl connection = new Connection(isoutgoing, name, address, 
                protocolstack.UnreliableSenders[address], protocolstack.UnreliableSinks[address], this);
            connection.ReleaseCallback =
                new QS.Fx.Base.ContextCallback<Connections_2_.IConnectionControl>(this._ReleaseConnectionCallback);
            connections.Add(name, connection);
            return connection;
        }

        #endregion

        #region _NewEndpointNo

        private ulong _NewEndpointNo()
        {
            return 1; // (ulong)(DateTime.Now.Ticks);
        }

        #endregion

        #region _StartConnection

        private void _StartConnection(IConnectionControl connection)
        {
#if DEBUG_EnableLogging
            platform.Logger.Log("_StartConnection(\"" + connection.Name + "\")");
#endif

            switch (connection.ConnectionStatus)
            {
                case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnected:
                case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnecting:
                    {
                        connection.ConnectionStatus = QS._qss_x_.Connections_2_.ConnectionStatus.Connecting;                        
                        connection.ID = QS.Fx.Base.ID.Undefined;
                        connection.Endpoint1 = Math.Max(_NewEndpointNo(), connection.Endpoint1 + 1UL);
                        connection.Endpoint2 = 0UL;
                        connection.Phase = Phase.Negotiating;

                        connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node, 
                            new Open(scopename, scopeid, connection.Endpoint1, 
                                connection.Name, connection.ID, connection.Endpoint2, connection.Phase));

                        connection.Alarm = platform.AlarmClock.Schedule(
                            timeout, new QS.Fx.Clock.AlarmCallback(this._ConnectionAlarmCallback), connection);
                    }
                    break;

                case QS._qss_x_.Connections_2_.ConnectionStatus.Connected:
                case QS._qss_x_.Connections_2_.ConnectionStatus.Connecting:
                case QS._qss_x_.Connections_2_.ConnectionStatus.Reconnecting:
                    break;
            }
        }

        #endregion

        #region _StopConnection

        private void _StopConnection(IConnectionControl connection)
        {
#if DEBUG_EnableLogging
            platform.Logger.Log("_StopConnection(\"" + connection.Name + "\")");
#endif

            // TODO: Implement this...
        }

        #endregion

        #region _ConnectionAlarmCallback

        private void _ConnectionAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            IConnectionControl connection = (IConnectionControl) alarm.Context;
            lock (this)
            {
                switch (connection.ConnectionStatus)
                {
                    case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnected:
                    case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnecting:
                        break;

                    case QS._qss_x_.Connections_2_.ConnectionStatus.Connected:
                        {
                            // TODO: Should send a more complete info............
                            connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                new Sync(scopeid, connection.Endpoint1, connection.ID, connection.Endpoint2));
                            alarm.Reschedule();
                        }
                        break;

                    case QS._qss_x_.Connections_2_.ConnectionStatus.Connecting:
                    case QS._qss_x_.Connections_2_.ConnectionStatus.Reconnecting:
                        {
                            connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                new Open(scopename, scopeid, connection.Endpoint1,
                                    connection.Name, connection.ID, connection.Endpoint2, connection.Phase));
                            alarm.Reschedule();
                        }
                        break;
                }
            }
        }

        #endregion

        #region _ReceiveCallback

        private QS.Fx.Serialization.ISerializable _ReceiveCallback(QS._core_c_.Base3.InstanceID sourceaddress, QS.Fx.Serialization.ISerializable receivedobject)
        {
#if DEBUG_EnableLoggingForReceiveCallback
            platform.Logger.Log("_ReceiveCallback : " + QS.Fx.Printing.Printable.ToString(receivedobject));
#endif

            switch (receivedobject.SerializableInfo.ClassID)
            {
                #region Open

                case ((ushort) ClassID.Fx_Backbone_Node_Open):
                    {
                        Open open = (Open) receivedobject;
                        lock (this)
                        {
                            IConnectionControl connection;
                            if (connections.TryGetValue(open.Name1, out connection))
                            {
                                #region Connection exists

                                if (open.ID2.Equals(QS.Fx.Base.ID.Undefined))
                                {
                                    if (connection.ID.Equals(QS.Fx.Base.ID.Undefined) || open.ID1.Equals(connection.ID))
                                    {
                                        connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                            new Open(scopename, scopeid, connection.Endpoint1,
                                                connection.Name, connection.ID, connection.Endpoint2, connection.Phase));
                                    }
                                    else
                                    {
                                        // TODO: We should handle somehow the case where the source identity mismatches on the initial opening request
                                        System.Diagnostics.Debug.Assert(false,
                                            "Our name is \"" + scopename + "\" and id is " + scopeid.ToString() + ", and we have a connection to \"" +
                                            connection.Name + " with id " + connection.ID.ToString() + 
                                            ", but we received an open request with a wrong source identity:\n" + QS.Fx.Printing.Printable.ToString(open));
                                    }
                                }
                                else
                                {
                                    if (open.Name2.Equals(scopename) && open.ID2.Equals(scopeid))
                                    {
                                        switch (connection.ConnectionStatus)
                                        {
                                            #region Continue negotiation

                                            case QS._qss_x_.Connections_2_.ConnectionStatus.Connected:
                                            case QS._qss_x_.Connections_2_.ConnectionStatus.Connecting:
                                            case QS._qss_x_.Connections_2_.ConnectionStatus.Reconnecting:
                                                {
                                                    if (connection.ID.Equals(QS.Fx.Base.ID.Undefined))
                                                    {
                                                        #region Our side of the connection considers peer identity still undefined, so we define it now and continue

                                                        System.Diagnostics.Debug.Assert(
                                                            connection.ConnectionStatus == Connections_2_.ConnectionStatus.Connecting ||
                                                            connection.ConnectionStatus == Connections_2_.ConnectionStatus.Reconnecting);

                                                        System.Diagnostics.Debug.Assert(connection.Phase == Phase.Negotiating);

                                                        System.Diagnostics.Debug.Assert(connection.Endpoint2 == 0UL);
                                                        System.Diagnostics.Debug.Assert(open.Endpoint2 <= connection.Endpoint1);

                                                        connection.ID = open.ID1;
                                                        connections_byid.Add(connection.ID, connection);

                                                        connection.Endpoint2 = Math.Max(connection.Endpoint2, open.Endpoint1);

                                                        connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                                            new Open(scopename, scopeid, connection.Endpoint1,
                                                                connection.Name, connection.ID, connection.Endpoint2, connection.Phase));

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        #region Our side of the connection has a well defined peer identity

                                                        if (open.ID1.Equals(connection.ID))
                                                        {
                                                            System.Diagnostics.Debug.Assert(open.Endpoint2 <= connection.Endpoint1);

                                                            if (open.Endpoint1 >= connection.Endpoint2 && open.Endpoint2 == connection.Endpoint1)
                                                            {
                                                                if (open.Endpoint1 > connection.Endpoint2)
                                                                {
                                                                    #region Newer endpoint numbers, need to restart connection

                                                                    if (connection.ConnectionStatus == QS._qss_x_.Connections_2_.ConnectionStatus.Connected)
                                                                    {
                                                                        _CloseConnection(connection);
                                                                        connection.Alarm = platform.AlarmClock.Schedule(
                                                                            timeout, new QS.Fx.Clock.AlarmCallback(this._ConnectionAlarmCallback), connection);
                                                                    }

                                                                    connection.ConnectionStatus = QS._qss_x_.Connections_2_.ConnectionStatus.Connecting;
                                                                    connection.Endpoint2 = open.Endpoint1;
                                                                    connection.Phase = Phase.Negotiating;

                                                                    connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                                                        new Open(scopename, scopeid, connection.Endpoint1,
                                                                            connection.Name, connection.ID, connection.Endpoint2, connection.Phase));

                                                                    #endregion
                                                                }
                                                                else
                                                                {
                                                                    #region Endpoints match, we can consider the connection open

                                                                    if (connection.ConnectionStatus != QS._qss_x_.Connections_2_.ConnectionStatus.Connected)
                                                                    {
                                                                        connection.ConnectionStatus = QS._qss_x_.Connections_2_.ConnectionStatus.Connected;
                                                                        connection.Phase = Phase.Open;
                                                                        _OpenConnection(connection, false);
                                                                    }
                                                                    else
                                                                        System.Diagnostics.Debug.Assert(connection.Phase != Phase.Negotiating);

                                                                    if (open.Phase != Phase.Negotiating && connection.Phase != Phase.Acknowledged)
                                                                    {
                                                                        connection.Phase = Phase.Acknowledged;
                                                                        _OpenConnection(connection, true);
                                                                    }

                                                                    if (open.Phase != Phase.Acknowledged)
                                                                    {
                                                                        connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                                                            new Open(scopename, scopeid, connection.Endpoint1,
                                                                                connection.Name, connection.ID, connection.Endpoint2, connection.Phase));
                                                                    }

                                                                    #endregion
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            // TODO: We should handle somehow the case where the existing peer's id does not match our own records.
                                                            System.Diagnostics.Debug.Assert(false, "Not Implemented");
                                                        }

                                                        #endregion
                                                    }
                                                }
                                                break;

                                            #endregion

                                            #region Revive connection

                                            case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnected:
                                            case QS._qss_x_.Connections_2_.ConnectionStatus.Disconnecting:
                                                {
                                                    connection.ConnectionStatus = QS._qss_x_.Connections_2_.ConnectionStatus.Connecting;
                                                    connection.ID = open.ID1;
                                                    connection.Endpoint1 = Math.Max(connection.Endpoint1 + 1, _NewEndpointNo());
                                                    connection.Endpoint2 = open.Endpoint1;
                                                    connection.Phase = Phase.Negotiating;

                                                    connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                                        new Open(scopename, scopeid, connection.Endpoint1,
                                                            connection.Name, connection.ID, connection.Endpoint2, connection.Phase));

                                                    connection.Alarm = platform.AlarmClock.Schedule(
                                                        timeout, new QS.Fx.Clock.AlarmCallback(this._ConnectionAlarmCallback), connection);
                                                }
                                                break;

                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        // TODO: We should handle somehow the case where the new peer's belief about our name or id are wrong.
                                        System.Diagnostics.Debug.Assert(false,
                                            "Our name is \"" + scopename + "\" and id is " + scopeid.ToString() +
                                            ", but we received an open request with a wrong destination identity:\n" + QS.Fx.Printing.Printable.ToString(open));
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                #region A new connection

                                connection = _NewConnection(false, open.Name1, QS._qss_x_.Base1_.Address.QuickSilver(sourceaddress.Address));

                                connection.ConnectionStatus = QS._qss_x_.Connections_2_.ConnectionStatus.Connecting;
                                connection.ID = open.ID1;
                                connection.Endpoint1 = _NewEndpointNo();
                                connection.Endpoint2 = open.Endpoint1;
                                connection.Phase = Phase.Negotiating;

                                connections_byid.Add(connection.ID, connection);

                                connection.UnreliableSender.send((uint)ReservedObjectID.Fx_Backbone_Node,
                                    new Open(scopename, scopeid, connection.Endpoint1,
                                        connection.Name, connection.ID, connection.Endpoint2, connection.Phase));

                                connection.Alarm = platform.AlarmClock.Schedule(
                                    timeout, new QS.Fx.Clock.AlarmCallback(this._ConnectionAlarmCallback), connection);

                                #endregion
                            }
                    }
                    break;
                }

                #endregion

                #region Sync

                case ((ushort) ClassID.Fx_Backbone_Node_Sync):
                    {
                        Sync sync = (Sync) receivedobject;

                    }
                    break;

                #endregion

                #region Close

                case ((ushort) ClassID.Fx_Backbone_Node_Close):
                    {
                        Close close = (Close) receivedobject;

                    }
                    break;

                #endregion

                #region Message

                case ((ushort)ClassID.Fx_Backbone_Node_Message):
                    {
                        Incoming message = (Incoming) receivedobject;
                        if (message.ID2.Equals(scopeid))
                        {
                            IConnectionControl connection = null;
                            lock (this)
                            {
                                if (!connections_byid.TryGetValue(message.ID1, out connection))
                                    connection = null;
                            }

                            if (connection != null)
                                connection.Receive(message);
                        }
                    }
                    break;

                #endregion

                #region Acknowledgement

                case ((ushort)ClassID.Fx_Backbone_Node_Acknowledgement):
                    {
                        Acknowledgement acknowledgement = (Acknowledgement) receivedobject;
                        if (acknowledgement.ID2.Equals(scopeid))
                        {
                            IConnectionControl connection = null;
                            lock (this)
                            {
                                if (!connections_byid.TryGetValue(acknowledgement.ID1, out connection))
                                    connection = null;
                            }

                            if (connection != null)
                                connection.Receive(acknowledgement);
                        }
                    }
                    break;

                #endregion

                default:
                    throw new Exception("Received an object of unknown type.");
            }

            return null;
        }

        #endregion

        #region _OpenConnection

        private void _OpenConnection(IConnectionControl connection, bool acknowledged)
        {
#if DEBUG_EnableLoggingForConnectionOpeningAndClosing
            platform.Logger.Log("_OpenConnection(\"" + connection.Name + "\", " + connection.ID.ToString() +
                ", isoutgoing = " + connection.IsOutgoing.ToString() + ", acknowledged = " + acknowledged.ToString() + ")");
#endif

            if (acknowledged)
            {
                connection.ControllerConnection.ActivateChannel();
            }
            else
            {
                connection.Initialize();
                connection.ControllerConnection = controller.Create(connection.ControllerConnectionContext);
            }
        }

        #endregion

        #region _CloseConnection

        private void _CloseConnection(IConnectionControl connection)
        {
#if DEBUG_EnableLoggingForConnectionOpeningAndClosing
            platform.Logger.Log("_CloseConnection(\"" + connection.Name + "\", " + connection.ID.ToString() + ")");
#endif

            connection.ControllerConnection.Dispose();
            connection.ControllerConnection = null;
        }

        #endregion

        #region IConnectionControlContext Members

        QS.Fx.Base.ID IConnectionControlContext.ID
        {
            get { return scopeid; }
        }

        QS.Fx.Logging.ILogger IConnectionControlContext.Logger
        {
            get { return platform.Logger; }
        }

        QS.Fx.Clock.IClock IConnectionControlContext.Clock
        {
            get { return platform.Clock; }
        }

        QS.Fx.Clock.IAlarmClock IConnectionControlContext.AlarmClock
        {
            get { return platform.AlarmClock; }
        }

        #endregion
    }
}
