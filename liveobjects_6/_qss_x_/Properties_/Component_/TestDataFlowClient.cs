/*

Copyright (c) 2010 Colin Barth. All rights reserved.

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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.TestDataFlowClient, "Test Data Flow Client")]
    public sealed class TestDataFlowClient : QS._qss_x_.Properties_.Component_.Base_, QS.Fx.Interface.Classes.IDataFlowClient
    {
        public TestDataFlowClient(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("daclient_reference", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlow> daclient_reference,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)]
            bool _debug)
            : base(_mycontext, _debug)
        {
            this.daclient_reference = daclient_reference;
            this._daclient_endpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDataFlow,
                QS.Fx.Interface.Classes.IDataFlowClient>(this);
            this._daclient_endpoint.OnConnected +=
               new QS.Fx.Base.Callback(
                   delegate
                   {
                       this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.channelConnect)));
                   });
            this._daclient_endpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate
                    {
                        this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this.channelDisconnect)));
                    });
            QS.Fx.Object.Classes.IDataFlow daclient_proxy = this.daclient_reference.Dereference(this._mycontext);
            this._daclient_endpoint.Connect(daclient_proxy.DataFlow);

            logfilename = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, "lockstep_results.txt");
            id = new Random().Next(100).ToString();
        }

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDataFlow> daclient_reference;
        private QS.Fx.Endpoint.Internal.IDualInterface<
           QS.Fx.Interface.Classes.IDataFlow,
           QS.Fx.Interface.Classes.IDataFlowClient> _daclient_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Clock.IAlarm _alarm1;
        private QS.Fx.Clock.IAlarm _alarm2;
        private String logfilename;
        private String id;
        private static Object _lock = new Object();

        #endregion

        #region IDataFlowClient Members

        void QS.Fx.Interface.Classes.IDataFlowClient.Send(int id, long version, QS.Fx.Serialization.ISerializable value)
        {
#if VERBOSE
            if (this._logger != null)
            {
                this._logger.Log("-------------------------------------------------------------------------");
                this._logger.Log("TestDataFlowClient received message with (id, version, value) => (" +
                    id + "," + version + "," + ((QS.Fx.Base.Index)value).String + ")");
            }
#endif
            lock (_lock)
            {
                TextWriter tw = new StreamWriter(logfilename, true);
                tw.WriteLine(_mycontext.Platform.Clock.Time.ToString() + "--" + id + ": Begin step " + ((QS.Fx.Base.Index)value).String);
                tw.Close();
            }

            // push a task you have completed (i.e. I have completed task 2)
            // if someone else is still on task 1 => you will get a 2 back
            // once you get a 3 => you can proceed to step 3.

            QS.Fx.Base.Index completedId = (QS.Fx.Base.Index)value;
            double random = new Random().NextDouble() * 4.0 + 3.0;

            this._alarm1 = this._platform.AlarmClock.Schedule
            (
                random,
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
#if VERBOSE
                        if (this._logger != null)
                        {
                            this._logger.Log("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                            this._logger.Log("TestDataFlowClient sending message with (id, version, value) => (" +
                                id + "," + version + "," + completedId.String + ")");
                        }
#endif
                        lock (_lock)
                        {
                            TextWriter tw = new StreamWriter(logfilename, true);
                            tw.WriteLine(_mycontext.Platform.Clock.Time.ToString() + "--" + id + ": End step " + completedId.String);
                            tw.Close();
                        }
                        
                        this._daclient_endpoint.Interface.Send(0, 0, completedId);
                    }
                ),
                null
            );
            

        }

        #endregion

        #region channelConnect

        private void channelConnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("TestDataFlowClient connecting to daclient channel");
#endif
            this._alarm2 = this._platform.AlarmClock.Schedule
            (
                (3.0),
                new QS.Fx.Clock.AlarmCallback
                (
                    delegate(QS.Fx.Clock.IAlarm _alarm)
                    {
                        this._daclient_endpoint.Interface.Send(0, 0, new QS.Fx.Base.Index(1));
                    }
                ),
                null
            );

        }

        #endregion

        #region channelDisconnect

        private void channelDisconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("TestDataFlowClient disconnecting form daclient channel.");
#endif
        }

        #endregion
    }
}
