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

#include "stdafx.h"
#include <string.h>

// #define IGNORE_LIVEOBJECTS_8
 #define IGNORE_LIVEOBJECTS_9
// #define NOT_REFERENCING_INTERNAL_LIBRARIES
// #define DEBUGGING_JUST_RUN_SOME_TEST_CODE
// #define DEBUGGING_DISPLAY_INSPECTION_USER_INTERFACE
// #define DEBUGGING_SERVICE

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Text;
using namespace System::Security::Policy;
using namespace System::Reflection;
using namespace System::Threading;
using namespace System::IO;
using namespace System::Xml;
using namespace System::Xml::Serialization;
using namespace System::Windows::Forms;
using namespace System::ServiceProcess;

#if defined(DEBUGGING_DISPLAY_INSPECTION_USER_INTERFACE)

void _inspector_thread_callback()
{
	System::Windows::Forms::Application::Run(
		gcnew QS::GUI::Components::InspectionBox(
			QS::_qss_x_::Reflection_::Library::LocalLibrary));
}

System::Threading::Thread ^ _launch_inspector_window()
{
	System::Threading::Thread ^inspectorthread = 
		gcnew System::Threading::Thread(
			gcnew System::Threading::ThreadStart(&_inspector_thread_callback));
	inspectorthread->ApartmentState = System::Threading::ApartmentState::STA;
	inspectorthread->Start();
	return inspectorthread;
}

#endif

[STAThreadAttribute]
int main(array<String ^> ^args)
{
	try
	{
		String^ CURRENTDIR = Environment::CurrentDirectory;
		System::Diagnostics::ProcessModule^ mainmodule = System::Diagnostics::Process::GetCurrentProcess()->MainModule;
		String^ mainroot = System::IO::Directory::GetParent(System::IO::Path::GetDirectoryName(mainmodule->FileName))->FullName;
		String^ temproot = System::IO::Path::Combine(mainroot, L"temp");
		String^ myid;
		String^ mytemp;
		do
		{
			DateTime^ now = System::DateTime::Now::get();
			Random^ random = gcnew System::Random();			
			myid = now->ToString(L"yyyyMMddHHmmssff") + 
				System::Diagnostics::Process::GetCurrentProcess()->Id.ToString("0000000000") + 
				random->Next(10000).ToString(L"0000");
			mytemp = System::IO::Path::GetFullPath(System::IO::Path::Combine(temproot, myid));
		}
		while (System::IO::Directory::Exists(mytemp));
		System::IO::Directory::CreateDirectory(mytemp);
		System::IO::Directory::SetCurrentDirectory(mytemp);
		try	
		{
			QS::_assembly_liveobjects_1::_initialize();
#if defined(NOT_REFERENCING_INTERNAL_LIBRARIES)
#else
			QS::_assembly_liveobjects_4::_initialize();
			QS::_assembly_liveobjects_5::_initialize();
			QS::_assembly_liveobjects_6::_initialize();
			QS::_assembly_liveobjects_7::_initialize();
#if defined(IGNORE_LIVEOBJECTS_8)
#else
			QS::_assembly_liveobjects_8::_initialize();
#endif
#if defined(IGNORE_LIVEOBJECTS_9)
#else
			QS::_assembly_liveobjects_9::_initialize();
#endif
#endif
		}
		catch (Exception ^exc)
		{
			throw gcnew Exception(L"Initialization failed for one of the internal assemblies.", exc);
		}

		String^ runtimeroot = QS::Fx::Object::Runtime::ROOT;
		String^ scenariosroot = System::IO::Path::Combine(mainroot, L"scenarios");
		if (!Directory::Exists(scenariosroot))
			scenariosroot = System::IO::Path::Combine(runtimeroot, L"scenarios");
		String^ examplesroot = System::IO::Path::Combine(mainroot, L"examples");
		if (!Directory::Exists(examplesroot))
			examplesroot = System::IO::Path::Combine(runtimeroot, L"examples");

	//	QS::Fx::Reflection::IInterfaceClass^ foo1 = 
	//		QS::_qss_x_::Reflection_::Library::LocalLibrary->GetInterfaceClass("0`0:90E1B0E57AA741719D958642DCE67122`1");
	//	
	//	QS::Fx::Reflection::IInterfaceClass^ foo2 = 
	//		QS::_qss_x_::Reflection_::Library::LocalLibrary->GetInterfaceClass("0`0:C50F0744AD474A06832587B45F6B279C`1");
		
#if defined(DEBUGGING_JUST_RUN_SOME_TEST_CODE)

		System::Text::StringBuilder ^ss = gcnew System::Text::StringBuilder();

		// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		// @
		// @   SOME TEST CODE
		// @____________________________________________________________________________________________________

		


		// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

		System::Windows::Forms::Form^ form = gcnew System::Windows::Forms::Form();
		System::Windows::Forms::RichTextBox^ textbox = gcnew System::Windows::Forms::RichTextBox();
		textbox->Dock = System::Windows::Forms::DockStyle::Fill;
		textbox->Text = ss->ToString();
		form->Controls->Add(textbox);
		System::Windows::Forms::Application::Run(form);

		return 0;

#endif

#if defined(NOT_REFERENCING_INTERNAL_LIBRARIES)
		String^ myfolder = Path::GetDirectoryName(System::Diagnostics::Process::GetCurrentProcess()->MainModule->FileName);
		Assembly^ mainassembly = nullptr;
		for (int ind = 4; ind < 8; ind++)
		{
			String^ assemblyname = L"liveobjects_" + ind.ToString();
			String^ assemblyfile = myfolder + Path::DirectorySeparatorChar + assemblyname + L".dll";
			Assembly^ assembly = nullptr;
			try
			{
				assembly = Assembly::LoadFrom(assemblyfile);
			}
			catch (Exception^ exc)
			{
				throw gcnew Exception(L"Could not load assembly \"" + assemblyname + L"\" from file \"" + assemblyfile + L"\".", exc);
			}
			if (assembly == nullptr)
				throw gcnew Exception(L"Assembly object \"" + assemblyname + L"\" loaded from file \"" + assemblyfile + L"\" is null.");
			if (ind == 6)
				mainassembly = assembly;
			Type^ inittype = assembly->GetType(L"QS._assembly_" + assemblyname);
			if (inittype == nullptr)
				throw gcnew Exception(L"Could not find initialization type in assembly \"" + assemblyname + L"\".");
			MethodInfo^ initmethod = inittype->GetMethod(L"_initialize", BindingFlags::Static | BindingFlags::Public);
			if (initmethod == nullptr)
				throw gcnew Exception(L"The initialization type in assembly \"" + assemblyname + 
					L"\" does not appear to have the correct initialization method.");
			try	
			{
				initmethod->Invoke(nullptr, gcnew array<Object^>(0));
			}
			catch (Exception ^exc)
			{
				throw gcnew Exception(L"Initialization failed for assembly \"" + assemblyname + L"\".", exc);
			}
		}
#else
#endif	
		if (args->Length > 0)
		{
			String^ _first_lower = args[0]->ToLower();
			if (_first_lower->Equals("/install") || _first_lower->Equals("-install") || _first_lower->Equals("/i") || _first_lower->Equals("-i"))
			{
				array<String^>^ _myargs = System::Environment::GetCommandLineArgs();
				array<String^>^ _args = gcnew array<String^>(_myargs->Length - 1);
				_args[0] = Assembly::GetExecutingAssembly()->Location;
				Array::Copy(_myargs, 2, _args, 1, _args->Length - 1);
				AppDomain^ _dom = AppDomain::CreateDomain(L"execDom");
				Type^ _type = System::Object::typeid;
				String^ _path = _type->Assembly->Location;
				StringBuilder^ _sb = gcnew StringBuilder(_path->Substring(0, _path->LastIndexOf(L"\\")));
				_sb->Append(L"\\InstallUtil.exe");
				_dom->ExecuteAssembly(_sb->ToString(), gcnew Evidence(), _args);
			}
			else if (_first_lower->Equals("/simulate") || _first_lower->Equals("-simulate") || _first_lower->Equals("/s") || _first_lower->Equals("-s"))
			{
				System::String^		_parameter_name_nnodes				= "nnodes";
	/*
				System::String^		_parameter_name_nclients				= "nclients";
				System::String^		_parameter_name_nservers				= "nservers";
	*/
				System::String^		_parameter_name_mttb					= "mttb";
				System::String^		_parameter_name_mttf						= "mttf";
				System::String^		_parameter_name_mttr						= "mttr";
				System::String^		_parameter_name_cpu						= "cpu";
				System::String^		_parameter_name_speed				= "speed";
				System::String^		_parameter_name_bandwidth			= "bandwidth";
				System::String^		_parameter_name_recovery				= "recovery";
				System::String^		_parameter_name_loss					= "loss";
	/*
				System::String^		_parameter_name_client					= "client";
				System::String^		_parameter_name_service				= "service";
				System::String^		_parameter_name_bootstrap			= "bootstrap";
				System::String^		_parameter_name_test						= "test";
	*/
				System::String^		_parameter_name_scenario				= "scenario";

				int								_default_nnodes								= 5;
	/*
				int								_default_nclients								= 100;
				int								_default_nservers								= 0;
	*/
				double						_default_mttb										= 1;
				double						_default_mttf										= 3600;
				double						_default_mttr										= 60;
				double						_default_cpu										= 100;
				double						_default_speed									= 1000000000;
				double						_default_bandwidth							= 100;
				double						_default_recovery								= 100;
				double						_default_loss										= 0;
	/*
				System::String^		_default_client									= L"";
				System::String^		_default_service								= L"";
				System::String^		_default_bootstrap								= L"";
	*/
				System::String^		_default_scenario								= L"foo";

				System::Windows::Forms::Application::EnableVisualStyles();
				System::Windows::Forms::Application::SetCompatibleTextRenderingDefault(false); 

				QS::_core_c_::Components::AttributeSet^ _parameters = gcnew QS::_core_c_::Components::AttributeSet(args);

				int _parameter_nnodes = _parameters->contains(_parameter_name_nnodes) ? Convert::ToInt32(_parameters[_parameter_name_nnodes]) : _default_nnodes;
	/*
				int _parameter_nclients = _parameters->contains(_parameter_name_nclients) ? Convert::ToInt32(_parameters[_parameter_name_nclients]) : _default_nclients;
				int _parameter_nservers = _parameters->contains(_parameter_name_nservers) ? Convert::ToInt32(_parameters[_parameter_name_nservers]) : _default_nservers;
	*/
				System::String^ _parameter_subnet = 
					"100.0.0.0/" + (32 - ((int) System::Math::Ceiling(System::Math::Log((double)(_parameter_nnodes + 2), 2.0)))).ToString();
				double _parameter_mttb = _parameters->contains(_parameter_name_mttb) ? Convert::ToDouble(_parameters[_parameter_name_mttb]) : _default_mttb;    
				double _parameter_mttf = _parameters->contains(_parameter_name_mttf) ? Convert::ToDouble(_parameters[_parameter_name_mttf]) : _default_mttf;
				double _parameter_mttr = _parameters->contains(_parameter_name_mttr) ? Convert::ToDouble(_parameters[_parameter_name_mttr]) : _default_mttr;
				double _parameter_cpu = (_parameters->contains(_parameter_name_cpu) ? Convert::ToDouble(_parameters[_parameter_name_cpu]) : _default_cpu) / 100.0;
				double _parameter_speed = _parameters->contains(_parameter_name_speed) ? Convert::ToDouble(_parameters[_parameter_name_speed]) : _default_speed;
				double _parameter_bandwidth = _parameters->contains(_parameter_name_bandwidth) ? Convert::ToDouble(_parameters[_parameter_name_bandwidth]) : _default_bandwidth;
				double _parameter_recovery = _parameters->contains(_parameter_name_recovery) ? Convert::ToDouble(_parameters[_parameter_name_recovery]) : _default_recovery;
				double _parameter_loss = _parameters->contains(_parameter_name_loss) ? Convert::ToDouble(_parameters[_parameter_name_loss]) : _default_loss;
				System::String^ _parameter_scenario = _parameters->contains(_parameter_name_scenario) ? ((System::String^) _parameters[_parameter_name_scenario]) : _default_scenario;
				if (!(_parameter_scenario->Contains("/") || _parameter_scenario->Contains("\\") || _parameter_scenario->Contains(Convert::ToString(Path::DirectorySeparatorChar))))
				{
					if (!_parameter_scenario->EndsWith(".xml"))
						_parameter_scenario = _parameter_scenario + ".xml";
					String^ _scenario_location = CURRENTDIR + "\\" + _parameter_scenario;
					if (File::Exists(_scenario_location))
						_parameter_scenario = _scenario_location;
					else
					{
						_scenario_location = scenariosroot + "\\" + _parameter_scenario;
						if (File::Exists(_scenario_location))
							_parameter_scenario = _scenario_location;
						else
							throw gcnew Exception("Cannot locate the scenario file.");
					}
				}
				System::IO::StreamReader^ _streamreader = gcnew System::IO::StreamReader(_parameter_scenario);
				_parameter_scenario = _streamreader->ReadToEnd();
				_streamreader->Close();

	/*
				if (_parameters->contains(L"asynchronous") || _parameters->contains(L"async") || _parameters->contains(L"a"))
				{
					QS::Fx::Object::Runtime::SynchronizationOption = 
						(QS::Fx::Object::Runtime::SynchronizationOption & ~QS::Fx::Base::SynchronizationOption::Synchronous) | 
							QS::Fx::Base::SynchronizationOption::Asynchronous;
				}				

				if (_parameters->contains(L"multithreaded") || _parameters->contains(L"multi") || _parameters->contains(L"m"))
				{
					QS::Fx::Object::Runtime::SynchronizationOption = 
						(QS::Fx::Object::Runtime::SynchronizationOption & ~QS::Fx::Base::SynchronizationOption::Singlethreaded) | 
							QS::Fx::Base::SynchronizationOption::Multithreaded;
				}				
	*/

	/*
				System::String^ _parameter_client;
				System::String^ _parameter_service;
				System::String^ _parameter_bootstrap;
				if (_parameters->contains(_parameter_name_test))
				{
					_parameter_client = System::IO::Path::Combine(examplesroot, "test_" + ((System::String^) _parameters[_parameter_name_test]) + "_client.liveobject");
					_parameter_service = System::IO::Path::Combine(examplesroot, "test_" + ((System::String^) _parameters[_parameter_name_test]) + "_service.liveobject");
					_parameter_bootstrap = System::IO::Path::Combine(examplesroot, "test_" + ((System::String^) _parameters[_parameter_name_test]) + "_bootstrap.liveobject");
				}
				else
				{
					_parameter_client = _parameters->contains(_parameter_name_client) ? ((System::String^) _parameters[_parameter_name_client]) : _default_client;
					_parameter_service = _parameters->contains(_parameter_name_service) ? ((System::String^) _parameters[_parameter_name_service]) : _default_service;
					_parameter_bootstrap = _parameters->contains(_parameter_name_bootstrap) ? ((System::String^) _parameters[_parameter_name_bootstrap]) : _default_bootstrap;
				}

				System::IO::StreamReader^ _streamreader = gcnew System::IO::StreamReader(_parameter_client);
				_parameter_client = _streamreader->ReadToEnd();
				_streamreader->Close();
				_streamreader = gcnew System::IO::StreamReader(_parameter_service);
				_parameter_service = _streamreader->ReadToEnd();
				_streamreader->Close();
				_streamreader = gcnew System::IO::StreamReader(_parameter_bootstrap);
				_parameter_bootstrap = _streamreader->ReadToEnd();
				_streamreader->Close();
	*/

				System::Collections::Generic::IDictionary<System::String^, System::String^>^ _application_parameters = 
					gcnew System::Collections::Generic::Dictionary<System::String^, System::String^>();

				for each (System::String^ _name in _parameters->Attributes->Keys)
				{
					if (!_name->Equals(_parameter_name_nnodes) && 
	/*
						!_name->Equals(_parameter_name_nclients) && 
						!_name->Equals(_parameter_name_nservers) && 
	*/
						!_name->Equals(_parameter_name_mttb) && 
						!_name->Equals(_parameter_name_mttf) && 
						!_name->Equals(_parameter_name_mttr) &&
						!_name->Equals(_parameter_name_cpu) && 
						!_name->Equals(_parameter_name_speed) && 
						!_name->Equals(_parameter_name_bandwidth) && 
						!_name->Equals(_parameter_name_recovery) && 
						!_name->Equals(_parameter_name_loss) &&
	/*
						!_name->Equals(_parameter_name_client) &&
						!_name->Equals(_parameter_name_service) &&
						!_name->Equals(_parameter_name_bootstrap) &&
	*/
						!_name->Equals(_parameter_name_scenario))
					{
						_application_parameters->Add(_name, (System::String^) _parameters->Attributes[_name]);
					}
				}

	/*
				_application_parameters->Add(_parameter_name_client, _parameter_client);
				_application_parameters->Add(_parameter_name_service, _parameter_service);
				_application_parameters->Add(_parameter_name_bootstrap, _parameter_bootstrap);
	*/

				System::Diagnostics::Process::GetCurrentProcess()->ProcessorAffinity = IntPtr(1);
				if (System::Diagnostics::Process::GetCurrentProcess()->ProcessorAffinity.ToInt32() != 1)
					throw gcnew Exception("Could not set change process affinity.");

				QS::_core_c_::Core::Clock^ _clock = gcnew QS::_core_c_::Core::Clock();

				QS::Fx::Object::Runtime::Run
				(
					_clock, 
					_parameter_nnodes,
	/*
					_parameter_nclients,
					_parameter_nservers,
	*/
					_parameter_subnet, 
					_parameter_mttb, 
					_parameter_mttf,
					_parameter_mttr, 
					_parameter_cpu, 
					_parameter_speed, 
					_parameter_bandwidth, 
					_parameter_recovery,
					_parameter_loss, 
					_parameter_scenario, 
	/*
					_parameter_client, 
					_parameter_service,
					_parameter_bootstrap,
	*/
					_application_parameters
				);

				QS::Fx::Object::Runtime::Shutdown();
			}
			else if (_first_lower->Equals("/compile") || _first_lower->Equals("-compile") || _first_lower->Equals("/c") || _first_lower->Equals("-c"))
			{
				String^ _name = nullptr;
				String^ _output = nullptr;

				IDictionary<String^, int>^ _map = gcnew Dictionary<String^, int>();
				const int PARAMETER_NONE = 0;
				const int PARAMETER_OUTPUT = 1;				
				_map->Add("output", PARAMETER_OUTPUT);
				_map->Add("out", PARAMETER_OUTPUT);
				_map->Add("o", PARAMETER_OUTPUT);				
				for (int i = 1; i < args->Length; i++)
				{
					int paramno = 0;
					String^ param = nullptr;
					if (args[i]->StartsWith("/") || args[i]->StartsWith("-"))
					{
						int separator = args[i]->IndexOf(":");
						if ((separator < 0) || (separator >= args[i]->Length))
							separator = args[i]->Length;
						String^ parname = args[i]->Substring(1, separator - 1);
						if (_map->TryGetValue(parname, paramno))
						{
							if ((separator >= 0) && (separator < args[i]->Length))
								param = args[i]->Substring(separator + 1);
						}
						else
							paramno = PARAMETER_NONE;
					}
					switch (paramno)
					{
						case PARAMETER_OUTPUT:
							_output = System::IO::Path::GetFullPath(param);
							break;

						default:
							if (_name == nullptr)
								_name = System::IO::Path::GetFullPath(args[i]);
							else
								throw gcnew Exception("Cannot compile more than one file at a time.");
							break;
					}
				}

				if (_name == nullptr)
					Console::WriteLine("Cannot compile, the file to compile has not been specified.");
				else
				{
					QS::Fx::Object::Compiler::Compile(_name, _output);
				}
			}
			else if (_first_lower->Equals("/generate") || _first_lower->Equals("-generate") || _first_lower->Equals("/g") || _first_lower->Equals("-g"))
			{
				String^ _name = nullptr;
				String^ _output = CURRENTDIR;

				IDictionary<String^, int>^ _map = gcnew Dictionary<String^, int>();
				const int PARAMETER_NONE = 0;
				const int PARAMETER_OUTPUT = 1;				
				_map->Add("output", PARAMETER_OUTPUT);
				_map->Add("out", PARAMETER_OUTPUT);
				_map->Add("o", PARAMETER_OUTPUT);				
				for (int i = 1; i < args->Length; i++)
				{
					int paramno = 0;
					String^ param = nullptr;
					if (args[i]->StartsWith("/") || args[i]->StartsWith("-"))
					{
						int separator = args[i]->IndexOf(":");
						if ((separator < 0) || (separator >= args[i]->Length))
							separator = args[i]->Length;
						String^ parname = args[i]->Substring(1, separator - 1);
						if (_map->TryGetValue(parname, paramno))
						{
							if ((separator >= 0) && (separator < args[i]->Length))
								param = args[i]->Substring(separator + 1);
						}
						else
							paramno = PARAMETER_NONE;
					}
					switch (paramno)
					{
						case PARAMETER_OUTPUT:
							_output = System::IO::Path::GetFullPath(param);
							break;

						default:
							if (_name == nullptr)
								_name = System::IO::Path::GetFullPath(args[i]);
							else
								throw gcnew Exception("Cannot generate more than one library at a time.");
							break;
					}
				}

				if (!System::IO::Directory::Exists(_output))
					throw gcnew Exception("Cannot generate, the target directory \"" + _output + "\" does not exist.");
				else
				{
					QS::Fx::Object::Compiler::Generate(_output);
				}
			}
			else if (_first_lower->Equals("/deploy") || _first_lower->Equals("-deploy"))
			{
				String^ _filetodeploy = nullptr;
				//String^ _output = CURRENTDIR;

				IDictionary<String^, int>^ _map = gcnew Dictionary<String^, int>();
				const int PARAMETER_NONE = 0;
				//const int PARAMETER_OUTPUT = 1;				
				//_map->Add("output", PARAMETER_OUTPUT);
				//_map->Add("out", PARAMETER_OUTPUT);
				//_map->Add("o", PARAMETER_OUTPUT);				
				for (int i = 1; i < args->Length; i++)
				{
					int paramno = 0;
					String^ param = nullptr;
					if (args[i]->StartsWith("/") || args[i]->StartsWith("-"))
					{
						int separator = args[i]->IndexOf(":");
						if ((separator < 0) || (separator >= args[i]->Length))
							separator = args[i]->Length;
						String^ parname = args[i]->Substring(1, separator - 1);
						if (_map->TryGetValue(parname, paramno))
						{
							if ((separator >= 0) && (separator < args[i]->Length))
								param = args[i]->Substring(separator + 1);
						}
						else
							paramno = PARAMETER_NONE;
					}
					switch (paramno)
					{
						//case PARAMETER_OUTPUT:
						//	_output = System::IO::Path::GetFullPath(param);
						//	break;
						default:
							if (_filetodeploy == nullptr)
								_filetodeploy = System::IO::Path::GetFullPath(args[i]);
							else
								throw gcnew Exception("Cannot deploy more than one library at a time.");
							break;
					}
				}

				if (!System::IO::File::Exists(_filetodeploy))
					throw gcnew Exception("Cannot deploy, the file \"" + _filetodeploy + "\" does not exist.");
				else
				{
					QS::Fx::Object::Compiler::Deploy(_filetodeploy);
				}
			}
			else
			{
#if defined(NOT_REFERENCING_INTERNAL_LIBRARIES)
				Type^ runtype = mainassembly->GetType(L"QS.Fx.Object.Runtime");
				array<Type^>^ runtypeargtypes = gcnew array<Type^>(2);
				runtypeargtypes[0] = System::String::typeid;
				runtypeargtypes[1] = System::Boolean::typeid;
				MethodInfo^ runmethod = runtype->GetMethod(L"Run", runtypeargtypes);
				array<Object^>^ runtypeargs = gcnew array<Object^>(2);
				runtypeargs[0] = args[0];
				runtypeargs[1] = true;
				runmethod->Invoke(nullptr, runtypeargs);
#else
#if defined(DEBUGGING_DISPLAY_INSPECTION_USER_INTERFACE)
				System::Windows::Forms::Application::Run(
					gcnew QS::GUI::Components::InspectionBox(
						QS::_qss_x_::Reflection_::Library::LocalLibrary));
				// System::Threading::Thread ^inspectorthread = _launch_inspector_window();
#endif

				String^ _name = nullptr;
				IDictionary<String^, int>^ _map = gcnew Dictionary<String^, int>();
				IDictionary<String^, String^>^ _configurationoptions = gcnew Dictionary<String^, String^>();

				const int PARAMETER_DEBUG = 1;
				const int PARAMETER_ASYNCHRONOUS = 2;
				const int PARAMETER_MULTITHREADED = 3;
				const int PARAMETER_PINTHREADS = 4;
				const int PARAMETER_INSPECTION = 5;
				const int PARAMETER_POLICY = 6;
				const int PARAMETER_REPLICATION = 7;
				const int PARAMETER_EXTERNAL = 8;
				const int PARAMETER_NUMBEROFWORKERS = 9;
				const int PARAMETER_OWNER = 10;
				const int PARAMETER_SERIALIZATION = 11;
				const int PARAMETER_FLOWCONTROL = 12;
				const int PARAMETER_SYNCHRONOUS = 13;
				const int PARAMETER_BYPASS = 14;
				const int PARAMETER_TRANSFER = 15;
				const int PARAMETER_CONFIGURE = 16;
				const int PARAMETER_LOGFILE = 17;
				const int PARAMETER_SILENT = 18;
				const int PARAMETER_QSMTTL = 19;

				_map->Add("debug", PARAMETER_DEBUG);
				_map->Add("dbg", PARAMETER_DEBUG);
				_map->Add("d", PARAMETER_DEBUG);
				
				_map->Add("asynchronous", PARAMETER_ASYNCHRONOUS);
				_map->Add("async", PARAMETER_ASYNCHRONOUS);
				_map->Add("a", PARAMETER_ASYNCHRONOUS);
				
				_map->Add("multithreaded", PARAMETER_MULTITHREADED);
				_map->Add("multi", PARAMETER_MULTITHREADED);
				_map->Add("m", PARAMETER_MULTITHREADED);

				_map->Add("pinthreads", PARAMETER_PINTHREADS);
				_map->Add("pin", PARAMETER_PINTHREADS);
				_map->Add("p", PARAMETER_PINTHREADS);

				_map->Add("q", PARAMETER_INSPECTION);

				_map->Add("r", PARAMETER_REPLICATION);

				_map->Add("x", PARAMETER_EXTERNAL);

				_map->Add("n", PARAMETER_NUMBEROFWORKERS);

				_map->Add("o", PARAMETER_OWNER);

				_map->Add("serialization", PARAMETER_SERIALIZATION);

				_map->Add("flowcontrol", PARAMETER_FLOWCONTROL);
				_map->Add("flow", PARAMETER_FLOWCONTROL);
				_map->Add("fc", PARAMETER_FLOWCONTROL);
				_map->Add("f", PARAMETER_FLOWCONTROL);

				_map->Add("y", PARAMETER_SYNCHRONOUS);

				_map->Add("bypass", PARAMETER_BYPASS);
				_map->Add("b", PARAMETER_BYPASS);

				_map->Add("t", PARAMETER_TRANSFER);

				_map->Add("config", PARAMETER_CONFIGURE);
				_map->Add("cfg", PARAMETER_CONFIGURE);
				_map->Add("c", PARAMETER_CONFIGURE);

				_map->Add("logfile", PARAMETER_LOGFILE);
				_map->Add("log", PARAMETER_LOGFILE);
				_map->Add("l", PARAMETER_LOGFILE);

				_map->Add("silent", PARAMETER_SILENT);

				_map->Add("qsmttl", PARAMETER_QSMTTL);
				

				StringBuilder^ _commandline = gcnew StringBuilder();
				bool _hasspace = false;
				bool pinthreads = false;

				//Qi
				if (_first_lower->Equals("/deploy"))
				{	
					QS::Fx::Object::Runtime::Silent = true;
				}

				for (int i = 0; i < args->Length; i++)
				{
					bool _addtocommandline = true;
					int paramno = 0;
					if (args[i]->StartsWith("/") || args[i]->StartsWith("-"))
					{
						int separator = args[i]->IndexOf(":");
						if ((separator < 0) || (separator >= args[i]->Length))
							separator = args[i]->Length;
						String^ parname = args[i]->Substring(1, separator - 1);
						if (!_map->TryGetValue(parname, paramno))
							paramno = 0;
					}
					switch (paramno)
					{
						case PARAMETER_DEBUG:
						{
							QS::Fx::Object::Runtime::Debug = true;
						}
						break;

						case PARAMETER_SILENT:
						{
							QS::Fx::Object::Runtime::Silent = true;
						}

						case PARAMETER_ASYNCHRONOUS:
						{
							QS::Fx::Object::Runtime::SynchronizationOption = 
								(QS::Fx::Object::Runtime::SynchronizationOption & ~QS::Fx::Base::SynchronizationOption::Synchronous) | 
									QS::Fx::Base::SynchronizationOption::Asynchronous;
						}
						break;

						case PARAMETER_MULTITHREADED:
						{
							QS::Fx::Object::Runtime::SynchronizationOption = 
								(QS::Fx::Object::Runtime::SynchronizationOption & ~QS::Fx::Base::SynchronizationOption::Singlethreaded) | 
									QS::Fx::Base::SynchronizationOption::Multithreaded;
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::Concurrency = Convert::ToInt32(args[i]->Substring(_delimiter + 1));
							}
							else
							{
								QS::Fx::Object::Runtime::Concurrency = 2 * Environment::ProcessorCount;
							}
						}
						break;

						case PARAMETER_PINTHREADS:
						{
							pinthreads = true;
						}
						break;

						case PARAMETER_INSPECTION:
						{
							QS::Fx::Object::Runtime::Inspection = true;
						}
						break;

						case PARAMETER_POLICY:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::Policy = ((QS::Fx::Scheduling::Policy) Convert::ToInt32(args[i]->Substring(_delimiter + 1)));
							}
						}
						break;

						case PARAMETER_REPLICATION:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								int _count = Convert::ToInt32(args[i]->Substring(_delimiter + 1));
								QS::Fx::Object::Runtime::NumberOfReplicas = _count;
								if (_count > 0)
									QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::ToProcess;
								else
									QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication & ~QS::Fx::Replication::Policy::ToProcess;
							}
							else
							{
								QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::ToProcess;
								QS::Fx::Object::Runtime::NumberOfReplicas = 0;
							}
							QS::Fx::Object::Runtime::SynchronizationOption = 
								(QS::Fx::Object::Runtime::SynchronizationOption
									& ~QS::Fx::Base::SynchronizationOption::Singlethreaded
									& ~QS::Fx::Base::SynchronizationOption::Synchronous) 
									| QS::Fx::Base::SynchronizationOption::Multithreaded
									| QS::Fx::Base::SynchronizationOption::Asynchronous;
							if (QS::Fx::Object::Runtime::Concurrency == 0)
								QS::Fx::Object::Runtime::Concurrency = 2 * Environment::ProcessorCount;
						}
						break;

						case PARAMETER_EXTERNAL:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::ExternalConfiguration = args[i]->Substring(_delimiter + 1);
								QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::ToNetwork;
							}
						}
						break;

						case PARAMETER_NUMBEROFWORKERS:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								int _count = Convert::ToInt32(args[i]->Substring(_delimiter + 1));
								QS::Fx::Object::Runtime::NumberOfWorkers = _count;
								if (_count > 0)
									QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::ToMachine;
								else
									QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication & ~QS::Fx::Replication::Policy::ToMachine;
							}
							else
							{
								QS::Fx::Object::Runtime::NumberOfWorkers = 1;
								QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::ToMachine;
							}
						}
						break;

						case PARAMETER_OWNER:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::Owner = args[i]->Substring(_delimiter + 1);
							}
						}
						break;						

						case PARAMETER_SERIALIZATION:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								String^ _serializationtype = args[i]->Substring(_delimiter + 1);
								if (_serializationtype->Equals("binary"))
									QS::Fx::Object::Runtime::SerializationType = 1;
								else if (_serializationtype->Equals("xml"))
									QS::Fx::Object::Runtime::SerializationType = 2;
								else if (_serializationtype->Equals("internal"))
									QS::Fx::Object::Runtime::SerializationType = 3;
								else 
									QS::Fx::Object::Runtime::SerializationType = 0;
							}
						}
						break;						

						case PARAMETER_FLOWCONTROL:
						{
							QS::Fx::Object::Runtime::FlowControlEnabled = true;
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								double _interval = Convert::ToDouble(args[i]->Substring(_delimiter + 1));
								QS::Fx::Object::Runtime::FlowControlInterval = _interval;
							}
							else
								QS::Fx::Object::Runtime::FlowControlInterval = 1;
						}
						break;		

						case PARAMETER_SYNCHRONOUS:
						{
							QS::Fx::Object::Runtime::SynchronizationOption = 
								(QS::Fx::Object::Runtime::SynchronizationOption 
									& ~QS::Fx::Base::SynchronizationOption::Asynchronous
									& ~QS::Fx::Base::SynchronizationOption::Multithreaded
									& ~QS::Fx::Base::SynchronizationOption::Replicated) 
									| QS::Fx::Base::SynchronizationOption::Synchronous 
									| QS::Fx::Base::SynchronizationOption::Singlethreaded;
							QS::Fx::Object::Runtime::Concurrency = 1;
						}
						break;

						case PARAMETER_BYPASS:
						{
							QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::Bypass;
						}
						break;

						case PARAMETER_TRANSFER:
						{
							QS::Fx::Object::Runtime::Replication = QS::Fx::Object::Runtime::Replication | QS::Fx::Replication::Policy::Transfer;
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::TransferTimeout = Convert::ToDouble(args[i]->Substring(_delimiter + 1));
							}
							else
							{
								QS::Fx::Object::Runtime::TransferTimeout = 1;
							}
						}
						break;

						case PARAMETER_CONFIGURE:
						{
							_addtocommandline = false;
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								cli::array<String^>^ _assignments =  args[i]->Substring(_delimiter + 1)->Split(',');
								for each (String^ _assignment in _assignments)
								{
									cli::array<String^>^ _parts =_assignment->Split('=');
									if (_parts->Length == 2)
										_configurationoptions->Add(_parts[0], _parts[1]);
									else
										throw gcnew Exception("Unrecognized configuration option \"" + _assignment + "\".");
								}
							}
							else
								throw gcnew Exception("Missing configuration options following \"-c\".");
						}
						break;

						case PARAMETER_LOGFILE:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime::LogFile = args[i]->Substring(_delimiter + 1);
							}
						}
						break;

						case PARAMETER_QSMTTL:
						{
							int _delimiter = args[i]->IndexOf(L":");
							if (_delimiter > 0 && _delimiter < args[i]->Length)
							{
								QS::Fx::Object::Runtime4::QsmTTL = Convert::ToInt32(args[i]->Substring(_delimiter + 1));
							}
						}
						break;

						default:
						{
							_addtocommandline = false;
							if (_name == nullptr)
								_name = args[i];
							else
								throw gcnew Exception("Cannot run more than one object at a time.");
						}
						break;
					}
					if (_addtocommandline)
					{
						if (!_hasspace)
							_hasspace = true;
						else
							_commandline->Append(" ");
						_commandline->Append(args[i]);
					}
				}
				QS::Fx::Object::Runtime::CommandLine = _commandline->ToString();

				QS::Fx::Object::RuntimeContext^ _context = gcnew QS::Fx::Object::RuntimeContext(
					mytemp, QS::Fx::Object::Runtime::Concurrency, QS::Fx::Object::Runtime::Policy, pinthreads);

				QS::Fx::Object::Runtime::Run(_name, true, _context, _configurationoptions);

#if defined(DEBUGGING_DISPLAY_INSPECTION_USER_INTERFACE)
				// inspectorthread->Abort();
				// inspectorthread->Join();
#endif
#endif

				dynamic_cast<QS::Fx::Object::IRuntimeContext^>(_context)->Release();
			}
		}
		else
		{
#if defined(NOT_REFERENCING_INTERNAL_LIBRARIES)
			Type^ runtype = mainassembly->GetType(L"QS.Fx.Service.Service");
			ConstructorInfo^ runmethod = runtype->GetConstructor(gcnew array<Type^>(0));
			Object^ runobject = runmethod->Invoke(gcnew array<Object^>(0));
			ServiceBase::Run(dynamic_cast<System::ServiceProcess::ServiceBase^>(runobject));
#else
#if defined(DEBUGGING_SERVICE)
			if (!System::Diagnostics::Debugger::IsAttached)
				System::Diagnostics::Debugger::Launch();
			System::Diagnostics::Debugger::Break();
#endif



			ServiceBase::Run(
				gcnew QS::Fx::Service::Service(
					QS::Fx::Object::RuntimeContext::Launcher(mytemp, QS::Fx::Object::Runtime::Concurrency, QS::Fx::Object::Runtime::Policy, false)));
#endif
		}
	}
	catch (Exception ^exc)
	{
		StringBuilder^ ss = gcnew StringBuilder();
		bool isfirst = true;
		while (exc != nullptr)
		{
			if (isfirst)
				isfirst = false;
			else
				ss->AppendLine(gcnew String('-', 80));
			ss->AppendLine(exc->ToString());
			ss->AppendLine(exc->StackTrace);
			exc = exc->InnerException;
		}
		System::Windows::Forms::Form^ form = gcnew System::Windows::Forms::Form();
		System::Windows::Forms::RichTextBox^ box = gcnew System::Windows::Forms::RichTextBox();
		box->Text = ss->ToString();
		box->Dock = System::Windows::Forms::DockStyle::Fill;
		form->Controls->Add(box);
		form->Text = "Unhandled Exception";
		form->WindowState = System::Windows::Forms::FormWindowState::Maximized;
		System::Windows::Forms::Application::Run(form);
		//System::Windows::Forms::MessageBox::Show(ss->ToString(), "Exception", 
		//	System::Windows::Forms::MessageBoxButtons::OK, System::Windows::Forms::MessageBoxIcon::Error,
		//	System::Windows::Forms::MessageBoxDefaultButton::Button1);
	}

	return 0;
}
