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
using System.Xml.Serialization;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.FolderView, "FolderView", "A user interface element showing contents of a folder.")]
    public sealed partial class FolderView
        : QS.Fx.Component.Classes.UI, QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>
    {
        #region Constructor

        public FolderView(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("folder", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader) 
            : base(_mycontext)
        {
            this._mycontext = _mycontext;

            InitializeComponent();

            this._folder = _folder;

            this._folderendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
                QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>>(this);

            this._folderendpoint.OnConnect += new QS.Fx.Base.Callback(this._FolderConnectCallback);
            this._folderendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._FolderDisconnectCallback);

            this._loaderendpoint =
                _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();

            if (_loader != null)
            {
                this._loaderobject = _loader.Dereference(_mycontext);
                this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loaderobject.Endpoint);
            }
            else
                throw new Exception("Folder view cannot run without the attached loader.");

            if (_folder != null)
            {
                this._folderobject = _folder.Dereference(_mycontext);
                this._folderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._folderendpoint).Connect(_folderobject.Endpoint);
            }
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("folderobject")]
        private QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject> _folderobject;

        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable("folder")]
        private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>> _folder;

        [QS.Fx.Base.Inspectable("folderendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IDictionary<string, QS.Fx.Object.Classes.IObject>,
            QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>> _folderendpoint;

        [QS.Fx.Base.Inspectable("loaderobject")]
        private QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderobject;

        [QS.Fx.Base.Inspectable("loaderendpoint")]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;

        [QS.Fx.Base.Inspectable("folderconnection")]
        private QS.Fx.Endpoint.IConnection _folderconnection;

        [QS.Fx.Base.Inspectable("loaderconnection")]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        #endregion

        #region _FolderConnectCallback

        private void _FolderConnectCallback()
        {
            lock (this)
            {
                this.Enabled = true;
                //this._Refresh();
            }
        }

        #endregion

        #region _FolderDisconnectCallback

        private void _FolderDisconnectCallback()
        {
            this.Enabled = false;
        }

        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            lock (this)
            {
                if (_loaderendpoint.IsConnected && _folderendpoint.IsConnected && !_folderendpoint.Interface.IsReadOnly())
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

        #region _DragDrop

        private void _DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] _filenames = (string[]) e.Data.GetData(DataFormats.FileDrop);
                foreach (string _filename in _filenames)
                {
                    string _text;
                    using (StreamReader _streamreader = new StreamReader(_filename))
                    {
                        _text = _streamreader.ReadToEnd();
                    }
                    _Add(_text);
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text, false))
            {
                string _text = (string) e.Data.GetData(DataFormats.Text);
                _Add(_text);
            }
            else if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
            {
                string _text = (string) e.Data.GetData(DataFormats.UnicodeText);
                _Add(_text);
            }
            else
                throw new Exception("The drag and drop operation cannot continue because none of the data formats was recognized.");
        }

        #endregion

        #region _DragLeave

        private void _DragLeave(object sender, EventArgs e)
        {
        }

        #endregion

        #region _MouseDoubleClick

        private void _MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count == 1)
            {
                _FolderItem _folderitem = (_FolderItem) listView1.SelectedItems[0];
                if (_folderitem != null)
                {
                    if (_folderitem._object.ObjectClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ObjectClasses.Window)))
                    {
                        QS.Fx.Object.Classes.IObject _actualobject = _folderitem._object.Dereference(_mycontext);
                        if (_actualobject is System.Windows.Forms.Form)
                            ((System.Windows.Forms.Form)_actualobject).Show();
                    }
                }
            }
        }

        #endregion

        #region _ItemDrag
        
        private void _ItemDrag(object sender, ItemDragEventArgs e)
        {
            lock (this)
            {
                _FolderItem _folderitem = (_FolderItem)e.Item;
                if (_folderitem != null)
                {
                    string _id = _folderitem._name;
                    QS.Fx.Reflection.Xml.ReferenceObject _xmlobject =
                        new QS.Fx.Reflection.Xml.ReferenceObject(
                            _id, QS.Fx.Attributes.Attributes.Serialize(_folderitem._object.Attributes), 
                            _folderitem._object.ObjectClass.Serialize,
                            null,
                            null, 
                            this._folder.Serialize);
                    QS.Fx.Reflection.Xml.Root _xmlroot = new QS.Fx.Reflection.Xml.Root(_xmlobject);
                    StringBuilder sb = new StringBuilder();
                    using (StringWriter writer = new StringWriter(sb))
                    {
                        (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(writer, _xmlroot);
                    }
                    string _objectxml = sb.ToString();
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

        #region _KeyDown

        private void _KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                lock (_folderendpoint)
                {
                    foreach (_FolderItem item in listView1.SelectedItems)
                    {
                        _folderendpoint.Interface.Remove(item._name);
                    }
                }

                _Refresh();
            }
        }

        #endregion

        #region _Add

        private void _Add(string _objectxml)
        {
            lock (_folderendpoint)
            {
                if (_loaderendpoint.IsConnected && _folderendpoint.IsConnected && !_folderendpoint.Interface.IsReadOnly())
                {
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref = this._loaderendpoint.Interface.Load(_objectxml);
                    string _key = QS.Fx.Attributes.Attribute.ValueOf(_objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);

                    bool _alreadythere = false;
                    foreach (_FolderItem _this_folderitem in this.listView1.Items)
                    {
                        if (_this_folderitem._name.Equals(_key))
                        {
                            _alreadythere = true;
                            break;
                        }
                    }

                    if (!_alreadythere)
                        _folderendpoint.Interface.Add(_key, _objectref);
                    else
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
                            _folderendpoint.Interface.Add(_new_key, _objectref);                                
                        }
                        else
                        {
                            // The user gave up on inserting this object.
                        }
                    }
                }
            }
        }

        private static Random _renamerandom = new Random();

        #endregion

        #region Class _FolderItem

        private class _FolderItem : ListViewItem
        {
            public _FolderItem(string _name, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object) : base(_name, 0)
            {
                this._name = _name;
                this._object = _object;

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

        #region _Menu_Refresh

        private void _Menu_Refresh(object sender, EventArgs e)
        {
            this._Refresh();
        }

        #endregion

        #region _Refresh

        private void _Refresh()
        {
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(new QS.Fx.Base.Callback(this._Refresh), new object[0]);
            }
            else
            {
                List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>> _objects = 
                    new List<KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>>();
                lock (_folderendpoint)
                {
                    IEnumerable<string> _names = _folderendpoint.Interface.Keys();
                    foreach (string _name in _names)
                    {
                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object;
                        if (_folderendpoint.Interface.TryGetObject(_name, out _object))
                            _objects.Add(new KeyValuePair<string,QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(_name, _object));
                        //else
                            // Should not throw exception - it is possible that in between getting the key list and trying to
                            // get a specific key, a certain key is deleted from the folder by another client of that folder
                            //throw new Exception("Could not fetch object named \"" + _name + "\" from the folder.");
                    }
                }

                lock (this)
                {
                    listView1.BeginUpdate();
                    listView1.Clear();
                    try
                    {
                        foreach (KeyValuePair<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>> _element in _objects)
                            listView1.Items.Add(new _FolderItem(_element.Key, _element.Value));
                    }
                    finally
                    {
                        listView1.EndUpdate();
                    }
                }
            }
        }

        #endregion

        #region IDictionaryClient<string,IObject> Members

        void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Ready()
        {
            System.Threading.ThreadPool.QueueUserWorkItem
            (
                new System.Threading.WaitCallback
                (
                    delegate(object _o) 
                    { 
                        this._Refresh(); 
                    }
                )
            );
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>.Added(
            string _key, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _object)
        {
            if (listView1.InvokeRequired)
            {
                listView1.BeginInvoke(
                    new QS.Fx.Base.ContextCallback<string, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject>>(
                        ((QS.Fx.Interface.Classes.IDictionaryClient<string, QS.Fx.Object.Classes.IObject>) this).Added),
                        new object[] { _key, _object });
            }
            else
            {
                lock (this)
                {
                    listView1.Items.Add(new _FolderItem(_key, _object));
                }
            }
        }

        void QS.Fx.Interface.Classes.IDictionaryClient<String, QS.Fx.Object.Classes.IObject>.Removed(
            string _key)
        {
            System.Threading.ThreadPool.QueueUserWorkItem
            (
                new System.Threading.WaitCallback
                (
                    delegate(object _o)
                    {
                        this._Refresh();
                    }
                )
            );
        }

        #endregion
    }
}
