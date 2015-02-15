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

namespace QS._qss_c_.Receivers4
{
    /// <summary>
    /// This class represents a controller that instantiates receiving agents for region views and tears them
    /// down when necessary.
    /// </summary>    
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class RegionalController : QS.Fx.Inspection.Inspectable,
        IRegionalController, IReceivingAgentCollection<Base3_.RVID>, ICollectingAgentCollection<Base3_.RVID>, QS._core_c_.Diagnostics2.IModule
    {
        private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForReceiverContexts = new QS._core_c_.Diagnostics2.Container();
        private QS._core_c_.Diagnostics2.Container diagnosticsContainerForAcknowledgementContexts = new QS._core_c_.Diagnostics2.Container();

        #region IModule Members

        QS._core_c_.Diagnostics2.IComponent QS._core_c_.Diagnostics2.IModule.Component
        {
            get { return diagnosticsContainer; }
        }

        #endregion

        #region Constructor

        public RegionalController(QS._core_c_.Base3.InstanceID localAddress, QS.Fx.Logging.ILogger logger, Base3_.IDemultiplexer demultiplexer, 
            QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, Membership2.Controllers.IMembershipController membershipController,
            IReceivingAgentClass receivingAgentClass, ICollectingAgentClass collectingAgentClass)
        {
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("ReceiverContexts", diagnosticsContainerForReceiverContexts);
            ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("AcknowledgementContexts", diagnosticsContainerForAcknowledgementContexts);

            this.localAddress = localAddress;
            this.logger = logger;
            this.demultiplexer = demultiplexer;
            this.alarmClock = alarmClock;
            this.membershipController = membershipController;
            this.clock = clock;
            this.receivingAgentClass = receivingAgentClass;
            this.collectingAgentClass = collectingAgentClass;

            ((Membership2.Consumers.IRegionChangeProvider) membershipController).OnChange += 
                new QS._qss_c_.Membership2.Consumers.RegionChangedCallback(membershipController_OnChange);
            demultiplexer.register((uint)ReservedObjectID.Receivers4_RegionalController_AgentChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.AgentReceiveCallback));
            demultiplexer.register((uint)ReservedObjectID.Receivers4_RegionalController_MessageChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.MessageReceiveCallback));
            demultiplexer.register((uint)ReservedObjectID.Receivers4_RegionalController_AckChannel,
                new QS._qss_c_.Base3_.ReceiveCallback(this.AcknowledgementCallback));

            receiverCollectionInspectableWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, RegionalContext>(
                    "Receiver Contexts", registered,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, RegionalContext>.ConversionCallback(
                        Base3_.RVID.FromString));

            acknowledgementContextsInspectableWrapper =
                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, AcknowledgementContext>(
                    "Acknowledgement Contexts", acknowledgementContexts,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS._qss_c_.Base3_.RVID, AcknowledgementContext>.ConversionCallback(
                        Base3_.RVID.FromString));
        }

        private QS._core_c_.Base3.InstanceID localAddress;
        private QS.Fx.Logging.ILogger logger;
        private Base3_.IDemultiplexer demultiplexer;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.IClock clock;
        private Membership2.Controllers.IMembershipController membershipController;
        private IReceivingAgentClass receivingAgentClass;
        private ICollectingAgentClass collectingAgentClass;
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<Base3_.RVID, RegionalContext> registered = new Dictionary<Base3_.RVID, RegionalContext>();
        [QS._core_c_.Diagnostics.ComponentCollection]
        private IDictionary<Base3_.RVID, AcknowledgementContext> acknowledgementContexts =
            new Dictionary<Base3_.RVID, AcknowledgementContext>();
        
        // FIX: 20070503
        private bool disabled = false;

        [QS.Fx.Base.Inspectable("Receiver Contexts")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, RegionalContext> receiverCollectionInspectableWrapper;

        [QS.Fx.Base.Inspectable("Acknowledgement Contexts")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<Base3_.RVID, AcknowledgementContext> acknowledgementContextsInspectableWrapper;

        #endregion

        #region Processing membership changes

        void membershipController_OnChange(QS._qss_c_.Membership2.Consumers.RegionChange change)
        {
            if (disabled)
            {
                logger.Log(this, "___membershipController_OnChange : DISABLED");
            }
            else
            {
                lock (this)
                {
                    Base3_.RVID rvid;

                    if (change.CurrentView != null)
                    {
                        rvid = new QS._qss_c_.Base3_.RVID(change.CurrentView.Region.ID, change.CurrentView.SeqNo);
                        
                        logger.Log(this, "___membershipController_OnChange : Processing\n" + QS.Fx.Printing.Printable.ToString(change));

                        if (!registered.ContainsKey(rvid))
                        {
                            logger.Log(this, "___membershipController_OnChange : Creating ( " + rvid.ToString() + " )");

                            RegionalContext context = new RegionalContext(this, rvid);
                            context.Agent = receivingAgentClass.CreateAgent(context);
                            registered.Add(rvid, context);
                        }
                    }
                    else
                        rvid = QS._qss_c_.Base3_.RVID.Undefined; // hack

                    // TODO: Recycling of agents for old, unwanted views.

                    List<Base3_.RVID> toremove = new List<QS._qss_c_.Base3_.RVID>();

                    foreach (KeyValuePair<Base3_.RVID, RegionalContext> element in registered)
                    {
                        if (!element.Key.Equals(rvid))
                        {
                            toremove.Add(element.Key);
                            logger.Log(this, "___membershipController_OnChange : Finalize ( " + element.Key.ToString() + " )");
                            element.Value.Agent.Shutdown();
                            if (element.Value.Agent is IDisposable)
                                ((IDisposable) element.Value.Agent).Dispose();
                            if (element.Value is IDisposable)
                                ((IDisposable) element.Value).Dispose();
                        }
                    }

                    foreach (Base3_.RVID _rvid in toremove)
                        registered.Remove(_rvid);

                    List<Base3_.RVID> _acktoremove = new List<QS._qss_c_.Base3_.RVID>();
                    foreach (Membership2.ClientState.IRegion _region in change.RegionsRemoved)
                    {
                        foreach (KeyValuePair<Base3_.RVID, AcknowledgementContext> _element in acknowledgementContexts)
                        {
                            if (_element.Key.RegionID.Equals(_region.ID))
                                _acktoremove.Add(_element.Key);
                        }
                    }

                    foreach (Base3_.RVID _rvid in _acktoremove)
                    {
                        ((IDisposable) acknowledgementContexts[_rvid].Agent).Dispose();
                        acknowledgementContexts.Remove(_rvid);
                    }
                }
            }
        }

        #endregion

        #region Adjusting Configuration

        public bool IsDisabled
        {
            get { return disabled; }
            set { disabled = value; }
        }

        #endregion

        #region GetContext

        private RegionalContext GetContext(Base3_.RVID destinationAddress, bool createOnDemand)
        {
            RegionalContext element = null;
            lock (this)
            {
                if (!registered.TryGetValue(destinationAddress, out element))
                {
                    if (createOnDemand)
                    {
                        element = new RegionalContext(this, destinationAddress);
                        registered.Add(destinationAddress, element);
                    }
                    else
                    {
                        // throw new Exception("Received a message targeted at a nonexisting receiving agent.");
                    }
                }
            }
            return element;
        }

        #endregion

        #region GetAckContext

        private AcknowledgementContext GetAckContext(Base3_.RVID destinationAddress, bool createOnDemand)
        {
            AcknowledgementContext element = null;
            lock (this)
            {
                if (acknowledgementContexts.ContainsKey(destinationAddress))
                    element = acknowledgementContexts[destinationAddress];
                else
                {
                    if (createOnDemand)
                        acknowledgementContexts[destinationAddress] = element = new AcknowledgementContext(this, destinationAddress);
                    else
                    {
                        // throw new Exception("Received a message targeted at a nonexisting receiving agent.");
                    }
                }
            }
            return element;
        }

        #endregion

        #region Receive callbacks

        private QS.Fx.Serialization.ISerializable AgentReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Rings4.ObjectRV objectRV = receivedObject as Rings4.ObjectRV;
            if (objectRV == null)
                throw new Exception("Received a message of incompatible type.");

            RegionalContext element = GetContext(objectRV.Address, false);

            return (element != null) ? ((Base3_.IDemultiplexer) element).dispatch(
                objectRV.Message.destinationLOID, sourceAddress, objectRV.Message.transmittedObject) : null;
        }

        private QS.Fx.Serialization.ISerializable MessageReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Multicasting5.MessageRV message = receivedObject as Multicasting5.MessageRV;
            if (message == null)
                throw new Exception("Received a message of an incompatible type.");

            ((IRegionalController) this).Receive(sourceAddress, message.RVID, message.SeqNo, message.EncapsulatedMessage, false, false);

            return null;
        }

        private QS.Fx.Serialization.ISerializable AcknowledgementCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            Rings4.ObjectRV objectRV = receivedObject as Rings4.ObjectRV;
            if (objectRV == null)
                throw new Exception("Received a message of an incompatible type.");

            ((IRegionalController)this).Acknowledge(sourceAddress, objectRV.Address, objectRV.Message);

            return null;
        }

        #endregion

        #region Class RegionalContext

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class RegionalContext : QS.Fx.Inspection.Inspectable, IReceivingAgentContext, Base3_.IDemultiplexer, IDisposable
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            public RegionalContext(RegionalController owner, Base3_.RVID address)
            {
                ((QS._core_c_.Diagnostics2.IContainer)owner.diagnosticsContainerForReceiverContexts).Register(
                    address.ToString(), diagnosticsContainer);

                this.owner = owner;
                this.address = address;

                regionVC = (Membership2.Controllers.IRegionViewController) 
                    owner.membershipController.lookupRegion(address.RegionID)[address.SeqNo];
                receiverAddresses = new QS._core_c_.Base3.InstanceID[regionVC.Members.Count];
                regionVC.Members.CopyTo(receiverAddresses, 0);
                Array.Sort<QS._core_c_.Base3.InstanceID>(receiverAddresses);
            }

            private bool disposed;

            private RegionalController owner;
            private Base3_.RVID address;
            private Membership2.Controllers.IRegionViewController regionVC;
            private QS._core_c_.Base3.InstanceID[] receiverAddresses;
            private IDictionary<uint, Base3_.ReceiveCallback> receiveCallbacks = new Dictionary<uint, Base3_.ReceiveCallback>();            
            private IReceivingAgent agent;

            [QS._core_c_.Diagnostics.Component("Agent")]      
            public IReceivingAgent Agent
            {
                set                 
                {
                    if (value is QS._core_c_.Diagnostics2.IModule)
                        ((QS._core_c_.Diagnostics2.IContainer)diagnosticsContainer).Register("Agent", ((QS._core_c_.Diagnostics2.IModule) value).Component);
                    agent = value; 
                }

                get { return agent; }
            }

            #region IReceivingAgentContext Members

            QS.Fx.Serialization.ISerializable IReceivingAgentContext.ID
            {
                get { return address; }
            }

            QS._core_c_.Base3.InstanceID[] IReceivingAgentContext.ReceiverAddresses
            {
                get { return receiverAddresses; }
            }

            Failure_.ISource IReceivingAgentContext.FailureSource
            {
                get { return regionVC; }
            }

            QS._core_c_.Base3.Message IReceivingAgentContext.Message(QS._core_c_.Base3.Message message, DestinationType destinationType)
            {
                switch (destinationType)
                {
                    case DestinationType.Receiver:
                        return new QS._core_c_.Base3.Message((uint)ReservedObjectID.Receivers4_RegionalController_AgentChannel,
                            new Rings4.ObjectRV(address, message));

                    case DestinationType.Sender:
                        return new QS._core_c_.Base3.Message((uint)ReservedObjectID.Receivers4_RegionalController_AckChannel,
                            new Rings4.ObjectRV(address, message));

                    default:
                        throw new NotSupportedException("Wrong type.");
                }
            }

            Base3_.IDemultiplexer IReceivingAgentContext.Demultiplexer
            {
                get { return this; }
            }

            #endregion

            #region IDemultiplexer Members

            void QS._qss_c_.Base3_.IDemultiplexer.register(uint localObjectID, QS._qss_c_.Base3_.ReceiveCallback receiveCallback)
            {
                lock (this)
                {
                    if (!receiveCallbacks.ContainsKey(localObjectID))
                        receiveCallbacks.Add(localObjectID, receiveCallback);
                    else
                        throw new Exception("This component has already been registered.");
                }
            }

            void QS._qss_c_.Base3_.IDemultiplexer.unregister(uint localObjectID)
            {
                throw new NotImplementedException();
            }

            QS.Fx.Serialization.ISerializable QS._qss_c_.Base3_.IDemultiplexer.dispatch(uint destinationLOID, QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                Base3_.ReceiveCallback callback = null;
                lock (this)
                {
                    if (disposed)
                        owner.logger.Log("regional context cannot dispatch to " + destinationLOID.ToString() + ", regional context " + address.ToString() + " is disposed");
                    else
                    {
                        if (receiveCallbacks.ContainsKey(destinationLOID))
                            callback = receiveCallbacks[destinationLOID];
                        else
                            throw new Exception("Cannot dispatch, no component registered with LOID = " + destinationLOID.ToString());
                    }
                }

                return (callback != null) ? callback(sourceAddress, receivedObject) : null;
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                disposed = true;
            }

            #endregion
        }

        #endregion

        #region Class AcknowledgementContext

        [QS._core_c_.Diagnostics.ComponentContainer]
        private class AcknowledgementContext : QS.Fx.Inspection.Inspectable, ICollectingAgentContext, Base3_.IDemultiplexer
        {
            private QS._core_c_.Diagnostics2.Container diagnosticsContainer = new QS._core_c_.Diagnostics2.Container();

            public AcknowledgementContext(RegionalController owner, Base3_.RVID address)
            {
                ((QS._core_c_.Diagnostics2.IContainer)owner.diagnosticsContainerForAcknowledgementContexts).Register(
                    address.ToString(), diagnosticsContainer, QS._core_c_.Diagnostics2.RegisteringMode.Override);

                this.owner = owner;
                this.address = address;
                this.agent = owner.collectingAgentClass.CreateAgent(this);

                QS._core_c_.Diagnostics2.Helper.RegisterLocal(diagnosticsContainer, this);
            }

            private RegionalController owner;
            private Base3_.RVID address;
            private IDictionary<uint, Base3_.ReceiveCallback> receiveCallbacks = new Dictionary<uint, Base3_.ReceiveCallback>();
            private ICollectingAgent agent;

            public void AcknowledgementReceiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS._core_c_.Base3.Message message)
            {
                ((Base3_.IDemultiplexer) this).dispatch(message.destinationLOID, sourceAddress, message.transmittedObject);
            }

            [QS._core_c_.Diagnostics.Component("Agent")]            
            [QS._core_c_.Diagnostics2.Module("Agent")]
            public ICollectingAgent Agent
            {
                get { return agent; }
            }

            #region IDemultiplexer Members

            void QS._qss_c_.Base3_.IDemultiplexer.register(uint localObjectID, QS._qss_c_.Base3_.ReceiveCallback receiveCallback)
            {
                lock (this)
                {
                    if (!receiveCallbacks.ContainsKey(localObjectID))
                        receiveCallbacks.Add(localObjectID, receiveCallback);
                    else
                        throw new Exception("This component has already been registered.");
                }
            }

            void QS._qss_c_.Base3_.IDemultiplexer.unregister(uint localObjectID)
            {
                throw new NotImplementedException();
            }

            QS.Fx.Serialization.ISerializable QS._qss_c_.Base3_.IDemultiplexer.dispatch(uint destinationLOID, QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
            {
                Base3_.ReceiveCallback callback;
                lock (this)
                {
                    if (receiveCallbacks.ContainsKey(destinationLOID))
                        callback = receiveCallbacks[destinationLOID];
                    else
                        throw new Exception("Cannot dispatch, no component registered with LOID = " + destinationLOID.ToString());
                }

                return callback(sourceAddress, receivedObject);
            }

            #endregion

            #region ICollectingAgentContext Members

            QS._core_c_.Base3.Message ICollectingAgentContext.Message(QS._core_c_.Base3.Message message)
            {
                return new QS._core_c_.Base3.Message((uint)ReservedObjectID.Receivers4_RegionalController_AckChannel,
                    new Rings4.ObjectRV(address, message));
            }

            QS.Fx.Serialization.ISerializable ICollectingAgentContext.ID
            {
                get { return address; }
            }

            QS._qss_c_.Base3_.IDemultiplexer ICollectingAgentContext.Demultiplexer
            {
                get { return this; }
            }

            #endregion
        }

        #endregion

        #region IRegionalController Members

        void IRegionalController.Receive(QS._core_c_.Base3.InstanceID sourceAddress, 
            QS._qss_c_.Base3_.RVID destinationAddress, uint sequenceNo, QS._core_c_.Base3.Message message, bool retransmission, bool forwarding)
        {
            RegionalContext regionalContext = GetContext(destinationAddress, false);
            if (regionalContext != null)
            {
                IReceivingAgent agent = regionalContext.Agent;
                if (agent != null)
                    agent.Receive(sourceAddress, sequenceNo, message, retransmission, forwarding);
                else
                {
                    // ............
                }
            }
        }

        void IRegionalController.Acknowledge(
            QS._core_c_.Base3.InstanceID sourceAddress, Base3_.RVID destinationAddress, QS._core_c_.Base3.Message message)
        {
            AcknowledgementContext ackContext = GetAckContext(destinationAddress, false);
            if (ackContext != null)
                ackContext.AcknowledgementReceiveCallback(sourceAddress, message);
            else
                throw new Exception("__Acknowledge: cannot find acknowledgement context for " + destinationAddress.ToString());
        }

        #endregion

        #region IReceivingAgentCollection<RVID> Members

        IReceivingAgent IReceivingAgentCollection<QS._qss_c_.Base3_.RVID>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return GetContext(destinationAddress, true).Agent; }
        }

        bool IReceivingAgentCollection<QS._qss_c_.Base3_.RVID>.TryGetAgent(
            QS._qss_c_.Base3_.RVID address, out IReceivingAgent receiving_agent, bool create_on_demand)
        {
            RegionalContext regionalContext = GetContext(address, create_on_demand);
            receiving_agent = (regionalContext != null) ? regionalContext.Agent : null;
            return receiving_agent != null;
        }

        #endregion

        #region ICollectingAgentCollection<RVID> Members

        ICollectingAgent ICollectingAgentCollection<QS._qss_c_.Base3_.RVID>.this[QS._qss_c_.Base3_.RVID destinationAddress]
        {
            get { return GetAckContext(destinationAddress, true).Agent; }
        }

        #endregion
    }
}
