using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Dashboard
{
    /* Each line in the circuit is an object of this class
     */ 
    class LineControl : System.Windows.Forms.Panel
    {
        private String id;
        private String name;
        private float x;
        private float y;
        private bool isHighlighed = false;
        private Color LineColor = Constants.NORMAL_LINE_COLOR;

        public LineControl(String id, float p1, float p2, float p3, float p4, String name)
        {
            this.id = id;
            this.x = Math.Abs(p1 - p3);
            this.y = Math.Abs(p2 - p4);
            this.BackColor = Color.Black;
            this.Paint += LineControl_Paint;
            this.Size = x > y ? new Size((int)x, 2) : new Size(2, (int)y); //new Size(804, 498); 
            this.name = name;
            
        }

        void LineControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Pen pen = new Pen(LineColor, 4.0f);
            
            if (isHighlighed == true)
            {
                pen.Color = SystemColors.Highlight;
                pen.Width = 10.0f;
            }
            e.Graphics.DrawLine(pen, 0, 0, x, y); 

        }

        public void UpdateLine(Color ColorIndicator)
        {
            this.LineColor = ColorIndicator;
            this.Invalidate();

        }
        
        public float getX()
        {
            return this.x;
        }

        public float getY()
        {
            return this.y;
        }

        public string getId()
        {
            return this.id;
        }

        public string getName()
        {
            return this.name;
        }

        public void setName(String lineName)
        {
            this.name = lineName;
        }

        public void highlightControl(bool highlight)
        {
            this.isHighlighed = highlight;
            if(highlight)
            {
                this.Size = x > y ? new Size((int)x, 5) : new Size(5, (int)y);
            }
            else
            {
                this.Size = x > y ? new Size((int)x, 2) : new Size(2, (int)y);
            }
            this.Invalidate();
        }
    }
}
