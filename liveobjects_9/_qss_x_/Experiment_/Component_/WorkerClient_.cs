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
using System.Windows.Forms;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass("45882934EE634612BDC76CFA20064D61")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    [QS._qss_x_.Reflection_.Internal]
    public sealed class WorkerClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IWorkerClient_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public WorkerClient_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWorker_> _workerreference,
            [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
            int _count)
        {
            this._mycontext = _mycontext;
            this._workerreference = _workerreference;
            this._workerproxy = this._workerreference.Dereference(this._mycontext);
            this._workerendpoint = this._mycontext.DualInterface<
                QS._qss_x_.Experiment_.Interface_.IWorker_,
                    QS._qss_x_.Experiment_.Interface_.IWorkerClient_>(this);
            this._workerconnection = this._workerendpoint.Connect(this._workerproxy._Work);
            for (int _i = 1; _i <= _count; _i++)
                this._workerendpoint.Interface._Work(_i.ToString());
            this._workerendpoint.Interface._Done();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IWorker_> _workerreference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Experiment_.Object_.IWorker_ _workerproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IWorker_,
                QS._qss_x_.Experiment_.Interface_.IWorkerClient_> _workerendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _workerconnection;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

        #region IWorkerClient_ Members

        void QS._qss_x_.Experiment_.Interface_.IWorkerClient_._Done(string _s)
        {
            this._mycontext.Platform.Scheduler.Schedule(
                new QS.Fx.Base.Event(
                    new QS.Fx.Base.ContextCallback(
                        delegate (object _o) { this._mycontext.Platform.Logger.Log((string) _o); }), _s));
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    
    }
}
