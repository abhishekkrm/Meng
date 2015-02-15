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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QS._qss_x_.Experiment_.Component_
{
    //[QS.Fx.Reflection.ComponentClass("8A4F33280D4A4F06AAF902398E362FDF")]
    //[QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    //[QS._qss_x_.Reflection_.Internal]
    //public sealed class DictionaryClient_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, QS._qss_x_.Experiment_.Interface_.IDictionaryClient_
    //{
    //    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    //    #region Constructor

    //    public DictionaryClient_(QS.Fx.Object.IContext _mycontext,
    //        [QS.Fx.Reflection.Parameter("concurrency", QS.Fx.Reflection.ParameterClass.Value)] 
    //        int _concurrency,
    //        [QS.Fx.Reflection.Parameter("count", QS.Fx.Reflection.ParameterClass.Value)] 
    //        int _count,
    //        [QS.Fx.Reflection.Parameter("batch", QS.Fx.Reflection.ParameterClass.Value)] 
    //        int _batch,
    //        [QS.Fx.Reflection.Parameter("range", QS.Fx.Reflection.ParameterClass.Value)] 
    //        int _range,
    //        [QS.Fx.Reflection.Parameter("work", QS.Fx.Reflection.ParameterClass.Value)] 
    //        QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference)
    //    {
    //        this._mycontext = _mycontext;
    //        this._concurrency = _concurrency;
    //        this._count = _count;
    //        this._batch = _batch;
    //        this._range = _range;
    //        this._workreference = _workreference;
    //        this._workproxy = this._workreference.Dereference(this._mycontext);
    //        this._workendpoint = this._mycontext.DualInterface<
    //            QS._qss_x_.Experiment_.Interface_.IDictionary_,
    //                QS._qss_x_.Experiment_.Interface_.IDictionaryClient_>(this);
    //        this._workconnection = this._workendpoint.Connect(this._workproxy._Work);
    //        this._threads = new Thread[this._concurrency];
    //        for (int _i = 0; _i < this._concurrency; _i++)
    //        {
    //            this._threads[_i] = new Thread(new ThreadStart(this._Work));
    //            this._threads[_i].Start();
    //        }
    //    }

    //    #endregion

    //    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

    //    #region Fields

    //    [QS.Fx.Base.Inspectable]
    //    private QS.Fx.Object.IContext _mycontext;
    //    [QS.Fx.Base.Inspectable]
    //    private int _concurrency;
    //    [QS.Fx.Base.Inspectable]
    //    private int _count;
    //    [QS.Fx.Base.Inspectable]
    //    private int _batch;
    //    [QS.Fx.Base.Inspectable]
    //    private int _range;
    //    [QS.Fx.Base.Inspectable]
    //    private QS.Fx.Object.IReference<QS._qss_x_.Experiment_.Object_.IDictionary_> _workreference;
    //    [QS.Fx.Base.Inspectable]
    //    private QS._qss_x_.Experiment_.Object_.IDictionary_ _workproxy;
    //    [QS.Fx.Base.Inspectable]
    //    private QS.Fx.Endpoint.Internal.IDualInterface<
    //        QS._qss_x_.Experiment_.Interface_.IDictionary_,
    //            QS._qss_x_.Experiment_.Interface_.IDictionaryClient_> _workendpoint;
    //    [QS.Fx.Base.Inspectable]
    //    private QS.Fx.Endpoint.IConnection _workconnection;
    //    [QS.Fx.Base.Inspectable]
    //    private Thread[] _threads;
    //    [QS.Fx.Base.Inspectable]
    //    private int _waiting1;
    //    [QS.Fx.Base.Inspectable]
    //    private int _waiting2;
    //    [QS.Fx.Base.Inspectable]
    //    private ManualResetEvent _ready1 = new ManualResetEvent(false);
    //    [QS.Fx.Base.Inspectable]
    //    private ManualResetEvent _ready2 = new ManualResetEvent(false);
    //    [QS.Fx.Base.Inspectable]
    //    private int _todo1;
    //    [QS.Fx.Base.Inspectable]
    //    private int _todo2;
    //    [QS.Fx.Base.Inspectable]
    //    private int _done1;
    //    [QS.Fx.Base.Inspectable]
    //    private int _done2;
    //    [QS.Fx.Base.Inspectable]
    //    private bool _started;
    //    [QS.Fx.Base.Inspectable]
    //    private bool _finished;
    //    [QS.Fx.Base.Inspectable]
    //    private double _timestamp;

    //    #endregion

    //    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

    //    #region _Work

    //    private void _Work()
    //    {
    //        while (!this._finished)
    //        {
    //            if (Interlocked.Increment(ref this._waiting1) == this._concurrency)
    //            {
    //                if (!this._started)
    //                {
    //                    MessageBox.Show("Are you ready to start the experiment?", "Ready?",
    //                        MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
    //                    this._started = true;
    //                    this._timestamp = this._mycontext.Platform.Clock.Time;
    //                    this._todo1 = this._count;
    //                }
    //                this._done2 = 0;
    //                this._todo2 = Math.Min(this._todo1, this._batch);
    //                this._waiting2 = 0;
    //                this._ready2.Reset();
    //                this._ready1.Set();
    //            }
    //            else
    //                this._ready1.WaitOne();
    //            int _seqno;
    //            while ((_seqno = Interlocked.Increment(ref this._done2)) <= this._todo2)
    //            {
    //                string _key = _seqno.ToString();
    //                string _value = string.Empty;
    //                this._workendpoint.Interface._Add(_key, _value);
    //            }
    //            if (Interlocked.Increment(ref this._waiting2) == this._concurrency)
    //            {
    //                this._workendpoint.Interface._Clear();
    //                this._done1 += this._todo2;
    //                this._todo1 -= this._todo2;
    //                if (this._todo1 == 0)
    //                {
    //                    this._finished = true;
    //                    this._mycontext.Platform.Logger.Log("Duration : " + (this._mycontext.Platform.Clock.Time - this._timestamp).ToString());
    //                }
    //                this._waiting1 = 0;
    //                this._ready1.Reset();
    //                this._ready2.Set();
    //            }
    //            else
    //                this._ready2.WaitOne();
    //        }
    //    }

    //    #endregion

    //    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

    //    #region IDictionaryClient_ Members

    //    //void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Done(string _o)
    //    //{
    //    //    MessageBox.Show(_o);
    //    //}

    //    #endregion

    //    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@    

    //    #region IDictionaryClient_ Members

    //    void QS._qss_x_.Experiment_.Interface_.IDictionaryClient_._Got(string _key, IList<string> _value)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    #endregion
    //}
}
