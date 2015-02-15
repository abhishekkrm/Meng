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
    [QS.Fx.Serialization.ClassID(ClassID.Connections_Message)]
    public class Message : QS.Fx.Serialization.ISerializable
    {
        public enum Type : byte
        {
            Request, OneWayRequest, Response
        }

        #region Constructors

        public Message()
        {
        }

        public Message(QS.Fx.Serialization.ISerializable argumentObject)
        {
            this.type = Type.OneWayRequest;
            this.argumentObject = argumentObject;
        }

        public Message(uint sequenceNo, QS.Fx.Serialization.ISerializable argumentObject)
        {
            this.type = Type.Request;
            this.sequenceNo = sequenceNo;
            this.argumentObject = argumentObject;
        }

        public Message(uint sequenceNo, bool succeeded, QS.Fx.Serialization.ISerializable argumentObject, Exception exception)
        {
            if (succeeded)
            {
                if (exception != null)
                    throw new ArgumentException("If succeeded, exception should be null.");
            }
            else
            {
                if (exception == null)
                    throw new ArgumentException("If not succeeded, exception cannot be null.");
            }

            this.type = Type.Response;
            this.sequenceNo = sequenceNo;
            this.succeeded = succeeded;
            this.argumentObject = argumentObject;
            this.exceptionWrapper = (exception != null) ? new QS._core_c_.Base2.StringWrapper(exception.ToString()) : null;
        }

        #endregion

        private Type type;
        private QS.Fx.Serialization.ISerializable argumentObject;

        private uint sequenceNo;
        private bool succeeded;
        private QS._core_c_.Base2.StringWrapper exceptionWrapper;

        #region Accessors

        public Type TypeOf
        {
            get { return type; }
        }

        public QS.Fx.Serialization.ISerializable ArgumentObject
        {
            get { return argumentObject; }
        }

        public uint SequenceNo
        {
            get { return sequenceNo; }
        }

        public bool Succeeded
        {
            get { return succeeded; }
        }

        public System.Exception Exception
        {
            get { return (exceptionWrapper != null) ? new Exception(exceptionWrapper.String) : null; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                int size = sizeof(byte) + sizeof(ushort);
                if (type != Type.OneWayRequest)
                {
                    size += sizeof(uint);
                    if (type == Type.Response)
                        size += sizeof(bool);
                }

                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Connections_Message, (ushort) size, size, 0);
                if (argumentObject != null)
                    info.AddAnother(argumentObject.SerializableInfo);                    
                if (exceptionWrapper != null)
                    info.AddAnother(exceptionWrapper.SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            bool withSequenceNo = (type != Type.OneWayRequest), withSucceeded = false, withException = false;            
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                *pheader = (byte) type;
                *((ushort*)(pheader + sizeof(byte))) = 
                    (argumentObject != null) ? argumentObject.SerializableInfo.ClassID : (ushort) ClassID.Nothing;
                if (withSequenceNo)
                {
                    *((uint*)(pheader + sizeof(byte) + sizeof(ushort))) = sequenceNo;
                    if (withSucceeded = (type == Type.Response))
                    {
                        *((bool*)(pheader + sizeof(byte) + sizeof(ushort) + sizeof(uint))) = succeeded;
                        withException = !succeeded;
                    }
                }
            }
            header.consume(sizeof(byte) + sizeof(ushort) + (withSequenceNo ? (sizeof(uint) + (withSucceeded ? sizeof(bool) : 0)): 0));

            if (argumentObject != null)
                argumentObject.SerializeTo(ref header, ref data);
            if (withException)
                exceptionWrapper.SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            bool withSequenceNo, withSucceeded = false, withException = false;
            ushort classID; // = (argumentObject != null) ? argumentObject.SerializableInfo.ClassID : ClassID.Nothing;
            fixed (byte* pbuffer = header.Array)
            {
                byte* pheader = pbuffer + header.Offset;
                type = (Type) (*pheader);
                classID = *((ushort*)(pheader + sizeof(byte)));
                if (withSequenceNo = (type != Type.OneWayRequest))
                {
                    sequenceNo = *((uint*)(pheader + sizeof(byte) + sizeof(ushort)));
                    if (withSucceeded = (type == Type.Response))
                    {
                        succeeded = *((bool*)(pheader + sizeof(byte) + sizeof(ushort) + sizeof(uint)));
                        withException = !succeeded;
                    }
                }
            }
            header.consume(sizeof(byte) + sizeof(ushort) + (withSequenceNo ? (sizeof(uint) + (withSucceeded ? sizeof(bool) : 0)) : 0));

            if (classID == (ushort) ClassID.Nothing)
                argumentObject = null;
            else
            {
                argumentObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
                argumentObject.DeserializeFrom(ref header, ref data);
            }

            if (withException)
            {
                exceptionWrapper = new QS._core_c_.Base2.StringWrapper();
                exceptionWrapper.DeserializeFrom(ref header, ref data);
            }
        }

        #endregion

        public override string ToString()
        {
            return "\nType : " + type.ToString() + "\nArgument : " + QS._core_c_.Helpers.ToString.Object(argumentObject) + "\nSequenceNo : " +
                sequenceNo.ToString() + "\nSucceeded : " + succeeded.ToString() + "\nException : " + QS._core_c_.Helpers.ToString.Object(exceptionWrapper) + "\n";
        }
    }
}
