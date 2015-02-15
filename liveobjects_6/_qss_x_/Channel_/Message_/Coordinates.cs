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
    [QS.Fx.Printing.Printable("Coordinates", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Coordinates)]
    public sealed class Coordinates : ICoordinates, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Coordinates(
            float _tm, float _px, float _py, float _pz, float _dx, float _dy, float _dz, float _rx, float _ry, float _rz, float _ax, float _ay, float _az)
        {
            this._tm = _tm;
            this._px = _px;
            this._py = _py;
            this._pz = _pz;
            this._dx = _dx;
            this._dy = _dy;
            this._dz = _dz;
            this._rx = _rx;
            this._ry = _ry;
            this._rz = _rz;
            this._ax = _ax;
            this._ay = _ay;
            this._az = _az;
        }

        public Coordinates()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable("TM")]
        private float _tm;
        [QS.Fx.Printing.Printable("PX")]
        private float _px;
        [QS.Fx.Printing.Printable("PY")]
        private float _py;
        [QS.Fx.Printing.Printable("PZ")]
        private float _pz;
        [QS.Fx.Printing.Printable("DX")]
        private float _dx;
        [QS.Fx.Printing.Printable("DY")]
        private float _dy;
        [QS.Fx.Printing.Printable("DZ")]
        private float _dz;
        [QS.Fx.Printing.Printable("RX")]
        private float _rx;
        [QS.Fx.Printing.Printable("RY")]
        private float _ry;
        [QS.Fx.Printing.Printable("RZ")]
        private float _rz;
        [QS.Fx.Printing.Printable("AX")]
        private float _ax;
        [QS.Fx.Printing.Printable("AY")]
        private float _ay;
        [QS.Fx.Printing.Printable("AZ")]
        private float _az;

        #endregion

        #region ICoordinates Members

        float ICoordinates.TM
        {
            get { return this._tm; }
            set { this._tm = value; }
        }

        float ICoordinates.PX
        {
            get { return this._px; }
            set { this._px = value; }
        }

        float ICoordinates.PY
        {
            get { return this._py; }
            set { this._py = value; }
        }

        float ICoordinates.PZ
        {
            get { return this._pz; }
            set { this._pz = value; }
        }

        float ICoordinates.DX
        {
            get { return this._dx; }
            set { this._dx = value; }
        }

        float ICoordinates.DY
        {
            get { return this._dy; }
            set { this._dy = value; }
        }

        float ICoordinates.DZ
        {
            get { return this._dz; }
            set { this._dz = value; }
        }

        float ICoordinates.RX
        {
            get { return this._rx; }
            set { this._rx = value; }
        }

        float ICoordinates.RY
        {
            get { return this._ry; }
            set { this._ry = value; }
        }

        float ICoordinates.RZ
        {
            get { return this._rz; }
            set { this._rz = value; }
        }

        float ICoordinates.AX
        {
            get { return this._ax; }
            set { this._ax = value; }
        }

        float ICoordinates.AY
        {
            get { return this._ay; }
            set { this._ay = value; }
        }

        float ICoordinates.AZ
        {
            get { return this._az; }
            set { this._az = value; }
        }

        #endregion

        #region ISerializable Members

        QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get 
            {
                return new QS.Fx.Serialization.SerializableInfo(
                    (ushort)QS.ClassID.Fx_Channel_Message_Coordinates, 13 * sizeof(float), 13 * sizeof(float), 0);
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                *((float*)pheader) = this._tm;
                pheader += sizeof(float);
                *((float*)pheader) = this._px;
                pheader += sizeof(float);
                *((float*)pheader) = this._py;
                pheader += sizeof(float);
                *((float*)pheader) = this._pz;
                pheader += sizeof(float);
                *((float*)pheader) = this._dx;
                pheader += sizeof(float);
                *((float*)pheader) = this._dy;
                pheader += sizeof(float);
                *((float*)pheader) = this._dz;
                pheader += sizeof(float);
                *((float*)pheader) = this._rx;
                pheader += sizeof(float);
                *((float*)pheader) = this._ry;
                pheader += sizeof(float);
                *((float*)pheader) = this._rz;
                pheader += sizeof(float);
                *((float*)pheader) = this._ax;
                pheader += sizeof(float);
                *((float*)pheader) = this._ay;
                pheader += sizeof(float);
                *((float*)pheader) = this._az;
            }
            header.consume(13 * sizeof(float));
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* parray = header.Array)
            {
                byte* pheader = parray + header.Offset;
                this._tm = *((float*)pheader);
                pheader += sizeof(float);
                this._px = *((float*)pheader);
                pheader += sizeof(float);
                this._py = *((float*)pheader);
                pheader += sizeof(float);
                this._pz = *((float*)pheader);
                pheader += sizeof(float);
                this._dx = *((float*)pheader);
                pheader += sizeof(float);
                this._dy = *((float*)pheader);
                pheader += sizeof(float);
                this._dz = *((float*)pheader);
                pheader += sizeof(float);
                this._rx = *((float*)pheader);
                pheader += sizeof(float);
                this._ry = *((float*)pheader);
                pheader += sizeof(float);
                this._rz = *((float*)pheader);
                pheader += sizeof(float);
                this._ax = *((float*)pheader);
                pheader += sizeof(float);
                this._ay = *((float*)pheader);
                pheader += sizeof(float);
                this._az = *((float*)pheader);
            }
            header.consume(13 * sizeof(float));
        }

        #endregion
    }
}
