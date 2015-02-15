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

namespace QS._qss_c_.Connections_
{
    [QS.Fx.Serialization.ClassID(ClassID.Connections_MethodCall)]
    public class MethodCall : QS.Fx.Serialization.ISerializable
    {
        public static QS.Fx.Serialization.ISerializable EncodeObject(object responseObject)
        {
            return new Base3_.BinaryObject((responseObject != null) ? responseObject : Base2_.NullObject.Object);
        }

        public static object DecodeObject(QS.Fx.Serialization.ISerializable responseObject)
        {
            if (responseObject is Base2_.NullObject)
                return null;
            else
                if (responseObject is Base3_.BinaryObject)
                {
                    object obj = ((Base3_.BinaryObject)responseObject).Object;
                    return (obj is Base2_.NullObject) ? null : obj;
                }
                else
                    throw new Exception("Unknown object type.");
        }

        public MethodCall()
        {
        }

        public MethodCall(string methodName, object[] arguments)
        {
            wrappedMethodName = new QS._core_c_.Base2.StringWrapper(methodName);
            wrappedArguments = new QS._qss_c_.Base3_.BinaryObject[arguments.Length];
            for (int ind = 0; ind < arguments.Length; ind++)
                wrappedArguments[ind] = (Base3_.BinaryObject) EncodeObject(arguments[ind]);
        }

        private QS._core_c_.Base2.StringWrapper wrappedMethodName;
        private Base3_.BinaryObject[] wrappedArguments;

        public string MethodName
        {
            get { return wrappedMethodName.String; }
        }

        public object[] Arguments
        {
            get
            {
                object[] arguments = new object[wrappedArguments.Length];
                for (int ind = 0; ind < arguments.Length; ind++)
                    arguments[ind] = DecodeObject(wrappedArguments[ind]);
                return arguments;
            }
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Connections_MethodCall, (ushort)sizeof(ushort), sizeof(ushort), 0);
                info.AddAnother(wrappedMethodName.SerializableInfo);
                foreach (Base3_.BinaryObject wrappedArgument in wrappedArguments)
                    info.AddAnother(((QS.Fx.Serialization.ISerializable) wrappedArgument).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = (ushort)wrappedArguments.Length;
            }
            header.consume(sizeof(ushort));
            wrappedMethodName.SerializeTo(ref header, ref data);
            foreach (Base3_.BinaryObject wrappedArgument in wrappedArguments)
                ((QS.Fx.Serialization.ISerializable)wrappedArgument).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int narguments;
            fixed (byte* pbuffer = header.Array)
            {
                narguments = (int)(*((ushort*)(pbuffer + header.Offset)));
            }
            header.consume(sizeof(ushort));
            wrappedMethodName = new QS._core_c_.Base2.StringWrapper();
            wrappedMethodName.DeserializeFrom(ref header, ref data);
            wrappedArguments = new QS._qss_c_.Base3_.BinaryObject[narguments];
            for (int ind = 0; ind < narguments; ind++)
            {
                wrappedArguments[ind] = new QS._qss_c_.Base3_.BinaryObject();
                ((QS.Fx.Serialization.ISerializable)wrappedArguments[ind]).DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
