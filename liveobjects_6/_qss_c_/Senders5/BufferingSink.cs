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
	public class BufferingSink : ISimpleSink, ISource
	{
		public BufferingSink(IGenericSink messageSink, Buffering_3_.IControllerClass controllerClass)
		{
			this.messageSink = messageSink;
			this.controllerClass = controllerClass;
		}

		private IGenericSink messageSink;
		private ISource source;
		private Buffering_3_.IControllerClass controllerClass;

		#region ISimpleSink Members

		void IGenericSink.Signal()
		{
			messageSink.Signal();			
		}

		ISource ISimpleSink.Source
		{
			get { return source; }
			set { source = value; }
		}

		#endregion

		#region ISource Members

		bool ISource.Ready
		{
			get { return source.Ready; }
		}

		QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> ISource.Get(uint maximumSize)
		{
			System.Collections.Generic.List<Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>> messages =
				new List<QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>>();
			Buffering_3_.IController bufferingController = controllerClass.CreateController((int) maximumSize);
			Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message;
			while (source.Ready && (message = source.Get((uint)bufferingController.CurrentCapacity)) != null)
			{
				bufferingController.append(message.Argument.destinationLOID, message.Argument.transmittedObject);
				messages.Add(message);
			}
			bufferingController.flush();
			Buffering_3_.IMessageCollection transmittedObject = bufferingController.ReadyQueue.Dequeue();
			if (bufferingController.ReadyQueue.Count > 0 || !bufferingController.Empty)
				throw new Exception("Internal error: Encountered leftovers from buffering.");
			return new MessageCollection(transmittedObject, messages);
		}

		#endregion

		#region Class MessageCollection

		private class MessageCollection : Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>
		{
			public MessageCollection(QS.Fx.Serialization.ISerializable transmittedObject,
				System.Collections.Generic.IEnumerable<Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>> messages)
			{
				this.transmittedObject = transmittedObject;
				this.messages = messages;
			}

			private QS.Fx.Serialization.ISerializable transmittedObject;
			private System.Collections.Generic.IEnumerable<Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>> messages;

			#region IAsynchronousRequest<Message> Members

			QS._core_c_.Base3.Message QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Argument
			{
				get { return new QS._core_c_.Base3.Message((uint) ReservedObjectID.Unwrapper, transmittedObject); }
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Completed()
			{
				foreach (Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message in messages)
					message.Completed();
			}

			void QS._qss_c_.Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message>.Failed(Exception exception)
			{
				foreach (Base3_.IAsynchronousRequest<QS._core_c_.Base3.Message> message in messages)
					message.Failed(exception);
			}

			#endregion
		}

		#endregion
	}
}
