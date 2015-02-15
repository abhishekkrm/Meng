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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for MembershipViewAddress.
	/// </summary>
	/// 
	public interface IMembershipViewRef : System.Collections.ICollection
	{
		GMS.GroupId GroupID
		{
			get;
		}

		uint ViewSeqNo
		{
			get;
		}

		GMS.IView MembershipView
		{
			get;
		}
	}

	public class MembershipViewRef : IMembershipViewRef
	{
		public MembershipViewRef(GMS.GroupId groupID, GMS.IView membershipView)
		{
			this.groupID = groupID;
			this.membershipView = membershipView;
		}

		private GMS.GroupId groupID;
		private GMS.IView membershipView;

		#region IMembershipViewRef Members

		public QS.GMS.GroupId GroupID
		{
			get
			{
				return groupID;
			}
		}

		public uint ViewSeqNo
		{
			get
			{
				return membershipView.SeqNo;
			}
		}

		public QS.GMS.IView MembershipView
		{
			get
			{
				return membershipView;
			}
		}

		#endregion

		#region ICollection Members

		public int Count
		{
			get
			{
				return membershipView.NumberOfSubViews;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public void CopyTo(System.Array destinationArray, int offset)
		{
			for (uint ind = 0; ind < membershipView.NumberOfSubViews; ind++)
			{
				destinationArray.SetValue(new QS.Fx.Network.NetworkAddress(membershipView[ind].IPAddress, 
					membershipView[ind].PortNumber), offset + (int) ind);
			}
		}

		#endregion

		#region IEnumerable Members

		public System.Collections.IEnumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		#endregion

		#region Class Enumerator

		private class Enumerator : System.Collections.IEnumerator
		{
			public Enumerator(MembershipViewRef membershipViewRef)
			{
				this.membershipViewRef = membershipViewRef;
				this.currentNode = -1;
			}

			private MembershipViewRef membershipViewRef;
			private int currentNode;

			#region IEnumerator Members

			public void Reset()
			{
				this.currentNode = -1;
			}

			public object Current
			{
				get
				{
					if (currentNode < 0 || currentNode >= membershipViewRef.membershipView.NumberOfSubViews)
						throw new Exception("Enumerator is outside the range of values of the collection.");
					GMS.ISubView subView = membershipViewRef.membershipView[(uint) currentNode];
					return new QS.Fx.Network.NetworkAddress(subView.IPAddress, subView.PortNumber);
				}
			}

			public bool MoveNext()
			{
				if (currentNode++ < membershipViewRef.membershipView.NumberOfSubViews)
					return true;
				else
				{
					currentNode = membershipViewRef.membershipView.NumberOfSubViews;
					return false;
				}
			}

			#endregion
		}

		#endregion
	}
}
