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

#define DEBUG_VSSender

using System;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace QS._qss_c_.VS3
{
/*
	/// <summary>
	/// Summary description for VSSender.
	/// </summary>
	public class VSSender : Base.ISender, IViewController, Base.IClient, Flushing.IFlushingReportConsumer
	{
		private const uint anticipatedNumberOfGroups			=		20;
		private const uint anticipatedNumberOfViews				=		5;
		private const uint initialOutgoingWindowSize				=		10;
		private const uint initialIncomingWindowSize				=		10;

		public VSSender(QS.Fx.Logging.ILogger logger, QS.Fx.Network.NetworkAddress localAddress, Multicasting.IMulticastingDevice multicastingDevice,
			Base.IDemultiplexer demultiplexer, Base.ISender acknowledgementSender, Base.ISender forwardingSender, Flushing.IFlushingDevice flushingDevice)
		{
			this.logger = logger;
			
			this.multicastingDevice = multicastingDevice;
			this.acknowledgementSender = acknowledgementSender;
			this.forwardingSender = forwardingSender;
			this.localAddress = localAddress;
			this.flushingDevice = flushingDevice;
			this.demultiplexer = demultiplexer;

			flushingDevice.linkToReportConsumer(this);

			groups = new Collections.Hashtable(anticipatedNumberOfGroups);

			demultiplexer.register(this, new Dispatchers.DirectDispatcher(new Base.OnReceive(this.receiveCallback)));

			Base.Serializer.Get.register(ClassID.VSSender_Message, new Base.CreateSerializable(Message.createSerializable));
			Base.Serializer.Get.register(ClassID.VSSender_Acknowledgement, new Base.CreateSerializable(Acknowledgement.createSerializable));
			Base.Serializer.Get.register(ClassID.FlushingReport, new Base.CreateSerializable(Flushing.FlushingReport.createSerializable));
//			Base.Serializer.Get.register(ClassID.ReceiverReport, new Base.CreateSerializable(Flushing.FlushingReport.ReceiverReport.createSerializable));
		}

		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.Network.NetworkAddress localAddress;
		private Multicasting.IMulticastingDevice multicastingDevice;
		private Flushing.IFlushingDevice flushingDevice;
		private Base.ISender acknowledgementSender, forwardingSender;
		private Base.IDemultiplexer demultiplexer;
		private Collections.IDictionary groups;
		private GMS.ViewChangeGoAhead viewChangeGoAhead;

		private enum ProtocolPhase : ushort 
		{
			INITIALLY_SENDING_MESSAGE, READY_TO_DELIVER_MESSAGE, MESSAGE_DELIVERED_EVERYWHERE, MESSAGE_CLEANUP_COMPLETE, 
			ALL_PHASES_COMPLETE
		}

		private const string HORIZONTAL_BAR = "------------------------------------------------------------------------------------------------------------------------------------------------------";
		public void debugDumpState()
		{
			logger.Log(this, "\n" + HORIZONTAL_BAR + "\nFULL DUMP @ " +
				DateTime.Now.ToString() + "\n" + this.AllToString + HORIZONTAL_BAR + "\n");
		}

		public string AllToString
		{
			get
			{
				string debug_string = "";
				foreach (Group group in groups.Values)
					debug_string = debug_string + group.AllToString;
				return debug_string;
			}
		}

		private class ForwardingRequest : Collections.GenericBiLinkable
		{
			public ForwardingRequest(QS.Fx.Network.NetworkAddress originalSender, Message message, QS.Fx.Network.NetworkAddress forwardingDestination) : base()
			{
				this.forwardedMessage = new ForwardedMessage(originalSender, message);
				this.forwardingDestination = forwardingDestination;
			}

			public class ForwardedMessage : Base.IMessage
			{
				public ForwardedMessage(QS.Fx.Network.NetworkAddress sender, Message message)
				{
					this.sender = sender;
					this.message = message;
				}

				public QS.Fx.Network.NetworkAddress SourceAddress
				{
					get
					{
						return sender;
					}
				}

				public Message ReceivedMessage
				{
					get
					{
						return message;
					}
				}

				private QS.Fx.Network.NetworkAddress sender;
				private Message message;

				#region ISerializable Members

				public QS.ClassID ClassIDAsSerializable
				{
					get
					{
						return ClassID.ForwardingRequest_ForwardedMessage;
					}
				}

				public void save(Stream stream)
				{
					sender.save(stream);
					message.save(stream);
				}

				public void load(Stream stream)
				{
					sender = new QS.Fx.Network.NetworkAddress();
					sender.load(stream);
					message = new Message();
					message.load(stream);
				}

				#endregion
			}

			public QS.Fx.Network.NetworkAddress DestinationAddress
			{
				get
				{
					return forwardingDestination;
				}
			}

			public ForwardedMessage MessageToForward
			{
				get
				{
					return forwardedMessage;
				}
			}

			private QS.Fx.Network.NetworkAddress forwardingDestination;
			private ForwardedMessage forwardedMessage;
		}

		private class Group
		{
			public Group(GMS.GroupId groupID, SendCallback sendCallback, QS.Fx.Logging.ILogger logger, GMS.ViewChangeGoAhead viewChangeGoAhead)
			{
				this.groupID = groupID;
				this.views = new Collections.Hashtable(anticipatedNumberOfViews);
				this.currentView = null;
				this.outgoingWindow = new OutgoingWindow(initialOutgoingWindowSize);
				this.waitingOutgoing = new Collections.RawQueue();
				this.sendCallback = sendCallback;
				this.flushingViews = new Collections.BiLinkableCollection();

				this.viewChangeGoAhead = viewChangeGoAhead;
				Debug.Assert((viewChangeGoAhead != null), "viewChangeGoAhead is null");

				this.logger = logger;
			}

			private GMS.GroupId groupID;
			private Collections.IDictionary views;
			private View currentView;
			public Collections.IRawQueue waitingOutgoing;
			private OutgoingWindow outgoingWindow;
			private SendCallback sendCallback;
			private Collections.IBiLinkableCollection flushingViews;
			private GMS.ViewChangeGoAhead viewChangeGoAhead;

			private QS.Fx.Logging.ILogger logger;

			public string AllToString
			{
				get
				{
					string debug_string = "Group(" + groupID.ToString() + "; waiting : " + waitingOutgoing.size() + "; outgoingWindow : " + 
						outgoingWindow.AllToString + ")\nCurrent View:\n" + currentView.AllToString + "Flushing Views:\n";
					foreach (View view in flushingViews.Elements)
						debug_string = debug_string + view.AllToString + "\n";
					return debug_string;
				}
			}

			public GMS.GroupId GroupID
			{
				get
				{
					return groupID;
				}
			}

			public override string ToString()
			{
				return "Group(" + groupID.ToString() + ")";
			}

			#region Initiate a View Change

			public void initiateViewChange(GMS.IView membershipView, QS.Fx.Network.NetworkAddress localAddress, Flushing.IFlushingDevice flushingDevice, 
				GMS.ViewChangeGoAhead viewChangeGoAhead)
			{
#if DEBUG_VSSender
				logger.Log(this, "initiateViewChange, GID = " + groupID.ToString());
#endif

				View previousView = currentView;
				currentView = new View(membershipView, localAddress, groupID, logger);
				views[currentView.MembershipView.SeqNo] = currentView;

				if (previousView != null)
				{
#if DEBUG_VSSender
					logger.Log(this, "we need to do some flushing for view : " + previousView.ToString());
#endif

					Collections.ISet liveAddresses = null, deadAddresses = null;
					Flushing.FlushingReport.ReceiverReport[] receiverReports = previousView.initiateFlushing(currentView, ref liveAddresses, ref deadAddresses);

#if DEBUG_VSSender
					logger.Log(this, "Live Addresses : " + liveAddresses.ToString() + "; Dead Addresses : " + deadAddresses.ToString());			
#endif			
		
					Flushing.FlushingReport newestFlushingReport = new Flushing.FlushingReport(
						groupID, previousView.MembershipView.SeqNo, true, previousView.NumberOfMessagesSent, receiverReports);
					
					System.Collections.ArrayList reportsOfOlderViews = new System.Collections.ArrayList();
					foreach (View flushingView in flushingViews.Elements)
					{
						reportsOfOlderViews.Add(new Flushing.FlushingReport(groupID, flushingView.MembershipView.SeqNo, false, 0, flushingView.markAsCrashed(deadAddresses)));

						// something about forwarding, tell them to stop forwarding to those addresses...
					}

					Flushing.FlushingReport[] flushingReports = new Flushing.FlushingReport[1 + reportsOfOlderViews.Count];
					flushingReports[0] = newestFlushingReport;
					reportsOfOlderViews.CopyTo(0, flushingReports, 1, reportsOfOlderViews.Count);

#if DEBUG_VSSender
					logger.Log(this, "All flushing reports ready.");			
#endif			
	
					flushingDevice.distributeFlushingReports((QS.Fx.Network.NetworkAddress []) liveAddresses.ToArray(typeof(QS.Fx.Network.NetworkAddress)), flushingReports);

					flushingViews.insertAtTail(previousView);


					// ...........
				}
				else
				{
					performGarbageCollectionOfCompleteViews();
				}
				
				Monitor.Exit(this);
			}

			#endregion

			#region Complete Views Garbage Collection

			private void performGarbageCollectionOfCompleteViews()
			{
				while (flushingViews.Count > 0)
				{
					View view = (View) flushingViews.elementAtHead();
					if (view.FlushingComplete)					
						flushingViews.remove(view);
					else
						break;
				}
				
				if (flushingViews.Count == 0 && currentView != null && !currentView.Installed)
				{
					currentView.install();

#if DEBUG_VSSender
					logger.Log(this, "calling viewChangeGoAhead(" + groupID.ToString() + ", " +  currentView.MembershipView.SeqNo.ToString() + ")");
#endif

					viewChangeGoAhead(groupID, currentView.MembershipView.SeqNo);
				}
			}

			#endregion

			#region Processing of Flushing Reports

			public void deliverAllTheRestAfterFlushing(uint viewSeqNo, Base.IDemultiplexer demultiplexer)
			{
				View view = lookupView(viewSeqNo);
				if (view != null)
				{
					Monitor.Exit(this);
					view.deliverAllTheRestAfterFlushing(demultiplexer);
					Monitor.Exit(view);
				}
				else
					Monitor.Exit(this);
			}

			public Collections.IBiLinkableCollection incorporateFlushingReport(QS.Fx.Network.NetworkAddress reportSender, Flushing.FlushingReport flushingReport)
			{
				Collections.IBiLinkableCollection forwardingRequests = null;

				View view = lookupView(flushingReport.ViewSeqNo);
				if (view != null)
				{
					forwardingRequests = view.incorporateFlushingReport(reportSender, flushingReport);

					performGarbageCollectionOfCompleteViews();

					Monitor.Exit(view);
				}

				Monitor.Exit(this);			

				return forwardingRequests;
			}

			#endregion

			#region Registering Outgoing

			public void registerOutgoing(IOutgoingMessageRef outgoingRef)
			{
#if DEBUG_VSSender
				logger.Log(this, "Group(" + groupID.ToString() + ").registerOutgoing(" + outgoingRef.ToString() + ")");
#endif		

				if (currentView == null)
				{
					Monitor.Exit(this);
					throw new Exception("no view is currently installed");
				}

				OutgoingSlot outgoingSlot = new OutgoingSlot(outgoingRef);

				if (outgoingWindow.HasSpace)
				{
#if DEBUG_VSSender
					logger.Log(this, "OutgoingWindow.HasSpace, inserting " + outgoingRef.ToString());				
#endif

					Monitor.Enter(outgoingSlot);

					insertIntoAnOutgoingWindow(outgoingSlot);
					
					Monitor.Exit(this);

					outgoingSlot.multicast(sendCallback);

					Monitor.Exit(outgoingSlot);
				}
				else
				{
#if DEBUG_VSSender
					logger.Log(this, "no space left, enqueueing " + outgoingRef.ToString());				
#endif

					waitingOutgoing.enqueue(outgoingSlot);
					Monitor.Exit(this);
				}
			}

			private void insertIntoAnOutgoingWindow(OutgoingSlot outgoingSlot)
			{
				uint inGroupSeqNo = outgoingWindow.append(outgoingSlot);
				lock (currentView)
				{
					currentView.sendingAMessage(inGroupSeqNo);
				}
				
				uint withinViewSeqNo = currentView.group2ViewSeqNo(inGroupSeqNo);
				outgoingSlot.registerInGroup(this, currentView, withinViewSeqNo);
			}

			#endregion

			#region Finders

			private View lookupView(uint viewSeqNo)
			{
				Collections.IDictionaryEntry dic_en = views.lookup(viewSeqNo);
				if (dic_en != null)
				{
					View view = (View) dic_en.Value;
					if (view != null)
					{
						Monitor.Enter(view);
						return view;
					}
				}				
				return null;
			}

			private OutgoingSlot lookupOutgoingSlot(View view, uint withinViewSeqNo)
			{
				if (view.NumberOfMessagesSent > 0)
				{
					uint inGroupSeqNo = view.view2GroupSeqNo(withinViewSeqNo);
					OutgoingSlot outgoingSlot = outgoingWindow.lookup(inGroupSeqNo);
					if (outgoingSlot != null)
					{
						Monitor.Enter(outgoingSlot);
						return outgoingSlot;
					}
				}				
				return null;
			}

			#endregion

			#region Processing of Acknowledgements

			public void processAcknowledgement(uint viewSeqNo, uint withinViewSeqNo, ProtocolPhase protocolPhase, 
				QS.Fx.Network.NetworkAddress networkAddress)
			{
				View view = lookupView(viewSeqNo);

				if (view != null)
				{
					OutgoingSlot outgoingSlot = lookupOutgoingSlot(view, withinViewSeqNo);

					Monitor.Exit(this);
					
					if (outgoingSlot != null)
					{
						if (protocolPhase == outgoingSlot.CurrentPhase)
						{
							outgoingSlot.acknowledge(networkAddress);
						}
						
						Monitor.Exit(outgoingSlot);
					}

					Monitor.Exit(view);
				}
				else
					Monitor.Exit(this);
			}	

			public void acknowledgementComplete(uint viewSeqNo, uint withinViewSeqNo)
			{
#if DEBUG_VSSender
				logger.Log(this, this.ToString() + ".acknowledgementComplete(viewSeqNo:" + viewSeqNo.ToString() + ", withinViewSeqNo:" + withinViewSeqNo.ToString());
#endif

				View view = lookupView(viewSeqNo);

				if (view != null)
				{
					OutgoingSlot outgoingSlot = lookupOutgoingSlot(view, withinViewSeqNo);
					uint withinGroupSeqNo = currentView.view2GroupSeqNo(withinViewSeqNo);

					Monitor.Exit(view);

					if (outgoingSlot != null)
					{
						if (outgoingSlot.initiateNextProtocolPhase())
						{
							Monitor.Exit(this);

							outgoingSlot.multicast(sendCallback);
							Monitor.Exit(outgoingSlot);

							return;
						}
						else
						{
							Monitor.Exit(outgoingSlot);

							outgoingWindow.remove(withinGroupSeqNo);

							while (outgoingWindow.HasSpace && waitingOutgoing.size() > 0)
							{
								OutgoingSlot anotherSlot = (OutgoingSlot) waitingOutgoing.dequeue();
								Monitor.Enter(anotherSlot);

								insertIntoAnOutgoingWindow(anotherSlot);
								Monitor.Exit(this);

								anotherSlot.multicast(sendCallback);
								Monitor.Exit(anotherSlot);

								Monitor.Enter(this);
							}
						}
					}
				}

				Monitor.Exit(this);
			}
	
			#endregion

			#region Processing of received Messages
			
			public bool processReceivedMessage(QS.Fx.Network.NetworkAddress networkAddress, Message message, Base.IDemultiplexer demultiplexer)
			{
				View view = lookupView(message.viewSeqNo);
				if (view != null)
				{
					Monitor.Exit(this);
					bool shouldSendACK = view.processReceivedMessage(networkAddress, message, demultiplexer);
					Monitor.Exit(view);

					performGarbageCollectionOfCompleteViews();

					return shouldSendACK;
				}
				else
				{
					Monitor.Exit(this);
					return false;
				}
			}

			#endregion

			#region Definition of a View

			private class View : Collections.GenericBiLinkable, Multicasting.IAddressSet
			{
				#region Constructors

				public View(GMS.IView membershipView, QS.Fx.Network.NetworkAddress localAddress, GMS.GroupId groupID, QS.Fx.Logging.ILogger logger)
				{
					this.logger = logger;

					// this.demultiplexer = demultiplexer;
					// this.viewChangeGoAhead = viewChangeGoAhead;
					this.groupID = groupID;

					this.membershipView = membershipView;				
					this.indexOfAddresses = new Collections.Hashtable((uint) membershipView.NumberOfSubViews);
					this.incomingWindows = new IncomingWindow[membershipView.NumberOfSubViews];
					this.peerStates = new StateOfAPeer[membershipView.NumberOfSubViews];
					this.peersNotReady = (uint) membershipView.NumberOfSubViews;

					for (uint ind = 0; ind < membershipView.NumberOfSubViews; ind++)
					{
						peerStates[ind] = new StateOfAPeer((uint) membershipView.NumberOfSubViews);
						incomingWindows[ind] = new IncomingWindow(initialIncomingWindowSize);
						QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(membershipView[ind].IPAddress, membershipView[ind].PortNumber);
						indexOfAddresses[networkAddress] = ind;
						if (networkAddress.Equals(localAddress))
						{
							localReceivers = new uint[membershipView[ind].Count];
							for (uint recno = 0; recno < localReceivers.Length; recno++)
								localReceivers[recno] = (membershipView[ind])[recno];
						}
					}

					firstMessageSentInGroupSeqNo = numberOfMessagesSent = 0;

					this.state = State.INITIALLY_ANNOUNCED;
				}

				#endregion

				private GMS.IView membershipView;
				private Collections.IDictionary indexOfAddresses;
				private IncomingWindow[] incomingWindows;
				private uint firstMessageSentInGroupSeqNo, numberOfMessagesSent, peersNotReady;
				private StateOfAPeer[] peerStates; 
				private State state;
				private System.Collections.ArrayList indexesOfAllCrashedMembers = null;
				private uint[] localReceivers;

				private QS.Fx.Logging.ILogger logger;

				private GMS.GroupId groupID;

				private enum State
				{
					INITIALLY_ANNOUNCED, INSTALLED, FLUSHING, ALL_OPERATIONS_COMPLETED
				}

				public void install()
				{
					state = State.INSTALLED;
				}

				public bool Installed
				{
					get
					{
						return state > State.INITIALLY_ANNOUNCED;
					}
				}

				public override string ToString()
				{
					return "View(" +  groupID.ToString() + ":" + membershipView.SeqNo.ToString() + ")";
				}

				public string AllToString
				{
					get
					{
						string debug_string = "  View : " + membershipView.ToString() + "; state : " + state.ToString() + "; sent : " + numberOfMessagesSent.ToString() + "; Peers :\n";
						for (uint ind = 0; ind < membershipView.NumberOfSubViews; ind++)
							debug_string = debug_string + "    Peer: " + this[ind].ToString() + "; State : " + peerStates[ind].ToString() + 
								"; incomingWindow : " + incomingWindows[ind].AllToString + "\n";
						return debug_string;
					}
				}

				#region Handling of Flushing Reports

				public bool FlushingComplete
				{
					get
					{
						return state == State.ALL_OPERATIONS_COMPLETED;
					}
				}

				public Flushing.FlushingReport.ReceiverReport[] initiateFlushing(
					View overridingView, ref Collections.ISet liveNodes, ref Collections.ISet deadNodes)
				{
					this.state = State.FLUSHING;
					identifyAliveAndCrashedNodes(overridingView, ref liveNodes, ref deadNodes);
					return markAsCrashed(deadNodes);
				}

				private void flushingComplete()
				{
					state = State.ALL_OPERATIONS_COMPLETED;
					// checkWhetherWeCanInstall();
				}

				public void deliverAllTheRestAfterFlushing(Base.IDemultiplexer demultiplexer)
				{
#if DEBUG_VSSender
					logger.Log(this, this.ToString() + ".deliverAllTheRestAfterFlushing();");
#endif

					foreach (IncomingWindow incomingWindow in incomingWindows)
					{
						while (incomingWindow.NumberConsumed < incomingWindow.NumberReceived)
						{
							IncomingSlot incomingSlot = incomingWindow.consume();

#if DEBUG_VSSender
							logger.Log(this, this.ToString() + " after-flushing delivery of " + incomingSlot.ToString());
#endif

							foreach (uint receiverLOID in LocalReceivers)
							{
								demultiplexer.demultiplex(receiverLOID, incomingSlot.SenderAddress, incomingSlot.Data);
							}

							incomingSlot.disposeOfApplicationData();
							incomingSlot.CleanupAllowed = true;
						}

						incomingWindow.cleanupWhateverYouCan();
					}
				}

				private void checkPeerForReadiness(uint indexOfTheSender)
				{
					if (state == State.FLUSHING && !peerStates[indexOfTheSender].Ready && peerStates[indexOfTheSender].FlushingInformationReady && 
						incomingWindows[indexOfTheSender].NumberReceived >= peerStates[indexOfTheSender].NumberOfMessagesRequired)
					{
						peerStates[indexOfTheSender].Ready = true;
						peersNotReady--;

						if (peersNotReady == 0)
						{
							flushingComplete();
						}
					}
				}

				// returns forwarding requests
				public Collections.IBiLinkableCollection incorporateFlushingReport(QS.Fx.Network.NetworkAddress reportSender, Flushing.FlushingReport flushingReport)
				{
					Collections.IBiLinkableCollection forwardingRequests = null;
					
#if DEBUG_VSSender
					logger.Log(this, this.ToString() + ".incorporateFlushingReport(" + reportSender.ToString() + ", " + flushingReport.ToString());
#endif

					uint indexOfAMember = UInt32.MaxValue;
					if (lookupNetworkAddress(reportSender, ref indexOfAMember))
					{
						StateOfAPeer peerState = peerStates[indexOfAMember];
						
						if (flushingReport.IsInitial)
						{
							peerState.NumberOfMessagesSent = flushingReport.NumberOfMessagesSent;
							checkPeerForReadiness(indexOfAMember);
						}

						forwardingRequests = new Collections.BiLinkableCollection(); 

						foreach (Flushing.FlushingReport.ReceiverReport receiverReport in flushingReport.ReceiverReports)
						{
							uint indexOfASource = UInt32.MaxValue;
							if (lookupNetworkAddress(receiverReport.SourceAddress, ref indexOfASource))
							{
								StateOfAPeer sourceState = peerStates[indexOfASource];
								sourceState.updateWithNumberOfMessagesConsumed(indexOfAMember, receiverReport.NumberOfMessagesConsumed);
								checkPeerForReadiness(indexOfASource);

								// now we do something to forward... this is not super efficient, and it need to be changed in future
								
								// data source					--> indexOfASource
								// how much we received			--> incomingWindows[indexOfASource].NumberReceived
								// the guy that may need data		--> indexOfAMember
								// how much he consumed			--> receiverReport.NumberOfMessagesConsumed

								if (receiverReport.NumberOfMessagesConsumed < incomingWindows[indexOfASource].NumberReceived)
								{
									for (uint seqno = receiverReport.NumberOfMessagesConsumed; 
										seqno < incomingWindows[indexOfASource].NumberReceived; seqno++)
									{
										IncomingSlot incomingSlot = (IncomingSlot) incomingWindows[indexOfASource].lookup(seqno);						

										QS.Fx.Network.NetworkAddress originalSenderNetAddr = (QS.Fx.Network.NetworkAddress) incomingSlot.SenderAddress;
										Message message = new Message(((Base.ObjectAddress) incomingSlot.SenderAddress).LocalObjectID, groupID, membershipView.SeqNo, seqno, 
											incomingSlot.CurrentPhase, incomingSlot.Data, false, false, false); // for now, just false everywhere
										QS.Fx.Network.NetworkAddress forwardingDestinationNetAddr = this[indexOfASource];
			
										ForwardingRequest req = new ForwardingRequest(originalSenderNetAddr, message, forwardingDestinationNetAddr);
										forwardingRequests.insertAtTail(req);
									}
								}
							}
						}
					}

					return forwardingRequests;
				}

				#endregion

				#region Crashed node Identification and Marking

				public Flushing.FlushingReport.ReceiverReport[] markAsCrashed(Collections.ISet deadAddresses)
				{
					if (indexesOfAllCrashedMembers == null)
						indexesOfAllCrashedMembers = new System.Collections.ArrayList(5);

					System.Collections.ArrayList receiverReports = new System.Collections.ArrayList();
					
					System.Collections.ArrayList temp = new System.Collections.ArrayList();					
					foreach (QS.Fx.Network.NetworkAddress networkAddress in (QS.Fx.Network.NetworkAddress[]) deadAddresses.ToArray(typeof(QS.Fx.Network.NetworkAddress)))
					{
						uint indexOfAMember = UInt32.MaxValue;
						if (lookupNetworkAddress(networkAddress, ref indexOfAMember))
						{
							temp.Add(indexOfAMember);
							indexesOfAllCrashedMembers.Add(indexOfAMember);
						}
					}
					uint[] indexesOfNewDeadAddresses = (uint[]) temp.ToArray(typeof(uint));

					for (uint ind = 0; ind < this.Count; ind++)
					{
						if (peerStates[ind].Crashed)
							peerStates[ind].updateWithCrashes(indexesOfNewDeadAddresses);
					}

					foreach (uint indexOfAMember in indexesOfNewDeadAddresses)
					{
						IncomingWindow incomingWindow = (IncomingWindow) incomingWindows[indexOfAMember];
						receiverReports.Add(new Flushing.FlushingReport.ReceiverReport(this[indexOfAMember], incomingWindow.NumberConsumed));							

						StateOfAPeer peerState = peerStates[indexOfAMember];
						peerState.markAsCrashed((uint[]) indexesOfAllCrashedMembers.ToArray(typeof(uint)));
					}

					return (Flushing.FlushingReport.ReceiverReport[]) receiverReports.ToArray(typeof(Flushing.FlushingReport.ReceiverReport));
				}

				private void identifyAliveAndCrashedNodes(View newView, ref Collections.ISet liveNodes, ref Collections.ISet deadNodes)
				{
					uint ancitipatedSetSize = (uint) membershipView.NumberOfSubViews;
					deadNodes = new Collections.HashSet(ancitipatedSetSize);
					liveNodes = new Collections.HashSet(ancitipatedSetSize);

					for (uint ind = 0; ind < membershipView.NumberOfSubViews; ind++)
					{
						QS.Fx.Network.NetworkAddress networkAddress = 
							new QS.Fx.Network.NetworkAddress(membershipView[ind].IPAddress, membershipView[ind].PortNumber);
						deadNodes.insert(networkAddress);
					}

					for (uint ind = 0; ind < newView.MembershipView.NumberOfSubViews; ind++)				
					{
						QS.Fx.Network.NetworkAddress networkAddress = 
							new QS.Fx.Network.NetworkAddress(newView.MembershipView[ind].IPAddress, newView.MembershipView[ind].PortNumber);
						QS.Fx.Network.NetworkAddress removedAddress = (QS.Fx.Network.NetworkAddress) deadNodes.remove(networkAddress);					
						if (removedAddress != null)
							liveNodes.insert(removedAddress);
					}
				}			

				#endregion

				#region Accessors and Helpers

				public uint NumberOfMessagesSent
				{
					get
					{
						return numberOfMessagesSent;
					}
				}

				public void sendingAMessage(uint inGroupSeqNo)
				{
					if (numberOfMessagesSent == 0)
						firstMessageSentInGroupSeqNo = inGroupSeqNo;
					numberOfMessagesSent++;
				}

				public uint group2ViewSeqNo(uint inGroupSeqNo)
				{
					return inGroupSeqNo + 1 - firstMessageSentInGroupSeqNo;
				}

				public uint view2GroupSeqNo(uint inViewSeqNo)
				{
					return firstMessageSentInGroupSeqNo + inViewSeqNo - 1;
				}

				public GMS.IView MembershipView
				{
					get
					{
						return membershipView;
					}
				}

				private bool containsAddress(QS.Fx.Network.NetworkAddress networkAddress)
				{
					return indexOfAddresses.lookup(networkAddress) != null;
				}

				private bool lookupNetworkAddress(QS.Fx.Network.NetworkAddress address, ref uint indexOf)
				{
					Collections.IDictionaryEntry dic_en = indexOfAddresses.lookup(address);
					if (dic_en != null)
					{
						indexOf = (uint) dic_en.Value;
						return true;
					}
					else
						return false;
				}

				#region IAddressSet Members

				public uint Count
				{
					get
					{
						return (uint) membershipView.NumberOfSubViews;
					}
				}

				public QS.Fx.Network.NetworkAddress this[uint indexOf]
				{
					get
					{
						return new QS.Fx.Network.NetworkAddress(membershipView[indexOf].IPAddress, membershipView[indexOf].PortNumber);
					}
				}

				public uint IndexOf(QS.Fx.Network.NetworkAddress address)
				{
					return (uint) indexOfAddresses[address];
				}

				#endregion

				private uint[] LocalReceivers
				{
					get
					{						
						return localReceivers;
					}
				}

				#endregion

				#region Processing of received Messages

				public bool processReceivedMessage(QS.Fx.Network.NetworkAddress networkAddress, Message message, Base.IDemultiplexer demultiplexer)
				{
// #if DEBUG_VSSender
//					logger.Log(this, this.ToString() + ".processReceivedMessage(" + networkAddress.ToString() + ", " + message.ToString());
// #endif

					uint indexOfTheSender = UInt32.MaxValue;
					if (lookupNetworkAddress(networkAddress, ref indexOfTheSender))
					{
						IncomingWindow incomingWindow = incomingWindows[indexOfTheSender];
						
						if (!incomingWindow.accepts(message.withinViewSeqNo))
						{
							// must be either old or too far ahead
							return false;
						}

						IncomingSlot incomingSlot = incomingWindow.lookup(message.withinViewSeqNo);

						if (incomingSlot != null)
						{
							if (message.protocolPhase > incomingSlot.CurrentPhase)
							{
// #if DEBUG_VSSender
// 								logger.Log(this, "new phase : " + message.protocolPhase.ToString());
// #endif

								incomingSlot.CurrentPhase = message.protocolPhase;
								switch (message.protocolPhase)
								{ 
									case ProtocolPhase.MESSAGE_DELIVERED_EVERYWHERE:
									{
										incomingSlot.DisposeAllowed = true;
										if (incomingWindow.alreadyConsumed(message.withinViewSeqNo))
											incomingSlot.disposeOfApplicationData();										
									}
									break;

									case ProtocolPhase.MESSAGE_CLEANUP_COMPLETE:
									{
										incomingSlot.CleanupAllowed = true;
										incomingWindow.cleanupWhateverYouCan();
									}
									break;

									default:
									{
										Debug.Assert(false);
									}
									break;
								}					

								return true;
							}
// #if DEBUG_VSSender
// 							else
//								logger.Log(this, "same phase, ignoring");
// #endif
						}
						else
						{
							if (message.protocolPhase == ProtocolPhase.INITIALLY_SENDING_MESSAGE)
							{
								if (incomingWindow.accepts(message.withinViewSeqNo))
								{
									bool deliveryAllowed = (state == State.INSTALLED) || ((state == State.FLUSHING) && !peerStates[indexOfTheSender].Crashed);

#if DEBUG_VSSender
									logger.Log(this, "deliveryAllowed : " + deliveryAllowed.ToString());
#endif

									incomingSlot = new IncomingSlot(new Base.ObjectAddress(networkAddress, message.senderLOID), message.message, deliveryAllowed);
									
									incomingWindow.insert(message.withinViewSeqNo, incomingSlot);
									
									while ((incomingWindow.NumberConsumed < incomingWindow.NumberReceived) && incomingWindow.NextConsumable.DeliveryAllowed)
									{
#if DEBUG_VSSender
										logger.Log(this, "CONSUMING");
#endif

										incomingSlot = incomingWindow.consume();
										
										foreach (uint receiverLOID in LocalReceivers)
										{
#if DEBUG_VSSender
											logger.Log(this, "PASSING TO LOCAL RECEIVED : " + receiverLOID.ToString());
#endif

											demultiplexer.demultiplex(receiverLOID, incomingSlot.SenderAddress, incomingSlot.Data);
										}

										if (incomingSlot.DisposeAllowed)
											incomingSlot.disposeOfApplicationData();
									}

									incomingWindow.cleanupWhateverYouCan();

									checkPeerForReadiness(indexOfTheSender);
									
									return true;
								}
							}
						}
					}

					return false;
				}

				#endregion

				#region State of a Peer

				private class StateOfAPeer
				{
					public StateOfAPeer(uint numberOfMembers)
					{
						this.numberOfMembers = numberOfMembers;
						crashed = totalNumberOfMessagesSentArrived = numbersOfMessagesConsumedFromAllLiveNodesArrived = false;
						totalNumberOfMessagesSent = maximumConsumedAmongAlive = 0;
						numberOfConsumersToReceiveStateFrom = numberOfMembers;
					}

					private bool crashed, totalNumberOfMessagesSentArrived, numbersOfMessagesConsumedFromAllLiveNodesArrived, flushingReady;
					private uint totalNumberOfMessagesSent, maximumConsumedAmongAlive, numberOfMembers, numberOfConsumersToReceiveStateFrom;
				
					private uint[] numbersOfMessagesConsumed = null;
					private bool[] numbersOfMessagesConsumedAreKnown = null;
					private Collections.IPriorityQueue consumedQueue = null;

					public override string ToString()
					{
						return "(crash=" + crashed.ToString() + ";ksent=" + totalNumberOfMessagesSentArrived.ToString() + ";kcons=" + 
							numbersOfMessagesConsumedFromAllLiveNodesArrived.ToString() + ";ready=" + flushingReady.ToString() + ";tsent=" + totalNumberOfMessagesSent.ToString() +
							";mcons=" + maximumConsumedAmongAlive.ToString() + ";cleft=" + numberOfConsumersToReceiveStateFrom.ToString() + ")";
					}

					public bool Ready
					{
						set
						{
							flushingReady = value;
						}

						get
						{
							return flushingReady;
						}
					}

					private void ensureExistenceOf_numbersOfMessagesConsumed()
					{
						if (numbersOfMessagesConsumedAreKnown == null)
						{
							numbersOfMessagesConsumedAreKnown = new bool[numberOfMembers];
							for (uint ind = 0; ind < numberOfMembers; ind++)
								numbersOfMessagesConsumedAreKnown[ind] = false;
						}
						if (numbersOfMessagesConsumed == null)
							numbersOfMessagesConsumed = new uint[numberOfMembers];

						if (consumedQueue == null)
							consumedQueue = new Collections.BHeap(5, 2);
					}

					public void markAsCrashed(uint[] indexesOfAllCrashedMembers)
					{
						Debug.Assert(!crashed);
						crashed = true;
						ensureExistenceOf_numbersOfMessagesConsumed();
						markMembersAsCrashed(indexesOfAllCrashedMembers);
					}

					public void updateWithCrashes(uint[] indexesOfCrashedAddresses)
					{
						Debug.Assert(crashed);
						markMembersAsCrashed(indexesOfCrashedAddresses);
					}

					private void markMembersAsCrashed(uint[] indexesOfCrashedAddresses)
					{
						for (uint ind = 0; ind < indexesOfCrashedAddresses.Length; ind++)
						{
							uint indexOfAMember = indexesOfCrashedAddresses[ind];
							if (!numbersOfMessagesConsumedAreKnown[indexOfAMember])
							{
								numberOfConsumersToReceiveStateFrom--;
								numbersOfMessagesConsumed[indexOfAMember] = 0;
								numbersOfMessagesConsumedAreKnown[indexOfAMember] = true;

								if (numberOfConsumersToReceiveStateFrom == 0)
									numbersOfMessagesConsumedFromAllLiveNodesArrived = true;
							}
							else
							{
								numbersOfMessagesConsumed[indexOfAMember] = 0;
								recalculateMaximumConsumedAmongAlive();
							}
						}

					}

					private void recalculateMaximumConsumedAmongAlive()
					{
						maximumConsumedAmongAlive = 0;
						for (uint ind = 0; ind < numberOfMembers; ind++)
						{
							if (numbersOfMessagesConsumed[ind] > maximumConsumedAmongAlive)
								maximumConsumedAmongAlive = numbersOfMessagesConsumed[ind];
						}
					}

					public void updateWithNumberOfMessagesConsumed(uint indexOfAConsumer, uint numberOfMessagesConsumed)
					{
						ensureExistenceOf_numbersOfMessagesConsumed();
						if (!numbersOfMessagesConsumedAreKnown[indexOfAConsumer])
						{
							numberOfConsumersToReceiveStateFrom--;
							numbersOfMessagesConsumed[indexOfAConsumer] = numberOfMessagesConsumed;
							numbersOfMessagesConsumedAreKnown[indexOfAConsumer] = true;

							if (numberOfMessagesConsumed > maximumConsumedAmongAlive)
								maximumConsumedAmongAlive = numberOfMessagesConsumed;

							if (numberOfConsumersToReceiveStateFrom == 0)
								numbersOfMessagesConsumedFromAllLiveNodesArrived = true;
						}
					}

					public bool FlushingInformationReady
					{
						get
						{
							return (!crashed && totalNumberOfMessagesSentArrived) || (crashed && numbersOfMessagesConsumedFromAllLiveNodesArrived);
						}
					}

					public uint NumberOfMessagesRequired
					{
						get
						{
							return crashed ? maximumConsumedAmongAlive : totalNumberOfMessagesSent;
						}
					}

					public uint NumberOfMessagesSent
					{
						set
						{
							totalNumberOfMessagesSent = value;
							totalNumberOfMessagesSentArrived = true;
						}

//						get
//						{
//							return totalNumberOfMessagesSent;
//						}
					}

					public bool Crashed
					{
						get
						{
							return crashed;
						}
					}
				}

				#endregion

				#region IncomingSlot

				private class IncomingSlot
				{
					public IncomingSlot(Base.ObjectAddress senderAddress, Base.IMessage message, bool deliveryAllowed)
					{
						this.senderAddress = senderAddress;
						this.message = message;
						this.protocolPhase = ProtocolPhase.INITIALLY_SENDING_MESSAGE;
						this.deliveryAllowed = deliveryAllowed;
						this.disposeAllowed = this.cleanupAllowed = false;
					}

					private Base.ObjectAddress senderAddress;
					private Base.IMessage message;
					private ProtocolPhase protocolPhase;
					private bool deliveryAllowed, disposeAllowed, cleanupAllowed;

					public override string ToString()
					{
						return "(" + senderAddress.ToString() + " p" + ((uint) protocolPhase).ToString() + ")";
					}

					public bool CleanupAllowed
					{
						set
						{
							cleanupAllowed = value;
						}

						get
						{
							return cleanupAllowed;
						}
					}

					public bool DisposeAllowed
					{
						set
						{
							disposeAllowed = value;
						}

						get
						{
							return disposeAllowed;
						}
					}

					public bool DeliveryAllowed
					{
						get
						{
							return deliveryAllowed;
						}
					}

					public void disposeOfApplicationData()
					{
						message = null;
					}

					#region Accessors

					public ProtocolPhase CurrentPhase
					{
						get
						{
							return protocolPhase;
						}

						set
						{
							protocolPhase = value;
						}
					}

					public Base.IAddress SenderAddress
					{
						get
						{
							return senderAddress;
						}
					}

					public Base.IMessage Data
					{
						get
						{
							return message;
						}

					}

					#endregion
				}

				#endregion

				#region IncomingWindow

				private class IncomingWindow
				{
					public IncomingWindow(uint initialWindowSize)
					{
						fcWindow = new FlowControl.IncomingWindow(initialWindowSize);
					}

					private FlowControl.IIncomingWindow fcWindow;

					public override string ToString()
					{
						return fcWindow.ToString();
					}

					public string AllToString
					{
						get
						{
							return fcWindow.AllToString;
						}
					}

					public  void cleanupWhateverYouCan()
					{
						while (OldestConsumed && Oldest.CleanupAllowed) 
							cleanupOldest();
					}

					private IncomingSlot Oldest
					{
						get
						{
							return (IncomingSlot) fcWindow.first();
						}
					}

					private bool OldestConsumed
					{
						get
						{
							return fcWindow.firstConsumed();
						}
					}

					private void cleanupOldest()
					{
						fcWindow.cleanupOneGuy();
					}

					public IncomingSlot lookup(uint seqno)
					{
						return (IncomingSlot) fcWindow.lookup(seqno);
					}

					public bool accepts(uint seqno)
					{
						return fcWindow.accepts(seqno);
					}

					public void insert(uint seqno, IncomingSlot incomingSlot)
					{
						fcWindow.insert(seqno, incomingSlot);
					}

					public IncomingSlot consume()
					{
						return (IncomingSlot) fcWindow.consume();
					}

					public bool alreadyConsumed(uint seqno)
					{
						return fcWindow.consumed(seqno);
					}

					public IncomingSlot NextConsumable
					{
						get
						{
							return (IncomingSlot) fcWindow.NextConsumable;
						}
					}

					/// <summary>
					/// The number of items with subsequent sequence numbers starting from one that have been consumed or are
					/// waiting ready to be consumed. This is not the same as the number of items that have been inserted into
					/// the window, as some of the items received may not be consumable, hence not immediately deliverable...
					/// </summary>
					public uint NumberReceived
					{
						get
						{
							return fcWindow.lastConsumableSeqNo();
						}
					}

					/// <summary>
					/// The number of items in the window that have been consumed so far.
					/// </summary>
					public uint NumberConsumed
					{
						get
						{
							return fcWindow.lastConsumedSeqNo();
						}
					}
				}

				#endregion
			}

			#endregion

			#region Definition of OutgoingSlot

			private class OutgoingSlot : Collections.GenericLinkable
			{
				public OutgoingSlot(IOutgoingMessageRef messageRef)
				{
					this.messageRef = messageRef;
					this.group = null;
					this.view = null;
					this.protocolPhase = ProtocolPhase.INITIALLY_SENDING_MESSAGE;
					this.multicastRequestRef = null;
				}

				private ProtocolPhase protocolPhase;
				private IOutgoingMessageRef messageRef;
				private Group group;
				private View view;
				private uint withinViewSeqNo;

				private Multicasting.IMulticastRequestRef multicastRequestRef;

				public override string ToString()
				{
					return "(" + group.GroupID.ToString() + ":" + view.MembershipView.SeqNo.ToString() + "-" + withinViewSeqNo.ToString() + " p" + ((uint) protocolPhase).ToString() + ")";
				}

				public void acknowledge(QS.Fx.Network.NetworkAddress networkAddress)
				{
					try
					{
						if (multicastRequestRef != null)
						{
							multicastRequestRef.confirm(view.IndexOf(networkAddress));
						}
					}
					catch (Exception)
					{
					}
				}

//				public IMulticastRequestRef MulticastRequestRef
//				{
//					get
//					{
//						return multicastRequestRef;
//					}
//				}

				public ProtocolPhase CurrentPhase
				{
//					set
//					{
//						protocolPhase = value;
//					}

					get
					{
						return protocolPhase;
					}
				}

				public void registerInGroup(Group group, View view, uint withinViewSeqNo)
				{
					if (this.group != null)
						throw new Exception("already registered in group");

					this.group = group;
					this.view = view;
					this.withinViewSeqNo = withinViewSeqNo;

					GMS.IView membershipView = view.MembershipView;
				}

				public bool initiateNextProtocolPhase()
				{
					switch (protocolPhase)
					{
						case ProtocolPhase.INITIALLY_SENDING_MESSAGE:
						{
							protocolPhase = ProtocolPhase.MESSAGE_DELIVERED_EVERYWHERE;
							disposeOfMessage();
						}
						break;

						case ProtocolPhase.MESSAGE_DELIVERED_EVERYWHERE:
						{
							protocolPhase = ProtocolPhase.MESSAGE_CLEANUP_COMPLETE;
						}
						break;

						case ProtocolPhase.MESSAGE_CLEANUP_COMPLETE:
						{
							protocolPhase = ProtocolPhase.ALL_PHASES_COMPLETE;
							return false;
						}
						// break;
					}

					return true;
				}

				private void disposeOfMessage()
				{
				}

	//			public uint SeqNo
	//			{
	//				get
	//				{
	//					return withinViewSeqNo;
	//				}
	//			}

				public void multicast(SendCallback sendCallback)
				{
					multicastRequestRef = sendCallback(view, this.CreateMessage);
				}

				private Message CreateMessage
				{
					get
					{
						return new Message(messageRef.SenderLOID, messageRef.GroupID, view.MembershipView.SeqNo, withinViewSeqNo, protocolPhase, 
							messageRef.Message, messageRef.CausallyOrdered, messageRef.TotallyOrdered, messageRef.DeliveredAtomically);
					}
				}
			}

			#endregion

			#region Definition of OutgoingWindow

			private class OutgoingWindow
			{
				public OutgoingWindow(uint initialWindowSize)
				{
					fcWindow = new FlowControl.OutgoingWindow(initialWindowSize);
				}

				public string AllToString
				{
					get
					{
						return fcWindow.AllToString;
					}
				}

				public override string ToString()
				{
					return fcWindow.ToString();
				}

				private FlowControl.IOutgoingWindow fcWindow;

				public uint append(OutgoingSlot outgoingSlot)
				{
					return fcWindow.append(outgoingSlot);
				}

				public bool HasSpace
				{
					get
					{
						return fcWindow.hasSpace(); 
					}
				}

				public OutgoingSlot lookup(uint seqno)
				{
					return (OutgoingSlot) fcWindow.lookup(seqno);
				}

				public void remove(uint seqno)
				{
					fcWindow.remove(seqno);
				}
			}

			#endregion

			public void cleanup()
			{
				// ...should do something
			}
		}

		// ----------------------------------------------------------------------------------------------------------------------

		#region Helpers

		private bool localInstanceInView(GMS.IView view)
		{
			for (uint ind = 0; ind < view.NumberOfSubViews; ind++)
			{
				if (view[ind].IPAddress.Equals(localAddress.HostIPAddress) && view[ind].PortNumber.Equals(localAddress.PortNumber))
					return true;
			}
			return false;
		}

		#endregion

		#region IViewController Members

		public void viewChangeRequest(QS.GMS.GroupId gid, QS.GMS.IView view)
		{
			// Debug.Assert(false, "view change request");

#if DEBUG_VSSender
			logger.Log(this, "viewChangeRequest_enter : GID = " + gid.ToString() + ", View = " + view.ToString());
#endif

			if (!localInstanceInView(view))
			{
				viewChangeCleanup(gid);
			}
			else
			{
				Group group = null;

				lock (groups)
				{
					Collections.IDictionaryEntry dic_en = groups.lookupOrCreate(gid);
					if (dic_en.Value == null)
					{
						group = new Group(gid, new SendCallback(this.sendCallback), logger, viewChangeGoAhead);
						dic_en.Value = group;
					}
					else
						group = (Group) dic_en.Value;
				}

				group.initiateViewChange(view, localAddress, flushingDevice, viewChangeGoAhead);

				logger.Log(this, "initiateViewChange completed");

				
				// TODO:  Add VSSender.viewChangeRequest implementation





				// .............................................


			}

#if DEBUG_VSSender
			string debug_string = "viewChangeRequest_leave : GID = " + gid.ToString() + ", ViewSeqNo = " + view.SeqNo.ToString() + "\nGroups : \n";
			foreach (Group group in groups.Values)
				debug_string = debug_string + group.AllToString;
			logger.Log(this,  debug_string + "\n");
#endif
		}

		public void viewChangeAllDone(QS.GMS.GroupId gid, uint seqno)
		{
#if DEBUG_VSSender
			logger.Log(this, "enter viewChangeAllDone(" + gid.ToString() + ", " + seqno.ToString() + ")");
#endif

			Group group = lookupGroup(gid);
			if (group != null)
				group.deliverAllTheRestAfterFlushing(seqno, demultiplexer);
		}

		public void viewChangeCleanup(QS.GMS.GroupId gid)
		{
			lock (groups)
			{
				Collections.IDictionaryEntry dic_en = groups.remove(gid);
				if (dic_en != null)
				{
					Group group = (Group) dic_en.Value;					
					lock (group)
					{
						group.cleanup();
					}
				}
			}
		}

		public void registerGMSCallbacks(GMS.ViewChangeGoAhead viewChangeGoAhead)
		{
			this.viewChangeGoAhead = viewChangeGoAhead;
		}

		#endregion

		// -----------------------------------------------------------------------------------------------------------------------------------

		#region IFlushingReportConsumer members

		public void incorporateFlushingReport(QS.Fx.Network.NetworkAddress reportSender, Flushing.FlushingReport flushingReport)
		{
			Group group = lookupGroup(flushingReport.GroupID);
			if (group != null)
			{
				Collections.IBiLinkableCollection forwardingRequests = group.incorporateFlushingReport(reportSender, flushingReport);
				if (forwardingRequests != null)
				{
					while (forwardingRequests.Count > 0)
					{
						ForwardingRequest forwardingRequest = (ForwardingRequest) forwardingRequests.elementAtHead();
						forwardingRequests.remove(forwardingRequest);

						// for now, we just send directly...
						forwardingSender.send(this, new Base.ObjectAddress(forwardingRequest.DestinationAddress, this.LocalObjectID), 
							forwardingRequest.MessageToForward, null);
					}
				}
			}
		}

		#endregion

		#region Finders

		private Group lookupGroup(GMS.GroupId groupID)
		{
			Group group = null;
			lock (groups)
			{
				Collections.IDictionaryEntry dic_en = groups.lookup(groupID);
				if (dic_en != null)
				{
					group = (Group) dic_en.Value;
					Monitor.Enter(group);
				}
			}

			return group;
		}

		#endregion

		#region Multicasting

		private void multicast(IOutgoingMessageRef messageRef)
		{
#if DEBUG_VSSender
			logger.Log(this, "called multicast(" + messageRef.ToString() + ")");
#endif

			Group group = lookupGroup(messageRef.GroupID);
//			logger.Log(this, "lookupGroup succeeded");

			if (group != null)
			{
//				logger.Log(this, "registering outgoing");
				group.registerOutgoing(messageRef);
			}
#if DEBUG_VSSender
			else
			{
				logger.Log(this, "no such group is registered on this node");
			}
#endif
		}

		private delegate Multicasting.IMulticastRequestRef SendCallback(Multicasting.IAddressSet addresses, Message message);

		private Multicasting.IMulticastRequestRef sendCallback(Multicasting.IAddressSet addresses, Message message)
		{
			return multicastingDevice.multicast(this, addresses, this.LocalObjectID, message, 
				new Multicasting.MulticastRequestCallback(this.multicastCompletionCallback));
		}

		private void multicastCompletionCallback(Multicasting.IMulticastRequestRef requestRef, bool succeeded)
		{
			Debug.Assert(succeeded);
			Message message = (Message) requestRef.Message;
			Group group = lookupGroup(message.groupID);
			if (group != null)
				group.acknowledgementComplete(message.viewSeqNo, message.withinViewSeqNo);
		}

		#endregion

		#region Processing of Messages

		private bool processReceivedMessage(QS.Fx.Network.NetworkAddress networkAddress, Message message)
		{
			Group group = lookupGroup(message.groupID);
			if (group != null)
			{
				return group.processReceivedMessage(networkAddress, message, demultiplexer);
			}
			else
				return false;
		}

		#endregion

		#region Receive callback and processing of ACKs

		private void processAcknowledgement(QS.Fx.Network.NetworkAddress networkAddress, Acknowledgement ack)
		{
			Group group = lookupGroup(ack.groupID);
			if (group != null)
				group.processAcknowledgement(ack.viewSeqNo, ack.withinViewSeqNo, ack.protocolPhase, networkAddress);
		}

		private void receiveCallback(Base.IAddress source, Base.IMessage message)
		{
			if (!(source is Base.ObjectAddress))
				throw new Exception("unknown source address type");			
			Base.ObjectAddress sourceObjectAddress = (Base.ObjectAddress) source;

// #if DEBUG_VSSender
// 			logger.Log(this, "receiveCalback : " + source.ToString() + ", " + message.ToString());
// #endif

			if (message is ForwardingRequest.ForwardedMessage)
			{				
				ForwardingRequest.ForwardedMessage fmsg = (ForwardingRequest.ForwardedMessage) message;
				processReceivedMessage(fmsg.SourceAddress, fmsg.ReceivedMessage);
			} 

			if (message is Message)
			{
				Message vsMessage = (Message) message;
				if (processReceivedMessage((QS.Fx.Network.NetworkAddress) sourceObjectAddress, vsMessage))
				{
					// for now, just simple sending back will do...
					acknowledgementSender.send(this, sourceObjectAddress, vsMessage.CreateAcknowledgement, null);
				}
			}
			else if (message is Acknowledgement)
			{
				processAcknowledgement((QS.Fx.Network.NetworkAddress) sourceObjectAddress, (Acknowledgement) message);
			}
			else
				throw new Exception("unrecognized message type");
		} 


		#endregion

		#region IOutgoingMessageRef and OutgoingMessageRef

		private interface IOutgoingMessageRef
		{
			uint SenderLOID
			{
				get;
			}

			GMS.GroupId GroupID
			{
				get;
			}

			Base.IMessage Message
			{
				get;
			}

//			bool Cancelled
//			{
//				get;
//			}

			bool CausallyOrdered
			{
				get;
			}

			bool TotallyOrdered
			{
				get;
			}

			bool DeliveredAtomically
			{
				get;
			}

			void succeeded();
			void failed(Exception exc);
		}

		private class OutgoingMessageRef : Base.IMessageReference, IOutgoingMessageRef
		{
			public OutgoingMessageRef(Base.IClient sender, GMS.GroupId groupID, Base.IMessage message, Base.SendCallback sendCallback,
				bool causallyOrdered, bool totallyOrdered, bool deliveredAtomically)
			{
				this.sender = sender;
				this.groupID = groupID;
				this.message = message;
				this.sendCallback = sendCallback;

				cancelled = ignored = false;

				this.causallyOrdered = causallyOrdered;
				this.totallyOrdered = totallyOrdered;
				this.deliveredAtomically = deliveredAtomically;
			}

			private Base.IClient sender;
			private GMS.GroupId groupID;
			private Base.IMessage message;
			private Base.SendCallback sendCallback;

			private bool causallyOrdered, totallyOrdered, deliveredAtomically;
			private bool cancelled, ignored;

			public override string ToString()
			{
				return "OutgoingRef(" + sender.LocalObjectID.ToString() + " to " + groupID.ToString() + "; message : " + message.ToString() + ")";
			}

			#region IOutgoingMessageRef Members

			public void succeeded()
			{
				if (!ignored)
					sendCallback(this, true, null);
			}

			public void failed(Exception exc)
			{
				if (!ignored)
					sendCallback(this, false, exc);
			}

			public bool CausallyOrdered
			{
				get
				{
					return causallyOrdered;
				}
			}

			public bool TotallyOrdered
			{
				get
				{
					return totallyOrdered;
				}
			}

			public bool DeliveredAtomically
			{
				get
				{
					return deliveredAtomically;
				}
			}

//			public bool Cancelled
//			{
//				get
//				{
//					return cancelled;
//				}
//			}

			public uint SenderLOID
			{
				get
				{					
					return sender.LocalObjectID;
				}
			}

			public GMS.GroupId GroupID
			{
				get
				{
					return groupID;
				}
			}

			#endregion

			#region IMessageReference Members

			public QS.CMS.Base.IClient Sender
			{
				get
				{
					return sender;
				}
			}

			public QS.CMS.Base.IAddress Address
			{
				get
				{				
					return new Base.GroupAddress(groupID);
				}
			}

			public QS.CMS.Base.IMessage Message
			{
				get
				{
					return message;
				}
			}

			public void cancel()
			{
				cancelled = true;
			}

			public void ignore()
			{
				ignored = true;
			}

			#endregion
		}

		#endregion

		#region ISender Members

		public QS.CMS.Base.IMessageReference send(QS.CMS.Base.IClient theSender, QS.CMS.Base.IAddress destinationAddress, 
			QS.CMS.Base.IMessage message, QS.CMS.Base.SendCallback sendCallback)
		{
			try
			{
				if (!(destinationAddress is Base.GroupAddress))
					throw new Exception("Wrong address type!");

				OutgoingMessageRef messageRef = new OutgoingMessageRef(theSender, ((Base.GroupAddress) destinationAddress).GroupID, 
					message, sendCallback, false, false, false);

				this.multicast(messageRef);
				return messageRef;
			}
			catch (Exception exc)
			{
				logger.Log(this, "Cannot send message: " + exc.ToString());
				return null;
			}
		}

		#endregion

		#region Message and Acknowledgement

		private class Message : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new Message();
			}

			public Message()
			{
			}

			public Message(uint senderLOID, GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo, 
				ProtocolPhase protocolPhase, Base.IMessage message, bool causallyOrdered, bool totallyOrdered, bool deliveredAtomically)
			{
				this.causallyOrdered = causallyOrdered;
				this.totallyOrdered = totallyOrdered;
				this.deliveredAtomically = deliveredAtomically;
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.withinViewSeqNo = withinViewSeqNo;
				this.senderLOID = senderLOID;
				this.protocolPhase = protocolPhase;
				this.message = message;
			}

			public override string ToString()
			{
				// Debug.Assert((groupID != null), "group is null");
				// Debug.Assert((message != null), "message is null");

				return "(" + senderLOID.ToString() + "->" + groupID.ToString() + ":" + viewSeqNo.ToString() + ";seqno=" + withinViewSeqNo.ToString() + ";phase=" + 
					protocolPhase.ToString() + ((message != null) ? (";message=" + message.ToString()) : "; no message") + ")";
			}

			public ProtocolPhase protocolPhase;
			public Base.IMessage message;
			public bool causallyOrdered, totallyOrdered, deliveredAtomically;
			public GMS.GroupId groupID;
			public uint senderLOID, viewSeqNo, withinViewSeqNo;
	
			public Acknowledgement CreateAcknowledgement
			{
				get
				{
					return new Acknowledgement(groupID, viewSeqNo, withinViewSeqNo, protocolPhase);
				}
			}

			#region ISerializable Members

			public void save(Stream stream)
			{
				groupID.save(stream);
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(viewSeqNo);
				stream.Write(buffer, 0, buffer.Length);								
				buffer = System.BitConverter.GetBytes(withinViewSeqNo);
				stream.Write(buffer, 0, buffer.Length);				
				buffer = System.BitConverter.GetBytes(senderLOID);
				stream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes((ushort) protocolPhase);
				stream.Write(buffer, 0, buffer.Length);		
				buffer = System.BitConverter.GetBytes(causallyOrdered);
				stream.Write(buffer, 0, buffer.Length);		
				buffer = System.BitConverter.GetBytes(totallyOrdered);
				stream.Write(buffer, 0, buffer.Length);		
				buffer = System.BitConverter.GetBytes(deliveredAtomically);
				stream.Write(buffer, 0, buffer.Length);		

				if (protocolPhase == ProtocolPhase.INITIALLY_SENDING_MESSAGE)
				{
					buffer = System.BitConverter.GetBytes((ushort) message.ClassIDAsSerializable);
					stream.Write(buffer, 0, buffer.Length);			
					message.save(stream);
				}
			}

			public void load(Stream stream)
			{
				groupID = new GMS.GroupId();
				groupID.load(stream);
				byte[] buffer = new byte[17];
				stream.Read(buffer, 0, 17);
				viewSeqNo = System.BitConverter.ToUInt32(buffer, 0);
				withinViewSeqNo = System.BitConverter.ToUInt32(buffer, 4);
				senderLOID = System.BitConverter.ToUInt32(buffer, 8);
				protocolPhase = (ProtocolPhase) System.BitConverter.ToUInt16(buffer, 12);
				causallyOrdered = System.BitConverter.ToBoolean(buffer, 14);
				totallyOrdered = System.BitConverter.ToBoolean(buffer, 15);
				deliveredAtomically = System.BitConverter.ToBoolean(buffer, 16);

				if (protocolPhase == ProtocolPhase.INITIALLY_SENDING_MESSAGE)
				{
					stream.Read(buffer, 0, 2);
					ushort messageClassID = System.BitConverter.ToUInt16(buffer, 0);								
					message = (Base.IMessage) Base.Serializer.Get.createObject((ClassID) messageClassID);
					message.load(stream); 
				}
			}

			public ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.VSSender_Message;
				}
			}

			#endregion
		}

		private class Acknowledgement : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new Acknowledgement();
			}

			private Acknowledgement()
			{
			}

			public Acknowledgement(GMS.GroupId groupID, uint viewSeqNo, uint withinViewSeqNo, ProtocolPhase protocolPhase)
			{
				this.groupID = groupID;
				this.viewSeqNo = viewSeqNo;
				this.withinViewSeqNo = withinViewSeqNo;
				this.protocolPhase = protocolPhase;
			}

			public override string ToString()
			{
				return "(" + groupID.ToString() + ":" + viewSeqNo.ToString() + ";seqno=" + withinViewSeqNo.ToString() + ";phase=" + 
					protocolPhase.ToString() + ")";
			}

			public ProtocolPhase protocolPhase;
			public GMS.GroupId groupID;
			public uint viewSeqNo, withinViewSeqNo;

			#region ISerializable Members

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.VSSender_Acknowledgement;
				}
			}

			public void save(Stream stream)
			{
				groupID.save(stream);
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(viewSeqNo);
				stream.Write(buffer, 0, buffer.Length);								
				buffer = System.BitConverter.GetBytes(withinViewSeqNo);
				stream.Write(buffer, 0, buffer.Length);				
				buffer = System.BitConverter.GetBytes((ushort) protocolPhase);
				stream.Write(buffer, 0, buffer.Length);		
			}

			public void load(Stream stream)
			{
				groupID = new GMS.GroupId();
				groupID.load(stream);
				byte[] buffer = new byte[10];
				stream.Read(buffer, 0, 10);
				viewSeqNo = System.BitConverter.ToUInt32(buffer, 0);
				withinViewSeqNo = System.BitConverter.ToUInt32(buffer, 4);
				protocolPhase = (ProtocolPhase) System.BitConverter.ToUInt16(buffer, 8);
			}

			#endregion
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.VSSender;
			}
		}

		#endregion
	}
*/
}
