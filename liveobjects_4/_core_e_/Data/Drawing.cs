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

namespace QS._core_e_.Data
{
    public static class Drawing
    {
/*
        public void static DrawLine(System.Drawing.Graphics graphics, System.Drawing.Pen pen, int x1, int y1, int x2, int y2)
        graphics.DrawLine(pen1, last_xcoord, min_ycoord, last_xcoord, max_ycoord);
*/

        public static void DrawLine(Graphics graphics, Pen pen, float x1, float y1, float x2, float y2)
        {
            RectangleF bounds = graphics.VisibleClipBounds;
            graphics.DrawLine(pen, Math.Max(Math.Min(x1, bounds.Right), bounds.Left), Math.Max(Math.Min(y1, bounds.Bottom), bounds.Top),
                Math.Max(Math.Min(x2, bounds.Right), bounds.Left), Math.Max(Math.Min(y2, bounds.Bottom), bounds.Top));
        }

        public static void DrawEllipse(Graphics graphics, Pen pen, float x, float y, float w, float h)
        {
            RectangleF bounds = graphics.VisibleClipBounds;
            graphics.DrawEllipse(pen, Math.Max(Math.Min(x, bounds.Right), bounds.Left), Math.Max(Math.Min(y, bounds.Bottom), bounds.Top), w, h);                                
        }
    }
}
