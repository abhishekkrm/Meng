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
using System.Xml.Serialization;

namespace QS._qss_x_.Qsm_
{
    public sealed class QsmControllerObject_ : 
        QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>,
        IDisposable
    {
        #region Constructor

        public QsmControllerObject_(QS.Fx.Object.IContext _mycontext, QsmController_ _qsmcontroller)
        {
            this._qsmcontroller = _qsmcontroller;
            this._myendpoint = 
                _mycontext.ExportedInterface<
                    QS._qss_x_.Interface_.Classes_.IFactory2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>>(this._qsmcontroller);
        }

        #endregion

        #region Destructor

        ~QsmControllerObject_()
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
                }
            }
        }

        #endregion

        #region Fields

        private int _disposed;
        private QsmController_ _qsmcontroller;
        private QS.Fx.Endpoint.Classes.IExportedInterface<
            QS._qss_x_.Interface_.Classes_.IFactory2<
                QS.Fx.Endpoint.Classes.IDualInterface<
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>,
                    QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>> _myendpoint;

        #endregion

        #region IFactory2<IDualInterface<ICommunicationChannelClient<QsmControl_>,ICommunicationChannel<QsmControl_>>> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<
            QS._qss_x_.Interface_.Classes_.IFactory2<
                    QS.Fx.Endpoint.Classes.IDualInterface<
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>, 
                        QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>> 
        QS._qss_x_.Object_.Classes_.IFactory2<
            QS.Fx.Endpoint.Classes.IDualInterface<
                QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QsmControl_>, 
                QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QsmControl_>>>.Endpoint
        {
            get { return this._myendpoint; } 
        }

        #endregion
    }
}
