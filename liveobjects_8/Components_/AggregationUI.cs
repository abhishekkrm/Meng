/*

Copyright 2004-2009, Jared Cantwell. All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions 
   and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright notice, this list of 
   conditions and the following disclaimer in the documentation and/or other materials provided
  with the distribution.

THIS SOFTWARE IS PROVIDED "AS IS" BY THE ABOVE COPYRIGHT HOLDER(S) AND ALL OTHER CONTRIBUTORS 
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE ABOVE 
COPYRIGHT HOLDER(S) OR ANY OTHER CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND 
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED 
OF THE POSSIBILITY OF SUCH DAMAGE. 
 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace liveobjects_8.Components_
{
[QS.Fx.Reflection.ComponentClass(
        "D5821C252A3642edB46E09DD9BFE4CE2", "Aggregation UI", "Demos simple aggregation component")]
    public partial class AggregationUI : UserControl, QS.Fx.Object.Classes.IUI,
        QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>
    {
        public AggregationUI(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Aggregator", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>> _agg_reference
        )
        {
            this.myendpoint = _mycontext.ExportedUI(this);

            _endpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>,
                QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>>(this);
            _endpoint.Connect(_agg_reference.Dereference(_mycontext).Aggregator);

            InitializeComponent();
        }

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IAggregator<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>,
            QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>> _endpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedUI myendpoint;

        #region IAggregatorClient<Round_, Index> Members

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>.Phase(QS._qss_x_.Properties_.Value_.Round_ round)
        {
            _endpoint.Interface.Disseminate(round, new QS.Fx.Base.Index(0));
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>.Disseminating(QS._qss_x_.Properties_.Value_.Round_ round, QS.Fx.Base.Index message)
        {
                log.Text += Environment.NewLine + "Disseminating new message.";
        }

        void QS.Fx.Interface.Classes.IAggregatorClient<QS._qss_x_.Properties_.Value_.Round_, QS.Fx.Base.Index>.Aggregate(QS._qss_x_.Properties_.Value_.Round_ _round, IList<QS.Fx.Base.Index> _messages)
        {
            String input = "";
            int sum = 0;

            try
            {
                sum = Convert.ToInt32(numberTextBox.Text);
            }
            catch (Exception e)
            {
                sum = 0;
            }

            if (_messages != null)
            {
                foreach (QS.Fx.Base.Index _num in _messages)
                {
                    if (input.Length > 0) input += " + ";
                    input += _num.String;
                    sum += Convert.ToInt32(_num.String);
                }
            }

            _endpoint.Interface.Aggregate(_round, new QS.Fx.Base.Index(sum));

            //this.BeginInvoke(delegate()
            //{
            log.Text += Environment.NewLine + "Aggregating new message. input-->" + input + " ---out--->" + sum;
            //});
        }

        #endregion

        private void disseminate_Click_1(object sender, EventArgs e)
        {
            _endpoint.Interface.Disseminate(new QS._qss_x_.Properties_.Value_.Round_(new QS.Fx.Base.Incarnation(0), new QS.Fx.Base.Index(0)),
                                            new QS.Fx.Base.Index(0));
        }

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this.myendpoint; }
        }

        #endregion

        private void log_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
