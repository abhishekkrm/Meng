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

namespace QS._qss_e_.Launchers_
{
    public partial class ExperimentConsole : Form
    {
        public ExperimentConsole(string[] args) : this(ExperimentSpecification.Load(args[0]), args[1])
        {
        }

        public ExperimentConsole(ExperimentSpecification specification, string experiment_path)
        {
            InitializeComponent();

            try
            {
                this.specification = specification;
                textBox1.Text = specification.Class;
                foreach (ExperimentSpecification.Argument argument in specification.Arguments)
                    listView1.Items.Add(new ListViewItem(new string[] { argument.Name, argument.Value }));

                Components_.RemoteController1 controller =
                    new Components_.RemoteController1(
                        experiment_path,
                        QS._qss_c_.Base1_.Subnet.Collection(specification.Control_Subnets),
                        specification.CryptographicKey,
                        specification.Controller,
                        specification.Workers,
                        specification.Concurrency,
                        specification.Files,
                        specification.Path,
                        specification.Executable,
                        specification.Debug,
                        specification.User,
                        specification.Password,
                        specification.Domain,
                        specification.Restart,
                        specification.Upload
                    );

                controller.OnStarted += new EventHandler(this.ExperimentStartedCallback);
                controller.OnCompleted += new EventHandler(this.ExperimentCompletedCallback);
                controller.OnDestroyed += new EventHandler(this.ExperimentDestroyedCallback);

                controller.EnableNotification(specification.MailAccount, specification.MailPassword, specification.MailDomain, specification.MailServer,
                    specification.MailFrom, specification.MailTo, specification.MailToName);

                experimentController = controller;

                ((QS.GUI.Components.IInspector)inspector1).Add(experimentController);
                inspector1.ObjectSelector.ExpandAll();

                QS._core_c_.Components.AttributeSet attributes = new QS._core_c_.Components.AttributeSet();
                foreach (ExperimentSpecification.Argument argument in specification.Arguments)
                    attributes[argument.Name] = argument.Value;

                experimentController.Run(Type.GetType(specification.Class), attributes);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void ExperimentStartedCallback(object sender, EventArgs e)
        {
            
        }
        
        private void ExperimentCompletedCallback(object sender, EventArgs e)
        {
            
        }

        private void ExperimentDestroyedCallback(object sender, EventArgs e)
        {
            
        }

        private ExperimentSpecification specification;
        private QS._qss_e_.Components_.IExperimentController experimentController;

        private void ExperimentConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (experimentController != null)
                experimentController.Shutdown();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            experimentController.Shutdown();
            experimentController = null;
        }
    }
}
