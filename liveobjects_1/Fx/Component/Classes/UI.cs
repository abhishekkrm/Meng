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

namespace QS.Fx.Component.Classes
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.UI, "UI", "A base class for objects with a graphical user interface.")]
    public class UI : UserControl, QS.Fx.Object.Classes.ILogging_UI, QS.Fx.Inspection.IInspectable
    {
        #region Constructor

        public UI(QS.Fx.Object.IContext context)
        {
            this.context = context;
            this._ui = context.ExportedUI(this);
            this._logging = context.ImportedInterface<QS.Fx.Interface.Classes.ILog>();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext context;
        [QS.Fx.Base.Inspectable("ui")]
        private QS.Fx.Endpoint.Internal.IExportedUI _ui;
        [QS.Fx.Base.Inspectable("logging")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILog> _logging;

        private QS.Fx.Inspection.IAttributeCollection _attributecollection;

        #endregion

        #region ILogging_UI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.ILogging_UI.UI
        {
            get { return _ui; }
        }

        QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.ILog> QS.Fx.Object.Classes.ILogging_UI.Logging
        {
            get { return _logging; }
        }

        #endregion

        #region IInspectable Members

        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get
            {
                lock (this)
                {
                    if (this._attributecollection == null)
                        this._attributecollection = new QS.Fx.Inspection.AttributesOf(this);
                    return this._attributecollection;
                }
            }
        }

        #endregion

        #region Log

        protected void Log(string message)
        {
            if (_logging.IsConnected)
                _logging.Interface.Log(message);
        }

        #endregion
    }
}
