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

#define DEBUG_Scatterer

using System;
using System.Net;

namespace QS._qss_c_.Scattering_1_
{	
/*
	public class Scatterer : IScatterer
	{
		public Scatterer(Base2.IBlockSender underlyingSender, QS.Fx.Logging.ILogger logger)
		{
            this.logger = logger;
            this.underlyingSender = underlyingSender;
		}

		private Base2.IBlockSender underlyingSender;
        private QS.Fx.Logging.ILogger logger;

		#region IScatterer Members

		public void scatter(IScatterAddress scatterAddress, uint destinationLOID, QS.CMS.Base2.IBase2Serializable data)
		{
#if DEBUG_Scatterer
//          logger.Log(this, "Scattering " + data.Size.ToString() + " bytes (" + data.ToString() + ")to " + 
//              destinationLOID.ToString() + " at " + scatterAddress.ToString());
#endif

			switch (scatterAddress.AddressClass)
			{
				case AddressClass.ADDRESS_COLLECTION:
				{
                    underlyingSender.send(destinationLOID, (System.Collections.ICollection) scatterAddress, data);
				}
				break;

                case AddressClass.INDIVIDUAL_ADDRESS:
                {
                    underlyingSender.send(destinationLOID, scatterAddress as QS.Fx.Network.NetworkAddress, data);
                }
                break;

                default:
				{
					throw new Exception("Address " + scatterAddress.ToString() + " of class " + 
						scatterAddress.AddressClass.ToString() + " is not supported by the scatterer.");
				}
				// break;
			}
		}

		public uint MTU
		{
			get
			{
				return underlyingSender.MTU;
			}
		}

		#endregion
	}
*/ 
}
