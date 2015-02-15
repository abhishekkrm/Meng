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

namespace QS._qss_x_.ObjectDesigner_.Elements_
{
    public sealed class Element_Environment_ : Element_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_Environment_(Element_Environment_ _context, IDictionary<string, Element_Parameter_> _parameters,
            bool _automatic, IDictionary<string, QS.Fx.Reflection.IParameter> _template_parameters) : base(_automatic)
        {
            this._context = _context;
            this._parameters = _parameters;
            this._template_parameters = _template_parameters;
        }

        public Element_Environment_(Element_Environment_ _other) : base(true)
        {
            this._context = _other._context;
            if (_other._parameters != null)
            {
                this._parameters = new Dictionary<string, Element_Parameter_>();
                foreach (KeyValuePair<string, Element_Parameter_> _e in _other._parameters)
                    this._parameters.Add(_e.Key, new Element_Parameter_(_e.Value));
            }
            else
                this._parameters = null;
            this._template_parameters = _other._template_parameters;
        }

        #endregion

        #region Fields

        private Element_Environment_ _context;
        private IDictionary<string, Element_Parameter_> _parameters;
        private IDictionary<string, QS.Fx.Reflection.IParameter> _template_parameters, _reflected_parameters;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Context

        public Element_Environment_ _Context
        {
            get { return this._context; }
        }

        #endregion

        #region _Parameters

        public IDictionary<string, Element_Parameter_> _Parameters
        {
            get { return this._parameters; }
        }

        #endregion

        #region _Reflected_Parameters

        public IDictionary<string, QS.Fx.Reflection.IParameter> _Reflected_Parameters
        {
            get { return this._reflected_parameters; }
        }

        #endregion

        #region Serialize

        public QS.Fx.Reflection.Xml.Parameter[] _Serialize()
        {
            List<QS.Fx.Reflection.Xml.Parameter> _xml_parameters = new List<QS.Fx.Reflection.Xml.Parameter>();
            if (this._parameters != null)
            {
                foreach (QS._qss_x_.ObjectDesigner_.Elements_.Element_Parameter_ _parameter in this._parameters.Values)
                    _xml_parameters.Add(_parameter._Serialize());
            }
            return _xml_parameters.ToArray();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Validate

        public override void _Validate()
        {
            lock (this)
            {
                this._correct = true;
                this._error = null;
                if (this._parameters != null)
                {
                    this._reflected_parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
                    StringBuilder _ss = null;
                    foreach (Element_Parameter_ _parameter in this._parameters.Values)
                    {
                        _parameter._Validate();
                        if (_parameter._Correct)
                        {
                            QS.Fx.Reflection.IParameter _reflected_parameter = _parameter._Reflected_Parameter;
                            this._reflected_parameters.Add(_reflected_parameter.ID, _reflected_parameter);
                        }
                        else
                        {
                            this._correct = false;
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine("Error in parameter \"" + _parameter._ID + "\".");
                        }
                    }
                    if (!this._correct)
                        this._reflected_parameters = null;
                    if (_ss != null)
                        this._error = _ss.ToString();
                }
                else
                    this._reflected_parameters = null;
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
