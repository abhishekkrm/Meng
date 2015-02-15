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
    internal sealed class DualInterface_<ImportedInterface, ExportedInterface> 
        : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface>
        where ImportedInterface : class, QS.Fx.Interface.Classes.IInterface
        where ExportedInterface : class, QS.Fx.Interface.Classes.IInterface
    {
        #region Constructor

        public DualInterface_(ExportedInterface _exportedinterface) : base(null)
        {
            this._internalendpoint = null;
            this._importedconvertor = null;
            this._exportedconvertor = null;
            this._importedinterface = null;
            this._exportedinterface = _exportedinterface;
        }

        public DualInterface_(Endpoint_ _internalendpoint, QS._qss_x_.Reflection_.Internal_._internal_convertor _importedconvertor,
            QS._qss_x_.Reflection_.Internal_._internal_convertor _exportedconvertor) : base(_internalendpoint)
        {
            if (_internalendpoint is IDualInterface_)
                this._internalendpoint = (IDualInterface_) _internalendpoint;
            else
                throw new NotSupportedException();
            this._importedconvertor = _importedconvertor;
            this._exportedconvertor = _exportedconvertor;
            this._importedinterface = null;
            this._exportedinterface = null;
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private IDualInterface_ _internalendpoint;
        [QS.Fx.Base.Inspectable]
        private ImportedInterface _importedinterface;
        [QS.Fx.Base.Inspectable]
        private ExportedInterface _exportedinterface;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Reflection_.Internal_._internal_convertor _importedconvertor;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Reflection_.Internal_._internal_convertor _exportedconvertor;

        #endregion

        #region _Start and _Stop

        protected override void _Start(Endpoint_ _other)
        {
            IDualInterface_<ExportedInterface, ImportedInterface> other_ = _other as IDualInterface_<ExportedInterface, ImportedInterface>;
            if (other_ == null)
                throw new Exception("Endpoint type mismatch.");

            this._importedinterface = other_.ExportedInterface_;
            if (this._importedinterface == null)
                throw new Exception("Endpoint not configured.");

            if (this._internalendpoint != null)
            {
                object _interface = this._importedinterface;
                if (this._importedconvertor != null)
                    _interface = this._importedconvertor._convert(_interface);
                this._internalendpoint.ImportedInterface_ = _interface;
            }
        }

        protected override void _Stop(Endpoint_ _other)
        {
            this._importedinterface = null;

            if (this._internalendpoint != null)
                this._internalendpoint.ImportedInterface_ = null;
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IEndpoint_ Members

        QS._qss_x_.Endpoint_.Internal_.InterfaceClass_ QS._qss_x_.Endpoint_.Internal_.IEndpoint_.InterfaceClass_
        {
            get { return InterfaceClass_.DualInterface_; }
        }

        #endregion

        #region QS.Fx.Endpoint.Internal.IDualInterface<ImportedInterface,ExportedInterface> Members

        ImportedInterface QS.Fx.Endpoint.Internal.IDualInterface<ImportedInterface, ExportedInterface>.Interface
        {
            get
            {
                ImportedInterface _importedinterface = this._importedinterface;
                if (_importedinterface == null)
                    throw new Exception("The endpoint is not connected.");
                return _importedinterface;
            }
        }

        #endregion

        #region IDualInterface_ Members

        object IDualInterface_.ExportedInterface_
        {
            get { return ((QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface>) this).ExportedInterface_; }
        }

        object IDualInterface_.ImportedInterface_
        {
            set { ((QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface>) this).ImportedInterface_ = (ImportedInterface) value; }
        }

        #endregion

        #region QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface> Members

        ExportedInterface QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface>.ExportedInterface_
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
                                if (this._exportedconvertor != null)
                                    _interface = this._exportedconvertor._convert(_interface);
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

        ImportedInterface QS._qss_x_.Endpoint_.Internal_.IDualInterface_<ImportedInterface, ExportedInterface>.ImportedInterface_
        {
            set { this._importedinterface = value; }
        }

        #endregion
    }
}
