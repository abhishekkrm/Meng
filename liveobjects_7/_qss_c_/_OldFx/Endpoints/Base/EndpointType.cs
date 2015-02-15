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

#define DEBUG_DumpAssemblies

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace QS._qss_c_._OldFx.Endpoints.Base
{
    public sealed class EndpointType 
/*
        : IEquatable<EndpointType>
*/ 
    {
/*
        #region Static Constructor

        static EndpointType()
        {
#if DEBUG_DumpAssemblies
            assembly_builder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("quicksilver_endpoints"), AssemblyBuilderAccess.RunAndSave);
            module_builder = assembly_builder.DefineDynamicModule("quicksilver_endpoints", "quicksilver_endpoints.dll", true);
#else
            assembly_builder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("quicksilver_endpoints"), AssemblyBuilderAccess.Run);
            module_builder = assembly_builder.DefineDynamicModule("quicksilver_endpoints.dll");
#endif
        }

        #endregion

        #region Constructor

        public EndpointType(string name, Type[] provided, Type[] required)
        {
            this.provided = provided;
            this.required = required;

            Array.Sort<Type>(provided, TypeComparer);
            Array.Sort<Type>(required, TypeComparer);

            provided_indices = new Dictionary<Type, int>(provided.Length);
            provided_collection = new System.Collections.ObjectModel.Collection<Type>();

            for (int ind = 0; ind < provided.Length; ind++)
            {
                provided_indices.Add(provided[ind], ind);
                provided_collection.Add(provided[ind]);
            }
            
            required_indices = new Dictionary<Type, int>(required.Length);
            required_collection = new System.Collections.ObjectModel.Collection<Type>();

            for (int ind = 0; ind < required.Length; ind++)
            {
                required_indices.Add(required[ind], ind);
                required_collection.Add(required[ind]);
            }

            this.name = name;
            GenerateConstructor();
        }

        #endregion

        private Type[] provided, required;
        private IDictionary<Type, int> provided_indices, required_indices;
        private System.Collections.ObjectModel.Collection<Type> provided_collection, required_collection;
        private string name;
        private Type internalType;
        private ConstructorInfo constructorInfo;
        private static readonly AssemblyBuilder assembly_builder;
        private static readonly ModuleBuilder module_builder;

        private static readonly Comparison<Type> TypeComparer = new Comparison<Type>(EndpointType.CompareTypes);
        private static int CompareTypes(Type type1, Type type2)
        {
            return type1.Name.CompareTo(type2.Name);
        }

        #region GenerateConstructor

        // public Type AggregateInterfaces(string interface_name, Type[] interfaces)
        // {
        //     TypeBuilder builder = module_builder.DefineType(interface_name, TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract);
        //     foreach (Type type in interfaces)
        //         builder.AddInterfaceImplementation(type);
        //     return builder.CreateType();
        // }

        private void GenerateConstructor()
        {
            TypeBuilder type_builder = module_builder.DefineType(
                name, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(Endpoint));
        
            Type[] constructor_parameter_types = new Type[provided.Length + 1];
            constructor_parameter_types[0] = typeof(EndpointType);
            for (int ind = 0; ind < provided.Length; ind++)
                constructor_parameter_types[ind + 1] = provided[ind];

            ConstructorBuilder constructor_builder = type_builder.DefineConstructor(
                MethodAttributes.Public, CallingConventions.Standard, constructor_parameter_types);
            ILGenerator constructor_generator = constructor_builder.GetILGenerator();
            constructor_generator.Emit(OpCodes.Ldarg_0);
            constructor_generator.Emit(OpCodes.Ldarg_1);
            constructor_generator.Emit(OpCodes.Call, typeof(Endpoint).GetConstructor(new Type[] { typeof(EndpointType) }));

            MethodInfo method_info_BindRequiredInterfaces = 
                typeof(Endpoint).GetMethod("BindRequiredInterfaces", BindingFlags.Instance | BindingFlags.NonPublic);

            ParameterInfo[] parameters_BindRequiredInterfaces = method_info_BindRequiredInterfaces.GetParameters();
            Type[] parameter_types_BindRequiredInterfaces = new Type[parameters_BindRequiredInterfaces.Length];
            for (int ind = 0; ind < parameters_BindRequiredInterfaces.Length; ind++)
                parameter_types_BindRequiredInterfaces[ind] = parameters_BindRequiredInterfaces[ind].ParameterType;
            
            MethodBuilder method_builder_BindRequiredInterfaces = type_builder.DefineMethod(method_info_BindRequiredInterfaces.Name,
                MethodAttributes.Family | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.Standard,
                method_info_BindRequiredInterfaces.ReturnType, parameter_types_BindRequiredInterfaces);
            type_builder.DefineMethodOverride(method_builder_BindRequiredInterfaces, method_info_BindRequiredInterfaces);
            ILGenerator generator_BindRequiredInterfaces = method_builder_BindRequiredInterfaces.GetILGenerator();

            MethodInfo method_info_UnbindRequiredInterfaces =
                typeof(Endpoint).GetMethod("UnbindRequiredInterfaces", BindingFlags.Instance | BindingFlags.NonPublic);
            
            ParameterInfo[] parameters_UnbindRequiredInterfaces = method_info_UnbindRequiredInterfaces.GetParameters();
            Type[] parameter_types_UnbindRequiredInterfaces = new Type[parameters_UnbindRequiredInterfaces.Length];
            for (int ind = 0; ind < parameters_UnbindRequiredInterfaces.Length; ind++)
                parameter_types_UnbindRequiredInterfaces[ind] = parameters_UnbindRequiredInterfaces[ind].ParameterType;
            
            MethodBuilder method_builder_UnbindRequiredInterfaces = type_builder.DefineMethod(method_info_UnbindRequiredInterfaces.Name,
                MethodAttributes.Family | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.Standard,
                method_info_UnbindRequiredInterfaces.ReturnType, parameter_types_UnbindRequiredInterfaces);
            type_builder.DefineMethodOverride(method_builder_UnbindRequiredInterfaces, method_info_UnbindRequiredInterfaces);
            ILGenerator generator_UnbindRequiredInterfaces = method_builder_UnbindRequiredInterfaces.GetILGenerator();

            MethodInfo method_info_BindProvidedInterfaces =
                typeof(Endpoint).GetMethod("BindProvidedInterfaces", BindingFlags.Instance | BindingFlags.NonPublic);

            ParameterInfo[] parameters_BindProvidedInterfaces = method_info_BindProvidedInterfaces.GetParameters();
            Type[] parameter_types_BindProvidedInterfaces = new Type[parameters_BindProvidedInterfaces.Length];
            for (int ind = 0; ind < parameters_BindProvidedInterfaces.Length; ind++)
                parameter_types_BindProvidedInterfaces[ind] = parameters_BindProvidedInterfaces[ind].ParameterType;

            MethodBuilder method_builder_BindProvidedInterfaces = type_builder.DefineMethod(method_info_BindProvidedInterfaces.Name,
                MethodAttributes.Family | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.Standard,
                method_info_BindProvidedInterfaces.ReturnType, parameter_types_BindProvidedInterfaces);
            type_builder.DefineMethodOverride(method_builder_BindProvidedInterfaces, method_info_BindProvidedInterfaces);
            ILGenerator generator_BindProvidedInterfaces = method_builder_BindProvidedInterfaces.GetILGenerator();

            int count = 0;
            FieldBuilder[] field_builders = new FieldBuilder[provided.Length];
            foreach (Type interface_type in provided)
            {
                FieldBuilder field_builder = type_builder.DefineField("provided_" + (++count).ToString("0000"), interface_type, FieldAttributes.Private);
                field_builders[count - 1] = field_builder;

                generator_BindProvidedInterfaces.Emit(OpCodes.Ldarg_1);
                generator_BindProvidedInterfaces.Emit(OpCodes.Ldc_I4, count - 1);
                generator_BindProvidedInterfaces.Emit(OpCodes.Ldarg_0);
                generator_BindProvidedInterfaces.Emit(OpCodes.Ldfld, field_builder);
                generator_BindProvidedInterfaces.Emit(OpCodes.Stelem_Ref);

                constructor_generator.Emit(OpCodes.Ldarg_0);
                constructor_generator.Emit(OpCodes.Ldarg_S, (short) (count + 1));
                constructor_generator.Emit(OpCodes.Stfld, field_builder);
            }

            count = 0;
            field_builders = new FieldBuilder[required.Length];
            foreach (Type interface_type in required)
            {
                FieldBuilder field_builder = type_builder.DefineField("required_" + (++count).ToString("0000"), interface_type, FieldAttributes.Private);
                field_builders[count - 1] = field_builder;

                generator_BindRequiredInterfaces.Emit(OpCodes.Ldarg_0);
                generator_BindRequiredInterfaces.Emit(OpCodes.Ldarg_1);
                generator_BindRequiredInterfaces.Emit(OpCodes.Ldc_I4, count - 1);
                generator_BindRequiredInterfaces.Emit(OpCodes.Ldelem_Ref);
                generator_BindRequiredInterfaces.Emit(OpCodes.Stfld, field_builder);

                generator_UnbindRequiredInterfaces.Emit(OpCodes.Ldarg_0);
                generator_UnbindRequiredInterfaces.Emit(OpCodes.Ldnull);
                generator_UnbindRequiredInterfaces.Emit(OpCodes.Stfld, field_builder);

                Type accessor_type = typeof(IInterface<object>).GetGenericTypeDefinition().MakeGenericType(new Type[] { interface_type });

                type_builder.AddInterfaceImplementation(accessor_type);

                MethodInfo method_info = accessor_type.GetProperty("Interface").GetGetMethod();

                MethodBuilder method_builder = type_builder.DefineMethod(method_info.Name,
                    MethodAttributes.Private | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                    MethodAttributes.Virtual | MethodAttributes.Final, CallingConventions.Standard, interface_type, Type.EmptyTypes);
                type_builder.DefineMethodOverride(method_builder, method_info);

                ILGenerator method_generator = method_builder.GetILGenerator();

                method_generator.Emit(OpCodes.Ldarg_0);
                method_generator.Emit(OpCodes.Ldfld, field_builder);
                method_generator.Emit(OpCodes.Ret);
            }

            constructor_generator.Emit(OpCodes.Ret);
            
            generator_BindRequiredInterfaces.Emit(OpCodes.Ret);
            generator_UnbindRequiredInterfaces.Emit(OpCodes.Ret);
            generator_GetProvidedInterfaces.Emit(OpCodes.Ret);

            internalType = type_builder.CreateType();
            this.constructorInfo = internalType.GetConstructor(constructor_parameter_types);

#if DEBUG_DumpAssemblies
            assembly_builder.Save("quicksilver_endpoints.dll");
#endif
        }

        #endregion

        #region Static Members

        public static bool Match(EndpointType type1, EndpointType type2)
        {
            foreach (Type type in type1.Required)
                if (!type2.Provides(type))
                    return false;
            foreach (Type type in type2.Required)
                if (!type1.Provides(type))
                    return false;
            return true;
        }

        #endregion

        #region Main Functionality

        public bool Provides(Type type)
        {
            return provided_collection.Contains(type);            
        }

        public bool Requires(Type type)
        {
            return required_collection.Contains(type);
        }

        public IEnumerable<Type> Provided
        {
            get { return provided_collection; }
        }

        public IEnumerable<Type> Required
        {
            get { return required_collection; }
        }

        public bool Specializes(EndpointType endpointType)
        {
            foreach (Type type in endpointType.Provided)
                if (!provided_collection.Contains(type))
                    return false;
            foreach (Type type in required_collection)
                if (!endpointType.Requires(type))
                    return false;
            return true;
        }

        public bool Generalizes(EndpointType endpointType)
        {
            return endpointType.Specializes(this);
        }

        public bool Matches(EndpointType endpointType)
        {
            return EndpointType.Match(this, endpointType);
        }

        public Endpoint Create(params Binding[] bindings)
        {            
            if (bindings.Length != provided.Length)
                throw new Exception("Too many or too few bindings.");

            object[] objects = new object[provided.Length + 1];
            objects[0] = this;
            foreach (Binding binding in bindings)
                objects[provided_indices[binding.Interface] + 1] = binding.Implementation;

            return (Endpoint) constructorInfo.Invoke(objects);
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is EndpointType)
                return ((IEquatable<EndpointType>)this).Equals((EndpointType)obj);
            else
                throw new Exception("Cannot compare to an object that is not of class IEndpointType.");
        }

        public override int GetHashCode()
        {
            int result = 0;
            foreach (Type type in provided)
                result ^= type.GetHashCode();
            foreach (Type type in required)
                result ^= type.GetHashCode();
            return result;
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append("( ");
            bool isfirst = true;
            foreach (Type type in provided)
            {
                if (isfirst)
                    isfirst = false;
                else
                    s.Append(", ");
                s.Append(type.ToString());
            }
            s.Append(" : ");
            isfirst = true;
            foreach (Type type in required)
            {
                if (isfirst)
                    isfirst = false;
                else
                    s.Append(", ");
                s.Append(type.ToString());
            }
            s.Append(" )");
            return s.ToString();
        }

        public bool Equals(EndpointType other)
        {
            return this.Specializes(other) && other.Specializes(this);
        }

        #endregion
*/ 
    }
}
