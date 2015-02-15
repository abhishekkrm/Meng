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

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("17483F1313B44C748076C03D90D089A2")]
    public sealed class Whatever_ : Interfaces_.IWhateverObject_, Interfaces_.IWhatever_
    {
        #region Constructor

        public Whatever_(QS.Fx.Object.IContext _context,
            [QS.Fx.Reflection.Parameter("whatever", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _otherref)
        {
            this._context = _context;
            this._container = new System.Windows.Forms.SplitContainer();
            this._container.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this._textbox = new System.Windows.Forms.RichTextBox();
            this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this._container.Panel1.Controls.Add(this._textbox);
            this._uiendpoint = this._context.ExportedUI(this._container);
            this._whateverendpoint = this._context.DualInterface<Interfaces_.IWhatever_, Interfaces_.IWhatever_>(this);
            this._otherref = _otherref;
            if (this._otherref.ObjectClass.IsSubtypeOf(QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IUI>()))
            {
                this._uiref = this._otherref.CastTo<QS.Fx.Object.Classes.IUI>();
                this._ui = this._uiref.Dereference(this._context);
                this._uiendpoint2 = this._context.ImportedUI(this._container.Panel2);
                this._connection = this._uiendpoint2.Connect(this._ui.UI);
                this._uiendpoint2.UI.Dock = System.Windows.Forms.DockStyle.Fill;
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _context;
        private System.Windows.Forms.SplitContainer _container;
        private System.Windows.Forms.RichTextBox _textbox;
        private QS.Fx.Endpoint.Internal.IExportedUI _uiendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<Interfaces_.IWhatever_, Interfaces_.IWhatever_> _whateverendpoint;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _otherref;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _uiref;
        private QS.Fx.Object.Classes.IUI _ui;
        private QS.Fx.Endpoint.Internal.IImportedUI _uiendpoint2;
        private QS.Fx.Endpoint.IConnection _connection;

        #endregion

        #region IWhateverObject_ Members

        QS.Fx.Endpoint.Classes.IExportedUI liveobjects_8.Interfaces_.IWhateverObject_.UI
        {
            get { return this._uiendpoint; }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<liveobjects_8.Interfaces_.IWhatever_, liveobjects_8.Interfaces_.IWhatever_> liveobjects_8.Interfaces_.IWhateverObject_._Whatever
        {
            get { return this._whateverendpoint; }
        }

        #endregion

        #region IWhatever_ Members

        void liveobjects_8.Interfaces_.IWhatever_._Whatever()
        {            
        }

        #endregion
    }
}
