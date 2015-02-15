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

namespace QS._qss_c_.Gossiping_1_
{
/*
	public abstract class GossipingSender<C> : Base3.SenderClass<IGossipingSender, C>, 
		Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>, Base3.ISenderClass<Base3.IReliableSerializableSender>
		where C : IGossipingSender
	{
		public GossipingSender() : base()
		{
		}

		public abstract class Sender : TMS.Inspection.GenericInspectable, IGossipingSender
		{
			public Sender(QS.Fx.Network.NetworkAddress destinationAddress)
			{
				this.destinationAddress = destinationAddress;
			}

			protected QS.Fx.Network.NetworkAddress destinationAddress;

			#region ISerializableSender Members

			[TMS.Inspection.Inspectable]
			QS.Fx.Network.NetworkAddress QS.CMS.Base3.ISerializableSender.Address
			{
				get { return destinationAddress; }
			}

			public abstract void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data);

			[TMS.Inspection.Inspectable]
			public abstract int MTU
			{
				get ;
			}

			#endregion

			#region IComparable Members

			int IComparable.CompareTo(object obj)
			{
				QS.CMS.Base3.ISerializableSender anotherSender = obj as QS.CMS.Base3.ISerializableSender;
				return (anotherSender != null) ? destinationAddress.CompareTo(anotherSender.Address) 
					: destinationAddress.CompareTo(obj); 
			}

			#endregion

			#region IReliableSerializableSender Members

			public abstract Base3.IAsynchronousOperation BeginSend(uint destinationLOID, QS.Fx.Serialization.ISerializable data, 
				Base3.AsynchronousOperationCallback completionCallback, object asynchronousState);

			public abstract void EndSend(Base3.IAsynchronousOperation asynchronousOperation);

			#endregion
		}

		#region ISenderClass<ISerializableSender> Members

		QS.CMS.Base3.ISenderCollection<QS.CMS.Base3.ISerializableSender> QS.CMS.Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>.SenderCollection
		{
			get 
			{ 
				return new Base3.SenderCollectionCast<IGossipingSender, QS.CMS.Base3.ISerializableSender>(
					((Base3.ISenderClass<IGossipingSender>)this).SenderCollection);
			}
		}

		QS.CMS.Base3.ISerializableSender QS.CMS.Base3.ISenderClass<QS.CMS.Base3.ISerializableSender>.CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return this.createSender(destinationAddress);
		}

		#endregion

		#region ISenderClass<IReliableSerializableSender> Members

		QS.CMS.Base3.ISenderCollection<QS.CMS.Base3.IReliableSerializableSender> QS.CMS.Base3.ISenderClass<QS.CMS.Base3.IReliableSerializableSender>.SenderCollection
		{
			get
			{
				return new Base3.SenderCollectionCast<IGossipingSender, Base3.IReliableSerializableSender>(
					((Base3.ISenderClass<IGossipingSender>)this).SenderCollection);
			}
		}

		QS.CMS.Base3.IReliableSerializableSender QS.CMS.Base3.ISenderClass<QS.CMS.Base3.IReliableSerializableSender>.CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return this.createSender(destinationAddress);
		}

		#endregion
	}
*/
}
