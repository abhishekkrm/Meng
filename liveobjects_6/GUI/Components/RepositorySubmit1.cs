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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QS.GUI.Components
{
    public partial class RepositorySubmit1 : Form
    {
        public static string DefaultRepositoryServerName = "";
        public static string[] DefaultPath = new string[] { };

        public RepositorySubmit1(object dataObject) : this(DefaultRepositoryServerName, DefaultPath, dataObject)
        {
        }

        public RepositorySubmit1(string server, string[] path, object dataObject)
        {
            InitializeComponent();

            this.dataObject = dataObject;
            textBox1.Text = server;
            textBox2.Text = QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(path, ".");
        }

        private object dataObject;

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            throw new Exception("This component is obsolete.");

/*
            try
            {
                QS.TMS.Repository.IRepositoryClient client = new QS.TMS.Repository.Client(textBox1.Text);
                string link = client.Add(textBox2.Text.Split('.'), dataObject);
                textBox1.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Text = "http://" + textBox1.Text.Substring(0, textBox1.Text.LastIndexOf(":")) + 
                    "/Draw1.aspx?target=" + link + "&width=1024&height=768&filters=";
                toolStripButton1.Enabled = false;
                // toolStripButton2.Enabled = true; <-- cannot do this because we're not STA
                toolStripStatusLabel1.Text = "Submitted successfully";

                DefaultRepositoryServerName = textBox1.Text;
            }
            catch (Exception exc)
            {
                toolStripStatusLabel1.Text = "Could not submit";
                MessageBox.Show(exc.ToString(), "Cannot Submit", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
*/ 
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(statusStrip1.Text);
        }
    }
}
