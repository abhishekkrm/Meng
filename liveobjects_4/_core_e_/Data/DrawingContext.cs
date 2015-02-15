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
    public class DrawingContext : IDrawingContext
    {
        public DrawingContext() : this(true)
        {
        }

        public DrawingContext(bool connectingLines)
        {
            this.connectingLines = connectingLines;
        }

        private bool connectingLines;
        private int currentColor = 0;

        private static readonly Color[] dataColors = new Color[] 
        {
            Color.DarkRed, 
            Color.DarkBlue, 
            Color.DarkGreen, 
            Color.DarkViolet, 
            Color.DarkSlateBlue, 
            Color.DarkTurquoise, 
            Color.DarkSalmon,
            Color.DarkOliveGreen, 
            Color.DarkCyan, 
            Color.DarkKhaki, 
            Color.DarkMagenta,
            Color.Pink,
            Color.Navy,
            Color.LightBlue,
            Color.LightGreen,
            Color.LightSalmon,
            Color.LightCyan,
            Color.LightPink,
            Color.LemonChiffon,
            Color.Crimson
        };
        private readonly Color[] connectionColors = new Color[] 
        {
            Color.Red, 
            Color.Blue, 
            Color.Green, 
            Color.Violet, 
            Color.SlateBlue, 
            Color.Turquoise, 
            Color.Salmon, 
            Color.Olive, 
            Color.Cyan,
            Color.Khaki, 
            Color.Magenta,
            Color.Pink,
            Color.Navy,
            Color.LightBlue,
            Color.LightGreen,
            Color.LightSalmon,
            Color.LightCyan,
            Color.LightPink,
            Color.LemonChiffon,
            Color.Crimson
        };

        #region IDrawingContext Members

        bool IDrawingContext.ConnectingLines
        {
            get { return connectingLines; }
        }

        Color IDrawingContext.DataColor
        {
            get { return dataColors[currentColor]; }
        }

        Color IDrawingContext.ConnectionsColor
        {
            get { return connectionColors[currentColor]; }
        }

        void IDrawingContext.ChangeColors()
        {
            currentColor++;
            if (currentColor >= dataColors.Length)
                throw new Exception("Not enough colors.");            
        }

        #endregion
    }
}
