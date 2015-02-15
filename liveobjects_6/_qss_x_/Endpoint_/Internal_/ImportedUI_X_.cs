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
    internal sealed class ImportedUI_X_ : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IImportedUI_X_
    {
        #region Constructor

        public ImportedUI_X_
        (
#if XNA
            IGraphicsDeviceService _graphicsdevice, 
#endif
            QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback
        )
        : base(null)
        {
#if XNA
            this._graphicsdevice = _graphicsdevice;
            if (this._graphicsdevice == null)
                throw new Exception("Graphics device is null.");
#endif

            this._contentcallback = _contentcallback;
            if (this._contentcallback == null)
                throw new Exception("Content callback is null.");
        }

        #endregion

        #region Fields

#if XNA
        private IGraphicsDeviceService _graphicsdevice;
#endif
        private QS.Fx.Endpoint.Internal.Xna.ContentCallback _contentcallback;
        private QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback;
        private QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback;
        private QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback;

        #endregion

        #region _Start and _Stop

        protected override void _Start(Endpoint_ _other)
        {
            IExportedUI_X_ other_ = _other as IExportedUI_X_;
            if (other_ == null)
                throw new Exception("Endpoint type mismatch.");

            this._repositioncallback = other_.RepositionCallback_;
            this._updatecallback = other_.UpdateCallback_;
            this._drawcallback = other_.DrawCallback_;
        }

        protected override void _Stop(Endpoint_ _other)
        {
            this._repositioncallback = null;
            this._updatecallback = null;
            this._drawcallback = null;
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return QS._qss_x_.Endpoint_.Internal_.InterfaceClass_.Imported_UI_X_; }
        }

        #endregion

        #region IImportedUI_X Members

        void QS.Fx.Endpoint.Internal.IImportedUI_X.Reposition
        (
#if XNA
            Matrix _cameramatrix, 
            Matrix _projectionmatrix
#endif
        )
        {
            QS.Fx.Endpoint.Internal.Xna.RepositionCallback _repositioncallback = this._repositioncallback;
            if (_repositioncallback != null)
                _repositioncallback
                (
#if XNA
                    _cameramatrix, 
                    _projectionmatrix
#endif
                );
        }

        void QS.Fx.Endpoint.Internal.IImportedUI_X.Update
        (
#if XNA
            GameTime _time
#endif
        )
        {
            QS.Fx.Endpoint.Internal.Xna.UpdateCallback _updatecallback = this._updatecallback;
            if (_updatecallback != null)
                _updatecallback
                (
#if XNA
                    _time
#endif
                );
        }

        void QS.Fx.Endpoint.Internal.IImportedUI_X.Draw
        (
#if XNA
            GameTime _time
#endif
        )
        {
            QS.Fx.Endpoint.Internal.Xna.DrawCallback _drawcallback = this._drawcallback;
            if (_drawcallback != null)
                _drawcallback
                (
#if XNA
                    _time
#endif
                );
        }

        #endregion

        #region IImportedUI_X_ Members

#if XNA
        IGraphicsDeviceService IImportedUI_X_.GraphicsDevice_
        {
            get { return this._graphicsdevice; }
        }
#endif

        QS.Fx.Endpoint.Internal.Xna.ContentCallback IImportedUI_X_.ContentCallback_
        {
            get { return this._contentcallback; }
        }

        #endregion
    }
}
