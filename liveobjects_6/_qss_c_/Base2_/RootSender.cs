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
using System.Diagnostics;

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for ...
	/// </summary>
	/// 
	public class RootSender : IBlockSender, IBufferingSender, ISender, System.IDisposable
	{
		public RootSender(QS.Fx.Network.NetworkAddress localAddress, Devices_2_.ICommunicationsDevice communicationsDevice, 
			Base2_.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger) 
		{
			this.localAddress = localAddress;
			this.sendingSourceAddress = new QS.Fx.Network.NetworkAddress(localAddress.HostIPAddress, 0);
			this.logger = logger;
			this.communicationsDevice = communicationsDevice;
			this.demultiplexer = demultiplexer;

			listener = communicationsDevice.listenAt(localAddress.HostIPAddress, localAddress, this.ReceiveCallback);
		}

		private QS.Fx.Network.NetworkAddress localAddress, sendingSourceAddress;
		private Base2_.IDemultiplexer demultiplexer;
		private Devices_2_.ICommunicationsDevice communicationsDevice;
		private QS.Fx.Logging.ILogger logger;
        private Devices_2_.IListener listener;

		public QS.Fx.Network.NetworkAddress RootAddress
		{
			get
			{
				return this.localAddress;
			}
		}

		public Devices_2_.OnReceiveCallback ReceiveCallback
		{
			get
			{
				return new Devices_2_.OnReceiveCallback(this.receiveCallback);
			}
		}

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			try
			{
				Debug.Assert(blockOfData.SizeOfData >= protocolOverhead);
				uint replyPortNumber = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
				blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
				uint destinationLOID = BitConverter.ToUInt32(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer);
                blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
				QS.ClassID messageClassID = (QS.ClassID) BitConverter.ToUInt16(blockOfData.Buffer, 
					(int) blockOfData.OffsetWithinBuffer);
                blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
                QS._core_c_.Base2.IBase2Serializable serializableObject = QS._core_c_.Base2.Serializer.CommonSerializer.CreateObject(messageClassID);
				serializableObject.load(blockOfData);
				
				QS.Fx.Network.NetworkAddress responseAddress = 
					new QS.Fx.Network.NetworkAddress(sourceAddress.HostIPAddress, (int) replyPortNumber);
				demultiplexer.demultiplex(destinationLOID, responseAddress, destinationAddress, serializableObject);
			}
			catch (Exception exc)
			{
				logger.Log(this, "Receive Callback : " + exc.ToString());
			}
		}

//		private static uint sizeOfUInt32 = (uint) System.Runtime.InteropServices.Marshal.SizeOf(typeof(uint));

		#region IBlockSender Members

		public uint MTU
		{
			get
			{				
				return communicationsDevice.MTU - protocolOverhead;
			}
		}

        private static uint protocolOverhead = QS._core_c_.Base2.SizeOf.UInt16 + 2 * QS._core_c_.Base2.SizeOf.UInt32;

		public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IBase2Serializable serializableObject)
		{
			communicationsDevice.sendto(sendingSourceAddress, destinationAddress, serialize_outgoing(destinationLOID, serializableObject));						
		}

        public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress[] destinationAddresses, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            QS._core_c_.Base2.BlockOfData serialized_block = serialize_outgoing(destinationLOID, serializableObject);
            for (uint ind = 0; ind < destinationAddresses.Length; ind++)
                communicationsDevice.sendto(sendingSourceAddress, destinationAddresses[ind], serialized_block);
        }

        public void send(uint destinationLOID, System.Collections.ICollection destinationAddresses, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            QS._core_c_.Base2.BlockOfData serialized_block = serialize_outgoing(destinationLOID, serializableObject);
            lock (destinationAddresses.SyncRoot)
            {
                foreach (QS.Fx.Network.NetworkAddress destinationAddress in destinationAddresses)
                    communicationsDevice.sendto(sendingSourceAddress, destinationAddress, serialized_block);
            }
        }

        public void send(uint destinationLOID, System.Collections.Generic.ICollection<QS.Fx.Network.NetworkAddress> destinationAddresses, 
            QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            QS._core_c_.Base2.BlockOfData serialized_block = serialize_outgoing(destinationLOID, serializableObject);
            foreach (QS.Fx.Network.NetworkAddress destinationAddress in destinationAddresses)
                communicationsDevice.sendto(sendingSourceAddress, destinationAddress, serialized_block);
        }

        private QS._core_c_.Base2.BlockOfData serialize_outgoing(uint destinationLOID, QS._core_c_.Base2.IBase2Serializable serializableObject)
        {
            uint totalSize = serializableObject.Size + protocolOverhead;
            QS._core_c_.Base2.BlockOfData blockOfData = new QS._core_c_.Base2.BlockOfData(new byte[totalSize]);
            Buffer.BlockCopy(BitConverter.GetBytes(localAddress.PortNumber), 0,
                blockOfData.Buffer, (int)blockOfData.OffsetWithinBuffer, (int)QS._core_c_.Base2.SizeOf.UInt32);
            blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
            Buffer.BlockCopy(BitConverter.GetBytes(destinationLOID), 0,
                blockOfData.Buffer, (int)blockOfData.OffsetWithinBuffer, (int)QS._core_c_.Base2.SizeOf.UInt32);
            blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt32);
            Buffer.BlockCopy(BitConverter.GetBytes((ushort)serializableObject.ClassID), 0,
                blockOfData.Buffer, (int)blockOfData.OffsetWithinBuffer, (int)QS._core_c_.Base2.SizeOf.UInt16);
            blockOfData.consume(QS._core_c_.Base2.SizeOf.UInt16);
            serializableObject.save(blockOfData);
            blockOfData.resetCursor();

            return blockOfData;
        }

		#endregion

		#region IBufferingSender Members

		public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, QS._core_c_.Base2.IOutgoingData outgoingData)
		{
			this.send(destinationLOID, destinationAddress, (QS._core_c_.Base2.IBase2Serializable) outgoingData);
		}

		#endregion

		#region ISender Members

		public void send(uint destinationLOID, QS.Fx.Network.NetworkAddress destinationAddress, IMessage message)
		{
			this.send(destinationLOID, destinationAddress, message.AsOutgoingData);
		}

		#endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            // listener.Dispose();
        }

        #endregion
}
}
