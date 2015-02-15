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
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Pong)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Pong_ : _qss_x_.Experiment_.Object_.IPong_, QS._qss_x_.Experiment_.Interface_.IPong_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Pong_(QS.Fx.Object.IContext _mycontext)
        {
            this._mycontext = _mycontext;
            this._endpoint = this._mycontext.DualInterface<QS._qss_x_.Experiment_.Interface_.IPing_, QS._qss_x_.Experiment_.Interface_.IPong_>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPing_,
                QS._qss_x_.Experiment_.Interface_.IPong_>
                    _endpoint;
        [QS.Fx.Base.Inspectable]
        private Queue<int> _history = new Queue<int>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IPong_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPing_, 
                QS._qss_x_.Experiment_.Interface_.IPong_> 
                    QS._qss_x_.Experiment_.Object_.IPong_.Endpoint
        {
            get { return this._endpoint; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IPong_ Members

        [QS.Fx.Base.Synchronization(
            QS.Fx.Base.SynchronizationOption.Asynchronous | 
            QS.Fx.Base.SynchronizationOption.Multithreaded |
            QS.Fx.Base.SynchronizationOption.Concurrent)]
        void QS._qss_x_.Experiment_.Interface_.IPong_._Ping(int _x)
        {
            lock (this._history)
            {
                if (_x > 0)
                {
                    _history.Enqueue(_x);
                    Thread.Sleep(10);
                }
                else
                {
                    StringBuilder _s = new StringBuilder();
                    foreach (int _y in this._history)
                    {
                        _s.Append(_y);
                        _s.Append(", ");
                    }
                    _s.Append("...");
                    MessageBox.Show(_s.ToString());
                }
            }
            // this._endpoint.Interface._Pong(_x + 1);
        }

        #endregion
    }
}
