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
using System.Threading;

namespace QS._qss_c_.VS4
{
	/// <summary>
	/// Summary description for ViewController.
	/// </summary>
/*	/// 
	public class StandardViewController : IViewController
	{
		private const uint defaultAnticipatedNumberOfFailureNotificationConsumers = 5;
		private const uint defaultAnticipatedNumberOfGroups = 100;
		private const uint defaultAnticipatedNumberOfConcurrentViews = 3;

		public StandardViewController()
		{
			groups = new Collections.LinkableHashSet(defaultAnticipatedNumberOfGroups);
			failureNotificationConsumers = new System.Collections.ArrayList((int) defaultAnticipatedNumberOfFailureNotificationConsumers);
			broadcastFailureNotificationCallback = new VSFailureNotificationCallback(this.broadcastFailureNotification);
		}

		private IIncomingMessageContainerFactory incomingMessageContainerFactory;
		private QS.GMS.ViewChangeGoAhead installNowCallback;
		private QS.CMS.VS4.GroupReadyForCleanup cleanupReadyCallback;
		private Collections.ILinkableHashSet groups;
		private VSFailureNotificationCallback broadcastFailureNotificationCallback;
		private System.Collections.ArrayList failureNotificationConsumers;

		#region Group Members

		private class Group : Collections.GenericLinkable
		{
			public Group(GMS.GroupId groupID)
			{
				this.groupID = groupID;
				views = new Collections.LinkableHashSet(defaultAnticipatedNumberOfConcurrentViews);
			}

			private GMS.GroupId groupID; 
			private Collections.ILinkableHashSet views;
			private View currentView = null;

			#region GenericLinkable Members

			public override object Contents
			{
				get
				{
					return groupID;
				}
			}

			#endregion

			#region Helpers

			private View lookupView(uint seqNo, bool mustExist)
			{
				View view = null;
				lock (views)
				{
					view = (View) views.lookup(seqNo);
					if (view != null)
						Monitor.Enter(view);
					else if (mustExist)
						throw new Exception("could not find the requested view in group");
				}

				return view;
			}

			#endregion

			#region Processing of Outgoing Messages

			public void registerOutgoingMessage(ref uint viewSeqNo, ref uint withinViewSeqNo)
			{
				if (currentView != null)
				{
					viewSeqNo = currentView.SeqNo;
					lock (currentView)
					{
						withinViewSeqNo = currentView.NextRequestSeqNo;
					}
				}
				else
					throw new Exception("no views are currently installed in this group");
			}

			public void acknowledgedRangeOfMessages(uint viewSeqNo, uint maximumAcknowledgedSeqNo)
			{
				View view = this.lookupView(viewSeqNo, true);
				view.acknowledged(maximumAcknowledgedSeqNo);
				Monitor.Exit(view);
			}

			#endregion

			#region Processing of View Changes

			public void requestViewChange(QS.GMS.IView membershipView, VSFailureNotificationCallback broadcastFailureNotificationCallback)
			{
				View previousView = currentView;
				currentView = new View(membershipView);
				views.insert(currentView);

				if (previousView != null)
				{
					Collections.ISet liveAddresses = null, deadAddresses = null;
					previousView.identifyAliveAndCrashedNodes(currentView, ref liveAddresses, ref deadAddresses);

					
					
					
					
					// broadcastFailureNotificationCallback(this.groupID, ; ........................




				}				


				// ...............................
			}

			#endregion

			#region View Members

			public class View : Collections.GenericLinkable
			{
				public View(GMS.IView membershipView)
				{
					this.membershipView = membershipView;
				}

				private GMS.IView membershipView;
				private uint nextSeqNo = 1;
				private uint maximumAcknowledgedSeqNo = 0;

				#region Processing of Acknowledgements

				public void acknowledged(uint maximumAcknowledgedSeqNo)
				{
					if (maximumAcknowledgedSeqNo > this.maximumAcknowledgedSeqNo)
					{
						this.maximumAcknowledgedSeqNo = maximumAcknowledgedSeqNo;

						// .............. we should do something with it
					}
				}

				#endregion

				#region Crashing

				public void identifyAliveAndCrashedNodes(View newView, ref Collections.ISet liveNodes, ref Collections.ISet deadNodes)
				{
					uint ancitipatedSetSize = (uint) this.membershipView.NumberOfSubViews;
					deadNodes = new Collections.HashSet(ancitipatedSetSize);
					liveNodes = new Collections.HashSet(ancitipatedSetSize);

					for (uint ind = 0; ind < membershipView.NumberOfSubViews; ind++)
					{
						QS.Fx.Network.NetworkAddress networkAddress = 
							new QS.Fx.Network.NetworkAddress(membershipView[ind].IPAddress, membershipView[ind].PortNumber);
						deadNodes.insert(networkAddress);
					}

					for (uint ind = 0; ind < newView.membershipView.NumberOfSubViews; ind++)				
					{
						QS.Fx.Network.NetworkAddress networkAddress = 
							new QS.Fx.Network.NetworkAddress(newView.membershipView[ind].IPAddress, newView.membershipView[ind].PortNumber);
						QS.Fx.Network.NetworkAddress removedAddress = (QS.Fx.Network.NetworkAddress) deadNodes.remove(networkAddress);					
						if (removedAddress != null)
							liveNodes.insert(removedAddress);
					}
				}			

				#endregion

				#region GenericLinkable Members

				public override object Contents
				{
					get
					{
						return membershipView.SeqNo;
					}
				}

				#endregion

				#region Accessors

				public uint SeqNo
				{
					get
					{
						return membershipView.SeqNo;
					}
				}

				public uint NextRequestSeqNo
				{
					get
					{
						return nextSeqNo++;
					}
				}

				#endregion
			}

			#endregion
		}

		#endregion

		#region Helpers

		private Group lookupOrCreateGroup(GMS.GroupId groupID, bool creationAllowed)
		{
			Group group = null;
			lock (groups)
			{
				group = (Group) groups.lookup(groupID);
				if (group == null)
				{
					if (creationAllowed)
					{
						group = new Group(groupID);
						groups.insert(group);
					}
					else
						throw new Exception("group does not exist");
				}

				Monitor.Enter(group);
			}

			return group;
		}

		#endregion

		#region IVSViewChangeSynchronizer Members

		public void requestViewChange(QS.GMS.GroupId gid, QS.GMS.IView view)
		{
			Group group = this.lookupOrCreateGroup(gid, true);
			
			group.requestViewChange(view, this.broadcastFailureNotificationCallback);

			// .................................................................

			Monitor.Exit(group);
		}

		public void installmentComplete(QS.GMS.GroupId gid, uint seqno)
		{
			// TODO:  Add ViewController.installmentComplete implementation
		}

		public void prepareSmoothCleanup(QS.GMS.GroupId gid)
		{
			// TODO:  Add ViewController.prepareSmoothCleanup implementation
		}

		public void cleanupGroup(QS.GMS.GroupId gid)
		{
			// TODO:  Add ViewController.cleanupGroup implementation
		}

		public void registerGMSCallbacks(QS.GMS.ViewChangeGoAhead installNowCallback, QS.CMS.VS4.GroupReadyForCleanup cleanupReadyCallback)
		{
			this.installNowCallback = installNowCallback;
			this.cleanupReadyCallback = cleanupReadyCallback;
		}

		#endregion

		#region Processing of Failure Notifications
		
		private void broadcastFailureNotification(GMS.GroupId groupID, uint viewSeqNo, uint[] indexesOfFailedNodesWithinView)
		{
			lock (failureNotificationConsumers)
			{
				foreach (FailureNotificationRegistration registration in failureNotificationConsumers)
				{
					registration.Consumer.processFailureWithinViewNotification(groupID, viewSeqNo, indexesOfFailedNodesWithinView);
				}
			}
		}

		#region FailureNotificationRegistration

		private class FailureNotificationRegistration : IFailureNotificationRegistrationRef
		{
			public FailureNotificationRegistration(StandardViewController encapsulatingViewController, 
				IVSFailureNotificationConsumer failureNotificationConsumer)
			{
				this.encapsulatingViewController = encapsulatingViewController;
				this.failureNotificationConsumer = failureNotificationConsumer;
			}

			private StandardViewController encapsulatingViewController;
			private IVSFailureNotificationConsumer failureNotificationConsumer;

			public IVSFailureNotificationConsumer Consumer
			{
				get
				{
					return failureNotificationConsumer;
				}
			}

			#region IFailureNotificationRegistrationRef Members

			public void discontinue()
			{
				encapsulatingViewController.unregisterFailureNotificationConsumer(this);
			}

			#endregion
		}

		#endregion

		private void unregisterFailureNotificationConsumer(IFailureNotificationRegistrationRef failureNotificationRegistrationRef)
		{
			lock (this.failureNotificationConsumers)
			{
				this.failureNotificationConsumers.Remove(failureNotificationRegistrationRef);
			}
		}

		#region IVSFailureNotificationProvider Members

		public IFailureNotificationRegistrationRef registerFailureNotificationConsumer(IVSFailureNotificationConsumer failureNotificationConsumer)
		{
			IFailureNotificationRegistrationRef registrationRef = null;
			lock (this.failureNotificationConsumers)
			{
				registrationRef = new FailureNotificationRegistration(this, failureNotificationConsumer);
				this.failureNotificationConsumers.Add(registrationRef);
			}

			return registrationRef;
		}

		#endregion

		#endregion

		#region IVSSendingController Members

		public void registerOutgoingMessage(
			GMS.GroupId groupID, out IMembershipViewRef membershipViewRef, out uint withinViewSeqNo)
		{

			// ...................................

			membershipViewRef = null;
			withinViewSeqNo = 666;


//			Group group = this.lookupOrCreateGroup(groupID, false);
//			group.registerOutgoingMessage(ref viewSeqNo, ref withinViewSeqNo);
//			Monitor.Exit(group);
			
		}

		public void acknowledgedRangeOfMessages(GMS.GroupId groupID, uint viewSeqNo, uint maximumAcknowledgedSeqNo)
		{
			Group group = this.lookupOrCreateGroup(groupID, false);
			group.acknowledgedRangeOfMessages(viewSeqNo, maximumAcknowledgedSeqNo);
			Monitor.Exit(group);
		}

		#endregion

		#region IVSReceivingController Members

		public IIncomingMessageContainerFactory IncomingMessageContainerFactory
		{
			set
			{
				this.incomingMessageContainerFactory = value;
			}
		}

		#endregion
	}
*/
}
