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

namespace QS._core_x_.Base
{
    public sealed class Parameters : QS.Fx.Base.IParameters
    {
        #region Constructor

        public Parameters()
        {
        }

        public Parameters(QS.Fx.Base.IParameter[] parameters)
        {
            foreach (QS.Fx.Base.IParameter parameter in parameters)
                this.parameters.Add(parameter.Name, parameter);
        }

        #endregion

        #region RegisterLocal

        public void RegisterLocal(object obj)
        {
            Type type = obj.GetType();
            
            foreach (System.Reflection.FieldInfo info in type.GetFields(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                object[] attributes = info.GetCustomAttributes(typeof(QS.Fx.Base.ParameterAttribute), true);
                if (attributes.Length > 0)
                {
                    QS.Fx.Base.ParameterAttribute attribute = (QS.Fx.Base.ParameterAttribute)(attributes[0]);
                    
                    string name = attribute.Name;
                    if (name == null)
                        name = info.Name;

                    bool readable = (attribute.Access & QS.Fx.Base.ParameterAccess.Readable) == QS.Fx.Base.ParameterAccess.Readable;
                    bool writable = (attribute.Access & QS.Fx.Base.ParameterAccess.Writable) == QS.Fx.Base.ParameterAccess.Writable;

                    parameters.Add(name, (QS.Fx.Base.IParameter) 
                        typeof(ParameterField<object>).GetGenericTypeDefinition().MakeGenericType(info.FieldType).GetConstructor(
                            new Type[] { typeof(string), typeof(object), typeof(System.Reflection.FieldInfo), typeof(bool), typeof(bool) }).Invoke(
                                new object[] { name, obj, info, readable, writable }));
                }
            }

            foreach (System.Reflection.PropertyInfo info in type.GetProperties(
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
            {
                object[] attributes = info.GetCustomAttributes(typeof(QS.Fx.Base.ParameterAttribute), true);
                if (attributes.Length > 0)
                {
                    QS.Fx.Base.ParameterAttribute attribute = (QS.Fx.Base.ParameterAttribute)(attributes[0]);

                    string name = attribute.Name;
                    if (name == null)
                        name = info.Name;

                    bool readable = (attribute.Access & QS.Fx.Base.ParameterAccess.Readable) == QS.Fx.Base.ParameterAccess.Readable;
                    bool writable = (attribute.Access & QS.Fx.Base.ParameterAccess.Writable) == QS.Fx.Base.ParameterAccess.Writable;

                    parameters.Add(name, (QS.Fx.Base.IParameter)
                        typeof(ParameterProperty<object>).GetGenericTypeDefinition().MakeGenericType(info.PropertyType).GetConstructor(
                            new Type[] { typeof(string), typeof(object), typeof(System.Reflection.PropertyInfo), typeof(bool), typeof(bool) }).Invoke(
                                new object[] { name, obj, info, readable, writable }));
                }
            }
        }

        #endregion

        #region Helpers

        public static C Get<C>(QS.Fx.Base.IParameters parameters, string name, C defaultValue)
        {
            QS.Fx.Base.IParameter<C> parameter;
            return (parameters.TryGet<C>(name, QS.Fx.Base.ParameterAccess.Readable, out parameter)) ? parameter.Value : defaultValue;
        }

        #endregion

        private IDictionary<string, QS.Fx.Base.IParameter> parameters = new Dictionary<string, QS.Fx.Base.IParameter>();

        #region IParametersInfo Members

        IEnumerable<QS.Fx.Base.IParameterInfo> QS.Fx.Base.IParametersInfo.Parameters
        {
            get 
            {
                List<QS.Fx.Base.IParameterInfo> infos = new List<QS.Fx.Base.IParameterInfo>();
                foreach (QS.Fx.Base.IParameter parameter in parameters.Values)
                    infos.Add(parameter);
                return infos; 
            }
        }

        #endregion

        #region IParameters Members

        IEnumerable<string> QS.Fx.Base.IParameters.Names
        {
            get { return parameters.Keys; }
        }

        IEnumerable<QS.Fx.Base.IParameter> QS.Fx.Base.IParameters.Parameters
        {
            get { return parameters.Values; }
        }

        QS.Fx.Base.IParameter QS.Fx.Base.IParameters.Get(string name)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
                return parameter;
            else
                throw new Exception("Cannot find parameter \"" + name + "\".");
        }

        QS.Fx.Base.IParameter QS.Fx.Base.IParameters.Get(string name, Type type)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                if (parameter.Type.Equals(type))
                    return parameter;
                else
                    throw new Exception("Type mismatch.");
            }
            else
                throw new Exception("Cannot find parameter \"" + name + "\".");
        }

        QS.Fx.Base.IParameter QS.Fx.Base.IParameters.Get(string name, Type type, QS.Fx.Base.ParameterAccess access)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                if (parameter.Type.Equals(type))
                {
                    if ((parameter.Readable || ((access & QS.Fx.Base.ParameterAccess.Readable) != QS.Fx.Base.ParameterAccess.Readable)) &&
                        (parameter.Writable || ((access & QS.Fx.Base.ParameterAccess.Writable) != QS.Fx.Base.ParameterAccess.Writable)))
                    {
                        return parameter;
                    }
                    else
                        throw new Exception("Access denied."); 
                }
                else
                    throw new Exception("Type mismatch.");
            }
            else
                throw new Exception("Cannot find parameter \"" + name + "\".");
        }

        bool QS.Fx.Base.IParameters.TryGet(string name, out QS.Fx.Base.IParameter parameter)
        {
            return parameters.TryGetValue(name, out parameter);
        }

        bool QS.Fx.Base.IParameters.TryGet(string name, Type type, out QS.Fx.Base.IParameter parameter)
        {
            if (parameters.TryGetValue(name, out parameter))
            {
                if (parameter.Type.Equals(type))
                    return true;
                else
                    parameter = null;
            }

            return false;
        }

        bool QS.Fx.Base.IParameters.TryGet(string name, Type type, QS.Fx.Base.ParameterAccess access, out QS.Fx.Base.IParameter parameter)
        {
            if (parameters.TryGetValue(name, out parameter))
            {
                if (parameter.Type.Equals(type) &&
                    (parameter.Readable || ((access & QS.Fx.Base.ParameterAccess.Readable) != QS.Fx.Base.ParameterAccess.Readable)) &&
                    (parameter.Writable || ((access & QS.Fx.Base.ParameterAccess.Writable) != QS.Fx.Base.ParameterAccess.Writable)))
                {
                    return true;
                }
                else
                    parameter = null;
            }

            return false;
        }

        QS.Fx.Base.IParameter<C> QS.Fx.Base.IParameters.Get<C>(string name)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                QS.Fx.Base.IParameter<C> parameter_C = parameter as QS.Fx.Base.IParameter<C>;
                if (parameter_C != null)
                    return parameter_C;
                else
                    throw new Exception("Parameter \"" + name + "\" is not of type " + typeof(C).FullName + ".");
            }
            else
                throw new Exception("Cannot find parameter \"" + name + "\".");
        }

        QS.Fx.Base.IParameter<C> QS.Fx.Base.IParameters.Get<C>(string name, QS.Fx.Base.ParameterAccess access)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                QS.Fx.Base.IParameter<C> parameter_C = parameter as QS.Fx.Base.IParameter<C>;
                if (parameter_C != null)
                {
                    if ((parameter_C.Readable || ((access & QS.Fx.Base.ParameterAccess.Readable) != QS.Fx.Base.ParameterAccess.Readable)) &&
                        (parameter_C.Writable || ((access & QS.Fx.Base.ParameterAccess.Writable) != QS.Fx.Base.ParameterAccess.Writable)))
                    {
                        return parameter_C;
                    }
                    else
                        throw new Exception("Access denied.");
                }
                else
                    throw new Exception("Parameter \"" + name + "\" is not of type " + typeof(C).FullName + ".");
            }
            else
                throw new Exception("Cannot find parameter \"" + name + "\".");
        }

        bool QS.Fx.Base.IParameters.TryGet<C>(string name, out QS.Fx.Base.IParameter<C> parameter_C)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                parameter_C = parameter as QS.Fx.Base.IParameter<C>;
                if (parameter_C != null)
                    return true;
            }

            parameter_C = null;
            return false;
        }

        bool QS.Fx.Base.IParameters.TryGet<C>(string name, QS.Fx.Base.ParameterAccess access, out QS.Fx.Base.IParameter<C> parameter_C)
        {
            QS.Fx.Base.IParameter parameter;
            if (parameters.TryGetValue(name, out parameter))
            {
                parameter_C = parameter as QS.Fx.Base.IParameter<C>;
                if (parameter_C != null)
                {
                    if ((parameter_C.Readable || ((access & QS.Fx.Base.ParameterAccess.Readable) != QS.Fx.Base.ParameterAccess.Readable)) &&
                        (parameter_C.Writable || ((access & QS.Fx.Base.ParameterAccess.Writable) != QS.Fx.Base.ParameterAccess.Writable)))
                    {
                        return true;
                    }
                }
            }

            parameter_C = null;
            return false;
        }

        #endregion

        #region Accessors

        public void Add(QS.Fx.Base.IParameter parameter)
        {
            parameters.Add(parameter.Name, parameter);
        }

        #endregion
    }
}
