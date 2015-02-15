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

namespace QS.Fx.Xna
{
    public sealed class Content : QS.Fx.Xna.IContent, QS.Fx.Xna.IContentRef
    {
        #region Constructor

        public Content(QS.Fx.Xna.ContentClass _contentclass, string _id, object _content, QS.Fx.Base.ContextCallback<QS.Fx.Xna.Content> _disposecallback)
        {
            this._contentclass = _contentclass;
            this._id = _id;
            this._content = _content;
            this._disposecallback = _disposecallback;

            switch (_contentclass)
            {
                case QS.Fx.Xna.ContentClass.Model:
                    {
#if XNA
                        if (!(_content is Microsoft.Xna.Framework.Graphics.Model))
                            throw new Exception("Invalid content reference, the loaded object is not a model.");
#endif
                    }
                    break;

                case QS.Fx.Xna.ContentClass.Texture2D:
                    {
#if XNA
                        if (!(_content is Microsoft.Xna.Framework.Graphics.Texture2D))
                            throw new Exception("Invalid content reference, the loaded object is not a 2-dimensional texture.");
#endif
                    }
                    break;

                case QS.Fx.Xna.ContentClass.Texture3D:
                    {
#if XNA
                        if (!(_content is Microsoft.Xna.Framework.Graphics.Texture3D))
                            throw new Exception("Invalid content reference, the loaded object is not a 3-dimensional texture.");
#endif
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Xna.ContentClass _contentclass;
        private string _id;
        private object _content;
        private QS.Fx.Base.ContextCallback<QS.Fx.Xna.Content> _disposecallback;
        private bool _disposed;

        #endregion

        #region IContent Members

        QS.Fx.Xna.IContentRef QS.Fx.Xna.IContent.Reference
        {
            get { return this; }
        }

        object QS.Fx.Xna.IContent.Content
        {
            get 
            {
                lock (this)
                {
                    if (this._disposed)
                        throw new Exception("Cannot access the content because it has already been disposed.");
                    return this._content;
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (this._disposed)
                    throw new Exception("Cannot dispose the content because it has already been disposed.");
                this._disposed = true;
                if (this._disposecallback != null)
                    this._disposecallback(this);
            }
        }

        #endregion

        #region IContentRef Members

        QS.Fx.Xna.ContentClass QS.Fx.Xna.IContentRef.ContentClass
        {
            get { return this._contentclass; }
            set { throw new NotSupportedException(); }
        }

        string QS.Fx.Xna.IContentRef.ID
        {
            get { return this._id; }
            set { throw new NotSupportedException(); }
        }

        #endregion
    }
}
