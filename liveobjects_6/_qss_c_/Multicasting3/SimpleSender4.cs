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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Multicasting3
{
	public class SimpleSender4 : GroupSenderClass<IGroupSender, SimpleSender4.Sender>, ISimpleSender
	{
		public SimpleSender4()
		{
		}

		#region ISimpleSender Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get { return new List<QS._core_c_.Components.Attribute>(); }
		}

		#endregion

		#region Class Sender

		public class Sender : GroupSender
		{
			public Sender(SimpleSender4 owner, Base3_.GroupID groupID) : base(groupID)
			{
				this.owner = owner;
			}

			private SimpleSender4 owner;

			#region Class Request

			public class Request : Base3_.AsynchronousOperation
			{
				public Request(SimpleSender4.Sender owner, Multicasting3.MulticastMessage message,
					Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
					: base(completionCallback, asynchronousState)
				{
					this.owner = owner;
					this.message = message;
				}

				private SimpleSender4.Sender owner;
				private Multicasting3.MulticastMessage message;

				#region Accessors

				#endregion

				#region Callbacks

				public override void Unregister()
				{
					// .......................................................
				}

				#endregion
			}

			#endregion

			#region Callbacks

			#endregion

			#region Overrides from GroupSender

			public override QS._qss_c_.Base3_.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, QS._qss_c_.Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
			{
				// .....................................

				return null;
			}

			public override void EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
			{
			}

			public override int MTU
			{
				get { throw new System.NotImplementedException(); }
			}

			#endregion
		}

		#endregion

		protected override SimpleSender4.Sender createSender(QS._qss_c_.Base3_.GroupID groupID)
		{
			return new Sender(this, groupID);
		}
	}
}
