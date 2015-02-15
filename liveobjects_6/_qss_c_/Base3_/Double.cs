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

namespace QS._qss_c_.Base3_
{
	[QS.Fx.Serialization.ClassID(ClassID.Double)]
    public class Double : QS.Fx.Serialization.ISerializable, QS.Fx.Serialization.IStringSerializable, Aggregation3_.IAggregatable, IKnownClass
	{
		public Double()
		{
		}

		public Double(double x)
		{
			this.x = x;
		}

		public Double(string s) : this(System.Convert.ToDouble(s))
		{
		}

		protected double x;

		public double Value
		{
			get { return x; }
			set { x = value; }
		}

		public override string ToString()
		{
			return x.ToString();
		}

		#region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
		{
            get { return new QS.Fx.Serialization.SerializableInfo((ushort)this.ClassID, (ushort)sizeof(double), (ushort)sizeof(double), 0); }
		}

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				*((double*)(arrayptr + header.Offset)) = x;
			}
			header.consume(sizeof(double));
		}

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
		{
			fixed (byte* arrayptr = header.Array)
			{
				x = *((double*)(arrayptr + header.Offset));
			}
			header.consume(sizeof(double));
		}

		#endregion

		#region IStringSerializable Members

        ushort QS.Fx.Serialization.IStringSerializable.ClassID
		{
			get { return (ushort) this.ClassID; }
		}

        string QS.Fx.Serialization.IStringSerializable.AsString
		{
			get { return x.ToString(); }
			set { x = System.Convert.ToDouble(value); }
		}

		#endregion

		#region IAggregatable Members

		void QS._qss_c_.Aggregation3_.IAggregatable.aggregateWith(QS._qss_c_.Aggregation3_.IAggregatable anotherObject)
		{
			Double o = anotherObject as Double;
			if (o != null)
			{
				aggregateWith(o.x);
			}
			else
				throw new Exception("Cannot aggregate element of type " + GetType().FullName + " with " +
					QS._core_c_.Helpers.ToString.ObjectRef(anotherObject));
		}

		protected virtual void aggregateWith(double y)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IKnownClass Members

		public virtual ClassID ClassID
		{
			get { return ClassID.Double; }
		}

		#endregion
	}
}
