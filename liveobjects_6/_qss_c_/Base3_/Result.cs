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
    public struct Result : QS.Fx.Serialization.ISerializable
    {
        public Result(QS.Fx.Serialization.ISerializable resultObject) : this(true, resultObject, null)
        {
        }

        public Result(System.Exception exception) : this(false, null, exception)
        {
        }

        public Result(bool operationSucceeded, QS.Fx.Serialization.ISerializable resultObject, System.Exception exception)
        {
            this.operationSucceeded = operationSucceeded;
            this.resultObject = operationSucceeded ? resultObject : null;
            this.exceptionWrapper = operationSucceeded ? null : 
                new QS._core_c_.Base2.StringWrapper(exception != null ? exception.ToString() : "");
        }

        private bool operationSucceeded;
        private QS.Fx.Serialization.ISerializable resultObject;
        private QS._core_c_.Base2.StringWrapper exceptionWrapper;

        #region Accessors

        public bool OperationSucceeded
        {
            get
            {
                return operationSucceeded;
            }
        }

        public QS.Fx.Serialization.ISerializable Object
        {
            get
            {
                return resultObject;
            }
        }

        public System.Exception Exception
        {
            get
            {
                return operationSucceeded ? null : (new Exception("Operation failed:\n" + exceptionWrapper.String));
            }
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                if (operationSucceeded)
                {
                    if (resultObject != null)
                    {
                        QS.Fx.Serialization.SerializableInfo info = resultObject.SerializableInfo;
                        return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_Result, (ushort)(info.HeaderSize + 3), info.Size + 3, info.NumberOfBuffers);
                    }
                    else
                        return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_Result, 3, 3, 0);
                }
                else
                {
                    QS.Fx.Serialization.SerializableInfo info = exceptionWrapper.SerializableInfo;
                    return new QS.Fx.Serialization.SerializableInfo((ushort)QS.ClassID.Base3_Result, (ushort)(info.HeaderSize + 1), info.Size + 1, info.NumberOfBuffers);
                }
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref System.Collections.Generic.IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte *headerptr = arrayptr + header.Offset;
                *((bool*) headerptr) = operationSucceeded;
                if (operationSucceeded)
                {
                    *((ushort *) (headerptr + 1)) = (resultObject != null) ? resultObject.SerializableInfo.ClassID : (ushort) QS.ClassID.NullObject;
                    header.consume(3);
                    if (resultObject != null)
                        resultObject.SerializeTo(ref header, ref data);
                }
                else
                {
                    header.consume(1);
                    exceptionWrapper.SerializeTo(ref header, ref data);
                }
            }
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* arrayptr = header.Array)
            {
                byte *headerptr = arrayptr + header.Offset;
                operationSucceeded = *((bool*) headerptr);
                if (operationSucceeded)
                {
                    ushort classID = *((ushort *) (headerptr + 1));
                    header.consume(3);
                    if (classID == (ushort) QS.ClassID.NullObject)
                        resultObject = null;
                    else
                    {
                        resultObject = QS._core_c_.Base3.Serializer.CreateObject(classID);
                        resultObject.DeserializeFrom(ref header, ref data);
                    }
                }
                else
                {
                    header.consume(1);
                    exceptionWrapper = new QS._core_c_.Base2.StringWrapper();
                    exceptionWrapper.DeserializeFrom(ref header, ref data);
                }
            }
        }

        #endregion
    }
}
