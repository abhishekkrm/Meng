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

namespace QS._qss_c_.Base1_
{
/*
	/// <summary>
	/// Summary description for ViewAddress.
	/// </summary>
	[Serializable]
	public class ViewAddress : IAddress
	{
		public ViewAddress(GMS.GroupId groupID, uint viewSeqNo)
		{
			this.groupID = groupID;
			this.viewSeqNo = viewSeqNo;
		}

		private GMS.GroupId groupID;
		private uint viewSeqNo;

		public GMS.GroupId GroupID
		{
			get
			{
				return groupID;
			}
		}

		public GMS.IView View
		{
			get
			{
				throw new Exception("not supported");
//				return ViewAddress.viewController.resolve(this);
			}
		}

		public uint ViewSeqNo
		{
			get
			{
				return viewSeqNo;
			}
		}

		public override string ToString()
		{
			return "[GID:" + groupID.ToString() + ";VSN:" + viewSeqNo.ToString() + "]";
		}

		public override int GetHashCode()
		{			
			return groupID.GetHashCode() ^ viewSeqNo.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return ((obj is ViewAddress) && ((ViewAddress) obj).groupID == this.groupID && 
				((ViewAddress) obj).viewSeqNo == this.viewSeqNo);
		}

//		public static void linkToViewController(VS2.IViewController viewController)
//		{
//			ViewAddress.viewController = viewController;
//		}

//		private static VS2.IViewController viewController;

		#region IAddress Members

		public ObjectAddress[] Destinations
		{
			get
			{
				throw new Exception("not supported");
//				return ViewAddress.view2ObjectAddresses(ViewAddress.viewController.resolve(this));
			}
		}

		#endregion

		public static ObjectAddress[] view2ObjectAddresses(GMS.IView view)
		{
			ObjectAddress[] objectAddresses = new ObjectAddress[view.NumberOfMembers];
			
			uint member_index = 0;
			for (uint subview_index = 0; subview_index < view.NumberOfSubViews; subview_index++)
			{
				GMS.ISubView subView = view[subview_index];
			
				for (uint ind = 0; ind < subView.Count; ind++)
				{
					objectAddresses[member_index] = 
						new ObjectAddress(subView.IPAddress, subView.PortNumber, subView[ind]); 			
					member_index++;
				}
			}

			return objectAddresses;
		}
	}
*/
}
