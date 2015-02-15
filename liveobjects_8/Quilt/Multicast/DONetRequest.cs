/*

Copyright (c) 2004-2009 Qi Huang. All rights reserved.

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

using QS.Fx.Value.Classes;
using QS.Fx.Base;
using QS.Fx.Value;

namespace Quilt.Multicast
{
    [QS.Fx.Reflection.ValueClass("2C90FBF81A134922B8F0956CA2E4424A", "DONetRequest")]
    [QS.Fx.Serialization.ClassID(QS.ClassID.DonetRequest)]
    public sealed class DONetRequest:
        QS.Fx.Serialization.ISerializable
    {
        #region Field

        public double _seq_no;

        #endregion

        #region Constructor

        public DONetRequest(double _seq_no)
        {
            this._seq_no = _seq_no;
        }

        public DONetRequest()
        {
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.DonetRequest), sizeof(double));
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref ConsumableBlock header, ref IList<Block> data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    *((double*)(_pheader)) = _seq_no;
                }

                header.consume(sizeof(double));
            }
            catch (Exception exc)
            {
                throw new Exception("DONetRequest SerializeTo exception " + exc.Message);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref ConsumableBlock header, ref ConsumableBlock data)
        {
            try
            {
                fixed (byte* _pheader_0 = header.Array)
                {
                    byte* _pheader = _pheader_0 + header.Offset;
                    _seq_no = *((double*)(_pheader));
                }

                header.consume(sizeof(double));
            }
            catch (Exception exc)
            {
                throw new Exception("DONetRequest DeserializeFrom exception" + exc.Message);
            }
        }

        #endregion
    }
}
