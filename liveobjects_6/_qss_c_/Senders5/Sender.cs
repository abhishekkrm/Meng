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
	public class Sender : QS._qss_c_.Base3_.ISerializableSender, Base3_.IReliableSerializableSender
	{
		#region Class Collection

		public class Collection : Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, Sender>,
			Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>, Base3_.ISenderCollection<Base3_.IReliableSerializableSender>
		{
			public Collection(Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink> sinkCollection)
			{
				this.Callback = new Base3_.AutomaticCollection<QS.Fx.Network.NetworkAddress, Sender>.ConstructorCallback(
				delegate(QS.Fx.Network.NetworkAddress address)
				{					
					return new Sender(address, sinkCollection);
				});
			}

			#region ISenderCollection<ISerializableSender> Members

			QS._qss_c_.Base3_.ISerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.ISerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
			{
				get { return ((Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, Sender>) this)[destinationAddress]; }
			}

			#endregion

			#region ISenderCollection<IReliableSerializableSender> Members

			QS._qss_c_.Base3_.IReliableSerializableSender QS._qss_c_.Base3_.ISenderCollection<QS._qss_c_.Base3_.IReliableSerializableSender>.this[QS.Fx.Network.NetworkAddress destinationAddress]
			{
				get { return ((Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, Sender>)this)[destinationAddress]; }
			}

			#endregion

			#region IAttributeCollection Members

			IEnumerable<string> QS.Fx.Inspection.IAttributeCollection.AttributeNames
			{
				get 
				{
					foreach (QS.Fx.Network.NetworkAddress address in collection.Keys)
						yield return ((QS.Fx.Serialization.IStringSerializable)address).AsString;
				}
			}

			QS.Fx.Inspection.IAttribute QS.Fx.Inspection.IAttributeCollection.this[string attributeName]
			{
				get 
				{
					QS.Fx.Network.NetworkAddress address = new QS.Fx.Network.NetworkAddress();
					((QS.Fx.Serialization.IStringSerializable)address).AsString = attributeName;
					return new QS.Fx.Inspection.ScalarAttribute(attributeName, collection[address]);
				}
			}

			#endregion

			#region IAttribute Members

			string QS.Fx.Inspection.IAttribute.Name
			{
				get { return "Sender Collection"; }
			}

			QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
			}

			#endregion
		}

		#endregion

		public Sender(QS.Fx.Network.NetworkAddress destinationAddress, IGenericSink messageSink)
		{
			this.destinationAddress = destinationAddress;
			this.messageSink = messageSink;
		}

		public Sender(QS.Fx.Network.NetworkAddress destinationAddress, 
			Base3_.IAutomaticCollection<QS.Fx.Network.NetworkAddress, ICollectingSink> sinkCollection)
		{
			this.destinationAddress = destinationAddress;
			this.messageSink = sinkCollection[destinationAddress].Register(requestQueue);
		}

		private QS.Fx.Network.NetworkAddress destinationAddress;
		private IGenericSink messageSink;
		private RequestQueue requestQueue = new RequestQueue();

		public ISource Source
		{
			get { return requestQueue; }
		}

		#region ISerializableSender Members

		QS.Fx.Network.NetworkAddress QS._qss_c_.Base3_.ISerializableSender.Address
		{
			get { return destinationAddress; }
		}

		void QS._qss_c_.Base3_.ISerializableSender.send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
		{
			requestQueue.Enqueue(
				new Base3_.AsynchronousRequest1<QS._core_c_.Base3.Message>(new QS._core_c_.Base3.Message(destinationLOID, data)));
			messageSink.Signal();
		}

		int QS._qss_c_.Base3_.ISerializableSender.MTU
		{
            get { return int.MaxValue; } // needs to be changed
		}

		#endregion

		#region IComparable Members

		int IComparable.CompareTo(object obj)
		{
			return destinationAddress.CompareTo(
				(obj is QS._qss_c_.Base3_.ISerializableSender) ? (obj as QS._qss_c_.Base3_.ISerializableSender).Address : obj);
		}

		#endregion

		#region IReliableSerializableSender Members

		Base3_.IAsynchronousOperation Base3_.IReliableSerializableSender.BeginSend(uint destinationLOID, 
			QS.Fx.Serialization.ISerializable data, Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
		{
			Base3_.AsynchronousRequest3<QS._core_c_.Base3.Message> asynchronousRequest = 
				new Base3_.AsynchronousRequest3<QS._core_c_.Base3.Message>(new QS._core_c_.Base3.Message(destinationLOID, data), 
				completionCallback, asynchronousState);
			requestQueue.Enqueue(asynchronousRequest);
			return asynchronousRequest;
		}

		void QS._qss_c_.Base3_.IReliableSerializableSender.EndSend(QS._qss_c_.Base3_.IAsynchronousOperation asynchronousOperation)
		{
		}

		#endregion
	}
}
