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

namespace QS._qss_x_.Base1_
{
    public partial class ExceptionForm : Form
    {
        public ExceptionForm(System.Exception _exception) : this(null, null, _exception)
        {
        }

        public ExceptionForm(string _caption, string _message, System.Exception _exception)
        {
            InitializeComponent();

            if (_caption != null)
                this.Text = _caption;
            else
                this.Text = "Exception";

            StringBuilder _sb = new StringBuilder();
            if (_message != null)
            {
                _sb.AppendLine(_message);
                _sb.AppendLine();
            }
            if (_exception != null)
            {
                bool _isfirst = true;
                do
                {
                    _sb.AppendLine();
                    if (_isfirst)
                        _isfirst = false;
                    else
                    {
                        _sb.AppendLine(new string('-', 80));
                        _sb.AppendLine();
                    }
                    _sb.AppendLine(_exception.ToString());
                    _exception = _exception.InnerException;
                }
                while (_exception != null);
                _sb.AppendLine();
            }
            this.richTextBox1.Text = _sb.ToString();
        }
    }
}
