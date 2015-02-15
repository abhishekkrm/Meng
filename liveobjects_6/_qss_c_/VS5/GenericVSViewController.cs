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

// #define DEBUG_GenericVSViewController

using System;
using System.Threading;
using System.Diagnostics;

namespace QS._qss_c_.VS5
{
/*
	/// <summary>
	/// Summary description for ViewController.
	/// </summary>
	public abstract class GenericVSViewController : IVSViewController, IVSSender, Base.IClient
	{
		public GenericVSViewController(QS.Fx.Logging.ILogger logger, Base2.IDemultiplexer demultiplexer)
		{
			GenericVSView.register_serializable();

			Base2.Serializer.CommonSerializer.registerClass((ushort) QS.ClassID.VSMessage, typeof(GenericVSMessage));
			Base2.Serializer.CommonSerializer.registerClass(
				QS.ClassID.GenericRCCDescriptor, typeof(GenericRCCDescriptor));

			this.groupCollection = new Collections2.SynchronizedRBT(
				new Collections.HashedSplaySet(100), logger);
			this.logger = logger;
			demultiplexer.register(this.LocalObjectID, new Base2.ReceiveCallback(this.receiveCallback));

			outgoingIOCWrapper = new QS.CMS.Base2.IOCWrapper(new Base2.IOCLookupCallback(this.lookupOutgoing));
			incomingIOCWrapper = new QS.CMS.Base2.IOCWrapper(new Base2.IOCLookupCallback(this.lookupIncoming));
		}

		protected QS.Fx.Logging.ILogger logger;

		private Collections2.ISynchronizedRBT groupCollection;	
		private GMS.ViewChangeGoAhead readyToInstallCallback;
		private GroupReadyForCleanup readyToCleanupCallback;
		private Base2.IOCWrapper outgoingIOCWrapper, incomingIOCWrapper;

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, Base2.IBase2Serializable serializableObject)
		{
			IVSMessage message = (IVSMessage) serializableObject;
			IVSGroup group = this.lookupGroup(message.ID.GroupID);
			if (group != null)
				group.receive(sourceAddress, message);
			else
				throw new Exception("Group does not exist.");

			return null;
		}

		#region Base2.IIdentifiableObjectContainer Wrappers and Callbacks

		public Base2.IIdentifiableObjectContainer OutgoingIOC
		{
			get
			{
				return this.outgoingIOCWrapper;
			}
		}

		public Base2.IIdentifiableObjectContainer IncomingIOC
		{
			get
			{
				return this.incomingIOCWrapper;
			}
		}

		private Base2.IIdentifiableObject lookupOutgoing(QS.ClassID classID, Base2.IIdentifiableKey uniqueID)
		{
#if DEBUG_GenericVSViewController
			logger.Log(this, "lookupOutgoing_enter");
#endif

			IVSMessageID messageID = (IVSMessageID) uniqueID;
			IVSGroup group = this.lookupGroup(messageID.GroupID);

#if DEBUG_GenericVSViewController
			logger.Log(this, "lookupOutgoing_foundGroup(" + messageID.GroupID.ToString() + ")");
#endif

			if (group != null)
			{
				Base2.IIdentifiableObject result = group.lookupOutgoing(messageID);
				// Monitor.Exit(group);

#if DEBUG_GenericVSViewController
				logger.Log(this, "lookupOutgoing_leave");
#endif

				return result;
			}
			else 
				return null;
		}

		private Base2.IIdentifiableObject lookupIncoming(QS.ClassID classID, Base2.IIdentifiableKey uniqueID)
		{
			IVSMessageID messageID = (IVSMessageID) uniqueID;
			IVSGroup group = this.lookupGroup(messageID.GroupID);
			if (group != null)
			{
				Base2.IIdentifiableObject result = group.lookupIncoming(messageID);
				Monitor.Exit(group);

				return result;
			}
			else 
				return null;
		}

		#endregion

		#region Managing the collection of Groups

		protected abstract IVSGroup createGroup(GMS.GroupId groupID, GMS.IView membershipView, QS.Fx.Logging.ILogger logger);

		protected IVSGroup lookupGroup(GMS.GroupId groupID)
		{
			return (IVSGroup) groupCollection.lookup(groupID);
		}

		#endregion

		#region IVSSender Members

		public void send(QS.GMS.GroupId groupID, QS.CMS.Base2.IBase2Serializable serializableObject,
			Scattering.CompletionCallback completionCallback)
		{
			IVSGroup group = (IVSGroup) groupCollection.lookup(groupID);
			if (group != null)
			{
				group.multicast(serializableObject, completionCallback);
			}
			else
				throw new Exception("No such group exists!");
		}

		#endregion

		#region Class CallbackWrapper

		private struct CallbackWrapper
		{
			public CallbackWrapper(GenericVSViewController viewController, GMS.GroupId groupID, 
				GMS.IView membershipView)
			{
				this.viewController = viewController;
				this.groupID = groupID;
				this.membershipView = membershipView;
			}	

			private GenericVSViewController viewController;
			private GMS.GroupId groupID;
			private GMS.IView membershipView;

			private Collections.IBinaryTreeNode createCallback(System.IComparable key)
			{
				Debug.Assert(key.Equals(groupID));
				return viewController.createGroup(groupID, membershipView, viewController.logger);
			}

			public Collections2.CreateBinaryTreeNodeCallback Callback
			{
				get
				{
					return new Collections2.CreateBinaryTreeNodeCallback(this.createCallback);
				}
			}	
		}

		#endregion

		#region IVSViewController Members

		public void requestViewChange(GMS.GroupId gid, GMS.IView view)
		{
#if DEBUG_GenericVSViewController
			logger.Log(this, "_requestViewChange: " + gid.ToString() + ":" + view.SeqNo.ToString());
#endif

			bool createdAnew;
			IVSGroup group = (IVSGroup) groupCollection.lookupOrCreate(gid, 
				(new CallbackWrapper(this, gid, view)).Callback, out createdAnew);

			if (group != null)
			{
				if (createdAnew)	
				{
#if DEBUG_GenericVSViewController
					logger.Log(this, "_requestViewChange: Added a new group " + gid.ToString());
#endif

					Monitor.Exit(group);
				}
				else
				{
#if DEBUG_GenericVSViewController
					logger.Log(this, "_requestViewChange: Modifying group " + gid.ToString());
#endif

					group.requestViewChange(view);
				}
			}
			else
			{
				logger.Log(this, "Cannot perform the requested view change: cannot create a group.");
				throw new Exception("Cannot create a group");
			}
		}
		
		public void installmentComplete(GMS.GroupId gid, uint seqno)
		{
			// ......................................................
		}
				
		public void cleanupGroup(GMS.GroupId gid)
		{
			// ......................................................
		}

		public void prepareSmoothCleanup(GMS.GroupId gid)
		{
			throw new Exception("Currently not supported.");
		}

		public void registerGMSCallbacks(
			GMS.ViewChangeGoAhead readyToInstallCallback, GroupReadyForCleanup readyToCleanupCallback)
		{
			this.readyToInstallCallback = readyToInstallCallback;
			this.readyToCleanupCallback = readyToCleanupCallback;
		}

		#endregion

		public void linkToGMS(GMS.IGMS theGMS)
		{
			registerGMSCallbacks(theGMS.linkCMSToGMS(new GMS.ViewChangeRequest(requestViewChange), 
				new GMS.ViewChangeAllDone(installmentComplete), new GMS.ViewChangeCleanup(cleanupGroup)), null);
		}

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.ViewController_MessageChannel;
			}
		}

		#endregion
	}
*/ 
}
