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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Factory_2, "Factory_2", "An endpoint factory.")]
    public sealed class Factory_2<[QS.Fx.Reflection.Parameter("EndpointClass", QS.Fx.Reflection.ParameterClass.EndpointClass)] EndpointClass> :
        QS.Fx.Inspection.Inspectable,
        QS._qss_x_.Object_.Classes_.IFactory2<EndpointClass>
        where EndpointClass : class, QS.Fx.Endpoint.Classes.IEndpoint
    {
        #region Constructor

        public Factory_2(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("interface", QS.Fx.Reflection.ParameterClass.Value)] 
                QS._qss_x_.Interface_.IReference<QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass>> 
                    _interfaceref)
        {
            this._mycontext = _mycontext;
            if (_interfaceref == null)
                throw new Exception("Interface reference cannot be null.");
            this._interfaceref = _interfaceref;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Interface_.IReference<QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass>> _interfaceref;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass>> _endpoint;

        #endregion

        #region IFactory2<EndpointClass> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass>> QS._qss_x_.Object_.Classes_.IFactory2<EndpointClass>.Endpoint
        {
            get 
            {
                lock (this)
                {
                    if (this._endpoint == null)
                    {
                        QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass> _interface = this._interfaceref.Interface;
                        if (_interface == null)
                            throw new Exception("This object is unusable because the embedded interface reference resolved to a null interface.");

                        this._endpoint = _mycontext.ExportedInterface<QS._qss_x_.Interface_.Classes_.IFactory2<EndpointClass>>(_interface);
                    }
                    return this._endpoint;
                }
            } 
        }

        #endregion
    }
}
