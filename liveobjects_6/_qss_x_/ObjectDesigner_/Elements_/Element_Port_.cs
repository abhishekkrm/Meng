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
using System.Drawing;
using System.Drawing.Drawing2D;

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public abstract class Element_Port_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        protected Element_Port_(Category_ _category, bool _automatic) : base(_automatic)
        {
            this._category = _category;
        }

        #endregion

        #region Fields

        private Category_ _category;

        #endregion

        #region Constants

        private const float _Port_Width = 40;
        private const float _Port_Depth = 10;

        #endregion

        #region Category_

        public enum Category_
        {
            Endpoint_, Parameter_, From_, Reference_
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Accessors

        public Category_ _Category
        {
            get { return _category; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Recalculate_1

        public override void _Recalculate_1()
        {
            switch (_category)
            {
                case Category_.Endpoint_:
                case Category_.From_:
                    {
                        this._s.X = _Port_Depth;
                        this._s.Y = _Port_Width;
                    }
                    break;

                case Category_.Reference_:
                case Category_.Parameter_:
                    {
                        this._s.X = _Port_Width;
                        this._s.Y = _Port_Depth;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _Recalculate_2

        public override void _Recalculate_2()
        {
            this._q.X = this._p.X + this._s.X;
            this._q.Y = this._p.Y + this._s.Y;
        }

        #endregion

        #region _Draw

        public override void _Draw(Graphics _g, PointF _p0, PointF _q0, float _z)
        {
            this._CalculateCoordinates(_g, _p0, _z);
            if (this._selected || this._highlighted)
            {

                _g.DrawRectangle(
                    new Pen(this._selected ? (this._highlighted ? Brushes.GreenYellow : Brushes.Yellow) : Brushes.Cyan, 5 * _z), 
                    _pp.X - 3 * _z, _pp.Y - 3 * _z, _ps.X + 6 * _z, _ps.Y + 6 * _z);
            }
            _g.FillRectangle(this._correct ? Brushes.Black : Brushes.OrangeRed, _pp.X, _pp.Y, _ps.X, _ps.Y);
        }

        #endregion

        #region _Click

        public override Element_ _Click(PointF _m)
        {
            if (this._Overlapping(_m, _m))
                return this;
            else
                return null;
        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
