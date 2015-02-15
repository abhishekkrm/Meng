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

namespace QS._core_c_.Components
{
    // [QS.Fx.Serialization.ClassID(ClassID.Attribute)]
    [System.Serializable]
    public struct Attribute : QS.Fx.Inspection.IScalarAttribute, QS.Fx.Serialization.ISerializable
    {
        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Attribute, 0, 0, 0);
                info.AddAnother((new Base2.StringWrapper(name)).SerializableInfo);

                Serialization.SerializationClass serializationClass;
                if ((this.data) is QS.Fx.Serialization.ISerializable)
                    serializationClass = QS._core_c_.Serialization.SerializationClass.Base3_ISerializable;
                else
                {
                    throw new NotSupportedException("Cannot serialize data that does not implement QS.Fx.Serialization.ISerializable.");
                    // (new Base3.XmlObject(........................
                }

                info.ExtendHeader(sizeof(byte));

                switch (serializationClass)
                {
                    case QS._core_c_.Serialization.SerializationClass.Base3_ISerializable:
                    {
                        info.ExtendHeader(sizeof(ushort));
                        info.AddAnother(((QS.Fx.Serialization.ISerializable) (this.data)).SerializableInfo);
                    }
                    break;
                }

                return info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            (new Base2.StringWrapper(name)).SerializeTo(ref header, ref data);
            
            Serialization.SerializationClass serializationClass;
            if ((this.data) is QS.Fx.Serialization.ISerializable)
                serializationClass = QS._core_c_.Serialization.SerializationClass.Base3_ISerializable;
            else
            {
                throw new NotSupportedException("Cannot serialize data that does not implement QS.Fx.Serialization.ISerializable.");
                // (new Base3.XmlObject(........................
            }

            fixed (byte* pbuffer = header.Array)
            {
                *(pbuffer + header.Offset) = (byte)serializationClass;
            }
            header.consume(sizeof(byte));

            switch (serializationClass)
            {
                case QS._core_c_.Serialization.SerializationClass.Base3_ISerializable:
                {
                    fixed (byte* pbuffer = header.Array)
                    {
                        *((ushort*)(pbuffer + header.Offset)) = (ushort) ((QS.Fx.Serialization.ISerializable) (this.data)).SerializableInfo.ClassID;
                    }
                    header.consume(sizeof(ushort));

                    ((QS.Fx.Serialization.ISerializable) (this.data)).SerializeTo(ref header, ref data);
                }
                break;
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            Base2.StringWrapper wrapper = new QS._core_c_.Base2.StringWrapper();
            wrapper.DeserializeFrom(ref header, ref data);
            name = wrapper.String;
            
            Serialization.SerializationClass serializationClass;
            fixed (byte* pbuffer = header.Array)
            {
                serializationClass = (Serialization.SerializationClass) (*(pbuffer + header.Offset));
            }
            header.consume(sizeof(byte));

            switch (serializationClass)
            {
                case QS._core_c_.Serialization.SerializationClass.Base3_ISerializable:
                {
                    ushort classID;
                    fixed (byte* pbuffer = header.Array)
                    {
                        classID = *((ushort*)(pbuffer + header.Offset));
                    }
                    header.consume(sizeof(ushort));

                    this.data = Base3.Serializer.CreateObject(classID);
                    ((QS.Fx.Serialization.ISerializable) (this.data)).DeserializeFrom(ref header, ref data);
                }
                break;
            }
        }

        #endregion

        public Attribute(string name, object data)
        {
            this.name = name;
            this.data = data;
        }

        private string name;
        private object data;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public object Value
        {
            get { return data; }
            set { data = value; }
        }

		#region IScalarAttribute Members

		object QS.Fx.Inspection.IScalarAttribute.Value
		{
			get { return data; }
		}

		#endregion

		#region IAttribute Members

		string QS.Fx.Inspection.IAttribute.Name
		{
			get { return name; }
		}

		QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
		}

		#endregion      
    }
}
