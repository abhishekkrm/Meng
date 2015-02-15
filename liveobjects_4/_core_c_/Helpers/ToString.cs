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

namespace QS._core_c_.Helpers
{
    public static class ToString
    {
        public static string Exception(Exception exception)
        {
            StringBuilder s = new StringBuilder();
            while (exception != null)
            {
                s.AppendLine("Message = \"" + exception.Message + "\"\nSource = " + exception.Source.GetType().ToString() +
                    "\nStack Trace = \n" + exception.StackTrace + "\n");
                exception = exception.InnerException;
                if (exception != null)
                    s.AppendLine("----------------------------------------------------------------------------------------------");
            }
            return s.ToString();
        }

        public static string Attributes(QS._core_c_.Components.AttributeSet attributes)
        {
            return Attributes((QS.Fx.Inspection.IAttributeCollection) attributes);
        }

        public static string Attributes(QS.Fx.Inspection.IAttributeCollection attributes)
        {
            StringBuilder s = new StringBuilder();
            _processAttributes(s, 0, attributes);
            return s.ToString();
        }

        private const int tab_size = 2;
        private static void _processAttributes(StringBuilder s, int level, QS.Fx.Inspection.IAttributeCollection attributes)
        {
            s.Append("\n");
            s.Append(new string(' ', tab_size * level));
            s.Append("{\n");
            foreach (string name in attributes.AttributeNames)
            {
                QS.Fx.Inspection.IAttribute attribute = attributes[name];
                s.Append(new string(' ', tab_size * (level + 1)));
                s.Append(name);
                if (!attribute.Name.Equals(name))
                {
                    s.Append(" : ");
                    s.Append(attribute.Name);
                }
                s.Append(" = ");
                if (attribute.AttributeClass == QS.Fx.Inspection.AttributeClass.COLLECTION)
                {
                    _processAttributes(s, level + 1, (QS.Fx.Inspection.IAttributeCollection)attribute);
                }
                else
                {
                    object value = ((QS.Fx.Inspection.IScalarAttribute)attribute).Value;
                    s.Append(QS.Fx.Printing.Printable.ToString(value));
                    s.Append("\n");
                }
            }
            s.Append(new string(' ', tab_size * level));
            s.Append("}\n");
        }

        public static string CallbackException(Delegate callback, Exception exception)
        {
            StringBuilder s = new StringBuilder();
            s.Append("An exception was thrown while invoking ");
            if (callback != null)
            {
                s.Append("method ");
                s.Append((callback.Method != null) ? ("\"" + callback.Method.Name + "\" ") : "(null method) ");
                s.Append("against ");
                if (callback.Target != null)
                {
                    string target_string;
                    try
                    {
                        target_string = callback.Target.ToString();                        
                    }
                    catch (Exception)
                    {
                        target_string = null;
                    }
                    if (target_string != null)
                    {
                        s.Append("\"");
                        s.Append(target_string);
                        s.Append("\"");
                    }
                    else
                        s.Append("(no name)");
                    s.Append(" : ");
                    s.Append(callback.Target.GetType().ToString());
                    s.Append(" ");
                }
                else
                    s.Append("(null target) ");
            }
            else
                s.Append("(null callback) ");
            s.Append(".\n");
            s.Append(exception.ToString());
            return s.ToString();
        }

        public static string Strings(params string[] ss)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in ss)
                sb.Append(s);
            return sb.ToString();
        }

        public static string Object(object o)
        {
            return (o != null) ? o.ToString() : "(null)";
        }

        public static string ObjectRef(object o)
        {
            if (o != null)
            {
                StringBuilder s = new StringBuilder(o.GetType().Name);
                s.Append("(");
                s.Append(o.ToString());
                s.Append(")");
                return s.ToString();
            }
            else
                return "(null)";
        }

        public static string ReceivedObject(QS.Fx.Network.NetworkAddress sourceAddress, object o)
        {
            StringBuilder s = new StringBuilder((o != null) ? o.GetType().Name : "(null)");
            s.Append(" from ");
            s.Append(sourceAddress.ToString());
            if (o != null)
            {
                s.Append(":\n");
                s.Append(o.ToString());
                s.Append("\n");
            }
            return s.ToString();
        }

		public static string ReceivedObject(QS._core_c_.Base3.InstanceID sourceIID, object o)
		{
			StringBuilder s = new StringBuilder((o != null) ? o.GetType().Name : "(null)");
			s.Append(" from ");
			s.Append(sourceIID.ToString());
			if (o != null)
			{
				s.Append(":\n");
				s.Append(o.ToString());
				s.Append("\n");
			}
			return s.ToString();
		}

        public static string Message(System.Runtime.Remoting.Messaging.IMessage message)
        {
            StringBuilder s = new StringBuilder(message.GetType().Name);
            s.Append("\n(\n");
            for (System.Collections.IDictionaryEnumerator enumerator = message.Properties.GetEnumerator(); enumerator.MoveNext(); )
            {
                s.Append(Object(enumerator.Key));
                s.Append(" = ");
                s.AppendLine(Object(enumerator.Value));
            }
            s.Append(")\n");
            return s.ToString();
        }

		public static string Delegate(System.Delegate o)
		{
			StringBuilder s = new StringBuilder();
			s.Append(o.Target.GetType().Name);
			s.Append("(");
			s.Append(o.Target.ToString());
			s.Append(").");
			s.Append(o.Method.Name);
			return s.ToString();
		}
	}
}
