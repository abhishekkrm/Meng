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

namespace QS._qss_c_.Flushing_1_
{
/*
	/// <summary>
	/// Summary description for FlushingReports.
	/// </summary>
	// [Serializable]
	public class FlushingReport : Base.IMessage
	{
		public static Base.IBase1Serializable createSerializable()
		{
			return new FlushingReport();
		}

		private FlushingReport()
		{
		}

		// [Serializable]
		public class ReceiverReport : Base.IBase1Serializable
		{
//			public static Base.ISerializable createSerializable()
//			{
//				return new ReceiverReport();
//			}

			public ReceiverReport()
			{
			}

			public ReceiverReport(QS.Fx.Network.NetworkAddress sourceAddress, uint numberOfMessagesConsumed)
			{		
				this.sourceAddress = sourceAddress;
				this.numberOfMessagesConsumed = numberOfMessagesConsumed;
			}

			private QS.Fx.Network.NetworkAddress sourceAddress;
			private uint numberOfMessagesConsumed;

			public override string ToString()
			{
				return "RR(" + sourceAddress.ToString() + " consumed : " + numberOfMessagesConsumed.ToString() + ")";
			}

			#region Accessors

			public QS.Fx.Network.NetworkAddress SourceAddress
			{
				get
				{
					return sourceAddress;
				}
			}

			public uint NumberOfMessagesConsumed
			{
				get
				{
					return numberOfMessagesConsumed;
				}
			}

			#endregion

			#region ISerializable Members

			public QS.ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.ReceiverReport;
				}
			}

			public void save(System.IO.Stream stream)
			{
				sourceAddress.save(stream);
				byte[] buffer = System.BitConverter.GetBytes(numberOfMessagesConsumed);
				stream.Write(buffer, 0, buffer.Length);								
			}

			public void load(System.IO.Stream stream)
			{
				sourceAddress = new QS.Fx.Network.NetworkAddress();
				sourceAddress.load(stream);
				byte[] buffer = new byte[4];
				stream.Read(buffer, 0, 4);
				numberOfMessagesConsumed = System.BitConverter.ToUInt32(buffer, 0);
			}

			#endregion
		}

		public FlushingReport(GMS.GroupId groupID, uint viewSeqNo, bool isInitial, uint numberOfMessagesSent, ReceiverReport[] receiverReports)
		{
			this.groupID = groupID;
			this.viewSeqNo = viewSeqNo;
			this.isInitial = isInitial;
			this.numberOfMessagesSent = numberOfMessagesSent;
			this.receiverReports = receiverReports;
		}

		private GMS.GroupId groupID;
		private uint viewSeqNo, numberOfMessagesSent;
		private bool isInitial;
		private ReceiverReport[] receiverReports;

		public override string ToString()
		{
			string debug_string =  "FlushingReport(GID = " + groupID.ToString() + ", ViewSeqNo = " + viewSeqNo.ToString() + ", isInitial : " + isInitial.ToString() + ", numSent = " + 
				numberOfMessagesSent.ToString() + ", receiver reports : {";
			foreach (ReceiverReport receiverReport in receiverReports)
				debug_string = debug_string + " " + receiverReport.ToString();
			return debug_string + " })";
		}

		#region Accessors

		public GMS.GroupId GroupID
		{
			get
			{
				return groupID;
			}
		}

		public uint ViewSeqNo
		{
			get
			{
				return viewSeqNo;
			}
		}

		public bool IsInitial
		{
			get
			{
				return isInitial;
			}
		}

		public uint NumberOfMessagesSent
		{
			get
			{
				if (!isInitial)
					throw new Exception("incorrect usage");
				return numberOfMessagesSent;
			}
		}

		public ReceiverReport[] ReceiverReports
		{
			get
			{
				return receiverReports;
			}
		}

		#endregion

		#region ISerializable Members

		public QS.ClassID ClassIDAsSerializable
		{
			get
			{				
				return ClassID.FlushingReport;
			}
		}

		public void save(System.IO.Stream stream)
		{
			groupID.save(stream);
			byte[] buffer = System.BitConverter.GetBytes(viewSeqNo);
			stream.Write(buffer, 0, buffer.Length);		
			buffer = System.BitConverter.GetBytes(numberOfMessagesSent);
			stream.Write(buffer, 0, buffer.Length);		
			buffer = System.BitConverter.GetBytes(isInitial);
			stream.Write(buffer, 0, buffer.Length);		
			buffer = System.BitConverter.GetBytes((uint) receiverReports.Length);
			stream.Write(buffer, 0, buffer.Length);
			for (uint ind = 0; ind < (uint) receiverReports.Length; ind++)
				receiverReports[ind].save(stream);
		}

		public void load(System.IO.Stream stream)
		{
			groupID = new GMS.GroupId();
			groupID.load(stream);
			byte[] buffer = new byte[13];
			stream.Read(buffer, 0, 13);
			viewSeqNo = System.BitConverter.ToUInt32(buffer, 0);
			numberOfMessagesSent = System.BitConverter.ToUInt32(buffer, 4);
			isInitial = System.BitConverter.ToBoolean(buffer, 8);
			receiverReports = new ReceiverReport[System.BitConverter.ToUInt32(buffer, 9)];
			for (uint ind = 0; ind < (uint) receiverReports.Length; ind++)
			{
				receiverReports[ind] = new ReceiverReport();
				receiverReports[ind].load(stream);
			}
		}

		#endregion
	}
*/
}
