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

namespace QS._qss_c_.Senders_1_
{
	public class CompressingSender : Base1_.ISender, Base1_.IClient
		// , Base3.SenderClass<QS.CMS.Base3.ISerializableSender, CompressingSender.Sender>
	{
		public CompressingSender(Base1_.ISender underlyingSender, Base1_.IDemultiplexer demultiplexer)
		{
			this.underlyingSender = underlyingSender;
			this.demultiplexer = demultiplexer;

			demultiplexer.register(this, new Dispatchers_.DirectDispatcher(new QS._qss_c_.Base1_.OnReceive(receiveCallback)));
		}

		private Base1_.ISender underlyingSender;
		private Base1_.IDemultiplexer demultiplexer;

		#region Receive Callback

		private void receiveCallback(Base1_.IAddress sourceAddress, QS._core_c_.Base.IMessage message)
		{
			Base1_.Message wrapped_message = message as Base1_.Message;
			if (wrapped_message != null && wrapped_message.TheMessage is Base1_.CompressedObject)
			{
				QS._core_c_.Base.IBase1Serializable theObject = ((Base1_.CompressedObject)wrapped_message.TheMessage).Object;
				demultiplexer.demultiplex(wrapped_message.DestinationLOID, sourceAddress, (QS._core_c_.Base.IMessage)theObject);
			}
			else
				throw new Exception("Received wrong message type.");
		}	

		#endregion

		#region ISender Members

		QS._qss_c_.Base1_.IMessageReference QS._qss_c_.Base1_.ISender.send(
			QS._qss_c_.Base1_.IClient theSender, QS._qss_c_.Base1_.IAddress destinationAddress, QS._core_c_.Base.IMessage message, 
			QS._qss_c_.Base1_.SendCallback sendCallback)
		{
			Base1_.ObjectAddress address = destinationAddress as Base1_.ObjectAddress;
			if (address == null)
				throw new ArgumentException("Unsupported address type: " + QS._core_c_.Helpers.ToString.ObjectRef(destinationAddress));

			underlyingSender.send(theSender, new Base1_.ObjectAddress(address.NetworkAddress, ((Base1_.IClient)this).LocalObjectID),
				new Base1_.Message(address.LocalObjectID, new Base1_.CompressedObject(message)), sendCallback);

			return null;
		}

		#endregion

		#region Class Sender


		#endregion

		#region IClient Members

		uint QS._qss_c_.Base1_.IClient.LocalObjectID
		{
			get { return (uint) ReservedObjectID.CompressingSender; }
		}

		#endregion
	}
}
