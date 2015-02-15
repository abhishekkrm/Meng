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

namespace QS._qss_c_.VS_2_
{
	/// <summary>
	/// Summary description for IncomingSlot.
	/// </summary>
/*
	public abstract class IncomingMessageRef
	{
		public IncomingMessageRef(Base.ObjectAddress senderAddress, Group group, Group.View view, uint inViewSeqNo, 
			Base.IMessage message, VS2.IVSSender vsSender)
		{			
			this.group = group;
			this.view = view;
			// this.groupID = groupID;
			// this.viewSeqNo = viewSeqNo;
			this.senderAddress = senderAddress;			
			this.inViewSeqNo = inViewSeqNo;
			this.message = message;	
			this.deliveryAllowed = true;
			this.removingAllowed = false;
			this.vsSender = vsSender;
		}

		public void deliver()
		{
			vsSender.readyToDeliver(this);
			this.removingAllowed = true;

		}

		public abstract bool stable();

		protected Group group;
		protected Group.View view;

		protected Base.IMessage message;
		protected Base.ObjectAddress senderAddress;
		// protected GMS.GroupId groupID;
		// protected uint viewSeqNo;			
		protected uint inViewSeqNo;
		protected bool deliveryAllowed, removingAllowed;
		protected VS2.IVSSender vsSender;

		public IVSSender VSSender
		{
			get
			{
				return vsSender;
			}
		}

		public Group GroupOf
		{
			get
			{
				return group;
			}
		}

		public Group.View ViewOf
		{
			get
			{
				return view;
			}
		}

		public virtual bool DeliveryAllowed
		{
			get
			{
				return deliveryAllowed;
			}

			set
			{
				deliveryAllowed = value;
			}
		}

		public virtual bool RemovingAllowed
		{
			get
			{
				return removingAllowed;
			}
		}

		public Base.IAddress SenderAddress
		{
			get
			{
				return senderAddress;
			}
		}

//		public GMS.GroupId GroupID
//		{
//			get
//			{
//				return groupID;
//			}
//		}
//
//		public uint ViewSeqNo
//		{
//			get
//			{
//				return viewSeqNo;
//			}
//		}

		public uint InViewSeqNo
		{
			get
			{
				return inViewSeqNo;
			}
		}

		public Base.IMessage Message
		{
			get
			{
				return message;
			}
		}
	}
*/	
}
