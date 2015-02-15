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

#define DEBUG_N2FlushingDevice

using System;

namespace QS._qss_c_.Flushing_1_
{
/*
	/// <summary>
	/// Summary description for SimpleFlushingDevice.
	/// </summary>
	public class N2FlushingDevice : IFlushingDevice, Base.IClient
	{
		public N2FlushingDevice(QS.Fx.Logging.ILogger logger, Base.ISender underlyingSender, Base.IDemultiplexer demultiplexer)
		{
			this.logger = logger;
			this.underlyingSender = underlyingSender;

			demultiplexer.register(this, new Dispatchers.DirectDispatcher(new Base.OnReceive(this.receiveCallback)));
		}

		private QS.Fx.Logging.ILogger logger;
		private IFlushingReportConsumer reportConsumer = null;
		private Base.ISender underlyingSender;

		private void receiveCallback(Base.IAddress source, Base.IMessage message)
		{
			if (!(source is Base.ObjectAddress))
				throw new Exception("unknown source address type");			
			QS.Fx.Network.NetworkAddress sourceAddress = (QS.Fx.Network.NetworkAddress) source;

			if (!(message is FlushingReport))
				throw new Exception("message is not a flushing report");
			FlushingReport flushingReport = (FlushingReport) message;

			reportConsumer.incorporateFlushingReport(sourceAddress, flushingReport); 
		}

		#region IFlushingDevice Members

		public void linkToReportConsumer(IFlushingReportConsumer reportConsumer)
		{
			this.reportConsumer = reportConsumer;
		}

		public void distributeFlushingReports(QS.Fx.Network.NetworkAddress[] destinations, FlushingReport[] flushingReports)
		{
#if DEBUG_N2FlushingDevice
			string debug_message = "Asked to distribute flushing report.\nDestinations :";
			foreach (QS.Fx.Network.NetworkAddress networkAddress in destinations)
				debug_message = debug_message + " " + networkAddress.ToString();
			debug_message = debug_message + "\nReports :\n";
			foreach (FlushingReport flushingReport in flushingReports)
				debug_message = debug_message + " - " + flushingReport.ToString() + "\n";
			logger.Log(this, debug_message + "\n");
#endif

			foreach (FlushingReport flushingReport in flushingReports)
			{
				foreach (QS.Fx.Network.NetworkAddress networkAddress in destinations)
				{
					underlyingSender.send(this, new Base.ObjectAddress(networkAddress, this.LocalObjectID), flushingReport, null);
				}
			}
		}

		#endregion

		#region IClient Members

		public uint LocalObjectID
		{
			get
			{
				return (uint) ReservedObjectID.N2FlushingDevice;
			}
		}

		#endregion
	}
*/
}
