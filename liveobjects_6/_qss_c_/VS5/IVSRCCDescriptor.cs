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

namespace QS._qss_c_.VS5
{
	/// <summary>
	/// This interface represents an object that represents contents of a virtually synchronous receiver container,
	/// for use in flushing.
	/// </summary>
	public interface IVSRCCDescriptor : QS._core_c_.Base2.IBase2Serializable
	{
		IVSRCCDescriptor aggregateWith(IVSRCCDescriptor[] descriptors);
	}

	[QS.Fx.Serialization.ClassID(ClassID.GenericRCCDescriptor)]
	public class GenericRCCDescriptor : IVSRCCDescriptor
	{
		public GenericRCCDescriptor()
		{
		}

		public GenericRCCDescriptor(uint numberOfMessages)
		{
			this.numberOfMessages = numberOfMessages;
		}

		private uint numberOfMessages;

		public uint NumberOfMessages
		{
			set
			{
				numberOfMessages = value;
			}

			get
			{
				return numberOfMessages;
			}
		}

		#region IVSRCCDescriptor Members

		public IVSRCCDescriptor aggregateWith(IVSRCCDescriptor[] descriptors)
		{
			GenericRCCDescriptor result = new GenericRCCDescriptor(0);
			foreach (GenericRCCDescriptor descriptor in descriptors)
			{
				if (descriptor.numberOfMessages > result.numberOfMessages)
					result.numberOfMessages = descriptor.numberOfMessages;
			}
			return result;
		}

		#endregion

		#region Base2.ISerializable Members

		public uint Size
		{
			get
			{
				return QS._core_c_.Base2.SizeOf.UInt32;
			}
		}

		public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			numberOfMessages = QS._core_c_.Base2.Serializer.loadUInt32(blockOfData);
		}

		public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			QS._core_c_.Base2.Serializer.saveUInt32(numberOfMessages, blockOfData);
		}

		public QS.ClassID ClassID
		{
			get
			{
				return ClassID.GenericRCCDescriptor;
			}
		}

		#endregion

		public override string ToString()
		{
			return "RCC(" + numberOfMessages.ToString() + ")";
		}
	}
}
