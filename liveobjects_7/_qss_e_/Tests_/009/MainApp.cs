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
using System.IO;

namespace QS._qss_e_.Tests_.Test009
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		public MainApp(QS.Fx.Platform.IPlatform platform, QS._core_c_.Components.AttributeSet args)
		{
			QS._core_c_.Components.AttributeSet att = new QS._core_c_.Components.AttributeSet(3);
			att["foo"] = "kazik";
			att["babuna"] = "rumuna babuna rumuna";
			att["3"] = "--28 alamak ota";

			QS._qss_c_.Base2_.XmlWrapper wrappedGuy = new QS._qss_c_.Base2_.XmlWrapper(att);
			QS._qss_c_.Base2_.IIdentifiableObject isobjGuy = QS._qss_c_.Components_1_.Sequencer.wrap(wrappedGuy);

			platform.Logger.Log("Object: " + isobjGuy.ToString());
		}

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
