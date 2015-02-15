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

#define DEBUG_SimpleVC2
#define DEBUG_OnePhase_PassThrough
#define DEBUG_Disable_IPMulticast

using System;
using System.Threading;

namespace QS._qss_c_.VS5
{
/*
	/// <summary>
	/// Summary description for SimpleVC2.
	/// </summary>
	public class SimpleVC2 : GenericVSViewController
	{
		private const uint defaultWindowSize = 1000;

		public SimpleVC2(QS.Fx.Logging.ILogger logger, Base2.IMasterIOC masterIOC, 
			Scattering.IRetransmittingScatterer retransmittingScatterer, Base2.IDemultiplexer demultiplexer,
			Allocation.IAllocationClient allocationClient, 
            QS.Fx.Network.NetworkAddress localAddress, Devices2.ICommunicationsDevice multicastingDevice, Base2.RootSender rootSender) 
			: base(logger, demultiplexer)
		{           
			Base2.Serializer.CommonSerializer.registerClass(QS.ClassID.GenericVSMPPMID, typeof(GenericVSMPPMID));

            this.localAddress = localAddress;
            this.multicastingDevice = multicastingDevice;
            this.rootSender = rootSender;

            this.allocationClient = allocationClient;
			this.retransmittingScatterer = retransmittingScatterer;
			demultiplexer.register((uint) ReservedObjectID.ViewController_FlushingChannel, 
				new Base2.ReceiveCallback(this.flushingReceiveCallback));

			masterIOC.registerContainer(QS.CMS.Base2.ContainerClass.VSViewController, this.OutgoingIOC);
		}

		private Scattering.IRetransmittingScatterer retransmittingScatterer;
		private Allocation.IAllocationClient allocationClient;

        private QS.Fx.Network.NetworkAddress localAddress;
        private Devices2.ICommunicationsDevice multicastingDevice;
        private Base2.RootSender rootSender;

		#region Processing of Flushing Reports

		private QS.CMS.Base2.IBase2Serializable flushingReceiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable serializableObject)
		{
#if DEBUG_SimpleVC2
			logger.Log(this, "FlushingReceiveCallback : from " + sourceAddress.ToString() + ", messsage " +
				serializableObject.ToString());
#endif

			GenericVSView.GenericFlushingReport flushingReport = (GenericVSView.GenericFlushingReport) 
				((Components.Sequencer.IWrappedObject) serializableObject).SerializableObject;

			Group group = (Group) this.lookupGroup(flushingReport.GroupID);
			if (group != null)
				group.processFlushingReport(sourceAddress, flushingReport);	
		
			return null;
		}

		#endregion

		protected override IVSGroup createGroup(GMS.GroupId groupID, GMS.IView membershipView, QS.Fx.Logging.ILogger logger)
		{
			return new Group(groupID, membershipView, this, logger);
		}

		#region Class Group

		private class Group : GenericVSGroup
		{
			public Group(GMS.GroupId groupID, GMS.IView membershipView, SimpleVC2 encapsulatingVC, 
				QS.Fx.Logging.ILogger logger) : base(groupID, membershipView, logger)
			{
				this.encapsulatingVC = encapsulatingVC;
				outgoingQueue = new Collections.RawQueue();

				// for now, let just everybody do this
				if (encapsulatingVC.allocationClient != null)
				{
					encapsulatingVC.allocationClient.allocate(groupID, 
						new Allocation.AllocationCallback(this.allocationCallback));
				}
			}

			public SimpleVC2 encapsulatingVC;
			private FlowControl.IOutgoingWindow outgoingWindow = null;
			private Collections.IRawQueue outgoingQueue;

			private QS.Fx.Network.NetworkAddress allocatedGroupAddress = null;
            private Devices2.IListener listenerRef = null;

			#region Allocation Callback

			private void allocationCallback(Base2.IIdentifiableKey key, Allocation.IAllocatedObject allocatedObject)
			{
				if (!(key is GMS.GroupId && ((GMS.GroupId) key).Equals(this.groupID) && 
					allocatedObject.AllocatedObject is QS.Fx.Network.NetworkAddress))
				{
					throw new Exception("allocationCallback : bad arguments");
				}

				lock (this)
				{
					QS.Fx.Network.NetworkAddress allocatedGroupAddress = (QS.Fx.Network.NetworkAddress) allocatedObject.AllocatedObject;

                    listenerRef = encapsulatingVC.multicastingDevice.listenAt(
                        encapsulatingVC.localAddress.HostIPAddress, allocatedGroupAddress, encapsulatingVC.rootSender.ReceiveCallback);

                    // for now, just start using multicast address without waiting for other hosts
#if DEBUG_Disable_IPMulticast
#else
					this.allocatedGroupAddress = allocatedGroupAddress;
#endif

					logger.Log(this, "__allocatedGroupAddress : " + allocatedGroupAddress.ToString()); 
				}
			}

			#endregion

			#region Class OutgoingRequest

			[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
			private class OutgoingRequest : Scattering.RetransmittingScatterer.GenericRequest, Collections.ILinkable
			{
				public OutgoingRequest(GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo, 
					Base2.IBase2Serializable serializableObject, Scattering.IScatterSet scatterSet, Group associatedGroup, 
					Scattering.CompletionCallback completionCallback) 
					: base(associatedGroup.encapsulatingVC.LocalObjectID, scatterSet, new GenericVSMessage(
					new GenericVSMPPMID(groupID, viewSeqNo, withinViewSeqNo, ProtocolPhase.TRANSMITTING_DATA), 
					serializableObject), associatedGroup.encapsulatingVC.OutgoingIOC, completionCallback)
				{
					this.associatedGroup = associatedGroup;
				}

				private Group associatedGroup;			
				private uint withinGroupSeqNo;
				private OutgoingRequest next;

				#region Accessors

				public uint WithinGroupSeqNo
				{
					set
					{
						withinGroupSeqNo = value;
					}

					get
					{
						return withinGroupSeqNo;
					}
				}

				#endregion

				#region Completion Callback

				protected override void completionCallback(bool succeeded, Exception exception)
				{
#if DEBUG_SimpleVC2
//					associatedGroup.encapsulatingVC.logger.Log(this, 
//						"Callback : " + this.WrappedIdentifiableObject.UniqueID.ToString());
#endif

					GenericVSMPPMID messageID = (GenericVSMPPMID) this.WrappedIdentifiableObject.UniqueID;
					switch (messageID.Phase)
					{
						case ProtocolPhase.TRANSMITTING_DATA:
						{
							messageID.Phase = ProtocolPhase.DELIVERED_EVERYWHERE;
							
							// maybe not here yet ?
							this.externalCompletionCallback(succeeded, exception);

#if DEBUG_OnePhase_PassThrough
                            associatedGroup.completionCallback(this);
#else
							this.addressSet.reset();
							associatedGroup.encapsulatingVC.retransmittingScatterer.multicast(this);
#endif
						}
						break;

						case ProtocolPhase.DELIVERED_EVERYWHERE:
						{
							messageID.Phase = ProtocolPhase.GARBAGE_COLLECTION;

							this.addressSet.reset();
							associatedGroup.encapsulatingVC.retransmittingScatterer.multicast(this);
						}
						break;

						case ProtocolPhase.GARBAGE_COLLECTION:
						{
							associatedGroup.completionCallback(this);
						}
						break;
					}
				}

				#endregion

				public ProtocolPhase Phase
				{
					get
					{
						return ((IVSMPPMID) this.WrappedIdentifiableObject.UniqueID).Phase;
					}
				}

				#region ILinkable Members

				public QS.CMS.Collections.ILinkable Next
				{
					get
					{
						return next;
					}

					set
					{
						next = (OutgoingRequest) value;
					}
				}

				// public
				object QS.CMS.Collections.ILinkable.Contents
				{
					get
					{
						return this;
					}
				}

				#endregion
			}

			#endregion

			#region Class IncomingRequest

			#endregion		

			protected override IVSView createView(QS.GMS.IView membershipView, bool initiallySuspended, QS.Fx.Logging.ILogger logger)
			{
				if (outgoingWindow == null)
					outgoingWindow = new FlowControl.OutgoingWindow(defaultWindowSize);

				return new View(membershipView, initiallySuspended, outgoingWindow.NextAvailableSeqNo, logger, this);
			}

			private void completionCallback(OutgoingRequest request)
			{
				outgoingWindow.remove(request.WithinGroupSeqNo);
				if (outgoingWindow.hasSpace() && outgoingQueue.size() > 0)
				{
					processOutgoing((OutgoingRequest) outgoingQueue.dequeue());
				}
			}

			#region Multicasting

			private void processOutgoing(OutgoingRequest request)
			{
				request.WithinGroupSeqNo = outgoingWindow.append(request);
				encapsulatingVC.retransmittingScatterer.multicast(request);
			}

			protected override void multicastWrappedObject(GMS.GroupId groupID, uint viewSeqNo, 
				uint withinViewSeqNo, Base2.IBase2Serializable serializableObject, 
				Scattering.IScatterSet scatterSet, Scattering.CompletionCallback completionCallback)
			{
				scatterSet.ScatterAddress = this.allocatedGroupAddress;

#if DEBUG_SimpleVC2
//				scatterSet.Logger = this.logger;
#endif

				Monitor.Exit(currentView);

				OutgoingRequest request = new OutgoingRequest(groupID, viewSeqNo, withinViewSeqNo, 
					serializableObject, scatterSet, this, completionCallback);

				if (outgoingWindow.hasSpace())
				{				
					processOutgoing(request);
				}
				else
				{
					outgoingQueue.enqueue(request);
				}

				Monitor.Exit(this);
			}

			#endregion

			#region Support code for Object Container

			public override QS.CMS.Base2.IIdentifiableObject lookupOutgoing(IVSMessageID messageID)
			{
#if DEBUG_SimpleVC2
//				logger.Log(this, "lookupOutgoing_enter");
#endif

				QS.CMS.Base2.IIdentifiableObject result = null;
				View view = (View) this.lookupView(messageID.ViewSeqNo);

				if (view != null)
				{
#if DEBUG_SimpleVC2
//					logger.Log(this, 
//						"lookupOutgoing_foundView(" + messageID.ViewSeqNo.ToString() + ")");
#endif

					uint groupWinSeqNo = view.FirstGroupWinSeqNo + messageID.WithinViewSeqNo - 1;
					Monitor.Exit(view);

#if DEBUG_SimpleVC2
//					logger.Log(this, 
//						"lookupOutgoing_lookingIntoTheWindow: " + groupWinSeqNo.ToString());
#endif

					try
					{
						OutgoingRequest request = (OutgoingRequest) outgoingWindow.lookup(groupWinSeqNo);
						if (request.Phase == ((IVSMPPMID) messageID).Phase)
							result = request;
					}
// #if DEBUG_SimpleVC2
//					catch (Exception exc)
//					{
//						logger.Log(this, 
//							"lookupOutgoing_windowLookupError: " + exc.ToString());
// #else
					catch (Exception)
					{
// #endif
					}
				}
				else
					result = null;

#if DEBUG_SimpleVC2
//				logger.Log(this, "lookupOutgoing_releasingGroup");
#endif
				
				Monitor.Exit(this);

#if DEBUG_SimpleVC2
//				logger.Log(this, "lookupOutgoing_leave");
#endif

				return result;
			}

			#endregion

			#region Processing of Flushing Reports

			public void processFlushingReport(
				QS.Fx.Network.NetworkAddress sourceAddress, GenericVSView.GenericFlushingReport flushingReport)
			{
				View view = (View) this.lookupView(flushingReport.ViewSeqNo);
				if (view != null)
				{
					Monitor.Exit(this);
					view.processFlushingReport(sourceAddress, flushingReport);
				}
				else
					Monitor.Exit(this);
			}

			#endregion
		}

		#endregion		

		#region Class View

		private class View : GenericVSView
		{
			public View(GMS.IView membershipView, bool initiallySuspended, uint firstGroupWinSeqNo, 
				QS.Fx.Logging.ILogger logger, Group encapsulatingGroup) : base(membershipView, initiallySuspended, logger)
			{
				this.encapsulatingGroup = encapsulatingGroup;
				this.firstGroupWinSeqNo = firstGroupWinSeqNo;
			}

			private Group encapsulatingGroup;
			private uint firstGroupWinSeqNo;

			// private FlowControl.IIncomingWindow incomingWindow = null;

			#region Accessors

			public uint FirstGroupWinSeqNo
			{
				get
				{
					return firstGroupWinSeqNo;
				}
			}

			#endregion

			#region Processing of Received Messages

			public override void receive(QS.Fx.Network.NetworkAddress sourceAddress, IVSMessage message)
			{
#if DEBUG_SimpleVC2
//				logger.Log(this, 
//					"Receive Callback : from " + sourceAddress.ToString() + ", message : " + message.ToString());
#endif

				Monitor.Exit(this);

/-*
				if (incomingWindow == null)
					incomingWindow = new FlowControl.IncomingWindow(defaultWindowSize);

				// incomingWindow.
				incomingWindow.insert(message.ID.WithinViewSeqNo, message.WrappedObject);

				// ........................................				

				// ........................................				
*-/				

			}

			#endregion

			#region Distribution and Processing of Flushing Reports

			protected override IVSRCCDescriptor createRCCDescriptor(QS.Fx.Network.NetworkAddress sourceAddress)
			{
				// ...............................
				
				return new GenericRCCDescriptor(0);
			}

			protected override void distributeFlushingReport(bool initial, uint numberOfMessagesSent, 
				ReceiverReport[] receiverReports, Scattering.IScatterSet destinationSet)
			{
				GenericFlushingReport flushingReport = new GenericFlushingReport(encapsulatingGroup.GroupID, 
					this.MembershipView.SeqNo, initial, numberOfMessagesSent, receiverReports);

#if DEBUG_SimpleVC2
				logger.Log(this, "_distributeFlushingReport: " + flushingReport.ToString());
#endif

				encapsulatingGroup.encapsulatingVC.retransmittingScatterer.multicast(
					(uint) ReservedObjectID.ViewController_FlushingChannel, 
					destinationSet, Components.Sequencer.wrap(flushingReport), null);
			}

			public void processFlushingReport(
				QS.Fx.Network.NetworkAddress sourceAddress, GenericVSView.GenericFlushingReport flushingReport)
			{
#if DEBUG_SimpleVC2
				logger.Log(this, "View " + this.MembershipView.SeqNo + " -> ProcessFlushingReport");
#endif

				if (flushingReport.Initial)
					this.processFlushingReportOfASender(sourceAddress, flushingReport.NumberOfMessagesSent);

				this.processFlushingReportOfAReceiver(sourceAddress, flushingReport.ReceiverReports);
			}

			#endregion
		}

		#endregion
	}
*/
}
