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
    [QS.Fx.Serialization.ClassID(ClassID.Embeddings2_Types_Parameter)]
    public class Parameter : QS.Fx.Serialization.ISerializable, IEquatable<Parameter>
    {
        public Parameter()
        {
        }

        public Parameter(ParameterMode mode, ReferencedType type)
        {
            this.mode = mode;
            this.type = type;
        }

        [QS.Fx.Printing.Printable]
        private ParameterMode mode;
        [QS.Fx.Printing.Printable]
        private ReferencedType type;

        #region Accessors

        public ParameterMode Mode
        {
            get { return mode; }
        }

        private ReferencedType Type
        {
            get { return type; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(
                    (ushort)ClassID.Embeddings2_Types_Parameter, sizeof(byte), sizeof(byte), 1);
                info.AddAnother(((QS.Fx.Serialization.ISerializable)type).SerializableInfo);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                *(pbuffer + header.Offset) = (byte)mode;
            }
            header.consume(sizeof(byte));
            ((QS.Fx.Serialization.ISerializable)type).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* pbuffer = header.Array)
            {
                mode = (ParameterMode) (*(pbuffer + header.Offset));
            }
            header.consume(sizeof(byte));
            type = new ReferencedType();
            ((QS.Fx.Serialization.ISerializable)type).DeserializeFrom(ref header, ref data);
        }

        #endregion

        #region IEquatable<Parameter> Members

        bool IEquatable<Parameter>.Equals(Parameter other)
        {
            return mode.Equals(other.mode) && type.Equals(other.type);
        }

        #endregion
    }
}
