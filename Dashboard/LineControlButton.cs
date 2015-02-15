using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Dashboard
{
    /*
     * Each colored, numbered button to control the lines of the circuit is an object of this class
     */
    public partial class LineControlButton : Button
    {
        String id;
        int state;
        public LineControlButton(String name, String id)
        {
            this.Text = name;
            this.Height = 50;
            this.Width = 50;
            this.state = Constants.BUTTON_STATE_NORMAL;
            this.BackColor = Constants.NORMAL_LINE_COLOR;
        }
        public String ID
        {
            get { return this.id; }
            set { id = value; }
        }
        public int State
        {
            get { return this.state; }
            set { state = value; }
        }
    }
}
