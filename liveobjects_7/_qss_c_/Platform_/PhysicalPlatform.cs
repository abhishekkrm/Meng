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

// #define DEBUG_EnableMonitoringAgent

using System;
using System.Net;

namespace QS._qss_c_.Platform_
{
	/// <summary>
	/// Summary description for PhysicalPlatform.
	/// </summary>
    [QS.Fx.Base.Inspectable]
	public class PhysicalPlatform : QS.Fx.Inspection.Inspectable, IPlatform, QS._qss_e_.Base_1_.IStatisticsCollector
	{
        public enum Mode
        {
            Normal, SingleThreaded
        }

        private Mode operatingMode = Mode.Normal;

        public Mode OperatingMode
        {
            get { return operatingMode; }
            set { operatingMode = value; }
        }

        public PhysicalPlatform() 
            : this(new QS._core_c_.Base.ConsoleWrapper(new QS._core_c_.Base.WriteLineCallback(Console.WriteLine)))
        {
        }

        public PhysicalPlatform(QS.Fx.Logging.IConsole console)
            : this(new QS._core_c_.Base.Logger(QS._core_c_.Base2.PreciseClock.Clock, false, console))
		{
		}

		public PhysicalPlatform(QS._core_c_.Base.IReadableLogger logger)
		{
			this.logger = logger;
            eventLogger = new Logging_1_.EventLogger(QS._core_c_.Base2.PreciseClock.Clock, true);

			this.alarmClock = new Base1_.PQAlarmClock(new Collections_1_.BHeap(100, 2), logger);
			this.udpDevice = new QS._qss_c_.Devices_2_.UDPCommunicationsDevice(logger);
            this.clock = QS._core_c_.Base2.PreciseClock.Clock; 
            network = new Devices_3_.Network(logger);
            newNetwork = new Devices_4_.MyNetwork(logger);
            connections6 = new QS._qss_c_.Devices_6_.Connections(eventLogger);
            connections7 = new QS._qss_c_.Devices_7_.Connections(eventLogger);

            core = new QS._core_c_.Core.Core("C:\\.QuickSilver\\.Experiment_Results");
        }

#if DEBUG_EnableMonitoringAgent
		[TMS.Inspection.Inspectable("Monitoring Agent", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
		private Monitoring.Agent monitoringAgent = new QS.CMS.Monitoring.Agent("Monitoring agent in a physical platform.");
#endif

		private QS._core_c_.Base.IReadableLogger logger;
		private Devices_2_.UDPCommunicationsDevice udpDevice;
		private Base1_.PQAlarmClock alarmClock;
		private QS.Fx.Clock.IClock clock;
        private Devices_3_.Network network;
        private Devices_4_.INetwork newNetwork;
        private Devices_6_.Connections connections6;
        private Devices_7_.IConnections connections7;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Core.Core core;

		#region TMS.Base.IStatisticsCollector Members

		System.Collections.Generic.IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
            { 
#if DEBUG_EnableMonitoringAgent
                return ((TMS.Base.IStatisticsCollector)monitoringAgent).Statistics; 
#else
                return QS._qss_c_.Helpers_.ListOf<QS._core_c_.Components.Attribute>.Nothing;
#endif
            }
		}

		#endregion

		#region IPlatform Members

        QS.Fx.Logging.IEventLogger IPlatform.EventLogger
        {
            get { return Logging_1_.NoLogger.Logger; }
        }

        public void ReleaseResources()
        {
            switch (operatingMode)
            {
                case Mode.Normal:
                    network.ReleaseResources();
                    break;

                default:
                    break;
            }
        }

        public Devices_3_.INetwork Network
        {
            get
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return network;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public Devices_4_.INetwork NetworkConnections
        {
            get
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return newNetwork;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
        
        public IPAddress[] NICs
		{
			get
			{
				return Devices_2_.Network.LocalAddresses;
			}
		}

		public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return alarmClock;

                    case Mode.SingleThreaded:
                        return core;

                    default:
                        throw new NotSupportedException();
                }
			}
		}

		public QS.Fx.Logging.ILogger Logger
		{
			get
			{
				return logger;
			}
		}

		public Devices_2_.ICommunicationsDevice UDPDevice
		{
			get
			{
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return udpDevice;

                    default:
                        throw new NotSupportedException();
                }
			}
		}

		[QS.Fx.Base.Inspectable]
		public QS.Fx.Clock.IClock Clock
		{
			get
			{
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return clock;

                    case Mode.SingleThreaded:
                        return core;

                    default:
                        throw new NotSupportedException();
                }
            }
		}

        QS._qss_c_.Base6_.ICollectionOf<IPAddress, QS._qss_c_.Devices_6_.INetworkConnection> QS._qss_c_.Devices_6_.INetwork.Connections
        {
            get 
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return connections6;
                        
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        Devices_6_.ReceiveCallback Devices_6_.INetwork.ReceiveCallback
        {
            get 
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return ((Devices_6_.INetwork)connections6).ReceiveCallback; 

                    default:
                        throw new NotSupportedException();
                }
            }
            
            set 
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        ((Devices_6_.INetwork)connections6).ReceiveCallback = value;
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        Devices_7_.IConnections Virtualization_.ICommunicationSubsystem.Connections7
        {
            get
            {
                switch (operatingMode)
                {
                    case Mode.Normal:
                        return connections7;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        QS._core_c_.Core.ICore IPlatform.Core
        {
            get
            {
                switch (operatingMode)
                {
                    case Mode.SingleThreaded:
                        return core;

                    default:
                        throw new NotSupportedException();
                }
            }
        }

        #endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
            switch (operatingMode)
            {
                case Mode.Normal:
                    {
                        try
                        {
                            alarmClock.shutdown();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, "Cannot dispose the alarm clock: " + exc.ToString());
                        }

                        try
                        {
                            udpDevice.shutdown();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, "Cannot dispose the UCP communications device: " + exc.ToString());
                        }

#if DEBUG_EnableMonitoringAgent
                        try
                        {
                            ((IDisposable)monitoringAgent).Dispose();
                        }
                        catch (Exception exc)
                        {
                            logger.Log(this, "Cannot dispose the monitoring agent bastard: " + exc.ToString());
                        }
#endif
                    }
                    break;

                case Mode.SingleThreaded:
                    {
                        core.Dispose();
                    }
                    break;

                default:
                    break;
            }

        }

		#endregion
	}
}
