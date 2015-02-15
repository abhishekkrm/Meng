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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS._qss_x_.Base1_.SynchronizationClass(QS._qss_x_.Base1_.SynchronizationOption.Reentrant | QS._qss_x_.Base1_.SynchronizationOption.Asynchronous)]    
    [QS.Fx.Base.Inspectable]
    public class Replica : QS.Fx.Inspection.Inspectable, IDisposable, IReplicaControllerContext
    {
        #region Constructor

        public Replica(
            QS.Fx.Logging.ILogger logger, 
            QS._qss_c_.Base3_.IDemultiplexer demultiplexer, 
            Persistence_.IPersistent<IReplicaPersistentState, ReplicaPersistentState, ReplicaPersistentState.Operation> persistentStateController,
            string expectedReplicaName, 
            QS._qss_x_.Base1_.Address expectedReplicaAddress, 
            string expectedMachineName,
            bool takeOverAsMaster, 
            IList<QS._qss_x_.Base1_.Address> discoveryAddressesToAdd, 
            QS.Fx.Clock.IClock clock, 
            QS.Fx.Clock.IAlarmClock alarmClock, 
            QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender> unreliableSenders,
            QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address, 
                QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> unreliableSinks)
        {
            this.logger = logger;
            this.persistentStateController = persistentStateController;
            if (expectedReplicaName == null)
                throw new Exception("Replica name is null.");
            this.expectedReplicaName = expectedReplicaName;
            this.expectedReplicaAddress = expectedReplicaAddress;
            if (expectedMachineName == null)
                throw new Exception("Machine name is null.");
            this.expectedMachineName = expectedMachineName;
            this.takeOverAsMaster = takeOverAsMaster;
            this.discoveryAddressesToAdd = discoveryAddressesToAdd;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.unreliableSenders = unreliableSenders;
            this.unreliableSinks = unreliableSinks;

/*            
            tokenAlarmCallback = new QS.Fx.QS.Fx.Clock.AlarmCallback(this.TokenAlarmCallback);
            sendCompletionCallback = new QS.CMS.Base6.CompletionCallback<object>(this.SendCompletionCallback);            
*/ 

            persistentStateController.OnReady += new QS.Fx.Base.Callback(this.PersistentStateReadyCallback);

            demultiplexer.register(
                (uint) QS.ReservedObjectID.Fx_Machine_Components_Replica, new QS._qss_c_.Base3_.ReceiveCallback(this.ReceiveCallback));
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("Replica Controller")]
        private IReplicaController replicaController;

        [QS.Fx.Base.Inspectable("Persistent State Controller")]
        private Persistence_.IPersistent<IReplicaPersistentState, ReplicaPersistentState, ReplicaPersistentState.Operation> persistentStateController;

        [QS.Fx.Base.Inspectable("Persistent State")]
        private IReplicaPersistentState persistentState;

        [QS.Fx.Base.Inspectable("Nonpersistent State")]
        private IReplicaNonpersistentState nonpersistentState;

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private bool loaded, initialized;
        private string expectedReplicaName, expectedMachineName;
        private QS._qss_x_.Base1_.Address expectedReplicaAddress;
        private uint newReplicaIncarnation;
        private bool takeOverAsMaster;
        private IList<QS._qss_x_.Base1_.Address> discoveryAddressesToAdd;
        private QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, QS._qss_c_.Base3_.ISerializableSender> unreliableSenders;
        private QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address, 
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> unreliableSinks;

        private ReplicaControllerSettings replicaControllerSettings = new ReplicaControllerSettings();

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            replicaController.Dispose();
        }

        #endregion

        #region IReplicaControllerContext Members

        QS.Fx.Logging.ILogger IReplicaControllerContext.Logger
        {
            get { return logger; }
        }

        QS.Fx.Clock.IClock IReplicaControllerContext.Clock
        {
            get { return clock; }
        }

        QS.Fx.Clock.IAlarmClock IReplicaControllerContext.AlarmClock
        {
            get { return alarmClock; }
        }

        QS._qss_x_.Persistence_.IPersistent<IReplicaPersistentState, 
            ReplicaPersistentState, ReplicaPersistentState.Operation> IReplicaControllerContext.PersistencyController
        {
            get { return persistentStateController; }
        }

        IReplicaNonpersistentState IReplicaControllerContext.NonpersistentState
        {
            get { return nonpersistentState; }
        }

        QS._qss_c_.Base3_.ISenderCollection<QS._qss_x_.Base1_.Address, 
            QS._qss_c_.Base3_.ISerializableSender> IReplicaControllerContext.UnreliableSenders
        {
            get { return unreliableSenders; }
        }

        QS._qss_c_.Base6_.ICollectionOf<QS._qss_x_.Base1_.Address,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>> IReplicaControllerContext.UnreliableSinks
        {
            get { return unreliableSinks; }
        }

        #endregion

        #region PersistentStateReadyCallback

        private void PersistentStateReadyCallback()
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(!loaded && !initialized);
                System.Diagnostics.Debug.Assert(persistentStateController.Ready);

                if (!persistentStateController.Ready)
                    throw new Exception("PersistentStateReadyCallback called even though persistent state is not loaded yet.");
                    
                persistentState = persistentStateController.State;

                loaded = true;

                List<ReplicaPersistentState.IAction> actions = new List<ReplicaPersistentState.IAction>();

                if (persistentState.ReplicaName != null)
                {
                    if (!persistentState.ReplicaName.Equals(expectedReplicaName))
                    {
                        System.Diagnostics.Debug.Assert(false, "Replica name mismatch.");

                        logger.Log(this, 
                            "Replica name mismatch, expected \"" + expectedReplicaName + "\", but stored \"" + persistentState.ReplicaName + "\".");
                    }
                }
                else
                    actions.Add(new ReplicaPersistentState.SetReplicaName(expectedReplicaName));

                if (persistentState.MachineName != null)
                {
                    if (!persistentState.MachineName.Equals(expectedMachineName))
                    {
                        System.Diagnostics.Debug.Assert(false, "Machine name mismatch.");

                        logger.Log(this,
                            "Replica name mismatch, expected \"" + expectedMachineName + "\", but stored \"" + persistentState.MachineName + "\".");
                    }
                }
                else
                    actions.Add(new ReplicaPersistentState.SetMachineName(expectedMachineName));

                newReplicaIncarnation = persistentState.ReplicaIncarnation + 1;
                actions.Add(new ReplicaPersistentState.SetReplicaIncarnation(newReplicaIncarnation));

                if (persistentState.ReplicaAddress.IsNull)
                    actions.Add(new ReplicaPersistentState.SetReplicaAddress(expectedReplicaAddress));
                else
                {
                    if (!persistentState.ReplicaAddress.Equals(expectedReplicaAddress))
                    {
                        System.Diagnostics.Debug.Assert(false, "Replica address mismatch.");

                        logger.Log(this,
                            "Replica address mismatch, expected \"" + expectedReplicaAddress.ToString() + 
                                "\", but stored \"" + persistentState.ReplicaAddress.ToString()+ "\".");
                    }
                }                

                List<QS._qss_x_.Base1_.Address> _discoveryAddressesToAdd = new List<QS._qss_x_.Base1_.Address>();
                foreach (QS._qss_x_.Base1_.Address discoveryAddress in discoveryAddressesToAdd)
                {
                    if (!persistentState.DiscoveryAddresses.Contains(discoveryAddress))
                        _discoveryAddressesToAdd.Add(discoveryAddress);
                }

                if (_discoveryAddressesToAdd.Count > 0)
                {
                    actions.Add(new ReplicaPersistentState.ModifyDiscoveryAddresses(
                        ReplicaPersistentState.ModifyDiscoveryAddresses.Modification.Add, _discoveryAddressesToAdd));
                }

                if (persistentState.MembershipView.Incarnation == 0 && takeOverAsMaster)
                {
                    logger.Log(this, 
                        "No existing view found, this replica will act as a master in a newly created singleton view of a new machine incarnation.");

                    actions.Add(new ReplicaPersistentState.SetMembershipView(new Base.MembershipView(1, 1,
                        new Base.MemberInfo[] { 
                            new QS._qss_x_._Machine_1_.Base.MemberInfo(expectedReplicaName, newReplicaIncarnation, expectedReplicaAddress, 1) }, 1, 1)));

//                    actions.Add(new ReplicaPersistentState.SetMachineIncarnation(persistentState.MachineIncarnation + 1));
                }

                persistentStateController.Submit(new ReplicaPersistentState.Operation(actions.ToArray()),
                    new QS.Fx.Base.ContextCallback<ReplicaPersistentState.Operation>(this.InitializedCallback));
            }
        }

        #endregion

        #region InitializedCallback

        private void InitializedCallback(ReplicaPersistentState.Operation operation)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(loaded && !initialized);

                logger.Log(this, "Initialized incarnation " + persistentState.ReplicaIncarnation.ToString() +
                    " of replica \"" + persistentState.ReplicaName + "\" of machine \"" + persistentState.MachineName + "\".");

                logger.Log(this, "Replica address : " + persistentState.ReplicaAddress.ToString() + ".");

                logger.Log(this, "Last known machine incarnation is " + persistentState.MembershipView.Incarnation.ToString() + ".");

                logger.Log(this, "Addresses to use for discovery: " +
                    QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<QS._qss_x_.Base1_.Address>(persistentState.DiscoveryAddresses, ", ") + ".");

                initialized = true;

                nonpersistentState = new ReplicaNonpersistentState(persistentState);

                replicaController = new ReplicaController(this, replicaControllerSettings);
                replicaController.Initialize();
            }
        }

        #endregion

        #region ReceiveCallback

        private QS.Fx.Serialization.ISerializable ReceiveCallback(QS._core_c_.Base3.InstanceID address, QS.Fx.Serialization.ISerializable message)
        {
            lock (this)
            {
                if (initialized)
                {
                    if (message is Hello)
                    {
                        Hello hello = (Hello) message;
                        lock (this)
                        {
                            if (hello.MachineName.Equals(persistentState.MachineName) && !hello.ReplicaName.Equals(persistentState.ReplicaName))
                            {
                                replicaController.Hello(hello);
                            }
                        }
                    }
                    else if (message is Append)
                    {
                        throw new NotImplementedException();

/*
                        Append append = (Append) message;
                        lock (this)
                        {
                            if (append.MachineName.Equals(persistentState.MachineName) && 
                                append.MachineIncarnation.Equals(persistentState.MachineIncarnation))
                            {
                                replicaController.Append(append);
                            }
                        }
*/ 
                    }
                    else if (message is Token)
                    {
                        throw new NotImplementedException();

/*
                        Token token = (Token) message;
                        lock (this)
                        {
                            if (token.MachineName.Equals(persistentState.MachineName) && 
                                token.MachineIncarnation.Equals(persistentState.MachineIncarnation))
                            {
                                replicaController.Token(token);
                            }
                        }
*/ 
                    }
                    else
                    {
                        // TODO: Process received messages.....................................
                    }
                }
                else
                {
                    logger.Log(this, "Not processing the received message \"" +
                        message.GetType().ToString() + "\" because the replica is not done initializing yet.");
                }
            }

            return null;
        }

        #endregion

// #######################################################################################################
// ##### THE STUFF BELOW IS ALL COMMENTED OUT
// #######################################################################################################

/*        
        private QS.Fx.Clock.AlarmCallback tokenAlarmCallback;
        private QS.CMS.Base6.CompletionCallback<object> sendCompletionCallback;
        private QS.Fx.QS.Fx.Clock.IAlarm discoveryAlarm;
        private System.Collections.Generic.ICollection<QS.Fx.Base.Address> responded =
            new System.Collections.ObjectModel.Collection<QS.Fx.Base.Address>();
        private QS.Fx.Clock.IAlarm tokenAlarm;
        private ServiceController serviceController;
        private IList<Submission> operationsToSubmit = new List<Submission>();
        private bool registeredToSend, circulatingToken;
        private uint lastUsedMessageSeqNo;
//        private Queue<Request> transmitted = new Queue<Request>();
*/

        #region _SendHello

/*
        private void _SendHello()
        {
            List<QS.Fx.Base.Address> addresses = new List<QS.Fx.Base.Address>();

            if (discoveryAddresses != null)
                addresses.AddRange(discoveryAddresses);

            if (membershipView != null && membershipView.Members != null)
            {
                foreach (Base.IMemberInfo memberInfo in membershipView.Members)
                {
                    QS.Fx.Base.Address address = memberInfo.Address;
                    if (!address.Equals(replicaAddress) && !responded.Contains(address))
                        addresses.Add(address);
                }
            }

            Hello hello = new Hello(replicaName, replicaIncarnation, replicaAddress, 
                machineName, machineIncarnation, membershipView, suspectedInView, true);

            foreach (QS.Fx.Base.Address address in addresses)
            {
                logger.Log(this, "Sending discovery message to " + address.ToString());

                ((QS.CMS.Base3.ISerializableSender) unreliableSenders[address]).send(
                    (uint)QS.ReservedObjectID.Fx_Machine_Components_Replica, hello);
            }
        }
*/

        #endregion

        #region DiscoveryAlarmCallback

/*
        private void DiscoveryAlarmCallback(QS.Fx.QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                switch (status)
                {
                    case Status.Discovery:
                        {
                            _SendHello();

                            if (status == Status.Discovery)
                                alarmRef.Reschedule();
                            else
                                alarmRef.Cancel();
                        }
                        break;

                    default:
                        {
                            logger.Log(this, "Discovery callback called, but not discovering!");
                            alarmRef.Cancel();
                        }
                        break;
                }
            }
        }
*/

        #endregion

        #region IServiceControllerContext Members

/*
        QS.Fx.QS.Fx.Clock.IClock QS.Fx.Machine.Base.IServiceControllerContext.Clock
        {
            get { return clock; }
        }

        QS.Fx.QS.Fx.Clock.IAlarmClock QS.Fx.Machine.Base.IServiceControllerContext.AlarmClock
        {
            get { return alarmClock; }
        }

        void QS.Fx.Machine.Base.IServiceControllerContext.Submit(Base.IServiceControllerOperation operation, 
            QS.Fx.Base.ContextCallback<Base.IServiceControllerOperation, object> operationCompletionCallback, object callbackContext)
        {
            switch (status)
            {
                case Status.Coordinator:
                    {
                        operationsToSubmit.Add(new Submission(operation, operationCompletionCallback, callbackContext));

                        if (!registeredToSend)
                        {
                            registeredToSend = true;
                            unreliableSinks[persistentState.DisseminationAddress].Send(
                                new QS.CMS.Base6.GetObjectsCallback<QS.CMS.Base6.IAsynchronous<QS.CMS.Base3.Message>>(
                                    this.OutgoingCallback));
                        }
                    }
                    break;

                case Status.Singleton:
                    {
                        // ......to be implemented
                        logger.Log(this, "Submission as singleton to be implemented.");
                    }
                    break;

                default:
                    throw new Exception("Not a coordinator or a singleton!");
            }
        }
*/

        #endregion

        #region NewViewCallback

/*
        private void NewViewCallback(ReplicaPersistentState.Operation operation)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(loadingNewView);

                membershipView = newMembershipView;
                loadingNewView = false;

                AdjustToNewView();

                // ...........and now we should addounce the new view and only start working until we get confirmation from everybody
            }
        }
*/

        #endregion

        #region TokenAlarmCallback

/*
        private void TokenAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (isCoordinator)
                {
                    Token token = new Token(machineName, machineIncarnation, membershipView.SeqNo, lastUsedMessageSeqNo);

                    // ........................................................................................................................

                    ((QS.CMS.Base3.ISerializableSender)unreliableSenders[membershipView.Members[1].Address]).send(
                        (uint)QS.ReservedObjectID.Fx_Machine_Components_Replica, token);
                }

                circulatingToken = false;
            }
        }
*/

        #endregion
    }
}
