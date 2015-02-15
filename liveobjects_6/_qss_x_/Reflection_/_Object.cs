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

namespace QS._qss_x_.Reflection_
{
/*
    [QS.Fx.Printing.Printable("Object", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Object : QS.TMS.Inspection.Inspectable, IObject
    {
        #region Create(QS.Fx.Reflection.Xml.Object)

/-*
        public static Object Create(QS.Fx.Reflection.Xml.Object _xmlobject)
        {
            string _id = _xmlobject.ID;
            List<QS.Fx.Reflection.IEndpoint> _endpoints = new List<QS.Fx.Reflection.IEndpoint>();
            foreach (QS.Fx.Reflection.Xml.Endpoint _xmlendpoint in _xmlobject.Endpoints)
                _endpoints.Add(QS.Fx.Reflection.Endpoint.Create(_xmlendpoint));
            List<QS.Fx.Reflection.IComponent> _components = new List<QS.Fx.Reflection.IComponent>();
            foreach (QS.Fx.Reflection.Xml.Component _xmlcomponent in _xmlobject.Components)
                _components.Add(QS.Fx.Reflection.Component.Create(_xmlcomponent));
            return new Object(_id, _endpoints, _components);
        }
*-/

        #endregion

        #region Constructor

        public Object(string _id, IEnumerable<QS.Fx.Reflection.IEndpoint> _endpoints, IEnumerable<QS.Fx.Reflection.IComponent> _components)
        {
            this._id = _id;
            this._endpoints = new Dictionary<string, QS.Fx.Reflection.IEndpoint>();
            foreach (QS.Fx.Reflection.IEndpoint _endpoint in _endpoints)
                this._endpoints.Add(_endpoint.ID, _endpoint);
            this._components = new Dictionary<string, QS.Fx.Reflection.IComponent>();
            foreach (QS.Fx.Reflection.IComponent _component in _components)
                this._components.Add(_component.ID, _component);
        }

        #endregion

        #region Fields

        [QS.TMS.Inspection.Inspectable]
        [QS.Fx.Printing.Printable("id")]
        private string _id;
        [QS.Fx.Printing.Printable("endpoints", QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<string, QS.Fx.Reflection.IEndpoint> _endpoints;
        [QS.Fx.Printing.Printable("components", QS.Fx.Printing.PrintingStyle.Expanded)]
        private IDictionary<string, QS.Fx.Reflection.IComponent> _components;

        #endregion

        #region Inspection

        [QS.TMS.Inspection.Inspectable("_endpoints")]
        private QS.TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint> __inspectable_endpoints
        {
            get
            {
                return new TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint>("_endpoints", _endpoints,
                    new TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IEndpoint>.ConversionCallback(
                        delegate(string s) { return s; }));
            }
        }

        [QS.TMS.Inspection.Inspectable("_components")]
        private QS.TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IComponent> __inspectable_components
        {
            get
            {
                return new TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IComponent>("_components", _components,
                    new TMS.Inspection.DictionaryWrapper1<string, QS.Fx.Reflection.IComponent>.ConversionCallback(
                        delegate(string s) { return s; }));
            }
        }

        #endregion

        #region IObject Members

        string IObject.ID
        {
            get { return _id; }
        }

        IDictionary<string, QS.Fx.Reflection.IEndpoint> IObject.Endpoints
        {
            get { return new QS.Fx.Base.ReadonlyDictionaryOf<string, QS.Fx.Reflection.IEndpoint>(_endpoints);  }
        }

        IDictionary<string, QS.Fx.Reflection.IComponent> IObject.Components
        {
            get { return new QS.Fx.Base.ReadonlyDictionaryOf<string, QS.Fx.Reflection.IComponent>(_components); }
        }

        #endregion
    }
*/ 
}
