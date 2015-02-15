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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Component_
{
/*
    [QS.Fx.Reflection.ComponentClass(QS._qss_x_.Properties_.Component_.Classes_._Registration, "Properties Framework Registration Object")]
    public sealed class Registration_ : QS._qss_x_.Properties_.Component_.Base_, QS._qss_x_.Properties_.Object_.IRegistration_
    {
        #region Constructor

        public Registration_
        (
/-*
            [QS.Fx.Reflection.Parameter("properties", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IProperties_> _properties_reference
*-/
        )
        {
            this._registration_endpoint = _mycontext.ImportedInterface<QS._qss_x_.Properties_.Interface_.IRegistration_>();
            this._registration_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Registration_Connect);
            this._registration_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Registration_Disconnect);

/-*
            this._properties_reference = _properties_reference;
            this._top_receiver = new QS._qss_x_.Properties_.Base_.Receiver_(this._Top_Receive);
            this._top_endpoint = 
                _mycontext.DualInterface<
                    QS._qss_x_.Properties_.Interface_.IProperties_, 
                    QS._qss_x_.Properties_.Interface_.IProperties_>(this._top_receiver);
            this._top_endpoint.OnConnected += new QS.Fx.Base.Callback(this._Top_Connect);
            this._top_endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._Top_Disconnect);
*-/
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Registration_._Dispose");
#endif

            base._Dispose();
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        QS.Fx.Endpoint.Internal.IImportedInterface<QS._qss_x_.Properties_.Interface_.IRegistration_> _registration_endpoint;

/-*
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IReference<QS._qss_x_.Properties_.Object_.IProperties_> _properties_reference;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Object_.IProperties_ _properties_object;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Classes.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_, 
            QS._qss_x_.Properties_.Interface_.IProperties_> _properties_endpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _properties_connection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Properties_.Base_.Receiver_ _top_receiver;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS._qss_x_.Properties_.Interface_.IProperties_, 
            QS._qss_x_.Properties_.Interface_.IProperties_> _top_endpoint;
*-/

        #endregion

        #region IRegistration_ Members

        QS.Fx.Endpoint.Classes.IImportedInterface<QS._qss_x_.Properties_.Interface_.IRegistration_> QS._qss_x_.Properties_.Object_.IRegistration_.Registration
        {
            get { return this._registration_endpoint; }
        }

        #endregion


        #region _Start

        protected override void _Start()
        {
            base._Start();

#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Registration_._Start");
#endif
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._platform != null)
                this._platform.Logger.Log("Component_.Registration_._Stop");
#endif

            base._Stop();
        }

        #endregion

        #region _Registration_Connect
        
        private void _Registration_Connect()
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion

        #region _Registration_Disconnect

        private void _Registration_Disconnect()
        {
            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        }

        #endregion
    }
*/
}
