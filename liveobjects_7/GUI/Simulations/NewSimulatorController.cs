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

namespace QS.GUI.Simulations
{
    public partial class NewSimulatorController : UserControl, QS._qss_e_.Components_.IExperimentController, QS.Fx.Inspection.IScalarAttribute
    {
        public const int DefaultNumberOfNodes = QS._qss_c_.Simulations_2_.Defaults.NumberOfNodes;
        public const double DefaultLossRate = QS._qss_c_.Simulations_2_.Defaults.LossRate;
        public const double DefaultLatency = QS._qss_c_.Simulations_2_.Defaults.Latency; 
        public const int DefaultQueueSize = QS._qss_c_.Simulations_2_.Defaults.QueueSize;

        public NewSimulatorController()
        {
            InitializeComponent();
            
            simulator2.MonitoringCallback += new EventHandler(simulator2_MonitoringCallback);

            EnableButtons(false);

            toolStripTextBox1.Text = DefaultNumberOfNodes.ToString();
            toolStripTextBox2.Text = DefaultLossRate.ToString();
            toolStripTextBox3.Text = DefaultLatency.ToString();
            toolStripTextBox4.Text = DefaultQueueSize.ToString();
        }

        #region Monitoring Callback

        void simulator2_MonitoringCallback(object sender, EventArgs e)
        {
            Simulator.MonitoringCallbackArgs args = e as Simulator.MonitoringCallbackArgs;
            try
            {
                toolStrip1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(delegate
                {
                    try
                    {
                        toolStripStatusLabel2.Text = "Time: " + args.SimulationTime.ToString("0000.000000");
                        toolStripStatusLabel3.Text = "Speed " + args.SimulationSpeedup.ToString(".0") + "x";
                        toolStripStatusLabel4.Text = "Processing " + args.CurrentProcessingSpeed.ToString() + " events/s";
                        toolStripStatusLabel5.Text = "Done: " + args.NumberOfEventsProcessed.ToString();
                        toolStripStatusLabel6.Text = "Queue Size: " + args.NumberOfEventsInQueue.ToString();
                    }
                    catch (Exception)
                    {
                    }
                }));
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Internal Processing

        public void Continue()
        {
            if (((ISimulator)simulator2).Status == SimulatorStatus.UNINITIALIZED)
                throw new Exception("Not initialized!");

            ((ISimulator)simulator2).Continue();
            toolStripStatusLabel1.Text = ((ISimulator)simulator2).Status.ToString();
        }

        #endregion

        #region ToolStrip Buttons

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Continue();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).Pause();
            toolStripStatusLabel1.Text = ((ISimulator)simulator2).Status.ToString();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).StepForward();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).StepForward(10);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).StepForward(100);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).StepForward(1000);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).StepForward(10000);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            ((ISimulator)simulator2).Shutdown();
            toolStripStatusLabel1.Text = ((ISimulator)simulator2).Status.ToString();
        }

        #endregion

        #region IExperimentController Members

        private void EnableButtons(bool enable)
        {
            toolStripButton1.Enabled = enable;
            toolStripButton2.Enabled = enable;
            toolStripButton3.Enabled = enable;
            toolStripButton4.Enabled = enable;
            toolStripButton5.Enabled = enable;
            toolStripButton6.Enabled = enable;
            toolStripButton7.Enabled = enable;

            toolStripButton8.Enabled = false;
        }

        void QS._qss_e_.Components_.IExperimentController.Run(
            Type experimentClass, QS._core_c_.Components.AttributeSet experimentArgs)
        {
            if (((ISimulator)simulator2).Status != SimulatorStatus.UNINITIALIZED)
                throw new Exception("Already initialized!");

            ((ISimulator)simulator2).ExperimentClass = experimentClass;
            ((ISimulator)simulator2).Arguments = experimentArgs;

            ((ISimulator)simulator2).NumberOfNodes = Convert.ToInt32(toolStripTextBox1.Text);
            ((ISimulator)simulator2).PacketLossRate = Convert.ToDouble(toolStripTextBox2.Text);
            ((ISimulator)simulator2).NetworkLatency = Convert.ToDouble(toolStripTextBox3.Text);
            ((ISimulator)simulator2).IncomingQueueSize = Convert.ToInt32(toolStripTextBox4.Text);

            ((ISimulator)simulator2).Start();

            EnableButtons(true);
        }

        void QS._qss_e_.Components_.IExperimentController.Shutdown()
        {
        }

        Type QS._qss_e_.Components_.IExperimentController.Class
        {
            get { return ((ISimulator)simulator2).ExperimentClass; }
        }

        QS._core_c_.Components.AttributeSet QS._qss_e_.Components_.IExperimentController.Arguments
        {
            get { return ((ISimulator)simulator2).Arguments; }
        }

        QS._core_c_.Components.AttributeSet QS._qss_e_.Components_.IExperimentController.Results
        {
            get { return ((ISimulator)simulator2).Results; }
        }

        #endregion

        #region IScalarAttribute Members

        object QS.Fx.Inspection.IScalarAttribute.Value
        {
            get { return simulator2; }
        }

        #endregion

        #region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
        {
            get { return "Simulator Controller"; }
        }

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
        {
            get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
        }

        #endregion
    }
}
