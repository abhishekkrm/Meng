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
using System.Windows.Forms;

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_EndpointClass_ : Element_Class_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_EndpointClass_(
            string _id, QS.Fx.Attributes.IAttributes _attributes, Category_ _category, Element_Parameter_ _binding,
            Element_Environment_ _environment, bool _automatic, QS.Fx.Reflection.IEndpointClass _template_endpointclass)
            : base(_id, _attributes, _category, _binding, _environment, _automatic)
        {
            this._template_endpointclass = _template_endpointclass;
        }

        public Element_EndpointClass_(Element_EndpointClass_ _other) : base(_other)
        {
            this._template_endpointclass = _other._template_endpointclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IEndpointClass _template_endpointclass, _reflected_endpointclass;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Reflected_EndpointClass

        public QS.Fx.Reflection.IEndpointClass _Reflected_EndpointClass
        {
            get { return this._reflected_endpointclass; }
        }

        #endregion

        #region Serialize

        public QS.Fx.Reflection.Xml.EndpointClass _Serialize()
        {
            switch (this._category)
            {
                case Category_.Predefined_:
                    return new QS.Fx.Reflection.Xml.EndpointClass(this._id,
                        (this._environment != null) ? this._environment._Serialize() : new QS.Fx.Reflection.Xml.Parameter[0]);
                case Category_.Parameter_:
                    return ((Element_EndpointClass_)this._binding._Value)._Serialize();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            base._Rebuild();
            this.Text = "Endpoint" + this.Text;
        }

        #endregion

        #region _Validate

        public override void _Validate()
        {
            lock (this)
            {
                base._Validate();
                if (this._correct)
                {
                    switch (this._category)
                    {
                        case Category_.Predefined_:
                            {
                                try
                                {
                                    IDictionary<string, QS.Fx.Reflection.IParameter> _reflected_parameters = this._environment._Reflected_Parameters;
                                    this._reflected_endpointclass = this._template_endpointclass.Instantiate(_reflected_parameters.Values);
                                }
                                catch (Exception _exc)
                                {
                                    this._correct = false;
                                    this._error = _exc.ToString();
                                }
                            }
                            break;

                        case Category_.Parameter_:
                            {
                                if (this._binding._ParameterClass == QS.Fx.Reflection.ParameterClass.EndpointClass)
                                {
                                    if (this._binding._Value != null)
                                    {
                                        if (this._binding._Value is Element_EndpointClass_)
                                        {
                                            if (this._binding._Value._Correct)
                                            {
                                                this._reflected_endpointclass = ((Element_EndpointClass_)this._binding._Value)._Reflected_EndpointClass;
                                            }
                                            else
                                            {
                                                this._correct = false;
                                                this._error = "Error in parameter \"" + this._binding._ID + "\".";
                                            }
                                        }
                                        else
                                        {
                                            this._correct = false;
                                            this._error = "Parameter \"" + this._binding._ID + "\" is not an endpoint class.";
                                        }
                                    }
                                    else
                                    {
                                        this._correct = false;
                                        this._error = "Parameter \"" + this._binding._ID + "\" is undefined.";
                                    }
                                }
                                else
                                {
                                    this._correct = false;
                                    this._error = "Parameter \"" + this._binding._ID + "\" is not an endpoint class parameter.";
                                }
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }

        #endregion

        #region _Menu

        public override IEnumerable<Element_Action_> _Menu()
        {
            List<Element_Action_> _menu = new List<Element_Action_>(base._Menu());
            if ((this._correct) && (this._reflected_endpointclass != null))
            {
                _menu.Add(
                    new Element_Action_(
                        "Generate Code",
                            new QS.Fx.Base.ContextCallback(
                                this._GenerateCodeCallback), null));

                _menu.Add(
                    new Element_Action_(
                        "Generate Code for a Matching Endpoint", 
                            new QS.Fx.Base.ContextCallback(
                                this._GenerateMatchingEndpointCodeCallback), null));
            }
            return _menu;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _GenerateCodeCallback

        private void _GenerateCodeCallback(object _context)
        {
            lock (this)
            {
                if (this._correct)
                {
                    StringBuilder _ss = new StringBuilder();
                    this._GenerateTypeCode(this._reflected_endpointclass.UnderlyingType, _ss);
                    string _code = _ss.ToString();
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.Text, _code);
                    _dataobject.SetData(DataFormats.UnicodeText, _code);
                    Clipboard.SetDataObject(_dataobject);
                }
            }
        }

        #endregion

        #region _GenerateMatchingEndpointCodeCallback

        private static readonly Type _importedinterfacetemplate = 
            typeof(QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        private static readonly Type _exportedinterfacetemplate =
            typeof(QS.Fx.Endpoint.Classes.IExportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        private static readonly Type _dualinterfacetemplate =
            typeof(QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IInterface, QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();

        private static readonly Type _internal_importedinterfacetemplate =
            typeof(QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        private static readonly Type _internal_exportedinterfacetemplate =
            typeof(QS.Fx.Endpoint.Internal.IExportedInterface<QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();
        private static readonly Type _internal_dualinterfacetemplate =
            typeof(QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IInterface, QS.Fx.Interface.Classes.IInterface>).GetGenericTypeDefinition();

        private void _GenerateMatchingEndpointCodeCallback(object _context)
        {
            lock (this)
            {
                if (this._correct)
                {
                    Type _mytype = this._reflected_endpointclass.UnderlyingType;
                    Type _matchingtype;
                    if (_mytype.IsGenericType)
                    {
                        Type _mytypetemplate = _mytype.GetGenericTypeDefinition();
                        if (_mytypetemplate.Equals(_importedinterfacetemplate))
                            _matchingtype = _internal_exportedinterfacetemplate.MakeGenericType(_mytype.GetGenericArguments());
                        else if (_mytypetemplate.Equals(_exportedinterfacetemplate))
                            _matchingtype = _internal_importedinterfacetemplate.MakeGenericType(_mytype.GetGenericArguments());
                        else if (_mytypetemplate.Equals(_dualinterfacetemplate))
                        {
                            Type[] _a = _mytype.GetGenericArguments();
                            _matchingtype = _internal_dualinterfacetemplate.MakeGenericType(_a[1], _a[0]);
                        }
                        else
                            throw new NotImplementedException();
                    }
                    else
                    {
                        if (_mytype.Equals(typeof(QS.Fx.Endpoint.Classes.IEndpoint)))
                            _matchingtype = typeof(QS.Fx.Endpoint.Internal.IEndpoint);
                        else if (_mytype.Equals(typeof(QS.Fx.Endpoint.Classes.IExportedUI)))
                            _matchingtype = typeof(QS.Fx.Endpoint.Internal.IImportedUI);
                        else if (_mytype.Equals(typeof(QS.Fx.Endpoint.Classes.IImportedUI)))
                            _matchingtype = typeof(QS.Fx.Endpoint.Internal.IExportedUI);
                        else if (_mytype.Equals(typeof(QS.Fx.Endpoint.Classes.IExportedUI_X)))
                            _matchingtype = typeof(QS.Fx.Endpoint.Internal.IImportedUI_X);
                        else if (_mytype.Equals(typeof(QS.Fx.Endpoint.Classes.IImportedUI_X)))
                            _matchingtype = typeof(QS.Fx.Endpoint.Internal.IExportedUI_X);
                        else
                            throw new NotImplementedException();
                    }
                    StringBuilder _ss = new StringBuilder();
                    this._GenerateTypeCode(_matchingtype, _ss);
                    string _code = _ss.ToString();
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.Text, _code);
                    _dataobject.SetData(DataFormats.UnicodeText, _code);
                    Clipboard.SetDataObject(_dataobject);
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
