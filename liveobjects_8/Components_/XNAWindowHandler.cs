/*

Copyright (c) 2007-2009 Jared Cantwell (jmc279@cornell.edu), Petko Nikolov (pn42@cornell.edu), Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Threading;

#if XNA
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using System.Windows.Forms;
using MapLibrary;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class XnaWindowHandler
    {
        #region Fields

        // Xna Content Fields
        private IGraphicsDeviceService graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        // Livebojects system variables
        private QS.Fx.Endpoint.Internal.IImportedInterface<ICameraManagerOps> cameramanagerendpoint;
        private QS.Fx.Endpoint.IConnection cameramanagerconnection;
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        private QS.Fx.Endpoint.IConnection _loaderconnection;
        private QS._qss_x_.Content_.Xna_.IController_ _contentcontroller;
        private QS.Fx.Object.IContext _mycontext;

        // static references for type comparison
        private static readonly QS.Fx.Reflection.IObjectClass sharedFolderObject =
           QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();
        private static readonly QS.Fx.Reflection.IObjectClass uiobjectclass =
            QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IUI_X>();

        private _Folder rootFolder;
    
        // Camera update and position variables
        private Matrix view, proj;
        private Vector3 cameraPosition;
        private Vector3 cameraLookAt;
        private Vector3 cameraUp;

        // Set field of view of the camera in radians (pi/4 is 45 degrees).
        private float viewAngle = MathHelper.Pi / 4;

        // Set distance from the camera of the near and far clipping planes.
        private float nearClip = 1.0f;
        private float farClip = 350000.0f;

        // Used to handle the window as if it were a Windows Forms object
        private Control _form;

        /// GeoDiscoveryService Fields
        private QS.Fx.Endpoint.Internal.IImportedInterface<IGeoDiscoveryClientOps> _discclientendpoint;
        private QS.Fx.Endpoint.IConnection _discclientconnection;
        private IGeoDiscoveryClient _discClient;

        private bool running = true;
        private GridTranslator gridTrans = new GridTranslator();
        private Thread GeoDiscoveryServiceQuerier;
        private Thread GeoDiscoveryServiceUpdater;
        private Dictionary<string, DateTime> foreignObjects;

        private object UIObjectsReadLock1 = new object();
        private object UIObjectsReadLock2 = new object();

        private double lastGetDuration;

        /// Debug window Fields
        private List<String> message_log = new List<String>();
        private Boolean show_messages = false;
        private int messageTop = -1;

        // General Fields
        private KeyboardState lastKS;
        private Texture2D pixel;
        private bool mouseInBounds;
        private float mouseDelta;
        private Vector2 clickLoc = Vector2.Zero;
        private IServiceProvider Services;
        private int scrollAmount = 0;
        private int scrollTimer = 0; // only do a scroll action this many frames

        // Layer management size fields
        private int LAYERPANEL_WIDTH = 125;
        private int LAYERPANEL_VERTICAL_SEPARATION = 20;
        private int LAYERPANEL_BORDER_WIDTH = 3;
        private int LAYERPANEL_INDENT_AMOUNT = 10; // in pixels

        // Layer management color fields
        private Color LAYERPANEL_BORDER_COLOR = Color.Black;
        private Color LAYERPANEL_BG_COLOR = Color.White;
        private Color LAYERPANEL_NONE_COLOR = Color.Gray;
        private Color LAYERPANEL_TEXTCOLOR_ACTIVE = Color.Blue;
        private Color LAYERPANEL_TEXTCOLOR_INACTIVE = Color.Red;
        private Color LAYERPANEL_TEXTCOLOR_EXPANDED = Color.Green;
        private Color LAYERPANEL_TEXTCOLOR_UNEXPANDED = Color.Black;

        // Layer management etc
        private int SCROLL_TIMER_TICKS = 10;
        
        #endregion

        #region Constructor

        public XnaWindowHandler(
            IGraphicsDeviceService graphics,
            Control _form,
            IServiceProvider services,
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Dictionary", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IDictionary<
                        String, QS.Fx.Object.Classes.IObject>> _folder,
            [QS.Fx.Reflection.Parameter("CameraManager", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<ICameraManager> cameraManager,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader,
            [QS.Fx.Reflection.Parameter("GeoDiscoveryService", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<IGeoDiscoveryClient> _discClient,
            [QS.Fx.Reflection.Parameter("Resizeable", QS.Fx.Reflection.ParameterClass.Value)]
            bool resizeable,
            [QS.Fx.Reflection.Parameter("NearClip", QS.Fx.Reflection.ParameterClass.Value)]
            Single nearClip,
            [QS.Fx.Reflection.Parameter("FarClip", QS.Fx.Reflection.ParameterClass.Value)]
            Single farClip,
            [QS.Fx.Reflection.Parameter("WindowWidth", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 windowWidth,
            [QS.Fx.Reflection.Parameter("WindowHeight", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 windowHeight)
        {
            this._mycontext = _mycontext;
            this.graphics = graphics;
            this.Services = services;
            this._contentcontroller = new QS._qss_x_.Content_.Xna_.Controller_(QS._qss_x_.Content_.Controller_._Local, this.Services);

            // Camera Manager
            this.cameramanagerendpoint = _mycontext.ImportedInterface<ICameraManagerOps>();
            if (cameraManager != null)
                this.cameramanagerconnection = this.cameramanagerendpoint.Connect(cameraManager.Dereference(_mycontext).CameraManager);

            // Loader
            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
            if (_loader == null)
                throw new Exception("Cannot run without a loader.");
            this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);

            if (nearClip <= 0) nearClip = 1;
            this.nearClip = nearClip;

            if (farClip <= 0) farClip = this.farClip;
            this.farClip = farClip;

            // This configuration assumes Xna is implemented with a Windows Forms
            // behind the scene and hacks at it.  If this become not true, later
            // version may not work correctly.
            this._form = _form;
            this._form.AllowDrop = true;
            this._form.DragEnter += new System.Windows.Forms.DragEventHandler(this._DragEnter);
            this._form.DragDrop += new System.Windows.Forms.DragEventHandler(this._DragDrop);
            this._form.MouseDown += new System.Windows.Forms.MouseEventHandler(this._MouseDown);
            this._form.MouseMove += new System.Windows.Forms.MouseEventHandler(this._MouseMove);
            this._form.MouseUp += new System.Windows.Forms.MouseEventHandler(this._MouseUp);
            this._form.MouseEnter += new EventHandler(this._MouseEnter);
            this._form.MouseLeave += new EventHandler(this._MouseLeave);

            // Folder/Dictionary setup
            if (_folder != null)
            {
                string name = QS.Fx.Attributes.Attribute.ValueOf(_folder.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _folder.ID);
                rootFolder = new _Folder(this, _folder, name);
                rootFolder.Expanded = true;
                this._form.Text = "Connected to folder " + rootFolder.Name + ".";
            }

            // GeoDiscovery Client
            this._discclientendpoint = _mycontext.ImportedInterface<IGeoDiscoveryClientOps>();
            if (_discClient != null)
            {
                this._discClient = _discClient.Dereference(_mycontext);
                this._discclientconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._discclientendpoint).Connect(this._discClient.GeoDiscoveryClient);

                foreignObjects = new Dictionary<string, DateTime>();

                this.GeoDiscoveryServiceQuerier = new Thread(new ThreadStart(this.QueryGeoDiscoveryService));
                this.GeoDiscoveryServiceQuerier.Start();
                this.GeoDiscoveryServiceUpdater = new Thread(new ThreadStart(this.UpdateObjectLocations));
                this.GeoDiscoveryServiceUpdater.Start();
            }

            mouseInBounds = false;

            // Debugging Test messages!!
            message_log.Add("WeatherRenderer - No Internet Connection Found!");
            message_log.Add("MSN Virtual Earth - No credentials supplied, using cache.");
            message_log.Add("ImageRenderer - Cache doesn't exist.  Creating cache in ....");
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public void Initialize()
        {
            LayerPanelEntry.COLOR_ACTIVE = LAYERPANEL_TEXTCOLOR_ACTIVE;
            LayerPanelEntry.COLOR_INACTIVE = LAYERPANEL_TEXTCOLOR_INACTIVE;
        }
        #endregion

        #region Load/Unload Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        public void LoadContent()
        {
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = (SpriteFont)this.ContentCallback(new QS.Fx.Xna.ContentRef(QS.Fx.Xna.ContentClass.SpriteFont, "C312BA6E943345baA586A9263E840CC6`1:Kootenay10.xnb")).Content;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        public void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region LayerPanelEntry
        private class LayerPanelEntry
        {
            public static Color COLOR_ACTIVE;
            public static Color COLOR_INACTIVE;
            public bool active = true;
            public string text;
            public _Object _object;


            public QS.Fx.Endpoint.Classes.IImportedUI_X Endpoint
            {
                get { return _object.Endpoint; }
            }

            public Color Color
            {
                get
                {
                    if (active) return COLOR_ACTIVE;
                    else return COLOR_INACTIVE;
                }
            }

            public LayerPanelEntry(string text, _Object o)//QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> objref, QS.Fx.Endpoint.Classes.IImportedUI_X endpoint)
            {
                this._object = o;
                this.text = text;
            }
        }
        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Folder

        private class _Folder : IDisposable, QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>
        {
            #region Constructor

            public _Folder(
                    XnaWindowHandler _owner, 
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<
                        String, QS.Fx.Object.Classes.IObject>> _folder,
                    string name)
            {
                this._owner = _owner;
                //this.name = QS.Fx.Attributes.Attribute.ValueOf(_folder.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _folder.ID);
                this.name = name;

                // Folder/Dictionary setup
                this._folderendpoint = _owner._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>>(this);

                if (_folder != null)
                {
                    
                    this._folder = _folder.Dereference(_owner._mycontext);
                    this._folderconnection =
                        ((QS.Fx.Endpoint.Classes.IEndpoint)this._folderendpoint).Connect(this._folder.Endpoint);
                    this._folderinterface = this._folderendpoint.Interface;
                }
            }

            #endregion

            #region Fields

            //private static TextWriter tw = new StreamWriter("c:\\log.txt", false);

            private string name;
            private QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folder;
            private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>> _folderendpoint;
            private QS.Fx.Endpoint.IConnection _folderconnection;
            private QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folderinterface;

            private IDictionary<string, LayerPanelEntry> layers = new Dictionary<string, LayerPanelEntry>();
            private IDictionary<string, _Folder> _folders = new Dictionary<string, _Folder>();
            private IDictionary<string, _Object> _objects = new Dictionary<string, _Object>();
            private XnaWindowHandler _owner;

            /* Locks */
            //private object UIObjectsReadLock1 = new object();
            //private object UIObjectsReadLock2 = new object();


            #endregion

            public string Name
            {
                get { return name; }
            }

            public bool Expanded { get; set; }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                foreach (_Object o in _objects.Values)
                {
                    QS.Fx.Object.Classes.IObject obj = o.Object;
                    try
                    {
                        if (obj is IDisposable)
                            ((IDisposable)obj).Dispose();
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }

                if (_folder is IDisposable)
                    ((IDisposable)_folder).Dispose();

                foreach (_Folder f in _folders.Values)
                    ((IDisposable)f).Dispose();
            }

            #endregion

            #region IDictionaryClient<string,IObject> Members

            void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Ready()
            {
                try
                {
                    lock (_owner)
                    {
                        foreach (string _key in this._folderendpoint.Interface.Keys())
                        {
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef;

                            if (this._folderendpoint.Interface.TryGetObject(_key, out objRef))
                            {
                                Log("Ready(): " + _key);
                                _AddObject(_key, objRef);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log("Ready(): exception: " + ex.ToString());
                }
            }

            void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Added(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
            {
                Log("Added(): " + _key);
                lock (_owner)
                {
                    _AddObject(_key, _object);
                }
            }

            void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Removed(string _key)
            {
                try
                {

                    lock (_owner.UIObjectsReadLock1)
                    {
                        lock (_owner.UIObjectsReadLock2)
                        {
                            lock (this._objects)
                            {
                                if (_objects.ContainsKey(_key))
                                {
                                    layers.Remove(_key);
                                    ((IDisposable)_objects[_key]).Dispose();
                                    _objects.Remove(_key);

                                    /*
                                    if (_discclientendpoint.IsConnected)
                                        _discclientendpoint.Interface.Delete(_key);
                                     */
                                }
                                else if (_folders.ContainsKey(_key))
                                {
                                    _Folder f = _folders[_key];
                                    _folders.Remove(_key);
                                    ((IDisposable)f).Dispose();
                                }
                            }
                        }
                    }

                    /*
                    if (this._discclientendpoint.IsConnected)
                    {
                        lock (foreignObjects)
                        {
                            if (foreignObjects.ContainsKey(_key))
                                foreignObjects.Remove(_key);
                        }
                    }
                     */
                }
                catch (Exception ex)
                {
                    Log("Removed(): " + ex);
                }
            }

            #endregion

            private void _AddObject(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef)
            {
                try
                {
                    Log("Adding Object " + _key);
                    if (objRef == null) return;

                    if (objRef.ObjectClass.IsSubtypeOf(uiobjectclass))
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> objreference =
                            objRef.CastTo<QS.Fx.Object.Classes.IUI_X>();

                        lock (_owner.UIObjectsReadLock1) // lock all read locks - in effect get a write lock
                        {
                            lock (_owner.UIObjectsReadLock2)
                            {
                                lock (this._objects)
                                {
                                    _Object _obj = new _Object(_owner, objreference);

                                    if (!_objects.ContainsKey(_key))
                                    {
                                        _objects.Add(_key, _obj);
                                    }
                                    else
                                    {
                                        Log(_key + ": object exists");
                                    }

                                    if (!layers.ContainsKey(_key))
                                    {
                                        layers.Add(_key, new LayerPanelEntry(_key, _obj));
                                    }
                                    else
                                    {
                                        Log(_key + ": layer exists");
                                    }
                                    

                                    /*
                                    if (this._discclientendpoint.IsConnected)
                                    {
                                        lock (foreignObjects)
                                        {
                                            if (uiobject is IGeoPositionedOps && !foreignObjects.ContainsKey(_key)) //should be IGeoPositioned in reality, but dual inheritance is not allowed
                                            {
                                                RegisterWithDiscovery(_key, _object);
                                                Location loc = gridTrans.PixelToLatLon(((IGeoPositionedOps)uiobject).GetLocation());
                                                float minZoom = ((IGeoPositionedOps)uiobject).GetMinZoomLevel();
                                                float maxZoom = ((IGeoPositionedOps)uiobject).GetMaxZoomLevel();
                                                _discclientendpoint.Interface.UpdateLocation(_key, loc, minZoom, maxZoom);
                                            }
                                        }
                                    }
                                     */
                                }
                            }
                        }
                    }
                    else if (objRef.ObjectClass.IsSubtypeOf(sharedFolderObject))
                    {
                        Log(_key + ": folder");
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> _f =
                            objRef.CastTo<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();
                        lock (this._folders)
                        {
                            if (!_folders.ContainsKey(_key))
                            {
                                _folders.Add(_key, new _Folder(_owner, _f, _key));
                            }
                            else
                            {
                                Log(_key + ": folder exists");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log(_key + ": exception");
                }
            }

            private void Log(String msg)
            {
                //tw.WriteLine(msg);
                //tw.Flush();
            }

            public List<LayerPanelEntry> AllLayers
            {
                get
                {
                    List<LayerPanelEntry> all = new List<LayerPanelEntry>();
                    all.AddRange(layers.Values);

                    foreach (_Folder f in _folders.Values)
                        all.AddRange(f.AllLayers);

                    return all;
                }
            }

            public ICollection<LayerPanelEntry> Layers
            {
                get { return layers.Values; }
            }

            public ICollection<_Folder> SubFolders
            {
                get { return _folders.Values; }
            }

            /* Used to add objects to the underlying folder, which will trigger the Added() callback
             * above on its own to handle updating the internal state */
            public void Add(string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
            {
                lock (_owner)
                {
                    _folderendpoint.Interface.Add(_key, _objectref);
                }
            }
        }

        #endregion

        #region _Object

        private class _Object : IDisposable
        {
            public _Object(XnaWindowHandler _owner, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> _reference)
            {
                this._owner = _owner;
                this._reference = _reference;
                this._object = _reference.Dereference(_owner._mycontext);
                this._endpoint =
                    _owner._mycontext.ImportedUI_X
                    (
                    this._owner.graphics,
                    new QS.Fx.Endpoint.Internal.Xna.ContentCallback(this._owner.ContentCallback)
                    );

                this.Connect();
            }

            private bool _connected;
            private XnaWindowHandler _owner;
            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI_X> _reference;
            private QS.Fx.Object.Classes.IUI_X _object;
            private QS.Fx.Endpoint.Classes.IImportedUI_X _endpoint;
            private QS.Fx.Endpoint.IConnection _connection;

            public QS.Fx.Object.Classes.IObject Object
            {
                get { return _object; }
            }

            public QS.Fx.Endpoint.Classes.IImportedUI_X Endpoint
            {
                get { return _endpoint; }
            }

            public bool IsConnected
            {
                get { return _connected; }
            }

            public void Connect()
            {
                this._connection = this._endpoint.Connect(_object.UI);
                this._connected = true;
            }

            public void Disconnect()
            {
                if (this._connection != null)
                    this._connection.Dispose();
                this._connection = null;
                this._connected = false;
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
                    string[] _filenames = (string[])e.Data.GetData(System.Windows.Forms.DataFormats.FileDrop);
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
                    string _text = (string)e.Data.GetData(System.Windows.Forms.DataFormats.Text);
                    _Drop(_text);
                }
                else if (e.Data.GetDataPresent(System.Windows.Forms.DataFormats.UnicodeText, false))
                {
                    string _text = (string)e.Data.GetData(System.Windows.Forms.DataFormats.UnicodeText);
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
                        
                        try
                        {
                            rootFolder = new _Folder(this, _folder_ref, _key);

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
                                //if (this._cameraconnection != null)
                                //{
                                //    this._cameraconnection.Dispose();
                                // System.Threading.ThreadPool.QueueUserWorkItem(
                                //    new System.Threading.WaitCallback(this._AsynchronousDisconnectionCallback), this._cameraconnection);
                                //}
                                //this._cameraconnection =
                                //    ((QS.Fx.Endpoint.Classes.IEndpoint)this._cameraendpoint).Connect(_coordinates_object.Endpoint);
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
                                    rootFolder.Add(_key, _objectref);
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

        #region _MouseDown

        private void _MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                if (InInfoPane(e.X, e.Y))
                {
                    if (clickLoc.Equals(Vector2.Zero))
                        clickLoc = new Vector2(e.X, e.Y);
                }

                message_log.Add(clickLoc.X + " " + clickLoc.Y);
            }
        }

        #endregion

        #region _MouseMove

        private void _MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
        }

        #endregion

        #region _MouseEnter

        private void _MouseEnter(object sender, EventArgs e)
        {
            mouseInBounds = true;
        }

        #endregion

        #region _MouseLeave

        private void _MouseLeave(object sender, EventArgs e)
        {
            mouseInBounds = false;
        }

        #endregion

        #region _MouseUp

        private void _MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            lock (this)
            {
                if (!clickLoc.Equals(Vector2.Zero))
                {
                    int index = (int)clickLoc.Y / (int)LAYERPANEL_VERTICAL_SEPARATION;
                    object o = FromDisplayIndex(index);
                    if (o is _Folder)
                        (o as _Folder).Expanded = !(o as _Folder).Expanded;
                    else if (o is LayerPanelEntry)
                        (o as LayerPanelEntry).active = !(o as LayerPanelEntry).active;
                    else
                        ; // must be null, do nothing
                    clickLoc = Vector2.Zero;
                }
            }
        }

        #endregion

        #region Mouse Wheel delta change
        public void SetMouseWheelDelta(float delta)
        {
            mouseDelta = delta;
        }

        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            lock (this)
            {
                if (!mouseInBounds)
                    return;

                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouseState = Mouse.GetState();

                bool aDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A);
                bool zDown = keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Z);

                // see if we should scroll the scrollpane
                if (scrollTimer == 0 && (aDown || zDown))
                {
                    scrollAmount += aDown ? -1 : 1;
                    scrollAmount = Math.Min(MaxPaneIndex() - PanelDisplayCount(), scrollAmount);
                    scrollAmount = Math.Max(0, scrollAmount);
                    scrollTimer = SCROLL_TIMER_TICKS;
                }
                else if (scrollTimer != 0 && (aDown || zDown))
                {
                    scrollTimer--;
                }
                else // neither key pressed
                {
                    scrollTimer = 0;
                }

                if (this.cameramanagerendpoint.IsConnected && !InInfoPane(mouseState.X, mouseState.Y))
                {
                    InputState iState = new InputState(mouseState, keyboardState, mouseDelta);
                    mouseDelta = 0.0f;
                    Window_XStateEvent state = new Window_XStateEvent(iState);

                    UpdateWindowEvent uwe = this.cameramanagerendpoint.Interface.UpdateCamera(state);
                    cameraPosition = uwe.CameraPosition.Vector3_;
                    cameraLookAt = uwe.CameraReference.Vector3_ + cameraPosition;
                    cameraUp = uwe.CameraUp.Vector3_;


                    // Set up the view matrix and projection matrix.
                    view = Matrix.CreateLookAt(cameraPosition, cameraLookAt, cameraUp);

                    // Setup projection matrix.  Always recalculated in case aspect ratio changes
                    float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;
                    proj = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);

                    lock (UIObjectsReadLock1)
                    {
                        if (rootFolder != null)
                        {
                            foreach (LayerPanelEntry e in rootFolder.AllLayers)
                            {
                                if (e.active)
                                {
                                    ((QS.Fx.Endpoint.Internal.IImportedUI_X)e.Endpoint).Reposition(view, proj);
                                    ((QS.Fx.Endpoint.Internal.IImportedUI_X)e.Endpoint).Update(gameTime);
                                }
                            }
                        }
                    }
                }  // end cameraenpoint connected

                /*** In Progress: Intercepting debug messages ***
                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) && !lastKS.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    show_messages = !show_messages;
                }

                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E) && !lastKS.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                {
                    if (messageTop == -1)
                    {
                    }
                }

                if (keyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C) && !lastKS.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C))
                {
                   // show_messages = !show_messages;
                }
                *****/

                lastKS = keyboardState;
            }
        }

        #endregion

        #region ContentCallback
        QS.Fx.Xna.IContent ContentCallback(QS.Fx.Xna.IContentRef target)
        {
            lock (this)
            {
                return this._contentcontroller._GetContent(target);
            }

        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Draw(GameTime gameTime)
        {
            lock (this)
            {
                graphics.GraphicsDevice.Clear(Color.Black);

                if (rootFolder == null) return;

                lock (UIObjectsReadLock1)
                {
                    foreach (LayerPanelEntry e in rootFolder.AllLayers)
                    {
                        if (e.active)
                            ((QS.Fx.Endpoint.Internal.IImportedUI_X)e.Endpoint).Draw(gameTime);
                    }
                }

                spriteBatch.Begin();

                lock (UIObjectsReadLock1)
                {
                    int viewportWidth = graphics.GraphicsDevice.Viewport.Width;
                    // draw border as a rectangle, then background as another one on top of it
                    Rectangle borderRect = new Rectangle(
                        viewportWidth - LAYERPANEL_WIDTH - LAYERPANEL_BORDER_WIDTH,
                        0,
                        LAYERPANEL_WIDTH + LAYERPANEL_BORDER_WIDTH,
                        LAYERPANEL_VERTICAL_SEPARATION * Math.Max(MaxPaneIndex(), 1) + LAYERPANEL_BORDER_WIDTH);
                    spriteBatch.Draw(pixel, borderRect, LAYERPANEL_BORDER_COLOR);
                    // Panel background
                    Rectangle backRect = new Rectangle(
                        viewportWidth - LAYERPANEL_WIDTH,
                        0,
                        LAYERPANEL_WIDTH,
                        LAYERPANEL_VERTICAL_SEPARATION * Math.Max(MaxPaneIndex(), 1));
                    spriteBatch.Draw(pixel, backRect, LAYERPANEL_BG_COLOR);

                    // display (none) if there are no elements
                    if (FromDisplayIndex(0) == null)
                    {
                        Vector2 pos = new Vector2(viewportWidth - LAYERPANEL_WIDTH, 0);
                        spriteBatch.DrawString(spriteFont, "(none)", pos, LAYERPANEL_NONE_COLOR);
                    }
                    else
                    {
                        int index = -1;
                        DrawFolder(rootFolder, ref index, 0);
                    }
                }

                spriteBatch.End();

                /*** In Progress: Intercepting debug messages ***
                if (show_messages)
                {
                    float y_start = (float) graphics.GraphicsDevice.Viewport.Height * 3f / 4f;

                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                    Color bgcolor = new Color(Color.Black, 1f);
                    int height = graphics.GraphicsDevice.Viewport.Height - (int) y_start;
                    spriteBatch.Draw(pixel, new Rectangle(0, (int)y_start, graphics.GraphicsDevice.Viewport.Width, height), bgcolor);

                    int max = height / (int)spriteFont.MeasureString(" ").Y;
                    int start = Math.Max(message_log.Count - max, 0);// +messageOffset;
                    int end = Math.Min(message_log.Count - 1, start + max);
                

                    for (int i = start; i <= end; i++ )
                    {
                        String message = message_log[i];
                        float yOffset = (float)(i - start) * spriteFont.MeasureString(" ").Y + y_start;
                        float xOffset = 10f;
                        spriteBatch.DrawString(spriteFont, "Log: " + message, new Vector2(xOffset, yOffset), Color.Green);
                    }

                    spriteBatch.End();
                }
                ****/
            }
        }
        #endregion

        #region Cleanup
        public void Cleanup()
        {
            running = false;
            Thread.Sleep(1000);
            if (rootFolder != null)
            {
                lock (UIObjectsReadLock1)
                {
                    lock (UIObjectsReadLock2)
                    {
                        ((IDisposable)rootFolder).Dispose();
                    }
                }
            }
            
            if (this._discClient != null && this._discClient is IDisposable)
                ((IDisposable)this._discClient).Dispose();
        }
        #endregion

        #region GeoDiscovery Methods

        private void RegisterWithDiscovery(String key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            /*
            string _objectxml;

            StringBuilder sb = new StringBuilder();
            using (StringWriter writer = new StringWriter(sb))
            {
                try
                {
                    (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(_object.Serialize));
                }
                catch (Exception _exc)
                {
                    throw new Exception("Could not register object with Discovery because the object could not be serialized.\n", _exc);
                }
            }
            _objectxml = sb.ToString();

            _discclientendpoint.Interface.Register(key, _objectxml);
             */
        }

        private void QueryGeoDiscoveryService()
        {
            /*
            while (running)
            {
                Thread.Sleep(1000);
                float x = cameraPosition.X;
                float y = cameraPosition.Y;
                float z = cameraPosition.Z;
                float range = z * 360f / 131072f;
                Location loc = gridTrans.PixelToLatLon(y, x, z);
                float top = loc.Latitude + 1.5f * range;
                float bottom = loc.Latitude - 1.5f * range;
                float left = loc.Longitude - 1.5f * range;
                float right = loc.Longitude + 1.5f * range;

                DateTime start = DateTime.Now;
                Dictionary<string, string> objDictionary = _discclientendpoint.Interface.GetObjectKeys(top, bottom, left, right, z).objDictionary;

                if (_loaderendpoint.IsConnected && !_folderendpoint.Interface.IsReadOnly())
                {
                    foreach (KeyValuePair<string, string> obj in objDictionary)
                    {
                        bool contained = false;
                        lock (foreignObjects)
                            contained = foreignObjects.ContainsKey(obj.Key);
                        if (!contained)
                        {
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(obj.Value);
                            //MessageBox.Show("adding object " + obj.Key);

                            lock (foreignObjects)
                                foreignObjects.Add(obj.Key, DateTime.Now);
                            _folderendpoint.Interface.Add(obj.Key, _objectref);
                        }
                        else
                        {
                            lock (foreignObjects)
                                foreignObjects[obj.Key] = DateTime.Now;
                        }
                    }
                }

                lastGetDuration = 1.0 + (DateTime.Now - start).TotalSeconds;
            }
             */
        }

        private void UpdateObjectLocations()
        {
            /*
            List<string> toRemove = new List<string>();
            while (running)
            {
                Thread.Sleep(1000);
                lock (UIObjectsReadLock2)
                {
                    foreach (KeyValuePair<string, QS.Fx.Object.Classes.IObject> obj in uiobjects)
                    {
                        lock (foreignObjects)
                        {
                            if (!foreignObjects.ContainsKey(obj.Key))
                            {
                                if (obj.Value is IGeoPositionedOps && ((IGeoPositionedOps)obj.Value).IsMobile())
                                {
                                    Location loc = gridTrans.PixelToLatLon(((IGeoPositionedOps)obj.Value).GetLocation());
                                    float minZoom = ((IGeoPositionedOps)obj.Value).GetMinZoomLevel();
                                    float maxZoom = ((IGeoPositionedOps)obj.Value).GetMaxZoomLevel();
                                    _discclientendpoint.Interface.UpdateLocation(obj.Key, loc, minZoom, maxZoom);
                                }
                            }
                            else
                            {
                                Location objLoc = gridTrans.PixelToLatLon(((IGeoPositionedOps)obj.Value).GetLocation());
                                float minZoom = ((IGeoPositionedOps)obj.Value).GetMinZoomLevel();
                                float maxZoom = ((IGeoPositionedOps)obj.Value).GetMaxZoomLevel();
                                float x = cameraPosition.X;
                                float y = cameraPosition.Y;
                                float z = cameraPosition.Z;
                                float range = z * 360f / 131072f;
                                Location loc = gridTrans.PixelToLatLon(y, x, z);
                                float top = loc.Latitude + 1.5f * range;
                                float bottom = loc.Latitude - 1.5f * range;
                                float left = loc.Longitude - 1.5f * range;
                                float right = loc.Longitude + 1.5f * range;
                                if (objLoc.Latitude > top || objLoc.Latitude < bottom ||
                                    objLoc.Longitude < left || objLoc.Longitude > right ||
                                    z > maxZoom || z < minZoom)
                                {
                                    toRemove.Add(obj.Key);
                                }
                                else if (DateTime.Now - foreignObjects[obj.Key] > new TimeSpan(0, 0, (int)(3.0 * lastGetDuration)))
                                    toRemove.Add(obj.Key);
                            }
                        }
                    }
                }

                foreach (String key in toRemove)
                {
                    _folderendpoint.Interface.Remove(key);
                    /*
                    lock (foreignObjects)
                    {
                        if (foreignObjects.ContainsKey(key))
                            foreignObjects.Remove(key);
                    }
                     * *//*
                }
                toRemove.Clear();
            }
    */
        }

        #endregion

        #region Helper methods
        private bool InInfoPane(int x, int y)
        {
            return x > graphics.GraphicsDevice.Viewport.Width - LAYERPANEL_WIDTH &&
                    y < LAYERPANEL_VERTICAL_SEPARATION * MaxPaneIndex();
        }

        /**
         * Returns either the Folder or Object that is at that display index.
         * Returns null if index is out of bounds.
         */
        private object FromDisplayIndex(int index)
        {
            int curr = -1;
            return DispIndexHelper(index + scrollAmount, rootFolder, ref curr);
        }

        private object DispIndexHelper(int index, _Folder self, ref int curr)
        {
            if (curr == index)
                return self;

            curr++;

            if (!self.Expanded)    
                return null;

            foreach (_Folder child in self.SubFolders)
            {
                object ret = DispIndexHelper(index, child, ref curr);
                if (ret != null)
                    return ret;
            }

            foreach (LayerPanelEntry child in self.Layers)
            {
                if (curr == index)
                    return child;
                curr++;
            }

            return null;
        }

        /**
         * Returns one more than the highest possible index in the pane.
         */
        private int MaxPaneIndex()
        {
            int index = -1;
            MaxPaneIndexHelper(rootFolder, ref index);
            return index;
        }

        private void MaxPaneIndexHelper(_Folder self, ref int index)
        {
            index++;

            if (!self.Expanded)
                return;

            foreach (_Folder child in self.SubFolders)
                MaxPaneIndexHelper(child, ref index);

            index += self.Layers.Count;
        }

        /**
         * Draw a folder and its contents to the panel.
         */
        private void DrawFolder(_Folder self, ref int index, int indent)
        {
            DrawLabel("[" + self.Name + "]", index, indent,
                self.Expanded ? LAYERPANEL_TEXTCOLOR_EXPANDED : LAYERPANEL_TEXTCOLOR_UNEXPANDED);

            index++;

            if (!self.Expanded)
                return;

            foreach (_Folder child in self.SubFolders)
                DrawFolder(child, ref index, indent + 1);

            foreach (LayerPanelEntry e in self.Layers)
            {
                DrawLabel(e.text, index, indent + 1, e.Color);
                index++;
            }
        }

        /**
         * Draw a label in the correct position.
         */
        private void DrawLabel(string text, int index, int indent, Color color)
        {
            int viewportWidth = graphics.GraphicsDevice.Viewport.Width;
            Vector2 pos = new Vector2(viewportWidth - LAYERPANEL_WIDTH + indent * LAYERPANEL_INDENT_AMOUNT,
                LAYERPANEL_VERTICAL_SEPARATION * (index - scrollAmount));
            spriteBatch.DrawString(spriteFont, text, pos, color);
            
        }

        private int PanelDisplayCount()
        {
            return graphics.GraphicsDevice.Viewport.Height / LAYERPANEL_VERTICAL_SEPARATION;
        }
        #endregion
    }

}

#endif


