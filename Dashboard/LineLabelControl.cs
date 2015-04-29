using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dashboard
{
    /*
     * Each numbered label for a line is an object of this class
     */ 
    class LineLabelControl : System.Windows.Forms.Panel
    {
        String Id = null;
        Label NameLabel = null;
       
        // Create a new PictureButton control and hook up its properties. 
        public LineLabelControl(String Id, String Name)
        {
            InitializeComponent();

            // Display the OK close button. 
            this.Id = Id;
            this.Size = new Size(14, 12);
            NameLabel = new Label(); 
            NameLabel.Text = Name;
            NameLabel.BackColor = Color.Transparent;
            NameLabel.ForeColor = Color.White;
            NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            NameLabel.Parent = this;
 
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
        }

        private void InitializeComponent()
        {
           // Return
        }

   
        public string getId()
        {
            return this.Id;
        }

    }
    

}
