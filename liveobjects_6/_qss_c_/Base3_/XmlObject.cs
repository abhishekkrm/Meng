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
using System.IO;
using System.Xml.Serialization;

namespace QS._qss_c_.Base3_
{
    [QS.Fx.Serialization.ClassID(ClassID.Base3_XmlObject)]
    public class XmlObject : QS.Fx.Serialization.ISerializable
    {
        public XmlObject()
        {
        }

        public XmlObject(object obj)
        {
            if (obj == null)
                typeWrapper = new QS._core_c_.Base2.StringWrapper(string.Empty);
            else
            {
                typeWrapper = new QS._core_c_.Base2.StringWrapper(obj.GetType().AssemblyQualifiedName);

                MemoryStream memoryStream = new MemoryStream();
                (new XmlSerializer(obj.GetType())).Serialize(memoryStream, obj);
                objectAsBytes = new QS._core_c_.Base2.BlockOfData(memoryStream);
            }
        }

        private QS._core_c_.Base2.StringWrapper typeWrapper;
        private QS._core_c_.Base2.BlockOfData objectAsBytes;

        public object Object
        {
            get
            {
                string typeString = typeWrapper.String;
                if (typeString == string.Empty)
                    return null;
                else
                {
                    Type objtype = Type.GetType(typeString);
                    if (objtype == null)
                        throw new Exception("Could not load type \"" + typeString + "\".");
                    return (new XmlSerializer(objtype)).Deserialize(objectAsBytes.AsStream);
                }
            }
        }

        public override string ToString()
        {
            return "XmlObject(" + typeWrapper.String + ")";
        }

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Base3_XmlObject, 0, 0, 0);
                info.AddAnother(typeWrapper.SerializableInfo);
                if (objectAsBytes != null)
                    info.AddAnother(objectAsBytes.SerializableInfo);
                return info;
            }
        }

        void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            typeWrapper.SerializeTo(ref header, ref data);
            if (objectAsBytes != null)
                objectAsBytes.SerializeTo(ref header, ref data);
        }

        void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            typeWrapper = new QS._core_c_.Base2.StringWrapper();
            typeWrapper.DeserializeFrom(ref header, ref data);
            if (typeWrapper.String != string.Empty)
            {
                objectAsBytes = new QS._core_c_.Base2.BlockOfData();
                objectAsBytes.DeserializeFrom(ref header, ref data);
            }
        }

        #endregion
    }
}
