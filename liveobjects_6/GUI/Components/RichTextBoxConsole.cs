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
using System.Text;

namespace QS.GUI.Components
{
    public sealed class RichTextBoxConsole : QS.Fx.Logging.IConsole
    {
        public RichTextBoxConsole(System.Windows.Forms.RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
        }

        private System.Windows.Forms.RichTextBox richTextBox;

        #region IConsole Members

        void QS.Fx.Logging.IConsole.Log(string s)
        {
            lock (richTextBox)
            {
                try
                {
                    richTextBox.AppendText(s);
                    richTextBox.AppendText("\n");
                }
                catch (Exception exc)
                {
                    richTextBox.BeginInvoke(new QS.Fx.Base.ContextCallback<string>(
                        delegate (string _s)
                        {
                            try
                            {
                                richTextBox.AppendText(_s);
                                richTextBox.AppendText("\n");
                            }
                            catch (Exception _exc)
                            {
                                System.Windows.Forms.MessageBox.Show("Can't log a message.\n" + _exc.ToString(), "Error", 
                                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                        }), s);
                }
            }
        }

        #endregion
    }
}
