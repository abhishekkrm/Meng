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
	/// Summary description for IDemultiplexer.
	/// </summary>
	public interface IDemultiplexer
	{
		void register(uint localObjectID, ReceiveCallback receiveCallback);
		QS._core_c_.Base2.IBase2Serializable demultiplex(uint localObjectID, QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable serializableObject);
	}

	public delegate QS._core_c_.Base2.IBase2Serializable ReceiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, 
		QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable serializableObject);

	public class Demultiplexer : IDemultiplexer
	{
		public Demultiplexer(QS.Fx.Logging.ILogger logger)
		{
			this.logger = logger;
			callbacks = new QS._core_c_.Collections.Hashtable(100);
		}

		private QS._core_c_.Collections.IDictionary callbacks;
		private QS.Fx.Logging.ILogger logger;

		#region IDemultiplexer Members

		public void register(uint localObjectID, QS._qss_c_.Base2_.ReceiveCallback receiveCallback)
		{
			lock (callbacks)
			{
				if (callbacks.lookup(localObjectID) != null)
					throw new Exception("Cannot register : object with ID = " + localObjectID.ToString() + " has already been registered.");
				callbacks[localObjectID] = receiveCallback;
			}
		}

		public QS._core_c_.Base2.IBase2Serializable demultiplex(uint localObjectID, QS.Fx.Network.NetworkAddress sourceAddress, 
			QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			QS._core_c_.Collections.IDictionaryEntry dic_en = callbacks.lookup(localObjectID);
			if (dic_en == null)
				throw new Exception("Cannot demultiplex : object with ID = " + localObjectID.ToString() + 
					(":" + ((ReservedObjectID) localObjectID).ToString()) + " has not been registered.");
			QS._qss_c_.Base2_.ReceiveCallback receiveCallback = (QS._qss_c_.Base2_.ReceiveCallback) dic_en.Value;
			
			return receiveCallback(sourceAddress, destinationAddress, serializableObject);
		}

		#endregion
	}
}
