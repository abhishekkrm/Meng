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
    public partial class ObjectVisualizer : UserControl, IObjectVizualizer
    {
        public ObjectVisualizer()
        {
            InitializeComponent();
        }

        private object visualizedObject;
        private QS._core_c_.Base.IOutputReader readerAttachedToConsole;
        private QS._qss_c_.Logging_1_.IEventSource eventSourceAttached;

        #region Clicking

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            switchDisplayMode(DisplayMode.Logs);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            switchDisplayMode(DisplayMode.Data);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            switchDisplayMode(DisplayMode.Text);
        }

        #endregion

        #region IObjectVizualizer Members

        void IObjectVizualizer.ForceRefresh()
        {
            eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Refresh));
            richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Refresh));
            newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Refresh));
            dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Refresh));
            hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Refresh));
        }

        private enum DisplayMode  
        {
            Logs, Data, Text, None, Events, Hierarchy
        }

        private void switchDisplayMode(DisplayMode mode)
        {
            switch (mode)
            {
                case DisplayMode.Hierarchy:
                    {
                        eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Hide));
                        richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Hide));
                        newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Hide));
                        dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Hide));
                        hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Show));
                    }
                    break;

                case DisplayMode.Events:
                {
                    eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Show));
                    richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Hide));
                    newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Hide));
                    dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Hide));
                    hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Hide));
                }
                break;

                case DisplayMode.Text:
                {
                    eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Hide));
                    richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Show));
                    newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Hide));
                    dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Hide));
                    hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Hide));
                }
                break;

                case DisplayMode.Data:
                {
                    eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Hide));
                    richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Hide));
                    newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Hide));
                    dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Show));
                    hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Hide));
                }
                break;

                case DisplayMode.Logs:
                {
                    eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Hide));
                    richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Hide));
                    newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Show));
                    dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Hide));
                    hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Hide));
                }
                break;

                case DisplayMode.None:
                {
                    eventLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(eventLogWindow1.Hide));
                    richTextBox2.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(richTextBox2.Hide));
                    newLogWindow1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(newLogWindow1.Hide));
                    dataSetVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(dataSetVisualizer1.Hide));
                    hierarchyVisualizer1.BeginInvoke(new QS._qss_c_.Base3_.NoArgumentCallback(hierarchyVisualizer1.Hide));
                }
                break;
            }
        }

        object IObjectVizualizer.VisualizedObject
        {
            get { return visualizedObject; }
            set
            {
                lock (this)
                {
                    if (visualizedObject != value)
                    {
                        visualizedObject = value;

                        if (readerAttachedToConsole != null)
                        {
                            readerAttachedToConsole.Console = null;
                            readerAttachedToConsole = null;
                        }

                        if (eventSourceAttached != null)
                        {
                            eventSourceAttached.Logger = null;
                            eventSourceAttached = null;
                        }

                        if (visualizedObject == null)
                        {
                            toolStripButton1.Enabled = false; // text disabled
                            toolStripButton2.Enabled = false; // data disabled
                            toolStripButton3.Enabled = false; // logs disabled

                            switchDisplayMode(DisplayMode.None);

                            richTextBox1.Text = string.Empty;

                            richTextBox2.Text = string.Empty;
                            newLogWindow1.Clear();
                            ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = null;

                            ((QS.Fx.Logging.IEventLogger)eventLogWindow1).ResetComponent();
                        }
                        else
                        {
                            richTextBox1.Text = visualizedObject.GetType().FullName;

                            if (visualizedObject is QS._qss_x_.Inspection_.Hierarchy_.IHierarchyView)
                            {
                                toolStripButton1.Enabled = false; // text disabled
                                toolStripButton2.Enabled = false; // data disabled
                                toolStripButton3.Enabled = false; // logs disabled

                                switchDisplayMode(DisplayMode.Hierarchy);

                                ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyVisualizer)hierarchyVisualizer1).HierarchyView =
                                    ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyView)visualizedObject);

                                richTextBox2.Text = string.Empty;
                                newLogWindow1.Clear();
                                ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = null;
                            }
                            else if (visualizedObject is QS._core_e_.Data.IDataSet)
                            {
                                toolStripButton1.Enabled = false; // text disabled
                                toolStripButton2.Enabled = true; // data enabled
                                toolStripButton3.Enabled = false; // logs disabled

                                switchDisplayMode(DisplayMode.Data);
                
                                ((QS.GUI.Components.IDataSetVizualizer) dataSetVisualizer1).SourceData = 
                                    ((QS._core_e_.Data.IDataSet) visualizedObject);

                                richTextBox2.Text = string.Empty;
                                newLogWindow1.Clear();
                                ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyVisualizer)hierarchyVisualizer1).HierarchyView = null;
                            }
                            else if (visualizedObject is QS._qss_c_.Logging_1_.IEventSource)
                            {
                                toolStripButton1.Enabled = false; // text disabled
                                toolStripButton2.Enabled = false; // data disabled
                                toolStripButton3.Enabled = false; // logs disabled

                                eventSourceAttached = (QS._qss_c_.Logging_1_.IEventSource)visualizedObject;

                                switchDisplayMode(DisplayMode.Events);

                                QS.Fx.Logging.IEventLogger ourLogger =
                                    (QS.Fx.Logging.IEventLogger)eventLogWindow1;
                                ourLogger.ResetComponent();

                                eventSourceAttached.Logger = null; // some nasty hack
                                eventSourceAttached.Logger = ourLogger;

                                newLogWindow1.Clear();
                                richTextBox2.Text = string.Empty;
                                ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = null;
                                ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyVisualizer)hierarchyVisualizer1).HierarchyView = null;
                            }
                            else if (visualizedObject is QS._core_c_.Base.IOutputReader ||
                                visualizedObject is QS._qss_e_.Management_.IManagedComponent)
                            {
                                toolStripButton1.Enabled = false; // text disabled
                                toolStripButton2.Enabled = false; // data disabled
                                toolStripButton3.Enabled = true; // logs enabled

                                if (visualizedObject is QS._core_c_.Base.IOutputReader)
                                    readerAttachedToConsole = (QS._core_c_.Base.IOutputReader)visualizedObject;
                                else if (visualizedObject is QS._qss_e_.Management_.IManagedComponent)
                                    readerAttachedToConsole = ((QS._qss_e_.Management_.IManagedComponent)visualizedObject).Log;

                                switchDisplayMode(DisplayMode.Logs);

                                QS.Fx.Logging.IConsole ourConsole = (QS.Fx.Logging.IConsole) newLogWindow1;
                                newLogWindow1.Clear();
                                readerAttachedToConsole.Console = ourConsole;

                                richTextBox2.Text = string.Empty;
                                ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = null;
                                ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyVisualizer)hierarchyVisualizer1).HierarchyView = null;
                            }
                            else
                            {
                                toolStripButton1.Enabled = true; // text enabled
                                toolStripButton2.Enabled = false; // data disabled
                                toolStripButton3.Enabled = false; // logs disabled

                                switchDisplayMode(DisplayMode.Text);

                                if (visualizedObject is QS._qss_c_.Base3_.IFormattable)
                                    richTextBox2.Text = ((QS._qss_c_.Base3_.IFormattable)visualizedObject).ToString(0);
                                else
                                    richTextBox2.Text = QS.Fx.Printing.Printable.ToString(visualizedObject);

                                newLogWindow1.Clear();
                                ((QS.GUI.Components.IDataSetVizualizer)dataSetVisualizer1).SourceData = null;
                                ((QS._qss_x_.Inspection_.Hierarchy_.IHierarchyVisualizer)hierarchyVisualizer1).HierarchyView = null;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
