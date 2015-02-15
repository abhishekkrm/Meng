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

namespace QS.Fx.Configuration
{
    [QS.Fx.Printing.Printable("Parameters", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [XmlType("Configuration")]
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.Configuration)]
    public sealed class Configuration : IConfiguration
    {
        public Configuration(IEnumerable<Parameter> _parameters)
        {
            this._parameters = new Dictionary<string, Parameter>();
            foreach (Parameter p in _parameters)
                this._parameters.Add(p.Name, p);
        }

        public Configuration()
        {
        }

        private Dictionary<string, Parameter> _parameters;

        [XmlElement("Parameter")]
        public Parameter[] Parameters
        {
            get 
            {
                lock (this)
                {
                    if (_parameters != null)
                    {
                        Parameter[] _pp = new Parameter[_parameters.Count];
                        _parameters.Values.CopyTo(_pp, 0);
                        return _pp;
                    }
                    else
                        return null;
                }
            }

            set
            {
                lock (this)
                {
                    if (value != null)
                    {
                        _parameters = new Dictionary<string, Parameter>(value.Length);
                        foreach (Parameter _p in value)
                            _parameters.Add(((IParameter)_p).Name, _p);
                    }
                    else
                        _parameters = new Dictionary<string, Parameter>(0);
                }
            }
        }

        #region IConfiguration Members

        bool IConfiguration.ContainsParameter(string _name)
        {
            lock (this)
            {
                return _parameters.ContainsKey(_name);
            }
        }

        IParameter IConfiguration.GetParameter(string _name)
        {
            lock (this)
            {
                return _parameters[_name];
            }
        }

        bool IConfiguration.TryGetParameter(string _name, out IParameter _parameter)
        {
            lock (this)
            {
                Parameter _o;
                if (_parameters.TryGetValue(_name, out _o))
                {
                    _parameter = _o;
                    return true;
                }
                else
                {
                    _parameter = null;
                    return false;
                }
            }
        }

        [XmlIgnore]
        IEnumerable<IParameter> IConfiguration.Parameters
        {
            get 
            {
                lock (this)
                {
                    List<IParameter> pp = new List<IParameter>();
                    foreach (Parameter p in _parameters.Values)
                        pp.Add(p);
                    return pp;
                }
            }
        }

        IConfiguration IConfiguration.GetConfiguration(string _namespace)
        {
            lock (this)
            {
                string e = _namespace + ".";
                List<Parameter> pp = new List<Parameter>();
                foreach (Parameter p in _parameters.Values)
                {
                    if (p.Name.StartsWith(e))
                        pp.Add(new Parameter(p.Name.Substring(e.Length), p.Value));
                }
                return new Configuration(pp);
            }
        }

        #endregion
    }
}
