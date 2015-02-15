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
    internal sealed class ImportedInterface_<ImportedInterface> 
        : Endpoint_, QS._qss_x_.Endpoint_.Internal_.IImportedInterface_<ImportedInterface>
        where ImportedInterface : class, QS.Fx.Interface.Classes.IInterface
    {
        #region Constructor

        public ImportedInterface_() : base(null)
        {
            this._internalendpoint = null;
            this._internalconvertor = null;
            this._importedinterface = null;
        }

        public ImportedInterface_(Endpoint_ _internalendpoint, QS._qss_x_.Reflection_.Internal_._internal_convertor _internalconvertor)
            : base(_internalendpoint)
        {
            if (_internalendpoint is IImportedInterface_)
                this._internalendpoint = (IImportedInterface_) _internalendpoint;
            else
                throw new NotSupportedException();
            this._internalconvertor = _internalconvertor;
            this._importedinterface = null;
        }

        #endregion

        #region Fields

        private IImportedInterface_ _internalendpoint;
        private ImportedInterface _importedinterface;
        private QS._qss_x_.Reflection_.Internal_._internal_convertor _internalconvertor;

        #endregion

        #region _Start and _Stop

        protected override void _Start(Endpoint_ _other)
        {
            IExportedInterface_<ImportedInterface> other_ = _other as IExportedInterface_<ImportedInterface>;
            if (other_ == null)
                throw new Exception("Endpoint type mismatch.");

            this._importedinterface = other_.ExportedInterface_;
            if (this._importedinterface == null)
                throw new Exception("Endpoint not configured.");

            if (this._internalendpoint != null)
            {
                object _interface = this._importedinterface;
                if (this._internalconvertor != null)
                    _interface = this._internalconvertor._convert(_interface);
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
            get { return InterfaceClass_.ImportedInterface_; }
        }

        #endregion

        #region QS.Fx.Endpoint.Internal.IImportedInterface<ImportedInterface> Members

        ImportedInterface QS.Fx.Endpoint.Internal.IImportedInterface<ImportedInterface>.Interface
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

        #region IImportedInterface_ Members

        object IImportedInterface_.ImportedInterface_
        {
            set { ((QS._qss_x_.Endpoint_.Internal_.IImportedInterface_<ImportedInterface>)this).ImportedInterface_ = (ImportedInterface)value; }
        }

        #endregion

        #region IImportedInterface_<ImportedInterface> Members

        ImportedInterface IImportedInterface_<ImportedInterface>.ImportedInterface_
        {
            set { this._importedinterface = value; }
        }

        #endregion
    }
}
