/*

Copyright (c) 2009 Mihir Patel, Umang Kaul. All rights reserved.

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
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;


namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("E15D2EA1513A4cdeB5BC81E9B594E116`1", "Control")]
    public sealed partial class Control 
        :
        UserControl,
        QS.Fx.Object.Classes.IUI,
        IOutgoing,
        QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>
    {
        public Control(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("rootFolder", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> rootFolder,      
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader)            
        {
            Isis.IsisSystem.Start();

            myGroup = Isis.Group.Lookup("Circuit");
            if (myGroup == null)
                myGroup = new Isis.Group("Circuit");

            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name1, string val)
            {
                if (val.StartsWith("newf"))
                {
                    string _objectxml = "";
                    string fname = val.Substring(4);
                    using (StreamReader _streamreader = new StreamReader(fname))
                    {
                        _objectxml = _streamreader.ReadToEnd();
                    }
                    _Add(_objectxml, fname);
                }
            };
            myGroup.Join();

            this._mycontext = _mycontext;
            InitializeComponent();
            this.uiendpoint = _mycontext.ExportedUI(this);

            this._loaderendpoint =
                _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();
            this._loaderconnection = this._loaderendpoint.Connect(_loader.Dereference(_mycontext).Endpoint);
                        
            this.path = "root";
            this.errFlag = false;   
            button6.Enabled = false;
            
            endPoint =
                this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>(this);
            folder = rootFolder.Dereference(this._mycontext);
            endPoint.Connect(folder.Endpoint);

            string name = QS.Fx.Attributes.Attribute.ValueOf(rootFolder.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, rootFolder.ID);
            ((Control)this).Text = "Connected to folder " + name + ".";


            connections = new Dictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
                QS.Fx.Endpoint.IConnection>();
            endptStack = new Stack<QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>>(10);

            busyFlag = false;//used when creating new folder, to suppress refresh callback

        }
        delegate void stringArg(string who, string val);
        Isis.Group myGroup;
        int UPDATE = 1;

        private System.Windows.Forms.ImageList Large;
        private QS.Fx.Endpoint.Internal.IExportedUI uiendpoint;
        private QS.Fx.Endpoint.IConnection _loaderconnection;        
                
        public string upFolder;
        public string path;
        public string newName, oldkey;
        public bool errFlag, busyFlag;
        private QS.Fx.Object.IContext _mycontext;
        private static readonly QS.Fx.Reflection.IObjectClass sharedFolderObject =
           QS.Fx.Reflection.Library.LocalLibrary.ObjectClass<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();

        public QS.Fx.Endpoint.Internal.IDualInterface<
        QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>> endPoint;

        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> folderRef;
        QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject> folder;
        
        
        public QS.Fx.Endpoint.IConnection _conn;
        
        public IDictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> objMap;
        
        public static Stack<QS.Fx.Endpoint.Internal.IDualInterface<
        QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
        QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>> endptStack;

        //stores connections to shared folder object endpoints
        private IDictionary<QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>,
           QS.Fx.Endpoint.IConnection> connections;

        public ListViewItem selection;
        [QS.Fx.Base.Inspectable("folder")]
        private QS.Fx.Object.IReference<IChannelFolder> _folder;       

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        //public String oldID;

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this.uiendpoint; }
        }
        
        void IOutgoing.Ready()
        {
            //this.RefreshCallback();
        }

        void IOutgoing.Alert(string from,string objectxml)
        {
            /*
            if (from.Equals("-1"))
            {
                MessageBox.Show("Already exists!");
                listView1.Items.Remove(listView1.FindItemWithText(objectxml, true, 0, true));
                ListViewItem l1;

                int totalCount = listView1.Items.Count;
                int newCount = 0;
                for (int i = 0; i < totalCount; i++)
                {
                    if (listView1.Items[i].Text.StartsWith("New Folder"))
                        newCount++;
                }
                if (newCount == 0)
                    l1 = new ListViewItem("New Folder", 0);
                else
                    l1 = new ListViewItem("New Folder (" + newCount + ")", 0);
                listView1.Items.Add(l1);
                l1.BeginEdit();

            }
            else
            {
                this.RefreshCallback();                
            }
             * */
        }

        private void RefreshCallback()
        {        
            /*    
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new QS.Fx.Base.Callback(this.RefreshCallback), new object[0]);                
            }
            else
            {
                if (this.path.Equals("root"))
                    button6.Enabled = false;
                else
                    button6.Enabled = true;

                List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _objects =
                    new List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
                List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _sharedfolders =
                    new List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
                List<KeyValuePair<string, string>> _folders = new List<KeyValuePair<string,string>>();

                lock (this)
                {
                    Channels channels;                    
                    channels = folderendpoint.Interface.Channels(this.path);                        
                    if (!path.Equals("root"))//go to parent folder
                    {
                        int endind = path.LastIndexOf('/');                        
                        string path_parent = path.Substring(0,endind);
                        int startind;
                        if (path_parent.Contains('/'))
                            startind = path_parent.LastIndexOf('/')+1;
                        else
                            startind = 0;
                        upFolder = path.Substring(startind,endind-startind);
                        //_folders.Add(new KeyValuePair<string,string>(parentfolder, ""));
                    }

                    foreach (Channel channel in channels.channels)
                    {
                        
                        if(channel.objectxml.Equals("thisisafolder"))
                            _folders.Add(new KeyValuePair<string,string>(channel.id, ""));
                        
                        else if(channel.objectxml.Contains("thisisasharedfolder"))
                        {
                            //QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(channel.objectxml);
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(channel.objectxml);
                            _sharedfolders.Add(new KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(channel.id, _objectref));
                        }
                        else
                        {
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(channel.objectxml);                        
                            _objects.Add(new KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(channel.id, _objectref));
                        }
                        
                    }
                }

                lock (this)
                {
                    listView1.BeginUpdate();
                    listView1.Clear();
                    try
                    {
                        foreach (KeyValuePair<string, string> folditem in _folders)
                        {
                            string currentfolder = path;
                            listView1.Items.Add(new _FoldItem(folditem.Key, currentfolder));
                        }
                        foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _element in _objects)
                        {                            
                            listView1.Items.Add(new _FolderItem(_element.Key, _element.Value));                            
                        }
                        foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _element in _sharedfolders)
                        {
                            listView1.Items.Add(new _SharedFolder(_element.Key, _element.Value));
                        }
                    }
                    finally
                    {
                        listView1.EndUpdate();
                        listView1.Refresh();
                    }
                }
            }
            */
        }

        #region _refresh
        private void _refresh()
        {
                                                 
            List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _objects =
                    new List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
            List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _sharedfolders =
                new List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();


            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new QS.Fx.Base.Callback(this._refresh), new object[0]);
            }
            else
            {
                pathLabel.Text = path;
                if (endptStack.Count == 0)
                    this.button6.Enabled = false;
                else
                    this.button6.Enabled = true;
                IEnumerator<KeyValuePair<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> iterator = objMap.GetEnumerator();
                while (iterator.MoveNext())
                {
                    if (iterator.Current.Value.ObjectClass.IsSubtypeOf(sharedFolderObject))
                        _sharedfolders.Add(new KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(iterator.Current.Key, iterator.Current.Value));
                    else
                        _objects.Add(new KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(iterator.Current.Key, iterator.Current.Value));
                }

                lock (this)
                {
                    listView1.BeginUpdate();
                    listView1.Clear();
                    try
                    {
                        foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _element in _objects)
                        {
                            listView1.Items.Add(new _FolderItem(_element.Key, _element.Value));
                        }
                        foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _element in _sharedfolders)
                        {
                            listView1.Items.Add(new _SharedFolder(_element.Key, _element.Value));
                        }
                    }
                    finally
                    {
                        listView1.EndUpdate();
                        listView1.Refresh();
                    }
                }
            }
        }
        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected)
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                        e.Data.GetDataPresent(DataFormats.Text, false) || e.Data.GetDataPresent(DataFormats.UnicodeText, false))
                    {
                        e.Effect = DragDropEffects.All;
                    }
                }
                else
                    e.Effect = DragDropEffects.None;
            }
        }

        #endregion

        #region _MouseDoubleClick

        private void _MouseDoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                _FolderItem _sI = (_FolderItem)listView1.SelectedItems[0];
                
                if (listView1.SelectedItems[0].ImageIndex == 1)
                {
                    StringBuilder sb = new StringBuilder();
                    using (StringWriter writer = new StringWriter(sb))
                    {
                        (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(_sI._object.Serialize));
                    }
                    string _objectxml = sb.ToString();
                    myGroup.Send(UPDATE, "Test", "newx" + _objectxml);
                }

                if (listView1.SelectedItems[0].ImageIndex == 3)
                {
                    _SharedFolder _sFolder;                    
                    lock (this)
                    {
                        _sFolder = (_SharedFolder)listView1.SelectedItems[0];               
                        this.path = this.path + "/" + _sFolder._name;
                        
                        this.getChildren(_sFolder._name, _sFolder._object);                        
                    }                    
                }

                //MessageBox.Show(folderendpoint.Interface.GetXML(this.parentfolder, this.currentfolder, listView1.SelectedItems[0].Text));
                /*
                    _FolderItem _folderitem = (_FolderItem)listView1.SelectedItems[0];
                    if (_folderitem != null)
                    {
                        if (_folderitem._object.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window)))
                        {
                            QS.Fx.Object.Classes.IObject _actualobject = _folderitem._object.Dereference(_mycontext);
                            if (_actualobject is System.Windows.Forms.Form)
                                ((System.Windows.Forms.Form)_actualobject).Show();
                        }
                    }
                 * */
                
                 
            }
        }

        #endregion

        #region _DragDrop

        private void _DragDrop(object sender, DragEventArgs e)
        {
            lock (this)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                {
                    string[] _handles = (string[])e.Data.GetData(DataFormats.FileDrop);                                        
                    foreach (string s in _handles)
                    {
                        if (Directory.Exists(s))
                        {
                            handleDragDropFolder(s);                                                        
                                //endPoint = endptStack.Pop();
                            if (endPoint.IsConnected)
                                endPoint_OnConnected();                            
                        }
                        else if (File.Exists(s))
                        {
                            string _text;
                            using (StreamReader _streamreader = new StreamReader(s))
                            {
                                _text = _streamreader.ReadToEnd();
                            }
                            _Add(_text, s);
                        }
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.Text);
                    _Add(_text, "");
                }
                else if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.UnicodeText);
                    _Add(_text, "");
                }
                else
                    throw new Exception("The drag and drop operation cannot continue because none of the data formats was recognized.");
            }
        }
        #endregion

        private void handleDragDropFolder(String s)
        {
            //store all *.liveobject files of the directory into that folder in listview
            DirectoryInfo di = new DirectoryInfo(s);
            FileInfo[] files = di.GetFiles();
            // use a boolean to track if this is a "special" .default folder
            bool autoFolder = false;
            
            foreach (FileInfo file in files)
            {
                if (file.Name.Contains(".default"))
                {
                    string _text;
                    using (StreamReader _streamreader = new StreamReader(file.FullName))
                    {
                        _text = _streamreader.ReadToEnd();
                    }
                    _AddNewFolder(_text);
                    this.path = this.path + "/" + file.Name;
                    autoFolder = true;
                    break;
                }
            }

            foreach (FileInfo file in files)
            {
                if (!file.Name.Contains(".default"))
                {
                    string _text;
                    using (StreamReader _streamreader = new StreamReader(file.FullName))
                    {
                        _text = _streamreader.ReadToEnd();
                    }
                    if (_loaderendpoint.IsConnected)
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                        try
                        {
                            _objectref = this._loaderendpoint.Interface.Load(_text);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Couldn't load object " + file.FullName);
                            continue;
                        }                        
                        string _key = QS.Fx.Attributes.Attribute.ValueOf(_objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);
                        if (endPoint.Interface.ContainsKey(_key))
                        {
                            continue;
                        }
                        try
                        {
                            endPoint.Interface.Add(_key, _objectref);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            
            DirectoryInfo[] subDirs = di.GetDirectories();
            foreach (DirectoryInfo dir in subDirs)
            {
                handleDragDropFolder(dir.FullName);             
            }
            this.busyFlag = false;

            // if it was a "special" .default folder, then pop it off the stack, otherwise the current folder is what we want
            if (autoFolder)
            {
                endPoint.Disconnect();
                if (endptStack.Count > 0)
                {
                    endPoint = endptStack.Pop();
                }

                try
                {
                    this.path = this.path.Substring(0, this.path.LastIndexOf('/'));
                }
                catch (Exception)
                {
                }
            }
        }

        #region AddFolderHack
        private void _AddNewFolder(string _objectxml)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;                    
                    _objectref = this._loaderendpoint.Interface.Load(_objectxml);                                      
                    string _key = QS.Fx.Attributes.Attribute.ValueOf(_objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);

                    //MessageBox.Show("Checking contains folder: " + _key);
                    if (endPoint.IsConnected && !endPoint.Interface.ContainsKey(_key))
                    {
                        try
                        {
                           // MessageBox.Show("Adding folder: " + _key);
                            endPoint.Interface.Add(_key, _objectref);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Exception adding folder: " + _key);
                        }
                    }

                    endptStack.Push(endPoint);
                        
                    //connecting the endpoint of the new folder 
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> folderRef;

                    folderRef = _objectref.CastTo<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();

                    QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject> folder;
                    folder = folderRef.Dereference(this._mycontext);

                    endPoint =
                    this._mycontext.DualInterface<
                    QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
                    QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>(this);
                    this.busyFlag = true;
                    _conn = endPoint.Connect(folder.Endpoint);
                    System.Threading.Thread.Sleep(50);
                    endPoint_OnConnected();
                    
                                         
                }
            }
        }

        #endregion

        #region _DragLeave

        private void _DragLeave(object sender, EventArgs e)
        {            
        }

        #endregion

        #region _ItemDrag

        private void _ItemDrag(object sender, ItemDragEventArgs e)
        {                            
            lock (this)
            {
                string _objectxml;
                _FolderItem _folderitem;
                _SharedFolder _shFolder;
                string _id;
                if (listView1.SelectedItems[0].ImageIndex == 1 || listView1.SelectedItems[0].ImageIndex == 3)
                {
                    if (listView1.SelectedItems[0].ImageIndex == 1)
                    {
                        _folderitem = (_FolderItem)e.Item;
                        _id = _folderitem._name;
                        
                        StringBuilder sb = new StringBuilder();
                        using (StringWriter writer = new StringWriter(sb))
                        {
                            (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(_folderitem._object.Serialize));
                        }
                        _objectxml = sb.ToString();                        
                    }
                    else
                    {
                        _shFolder = (_SharedFolder)e.Item;
                        _id = _shFolder._name;                        
                        StringBuilder sb = new StringBuilder();
                        using (StringWriter writer = new StringWriter(sb))
                        {
                            (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, new QS.Fx.Reflection.Xml.Root(_shFolder._object.Serialize));
                        }
                        _objectxml = sb.ToString();                        
                    }                     
                   
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.UnicodeText, _objectxml);
                    string _tempfile = Path.GetTempPath() + Path.DirectorySeparatorChar + _id + ".liveobject";
                    if (File.Exists(_tempfile))
                    {
                        int k = 0;
                        do
                        {
                            k++;
                            _tempfile = Path.GetTempPath() + Path.DirectorySeparatorChar + _id + "_" + k.ToString() + ".liveobject";
                        }
                        while (File.Exists(_tempfile));
                    }
                    using (StreamWriter writer = new StreamWriter(_tempfile, false))
                    {
                        writer.WriteLine(_objectxml);
                    }
                    System.Collections.Specialized.StringCollection _c = new System.Collections.Specialized.StringCollection();
                    _c.Add(_tempfile);
                    _dataobject.SetFileDropList(_c);
                    // TODO: There is a leak here, we should somehow get rid of the temporary file afterwards.
                    DoDragDrop(_dataobject, DragDropEffects.Move);
                }
            }
        }

        #endregion

        #region _Add

        private void _Add(string _objectxml, string _filename)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected)
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                    try
                    {
                        _objectref = this._loaderendpoint.Interface.Load(_objectxml);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Couldn't load object " + _filename);
                        return;
                    }
                    string _key = QS.Fx.Attributes.Attribute.ValueOf(_objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);

                    if(!endPoint.IsConnected)
                        return;

                    if (endPoint.Interface.ContainsKey(_key) && !_objectref.ObjectClass.IsSubtypeOf(sharedFolderObject))
                    {
                        if (MessageBox.Show("This folder already contains an object with identifier \"" + _key +
                            "\". Do you want to automatically rename your object before inserting in into this folder?",
                            "Object already exists in the folder",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            DateTime _now = DateTime.Now;
                            string _timestamp = _now.Year.ToString("0000") + "_" + _now.Month.ToString("00") +
                                "_" + _now.Day.ToString("00") + "_" + _now.Hour.ToString("00") + "_" + _now.Minute.ToString("00") +
                                "_" + _now.Second.ToString("00") + "_" + _renamerandom.Next(1000000).ToString("000000");
                            string _new_key = _key + "_" + _timestamp;
                            _key = _new_key;
                        }
                        else
                        {
                            // The user gave up on inserting this object.
                            return;
                        }
                    }
                    else if (endPoint.Interface.ContainsKey(_key) && _objectref.ObjectClass.IsSubtypeOf(sharedFolderObject))
                    {
                        // we don't allow creating new folders on the fly, so just ignore it.
                        return;
                    }

                    endPoint.Interface.Add(_key, _objectref);
                    endPoint_OnConnected();
                }
            }
        }

        private static Random _renamerandom = new Random();

        #endregion

        #region Class _FolderItem

        private class _FolderItem : ListViewItem
        {
            public _FolderItem(string _name, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
                : base(_name, 0)
            {
                this._name = _name;
                this._object = _object;
                this.ImageIndex = 1; //liveobject file
                StringBuilder sb = new StringBuilder();
                QS.Fx.Attributes.IAttribute _attribute;
                if (this._object.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _attribute))
                    sb.AppendLine(_attribute.Value);
                if (this._object.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_comment, out _attribute))
                    sb.AppendLine(_attribute.Value);
                string s = sb.ToString().Trim();
                if (s.Length > 0)
                    this.ToolTipText = s;
                 
            }

            public string _name;
            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
        }

        #endregion

        #region Class _SharedFolder

        private class _SharedFolder : ListViewItem
        {
            public _SharedFolder(string _name, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
                : base(_name, 0)
            {
                this._name = _name;
                this._object = _object;
                this.ImageIndex = 3; //shared folder
                StringBuilder sb = new StringBuilder();
                QS.Fx.Attributes.IAttribute _attribute;
                if (this._object.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _attribute))
                    sb.AppendLine(_attribute.Value);
                if (this._object.Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_comment, out _attribute))
                    sb.AppendLine(_attribute.Value);
                string s = sb.ToString().Trim();
                if (s.Length > 0)
                    this.ToolTipText = s;
            }

            public string _name;
            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
        }
        #endregion

        #region Browse button
        private void button4_Click(object sender, EventArgs e)
        {            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            
            openFileDialog1.InitialDirectory = "c:\\Users\\Mihir\\Desktop\\objects";
            openFileDialog1.Filter = "LiveObject files (*.liveobject)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {                
                try
                {
                    string _filename = openFileDialog1.FileName;
                    string _text;
                    using (StreamReader _streamreader = new StreamReader(_filename))
                    {
                        _text = _streamreader.ReadToEnd();
                    }
                    _Add(_text, _filename);
                }
                catch (Exception)
                {
                    MessageBox.Show("Incorrect Path");
                }
            }            
        }
        #endregion

        #region Right Click Menu
                
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                string keyItem;                                
                endPoint.Interface.Remove(selection.Text);
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                this.oldkey = listView1.FocusedItem.Text;
                listView1.FocusedItem.BeginEdit();                        
            }
        }
        
        private void _AfterLabelEdit(Object sender, System.Windows.Forms.LabelEditEventArgs e)
        {
            lock (this)
            {
                string renamed = e.Label;

                if (endPoint.Interface.ContainsKey(renamed))
                {
                    MessageBox.Show("Folder already contains a file with the same name");
                    listView1.FocusedItem.BeginEdit();
                }

                //get objref, and write new name in XML spec
                if(listView1.FocusedItem.ImageIndex == 1)
                {
                    _FolderItem LOfile = (_FolderItem)listView1.FocusedItem;                    
                    //QS.Fx.Attributes.Attribute.ValueOf(LOfile._object.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, LOfile._object.ID) = renamed;                    
                    //LOfile._object.ID = renamed;                                        
                    MessageBox.Show("Name in ID = " + LOfile._object.ID + ";\n" + "Name in XML = " + QS.Fx.Attributes.Attribute.ValueOf(LOfile._object.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, LOfile._object.ID));                     
                }
                else if (listView1.FocusedItem.ImageIndex == 3)
                {
                    _SharedFolder sFolder = (_SharedFolder)listView1.FocusedItem;
                    MessageBox.Show("Folders cannot be renamed.");
                }

                //change name in folder
                                
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef;
                if(endPoint.Interface.TryGetObject(this.oldkey, out objRef))
                {
                    endPoint.Interface.Remove(this.oldkey);
                    endPoint.Interface.Add(renamed, objRef);
                }                                    
            }
        }
        
        private void _MouseDown(object sender, MouseEventArgs e)
        {
            selection = listView1.GetItemAt(e.X, e.Y);
        }

        #endregion

        #region Back_Button
        private void button6_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                this.path = pathLabel.Text.Substring(0, pathLabel.Text.LastIndexOf('/'));
                pathLabel.Text = this.path;
               
                endPoint.Disconnect();
                endPoint = endptStack.Pop();
                endPoint_OnConnected();                
            }
        }
        #endregion

        #region IDictionaryClient<String,IObject> Members

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Ready()
        {                        
            System.Threading.ThreadPool.QueueUserWorkItem
            (
                new System.Threading.WaitCallback
                (
                    delegate(object _o)
                    {
                        lock (this)
                        {
                            this.endPoint_OnConnected();
                        }
                    }
                )
            );
             
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Added(String _keyName, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _key)
        {
            System.Threading.ThreadPool.QueueUserWorkItem
            (
                new System.Threading.WaitCallback
                (
                    delegate(object _o)
                    {
                        lock (this)
                        {
                            this.endPoint_OnConnected();
                        }
                    }
                )
            );
        }
        
        
        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Removed(String _key)
        {
            System.Threading.ThreadPool.QueueUserWorkItem
            (
                new System.Threading.WaitCallback
                (
                    delegate(object _o)
                    {
                        lock (this)
                        {
                            this.endPoint_OnConnected();
                        }
                    }
                )
            );
        }

        #endregion

        void endPoint_OnConnected()
        {
            lock (this)
            {
                if (busyFlag == false && endPoint.IsConnected)
                {
                    objMap =
                        new Dictionary<String, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>();

                    foreach (String _name in endPoint.Interface.Keys())
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objRef;
                        if (endPoint.Interface.TryGetObject(_name, out objRef))
                        {
                            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> objReference =
                                objRef.CastTo<QS.Fx.Object.Classes.IObject>();

                            objMap.Add(_name, objReference);
                        }
                    }
                    this._refresh();
                }
            }
        }

        void getChildren(String _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _keyRef)
        {
            //cast it to a folder object  
            lock (this)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>> folderRef;

                folderRef = _keyRef.CastTo<QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>>();

                QS.Fx.Object.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject> folder;
                folder = folderRef.Dereference(this._mycontext);            
                endptStack.Push(endPoint);
                endPoint =
                this._mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionary<String, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>>(this);
                
                _conn = endPoint.Connect(folder.Endpoint);
                if (endPoint.IsConnected)
                    endPoint_OnConnected();
            }
        }

        #region Switch View
        private void button7_Click(object sender, EventArgs e)
        {
            Size s;
            if (button7.Text.Contains("List"))
            {                
                listView1.SmallImageList = Small;
                //listView1.View = View.SmallIcon;
                listView1.View = View.List;
                button7.Text = "Switch to Icon View";
            }
            else
            {                
                listView1.LargeImageList = Large;
                listView1.View = View.LargeIcon;
                button7.Text = "Switch to List View";
            }
            //this._refresh();
        }
        #endregion

        #region Folder Browse Button
        private void button1_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                FolderBrowserDialog fd = new FolderBrowserDialog();
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    string folderName = fd.SelectedPath;
                    
                    handleDragDropFolder(folderName);

                    if (endPoint.IsConnected)
                        endPoint_OnConnected();                 
                }
            }
        }
        #endregion
    }    
}
