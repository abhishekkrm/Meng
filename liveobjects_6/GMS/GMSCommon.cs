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
using System.Net;
using System.IO;
using System.Collections;

namespace QS
{
	namespace GMS
	{
		[Serializable]
		[QS.Fx.Serialization.ClassID(QS.ClassID.GroupID)]
		public class GroupId : QS._core_c_.Base.IBase1Serializable, QS._qss_c_.Base2_.IIdentifiableKey
		{
			public static void register_serializable()
			{
				QS._core_c_.Base2.Serializer.CommonSerializer.registerClass(
					QS.ClassID.GroupID, typeof(QS.GMS.GroupId));
			}

			int id;
			public int ID
			{
				get
				{
					return id;
				}
			}
			public GroupId(int groupid)
			{
				id = groupid;
			}

			public GroupId()
			{
			}

//			// SPL
//			public override bool Equals(object obj)
//			{
//				if (obj.GetType() == this.GetType()) 
//				{
//					return ID == ((GroupId)obj).ID;
//				} 
//				else 
//				{
//					return false;
//				}
//			}
 

			public override string ToString()
			{
				return id.ToString();
			}

			public override int GetHashCode()
			{
				return id;
			}

			public override bool Equals(object obj)
			{
				return (obj is GroupId) && id.Equals(((GroupId) obj).id);
			}

			#region QS.CMS.Base.ISerializable Members

			public ClassID ClassIDAsSerializable
			{
				get
				{
					return ClassID.GroupID;
				}
			}

			public void load(System.IO.Stream memoryStream)
			{
				//memoryStream.Seek(0, SeekOrigin.Begin);
				byte[] buffer = new byte[4];
				memoryStream.Read(buffer, 0, 4);
				id = System.BitConverter.ToInt32(buffer, 0);
			}

			public void save(System.IO.Stream memoryStream)
			{
				byte[] buffer;
				buffer = System.BitConverter.GetBytes(id);
				memoryStream.Write(buffer, 0, buffer.Length);				
			}

			#endregion

			#region QS.CMS.Base2.ISerializable Members

			public uint Size
			{
				get
				{
					return QS._core_c_.Base2.SizeOf.Int32;
				}
			}

			public void load(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				this.id = QS._core_c_.Base2.Serializer.loadInt32(blockOfData);
			}

			public void save(QS._core_c_.Base2.IBlockOfData blockOfData)
			{
				QS._core_c_.Base2.Serializer.saveInt32(this.id, blockOfData);
			}

			public QS.ClassID ClassID
			{
				get
				{
					return QS.ClassID.GroupID;
				}
			}

			#endregion

			#region IComparable Members

			public int CompareTo(object obj)
			{
				return (obj is GroupId) ? this.id.CompareTo(((GroupId) obj).id) : -1;
			}

			#endregion

			#region QS.CMS.Base2.IIdentifiableKey Members

			public QS._qss_c_.Base2_.ContainerClass ContainerClass
			{
				get
				{
					return QS._qss_c_.Base2_.ContainerClass.DefaultContainer;
				}
			}

			#endregion
		}
	}
}
