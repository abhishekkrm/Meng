/*

Copyright (c) 2009 Chuck Sakoda. All rights reserved.

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
using System.Linq;
using System.Text;

namespace QS._qss_x_.Azure_
{
    [QS.Fx.Reflection.ComponentClass("72CC9F23A2A741eb9F87A0AC6D105BEF", "AzureClientTest_")]
    public class AzureClientTest_<
        [QS.Fx.Reflection.Parameter("DataClass", QS.Fx.Reflection.ParameterClass.ValueClass)] DataClass>
         : QS.Fx.Inspection.Inspectable,
            QS.Fx.Object.Classes.IObject,
            QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>
        
        where DataClass : class, QS.Fx.Serialization.ISerializable, QS.Fx.Value.Classes.IText
    {
        

        public AzureClientTest_(
            QS.Fx.Object.IContext _mycontext,
                        [QS.Fx.Reflection.Parameter("azure channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
            QS._qss_x_.Azure_.Object_.IAzureWorkerChannel_<DataClass>>
                    _azure_ref)
        {
            this._mycontext = _mycontext;
            QS._qss_x_.Azure_.Object_.IAzureWorkerChannel_<DataClass> _obj_ = _azure_ref.Dereference(_mycontext);
            this._azure_endpoint =
                _mycontext.DualInterface<
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_,
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>>(this);

            this._timer = new System.Timers.Timer();
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
            this._timer.Interval = 2500;
            this._timer.AutoReset = false;
            

            this._azure_endpoint.Connect(_obj_._Channel);
            this._timer.Start();
            //this._azure_endpoint.Interface.Ready();
        }

        void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _azure_endpoint.Interface.Ready();
        }

        QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Azure_.Interface_.IAzureWorkerChannel_,
                QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>> _azure_endpoint;

        private QS.Fx.Object.IContext _mycontext;
        private System.Timers.Timer _timer;
        #region IAzureWorkerChannelClient_<DataClass> Members

        void QS._qss_x_.Azure_.Interface_.IAzureWorkerChannelClient_<DataClass>.Data(DataClass data)
        {
            _mycontext.Platform.Logger.Log("Got some data: "+data.Text);
            _timer.Start();
            //_azure_endpoint.Interface.Ready();
        }

        #endregion
    }
}
