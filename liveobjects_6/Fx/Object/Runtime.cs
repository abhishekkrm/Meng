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

#define DEBUG_UI

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace QS.Fx.Object
{
    public static class Runtime
    {
        public static string ROOT
        {
            get { return QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_; }
        }

        public static void Shutdown()
        {
            QS._qss_x_.Registry_._Registry._Shutdown();
        }

        #region Constants

        public static bool Silent = false;
        public static bool Debug = false;
        public static bool Inspection = true;
        public static QS.Fx.Base.SynchronizationOption SynchronizationOption =
            QS.Fx.Base.SynchronizationOption.Asynchronous | QS.Fx.Base.SynchronizationOption.Singlethreaded;
        public static int Concurrency = 0;
        public static QS.Fx.Scheduling.Policy Policy = QS.Fx.Scheduling.Policy.BalancedRoundRobin;
        public static int MaximumNumberOfEventsToHandleSequentially = 10;
        public static double MaximumAmountOfTimeToHandleEventsSequentially = 0.01;
        public static QS.Fx.Replication.Policy Replication = QS.Fx.Replication.Policy.ToProcess | QS.Fx.Replication.Policy.Aggressive;
        public static string ExternalConfiguration = null;
        public static int NumberOfWorkers = 0;
        public static int NumberOfReplicas = 0;
        public static string Owner = null;
        public static int SerializationType = 0;
        public static bool FlowControlEnabled = false;
        public static double FlowControlInterval = 0;
        public static double TransferTimeout = 1;
        public static string CommandLine;
        public static string LogFile = null;

        #endregion

        private static readonly string _ERROR_FILE =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ +
            Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "errors.txt";

        public static void Run
        (
            QS.Fx.Clock.IClock _clock,
            int _parameter_nnodes,
            /*
                        int _parameter_nclients,
                        int _parameter_nservers,
            */
            string _parameter_subnet,
            double _parameter_mttb,
            double _parameter_mttf,
            double _parameter_mttr,
            double _parameter_cpu,
            double _parameter_speed,
            double _parameter_bandwidth,
            double _parameter_recovery,
            double _parameter_loss,
            string _parameter_scenario,
            /*
                        string _parameter_client, 
                        string _parameter_service,
                        string _parameter_bootstrap,
            */
            IDictionary<string, string> _application_parameters
        )
        {
            QS._qss_x_.Simulations_.Scenario_ _scenario_def;
            using (StringReader _reader = new StringReader(_parameter_scenario))
            {
                _scenario_def = (QS._qss_x_.Simulations_.Scenario_)(new XmlSerializer(typeof(QS._qss_x_.Simulations_.Scenario_))).Deserialize(_reader);
            }
            QS._qss_x_.Simulations_.IScenario_ _scenario;
            string _scenario_identifier = _scenario_def._Identifier;
            if (_scenario_identifier.Equals("synchronization"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Synchronization_();
            else if (_scenario_identifier.Equals("multicast"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Multicast_();
            else if (_scenario_identifier.Equals("foo"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Foo_();
            else if (_scenario_identifier.Equals("ring"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Ring_();
            else if (_scenario_identifier.Equals("tree"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Tree_();
            else if (_scenario_identifier.Equals("bootstrap"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Bootstrap_();
            else if (_scenario_identifier.Equals("aggregation"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Aggregation_();
            else if (_scenario_identifier.Equals("dissemination"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Dissemination_();
            else if (_scenario_identifier.Equals("replication"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Replication_();
            else if (_scenario_identifier.Equals("storage"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_Storage_();
            else if (_scenario_identifier.Equals("dataflow"))
                _scenario = new QS._qss_x_.Scenarios_.Scenario_DataFlow_();
            else
                throw new Exception("Unknown scenario \"" + _scenario_identifier + "\".");
            IDictionary<string, QS.Fx.Reflection.Xml.Parameter> _ppppp = new Dictionary<string, QS.Fx.Reflection.Xml.Parameter>();
            foreach (QS.Fx.Reflection.Xml.Parameter _pppp in _scenario_def._Parameters)
            {
                _ppppp.Add(_pppp.ID, _pppp);
            }
            QS._qss_x_.Simulations_.ITask_[] _parameter_tasks = _scenario._Create(_parameter_nnodes, _ppppp);
            QS._qss_x_.Simulations_.Simulation _simulation =
                new QS._qss_x_.Simulations_.Simulation(
                    _clock,
                    new QS._qss_c_.Base1_.Subnet(_parameter_subnet),
                    _parameter_nnodes,
                    _parameter_tasks,
                /*
                                    _parameter_nclients,
                                    _parameter_nservers,
                */
                    _parameter_mttb,
                    _parameter_mttf,
                    _parameter_mttr,
                    _parameter_bandwidth,
                    _parameter_recovery,
                    _parameter_loss,
                    typeof(QS._qss_x_.Simulations_.Application_),
                    _application_parameters,
                    _parameter_cpu,
                    _parameter_speed);

            System.Windows.Forms.Application.Run(new QS.GUI.Fx.Simulations.SimulationForm(_simulation));

            ((QS._qss_x_.Simulations_.ISimulation)_simulation).Shutdown();
        }

        public static void Run(string _objectfile, bool _interactive)
        {
            Run(_objectfile, _interactive, null, null);
        }

        public static void Run(string _objectfile, bool _interactive, IRuntimeContext _context, IDictionary<string, string> _configurationoptions)
        {
            try
            {
                string _objectxml = null;
                if (QS.Fx.Object.Runtime.Owner == null)
                    using (StreamReader _streamreader = new StreamReader(_objectfile))
                        _objectxml = _streamreader.ReadToEnd();
                QS.Fx.Object.Runtime.Run(_objectxml, _context, _configurationoptions);
            }
            catch (Exception _exc)
            {
                if (_interactive)
                {
                    // System.Windows.Forms.MessageBox.Show(_message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    try
                    {
                        QS._qss_x_.Gui_.ExceptionForm_ _exceptionform = new QS._qss_x_.Gui_.ExceptionForm_(_exc);
                        _exceptionform.ShowDialog();
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    string _message = "Exception caught while running \"" + _objectfile + "\".\n\n" + _exc.ToString();
                    using (StreamWriter _writer = new StreamWriter(_ERROR_FILE))
                    {
                        _writer.WriteLine("[" + DateTime.Now.ToString() + "]\n" + _message + "\n");
                    }
                }
            }
        }

        public static QS.Fx.Object.IEmbeddedObject Embed(string _objectxml)
        {
            QS.Fx.Object.IContext _mycontext = null;

            throw new QS.Fx.Base.TODO();

            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
            try
            {
                using (QS._qss_x_.Component_.Classes_.Loader _loader = new QS._qss_x_.Component_.Classes_.Loader(
                    _mycontext,
                    QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                        QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                {
                    QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject> _interface;
                    using (QS._qss_x_.Component_.Classes_.Service<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>.Connect(
                        _mycontext, _loader, out _interface))
                    {
                        _objectref = _interface.Load(_objectxml);
                    }
                }
            }
            catch (Exception _exc)
            {
                throw new Exception("Could not load the object description from the XML definition, the object description may be malformed.", _exc);
            }
            if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window)))
            {
                QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                if (_object is System.Windows.Forms.Form)
                {
                    return new QS._qss_x_.Object_.EmbeddedObject(_objectxml, _objectref, _object,
                        new IDisposable[] { ((IDisposable)_object) },
                        ((System.Windows.Forms.Control)_object));
                }
                else
                    throw new Exception("Currently only Window objects derived from \"" +
                        typeof(System.Windows.Forms.Form).Name + "\" are supported by the runtime.");
            }
            else if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window_X)))
            {
#if XNA
                QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                if (_object is Microsoft.Xna.Framework.Game)
                {
                    return new QS._qss_x_.Object_.EmbeddedObject(_objectxml, _objectref, _object,
                        new IDisposable[] { ((IDisposable)_object) },
                        (System.Windows.Forms.Control)
                            System.Windows.Forms.Form.FromHandle(((Microsoft.Xna.Framework.Game)_object).Window.Handle));
                }
                else
                    throw new Exception("Currently only Windows XNA objects derived from \"" +
                        typeof(Microsoft.Xna.Framework.Game).Name + "\" are supported by the runtime.");
#else
                throw new NotSupportedException();
#endif
            }
            else if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.UI)))
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _ui = _objectref.CastTo<QS.Fx.Object.Classes.IUI>();
                QS.Fx.Object.Classes.IUI _object = _ui.Dereference(_mycontext);
                System.Windows.Forms.Control _containercontrol = new System.Windows.Forms.Control();
                QS.Fx.Endpoint.Internal.IImportedUI _endpoint = _mycontext.ImportedUI(_containercontrol);
                QS.Fx.Endpoint.IConnection _connection = ((QS.Fx.Endpoint.Classes.IEndpoint)_endpoint).Connect(_object.UI);
                return new QS._qss_x_.Object_.EmbeddedObject(_objectxml, _objectref, _object, new IDisposable[] { _connection }, _containercontrol);
            }
            else if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.UI_X)))
            {
                throw new Exception("The XNA-based UI objects are currently not supported for embedding at this time; try XNA-based Window objects.");
                /*
                                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> _ui = _objectref.CastTo<QS.Fx.Object.Classes.IUI_X>();

                                using (QS.Fx.Component.Classes.Loader _loader = new QS.Fx.Component.Classes.Loader(
                                    QS.Fx.Object.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                                        QS.Fx.Reflection.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                                {
                                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> __loader =
                                        QS.Fx.Object.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>>.Create(_loader, null, null);

                                    QS.Fx.Component.Classes.Window_X _window =
                                        new QS.Fx.Component.Classes.Window_X(
                                            QS.Fx.Attributes.Attribute.ValueOf(_ui.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, "QuickSilver")
                                            , __loader,
                                            @"C:\Users\krzys\Work\QuickSilver\@Content\root.xml",
                                            _ui);

                                    ((Microsoft.Xna.Framework.Game)_window).Run();
                                }
                */
            }
            else
            {
                string _objectname;
                QS.Fx.Attributes.IAttribute _nameattribute;
                if (_objectref.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                    _objectname = "Object \"" + _nameattribute.Value + "\"";
                else
                    _objectname = "Unnamed Object";
                string _objectclassname;
                QS.Fx.Attributes.IAttribute _classnameattribute;
                if (_objectref.ObjectClass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _classnameattribute))
                    _objectclassname = "class \"" + _classnameattribute.Value + "\"";
                else
                    _objectclassname = "unnamed class";
                QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                System.Windows.Forms.Control _containercontrol = new System.Windows.Forms.Control();
                System.Windows.Forms.Label _label = new System.Windows.Forms.Label();
                _label.Text = _objectname;
                _label.AutoSize = false;
                _label.Dock = System.Windows.Forms.DockStyle.Fill;
                _containercontrol.Controls.Add(_label);
                /*
                                string _tooltiptext = "Running " + _objectname + " of " + _objectclassname + ".";
                */
                return new QS._qss_x_.Object_.EmbeddedObject(_objectxml, _objectref, _object, new IDisposable[0], _containercontrol);
            }
        }

        public static void Run(string _objectxml, IRuntimeContext _context, IDictionary<string, string> _configurationoptions)
        {
            if (_configurationoptions != null)
            {
                foreach (KeyValuePair<string, string> _configurationoption in _configurationoptions)
                {
                    string _from = "@{" + _configurationoption.Key + "}";
                    string _to = _configurationoption.Value;
                    if (_objectxml.Contains(_from))
                        _objectxml = _objectxml.Replace(_from, _to);
                }
            }

            QS.Fx.Object.IContext _mycontext =
                new QS._qss_x_.Object_.Context_(
                    _context.Platform,
                    QS._qss_x_.Object_.Context_.ErrorHandling_.Halt,
                    QS.Fx.Object.Runtime.SynchronizationOption,
                    QS.Fx.Object.Runtime.SynchronizationOption);

            QS._qss_x_.Runtime_.RemoteContext_ _remotecontext =
                new QS._qss_x_.Runtime_.RemoteContext_(_context, QS.Fx.Object.Runtime.NumberOfWorkers, QS.Fx.Object.Runtime.ExternalConfiguration);

            ((QS._qss_x_.Object_.IInternal_)_mycontext)._RemoteContext = _remotecontext;

            System.Windows.Forms.Application.EnableVisualStyles();

            if (QS.Fx.Object.Runtime.Owner == null)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = null;

                using (QS._qss_x_.Component_.Classes_.Loader _loader = new QS._qss_x_.Component_.Classes_.Loader(_mycontext,
                    QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                        QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                {
                    QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject> _interface;
                    using (QS._qss_x_.Component_.Classes_.Service<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>.Connect(
                        _mycontext, _loader, out _interface))
                    {
                        _objectref = _interface.Load(_objectxml);
                    }
                }

                if (_objectref != null)
                {
                    if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window)))
                    {
                        QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                        bool _contextual = (_context != null) && (_object is QS._qss_x_.Platform_.IApplication);
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Start(_context.Platform, null);
                        if (_object is System.Windows.Forms.Form)
                        {

                            System.Windows.Forms.Application.Run((System.Windows.Forms.Form)_object);
                        }
                        else
                            throw new Exception("Currently only Window objects derived from \"" +
                                typeof(System.Windows.Forms.Form).Name + "\" are supported by the runtime.");
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Stop();

                        if (_object is IDisposable)
                            ((IDisposable)_object).Dispose();
                    }
                    else if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window_X)))
                    {
#if XNA
                        QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                        bool _contextual = (_context != null) && (_object is QS._qss_x_.Platform_.IApplication);
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Start(_context.Platform, null);
                        if (_object is Microsoft.Xna.Framework.Game)
                        {
                            ((Microsoft.Xna.Framework.Game)_object).Run();
                        }
                        else
                            throw new Exception("Currently only Windows XNA objects derived from \"" +
                                typeof(Microsoft.Xna.Framework.Game).Name + "\" are supported by the runtime.");
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Stop();
#else
                        throw new NotSupportedException();
#endif
                    }
                    else if (_objectref.ObjectClass.IsSubtypeOf(
                        QS._qss_x_.Reflection_.Library.LocalLibrary.GetObjectClass(QS.Fx.Reflection.ObjectClasses.UI)))
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _ui = _objectref.CastTo<QS.Fx.Object.Classes.IUI>();

#if DEBUG_UI

                        QS.Fx.Object.Classes.IUI _object = _ui.Dereference(_mycontext);

                        //_context.Platform.Logger.Log("Processor Count ( " + Environment.ProcessorCount.ToString() + " )");
                        //_context.Platform.Logger.Log("Processor Affinity ( 0x" +
                        //    System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity.ToInt32().ToString("x") + " )");

                        bool _contextual = (_context != null) && (_object is QS._qss_x_.Platform_.IApplication);
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Start(_context.Platform, null);

                        DebuggerForm form = (new DebuggerForm(
                                QS.Fx.Attributes.Attribute.ValueOf(_ui.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, "(unnamed)"),
                                null,
                                _object,
                                _context,
                                QS.Fx.Object.Runtime.Inspection));

                        if (!Silent)
                        {
                            form.ShowDialog();
                        }
                        else
                        {
                            while (true)
                            {
                                Thread.Sleep(30 * 1000);
                            }
                        }
                        
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Stop();

                        if (_object is IDisposable)
                            ((IDisposable)_object).Dispose();

#else

                    using (QS._qss_x_.Component_.Classes_.Loader _loader = new QS._qss_x_.Component_.Classes_.Loader(
                        _mycontext,
                        QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                            QS._qss_x_.Reflection_.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> __loader =
                            QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>>.Create(
                                _loader, null,
                                QS._qss_x_.Reflection_.Library.ObjectClassOf(
                                    typeof(QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>)));

                        QS._qss_x_.Component_.Classes_.Window _window =
                            new QS._qss_x_.Component_.Classes_.Window(
                                _mycontext,
                                QS.Fx.Attributes.Attribute.ValueOf(_ui.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, "QuickSilver")
                                , __loader, _ui);

                        bool _contextual = (_context != null) && (_window is QS._qss_x_.Platform_.IApplication);
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_window).Start(_context.Platform, null);

                        if (QS.Fx.Object.Runtime.Debug)
                        {
                            string _objectname;
                            QS.Fx.Attributes.IAttribute _nameattribute;
                            if (_objectref.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                                _objectname = "Object \"" + _nameattribute.Value + "\"";
                            else
                                _objectname = "Unnamed Object";

                            string _objectclassname;
                            QS.Fx.Attributes.IAttribute _classnameattribute;
                            if (_objectref.ObjectClass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _classnameattribute))
                                _objectclassname = "class \"" + _classnameattribute.Value + "\"";
                            else
                                _objectclassname = "unnamed class";

                            (new DebuggerForm(_objectname, _objectclassname, _window, _context, QS.Fx.Object.Runtime.Inspection)).Show();
                        }

                        System.Windows.Forms.Application.Run((System.Windows.Forms.Form)_window);

                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_window).Stop();

                        if (_window is IDisposable)
                            ((IDisposable)_window).Dispose();
                    }
#endif
                    }
                    else if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.UI_X)))
                    {
                        throw new NotImplementedException();
                        /*
                                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> _ui = _objectref.CastTo<QS.Fx.Object.Classes.IUI_X>();

                                        using (QS.Fx.Component.Classes.Loader _loader = new QS.Fx.Component.Classes.Loader(
                                            QS.Fx.Object.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILibrary>>.Create(
                                                QS.Fx.Reflection.Library.LocalLibrary.GetComponentClass(QS.Fx.Reflection.ComponentClasses.Library))))
                                        {
                                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> __loader =
                                                QS.Fx.Object.Reference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>>.Create(_loader, null, null);

                                            QS.Fx.Component.Classes.Window_X _window =
                                                new QS.Fx.Component.Classes.Window_X(
                                                    QS.Fx.Attributes.Attribute.ValueOf(_ui.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, "QuickSilver")
                                                    , __loader,
                                                    // @"C:\Users\krzys\Work\QuickSilver\@Content\root.xml",
                                                    _ui,
                                                    null,
                                                    1024,
                                                    768);

                                            ((Microsoft.Xna.Framework.Game) _window).Run();
                                        }
                        */
                    }
                    else
                    {
                        string _objectname;
                        QS.Fx.Attributes.IAttribute _nameattribute;
                        if (_objectref.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                            _objectname = "Object \"" + _nameattribute.Value + "\"";
                        else
                            _objectname = "Unnamed Object";

                        string _objectclassname;
                        QS.Fx.Attributes.IAttribute _classnameattribute;
                        if (_objectref.ObjectClass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _classnameattribute))
                            _objectclassname = "class \"" + _classnameattribute.Value + "\"";
                        else
                            _objectclassname = "unnamed class";

                        QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);

                        _context.Platform.Logger.Log("Processor Count ( " + Environment.ProcessorCount.ToString() + " )");
                        _context.Platform.Logger.Log("Processor Affinity ( 0x" +
                            System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity.ToInt32().ToString("x") + " )");

                        bool _contextual = (_context != null) && (_object is QS._qss_x_.Platform_.IApplication);
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Start(_context.Platform, null);


#if RUN_HACK_SLEEP
                        Thread.Sleep(10000000);
#else
                        DebuggerForm form = (new DebuggerForm(_objectname, _objectclassname, _object, _context, QS.Fx.Object.Runtime.Inspection));
                        if (!Silent)
                        {
                            form.ShowDialog();
                        }
                        else
                        {
                            while (true)
                            {
                                Thread.Sleep(30 * 1000);
                            }
                        }
#endif
                        if (_contextual)
                            ((QS._qss_x_.Platform_.IApplication)_object).Stop();

                        if (_object is IDisposable)
                            ((IDisposable)_object).Dispose();
                    }
                }
            }
            else
            {
                QS._qss_x_.Runtime_.Host_ _host = new QS._qss_x_.Runtime_.Host_(_mycontext, QS.Fx.Object.Runtime.Owner);

                _context.Platform.Logger.Log("Processor Count ( " + Environment.ProcessorCount.ToString() + " )");
                _context.Platform.Logger.Log("Processor Affinity ( 0x" +
                    System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity.ToInt32().ToString("x") + " )");

                (new DebuggerForm("Host", "Host", _host, _context, QS.Fx.Object.Runtime.Inspection)).ShowDialog();

                ((IDisposable)_host).Dispose();
            }

            if (_remotecontext != null)
                ((IDisposable)_remotecontext).Dispose();
        }
    }
}
