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
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Designer_2,
        "Designer_2", "This component can be used to view the structure of or edit the descriptions of objects.")]
    public sealed partial class Designer_2 : QS.Fx.Component.Classes.UI
    {
        #region Constructor

        public Designer_2(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("deserializer", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS._qss_x_.Interface_.Classes_.IDeserializer>> _deserializer,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader)
            : base(_mycontext)
        {
            InitializeComponent();

            if (_deserializer == null)
                throw new Exception("The loader object reference is empty, but the designer object cannot work without the attached loader object.");

            if (_loader == null)
                throw new Exception("The loader object reference is empty, but the designer object cannot work without the attached loader object.");

            this._deserializerendpoint = _mycontext.ImportedInterface<QS._qss_x_.Interface_.Classes_.IDeserializer>();
            this._loaderendpoint = _mycontext.ImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();

            this._deserializerconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._deserializerendpoint).Connect(_deserializer.Dereference(_mycontext).Endpoint);
            this._loaderconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _deserializerconnection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS._qss_x_.Interface_.Classes_.IDeserializer> _deserializerendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _loaderconnection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>> _loaderendpoint;
        [QS.Fx.Base.Inspectable]
        private bool _loaded;
        [QS.Fx.Base.Inspectable]
        private _Node _loaded_root;      

        #endregion 

        #region _ImageIndexes

        private enum _ImageIndexes : int
        {
            Unrecognized = 0,
            Object,
            Class,
            Parameter,
            Endpoint,
            Value
        }

        #endregion

        #region Fonts

        private static readonly Font _Font_Regular = new Font(FontFamily.GenericSansSerif, (float)8.25, FontStyle.Regular);
        private static readonly Font _Font_Object = new Font(FontFamily.GenericSansSerif, (float)8.25, FontStyle.Bold | FontStyle.Underline);
        private static readonly Font _Font_Value = new Font(FontFamily.GenericSansSerif, (float)8.25, FontStyle.Bold);
        private static readonly Font _Font_Parameter = new Font(FontFamily.GenericSansSerif, (float)8.25, FontStyle.Bold);

        #endregion

        #region _Load

        private void _Load(string _xmlspecification)
        {

            if (!this._loaded)
            {
                this._loaded = true;
                try
                {                    
/*
                    using (System.Xml.Serialization

                    this._deserializerendpoint.Interface.DeserializeObject(
                    this._loadedobjectref = this._loaderendpoint.Interface.Load(_xmlspecification);



                    treeView1.Nodes.Clear();
                    _Node _node = new _Node(this, _Node._Type.Object, this._loadedobjectref);
                    treeView1.Nodes.Add(_node);
                    treeView1.Visible = true;
*/ 
                }
                catch (Exception _exc)
                {
                    this._loaded = false;
/*
                    this._loadedobjectref = null;
*/
                    treeView1.Nodes.Clear();
                    treeView1.Visible = false;
                    (new QS._qss_x_.Base1_.ExceptionForm(_exc)).Show();
                }
            }
        }

        #endregion

        #region Class _Node ******************************************

        private class _Node : TreeNode
        {
            #region _Type

            public enum _Type
            {
                ValueClass, InterfaceClass, EndpointClass, ObjectClass, Value, Object, Endpoint, Parameter
            }

            #endregion

            #region Constructor

            public _Node(Designer _designer, _Type _type, object _data)
                : base()
            {
                this._designer = _designer;
                this._type = _type;
                this._data = _data;

                _Refresh();
            }

            #endregion

            #region Fields

            private Designer _designer;
            private _Type _type;
            private object _data;

            #endregion

            #region _Refresh ****************************************************************

            public void _Refresh()
            {
                this.BackColor = System.Drawing.Color.White;
                this.ForeColor = System.Drawing.Color.Black;
                this.NodeFont = _Font_Regular;
                this.Nodes.Clear();
                try
                {
                    switch (_type)
                    {
/*
                        #region ValueClass

                        case _Type.ValueClass:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Class;
                                this.SelectedImageIndex = (int)_ImageIndexes.Class;
                                if (_data == null)
                                    throw new Exception("Value class cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IValueClass))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IValueClass).Name + "\"");
                                QS.Fx.Reflection.IValueClass _valueclass = (QS.Fx.Reflection.IValueClass)_data;
                                string _name = QS.Fx.Attributes.Attribute.ValueOf(_valueclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null);
                                this.Text = "ValueClass" + ((_name != null) ? (" \"" + _name + "\"") : ((_valueclass.ID != null) ? (" " + _valueclass.ID) : string.Empty));
                                string _comment = QS.Fx.Attributes.Attribute.ValueOf(_valueclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null);
                                this.ToolTipText = _comment;
                                foreach (QS.Fx.Reflection.IParameter _parameter in _valueclass.ClassParameters.Values)
                                    this.Nodes.Add(new _Node(this._designer, _Type.Parameter, _parameter));
                            }
                            break;

                        #endregion

                        #region InterfaceClass

                        case _Type.InterfaceClass:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Class;
                                this.SelectedImageIndex = (int)_ImageIndexes.Class;
                                if (_data == null)
                                    throw new Exception("Interface class cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IInterfaceClass))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IInterfaceClass).Name + "\"");
                                QS.Fx.Reflection.IInterfaceClass _interfaceclass = (QS.Fx.Reflection.IInterfaceClass)_data;
                                string _name = QS.Fx.Attributes.Attribute.ValueOf(_interfaceclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null);
                                this.Text = "InterfaceClass" + ((_name != null) ? (" \"" + _name + "\"") : ((_interfaceclass.ID != null) ? (" " + _interfaceclass.ID) : string.Empty));
                                string _comment = QS.Fx.Attributes.Attribute.ValueOf(_interfaceclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null);
                                this.ToolTipText = _comment;
                                foreach (QS.Fx.Reflection.IParameter _parameter in _interfaceclass.ClassParameters.Values)
                                    this.Nodes.Add(new _Node(this._designer, _Type.Parameter, _parameter));
                            }
                            break;

                        #endregion

                        #region EndpointClass

                        case _Type.EndpointClass:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Class;
                                this.SelectedImageIndex = (int)_ImageIndexes.Class;
                                if (_data == null)
                                    throw new Exception("Endpoint class cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IEndpointClass))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IEndpointClass).Name + "\"");
                                QS.Fx.Reflection.IEndpointClass _endpointclass = (QS.Fx.Reflection.IEndpointClass)_data;
                                string _name = QS.Fx.Attributes.Attribute.ValueOf(_endpointclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null);
                                this.Text = "EndpointClass" + ((_name != null) ? (" \"" + _name + "\"") : ((_endpointclass.ID != null) ? (" " + _endpointclass.ID) : string.Empty));
                                string _comment = QS.Fx.Attributes.Attribute.ValueOf(_endpointclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null);
                                this.ToolTipText = _comment;
                                foreach (QS.Fx.Reflection.IParameter _parameter in _endpointclass.ClassParameters.Values)
                                    this.Nodes.Add(new _Node(this._designer, _Type.Parameter, _parameter));
                            }
                            break;

                        #endregion

                        #region ObjectClass

                        case _Type.ObjectClass:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Class;
                                this.SelectedImageIndex = (int)_ImageIndexes.Class;
                                if (_data == null)
                                    throw new Exception("Object class cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IObjectClass))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IObjectClass).Name + "\"");
                                QS.Fx.Reflection.IObjectClass _objectclass = (QS.Fx.Reflection.IObjectClass)_data;
                                string _name = QS.Fx.Attributes.Attribute.ValueOf(_objectclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null);
                                this.Text = "ObjectClass" + ((_name != null) ? (" \"" + _name + "\"") : ((_objectclass.ID != null) ? (" " + _objectclass.ID) : string.Empty));
                                string _comment = QS.Fx.Attributes.Attribute.ValueOf(_objectclass.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null);
                                this.ToolTipText = _comment;
                                foreach (QS.Fx.Reflection.IParameter _parameter in _objectclass.ClassParameters.Values)
                                    this.Nodes.Add(new _Node(this._designer, _Type.Parameter, _parameter));
                                foreach (QS.Fx.Reflection.IEndpoint _endpoint in _objectclass.Endpoints.Values)
                                    this.Nodes.Add(new _Node(this._designer, _Type.Endpoint, _endpoint));
                            }
                            break;

                        #endregion

                        #region Value

                        case _Type.Value:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Value;
                                this.SelectedImageIndex = (int)_ImageIndexes.Value;
                                this.NodeFont = _Font_Value;
                                if (_data != null)
                                {
                                    this.ForeColor = System.Drawing.Color.Green;
                                    this.Text = _data.ToString();
                                }
                                else
                                {
                                    this.Text = "null";
                                    this.ForeColor = System.Drawing.Color.Maroon;
                                }
                            }
                            break;

                        #endregion

                        #region Object

                        case _Type.Object:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Object;
                                this.SelectedImageIndex = (int)_ImageIndexes.Object;
                                this.NodeFont = _Font_Object;
                                if (_data != null)
                                {
                                    this.ForeColor = System.Drawing.Color.Blue;
                                    if (!(_data is QS.Fx.Reflection.IObject))
                                        throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IObject).Name + "\"");
                                    QS.Fx.Reflection.IObject _object = (QS.Fx.Reflection.IObject)_data;
                                    string _name = QS.Fx.Attributes.Attribute.ValueOf(_object.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, null);
                                    this.Text = "Object" + ((_name != null) ? (" \"" + _name + "\"") : ((_object.ID != null) ? (" " + _object.ID) : string.Empty));
                                    string _comment = QS.Fx.Attributes.Attribute.ValueOf(_object.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_comment, null);
                                    this.ToolTipText = _comment;
                                    foreach (QS.Fx.Reflection.IParameter _parameter in _object.ClassParameters.Values)
                                        this.Nodes.Add(new _Node(this._designer, _Type.Parameter, _parameter));
                                    this.Nodes.Add(new _Node(this._designer, _Type.ObjectClass, _object.ObjectClass));
                                }
                                else
                                {
                                    this.Text = "null";
                                    this.ForeColor = System.Drawing.Color.Maroon;
                                }
                            }
                            break;

                        #endregion

                        #region Endpoint

                        case _Type.Endpoint:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Endpoint;
                                this.SelectedImageIndex = (int)_ImageIndexes.Endpoint;
                                if (_data == null)
                                    throw new Exception("Endpoint cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IEndpoint))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IEndpoint).Name + "\"");
                                QS.Fx.Reflection.IEndpoint _endpoint = (QS.Fx.Reflection.IEndpoint)_data;
                                this.Text = "Endpoint \"" + _endpoint.ID + "\"";
                                this.ToolTipText = null;
                                this.Nodes.Add(new _Node(this._designer, _Type.EndpointClass, _endpoint.EndpointClass));
                            }
                            break;

                        #endregion

                        #region Parameter

                        case _Type.Parameter:
                            {
                                this.ImageIndex = (int)_ImageIndexes.Parameter;
                                this.SelectedImageIndex = (int)_ImageIndexes.Parameter;
                                this.NodeFont = _Font_Parameter;
                                if (_data == null)
                                    throw new Exception("Parameter cannot be null.");
                                if (!(_data is QS.Fx.Reflection.IParameter))
                                    throw new Exception("Expecting element of type \"" + typeof(QS.Fx.Reflection.IParameter).Name + "\"");
                                QS.Fx.Reflection.IParameter _parameter = (QS.Fx.Reflection.IParameter)_data;
                                this.Text = "Parameter \"" + _parameter.ID + "\"";
                                this.ToolTipText = null;
                                switch (_parameter.ParameterClass)
                                {
                                    case QS.Fx.Reflection.ParameterClass.ValueClass:
                                        this.Nodes.Add(new _Node(this._designer, _Type.ValueClass, _parameter.Value));
                                        break;

                                    case QS.Fx.Reflection.ParameterClass.InterfaceClass:
                                        this.Nodes.Add(new _Node(this._designer, _Type.InterfaceClass, _parameter.Value));
                                        break;

                                    case QS.Fx.Reflection.ParameterClass.EndpointClass:
                                        this.Nodes.Add(new _Node(this._designer, _Type.EndpointClass, _parameter.Value));
                                        break;

                                    case QS.Fx.Reflection.ParameterClass.ObjectClass:
                                        this.Nodes.Add(new _Node(this._designer, _Type.ObjectClass, _parameter.Value));
                                        break;

                                    case QS.Fx.Reflection.ParameterClass.Value:
                                        {
                                            this.Nodes.Add(new _Node(this._designer, _Type.ValueClass, _parameter.ValueClass));
                                            if (_parameter.ValueClass.ID.Equals(new QS.Fx.Base.ID(QS.Fx.Reflection.ValueClasses._Object)))
                                                this.Nodes.Add(new _Node(this._designer, _Type.Object, _parameter.Value));
                                            else
                                                this.Nodes.Add(new _Node(this._designer, _Type.Value, _parameter.Value));
                                        }
                                        break;
                                }
                            }
                            break;

                        #endregion
*/

                        #region Unrecognized

                        default:
                            {
                                this.Text = "unrecognized element";
                                this.ImageIndex = (int)_ImageIndexes.Unrecognized;
                                this.SelectedImageIndex = (int)_ImageIndexes.Unrecognized;
                                throw new Exception("Unrecognized element type: \"" + _type.ToString() + "\".");
                            }

                        #endregion
                    }
                }
                catch (Exception _exc)
                {
                    this.ForeColor = System.Drawing.Color.Gray;
                    this.ToolTipText = _exc.ToString();
                }
            }

            #endregion

            #region _Drag ***********************************************************

            public void _Drag()
            {
                switch (this._type)
                {
                    case _Type.Object:
                        {
/*
                            DataObject _dataobject = new DataObject();
                            _dataobject.SetData(DataFormats.Text, "Here we are dragging the XML definition of " + this.Text + ".");
                            this.TreeView.DoDragDrop(_dataobject, DragDropEffects.Copy);
*/ 
                        }
                        break;

                    case _Type.Value:
                        {
                            DataObject _dataobject = new DataObject();
                            _dataobject.SetData(DataFormats.Text, _data.ToString());
                            this.TreeView.DoDragDrop(_dataobject, DragDropEffects.Copy);
                        }
                        break;

                    default:
                        break;
                }
            }

            #endregion

            #region _Over

            public void _Over(DragEventArgs e)
            {
                switch (this._type)
                {
                    case _Type.Object:
                    case _Type.Value:
                        {
                            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.Text, true))
                                e.Effect = DragDropEffects.All;
                            else
                                e.Effect = DragDropEffects.None;
                        }
                        break;

                    default:
                        e.Effect = DragDropEffects.None;
                        break;
                }
            }

            #endregion

            #region _Drop (1)

            public void _Drop(DragEventArgs e)
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
                            lock (this)
                            {
                                _Drop(_text);
                            }
                        }
                    }
                    else if (e.Data.GetDataPresent(DataFormats.Text, true))
                    {
                        string _text = (string)e.Data.GetData(DataFormats.Text);
                        lock (this)
                        {
                            _Drop(_text);
                        }
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

            #region _Drop (2) **********************************************

            public void _Drop(string _text)
            {
                this.TreeView.BeginUpdate();
                switch (this._type)
                {
                    case _Type.Object:
                        {
/*
                            try
                            {
                                this._data = this._designer._loaderendpoint.Interface.Load(_text);
                                _Refresh();
                            }
                            catch (Exception _exc)
                            {
                                (new QS.Fx.Base.ExceptionForm(_exc)).Show();
                            }
*/
                        }
                        break;

                    case _Type.Value:
                        {
                            this._data = _text;
                            _Refresh();
                        }
                        break;

                    default:
                        break;
                }
                this.TreeView.EndUpdate();
                this.TreeView.Refresh();
            }

            #endregion

            #region _DoubleClick

            public void _DoubleClick()
            {
            }

            #endregion
        }

        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.Text, true))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
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
                        lock (this)
                        {
                            _Load(_text);
                        }
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text, true))
                {
                    string _text = (string)e.Data.GetData(DataFormats.Text);
                    lock (this)
                    {
                        _Load(_text);
                    }
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

        #region _TreeView_ItemDrag

        private void _TreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            _Node _node = e.Item as _Node;
            if (_node != null)
            {
                lock (this)
                {
                    _node._Drag();
                }
            }
        }

        #endregion

        #region _TreeView_DragEnter

        private void _TreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.Text, true))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        #region _TreeView_DragOver

        private void _TreeView_DragOver(object sender, DragEventArgs e)
        {
            lock (this)
            {
                TreeNode _node = treeView1.GetNodeAt(this.PointToClient(new Point(e.X, e.Y)));
                if (_node != null)
                {
                    treeView1.SelectedNode = _node;
                    _Node __node = _node as _Node;
                    if (__node != null)
                        __node._Over(e);
                }
            }
        }

        #endregion

        #region _TreeView_DragDrop

        private void _TreeView_DragDrop(object sender, DragEventArgs e)
        {
            lock (this)
            {
                _Node _node = treeView1.SelectedNode as _Node;
                if (_node != null)
                    _node._Drop(e);
            }
        }

        #endregion

        #region _TreeView_NodeMouseDoubleClick

        private void _TreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            _Node _node = e.Node as _Node;
            if (_node != null)
            {
                lock (this)
                {
                    _node._DoubleClick();
                }
            }
        }

        #endregion

        #region _Click_Button_Compile

        private void _Click_Button_Compile(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
