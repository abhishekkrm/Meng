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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.SecureUI_, "SecureUI", "An example graphical object with secure content.")]
    public partial class SecureUI_
        // <[QS.Fx.Reflection.Parameter("AuthenticatingClass", QS.Fx.Reflection.ParameterClass.ObjectClass)] AuthenticatingClass> 
        // QS.Fx.Object.Classes.ISecureUI<AuthenticatingClass>        
        // where AuthenticatingClass : class, QS.Fx.Object.Classes.IObject
        : UserControl, 
        QS.Fx.Object.Classes.ISecureUI
    {
        #region Constructor

        public SecureUI_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Content", QS.Fx.Reflection.ParameterClass.Value)] QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _contentref)
        {
            InitializeComponent();
            this._ui = _mycontext.ExportedUI(this);
            this._container = _mycontext.ImportedUI(this);
            this._content = _contentref.Dereference(_mycontext);
            this._connection = this._container.Connect(this._content.UI);
            this._container.UI.Dock = DockStyle.Fill;
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedUI _ui;
        private QS.Fx.Endpoint.Internal.IImportedUI _container;
        private QS.Fx.Object.Classes.IUI _content;
        private QS.Fx.Endpoint.IConnection _connection;

        #endregion

        #region IClassifiedUI Members

        // QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.ISecureUI<AuthenticatingClass>.UI
        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.ISecureUI.UI
        {
            get { return this._ui; }
        }

        #endregion
    }
}
