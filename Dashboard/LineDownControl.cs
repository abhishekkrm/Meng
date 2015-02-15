using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Dashboard
{
    class LineDownControl : System.Windows.Forms.Panel
    {
        String Id = null;
        Label NameLabel = null;
       
        // Create a new PictureButton control and hook up its properties. 
        public LineDownControl(String Id, String Name)
        {
            InitializeComponent();

            // Display the OK close button. 
            this.Id = Id;
            this.Size = new Size(20, 20);
            NameLabel = new Label(); 
            NameLabel.Text = Name;
            NameLabel.ForeColor = Color.White;
            NameLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            NameLabel.Parent = this;
 
        }

       

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
        }

        private void InitializeComponent()
        {
           // this.Text = "Picture Button Demo";
        }

   
        public string getId()
        {
            return this.Id;
        }

    }
    

}
