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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_c_.Base1_
{
    public static class StringSerializer
    {
        #region Type2String

        /// <summary>
        /// Serializes the name of the type.
        /// </summary>
        public static string Type2String(Type type)
        {
            return type.FullName;
        }

        #endregion

        #region String2Type

        /// <summary>
        /// Finds the type given its name.
        /// </summary>
        public static Type String2Type(string data)
        {
            Type result = null;
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(data);
                
                if (type != null)
                {
                    if (result != null)
                    {
                        string problem ="Cannot find type \"" + data + "\" because more than one instance of the type exists in the loaded assemblies.";
                        System.Diagnostics.Debug.Assert(false, problem);
                        throw new Exception(problem);
                    }

                    result = type;
                }
            }

            if (result == null)
            {
                string problem = "Cannot find type \"" + data + "\" because no such the type exists in any of the loaded assemblies.";
                System.Diagnostics.Debug.Assert(false, problem);
                throw new Exception(problem);
            }

            return result;
        }

        #endregion

        #region Object2String

        /// <summary>
        /// Serializes only the object.
        /// </summary>
        public static string Object2String(Type type, object obj)
        {
            MemoryStream memory_stream = new MemoryStream();
            (new XmlSerializer(type)).Serialize(memory_stream, obj);
            return Convert.ToBase64String(
                memory_stream.GetBuffer(), 0, (int)memory_stream.Length, Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// Serialized both the object's type and the object itself.
        /// </summary>
        public static string Object2String(object obj)
        {
            Type type = obj.GetType();
            string s1 = Type2String(type);
            string s2 = Object2String(type, obj);
            return s1 + "\n" + s2;
        }

        #endregion

        #region String2Object

        /// <summary>
        /// Deserializes only the object, given that the type is already known.
        /// </summary>
        public static object String2Object(Type type, string data)
        {
            return (new XmlSerializer(type)).Deserialize(new MemoryStream(Convert.FromBase64String(data)));
        }

        /// <summary>
        /// Deserializes the type first, and then the object of that type.
        /// </summary>
        public static object String2Object(string data)
        {
            int newline_position = data.IndexOf('\n');
            string s1 = data.Substring(0, newline_position);
            string s2 = data.Substring(newline_position + 1);
            Type type = String2Type(s1);
            return String2Object(type, s2);
        }

        #endregion

        #region SaveString

        public unsafe static void SaveString(Stream stream, string data)
        {
            byte[] bytes1 = new byte[sizeof(int)];
            byte[] bytes2 = System.Text.Encoding.ASCII.GetBytes(data);
            fixed (byte* pbuffer = bytes1)
            {
                *((int*)pbuffer) = bytes2.Length;
            }
            stream.Write(bytes1, 0, bytes1.Length);
            stream.Write(bytes2, 0, bytes2.Length);
        }

        #endregion

        #region LoadString

        public unsafe static string LoadString(Stream stream)
        {
            byte[] bytes1 = new byte[sizeof(int)];
            stream.Read(bytes1, 0, bytes1.Length);
            int bytes2_count;
            fixed (byte* pbuffer = bytes1)
            {
                bytes2_count = *((int*)pbuffer);
            }
            byte[] bytes2 = new byte[bytes2_count];
            stream.Read(bytes2, 0, bytes2.Length);
            return System.Text.Encoding.ASCII.GetString(bytes2);
        }

        #endregion

        #region SaveObject

        public static void SaveObject(Stream stream, object obj)
        {
            SaveString(stream, Object2String(obj));            
        }

        #endregion

        #region LoadObject

        public static object LoadObject(Stream stream)
        {
            return String2Object(LoadString(stream));
        }

        #endregion
    }
}
