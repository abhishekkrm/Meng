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

namespace QS._qss_e_.Tests_.Test007
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			uint routingBase = Convert.ToUInt32(args["base"]);
			uint memberCount = Convert.ToUInt32(args["size"]);

			QS._qss_c_.Routing_1_.IRoutingStructure routingStructure =
				(new QS._qss_c_.Routing_1_.PrefixRouting(routingBase)).instantiate(memberCount, 0);

			uint destination = Convert.ToUInt32(args["root"]);

			for (uint ind = 0; ind < memberCount; ind++)
			{
				string in_string = "";
				string out_string = "";
				uint[] incoming_fingers = routingStructure.incomingAt(ind, destination);
				for (int fno = 0; fno < incoming_fingers.Length; fno++)
					in_string += incoming_fingers[fno].ToString("00") + " ";
				uint[] outgoing_fingers = routingStructure.outgoingAt(ind, destination);
				for (int fno = 0; fno < outgoing_fingers.Length; fno++)
					out_string += outgoing_fingers[fno].ToString("00") + " ";

				platform.Logger.Log(ind.ToString("00") + 
					" : \tout \t" + out_string + " \tin \t" + in_string);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
