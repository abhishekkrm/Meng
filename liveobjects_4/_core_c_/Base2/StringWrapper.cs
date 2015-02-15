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

namespace QS._core_c_.Base2
{
	/// <summary>
	/// Summary description for StringWrapper.
	/// </summary>
	[QS.Fx.Serialization.ClassID(QS.ClassID.Base2_StringWrapper)]
	public class StringWrapper : Base2.BlockOfData, QS.Fx.Serialization.ISerializable
	{
		private static bool dummy = register_serializable();
		public static bool register_serializable()
		{
			Base2.Serializer.CommonSerializer.registerClass((ushort) QS.ClassID.Base2_StringWrapper, typeof(StringWrapper));
			return true;
		}

		public StringWrapper()
		{
		}

		public StringWrapper(string s) : base(System.Text.Encoding.Unicode.GetBytes(s))
		{
		}

        public StringWrapper(params string[] s) : this(ToString(s))
        {
        }

        private static string ToString(string[] s)
        {
            System.Text.StringBuilder ss = new System.Text.StringBuilder();
            foreach (string x in s)
                ss.Append(x);
            return ss.ToString();
        }

		public string String
		{
			get
			{
				return System.Text.Encoding.Unicode.GetString(
					this.Buffer, (int) this.OffsetWithinBuffer, (int) this.SizeOfData);
			}
		}

		public override string ToString()
		{
			return this.String;
		}

		#region ISerializable Members

		public override QS.ClassID ClassID
		{
			get
			{
				return QS.ClassID.Base2_StringWrapper;
			}
		}

		#endregion

        #region ISerializable Members

        public new QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = base.SerializableInfo;
                info.ClassID = (ushort) QS.ClassID.Base2_StringWrapper;
                return info;
            }
        }

        #endregion
    }
}
