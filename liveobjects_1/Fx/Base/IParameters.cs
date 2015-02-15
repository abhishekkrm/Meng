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

namespace QS.Fx.Base
{
    /// <summary>
    /// Represents a list of configuration parameters of the specific module.
    /// </summary>
    public interface IParameters : IParametersInfo
    {
        /// <summary>
        /// The list of parameter names.
        /// </summary>
        IEnumerable<string> Names
        {
            get;
        }

        /// <summary>
        /// The list of parameters.
        /// </summary>
        new IEnumerable<IParameter> Parameters
        {
            get;
        }

        /// <summary>
        /// Returns a configuration parameter with the given name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <returns>The parameter.</returns>
        IParameter Get(string name);

        /// <summary>
        /// Returns a configuration parameter with the given name and data type.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter data type.</param>
        /// <returns>The parameter.</returns>
        IParameter Get(string name, Type type);

        /// <summary>
        /// Returns a configuration parameter with the given name, data type and access.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter type.</param>
        /// <param name="access">Parameter access.</param>
        /// <returns>The parameter.</returns>
        IParameter Get(string name, Type type, ParameterAccess access);

        /// <summary>
        /// Returns a configuration parameter with the given name.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="parameter">The returned parameter.</param>
        /// <returns>True if a parameter was returned, false if a parameter with the requested name could not be found.</returns>
        bool TryGet(string name, out IParameter parameter);

        /// <summary>
        /// Returns a configuration parameter with the given name and data type.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter data type.</param>
        /// <param name="parameter">The returned parameter.</param>
        /// <returns>True if a parameter was returned, false if a parameter with the requested name and data type could not be found.</returns>
        bool TryGet(string name, Type type, out IParameter parameter);

        /// <summary>
        /// Returns a configuration parameter with the given name, data type and access.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="type">Parameter data type.</param>
        /// <param name="access">Parameter access.</param>
        /// <param name="parameter">The returned parameter.</param>
        /// <returns>True if a parameter was returned, false if a parameter with the requested name, data type 
        /// and access mode could not be found.</returns>
        bool TryGet(string name, Type type, ParameterAccess access, out IParameter parameter);

        /// <summary>
        /// Returns a configuration parameter with the given name and data type.
        /// </summary>
        /// <typeparam name="C">Parameter data type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <returns>The parameter.</returns>
        IParameter<C> Get<C>(string name);

        /// <summary>
        /// Returns a configuration parameter with the given name, data type and access.
        /// </summary>
        /// <typeparam name="C">Parameter data type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="access">Parameter access.</param>
        /// <returns>The parameter.</returns>
        IParameter<C> Get<C>(string name, ParameterAccess access);

        /// <summary>
        /// Returns a configuration parameter with the given name and data type.
        /// </summary>
        /// <typeparam name="C">Parameter data type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="parameter">The returned parameter.</param>
        /// <returns>True if a parameter was returned, false if a parameter with the requested name and data type could not be found.</returns>
        bool TryGet<C>(string name, out IParameter<C> parameter);

        /// <summary>
        /// Returns a configuration parameter with the given name, data type and access.
        /// </summary>
        /// <typeparam name="C">Parameter data type.</typeparam>
        /// <param name="name">Parameter name.</param>
        /// <param name="access">Parameter access.</param>
        /// <param name="parameter">The returned parameter.</param>
        /// <returns>True if a parameter was returned, false if a parameter with the requested name, data type 
        /// and access mode could not be found.</returns>
        bool TryGet<C>(string name, ParameterAccess access, out IParameter<C> parameter);
    }
}
