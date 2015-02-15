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
    public sealed class EmbeddedObject : QS.Fx.Object.IEmbeddedObject
    {
        #region Constructor

        public EmbeddedObject(
            string _objectxml, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref, QS.Fx.Object.Classes.IObject _object,
            IEnumerable<IDisposable> _todispose, System.Windows.Forms.Control _containercontrol)
        {
            this._objectxml = _objectxml;
            this._objectref = _objectref;
            this._object = _object;
            this._todispose = _todispose;
            this._containercontrol = _containercontrol;
        }

        #endregion

        #region Fields

        private string _objectxml;
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
        private QS.Fx.Object.Classes.IObject _object;
        private IEnumerable<IDisposable> _todispose;
        private System.Windows.Forms.Control _containercontrol;
        private bool _disposed;

        #endregion

        #region IEmbeddedObject Members

        string QS.Fx.Object.IEmbeddedObject.Xml
        {
            get
            {
                lock (this)
                {
                    return _objectxml;
                }
            }
        }

        System.Windows.Forms.Control QS.Fx.Object.IEmbeddedObject.UI
        {
            get
            {
                lock (this)
                {
                    return _containercontrol;
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (!_disposed)
                {
                    _objectxml = null;
                    _objectref = null;
                    _object = null;
                    if (_todispose != null)
                    {
                        foreach (IDisposable _o in _todispose)
                        {
                            try
                            {
                                _o.Dispose();
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    _containercontrol = null;
                    _disposed = true;
                }
            }
        }

        #endregion
    }
}
