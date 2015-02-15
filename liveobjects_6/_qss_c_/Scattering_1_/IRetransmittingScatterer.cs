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

namespace QS._qss_c_.Scattering_1_
{
	/// <summary>
	/// Summary description for IMulticastingDevice.
	/// </summary>
	public interface IRetransmittingScatterer
	{
		void multicast(uint destinationLOID, Scattering_1_.IScatterSet destinationAddressSet, 
			Base2_.IIdentifiableObject message, CompletionCallback completionCallback);

		void multicast(IRetransmittingScattererRequest request);

		uint MTU
		{
			get;
		}

		double RetransmissionTimeout
		{
			get;
			set;
		}

		Base2_.IIdentifiableObjectContainer IOContainer
		{
			set;
		}
	}

	public interface IRetransmittingScattererRequest : Base2_.IIdentifiableObject
	{
		uint DestinationLOID
		{
			get;
		}

		Scattering_1_.IScatterSet AddressSet
		{
			get;
		}

		Base2_.IIdentifiableObject WrappedIdentifiableObject
		{
			get;
		}
		
		CompletionCallback Callback
		{
			get;
		}

		QS.Fx.Clock.IAlarm AlarmRef
		{
			get;
			set;
		}
	}
}
