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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS.Fx.Inspection
{
	public class AttributesOf : AttributeCollection
	{
		public AttributesOf(object inspectedObject) : base()
		{
			System.Type objectType = inspectedObject.GetType();
			if (!objectType.IsClass)
				throw new ArgumentException("Must be a class.");

			this.name = "AttributesOf : " + objectType.FullName;

			bool inspectableClass = objectType.GetCustomAttributes(typeof(QS.Fx.Base.InspectableAttribute), true).Length > 0;

            List<System.Reflection.PropertyInfo> propertyInfos = new List<System.Reflection.PropertyInfo>();
            propertyInfos.AddRange(
                objectType.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance));
            Type thisType;
/*
            thisType = objectType;
            while (thisType.BaseType != null)
            {
                thisType = thisType.BaseType;
                foreach (System.Reflection.PropertyInfo propertyInfo in thisType.GetProperties(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (propertyInfo.)
                        propertyInfos.Add(propertyInfo);
                }
            }
*/

            foreach (System.Reflection.PropertyInfo propertyInfo in propertyInfos)
			{
                object[] inspectableAttributes = propertyInfo.GetCustomAttributes(typeof(QS.Fx.Base.InspectableAttribute), true);
                if (inspectableAttributes.Length > 0 && inspectableAttributes[0] is QS.Fx.Base.InspectableAttribute && propertyInfo.CanRead)
				{
                    QS.Fx.Base.InspectableAttribute inspectableAttribute = inspectableAttributes[0] as QS.Fx.Base.InspectableAttribute;

					string name = inspectableAttribute.Name;
					if (name == null)
						name = propertyInfo.Name;
					bool isReadOnly = !propertyInfo.CanWrite || inspectableAttribute.ReadOnly;

                    try
                    {
                        this.Add(wrap(propertyInfo.PropertyType, new PropertyWrapper(inspectedObject, propertyInfo, name, isReadOnly)));
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot add property \"" + propertyInfo.Name + "\" as \"" + name + "\".", exc);
                    }
				}
				else if (inspectableClass)
				{
                    try
                    {
                        this.Add(wrap(propertyInfo.PropertyType,
                            new PropertyWrapper(inspectedObject, propertyInfo, propertyInfo.Name, true)));
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot add property \"" + propertyInfo.Name + "\".", exc);
                    }
				}
			}

            List<System.Reflection.FieldInfo> fieldInfos = new List<System.Reflection.FieldInfo>();
            fieldInfos.AddRange(
                objectType.GetFields(
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic | 
                    System.Reflection.BindingFlags.Instance));
            thisType = objectType;
            while (thisType.BaseType != null)
            {
                thisType = thisType.BaseType;
                foreach (System.Reflection.FieldInfo fieldInfo in thisType.GetFields(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
                {
                    if (fieldInfo.IsPrivate)
                        fieldInfos.Add(fieldInfo);
                }
            }

			foreach (System.Reflection.FieldInfo fieldInfo in fieldInfos)
			{
                object[] inspectableAttributes = fieldInfo.GetCustomAttributes(typeof(QS.Fx.Base.InspectableAttribute), true);
                if (inspectableAttributes.Length > 0 && inspectableAttributes[0] is QS.Fx.Base.InspectableAttribute)
				{
                    QS.Fx.Base.InspectableAttribute inspectableAttribute = inspectableAttributes[0] as QS.Fx.Base.InspectableAttribute;

					string name = inspectableAttribute.Name;
					if (name == null)
						name = fieldInfo.Name;
                    if (!fieldInfo.DeclaringType.Equals(objectType))
                        name = "[" + fieldInfo.DeclaringType.Name + "] " + name;
					bool isReadOnly = fieldInfo.IsInitOnly || inspectableAttribute.ReadOnly;

					// force read-only
					isReadOnly = true;

                    try
                    {
					    this.Add(wrap(fieldInfo.FieldType, new FieldWrapper(inspectedObject, fieldInfo, name, isReadOnly)));
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot add field \"" + fieldInfo.Name + "\" as \"" + name + "\".", exc);
                    }
				}
				else if (inspectableClass)
				{
                    try
                    {
					    this.Add(wrap(fieldInfo.FieldType, new FieldWrapper(inspectedObject, fieldInfo, fieldInfo.Name, true)));
                    }
                    catch (Exception exc)
                    {
                        throw new Exception("Cannot add field \"" + fieldInfo.Name + "\".", exc);
                    }
                }
			}
		}

		private static IAttribute wrap(System.Type attributeType, IScalarAttribute attribute)
		{
			if (attributeType.IsArray)
			{
				return (IAttribute) new ArrayUnroller(attribute);
			}
			else if (typeof(System.Collections.ICollection).IsAssignableFrom(attributeType))
			{
				return (IAttribute) new CollectionUnroller(attribute);
			}
			else if (CollectionUnroller.IsGenericCollection(attributeType))
			{
				return (IAttribute)CollectionUnroller.UnrollGeneric(attributeType, attribute);
			}
			else if (DictionaryUnroller.InstantiatesToDictionary2(attributeType))
			{
				return (IAttribute)DictionaryUnroller.UnrollToDictionary2(attributeType, attribute);
			}
			else
			{
				return (IAttribute)attribute;
			}
		}
	}
}
