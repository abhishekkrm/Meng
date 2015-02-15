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

namespace QS._qss_c_.Collections_6_
{
    public class BinaryTree<C> : IBinaryTree<C>, Base1_.IDrawable where C : class, IBTNode<C>
    {
        public BinaryTree()
        {
        }

        protected C root;

        #region IBinaryTree<C> Members

        C IBinaryTree<C>.Root
        {
            get { return root; }
        }

        #endregion

        #region Static Helpers

        public static int HeightOf(C root)
        {
            Queue<C> inQueue = new Queue<C>(), outQueue = new Queue<C>();
            inQueue.Enqueue(root);
            int height = 0;
            while (inQueue.Count > 0)
            {
                height++;
                foreach (C node in inQueue)
                {
                    if (node.Left != null)
                        outQueue.Enqueue(node.Left);
                    if (node.Right != null)
                        outQueue.Enqueue(node.Right);
                }
                Queue<C> temp = inQueue;
                temp.Clear();
                inQueue = outQueue;
                outQueue = temp;
            }
            return height;
        }

        public static void DrawOn(System.Drawing.Graphics g, C root)
        {
            int height = HeightOf(root);

            g.Clear(Color.White);

            if (root != null)
            {
                RectangleF bounds = g.VisibleClipBounds;
                Font font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular, GraphicsUnit.Pixel);

                double x0 = (double)bounds.Left;
                double y0 = (double)bounds.Top;
                double dx = (double)bounds.Width;
                double dy = ((double)bounds.Height) / height;

                drawSubtree(g, font, root, x0 + dx / 2, y0 + dy / 2, dx / 2, dy);
            }
        }

        private static void drawSubtree(System.Drawing.Graphics g, System.Drawing.Font font, 
            QS._qss_c_.Collections_6_.IBTNode<C> node, double x0, double y0, double dx, double dy)
        {
            if (node.Left != null)
            {
                double xL = x0 - dx / 2;
                double yL = y0 + dy;
                g.DrawLine(Pens.Gray, (float)x0, (float)y0, (float)xL, (float)yL);
                drawSubtree(g, font, node.Left, xL, yL, dx / 2, dy);
            }

            if (node.Right != null)
            {
                double xR = x0 + dx / 2;
                double yR = y0 + dy;
                g.DrawLine(Pens.Gray, (float)x0, (float)y0, (float)xR, (float)yR);
                drawSubtree(g, font, node.Right, xR, yR, dx / 2, dy);
            }

            string s = node.ToString();
            SizeF stringSize = g.MeasureString(s, font);

            double sdx = (double) stringSize.Width;
            double sdy = (double) stringSize.Height;
            double sx = x0 - sdx / 2;
            double sy = y0 - sdy / 2;

            g.FillRectangle(Brushes.LightYellow, (float)sx - 1, (float)sy - 1, (float)sdx + 2, (float)sdy + 2);
            g.DrawString(s, font, Brushes.Black, (float)sx, (float)sy);
        }

        #endregion

        #region IDrawable Members

        void QS._qss_c_.Base1_.IDrawable.DrawOn(Graphics g)
        {
            DrawOn(g, root);
        }

        #endregion
    }
}
