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

namespace QS._qss_c_.IPMulticast_
{
	/// <summary>
	/// Summary description for SignupClient.
	/// </summary>
/* 
	public class SignupClient : IClientAgent
	{
		public SignupClient()
		{
			// .....................
		}

		private class GroupSubscriptionRef : Base2.GenericSharedObject, IGroupSubscriptionRef
		{
			public GroupSubscriptionRef(Base2.IMembershipViewRef membershipViewRef, 
				SignupClient associatedSignupClient, QS.CMS.IPMulticast.GroupSubscriptionCallback subscriptionCallback)
			{
				this.membershipViewRef = membershipViewRef;
				this.associatedSignupClient = associatedSignupClient;
				this.subscriptionCallback = subscriptionCallback;
			}

			private Base2.IMembershipViewRef membershipViewRef;
			private SignupClient associatedSignupClient;
			private QS.CMS.IPMulticast.GroupSubscriptionCallback subscriptionCallback;

			private QS.Fx.Network.NetworkAddress networkAddress; // ........................................

			#region IGroupSubscriptionRef Members

			public QS.Fx.Network.NetworkAddress Address
			{
				get
				{
					return networkAddress;
				}
			}

			#endregion

			#region GenericSharedObject Overrides

			protected override void ObjectReleased()
			{
				// ............................................
			}

			#endregion
		}

		#region IClientAgent Members

		public void subscribe(Base2.IMembershipViewRef membershipViewRef, 
			QS.CMS.IPMulticast.GroupSubscriptionCallback subscriptionCallback)
		{
			GroupSubscriptionRef subscriptionRef = 
				new GroupSubscriptionRef(membershipViewRef, this, subscriptionCallback);

			// ................
		}

		#endregion
	}
*/	
}
