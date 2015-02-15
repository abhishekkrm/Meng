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

// #define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace QS._qss_x_.Properties_.Component_
{
/*
    public abstract class Root_ : QS._qss_x_.Properties_.Component_.Base_, QS._qss_x_.Properties_.Object_.IProperties_
    {
        #region Constructor

        protected Root_()
        {
            this._bottom_receiver = new QS._qss_x_.Properties_.Base_.Receiver_(this._Bottom_Receive);
            this._bottom_endpoint =
                _mycontext.DualInterface<
                    QS._qss_x_.Properties_.Interface_.IProperties_,
                    QS._qss_x_.Properties_.Interface_.IProperties_>(this._bottom_receiver);
            this._bottom_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Bottom_Connect);
            this._bottom_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Bottom_Disconnect);
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Root_._Dispose");
#endif

            base._Dispose();
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
            base._Start();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Root_._Start");
#endif
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Root_._Stop");
#endif

            base._Stop();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.Receiver_ _bottom_receiver;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_,
            QS._qss_x_.Properties_.Interface_.IProperties_> _bottom_endpoint;

        #endregion

        #region IPropertiesObject_ Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_,
            QS._qss_x_.Properties_.Interface_.IProperties_>
                QS._qss_x_.Properties_.Object_.IProperties_.Properties
        {
            get { return this._bottom_endpoint; }
        }

        #endregion

        #region _Bottom_Connect

        protected virtual void _Bottom_Connect()
        {
        }

        #endregion

        #region _Bottom_Disconnect

        protected virtual void _Bottom_Disconnect()
        {
        }

        #endregion

        #region _Bottom_Send

        protected void _Bottom_Send(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
            this._bottom_endpoint.Interface.Value(_id, _version, _value);
        }

        #endregion

        #region _Bottom_Receive

        protected virtual void _Bottom_Receive(uint _id, QS._qss_x_.Properties_.Value_.IVersion_ _version, QS._qss_x_.Properties_.Value_.IValue_ _value)
        {
        }

        #endregion
    }
*/ 
}
