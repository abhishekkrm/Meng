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

namespace QS._qss_x_.Channel_.Message_.Video
{
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Video_Frame)]
    public sealed class Frame : IFrame
    {
        #region Constructors

        public Frame(double time, int width, int height, byte[] data)
        {
            this.time = time;
            this.width = width;
            this.height = height;
            this.data = data;
        }

        public Frame()
        {
        }

        #endregion

        #region Fields

        private double time;
        private int width, height;
        private byte[] data;

        #endregion

        #region IFrame Members

        double IFrame.Time
        {
            get { return time; }
        }

        int IFrame.Width
        {
            get { return width; }
        }

        int IFrame.Height
        {
            get { return height; }
        }

        byte[] IFrame.Data
        {
            get { return data; }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info =
                    new QS.Fx.Serialization.SerializableInfo(
                        (ushort) QS.ClassID.Fx_Channel_Message_Video_Frame,
                        3 * sizeof(int) + sizeof(double), 3 * sizeof(int) + sizeof(double) + data.Length, 1);
                return info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                *((double*)pheader) = this.time;
                pheader += sizeof(double);
                *((int*)pheader) = this.width;
                pheader += sizeof(int);
                *((int*)pheader) = this.height;
                pheader += sizeof(int);
                *((int*)pheader) = this.data.Length;
            }
            header.consume(3 * sizeof(int) + sizeof(double));
            data.Add(new QS.Fx.Base.Block(this.data));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* _pheader = header.Array)
            {
                byte* pheader = _pheader + header.Offset;
                this.time = *((double*)pheader);
                pheader += sizeof(double);
                this.width = *((int*)pheader);
                pheader += sizeof(int);
                this.height = *((int*)pheader);
                pheader += sizeof(int);
                count = *((int*)pheader);
            }
            header.consume(3 * sizeof(int) + sizeof(double));
            this.data = new byte[count];
            Buffer.BlockCopy(data.Array, data.Offset, this.data, 0, count);
            data.consume(count);
        }

        #endregion
    }
}
