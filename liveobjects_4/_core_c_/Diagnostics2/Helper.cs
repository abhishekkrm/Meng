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

namespace QS._core_c_.Diagnostics2
{
    public static class Helper
    {
        public static void RegisterLocal(IContainer container, object target)
        {
            Type type = target.GetType();

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] markings = field.GetCustomAttributes(typeof(QS._core_c_.Diagnostics2.PropertyAttribute), true);
                if (markings.Length > 0)                            
                {
                    string name = ((QS._core_c_.Diagnostics2.PropertyAttribute)markings[0]).Name;
                    if (name == null)
                        name = field.Name;
                    container.Register(name, new QS._core_c_.Diagnostics2.Property(target, field));                    
                }

                markings = field.GetCustomAttributes(typeof(QS._core_c_.Diagnostics2.ModuleAttribute), true);
                if (markings.Length > 0)
                {
                    string name = ((QS._core_c_.Diagnostics2.ModuleAttribute)markings[0]).Name;
                    if (name == null)
                        name = field.Name;

                    Object oo = field.GetValue(target);
                    if (oo != null)
                    {
                        QS._core_c_.Diagnostics2.IModule o = oo as QS._core_c_.Diagnostics2.IModule;
                        if (o != null)
                        {
                            container.Register(name, o.Component);
                        }
                        else
                            throw new Exception("Field \"" + field.Name + "\" is marked as module, but it is not a module.");
                    }
                }
            }

            foreach (PropertyInfo propertyinfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] markings = propertyinfo.GetCustomAttributes(typeof(QS._core_c_.Diagnostics2.PropertyAttribute), true);
                if (markings.Length > 0)
                {
                    string name = ((QS._core_c_.Diagnostics2.PropertyAttribute)markings[0]).Name;
                    if (name == null)
                        name = propertyinfo.Name;
                    container.Register(name, new QS._core_c_.Diagnostics2.Property(target, propertyinfo));                    
                }

                markings = propertyinfo.GetCustomAttributes(typeof(QS._core_c_.Diagnostics2.ModuleAttribute), true);
                if (markings.Length > 0)
                {
                    string name = ((QS._core_c_.Diagnostics2.ModuleAttribute)markings[0]).Name;
                    if (name == null)
                        name = propertyinfo.Name;
                    QS._core_c_.Diagnostics2.IModule o = propertyinfo.GetValue(target, new object[] { }) as QS._core_c_.Diagnostics2.IModule;

                    if (o != null)
                    {
                        container.Register(name, o.Component);
                    }
                    else
                        throw new Exception("Property \"" + propertyinfo.Name + "\" is marked as module, but it is not a module.");
                }
            }
        }

        public static QS._core_c_.Components.AttributeSet Collect(IContainer container)
        {
            QS._core_c_.Components.AttributeSet attributes = new QS._core_c_.Components.AttributeSet();
            foreach (IComponent component in container.Subcomponents)
            {
                object value = null;
                switch (component.Class)
                {
                    case ComponentClass.Property:
                        {
                            value = ((IProperty)component).Value;
                            if (value != null)
                            {
                                if (value is QS._core_c_.Statistics.IFileOutput)
                                {
                                    // leave as is
                                }
                                else
                                {
                                    if (value.GetType().IsArray)
                                    {
                                        QS._core_c_.Components.AttributeSet elements = new QS._core_c_.Components.AttributeSet();
                                        int ind = 0;
                                        foreach (object o in ((Array)value))
                                            elements.Add(new QS._core_c_.Components.Attribute("(" + (++ind).ToString("000000") + ")", o));
                                        value = elements;
                                    }
                                    else if (value is QS._core_c_.Diagnostics.IDataCollector)
                                        value = ((QS._core_c_.Diagnostics.IDataCollector)value).DataSet;
                                }
                            }
                        }
                        break;

                    case ComponentClass.Container:
                        {
                            value = Collect((IContainer)component);
                        }
                        break;
                }

                if (value != null)
                    attributes.Add(new QS._core_c_.Components.Attribute(component.Name, value));
            }
            return attributes;
        }
    }
}
