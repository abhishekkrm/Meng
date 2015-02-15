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

namespace QS._qss_e_.Experiments_
{
	/// <summary>
	/// Summary description for Experiment_000.
	/// </summary>
	public class Experiment_000 : IExperiment
	{
		public Experiment_000()
		{
		}

		private void moo(QS.Fx.Network.NetworkAddress addr)
		{
			addr.PortNumber = addr.PortNumber ^ 66;
		}

		private delegate void MooCallback(QS.Fx.Network.NetworkAddress addr);

/*
		static unsafe void Copy1(uint num, byte[] buffer, ref uint buffer_offset, ref uint buffer_size)
		{
			fixed (byte* pbuffer = buffer)
			{
				*((uint *) (pbuffer + buffer_offset)) = num;
			}

			buffer_offset += 4;
			buffer_size -= 4;
		}

		static unsafe void Copy2(uint num, byte* pbuffer, ref uint buffer_offset, ref uint buffer_size)
		{
			*((uint *) (pbuffer + buffer_offset)) = num;

			buffer_offset	+= sizeof(uint);
			buffer_size		-= sizeof(uint);
		}

		static string Array2String(System.Array array)
		{
			System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
			for (int ind = 0; ind < array.Length; ind++)
			{
				if (ind > 0)
					stringBuilder.Append(",");
				stringBuilder.Append(array.GetValue(ind).ToString());
			}

			return stringBuilder.ToString();
		}
*/
		private const uint ntimes = 10000000;

		public void run(Runtime_.IEnvironment environment, QS.Fx.Logging.ILogger logger,
            QS._core_c_.Components.IAttributeSet args, QS._core_c_.Components.IAttributeSet results)
        {
            QS._core_e_.Data.DataSeries dataSeries = new QS._core_e_.Data.DataSeries(100);
            for (int ind = 0; ind < 100; ind++)
                dataSeries[ind] = Math.Pow(((double)ind) / 100.0, 2.0);
            results["boo"] = dataSeries;

/*
            System.Web.Mail.MailMessage message = new System.Web.Mail.MailMessage();
            message.From = "QuickSilver Scheduler Service";
            message.To = "krzys@cs.cornell.edu";
            message.Subject = "Experiment Complete";
            message.Body = "Aaa...";
            message.BodyFormat = System.Web.Mail.MailFormat.Text;
            System.Web.Mail.SmtpMail.SmtpServer = "mail.cs.cornell.edu";
            System.Web.Mail.SmtpMail.Send(message);
*/


//			QS.CMS.Devices3.ScatterSocket scatterSocket = new QS.CMS.Devices3.ScatterSocket();
//			scatterSocket.send_one();



/*

//			int shit = 3;
			double t1, t2;

			QS.Fx.Network.NetworkAddress addr = new QS.Fx.Network.NetworkAddress(System.Net.IPAddress.Loopback, 3445);

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				addr.PortNumber = addr.PortNumber ^ 66;
			}
			
			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;
			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				moo(addr);
			}
			
			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;
			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));

			MooCallback mooCallback = new MooCallback(moo);
			
			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				mooCallback(addr);
			}
			
			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;
			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				lock (addr)
				{
					moo(addr);
				}
			}
			
			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;
			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				addr = new QS.Fx.Network.NetworkAddress(System.Net.IPAddress.Loopback, 3445);
				lock (addr)
				{
					moo(addr);
				}
			}
			
			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;
			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));
*/

/*
			byte[] bb = new byte[20];				
			uint offs, size;		

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
				Buffer.BlockCopy(BitConverter.GetBytes(ind), 0, bb, 0, 4);

			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;

			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));

			t1 = QS.CMS.Base2.PreciseClock.Clock.Time;

			for (uint ind = 0; ind < ntimes; ind++)
			{
				unsafe
				{
					fixed (byte* pbuffer = bb)
					{
						*((uint *) pbuffer) = ind;
					}
				}
			}

			t2 = QS.CMS.Base2.PreciseClock.Clock.Time;

			logger.Log(((t2 - t1) / ntimes).ToString(".000000000000"));
*/

//			logger.Log(Array2String(bb));
//			logger.Log(offs.ToString() + "," + size.ToString());		

//			Buffer.BlockCopy(BitConverter.GetBytes(shit), 0, bb, 0, 4);

//			uint buffer_size = 10;
//			byte[] buffer = new byte[buffer_size];
//			uint buffer_offset = 0;

//			QS.CMS.Components.SeqNo seqno = new QS.CMS.Components.SeqNo(12);
			
//			QS.CMS.Base2.BlockOfData block = new QS.CMS.Base2.BlockOfData(buffer, buffer_offset, buffer_size);
//			seqno.save(block);

//			uint offs = 0;
//			uint size = 4;
//			QS.CMS.Base3.Serializer.SaveUInt32(3, bb, ref offs, ref size);
		}

        #region IDisposable Members

        public void Dispose()
        {
		}

		#endregion	
	}
}
