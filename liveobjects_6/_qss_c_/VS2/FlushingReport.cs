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
	/// Summary description for FlushingReport.
	/// </summary>
/*
	[Serializable]
	public class FlushingReport
	{
		public FlushingReport(Group group, Group.View view) // , Collections.ISet liveAddresses)
		{
			viewAddress = new Base.ViewAddress(group.gid, view.theView.SeqNo);
			
//			firstMessageSentSeqNo = view.firstMessageSentInGroupSeqNo;
			numberOfMessagesSent = view.numberOfMessagesSent;
			
			receivedMessages = new ReceivedMessages[view.theView.NumberOfSubViews];
			uint index = 0;			
			for (uint ind = 0; ind < view.theView.NumberOfSubViews; ind++) 				
			{
				QS.Fx.Network.NetworkAddress networkAddress = new QS.Fx.Network.NetworkAddress(view.theView[ind].IPAddress, view.theView[ind].PortNumber);
				Collections.IDictionaryEntry dic_en = view.incomingWindows.lookup(networkAddress);
				System.Collections.ICollection unstableSeqNoList = null;
				if (dic_en != null)
				{			
					FlowControl.IIncomingWindow incomingWindow = (FlowControl.IIncomingWindow) dic_en.Value;
					Debug.Assert(incomingWindow != null);
					unstableSeqNoList = incomingWindow.selectObjects(new FlowControl.AcceptObjectCallback(this.CheckUnstable));
				}
				else
				{
					// just create something empty
					unstableSeqNoList = new System.Collections.ArrayList();
				}

				receivedMessages[index++] = new ReceivedMessages(networkAddress, unstableSeqNoList);
			}
		}

		public Base.ViewAddress viewAddress;
		public uint numberOfMessagesSent; // firstMessageSentSeqNo, 
		public ReceivedMessages[] receivedMessages;

		public override string ToString()
		{
			string result = "{ " + viewAddress.ToString() + ", " + "+" + numberOfMessagesSent.ToString(); // firstMessageSentSeqNo.ToString() + 
			if (receivedMessages != null)
			{
				result = result + "; " + receivedMessages.Length.ToString() + ":[";
				foreach (ReceivedMessages recvmsg in receivedMessages)
					result = result + " " + recvmsg.ToString();
				result = result + " ]";
			}
			else
				result = result + ", null";
			return result + " }";
		}
	
		[Serializable]
		public class ReceivedMessages
		{
			public ReceivedMessages(QS.Fx.Network.NetworkAddress networkAddress, System.Collections.ICollection theList)
			{
				this.networkAddress = networkAddress;

				unstableMessageSeqNoList = new uint[theList.Count];
				uint index = 0;
				foreach (uint seqno in theList)
					unstableMessageSeqNoList[index++] = seqno;
			}

			public override int GetHashCode()
			{
				return networkAddress.GetHashCode();
			}

			public override bool Equals(object obj)
			{
				if (obj is QS.Fx.Network.NetworkAddress)
					return networkAddress.Equals(obj);
				else if (obj is ReceivedMessages)
					return networkAddress.Equals(((ReceivedMessages) obj).networkAddress);
				else
					return false;
			}

			public override string ToString()
			{
				string result = "( " + networkAddress.ToString();
				foreach (uint seqno in unstableMessageSeqNoList)
					result = result + seqno.ToString() + " ";
				return result + ")";
			}

			public QS.Fx.Network.NetworkAddress networkAddress;
			public uint[] unstableMessageSeqNoList;
		}			

		private bool CheckUnstable(object obj)
		{
			IncomingMessageRef messageRef = (IncomingMessageRef) obj;
			return (messageRef.ViewOf.theView.SeqNo == viewAddress.ViewSeqNo) && messageRef.stable();
		}

		// returns a collection of "ReceivedMessages" structures
		public ReceivedMessages[] diff(FlushingReport anotherReport)
		{
			System.Collections.ArrayList theDiff = new System.Collections.ArrayList();

			Collections.ISet workSet = new Collections.HashSet((uint) receivedMessages.Length);
			foreach (ReceivedMessages rmsg in receivedMessages)
				workSet.insert(rmsg);

			foreach (ReceivedMessages rmsg in anotherReport.receivedMessages)
			{
				ReceivedMessages irmsg = (ReceivedMessages) workSet.lookup(rmsg.networkAddress);
				if (irmsg != null)
				{
					Collections.ISet iworkSet = new Collections.HashSet((uint) irmsg.unstableMessageSeqNoList.Length);
					foreach (uint seqno in irmsg.unstableMessageSeqNoList)
						iworkSet.insert(seqno);

					foreach (uint seqno in rmsg.unstableMessageSeqNoList)
						iworkSet.remove(seqno);

					// for now, some stupid hacking

					System.Collections.ArrayList seqno_diff = new System.Collections.ArrayList();
					foreach (uint seqno in iworkSet.Elements)
						seqno_diff.Add(seqno);

					theDiff.Add(new ReceivedMessages(rmsg.networkAddress, seqno_diff));
				}
				else
				{
					theDiff.Add(rmsg);
				}
			}

			ReceivedMessages[] result = new ReceivedMessages[theDiff.Count];
			uint index = 0;
			foreach (ReceivedMessages rmsg in theDiff)
				result[index++] = rmsg;
			
			return result;
		}
	}
*/	
}
