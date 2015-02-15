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

namespace QS._qss_x_.Channel_.Message_
{
    [QS.Fx.Printing.Printable("Color", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Color)]
    public sealed class Color : IColor, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Color(byte _R, byte _G, byte _B, byte _A)
        {
            this._R = _R;
            this._G = _G;
            this._B = _B;
            this._A = _A;
        }

        public Color()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("R")]
        private byte _R;
        [QS.Fx.Printing.Printable("G")]
        private byte _G;
        [QS.Fx.Printing.Printable("B")]
        private byte _B;
        [QS.Fx.Printing.Printable("A")]
        private byte _A;

        #endregion

        #region IColor Members

        byte IColor.R
        {
            get { return this._R; }
            set { this._R = value; }
        }

        byte IColor.G
        {
            get { return this._G; }
            set { this._G = value; }
        }

        byte IColor.B
        {
            get { return this._B; }
            set { this._B = value; }
        }

        byte IColor.A
        {
            get { return this._A; }
            set { this._A = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Channel_Message_Color, 4 * sizeof(byte), 4 * sizeof(byte), 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *pheader = this._R;
                pheader += sizeof(byte);
                *pheader = this._G;
                pheader += sizeof(byte);
                *pheader = this._B;
                pheader += sizeof(byte);
                *pheader = this._A;
            }
            header.consume(4 * sizeof(byte));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                this._R = *pheader;
                pheader += sizeof(byte);
                this._G = *pheader;
                pheader += sizeof(byte);
                this._B = *pheader;
                pheader += sizeof(byte);
                this._A = *pheader;
            }
            header.consume(4 * sizeof(byte));
        }

        #endregion
    }
}
