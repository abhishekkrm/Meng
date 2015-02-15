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
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Senders5
{
	public class SendingSink : ISimpleSink
	{
		#region Class Collection

		public class Collection : Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink>
		{
			public Collection(Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender> underlyingSenderCollection, 
				System.Type flowControllerClass) // : base()
			{
				if (!typeof(FlowControl4.IFlowController).IsAssignableFrom(flowControllerClass))
					throw new ArgumentException("Wrong flow controller class.");
				System.Reflection.ConstructorInfo flowControllerClassConstructorInfo = 
					flowControllerClass.GetConstructor(Type.EmptyTypes);
				if (flowControllerClassConstructorInfo == null)
					throw new ArgumentException("The given flow controller class does not have a no-argument constructor.");	
			
				this.Callback = new Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink>.ConstructorCallback(
				delegate(QS.Fx.Network.NetworkAddress address)
				{
					return new CollectingSink(new SendingSink(underlyingSenderCollection[address],
						(FlowControl4.IFlowController)flowControllerClassConstructorInfo.Invoke(new object[] { })));
				});
			}
		}

		#endregion

		public SendingSink(QS._qss_c_.Base3_.ISerializableSender underlyingSender, FlowControl4.IFlowController flowController)
			: this(underlyingSender, flowController, null)
		{
		}

		public SendingSink(
			QS._qss_c_.Base3_.ISerializableSender underlyingSender, FlowControl4.IFlowController flowController, ISource source)
		{
			this.flowController = flowController;
			this.underlyingSender = underlyingSender;
			this.source = source;
		}

		private FlowControl4.IFlowController flowController;
		private bool waiting = false;
		private QS._qss_c_.Base3_.ISerializableSender underlyingSender;
		private ISource source;

		#region Internal Processing

		private void Send()
		{
			Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> request;
			while (source.Ready && flowController.Ready && (request = source.Get((uint)underlyingSender.MTU)) != null)
			{
				underlyingSender.send(request.Argument.destinationLOID, request.Argument.transmittedObject);
				flowController.Consume();
			}
		}

		private void ReadyCallback()
		{
			lock (this)
			{
				Send();
				if (source.Ready)
					flowController.Wait(new QS._qss_c_.Base3_.NoArgumentCallback(ReadyCallback));
				else
					waiting = false;
			}
		}

		#endregion

		#region ISimpleSink Members

		ISource ISimpleSink.Source
		{
			get { return source; }
			set { source = value; }
		}

		void IGenericSink.Signal()
		{
			lock (this)
			{
				Send();
				if (!waiting && source.Ready)
				{
					waiting = true;
					flowController.Wait(new QS._qss_c_.Base3_.NoArgumentCallback(ReadyCallback));
				}
			}
		}

		#endregion

//		#region IDisposable Members
//
//		void IDisposable.Dispose()
//		{
//		}
//
//		#endregion
	}
}
