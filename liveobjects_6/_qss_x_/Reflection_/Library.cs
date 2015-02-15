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

#define DEBUG_GENERATE_SOURCES_ON_THE_DISK
#define DEBUG_INCLUDE_INSPECTION_CODE
#define DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace QS._qss_x_.Reflection_
{
    public sealed class Library : 
        QS.Fx.Inspection.Inspectable, 
        QS.Fx.Interface.Classes.ILibrary, 
        QS._qss_x_.Interface_.Classes_.ILibrary2_,
        QS.Fx.Reflection.Library.ILibrary
    {
        #region _LIVEOBJECTS_ROOT_
#if !LINUX_LIVEOBJECTS_ROOT
        public static string _LIVEOBJECTS_ROOT_
        {
            get
            {
                if (_liveobjects_root_ == null)
                {
                    lock (typeof(Library))
                    {
                        if (_liveobjects_root_ == null)
                        {
                            RegistryKey _key_software, _key_liveobjects;
                            string _my_root = null;
                            bool _ok = false;
                            _key_software =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE",
                                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree,
                                    System.Security.AccessControl.RegistryRights.ReadKey);
                            if (_key_software != null)
                            {
                                _key_liveobjects =
                                    _key_software.OpenSubKey(
                                    "Live Distributed Objects",
                                    Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree,
                                    System.Security.AccessControl.RegistryRights.ReadKey);
                                if (_key_liveobjects != null)
                                {
                                    _my_root = (string) _key_liveobjects.GetValue("Root");
                                    if (_my_root != null)
                                        _ok = true;
                                }
                            }
                            if (!_ok)
                            {
                                try
                                {
                                    _key_software =
                                        Registry.LocalMachine.OpenSubKey(
                                            "SOFTWARE",
                                            Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree,
                                            System.Security.AccessControl.RegistryRights.FullControl);
                                    if (_key_software == null)
                                    {
                                        _key_software = Registry.LocalMachine.CreateSubKey("SOFTWARE");
                                        if (_key_software == null)
                                            throw new Exception("Could not access or create HKLM\\SOFTWARE section of the registry");
                                    }
                                    _key_liveobjects =
                                        _key_software.OpenSubKey(
                                        "Live Distributed Objects",
                                        Microsoft.Win32.RegistryKeyPermissionCheck.ReadWriteSubTree,
                                        System.Security.AccessControl.RegistryRights.FullControl);
                                    if (_key_liveobjects == null)
                                    {
                                        _key_liveobjects = _key_software.CreateSubKey("Live Distributed Objects");
                                        if (_key_liveobjects == null)
                                            throw new Exception("Could not access or create HKLM\\SOFTWARE\\Live Distributed Objects section of the registry");
                                    }
                                    _my_root = (string)_key_liveobjects.GetValue("Root");
                                    if (_my_root == null)
                                    {
                                        _my_root = Directory.GetParent(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)).FullName;
                                        if (_my_root == null)
                                            throw new Exception("Could not calculate the root path of the distribution.");
                                        _key_liveobjects.SetValue("Root", _my_root, RegistryValueKind.String);
                                        string _my_root_2 = (string)_key_liveobjects.GetValue("Root");
                                        if ((_my_root_2 == null) || (!_my_root_2.Equals(_my_root)))
                                            throw new Exception("Could not create a registry key for the root path of the distribution.");
                                    }
                                }
                                catch (Exception _exc)
                                {
                                    string _message =
                                        "Cannot start the runtime because the registry key pointing to the root path of the distribution has not been found and the attempt to create the key automatically has failed.";
                                    try
                                    {
                                        System.Windows.Forms.MessageBox.Show(
                                            _message + "\n\n" + _exc.ToString(), 
                                            "Exception", System.Windows.Forms.MessageBoxButtons.OK, 
                                            System.Windows.Forms.MessageBoxIcon.Error);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    throw new Exception(_message, _exc);
                                }
                            }
                            _liveobjects_root_ = _my_root;
                        }
                    }
                }
                return _liveobjects_root_;
            }
        }
#else
        public static string _LIVEOBJECTS_ROOT_
        {
            get
            {
                if (_liveobjects_root_ == null)
                {
                    lock (typeof(Library))
                    {
                        if (_liveobjects_root_ == null)
                        {
                            string _my_root = null;
                            
                            _my_root = Directory.GetParent(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)).FullName;
                            if (_my_root == null)
                                throw new Exception("Could not calculate the root path of the distribution.");

                            _liveobjects_root_ = _my_root;
                        }
                    }
                }

                return _liveobjects_root_;
            }
        }

#endif
        private static string _liveobjects_root_ = null;

        #endregion

        #region Initialization

        static Library()
        {
            _library = null;
        }

        public static QS.Fx.Interface.Classes.ILibrary LocalLibrary
        {
            get { return _LocalLibrary; }
        }

        internal static QS._qss_x_.Reflection_.Library LocalLibrary_
        {
            get { return _LocalLibrary; }
        }

        private static Library _LocalLibrary
        {
            get
            {
                lock (typeof(Library))
                {
                    if (_library == null)
                        _library = new Library();
                    return _library;
                }
            }
        }

        private static Library _library;

        #endregion 

        #region Constructor

        private Library()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(this.__currentdomain_AssemblyResolve);
            try
            {
                QS.Fx.Base.ID _namespace_id;
                ulong _namespace_incarnation;
                string _namespace_name, _namespace_description;
                _GetLibraryAttributes(System.Reflection.Assembly.GetExecutingAssembly(),
                    out _namespace_id, out _namespace_incarnation, out _namespace_name, out _namespace_description);
                IEnumerable<System.Reflection.Assembly> _my_assemblies = QS._qss_x_.Base1_.Assemblies.RegisteredAssemblies;
                foreach (System.Reflection.Assembly _a in _my_assemblies)
                {
                    QS.Fx.Base.ID _a_namespace_id;
                    ulong _a_namespace_incarnation;
                    string _a_namespace_name, _a_namespace_description;
                    _GetLibraryAttributes(_a,
                        out _a_namespace_id, out _a_namespace_incarnation, out _a_namespace_name, out _a_namespace_description);
                    if (!_a_namespace_id.Equals(_namespace_id) || !_a_namespace_incarnation.Equals(_namespace_incarnation) ||
                        !_a_namespace_name.Equals(_namespace_name) || !_a_namespace_description.Equals(_namespace_description))
                        throw new Exception("Cannot initialize the runtime because the assemblies are not compatible. Assembly \"" +
                            _a.FullName + "\" has id = " + _a_namespace_id.ToString() + ", version = " + _a_namespace_incarnation.ToString() +
                            ", name = \"" + _a_namespace_name + "\", and description = \"" + _a_namespace_description + "\", whereas the executing assembly has id = " +
                            _namespace_id.ToString() + ", version = " + _namespace_incarnation.ToString() + ", name = \"" + _namespace_name + "\", and description = \"" +
                            _namespace_description + "\".");
                }

                Namespace_ _namespace = new Namespace_(
                    this, _namespace_id, _namespace_incarnation, _namespace_name, _namespace_description, _my_assemblies);

                _namespaces.Add(_namespace_id, _namespace);

                _start_compilation();

                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_bool, 0UL, typeof(bool));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_byte, 0UL, typeof(byte));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_char, 0UL, typeof(char));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_short, 0UL, typeof(short));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_int, 0UL, typeof(int));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_long, 0UL, typeof(long));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_string, 0UL, typeof(string));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ushort, 0UL, typeof(ushort));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_uint, 0UL, typeof(uint));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ulong, 0UL, typeof(ulong));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_void, 0UL, typeof(void));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_float, 0UL, typeof(float));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_double, 0UL, typeof(double));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_object, 0UL, typeof(object));

                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_IEnumerable, 0UL, 
                    typeof(System.Collections.Generic.IEnumerable<object>).GetGenericTypeDefinition());
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_IDictionary, 0UL,
                    typeof(System.Collections.Generic.IDictionary<object, object>).GetGenericTypeDefinition());
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_IList, 0UL,
                    typeof(System.Collections.Generic.IList<object>).GetGenericTypeDefinition());
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ICollection, 0UL,
                    typeof(System.Collections.Generic.ICollection<object>).GetGenericTypeDefinition());
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ArraySegment, 0UL,
                    typeof(System.ArraySegment<object>).GetGenericTypeDefinition());

                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_Array, 0UL, typeof(System.Array));

                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_bools, 0UL, typeof(bool[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_bytes, 0UL, typeof(byte[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_chars, 0UL, typeof(char[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_shorts, 0UL, typeof(short[]));                
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ints, 0UL, typeof(int[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_longs, 0UL, typeof(long[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_strings, 0UL, typeof(string[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ushorts, 0UL, typeof(ushort[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_uints, 0UL, typeof(uint[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_ulongs, 0UL, typeof(ulong[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_floats, 0UL, typeof(float[]));
                _RegisterValueClass(_namespace, QS.Fx.Reflection.ValueClasses._system_doubles, 0UL, typeof(double[]));

                foreach (System.Reflection.Assembly _assembly in QS._qss_x_.Base1_.Assemblies.RegisteredAssemblies)
                {
                    try
                    {
                        //                        System.Windows.Forms.MessageBox.Show("Processing types in \"" + _assembly.GetName().Name + "\".",
                        //                            "Confirmation", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);

                        Type[] _types;
                        try
                        {
                            _types = _assembly.GetTypes();
                        }
                        catch (Exception exc)
                        {
                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("Could not load types from assembly \"" + _assembly.FullName +
                                "\", this could be a result of a misconfigured .NET framework or some .NET components on your system.\n" + exc.ToString());
                            if (exc is System.Reflection.ReflectionTypeLoadException)
                            {
                                foreach (Exception e2 in ((System.Reflection.ReflectionTypeLoadException)exc).LoaderExceptions)
                                {
                                    for (Exception e = e2; e != null; e = e.InnerException)
                                    {
                                        builder.AppendLine(
                                            "message = " + e.Message + "\nstack = " + e.StackTrace + "\nsource = " + e.Source + "\ntarget = " + e.TargetSite);
                                        if (e is System.IO.FileNotFoundException)
                                            builder.AppendLine("file = \"" + ((System.IO.FileNotFoundException)e).FileName + "\"\nlog = " +
                                                ((System.IO.FileNotFoundException)e).FusionLog);
                                    }
                                }
                            }
                            throw new Exception(builder.ToString());
                        }

                        for (int _pass = 1; _pass <= 2; _pass++)
                        {
                            foreach (Type _type in _types)
                            {
                                try
                                {
                                    this._Register(_pass, _type);
                                }
                                catch (Exception exc)
                                {
                                    throw new Exception("Could not register type \"" + _type.Name + "\" from assembly \"" + _assembly.GetName() +
                                        "\", the type may not have been properly annotated as a live object component.", exc);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        if (_assembly.FullName.ToLower().Contains("liveobjects"))
                            throw new Exception("Could not load types from a core live objects runtime component.", exc);
                        else
                            throw new Exception("Could not load types from an external component library.", exc);
                    }
                }

                _stop_compilation();
            }
            catch (Exception exc)
            {
                throw new Exception("Could not build the base library of live objects to bootstrap the system.", exc);
            }
        }

        #endregion

        #region Fields

        private IDictionary<QS.Fx.Base.ID, Namespace_> _namespaces = new Dictionary<QS.Fx.Base.ID, Namespace_>();

        private IDictionary<Type, QS.Fx.Reflection.IValueClass> _valueclasses = new Dictionary<Type, QS.Fx.Reflection.IValueClass>();
        private IDictionary<Type, QS.Fx.Reflection.IInterfaceClass> _interfaceclasses = new Dictionary<Type, QS.Fx.Reflection.IInterfaceClass>();
        private IDictionary<Type, QS.Fx.Reflection.IEndpointClass> _endpointclasses = new Dictionary<Type, QS.Fx.Reflection.IEndpointClass>();
        private IDictionary<Type, QS.Fx.Reflection.IObjectClass> _objectclasses = new Dictionary<Type, QS.Fx.Reflection.IObjectClass>();
        private IDictionary<Type, QS.Fx.Reflection.IComponentClass> _componentclasses = new Dictionary<Type, QS.Fx.Reflection.IComponentClass>();

        private IDictionary<Type, QS.Fx.Reflection.IInterfaceConstraintClass> _interfaceconstraintclasses = new Dictionary<Type, QS.Fx.Reflection.IInterfaceConstraintClass>();
        private IDictionary<Type, QS.Fx.Reflection.IEndpointConstraintClass> _endpointconstraintclasses = new Dictionary<Type, QS.Fx.Reflection.IEndpointConstraintClass>();
        private IDictionary<Type, QS.Fx.Reflection.IObjectConstraintClass> _objectconstraintclasses = new Dictionary<Type, QS.Fx.Reflection.IObjectConstraintClass>();

        private _Compilation _compilation;

        #endregion

        #region _Compilation

        private struct _Compilation
        {
            public int _seqno;
            public bool _started;
            public StringBuilder _source;
            public IList<Type> _types;
        }

        #endregion

        #region _start_compilation

        public void _start_compilation()
        {
            if (_compilation._started)
                throw new Exception("Compilation is already started.");
            _compilation._started = true;
            _compilation._seqno++;
            _compilation._source = new StringBuilder();
            _compilation._types = new List<Type>();
        }

        #endregion

        #region _stop_compilation

        public void _stop_compilation()
        {
            Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
            System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();
            ICollection<string> _aaaa = new System.Collections.ObjectModel.Collection<string>();
            foreach (System.Reflection.Assembly _aa in QS._qss_x_.Base1_.Assemblies.RegisteredAssemblies)
            {
                // nasty hack
                if (!_aa.FullName.Contains("liveobjects_7"))
                {
                    string _location = _aa.Location;
                    if (!_aaaa.Contains(_location))
                        _aaaa.Add(_location);
                }
            }
            foreach (Type _tttt in _compilation._types)
            {
                System.Reflection.Assembly _aa = _tttt.Assembly;
                string _location = _aa.Location;
                if (!_aaaa.Contains(_location))
                    _aaaa.Add(_location);
            }
            foreach (string _location in _aaaa)
                parameters.ReferencedAssemblies.Add(_location);
            parameters.GenerateExecutable = false;
            parameters.IncludeDebugInformation = true;
            string _sourcename = "liveobjects_autogenerated_" + _compilation._seqno.ToString("000000");
            parameters.OutputAssembly = _sourcename + ".dll";
            parameters.CompilerOptions = "/unsafe /optimize";
            string _source = _compilation._source.ToString();
            System.CodeDom.Compiler.CompilerResults results_;
#if DEBUG_GENERATE_SOURCES_ON_THE_DISK
            string _sourcefilename = _sourcename + ".cs";
            using (StreamWriter _sourcestreamwriter = new StreamWriter(_sourcefilename, false))
            {
                _sourcestreamwriter.Write(_source);
            }
            parameters.GenerateInMemory = false;
            results_ = provider.CompileAssemblyFromFile(parameters, new string[] { _sourcefilename });
#else
            parameters.GenerateInMemory = true;
            results_ = provider.CompileAssemblyFromSource(parameters, new string[] { _source });
#endif
            if (results_.Errors.HasErrors)
            {
                StringBuilder ss = new StringBuilder();
                int _ecount = 0;
                foreach (System.CodeDom.Compiler.CompilerError _e in results_.Errors)
                {
                    _ecount++;
                    if (_ecount > 10)
                    {
                        ss.Append("there were more errors\n");
                        break;
                    }
                    ss.Append("error in line ");
                    ss.Append(_e.Line.ToString());
                    ss.Append(" at column ");
                    ss.Append(_e.Column.ToString());
                    ss.Append(" :\n");
                    ss.Append(_e.ErrorText);
                    ss.Append("\n\n\n");
                    using (System.IO.StringReader rr = new System.IO.StringReader(_source))
                    {
                        int lli = 1;
                        string ll;
                        while ((ll = rr.ReadLine()) != null)
                        {
                            int _d = lli - _e.Line;
                            if ((_d > -100) && (_d < 100))
                            {
                                ss.Append(lli.ToString("000000"));
                                ss.Append(" : ");
                                ss.AppendLine(ll);
                            }
                            lli++;
                        }
                    }
                }
                throw new Exception("Could not compile autogenerated assembly.\n" + ss.ToString());
            }
            System.Reflection.Assembly assembly_ = results_.CompiledAssembly;
#if !LINUX_MODULERESOLVE
            assembly_.ModuleResolve += new System.Reflection.ModuleResolveEventHandler(this.__assembly_ModuleResolveCallback);
#endif
            Type[] _compiled_assembly_types;
            try
            {
                _compiled_assembly_types = assembly_.GetTypes();
            }
            catch (Exception _e)
            {
                StringBuilder ss = new StringBuilder();
                ss.Append("Could not load types from autogenerated assembly \"" + assembly_.FullName + "\".");
                throw new Exception(ss.ToString());
            }
            foreach (Type _compiled_type in _compiled_assembly_types)
            {
                object[] _aa = _compiled_type.GetCustomAttributes(typeof(QS.Fx.Internal.I000004), false);
                if (_aa.Length == 1)
                {
                    QS.Fx.Internal.I000004 _a = (QS.Fx.Internal.I000004)_aa[0];
                    QS.Fx.Internal.I000005 _autogenerated_category = _a.a;
                    QS.Fx.Base.ID _ns_id = new QS.Fx.Base.ID(_a.b);
                    ulong _ns_in = _a.c;
                    Namespace_ _ns = _GetNamespace(_ns_id, _ns_in);
                    QS.Fx.Base.ID _cc_id = new QS.Fx.Base.ID(_a.d);
                    ulong _cc_in = _a.e;
                    switch (_autogenerated_category)
                    {
                        case QS.Fx.Internal.I000005.i1:
                            {
                                QS._qss_x_.Reflection_.InterfaceClass _interfaceclass =
                                    (QS._qss_x_.Reflection_.InterfaceClass)_ns._GetInterfaceClass(_cc_id, _cc_in);
                                _interfaceclass.internal_info_._frontend_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _interfaceclass.internal_info_._frontend_code = null;
#endif
                            }
                            break;

                        case QS.Fx.Internal.I000005.i2:
                            {
                                QS._qss_x_.Reflection_.InterfaceClass _interfaceclass =
                                    (QS._qss_x_.Reflection_.InterfaceClass)_ns._GetInterfaceClass(_cc_id, _cc_in);
                                _interfaceclass.internal_info_._backend_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _interfaceclass.internal_info_._backend_code = null;
#endif
                            }
                            break;

                        case QS.Fx.Internal.I000005.i3:
                            {
                                QS._qss_x_.Reflection_.InterfaceClass _interfaceclass =
                                    (QS._qss_x_.Reflection_.InterfaceClass)_ns._GetInterfaceClass(_cc_id, _cc_in);
                                _interfaceclass.internal_info_._interceptor_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _interfaceclass.internal_info_._interceptor_code = null;
#endif
                            }
                            break;

                        case QS.Fx.Internal.I000005.o1:
                            {
                                QS._qss_x_.Reflection_.ObjectClass _objectclass =
                                    (QS._qss_x_.Reflection_.ObjectClass)_ns._GetObjectClass(_cc_id, _cc_in);
                                _objectclass.internal_info_._frontend_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _objectclass.internal_info_._frontend_code = null;
#endif
                            }
                            break;

                        case QS.Fx.Internal.I000005.o2:
                            {
                                QS._qss_x_.Reflection_.ObjectClass _objectclass =
                                    (QS._qss_x_.Reflection_.ObjectClass)_ns._GetObjectClass(_cc_id, _cc_in);
                                _objectclass.internal_info_._backend_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _objectclass.internal_info_._backend_code = null;
#endif
                            }
                            break;

                        case QS.Fx.Internal.I000005.v1:
                            {
                                QS._qss_x_.Reflection_.ValueClass _valueclass =
                                    (QS._qss_x_.Reflection_.ValueClass)_ns._GetValueClass(_cc_id, _cc_in);
                                _valueclass.internal_info_._serialization_type = _compiled_type;
#if DEBUG_KEEP_GENERATED_FRONTEND_AND_BACKEND_CODE_FOR_DEBUGGING_PURPOSES
#else
                            _valueclass.internal_info_._serialization_code = null;
#endif
                            }
                            break;

                        default:
                            throw new Exception("Unknown type of autogenerated code.");
                    }
                }
                else
                {
                    if (!typeof(QS.Fx.Base.IEvent).IsAssignableFrom(_compiled_type))
                        throw new Exception("Autogenerated type \"" + _compiled_type.ToString() + "\" is not an event type, and it has not been correctly annotated.");
                }
            }
            _compilation._source = null;
            _compilation._started = false;
            _compilation._types = null;
        }

        System.Reflection.Assembly __currentdomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string _name = args.Name;
            foreach (System.Reflection.Assembly _a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (_a.FullName.Equals(_name))
                    return _a;
            }
            return null;
        }

        System.Reflection.Module __assembly_ModuleResolveCallback(object sender, ResolveEventArgs e)
        {
            string _name = e.Name;
            return null;
        }

        #endregion

        #region Inspection

#if DEBUG_INCLUDE_INSPECTION_CODE

        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Namespace_> __inspectable_namespaces__;
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IValueClass> __inspectable_valueclasses__;
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IInterfaceClass> __inspectable_interfaceclasses__;
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IEndpointClass> __inspectable_endpointclasses__;
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IObjectClass> __inspectable_objectclasses__;
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IComponentClass> __inspectable_componentclasses__;

        [QS.Fx.Base.Inspectable("_namespaces")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Namespace_> __inspectable_namespaces
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_namespaces__ == null)
                        __inspectable_namespaces__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Namespace_>("_namespaces", _namespaces,
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, Namespace_>.ConversionCallback(
                                    QS.Fx.Base.ID.FromString));
                    return __inspectable_namespaces__;
                }
            }
        }

        [QS.Fx.Base.Inspectable("_valueclasses")]
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IValueClass> __inspectable_valueclasses
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_valueclasses__ == null)
                        __inspectable_valueclasses__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IValueClass>("_valueclasses", _valueclasses,
                                new QS._qss_e_.Inspection_.ObjectToStringCallback<Type>(delegate (Type t) { return t.AssemblyQualifiedName; }), 
                                new QS._qss_e_.Inspection_.StringToObjectCallback<Type>(
                                    delegate (string s) 
                                    { 
                                        System.Type t = System.Type.GetType(s);
                                        return t;
                                    })); 
                    return __inspectable_valueclasses__;
                }
            }
        }

        [QS.Fx.Base.Inspectable("_interfaceclasses")]
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IInterfaceClass> __inspectable_interfaceclasses
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_interfaceclasses__ == null)
                        __inspectable_interfaceclasses__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IInterfaceClass>("_interfaceclasses", _interfaceclasses,
                                new QS._qss_e_.Inspection_.ObjectToStringCallback<Type>(delegate(Type t) { return t.AssemblyQualifiedName; }),
                                new QS._qss_e_.Inspection_.StringToObjectCallback<Type>(delegate(string s) { return System.Type.GetType(s); }));
                    return __inspectable_interfaceclasses__;
                }
            }
        }

        [QS.Fx.Base.Inspectable("_endpointclasses")]
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IEndpointClass> __inspectable_endpointclasses
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_endpointclasses__ == null)
                        __inspectable_endpointclasses__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IEndpointClass>("_endpointclasses", _endpointclasses,
                                new QS._qss_e_.Inspection_.ObjectToStringCallback<Type>(delegate(Type t) { return t.AssemblyQualifiedName; }),
                                new QS._qss_e_.Inspection_.StringToObjectCallback<Type>(delegate(string s) { return System.Type.GetType(s); }));
                    return __inspectable_endpointclasses__;
                }
            }
        }

        [QS.Fx.Base.Inspectable("_objectclasses")]
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IObjectClass> __inspectable_objectclasses
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_objectclasses__ == null)
                        __inspectable_objectclasses__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IObjectClass>("_objectclasses", _objectclasses,
                                new QS._qss_e_.Inspection_.ObjectToStringCallback<Type>(delegate(Type t) { return t.AssemblyQualifiedName; }),
                                new QS._qss_e_.Inspection_.StringToObjectCallback<Type>(delegate(string s) { return System.Type.GetType(s); }));
                    return __inspectable_objectclasses__;
                }
            }
        }

        [QS.Fx.Base.Inspectable("_componentclasses")]
        private QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IComponentClass> __inspectable_componentclasses
        {
            get
            {
                lock (this)
                {
                    if (__inspectable_componentclasses__ == null)
                        __inspectable_componentclasses__ =
                            new QS._qss_e_.Inspection_.DictionaryWrapper4<Type, QS.Fx.Reflection.IComponentClass>("_componentclasses", _componentclasses,
                                new QS._qss_e_.Inspection_.ObjectToStringCallback<Type>(delegate(Type t) { return t.AssemblyQualifiedName; }),
                                new QS._qss_e_.Inspection_.StringToObjectCallback<Type>(delegate(string s) { return System.Type.GetType(s); }));
                    return __inspectable_componentclasses__;
                }
            }
        }

#endif

        #endregion

        #region _StringToComponentID

        public static void _StringToComponentID(string _s, 
            out QS.Fx.Base.ID _namespace_id, out ulong _namespace_incarnation, out QS.Fx.Base.ID _id, out ulong _incarnation)
        {
            int p = _s.IndexOf(':');
            if (p >= 0 && p < _s.Length)
            {
                int q = _s.IndexOf('`', 0, p);
                if (q >= 0 && q < p)
                    _namespace_incarnation = (q < p - 1) ? Convert.ToUInt64(_s.Substring(q + 1, p - q - 1)) : 0UL;
                else
                {
                    q = p;
                    _namespace_incarnation = 0;
                }
                _namespace_id = (q > 0) ? new QS.Fx.Base.ID(QS.Fx.Base.Int128.FromString(_s.Substring(0, q))) : QS.Fx.Base.ID._0;
            }
            else
            {
                p = -1;
                _namespace_id = QS.Fx.Base.ID._0;
                _namespace_incarnation = 0;
            }
            int r = _s.IndexOf('`', p + 1);
            if (r > p && r < _s.Length)
                _incarnation = (r + 1 < _s.Length) ? Convert.ToUInt64(_s.Substring(r + 1)) : 0UL;
            else
            {
                r = _s.Length;
                _incarnation = 0;
            }
            _id = (r > p + 1) ? new QS.Fx.Base.ID(QS.Fx.Base.Int128.FromString(_s.Substring(p + 1, r - p - 1))) : QS.Fx.Base.ID._0;
        }

        #endregion

        #region _ComponentIDToString

        public static string _ComponentIDToString(
            QS.Fx.Base.ID _namespace_id, ulong _namespace_incarnation, QS.Fx.Base.ID _id, ulong _incarnation)
        {
            StringBuilder _ss = new StringBuilder();
            if (!_namespace_id.Equals(QS.Fx.Base.ID._0) || _namespace_incarnation > 0)
            {
                _ss.Append(_namespace_id.ToString());
                if (_namespace_incarnation > 0)
                {
                    _ss.Append('`');
                    _ss.Append(_namespace_incarnation.ToString());
                }
                _ss.Append(':');
            }
            _ss.Append(_id.ToString());
            if (_incarnation > 0)
            {
                _ss.Append('`');
                _ss.Append(_incarnation.ToString());
            }
            return _ss.ToString();
        }

        #endregion

        #region _GetLibraryAttributes

        public static void _GetLibraryAttributes(System.Reflection.Assembly _a, 
            out QS.Fx.Base.ID _id, out ulong _incarnation, out string _name, out string _description)
        {
            object[] _aa = _a.GetCustomAttributes(typeof(QS.Fx.Reflection.LibraryAttribute), false);
            if (_aa.Length != 1)
                throw new Exception("Assembly \"" + _a.FullName + "\" has not defined \"LibraryAttribute\".");
            QS.Fx.Reflection.LibraryAttribute _aaa = (QS.Fx.Reflection.LibraryAttribute)_aa[0];
            _id = _aaa.ID;
            _incarnation = _aaa.Version;
            _name = _aaa.Name;
            _description = _aaa.Description;
        }

        #endregion

        #region ILibrary Members

        QS.Fx.Reflection.IValueClass QS.Fx.Interface.Classes.ILibrary.GetValueClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetValueClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IInterfaceClass QS.Fx.Interface.Classes.ILibrary.GetInterfaceClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetInterfaceClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IEndpointClass QS.Fx.Interface.Classes.ILibrary.GetEndpointClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetEndpointClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IObjectClass QS.Fx.Interface.Classes.ILibrary.GetObjectClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetObjectClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IComponentClass QS.Fx.Interface.Classes.ILibrary.GetComponentClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetComponentClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IInterfaceConstraintClass QS.Fx.Interface.Classes.ILibrary.GetInterfaceConstraintClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetInterfaceConstraintClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IEndpointConstraintClass QS.Fx.Interface.Classes.ILibrary.GetEndpointConstraintClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetEndpointConstraintClass(_id, _incarnation);
            }
        }

        QS.Fx.Reflection.IObjectConstraintClass QS.Fx.Interface.Classes.ILibrary.GetObjectConstraintClass(string id)
        {
            lock (this)
            {
                QS.Fx.Base.ID _namespace_id, _id;
                ulong _namespace_incarnation, _incarnation;
                _StringToComponentID(id, out _namespace_id, out _namespace_incarnation, out _id, out _incarnation);
                Namespace_ _namespace = _GetNamespace(_namespace_id, _namespace_incarnation);
                return _namespace._GetObjectConstraintClass(_id, _incarnation);
            }
        }

        #endregion

        #region _RegisterClass

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IValueClass _valueclass)
        {
            if (!_valueclasses.ContainsKey(_underlyingtype))
                _valueclasses.Add(_underlyingtype, _valueclass);
            else
                throw new Exception("Value class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IInterfaceClass _interfaceclass)
        {
            if (!_interfaceclasses.ContainsKey(_underlyingtype))
                _interfaceclasses.Add(_underlyingtype, _interfaceclass);
            else
                throw new Exception("Interface class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IEndpointClass _endpointclass)
        {
            if (!_endpointclasses.ContainsKey(_underlyingtype))
                _endpointclasses.Add(_underlyingtype, _endpointclass);
            else
                throw new Exception("Endpoint class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IObjectClass _objectclass)
        {
            if (!_objectclasses.ContainsKey(_underlyingtype))
                _objectclasses.Add(_underlyingtype, _objectclass);
            else
                throw new Exception("Object class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IComponentClass _componentclass)
        {
            if (!_componentclasses.ContainsKey(_underlyingtype))
                _componentclasses.Add(_underlyingtype, _componentclass);
            else
                throw new Exception("Component class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IInterfaceConstraintClass _interfaceconstraintclass)
        {
            if (!_interfaceconstraintclasses.ContainsKey(_underlyingtype))
                _interfaceconstraintclasses.Add(_underlyingtype, _interfaceconstraintclass);
            else
                throw new Exception("Interface constraint class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IEndpointConstraintClass _endpointconstraintclass)
        {
            if (!_endpointconstraintclasses.ContainsKey(_underlyingtype))
                _endpointconstraintclasses.Add(_underlyingtype, _endpointconstraintclass);
            else
                throw new Exception("Endpoint constraint class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        private void _RegisterClass(Type _underlyingtype, QS.Fx.Reflection.IObjectConstraintClass _objectconstraintclass)
        {
            if (!_objectconstraintclasses.ContainsKey(_underlyingtype))
                _objectconstraintclasses.Add(_underlyingtype, _objectconstraintclass);
            else
                throw new Exception("Object constraint class for type \"" + _underlyingtype.FullName + "\" has already been registered.");
        }

        #endregion

        #region _GetNamespaceOfType

        private Namespace_ _GetNamespaceOfType(Type _type)
        {
            System.Reflection.Assembly _a = _type.Assembly;
            object[] _aa = _a.GetCustomAttributes(typeof(QS.Fx.Reflection.LibraryAttribute), false);
            if (_aa.Length != 1)
                throw new Exception("Assembly \"" + _a.FullName + "\" has not defined \"LibraryAttribute\".");
            QS.Fx.Reflection.LibraryAttribute _aaa = (QS.Fx.Reflection.LibraryAttribute) _aa[0];
            QS.Fx.Base.ID _id = _aaa.ID;
            ulong _incarnation = _aaa.Version;
            Namespace_ _namespace;
            if (!_namespaces.TryGetValue(_id, out _namespace))
                throw new Exception("Cannot resolve type \"" + _type.FullName + "\" because library \"" + _id.ToString() + 
                    "\" in which it has been defined has not been registered; there's a possible illegal dependency between different component libraries.");
            if (_namespace.Incarnation != _incarnation)
                throw new Exception("Cannot resolve type \"" + _type.FullName + "\" because library \"" + _id.ToString() +
                    "\" in which it has been defined has version number " + _incarnation.ToString() + ", which is different from version number " + 
                    _namespace.Incarnation.ToString() + " with which this library has been registered.");
            return _namespace;
        }

        #endregion

        #region LibraryType

//        public enum LibraryType
//        {
//            Managed, Unmanaged
//        }

        #endregion

        #region Metadata

        [System.Xml.Serialization.XmlType("Library")]
        public sealed class _library_Metadata_
        {
            public _library_Metadata_()
            {
            }

            private QS.Fx.Base.ID _id;
            private ulong _version;
            private string _name, _description;
            private _library_Library_[] _libraries;

            [System.Xml.Serialization.XmlAttribute("id")]
            public string ID_
            {
                get 
                {
                    return this._id.ToString() + "`" + this._version.ToString();
                }

                set 
                {
                    int p = value.IndexOf('`');
                    if (p >= 0 && p < value.Length)
                    {
                        this._id = (p > 0) ? new QS.Fx.Base.ID(value.Substring(0, p)) : QS.Fx.Base.ID._0;
                        this._version = (p < (value.Length - 1)) ? Convert.ToUInt64(value.Substring(p + 1)) : 0;
                    }
                    else
                    {
                        this._id = new QS.Fx.Base.ID(value);
                        this._version = 0;
                    }
                }
            }

            [System.Xml.Serialization.XmlAttribute("name")]
            public string Name
            {
                get { return this._name; }
                set { this._name = value; }
            }

            [System.Xml.Serialization.XmlAttribute("description")]
            public string Description
            {
                get { return this._description; }
                set { this._description = value; }
            }

            [System.Xml.Serialization.XmlIgnore]
            public QS.Fx.Base.ID ID
            {
                get { return this._id; }
                set { this._id = value; }
            }

            [System.Xml.Serialization.XmlIgnore]
            public ulong Version
            {
                get { return this._version; }
                set { this._version = value; }
            }

            [System.Xml.Serialization.XmlElement("Include")]
            public _library_Library_[] Libraries
            {
                get { return _libraries; }
                set { _libraries = value; }
            }

            [System.Xml.Serialization.XmlType("Include")]
            public sealed class _library_Library_
            {
                public _library_Library_()
                {
                }

                private string filename;
//                private LibraryType type;

                [System.Xml.Serialization.XmlAttribute("filename")]
                public string FileName
                {
                    get { return filename; }
                    set { filename = value; }
                }

//                [System.Xml.Serialization.XmlAttribute("type")]
//                public LibraryType Type
//                {
//                    get { return type; }
//                    set { type = value; }
//                }
            }
        }

        #endregion

        #region _Load

        public void _Load()
        {
            lock (this)
            {
                string _librariesroot = _LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + "libraries";
                if (Directory.Exists(_librariesroot))
                {
                    foreach (string _folder in Directory.GetDirectories(_librariesroot))
                    {
                        QS.Fx.Base.ID _namespace_id = new QS.Fx.Base.ID(
                            _folder.Substring(_folder.LastIndexOfAny(new char[] { '\\', '/', Path.DirectorySeparatorChar }) + 1));
                        if (!this._namespaces.ContainsKey(_namespace_id))
                        {
                            _GetNamespace(_namespace_id, 0);
                        }
                    }
                }
            }
        }

        #endregion

        #region _GetNamespace

        private Namespace_ _GetNamespace(QS.Fx.Base.ID _namespace_id, ulong _namespace_incarnation)
        {
            Namespace_ _namespace;
            if (_namespaces.TryGetValue(_namespace_id, out _namespace))
            {
                if (_namespace.Incarnation < _namespace_incarnation)
                    throw new Exception("Cannot load a component from library \"" + _namespace.ID.ToString() + " because the version " +
                        _namespace.Incarnation.ToString() + " of the library, which is already loaded into the process, is lower than the version " +
                        _namespace_incarnation.ToString() + " required by the component; to be able to run this component, you must terminate the " +
                        "current process and update your runtime environment.");
                else
                    return _namespace;
            }
            else
            {
                string _component_root_0 = _LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar +
                    "libraries" + Path.DirectorySeparatorChar + _namespace_id.ToString();

                if (!Directory.Exists(_component_root_0))
                {
                    // TODO: We should load the component from the web!

                    // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ HERE @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                        " because the library is not installed on the system and the automatic library download feature has not been configured.");
                }

                string _component_root = null;
                ulong _i = 0;
                foreach (string _x in Directory.GetDirectories(_component_root_0, "*", SearchOption.TopDirectoryOnly))
                {
                    string _y = _x.Substring(_x.LastIndexOfAny(new char[] { '\\', '/', Path.DirectorySeparatorChar }) + 1);
                    try
                    {
                        ulong _i_ = Convert.ToUInt64(_y);
                        if (_i_ > _i)
                        {
                            _i = _i_;
                            _component_root = _x;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (_component_root == null)
                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                        " because no versions of the library have been installed at \"" + _component_root_0 + "\".");

                string _metadata_filename = _component_root + Path.DirectorySeparatorChar + "metadata.xml";
                if (!File.Exists(_metadata_filename))
                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() + "`" + _i.ToString() +
                        " because the library metadata file is missing at \"" + _metadata_filename + "\".");

                _library_Metadata_ _metadata;
                try
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(_metadata_filename))
                    {
                        _metadata = (_library_Metadata_)(new System.Xml.Serialization.XmlSerializer(typeof(_library_Metadata_))).Deserialize(reader);
                    }
                }
                catch (Exception exc)
                {
                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() + "`" + _i.ToString() +
                        " because the library metadata file could not be loaded.", exc);
                }
                if (!(_metadata.ID.Equals(_namespace_id) && _metadata.Version.Equals(_i)))
                {
                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() + "`" + _i.ToString() +
                        " because the metadata at \"" + _metadata_filename + "\" is corrupt.");
                }
                if (_metadata.Version < _namespace_incarnation)
                {
                    throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                        " because the installed version is " + _metadata.Version.ToString() + 
                        ", which is smaller than the requested version " + _namespace_incarnation.ToString() + ".");
                }
                string _component_root_data = _component_root + System.IO.Path.DirectorySeparatorChar + "data";

                List<Type> todo = new List<Type>();
                List<System.Reflection.Assembly> _m_assemblies = new List<System.Reflection.Assembly>();
                foreach (_library_Metadata_._library_Library_ _m_library in _metadata.Libraries)
                {
                    if (_m_library.FileName.Contains("\\") || _m_library.FileName.Contains("/") || _m_library.FileName.Contains(".."))
                        throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                            " because the path \"" + _m_library.FileName + "\" is illegal or points outside the library folder.");
                    string _assembly_filename = System.IO.Path.GetFullPath(System.IO.Path.Combine(_component_root_data, _m_library.FileName));
                    if (!System.IO.File.Exists(_assembly_filename))
                        throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                            " because the component \"" + _assembly_filename + "\" is not present in the local filesystem.");
                    bool ismanaged_ = true; 
                    // HACK: commented out: QS._core_x_.Reflection.Library.IsManaged(_assembly_filename);
                    if (ismanaged_)
                    {
                        System.Reflection.Assembly _m_assembly;
                        try
                        {
                            _m_assembly = System.Reflection.Assembly.LoadFrom(_assembly_filename);
                        }
                        catch (Exception exc)
                        {
                            throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                                " because the component assembly \"" + _assembly_filename + "\" could not be loaded.", exc);
                        }
                        _m_assemblies.Add(_m_assembly);
                        Type[] _m_types;
                        try
                        {
                            _m_types = _m_assembly.GetTypes();
                        }
                        catch (Exception exc)
                        {
                            throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                                " because the list of types in the component assembly \"" + _m_assembly.FullName.ToString() + 
                                "\" could not be retrieved through reflection.", exc);
                        }
                        QS.Fx.Base.ID _m_namespace_id;
                        ulong _m_namespace_incarnation;
                        string _m_namespace_name, _m_namespace_description;
                        _GetLibraryAttributes(_m_assembly, 
                            out _m_namespace_id, out _m_namespace_incarnation, out _m_namespace_name, out _m_namespace_description);
                        if (!_m_namespace_id.Equals(_namespace_id))
                        {
                            throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                                " because the installed component assembly \"" + _m_assembly.FullName.ToString() +
                                "\" is marked as a part of a different library \"" + _m_namespace_id.ToString() + "\".");
                        }
                        if (_m_namespace_incarnation != _metadata.Version)
                        {
                            throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                                " because the installed component assembly \"" + _m_assembly.FullName.ToString() +
                                "\" has a version number " + _m_namespace_incarnation.ToString() + ", which is different than the version number " + 
                                _namespace_incarnation.ToString() + " defined in the metadata.");
                        }
                        todo.AddRange(_m_types);
                    }
                    else
                        throw new Exception("Cannot load a component from library \"" + _namespace_id.ToString() +
                            " because the component \"" + _assembly_filename + 
                            "\" looks like an unmanaged component, and unmanaged components are not yet supported in this release.");
                }

                _namespace = new Namespace_(
                    this, _namespace_id, _metadata.Version, _metadata.Name, _metadata.Description, _m_assemblies);

                _namespaces.Add(_namespace_id, _namespace);

                _start_compilation();

                for (int _pass = 1; _pass <= 2; _pass++)
                {
                    foreach (Type type in todo)
                    {
                        _Register(_pass, type);
                    }
                }

                _stop_compilation();

                return _namespace;
            }
        }

        #endregion

        #region Namespace_

        internal class Namespace_ : QS.Fx.Inspection.Inspectable, QS._qss_x_.Interface_.Classes_.ILibrary2_
        {
            #region Constructor

            public Namespace_(Library _library, QS.Fx.Base.ID _namespace_id, ulong _namespace_incarnation, 
                string _namespace_name, string _namespace_description, IEnumerable<System.Reflection.Assembly> _assemblies)
            {
                this._library = _library;
                this._namespace_id = _namespace_id;
                this._namespace_incarnation = _namespace_incarnation;
                this._namespace_name = _namespace_name;
                this._namespace_description = _namespace_description;
                foreach (System.Reflection.Assembly _a in _assemblies)
                    this._assemblies.Add(_a);
                this._autogeneratedid = _namespace_prefix  + _namespace_id.ToString() + "_" + _namespace_incarnation.ToString();
                this._uuid = (this._namespace_id != null) ? (this._namespace_id.ToString() + "`" + this._namespace_incarnation.ToString()) : null;
            }

            internal const string _namespace_prefix = "liveobjects.autogenerated_";

            #endregion

            #region Fields

            private Library _library;
            [QS.Fx.Base.Inspectable]
            private QS.Fx.Base.ID _namespace_id;
            [QS.Fx.Base.Inspectable]
            private ulong _namespace_incarnation;
            [QS.Fx.Base.Inspectable]
            private string _namespace_name, _namespace_description;
            private string _autogeneratedid;
            private string _uuid;
            private IList<System.Reflection.Assembly> _assemblies = new List<System.Reflection.Assembly>();

            // private List<System.Reflection.Assembly> _assemblies = new List<System.Reflection.Assembly>();

            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass> _valueclasses = new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass> _interfaceclasses = new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass> _endpointclasses = new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass> _objectclasses = new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass> _componentclasses = new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass>();

            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceConstraintClass> _interfaceconstraintclasses =
                new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceConstraintClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointConstraintClass> _endpointconstraintclasses =
                new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointConstraintClass>();
            private IDictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectConstraintClass> _objectconstraintclasses =
                new Dictionary<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectConstraintClass>();

            #endregion

            #region Inspection

#if DEBUG_INCLUDE_INSPECTION_CODE

            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass> __inspectable_valueclasses__;
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass> __inspectable_interfaceclasses__;
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass> __inspectable_endpointclasses__;
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass> __inspectable_objectclasses__;
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass> __inspectable_componentclasses__;

            [QS.Fx.Base.Inspectable("_valueclasses")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass> __inspectable_valueclasses
            {
                get
                {
                    lock (this)
                    {
                        if (__inspectable_valueclasses__ == null)
                            __inspectable_valueclasses__ =
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass>("_valueclasses", _valueclasses,
                                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IValueClass>.ConversionCallback(
                                        QS.Fx.Base.ID.FromString));
                        return __inspectable_valueclasses__;
                    }
                }
            }

            [QS.Fx.Base.Inspectable("_interfaceclasses")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass> __inspectable_interfaceclasses
            {
                get
                {
                    lock (this)
                    {
                        if (__inspectable_interfaceclasses__ == null)
                            __inspectable_interfaceclasses__ =
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass>("_interfaceclasses", _interfaceclasses,
                                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IInterfaceClass>.ConversionCallback(
                                        QS.Fx.Base.ID.FromString));
                        return __inspectable_interfaceclasses__;
                    }
                }
            }

            [QS.Fx.Base.Inspectable("_endpointclasses")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass> __inspectable_endpointclasses
            {
                get
                {
                    lock (this)
                    {
                        if (__inspectable_endpointclasses__ == null)
                            __inspectable_endpointclasses__ =
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass>("_endpointclasses", _endpointclasses,
                                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IEndpointClass>.ConversionCallback(
                                        QS.Fx.Base.ID.FromString));
                        return __inspectable_endpointclasses__;
                    }
                }
            }

            [QS.Fx.Base.Inspectable("_objectclasses")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass> __inspectable_objectclasses
            {
                get
                {
                    lock (this)
                    {
                        if (__inspectable_objectclasses__ == null)
                            __inspectable_objectclasses__ =
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass>("_objectclasses", _objectclasses,
                                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IObjectClass>.ConversionCallback(
                                        QS.Fx.Base.ID.FromString));
                        return __inspectable_objectclasses__;
                    }
                }
            }

            [QS.Fx.Base.Inspectable("_componentclasses")]
            private QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass> __inspectable_componentclasses
            {
                get
                {
                    lock (this)
                    {
                        if (__inspectable_componentclasses__ == null)
                            __inspectable_componentclasses__ =
                                new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass>("_componentclasses", _componentclasses,
                                    new QS._qss_e_.Inspection_.DictionaryWrapper1<QS.Fx.Base.ID, QS.Fx.Reflection.IComponentClass>.ConversionCallback(
                                        QS.Fx.Base.ID.FromString));
                        return __inspectable_componentclasses__;
                    }
                }
            }

#endif

            #endregion

            #region Accessors

            public QS.Fx.Base.ID ID
            {
                get { return _namespace_id; }
                set { _namespace_id = value; }
            }

            public ulong Incarnation
            {
                get { return _namespace_incarnation; }
                set { _namespace_incarnation = value; }
            }

            internal string autogenerated_id_
            {
                get { return this._autogeneratedid; }
            }

            internal string uuid_
            {
                get { return this._uuid; }
            }

            internal IEnumerable<System.Reflection.Assembly> Assemblies_
            {
                get 
                {
                    lock (this)
                    {
                        return this._assemblies;
                    }
                }
            }

            internal void _AddAssembly(System.Reflection.Assembly _a)
            {
                lock (this)
                {
                    this._assemblies.Add(_a);
                }
            }

            #endregion

            #region _RegisterClass

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IValueClass _valueclass)
            {
                if (!_valueclasses.ContainsKey(_id))
                    _valueclasses.Add(_id, _valueclass);
                else
                    throw new Exception("Value class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IInterfaceClass _interfaceclass)
            {
                if (!_interfaceclasses.ContainsKey(_id))
                    _interfaceclasses.Add(_id, _interfaceclass);
                else
                    throw new Exception("Interface class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IEndpointClass _endpointclass)
            {
                if (!_endpointclasses.ContainsKey(_id))
                    _endpointclasses.Add(_id, _endpointclass);
                else
                    throw new Exception("Endpoint class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IObjectClass _objectclass)
            {
                if (!_objectclasses.ContainsKey(_id))
                    _objectclasses.Add(_id, _objectclass);
                else
                    throw new Exception("Object class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IComponentClass _componentclass)
            {
                if (!_componentclasses.ContainsKey(_id))
                    _componentclasses.Add(_id, _componentclass);
                else
                    throw new Exception("Component class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IInterfaceConstraintClass _interfaceconstraintclass)
            {
                if (!_interfaceconstraintclasses.ContainsKey(_id))
                    _interfaceconstraintclasses.Add(_id, _interfaceconstraintclass);
                else
                    throw new Exception("Interface constraint class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IEndpointConstraintClass _endpointconstraintclass)
            {
                if (!_endpointconstraintclasses.ContainsKey(_id))
                    _endpointconstraintclasses.Add(_id, _endpointconstraintclass);
                else
                    throw new Exception("Endpoint constraint class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            public void _RegisterClass(QS.Fx.Base.ID _id, QS.Fx.Reflection.IObjectConstraintClass _objectconstraintclass)
            {
                if (!_objectconstraintclasses.ContainsKey(_id))
                    _objectconstraintclasses.Add(_id, _objectconstraintclass);
                else
                    throw new Exception("Object constraint class with id = \"" + _id.ToString() + "\" has already been registered in library \"" + this._namespace_id + "\".");
            }

            #endregion

            #region _GetValueClass

            public QS.Fx.Reflection.IValueClass _GetValueClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IValueClass _valueclass;
                if (!_valueclasses.TryGetValue(_id, out _valueclass))
                    throw new Exception("Value class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_valueclass.Incarnation < _incarnation)
                    throw new Exception("Value class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " + 
                        _valueclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _valueclass;
            }

            #endregion

            #region _GetInterfaceClass

            public QS.Fx.Reflection.IInterfaceClass _GetInterfaceClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IInterfaceClass _interfaceclass;
                if (!_interfaceclasses.TryGetValue(_id, out _interfaceclass))
                    throw new Exception("Interface class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_interfaceclass.Incarnation < _incarnation)
                    throw new Exception("Interface class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _interfaceclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _interfaceclass;
            }

            #endregion

            #region _GetEndpointClass

            public QS.Fx.Reflection.IEndpointClass _GetEndpointClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IEndpointClass _endpointclass;
                if (!_endpointclasses.TryGetValue(_id, out _endpointclass))
                    throw new Exception("Endpoint class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_endpointclass.Incarnation < _incarnation)
                    throw new Exception("Endpoint class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _endpointclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _endpointclass;
            }

            #endregion

            #region _GetObjectClass

            public QS.Fx.Reflection.IObjectClass _GetObjectClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IObjectClass _objectclass;
                if (!_objectclasses.TryGetValue(_id, out _objectclass))
                    throw new Exception("Object class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_objectclass.Incarnation < _incarnation)
                    throw new Exception("Object class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _objectclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _objectclass;
            }

            #endregion

            #region _GetComponentClass

            public QS.Fx.Reflection.IComponentClass _GetComponentClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IComponentClass _componentclass;
                if (!_componentclasses.TryGetValue(_id, out _componentclass))
                    throw new Exception("Component class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>) _componentclass).Incarnation < _incarnation)
                    throw new Exception("Component class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).Incarnation.ToString() + 
                        ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _componentclass;
            }

            #endregion

            #region _GetInterfaceConstraintClass

            public QS.Fx.Reflection.IInterfaceConstraintClass _GetInterfaceConstraintClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IInterfaceConstraintClass _interfaceconstraintclass;
                if (!_interfaceconstraintclasses.TryGetValue(_id, out _interfaceconstraintclass))
                    throw new Exception("Interface constraint class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_interfaceconstraintclass.Incarnation < _incarnation)
                    throw new Exception("Interface constraint class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _interfaceconstraintclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _interfaceconstraintclass;
            }

            #endregion

            #region _GetEndpointConstraintClass

            public QS.Fx.Reflection.IEndpointConstraintClass _GetEndpointConstraintClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IEndpointConstraintClass _endpointconstraintclass;
                if (!_endpointconstraintclasses.TryGetValue(_id, out _endpointconstraintclass))
                    throw new Exception("Endpoint constraint class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_endpointconstraintclass.Incarnation < _incarnation)
                    throw new Exception("Endpoint constraint class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _endpointconstraintclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _endpointconstraintclass;
            }

            #endregion

            #region _GetObjectConstraintClass

            public QS.Fx.Reflection.IObjectConstraintClass _GetObjectConstraintClass(QS.Fx.Base.ID _id, ulong _incarnation)
            {
                QS.Fx.Reflection.IObjectConstraintClass _objectconstraintclass;
                if (!_objectconstraintclasses.TryGetValue(_id, out _objectconstraintclass))
                    throw new Exception("Object constraint class " + _id.ToString() + " has not been defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + ".");
                if (_objectconstraintclass.Incarnation < _incarnation)
                    throw new Exception("Object constraint class " + _id.ToString() + " defined in the currently loaded version " +
                        _namespace_incarnation.ToString() + " of library " + _namespace_id.ToString() + " has a version number " +
                        _objectconstraintclass.Incarnation.ToString() + ", which is lower than the requested version number " + _incarnation.ToString() + ".");
                return _objectconstraintclass;
            }

            #endregion

            #region ILibrary2_ Members

            IEnumerable<QS.Fx.Reflection.IValueClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ValueClasses()
            {
                return new List<QS.Fx.Reflection.IValueClass>(this._valueclasses.Values);
            }

            IEnumerable<QS.Fx.Reflection.IInterfaceClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.InterfaceClasses()
            {
                return new List<QS.Fx.Reflection.IInterfaceClass>(this._interfaceclasses.Values);
            }

            IEnumerable<QS.Fx.Reflection.IEndpointClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.EndpointClasses()
            {
                return new List<QS.Fx.Reflection.IEndpointClass>(this._endpointclasses.Values);
            }

            IEnumerable<QS.Fx.Reflection.IObjectClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ObjectClasses()
            {
                return new List<QS.Fx.Reflection.IObjectClass>(this._objectclasses.Values);
            }

            IEnumerable<QS.Fx.Reflection.IComponentClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ComponentClasses()
            {
                return new List<QS.Fx.Reflection.IComponentClass>(this._componentclasses.Values);
            }

            #endregion
        }

        #endregion

        #region _Register

        private void _Register(int _pass, Type _type)
        {
            switch (_pass)
            {
                case 1:
                    {
                        _RegisterInterfaceConstraintClass(_type, false);
                        _RegisterEndpointConstraintClass(_type, false);
                        _RegisterObjectConstraintClass(_type, false);
                    }
                    break;

                case 2:
                    {
                        _RegisterValueClass(_type, false);
                        _RegisterInterfaceClass(_type, false);
                        _RegisterEndpointClass(_type, false);
                        _RegisterObjectClass(_type, false);
                        _RegisterComponentClass(_type, false);
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region _RegisterValueClass(string,Type)

        private void _RegisterValueClass(Namespace_ _namespace, string _id, ulong _incarnation, Type _type)
        {
            IDictionary<string, QS.Fx.Reflection.IParameter> _parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            if (_type.IsGenericTypeDefinition)
            {
                foreach (Type _argument in _type.GetGenericArguments())
                {
                    QS.Fx.Reflection.IParameter _parameter = new QS.Fx.Reflection.Parameter(_argument.Name, null, QS.Fx.Reflection.ParameterClass.ValueClass, null, null, null);
                    _parameters.Add(_parameter.ID, _parameter);
                }
            }
            QS.Fx.Base.ID __id = new QS.Fx.Base.ID(_id);
            ValueClass _valueclass = new ValueClass(
                null, _namespace, __id, _incarnation, _type.Name, 
                "This is a value class based on the .NET type \"" + _type.FullName + "\".", _type, _parameters, _parameters);
            _namespace._RegisterClass(__id, _valueclass);
            this._RegisterClass(_type, _valueclass);
        }

        #endregion

        #region _RegisterValueClass(Type,bool)

        private QS.Fx.Reflection.IValueClass _RegisterValueClass(Type _type, bool _force)
        {
            if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                throw new Exception("Cannot register type \"" + _type.ToString() +
                    "\" as a value class because it is a generic type, but not a generic type definition.");
            QS.Fx.Reflection.IValueClass _valueclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ValueClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                QS.Fx.Reflection.ValueClassAttribute _attribute = (QS.Fx.Reflection.ValueClassAttribute)_attributes[0];
                if (!_valueclasses.TryGetValue(_type, out _valueclass))
                {
                    IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
                    _GetParametersOf(_type, false, out _classparameters, out _openparameters);
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    ValueClass _m_valueclass = new ValueClass(null, _namespace, _attribute.ID, _attribute.Version,
                        ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                        ((_attribute.Comment != null) ? _attribute.Comment : "This is a value class based on the .NET type \"" + _type.FullName + "\"."),
                        _type, _classparameters, _openparameters);
                    _valueclass = _m_valueclass;
                    this._RegisterClass(_type, _valueclass);
                    _namespace._RegisterClass(_attribute.ID, _valueclass);
                    QS._qss_x_.Reflection_.Internal_._internal_info_valueclass _internal_info;
                    Internal_._internal._generate_valueclass_serialization_code(
                        _type, _namespace.ID, _namespace.Incarnation, _valueclass.ID, _valueclass.Incarnation,
                        _namespace.autogenerated_id_, _m_valueclass.autogenerated_id_, out _internal_info);
                    _compilation._source.AppendLine(_internal_info._serialization_code);
                    _m_valueclass.internal_info_ = _internal_info;
                    _compilation._types.Add(_type);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a value class because it has not been decorated with the \"ValueClass\" attribute.");
            }
            return _valueclass;
        }

        #endregion

        #region _RegisterInterfaceClass

        private QS.Fx.Reflection.IInterfaceClass _RegisterInterfaceClass(Type _type, bool _force)
        {
            if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                throw new Exception("Cannot register type \"" + _type.ToString() + 
                    "\" as an interface class because it is a generic type, but not a generic type definition.");
            QS.Fx.Reflection.IInterfaceClass _interfaceclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.InterfaceClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (!_type.IsInterface)
                    throw new Exception("Cannot register type \"" + _type.ToString() + 
                        "\" as an interface class because it is not an interface type.");
                QS.Fx.Reflection.InterfaceClassAttribute _attribute = (QS.Fx.Reflection.InterfaceClassAttribute) _attributes[0];
                if (!_interfaceclasses.TryGetValue(_type, out _interfaceclass))
                {
                    IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
                    _GetParametersOf(_type, false, out _classparameters, out _openparameters);
                    IDictionary<string, QS.Fx.Reflection.IOperation> _operations = new Dictionary<string, QS.Fx.Reflection.IOperation>();

                    System.Reflection.BindingFlags _flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
                    if (_type.GetEvents(_flags).Length > 0)
                        throw new Exception("Cannot register type \"" + _type.ToString() +
                            "\" as an interface class because it contains event declarations.");
                    if (_type.GetProperties(_flags).Length > 0)
                        throw new Exception("Cannot register type \"" + _type.ToString() +
                            "\" as an interface class because it contains property declarations.");
                    foreach (System.Reflection.MethodInfo _methodinfo in _type.GetMethods(_flags))
                    {
                        object[] _operationattributes = _methodinfo.GetCustomAttributes(typeof(QS.Fx.Reflection.OperationAttribute), true);
                        if (_operationattributes != null && _operationattributes.Length > 0)
                        {
                            QS.Fx.Reflection.OperationAttribute _operationattribute = (QS.Fx.Reflection.OperationAttribute) _operationattributes[0];
                            IDictionary<string, QS.Fx.Reflection.IValue> _incoming = new Dictionary<string, QS.Fx.Reflection.IValue>();
                            IDictionary<string, QS.Fx.Reflection.IValue> _outgoing = new Dictionary<string, QS.Fx.Reflection.IValue>();
                            if (!_methodinfo.ReturnType.Equals(typeof(void)))
                                _outgoing.Add(string.Empty,
                                    new Value(string.Empty, _GetValueClass(_methodinfo.ReturnType, _classparameters), _methodinfo.ReturnParameter));
                            foreach (System.Reflection.ParameterInfo _parameterinfo in _methodinfo.GetParameters())
                            {
                                if (!_parameterinfo.IsRetval)
                                {
                                    Type _underlyingtype = _parameterinfo.ParameterType;
                                    if (_underlyingtype.IsByRef)
                                        _underlyingtype = _underlyingtype.GetElementType();
                                    QS.Fx.Reflection.IValue _value = new Value(_parameterinfo.Name, _GetValueClass(_underlyingtype, _classparameters), _parameterinfo);
                                    if (_parameterinfo.ParameterType.IsByRef)
                                    {
                                        _outgoing.Add(_parameterinfo.Name, _value);                                            
                                        if (!_parameterinfo.IsOut)
                                            _incoming.Add(_parameterinfo.Name, _value);                                            
                                    }
                                    else
                                        _incoming.Add(_parameterinfo.Name, _value);                                            
                                }
                            }
                            Operation _operation = new Operation(_operationattribute.ID, 
                                new OperationClass(new MessageClass(_incoming), new MessageClass(_outgoing)), _methodinfo);
                            try
                            {
                                _operations.Add(_operationattribute.ID, _operation);
                            }
                            catch (Exception _exc)
                            {
                                throw new Exception("foo", _exc);
                            }
                        }
                        else
                            throw new Exception("Cannot register type \"" + _type.ToString() +
                                "\" as an interface class because it contains a declaration of a method \"" + _methodinfo.Name + 
                                "\" that has not been decorated with the \"Operation\" attribute.");
                    }
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    InterfaceClass _m_interfaceclass = new InterfaceClass(null, _namespace, _attribute.ID, _attribute.Version,
                        ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                        ((_attribute.Comment != null) ? _attribute.Comment : "This is an interface class based on the .NET type \"" + _type.FullName + "\"."),
                        _type, _classparameters, _openparameters, _operations);
                    _interfaceclass = _m_interfaceclass;
                    this._RegisterClass(_type, _interfaceclass);
                    _namespace._RegisterClass(_attribute.ID, _interfaceclass);

                    QS._qss_x_.Reflection_.Internal_._internal_info_interfaceclass _internal_info;
                    Internal_._internal._generate_interface_frontend_and_backend(
                        _type, _namespace.ID, _namespace.Incarnation, _interfaceclass.ID, _interfaceclass.Incarnation, 
                        _namespace.autogenerated_id_, _m_interfaceclass.autogenerated_id_, out _internal_info);
                    _compilation._source.AppendLine(_internal_info._frontend_code);
                    _compilation._source.AppendLine(_internal_info._backend_code);
                    _compilation._source.AppendLine(_internal_info._interceptor_code);
                    _m_interfaceclass.internal_info_ = _internal_info;
                    _compilation._types.Add(_type);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() + 
                        "\" as an interface class because it has not been decorated with the \"InterfaceClass\" attribute.");
            }
            return _interfaceclass;
        }

        #endregion

        #region _RegisterEndpointClass

        private QS.Fx.Reflection.IEndpointClass _RegisterEndpointClass(Type _type, bool _force)
        {
            if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                throw new Exception("Cannot register type \"" + _type.ToString() +
                    "\" as an endpoint class because it is a generic type, but not a generic type definition.");
            QS.Fx.Reflection.IEndpointClass _endpointclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (!_type.IsInterface)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an endpoint class because it is not an interface type.");
                if (!typeof(QS.Fx.Endpoint.Classes.IEndpoint).IsAssignableFrom(_type))
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an endpoint class because it is not derived from \"" + typeof(QS.Fx.Endpoint.Classes.IEndpoint) + "\".");
                QS.Fx.Reflection.EndpointClassAttribute _attribute = (QS.Fx.Reflection.EndpointClassAttribute)_attributes[0];
                if (!_endpointclasses.TryGetValue(_type, out _endpointclass))
                {
                    IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
                    _GetParametersOf(_type, false, out _classparameters, out _openparameters);
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    _endpointclass = new EndpointClass(null, _namespace, _attribute.ID, _attribute.Version,
                        ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                        ((_attribute.Comment != null) ? _attribute.Comment : "This is an endpoint class based on the .NET type \"" + _type.FullName + "\"."),
                        _type, _classparameters, _openparameters);
                    this._RegisterClass(_type, _endpointclass);
                    _namespace._RegisterClass(_attribute.ID, _endpointclass);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an endpoint class because it has not been decorated with the \"EndpointClass\" attribute.");
            }
            return _endpointclass;
        }

        #endregion

        #region _RegisterObjectClass

        private static readonly Type _authenticatedobjecttype = 
            typeof(QS.Fx.Object.Classes.IAuthenticatedObject<QS.Fx.Object.Classes.IObject>).GetGenericTypeDefinition();

        private QS.Fx.Reflection.IObjectClass _RegisterObjectClass(Type _type, bool _force)
        {
            if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                throw new Exception("Cannot register type \"" + _type.ToString() +
                    "\" as an object class because it is a generic type, but not a generic type definition.");
            QS.Fx.Reflection.IObjectClass _objectclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ObjectClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (!_type.IsInterface)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an object class because it is not an interface type.");
                if (!typeof(QS.Fx.Object.Classes.IObject).IsAssignableFrom(_type))
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an object class because it is not derived from \"" + typeof(QS.Fx.Object.Classes.IObject) + "\".");
                Type _authoritytype = null;
                foreach (Type _inheritedtype in _type.GetInterfaces())
                {
                    if (!_inheritedtype.Equals(typeof(QS.Fx.Object.Classes.IObject)))
                    {
                        bool _isbad = true;
                        if (_inheritedtype.IsGenericType)
                        {
                            Type _inheritedtypetenmplate = _inheritedtype.GetGenericTypeDefinition();
                            if (_inheritedtypetenmplate.Equals(_authenticatedobjecttype))
                            {
                                Type[] _inheritedtypetenmplateargs = _inheritedtype.GetGenericArguments();
                                if ((_inheritedtypetenmplateargs != null) && (_inheritedtypetenmplateargs.Length == 1))
                                {
                                    if (_authoritytype == null)
                                    {
                                        _authoritytype = _inheritedtypetenmplateargs[0];
                                        _isbad = false;
                                    }
                                }
                            }
                        }
                        if (_isbad)
                            throw new Exception("Cannot register type \"" + _type.ToString() +
                                "\" as an object class because it is derived from a type \"" + _inheritedtype.FullName +
                                "\", different from \"" + typeof(QS.Fx.Object.Classes.IObject) + "\" and \"" + 
                                typeof(QS.Fx.Object.Classes.IAuthenticatedObject<QS.Fx.Object.Classes.IObject>) + "\", which is not permitted.");
                    }
                }
                QS.Fx.Reflection.ObjectClassAttribute _attribute = (QS.Fx.Reflection.ObjectClassAttribute)_attributes[0];
                if (!_objectclasses.TryGetValue(_type, out _objectclass))
                {
                    IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
                    _GetParametersOf(_type, false, out _classparameters, out _openparameters);
                    IDictionary<string, QS.Fx.Reflection.IEndpoint> _endpoints = new Dictionary<string, QS.Fx.Reflection.IEndpoint>();
                    foreach (System.Reflection.PropertyInfo _propertyinfo in 
                        _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                    {
                        object[] _endpointattributes = _propertyinfo.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointAttribute), true);
                        if (_endpointattributes != null && _endpointattributes.Length > 0)
                        {
                            QS.Fx.Reflection.EndpointAttribute _endpointattribute = (QS.Fx.Reflection.EndpointAttribute)_endpointattributes[0];
                            QS.Fx.Reflection.IEndpointClass _endpointclass = _GetEndpointClass(_propertyinfo.PropertyType, _classparameters);

                            Endpoint _endpoint = new Endpoint(_endpointattribute.ID, _endpointclass, _propertyinfo);
                            _endpoints.Add(_endpointattribute.ID, _endpoint);

                            object[] _endpointconstraintattributes;
                            _endpointconstraintattributes = _propertyinfo.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointConstraintAttribute), false);
                            if (_endpointconstraintattributes != null && _endpointconstraintattributes.Length > 0)
                            {
                                foreach (QS.Fx.Reflection.EndpointConstraintAttribute _endpointconstraintattribute in _endpointconstraintattributes)
                                {
                                    QS.Fx.Reflection.IEndpointConstraintClass _constraintclass =
                                        ((QS.Fx.Interface.Classes.ILibrary)this).GetEndpointConstraintClass(_endpointconstraintattribute.ConstraintClass);
                                    QS.Fx.Reflection.IEndpointConstraint _constraint = _constraintclass.CreateConstraint();
                                    _constraint.Initialize(_endpointconstraintattribute.Constraint, _endpointclass);
                                    _endpoint._AddConstraint(
                                        _endpointconstraintattribute.ConstraintKind, 
                                        _endpointconstraintattribute.ConstraintClass,
                                        new EndpointConstraint((EndpointConstraintClass) _constraintclass, _constraint));
                                }
                            }
                        }
                    }

                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    ObjectClass _m_objectclass =  new ObjectClass(null, _namespace, _attribute.ID, _attribute.Version,
                        ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                        ((_attribute.Comment != null) ? _attribute.Comment : "This is an object class based on the .NET type \"" + _type.FullName + "\"."),
                        _type, _classparameters, _openparameters, _endpoints, null);
                    _objectclass = _m_objectclass;
                    this._RegisterClass(_type, _objectclass);
                    _namespace._RegisterClass(_attribute.ID, _objectclass);

                    object[] _objectconstraintattributes;
                    _objectconstraintattributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ObjectConstraintAttribute), false);
                    if (_objectconstraintattributes != null && _objectconstraintattributes.Length > 0)
                    {
                        foreach (QS.Fx.Reflection.ObjectConstraintAttribute _objectconstraintattribute in _objectconstraintattributes)
                        {
                            QS.Fx.Reflection.IObjectConstraintClass _constraintclass =
                                ((QS.Fx.Interface.Classes.ILibrary)this).GetObjectConstraintClass(_objectconstraintattribute.ConstraintClass);
                            QS.Fx.Reflection.IObjectConstraint _constraint = _constraintclass.CreateConstraint();
                            _constraint.Initialize(_objectconstraintattribute.Constraint, _objectclass);
                            _m_objectclass._AddConstraint(
                                _objectconstraintattribute.ConstraintKind, 
                                _objectconstraintattribute.ConstraintClass,
                                new ObjectConstraint((ObjectConstraintClass)_constraintclass, _constraint));
                        }
                    }

                    if (_authoritytype != null)
                    {
                        QS.Fx.Reflection.IObjectClass _authenticatingclass = _GetObjectClass(_authoritytype, _classparameters);
                        _m_objectclass._SetAuthenticatingClass(_authenticatingclass);
                    }

                    QS._qss_x_.Reflection_.Internal_._internal_info_objectclass _internal_info;
                    Internal_._internal._generate_object_frontend_and_backend(
                        _type, _namespace.ID, _namespace.Incarnation, _objectclass.ID, _objectclass.Incarnation,
                        _namespace.autogenerated_id_, _m_objectclass.autogenerated_id_,
                        out _internal_info);
                    _compilation._source.AppendLine(_internal_info._frontend_code);
                    _compilation._source.AppendLine(_internal_info._backend_code);
                    _m_objectclass.internal_info_ = _internal_info;
                    _compilation._types.Add(_type);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as an object class because it has not been decorated with the \"ObjectClass\" attribute.");
            }
            return _objectclass;
        }

        #endregion

        #region _RegisterComponentClass

        private QS.Fx.Reflection.IComponentClass _RegisterComponentClass(Type _type, bool _force)
        {
            if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                throw new Exception("Cannot register type \"" + _type.ToString() +
                    "\" as a component class because it is a generic type, but not a generic type definition.");
            QS.Fx.Reflection.IComponentClass _componentclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ComponentClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (!_type.IsClass)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a component class because it is not a class type.");
//                if (!typeof(QS.Fx.Component.Classes.Component).IsAssignableFrom(_type))
//                    throw new Exception("Cannot register type \"" + _type.ToString() +
//                        "\" as a component class because it is not derived from \"" + typeof(QS.Fx.Component.Classes.Component) + "\".");
                QS.Fx.Reflection.ComponentClassAttribute _attribute = (QS.Fx.Reflection.ComponentClassAttribute)_attributes[0];
                if (!_componentclasses.TryGetValue(_type, out _componentclass))
                {
                    IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
                    _GetParametersOf(_type, true, out _classparameters, out _openparameters);
                    QS.Fx.Reflection.IObjectClass _objectclass = null;
                    Type _interface_type = null;
                    foreach (Type __interface_type in _type.GetInterfaces())
                    {
                        if (typeof(QS.Fx.Object.Classes.IObject).IsAssignableFrom(__interface_type))
                        {
                            QS.Fx.Reflection.IObjectClass __objectclass = _GetObjectClass(__interface_type, _classparameters);
                            if (__objectclass != null)
                            {
                                if (_objectclass != null && !_interface_type.IsAssignableFrom(__interface_type))
                                {
                                    if (!__interface_type.IsAssignableFrom(_interface_type))
                                    {
                                        throw new Exception("Cannot register type \"" + _type.ToString() +
                                            "\" as a component class because it implements at least two different object classes that do not derive from one another: \"" +
                                            _objectclass.ID + " (" + _NameOf(_objectclass.Attributes) + ") and " + __objectclass.ID + " (" + _NameOf(__objectclass.Attributes) + ").");
                                    }
                                }
                                else
                                {
                                    _interface_type = __interface_type;
                                    _objectclass = __objectclass;
                                }
                            }
                        }
                    }
                    if (_objectclass == null)
                        throw new Exception("Cannot register type \"" + _type.ToString() +
                            "\" as a component class because it does not implement any of the registered object classes.");
                    System.Reflection.ConstructorInfo _constructor = null;
                    foreach (System.Reflection.ConstructorInfo _someconstructor in _type.GetConstructors())
                    {
                        if (_someconstructor.GetParameters().Length > 0)
                        {
                            _constructor = _someconstructor;
                            break;
                        }
                    }
                    Namespace_ _namespace = _GetNamespaceOfType(_type);

                    QS.Fx.Base.SynchronizationOption _synchronizationoption = QS.Fx.Base.SynchronizationOption.None;
                    _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Base.SynchronizationAttribute), false);
                    if (_attributes != null && _attributes.Length > 0)
                    {
                        foreach (QS.Fx.Base.SynchronizationAttribute _synchronizationattribute in _attributes)
                        {
                            _synchronizationoption = _synchronizationoption | _synchronizationattribute.Option;
                        }
                    }
                    _componentclass = new ComponentClass(null, _namespace, _attribute.ID, _attribute.Version,
                        ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                        ((_attribute.Comment != null) ? _attribute.Comment : "This is a component class based on the .NET type \"" + _type.FullName + "\"."),
                        _type, _classparameters, _openparameters, _objectclass, _constructor, _synchronizationoption);
                    this._RegisterClass(_type, _componentclass);
                    _namespace._RegisterClass(_attribute.ID, _componentclass);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a component class because it has not been decorated with the \"ComponentClass\" attribute.");
            }
            return _componentclass;
        }

        #endregion

        #region _NameOf

        private static string _NameOf(QS.Fx.Attributes.IAttributes _attributes)
        {
            QS.Fx.Attributes.IAttribute _nameattribute;
            if (_attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute) && (_nameattribute.Value != null) && (_nameattribute.Value.Length > 0))
                return "\"" + _nameattribute.Value + "\"";
            else
                return "unnamed";
        }

        #endregion

        #region _AttributeOf

        private static string _AttributeOf(QS.Fx.Attributes.IAttributes _attributes, QS.Fx.Attributes.IAttributeClass _attributeclass, string _defaultvalue)
        {
            QS.Fx.Attributes.IAttribute _attribute;
            if (_attributes.Get(_attributeclass, out _attribute))
                return _attribute.Value;
            else
                return _defaultvalue;
        }

        #endregion

        #region _GetParametersOf

        private void _GetParametersOf(Type _type, bool _includeconstructor, 
            out IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, out IDictionary<string, QS.Fx.Reflection.IParameter> _openparameters)
        {
            _classparameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            _openparameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            if (_type.IsGenericTypeDefinition)
            {
                foreach (Type _argument in _type.GetGenericArguments())
                {
                    object[] _parameterattributes = _argument.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), true);
                    if (_parameterattributes != null && _parameterattributes.Length > 0)
                    {
                        QS.Fx.Reflection.ParameterAttribute _parameterattribute = (QS.Fx.Reflection.ParameterAttribute)_parameterattributes[0];
                        QS.Fx.Reflection.IParameter _classparameter, _openparameter;
                        _GetParameterOf(_parameterattribute, out _classparameter, out _openparameter);
                        if (((QS.Fx.Reflection.IParameter)_classparameter).ParameterClass == QS.Fx.Reflection.ParameterClass.Value)
                            throw new Exception("Cannot use value parameters in generic arguments.");
                        _classparameters.Add(_parameterattribute.ID, _classparameter);
                        _openparameters.Add(_parameterattribute.ID, _openparameter);
                    }
                    else
                        throw new Exception("Cannot register class \"" + _type.ToString() + "\" because it has a generic argument \"" +
                            _argument.Name + "\" that has not been decorated as a \"Parameter\".");
                }
            }
            if (_includeconstructor)
            {
                if (!_type.IsClass)
                    throw new Exception("Must be a class to have a constructor.");
                System.Reflection.ConstructorInfo[] _constructors = _type.GetConstructors();
                //if (_constructors == null || _constructors.Length < 1)
                //    throw new Exception("Cannot register component class \"" + _type.ToString() + "\" because it does not have any constructors defined.");
                //if (_constructors.Length > 1)
                //    throw new Exception("Cannot register component class \"" + _type.ToString() + "\" because it has more than one constructor defined.");
                System.Reflection.ConstructorInfo _constructor = null;
                foreach (System.Reflection.ConstructorInfo _someconstructor in _constructors)
                {
                    if (_someconstructor.GetParameters().Length > 0)
                    {
                        _constructor = _someconstructor;
                        break;
                    }
                }
                if (_constructor == null)
                    throw new Exception("Cannot find suitable constructors in \"" + _type.ToString() + "\".");
                bool _found_mycontext = false;
                foreach (System.Reflection.ParameterInfo _parameterinfo in _constructor.GetParameters())
                {
                    if (!_parameterinfo.IsRetval)
                    {
                        if (_parameterinfo.ParameterType.IsByRef || _parameterinfo.IsOut || _parameterinfo.IsOptional)
                            throw new Exception("Cannot register component class \"" + _type.ToString() + "\" because parameter \"" +
                                _parameterinfo.Name + "\" of the constructor is either optional or is an output or by reference parameter.");
                        //                        if (_parameterinfo.ParameterType != typeof(System.String))
                        //                            throw new Exception("Cannot register component class \"" + _type.ToString() + "\" because parameter \"" +
                        //                                _parameterinfo.Name + "\" of the constructor is of type other than System.String.");
                        if (_parameterinfo.ParameterType.Equals(typeof(QS.Fx.Object.IContext)))
                        {
                            if (_found_mycontext)
                                throw new Exception("Cannot get the context parameter twice.");
                            else
                                _found_mycontext = true;
                        }
                        else
                        {
                            if (!_found_mycontext)
                                throw new Exception("The context parameter must be the first in the constructor.");
                            object[] _parameterattributes = _parameterinfo.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), true);
                            if (_parameterattributes != null && _parameterattributes.Length > 0)
                            {
                                QS.Fx.Reflection.ParameterAttribute _parameterattribute = (QS.Fx.Reflection.ParameterAttribute)_parameterattributes[0];
                                if (_parameterattribute.ParameterClass != QS.Fx.Reflection.ParameterClass.Value)
                                    throw new Exception("Cannot use non-value parameters in constructor arguments.");
                                QS.Fx.Reflection.IValueClass _valueclass = _GetValueClass(_parameterinfo.ParameterType, _classparameters);
                                if (_parameterattribute.DefaultValue != null)
                                {
                                    // ....................here we should check if the default value is indeed of the right value class
                                }
                                QS.Fx.Reflection.Parameter _openparameter = new QS.Fx.Reflection.Parameter(_parameterattribute.ID,
                                    new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[] { 
                                    new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, _parameterattribute.Comment) }),
                                    QS.Fx.Reflection.ParameterClass.Value, _valueclass, _parameterattribute.DefaultValue, null);
                                QS.Fx.Reflection.Parameter _classparameter = new QS.Fx.Reflection.Parameter(
                                    _parameterattribute.ID,
                                    new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[] { 
                                    new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, _parameterattribute.Comment) }),
                                    QS.Fx.Reflection.ParameterClass.Value, _valueclass, _parameterattribute.DefaultValue, new ParameterValue(_openparameter));
                                _classparameters.Add(_parameterattribute.ID, _classparameter);
                                _openparameters.Add(_parameterattribute.ID, _openparameter);
                            }
                            else
                                throw new Exception("Cannot register component class \"" + _type.ToString() + "\" because the constructor has a parameter \"" +
                                    _parameterinfo.Name + "\" that has not been decorated as a \"Parameter\".");
                        }
                    }
                }
            }
        }

        #endregion

        #region _GetParameterOf

        private void _GetParameterOf(QS.Fx.Reflection.ParameterAttribute _parameterattribute, 
            out QS.Fx.Reflection.IParameter _classparameter, out QS.Fx.Reflection.IParameter _openparameter)
        {
            QS.Fx.Reflection.IValueClass _valueclass = null;
            object _defaultvalue = null;
            switch (_parameterattribute.ParameterClass)
            {
                case QS.Fx.Reflection.ParameterClass.Value:
                    throw new NotSupportedException();

                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    if (_parameterattribute.DefaultValue != null)
                        _defaultvalue = _GetValueClass((Type)_parameterattribute.DefaultValue, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    if (_parameterattribute.DefaultValue != null)
                        _defaultvalue = _GetInterfaceClass((Type)_parameterattribute.DefaultValue, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    if (_parameterattribute.DefaultValue != null)
                        _defaultvalue = _GetEndpointClass((Type)_parameterattribute.DefaultValue, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    if (_parameterattribute.DefaultValue != null)
                        _defaultvalue = _GetObjectClass((Type)_parameterattribute.DefaultValue, null);
                    break;

                default:
                    throw new NotImplementedException();
            }
            _openparameter = new QS.Fx.Reflection.Parameter(_parameterattribute.ID,
                new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[] { 
                    new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, _parameterattribute.Comment) }),
                _parameterattribute.ParameterClass, _valueclass, _defaultvalue, null);

            object _value = null;            
            switch (_parameterattribute.ParameterClass)
            {
                case QS.Fx.Reflection.ParameterClass.Value:
                    throw new NotSupportedException();

                case QS.Fx.Reflection.ParameterClass.ValueClass:
                    _value = new QS._qss_x_.Reflection_.ValueClass(null, null, null, 0UL, _parameterattribute.ID, null, null, null, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                    _value = new QS._qss_x_.Reflection_.InterfaceClass(null, null, null, 0UL, _parameterattribute.ID, null, null, null, null, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.EndpointClass:
                    _value = new QS._qss_x_.Reflection_.EndpointClass(null, null, null, 0UL, _parameterattribute.ID, null, null, null, null);
                    break;

                case QS.Fx.Reflection.ParameterClass.ObjectClass:
                    _value = new QS._qss_x_.Reflection_.ObjectClass(null, null, null, 0UL, _parameterattribute.ID, null, null, null, null, null, null);
                    break;

                default:
                    throw new NotImplementedException();
            }
            _classparameter = new QS.Fx.Reflection.Parameter(_parameterattribute.ID,
                new QS.Fx.Attributes.Attributes(new QS.Fx.Attributes.IAttribute[] { 
                    new QS.Fx.Attributes.Attribute(QS.Fx.Attributes.AttributeClasses.CLASS_comment, _parameterattribute.Comment) }),
                _parameterattribute.ParameterClass, _valueclass, _defaultvalue, _value);
        }

        #endregion

        #region _GetValueClass

        private QS.Fx.Reflection.IValueClass _GetValueClass(Type _type, IDictionary<string, QS.Fx.Reflection.IParameter> _parameters)
        {
            QS.Fx.Reflection.IValueClass _valueclass;
            if (_type.IsGenericParameter)
            {
                object[] _my_parameter_attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), false);
                if (_my_parameter_attributes.Length != 1)
                    throw new Exception("Generic parameter type \"" + _type.Name + "\" has not been annotated as a template parameter.");
                QS.Fx.Reflection.ParameterAttribute _my_parameter_attribute = (QS.Fx.Reflection.ParameterAttribute)_my_parameter_attributes[0];
                string _my_parameter_name = _my_parameter_attribute.ID;
                if (_parameters != null)
                {
                    QS.Fx.Reflection.IParameter _parameter;
                    if (!_parameters.TryGetValue(_my_parameter_name, out _parameter))
                        throw new Exception("Undefined parameter \"" + _type.Name + "\".");
                    if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.ValueClass)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is of a parameter class different than the value class.");
                    if (_parameter.Value == null)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is null.");
                    if (_parameter.Value is QS.Fx.Reflection.IValueClass)
                        _valueclass = (QS.Fx.Reflection.IValueClass)_parameter.Value;
                    else
                        throw new Exception("Parameter \"" + _parameter.ID + "\" has been assigned a value that isn't of a value class.");
                }
                else
                    throw new Exception("Cannot resolve generic parameter \"" + _my_parameter_name + 
                        "\" because there are no parameters defined in the supplied environment.");
            }
            else
            {
                if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                {
                    Type _base_type = _type.GetGenericTypeDefinition();
                    QS.Fx.Reflection.IValueClass _base_valueclass = _GetValueClass(_base_type, null);
                    IDictionary<string, QS.Fx.Reflection.IParameter> _class_parameters, _open_parameters;
                    _InstantiateParameters(_base_type, _type, _base_valueclass.ClassParameters, _parameters, out _class_parameters, out _open_parameters);
                    return new ValueClass(_base_valueclass,
                        ((ValueClass) _base_valueclass)._Namespace, _base_valueclass.ID, _base_valueclass.Incarnation,
                        _AttributeOf(_base_valueclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null),
                        _AttributeOf(_base_valueclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null),
                        _base_type, _class_parameters, _open_parameters);
                }
                else
                {
                    if (!_valueclasses.TryGetValue(_type, out _valueclass))
                        _valueclass = _RegisterValueClass(_type, true);
                }
            }
            return _valueclass;
        }

        #endregion

        #region _GetInterfaceClass

        private QS.Fx.Reflection.IInterfaceClass _GetInterfaceClass(Type _type, IDictionary<string, QS.Fx.Reflection.IParameter> _parameters)
        {
            QS.Fx.Reflection.IInterfaceClass _interfaceclass;
            if (_type.IsGenericParameter)
            {
                object[] _my_parameter_attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), false);
                if (_my_parameter_attributes.Length != 1)
                    throw new Exception("Generic parameter type \"" + _type.Name + "\" has not been annotated as a template parameter.");
                QS.Fx.Reflection.ParameterAttribute _my_parameter_attribute = (QS.Fx.Reflection.ParameterAttribute)_my_parameter_attributes[0];
                string _my_parameter_name = _my_parameter_attribute.ID;
                if (_parameters != null)
                {
                    QS.Fx.Reflection.IParameter _parameter;
                    if (!_parameters.TryGetValue(_my_parameter_name, out _parameter))
                        throw new Exception("Undefined parameter \"" + _type.Name + "\".");
                    if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.InterfaceClass)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is of a parameter class different than the interface class.");
                    if (_parameter.Value == null)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is null.");
                    if (_parameter.Value is QS.Fx.Reflection.IInterfaceClass)
                        _interfaceclass = (QS.Fx.Reflection.IInterfaceClass)_parameter.Value;
                    else
                        throw new Exception("Parameter \"" + _parameter.ID + "\" has been assigned a value that isn't of an interface class.");
                }
                else
                    throw new Exception("Cannot resolve generic parameter \"" + _my_parameter_name +
                        "\" because there are no parameters defined in the supplied environment.");
            }
            else
            {
                if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                {
                    Type _base_type = _type.GetGenericTypeDefinition();
                    QS.Fx.Reflection.IInterfaceClass _base_interfaceclass = _GetInterfaceClass(_base_type, null);
                    IDictionary<string, QS.Fx.Reflection.IParameter> _class_parameters, _open_parameters;
                    _InstantiateParameters(_base_type, _type, _base_interfaceclass.ClassParameters, _parameters, 
                        out _class_parameters, out _open_parameters);

                    IDictionary<string, QS.Fx.Reflection.IOperation> _operations = new Dictionary<string, QS.Fx.Reflection.IOperation>();

                    // FIX:20071229
                    foreach (KeyValuePair<string, QS.Fx.Reflection.IOperation> _operation in _base_interfaceclass.Operations)
                        _operations.Add(_operation.Key, _operation.Value.Instantiate(_class_parameters.Values));

                    return new InterfaceClass(
                        _base_interfaceclass,
                        ((InterfaceClass) _base_interfaceclass)._Namespace, _base_interfaceclass.ID, _base_interfaceclass.Incarnation,
                        _AttributeOf(_base_interfaceclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null),
                        _AttributeOf(_base_interfaceclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null),
                        _base_type, _class_parameters, _open_parameters, _operations);
                }
                else
                {
                    if (!_interfaceclasses.TryGetValue(_type, out _interfaceclass))
                        _interfaceclass = _RegisterInterfaceClass(_type, true);
                }
            }
            return _interfaceclass;
        }

        #endregion

        #region _GetEndpointClass

        private QS.Fx.Reflection.IEndpointClass _GetEndpointClass(Type _type, IDictionary<string, QS.Fx.Reflection.IParameter> _parameters)
        {
            QS.Fx.Reflection.IEndpointClass _endpointclass;
            if (_type.IsGenericParameter)
            {
                object[] _my_parameter_attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), false);
                if (_my_parameter_attributes.Length != 1)
                    throw new Exception("Generic parameter type \"" + _type.Name + "\" has not been annotated as a template parameter.");
                QS.Fx.Reflection.ParameterAttribute _my_parameter_attribute = (QS.Fx.Reflection.ParameterAttribute)_my_parameter_attributes[0];
                string _my_parameter_name = _my_parameter_attribute.ID;
                if (_parameters != null)
                {
                    QS.Fx.Reflection.IParameter _parameter;
                    if (!_parameters.TryGetValue(_my_parameter_name, out _parameter))
                        throw new Exception("Undefined parameter \"" + _type.Name + "\".");
                    if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.EndpointClass)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is of a parameter class different than the endpoint class.");
                    if (_parameter.Value == null)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is null.");
                    if (_parameter.Value is QS.Fx.Reflection.IEndpointClass)
                        _endpointclass = (QS.Fx.Reflection.IEndpointClass)_parameter.Value;
                    else
                        throw new Exception("Parameter \"" + _parameter.ID + "\" has been assigned a value that isn't of an endpoint class.");
                }
                else
                    throw new Exception("Cannot resolve generic parameter \"" + _my_parameter_name +
                        "\" because there are no parameters defined in the supplied environment.");
            }
            else
            {
                if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                {
                    Type _base_type = _type.GetGenericTypeDefinition();
                    QS.Fx.Reflection.IEndpointClass _base_endpointclass = _GetEndpointClass(_base_type, null);
                    IDictionary<string, QS.Fx.Reflection.IParameter> _class_parameters, _open_parameters;
                    _InstantiateParameters(_base_type, _type, _base_endpointclass.ClassParameters, _parameters, out _class_parameters, out _open_parameters);
                    return new EndpointClass(
                        _base_endpointclass,
                        ((EndpointClass) _base_endpointclass)._Namespace, _base_endpointclass.ID, _base_endpointclass.Incarnation,
                        _AttributeOf(_base_endpointclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null),
                        _AttributeOf(_base_endpointclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null),
                        _base_type, _class_parameters, _open_parameters);
                }
                else
                {
                    if (!_endpointclasses.TryGetValue(_type, out _endpointclass))
                        _endpointclass = _RegisterEndpointClass(_type, true);
                }
            }
            return _endpointclass;
        }

        #endregion

        #region _GetObjectClass

        private QS.Fx.Reflection.IObjectClass _GetObjectClass(Type _type, IDictionary<string, QS.Fx.Reflection.IParameter> _parameters)
        {
            QS.Fx.Reflection.IObjectClass _objectclass;
            if (_type.IsGenericParameter)
            {
                object[] _my_parameter_attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), false);
                if (_my_parameter_attributes.Length != 1)
                    throw new Exception("Generic parameter type \"" + _type.Name + "\" has not been annotated as a template parameter.");
                QS.Fx.Reflection.ParameterAttribute _my_parameter_attribute = (QS.Fx.Reflection.ParameterAttribute)_my_parameter_attributes[0];
                string _my_parameter_name = _my_parameter_attribute.ID;
                if (_parameters != null)
                {
                    QS.Fx.Reflection.IParameter _parameter;
                    if (!_parameters.TryGetValue(_my_parameter_name, out _parameter))
                        throw new Exception("Undefined parameter \"" + _type.Name + "\".");
                    if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.ObjectClass)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is of a parameter class different than the object class.");
                    if (_parameter.Value == null)
                        throw new Exception("Parameter \"" + _parameter.ID + "\" is null.");
                    if (_parameter.Value is QS.Fx.Reflection.IObjectClass)
                        _objectclass = (QS.Fx.Reflection.IObjectClass)_parameter.Value;
                    else
                        throw new Exception("Parameter \"" + _parameter.ID + "\" has been assigned a value that isn't of an object class.");
                }
                else
                    throw new Exception("Cannot resolve generic parameter \"" + _my_parameter_name +
                        "\" because there are no parameters defined in the supplied environment.");
            }
            else
            {
                if (_type.IsGenericType && !_type.IsGenericTypeDefinition)
                {
                    Type _base_type = _type.GetGenericTypeDefinition();
                    QS.Fx.Reflection.IObjectClass _base_objectclass = _GetObjectClass(_base_type, null);
                    IDictionary<string, QS.Fx.Reflection.IParameter> _class_parameters, _open_parameters;
                    _InstantiateParameters(_base_type, _type, _base_objectclass.ClassParameters, _parameters, out _class_parameters, out _open_parameters);                    
                    IDictionary<string, QS.Fx.Reflection.IEndpoint> _endpoints = new Dictionary<string, QS.Fx.Reflection.IEndpoint>();
                    foreach (KeyValuePair<string, QS.Fx.Reflection.IEndpoint> _endpoint in _base_objectclass.Endpoints)
                        _endpoints.Add(_endpoint.Key, _endpoint.Value.Instantiate(_class_parameters.Values));
/*                    
                    foreach (System.Reflection.PropertyInfo _propertyinfo in
                        _type.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public))
                    {
                        object[] _endpointattributes = _propertyinfo.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointAttribute), true);
                        if (_endpointattributes != null && _endpointattributes.Length > 0)
                        {
                            QS.Fx.Reflection.EndpointAttribute _endpointattribute = (QS.Fx.Reflection.EndpointAttribute)_endpointattributes[0];
                            IEndpointClass _endpointclass = _GetEndpointClass(_propertyinfo.PropertyType, _parameters);
                            Endpoint _endpoint = new Endpoint(_endpointattribute.ID, _endpointclass, _propertyinfo);
                            _endpoints.Add(_endpointattribute.ID, _endpoint);
                        }
                    }
*/ 
                    return new ObjectClass(
                        _base_objectclass,
                        ((ObjectClass) _base_objectclass)._Namespace, _base_objectclass.ID, _base_objectclass.Incarnation,
                        _AttributeOf(_base_objectclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null),
                        _AttributeOf(_base_objectclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null),
                        _base_type, _class_parameters, _open_parameters, _endpoints,
                        ((_base_objectclass.AuthenticatingClass != null) ? _base_objectclass.Instantiate(_class_parameters.Values) : null));
                }
                else
                {
                    if (!_objectclasses.TryGetValue(_type, out _objectclass))
                        _objectclass = _RegisterObjectClass(_type, true);
                }
            }
            return _objectclass;
        }

        #endregion

        #region _GetObjectClassOfComponent

        private QS.Fx.Reflection.IObjectClass _GetObjectClassOfComponent(Type _type)
        {
            IDictionary<string, QS.Fx.Reflection.IParameter> _classparameters, _openparameters;
            _GetParametersOf(_type, true, out _classparameters, out _openparameters);
            QS.Fx.Reflection.IObjectClass _objectclass = null;
            Type _interface_type = null;
            foreach (Type __interface_type in _type.GetInterfaces())
            {
                if (typeof(QS.Fx.Object.Classes.IObject).IsAssignableFrom(__interface_type))
                {
                    QS.Fx.Reflection.IObjectClass __objectclass = _GetObjectClass(__interface_type, _classparameters);
                    if (__objectclass != null)
                    {
                        if (_objectclass != null && !_interface_type.IsAssignableFrom(__interface_type))
                        {
                            if (!__interface_type.IsAssignableFrom(_interface_type))
                            {
                                throw new Exception("Cannot determine the object class of the physical type \"" + _type.ToString() +
                                    "\" because it implements at least two different object classes that do not derive from one another: \"" +
                                    _objectclass.ID + " (" + _NameOf(_objectclass.Attributes) + ") and " + __objectclass.ID + " (" + _NameOf(__objectclass.Attributes) + ").");
                            }
                        }
                        else
                        {
                            _interface_type = __interface_type;
                            _objectclass = __objectclass;
                        }
                    }
                }
            }
            if (_objectclass == null)
                throw new Exception("Cannot determine the object class of the physical type \"" + _type.ToString() +
                    "\" because it does not implement any of the registered object classes.");
            return _objectclass;
        }

        #endregion

        #region _InstantiateParameters (1)

        private void _InstantiateParameters(Type _base_type, Type _type, IDictionary<string, QS.Fx.Reflection.IParameter> _base_class_parameters,
            IDictionary<string, QS.Fx.Reflection.IParameter> _parameters, out IDictionary<string, QS.Fx.Reflection.IParameter> _class_parameters, 
            out IDictionary<string, QS.Fx.Reflection.IParameter> _open_parameters)
        {
            _class_parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            _open_parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            Type[] _base_arguments = _base_type.GetGenericArguments();
            Type[] _arguments = _type.GetGenericArguments();
            for (int _k = 0; _k < _arguments.Length; _k++)
            {
                Type _base_argument = _base_arguments[_k];
                Type _argument = _arguments[_k];
                QS.Fx.Reflection.ParameterAttribute _base_parameter_attribute;
                object[] _base_parameter_attributes = _base_argument.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), false);
                if (_base_parameter_attributes.Length > 0)
                    _base_parameter_attribute = (QS.Fx.Reflection.ParameterAttribute)_base_parameter_attributes[0];
                else
                    _base_parameter_attribute = new QS.Fx.Reflection.ParameterAttribute(_base_argument.Name, QS.Fx.Reflection.ParameterClass.ValueClass);
                QS.Fx.Reflection.IParameter _base_parameter = _base_class_parameters[_base_parameter_attribute.ID];
                object _value = null;
                switch (_base_parameter.ParameterClass)
                {
                    case QS.Fx.Reflection.ParameterClass.Value:
                        throw new Exception("A generic type parameter cannt be of class \"Value\", the type was constructed incorrectly.");

                    case QS.Fx.Reflection.ParameterClass.ValueClass:
                        _value = _GetValueClass(_argument, _parameters);
                        break;

                    case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                        _value = _GetInterfaceClass(_argument, _parameters);
                        break;

                    case QS.Fx.Reflection.ParameterClass.EndpointClass:
                        _value = _GetEndpointClass(_argument, _parameters);
                        break;

                    case QS.Fx.Reflection.ParameterClass.ObjectClass:
                        _value = _GetObjectClass(_argument, _parameters);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                _class_parameters.Add(_base_parameter.ID,
                    new QS.Fx.Reflection.Parameter(_base_parameter.ID, _base_parameter.Attributes, _base_parameter.ParameterClass, null, _base_parameter.DefaultValue, _value));
            }
        }

        #endregion

        #region ComponentClassOf

        public static QS.Fx.Reflection.IObjectClass ObjectClassOfComponent(Type _type)
        {
            lock (_LocalLibrary)
            {
                return _LocalLibrary._GetObjectClassOfComponent(_type);
            }
        }

        #endregion

        #region ObjectClassOf

        public static QS.Fx.Reflection.IObjectClass ObjectClassOf(Type _type)
        {
            lock (_LocalLibrary)
            {
                return _LocalLibrary._GetObjectClass(_type, new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        #endregion

        #region InterfaceClassOf

        public static QS.Fx.Reflection.IInterfaceClass InterfaceClassOf(Type _type)
        {
            lock (_LocalLibrary)
            {
                return _LocalLibrary._GetInterfaceClass(_type, new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        #endregion

        #region ValueClassOf

        public static QS.Fx.Reflection.IValueClass ValueClassOf(Type _type)
        {
            lock (_LocalLibrary)
            {
                return _LocalLibrary._GetValueClass(_type, new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        #endregion

        #region _IsAnObjectRepository

        private static QS.Fx.Reflection.IObjectClass _object_repository_objectclass = null;

        public static bool _IsAnObjectRepository(QS.Fx.Reflection.IObjectClass _objectclass)
        {
            if (_objectclass != null)
            {
                if (_object_repository_objectclass == null)
                {
                    lock (typeof(Library))
                    {
                        if (_object_repository_objectclass == null)
                        {
                            _object_repository_objectclass = 
                                QS._qss_x_.Reflection_.Library.ObjectClassOf(
                                    typeof(QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>));
                        }
                    }
                }
                return _objectclass.IsSubtypeOf(_object_repository_objectclass);
            }
            else
                return false;
        }

        #endregion

        #region IsObjectReference

        private static readonly QS.Fx.Base.ID _objectreference_valueclass_id = new QS.Fx.Base.ID(QS.Fx.Reflection.ValueClasses._Object);
        
        public static bool _IsAnObjectReference(QS.Fx.Reflection.IValueClass _valueclass, out QS.Fx.Reflection.IObjectClass _objectclass)
        {
            _objectclass = null;
            QS._qss_x_.Reflection_.ValueClass _m_valueclass = _valueclass as QS._qss_x_.Reflection_.ValueClass;
            if ((_m_valueclass != null) && _m_valueclass._Namespace.ID.Equals(QS.Fx.Base.ID._0) &&
                _valueclass.ID.Equals(_objectreference_valueclass_id))
            {
                object _v = _valueclass.ClassParameters["ObjectClass"].Value;
                while (!(_v is QS.Fx.Reflection.IObjectClass))
                {
                    if (!(_v is QS._qss_x_.Reflection_.ParameterValue))
                        throw new NotImplementedException();
                    _v = ((QS._qss_x_.Reflection_.IParameterValue)_v).Parameter.Value;
                }
                _objectclass = (QS.Fx.Reflection.IObjectClass)_v;
                return true;
            }
            else
                return false;
        }

        public static bool _IsAnObjectReference(QS.Fx.Reflection.IValueClass _valueclass)
        {
            QS._qss_x_.Reflection_.ValueClass _m_valueclass = _valueclass as QS._qss_x_.Reflection_.ValueClass;
            return ((_m_valueclass != null) && _m_valueclass._Namespace.ID.Equals(QS.Fx.Base.ID._0) &&
                _valueclass.ID.Equals(_objectreference_valueclass_id));
        }

        #endregion

        #region _InstantiateParameters (2)

        internal static void _InstantiateParameters(IEnumerable<QS.Fx.Reflection.IParameter> _parameters,
            IDictionary<string, QS.Fx.Reflection.IParameter> _old_class_parameters, IDictionary<string, QS.Fx.Reflection.IParameter> _old_open_parameters,
            out IDictionary<string, QS.Fx.Reflection.IParameter> _new_class_parameters, out IDictionary<string, QS.Fx.Reflection.IParameter> _new_open_parameters)
        {
            _new_class_parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            _new_open_parameters = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            Dictionary<string, QS.Fx.Reflection.IParameter> _parameters_dictionary = new Dictionary<string, QS.Fx.Reflection.IParameter>();
            if (_parameters != null)
            {
                foreach (QS.Fx.Reflection.IParameter _parameter in _parameters)
                {
                    _parameters_dictionary.Add(_parameter.ID, _parameter);
                }
            }
            if (_old_class_parameters != null)
            {
                foreach (QS.Fx.Reflection.IParameter _old_class_parameter in _old_class_parameters.Values)
                {
                    QS.Fx.Reflection.IValueClass _new_class_parameter_valueclass = _old_class_parameter.ValueClass;
                    object _new_class_parameter_value = _old_class_parameter.Value;
                    switch (_old_class_parameter.ParameterClass)
                    {
                        case QS.Fx.Reflection.ParameterClass.ValueClass:
                            {
                                QS.Fx.Reflection.IValueClass _valueclass = (QS.Fx.Reflection.IValueClass)_old_class_parameter.Value;
                                if (_valueclass.ID == null)
                                {
                                    string _valueclass_name = _valueclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name).Value;
                                    QS.Fx.Reflection.IParameter _parameter;
                                    if (_parameters_dictionary.TryGetValue(_valueclass_name, out _parameter))
                                    {
                                        if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.ValueClass)
                                            throw new Exception("Cannot substitute value class parameter \"" + _valueclass_name +
                                                "\" because the new value is of an incompatible type.");
                                        _new_class_parameter_value = (QS.Fx.Reflection.IValueClass)_parameter.Value;
                                    }
                                    else
                                    {
                                        if (!_old_open_parameters.ContainsKey(_valueclass_name))
                                            throw new Exception("While substituting parameters, encountered a reference to unknown value class parameter \"" + _valueclass_name + "\".");
                                    }
                                }
                                else
                                    _new_class_parameter_value = _valueclass.Instantiate(_parameters);
                            }
                            break;

                        case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                            {
                                QS.Fx.Reflection.IInterfaceClass _interfaceclass = (QS.Fx.Reflection.IInterfaceClass)_old_class_parameter.Value;
                                if (_interfaceclass.ID == null)
                                {
                                    string _interfaceclass_name = _interfaceclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name).Value;
                                    QS.Fx.Reflection.IParameter _parameter;
                                    if (_parameters_dictionary.TryGetValue(_interfaceclass_name, out _parameter))
                                    {
                                        if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.InterfaceClass)
                                            throw new Exception("Cannot substitute interface class parameter \"" + _interfaceclass_name +
                                                "\" because the new value is of an incompatible type.");
                                        _new_class_parameter_value = (QS.Fx.Reflection.IInterfaceClass)_parameter.Value;
                                    }
                                    else
                                    {
                                        if (!_old_open_parameters.ContainsKey(_interfaceclass_name))
                                            throw new Exception("While substituting parameters, encountered a reference to unknown interface class parameter \"" +
                                                _interfaceclass_name + "\".");
                                    }
                                }
                                else
                                    _new_class_parameter_value = _interfaceclass.Instantiate(_parameters);
                            }
                            break;

                        case QS.Fx.Reflection.ParameterClass.EndpointClass:
                            {
                                QS.Fx.Reflection.IEndpointClass _endpointclass = (QS.Fx.Reflection.IEndpointClass)_old_class_parameter.Value;
                                if (_endpointclass.ID == null)
                                {
                                    string _endpointclass_name = _endpointclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name).Value;
                                    QS.Fx.Reflection.IParameter _parameter;
                                    if (_parameters_dictionary.TryGetValue(_endpointclass_name, out _parameter))
                                    {
                                        if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.EndpointClass)
                                            throw new Exception("Cannot substitute endpoint class parameter \"" + _endpointclass_name +
                                                "\" because the new value is of an incompatible type.");
                                        _new_class_parameter_value = (QS.Fx.Reflection.IEndpointClass)_parameter.Value;
                                    }
                                    else
                                    {
                                        if (!_old_open_parameters.ContainsKey(_endpointclass_name))
                                            throw new Exception("While substituting parameters, encountered a reference to unknown endpoint class parameter \"" +
                                                _endpointclass_name + "\".");
                                    }
                                }
                                else
                                    _new_class_parameter_value = _endpointclass.Instantiate(_parameters);
                            }
                            break;

                        case QS.Fx.Reflection.ParameterClass.ObjectClass:
                            {
                                QS.Fx.Reflection.IObjectClass _objectclass = (QS.Fx.Reflection.IObjectClass)_old_class_parameter.Value;
                                if (_objectclass.ID == null)
                                {
                                    string _objectclass_name = _objectclass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name).Value;
                                    QS.Fx.Reflection.IParameter _parameter;
                                    if (_parameters_dictionary.TryGetValue(_objectclass_name, out _parameter))
                                    {
                                        if (_parameter.ParameterClass != QS.Fx.Reflection.ParameterClass.ObjectClass)
                                            throw new Exception("Cannot substitute object class parameter \"" + _objectclass_name +
                                                "\" because the new value is of an incompatible type.");
                                        _new_class_parameter_value = (QS.Fx.Reflection.IObjectClass)_parameter.Value;
                                    }
                                    else
                                    {
                                        if (!_old_open_parameters.ContainsKey(_objectclass_name))
                                            throw new Exception("While substituting parameters, encountered a reference to unknown object class parameter \"" +
                                                _objectclass_name + "\".");
                                    }
                                }
                                else
                                    _new_class_parameter_value = _objectclass.Instantiate(_parameters);
                            }
                            break;

                        case QS.Fx.Reflection.ParameterClass.Value:
                            {
                                _new_class_parameter_valueclass = _new_class_parameter_valueclass.Instantiate(_parameters);
                                if (_old_class_parameter.Value is IParameterValue)
                                {
                                    IParameterValue _parametervalue = (IParameterValue)_old_class_parameter.Value;
                                    QS.Fx.Reflection.IParameter _parameter;
                                    if (_parameters_dictionary.TryGetValue(_parametervalue.Parameter.ID, out _parameter))
                                    {
                                        _new_class_parameter_value = _parameter.Value;
                                    }
                                    else
                                    {
                                        if (!_old_open_parameters.ContainsKey(_parametervalue.Parameter.ID))
                                            throw new Exception("While substituting parameters, encountered a reference to unknown value parameter \"" +
                                                _parametervalue.Parameter.ID + "\".");
                                    }
                                }
                            }
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    _new_class_parameters[_old_class_parameter.ID] = new QS.Fx.Reflection.Parameter(_old_class_parameter.ID, _old_class_parameter.Attributes, _old_class_parameter.ParameterClass,
                        _new_class_parameter_valueclass, _old_class_parameter.DefaultValue, _new_class_parameter_value);
                }
            }
        }

        #endregion

        #region _InstantiateParameters (3)

        internal static Type _InstantiateParameters(Type _underlyingtype, IDictionary<string, QS.Fx.Reflection.IParameter> _parameters)
        {
            if (_underlyingtype == null)
                throw new Exception("Cannot instantiate parameters because the underlying type is null.");

            if (_underlyingtype.IsGenericType)
            {
                if (!_underlyingtype.IsGenericTypeDefinition)
                    throw new Exception("Cannot create element because the metadata was constructed incorrectly: the underying type is generic, but not a generic definition.");
                Type[] _arguments = _underlyingtype.GetGenericArguments();
                for (int _k = 0; _k < _arguments.Length; _k++)
                {
                    Type _argument = _arguments[_k];
                    object[] _parameterattributes = _argument.GetCustomAttributes(typeof(QS.Fx.Reflection.ParameterAttribute), true);
                    string _parameter_id;
                    if (_parameterattributes == null || _parameterattributes.Length != 1)
                    {
                        _parameter_id = _argument.Name;
                        // throw new Exception("Cannot create element because the generic parameters have not been properly annotated with the \"Parameter\" attribute.");
                    }
                    else
                    {
                        QS.Fx.Reflection.ParameterAttribute _parameterattribute = (QS.Fx.Reflection.ParameterAttribute)_parameterattributes[0];
                        _parameter_id = _parameterattribute.ID;
                    }
                    QS.Fx.Reflection.IParameter _parameter = _parameters[_parameter_id];
                    switch (_parameter.ParameterClass)
                    {
                        case QS.Fx.Reflection.ParameterClass.Value:
                            throw new Exception("Cannot create element because a generic type parameter has been incorrectly annotated as a \"Value\" parameter.");

                        case QS.Fx.Reflection.ParameterClass.ValueClass:
                            _argument = ((QS.Fx.Reflection.IValueClass)_parameter.Value).UnderlyingType;
                            break;

                        case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                            _argument = ((QS.Fx.Reflection.IInterfaceClass)_parameter.Value).UnderlyingType;
                            break;

                        case QS.Fx.Reflection.ParameterClass.EndpointClass:
                            _argument = ((QS.Fx.Reflection.IEndpointClass)_parameter.Value).UnderlyingType;
                            break;

                        case QS.Fx.Reflection.ParameterClass.ObjectClass:
                            _argument = ((QS.Fx.Reflection.IObjectClass)_parameter.Value).UnderlyingType;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    _arguments[_k] = _argument;
                }
                return _underlyingtype.MakeGenericType(_arguments);
            }
            else
                return _underlyingtype;
        }

        #endregion

        #region ILibrary2_ Members

        IEnumerable<QS.Fx.Reflection.IValueClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ValueClasses()
        {
            lock (this)
            {
                List<QS.Fx.Reflection.IValueClass> _result = new List<QS.Fx.Reflection.IValueClass>();
                foreach (Namespace_ _namespace in this._namespaces.Values)
                    _result.AddRange(((QS._qss_x_.Interface_.Classes_.ILibrary2_)_namespace).ValueClasses());
                return _result;
            }
        }

        IEnumerable<QS.Fx.Reflection.IInterfaceClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.InterfaceClasses()
        {
            lock (this)
            {
                List<QS.Fx.Reflection.IInterfaceClass> _result = new List<QS.Fx.Reflection.IInterfaceClass>();
                foreach (Namespace_ _namespace in this._namespaces.Values)
                    _result.AddRange(((QS._qss_x_.Interface_.Classes_.ILibrary2_)_namespace).InterfaceClasses());
                return _result;
            }
        }

        IEnumerable<QS.Fx.Reflection.IEndpointClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.EndpointClasses()
        {
            lock (this)
            {
                List<QS.Fx.Reflection.IEndpointClass> _result = new List<QS.Fx.Reflection.IEndpointClass>();
                foreach (Namespace_ _namespace in this._namespaces.Values)
                    _result.AddRange(((QS._qss_x_.Interface_.Classes_.ILibrary2_)_namespace).EndpointClasses());
                return _result;
            }
        }

        IEnumerable<QS.Fx.Reflection.IObjectClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ObjectClasses()
        {
            lock (this)
            {
                List<QS.Fx.Reflection.IObjectClass> _result = new List<QS.Fx.Reflection.IObjectClass>();
                foreach (Namespace_ _namespace in this._namespaces.Values)
                    _result.AddRange(((QS._qss_x_.Interface_.Classes_.ILibrary2_)_namespace).ObjectClasses());
                return _result;
            }
        }

        IEnumerable<QS.Fx.Reflection.IComponentClass> QS._qss_x_.Interface_.Classes_.ILibrary2_.ComponentClasses()
        {
            lock (this)
            {
                List<QS.Fx.Reflection.IComponentClass> _result = new List<QS.Fx.Reflection.IComponentClass>();
                foreach (Namespace_ _namespace in this._namespaces.Values)
                    _result.AddRange(((QS._qss_x_.Interface_.Classes_.ILibrary2_)_namespace).ComponentClasses());
                return _result;
            }
        }

        #endregion

        #region Register_

        internal static void Register_()
        {
            QS.Fx.Reflection.Library.Initialize(LocalLibrary_);
        }

        #endregion

        #region ICreate Members

        QS.Fx.Reflection.IValueClass QS.Fx.Reflection.Library.ILibrary.ValueClass<C>()
        {
            lock (this)
            {
                return this._GetValueClass(typeof(C), new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        QS.Fx.Reflection.IValueClass QS.Fx.Reflection.Library.ILibrary.ValueClass(string _uuid, params QS.Fx.Reflection.Parameter[] _parameters)
        {
            lock (this)
            {
                QS.Fx.Reflection.IValueClass _valueclass = ((QS.Fx.Interface.Classes.ILibrary) this).GetValueClass(_uuid);
                if (_parameters != null)
                    _valueclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IValueClass>) _valueclass).Instantiate(_parameters);
                return _valueclass;
            }
        }

        QS.Fx.Reflection.IInterfaceClass QS.Fx.Reflection.Library.ILibrary.InterfaceClass<C>()
        {
            lock (this)
            {
                return this._GetInterfaceClass(typeof(C), new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        QS.Fx.Reflection.IInterfaceClass QS.Fx.Reflection.Library.ILibrary.InterfaceClass(string _uuid, params QS.Fx.Reflection.Parameter[] _parameters)
        {
            lock (this)
            {
                QS.Fx.Reflection.IInterfaceClass _interfaceclass = ((QS.Fx.Interface.Classes.ILibrary)this).GetInterfaceClass(_uuid);
                if (_parameters != null)
                    _interfaceclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IInterfaceClass>)_interfaceclass).Instantiate(_parameters);
                return _interfaceclass;
            }
        }

        QS.Fx.Reflection.IEndpointClass QS.Fx.Reflection.Library.ILibrary.EndpointClass<C>()
        {
            lock (this)
            {
                return this._GetEndpointClass(typeof(C), new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        QS.Fx.Reflection.IEndpointClass QS.Fx.Reflection.Library.ILibrary.EndpointClass(string _uuid, params QS.Fx.Reflection.Parameter[] _parameters)
        {
            lock (this)
            {
                QS.Fx.Reflection.IEndpointClass _endpointclass = ((QS.Fx.Interface.Classes.ILibrary) this).GetEndpointClass(_uuid);
                if (_parameters != null)
                    _endpointclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IEndpointClass>)_endpointclass).Instantiate(_parameters);
                return _endpointclass;
            }
        }

        QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.Library.ILibrary.ObjectClass<C>()
        {
            lock (this)
            {
                return this._GetObjectClass(typeof(C), new Dictionary<string, QS.Fx.Reflection.IParameter>());
            }
        }

        QS.Fx.Reflection.IObjectClass QS.Fx.Reflection.Library.ILibrary.ObjectClass(string _uuid, params QS.Fx.Reflection.Parameter[] _parameters)
        {
            lock (this)
            {
                QS.Fx.Reflection.IObjectClass _objectclass = ((QS.Fx.Interface.Classes.ILibrary) this).GetObjectClass(_uuid);
                if (_parameters != null)
                    _objectclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IObjectClass>)_objectclass).Instantiate(_parameters);
                return _objectclass;
            }
        }

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> QS.Fx.Reflection.Library.ILibrary.Object(string _uuid, params QS.Fx.Reflection.Parameter[] _parameters)
        {
            lock (this)
            {
                QS.Fx.Reflection.IComponentClass _componentclass = ((QS.Fx.Interface.Classes.ILibrary) this).GetComponentClass(_uuid);
                if (_parameters != null)
                    _componentclass = ((QS.Fx.Reflection.IClass<QS.Fx.Reflection.IComponentClass>)_componentclass).Instantiate(_parameters);
                return _componentclass;
            }
        }

        #endregion

        #region _RegisterInterfaceConstraintClass

        private QS.Fx.Reflection.IInterfaceConstraintClass _RegisterInterfaceConstraintClass(Type _type, bool _force)
        {
            QS.Fx.Reflection.IInterfaceConstraintClass _interfaceconstraintclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.InterfaceConstraintClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (_type.IsGenericType || _type.IsGenericTypeDefinition)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is a generic type.");
                if (!_type.IsClass || !_type.IsSealed)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is not a sealed class.");
                System.Reflection.ConstructorInfo _constructor = _type.GetConstructor(Type.EmptyTypes);
                if (_constructor == null)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not have a no-parameter constructor.");
                Type _type2 = typeof(QS.Fx.Reflection.IInterfaceConstraint<QS.Fx.Interface.ConstraintClasses.Some>).GetGenericTypeDefinition().MakeGenericType(_type);
                if ((_type2 == null) || !_type2.IsAssignableFrom(_type))
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not implement \"ObjectConstraint\" of itself.");
                if (!_interfaceconstraintclasses.TryGetValue(_type, out _interfaceconstraintclass))
                {
                    QS.Fx.Reflection.InterfaceConstraintClassAttribute _attribute = (QS.Fx.Reflection.InterfaceConstraintClassAttribute)_attributes[0];
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    InterfaceConstraintClass _m_interfaceconstraintclass =
                        new InterfaceConstraintClass
                        (
                            _namespace,
                            _attribute.ID,
                            _attribute.Version,
                            ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                            ((_attribute.Comment != null) ? _attribute.Comment : "This is an object constraint class based on the .NET type \"" + _type.FullName + "\"."),
                            _type,
                            null,
                            null
                        );
                    _interfaceconstraintclass = _m_interfaceconstraintclass;
                    this._RegisterClass(_type, _interfaceconstraintclass);
                    _namespace._RegisterClass(_attribute.ID, _interfaceconstraintclass);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a constraint class because it has not been decorated with the \"InterfaceConstraintClass\" attribute.");
            }
            return _interfaceconstraintclass;
        }

        #endregion

        #region _RegisterEndpointConstraintClass

        private QS.Fx.Reflection.IEndpointConstraintClass _RegisterEndpointConstraintClass(Type _type, bool _force)
        {
            QS.Fx.Reflection.IEndpointConstraintClass _endpointconstraintclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.EndpointConstraintClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (_type.IsGenericType || _type.IsGenericTypeDefinition)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is a generic type.");
                if (!_type.IsClass || !_type.IsSealed)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is not a sealed class.");
                System.Reflection.ConstructorInfo _constructor = _type.GetConstructor(Type.EmptyTypes);
                if (_constructor == null)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not have a no-parameter constructor.");
                Type _type2 = typeof(QS.Fx.Reflection.IEndpointConstraint<QS.Fx.Endpoint.ConstraintClasses.Secure>).GetGenericTypeDefinition().MakeGenericType(_type);
                if ((_type2 == null) || !_type2.IsAssignableFrom(_type))
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not implement \"EndpointConstraint\" of itself.");
                if (!_endpointconstraintclasses.TryGetValue(_type, out _endpointconstraintclass))
                {
                    QS.Fx.Reflection.EndpointConstraintClassAttribute _attribute = (QS.Fx.Reflection.EndpointConstraintClassAttribute)_attributes[0];
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    EndpointConstraintClass _m_endpointconstraintclass =
                        new EndpointConstraintClass
                        (
                            _namespace,
                            _attribute.ID,
                            _attribute.Version,
                            ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                            ((_attribute.Comment != null) ? _attribute.Comment : "This is an endpoint constraint class based on the .NET type \"" + _type.FullName + "\"."),
                            _type,
                            null,
                            null,
                            _constructor,
                            _type2
                        );
                    _endpointconstraintclass = _m_endpointconstraintclass;
                    this._RegisterClass(_type, _endpointconstraintclass);
                    _namespace._RegisterClass(_attribute.ID, _endpointconstraintclass);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a constraint class because it has not been decorated with the \"EndpointConstraintClass\" attribute.");
            }
            return _endpointconstraintclass;
        }

        #endregion

        #region _RegisterObjectConstraintClass

        private QS.Fx.Reflection.IObjectConstraintClass _RegisterObjectConstraintClass(Type _type, bool _force)
        {
            QS.Fx.Reflection.IObjectConstraintClass _objectconstraintclass = null;
            object[] _attributes;
            _attributes = _type.GetCustomAttributes(typeof(QS.Fx.Reflection.ObjectConstraintClassAttribute), false);
            if (_attributes != null && _attributes.Length > 0)
            {
                if (_type.IsGenericType || _type.IsGenericTypeDefinition)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is a generic type.");
                if (!_type.IsClass || !_type.IsSealed)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it is not a sealed class.");
                System.Reflection.ConstructorInfo _constructor = _type.GetConstructor(Type.EmptyTypes);
                if (_constructor == null)
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not have a no-parameter constructor.");
                Type _type2 = typeof(QS.Fx.Reflection.IObjectConstraint<QS.Fx.Object.ConstraintClasses.Secure>).GetGenericTypeDefinition().MakeGenericType(_type);
                if ((_type2 == null) || !_type2.IsAssignableFrom(_type))
                    throw new Exception("Cannot register type \"" + _type.ToString() + "\" as a constraint class because it does not implement \"ObjectConstraint\" of itself.");
                if (!_objectconstraintclasses.TryGetValue(_type, out _objectconstraintclass))
                {
                    QS.Fx.Reflection.ObjectConstraintClassAttribute _attribute = (QS.Fx.Reflection.ObjectConstraintClassAttribute)_attributes[0];
                    Namespace_ _namespace = _GetNamespaceOfType(_type);
                    ObjectConstraintClass _m_objectconstraintclass = 
                        new ObjectConstraintClass
                        (
                            _namespace, 
                            _attribute.ID, 
                            _attribute.Version,
                            ((_attribute.Name != null) ? _attribute.Name : _type.Name),
                            ((_attribute.Comment != null) ? _attribute.Comment : "This is an object constraint class based on the .NET type \"" + _type.FullName + "\"."),
                            _type, 
                            null, 
                            null,
                            _constructor,
                            _type2
                        );
                    _objectconstraintclass = _m_objectconstraintclass;
                    this._RegisterClass(_type, _objectconstraintclass);
                    _namespace._RegisterClass(_attribute.ID, _objectconstraintclass);
                }
            }
            else
            {
                if (_force)
                    throw new Exception("Cannot register type \"" + _type.ToString() +
                        "\" as a constraint class because it has not been decorated with the \"ObjectConstraintClass\" attribute.");
            }
            return _objectconstraintclass;
        }

        #endregion
    }
}
