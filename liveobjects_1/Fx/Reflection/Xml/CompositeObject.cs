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
using System.Xml.Serialization;

namespace QS.Fx.Reflection.Xml
{
    [QS.Fx.Printing.Printable("CompositeObject", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [XmlInclude(typeof(ReferenceObject))]
    [XmlInclude(typeof(CompositeObject))]
    [XmlType(TypeName = "CompositeObject")]
    public sealed class CompositeObject : Object
    {
        #region Constructors

        public CompositeObject(string _id, Attribute[] _attributes, ObjectClass _objectclass, Object _authority, 
            Parameter[] _parameters, Component[] _components, Endpoint[] _endpoints, Connection[] _connections)
            : base(_id, _attributes, _objectclass, _authority, _parameters)
        {
            this._components = _components;
            this._endpoints = _endpoints;
            this._connections = _connections;
        }

        public CompositeObject() : base()
        {
        }

        #endregion

        #region Fields

        private Component[] _components;
        private Endpoint[] _endpoints;
        private Connection[] _connections;

        #endregion

        #region Accessors

        [XmlElement("Component")]
        public Component[] Components
        {
            get { return _components; }
            set { _components = value; }
        }

        [XmlElement("Endpoint")]
        public Endpoint[] Endpoints
        {
            get { return _endpoints; }
            set { _endpoints = value; }
        }

        [XmlElement("Connection")]
        public Connection[] Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }

        #endregion

        #region Class Component

        [QS.Fx.Printing.Printable("Component", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [XmlType(TypeName = "Component")]
        public sealed class Component
        {
            #region Component

            public Component(string _id, Object _object)
            {
                this._id = _id;
                this._object = _object;
            }

            public Component()
            {
            }

            #endregion

            #region Fields

            private string _id;
            private Object _object;

            #endregion

            #region Accessors

            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [XmlElement("Object")]
            public Object Object
            {
                get { return _object; }
                set { _object = value; }
            }

            #endregion
        }

        #endregion

        #region Class Endpoint

        [QS.Fx.Printing.Printable("Endpoint", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
        [XmlType(TypeName = "Endpoint")]
        public sealed class Endpoint
        {
            #region Endpoint

            public Endpoint(string _id, string _from)
            {
                this._id = _id;
                this._from = _from;
            }

            public Endpoint()
            {
            }

            #endregion

            #region Fields

            private string _id, _from;

            #endregion

            #region Accessors

            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            [XmlAttribute("from")]
            public string From
            {
                get { return _from; }
                set { _from = value; }
            }

            #endregion
        }

        #endregion

        #region Class Connection

        [XmlType(TypeName = "Connection")]
        public sealed class Connection
        {
            public Connection(string _from, string _to)
            {
                this._from = _from;
                this._to = _to;
            }

            public Connection()
            {
            }

            private string _from, _to;

            [XmlAttribute("from")]
            public string From
            {
                get { return _from; }
                set { _from = value; }
            }

            [XmlAttribute("to")]
            public string To
            {
                get { return _to; }
                set { _to = value; }
            }
        }

        #endregion
    }
}
