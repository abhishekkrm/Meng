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
using System.Net;
using System.Threading;
using System.Text;
using System.Diagnostics;

namespace QS._qss_e_.Tests_.Test003
{
	/// <summary>
	/// Summary description for MainApp.
	/// </summary>
	public class MainApp : System.IDisposable
	{
		private QS.Fx.Network.NetworkAddress sourceAddress;
		public MainApp(QS._core_c_.Components.AttributeSet args, QS.Fx.Logging.ILogger logger)
		{
			this.random = new Random();
			this.logger = logger;

			sourceAddress = new QS.Fx.Network.NetworkAddress(QS._qss_c_.Devices_2_.Network.LocalAddresses[0], 0);

			udpDevice = new QS._qss_c_.Devices_2_.UDPCommunicationsDevice(logger);

			if (args.contains("sender"))
			{
				using (QS._qss_c_.Components_1_.Controller controller = new QS._qss_c_.Components_1_.Controller(
						   logger, IPAddress.Parse((string) args["base"]), Convert.ToUInt32((string) args["portno"])))
				{
					uint numberOfGroups = Convert.ToUInt32((string) args["groups"]);
					uint numberOfMessages = Convert.ToUInt32((string) args["count"]);
					uint numberOfSamples = Convert.ToUInt32((string) args["samples"]);
					
					double cpu_utilization = measureCPUUtilization(controller, numberOfGroups, numberOfMessages, numberOfSamples);
					logger.Log(null, numberOfGroups.ToString() + "\t" + cpu_utilization);
				}
			}
			else
			{
				measuringThread = new Thread(new ThreadStart(measuringThreadMainLoop));
				measuringThread.Start();
			}
		}

		double measureCPUUtilization(QS._qss_c_.Components_1_.Controller controller, uint numberOfGroups, uint numberOfMessages, uint numberOfSamples)
		{
			double accumulatedValue = 0;
			for (uint ind = 0; ind < numberOfSamples; ind++)
				accumulatedValue += takeOneSample(controller, numberOfGroups, numberOfMessages);
			return accumulatedValue / numberOfSamples;
		}

		double takeOneSample(QS._qss_c_.Components_1_.Controller controller, uint numberOfGroups, uint numberOfMessages)
		{
			object response = controller.invoke(new QS._qss_c_.Components_1_.CallRequest(typeof(QS._qss_e_.Tests_.Test003.MainApp).GetMethod("signup", new System.Type[] { 
				typeof(uint) }), new object[] { numberOfGroups }));
			
			QS._core_c_.Base2.IBlockOfData blockOfData = new QS._core_c_.Base2.BlockOfData(Encoding.ASCII.GetBytes("ala ma kota"));

			for (uint ind = 0; ind < numberOfMessages; ind++)
			{
				udpDevice.sendto(this.sourceAddress, new QS.Fx.Network.NetworkAddress(MainApp.shiftedAddress(
					originOfGroupAddressSpace, (uint) (13 * random.Next(256 * 256 * 256) + 2)), (int) (60500 + random.Next(5000))), blockOfData);
			}
							
			double cpu_utilization = (double) controller.invoke(
				new QS._qss_c_.Components_1_.CallRequest(typeof(QS._qss_e_.Tests_.Test003.MainApp).GetMethod("signoff", System.Type.EmptyTypes), new object[] {}));

			// logger.Log(null, "CPU Utilization : " + cpu_utilization.ToString());

			return cpu_utilization;
		}

		private Random random;
		private QS._qss_c_.Devices_2_.ICommunicationsDevice udpDevice;
		private QS.Fx.Logging.ILogger logger;
		private Thread measuringThread;
		private AutoResetEvent measuringThreadGo = new AutoResetEvent(false);
		private AutoResetEvent measurementReady = new AutoResetEvent(false);
		private AutoResetEvent finishMeasurements = new AutoResetEvent(false);
		private bool finishingNow, terminatingNow = false;

		private double averageCPUUtilization;

		private void measuringThreadMainLoop()
		{
			using (PerformanceCounter performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true))
			{
				for (measuringThreadGo.WaitOne(); !terminatingNow; measuringThreadGo.WaitOne())
				{
					double cpu_accumulated = 0;
					uint num_samples = 0;

					while (!finishingNow)
					{
						float cpu_utilization = performanceCounter.NextValue();

						cpu_accumulated += cpu_utilization;
						num_samples++;

						// logger.Log(null, " .................... one measurement : CPU utilization : " + cpu_utilization.ToString());
						finishMeasurements.WaitOne(100, false);
					}

					averageCPUUtilization = (num_samples > 0) ? (cpu_accumulated / num_samples) : 0;

					measurementReady.Set();
				}
			}
		}
		
		private IPAddress originOfGroupAddressSpace = IPAddress.Parse("224.0.0.1");
		public bool signup(uint numberOfGroups)
		{
			logger.Log(null, "signup : " + numberOfGroups.ToString());

			try
			{
				logger.Log(null, "signup_enter : " + DateTime.Now.ToLongTimeString());
				for (uint ind = 0; ind < numberOfGroups; ind++)
				{
					udpDevice.listenAt(sourceAddress.HostIPAddress, new QS.Fx.Network.NetworkAddress(MainApp.shiftedAddress(
						originOfGroupAddressSpace, (uint) (13 * random.Next(256 * 256 * 256) + 1)), (int) (10500 + ind) ),
						new QS._qss_c_.Devices_2_.OnReceiveCallback(this.receiveCallback));
				}
				logger.Log(null, "signup_leave : " + DateTime.Now.ToLongTimeString());
			}
			catch (Exception exc)
			{
				logger.Log(null, exc.ToString());
			}

			logger.Log(null, "test_enter");

			finishingNow = false;
			measuringThreadGo.Set();

			return true;
		}

		public double signoff()
		{
			finishingNow = true;
			finishMeasurements.Set();
			measurementReady.WaitOne();

			logger.Log(null, "test_leave : " + averageCPUUtilization.ToString());

			shutdown();

			return averageCPUUtilization;
		}

		private void shutdown()
		{
			try
			{
				udpDevice.shutdown();
			}
			catch (Exception exc)
			{
				logger.Log(this, "Dispose : " + exc.ToString());
			}
		}

		private void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress, 
			QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			logger.Log(null, Encoding.ASCII.GetString(blockOfData.Buffer, (int) blockOfData.OffsetWithinBuffer, (int) blockOfData.SizeOfData));
		}

		#region Address Conversions

		private static uint addressToNumber(IPAddress address)
		{
			return int_bytesToNumberAndSwap(address.GetAddressBytes());
		}

		private static IPAddress numberToAddress(uint number)
		{
			return new IPAddress(int_bytesToNumberAndSwap(System.BitConverter.GetBytes(number)));
		}

		private static uint int_bytesToNumberAndSwap(byte[] bytes)
		{
			return ((((uint) bytes[0]) * 256 + ((uint) bytes[1])) * 256 + ((uint) bytes[2])) * 256 + ((uint) bytes[3]);
		}

		private static IPAddress shiftedAddress(IPAddress baseAddress, uint offset)
		{
			return numberToAddress(addressToNumber(baseAddress) + offset);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			terminatingNow = true;
			measuringThreadGo.Set();

			// logger.Log(this, "Dispose");
			shutdown();			
		}

		#endregion
	}
}
