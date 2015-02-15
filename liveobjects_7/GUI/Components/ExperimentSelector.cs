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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#endregion

namespace QS.GUI.Components
{
	public partial class ExperimentSelector : Component
	{
        static ExperimentSelector()
        {
            System.Type experimentClass = null;
            foreach (Type type in System.Reflection.Assembly.GetAssembly(typeof(QS._qss_e_.Experiments_.IExperiment)).GetTypes())
            {
                if (type.GetCustomAttributes(typeof(QS._qss_e_.Experiments_.DefaultExperimentAttribute), false).Length > 0)
                {
                    experimentClass = type;
                    break;
                }
            }
            defaultClass = experimentClass;
        }

		private static readonly System.Type defaultClass;

		public ExperimentSelector()
		{
			InitializeComponent();
		}

		public ExperimentSelector(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
        }

		private System.Windows.Forms.ComboBox comboBox;

        public void ResetSelection()
        {
            try
            {
                comboBox.SelectedItem = defaultClass;
            }
            catch (Exception)
            {
                if (comboBox.Items.Count > 0)
                    comboBox.SelectedIndex = 0;
            }
        }

		public System.Windows.Forms.ComboBox ComboBox
		{
			get { return comboBox; }
			set 
			{ 
				comboBox = value;
				PopulateComboBox();
			}
		}

		private void PopulateComboBox()
		{
			if (comboBox != null)
			{
				System.Type experimentType = typeof(QS._qss_e_.Experiments_.IExperiment);
				System.Reflection.Assembly quicksilverAssembly = System.Reflection.Assembly.GetAssembly(experimentType);
				foreach (System.Type type in quicksilverAssembly.GetTypes())
				{
					if (experimentType.IsAssignableFrom(type) && type.IsClass)
						comboBox.Items.Add(type);
				}

                ResetSelection();
			}
		}

		public System.Type ExperimentClass
		{
			get { return (comboBox != null) ? (System.Type) comboBox.SelectedItem : null; }
/*
			set
			{
                if (value != null)
                {
                    try
                    {
                        comboBox.SelectedItem = value;
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                    ResetSelection();
			}
*/ 
		}

//		public event EventHandler SelectionChanged
//		{
//			add { comboBox.SelectedIndexChanged += value; }
//			remove { comboBox.SelectedIndexChanged -= value; }
//		}
	}
}
