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
using System.IO;

namespace QS.GUI.Components
{
    public partial class DataSetVisualizer : UserControl, IDataSetVizualizer
    {
        public DataSetVisualizer()
        {
            InitializeComponent();

            // ((QS.GUI.Components.ISelectionViewer)selectionViewer1).DataWindow = dataWindow1;

            ((QS.GUI.Components.IDataSetFilterSelector)dataSetFilterSelector1).FilterChanged += 
                new EventHandler(delegate(object sender, EventArgs e)
                {
                    ((QS.GUI.Components.IDataWindow) enhancedDataWindowWithControls1).Data =
                        ((QS.GUI.Components.IDataSetFilterSelector)dataSetFilterSelector1).FilteredData;                    
                });
        }

        public string[] FilterPropertyNames
        {
            get { return dataSetFilterSelector1.PropertyNames; }
        }

        #region Clicking

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            QS._core_e_.Data.IDataSet sourceData = ((QS.GUI.Components.IDataWindow)enhancedDataWindowWithControls1).Data;
            if (sourceData != null)
            {
                using (QS._qss_e_.Data_.DataSet.Printer printer = new QS._qss_e_.Data_.DataSet.Printer(sourceData))
                {
                    printDialog1.Document = printer.Document;                    
                    printDialog1.PrinterSettings.DefaultPageSettings.Landscape = true;
                    if (printDialog1.ShowDialog() == DialogResult.OK)
                    {
                        printer.print();
                    }
                }
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            QS._core_e_.Data.IData cropped =
                ((IEnhancedDataWindowWithControls)enhancedDataWindowWithControls1).Data.Crop(
                    ((IEnhancedDataWindowWithControls)enhancedDataWindowWithControls1).View);
            if (cropped != null)
            {
                QS._core_e_.Data.IDataSet cropped_ = QS._qss_e_.Data_.Convert.ToDataSeries(cropped);
                if (cropped_ != null)
                    QS._qss_e_.Data_.DataSet.CopyToClipboardAsImage(cropped_);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            QS._core_e_.Data.IDataSet sourceData = ((QS.GUI.Components.IDataWindow)enhancedDataWindowWithControls1).Data;
            if (sourceData != null)
            {
                string s = QS._qss_e_.Data_.DataSet.ToString(sourceData);
                if (s.Length > 0)
                {
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (TextWriter writer = new StreamWriter(saveFileDialog1.FileName))
                        {
                            writer.WriteLine(s);
                            writer.Flush();
                        }
                    }
                }
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            QS._core_e_.Data.IDataSet sourceData = ((QS.GUI.Components.IDataWindow)enhancedDataWindowWithControls1).Data;
            if (sourceData != null)
            {
                string s = QS._qss_e_.Data_.DataSet.ToString(sourceData);
                if (s.Length > 0)
                    Clipboard.SetDataObject(s);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            (new QS.GUI.Components.RepositorySubmit1(((IDataSetVizualizer)this).DisplayedData)).ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SaveToRepository saveToRepository = new SaveToRepository(((IDataSetVizualizer)this).DisplayedData);
            saveToRepository.ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            QS._core_e_.Data.IData cropped =
                ((IEnhancedDataWindowWithControls)enhancedDataWindowWithControls1).Data.Crop(
                    ((IEnhancedDataWindowWithControls)enhancedDataWindowWithControls1).View);

            string s = QS._qss_e_.Data_.DataSet.ToString( QS._qss_e_.Data_.Convert.ToDataSeries(cropped));
            if (s.Length > 0)
                Clipboard.SetDataObject(s);
        }

        #endregion

        #region IDataSetVizualizer Members

        QS._core_e_.Data.IDataSet IDataSetVizualizer.SourceData
        {
            get { return ((QS.GUI.Components.IDataSetFilterSelector)dataSetFilterSelector1).SourceData; }
            set { ((QS.GUI.Components.IDataSetFilterSelector)dataSetFilterSelector1).SourceData = value; }
        }

        QS._core_e_.Data.IDataSet IDataSetVizualizer.DisplayedData
        {
            get { return ((QS.GUI.Components.IDataWindow)enhancedDataWindowWithControls1).Data; }
        }

        QS._core_e_.Data.IView IDataSetVizualizer.View
        {
            get { return ((QS.GUI.Components.IEnhancedDataWindowWithControls) enhancedDataWindowWithControls1).View; }
        }

        #endregion
    }
}
