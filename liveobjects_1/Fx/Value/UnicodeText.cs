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

namespace QS.Fx.Value
{
    [QS.Fx.Reflection.ValueClass("A72E4FD10892421d8D2E49E0CE820903", "UnicodeText")]
    [QS.Fx.Printing.Printable("UnicodeText", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.UnicodeText)]
    public sealed class UnicodeText : QS.Fx.Value.Classes.IText
    {
        #region Constructors

        public UnicodeText(string s)
        {
            this._block = new QS.Fx.Base.Block(System.Text.Encoding.Unicode.GetBytes(s));            
        }

        public UnicodeText()
        {
        }

        #endregion

        #region Casting

        public static implicit operator UnicodeText(string s)
        {
            return new UnicodeText(s);
        }

        public static implicit operator string(UnicodeText s)
        {
            return ((QS.Fx.Value.Classes.IText) s).Text;
        }

        #endregion

        #region Fields

        private QS.Fx.Base.Block _block;

        #endregion

        #region IText Members

        [QS.Fx.Printing.Printable("Text")]
        string QS.Fx.Value.Classes.IText.Text
        {
            get { return System.Text.Encoding.Unicode.GetString(_block.buffer, (int) _block.offset, (int) _block.size); }
            set { this._block = new QS.Fx.Base.Block(System.Text.Encoding.Unicode.GetBytes(value)); }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.UnicodeText, sizeof(uint), sizeof(uint) + (int) _block.size, 1); }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte *pheader = header.Array)
            {
                *((uint*)(pheader + header.Offset)) = _block.size;
            }
            header.consume(sizeof(uint));
            data.Add(_block);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            uint _size; 
            fixed (byte* pheader = header.Array)
            {
                _size = *((uint*)(pheader + header.Offset));
            }
            header.consume(sizeof(uint));
            _block = new QS.Fx.Base.Block(data.Array, (uint) data.Offset, _size); 
            data.consume((int) _size);
        }

        #endregion

        //#region Extensions

        //public static QS.Fx.Value.Classes.IText .ctor(this string s)
        //{
        //    return new UnicodeText(s);
        //}

        //#endregion
    }
}
