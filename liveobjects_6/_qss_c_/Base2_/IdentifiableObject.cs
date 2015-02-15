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
	/// Summary description for IIdentifiableObject.
	/// </summary>
	public interface IIdentifiableObject : QS._core_c_.Base2.IBase2Serializable, Collections_1_.IBinaryTreeNode
	{
		IIdentifiableKey UniqueID
		{
			get;
		}
	}

	public interface IIdentifiableKey : QS._core_c_.Base2.IBase2Serializable, System.IComparable
	{
		QS._qss_c_.Base2_.ContainerClass ContainerClass 
		{
			get;
		}
	}

	[QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
	public abstract class IdentifiableObject : Collections_1_.GenericBinaryTreeNode, IIdentifiableObject
	{
		public IdentifiableObject()
		{
		}

		public override int GetHashCode()
		{
			int c1 = this.UniqueID.ClassID.GetHashCode();
			int c2 = this.UniqueID.GetHashCode();
			return  c1 ^ c2; 
		}

		public override bool Equals(object obj)
		{
			return (obj is IIdentifiableObject) && 
				this.UniqueID.ClassID.Equals(((IIdentifiableObject) obj).UniqueID.ClassID) &&
				this.UniqueID.Equals(((IIdentifiableObject) obj).UniqueID);
		}

		public override int CompareTo(object obj)
		{
			try
			{
				IIdentifiableKey anotherID;
				if (obj is IIdentifiableKey)
					anotherID = (IIdentifiableKey) obj;
				else if (obj is IIdentifiableObject)
					anotherID = ((IIdentifiableObject) obj).UniqueID;
				else
					throw new Exception("Object type mismatch in comparison, called IdentifiableObject:" + 
						this.GetType().Name + ".CompareTo(" + obj.GetType().Name + ")");

				int result = this.UniqueID.ClassID.CompareTo(anotherID.ClassID);
				return (result == 0) ? (this.UniqueID.CompareTo(anotherID)) : result;
			}
			catch (Exception exc)
			{
				throw new Exception("\nException caught in IdentifiableObject.CompareTo\n{" + 
					exc.ToString() + "\n" + exc.StackTrace + "\n}\n", exc);
			}
        }

		public override string ToString()
		{
			return this.UniqueID.ToString();
		}

		public override IComparable Contents
		{
			get
			{
				return this;
			}
		}

		#region IIdentifiableSerializableObject Members

		public abstract IIdentifiableKey UniqueID
		{
			get;
		}

		#endregion

		#region ISerializable Members

		public virtual uint Size
		{
			get
			{
				throw new Exception("Operation not supported in this context.");
			}
		}

		public virtual void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			throw new Exception("Operation not supported in this context.");
		}

		public virtual void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			throw new Exception("Operation not supported in this context.");
		}

		public virtual QS.ClassID ClassID
		{
			get
			{
				throw new Exception("Operation not supported in this context.");
			}
		}

		#endregion
	}

    [QS.Fx.Serialization.ClassID(QS.ClassID.Nothing)]
	public class IdentifiableKeyWrapper : IdentifiableObject
	{
		public IdentifiableKeyWrapper(QS.ClassID classID, IIdentifiableKey objectID)
		{
			this.classID = classID;
			this.objectID = objectID;
		}

		private QS.ClassID classID;
		private Base2_.IIdentifiableKey objectID;

		#region Overridden Members

		public override IIdentifiableKey UniqueID
		{
			get
			{
				return objectID;
			}
		}

		public override QS.ClassID ClassID
		{
			get
			{
				return classID;
			}
		}

		public override void load(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			throw new Exception("not supported");
		}

		public override void save(QS._core_c_.Base2.IBlockOfData blockOfData)
		{
			throw new Exception("not supported");			
		}

		public override uint Size
		{
			get
			{
				throw new Exception("not supported");			
			}
		}

		#endregion
	}
}
