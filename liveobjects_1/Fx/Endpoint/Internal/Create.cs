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

namespace QS.Fx.Endpoint.Internal
{
    
    public static class Create
    {
        public static IExportedInterface<I> ExportedInterface<I>(I _exportedinterface)
            where I : class, QS.Fx.Interface.Classes.IInterface
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.ExportedInterface<I>(_exportedinterface);
        }

        public static IImportedInterface<I> ImportedInterface<I>()
            where I : class, QS.Fx.Interface.Classes.IInterface
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.ImportedInterface<I>();
        }

        public static IDualInterface<I, J> DualInterface<I, J>(J _exportedinterface)
            where I : class, QS.Fx.Interface.Classes.IInterface
            where J : class, QS.Fx.Interface.Classes.IInterface
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.DualInterface<I, J>(_exportedinterface);
        }

        public static IExportedUI ExportedUI(System.Windows.Forms.Control _ui)
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.ExportedUI(_ui);
        }

        public static IImportedUI ImportedUI(System.Windows.Forms.Control _containerui)
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.ImportedUI(_containerui);
        }

        public static IExportedUI_X ExportedUI_X(
            QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback,
            QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback,
            QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback)
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return _m_create.ExportedUI_X(_repositioncallback, _updatecallback, _drawcallback);
        }

        public static IImportedUI_X ImportedUI_X
        (
#if XNA
            Microsoft.Xna.Framework.GraphicsDeviceManager _graphicsdevice,
#endif
            QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback
        )
        {
            if (_m_create == null)
                throw new NotImplementedException();
            return 
                _m_create.ImportedUI_X
                (
#if XNA
                    _graphicsdevice, 
#endif
                    _contentcallback
                );
        }

        private static ICreate _m_create;

        public static void Initialize(ICreate _create)
        {
            if (System.Threading.Interlocked.CompareExchange<ICreate>(ref _m_create, _create, null) != null)
                throw new Exception(
                    "Could not initialize the endpoint factory because the factory has already been initialized with some other object.");
        }
    }
}
