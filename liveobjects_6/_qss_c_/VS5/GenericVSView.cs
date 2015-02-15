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

// #define DEBUG_GenericVSView

using System;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_c_.VS5
{
	/// <summary>
	/// Summary description for VSView.
	/// </summary>
/*
	public abstract class GenericVSView : Collections.GenericBinaryTreeNode, IVSView
	{
		public static void register_serializable()
		{
			Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.FlushingReport, typeof(GenericVSView.GenericFlushingReport));
			Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.ReceiverReport, typeof(GenericVSView.ReceiverReport));
		}

		public GenericVSView(GMS.IView membershipView, bool initiallySuspended, QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;

			this.membershipView = membershipView;
			this.prevView = this.nextView = null;
			crashConsumers = new Collections.BiLinkableCollection();
			this.flushing = false;

			// this.deliverySuppressed = initiallySuspended;
		}

		protected QS.Fx.Logging.ILogger logger;

		private Collections.IBiLinkableCollection crashConsumers;
		private GMS.IView membershipView;
		private IVSView prevView, nextView;
		// private bool deliverySuppressed;
		private bool flushing;
		private uint numberOfRequestsSent = 0;

		// protected abstract void deliverAllBufferedMessages();

		private FlushingState flushingState = null;

		private void suspend()
		{
			// this.deliverySuppressed = true;

			// .......................................
		}

		#region Distribution and Processing of Flushing Reports

		private class FlushingState
		{
			public FlushingState(GenericVSView view)
			{
				this.view = view;

				peers = new Collections.LinkableHashSet((uint) view.membershipView.NumberOfSubViews);
				liveAddresses = new Collections.HashSet((uint) view.membershipView.NumberOfSubViews);
				deadAddresses = new Collections.HashSet((uint) view.membershipView.NumberOfSubViews);

				pendingStatusCount = (uint) view.membershipView.NumberOfSubViews;

				for (uint ind = 0; ind < view.membershipView.NumberOfSubViews; ind++)
				{
					QS.Fx.Network.NetworkAddress address = GenericVSView.SubView2NetworkAddress(view.membershipView[ind]);
					liveAddresses.insert(address);
					peers.insert(new PeerInfo(address));
				}
			}

			private GenericVSView view;

			private void updatePendingCount(bool readyBefore, bool readyAfter)
			{
/-*
				if (readyBefore)
				{
					if (!readyAfter)
						pendingStatusCount++;
				}
				else
				{
					if (readyAfter)
					{
						pendingStatusCount--;

						if (pendingStatusCount == 0)
							view.flushingStateReady();
					}
				}
*-/				
			}

			public void processCrashes(Collections.ISet networkAddresses)
			{
				foreach (QS.Fx.Network.NetworkAddress new_dead in networkAddresses.Elements)
				{
					foreach (QS.Fx.Network.NetworkAddress old_dead in deadAddresses.Elements)
					{
						PeerInfo peer = (PeerInfo) peers.lookup(old_dead);
						bool readyBefore = peer.StatusReady;
						peer.processNewCrash(new_dead);
						updatePendingCount(readyBefore, peer.StatusReady);
					}
				}
				
				foreach (QS.Fx.Network.NetworkAddress new_dead in networkAddresses.Elements)
				{
					liveAddresses.remove(new_dead);
					deadAddresses.insert(new_dead);
				}

				foreach (QS.Fx.Network.NetworkAddress new_dead in networkAddresses.Elements)
				{
					PeerInfo peer = (PeerInfo) peers.lookup(new_dead);
					bool readyBefore = peer.StatusReady;
					peer.markAsCrashed(liveAddresses);
					updatePendingCount(readyBefore, peer.StatusReady);
				}
			}

			public void insertSFR(QS.Fx.Network.NetworkAddress networkAddress, uint numberOfMessagesSent)
			{
				PeerInfo peer = (PeerInfo) peers.lookup(networkAddress);
				bool readyBefore = peer.StatusReady;
				peer.insertSFR(numberOfMessagesSent);
				updatePendingCount(readyBefore, peer.StatusReady);
			}

			public void insertRFR(QS.Fx.Network.NetworkAddress networkAddress, ReceiverReport[] receiverReports)
			{
				foreach (ReceiverReport report in receiverReports)
				{
					PeerInfo peer = (PeerInfo) peers.lookup(report.Address);										
					bool readyBefore = peer.StatusReady;
					peer.markAsCrashed(liveAddresses);
					report.Address = networkAddress;
					peer.insertRFR(report);
					updatePendingCount(readyBefore, peer.StatusReady);
				}
			}

			private Collections.ILinkableHashSet peers;
			private Collections.ISet liveAddresses, deadAddresses;
			private uint pendingStatusCount;

			#region Class PeerInfo

			#region Interface ISourceStatus

			public interface ISourceStatus
			{
				bool StatusReady
				{
					get;
				}

				IVSRCCDescriptor Descriptor
				{
					get;
				}

				QS.Fx.Network.NetworkAddress Address
				{
					get;
				}
			}

			#endregion

			public class PeerInfo : Collections.GenericLinkable, ISourceStatus
			{
				public PeerInfo(QS.Fx.Network.NetworkAddress peerAddress)
				{
					this.peerAddress = peerAddress;
					crashed = numberOfMessagesSentKnown = readyAsCrashed = false;
					liveAwaiting = receiverReports = null;
					senderRCCDescriptor = new GenericRCCDescriptor();
				}

				private QS.Fx.Network.NetworkAddress peerAddress;
				private bool crashed, numberOfMessagesSentKnown, readyAsCrashed;
				private Collections.ISet liveAwaiting, receiverReports;		
				private IVSRCCDescriptor aggregateRCCDescriptor = null;
				private GenericRCCDescriptor senderRCCDescriptor;

				#region ISourceStatus Members

				public bool StatusReady
				{
					get
					{
						return crashed && readyAsCrashed || !crashed && numberOfMessagesSentKnown;
					}
				}

				public IVSRCCDescriptor Descriptor
				{
					get
					{
						return crashed ? aggregateRCCDescriptor : senderRCCDescriptor;
					}
				}

				public QS.Fx.Network.NetworkAddress Address
				{
					get
					{
						return peerAddress;
					}
				}

				#endregion

				public void markAsCrashed(Collections.ISet liveAddresses)
				{
					if (!crashed)
					{
						crashed = true;

						liveAwaiting = new Collections.HashSet(liveAddresses.Count);
						foreach (QS.Fx.Network.NetworkAddress address in liveAddresses.Elements)
							liveAwaiting.insert(address);
						receiverReports = new Collections.HashSet(liveAddresses.Count);
					}
				}

				public void processNewCrash(QS.Fx.Network.NetworkAddress new_dead)
				{
					if (!crashed)
						throw new Exception("Peer was expected to be marked as crashed already!");

					if (receiverReports.contains(new_dead))
					{
						receiverReports.remove(new_dead);
						if (readyAsCrashed)
							calculateAggregate();
					}

					if (liveAwaiting.contains(new_dead))
					{
						liveAwaiting.remove(new_dead);
						if (liveAwaiting.Count == 0)
						{
							calculateAggregate();
							readyAsCrashed = true;
						}
					}

				}

				public void insertSFR(uint numberOfMessagesSent)
				{
					if (!crashed && !numberOfMessagesSentKnown)
					{
						senderRCCDescriptor.NumberOfMessages = numberOfMessagesSent;
						numberOfMessagesSentKnown = true;
					}
				}

				public void insertRFR(ReceiverReport report)
				{
					if (!crashed)
						throw new Exception("Peer was expected to be marked as crashed already!");
					liveAwaiting.remove(report.Address);
					receiverReports.insert(report);
					if (liveAwaiting.Count == 0)
					{
						calculateAggregate();
						readyAsCrashed = true;
					}
				}

				private void calculateAggregate()
				{
					IVSRCCDescriptor[] descriptors = 
						(IVSRCCDescriptor[]) receiverReports.ToArray(typeof(IVSRCCDescriptor));
					aggregateRCCDescriptor = descriptors[0].aggregateWith(descriptors);
				}

				#region Overrides

				public override object Contents
				{
					get
					{
						return peerAddress;
					}
				}

				#endregion
			}
			
			#endregion
		}

		protected void processFlushingReportOfASender(QS.Fx.Network.NetworkAddress networkAddress, uint numberOfMessagesSent)
		{
			if (flushingState == null)
				flushingState = new FlushingState(this);

			flushingState.insertSFR(networkAddress, numberOfMessagesSent);

			// .......................................
		}

		protected void processFlushingReportOfAReceiver(
			QS.Fx.Network.NetworkAddress networkAddress, ReceiverReport[] receiverReports)
		{
			if (flushingState == null)
				flushingState = new FlushingState(this);

			flushingState.insertRFR(networkAddress, receiverReports);

			// .......................................
		}

		protected abstract IVSRCCDescriptor createRCCDescriptor(QS.Fx.Network.NetworkAddress sourceAddress);

		protected abstract void distributeFlushingReport(bool initial, uint numberOfMessagesSent, 
			ReceiverReport[] receiverReports, Scattering.IScatterSet destinationSet);

		#region Class ReceiverReport

		[QS.Fx.Serialization.ClassID(QS.ClassID.ReceiverReport)]
		public class ReceiverReport : Collections.GenericLinkable, Base2.IBase2Serializable
		{
			public ReceiverReport()
			{
			}

			public ReceiverReport(QS.Fx.Network.NetworkAddress address, IVSRCCDescriptor associatedRCCDescriptor)
			{
				this.address = address;
				this.associatedRCCDescriptor = associatedRCCDescriptor;
			}

			private QS.Fx.Network.NetworkAddress address;
			private IVSRCCDescriptor associatedRCCDescriptor;

			public QS.Fx.Network.NetworkAddress Address
			{
				set
				{
					address = value;
				}

				get
				{
					return address;
				}
			}

			public IVSRCCDescriptor RCCDescriptor
			{
				get
				{
					return this.associatedRCCDescriptor;
				}
			}

			#region ISerializable Members

			public uint Size
			{
				get
				{
					return address.Size + Base2.SizeOf.UInt16 + associatedRCCDescriptor.Size;
				}
			}

			public void load(QS.CMS.Base2.IBlockOfData blockOfData)
			{
				address = new QS.Fx.Network.NetworkAddress();
				((Base2.IBase2Serializable) address).load(blockOfData);
				QS.ClassID classID = (QS.ClassID) Base2.Serializer.loadUInt16(blockOfData);
				associatedRCCDescriptor = (IVSRCCDescriptor) Base2.Serializer.CommonSerializer.CreateObject(classID);				
				associatedRCCDescriptor.load(blockOfData);
			}

			public void save(QS.CMS.Base2.IBlockOfData blockOfData)
			{
				((Base2.IBase2Serializable) address).save(blockOfData);
				Base2.Serializer.saveUInt16((ushort) associatedRCCDescriptor.ClassID, blockOfData);
				associatedRCCDescriptor.save(blockOfData);
			}

			public QS.ClassID ClassID
			{
				get
				{
					return ClassID.ReceiverReport;
				}
			}

			#endregion

			public override object Contents
			{
				get
				{
					return address;
				}
			}

			public override string ToString()
			{
				return "RR(" + address.ToString() + " " + associatedRCCDescriptor.ToString() + ")";
			}
		}

		#endregion

		#region Class GenericFlushingReport

		[QS.Fx.Serialization.ClassID(QS.ClassID.FlushingReport)]
		public class GenericFlushingReport : Base2.IBase2Serializable
		{
			public GenericFlushingReport()
			{
			}

			public GenericFlushingReport(GMS.GroupId groupID, uint viewSeqNo, bool initial, uint numberOfMessagesSent, 
				ReceiverReport[] receiverReports)
			{
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.initial = initial;
				this.numberOfMessagesSent = numberOfMessagesSent;
				this.receiverReports = receiverReports;
			}
			
			private GMS.GroupId groupID; 
			private bool initial;
			private uint viewSeqNo, numberOfMessagesSent;
			private ReceiverReport[] receiverReports;

			#region Accessors

			public GMS.GroupId GroupID
			{
				get
				{
					return groupID;
				}
			}

			public bool Initial
			{
				get
				{
					return initial;
				}
			}

			public uint ViewSeqNo
			{
				get
				{
					return viewSeqNo;
				}
			}

			public uint NumberOfMessagesSent
			{
				get
				{
					return numberOfMessagesSent;
				}
			}

			public ReceiverReport[] ReceiverReports
			{
				get
				{
					return receiverReports;
				}
			}

			#endregion

			#region ISerializable Members

			public uint Size
			{
				get
				{
					return ((uint) (2 + (initial ? 1 : 0))) * Base2.SizeOf.UInt32 + Base2.SizeOf.Bool + 
						Base2.SizeOf.Int32 + 
						((receiverReports.Length > 0) ? (((uint) receiverReports.Length) * receiverReports[0].Size) : 0);
				}
			}

			public void load(QS.CMS.Base2.IBlockOfData blockOfData)
			{
				groupID = new GMS.GroupId(Base2.Serializer.loadInt32(blockOfData));
				viewSeqNo = Base2.Serializer.loadUInt32(blockOfData);
				initial = Base2.Serializer.loadBool(blockOfData);
				if (initial)
					numberOfMessagesSent = Base2.Serializer.loadUInt32(blockOfData);
				uint nreceivers = Base2.Serializer.loadUInt32(blockOfData);
				if (nreceivers > 0)
				{					
					receiverReports = new ReceiverReport[nreceivers];
					for (uint ind = 0; ind < nreceivers; ind++)
					{
						receiverReports[ind] = new ReceiverReport();
						receiverReports[ind].load(blockOfData);
					}
				}
				else
					receiverReports = null;
			}

			public void save(QS.CMS.Base2.IBlockOfData blockOfData)
			{
				Base2.Serializer.saveInt32(groupID.ID, blockOfData);
				Base2.Serializer.saveUInt32(viewSeqNo, blockOfData);
				Base2.Serializer.saveBool(initial, blockOfData);
				if (initial)
					Base2.Serializer.saveUInt32(numberOfMessagesSent, blockOfData);
				Base2.Serializer.saveUInt32((uint) receiverReports.Length, blockOfData);
				foreach (ReceiverReport report in receiverReports)
					report.save(blockOfData);
			}

			public QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.FlushingReport;
				}
			}

			#endregion

			public override string ToString()
			{
				string receiverReports_AsString = "";
				foreach (ReceiverReport report in receiverReports)
					receiverReports_AsString += "  " + report.ToString() + "\n";
				return "\nFlushingReport(\n  " + groupID.ToString() + ":" + viewSeqNo.ToString() + 
					(initial ? "/I" : "/U") + ", sent: " + numberOfMessagesSent.ToString() + ", received:\n" +
					receiverReports_AsString + ")";
			}
		}

		#endregion

		#endregion

		#region Processing after Crashes

		#region IVSCrashNotificationProvider Members

		public void signUpConsumer(IVSCrashNotificationConsumer consumer)
		{
			crashConsumers.insertAtTail(consumer);
		}

		public void removeConsumer(IVSCrashNotificationConsumer consumer)
		{
			crashConsumers.remove(consumer);
		}

		#endregion

		#region Identifying Alive and Crashed Nodes

		public static void IdentifyAliveAndCrashedNodes(GMS.IView oldMembershipView, GMS.IView newMembershipView, 
			out Collections.ISet networkAddressesOfLiveNodes, out Collections.ISet networkAddressesOfDeadNodes)
		{
			uint ancitipatedSetSize = (uint) oldMembershipView.NumberOfSubViews;
			networkAddressesOfDeadNodes = new Collections.HashSet(ancitipatedSetSize);
			networkAddressesOfLiveNodes = new Collections.HashSet(ancitipatedSetSize);

			for (uint ind = 0; ind < oldMembershipView.NumberOfSubViews; ind++)
			{
				networkAddressesOfDeadNodes.insert(
					new QS.Fx.Network.NetworkAddress(oldMembershipView[ind].IPAddress, oldMembershipView[ind].PortNumber));
			}

			for (uint ind = 0; ind < newMembershipView.NumberOfSubViews; ind++)				
			{
				QS.Fx.Network.NetworkAddress removedAddress = (QS.Fx.Network.NetworkAddress) networkAddressesOfDeadNodes.remove(
					new QS.Fx.Network.NetworkAddress(newMembershipView[ind].IPAddress, newMembershipView[ind].PortNumber));					
				if (removedAddress != null)
					networkAddressesOfLiveNodes.insert(removedAddress);
			}
		}			

		#endregion

		#endregion

		#region IVSView Members

		public void processCrashes(Collections.ISet deadAddresses)
		{
			if (flushingState == null)
				flushingState = new FlushingState(this);

			flushingState.processCrashes(deadAddresses);

			foreach (QS.Fx.Network.NetworkAddress deadAddress in deadAddresses.Elements)
			{
#if DEBUG_GenericVSView
				logger.Log(this, "_processCrashes " + this.membershipView.SeqNo.ToString() + " : " +
					deadAddress.ToString());
#endif

				foreach (IVSCrashNotificationConsumer crashConsumer in crashConsumers)
				{
					crashConsumer.consumeCrashNotification(deadAddress);
				}
			}

			if (prevView != null)
			{
				lock (prevView)
				{
					Collections.ISet carriedOver = new Collections.HashSet(deadAddresses.Count);
					for (uint ind = 0; ind < prevView.MembershipView.NumberOfSubViews; ind++)
					{
						QS.Fx.Network.NetworkAddress networkAddress = 
							GenericVSView.SubView2NetworkAddress(prevView.MembershipView[ind]);
						if (deadAddresses.contains(networkAddress))
							carriedOver.insert(networkAddress);
					}

					if (carriedOver.Count > 0)
						prevView.processCrashes(carriedOver);
				}
			}

			GenericVSScatterSet survivingScatterSet = new GenericVSScatterSet(this);
			ReceiverReport[] receiverReports = new ReceiverReport[deadAddresses.Count];
			uint index = 0;
			foreach (QS.Fx.Network.NetworkAddress sourceAddress in deadAddresses.Elements)
			{
				survivingScatterSet.consumeCrashNotification(sourceAddress);
				receiverReports[index++] = new ReceiverReport(sourceAddress, this.createRCCDescriptor(sourceAddress));
			}

			this.distributeFlushingReport(!flushing, this.numberOfRequestsSent, receiverReports, survivingScatterSet);

			if (!flushing)
			{
				flushing = true;
				this.suspend();
			}

			// ..........................................


		}

		public virtual void receive(QS.Fx.Network.NetworkAddress sourceAddress, IVSMessage message)
		{
			Monitor.Exit(this);
			throw new Exception("not implemented");
		}

		public GMS.IView MembershipView
		{
			get
			{
				return membershipView;
			}
		}

		public uint NextMessageSeqNo
		{
			get
			{
				return ++numberOfRequestsSent;
			}
		}

		public IVSView PrevVSView			           
		{
			set
			{
				prevView = value;
			}

			get
			{
				return prevView;
			}
		}

		public IVSView NextVSView			           
		{
			set
			{
				nextView = value;
			}

			get
			{
				return nextView;
			}
		}

		#endregion

		#region Collections.GenericBinaryTreeNode Overrides

		public override System.IComparable Contents
		{
			get
			{
				return membershipView.SeqNo;
			}
		}

		#endregion

		#region Helpers

		public static QS.Fx.Network.NetworkAddress SubView2NetworkAddress(GMS.ISubView subView)
		{
			return new QS.Fx.Network.NetworkAddress(subView.IPAddress, subView.PortNumber);
		}

		#endregion
	}
*/ 
}
