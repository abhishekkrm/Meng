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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Base.Inspectable]
    public class ReplicaController : QS.Fx.Inspection.Inspectable, IReplicaController, ServiceControl.IServiceControllerContext
    {
        #region Constructor

        public ReplicaController(IReplicaControllerContext context, IReplicaControllerSettings settings)
        {
            this.context = context;
            this.settings = settings;

            persistentState = context.PersistencyController.State;
            nonpersistentState = context.NonpersistentState;
        }

        #endregion

        #region Fields

        private IReplicaControllerContext context;
        private IReplicaControllerSettings settings;

        [QS.Fx.Printing.Printable("Persistent State")]
        [QS.Fx.Base.Inspectable]
        private IReplicaPersistentState persistentState;

        [QS.Fx.Printing.Printable("Nonpersistent State")]
        [QS.Fx.Base.Inspectable]
        private IReplicaNonpersistentState nonpersistentState;

        [QS.Fx.Printing.Printable("Service Controller")]
        [QS.Fx.Base.Inspectable]
        private ServiceControl.IServiceController serviceController;

        [QS.Fx.Base.Inspectable]
        private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> disseminationSink;

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region IServiceControllerContext Members

        QS.Fx.Logging.ILogger QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerContext.Logger
        {
            get { return context.Logger; }
        }

        QS.Fx.Clock.IClock QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerContext.Clock
        {
            get { return context.Clock; }
        }

        QS.Fx.Clock.IAlarmClock QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerContext.AlarmClock
        {
            get { return context.AlarmClock; }
        }

        void QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerContext.Submit(
            QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerOperation operation,
            QS.Fx.Base.ContextCallback<QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerOperation, object> operationCompletionCallback,
            object callbackContext)
        {
            ((IReplicaController)this).Submit(operation, operationCompletionCallback, callbackContext);
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Helper functions related to the discovery process
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region _DiscoveryAlarmCallback

        private void _DiscoveryAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (nonpersistentState.Status == ReplicaStatus.Standby && !nonpersistentState.DiscoveredReadQuorum)
                {
                    double toWait = nonpersistentState.DiscoveryTimestamp + settings.DiscoveryTimeout - context.Clock.Time;
                    if (toWait > 0.001)
                        alarm.Reschedule(toWait);
                    else
                    {
                        nonpersistentState.DiscoveryAlarm = null;
                        _DiscoveryFailed();
                    }
                }
                else
                    nonpersistentState.DiscoveryAlarm = null;
            }
        }

        #endregion

        #region _DiscoveryResendAlarmCallback

        private void _DiscoveryResendAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (nonpersistentState.Status == ReplicaStatus.Standby && !nonpersistentState.DiscoveredReadQuorum)
                {
                    bool directly = nonpersistentState.HasView && (persistentState.DiscoveryAddresses.Count == 0 ||
                        (context.Clock.Time - nonpersistentState.DiscoveryTimestamp > settings.DiscoveryBroadcastTimeout));

                    _BroadcastHello(directly);

                    alarm.Reschedule(1 / settings.DiscoveryRate);
                }
                else
                    nonpersistentState.DiscoveryResendAlarm = null;
            }
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _DiscoveryFailed

        private void _DiscoveryFailed()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________DiscoveryFailed");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        #region _BroadcastHello

        private void _BroadcastHello(bool directly)
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________BroadcastDiscovery");
#endif

            Hello hello = new Hello(persistentState.ReplicaName, persistentState.ReplicaIncarnation, persistentState.ReplicaAddress,
                persistentState.MachineName, persistentState.MembershipView, true);
            
            List<QS._qss_x_.Base1_.Address> addresses = new List<QS._qss_x_.Base1_.Address>();

            if (directly)
            {
                Base.IMembershipView view = nonpersistentState.CurrentView;                
                System.Diagnostics.Debug.Assert(view !=null && view.Members != null);

                for (int index = 0; index < view.Members.Length; index++)
                {
                    Base.IMemberInfo memberInfo = view.Members[index];
                    if (!memberInfo.Name.Equals(persistentState.ReplicaName))
                    {
                        IPeerInfo peerInfo = nonpersistentState.PeerInfo(index);                        
                        if (!peerInfo.Discovered || peerInfo.CurrentViewNumber < nonpersistentState.CurrentView.SeqNo)
                            addresses.Add(memberInfo.Address);
                    }                    
                }
            }
            else
            {
                addresses.AddRange(persistentState.DiscoveryAddresses);
            }

            System.Diagnostics.Debug.Assert(addresses.Count > 0);

            foreach (QS._qss_x_.Base1_.Address address in addresses)
            {
                ((QS._qss_c_.Base3_.ISerializableSender)context.UnreliableSenders[address]).send(
                    (uint)QS.ReservedObjectID.Fx_Machine_Components_Replica, hello);
            }
        }

        #endregion

        #region _StartDiscovery

        private void _StartDiscovery()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________StartDiscovery");
#endif

            nonpersistentState.DiscoveryTimestamp = context.Clock.Time;

            if (nonpersistentState.DiscoveryAlarm != null)
                nonpersistentState.DiscoveryAlarm.Cancel();

            nonpersistentState.DiscoveryAlarm = context.AlarmClock.Schedule(
                settings.DiscoveryTimeout, new QS.Fx.Clock.AlarmCallback(_DiscoveryAlarmCallback), null);

            _BroadcastHello(nonpersistentState.HasView && (persistentState.DiscoveryAddresses.Count == 0));

            if (nonpersistentState.DiscoveryResendAlarm != null)
                nonpersistentState.DiscoveryResendAlarm.Cancel();

            nonpersistentState.DiscoveryResendAlarm = context.AlarmClock.Schedule(
                1 / settings.DiscoveryRate, new QS.Fx.Clock.AlarmCallback(_DiscoveryResendAlarmCallback), null);

            nonpersistentState.RunningDiscovery = true;
        }

        #endregion

        #region _StopDiscovery

        private void _StopDiscovery()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________StopDiscovery");
#endif

            if (nonpersistentState.DiscoveryAlarm != null)
                nonpersistentState.DiscoveryAlarm.Cancel();

            nonpersistentState.DiscoveryAlarm = null;

            if (nonpersistentState.DiscoveryResendAlarm != null)
                nonpersistentState.DiscoveryResendAlarm.Cancel();

            nonpersistentState.DiscoveryResendAlarm = null;

            nonpersistentState.RunningDiscovery = false;
        }

        #endregion

        #region _SendHello

        private void _SendHello(QS._qss_x_.Base1_.Address address, bool request_response)
        {
            ((QS._qss_c_.Base3_.ISerializableSender) context.UnreliableSenders[address]).send(
                (uint) QS.ReservedObjectID.Fx_Machine_Components_Replica, 
                new Hello(persistentState.ReplicaName, persistentState.ReplicaIncarnation, persistentState.ReplicaAddress,
                    persistentState.MachineName, nonpersistentState.CurrentView, request_response));
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Helper functions related to synchronizing
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region [ **** UNFINISHED **** ] _StartSynchronizing

        private void _StartSynchronizing()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________StartSynchronizing");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // .........................

            nonpersistentState.Synchronizing = true;
        }

        #endregion

        #region [ **** UNFINISHED **** ] _StopSynchronizing

        private void _StopSynchronizing()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________StopSynchronizing");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // .........................

            nonpersistentState.Synchronizing = false;
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Helper functions related to state changes
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region _SwitchTo

        private void _SwitchTo(ReplicaStatus newstatus)
        {
            switch (nonpersistentState.Status)
            {
                case ReplicaStatus.Blocked:
                    _LeaveBlocked();
                    break;

                case ReplicaStatus.Standby:
                    _LeaveStandby();
                    break;

                case ReplicaStatus.Cohort:
                    _LeaveCohort();
                    break;

                case ReplicaStatus.Candidate:
                    _LeaveCandidate();
                    break;

                case ReplicaStatus.Coordinator:
                    _LeaveCoordinator();
                    break;
            }

            nonpersistentState.Status = newstatus;

            switch (nonpersistentState.Status)
            {
                case ReplicaStatus.Blocked:
                    _EnterBlocked();
                    break;

                case ReplicaStatus.Standby:
                    _EnterStandby();
                    break;

                case ReplicaStatus.Cohort:
                    _EnterCohort();
                    break;

                case ReplicaStatus.Candidate:
                    _EnterCandidate();
                    break;

                case ReplicaStatus.Coordinator:
                    _EnterCoordinator();
                    break;
            }
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _EnterBlocked

        private void _EnterBlocked()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________EnterBlocked");
#endif

            // ...............................................................................................................................................................................            
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _LeaveBlocked

        private void _LeaveBlocked()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________LeaveBlocked");
#endif

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ **** UNFINISHED **** ] _EnterStandby

        private void _EnterStandby()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________EnterStandby");
#endif

            if (nonpersistentState.IsInView)
            {
                if (nonpersistentState.IsCoordinator)
                {
                    _SwitchTo(ReplicaStatus.Coordinator);
                }
                else
                {
                    _SwitchTo(ReplicaStatus.Cohort);
                }
            }
            else
            {
                if (nonpersistentState.DiscoveredReadQuorum)
                {
                    if (!nonpersistentState.Synchronized && !nonpersistentState.Synchronizing)
                        _StartSynchronizing();

                    // ...............................................................................................................................................................................
                }
                else
                {
                    if (!nonpersistentState.RunningDiscovery)
                        _StartDiscovery();
                }
            }

            // TODO: We should start the candidate timer........
        }

        #endregion

        #region [ **** UNFINISHED **** ] _LeaveStandby

        private void _LeaveStandby()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________LeaveStandby");
#endif

            if (nonpersistentState.RunningDiscovery)
                _StopDiscovery();

            if (nonpersistentState.Synchronizing)
                _StopSynchronizing();

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _EnterCohort

        /// <summary>
        /// Invoked when transitioning into the cohort phase.
        /// </summary>
        private void _EnterCohort()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________EnterCohort");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _LeaveCohort

        /// <summary>
        /// Invoked when transitioning out of the cohort phase.
        /// </summary>
        private void _LeaveCohort()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________LeaveCohort");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _EnterCandidate

        /// <summary>
        /// Invoked when transitioning into the candidate phase.
        /// </summary>
        private void _EnterCandidate()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________EnterCandidate");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _LeaveCandidate

        /// <summary>
        /// Invoked when transitioning out of the candidate phase.
        /// </summary>
        private void _LeaveCandidate()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________LeaveCandidate");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        #region [ **** UNFINISHED **** ] _EnterCoordinator

        /// <summary>
        /// Invoked when transitioning into the coordinator phase.
        /// </summary>
        private void _EnterCoordinator()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________EnterCoordinator");
#endif

            serviceController = new ServiceController(this);
            serviceController.Start();
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _LeaveCoordinator

        /// <summary>
        /// Invoked when transitioning out of the coordinator phase.
        /// </summary>
        private void _LeaveCoordinator()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________LeaveCoordinator");
#endif

            serviceController.Stop();

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Processing external events
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region IReplicaController Members

        #region IReplicaController.Initialize

        void IReplicaController.Initialize()
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________IReplicaController.Initialize");
#endif

            System.Diagnostics.Debug.Assert(nonpersistentState.Status == ReplicaStatus.Blocked);

            _SwitchTo(ReplicaStatus.Standby);
        }

        #endregion

        #region [ **** UNFINISHED **** ] IReplicaController.Hello

        void IReplicaController.Hello(Hello hello)
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________IReplicaController.Hello");
#endif

            int version_comparison_result = hello.MembershipView.Incarnation.CompareTo(nonpersistentState.CurrentView.Incarnation);
            if (version_comparison_result == 0)
                version_comparison_result = hello.MembershipView.SeqNo.CompareTo(nonpersistentState.CurrentView.SeqNo);

            if (version_comparison_result < 0)
            {
                _SendHello(hello.ReplicaAddress, false);
            }
            else
            {
                if (version_comparison_result > 0)
                {
#if DEBUG_LogGenerously
                    context.Logger.Log("A new view discovered through Hello:\n" + hello.MembershipView.ToString());
#endif

                    nonpersistentState.CurrentView = hello.MembershipView;

                    context.PersistencyController.Submit(new ReplicaPersistentState.Operation(
                        new ReplicaPersistentState.IAction[] { new ReplicaPersistentState.SetMembershipView(nonpersistentState.CurrentView) }), null);
                }

                for (int ind = 0; ind < nonpersistentState.CurrentView.Members.Length; ind++)
                {
                    if (nonpersistentState.CurrentView.Members[ind].Name.Equals(hello.ReplicaName))
                    {
                        IPeerInfo peerInfo = nonpersistentState.PeerInfo(ind);

                        if (!peerInfo.Discovered)
                        {
                            peerInfo.Discovered = true;
                            nonpersistentState.DiscoveredWeight += nonpersistentState.CurrentView.Members[ind].Weight;
                        }

                        if (hello.ReplicaIncarnation > peerInfo.ReplicaIncarnation)
                            peerInfo.ReplicaIncarnation = hello.ReplicaIncarnation;

                        if (hello.MembershipView.Incarnation > peerInfo.MachineIncarnation ||
                            ((hello.MembershipView.Incarnation == peerInfo.MachineIncarnation) && 
                                (hello.MembershipView.SeqNo > peerInfo.CurrentViewNumber)))
                        {
                            peerInfo.MachineIncarnation = hello.MembershipView.Incarnation;
                            peerInfo.CurrentViewNumber = hello.MembershipView.SeqNo;
                        }

                        break;
                    }
                }

                // TODO: We should update all processes...............
                // - may change view and as a result start/stop being in the view
                // - may get read quorum now because of discovery
                // - may stop having a read quorum because of the view change

/*
                switch (nonpersistentState.Status)
                {
                    case ReplicaStatus.Blocked:
                        {
                            System.Diagnostics.Debug.Assert(false, "Not Implemented");
                            // ................................................................................................................................................................
                        }
                        break;

                    case ReplicaStatus.Standby:
                        {
                            System.Diagnostics.Debug.Assert(false, "Not Implemented");
                            // ................................................................................................................................................................
                        }
                        break;

                    default:
                        {
                            System.Diagnostics.Debug.Assert(false, "Not Implemented");
                        }
                        break;
                }
*/

                if (hello.Respond)
                    _SendHello(hello.ReplicaAddress, false);
            }
        }

        #endregion

        #region [ NOT IMPLEMENTED ] IReplicaController.Append

        void IReplicaController.Append(Append append)
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________IReplicaController.Append");
#endif

            if (append.MachineName == persistentState.MachineName)
            {
                int version_comparison_result = append.MachineIncarnation.CompareTo(nonpersistentState.CurrentView.Incarnation);
                if (version_comparison_result == 0)
                    version_comparison_result = append.ViewSeqNo.CompareTo(nonpersistentState.CurrentView.SeqNo);

                if (version_comparison_result > 0)
                {
#if DEBUG_LogGenerously
                    context.Logger.Log("A new view discovered through Append { \n" + 
                        append.MachineIncarnation.ToString() + ":" + append.ViewSeqNo.ToString());
#endif

                    System.Diagnostics.Debug.Assert(false, "Not Implemented");

                    // TODO: Should somehow invalidate this view.......
                    _SwitchTo(ReplicaStatus.Standby);
                }
                else if (version_comparison_result < 0)
                {
                    _SendHello(append.ReplicaAddress, false);
                }
                else
                {
                    // ..............



                    List<ReplicaPersistentState.IAction> actions = null;

                    List<Submission> submissions = new List<Submission>(append.NewOperations.Count);
                    foreach (ServiceControl.IServiceControllerOperation operation in append.NewOperations)
                    {
                        if (operation.Persistent)
                        {
                            if (actions == null)
                                actions = new List<ReplicaPersistentState.IAction>();

                            // should record persistent action..........
                        }

                        submissions.Add(new Submission(operation, null, null));
                    }

                    // for now, we will just reuse the same structure
                    Request request = new Request(append.MachineName, append.ReplicaAddress, 
                        append.MachineIncarnation, append.ViewSeqNo, append.MessageSeqNo, submissions, null);



                    // ...............................................................................................................................................................................

/*
                    logger.Log(this, "Processing Append:\n" + append.ToString());

                    // ................................................................................................................................................
*/
                }
            }
            else
            {
#if DEBUG_LogGenerously
                context.Logger.Log("Append is for other machine: \"" + append.MachineName + "\".");
#endif
            }
        }

        #endregion

        #region [ NOT IMPLEMENTED ] IReplicaController.Token

        void IReplicaController.Token(Token token)
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________IReplicaController.Token");
#endif

            System.Diagnostics.Debug.Assert(false, "Not Implemented");

            // ...............................................................................................................................................................................

/*
            logger.Log(this, "Processing Token:\n" + token.ToString());

            if (isCoordinator)
            {

                // ................................................................................................................................................

                logger.Log(this, "This is coordinator, not forwarding token.");
            }
            else
            {

                // ................................................................................................................................................

                QS.Fx.Base.Address nextAddress = 
                    membershipView.Members[((positionInView + 1) % membershipView.Members.Length)].Address;

                logger.Log(this, "Forwarding token to : " + nextAddress.ToString());

                ((QS.CMS.Base3.ISerializableSender) unreliableSenders[nextAddress]).send(
                    (uint)QS.ReservedObjectID.Fx_Machine_Components_Replica, token);
            }
*/ 
        }

        #endregion

        #region [ NOT IMPLEMENTED ] IReplicaController.Submit

        void IReplicaController.Submit(ServiceControl.IServiceControllerOperation operation,
            QS.Fx.Base.ContextCallback<ServiceControl.IServiceControllerOperation, object> 
            operationCompletionCallback, object callbackContext)
        {
#if DEBUG_LogGenerously
            context.Logger.Log("__________IReplicaController.Submit");
#endif

            if (nonpersistentState.Status == ReplicaStatus.Coordinator)
            {
                nonpersistentState.PendingOperations.Enqueue(
                    new Submission(operation, operationCompletionCallback, callbackContext));

                if (!nonpersistentState.PendingOperationsRegistered)
                {
                    nonpersistentState.PendingOperationsRegistered = true;

                    if (nonpersistentState.IsSingleton)
                    {
                        if (nonpersistentState.PendingOperationsAlarm != null && !nonpersistentState.PendingOperationsAlarm.Cancelled)
                            nonpersistentState.PendingOperationsAlarm.Cancel();

                        nonpersistentState.PendingOperationsAlarm = context.AlarmClock.Schedule(
                            settings.PendingOperationsBatchingInterval, new QS.Fx.Clock.AlarmCallback(this._PendingOperationsAlarmCallback), null);                        
                    }
                    else
                    {
                        if (disseminationSink == null)
                        {
                            if (persistentState.DisseminationAddress != null && !persistentState.DisseminationAddress.IsNull)
                                disseminationSink = context.UnreliableSinks[persistentState.DisseminationAddress];
                            else
                            {
                                System.Diagnostics.Debug.Assert(false, "Not Implemented");
                            }
                        }

                        disseminationSink.Send(
                            new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(
                            this._DisseminationOutgoingCallback));
                    }
                }
            }
            else
                System.Diagnostics.Debug.Assert(false, "Service controller should have been destroyed when leaving \"Coordinator\".");

            // ...............................................................................................................................................................................
        }

        #endregion

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Methods related to dissemination of operations
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        #region [ NOT IMPLEMENTED ] _PendingOperationsAlarmCallback

        private void _PendingOperationsAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {               
                if (nonpersistentState.PendingOperations.Count > 0)
                {
                    System.Diagnostics.Debug.Assert(false, "Not Implemented");

                    // _ProcessPendingOperations.......................................................................

                    nonpersistentState.PendingOperationsRegistered = false;
                }
            }
        }

        #endregion

        #region _DisseminationOutgoingCallback

        private void _DisseminationOutgoingCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sendQueue, 
            int maxToSend, out int numReturned, out bool moreToSend)
        {
            lock (this)
            {
                System.Diagnostics.Debug.Assert(nonpersistentState.Status == ReplicaStatus.Coordinator);

                // TODO: Should check for space in the packet, may need to limit the number of operations sent....................

                Request request = new Request(
                    persistentState.MachineName, persistentState.ReplicaAddress, nonpersistentState.CurrentView.Incarnation,
                    nonpersistentState.CurrentView.SeqNo, ++nonpersistentState.LastSubmittedMessageNumber, 
                    new List<Submission>(nonpersistentState.PendingOperations),
                    new QS._core_c_.Base6.CompletionCallback<object>(this._DisseminationCompleteCallback));

                sendQueue.Enqueue(request);

                numReturned = 1;
                moreToSend = false;
                
                nonpersistentState.PendingOperationsRegistered = false; 
            }
        }

        #endregion

        #region _DisseminationCompleteCallback

        private void _DisseminationCompleteCallback(bool succeeded, Exception exception, object callbackContext)
        {
            lock (this)
            {
                if (!nonpersistentState.TokenCirculationIsActive)
                {
                    if (nonpersistentState.TokenCirculationAlarm != null && !nonpersistentState.TokenCirculationAlarm.Cancelled)
                        nonpersistentState.TokenCirculationAlarm.Cancel();

                    nonpersistentState.TokenTimestamp = context.Clock.Time;
                    nonpersistentState.TokenCirculationAlarm = context.AlarmClock.Schedule(
                        1 / settings.TokenRate, new QS.Fx.Clock.AlarmCallback(this._TokenCirculationAlarmCallback), null);

                    nonpersistentState.TokenCirculationIsActive = true;

                    _CirculateToken();
                }
            }
        }

        #endregion

        #region _TokenCirculationAlarmCallback

        private void _TokenCirculationAlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                if (nonpersistentState.TokenCirculationIsActive)
                {
                    alarm.Reschedule(1 / settings.TokenRate);

                    _CirculateToken();
                }
                else
                {
                    if (!alarm.Cancelled)
                        alarm.Cancel();
                }
            }
        }

        #endregion

        #region _CirculateToken

        private void _CirculateToken()
        {
            System.Diagnostics.Debug.Assert(
                nonpersistentState.Status == ReplicaStatus.Coordinator && nonpersistentState.PositionInView == 0);

            Token token = new Token(persistentState.MachineName, nonpersistentState.CurrentView.Incarnation, 
                nonpersistentState.CurrentView.SeqNo, nonpersistentState.LastSubmittedMessageNumber);

            ((QS._qss_c_.Base3_.ISerializableSender) context.UnreliableSenders[nonpersistentState.CurrentView.Members[1].Address]).send(
                        (uint) QS.ReservedObjectID.Fx_Machine_Components_Replica, token);
            
            // ................................................................................................................................................................
        }

        #endregion

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
