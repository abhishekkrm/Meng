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

namespace QS._qss_c_.Membership_3_.Responses
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(ClassID.Membership3_Responses_Exception)]
    public class Error : Response
    {
        public Error()
        {
        }

        public Error(int sequenceNo, System.Exception exception)
            : base(sequenceNo)
        {
            this.exception = exception;
        }

        [QS.Fx.Printing.Printable]
        private System.Exception exception;

        public System.Exception Exception
        {
            get { return exception; }
        }

        public override ResponseType ResponseType
        {
            get { return ResponseType.Exception; }
        }

        #region ISerializable Members

        public override QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo info = 
                    new QS.Fx.Serialization.SerializableInfo((ushort)ClassID.Membership3_Responses_Exception, sizeof(ushort), sizeof(ushort), 1);
                info.AddAnother(base.SerializableInfo);
                return info;
            }
        }

        public unsafe override void SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            base.SerializeTo(ref header, ref data);
            byte[] nameBytes = System.Text.Encoding.ASCII.GetBytes(exception.Message);
            fixed (byte* pbuffer = header.Array)
            {
                *((ushort*)(pbuffer + header.Offset)) = (ushort)nameBytes.Length;
            }
            data.Add(new QS.Fx.Base.Block(nameBytes));
        }

        public unsafe override void DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            base.DeserializeFrom(ref header, ref data);
            int nbytes;
            fixed (byte* pbuffer = header.Array)
            {
                nbytes = (int)(*((ushort*)(pbuffer + header.Offset)));
            }
            exception = new Exception(System.Text.Encoding.ASCII.GetString(data.Array, data.Offset, nbytes));
            data.consume(nbytes);           
        }

        #endregion
    }
}
