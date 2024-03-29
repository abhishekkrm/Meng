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
    public sealed class Element_InterfaceClass_ : Element_Class_
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Element_InterfaceClass_(
            string _id, QS.Fx.Attributes.IAttributes _attributes, Category_ _category, Element_Parameter_ _binding,
            Element_Environment_ _environment, bool _automatic, QS.Fx.Reflection.IInterfaceClass _template_interfaceclass)
            : base(_id, _attributes, _category, _binding, _environment, _automatic)
        {
            this._template_interfaceclass = _template_interfaceclass;
        }

        public Element_InterfaceClass_(Element_InterfaceClass_ _other) : base(_other)
        {
            this._template_interfaceclass = _other._template_interfaceclass;
        }

        #endregion

        #region Fields

        private QS.Fx.Reflection.IInterfaceClass _template_interfaceclass, _reflected_interfaceclass;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Reflected_ValueClass

        public QS.Fx.Reflection.IInterfaceClass _Reflected_InterfaceClass
        {
            get { return this._reflected_interfaceclass; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Rebuild

        public override void _Rebuild()
        {
            base._Rebuild();
            this.Text = "Interface" + this.Text;
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
                                    this._reflected_interfaceclass = this._template_interfaceclass.Instantiate(_reflected_parameters.Values);
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
                                if (this._binding._ParameterClass == QS.Fx.Reflection.ParameterClass.InterfaceClass)
                                {
                                    if (this._binding._Value != null)
                                    {
                                        if (this._binding._Value is Element_InterfaceClass_)
                                        {
                                            if (this._binding._Value._Correct)
                                            {
                                                this._reflected_interfaceclass = ((Element_InterfaceClass_)this._binding._Value)._Reflected_InterfaceClass;
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
                                            this._error = "Parameter \"" + this._binding._ID + "\" is not an interface class.";
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
                                    this._error = "Parameter \"" + this._binding._ID + "\" is not an interface class parameter.";
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

        #region Serialize

        public QS.Fx.Reflection.Xml.InterfaceClass _Serialize()
        {
            switch (this._category)
            {
                case Category_.Predefined_:
                    return new QS.Fx.Reflection.Xml.InterfaceClass(this._id,
                        (this._environment != null) ? this._environment._Serialize() : new QS.Fx.Reflection.Xml.Parameter[0]);
                case Category_.Parameter_:
                    return ((Element_InterfaceClass_)this._binding._Value)._Serialize();
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
