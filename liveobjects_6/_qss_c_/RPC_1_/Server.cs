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

namespace QS._qss_c_.RPC_1_
{
/*
	/// <summary>
	/// Aaa...
	/// </summary>
	public class Server : RPC.IServer, Base.IClient
	{
		private static bool dummy = Server.registerSerializableClasses();

		public Server(uint localObjectID, Base.ISender sender, Base.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger,
			uint anticipatedNumberOfCallees)
		{
			this.logger = logger;

			this.sender = sender;	
			this.registeredCallees = new Collections.Hashtable(anticipatedNumberOfCallees);
			this.localObjectID = localObjectID;

			demultiplexer.register(this, new CMS.Dispatchers.MultithreadedDispatcher(
				new CMS.Base.OnReceive(this.receiveCallback)));
		}

		private Base.ISender sender;
		private Collections.IDictionary registeredCallees;
		private QS.Fx.Logging.ILogger logger;
		private uint localObjectID;

		private void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message)
		{
			try
			{
				if (!(message is Server.Request))
					throw new Exception("Unknown message type");

				if (!(source is Base.ObjectAddress))
					throw new Exception("Wrong sender address");

				Server.Request request = (Server.Request) message;

				SynchronousCallback callback;
				lock (registeredCallees)
				{
					callback = (SynchronousCallback) registeredCallees[request.targetLOID];
				}
				
				Base.ObjectAddress sourceAddress = (Base.ObjectAddress) source;
				Base.ObjectAddress callerAddress = new Base.ObjectAddress(
					sourceAddress.HostIPAddress, sourceAddress.PortNumber, request.senderLOID);

				Base.IBase1Serializable result = callback(callerAddress, request.argument);
				if (result == null)
					result = CMS.Base.EmptyObject.Object;

				Server.Response response = new Response(request.seqno, result);

/-*
				logger.Log(this, 
					"i will now reply from RPC server with " + response.ToString() + " to " + source.ToString());
*-/

				sender.send(this, source, response, null);
			}
			catch (Exception exc)
			{
				logger.Log(this, "ReceiveCallback : " + exc.ToString());
			}
		}

		#region IServer Members

		public void register(Base.IClient theClient, SynchronousCallback synchronousCallback)
		{
			lock (registeredCallees)
			{
				Collections.IDictionaryEntry dic_en = registeredCallees.lookupOrCreate(theClient.LocalObjectID);
				if (dic_en.Value != null)
					throw new Exception("caller already registered");

				dic_en.Value = synchronousCallback;
			}
		}
		
		#endregion 

		private static bool myclasses_registered = false;
		public static bool registerSerializableClasses()
		{
			if (!myclasses_registered)
			{
				CMS.Base.Serializer.Get.register(ClassID.RPCServerRequest, new CMS.Base.CreateSerializable(Request.createSerializable));
				CMS.Base.Serializer.Get.register(ClassID.RPCServerResponse, new CMS.Base.CreateSerializable(Response.createSerializable));

				myclasses_registered = true;
			}

			return true;
		}

		public class Request : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new Request();
			}

			private Request()
			{
			}

			public Request(uint senderLOID, uint targetLOID, uint seqno, Base.IBase1Serializable argument)
			{
				this.senderLOID = senderLOID;
				this.targetLOID = targetLOID;
				this.seqno = seqno;
				this.argument = argument;
			}

			public uint senderLOID, targetLOID, seqno;
			public Base.IBase1Serializable argument;

			#region ISerializable Members

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.RPCServerRequest;
				}
			}

			public void save(System.IO.Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(senderLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(targetLOID);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes(seqno);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes((ushort) argument.ClassIDAsSerializable);
				memoryStream.Write(buffer, 0, buffer.Length);
				argument.save(memoryStream);
			}

			public void load(System.IO.Stream memoryStream)
			{
				byte[] buffer = new byte[14];
				memoryStream.Read(buffer, 0, 14);
				senderLOID = System.BitConverter.ToUInt32(buffer, 0);
				targetLOID = System.BitConverter.ToUInt32(buffer, 4);
				seqno = System.BitConverter.ToUInt32(buffer, 8);				
				ushort argumentClassID = System.BitConverter.ToUInt16(buffer, 12);				
				argument = (Base.IMessage) Base.Serializer.Get.createObject((ClassID) argumentClassID);
				argument.load(memoryStream); 			
			}

			#endregion
		}

		public class Response : Base.IMessage
		{
			public static Base.IBase1Serializable createSerializable()
			{
				return new Response();
			}

			private Response()
			{
			}

			public Response(uint seqno, Base.IBase1Serializable result)
			{
				this.seqno = seqno;
				this.result = result;
			}

			public uint seqno;
			public Base.IBase1Serializable result;

			#region ISerializable Members

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.RPCServerResponse;
				}
			}

			public void save(System.IO.Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(seqno);
				memoryStream.Write(buffer, 0, buffer.Length);
				buffer = System.BitConverter.GetBytes((ushort) result.ClassIDAsSerializable);
				memoryStream.Write(buffer, 0, buffer.Length);
				result.save(memoryStream);
			}

			public void load(System.IO.Stream memoryStream)
			{
				byte[] buffer = new byte[6];
				memoryStream.Read(buffer, 0, 6);
				seqno = System.BitConverter.ToUInt32(buffer, 0);				
				ushort resultClassID = System.BitConverter.ToUInt16(buffer, 4);				
				result = (Base.IMessage) Base.Serializer.Get.createObject((ClassID) resultClassID);
				result.load(memoryStream); 			
			}

			#endregion

			public override string ToString()
			{
				return "[" + seqno + ", " + result.ToString() + "]";
			}

		}

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return localObjectID;
			}
		}

		#endregion
	}
*/
}
