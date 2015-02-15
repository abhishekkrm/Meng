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
using System.Threading;

namespace QS._qss_c_.RPC_1_
{
/*
	/// <summary>
	/// Aaa...
	/// </summary>
	public class Client : RPC.IClient, Base.IClient
	{
		private static bool dummy = Server.registerSerializableClasses();

		public Client(uint clientObjectID, uint serverObjectID, Base.ISender sender, 
			Base.IDemultiplexer demultiplexer, QS.Fx.Logging.ILogger logger,
			uint anticipatedNumberOfConcurrentCalls)
		{
			this.clientObjectID = clientObjectID;
			this.serverObjectID = serverObjectID;
			this.sender = sender;		
			this.pendingCalls = new Collections.Hashtable(anticipatedNumberOfConcurrentCalls);
			this.logger = logger;

			demultiplexer.register(this, new CMS.Dispatchers.DirectDispatcher(
				new CMS.Base.OnReceive(this.receiveCallback)));
		}

		private Base.ISender sender;
		private Collections.Hashtable pendingCalls;
		private QS.Fx.Logging.ILogger logger;
		private uint clientObjectID, serverObjectID;

		private uint lastSeqNo = 0;

		private class Collector
		{
			public Collector(uint senderLOID)
			{
				this.senderLOID = senderLOID;

				collected = new AutoResetEvent(false);
				resultQueue = new Collections.RawQueue();
			}

			public IResult[] results()
			{
				IResult[] result = new IResult[resultQueue.size()];
				for (uint ind = 0; ind < result.Length; ind++)
					result[ind] = (IResult) resultQueue.dequeue();
				return result;
			}

			public void add(Base.ObjectAddress source, Base.IBase1Serializable result)
			{
				resultQueue.enqueue(new CallResult(
					new Base.ObjectAddress(source.HostIPAddress, source.PortNumber, senderLOID), result));
			}

			public void wait(System.TimeSpan timeout)
			{
				collected.WaitOne(timeout, false);
			}

			private AutoResetEvent collected;
			private Collections.IRawQueue resultQueue;
			private uint senderLOID;
		}

		private void receiveCallback(CMS.Base.IAddress source, CMS.Base.IMessage message)
		{
			try
			{
				if (!(message is Server.Response))
					throw new Exception("Unknown source type");

				if (!(source is Base.ObjectAddress))
					throw new Exception("Unknown message type!");

				Server.Response response = (Server.Response) message;

/-*
				logger.Log(this, "received response from the RPC server : " + response.ToString());
*-/

				lock (pendingCalls)
				{
					Collections.IDictionaryEntry dic_en = pendingCalls.lookup(response.seqno);
					if (dic_en != null)
					{
						Collector collector = (Collector) dic_en.Value;
						collector.add((Base.ObjectAddress) source, response.result);
					}
				}
			}
			catch (Exception exc)
			{
				logger.Log(this, "ReceiveCallback : " + exc.ToString());
			}
		}

		#region RPC.IClient Members

		public IResult[] synchronousCollect(Base.IClient theCaller, Base.IAddress calleeAddress, 
			Base.IBase1Serializable argument, System.TimeSpan timeout)
		{
			uint seqno;
			Collector collector = new Collector(theCaller.LocalObjectID);

			lock (pendingCalls)
			{
				seqno = ++lastSeqNo;
				pendingCalls[seqno] = collector;
			}

			foreach (Base.ObjectAddress address in calleeAddress.Destinations)
			{				
				Server.Request request = new Server.Request(
					theCaller.LocalObjectID, address.LocalObjectID, lastSeqNo, argument);

				// logger.Log(this, "...........in collect -> " + address.ToString());

				sender.send(this, 
					new Base.ObjectAddress(address.HostIPAddress, (int) address.PortNumber, 
					(uint) serverObjectID), request, null);
			}

			collector.wait(timeout);

			lock (pendingCalls)
			{
				pendingCalls.remove(seqno);
			}

			return collector.results();
		}

		#endregion

		#region Base.IClient Members

		public uint LocalObjectID
		{
			get
			{
				return clientObjectID;
			}
		}

		#endregion

		private class CallResult : Collections.GenericLinkable, IResult
		{
			public CallResult(Base.ObjectAddress source, Base.IBase1Serializable result)
			{
				this.source = source;
				this.result = result;
			}

			private Base.ObjectAddress source;
			private Base.IBase1Serializable result;

			public Base.ObjectAddress Source
			{
				get
				{
					return source;
				}
			}

			public Base.IBase1Serializable Result
			{
				get
				{
					return result;
				}
			}
		}
	}
*/
}
