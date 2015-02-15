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

namespace QS._qss_x_.Experiment_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Ping)]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Ping_ : _qss_x_.Experiment_.Object_.IPing_, QS._qss_x_.Experiment_.Interface_.IPing_ 
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Ping_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("pong", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<_qss_x_.Experiment_.Object_.IPong_> _pongref)
        {
            this._mycontext = _mycontext;
            this._pongref = _pongref;
            this._pongproxy = this._pongref.Dereference(this._mycontext);
            this._endpoint = this._mycontext.DualInterface<QS._qss_x_.Experiment_.Interface_.IPong_, QS._qss_x_.Experiment_.Interface_.IPing_>(this);
            this._connection = this._endpoint.Connect(this._pongproxy.Endpoint);
            for (int _x = 1; _x <= 100; _x++)
                this._endpoint.Interface._Ping(_x);
            this._endpoint.Interface._Ping(-1);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<_qss_x_.Experiment_.Object_.IPong_> _pongref;
        [QS.Fx.Base.Inspectable]
        private _qss_x_.Experiment_.Object_.IPong_ _pongproxy;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Experiment_.Interface_.IPong_,
                QS._qss_x_.Experiment_.Interface_.IPing_>
                    _endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _connection;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IPing_ Members

        void QS._qss_x_.Experiment_.Interface_.IPing_._Pong(int _x)
        {            
        }

        #endregion
    }
}
