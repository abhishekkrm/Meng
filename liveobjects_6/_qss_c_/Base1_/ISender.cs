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
	/// <summary>
	/// This class represents a sender. Senders of various types are 
	/// available, providing a variety of performance or reliability 
	/// guarantees, for unicast, multicast and anycast communication 
	/// modes and ranging from plain broadcast through IP multicast, 
	/// application-level multicast with no guarantees, then FBCAST, 
	/// CBCAST and ABCAST.
	/// </summary>
	public interface ISender
	{
		/// <summary>
		/// Send a message.
		/// </summary>
		/// <param name="destinationAddress">Address of the intended recipient.
		/// </param>
		/// <param name="sourceModuleID">Sender module identifier.</param>
		/// <param name="message">Message body.</param>
		/// <param name="sendCallback">A function to be called to notify 
		/// about successful delivery or to report an error.</param>
		/// <returns>Reference that can be used to cancel a message,
		/// ignore confirmation, or that could be used to match this
		/// message with confirmation or error notification.</returns>
		IMessageReference send(
			IClient theSender, IAddress destinationAddress, 
			QS._core_c_.Base.IMessage message, SendCallback sendCallback);
	}

	/// <summary>
	/// These functions are called upon message delivery, or when an
	/// error of some sort occurred. In the latter case the specific
	/// problem can be communicated via passing an exception object.
	/// </summary>
	public delegate void SendCallback(IMessageReference messageRef, 
		bool success, System.Exception exception);

	/// <summary>
	/// This interface is implemented by data structures internal to 
	/// the senders, used for the purpose of identifying 
	/// </summary>
	public interface IMessageReference
	{
		IClient Sender
		{
			get;
		}

		IAddress Address
		{
			get;
		}

		QS._core_c_.Base.IMessage Message
		{
			get;
		}

		/// <summary>
		/// Call to cancel the sending of a message. The message may
		/// have been sent already, but no more explicit attempts to 
		/// send will be taken.
		/// </summary>
		void cancel();

		/// <summary>
		/// Call it to ignore any confirmations or errors related to 
		/// the sending of this message.
		/// </summary>
		void ignore();
	}
}
