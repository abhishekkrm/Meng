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

namespace QS._qss_c_.Membership_1_
{
/*
	/// <summary>
	/// Summary description for FakeGMS.
	/// </summary>
	public class Client : QS.GMS.IGMS, Base.IClient
	{
		public Client(QS.Fx.Logging.ILogger logger, Base2.IDemultiplexer demultiplexer)
		{
			this.logger = logger;
			demultiplexer.register(this.LocalObjectID, new Base2.ReceiveCallback(this.receiveCallback));
		}

		private QS.Fx.Logging.ILogger logger;

		private QS.GMS.ViewChangeRequest vcr;
		private QS.GMS.ViewChangeAllDone vcad;
		private QS.GMS.ViewChangeCleanup vcc;

		private uint internalSeqNo = 0;

		private QS.CMS.Base2.IBase2Serializable receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
			Base2.IBase2Serializable wrappedObject)
		{
			Base2.BlockOfData compressedObject = 
				(Base2.BlockOfData) ((CMS.Components.Sequencer.IWrappedObject) wrappedObject).SerializableObject;
			GMS.ClientServer.WholeViewMessage wholeViewMessage = new QS.GMS.ClientServer.WholeViewMessage();
			wholeViewMessage.load(compressedObject.AsStream);

			lock (this)
			{
				if (wholeViewMessage.SeqNo > internalSeqNo)
				{
					if (wholeViewMessage.SeqNo > internalSeqNo + 1)
						throw new Exception("View changes arrived in the wrong sequence, bad, bad, bad!!!!!!");

					internalSeqNo = wholeViewMessage.SeqNo;

//					logger.Log(this, "_ReceiveCallback: " + wholeViewMessage.GroupID.ToString() + ":" + 
//						wholeViewMessage.View.SeqNo.ToString());

					if (vcr != null)
						vcr(wholeViewMessage.GroupID, wholeViewMessage.View);
				}
//				else
//					logger.Log(this, "ignoring duplicate notification");
			}

			return null;
		}

		private void viewChangeGoAhead(GMS.GroupId gid, uint seqno)
		{
			logger.Log(this, "_ViewChangeGoAhead " + gid.ToString() + ":" + seqno.ToString());

			if (vcad != null)
				vcad(gid, seqno);
		}

		#region IGMS Members

		public QS.GMS.ViewChangeGoAhead linkCMSToGMS(QS.GMS.ViewChangeRequest vcr, QS.GMS.ViewChangeAllDone vcad, 
			QS.GMS.ViewChangeCleanup vcc)
		{
			this.vcr = vcr;
			this.vcad = vcad;
			this.vcc = vcc;

			return new GMS.ViewChangeGoAhead(this.viewChangeGoAhead);
		}

		public void joinGroup(QS.GMS.GroupId gid, uint nid, QS.GMS.ViewChangeUpcall vcu)
		{
			throw new Exception("not supported");
		}

		public void leaveGroup(QS.GMS.GroupId gid, uint nid)
		{
			throw new Exception("not supported");
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.Membership_Client;
			}
		}

		#endregion
	}
*/ 
}
