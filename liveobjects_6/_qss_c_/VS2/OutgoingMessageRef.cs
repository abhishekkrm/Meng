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
using System.Diagnostics;

namespace QS._qss_c_.VS_2_
{
	/// <summary>
	/// Summary description for OutgoingSlot.
	/// </summary>
/* 
	public abstract class OutgoingMessageRef
	{
		public OutgoingMessageRef(uint senderLOID, GMS.GroupId groupID, Base.IMessage message, Base.SendCallback sendCallback)
		{
			this.senderLOID = senderLOID;
			this.message = message;
			this.sendCallback = sendCallback;
			this.groupID = groupID;
		}

		// sanity checking for debugging purposes...
		protected bool registeredInGroup = false;

		protected uint senderLOID;
		protected GMS.GroupId groupID;
		protected Base.IMessage message;
		protected Base.SendCallback sendCallback;
		
		// these are assigned by the view controller
		protected Group.View view = null;
		protected uint inViewSeqNo;

		public override string ToString()
		{
			return "OutgoingMessageRef(LOID:" + senderLOID.ToString() + "->GID:" + groupID.ToString() + 
				";VSN:" + view.theView.SeqNo.ToString() + ";SEQ:" + inViewSeqNo.ToString() + " \"" + message.ToString() + "\")";
		}

		protected abstract void initializeAfterRegistering();

//		protected abstract bool stable();

		public void registerInGroup(Group.View view, uint inViewSeqNo)
		{
			Debug.Assert(!registeredInGroup);			
			registeredInGroup = true;

			this.view = view;
			this.inViewSeqNo = inViewSeqNo;

			initializeAfterRegistering();
		}

		public Base.IAddress Address
		{
			get
			{
				return new Base.ViewAddress(groupID, view.theView.SeqNo);
			}
		}

		public GMS.IView View
		{
			get
			{
				return view.theView;
			}
		}

		public uint InViewSeqNo
		{
			get
			{
				return inViewSeqNo;
			}
		}

		public uint SenderLOID
		{
			get
			{
				return senderLOID;
			}
		}

		public GMS.GroupId GroupID
		{
			get
			{
				return groupID;
			}
		}

		public Base.IMessage Message
		{
			get
			{
				return message;
			}
		}

		public Base.SendCallback SendCallback
		{
			get
			{
				return sendCallback;
			}
		}
	}
*/	
}
