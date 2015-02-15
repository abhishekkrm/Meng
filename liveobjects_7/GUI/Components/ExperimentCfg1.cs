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
    public partial class ExperimentCfg1 : UserControl, IExperimentConfigurator
    {
        public ExperimentCfg1()
        {
            InitializeComponent();
        }

        private EventHandler changed;

        #region IExperimentConfigurator Members

        Type IExperimentConfigurator.Class
        {
            get { return experimentSelector1.ExperimentClass; }
        }

        QS._core_c_.Components.AttributeSet IExperimentConfigurator.Arguments
        {
            get 
            {
                try
                {
                    return new QS._core_c_.Components.AttributeSet(richTextBox1.Text);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        event EventHandler IExperimentConfigurator.Changed
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        #endregion

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Type experimentClass = experimentSelector1.ExperimentClass;
            try
            {
                QS._core_c_.Components.AttributeSet attributes = new QS._core_c_.Components.AttributeSet(
                    ((experimentClass.GetCustomAttributes(typeof(QS._qss_e_.Base_1_.ArgumentsAttribute),
                    false)[0]) as QS._qss_e_.Base_1_.ArgumentsAttribute).Arguments);

                richTextBox1.Text = attributes.AsString;
            }
            catch (Exception)
            {
                richTextBox1.Text = "";
            }

            if (changed != null)
                changed(this, null);
        }
    }
}
