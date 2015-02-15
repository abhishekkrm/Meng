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

using System.Diagnostics;

namespace QS._qss_c_.Base3_
{
	public struct LoggedOperation : System.IDisposable
	{
		#region System.IDisposable Members

		void System.IDisposable.Dispose()
		{
			argument1 = argument2 = argument3 = null;
		}

		#endregion

		public static readonly IFormatter DefaultFormatter = new BaseFormatter();
		public interface IFormatter
		{
			string ToString(LoggedOperation operation);
		}

		private class BaseFormatter : IFormatter
		{
			string IFormatter.ToString(LoggedOperation operation)
			{
				StringBuilder s = new StringBuilder();
				s.Append("[");
				if (operation.timestamp != double.NegativeInfinity)
				{
					s.Append(operation.timestamp.ToString("000000000.000000000"));
					s.Append(":");
				}
				s.Append(operation.type.Name);
				s.Append(".");
				s.Append(System.Enum.GetName(operation.type, operation.operationCode));
				if (operation.argument1 != null || operation.argument2 != null || operation.argument3 != null)
				{
					s.Append("(");
					bool separate = false;
					if (operation.argument1 != null)
					{
						AppendString(s, operation.argument1);
						separate = true;
					}
					if (operation.argument2 != null)
					{
						if (separate)
							s.Append(", ");
						AppendString(s, operation.argument2);
						separate = true;
					}
					if (operation.argument3 != null)
					{
						if (separate)
							s.Append(", ");
						AppendString(s, operation.argument3);
					}
					s.Append(")");
				}
				s.Append("]");
				return s.ToString();
			}
		}

		private static void AppendString(StringBuilder s, object o)
		{
			if (o == null)
			{
				s.Append("null");
			}
			else if (o is System.Collections.IEnumerable)
			{
				s.Append("(");
				bool isfirst = true;
				foreach (object x in ((System.Collections.IEnumerable) o))
				{
					if (isfirst)
						isfirst = false;
					else
						s.Append(", ");
					AppendString(s, x);
				}
				s.Append(")");
			}
			else
			{
				s.Append(o.ToString());
			}
		}

		public static LoggedOperation Operation<C>(C operation)
			where C : IConvertible
		{
			return Operation<C>(double.NegativeInfinity, operation);
		}

		public static LoggedOperation Operation<C>(C operation, object argument1)
			where C : IConvertible
		{
			return Operation<C>(double.NegativeInfinity, operation, argument1);
		}

		public static LoggedOperation Operation<C>(C operation, object argument1, object argument2)
			where C : IConvertible
		{
			return Operation<C>(double.NegativeInfinity, operation, argument1, argument2);
		}

		public static LoggedOperation Operation<C>(C operation, 
			object argument1, object argument2, object argument3) where C : IConvertible
		{
			return Operation<C>(double.NegativeInfinity, operation, argument1, argument2, argument3);
		}

		public static LoggedOperation Operation<C>(double timestamp, C operation)
			where C : IConvertible
		{
			return Operation<C>(timestamp, operation, null);
		}

		public static LoggedOperation Operation<C>(double timestamp, C operation, object argument1)
			where C : IConvertible
		{
			return Operation<C>(timestamp, operation, argument1, null);
		}

		public static LoggedOperation Operation<C>(double timestamp, C operation, 
			object argument1, object argument2) where C : IConvertible
		{
			return Operation<C>(timestamp, operation, argument1, argument2, null);
		}

		public static LoggedOperation Operation<C>(double timestamp, C operation,
			object argument1, object argument2, object argument3) where C : IConvertible
		{
			return Operation<C>(DefaultFormatter, timestamp, operation, argument1, argument2, argument3);
		}

		public static LoggedOperation Operation<C>(IFormatter formatter, double timestamp, C operation,
			object argument1, object argument2, object argument3) where C : IConvertible
		{
			Debug.Assert(
				operation.GetTypeCode().Equals(TypeCode.Int16) ||
				operation.GetTypeCode().Equals(TypeCode.Int32) ||
				operation.GetTypeCode().Equals(TypeCode.UInt16) ||
				operation.GetTypeCode().Equals(TypeCode.UInt32));
			return new LoggedOperation(operation.GetType(), operation.ToInt32(null), timestamp, formatter,
				argument1, argument2, argument3);
		}

		private LoggedOperation(System.Type type, System.Int32 operationCode, double timestamp,
			IFormatter formatter, object argument1, object argument2, object argument3)
		{
			Debug.Assert(argument1 != null || argument2 == null);

			this.type = type;
			this.operationCode = operationCode;
			this.timestamp = timestamp;
			this.argument1 = argument1;
			this.argument2 = argument2;
			this.argument3 = argument3;
			this.formatter = formatter;
		}

		private double timestamp;
		private System.Int32 operationCode;
		private object argument1, argument2, argument3;
		private IFormatter formatter;
		private System.Type type;

		public System.Type Type
		{
			get { return type; }
//			set { type = value; }
		}

		public System.Int32 Code
		{
			get { return operationCode; }
//			set { operationCode = value; }
		}

		public object OperationObject
		{
			get { return System.Enum.ToObject(type, operationCode); }
//			set 
//			{ 
//			}
		}

		public double TimeStamp
		{
			get { return timestamp; }
//			set { timestamp = value; }
		}

		public object Argument1
		{
			get { return argument1; }
//			set  { argument1 = value; }
		}

		public object Argument2
		{
			get { return argument1; }
//			set { argument1 = value; }
		}

		public object Argument3
		{
			get { return argument1; }
//			set { argument1 = value; }
		}

		public IFormatter Formatter
		{
			get { return formatter; }
//			set { formatter = value; }
		}

		public override string ToString()
		{
			return formatter.ToString(this);
		}
	}
}
