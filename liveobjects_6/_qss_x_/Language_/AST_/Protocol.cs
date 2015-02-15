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

namespace QS._qss_x_.Language_.AST_
{
    [XmlType("Protocol")]
    [XmlInclude(typeof(Aggregation))]
    [XmlInclude(typeof(Dissemination))]
    [XmlInclude(typeof(Gossip))]
    public sealed class Protocol
    {
        #region Constructors

        public Protocol(
            string _name, Interface _interface, List<Property> _properties, List<Binding> _bindings, List<Flow> _flows,
            List<Rule> _rules, List<Condition> _conditions)
        {
            this._name = _name;
            this._interface = _interface;
            this._properties = _properties;
            this._bindings = _bindings;
            this._flows = _flows;
            this._rules = _rules;
            this._conditions = _conditions;
        }

        public Protocol()
        {
        }

        #endregion

        #region Fields

        private string _name;
        private Interface _interface;
        private List<Property> _properties;
        private List<Binding> _bindings;
        private List<Flow> _flows;
        private List<Rule> _rules;
        private List<Condition> _conditions;

        #endregion

        #region Accessors

        [XmlAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [XmlElement("Interface")]
        public Interface Interface
        {
            get { return _interface; }
            set { _interface = value; }
        }

        [XmlElement("Property")]
        public Property[] Properties
        {
            get { return (_properties != null) ? _properties.ToArray() : null; }
            set { _properties = (value != null) ? new List<Property>(value) : new List<Property>(); }
        }

/*
        [XmlIgnore]
        public List<Property> Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }
*/

        [XmlElement("Binding")]
        public Binding[] Bindings
        {
            get { return (_bindings != null) ? _bindings.ToArray() : null; }
            set { _bindings = (value != null) ? new List<Binding>(value) : new List<Binding>(); }
        }

/*
        [XmlIgnore]
        public List<Binding> Bindings
        {
            get { return _bindings; }
            set { _bindings = value; }
        }
*/

        [XmlElement("Flow")]
        public Flow[] Flows
        {
            get { return (_flows != null) ? _flows.ToArray() : null; }
            set { _flows = (value != null) ? new List<Flow>(value) : new List<Flow>(); }
        }

/*
        [XmlIgnore]
        public List<Flow> Flows
        {
            get { return _flows; }
            set { _flows = value; }
        }
*/

        [XmlElement("Rule")]
        public Rule[] Rules
        {
            get { return (_rules != null) ? _rules.ToArray() : null; }
            set { _rules = (value != null) ? new List<Rule>(value) : new List<Rule>(); }
        }

/*
        [XmlIgnore]
        public List<Rule> Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }
*/

        [XmlElement("Condition")]
        public Condition[] Conditions
        {
            get { return (_conditions != null) ? _conditions.ToArray() : null; }
            set { _conditions = (value != null) ? new List<Condition>(value) : new List<Condition>(); }
        }

/*
        [XmlIgnore]
        public List<Condition> Conditions
        {
            get { return _conditions; }
            set { _conditions = value; }
        }
*/

        #endregion

/*
        #region Overridden from System.Object

        public override bool Equals(object obj)
        {
            if (obj is Protocol)
            {
                Protocol other = (Protocol) obj;
                return _name.Equals(other._name)
                    && _interface.Equals(other._interface)
                    && _properties.Equals(other._properties)
                    && _bindings.Equals(other._bindings)
                    && _flows.Equals(other._flows)
                    && _rules.Equals(other._rules)
                    && _conditions.Equals(other._conditions);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _name.GetHashCode() ^ _interface.GetHashCode() ^ _properties.GetHashCode() ^
                _bindings.GetHashCode() ^ _flows.GetHashCode() ^ _rules.GetHashCode() ^ _conditions.GetHashCode();
        }

        #endregion
*/ 
    }
}
