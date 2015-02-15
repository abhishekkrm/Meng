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

namespace QS._qss_c_.Base2_
{
	/// <summary>
	/// Summary description for NullObject.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.NullObject)]
    [Serializable]
	public class NullObject : QS._core_c_.Base2.IBase2Serializable, QS.Fx.Serialization.ISerializable
	{
		public static NullObject Object
		{
			get
			{
				return commonObject;
			}
		}

		private static NullObject commonObject = new NullObject();
		public NullObject()
		{
		}

		#region ISerializable Members

		public uint Size
		{
			get
			{
				return 0;
			}
		}

		public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
		}

		public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
		}

		public QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.NullObject;
			}
		}

		#endregion

        #region QS.Fx.Serialization.ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.NullObject, 0, 0, 0); }
        }

        public void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
        }

        public void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
        }

        #endregion
    }
}
