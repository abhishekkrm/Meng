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

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.InfrastructureUI,
        "InfrastructureUI", "A frontend interface to the infrastructure object.")]
    public sealed partial class InfrastructureUI_ : UserControl, QS.Fx.Object.Classes.IUI, IInfrastructureClient_
    {
        #region Constructor

        public InfrastructureUI_(
             [QS.Fx.Reflection.Parameter("infrastructure", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<IInfrastructureObject_> _infrastructure_object_reference)
        {
            InitializeComponent();

            this._ui_endpoint = _mycontext.ExportedUI(this);
            
            if (_infrastructure_object_reference != null)
            {
                this._infrastructure_endpoint = _mycontext.DualInterface<IInfrastructure_, IInfrastructureClient_>(this);
                this._infrastructure_connection = this._infrastructure_endpoint.Connect(_infrastructure_object_reference.Object.Infrastructure);
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedUI _ui_endpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<IInfrastructure_, IInfrastructureClient_> _infrastructure_endpoint;
        private QS.Fx.Endpoint.IConnection _infrastructure_connection;

        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this._ui_endpoint; }
        }

        #endregion
    }
*/
}
