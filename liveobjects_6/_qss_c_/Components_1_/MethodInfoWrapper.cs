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

namespace QS._qss_c_.Components_1_
{
	/// <summary>
	/// Summary description for MethodInfoWrapper.
	/// </summary>
	[Serializable]
	public class MethodInfoWrapper
	{
		public MethodInfoWrapper()
		{
		}

		public MethodInfoWrapper(System.Reflection.MethodInfo methodInfo)
		{
            if (methodInfo == null)
                throw new Exception("MethodInfo is NULL");

			objectTypeWrapper = new TypeWrapper(methodInfo.ReflectedType);
			methodName = methodInfo.Name;
			System.Reflection.ParameterInfo[] parameters = methodInfo.GetParameters();
			argumentTypeWrappers = new TypeWrapper[parameters.Length];
			for (uint ind = 0; ind < parameters.Length; ind++)
				argumentTypeWrappers[ind] = new TypeWrapper(parameters[ind].ParameterType);
		}

		private string methodName;
		private TypeWrapper objectTypeWrapper;
		private TypeWrapper[] argumentTypeWrappers;

		public string MethodName
		{
			get
			{
				return methodName;
			}

			set
			{
				methodName = value;
			}
		}

		public string ObjectTypeName
		{
			get
			{
				return objectTypeWrapper.TypeName;
			}

			set
			{
				objectTypeWrapper = new TypeWrapper();
				objectTypeWrapper.TypeName = value;
			}
		}

		[System.Xml.Serialization.XmlElement("ArgumentType")]
		public string[] ArgumentTypeNames
		{
			get
			{
				string[] names = new string[argumentTypeWrappers.Length];
				for (uint ind = 0; ind < names.Length; ind++)
					names[ind] = argumentTypeWrappers[ind].TypeName;
				return names;
			}

			set
			{
				argumentTypeWrappers = new TypeWrapper[value.Length];
				for (uint ind = 0; ind < value.Length; ind++)
				{
					argumentTypeWrappers[ind] = new TypeWrapper();
					argumentTypeWrappers[ind].TypeName = value[ind];
				}
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Reflection.MethodInfo WrappedMethodInfo
		{
			get
			{
				System.Type[] argumentTypes = new System.Type[argumentTypeWrappers.Length];
                for (uint ind = 0; ind < argumentTypeWrappers.Length; ind++)
                {
                    argumentTypes[ind] = argumentTypeWrappers[ind].WrappedType;
                    if (argumentTypes[ind] == null)
                        throw new Exception("Could not resolve type \"" + argumentTypeWrappers[ind].TypeName + "\".");
                }

                if (methodName == null)
                    throw new Exception("Method is NULL.");

				return objectTypeWrapper.WrappedType.GetMethod(methodName, argumentTypes);
			}
		}

		public override string ToString()
		{
			string signature = null;
			for (uint ind = 0; ind < argumentTypeWrappers.Length; ind++)
				signature = ((signature != null) ? (signature + ", ") : "") + argumentTypeWrappers[ind].ToString();
			signature = objectTypeWrapper.ToString() + "." + methodName + "(" + ((signature != null) ? signature : "") + ")";
			return signature;
		}
	}

	[Serializable]
	public class TypeWrapper
	{
		public TypeWrapper()
		{
		}

		public TypeWrapper(System.Type type)
		{
			this.typeName = type.FullName;
		}

		private string typeName;

		public override string ToString()
		{
			return typeName;
		}

		public string TypeName
		{
			get
			{
				return typeName;
			}

			set
			{
				typeName = value;
			}
		}

		[System.Xml.Serialization.XmlIgnore]
		public System.Type WrappedType
		{
			get
			{
                foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type result = assembly.GetType(typeName);
                    if (result != null)
                        return result;
                }
                
                throw new Exception("Could not resolve type \"" + typeName + "\".");
			}
		}
	}
}
