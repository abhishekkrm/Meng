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

#define DEBUG_VSGroup

using System;
using System.Diagnostics;
using System.Threading;

namespace QS._qss_c_.VS_2_
{
/*
	/// <summary>
	/// Summary description for Group.
	/// </summary>
	public class Group
	{
		public Group(GMS.GroupId gid, uint anticipatedNumberOfViews, uint anticipatedGroupSize, uint outgoingWindowSize, 
			uint incomingWindowSize, bool enqueueingOutgoing)
		{
			this.gid = gid;
			this.views = new Collections.Hashtable(anticipatedNumberOfViews);
			this.currentView = null;
			this.flushingViews = new Collections.BiLinkableCollection();

			this.outgoingWindow = new FlowControl.OutgoingWindow(outgoingWindowSize);
			if (enqueueingOutgoing)
			{
				waitingOutgoing = new Collections.RawQueue();
			}
		}

		public GMS.GroupId gid;
		public Collections.Hashtable views;
		public View currentView;
		public Collections.IBiLinkableCollection flushingViews;
		public FlowControl.IOutgoingWindow outgoingWindow;
		public Collections.IRawQueue waitingOutgoing;

		public class View : Collections.GenericBiLinkable
		{
			private QS.Fx.Logging.ILogger logger;

			public View(Group itsGroup, GMS.IView membership, View.State initialState, QS.Fx.Network.NetworkAddress localAddress, QS.Fx.Logging.ILogger logger)
			{
				this.logger = logger;

				this.itsGroup = itsGroup;
				theView = membership;
				this.incomingWindows = new Collections.Hashtable((uint) membership.NumberOfSubViews);
				receiverStates = new Collections.Hashtable((uint) theView.NumberOfSubViews);

				for (uint ind = 0; ind < theView.NumberOfSubViews; ind++)
				{
					receiverStates[new QS.Fx.Network.NetworkAddress(theView[ind].IPAddress, theView[ind].PortNumber)] = new ReceiverState();

//					logger.Log(this, 
//						"subview[" + ind.ToString() + "] -> " + theView[ind].IPAddress.ToString() + ":" + theView[ind].PortNumber);
					
					if (theView[ind].IPAddress.Equals(localAddress.HostIPAddress) && theView[ind].PortNumber.Equals(localAddress.PortNumber))
					{
						localReceivers = new uint[theView[ind].Count];
						for (uint kk = 0; kk < theView[ind].Count; kk++)
							localReceivers[kk] = (theView[ind])[kk];						

//						logger.Log(this, "YES!");
					}
				}
				this.numberOfPendingFlushingReports = (uint) theView.NumberOfSubViews;

				this.state = initialState;

				this.numberOfMessagesSent = 0;

				if (localReceivers == null)
				{
					throw new Exception("Local instance with address " + localAddress.ToString() + " is not a member of this view "
						+ membership.ToString());
				}

				flushingReportsArrived = false;
			}

			public override string ToString()
			{
				return "(" + state.ToString() + "; " + theView.ToString() + "; " + firstMessageSentInGroupSeqNo.ToString() + "+" +
					numberOfMessagesSent.ToString() + ")";
			}

			private Group itsGroup;

			public GMS.IView theView;
			public Collections.Hashtable incomingWindows;
			public State state;
			public uint firstMessageSentInGroupSeqNo, numberOfMessagesSent;
			public Collections.Hashtable receiverStates;

			private uint numberOfPendingFlushingReports;
			public bool flushingReportsArrived;
			public FlushingReport flushingReport;

			private static bool incomingRef_isStable(object obj)
			{
				return ((IncomingMessageRef) obj).stable();
			}

			public uint lastStableSeqNo(QS.Fx.Network.NetworkAddress networkAddress) // so nasty
			{
				FlowControl.IIncomingWindow incomingWindow = (FlowControl.IIncomingWindow) incomingWindows[networkAddress];
				uint lastGoodSeqNo = incomingWindow.lastConsumedSeqNo();
				System.Collections.ICollection unstableSeqNoList = 
					incomingWindow.selectObjects(new FlowControl.AcceptObjectCallback(Group.View.incomingRef_isStable));
				Collections.ISet iset = new Collections.HashSet((uint) unstableSeqNoList.Count);
				foreach (uint seqno in unstableSeqNoList)
					iset.insert(seqno);
				while (iset.contains(lastGoodSeqNo + 1))
					lastGoodSeqNo++;
				return lastGoodSeqNo;
			}

			public ForwardingRequest[] createForwardingRequests(QS.Fx.Network.NetworkAddress destination)
			{
				System.Collections.ArrayList msgfwd_list = new System.Collections.ArrayList();
				
				if (destination != null)
				{
					View.MessagesToForward msgfwd = messagesToForward(destination);
					if (msgfwd != null)
						msgfwd_list.Add(msgfwd);
				}
				else
				{
					View.MessagesToForward[] msgfwd_array = messagesToForward();				
					for (uint ind = 0; ind < msgfwd_array.Length; ind++)
					{
						if (msgfwd_array[ind] != null)
							msgfwd_list.Add(msgfwd_array[ind]);
					}
				}
				
				ForwardingRequest[] results = new ForwardingRequest[msgfwd_list.Count];
				uint index = 0;
				foreach (MessagesToForward msgfwd in msgfwd_list)
					results[index++] = messagesToForward2ForwardingRequest(msgfwd);
				return results;
			}

			private ForwardingRequest messagesToForward2ForwardingRequest(View.MessagesToForward fwdmsg)
			{
				if (fwdmsg == null)
					return null;

				System.Collections.ArrayList forwardingRefs = new System.Collections.ArrayList();

				foreach (FlushingReport.ReceivedMessages receivedMessages in fwdmsg.receivedMessages)
				{
					Collections.IDictionaryEntry dic_en = incomingWindows.lookup(receivedMessages.networkAddress);					 
					if (dic_en != null)
					{
						FlowControl.IIncomingWindow incomingWindow = (FlowControl.IIncomingWindow) dic_en.Value;
						
						foreach (uint seqno in receivedMessages.unstableMessageSeqNoList)
						{
							IncomingMessageRef messageRef = (IncomingMessageRef) incomingWindow.lookup(seqno);

							ForwardingRequest.MessageRef fwdmref = new ForwardingRequest.MessageRef(
								(Base.ObjectAddress) messageRef.SenderAddress, new Base.ViewAddress(itsGroup.gid, theView.SeqNo),
								messageRef.Message, messageRef.InViewSeqNo, messageRef.VSSender);

							forwardingRefs.Add(fwdmref);
						}
					}
				}
			
				ForwardingRequest.MessageRef[] refArray = new ForwardingRequest.MessageRef[forwardingRefs.Count];
				uint index = 0;
				foreach (ForwardingRequest.MessageRef mref in forwardingRefs)
					refArray[index++] = mref;

				return new ForwardingRequest(fwdmsg.networkAddress, refArray);
			}

			public sealed class MessagesToForward
			{
				public MessagesToForward(QS.Fx.Network.NetworkAddress networkAddress, FlushingReport.ReceivedMessages[] receivedMessages)
				{
					this.networkAddress = networkAddress;
					this.receivedMessages = receivedMessages;
				}

				public QS.Fx.Network.NetworkAddress networkAddress;
				public FlushingReport.ReceivedMessages[] receivedMessages;
			}

			public MessagesToForward[] messagesToForward()
			{
				if (flushingReport != null)
				{
					System.Collections.ArrayList list = new System.Collections.ArrayList();
					foreach (QS.Fx.Network.NetworkAddress networkAddress in receiverStates.Keys)
					{
						MessagesToForward fwdreq = messagesToForward(networkAddress);
						if (fwdreq != null)
							list.Add(fwdreq);
					}

					MessagesToForward[] results = new MessagesToForward[list.Count];
					uint index = 0;
					foreach (MessagesToForward fwdreq in list)
						results[index++] = fwdreq;

					return results;
				}
				else
					return null;
			}

			public MessagesToForward messagesToForward(QS.Fx.Network.NetworkAddress networkAddress)
			{
				if (flushingReport != null && networkAddress != null)
				{
					Collections.IDictionaryEntry dic_en = receiverStates.lookup(networkAddress);
					if (dic_en != null)
					{
						ReceiverState receiverState = (ReceiverState) dic_en.Value;

						Debug.Assert((receiverState != null), "receiver state for " + networkAddress.ToString() + " is null");
						Debug.Assert((flushingReport != null), "local flushing report is null");

//						Debug.Assert((receiverState.flushingReport != null), 
//							"flushing report for " + networkAddress.ToString() + " is null");

						if (receiverState.flushingReport != null)
						{
#if DEBUG_VSGroup
							logger.Log(this, "running diff on:\na) " + flushingReport.ToString() + "\nb) " + 
								receiverState.flushingReport.ToString());
#endif

							return new MessagesToForward(networkAddress, flushingReport.diff(receiverState.flushingReport));
						}
						else
							return null;
					}
					else
						return null;
				}
				else
					return null;
			}

			public void installLocalFlushingReport(FlushingReport flushingReport)
			{
				this.flushingReport = flushingReport;

#if DEBUG_VSGroup
				logger.Log(this, "view " + theView.SeqNo + " installing local report :\n" + flushingReport.ToString());
#endif

				if (numberOfPendingFlushingReports == 0)
				{
					flushingReportsAllReady();
				}
			}

			public void installRemoteFlushingReport(
				QS.Fx.Network.NetworkAddress sourceAddress, FlushingReport flushingReport)
			{
#if DEBUG_VSGroup
				logger.Log(this, "view " + theView.SeqNo + " installing a remote report from " + 
					sourceAddress.ToString() + " :\n" + flushingReport.ToString());
#endif
				
				ReceiverState receiverState = (ReceiverState) receiverStates[sourceAddress];
				if (!receiverState.failed && receiverState.flushingReport == null)
				{
					receiverState.flushingReport = flushingReport;
					numberOfPendingFlushingReports--;

					if (numberOfPendingFlushingReports == 0)
					{
						if (this.flushingReport != null)
							flushingReportsAllReady();
					}
					else
					{
#if DEBUG_VSGroup
						logger.Log(this, "number of pending reports : " + numberOfPendingFlushingReports);
#endif
					}
				}
			}

			public void flushingReportsAllReady()
			{
#if DEBUG_VSGroup
				logger.Log(this, "view " + theView.SeqNo + " has all flushing reports ready");
#endif

				flushingReportsArrived = true;
				calculateMessagesToWaitFor();
			}

			public void calculateMessagesToWaitFor()
			{
				Collections.IDictionary dict = new Collections.Hashtable((uint) theView.NumberOfSubViews);
				for (uint ind = 0; ind < theView.NumberOfSubViews; ind++)
				{
					QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(theView[ind].IPAddress, theView[ind].PortNumber);
					dict[networkAddress] = new Collections.HashSet(20); // some arbitrary setting for now
				}

				foreach (ReceiverState receiverState in receiverStates.Values)
				{
					if (!receiverState.failed && receiverState.flushingReport.receivedMessages != null)
					{
						foreach (FlushingReport.ReceivedMessages receivedMessages in receiverState.flushingReport.receivedMessages)
						{
							Collections.ISet seqno_set = (Collections.ISet) dict[receivedMessages.networkAddress];
							foreach (uint seqno in receivedMessages.unstableMessageSeqNoList)
							{
								if (!seqno_set.contains(seqno))
									seqno_set.insert(seqno);
							}
						}
					}
				}

				foreach (QS.Fx.Network.NetworkAddress senderAddress in dict.Keys)
				{
					Collections.ISet seqno_set = (Collections.ISet) dict[senderAddress];
					Collections.IPriorityQueue ipQueue = new Collections.BHeap(seqno_set.Count, 2);
					foreach (uint seqno in seqno_set.Elements)
						ipQueue.insert(seqno);

					uint last_accepted = ((ReceiverState) receiverStates[senderAddress]).lastStableSeqNoAtFlushingStartTime;
					while (!ipQueue.isempty())
					{
						uint seqno = (uint) ipQueue.deletemin();
						uint expected_seqno = last_accepted + 1;
						if (seqno > expected_seqno)
							break;
						if (seqno == expected_seqno)
							last_accepted++;
						else
						{
							// already included, just ignore...
						}
					}

					((ReceiverState) receiverStates[senderAddress]).lastRequiredSeqNo = last_accepted;

#if DEBUG_VSGroup
					logger.Log(this, "Calculated : The last required seqno for " + senderAddress.ToString() + " in view GID:" + 
						itsGroup.gid.ToString() + ";VSN:" + theView.SeqNo.ToString() + " is now (" + last_accepted.ToString() + ").");
#endif
				}
			}

			public void markAsCrashed(Collections.ISet deadAddresses)
			{
#if DEBUG_VSGroup
				logger.Log(this, "marking crashed in view " + this.ToString());
#endif

				foreach (QS.Fx.Network.NetworkAddress networkAddress in receiverStates.Keys)
				{
#if DEBUG_VSGroup
					logger.Log(this, "checking whether " + networkAddress.ToString() + " is in " + deadAddresses.ToString());
#endif

					if (deadAddresses.contains(networkAddress))
					{
						ReceiverState receiverState = (ReceiverState) receiverStates[networkAddress];
						if (!receiverState.failed)
						{
#if DEBUG_VSGroup
							logger.Log(this, "marking " + networkAddress.ToString() + " as crashed in " + this.ToString());
#endif
							receiverState.failed = true;
							if (receiverState.flushingReport == null)
								numberOfPendingFlushingReports--;
							else
								receiverState.flushingReport = null;

#if DEBUG_VSGroup
							logger.Log(this, "pending reports : " + numberOfPendingFlushingReports);
#endif
						}
						else
						{
#if DEBUG_VSGroup
							logger.Log(this, "address " + networkAddress.ToString() + " already marked as crashed in " + this.ToString());
#endif
						}
					}
				}
			}

			private uint[] localReceivers = null;

			public uint[] LocalReceivers
			{
				get
				{
					return localReceivers;
				}
			}

			public enum State
			{
				// UNKNOWN,
				INITIALLY_ANNOUNCED, INSTALLED, FLUSHING
			}

			public class ReceiverState // should be more like a "peer state" now...
			{
				public ReceiverState()
				{
					this.failed = false;
					this.flushingReport = null;
				}

				public bool failed;
				public uint lastStableSeqNoAtFlushingStartTime;
				public uint lastRequiredSeqNo; // last seqno required for this receiver to be considered done as a source
				public FlushingReport flushingReport;
			}

			public void identifyAliveAndCrashedNodes(View newView, ref Collections.ISet liveNodes, ref Collections.ISet deadNodes)
			{
				View oldView = this;

// #if DEBUG_VSGroup
//				logger.Log(this, "identifying alive and crashed nodes, view change " + 
//					oldView.ToString() + " -> " + newView.ToString());
// #endif

				uint ancitipatedSetSize = (uint) theView.NumberOfSubViews;
				deadNodes = new Collections.HashSet(ancitipatedSetSize);
				liveNodes = new Collections.HashSet(ancitipatedSetSize);

				for (uint ind = 0; ind < oldView.theView.NumberOfSubViews; ind++)
				{
					QS.Fx.Network.NetworkAddress networkAddress = 
						new QS.Fx.Network.NetworkAddress(oldView.theView[ind].IPAddress, oldView.theView[ind].PortNumber);
					deadNodes.insert(networkAddress);
				}

				for (uint ind = 0; ind < newView.theView.NumberOfSubViews; ind++)				
				{
					QS.Fx.Network.NetworkAddress networkAddress = 
						new QS.Fx.Network.NetworkAddress(newView.theView[ind].IPAddress, newView.theView[ind].PortNumber);

					// logger.Log(null, "enter : " + deadNodes.ToString() + ", removing : " + networkAddress.ToString());

					QS.Fx.Network.NetworkAddress removedAddress = (QS.Fx.Network.NetworkAddress) deadNodes.remove(networkAddress);
					
					// logger.Log(null, "leave : " + deadNodes.ToString() + ", produced : " + 
					//	((removedAddress != null) ? removedAddress.ToString() : "null"));

					if (removedAddress != null)
					{
						liveNodes.insert(removedAddress);
					}
				}
			}
		}

		public class WaitingRef : Collections.GenericLinkable
		{
			public WaitingRef(OutgoingMessageRef outgoingMessageRef, 
				IVSSender vsSender)
			{
				this.outgoingMessageRef = outgoingMessageRef;
				this.vsSender = vsSender;
			}

			public OutgoingMessageRef outgoingMessageRef;
			public IVSSender vsSender;
		}
	}
*/
}
