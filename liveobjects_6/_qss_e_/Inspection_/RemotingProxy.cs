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

#define DEBUG_RemotingProxy

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_e_.Inspection_
{
    public abstract class RemotingProxy : QS.Fx.Inspection.IInspectable, QS.Fx.Inspection.IScalarAttribute
	{
		public RemotingProxy()
		{
		}

		protected abstract object MakeCall(QS._core_c_.Components.AttributeSet arguments);

		#region IInspectable Members

		private QS.Fx.Inspection.IAttributeCollection attributeCollection = null;
		private System.Object mylock = new Object();
		private QS.Fx.Inspection.IAttributeCollection AttributeCollection
		{
			get
			{
				lock (mylock)
				{
					if (attributeCollection == null)
						attributeCollection = this.CreateProxy(new string[] {}) as QS.Fx.Inspection.IAttributeCollection;
				}

				return attributeCollection;
			}
		}

		QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
		{
			get { return this.AttributeCollection; }
		}

		#endregion

		#region Creating Proxies and Communication

		private enum CallCategory
		{
			IDENTIFY_ATTRIBUTE, GET_ATTRIBUTE_NAMES, GET_SCALAR_ATTRIBUTE_VALUE
		}

		private AttributeProxy CreateProxy(string[] path)
		{
			QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet();
			arguments["path"] = path;
			arguments["callCategory"] = CallCategory.IDENTIFY_ATTRIBUTE;

			object res = this.MakeCall(arguments);
			if (res is QS._core_c_.Components.AttributeSet)
			{
				QS._core_c_.Components.AttributeSet result = res as QS._core_c_.Components.AttributeSet;

				string name = result["name"] as string;
				QS.Fx.Inspection.AttributeClass attributeClass = (QS.Fx.Inspection.AttributeClass)result["class"];

				return this.CreateProxy(path, name, attributeClass);
			}
			else
				throw new Exception("Cannot create a proxy: " + QS._core_c_.Helpers.ToString.ObjectRef(res));
		}

		private AttributeProxy CreateProxy(string[] path, string name, QS.Fx.Inspection.AttributeClass attributeClass)
		{
			switch (attributeClass)
			{
				case QS.Fx.Inspection.AttributeClass.COLLECTION:
					return new AttributeCollectionProxy(this, path, name);

				case QS.Fx.Inspection.AttributeClass.SCALAR:
					return new ScalarAttributeProxy(this, path, name);

				default:
					return null;
			}
		}

		private string[] GetAttributeNames(string[] path)
		{
			QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet();
			arguments["path"] = path;
			arguments["callCategory"] = CallCategory.GET_ATTRIBUTE_NAMES;

			return (string[])this.MakeCall(arguments);
		}

		private QS._core_c_.Components.AttributeSet GetScalarAttributeValue(string[] path)
		{
			QS._core_c_.Components.AttributeSet arguments = new QS._core_c_.Components.AttributeSet();
			arguments["path"] = path;
			arguments["callCategory"] = CallCategory.GET_SCALAR_ATTRIBUTE_VALUE;

			object result_object = this.MakeCall(arguments);

			if (result_object is Exception)
				throw new Exception("Cannot get value.", result_object as Exception);

			QS._core_c_.Components.AttributeSet result = result_object as QS._core_c_.Components.AttributeSet;
			return result;
		}

		#endregion

		#region Proxy Classes

		#region Class AttributeProxy

		private abstract class AttributeProxy : QS.Fx.Inspection.IAttribute
		{
			public AttributeProxy(RemotingProxy remotingProxy, string[] path, string name)
			{
				this.remotingProxy = remotingProxy;
				this.path = path;
				this.name = name;
			}

			protected RemotingProxy remotingProxy;
			protected string[] path;
			protected string name;

			#region IAttribute Members

			public string Name
			{
				get { return name; }
			}

			public abstract QS.Fx.Inspection.AttributeClass AttributeClass
			{
				get;
			}

			#endregion
		}

		#endregion

		#region Class ScalarAttributeProxy

		private class ScalarAttributeProxy : AttributeProxy, QS.Fx.Inspection.IScalarAttribute
		{
			public ScalarAttributeProxy(RemotingProxy remotingProxy, string[] path, string name) 
			: base(remotingProxy, path, name)
			{
			}

			#region IAttribute Members

			public override QS.Fx.Inspection.AttributeClass AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
			}

			#endregion

			#region IScalarAttribute Members

			public object Value
			{
				get
				{
					QS._core_c_.Components.AttributeSet result = remotingProxy.GetScalarAttributeValue(path);
					try
					{
						bool isAttribute = (bool)result["isAttribute"];
						if (isAttribute)
						{
							string[] newpath = new string[path.Length + 1];
							for (int ind = 0; ind < path.Length; ind++)
								newpath[ind] = path[ind];
							newpath[path.Length] = "value";

							return remotingProxy.CreateProxy(newpath, (string)result["name"], (QS.Fx.Inspection.AttributeClass)result["class"]);
						}
						else
						{
							bool isInspectable = (bool)result["isInspectable"];
							if (isInspectable)
							{
								string collectionName = (string)result["collectionName"];

								string[] newpath = new string[path.Length + 2];
								for (int ind = 0; ind < path.Length; ind++)
									newpath[ind] = path[ind];
								newpath[path.Length] = "value";
								newpath[path.Length + 1] = "collection";

								return new InspectableProxy(remotingProxy, newpath, collectionName);
							}
							else
								return result["scalarValue"];
						}
					}
					catch (Exception exc)
					{
						throw new Exception("Cannot load attribute " + name + " from " +
							QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(path, ".") +
							". Remote proxy returned " + result.ToString() + ".", exc);
					}
				}
			}

			#endregion
		}

		#endregion

		#region Class AttributeCollectionProxy

		private class AttributeCollectionProxy : AttributeProxy, QS.Fx.Inspection.IAttributeCollection
		{
			public AttributeCollectionProxy(RemotingProxy remotingProxy, string[] path, string name) 
			: base(remotingProxy, path, name)
			{
			}

			#region IAttributeCollection Members

			public System.Collections.Generic.IEnumerable<string> AttributeNames
			{
				get { return remotingProxy.GetAttributeNames(path); }
			}

			public QS.Fx.Inspection.IAttribute this[string attributeName]
			{
				get
				{
					string[] newpath = new string[path.Length + 1];
					for (int ind = 0; ind < path.Length; ind++)
						newpath[ind] = path[ind];
					newpath[path.Length] = attributeName;

					return remotingProxy.CreateProxy(newpath);
				}
			}

			#endregion

			#region IAttribute Members

			public override QS.Fx.Inspection.AttributeClass AttributeClass
			{
				get { return QS.Fx.Inspection.AttributeClass.COLLECTION; }
			}

			#endregion
		}

		#endregion

		#region Class InspectableProxy

		private class InspectableProxy : QS.Fx.Inspection.IInspectable
		{
			public InspectableProxy(RemotingProxy remotingProxy, string[] collectionPath, string collectionName)
			{
				this.collectionProxy = new AttributeCollectionProxy(remotingProxy, collectionPath, collectionName);
			}

			private AttributeCollectionProxy collectionProxy;

			#region IInspectable Members

			public QS.Fx.Inspection.IAttributeCollection Attributes
			{
				get { return collectionProxy; }
			}

			#endregion
		}

		#endregion

		#endregion

		#region Dispatching Call

		public static object DispatchCall(QS.Fx.Inspection.IInspectable inspectableObject, QS._core_c_.Components.AttributeSet arguments)
		{
			switch ((CallCategory)arguments["callCategory"])
			{
				case CallCategory.IDENTIFY_ATTRIBUTE:
					return IdentifyAttribute(inspectableObject, (string[])arguments["path"]);

				case CallCategory.GET_ATTRIBUTE_NAMES:
					return GetAttributeNames(inspectableObject, (string[])arguments["path"]);

				case CallCategory.GET_SCALAR_ATTRIBUTE_VALUE:
					return GetScalarAttributeValue(inspectableObject, (string[])arguments["path"]);

				default:
					throw new Exception("Unknown call type.");
			}
		}

		private static QS.Fx.Inspection.IAttribute GetAttribute(QS.Fx.Inspection.IInspectable inspectableObject, string[] path)
		{
			object attribute = inspectableObject.Attributes;
			foreach (string path_element in path)
			{
				if (attribute is QS.Fx.Inspection.IAttributeCollection)
					attribute = (attribute as QS.Fx.Inspection.IAttributeCollection)[path_element];
				else if (attribute is QS.Fx.Inspection.IScalarAttribute && path_element == "value")
					attribute = (attribute as QS.Fx.Inspection.IScalarAttribute).Value;
				else if (attribute is QS.Fx.Inspection.IInspectable && path_element == "collection")
					attribute = (attribute as QS.Fx.Inspection.IInspectable).Attributes;
				else
					throw new Exception("Path not found: Cannot find \"" + path_element + "\" in " + attribute.GetType().FullName);
			}

			if (attribute == null)
				throw new Exception("Attribute " + QS._core_c_.Helpers.CollectionHelper.ToStringSeparated<string>(path, ".") + " is null.");

			if (!(attribute is QS.Fx.Inspection.IAttribute))
				throw new Exception("Not an attribute");

			return attribute as QS.Fx.Inspection.IAttribute;
		}

		private static void IdentifyAttribute(QS.Fx.Inspection.IInspectable inspectableObject, string[] path, out string attributeName, out QS.Fx.Inspection.AttributeClass attributeClass)
		{
			QS.Fx.Inspection.IAttribute attribute = GetAttribute(inspectableObject, path);
			attributeName = attribute.Name;
			attributeClass = attribute.AttributeClass;
		}

		private static QS._core_c_.Components.AttributeSet IdentifyAttribute(QS.Fx.Inspection.IInspectable inspectableObject, string[] path)
		{
			string name;
			QS.Fx.Inspection.AttributeClass attributeClass;
			QS._qss_e_.Inspection_.RemotingProxy.IdentifyAttribute(inspectableObject, path, out name, out attributeClass);

			QS._core_c_.Components.AttributeSet response = new QS._core_c_.Components.AttributeSet(2);
			response["name"] = name;
			response["class"] = attributeClass;
			return response;
		}

		private static string[] GetAttributeNames(QS.Fx.Inspection.IInspectable inspectableObject, string[] path)
		{
			QS.Fx.Inspection.IAttributeCollection attributeCollection = (QS.Fx.Inspection.IAttributeCollection)GetAttribute(inspectableObject, path);
			System.Collections.Generic.List<string> names = new System.Collections.Generic.List<string>();
			foreach (string name in attributeCollection.AttributeNames)
				names.Add(name);
			return names.ToArray();
		}

		private static void GetScalarAttributeValue(QS.Fx.Inspection.IInspectable inspectableObject, string[] path, out bool isAttribute,
			out string attributeName, out QS.Fx.Inspection.AttributeClass attributeClass, out bool isInspectable, out string inspectableCollectionName,
			out object scalarValue)
		{
			QS.Fx.Inspection.IAttribute attribute = GetAttribute(inspectableObject, path);
			if (attribute == null)
				throw new Exception("Attribute not found.");

			if (attribute.AttributeClass == QS.Fx.Inspection.AttributeClass.SCALAR)
			{
				object attributeValue = (attribute as QS.Fx.Inspection.IScalarAttribute).Value;
				if (isAttribute = attributeValue is QS.Fx.Inspection.IAttribute)
				{
					QS.Fx.Inspection.IAttribute valueAsAttribute = attributeValue as QS.Fx.Inspection.IAttribute;
					attributeName = valueAsAttribute.Name;
					attributeClass = valueAsAttribute.AttributeClass;

					isInspectable = false;
					inspectableCollectionName = null;
					scalarValue = null;
				}
				else
				{
					attributeName = null;
					attributeClass = default(QS.Fx.Inspection.AttributeClass); // whatever

					if (isInspectable = attributeValue is QS.Fx.Inspection.IInspectable)
					{
						QS.Fx.Inspection.IAttributeCollection inspectableCollection = (attributeValue as QS.Fx.Inspection.IInspectable).Attributes;
						inspectableCollectionName = inspectableCollection.Name;

						scalarValue = null;
					}
					else
					{
						scalarValue = (attributeValue != null)
							? ((attributeValue.GetType().IsSerializable && (attributeValue.GetType().GetCustomAttributes(typeof(Inspection_.AsStringAttribute), false).Length == 0))
								? attributeValue : (attributeValue.GetType().FullName + " : " + attributeValue.ToString())) : "(null)";

						inspectableCollectionName = null;
					}
				}
			}
			else
			{
				throw new Exception("This is not a scalar attribute.");
			}
		}

		private static QS._core_c_.Components.AttributeSet GetScalarAttributeValue(QS.Fx.Inspection.IInspectable inspectableObject, string[] path)
		{
			try
			{
				bool isAttribute, isInspectable;
				string attributeName, inspectableCollectionName;
				object scalarValue;
				QS.Fx.Inspection.AttributeClass attributeClass;

				QS._qss_e_.Inspection_.RemotingProxy.GetScalarAttributeValue(inspectableObject, path, out isAttribute, 
					out attributeName, out attributeClass, out isInspectable, out inspectableCollectionName, out scalarValue);

				QS._core_c_.Components.AttributeSet response = new QS._core_c_.Components.AttributeSet(3);
				response["isAttribute"] = isAttribute;
				if (isAttribute)
				{
					response["name"] = attributeName;
					response["class"] = attributeClass;
				}
				else
				{
					response["isInspectable"] = isInspectable;
					if (isInspectable)
						response["collectionName"] = inspectableCollectionName;
					else
						response["scalarValue"] = scalarValue;
				}

				return response;
			}
			catch (Exception)
			{
				return new QS._core_c_.Components.AttributeSet("Error", "This is not a scalar attribute.");
			}
		}

		#endregion

		#region IScalarAttribute Members

		object QS.Fx.Inspection.IScalarAttribute.Value
		{
			get { return this.AttributeCollection; }
		}

		#endregion

		#region IAttribute Members

        string QS.Fx.Inspection.IAttribute.Name
		{
			get { return "RemotingProxy: " + this.GetType().FullName; }
		}

        QS.Fx.Inspection.AttributeClass QS.Fx.Inspection.IAttribute.AttributeClass
		{
			get { return QS.Fx.Inspection.AttributeClass.SCALAR; }
		}

		#endregion
	}
}
