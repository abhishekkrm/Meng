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
    public partial class ExperimentControllerSelector : UserControl, QS._qss_e_.Components_.IExperimentController, QS.Fx.Inspection.IScalarAttribute
    {
        public ExperimentControllerSelector()
        {
            InitializeComponent();
        }

        public void AddController(QS._qss_c_.Base3_.Constructor<QS._qss_e_.Components_.IExperimentController> experimentControllerConstructor, string description)
        {
            lock (this)
            {
                if (locked)
                    throw new Exception("Cannot add controller, already selected.");

                comboBox1.Items.Add(new Wrapper(experimentControllerConstructor, description));

                if (comboBox1.SelectedItem == null)
                    comboBox1.SelectedIndex = 0;
            }
        }

        private bool locked = false;
        private QS._qss_e_.Components_.IExperimentController selectedController;

        #region Class Wrapper

        private class Wrapper
        {
            public Wrapper(QS._qss_c_.Base3_.Constructor<QS._qss_e_.Components_.IExperimentController> experimentControllerConstructor, string description)
            {
                this.experimentControllerConstructor = experimentControllerConstructor;
                this.description = description;
            }

            private QS._qss_c_.Base3_.Constructor<QS._qss_e_.Components_.IExperimentController> experimentControllerConstructor;
            private string description;

            public QS._qss_e_.Components_.IExperimentController ExperimentController
            {
                get { return experimentControllerConstructor(); }
            }

            public override string ToString()
            {
                return description;
            }
        }

        #endregion

        #region IExperimentController Members

        void QS._qss_e_.Components_.IExperimentController.Run(Type experimentClass, QS._core_c_.Components.AttributeSet experimentArgs)
        {
            lock (this)
            {
                if (!locked)
                    Lock();

                selectedController.Run(experimentClass, experimentArgs);
            }
        }

        void QS._qss_e_.Components_.IExperimentController.Shutdown()
        {
            lock (this)
            {
                if (!locked)
                    throw new Exception("Not running.");

                selectedController.Shutdown();
            }
        }

        Type QS._qss_e_.Components_.IExperimentController.Class
        {
            get { return (selectedController != null) ? selectedController.Class : null; }
        }

        QS._core_c_.Components.AttributeSet QS._qss_e_.Components_.IExperimentController.Arguments
        {
            get { return (selectedController != null) ? selectedController.Arguments : null; }
        }

        QS._core_c_.Components.AttributeSet QS._qss_e_.Components_.IExperimentController.Results
        {
            get { return (selectedController != null) ? selectedController.Results : null; }
        }

        #endregion

        private void comboBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                Lock();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                Lock();
            }
        }

        private void Lock()
        {
            if (locked)
                throw new Exception("Already selected.");
            if (comboBox1.SelectedItem == null)
                throw new Exception("Nothing selected.");

            button1.Enabled = false;
            comboBox1.Enabled = false;
            comboBox1.BackColor = Color.White;
            comboBox1.ForeColor = Color.DarkGray;
            locked = true;
            selectedController = ((Wrapper)comboBox1.SelectedItem).ExperimentController;
        }

        #region IScalarAttribute Members

        object QS.Fx.Inspection.IScalarAttribute.Value
        {
            get { return selectedController; }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Experiment Controller"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
        }

        #endregion
    }
}
