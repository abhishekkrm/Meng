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
using System.Reflection;

namespace QS._qss_c_.Logging_1_
{
    public class EventClass : QS.Fx.Logging.IEventClass
    {
        public EventClass(System.Type eventType)
        {
            if (!typeof(QS.Fx.Logging.IEvent).IsAssignableFrom(eventType))
                throw new Exception("The given event type does not inherit from IEvent.");

            object[] nameAttributes = eventType.GetCustomAttributes(typeof(EventNameAttribute), false);
            if (nameAttributes.Length > 0)
                name = ((EventNameAttribute)nameAttributes[0]).Name;
            else
                name = eventType.Name;

            foreach (FieldInfo fieldInfo in eventType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] propertyAttributes = fieldInfo.GetCustomAttributes(typeof(EventPropertyAttribute), true);
                if (propertyAttributes.Length > 0)
                {
                    string property_name = ((EventPropertyAttribute)propertyAttributes[0]).Name;
                    if (property_name == null)
                        property_name = fieldInfo.Name;

                    propertyGetters.Add(property_name, new FieldGetter(fieldInfo));
                }
            }

            foreach (PropertyInfo propertyInfo in eventType.GetProperties(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                object[] propertyAttributes = propertyInfo.GetCustomAttributes(typeof(EventPropertyAttribute), true);
                if (propertyAttributes.Length > 0)
                {
                    string property_name = ((EventPropertyAttribute)propertyAttributes[0]).Name;
                    if (property_name == null)
                        property_name = propertyInfo.Name;

                    propertyGetters.Add(property_name, new PropertyGetter(propertyInfo));
                }
            }
        }

        private delegate object GetValueCallback(QS.Fx.Logging.IEvent eventObject);

        private string name;
        private IDictionary<string, IGetter> propertyGetters = new Dictionary<string, IGetter>();

        #region Getter Classes

        private interface IGetter
        {
            object GetValue(object obj);
        }

        private class FieldGetter : IGetter
        {
            public FieldGetter(FieldInfo info)
            {
                this.info = info;
            }

            private FieldInfo info;

            #region IGetter Members

            object IGetter.GetValue(object obj)
            {
                return info.GetValue(obj);
            }

            #endregion
        }

        private class PropertyGetter : IGetter
        {
            public PropertyGetter(PropertyInfo info)
            {
                this.info = info;
            }

            private PropertyInfo info;

            #region IGetter Members

            object IGetter.GetValue(object obj)
            {
                return info.GetValue(obj, null);
            }

            #endregion
        }

        #endregion

        public object PropertyOf(QS.Fx.Logging.IEvent eventObject, string propertyName)
        {
            return propertyGetters[propertyName].GetValue(eventObject);
        }

        #region IEventClass Members

        string QS.Fx.Logging.IEventClass.Name
        {
            get { return name; }
        }

        IEnumerable<string> QS.Fx.Logging.IEventClass.Properties
        {
            get { return propertyGetters.Keys; }
        }

        #endregion
    }
}
