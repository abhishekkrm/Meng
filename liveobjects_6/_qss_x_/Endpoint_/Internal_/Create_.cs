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

namespace QS._qss_x_.Endpoint_.Internal_
{
/*
    internal class Create_ : QS.Fx.Endpoint.Internal.ICreate
    {
        #region Register

        internal static void Register_()
        {
            _mycontext.Initialize(new Create_());
        }

        #endregion

        #region Constructors

        private Create_()
        {
        }

        #endregion

        #region ICreate Members

        QS.Fx.Endpoint.Internal.IExportedInterface<I> QS.Fx.Endpoint.Internal.ICreate.ExportedInterface<I>(I _exportedinterface)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ExportedInterface_<I>(_exportedinterface);
        }

        QS.Fx.Endpoint.Internal.IImportedInterface<I> QS.Fx.Endpoint.Internal.ICreate.ImportedInterface<I>()
        {
            return new QS._qss_x_.Endpoint_.Internal_.ImportedInterface_<I>();
        }

        QS.Fx.Endpoint.Internal.IDualInterface<I, J> QS.Fx.Endpoint.Internal.ICreate.DualInterface<I, J>(J _exportedinterface)
        {
            return new QS._qss_x_.Endpoint_.Internal_.DualInterface_<I, J>(_exportedinterface);
        }

        QS.Fx.Endpoint.Internal.IExportedUI QS.Fx.Endpoint.Internal.ICreate.ExportedUI(System.Windows.Forms.Control _ui)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ExportedUI_(_ui);
        }

        QS.Fx.Endpoint.Internal.IImportedUI QS.Fx.Endpoint.Internal.ICreate.ImportedUI(System.Windows.Forms.Control _containerui)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ImportedUI_(_containerui);
        }

        QS.Fx.Endpoint.Internal.IExportedUI_X QS.Fx.Endpoint.Internal.ICreate.ExportedUI_X(
            QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback, 
            QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback, 
            QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback)
        {
            return new QS._qss_x_.Endpoint_.Internal_.ExportedUI_X_(_repositioncallback, _updatecallback, _drawcallback);
        }

        QS.Fx.Endpoint.Internal.IImportedUI_X QS.Fx.Endpoint.Internal.ICreate.ImportedUI_X
        (
#if XNA
            Microsoft.Xna.Framework.GraphicsDeviceManager _graphicsdevice, 
#endif
            QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback
        )
        {
            return 
                new QS._qss_x_.Endpoint_.Internal_.ImportedUI_X_
                (
#if XNA
                    _graphicsdevice, 
#endif
                    _contentcallback
                );
        }

        #endregion
    }
*/
}
