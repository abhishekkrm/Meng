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

// #define DEBUGGING_SERVICE

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Threading;

namespace QS.Fx.Service
{
    public partial class Service : ServiceBase
    {
        public Service(QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IRuntimeContext> _launcher)
        {
            this._launcher = _launcher;
            InitializeComponent();
        }

        private QS._qss_c_.Base3_.Constructor<QS.Fx.Object.IRuntimeContext> _launcher;

        private static readonly string _errors =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + 
                Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "errors.txt";

        private static readonly string _messages =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ +
                Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "messages.txt";

        private static readonly string _services_root =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + "services";

        private Thread _thread;
        private bool _exiting;
        private ManualResetEvent _exitingevent = new ManualResetEvent(false);
        private List<QS.Fx.Object.Classes.IObject> _serviceobjects = new List<QS.Fx.Object.Classes.IObject>();

        protected override void OnStart(string[] args)
        {
            using (StreamWriter _writer = new StreamWriter(_messages))
            {
                _writer.WriteLine("[" + DateTime.Now.ToString() + "]\nStart");
            }
            _exitingevent.Reset();
            _exiting = false;
            _thread = new Thread(new ThreadStart(this._ThreadCallback));
            _thread.Start();
        }

        protected override void OnStop()
        {
            _exiting = true;
            _exitingevent.Set();
            if (!_thread.Join(1000))
                _thread.Abort();
            QS.Fx.Object.Runtime.Shutdown();
            using (StreamWriter _writer = new StreamWriter(_messages, true))
            {
                _writer.WriteLine("[" + DateTime.Now.ToString() + "]\nStop");
            }
        }

        private void _ThreadCallback()
        {
#if DEBUGGING_SERVICE
            while (!System.Diagnostics.Debugger.IsAttached)
                Thread.Sleep(1000);
            System.Diagnostics.Debugger.Break();
#endif
            try
            {
                this._serviceobjects.Clear();

                QS.Fx.Object.IRuntimeContext _runtimecontext = _launcher();

                _runtimecontext.Platform.Logger.Log("Processor Count ( " + Environment.ProcessorCount.ToString() + " )");
                _runtimecontext.Platform.Logger.Log("Processor Affinity ( 0x" +
                    System.Diagnostics.Process.GetCurrentProcess().ProcessorAffinity.ToInt32().ToString("x") + " )");

                foreach (string _filename in Directory.GetFiles(_services_root, "*.liveobject", SearchOption.TopDirectoryOnly))
                {
                    string _objectxml = null;
                    using (StreamReader _streamreader = new StreamReader(_filename))
                    {
                        _objectxml = _streamreader.ReadToEnd();
                    }
                    
                    QS.Fx.Object.IContext _mycontext = new QS._qss_x_.Object_.Context_(_runtimecontext.Platform,
                            QS._qss_x_.Object_.Context_.ErrorHandling_.Halt, QS.Fx.Object.Runtime.SynchronizationOption, QS.Fx.Object.Runtime.SynchronizationOption);
                    
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
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
                        string _objectname;
                        QS.Fx.Attributes.IAttribute _nameattribute;
                        if (_objectref.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute))
                            _objectname = "Object \"" + _nameattribute.Value + "\"";
                        else
                            _objectname = "Unnamed Object";

                        string _objectclassname;
                        QS.Fx.Attributes.IAttribute _classnameattribute;
                        if (_objectref.ObjectClass.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _classnameattribute))
                            _objectclassname = "Class \"" + _classnameattribute.Value + "\"";
                        else
                            _objectclassname = "Unnamed Class";

                        using (StreamWriter _writer = new StreamWriter(_messages))
                        {
                            _writer.WriteLine("[" + DateTime.Now.ToString() + "]\nLaunching " + _objectname + " : " + _objectclassname);
                        }

                        try
                        {
                            QS.Fx.Object.Classes.IObject _object = _objectref.Dereference(_mycontext);
                            
                            _serviceobjects.Add(_object);

                            if (_object is QS._qss_x_.Platform_.IApplication)
                                ((QS._qss_x_.Platform_.IApplication) _object).Start(_mycontext.Platform, null);
                        }
                        catch (Exception _exc)
                        {
                            using (StreamWriter _writer = new StreamWriter(_messages))
                            {
                                StringBuilder _ss = new StringBuilder();
                                _ss.AppendLine("[" + DateTime.Now.ToString() + "]\nCould not launch " + _objectname + " : " + _objectclassname + "\n");
                                Exception _excc = _exc;
                                while (_excc != null)
                                {
                                    _ss.AppendLine(new string('-', 80));
                                    _ss.AppendLine(_excc.ToString());
                                    _ss.AppendLine(_excc.StackTrace);
                                    _excc = _excc.InnerException;
                                }
                                _writer.WriteLine(_ss.ToString());
                            }
                        }

                        using (StreamWriter _writer = new StreamWriter(_messages))
                        {
                            _writer.WriteLine("[" + DateTime.Now.ToString() + "]\nSuccessfully launched " + _objectname + " : " + _objectclassname);
                        }
                    }
                }                
                while (!_exiting)
                    _exitingevent.WaitOne();

                foreach (QS.Fx.Object.Classes.IObject _object in this._serviceobjects)
                {
                    try
                    {
                        if (_object is QS._qss_x_.Platform_.IApplication)
                            ((QS._qss_x_.Platform_.IApplication) _object).Stop();
                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        if (_object is IDisposable)
                            ((IDisposable)_object).Dispose();
                    }
                    catch (Exception)
                    {
                    }
                }
                this._serviceobjects.Clear();

                _runtimecontext.Release();
            }
            catch (Exception _exc)
            {
                using (StreamWriter _writer = new StreamWriter(_errors, true))
                {
                    StringBuilder _ss = new StringBuilder();
                    _ss.AppendLine("[" + DateTime.Now.ToString() + "]\nException while running the services.\n");
                    while (_exc != null)
                    {
                        _ss.AppendLine(new string('-', 80));
                        _ss.AppendLine(_exc.ToString());
                        _ss.AppendLine(new string('.', 40));
                        _ss.AppendLine(_exc.StackTrace);
                        _exc = _exc.InnerException;
                    }
                    _writer.WriteLine(_ss.ToString());
                }
            }
        }
    }
}
