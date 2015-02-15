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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endif

namespace QS._qss_x_.Endpoint_.Internal_
{
    internal sealed class ExportedUI_X_ : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IExportedUI_X_
    {
        #region Constructor

        public ExportedUI_X_(QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback,
            QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback, QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback)
            : base(null)
        {
            this._repositioncallback = _repositioncallback;
            this._updatecallback = _updatecallback;
            this._drawcallback = _drawcallback;

            if (this._repositioncallback == null)
                throw new Exception("Reposition callback is null.");
            if (this._updatecallback == null)
                throw new Exception("Update callback is null.");
            if (this._drawcallback == null)
                throw new Exception("Draw callback is null.");
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback;
        private QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback;
        private QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback;
#if XNA
        private IGraphicsDeviceService _graphicsdevice;
#endif
        private QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback;

        #endregion

        #region _Start and _Stop

        protected override void _Start(Endpoint_ _other)
        {
            IImportedUI_X_ other_ = _other as IImportedUI_X_;
            if (other_ == null)
                throw new Exception("Endpoint type mismatch.");

#if XNA
            this._graphicsdevice = other_.GraphicsDevice_;
#endif
            this._contentcallback = other_.ContentCallback_;
        }

        protected override void _Stop(Endpoint_ _other)
        {
#if XNA
            this._graphicsdevice = null;
#endif
            this._contentcallback = null;
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return QS._qss_x_.Endpoint_.Internal_.InterfaceClass_.ExportedUI_X_; }
        }

        #endregion

        #region QS.Fx.Endpoint.Internal.IExportedUI_X Members

#if XNA
        IGraphicsDeviceService QS.Fx.Endpoint.Internal.IExportedUI_X.GraphicsDevice
        {
            get 
            {
                IGraphicsDeviceService _graphicsdevice = this._graphicsdevice;
                if (_graphicsdevice == null)
                    throw new Exception("The endpoint is not connected.");
                return _graphicsdevice;
            }
        }
#endif

        QS.Fx.Xna.IContent QS.Fx.Endpoint.Internal.IExportedUI_X.Content(QS.Fx.Xna.IContentRef _contentref)
        {
            QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback = this._contentcallback;
            if (_contentcallback == null)
                throw new Exception("The endpoint is not connected.");
            return _contentcallback(_contentref);
        }

        #endregion

        #region IExportedUI_X_ Members

        QS.Fx.Endpoint.Internal.Xna.RepositionCallback IExportedUI_X_.RepositionCallback_
        {
            get { return this._repositioncallback; }
        }

        QS.Fx.Endpoint.Internal.Xna.UpdateCallback IExportedUI_X_.UpdateCallback_
        {
            get { return this._updatecallback; }
        }

        QS.Fx.Endpoint.Internal.Xna.DrawCallback IExportedUI_X_.DrawCallback_
        {
            get { return this._drawcallback; }
        }

        #endregion
    }
}
