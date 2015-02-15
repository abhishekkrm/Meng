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

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

#endregion

namespace QS.GUI.Components
{
	public partial class ArgumentsTextBox : UserControl
	{
		public ArgumentsTextBox()
		{
			InitializeComponent();
		}

		public QS._core_c_.Components.AttributeSet Arguments
		{
			get 
			{
				try
				{
					return new QS._core_c_.Components.AttributeSet(textBox1.Text);
				}
				catch (Exception)
				{
					textBox1.ForeColor = Color.Red;
					return null;
				}
			}

			set
			{
				try
				{
					StringBuilder sb = new StringBuilder();
					foreach (string name in value.Attributes.Keys)
					{
						sb.Append("-");
						sb.Append(name);
                        if (value[name] != null)
                        {
                            sb.Append(":");
                            string ss = value[name].ToString();
                            bool has_spaces = ss.Contains(" ");
                            if (has_spaces)
                                sb.Append("\"");
                            sb.Append(ss);
                            if (has_spaces)
                                sb.Append("\"");
                        }
						sb.Append(" ");
					}
					textBox1.Text = sb.ToString();
				}
				catch (Exception)
				{
				}
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			textBox1.ForeColor = Color.Black;
		}
	}
}
