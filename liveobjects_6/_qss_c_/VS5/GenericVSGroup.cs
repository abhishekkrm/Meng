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

// #define DEBUG_GenericVSGroup

using System;
using System.Threading;

namespace QS._qss_c_.VS5
{
/*
	/// <summary>
	/// Summary description for VSGroup.
	/// </summary>
	public abstract class GenericVSGroup : Collections.GenericBinaryTreeNode, IVSGroup
	{
		public static void register_serializable()
		{
			GenericVSMessage.register_serializable();
			GenericVSMessageID.register_serializable();
		}

		public GenericVSGroup(GMS.GroupId groupID, GMS.IView membershipView, QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;

			this.groupID = groupID;
			views = new Collections2.SynchronizedRBT(new Collections.RawSplayTree(), logger);
			views.insert(currentView = createView(membershipView, false, logger));
			oldestView = currentView;
		}

		protected QS.Fx.Logging.ILogger logger;

		private Collections2.ISynchronizedRBT views;	
		
		protected GMS.GroupId groupID;
		protected IVSView oldestView, currentView;

		#region Managing the collection of Views

		protected IVSView lookupView(uint viewSeqNo)
		{
			return (IVSView) views.lookup(viewSeqNo);
		}

		protected abstract IVSView createView(GMS.IView membershipView, bool initiallySuspended, QS.Fx.Logging.ILogger logger);

		#endregion

		protected abstract void multicastWrappedObject(GMS.GroupId groupID, uint viewSeqNo, 
			uint withinViewSeqNo, Base2.IBase2Serializable serializableObject, Scattering.IScatterSet scatterSet,
			Scattering.CompletionCallback completionCallback);

		#region IVSGroup Members

		public GMS.GroupId GroupID
		{
			get
			{
				return this.groupID;
			}
		}

		public void multicast(Base2.IBase2Serializable serializableObject, 
			Scattering.CompletionCallback completionCallback)
		{
			Monitor.Enter(currentView);
			GenericVSScatterSet scatterSet = new GenericVSScatterSet(currentView);

			this.multicastWrappedObject(groupID, currentView.MembershipView.SeqNo, 
				currentView.NextMessageSeqNo, serializableObject, scatterSet, completionCallback);
		}

		public void requestViewChange(QS.GMS.IView membershipView)
		{
#if DEBUG_GenericVSGroup
			logger.Log(this, "_requestViewChange: " + this.groupID.ToString() + ":" + 
				membershipView.SeqNo.ToString());
#endif

			IVSView previousView = currentView;
			lock (previousView)
			{
				views.insert(currentView = this.createView(membershipView, true, logger));

				IVSView updatingView = currentView;
				lock (updatingView)			
				{
					Monitor.Exit(this);

					updatingView.PrevVSView = previousView;
					previousView.NextVSView = updatingView;

					Collections.ISet liveAddresses, deadAddresses;
					GenericVSView.IdentifyAliveAndCrashedNodes(previousView.MembershipView, 
						updatingView.MembershipView, out liveAddresses, out deadAddresses);

					previousView.processCrashes(deadAddresses);


					// ...............................


				}

				

			}
		}

		public virtual Base2.IIdentifiableObject lookupOutgoing(IVSMessageID messageID)
		{
			Monitor.Exit(this);
			throw new Exception("not implemented");
		}

		public virtual Base2.IIdentifiableObject lookupIncoming(IVSMessageID messageID)
		{
			Monitor.Exit(this);
			throw new Exception("not implemented");
		}

		public virtual void receive(QS.Fx.Network.NetworkAddress sourceAddress, IVSMessage message)
		{
			IVSView view = this.lookupView(message.ID.ViewSeqNo);
			Monitor.Exit(this);

			if (view != null)
				view.receive(sourceAddress, message);
			else
				throw new Exception("View does not exist");
		}

		#endregion

		#region Collections.GenericBinaryTreeNode Overrides

		public override System.IComparable Contents
		{
			get
			{
				return groupID;
			}
		}

		#endregion
	}
*/ 
}
