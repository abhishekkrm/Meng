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
using System.Threading;

namespace QS._qss_x_.Properties_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.PropertiesUI,
        "PropertiesUI", "A frontend interface to the properties object.")]
    public sealed partial class PropertiesUI_ : UserControl, QS.Fx.Object.Classes.IUI, IProperties_
    {
        #region Constructor

        public PropertiesUI_(
             [QS.Fx.Reflection.Parameter("properties", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<IPropertiesObject_> _properties_object_reference)
        {
            InitializeComponent();

            this._ui_endpoint = _mycontext.ExportedUI(this);

            if (_properties_object_reference != null)
            {
                this._properties_endpoint = _mycontext.DualInterface<IProperties_, IProperties_>(this);
                this._properties_endpoint.OnConnected += new QS.Fx.Base.Callback(this._ConnectedCallback);
                this._properties_connection = this._properties_endpoint.Connect(_properties_object_reference.Object.Properties);
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedUI _ui_endpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<IProperties_, IProperties_> _properties_endpoint;
        private QS.Fx.Endpoint.IConnection _properties_connection;
        private bool _clear;
        private Queue<string> _log = new Queue<string>();

        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this._ui_endpoint; }
        }

        #endregion

        #region IProperties_ Members

        void IProperties_.Value(uint _id, IVersion_ _version, IValue_ _value)
        {
            StringBuilder _s = new StringBuilder();
            _s.Append("value(");
            _s.Append(_id.ToString());
            _s.Append(", ");
            _s.Append((_version != null) ? _version.ToString() : "?");
            _s.Append(", ");
            _s.Append((_value != null) ? _value.ToString() : "?");
            _s.AppendLine(")");
            string _ss = _s.ToString();
            lock (this)
            {
                this._log.Enqueue(_ss);
            }
            this._RefreshCallback();
        }

        #endregion

        #region _RefreshCallback

        private void _RefreshCallback()
        {
            try
            {
                if (this.richTextBox1.InvokeRequired)
                    this.richTextBox1.BeginInvoke(new QS.Fx.Base.Callback(this._RefreshCallback));
                else
                {
                    lock (this)
                    {
                        if (this._clear)
                        {
                            this.richTextBox1.Clear();
                            this._clear = false;
                        }
                        while (_log.Count > 0)
                            this.richTextBox1.AppendText(_log.Dequeue());
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region _ConnectedCallback

        private void _ConnectedCallback()
        {
            lock (this)
            {
                this._clear = true;
                this._log.Clear();
            }
        }

        #endregion
    }
*/
}
