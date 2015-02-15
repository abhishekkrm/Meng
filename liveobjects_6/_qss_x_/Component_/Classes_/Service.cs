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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Service, "Service", "A service object that exposes a certain interface.")]
    public class Service<[QS.Fx.Reflection.Parameter("InterfaceClass", QS.Fx.Reflection.ParameterClass.InterfaceClass)] InterfaceClass> :
        IDisposable, QS.Fx.Object.Classes.IService<InterfaceClass>
        where InterfaceClass : class, QS.Fx.Interface.Classes.IInterface
    {
        #region Constructor

        public Service(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("interface", QS.Fx.Reflection.ParameterClass.Value)] QS._qss_x_.Interface_.IReference<InterfaceClass> _interface)
        {
            this._endpoint = _mycontext.ExportedInterface<InterfaceClass>(_interface.Interface);
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedInterface<InterfaceClass> _endpoint;

        #endregion

        #region IService<InterfaceClass> Members

        QS.Fx.Endpoint.Classes.IExportedInterface<InterfaceClass> QS.Fx.Object.Classes.IService<InterfaceClass>.Endpoint
        {
            get { return _endpoint; }
        }

        #endregion

        #region Connect

        public static QS.Fx.Endpoint.IConnection Connect(QS.Fx.Object.IContext _mycontext, QS.Fx.Object.Classes.IService<InterfaceClass> _service, out InterfaceClass _interface)
        {
            QS.Fx.Endpoint.Internal.IImportedInterface<InterfaceClass> _endpoint = _mycontext.ImportedInterface<InterfaceClass>();
            QS.Fx.Endpoint.IConnection _connection = ((QS.Fx.Endpoint.Classes.IEndpoint)_endpoint).Connect(_service.Endpoint);
            _interface = _endpoint.Interface;
            return _connection;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
