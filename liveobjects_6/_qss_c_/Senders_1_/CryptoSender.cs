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
using System.Security.Cryptography;

namespace QS._qss_c_.Senders_1_
{
	/// <summary>
	/// This sender provides security via encryption/decryptions of messages sent through it.
	/// A single sender instance defines a single encrypted channel.
	/// </summary>
	public class CryptoSender : Base1_.ISender, Base1_.IClient
	{
        public CryptoSender(Base1_.ISender underlyingSender, Base1_.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger,
			SymmetricAlgorithm symmetricAlgorithm, byte[] key)
		{
			Base1_.Serializer.Get.register(ClassID.CryptographicSender_OurMessage, 
				new QS._core_c_.Base.CreateSerializable(OurMessage.createSerializable));

			this.underlyingSender = underlyingSender;
			this.demultiplexer = demultiplexer;
			this.logger = logger;

			this.symmetricAlgorithm = symmetricAlgorithm;
			this.key = key;

			demultiplexer.register(this, new QS._qss_c_.Dispatchers_.DirectDispatcher(
				new QS._qss_c_.Base1_.OnReceive(this.receiveCallback)));
		}

		private const uint localObjectID = (uint) ReservedObjectID.CryptographicSender;
		private const uint EncryptionBufferSize = 100;

		private Base1_.ISender underlyingSender;
		private Base1_.IDemultiplexer demultiplexer;
		private QS.Fx.Logging.ILogger logger;
		private SymmetricAlgorithm symmetricAlgorithm;
		private byte[] key;

		private void receiveCallback(QS._qss_c_.Base1_.IAddress source, QS._core_c_.Base.IMessage message)
		{
			if (!(message is OurMessage))
				throw new Exception("unknown message type");

			if (!(source is Base1_.ObjectAddress))
				throw new Exception("wrong source address");
			Base1_.ObjectAddress sourceObjectAddress = (Base1_.ObjectAddress) source;

			OurMessage ourmsg = (OurMessage) message;

			ICryptoTransform cryptoTransform;
			lock (symmetricAlgorithm)
			{
				cryptoTransform = symmetricAlgorithm.CreateDecryptor(key, ourmsg.initVector);
			}

			MemoryStream memoryStream = new MemoryStream(ourmsg.binaryData, 0, (int) ourmsg.sizeOfData);			
			CryptoStream encryptedStream = new CryptoStream(
				memoryStream, cryptoTransform, CryptoStreamMode.Read);

			QS._core_c_.Base.IMessage actualMessage = 
				(QS._core_c_.Base.IMessage) Base1_.Serializer.Get.createObject((ClassID) ourmsg.msgClassID);
			actualMessage.load(encryptedStream); 

			Base1_.ObjectAddress actualSourceAddress = new Base1_.ObjectAddress(sourceObjectAddress.NetworkAddress, ourmsg.senderLOID);

			demultiplexer.demultiplex(
				ourmsg.targetLOID, actualSourceAddress, actualMessage);
		}

		#region ISender Members

		public Base1_.IMessageReference send(Base1_.IClient theSender, 
			Base1_.IAddress destinationAddress, QS._core_c_.Base.IMessage message, Base1_.SendCallback sendCallback)
		{
			byte[] initializationVector;

			ICryptoTransform cryptoTransform;
			lock (symmetricAlgorithm)
			{
				symmetricAlgorithm.GenerateIV();
				initializationVector = symmetricAlgorithm.IV;
				cryptoTransform = symmetricAlgorithm.CreateEncryptor(key, initializationVector);
			}

			MemoryStream memoryStream = new MemoryStream();			
			CryptoStream encryptedStream = new CryptoStream(
				memoryStream, cryptoTransform, CryptoStreamMode.Write);
						
			message.save(encryptedStream);

			encryptedStream.Close();
			memoryStream.Close();

			byte[] encryptedArray = memoryStream.ToArray();

			foreach (Base1_.ObjectAddress address in destinationAddress.Destinations)
			{
				OurMessage ourmsg = new OurMessage(theSender.LocalObjectID, address.LocalObjectID, 
					(uint) message.ClassIDAsSerializable, encryptedArray, (uint) encryptedArray.Length, 
					initializationVector);

				underlyingSender.send(this, new Base1_.ObjectAddress(address.NetworkAddress, this.LocalObjectID), ourmsg, null);
			}

			return null;
		}

		#endregion

		private class OurMessage : QS._core_c_.Base.IMessage
		{
			public static QS._core_c_.Base.IBase1Serializable createSerializable()
			{
				return new OurMessage();
			}

			private OurMessage()
			{
			}

			public OurMessage(uint senderLOID, uint targetLOID, 
				uint msgClassID, byte[] binaryData, uint sizeOfData, byte[] initVector)
			{
				this.senderLOID = senderLOID;
				this.targetLOID = targetLOID;
				this.msgClassID = msgClassID;
				this.binaryData = binaryData;
				this.sizeOfData = sizeOfData;
				this.initVector = initVector;
			}

			public uint senderLOID, targetLOID, msgClassID, sizeOfData;
			public byte[] binaryData;
			public byte[] initVector;

			public static string byteArray2String(byte[] bytes, long count)
			{
				string str = count.ToString() + ":\"";
				for (int ind = 0; ind < count; ind++)
					str = str + ((bytes[ind] >=16) ? ((char) bytes[ind]).ToString() : ".");
				return str + "\"";
			}

			public override string ToString()
			{
				return "(" + senderLOID + ":" + targetLOID + " " + ((ClassID) msgClassID) + " " + 
					OurMessage.byteArray2String(binaryData, sizeOfData) + ")";
			}

			#region ISerializable Members

			public ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.CryptographicSender_OurMessage;
				}
			}

			public void save(System.IO.Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(senderLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(targetLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(msgClassID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(sizeOfData);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes((uint) initVector.Length);
				memoryStream.Write(buffer, 0, buffer.Length);
				memoryStream.Write(binaryData, 0, (int) sizeOfData);
				memoryStream.Write(initVector, 0, initVector.Length);
			}

			public void load(System.IO.Stream memoryStream)
			{
				byte[] buffer = new byte[20];
				memoryStream.Read(buffer, 0, 20);
				senderLOID = System.BitConverter.ToUInt32(buffer,  0);
				targetLOID = System.BitConverter.ToUInt32(buffer,  4);				
				msgClassID = System.BitConverter.ToUInt32(buffer,  8);				
				sizeOfData = System.BitConverter.ToUInt32(buffer, 12);
				uint vsize = System.BitConverter.ToUInt32(buffer, 16);
				binaryData = new byte[sizeOfData];
				initVector = new byte[vsize];
				memoryStream.Read(binaryData, 0, (int) sizeOfData); 
				memoryStream.Read(initVector, 0, (int) vsize); 				
			}

			#endregion
		}	
	
		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.CryptographicSender;
			}
		}

		#endregion
	}
}
