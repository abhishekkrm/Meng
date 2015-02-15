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

namespace QS._qss_e_.Parameters_.Specifications
{
    [Serializable]
    [XmlType("collection")]
    [XmlInclude(typeof(Parameter))]
    [XmlInclude(typeof(ParameterCollection))]
    public sealed class ParameterCollection : AbstractParameter
    {
        public ParameterCollection()
        {
        }

        public ParameterCollection(string name, string description, bool required, params AbstractParameter[] subparameters) 
            : base(name, description, required)
        {
            parameters = new Dictionary<string, AbstractParameter>(subparameters.Length);
            foreach (AbstractParameter parameter in subparameters)
                parameters.Add(parameter.Name, parameter);
        }

        private IDictionary<string, AbstractParameter> parameters;

        [XmlElement("item")]
        public AbstractParameter[] Parameters
        {
            get 
            { 
                ICollection<AbstractParameter> parameters = this.parameters.Values;
                AbstractParameter[] parametersAsArray = new AbstractParameter[parameters.Count];
                parameters.CopyTo(parametersAsArray, 0);
                return parametersAsArray; 
            }
            
            set 
            {
                parameters = new Dictionary<string, AbstractParameter>(value.Length);
                foreach (AbstractParameter parameter in value)
                    parameters.Add(parameter.Name, parameter);
            }
        }

        public override ParameterType Type
        {
            get { return ParameterType.Collection; }
        }
    }
}
