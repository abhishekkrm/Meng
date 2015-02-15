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

namespace QS._qss_c_.Buffering_3_
{
	public class ThresholdSender2 : Base3_.SenderClass<IBufferingSender>, Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>
	{
		public ThresholdSender2(Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection, IControllerClass controllerClass, 
			QS.Fx.Clock.IAlarmClock alarmClock, QS.Fx.Clock.IClock clock, QS.Fx.Logging.ILogger logger)		
		{
			this.underlyingCollection = underlyingCollection;
			this.controllerClass = controllerClass;
			this.logger = logger;
			this.alarmClock = alarmClock;
			this.clock = clock;
		}

		private Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingCollection;
		private IControllerClass controllerClass;
		private QS.Fx.Logging.ILogger logger;
		private QS.Fx.Clock.IAlarmClock alarmClock;
		private QS.Fx.Clock.IClock clock;

		protected override IBufferingSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return new Sender(this, destinationAddress);
		}

		#region ISenderClass<ISerializableSender> Members

		QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.SenderCollection
		{
			get
			{
				return new Base3_.SenderCollectionCast<Buffering_3_.IBufferingSender, QS._qss_c_.Base3_.ISerializableSender>(
					((Base3_.ISenderClass<IBufferingSender>)this).SenderCollection);
			}
		}

		QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderClass<QS._qss_c_.Base3_.ISerializableSender>.CreateSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
			return this.createSender(destinationAddress);
		}

		#endregion

		#region Class Sender

		public class Sender : Base3_.SerializableSender, IBufferingSender
		{
			public Sender(ThresholdSender2 owner, QS.Fx.Network.NetworkAddress destinationAddress) : base(owner.underlyingCollection[destinationAddress])
			{
				this.owner = owner;
				this.controller = owner.controllerClass.CreateController(underlyingSender.MTU);
			}

			private ThresholdSender2 owner;
			private IController controller;

			#region IBufferingSender Members

			public void flush()
			{
				throw new ArgumentException("Flushing is not supported with this sender.");
			}

			#endregion

			#region ISerializableSender Members

			public override void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
			{
				lock (this)
				{
					// ................................................................................
				}
			}

			public override int MTU
			{
				get { return controller.MTU; }
			}

			#endregion
		}

		#endregion
	}
}
