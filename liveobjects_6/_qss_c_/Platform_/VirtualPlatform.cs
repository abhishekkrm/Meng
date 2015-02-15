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
using System.Net;

namespace QS._qss_c_.Platform_
{
	/// <summary>
	/// Summary description for SimulatedPlatform.
	/// </summary>
	public class VirtualPlatform 
		: Virtualization_.VirtualCommunicationSubsystem, IPlatform, QS._qss_e_.Management_.IManagedComponent
	{
        public VirtualPlatform(QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, 
			Virtualization_.INetwork[] networks, QS._core_c_.Base.IReadableLogger logger) 
			: base(networks, logger)
		{
			this.clock = clock;
			this.alarmClock = alarmClock;
			// this.logger = logger;
		}

        public VirtualPlatform(QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, 
			Virtualization_.INetwork[] networks) : this(alarmClock, clock, networks, new QS._core_c_.Base.Logger(clock, true))
		{
		}

		// protected VirtualPlatform(Virtualization.INetwork[] networks) : this(null, networks)
		// {
		// }

        protected QS.Fx.Clock.IAlarmClock alarmClock;
		// protected Base.IReadableLogger logger;
		protected QS.Fx.Clock.IClock clock;

		public QS._core_c_.Base.IReadableLogger ReadableLogger
		{
			get
			{
				return logger;
			}
		}

		#region IPlatform Members

        QS.Fx.Logging.IEventLogger IPlatform.EventLogger
        {
            get { return Logging_1_.NoLogger.Logger; }
        }

        public new void ReleaseResources()
        {
            base.ReleaseResources();

            // TODO: Should release resources of the clock and alarm clock etc.
        }

		public QS.Fx.Logging.ILogger Logger
		{
			get
			{
				return logger;
			}
		}

        public QS.Fx.Clock.IAlarmClock AlarmClock
		{
			get
			{
				return alarmClock;
			}
		}

		public QS.Fx.Clock.IClock Clock
		{
			get
			{
				return clock;
			}
		}

        Devices_7_.IConnections Virtualization_.ICommunicationSubsystem.Connections7
        {
            get { throw new NotImplementedException(); }
        }

		#endregion

		#region IManagedComponent Members

		string QS._qss_e_.Management_.IManagedComponent.Name
		{
			get { return "Virtual Platform"; }
		}

		QS._qss_e_.Management_.IManagedComponent[] QS._qss_e_.Management_.IManagedComponent.Subcomponents
		{
			get { return null; }
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

        QS._core_c_.Core.ICore IPlatform.Core
        {
            get { throw new NotSupportedException(); }
        }
	}
}
