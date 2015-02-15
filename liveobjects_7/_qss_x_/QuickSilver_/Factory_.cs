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
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_x_.QuickSilver_
{
    [QS.Fx.Reflection.ComponentClass
    (
        QS.Fx.Reflection.ComponentClasses.QuickSilver_,
        "QuickSilver", 
        "A factory that creates proxies to interact with the single in-process shared instance of the QuickSilver communication system."
    )]
    public sealed class Factory_ : QS._qss_x_.Factory_.Factory_, IDisposable
    {
        #region Constructor

        public Factory_
        (
            QS.Fx.Object.IContext _mycontext, 
            [QS.Fx.Reflection.Parameter("configuration", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Configuration.Configuration _configuration,
            [QS.Fx.Reflection.Parameter("connection", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Qsm_.QsmControl_>> _connection,
            [QS.Fx.Reflection.Parameter("deserializer", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer
        )
        : base
        (
            _mycontext,
            typeof(Object_), 
            QS._qss_x_.Reflection_.Library.ObjectClassOf(typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>)),
            "QuickSilver",
            "QuickSilver",
            "A proxy that interacts with the single in-process shared instance of the QuickSilver communication system."
        )
        {
            this._mycontext = _mycontext;
            this._configuration = _configuration;
            this._connection = _connection;
            this._deserializer = _deserializer;
        }

        #endregion

        #region Destructor

        ~Factory_()
        {
            this._Dispose(false);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region _Dispose

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                    lock (this)
                    {
                        if (this._myquicksilver != null)
                        {
                            try
                            {
                                ((IDisposable)this._myquicksilver).Dispose();
                            }
                            catch (Exception)
                            {
                            }
                        }
                        this._myquicksilver = null;
                    }
                }
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QS.Fx.Configuration.IConfiguration _configuration;
        private QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Qsm_.QsmControl_>> _connection;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer;
        private QuickSilver_ _myquicksilver;
        private int _disposed;

        #endregion

        #region _Create

        protected override QS.Fx.Object.Classes.IObject _Create()
        {
            lock (this)
            {
                if (this._myquicksilver == null)
                    this._myquicksilver = new QuickSilver_(_mycontext, this._configuration, this._connection, this._deserializer);
                return (QS.Fx.Object.Classes.IObject) new Object_(_mycontext, this._myquicksilver);
            }            
        }

        #endregion
    }
}
