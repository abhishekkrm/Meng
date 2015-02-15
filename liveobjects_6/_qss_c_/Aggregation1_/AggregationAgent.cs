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

// #define DEBUG_AggregationAgent
// #define DEBUG_DeadlockDetection
// #define DEBUG_TrackRemovedControllers
// #define DEBUG_LogUnmatched
// #define DEBUG_ControllerNotFoundWarning
// #define DEBUG_CrashProcessing

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Threading;

namespace QS._qss_c_.Aggregation1_
{
    public class AggregationAgent : QS.Fx.Inspection.Inspectable, IAggregationAgent, Aggregation3_.IAgent
    {
        public AggregationAgent(QS.Fx.Logging.ILogger logger, QS._core_c_.Base3.InstanceID instanceID, 
            Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> instanceSenderCollection,
            Base3_.IDemultiplexer demultiplexer)
        {
            this.instanceID = instanceID;
            this.logger = logger;
            this.instanceSenderCollection = instanceSenderCollection;

            registeredClasses = new SortedDictionary<ClassID, IAggregationClass>();
            activeControllers = new SortedDictionary<AggregationID, AggregationController>();

            demultiplexer.register((uint)ReservedObjectID.Aggregation_AggregationAgent, new QS._qss_c_.Base3_.ReceiveCallback(this.receiveCallback));

			inspectableControllersWrapper = 
				new QS._qss_e_.Inspection_.DictionaryWrapper2<AggregationID, AggregationController>("Active Controllers", activeControllers);
		}

        private QS.Fx.Logging.ILogger logger;
        private IDictionary<ClassID, IAggregationClass> registeredClasses;
        private QS._core_c_.Base3.InstanceID instanceID;
        private IDictionary<AggregationID, AggregationController> activeControllers;
        // private Base3.IDemultiplexer demultiplexer;
        private Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> instanceSenderCollection;

		[QS.Fx.Base.Inspectable("Active Controllers", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private QS._qss_e_.Inspection_.DictionaryWrapper2<AggregationID, AggregationController> inspectableControllersWrapper;

		#region RemoveController

#if DEBUG_TrackRemovedControllers
		private IDictionary<AggregationID, AggregationController> removedControllers = new SortedDictionary<AggregationID, AggregationController>();
#endif

		private void removeController(AggregationID aggregationID)
		{
			try
			{
				lock (activeControllers)
				{
#if DEBUG_TrackRemovedControllers
					removedControllers[aggregationID] = activeControllers[aggregationID];
#endif

					activeControllers.Remove(aggregationID);
				}
			}
#pragma warning disable 0168
			catch (Exception exc)
			{
#if DEBUG_TrackRemovedControllers
				if (removedControllers.ContainsKey(aggregationID))
				{
					logger.Log(this, "__RemoveController(" + aggregationID.ToString() + "): This controller has already been removed.");
					return;
				}
#endif				

#if DEBUG_AggregationAgent
				logger.Log(this, "__RemoveController(" + aggregationID.ToString() + "): " + exc.ToString());
#endif
			}
#pragma warning restore 0168
		}

		#endregion

#if DEBUG_DeadlockDetection
		private Debugging.Locking.Log lockingLog = new QS.CMS.Debugging.Locking.Log();
#endif

		public Base3_.ISenderCollection<QS._core_c_.Base3.InstanceID, QS._qss_c_.Base3_.ISerializableSender> UnderlyingSenderCollection
		{
			set { instanceSenderCollection = value; }
		}

        #region Receive Callback

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceIID, QS.Fx.Serialization.ISerializable receivedObject)
        {
#if DEBUG_AggregationAgent
            logger.Log(this, "__ReceiveCallback : " + Helpers.ToString.ReceivedObject(sourceAddress, receivedObject));
#endif

			Aggregation3_.IAggregatable aggregatableObject = null;
			AggregationID aggregationID = receivedObject as AggregationID;
			if (aggregationID == null)
			{
				ControllerMessage cmsg = receivedObject as ControllerMessage;
				if (cmsg != null)
				{
					aggregatableObject = cmsg.ToAggregate;
					aggregationID = cmsg.AggregationID;
				}
			}

            if (aggregationID != null)
            {
				QS._core_c_.Base3.InstanceID instanceID = sourceIID;
				AggregationController aggregationController;

				try
				{
#if DEBUG_DeadlockDetection
					lockingLog.Add(Debugging.Locking.Operation.LOCK, "AggregationAgent.receiveCallback");
					Debugging.Locking.Check(this, logger, lockingLog);
#endif
					lock (activeControllers) // thread #1 holds a lock that thread #2 cannot grab
					{
						aggregationController = this.GetController(aggregationID);
					}

#if DEBUG_DeadlockDetection
					lockingLog.Add(Debugging.Locking.Operation.UNLOCK, "AggregationAgent.receiveCallback");
#endif

                    if (aggregationController != null)
                    {
                        // rearranged to remove the deadlock
                        aggregationController.acknowledged(instanceID, aggregatableObject); // nasty hack
                    }
                    else
                    {
#if DEBUG_ControllerNotFoundWarning
                        logger.Log(this, "__ReceiveCallback: Aggregation controller for {" + aggregationID.ToString() + "} not found.");
#endif
                    }
				}
				catch (Exception exc)
				{
// #if DEBUG_AggregationAgent
					logger.Log(this, "__ReceiveCallback : Cannot acknowledge request " + aggregationID.ToString() + " from " + instanceID.ToString() + ".\n" + exc.ToString());
// #endif
				}
			}
			else
                throw new Exception("Received object of incompatible type" + receivedObject.GetType().Name + ".");

            return null;
        }

        #endregion

		#region Class ControllerMessage

		[QS.Fx.Serialization.ClassID(ClassID.Aggregation_ControllerMessage)]
		private class ControllerMessage : QS.Fx.Serialization.ISerializable
		{
			public ControllerMessage()
			{
			}

			public ControllerMessage(AggregationID aggregationID, Aggregation3_.IAggregatable toAggregate)
			{
				this.AggregationID = aggregationID;
				this.ToAggregate = toAggregate;
			}

			public AggregationID AggregationID;
			public Aggregation3_.IAggregatable ToAggregate;

			#region ISerializable Members

			QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
			{
				get
				{
					return ((QS.Fx.Serialization.ISerializable) this.AggregationID).SerializableInfo.CombineWith(
						ToAggregate.SerializableInfo).Extend(
							(ushort) ClassID.Aggregation_ControllerMessage, (ushort) sizeof(ushort), 0, 0);
				}
			}

			unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
			{
				fixed (byte* arrayptr = header.Array)
				{
					*((ushort*)(arrayptr + header.Offset)) = ToAggregate.SerializableInfo.ClassID;
				}
				header.consume(sizeof(ushort));
				((QS.Fx.Serialization.ISerializable) this.AggregationID).SerializeTo(ref header, ref data);
				ToAggregate.SerializeTo(ref header, ref data);
			}

			unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
			{
				ushort classID;
				fixed (byte* arrayptr = header.Array)
				{
					classID = *((ushort*)(arrayptr + header.Offset));
				}
				header.consume(sizeof(ushort));
				this.AggregationID = new AggregationID();
				((QS.Fx.Serialization.ISerializable)this.AggregationID).DeserializeFrom(ref header, ref data);
				ToAggregate = (Aggregation3_.IAggregatable) QS._core_c_.Base3.Serializer.CreateObject(classID);
				ToAggregate.DeserializeFrom(ref header, ref data);
			}

			#endregion
		}

		#endregion

        #region Class AggregationController

        private class AggregationController : QS.Fx.Inspection.Inspectable, IAggregationController
        {
            public AggregationController(AggregationAgent owner, AggregationID aggregationID, 
                Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> routingStructure, Failure_.ISource failureSource)
            {
                this.aggregationID = aggregationID;
                this.owner = owner;
                this.routingStructure = routingStructure;

                submitted = false;
                incomingAddresses = routingStructure.Incoming(owner.instanceID, aggregationID.RootAddress);
                IList<QS._core_c_.Base3.InstanceID> outgoingAddresses = routingStructure.Outgoing(owner.instanceID, aggregationID.RootAddress);                
                if (outgoingAddresses.Count > 1)
                    throw new ArgumentException("Aggregation controller does not support routing schemes with multiple outgoing paths.");
                outgoingAddress = (outgoingAddresses.Count > 0) ? outgoingAddresses[0] : null;

// #if DEBUG_AggregationAgent
//                StringBuilder s = new StringBuilder("Calling constructor for ");
//                s.Append(aggregationID.ToString());
//                s.Append(" : incoming ");
//                s.Append(Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(incomingAddresses, ", "));
//                s.Append("; outgoing ");
//                s.Append(Helpers.ToString.Object(outgoingAddress));
//                owner.logger.Log(this,  s.ToString());
// #endif

                lock (this)
                {
                    IList<QS._core_c_.Base3.InstanceID> crashedAddresses;
                    failureSubscription = failureSource.subscribe(
                        new QS._qss_c_.Failure_.NotificationsCallback(this.notificationsCallback), out crashedAddresses);

                    foreach (QS._core_c_.Base3.InstanceID crashedAddress in crashedAddresses)
                        incomingAddresses.Remove(crashedAddress);

                    while (outgoingAddress != null && crashedAddresses.Contains(outgoingAddress))
                    {
                        outgoingAddresses = routingStructure.Outgoing(outgoingAddress, aggregationID.RootAddress);
                        outgoingAddress = (outgoingAddresses.Count > 0) ? outgoingAddresses[0] : null;
                    }

                    acknowledgementsCollected = (incomingAddresses.Count == 0);
                }
            }

			[QS.Fx.Base.Inspectable]
			public string CurrentStatus_AsString
			{
				get
				{
					StringBuilder s = new StringBuilder("Aggregation " + aggregationID.ToString() +
						"\n\nOutgoingAddress:\n" + QS._core_c_.Helpers.ToString.Object(outgoingAddress) + "\nIncomingAddresses:\n");
					foreach (QS._core_c_.Base3.InstanceID address in incomingAddresses)
						s.AppendLine(address.ToString());
					s.AppendLine("\nCallback: " + ((aggregationCallback != null) ? "YES" : "NO") + "\nSubmitted: " + 
						(submitted ? "YES" : "NO") + "\nAcknowledgementsCollected: " + 
						(acknowledgementsCollected ? "YES" : "NO"));
					return s.ToString();
				}
			}

			private AggregationID aggregationID;
            private AggregationAgent owner;
            private Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> routingStructure;
            private IList<QS._core_c_.Base3.InstanceID> incomingAddresses, unmatchedAcknowledgements;
            private QS._core_c_.Base3.InstanceID outgoingAddress;
            private Failure_.ISubscription failureSubscription;
            private AggregationCallback aggregationCallback = null;
            private bool submitted, acknowledgementsCollected;
			private Aggregation3_.IAggregatable toAggregate;

#if DEBUG_CrashProcessing
            [QS.TMS.Inspection.Inspectable("CrashProcessingLog", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
            private Base.Logger crashProcessingLog = new QS.CMS.Base.Logger(true);
#endif

			#region Processing the Aggregated Objects

			public Aggregation3_.IAggregatable AggregatedObject
			{
				get { return toAggregate; }
			}

			private void addAggregatable(Aggregation3_.IAggregatable aggregatableObject)
			{
				if (aggregatableObject != null)
				{
					if (this.toAggregate == null)
						this.toAggregate = aggregatableObject;
					else
						this.toAggregate.aggregateWith(aggregatableObject);
				}
			}

			#endregion

			AggregationID IAggregationController.ID
			{
				get { return aggregationID; }
			}	

            private void notificationsCallback(IList<QS._core_c_.Base3.InstanceID> asynchronousNotifications)
            {
#if DEBUG_CrashProcessing
                string MyDebugString = "__NotificationsCallback[" + aggregationID.ToString() + "] : ";
                crashProcessingLog.logMessage(this, MyDebugString + 
                    Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(asynchronousNotifications, ","));
#endif

                Monitor.Enter(this);
                
                bool shouldCheck = false, resendAcknowledgement = false;
                try
                {
                    bool parentChanged = false;
                    while (outgoingAddress != null && asynchronousNotifications.Contains(outgoingAddress))
                    {
                        IList<QS._core_c_.Base3.InstanceID> outgoingAddresses = routingStructure.Outgoing(outgoingAddress, aggregationID.RootAddress);
                        outgoingAddress = (outgoingAddresses.Count > 0) ? outgoingAddresses[0] : null;

#if DEBUG_CrashProcessing
                        crashProcessingLog.logMessage(this, MyDebugString + "Switched parent to " + Helpers.ToString.Object(outgoingAddress));
#endif

                        parentChanged = true;
                    }

                    resendAcknowledgement = parentChanged && acknowledgementsCollected;

                    foreach (QS._core_c_.Base3.InstanceID crashedAddress in asynchronousNotifications)
                    {
                        if (incomingAddresses.Remove(crashedAddress))
                        {
#if DEBUG_CrashProcessing
                            crashProcessingLog.logMessage(this, MyDebugString + "Removed crashed " + crashedAddress.ToString());
#endif

                            IList<QS._core_c_.Base3.InstanceID> newIncoming = routingStructure.Incoming(crashedAddress, aggregationID.RootAddress);
                            if (newIncoming != null)
                            {
                                foreach (QS._core_c_.Base3.InstanceID childAddress in newIncoming)
                                {
                                    incomingAddresses.Add(childAddress);

#if DEBUG_CrashProcessing
                                    crashProcessingLog.logMessage(this, MyDebugString + "Adopting child " + childAddress.ToString());
#endif
                                }

                                if (unmatchedAcknowledgements != null)
                                {
                                    foreach (QS._core_c_.Base3.InstanceID unmatchedAddress in unmatchedAcknowledgements)
                                    {
                                        if (incomingAddresses.Remove(unmatchedAddress))
                                        {
#if DEBUG_CrashProcessing
                                            crashProcessingLog.logMessage(this, MyDebugString + "Removed unmatched " + unmatchedAddress.ToString());
#endif
                                        }
                                    }
                                }
                            }

                            shouldCheck = true;
                        }
                    }
                }
                catch (Exception exc)
                {
                    Monitor.Exit(this);
                    throw new Exception("__NotificationsCallback: Internal error", exc);
                }

                if (shouldCheck)
                    shouldCheck = acknowledgementsCollected = (incomingAddresses.Count == 0);

                if (shouldCheck)
                {
#if DEBUG_CrashProcessing
                    crashProcessingLog.logMessage(this, MyDebugString + "Currently pending: " + 
                        Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(incomingAddresses, ","));
#endif

                    this.CheckCompletion();
                }
                else
                    Monitor.Exit(this);

                if (resendAcknowledgement)
                {
#if DEBUG_CrashProcessing
                    crashProcessingLog.logMessage(this, MyDebugString + "Resending acknowledgement to " + Helpers.ToString.Object(outgoingAddress));
#endif
                    notifyOugtoing();
                }
            }

            public AggregationCallback AggregationCallback
            {
                set { this.aggregationCallback = value; }
            }

            private void notifyOugtoing()
            {
                QS.Fx.Serialization.ISerializable toSend = aggregationID;
                if (toAggregate != null)
                    toSend = new ControllerMessage(aggregationID, toAggregate);

                owner.instanceSenderCollection[outgoingAddress].send(
                    (uint)ReservedObjectID.Aggregation_AggregationAgent, toSend);
            }

			// This method is always entered with a lock on "this", which should be released before exiting.
            private void CheckCompletion()
            {
				if (submitted && acknowledgementsCollected)
				{
#if DEBUG_AggregationAgent
                    owner.logger.Log(this, "__CheckCompletion: Ready.");
#endif
                    failureSubscription.Cancel();

					if (outgoingAddress != null)
					{
#if DEBUG_AggregationAgent
                        owner.logger.Log(this, "__CheckCompletion: forwarding to " + outgoingAddress.ToString());
#endif
						Monitor.Exit(this);

                        notifyOugtoing();
					}
					else if (aggregationCallback != null)
					{
						Monitor.Exit(this);

						aggregationCallback();
					}
					else
					{
#if DEBUG_TrackRemovedControllers
						if (owner.removedControllers.ContainsKey(aggregationID))
						{
							owner.logger.Log(this, "__CheckCompletion: Aggregation for " + aggregationID.ToString() +
								" is complete, but this controller has already been removed: an earlier aggregation must have completed.");
							Monitor.Exit(this);
							return;
						}
#endif

						Monitor.Exit(this);

#if DEBUG_LogUnmatched
						throw new Exception("Aggregation for " + aggregationID.ToString() +
							" is complete, but neither outgoing address nor a completion callback have been provided. Incoming addresses: " +
							Helpers.CollectionHelper.ToStringSeparated<QS._core_c_.Base3.InstanceID>(incomingAddresses, ", ") + ".");
#endif
					}

#if DEBUG_AggregationAgent
                    owner.logger.Log(this, "__removing: " + aggregationID.ToString());
#endif

					owner.removeController(aggregationID);
				}
				else
					Monitor.Exit(this);
			}

			public void Submit(QS.Fx.Serialization.ISerializable data, Aggregation3_.IAggregatable toAggregate)
			{
#if DEBUG_AggregationAgent
                owner.logger.Log(this, "__[" + aggregationID.ToString() + "].Submit");
#endif

				Monitor.Enter(this);
				if (!submitted)
				{
					submitted = true;
					addAggregatable(toAggregate);
					this.CheckCompletion();
				}
				else
					Monitor.Exit(this);
			}

			public void acknowledged(QS._core_c_.Base3.InstanceID instanceID, Aggregation3_.IAggregatable toAggregate)
			{
#if DEBUG_AggregationAgent
                owner.logger.Log(this, "__[" + aggregationID.ToString() + "].Acknowledged: " + instanceID.ToString());
#endif

				Monitor.Enter(this);

				addAggregatable(toAggregate);

                bool shouldCheck = false;

				if (!acknowledgementsCollected)
				{
                    if (incomingAddresses.Remove(instanceID))
                    {
                        if (incomingAddresses.Count == 0)
                        {
#if DEBUG_AggregationAgent
                    owner.logger.Log(this, "All incoming acknowledgements.");
#endif
                            acknowledgementsCollected = true;
                            shouldCheck = true;
                        }
                    }
                    else
                    {
                        if (unmatchedAcknowledgements == null)
                            unmatchedAcknowledgements = new List<QS._core_c_.Base3.InstanceID>();
                        unmatchedAcknowledgements.Add(instanceID);
                    }
				}

                if (shouldCheck)
                    this.CheckCompletion();
				else
					Monitor.Exit(this);
			}
        }

        #endregion

        #region GetController

        private AggregationController GetController(AggregationID aggregationID)
        {
            if (!activeControllers.ContainsKey(aggregationID))
            {
                IAggregationClass aggregationClass = 
					registeredClasses[((Base3_.IKnownClass) aggregationID.AggregationKey).ClassID];

                Routing_2_.IStructure<QS._core_c_.Base3.InstanceID> routingStructure;
                Failure_.ISource failureSource;
                if (aggregationClass.resolve(aggregationID.AggregationKey, out routingStructure, out failureSource))
                {
                    /*
                                    IGossipController gossipingController = gossipingControllerClass.CreateController(instanceID,
                                            routingStructure.Incoming(instanceID, rootAddress), routingStructure.Outgoing(instanceID, rootAddress));
                    */

                    AggregationController aggregationController = new AggregationController(this, aggregationID, routingStructure, failureSource);
                    activeControllers[aggregationID] = aggregationController;

                    return aggregationController;
                }
                else
                    return null;
            }
            else
            {
                return activeControllers[aggregationID];
            }
        }

        #endregion

        #region IAggregationAgent Members

        public void registerClass(IAggregationClass aggregationClass)
        {
            registeredClasses[aggregationClass.AssociatedClass] = aggregationClass;
        }

        public IAggregationController aggregate(IAggregationKey aggregationKey, AggregationCallback aggregationCallback)
        {
			AggregationController aggregationController;

#if DEBUG_DeadlockDetection
			lockingLog.Add(Debugging.Locking.Operation.LOCK, "AggregationAgent.aggregate");
			Debugging.Locking.Check(this, logger, lockingLog);
#endif
			lock (activeControllers) // thread #2 cannot lock
			{
                aggregationController = this.GetController(new AggregationID(aggregationKey, instanceID));
			}

#if DEBUG_DeadlockDetection
			lockingLog.Add(Debugging.Locking.Operation.UNLOCK, "AggregationAgent.aggregate");
#endif

            if (aggregationController != null)
            {
                aggregationController.AggregationCallback = aggregationCallback;
                return aggregationController;
            }
            else
            {
#if DEBUG_ControllerNotFoundWarning
                logger.Log(this, "__Aggregate: Cannot find controller for {" + aggregationKey.ToString() + "}.");
#endif
                return null;
            }
		}

		public void submit(IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress, QS.Fx.Serialization.ISerializable data, 
			Aggregation3_.IAggregatable toAggregate)
		{
#if DEBUG_DeadlockDetection
			lockingLog.Add(Debugging.Locking.Operation.LOCK, "AggregationAgent.submit");
			Debugging.Locking.Check(this, logger, lockingLog);
#endif

			AggregationController aggregationController;
			lock (activeControllers)
			{
				aggregationController = this.GetController(new AggregationID(aggregationKey, rootAddress));
			}

            if (aggregationController != null)
            {
                aggregationController.Submit(data, toAggregate);
            }
            else
            {
#if DEBUG_ControllerNotFoundWarning
                logger.Log(this, "__Submit: Cannot find controller for {" + aggregationKey.ToString() + "}.");
#endif
            }

#if DEBUG_DeadlockDetection
			lockingLog.Add(Debugging.Locking.Operation.UNLOCK, "AggregationAgent.submit");
#endif
		}

		public void submit(IAggregationKey aggregationKey, QS._core_c_.Base3.InstanceID rootAddress)
        {
			submit(aggregationKey, rootAddress, null, null);
		}

        #endregion

		#region Aggregation3.IAgent Members

		QS._qss_c_.Aggregation3_.IGroup QS._qss_c_.Aggregation3_.IAgent.GetGroup(QS._qss_c_.Aggregation3_.IGroupID aggregationGroupID)
		{
			Base3_.ViewID viewID = aggregationGroupID as Base3_.ViewID;
			if (viewID != null)
				return new _Aggregation3Group(this, viewID);
			else
				throw new Exception("This aggregation agent does not support this type of group.");
		}

		#endregion

		#region Class _Aggregation3Group

		private class _Aggregation3Group : Aggregation3_.IGroup
		{
			public _Aggregation3Group(AggregationAgent owner, Base3_.ViewID viewID)
			{
				this.owner = owner;
				this.viewID = viewID;
			}

			private AggregationAgent owner;
			private Base3_.ViewID viewID;

			#region IGroup Members

			QS._qss_c_.Aggregation3_.IChannel QS._qss_c_.Aggregation3_.IGroup.MyChannel
			{
				get { return new Channel(this, owner.instanceID); }
			}

			QS._qss_c_.Aggregation3_.IChannel QS._qss_c_.Aggregation3_.IGroup.GetChannel(QS._core_c_.Base3.InstanceID rootAddress)
			{
				return new Channel(this, rootAddress);
			}

			QS._qss_c_.Aggregation3_.IGroupID QS._qss_c_.Aggregation3_.IGroup.GroupID
			{
				get { return viewID; }
			}

			#endregion

			#region Class Channel

			private class Channel : Aggregation3_.IChannel
			{
				public Channel(_Aggregation3Group owner, QS._core_c_.Base3.InstanceID address)
				{
					this.owner = owner;
					this.address = address;
				}

				private _Aggregation3Group owner;
				private QS._core_c_.Base3.InstanceID address;

				#region IChannel Members

				QS._qss_c_.Aggregation3_.IController QS._qss_c_.Aggregation3_.IChannel.GetController(int seqno)
				{
					return new Controller(this, seqno);
				}

				QS._qss_c_.Aggregation3_.ChannelID QS._qss_c_.Aggregation3_.IChannel.ChannelID
				{
					get { return new Aggregation3_.ChannelID(owner.viewID, address); }
				}

				#endregion

				#region Class Controller

				private class Controller : Multicasting3.MessageID, Aggregation3_.IController
				{
					public Controller(Channel owner, int seqno) 
						: base(owner.owner.viewID.GroupID, owner.owner.viewID.ViewSeqNo, (uint) seqno)
					{
						this.owner = owner;
					}

					private Channel owner;

					#region IController Members

					QS._qss_c_.Aggregation3_.IChannel QS._qss_c_.Aggregation3_.IController.Channel
					{
						get { return owner; }
					}

					int QS._qss_c_.Aggregation3_.IController.SeqNo
					{
						get { return (int) this.MessageSeqNo; }
					}

					IAsyncResult QS._qss_c_.Aggregation3_.IController.BeginAggregate(AsyncCallback completionCallback, object asynchronousState)
					{
						IAggregationController aggregationController = 
							owner.owner.owner.aggregate(this, new AggregationCallback(
								delegate { completionCallback(new Base3_.PlaceholderAsyncResult(asynchronousState)); }));

						return null;
					}

					void QS._qss_c_.Aggregation3_.IController.Submit(QS.Fx.Serialization.ISerializable data, QS._qss_c_.Aggregation3_.IAggregatable toAggregate)
					{
						owner.owner.owner.submit(this, owner.address, data, toAggregate);
					}

					QS._qss_c_.Aggregation3_.IAggregatable QS._qss_c_.Aggregation3_.IController.AggregatedObject
					{
						get { return owner.owner.owner.activeControllers[new AggregationID(this, owner.address)].AggregatedObject; }
					}

					#endregion
				}

				#endregion
			}

			#endregion
		}

		#endregion
	}
}
