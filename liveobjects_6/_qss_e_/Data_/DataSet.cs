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
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

#endregion

namespace QS._qss_e_.Data_
{
    public static class DataSet        
    {
        public static string ToString(QS._core_e_.Data.IDataSet dataSource)
        {
            if (dataSource != null)
            {
                System.Text.StringBuilder s = new System.Text.StringBuilder();
                if (dataSource is QS._core_e_.Data.DataSeries)
                {
                    foreach (double x in ((QS._core_e_.Data.DataSeries)dataSource).Data)
                        s.AppendLine(x.ToString());
                }
                else if (dataSource is QS._core_e_.Data.XYSeries)
                {
                    foreach (QS._core_e_.Data.XY x in ((QS._core_e_.Data.XYSeries)dataSource).Data)
                        s.AppendLine(x.x.ToString() + "\t" + x.y.ToString());
                }

                if (s.Length > 0)
                    return s.ToString();
            }

            return string.Empty;
        }

        public static void CopyToClipboardAsImage(QS._core_e_.Data.IDataSet dataSet)
        {
            Bitmap b = new Bitmap(1024, 768);
            Graphics g = Graphics.FromImage(b);
            dataSet.draw(g);
            DataObject d = new DataObject();
            d.SetData(DataFormats.Bitmap, true, b);
            Clipboard.SetDataObject(d, true);
        }

        #region Class Printer 

        public class Printer : System.IDisposable
        {
            public Printer(QS._core_e_.Data.IDataSet dataSet)
            {
                this.dataSet = dataSet;
                printDocument = new System.Drawing.Printing.PrintDocument();
                // printDocument.PrinterSettings = printerSettings;
                printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDocument_PrintPage);
                printDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(printDocument_EndPrint);
                // printingComplete = new System.Threading.AutoResetEvent(true);
            }

            private QS._core_e_.Data.IDataSet dataSet;
            private System.Drawing.Printing.PrintDocument printDocument;
            // private System.Threading.AutoResetEvent printingComplete;

            public System.Drawing.Printing.PrintDocument Document
            {
                get
                {
                    return printDocument;
                }
            }

            public void print()
            {
                // printingComplete.Reset();
                printDocument.Print();
                // printingComplete.WaitOne();
            }

            private void printDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
            {
                // printingComplete.Set();
            }

            private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
            {
                dataSet.downsample(e.Graphics.VisibleClipBounds.Size.ToSize()).draw(e.Graphics);
                e.HasMorePages = false;
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion
        }

        #endregion
    }
}
