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
    public sealed class Object_ : QS._qss_x_.Folder_.Folder_
    {
        #region Constructor

        public Object_(QS.Fx.Object.IContext _mycontext, QuickSilver_ _myquicksilver)
            : base(_mycontext)
        {
            this._mycontext = _mycontext;
            this._myquicksilver = _myquicksilver;
            this._myconstructors.Add
            (
                "Qsm",
                new QS._qss_x_.Object_.Constructor_<QS._qss_x_.Qsm_.QsmApplicationObject_,
                    QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
                (
                    _mycontext,
                    "Qsm",
                    "QuickSilver Scalable Multicast",
                    "A client connection to QSM.",
                    new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._GetQsmApplicationObject)
                )
            );
            this._myconstructors.Add
            (
                "QsmController",
                new QS._qss_x_.Object_.Constructor_<QS._qss_x_.Qsm_.QsmControllerObject_,
                    QS._qss_x_.Object_.Classes_.IFactory2<
                        QS.Fx.Endpoint.Classes.IDualInterface<
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannelClient_1_<QS._qss_x_.Qsm_.QsmControl_>,
                            QS._qss_x_.Interface_.Classes_.ICommunicationChannel_1_<QS._qss_x_.Qsm_.QsmControl_>>>>
                (
                    _mycontext,
                    "QsmController",
                    "QuickSilver Scalable Multicast Controller",
                    "Channel controller for QSM.",
                    new QS._qss_c_.Base3_.Constructor<QS.Fx.Object.Classes.IObject>(this._GetQsmControllerObject)
                )
            );
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private QuickSilver_ _myquicksilver;

        #endregion

        #region _GetQsmApplicationObject

        private QS.Fx.Object.Classes.IObject _GetQsmApplicationObject()
        {
            return new QS._qss_x_.Qsm_.QsmApplicationObject_(_mycontext, this._myquicksilver._QsmApplication);
        }

        #endregion

        #region _GetQsmControllerObject

        private QS.Fx.Object.Classes.IObject _GetQsmControllerObject()
        {
            return new QS._qss_x_.Qsm_.QsmControllerObject_(_mycontext, this._myquicksilver._QsmController);
        }

        #endregion
    }
}
