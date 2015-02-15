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
using System.Reflection;

namespace QS.Fx.Printing
{
    /// <summary>
    /// The class used to display objects in a human-friendly manner.
    /// </summary>
    public static class Printable
    {
        /// <summary>
        /// Create a human-friendly string representing the given object, using the annotations that drive the printing output.
        /// </summary>
        /// <param name="printedObject">The object to display.</param>
        /// <returns>The display string.</returns>
        public static string ToString(object printedObject)
        {
            if (printedObject == null)
                return "null";
            else
            {
                System.Type type = printedObject.GetType();
                object[] attributes = type.GetCustomAttributes(typeof(PrintableAttribute), false);
                PrintingStyle style = (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Style : PrintingStyle.Undefined;
                return ToString(printedObject, style, PrintableAttribute.DefaultStyle, 0, false);
            }
        }

        public static string ToString(
            object printedObject, QS.Fx.Printing.PrintingStyle preferredStyle, QS.Fx.Printing.PrintingStyle defaultStyle, uint indentationLevel, bool indentFirst)
        {
            if (printedObject is System.Collections.IDictionary)
            {
                StringBuilder s = new StringBuilder();
                s.Append("(");
                bool isfirst2 = true;
                foreach (System.Collections.DictionaryEntry element in ((System.Collections.IDictionary)printedObject))
                {
                    if (isfirst2)
                        isfirst2 = false;
                    else
                        s.Append(", ");

                    s.Append(ToString(element.Key, preferredStyle, defaultStyle, indentationLevel, indentFirst));
                    s.Append(" : ");
                    s.Append(ToString(element.Value, preferredStyle, defaultStyle, indentationLevel, indentFirst));
                }
                s.Append(")");
                return s.ToString();
            }
            else if (printedObject is System.Collections.ICollection)
            {
                StringBuilder s = new StringBuilder();
                s.Append("(");
                bool isfirst2 = true;
                foreach (object element in ((System.Collections.ICollection)printedObject))
                {
                    if (isfirst2)
                        isfirst2 = false;
                    else
                        s.Append(", ");

                    s.Append(ToString(element, preferredStyle, defaultStyle, indentationLevel, indentFirst));
                }
                s.Append(")");
                return s.ToString();
            }
            else
            {
                switch (preferredStyle)
                {
                    case PrintingStyle.Native:
                        return (printedObject != null) ? printedObject.ToString() : "null";
                    case PrintingStyle.Compact:
                        return ToString_Compact(printedObject);
                    case PrintingStyle.Expanded:
                        return ToString_Expanded(printedObject, indentationLevel, indentFirst);
                    case PrintingStyle.Undefined:
                    default:
                        {
                            switch (defaultStyle)
                            {
                                case PrintingStyle.Compact:
                                    return ToString_Compact(printedObject);
                                case PrintingStyle.Expanded:
                                    return ToString_Expanded(printedObject, indentationLevel, indentFirst);
                                case PrintingStyle.Native:
                                case PrintingStyle.Undefined:
                                default:
                                    return (printedObject != null) ? printedObject.ToString() : "null";
                            }
                        }
                }
            }
        }

        private static string ToString_Compact(object printedObject)
        {
            if (printedObject == null)
                return "null";
            else if (printedObject.GetType().GetCustomAttributes(typeof(PrintableAttribute), false).Length > 0)
            {
                StringBuilder s = new StringBuilder();
                System.Type type = printedObject.GetType();
                object[] attributes = type.GetCustomAttributes(typeof(PrintableAttribute), false);
                string name = null;
                SelectionOption option = SelectionOption.Implicit;
                if (attributes.Length > 0)
                {
                    PrintableAttribute attribute = attributes[0] as PrintableAttribute;
                    name = attribute.Name;
                    option = attribute.SelectionOption;
                }
                s.Append(name != null ? name : type.Name);
                s.Append("(");
                bool isfirst = true;
                foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if ((option == SelectionOption.Implicit &&
                        info.GetCustomAttributes(typeof(NonPrintableAttribute), true).Length == 0) ||
                        (option == SelectionOption.Explicit &&
                        info.GetCustomAttributes(typeof(PrintableAttribute), true).Length > 0))
                    {
                        if (isfirst)
                            isfirst = false;
                        else
                            s.Append(", ");

                        attributes = info.GetCustomAttributes(typeof(PrintableAttribute), false);
                        name = (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Name : null;
                        s.Append(name != null ? name : info.Name);
                        s.Append("=");
                        // s.Append(ToString_Compact(info.GetValue(printedObject)));
                        s.AppendLine(ToString(info.GetValue(printedObject), (attributes[0] as PrintableAttribute).Style, PrintingStyle.Compact, 0, false));
                    }
                }
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if ((option == SelectionOption.Implicit &&
                        info.GetCustomAttributes(typeof(NonPrintableAttribute), true).Length == 0) ||
                        (option == SelectionOption.Explicit &&
                        info.GetCustomAttributes(typeof(PrintableAttribute), true).Length > 0))
                    {
                        if (isfirst)
                            isfirst = false;
                        else
                            s.Append(", ");

                        attributes = info.GetCustomAttributes(typeof(PrintableAttribute), false);
                        name = (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Name : null;
                        s.Append(name != null ? name : info.Name);
                        s.Append("=");
                        s.AppendLine(ToString(info.GetValue(printedObject, null), (attributes[0] as PrintableAttribute).Style, PrintingStyle.Compact, 0, false));
                    }
                }
                s.Append(")");
                return s.ToString();
            }
            else if (printedObject is System.Collections.IDictionary)
            {
                StringBuilder s = new StringBuilder();
                s.Append("(");
                bool isfirst2 = true;
                foreach (System.Collections.DictionaryEntry element in ((System.Collections.IDictionary)printedObject))
                {
                    if (isfirst2)
                        isfirst2 = false;
                    else
                        s.Append(", ");

                    s.Append(ToString_Compact(element.Key));
                    s.Append(" : ");
                    s.Append(ToString_Compact(element.Value));
                }
                s.Append(")");
                return s.ToString();
            }
            else if (printedObject is System.Collections.ICollection)
            {
                StringBuilder s = new StringBuilder();
                // s.Append(printedObject.GetType().Name);
                s.Append("(");
                bool isfirst2 = true;
                foreach (object element in ((System.Collections.ICollection)printedObject))
                {
                    if (isfirst2)
                        isfirst2 = false;
                    else
                        s.Append(", ");

                    s.Append(ToString_Compact(element));
                }
                s.Append(")");
                return s.ToString();
            }
            else if (printedObject is System.Delegate)
            {
                StringBuilder s = new StringBuilder();
                s.Append("delegate(");
                System.Delegate d = printedObject as System.Delegate;
                if (d == null)
                    s.Append("null");
                else
                {
                    if (d.Target == null)
                        s.Append("null");
                    else
                    {
                        s.Append(ToString_Compact(d.Target));
                        s.Append(" : ");
                        s.Append(d.Target.GetType().ToString());
                    }
                    s.Append(", ");
                    if (d.Method == null)
                        s.Append("null");
                    else
                        s.Append(d.Method.Name);
                }
                s.Append(")");
                return s.ToString();
            }
            else
                return printedObject.ToString();
        }

        private const uint IndentationSpaces = 2;

        private static string ToString_Expanded(object printedObject, uint indentationLevel, bool indentFirst)
        {
            StringBuilder s = new StringBuilder();
            if (indentFirst)
                s.Append(' ', (int) (indentationLevel * IndentationSpaces));           
            if (printedObject == null)
            {
                s.Append("null");
            }
            else if (printedObject.GetType().GetCustomAttributes(typeof(PrintableAttribute), false).Length > 0)
            {
                System.Type type = printedObject.GetType();
                object[] attributes = type.GetCustomAttributes(typeof(PrintableAttribute), false);
                string name = null;
                SelectionOption option = SelectionOption.Implicit;
                PrintingStyle _printingstyle = PrintingStyle.Expanded;
                if (attributes.Length > 0)
                {
                    PrintableAttribute attribute = attributes[0] as PrintableAttribute;
                    name = attribute.Name;
                    option = attribute.SelectionOption;
                    _printingstyle = attribute.Style;
                }
                if (_printingstyle == PrintingStyle.Native)
                    s.Append(printedObject.ToString());
                else
                {
                    s.AppendLine(name != null ? name : type.Name);
                    s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                    s.AppendLine("{");
                    foreach (FieldInfo info in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if ((option == SelectionOption.Implicit &&
                            info.GetCustomAttributes(typeof(NonPrintableAttribute), true).Length == 0) ||
                            (option == SelectionOption.Explicit &&
                            info.GetCustomAttributes(typeof(PrintableAttribute), true).Length > 0))
                        {
                            attributes = info.GetCustomAttributes(typeof(PrintableAttribute), false);
                            name = (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Name : null;
                            s.Append(' ', (int)((indentationLevel + 1) * IndentationSpaces));
                            s.Append(name != null ? name : info.Name);
                            s.Append(" = ");
                            s.AppendLine(ToString(info.GetValue(printedObject), (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Style : PrintingStyle.Undefined,
                                PrintingStyle.Expanded, indentationLevel + 1, false));
                        }
                    }
                    foreach (PropertyInfo info in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if ((option == SelectionOption.Implicit &&
                            info.GetCustomAttributes(typeof(NonPrintableAttribute), true).Length == 0) ||
                            (option == SelectionOption.Explicit &&
                            info.GetCustomAttributes(typeof(PrintableAttribute), true).Length > 0))
                        {
                            attributes = info.GetCustomAttributes(typeof(PrintableAttribute), false);
                            name = (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Name : null;
                            s.Append(' ', (int)((indentationLevel + 1) * IndentationSpaces));
                            s.Append(name != null ? name : info.Name);
                            s.Append(" = ");
                            s.AppendLine(ToString(info.GetValue(printedObject, null), 
                                (attributes.Length > 0) ? (attributes[0] as PrintableAttribute).Style : PrintingStyle.Undefined,
                                PrintingStyle.Expanded, indentationLevel + 1, false));
                        }
                    }
                    s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                    s.Append("}");
                }
            }
            else if (printedObject is System.Collections.IDictionary)
            {
                s.AppendLine("dictionary");
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.AppendLine("{");
                foreach (System.Collections.DictionaryEntry element in ((System.Collections.IDictionary)printedObject))
                {
                    s.Append(' ', (int)((indentationLevel + 1) * IndentationSpaces));
                    s.Append(ToString_Compact(element.Key));
                    s.Append(" : ");
                    s.AppendLine(ToString_Expanded(element.Value, indentationLevel + 2, false));
                }
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.Append("}");
            }
            else if (printedObject is System.Collections.ICollection)
            {
                s.AppendLine("collection");
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.AppendLine("{");
                foreach (object element in ((System.Collections.ICollection)printedObject))
                    s.AppendLine(ToString_Expanded(element, indentationLevel + 1, true));
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.Append("}");
            }
            else if (printedObject is System.Delegate)
            {
                System.Delegate d = printedObject as System.Delegate;
                s.AppendLine("delegate");
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.AppendLine("{");
                s.Append(' ', (int)((indentationLevel + 1) * IndentationSpaces));
                s.Append("target = ");
                s.AppendLine(ToString_Compact(d.Target));
                s.Append(' ', (int)((indentationLevel + 1) * IndentationSpaces));
                s.Append("method = ");
                s.AppendLine((d.Method == null) ? "null" : d.Method.Name);
                s.Append(' ', (int)(indentationLevel * IndentationSpaces));
                s.Append("}");
            }
            else
            {
                s.Append(printedObject.ToString());
            }
            return s.ToString();
        }
    }
}
