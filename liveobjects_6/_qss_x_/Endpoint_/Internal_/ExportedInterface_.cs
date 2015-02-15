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
    internal sealed class ExportedInterface_<ExportedInterface> 
        : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IExportedInterface_<ExportedInterface>
        where ExportedInterface : class, QS.Fx.Interface.Classes.IInterface
    {
        #region Constructor

        public ExportedInterface_(ExportedInterface _exportedinterface) : base(null)
        {
            this._internalendpoint = null;
            this._internalconvertor = null;
            this._exportedinterface = _exportedinterface;
        }

        public ExportedInterface_(Endpoint_ _internalendpoint, QS._qss_x_.Reflection_.Internal_._internal_convertor _internalconvertor) : base(_internalendpoint)
        {
            if (_internalendpoint is IExportedInterface_)
                this._internalendpoint = (IExportedInterface_) _internalendpoint;
            else
                throw new NotSupportedException();
            this._internalconvertor = _internalconvertor;
            this._exportedinterface = null;
        }

        #endregion

        #region Fields

        private IExportedInterface_ _internalendpoint;
        private ExportedInterface _exportedinterface;
        private QS._qss_x_.Reflection_.Internal_._internal_convertor _internalconvertor;

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return InterfaceClass_.ExportedInterface_; }
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IExportedInterface_ Members

        object QS._qss_x_.Endpoint_.Internal_.IExportedInterface_.ExportedInterface_
        {
            get { return ((QS._qss_x_.Endpoint_.Internal_.IExportedInterface_<ExportedInterface>)this).ExportedInterface_; }
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IExportedInterface_<ExportedInterface> Members

        ExportedInterface QS._qss_x_.Endpoint_.Internal_.IExportedInterface_<ExportedInterface>.ExportedInterface_
        {
            get
            {
                if (this._exportedinterface != null)
                    return this._exportedinterface;
                else 
                {
                    if (this._internalendpoint != null)
                    {
                        lock (this)
                        {
                            if ((this._exportedinterface == null) && (this._internalendpoint != null))
                            {
                                object _interface = this._internalendpoint.ExportedInterface_;
                                if (this._internalconvertor != null)
                                    _interface = this._internalconvertor._convert(_interface);
                                this._exportedinterface = (ExportedInterface) _interface;
                            }
                            return this._exportedinterface;
                        }
                    }
                    else
                        throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}
