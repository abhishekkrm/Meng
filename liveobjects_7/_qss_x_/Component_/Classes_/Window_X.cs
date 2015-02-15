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
using System.Runtime.InteropServices;
using System.IO;

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Window_X, "Window (XNA)", "A window containing an XNA game object.")]
    public sealed class Window_X : 
        Microsoft.Xna.Framework.Game, 
        QS.Fx.Object.Classes.IWindow_X, 
        QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
        QS.Fx.Inspection.IInspectable,
        QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>
    {
        #region Constructors

        public Window_X(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("caption", QS.Fx.Reflection.ParameterClass.Value)] 
                string _caption,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IService<
                        QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> 
                _loader,
//            [QS.Fx.Reflection.Parameter("content", QS.Fx.Reflection.ParameterClass.Value)] 
//                string _content_root,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>
                _folder,
            [QS.Fx.Reflection.Parameter("camera", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>>
                _camera,
            [QS.Fx.Reflection.Parameter("W", QS.Fx.Reflection.ParameterClass.Value)] 
                int _W,
            [QS.Fx.Reflection.Parameter("H", QS.Fx.Reflection.ParameterClass.Value)] 
                int _H            
            )
            : base()
        {
            this._mycontext = _mycontext;
            lock (this)
            {
                this._folderendpoint = 
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>, 
                        QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>>(this);

                if (_folder != null)
                {
                    this._form.Text = "Connected to folder " + 
                        QS.Fx.Attributes.Attribute.ValueOf(_folder.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _folder.ID) + ".";

                    this._folder = _folder.Dereference(_mycontext);
                    this._folderconnection =
                        ((QS.Fx.Endpoint.Classes.IEndpoint)this._folderendpoint).Connect(this._folder.Endpoint);
                    this._folderinterface = this._folderendpoint.Interface;
                }

                this._windowhandle = this.Window.Handle;
                this._systemmenu = GetSystemMenu(this._windowhandle.ToInt32(), 0);
                this._form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this._windowhandle);

                AppendMenu(this._systemmenu, _MENU_SEPARATOR, 0, null);
                AppendMenu(this._systemmenu, 0, _DISCONNECT_OBJECT, "Disconnect Object");
                EnableMenuItem(this._systemmenu, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_GRAYED);

                this._form.AllowDrop = true;
                this._form.DragEnter += new System.Windows.Forms.DragEventHandler(this._DragEnter);
                this._form.DragDrop += new System.Windows.Forms.DragEventHandler(this._DragDrop);

                this._form.MouseDown += new System.Windows.Forms.MouseEventHandler(this._MouseDown);
                this._form.MouseMove += new System.Windows.Forms.MouseEventHandler(this._MouseMove);
                this._form.MouseUp += new System.Windows.Forms.MouseEventHandler(this._MouseUp);
                this._form.MouseLeave += new EventHandler(this._MouseLeave);
                this._form.MouseWheel += new System.Windows.Forms.MouseEventHandler(this._MouseWheel);

                this.Window.Title = _caption;

                this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
                this._loaderendpoint.OnConnect += new QS.Fx.Base.Callback(this._LoaderConnectCallback);
                this._loaderendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._LoaderDisconnectCallback);

                if (_loader == null)
                    throw new Exception("Cannot run without a loader.");

                this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);

                this._graphics = new GraphicsDeviceManager(this);

                this._graphics.PreferredBackBufferWidth = _W;
                this._graphics.PreferredBackBufferHeight = _H;
                this._graphics.ApplyChanges();

                this._contentcontroller = new QS._qss_x_.Content_.Xna_.Controller_(QS._qss_x_.Content_.Controller_._Local, this.Services);

                this._cameraendpoint = 
                    _mycontext.DualInterface<
                        QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
                        QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>>(this);

                if (_camera != null)
                    this._cameraconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._cameraendpoint).Connect(_camera.Dereference(_mycontext).Endpoint);

                this._RefreshObjects();
            }
        }

        #endregion

        #region OnExiting

        protected override void OnExiting(object sender, EventArgs args)
        {
            lock (this)
            {
                foreach (_Object _o in this._objects.Values)
                {
                    _o.Disconnect();
                    ((IDisposable)_o).Dispose();
                }
            }
            base.OnExiting(sender, args);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        private System.Windows.Forms.Form _form;
        private IntPtr _windowhandle;
        private int _systemmenu;

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        [QS.Fx.Base.Inspectable("loaderconnection")]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        private Microsoft.Xna.Framework.GraphicsDeviceManager _graphics;
        private bool _initialized;
        private QS._qss_x_.Content_.Xna_.IController_ _contentcontroller;

        private QS.Fx.Inspection.IAttributeCollection _attributecollection;

        private float _aspectratio;
        private Microsoft.Xna.Framework.Vector3 _cameraposition, _cameratarget, _selectedposition;
        private bool _moving, _rotating;
        private float _selected_angle_x, _selected_angle_y; 
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

        private IDictionary<string, _Object> _objects = new Dictionary<string, _Object>();

        [QS.Fx.Base.Inspectable("folder")]
        private QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folder;
        [QS.Fx.Base.Inspectable("folderendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>> _folderendpoint;
        [QS.Fx.Base.Inspectable("folderconnection")]
        private QS.Fx.Endpoint.IConnection _folderconnection;
        [QS.Fx.Base.Inspectable("folderinterface")]
        private QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folderinterface;
        private DateTime lastcheck = DateTime.Now;

        [QS.Fx.Base.Inspectable("cameraendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>> _cameraendpoint;
        [QS.Fx.Base.Inspectable("cameraconnection")]
        private QS.Fx.Endpoint.IConnection _cameraconnection;
        private float _downward_rotation;
        // private Microsoft.Xna.Framework.Vector3 _camerarotation;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

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

        #region IWindow_X Members

//        QS.Fx.Endpoint.Classes.IClientOf<QS.Fx.Endpoint.Classes.IExportedUI> QS.Fx.Object.Classes.IWindow_X.UI
//        {
//            get { return _uiendpoint; }
//        }

        #endregion

        #region _UIConnectCallback

        private void _UIConnectCallback()
        {
            // EnableMenuItem(_systemmenu, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_ENABLED);
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
            // EnableMenuItem(_systemmenu, _DISCONNECT_OBJECT, _MF_BYCOMMAND | _MF_GRAYED);
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

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Object

        private class _Object : IDisposable
        {
            public _Object(Window_X _owner, QS.Fx.Object.Classes.IUI_X _object)
            {
                this._owner = _owner;
                this._object = _object;
                this._endpoint = 
                    _owner._mycontext.ImportedUI_X
                    (
#if XNA
                        this._owner._graphics, 
#endif
                        new QS.Fx.Endpoint.Internal.Xna.ContentCallback(this._owner._ContentCallback)
                    );
                // _endpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
                // _endpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);
            }

            private bool _connected;
            private Window_X _owner;
            private QS.Fx.Object.Classes.IUI_X _object;
            private QS.Fx.Endpoint.Internal.IImportedUI_X _endpoint;
            private QS.Fx.Endpoint.IConnection _connection;

            public bool IsConnected
            {
                get { return _connected; }
            }

            public void Connect()
            {
                this._connection = ((QS.Fx.Endpoint.Classes.IEndpoint) this._endpoint).Connect(_object.UI);
                this._connected = true;
            }

            public void Disconnect()
            {
                if (this._connection != null)
                    this._connection.Dispose();
                this._connection = null;
                this._connected = false;
            }

            public QS.Fx.Endpoint.Internal.IImportedUI_X Endpoint
            {
                get { return _endpoint; }
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                if (_object is IDisposable)
                    ((IDisposable)_object).Dispose();
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constants

        private const int _MENU_SEPARATOR = 0xA00;
        private const int _WM_SYSCOMMAND = 0x112;
        private const int _DISCONNECT_OBJECT = 1000;
        private const int _MF_BYCOMMAND = 0x0;
        private const int _MF_ENABLED = 0x0;
        private const int _MF_GRAYED = 0x1;

        #endregion

        #region Imported from Win32

        [DllImport("user32.dll")]
        private static extern int GetSystemMenu(int hwnd, int bRevert);
        
        [DllImport("user32.dll")]
        private static extern int AppendMenu(int hMenu, int Flagsw, int IDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        private static extern int EnableMenuItem(int hMenu, int ideEnableItem, int enable);

        #endregion

        #region Overridden WndProc **************

/*
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
 	         base.WndProc(ref m);
             if (m.Msg == _WM_SYSCOMMAND)
             {
                 if (m.WParam.ToInt32() == _DISCONNECT_OBJECT)
                 {
                     lock (this)
                     {
                         if (_uiconnection != null)
                         {
                             _uiconnection.Dispose();
                         }
                     }
                 }
             }
        }
*/

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _DragEnter

        private void _DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text, false) ||
                e.Data.GetDataPresent(System.Windows.Forms.DataFormats.UnicodeText, false))
            {
                e.Effect = System.Windows.Forms.DragDropEffects.All;
            }            
        }

        #endregion

        #region _DragDrop

        private void _DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.FileDrop, false))
                {
                    string[] _filenames = (string[]) e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
                    foreach (string _filename in _filenames)
                    {
                        string _text;
                        using (StreamReader _streamreader = new StreamReader(_filename))
                        {
                            _text = _streamreader.ReadToEnd();
                        }
                        _Drop(_text);
                    }
                }
                else if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.Text, false))
                {
                    string _text = (string) e.Data.GetData(System.Windows.Forms.DataFormats.Text);
                    _Drop(_text);
                }
                else if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.UnicodeText, false))
                {
                    string _text = (string) e.Data.GetData(System.Windows.Forms.DataFormats.UnicodeText);
                    _Drop(_text);
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

        #region _Drop

        private void _Drop(string _objectxml)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(_objectxml);
                    string _key = QS.Fx.Attributes.Attribute.ValueOf(_objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);
                    try
                    {
                        if (!_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Dictionary)))
                            throw new Exception();
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder_ref =
                            _objectref.CastTo<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>>();
                        QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folder_object = _folder_ref.Dereference(_mycontext);
                        try
                        {
                            foreach (_Object _o in this._objects.Values)
                                _o.Disconnect();
                            this._objects.Clear();

                            this._folderinterface = null;
                            if (this._folderconnection != null)
                            {
                                this._folderconnection.Dispose();
                                this._folderconnection = null;
                            }
                            this._folder = _folder_object;
                            this._folderconnection =
                                ((QS.Fx.Endpoint.Classes.IEndpoint)this._folderendpoint).Connect(this._folder.Endpoint);
                            this._folderinterface = this._folderendpoint.Interface;

                            this._RefreshObjects();

                            this._form.Text = "Connected to folder " +
                                QS.Fx.Attributes.Attribute.ValueOf(_folder_ref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _folder_ref.ID) + ".";
                        }
                        catch (Exception _exc)
                        {
                            System.Windows.Forms.MessageBox.Show(
                                "Cannot connect  \"Dictionary<string,Object>\"-typed object \"" + _key + "\" as the underlying folder to this 3D view because an error has occurred.\n\n" + 
                                _exc.ToString(),
                                "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception _exc)
                    {                        
                        try
                        {
                            if (!_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Value)))
                                throw new Exception();
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinates_ref = 
                                _objectref.CastTo<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>>();
                            QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates> _coordinates_object = _coordinates_ref.Dereference(_mycontext);
                            try
                            {
                                if (this._cameraconnection != null)
                                {
                                    this._cameraconnection.Dispose();
                                    // System.Threading.ThreadPool.QueueUserWorkItem(
                                    //    new System.Threading.WaitCallback(this._AsynchronousDisconnectionCallback), this._cameraconnection);
                                }
                                this._cameraconnection =
                                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._cameraendpoint).Connect(_coordinates_object.Endpoint);
                            }
                            catch (Exception _exc2)
                            {
                                System.Windows.Forms.MessageBox.Show(
                                    "Cannot connect  \"Coordinates\"-typed object \"" + _key + "\" to the camera in this 3D view because an error has occurred.\n\n" + _exc2.ToString(),
                                    "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception _exc3)
                        {
                            if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.UI_X)))
                            {
                                try
                                {
                                    this._folderinterface.Add(_key, _objectref);
                                }
                                catch (Exception _exc4)
                                {
                                    System.Windows.Forms.MessageBox.Show(
                                        "Cannot add \"UI_X\"-typed object \"" + _key + "\" to this folder-based 3D view because an error has occurred.\n\n" + _exc4.ToString(),
                                        "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                System.Windows.Forms.MessageBox.Show("Cannot add object \"" + _key + 
                                    "\" to this 3D view because the object is neither of \"Dictionary<string,Object>\", nor of \"Coordinates\", nor of \"UI_X\" types.", 
                                    "Exception", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _AsynchronousDisconnectionCallback

        private void _AsynchronousDisconnectionCallback(object _context)
        {
            try
            {
                QS.Fx.Endpoint.IConnection _connection = (QS.Fx.Endpoint.IConnection) _context;
                _connection.Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _RefreshObjects

        private void _RefreshObjects()
        {
            lock (this)
            {
                if (this._folderinterface != null)
                {
                    foreach (string _objectid in this._folderinterface.Keys())
                    {
                        if (!this._objects.ContainsKey(_objectid))
                        {
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._folderinterface.GetObject(_objectid);
                            _AddObject(_objectid, _objectref);
                        }
                    }
                }
            }
        }

        #endregion

        #region _AddObject

        private void _AddObject(string _objectid, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
        {
            lock (this)
            {
                if (_objectref.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.UI_X)))
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> _ui = _objectref.CastTo<QS.Fx.Object.Classes.IUI_X>();
                    _Object _o = new _Object(this, _ui.Dereference(_mycontext));
                    this._objects.Add(_objectid, _o);
                    if (_initialized)
                        _o.Connect();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CalculateCoordinates_1

        private Vector3 _CalculateCoordinates_1(System.Windows.Forms.MouseEventArgs e)
        {
            Vector3 _v = Vector3.Subtract(this._cameratarget, this._cameraposition);
            Vector3 _vx = Vector3.Multiply(Vector3.Cross(_v, Vector3.Up), ((float)Math.Tan(Math.PI / 16.0)));
            Vector3 _vy = Vector3.Multiply(Vector3.Cross(_v, Vector3.Left), ((float)Math.Tan(Math.PI / 16.0)) / this._aspectratio);
            float _mx = (float)(2 * ((((double)e.X) / ((double)this._graphics.GraphicsDevice.Viewport.Width)) - 0.5));
            float _my = (float)(2 * (0.5 - (((double)e.Y) / ((double)this._graphics.GraphicsDevice.Viewport.Height))));
            return Vector3.Add(this._cameratarget, Vector3.Add(Vector3.Multiply(_vx, _mx), Vector3.Multiply(_vy, _my)));
        }

        #endregion

        #region _CalculateCoordinates_2

        private void _CalculateCoordinates_2(System.Windows.Forms.MouseEventArgs e, out float _angle_x, out float _angle_y)
        {
            double _mx = (2 * ((((double) e.X) / ((double)this._graphics.GraphicsDevice.Viewport.Width)) - 0.5));
            double _my = (2 * (0.5 - (((double) e.Y) / ((double)this._graphics.GraphicsDevice.Viewport.Height))));
            _angle_x = (float) Math.Tan(_mx * Math.Atan(Math.PI / 16.0));
            _angle_y = (float) Math.Tan(_my * Math.Atan(Math.PI / 16.0) / (double) this._aspectratio);
        }

        #endregion

        #region _MouseDown

        private void _MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                if (this._cameraconnection != null)
                {
                }
                else
                {
                    if ((e.Button & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left)
                    {
                        this._moving = true;
                        this._selectedposition = _CalculateCoordinates_1(e);
                    }
                    if ((e.Button & System.Windows.Forms.MouseButtons.Right) == System.Windows.Forms.MouseButtons.Right)
                    {
                        this._rotating = true;
                        _CalculateCoordinates_2(e, out _selected_angle_x, out _selected_angle_y);
                    }
                }
            }
        }

        #endregion

        #region _MouseMove

        private void _MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                if (this._cameraconnection != null)
                {
                }
                else
                {
                    if (this._moving)
                    {
                        Vector3 _movement = Vector3.Subtract(this._selectedposition, _CalculateCoordinates_1(e));
                        this._cameraposition = Vector3.Add(this._cameraposition, _movement);
                        this._cameratarget = Vector3.Add(this._cameratarget, _movement);
                        this._CalculateCoordinates();
                    }
                    if (this._rotating)
                    {
                        float _previous_angle_x = this._selected_angle_x;
                        float _previous_angle_y = this._selected_angle_y;
                        _CalculateCoordinates_2(e, out _selected_angle_x, out _selected_angle_y);
                        float _ax = _previous_angle_x - this._selected_angle_x;
                        float _ay = _previous_angle_y - this._selected_angle_y;
                        this._cameraposition =
                            Vector3.Add(this._cameratarget, Vector3.Transform(Vector3.Subtract(this._cameraposition, this._cameratarget),
                                Matrix.CreateRotationX(-_ay) * Matrix.CreateRotationY(_ax)));
                        this._CalculateCoordinates();
                    }
                }
            }
        }

        #endregion

        #region _MouseLeave

        private void _MouseLeave(object sender, EventArgs e)
        {
        }

        #endregion

        #region _MouseUp

        private void _MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                if (this._cameraconnection != null)
                {
                }
                else
                {
                    if ((e.Button & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left)
                    {
                        this._moving = false;
                    }
                    if ((e.Button & System.Windows.Forms.MouseButtons.Right) == System.Windows.Forms.MouseButtons.Right)
                    {
                        this._rotating = false;
                    }
                }
            }
        }

        #endregion

        #region _MouseWheel

        private void _MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                double ticks = ((double)e.Delta) / ((double)120.0);

                if (this._cameraconnection != null)
                {
                    this._downward_rotation += (float) ticks * Microsoft.Xna.Framework.MathHelper.PiOver2 / 45.0f;
                }
                else
                {
                    float zoom = (float)Math.Pow(0.95, ticks);
                    this._cameraposition =
                        Microsoft.Xna.Framework.Vector3.Add(
                            this._cameratarget,
                            Microsoft.Xna.Framework.Vector3.Multiply(
                                Microsoft.Xna.Framework.Vector3.Subtract(
                                    this._cameraposition,
                                    this._cameratarget),
                                zoom));
                }
                this._CalculateCoordinates();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _ContentCallback

        private QS.Fx.Xna.IContent _ContentCallback(QS.Fx.Xna.IContentRef _contentref)
        {
            lock (this)
            {
                return this._contentcontroller._GetContent(_contentref);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CalculateCoordinates

        private void _CalculateCoordinates()
        {
            this._cameramatrix =
                Microsoft.Xna.Framework.Matrix.CreateLookAt(
                    _cameraposition,
                    _cameratarget,
                    Microsoft.Xna.Framework.Vector3.Backward);

            this._projectionmatrix =
                Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(
                    Microsoft.Xna.Framework.MathHelper.ToRadians(45.0f),
                    this._aspectratio,
                    1.0f,
                    100000.0f);
        }

        #endregion

        #region Overridden Initialize

        protected override void Initialize()
        {
            base.Initialize();
        }

        #endregion

        #region Overridden LoadGraphicsContent

        protected override void LoadContent()
        {
            lock (this)
            {
                this._aspectratio = this._graphics.GraphicsDevice.Viewport.Width / this._graphics.GraphicsDevice.Viewport.Height;
                this._cameraposition = new Microsoft.Xna.Framework.Vector3(0.0f, 50.0f, 50000.0f);
                this._cameratarget = Microsoft.Xna.Framework.Vector3.Zero;
                // this._camerarotation = Matrix.Identity;
                this._CalculateCoordinates();
                //if (loadAllContent)
                //{
                    this._initialized = true;
                    foreach (_Object _o in this._objects.Values)
                    {
                        if (!_o.IsConnected)
                            _o.Connect();
                    }
                //}
            }
        }

        #endregion

        #region Overridden UnloadGraphicsContent

        protected override void UnloadContent()
        {
/*
            if (unloadAllContent)
                _content.Unload();
*/
        }

        #endregion

        #region Overridden Update

        protected override void Update(GameTime gameTime)
        {
            lock (this)
            {
                DateTime _now = DateTime.Now;
                if ((_now - this.lastcheck).TotalMilliseconds > 200)
                {
                    this.lastcheck = _now;
                    this._RefreshObjects();
                }

                foreach (_Object _o in this._objects.Values)
                    _o.Endpoint.Update
                        (gameTime);
            }
            base.Update(gameTime);
        }

        #endregion

        #region Overridden Draw

        protected override void Draw(GameTime gameTime)
        {
            this._graphics.GraphicsDevice.Clear(
                this._folderendpoint.IsConnected ? 
                    Microsoft.Xna.Framework.Color.SkyBlue : Microsoft.Xna.Framework.Color.Black);
            lock (this)
            {
                if (this._cameraconnection != null)
                    ((QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>)this).Set(this._cameraendpoint.Interface.Get());
                foreach (_Object _o in this._objects.Values)
                {
                    _o.Endpoint.Reposition(this._cameramatrix, this._projectionmatrix);
                    _o.Endpoint.Draw(gameTime);
                }
            }
            base.Draw(gameTime);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region IValueClient<ICoordinates> Members

        void QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>.Set(QS._qss_x_.Channel_.Message_.ICoordinates _value)
        {
            if (_value != null)
            {
                lock (this)
                {
                    this._cameraposition = new Vector3(_value.PX, _value.PY, _value.PZ);
                    Vector3 _direction = Vector3.Multiply(Vector3.Up, 10000);
                    _direction = Vector3.Transform(_direction, Matrix.CreateRotationX(_value.RX + this._downward_rotation));
                    _direction = Vector3.Transform(_direction, Matrix.CreateRotationY(_value.RY));
                    _direction = Vector3.Transform(_direction, Matrix.CreateRotationZ(_value.RZ));
                    this._cameratarget = Vector3.Add(this._cameraposition, _direction);
                    this._CalculateCoordinates();
                }
            }
        }

        #endregion

        #region IDictionaryClient<string,IObject> Members

        void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Ready()
        {
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Added(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Removed(
            String _key)
        {
        }

        #endregion
    }
}

#endif
