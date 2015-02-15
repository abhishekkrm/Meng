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

#define DEBUG_ViewController

using System;
using System.Diagnostics;
using System.Threading;

namespace QS._qss_c_.VS_2_
{
/*
	/// <summary>
	/// Summary description for ViewController.
	/// </summary>
	public class ViewController : IViewController
	{
		public void registerGMSCallbacks(GMS.ViewChangeGoAhead viewChangeGoAhead)
		{
			this.viewChangeGoAhead = viewChangeGoAhead;
		}

		public void registerFlushingDevice(IFlushingDevice flushingDevice)
		{
			this.flushingDevice = flushingDevice;
		}

		private GMS.ViewChangeGoAhead viewChangeGoAhead;

		public ViewController(QS.Fx.Network.NetworkAddress localAddress, uint anticipatedNumberOfGroups, 
			uint anticipatedNumberOfOpenViewsAtATime, uint anticipatedGroupSize, uint outgoingWindowSize, uint incomingWindowSize, 
			bool enqueueingOutgoing, QS.Fx.Logging.ILogger logger)
		{
			this.localAddress = localAddress;
#if DEBUG_ViewController
			logger.Log(this, "ViewController initialized with local address " + localAddress.ToString());
#endif

			groups = new Collections.Hashtable(anticipatedNumberOfGroups);
			this.anticipatedNumberOfViews = anticipatedNumberOfOpenViewsAtATime;

			this.outgoingWindowSize = outgoingWindowSize;
			this.incomingWindowSize = incomingWindowSize;
			this.enqueueingOutgoing = enqueueingOutgoing;

			this.anticipatedGroupSize = anticipatedGroupSize;

			this.logger = logger;
		}

		private Collections.Hashtable groups;
		private uint anticipatedNumberOfViews, anticipatedGroupSize,
			outgoingWindowSize, incomingWindowSize;
		private bool enqueueingOutgoing;
		private QS.Fx.Network.NetworkAddress localAddress;
		private IFlushingDevice flushingDevice;
		private QS.Fx.Logging.ILogger logger;

		#region IViewController Members

		public OutgoingMessageRef lookupOutgoing(GMS.GroupId groupID, uint viewSeqNo, uint inViewSeqNo)
		{
			lock (groups)
			{
				Collections.IDictionaryEntry dic_en = groups.lookup(groupID);
				if (dic_en == null)
					return null;
				Group thisGroup = (Group) dic_en.Value;
				Debug.Assert(thisGroup != null, "group is null");

				Collections.IDictionaryEntry dic_ve = thisGroup.views.lookup(viewSeqNo);
				if (dic_ve == null)
					return null;
				Group.View view = (Group.View) dic_ve.Value;
				Debug.Assert(view != null, "view is null");
				Debug.Assert(view.numberOfMessagesSent > 0, "no messages have been sent from this view");
				uint inGroupSeqNo = view.firstMessageSentInGroupSeqNo + inViewSeqNo - 1;

				OutgoingMessageRef messageRef = (OutgoingMessageRef) thisGroup.outgoingWindow.lookup(inGroupSeqNo);
				if (messageRef != null)
					Monitor.Enter(messageRef);

				return messageRef;
			}
		}

//		public void releaseOutgoing(OutgoingMessageRef messageRef)
//		{
//			Monitor.Exit(messageRef);
//		}

		public void registerOutgoing(OutgoingMessageRef outgoingMessageRef, IVSSender vsSender)
		{
#if DEBUG_ViewController
			logger.Log(this, "RegisterOutgoing : " + outgoingMessageRef.ToString());
#endif

			Monitor.Enter(groups);

			Collections.IDictionaryEntry dic_en = groups.lookup(outgoingMessageRef.GroupID);

			if (dic_en == null)
			{
				Monitor.Exit(groups);
				throw new Exception("Group non-existing!");
			}

			Group thisGroup = (Group) dic_en.Value;

// #if DEBUG_ViewController
//			logger.Log(this, "we have a group");
// #endif

			if (thisGroup.outgoingWindow.hasSpace())
			{
				Monitor.Enter(outgoingMessageRef);

				insertIntoAnOutgoingWindow(thisGroup, outgoingMessageRef);

				Monitor.Exit(groups);

				vsSender.readyToSendOut(outgoingMessageRef);
				Monitor.Exit(outgoingMessageRef);
			}
			else
			{
				if (enqueueingOutgoing)
				{
					thisGroup.waitingOutgoing.enqueue(new Group.WaitingRef(outgoingMessageRef, vsSender));
				}
				else
					throw new Exception("Can neither send nor enqueue request!");

				Monitor.Exit(groups);
			}			
		}

		private void insertIntoAnOutgoingWindow(Group thisGroup, VS2.OutgoingMessageRef outgoingMessageRef)
		{
			Group.View thisView = thisGroup.currentView;
			if (thisView == null)
				throw new Exception("No views installed!");
			uint ingroup_seqno = thisGroup.outgoingWindow.append(outgoingMessageRef);

			if (thisView.numberOfMessagesSent == 0)
				thisView.firstMessageSentInGroupSeqNo = ingroup_seqno;
			thisView.numberOfMessagesSent++;

			uint inview_seqno = ingroup_seqno + 1 - thisView.firstMessageSentInGroupSeqNo;

			outgoingMessageRef.registerInGroup(thisView, inview_seqno);						

#if DEBUG_ViewController
			logger.Log(this, "inserted " + outgoingMessageRef.ToString() + " into outgoing window, window is now:\n" + 
				thisGroup.outgoingWindow.ToString());
#endif
		}

		public void acknowledgedOutgoing(GMS.GroupId groupID, uint viewSeqNo, uint inViewSeqNo)
		{
			lock (groups)
			{
				Collections.IDictionaryEntry dic_en = groups.lookup(groupID);
				if (dic_en != null)
				{
					Group thisGroup = (Group) dic_en.Value;

					Collections.IDictionaryEntry dic_ve = thisGroup.views.lookup(viewSeqNo);
					if (dic_ve != null)
					{
						Group.View view = (Group.View) dic_ve.Value;

						Debug.Assert(view.numberOfMessagesSent > 0);

						uint inGroupSeqNo = view.firstMessageSentInGroupSeqNo + inViewSeqNo - 1;

						thisGroup.outgoingWindow.remove(inGroupSeqNo);

						while (thisGroup.outgoingWindow.hasSpace() && (thisGroup.waitingOutgoing.size() > 0))
						{
							Group.WaitingRef waitingRef = (Group.WaitingRef) thisGroup.waitingOutgoing.dequeue();

							// #if DEBUG_ViewController
							// logger.Log(this, "locking an enqueued message");
							// #endif

							Monitor.Enter(waitingRef.outgoingMessageRef);

							// #if DEBUG_ViewController
							// logger.Log(this, "inserting an enqueued message into an outgoing window");
							// #endif

							insertIntoAnOutgoingWindow(thisGroup, waitingRef.outgoingMessageRef);

							Monitor.Exit(groups);

							// #if DEBUG_ViewController
							// logger.Log(this, "sending an enqueued message");
							// #endif

							waitingRef.vsSender.readyToSendOut(waitingRef.outgoingMessageRef);
							Monitor.Exit(waitingRef.outgoingMessageRef);

							// #if DEBUG_ViewController
							// logger.Log(this, "reacquiring big lock");
							// #endif

							Monitor.Enter(groups);
						}

						// #if DEBUG_ViewController
						// logger.Log(this, "done with enqueued messages, window:\n" + thisGroup.outgoingWindow.ToString());
						// #endif
					}
				}
			}			
		}

		public IncomingMessageRef lookupOrCreateIncoming(Base.ObjectAddress senderAddress, GMS.GroupId groupID, 
			uint viewSeqNo, uint inViewSeqNo, Base.IMessage message, IVSSender vsSender, bool creationAllowed)
		{
			Monitor.Enter(groups);

			Collections.IDictionaryEntry dic_en = groups.lookup(groupID);
			if (dic_en == null)
				throw new Exception("Group non-existing!");
			Group thisGroup = (Group) dic_en.Value;
			Debug.Assert(thisGroup != null);

			Collections.IDictionaryEntry inv_en	= thisGroup.views.lookup(viewSeqNo);
			if (inv_en == null)
				throw new Exception("View non-existing!");
			Group.View thisView = (Group.View) inv_en.Value;
			Debug.Assert(thisView != null);

			FlowControl.IIncomingWindow incomingWindow = null;
			IncomingMessageRef incomingMessageRef = null;

			Collections.IDictionaryEntry snd_en = 
				thisView.incomingWindows.lookupOrCreate((QS.Fx.Network.NetworkAddress) senderAddress);
			if (snd_en.Value == null)
			{
				incomingWindow = new FlowControl.IncomingWindow(incomingWindowSize);
				snd_en.Value = incomingWindow;
			}
			else
			{
				incomingWindow = (FlowControl.IIncomingWindow) snd_en.Value;
				incomingMessageRef = (IncomingMessageRef) incomingWindow.lookup(inViewSeqNo);
			}

			if (incomingMessageRef != null)
			{
				Monitor.Enter(incomingMessageRef);
			}
			else
			{
				// we will have to change it later anyway
				while (incomingWindow.firstConsumed() && ((IncomingMessageRef) incomingWindow.first()).RemovingAllowed)
				{
					incomingWindow.cleanupOneGuy();
				}

				if (creationAllowed && incomingWindow.accepts(inViewSeqNo))
				{
					incomingMessageRef = vsSender.createIncoming(senderAddress, thisGroup, thisView, inViewSeqNo, message);	
					
					if (thisView.state == Group.View.State.FLUSHING)
					{
						incomingMessageRef.DeliveryAllowed = false;
					}

					incomingWindow.insert(inViewSeqNo, incomingMessageRef);

					if (thisView.state == Group.View.State.FLUSHING)
					{
//						if (thisView.flushingReportsArrived)
//						{
//							Group.View.ReceiverState receiverState = (Group.View.ReceiverState) thisView.receiverStates[senderAddress];
//							if (incomingWindow.lastConsumableSeqNo() >= receiverState.lastRequiredSeqNo)
//							{
//								receiverState.unstableMessagesSecured();
//
//
//							}
//						}						
					}
					else if (thisView.state == Group.View.State.INSTALLED)
					{
						while (incomingWindow.ready() && ((IncomingMessageRef) incomingWindow.first()).DeliveryAllowed)
						{
							incomingMessageRef = (IncomingMessageRef) incomingWindow.consume();

							Monitor.Enter(incomingMessageRef);
							Monitor.Exit(groups);

							incomingMessageRef.deliver();

							Monitor.Exit(incomingMessageRef);
							Monitor.Enter(groups);
						}
					}
				}

				incomingMessageRef = null;
			}

			Monitor.Exit(groups);
			return incomingMessageRef;
		}

		private bool localInstanceInView(GMS.IView view)
		{
			for (uint ind = 0; ind < view.NumberOfSubViews; ind++)
			{
				if (view[ind].IPAddress.Equals(localAddress.HostIPAddress) && view[ind].PortNumber.Equals(localAddress.PortNumber))
					return true;
			}
			return false;
		}

		// --------------------------------------------------------------------------------------------------

		public void viewChangeRequest(GMS.GroupId gid, GMS.IView view)
		{
			Group thisGroup;

// #if DEBUG_ViewController
			logger.Log(this, "ViewChangeRequest : " + gid.ToString() + ", " + view.ToString());
// #endif

			Monitor.Enter(groups);

			if (!localInstanceInView(view))
			{
#if DEBUG_ViewController
				logger.Log(this, "this is a removal request, we remove this group now");
#endif

				Collections.IDictionaryEntry dic_enx = groups.lookup(gid);
				if (dic_enx != null)
				{
					thisGroup = (Group) dic_enx.Value;
					Debug.Assert(thisGroup != null);

					// for now, we will just remove it... later we may want to do something more fancy
					groups.remove(gid);

					// in general, we may need to update the flushing device, but we ignore it for now
				}

#if DEBUG_ViewController
				logger.Log(this, "result : groups = " + groups.ToString());
#endif

				Monitor.Exit(groups);
				return;
			}

			Collections.IDictionaryEntry dic_en = groups.lookupOrCreate(gid);
			if (dic_en.Value == null)
			{
				thisGroup = new Group(gid, anticipatedNumberOfViews, anticipatedGroupSize, 
					outgoingWindowSize, incomingWindowSize, enqueueingOutgoing);
				dic_en.Value = thisGroup;
			}
			else
				thisGroup = (Group) dic_en.Value;

			Group.View currentView = thisGroup.currentView;
			thisGroup.currentView = new Group.View(thisGroup, view, Group.View.State.INITIALLY_ANNOUNCED, localAddress, logger);
			thisGroup.views[thisGroup.currentView.theView.SeqNo] = thisGroup.currentView;

			if (currentView != null)
			{
#if DEBUG_ViewController
				logger.Log(this, "initiating flushing for view " + 
					currentView.theView.SeqNo.ToString() + " in group " + gid.ToString());
#endif

				currentView.state = Group.View.State.FLUSHING;	
				thisGroup.flushingViews.insertAtTail(currentView);

				foreach (QS.Fx.Network.NetworkAddress networkAddress in currentView.receiverStates.Keys)
				{
					uint lastStableSeqNo = currentView.lastStableSeqNo(networkAddress);
					((Group.View.ReceiverState) currentView.receiverStates[networkAddress]).lastStableSeqNoAtFlushingStartTime = lastStableSeqNo;					

#if DEBUG_ViewController
					logger.Log(this, "last stable seqno at flushing start for " + networkAddress.ToString() + 
						" in view " + currentView.ToString() + " is " + lastStableSeqNo.ToString());
#endif
				}

				Collections.ISet liveAddresses = null, deadAddresses = null;
				currentView.identifyAliveAndCrashedNodes(thisGroup.currentView, ref liveAddresses, ref deadAddresses);

#if DEBUG_ViewController
				logger.Log(this, "live addresses : " + liveAddresses.ToString() + ", dead addresses : " + deadAddresses.ToString());
#endif

				foreach (Group.View flushingView in thisGroup.flushingViews.Elements)
				{
					flushingView.markAsCrashed(deadAddresses);
					flushingDevice.ignoreCrashed(new Base.ViewAddress(thisGroup.gid, flushingView.theView.SeqNo), deadAddresses);
				}

#if DEBUG_ViewController
				logger.Log(this, "creating a local report, group " + thisGroup.gid.ToString() + ", view " + 
					currentView.theView.SeqNo.ToString() + ", live addresses : " + liveAddresses.ToString());
#endif

				FlushingReport flushingReport = new FlushingReport(thisGroup, currentView);

#if DEBUG_ViewController
				logger.Log(this, "created a local flushing report : " + flushingReport.ToString());
#endif

				currentView.installLocalFlushingReport(flushingReport);
	
				ForwardingRequest[] fwdreqs = currentView.createForwardingRequests(null);

				Monitor.Exit(groups);
				

				object[] liveAddrObjs = liveAddresses.Elements;
				QS.Fx.Network.NetworkAddress[] liveAddrs = new QS.Fx.Network.NetworkAddress[liveAddrObjs.Length];
				for (uint ind = 0; ind < liveAddrObjs.Length; ind++)
					liveAddrs[ind] = (QS.Fx.Network.NetworkAddress) liveAddrObjs[ind];

				flushingDevice.initiateFlushing(liveAddrs, flushingReport, fwdreqs);
			}
			else
			{
				Monitor.Exit(groups);
			}

#if DEBUG_ViewController
			logger.Log(this, "viewChangeRequest processing has completed for view " + 
				view.SeqNo.ToString() + " in group " + gid.ToString());

			logger.Log(this, "groups : " + groups.ToString());
#endif
		}

		public void viewChangeAllDone(GMS.GroupId gid, uint seqno)
		{

			// TODO:  Add ViewController.viewChangeAllDone implementation
		}

		public void viewChangeCleanup(GMS.GroupId gid)
		{
			throw new Exception("not implemented yet");
		}

		// --------------------------------------------------------------------------------------------------------------

		public ForwardingRequest[] flushingReportArrived(QS.Fx.Network.NetworkAddress sourceAddress, FlushingReport flushingReport)
		{
#if DEBUG_ViewController
			logger.Log(this, "received a flushing report from " + sourceAddress.ToString() + " : " + flushingReport.ToString());
#endif

			try
			{
				ForwardingRequest[] fwdreqs = null;

				lock (groups)
				{
					Group group = (Group) groups[flushingReport.viewAddress.GroupID];
					Group.View view = (Group.View) group.views[flushingReport.viewAddress.ViewSeqNo];
				
					view.installRemoteFlushingReport(sourceAddress, flushingReport);

					fwdreqs = view.createForwardingRequests(sourceAddress);
				}

				return fwdreqs;
			}
			catch (Exception exc)
			{

				logger.Log(this, "################### Could not install flushing report! ###################\n" + exc.ToString());
				return null;
			}
		}

		// --------------------------------------------------------------------------------------------------------------

		public GMS.IView resolve(Base.ViewAddress viewAddress)
		{
			// TODO:  Add ViewController.resolve implementation
			return null;
		}

		public GMS.IView resolve(Base.GroupAddress groupAddress)
		{
			// TODO:  Add ViewController.QS.CMS.VS.IViewController.resolve implementation
			return null;
		}

		#endregion
	}
*/
}
