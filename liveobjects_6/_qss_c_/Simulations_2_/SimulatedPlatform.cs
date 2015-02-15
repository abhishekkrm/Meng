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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;

namespace QS._qss_c_.Simulations_2_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
	public class SimulatedPlatform : QS.Fx.Inspection.Inspectable, ISimulatedPlatform
	{
		public SimulatedPlatform(QS.Fx.Logging.IEventLogger eventLogger,
			ISimulatedCPU simulatedCPU, Virtualization_.INetwork[] networks, int incomingQueueSize) 
		{
            this.eventLogger = eventLogger;
            // eventLogger = new QS.CMS.Logging.EventLogger(simulatedCPU, true);

			this.simulatedCPU = simulatedCPU; 
			logger = new QS._qss_c_.Base3_.Logger(simulatedCPU);
			communicationSubsystem = 
				new SimulatedCommunicationSubsystem(simulatedCPU, networks, incomingQueueSize);
            compatibilityWrapper = new QS._qss_c_.Devices_4_.CompatibilityWrapper(communicationSubsystem.Network);

            
		}

		[QS.Fx.Base.Inspectable("CPU", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private ISimulatedCPU simulatedCPU;
		[QS.Fx.Base.Inspectable("Log", QS.Fx.Base.AttributeAccess.ReadOnly)]
		private Base3_.Logger logger;
        [QS._core_c_.Diagnostics.Component("Communication Subsystem")]
		private SimulatedCommunicationSubsystem communicationSubsystem;
        private Devices_4_.INetwork compatibilityWrapper;

        // [TMS.Inspection.Inspectable("Event Log", QS.TMS.Inspection.AttributeAccess.ReadOnly)]
        private QS.Fx.Logging.IEventLogger eventLogger;

		#region ISimulatedPlatform Members

		ISimulatedCPU ISimulatedPlatform.SimulatedCPU
		{
			get { return simulatedCPU; }
		}

		#endregion

		#region IPlatform Members

        QS.Fx.Logging.IEventLogger QS._qss_c_.Platform_.IPlatform.EventLogger
        {
            get { return eventLogger; }
        }

        void QS._qss_c_.Platform_.IPlatform.ReleaseResources()
        {
            communicationSubsystem.ReleaseResources();

            // ............................technically, we should probably also remove any scheduled events
        }

        QS.Fx.Logging.ILogger QS._qss_c_.Platform_.IPlatform.Logger
		{
			get { return logger; }
		}

        QS.Fx.Clock.IAlarmClock QS._qss_c_.Platform_.IPlatform.AlarmClock
		{
			get { return simulatedCPU; }
		}

        QS.Fx.Clock.IClock QS._qss_c_.Platform_.IPlatform.Clock
		{
			get { return simulatedCPU; }
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			communicationSubsystem.Dispose();
		}

		#endregion

		#region ICommunicationSubsystem Members

		System.Net.IPAddress[] QS._qss_c_.Virtualization_.ICommunicationSubsystem.NICs
		{
			get { return communicationSubsystem.NICs; }
		}

		QS._qss_c_.Devices_2_.ICommunicationsDevice QS._qss_c_.Virtualization_.ICommunicationSubsystem.UDPDevice
		{
			get { return communicationSubsystem.UDPDevice; }
		}

		QS._qss_c_.Devices_3_.INetwork QS._qss_c_.Virtualization_.ICommunicationSubsystem.Network
		{
			get { return communicationSubsystem.Network; }
		}

        QS._qss_c_.Devices_4_.INetwork QS._qss_c_.Virtualization_.ICommunicationSubsystem.NetworkConnections
        {
            get { return compatibilityWrapper; }
        }

        Devices_7_.IConnections Virtualization_.ICommunicationSubsystem.Connections7
        {
            get { throw new NotImplementedException(); }
        }

		#endregion

		#region IManagedComponent Members

		string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Platform"; }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return new QS._qss_e_.Management_.IManagedComponent[] { simulatedCPU, communicationSubsystem }; }
		}

		QS._core_c_.Base.IOutputReader QS._qss_e_.Management_.IManagedComponent.Log
		{
			get { return logger; }
		}

		object QS._qss_e_.Management_.IManagedComponent.Component
		{
			get { return this; }
		}

		#endregion

        #region Devices6.INetwork Members

        QS._qss_c_.Base6_.ICollectionOf<IPAddress, QS._qss_c_.Devices_6_.INetworkConnection> QS._qss_c_.Devices_6_.INetwork.Connections
        {
            get { throw new NotSupportedException(); }
        }

        Devices_6_.ReceiveCallback Devices_6_.INetwork.ReceiveCallback
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        #endregion

        QS._core_c_.Core.ICore QS._qss_c_.Platform_.IPlatform.Core
        {
            get { throw new NotSupportedException(); }
        }
	}
}
