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

namespace QS._qss_x_.Reflection_.Internal_
{
    public sealed class _internal_object_proxy_map
    {
        #region Constructor

        public _internal_object_proxy_map(Type from, Type to)
        {
            this.from = from;
            this.to = to;
            IDictionary<string, System.Reflection.PropertyInfo> m1 = new Dictionary<string, System.Reflection.PropertyInfo>();
            foreach (Type interfacetype in to.GetInterfaces())
            {
                if (typeof(QS.Fx.Object.Classes.IObject).IsAssignableFrom(interfacetype))
                {
                    System.Reflection.PropertyInfo[] pp_ = interfacetype.GetProperties();
                    foreach (System.Reflection.PropertyInfo info in pp_)
                    {
                        object[] aa = info.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointAttribute), true);
                        if (aa.Length != 1)
                            throw new Exception("Property \"" + info.Name + "\" of type \"" + to.FullName +
                                "\" has not been decorated with \"" + typeof(QS.Fx.Reflection.EndpointAttribute).FullName + "\".");
                        QS.Fx.Reflection.EndpointAttribute a = (QS.Fx.Reflection.EndpointAttribute)aa[0];
                        string id = a.ID;
                        if (m1.ContainsKey(id))
                            throw new Exception("Cannot generate a mapping from type \"" + from.FullName + "\" to type \"" + to.FullName +
                                "\" because the target type inherits more than one property decorated with the \"" +
                                typeof(QS.Fx.Reflection.EndpointAttribute).FullName + "\" attribute for endpoint named \"" + id + "\".");
                        m1.Add(id, info);
                    }
                }
            }
            System.Reflection.PropertyInfo[] pp = from.GetProperties();
            this.mapping = new Dictionary<string, System.Reflection.PropertyInfo>(pp.Length);
            foreach (System.Reflection.PropertyInfo info in pp)
            {
                object[] aa = info.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointAttribute), true);
                if (aa.Length != 1)
                    throw new Exception("Property \"" + info.Name + "\" of type \"" + from.FullName + 
                        "\" has not been decorated with \"" + typeof(QS.Fx.Reflection.EndpointAttribute).FullName + "\".");
                QS.Fx.Reflection.EndpointAttribute a = (QS.Fx.Reflection.EndpointAttribute) aa[0];
                string id = a.ID;
                string key = info.GetGetMethod().Name;
                System.Reflection.PropertyInfo value;
                if (!m1.TryGetValue(id, out value))
                    throw new Exception("Cannot generate a mapping from object class \"" + from.FullName + 
                        "\" to object class \"" + to.FullName + "\" because the target doesn't define an endpoint named \"" + id + "\".");
                this.mapping.Add(key, value);
            }
        }

        #endregion

        #region Fields

        private Type from, to;
        private IDictionary<string, System.Reflection.PropertyInfo> mapping;

        #endregion

        #region _invoke

        public object _invoke(object o, string name)
        {
            System.Reflection.PropertyInfo info;
            if (!mapping.TryGetValue(name, out info))
                throw new Exception("Cannot invoke method \"" + name + "\" of type \"" + from.FullName +
                    "\" against type \"" + to.FullName + "\" because a suitable mapping has not been generated.");
            return info.GetValue(o, null);
        }

        #endregion
    }
}
