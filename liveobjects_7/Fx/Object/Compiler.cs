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
using System.IO;

namespace QS.Fx.Object
{
    public static class Compiler
    {
        #region Compile

        public static void Compile(string _filename, string _output)
        {
            string _code;
            using (StreamReader _reader = new StreamReader(_filename))
            {
                _code = _reader.ReadToEnd();
            }
            string _result;
            try
            {
                QS._qss_p_.Structure_.ILibrary_ _library = QS._qss_p_.Parser_.Parser._Parse(_code);
                QS._qss_p_.Printing_.Printer_ _printer = new QS._qss_p_.Printing_.Printer_(QS._qss_p_.Printing_.Printer_.Type_._Compiled);
                _library._Print(_printer);
                _result = _printer.ToString();
            }
            catch (Exception _exc)
            {
                StringBuilder _ss = new StringBuilder();
                _ss.Append("// ");
                foreach (char _c in _exc.ToString())
                {
                    _ss.Append(_c);
                    if (_c.Equals('\n'))
                        _ss.Append("// ");
                }
                _result = _ss.ToString();
            }
            using (StreamWriter _writer = new StreamWriter(_output, false))
            {
                _writer.WriteLine(_result);
            }
        }

        #endregion

        #region Generate

        public static void Generate(string _root)
        {
            string _id = QS.Fx.Base.ID.NewID().ToString();
            string _guid = Guid.NewGuid().ToString();
            string _liveobjects = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

            using (StreamWriter _w = new StreamWriter(_root + Path.DirectorySeparatorChar + "library.sln", false))
            {                
                _w.WriteLine("Microsoft Visual Studio Solution File, Format Version 10.00");
                _w.WriteLine("# Visual Studio 2008");
                _w.WriteLine("Project(\"" + _guid + "\") = \"library\", \"library.csproj\", \"{536BD1A2-A8DE-4161-8E44-7B28AA21DC97}\"");
                _w.WriteLine("EndProject");
                _w.WriteLine("Global");
                _w.WriteLine("GlobalSection(SolutionConfigurationPlatforms) = preSolution");
                _w.WriteLine("Debug|Any CPU = Debug|Any CPU");
                _w.WriteLine("Release|Any CPU = Release|Any CPU");
                _w.WriteLine("EndGlobalSection");
                _w.WriteLine("GlobalSection(ProjectConfigurationPlatforms) = postSolution");
                _w.WriteLine("{536BD1A2-A8DE-4161-8E44-7B28AA21DC97}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                _w.WriteLine("{536BD1A2-A8DE-4161-8E44-7B28AA21DC97}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                _w.WriteLine("{536BD1A2-A8DE-4161-8E44-7B28AA21DC97}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                _w.WriteLine("{536BD1A2-A8DE-4161-8E44-7B28AA21DC97}.Release|Any CPU.Build.0 = Release|Any CPU");
                _w.WriteLine("EndGlobalSection");
                _w.WriteLine("GlobalSection(SolutionProperties) = preSolution");
                _w.WriteLine("HideSolutionNode = FALSE");
                _w.WriteLine("EndGlobalSection");
                _w.WriteLine("EndGlobal");
            }

            using (StreamWriter _w = new StreamWriter(_root + Path.DirectorySeparatorChar + "library.csproj", false))
            {
                _w.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                _w.WriteLine("<Project ToolsVersion=\"3.5\" DefaultTargets=\"Build\" xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">");
                _w.WriteLine("<PropertyGroup>");
                _w.WriteLine("<Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>");
                _w.WriteLine("<Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>");
                _w.WriteLine("<ProductVersion>9.0.30729</ProductVersion>");
                _w.WriteLine("<SchemaVersion>2.0</SchemaVersion>");
                _w.WriteLine("<ProjectGuid>" + _guid + "</ProjectGuid>");
                _w.WriteLine("<OutputType>Library</OutputType>");
                _w.WriteLine("<AppDesignerFolder>Properties</AppDesignerFolder>");
                _w.WriteLine("<RootNamespace>liveobjects_library_" + _id + "</RootNamespace>");
                _w.WriteLine("<AssemblyName>liveobjects_library_" + _id + "</AssemblyName>");
                _w.WriteLine("<TargetFrameworkVersion>v3.5</TargetFrameworkVersion>");
                _w.WriteLine("<FileAlignment>512</FileAlignment>");
                _w.WriteLine("<SccProjectName></SccProjectName>");
                _w.WriteLine("<SccLocalPath></SccLocalPath>");
                _w.WriteLine("<SccAuxPath></SccAuxPath>");
                _w.WriteLine("<SccProvider></SccProvider>");
                _w.WriteLine("</PropertyGroup>");
                _w.WriteLine("<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">");
                _w.WriteLine("<DebugSymbols>true</DebugSymbols>");
                _w.WriteLine("<DebugType>full</DebugType>");
                _w.WriteLine("<Optimize>false</Optimize>");
                _w.WriteLine("<OutputPath>bin\\Debug\\</OutputPath>");
                _w.WriteLine("<DefineConstants>DEBUG;TRACE</DefineConstants>");
                _w.WriteLine("<ErrorReport>prompt</ErrorReport>");
                _w.WriteLine("<WarningLevel>4</WarningLevel>");
                _w.WriteLine("<AllowUnsafeBlocks>true</AllowUnsafeBlocks>");
                _w.WriteLine("</PropertyGroup>");
                _w.WriteLine("<PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">");
                _w.WriteLine("<DebugType>pdbonly</DebugType>");
                _w.WriteLine("<Optimize>true</Optimize>");
                _w.WriteLine("<OutputPath>bin\\Release\\</OutputPath>");
                _w.WriteLine("<DefineConstants>TRACE</DefineConstants>");
                _w.WriteLine("<ErrorReport>prompt</ErrorReport>");
                _w.WriteLine("<WarningLevel>4</WarningLevel>");
                _w.WriteLine("</PropertyGroup>");
                _w.WriteLine("<ItemGroup>");
                _w.WriteLine("<Reference Include=\"liveobjects_1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=0f84102260c4a1aa, processorArchitecture=MSIL\">");
                _w.WriteLine("<SpecificVersion>False</SpecificVersion>");
                _w.WriteLine("<HintPath>" + Path.GetDirectoryName(_liveobjects) + "\\liveobjects_1.dll</HintPath>");
                _w.WriteLine("</Reference>");
                _w.WriteLine("<Reference Include=\"System\" />");
                _w.WriteLine("<Reference Include=\"System.Core\">");
                _w.WriteLine("<RequiredTargetFramework>3.5</RequiredTargetFramework>");
                _w.WriteLine("</Reference>");
                _w.WriteLine("<Reference Include=\"System.Data\" />");
                _w.WriteLine("<Reference Include=\"System.Xml\" />");
                _w.WriteLine("</ItemGroup>");
                _w.WriteLine("<ItemGroup>");
                _w.WriteLine("<Compile Include=\"Properties\\AssemblyInfo.cs\" />");
                _w.WriteLine("</ItemGroup>");
                _w.WriteLine("<ItemGroup>");
                _w.WriteLine("<Compile Include=\"library.cs\" />");
                _w.WriteLine("</ItemGroup>");
                _w.WriteLine("<ItemGroup>");
                _w.WriteLine("<None Include=\"library\" />");
                _w.WriteLine("</ItemGroup>");
                _w.WriteLine("<ItemGroup>");
                _w.WriteLine("<Folder Include=\"Properties\\\" />");
                _w.WriteLine("</ItemGroup>");
                _w.WriteLine("<Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />");
                _w.WriteLine("<PropertyGroup>");
                _w.WriteLine("<PreBuildEvent>" + _liveobjects + " -compile -output:$(ProjectDir)library.cs $(ProjectDir)library</PreBuildEvent>");
                _w.WriteLine("<PostBuildEvent>" + _liveobjects + " -deploy $(TargetPath)</PostBuildEvent>");
                _w.WriteLine("</PropertyGroup>");
                _w.WriteLine("</Project>");
            }

            string _propertiesroot = _root + Path.DirectorySeparatorChar + "Properties";
            if (!Directory.Exists(_propertiesroot))
                Directory.CreateDirectory(_propertiesroot);

            using (StreamWriter _w = new StreamWriter(_propertiesroot + Path.DirectorySeparatorChar + "AssemblyInfo.cs", false))
            {
                _w.WriteLine("using System.Reflection;");
                _w.WriteLine("using System.Runtime.CompilerServices;");
                _w.WriteLine("using System.Runtime.InteropServices;");
                _w.WriteLine("");
                _w.WriteLine("[assembly: AssemblyTitle(\"\")]");
                _w.WriteLine("[assembly: AssemblyDescription(\"\")]");
                _w.WriteLine("[assembly: AssemblyConfiguration(\"\")]");
                _w.WriteLine("[assembly: AssemblyCompany(\"\")]");
                _w.WriteLine("[assembly: AssemblyProduct(\"\")]");
                _w.WriteLine("[assembly: AssemblyCopyright(\"\")]");
                _w.WriteLine("[assembly: AssemblyTrademark(\"\")]");
                _w.WriteLine("[assembly: AssemblyCulture(\"\")]");
                _w.WriteLine("[assembly: AssemblyVersion(\"1.0.0.0\")]");
                _w.WriteLine("[assembly: AssemblyFileVersion(\"1.0.0.0\")]");
                _w.WriteLine("[assembly: ComVisible(false)]");
                _w.WriteLine("[assembly: Guid(\"" + Guid.NewGuid().ToString() + "\")]");
            }

            using (StreamWriter _w = new StreamWriter(_root + Path.DirectorySeparatorChar + "library", false))
            {
                _w.WriteLine("library %" + _id + "`1 \"Some Stuff\" somestuff");
                _w.WriteLine("{");
                _w.WriteLine("    object %1`1 \"foo\" foo");
	            _w.WriteLine("    {");
                _w.WriteLine("    }");
                _w.WriteLine("}");
            }
        }

        #endregion

        #region Deploy

        public static void Deploy(string _filename)
        {
            System.Reflection.Assembly _assembly = System.Reflection.Assembly.LoadFrom(_filename);
            object[] _attributes = _assembly.GetCustomAttributes(typeof(QS.Fx.Reflection.LibraryAttribute), false);
            if (_attributes.Length != 1)
                throw new Exception("Assembly \"" + _assembly.FullName + "\" does not have \"LibraryAttribute\" defined.");
            QS.Fx.Reflection.LibraryAttribute _attribute = (QS.Fx.Reflection.LibraryAttribute) _attributes[0];
            string _id = _attribute.ID.ToString();
            string _version = _attribute.Version.ToString();
            string _name = _attribute.Name;
            string _description = _attribute.Description;
            string _liveobjects = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            string _root0 = Directory.GetParent(Path.GetDirectoryName(_liveobjects)).FullName;
            string _root1 = _root0 + Path.DirectorySeparatorChar + "libraries" + Path.DirectorySeparatorChar + _id;
            string _root2 = _root1 + Path.DirectorySeparatorChar + _version;
            string _root3 = _root2 + Path.DirectorySeparatorChar + "data";
            string _pdbfilename = Path.GetDirectoryName(_filename) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(_filename) + ".pdb";
            if (!Directory.Exists(_root1))
                Directory.CreateDirectory(_root1);
            if (Directory.Exists(_root2))
                Directory.Delete(_root2, true);
            Directory.CreateDirectory(_root2);
            Directory.CreateDirectory(_root3);
            File.Copy(_filename, _root3 + Path.DirectorySeparatorChar + Path.GetFileName(_filename));
            if (File.Exists(_pdbfilename))
                File.Copy(_pdbfilename, _root3 + Path.DirectorySeparatorChar + Path.GetFileName(_pdbfilename));
            using (StreamWriter _w = new StreamWriter(_root2 + Path.DirectorySeparatorChar + "metadata.xml", false))
            {
                _w.WriteLine("<Library id=\"" + _id + "`" + _version + "\">");
                _w.WriteLine("<Include filename=\"" + Path.GetFileName(_filename) + "\"/>");
                _w.WriteLine("</Library>");
            }
        }

        #endregion
    }
}
