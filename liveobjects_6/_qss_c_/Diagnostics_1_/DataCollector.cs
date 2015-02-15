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

namespace QS._qss_c_.Diagnostics_1_
{
    public static class DataCollector
    {
        public static QS._core_c_.Components.AttributeSet Collect(object dataObject)
        {
            QS._core_c_.Components.AttributeSet data = new QS._core_c_.Components.AttributeSet();
            _Collect1(data, dataObject, new System.Collections.ObjectModel.Collection<object>());
            return data;
        }

        private static void _Collect1(QS._core_c_.Components.AttributeSet data, object dataObject, ICollection<object> visitedObjects)
        {
            if (dataObject != null)
            {
                visitedObjects.Add(dataObject);

                if (dataObject.GetType().GetCustomAttributes(typeof(QS._core_c_.Diagnostics.IgnoreAttribute), false).Length == 0)
                {
                    if (dataObject is QS._core_c_.Diagnostics.IDataCollector)
                    {
                        data.Add(new QS._core_c_.Components.Attribute("this", ((QS._core_c_.Diagnostics.IDataCollector)dataObject).DataSet));
                    }
                    else
                    {
                        object[] containerAttribs = dataObject.GetType().GetCustomAttributes(typeof(QS._core_c_.Diagnostics.ComponentContainerAttribute), true);
                        QS.Fx.Diagnostics.SelectionOption selectionOption = (containerAttribs.Length > 0) ?
                            ((QS._core_c_.Diagnostics.ComponentContainerAttribute)containerAttribs[0]).SelectionOption : QS.Fx.Diagnostics.SelectionOption.Explicit;

                        switch (selectionOption)
                        {
                            case QS.Fx.Diagnostics.SelectionOption.Inspectable:
                                {
                                    /*
                                                                TMS.Inspection.IAttributeCollection attributeCollection;
                                                                if (dataObject is TMS.Inspection.IAttributeCollection)
                                                                    attributeCollection = ((TMS.Inspection.IAttributeCollection)dataObject);
                                                                else
                                                                    if (dataObject is TMS.Inspection.IInspectable)
                                                                        attributeCollection = ((TMS.Inspection.IInspectable)dataObject).Attributes;
                                                                    else
                                                                        attributeCollection = null;

                                                                if (attributeCollection != null)
                                                                {
                                                                    foreach (string name in attributeCollection.AttributeNames)
                                                                    {
                                                                        TMS.Inspection.IAttribute attribute = attributeCollection[name];


                                                                    }
                                                                }
                                    */
                                }
                                break;

                            case QS.Fx.Diagnostics.SelectionOption.Explicit:
                            case QS.Fx.Diagnostics.SelectionOption.Implicit:
                                {
                                    List<Member> members = new List<Member>();

                                    Type type = dataObject.GetType();
                                    FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                    foreach (FieldInfo fieldInfo in fields)
                                        members.Add(new Member(fieldInfo));
                                    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                                    foreach (PropertyInfo propertyInfo in properties)
                                        members.Add(new Member(propertyInfo));

                                    foreach (Member member in members)
                                    {
                                        string name = null;
                                        bool ignore = member.MemberInfo.GetCustomAttributes(typeof(QS._core_c_.Diagnostics.IgnoreAttribute), true).Length > 0;
                                        if (!ignore)
                                        {
                                            object[] attributeObjects = member.MemberInfo.GetCustomAttributes(typeof(QS._core_c_.Diagnostics.ComponentAttribute), true);
                                            if (attributeObjects.Length > 0)
                                            {
                                                name = (attributeObjects[0] as QS._core_c_.Diagnostics.ComponentAttribute).Name;
                                            }
                                            else
                                            {
                                                object[] objs = member.MemberInfo.GetCustomAttributes(typeof(QS._core_c_.Diagnostics.ComponentCollectionAttribute), true);
                                                if (objs.Length > 0)
                                                {
                                                    name = (objs[0] as QS._core_c_.Diagnostics.ComponentCollectionAttribute).Name;
                                                    if (name == null)
                                                        name = member.MemberInfo.Name;

                                                    object value = member.GetValueCallback(dataObject);
                                                    QS._core_c_.Components.AttributeSet children = new QS._core_c_.Components.AttributeSet();
                                                    data.Add(new QS._core_c_.Components.Attribute(name, children));
                                                    visitedObjects.Add(value);

                                                    _Collect3(children, value, visitedObjects, (objs[0] as QS._core_c_.Diagnostics.ComponentCollectionAttribute));

                                                    ignore = true;
                                                }
                                                else
                                                {
                                                    ignore = (selectionOption == QS.Fx.Diagnostics.SelectionOption.Explicit)
                                                        && !(typeof(QS._core_c_.Diagnostics.IDataCollector).IsAssignableFrom(member.MemberInfo.ReflectedType));
                                                }
                                            }
                                        }

                                        if (!ignore)
                                        {
                                            if (name == null)
                                                name = member.MemberInfo.Name;

                                            object value = member.GetValueCallback(dataObject);
                                            _Collect2(data, name, value, visitedObjects);
                                        }
                                        else
                                        {
                                            // data.Add(new QS.CMS.Components.Attribute("$" + member.MemberInfo.Name, "_"));
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }

        private static void _Collect2(QS._core_c_.Components.AttributeSet data, string name, object value, ICollection<object> visitedObjects)
        {
            if (value != null)
            {
                bool alreadySeen = false;
                foreach (object obj in visitedObjects)
                {
                    if (ReferenceEquals(obj, value))
                    {
                        alreadySeen = true;
                        break;
                    }
                }

                if (!alreadySeen)
                {
                    if (value is QS._core_c_.Diagnostics.IDataCollector)
                    {
                        QS._core_c_.Diagnostics.IDataCollector dataCollector = (QS._core_c_.Diagnostics.IDataCollector)value;
                        data.Add(new QS._core_c_.Components.Attribute(name, dataCollector.DataSet));

                        visitedObjects.Add(value);
                    }
                    else if (value is QS._core_c_.Diagnostics.IComponentContainer)
                    {
                        QS._core_c_.Components.AttributeSet children = new QS._core_c_.Components.AttributeSet();
                        data.Add(new QS._core_c_.Components.Attribute(name, children));
                        visitedObjects.Add(value);

                        foreach (KeyValuePair<string, QS.Fx.Diagnostics.IDiagnosticsComponent> element in ((QS._core_c_.Diagnostics.IComponentContainer)value))
                        {
                            QS._core_c_.Components.AttributeSet children2 = new QS._core_c_.Components.AttributeSet();
                            children.Add(new QS._core_c_.Components.Attribute(element.Key, children2));

                            _Collect1(children2, element.Value, visitedObjects);
                        }
                    }
                    else
                    {
                        QS._core_c_.Components.AttributeSet children = new QS._core_c_.Components.AttributeSet();
                        data.Add(new QS._core_c_.Components.Attribute(name, children));

                        _Collect1(children, value, visitedObjects);
                    }
                }
            }
        }

        private static void _Collect3(
            QS._core_c_.Components.AttributeSet data, object value, ICollection<object> visitedObjects, QS._core_c_.Diagnostics.ComponentCollectionAttribute attribute)
        {
            if (value is System.Collections.IDictionary)
            {
                foreach (System.Collections.DictionaryEntry element in ((System.Collections.IDictionary)value))
                {
                    QS._core_c_.Components.AttributeSet children = new QS._core_c_.Components.AttributeSet();
                    data.Add(new QS._core_c_.Components.Attribute(element.Key.ToString(), children));

                    _Collect1(children, element.Value, visitedObjects);
                }
            }
            else if (value is System.Collections.ICollection)
            {
                int index = 0;
                foreach (object element in ((System.Collections.ICollection)value))
                {
                    string key = "(" + index.ToString("00000000") + ")";

                    if (element is QS._core_c_.Diagnostics.IDataCollector)
                    {
                        visitedObjects.Add(element);
                        data.Add(new QS._core_c_.Components.Attribute(key, ((QS._core_c_.Diagnostics.IDataCollector)element).DataSet));
                    }
                    else
                    {
                        QS._core_c_.Components.AttributeSet children = new QS._core_c_.Components.AttributeSet();
                        data.Add(new QS._core_c_.Components.Attribute(key, children));

                        _Collect1(children, element, visitedObjects);
                    }
                    index++;
                }
            }
            else
            {
                throw new NotSupportedException("This collection type is not supported.");
            }
        }

/*
        private void _CollectInspectable(QS.CMS.Components.AttributeSet data, object value, IList<object> visitedObjects)
        {
        }
*/ 
    }
}
