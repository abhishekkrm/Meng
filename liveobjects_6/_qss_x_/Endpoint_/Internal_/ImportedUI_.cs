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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace QS._qss_x_.Endpoint_.Internal_
{
    internal sealed class ImportedUI_ : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IImportedUI_
    {
        #region Constructor

        public ImportedUI_(System.Windows.Forms.Control _containerui) : base(null)
        {
            if (_containerui == null)
                throw new Exception("Endpoint not configured.");
            this._containerui = _containerui;
        }

        #endregion

        #region Fields

        private System.Windows.Forms.Control _containerui, _ui;

        #endregion

        #region _Start and _Stop

        protected override void _Start(Endpoint_ _other)
        {
            IExportedUI_ other_ = _other as IExportedUI_;
            if (other_ == null)
                throw new Exception("Endpoint type mismatch.");

            this._ui = other_.ExportedUI_;
            if (this._ui == null)
                throw new Exception("Endpoint not configured.");

            this._containerui.Controls.Add(this._ui);
        }

        protected override void _Stop(Endpoint_ _other)
        {
            if (this._ui != null)
            {
                try
                {
                    _containerui.Controls.Remove(this._ui);
                }
                catch (Exception)
                {
                }
                this._ui = null;
            }
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return QS._qss_x_.Endpoint_.Internal_.InterfaceClass_.ImportedUI_; }
        }

        #endregion

        #region QS.Fx.Endpoint.Internal.IImportedUI Members

        System.Windows.Forms.Control QS.Fx.Endpoint.Internal.IImportedUI.UI
        {
            get 
            {
                System.Windows.Forms.Control _ui = this._ui;
                if (_ui == null)
                    throw new Exception("The endpoint is not connected.");
                return _ui;
            }
        }

        #endregion
    }
}
