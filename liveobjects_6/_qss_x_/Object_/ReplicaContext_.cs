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

namespace QS._qss_x_.Object_
{
    public sealed class ReplicaContext_ : QS.Fx.Object.IContext
    {
        #region Constructor

        public ReplicaContext_(QS.Fx.Object.IContext _mastercontext)
        {
            this._mastercontext = _mastercontext;
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mastercontext;

        #endregion

        #region IContext Members

        QS.Fx.Base.SynchronizationOption QS.Fx.Object.IContext.SynchronizationOption
        {
            get { throw new NotImplementedException(); }
        }

        void QS.Fx.Object.IContext.Enqueue(QS.Fx.Base.IEvent e)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Object.IContext.Error(string s, Exception e)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Object.IContext.Error(string s)
        {
            throw new NotImplementedException();
        }

        void QS.Fx.Object.IContext.Error(Exception e)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Platform.IPlatform QS.Fx.Object.IContext.Platform
        {
            get { return this._mastercontext.Platform; }
        }

        bool QS.Fx.Object.IContext.CanCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            throw new NotImplementedException();
        }

        ObjectClass QS.Fx.Object.IContext.SafeCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            throw new NotImplementedException();
        }

        ObjectClass QS.Fx.Object.IContext.UnsafeCast<ObjectClass>(QS.Fx.Object.Classes.IObject _proxy)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IExportedInterface<I> QS.Fx.Object.IContext.ExportedInterface<I>(I _exportedinterface)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IImportedInterface<I> QS.Fx.Object.IContext.ImportedInterface<I>()
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IDualInterface<I, J> QS.Fx.Object.IContext.DualInterface<I, J>(J _exportedinterface)
        {
            return this._mastercontext.DualInterface<I, J>(_exportedinterface);
        }

        QS.Fx.Endpoint.Internal.IExportedUI QS.Fx.Object.IContext.ExportedUI(System.Windows.Forms.Control _ui)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IImportedUI QS.Fx.Object.IContext.ImportedUI(System.Windows.Forms.Control _containerui)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IExportedUI_X QS.Fx.Object.IContext.ExportedUI_X(QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback, QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback, QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback)
        {
            throw new NotImplementedException();
        }

        QS.Fx.Endpoint.Internal.IImportedUI_X QS.Fx.Object.IContext.ImportedUI_X(
#if XNA
            Microsoft.Xna.Framework.Graphics.IGraphicsDeviceService _graphicsdevice,
#endif
QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback
       )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
