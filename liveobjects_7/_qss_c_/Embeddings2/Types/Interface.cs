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

namespace QS._qss_c_.Embeddings2.Types
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Embeddings2_Types_Interface)]
    public class Interface : QS.Fx.Serialization.ISerializable, IEquatable<Interface>
    {
        public Interface()
        {
        }

        #region Main Constructor

        public Interface(System.Type type)
        {
            if (!type.IsInterface)
                throw new Exception("Only interfaces are supported.");

            // List<Property> properties = new List<Property>();
            // foreach (System.Reflection.PropertyInfo info in type.GetProperties())
            //    properties.Add(new Property(info.Name, new ReferencedType(info.PropertyType)));
            // this.properties = properties.ToArray();

            List<Method> methods = new List<Method>();
            foreach (System.Reflection.MethodInfo info in type.GetMethods())
            {
                List<Parameter> arguments = new List<Parameter>();
                foreach (System.Reflection.ParameterInfo parameterInfo in info.GetParameters())
                {
                    ParameterMode mode;
                    Type parameterType;
                    if (parameterInfo.ParameterType.IsByRef)
                    {
                        parameterType = parameterInfo.ParameterType.GetElementType();
                        mode = parameterInfo.IsOut ? ParameterMode.Output : ParameterMode.InputOutput;
                    }
                    else
                    {
                        parameterType = parameterInfo.ParameterType;
                        mode = ParameterMode.Input;
                    }

                    arguments.Add(new Parameter(mode, new ReferencedType(parameterType)));
                }
                methods.Add(new Method(info.Name, new ReferencedType(info.ReturnType), arguments.ToArray()));
            }
            this.methods = methods.ToArray();

            // TODO: Ultimately we should also check for events etc. crap, but for now, we don't care.........
        }

        #endregion

        // [QS.Fx.Printing.Printable]
        // private Property[] properties;

        [QS.Fx.Printing.Printable]
        private Method[] methods;

        #region Accessors

        // public Property[] Properties
        // {
        //    get { return properties; }
        // }

        public Method[] Methods
        {
            get { return methods; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Embeddings2_Types_Interface, sizeof(ushort), sizeof(ushort), 0);
                foreach (Method method in methods)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable)method).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = (ushort)methods.Length;
            }
            header.consume(sizeof(ushort));
            foreach (Method method in methods)
                ((QS.Fx.Serialization.ISerializable) method).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* pbuffer = header.Array)
            {
                count = (int)(*((ushort*)(pbuffer + header.Offset)));
            }
            header.consume(sizeof(ushort));
            methods = new Method[count];
            for (int ind = 0; ind < count; ind++)
                ((QS.Fx.Serialization.ISerializable)(methods[ind] = new Method())).DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IEquatable<Interface> Members

        bool IEquatable<Interface>.Equals(Interface other)
        {
            if (methods.Length == other.methods.Length)
            {
                for (int ind = 0; ind < methods.Length; ind++)
                {
                    if (!methods[ind].Equals(other.methods[ind]))
                        return false;
                }
                return true;
            }
            else
                return false;
        }

        #endregion
    }
}
