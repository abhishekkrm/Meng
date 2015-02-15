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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Window, "Window", "A window containing a UI object.")]
    public partial class Window : Form, QS.Fx.Object.Classes.IWindow, QS.Fx.Inspection.IInspectable, QS._qss_x_.Platform_.IApplication, QS.Fx.Interface.Classes.ILog
    {
        #region Constructors

        public Window(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("caption", QS.Fx.Reflection.ParameterClass.Value)] string _caption,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader,
            [QS.Fx.Reflection.Parameter("ui", QS.Fx.Reflection.ParameterClass.Value)] QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _ui)
        {
            this._mycontext = _mycontext;

            InitializeComponent();

            this.Text = _caption;
#if !LINUX_MENUITEM
            this._system_menu_handle = QS._platform_specific_.GetSystemMenu(this.Handle.ToInt32(), 0);
            if (this._system_menu_handle != 0)
            {
                QS._platform_specific_.AppendMenu(_system_menu_handle, _MENU_SEPARATOR, 0, null);
                QS._platform_specific_.AppendMenu(_system_menu_handle, 0, _DISCONNECT_OBJECT, "Disconnect Object");
                QS._platform_specific_.EnableMenuItem(_system_menu_handle, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_GRAYED);
                QS._platform_specific_.EnableMenuItem(_system_menu_handle, _DESIGN_OBJECT, _MF_BYCOMMAND | _MF_GRAYED);
            }
#endif

            this._uiendpoint = _mycontext.ImportedUI(this);
            this._uiendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this._uiendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
            this._loaderendpoint.OnConnect += new QS.Fx.Base.Callback(this._LoaderConnectCallback);
            this._loaderendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._LoaderDisconnectCallback);

            if (_ui != null)
            {
                lock (this)
                {
                    this._uiobject = _ui.Dereference(_mycontext);
                    this._uiconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._uiendpoint).Connect(_uiobject.UI);
                }
            }

            if (_loader != null)
            {
                lock (this)
                {
                    this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
                }
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("context")]
        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable("uiobject")]
        private QS.Fx.Object.Classes.IUI _uiobject;

        [QS.Fx.Base.Inspectable("uiendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedUI _uiendpoint;

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;

        [QS.Fx.Base.Inspectable("uiconnection")]
        private QS.Fx.Endpoint.IConnection _uiconnection;

        [QS.Fx.Base.Inspectable("loaderconnection")]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        private int _system_menu_handle;
        private QS.Fx.Inspection.IAttributeCollection _attributecollection;
        private QS.Fx.Platform.IPlatform platform;

        private List<IDisposable> _todispose = new List<IDisposable>();

        #endregion

        #region Constants

        private const int _MENU_SEPARATOR = 0xA00;
        private const int _WM_SYSCOMMAND = 0x112;
        private const int _DISCONNECT_OBJECT = 1000;
        private const int _DESIGN_OBJECT = 1001;
        private const int _MF_BYCOMMAND = 0x0;
        private const int _MF_ENABLED = 0x0;
        private const int _MF_GRAYED = 0x1;

        #endregion

        #region Overridden WndProc

        protected override void WndProc(ref Message m)
        {
 	         base.WndProc(ref m);
             if (m.Msg == _WM_SYSCOMMAND)
             {
                 if (m.WParam.ToInt32() == _DISCONNECT_OBJECT)
                 {
                     lock (this)
                     {
                         if (this._uiconnection != null)
                         {
                             if (this._uiconnection is IDisposable)
                                 this._uiconnection.Dispose();
                             this._uiconnection = null;
                         }

                         if (this._uiobject != null)
                         {
                             if (this._uiobject is IDisposable)
                                 ((IDisposable)this._uiobject).Dispose();
                         }
                     }
                 }
                 else if (m.WParam.ToInt32() == _DESIGN_OBJECT)
                 {
                     lock (this)
                     {
                     }
                 }
             }
        }

        #endregion

        #region _UIConnectCallback

        private void _UIConnectCallback()
        {
            _uiendpoint.UI.Dock = DockStyle.Fill;
#if !LINUX_MENUITEM
            QS._platform_specific_.EnableMenuItem(_system_menu_handle, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_ENABLED);
#endif
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
#if !LINUX_MENUITEM
            QS._platform_specific_.EnableMenuItem(_system_menu_handle, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_GRAYED);
#endif
        }

        #endregion

        #region _LoaderConnectCallback

        private void _LoaderConnectCallback()
        {
        }

        #endregion

        #region _LoaderDisconnectCallback

        private void _LoaderDisconnectCallback()
        {
        }

        #endregion

        #region IWindow Members

//        QS.Fx.Endpoint.Classes.IClientOf<QS.Fx.Endpoint.Classes.IExportedUI> QS.Fx.Object.Classes.IWindow.UI
//        {
//            get { return _uiendpoint; }
//        }

        #endregion

        #region IInspectable Members

        QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
        {
            get
            {
                lock (this)
                {
                    if (_attributecollection == null)
                        _attributecollection = new QS.Fx.Inspection.AttributesOf(this);
                    return _attributecollection;
                }
            }
        }

        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(DataFormats.Text, false) || e.Data.GetDataPresent(DataFormats.UnicodeText, false))
            {
                e.Effect = DragDropEffects.All;
            }            
        }

        #endregion

        #region _DragDrop

        private void _DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    string[] _filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (_filenames.Length == 1)
                    {
                        string _text;
                        using (StreamReader _streamreader = new StreamReader(_filenames[0]))
                        {
                            _text = _streamreader.ReadToEnd();
                        }
                        _Add(_text);
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.Text);
                    _Add(_text);
                }
                else if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.UnicodeText);
                    _Add(_text);
                }
                else
                    throw new Exception("The drag and drop operation cannot continue because none of the data formats was recognized.");
            }
            catch (Exception _exc)
            {
                (new QS._qss_x_.Base1_.ExceptionForm(_exc)).ShowDialog();
            }
        }

        #endregion

        #region _DragLeave

        private void _DragLeave(object sender, EventArgs e)
        {
        }

        #endregion

        #region _Add

        private void _Add(string _xmldescription)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _reference = _loaderendpoint.Interface.Load(_xmldescription);                    
                    QS.Fx.Object.Classes.IObject _object = _reference.Dereference(_mycontext);
                    if (_object != null)
                    {
                        QS.Fx.Object.Classes.IUI _ui = _mycontext.SafeCast<QS.Fx.Object.Classes.IUI>(_object);
                        if (_ui != null)
                        {
                            lock (this)
                            {
                                this._uiobject = _ui;
                                if ((this._uiobject != null) && (this._uiobject is QS._qss_x_.Platform_.IApplication) && (this.platform != null))
                                    ((QS._qss_x_.Platform_.IApplication)this._uiobject).Start(this.platform, null);
                                this._uiconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._uiendpoint).Connect(_ui.UI);
                            }

                            QS.Fx.Object.Classes.ILogging _logging = _mycontext.SafeCast<QS.Fx.Object.Classes.ILogging>(_object);
                            if (_logging != null)                                
                                _todispose.Add(_mycontext.ExportedInterface<QS.Fx.Interface.Classes.ILog>(this).Connect(_logging.Logging));
                        }
                    }
                }
            }
        }

        #endregion

        #region IApplication Members

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            lock (this)
            {
                this.platform = platform;
                if ((this._uiobject != null) && (this._uiobject is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._uiobject).Start(platform, null);
            }
        }

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
            lock (this)
            {
                if ((this._uiobject != null) && (this._uiobject is QS._qss_x_.Platform_.IApplication))
                    ((QS._qss_x_.Platform_.IApplication)this._uiobject).Stop();
                this.platform = null;
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (this._uiconnection != null) 
                {
                    if (this._uiconnection is IDisposable)
                        this._uiconnection.Dispose();
                    this._uiconnection = null;
                }

                if (this._uiobject != null)
                {
                    if (this._uiobject is IDisposable)
                        ((IDisposable)this._uiobject).Dispose();
                }
            }
        }

        #endregion

        #region ILog Members

        void QS.Fx.Interface.Classes.ILog.Log(string message)
        {
            //MessageBox.Show(message, "Log", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        #endregion
    }
}
