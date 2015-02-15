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

namespace QS._qss_x_.ObjectDesigner_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.ObjectDesigner, "ObjectDesigner", "A visual designer for live objects.")]
    public sealed partial class Designer_ : UserControl, QS.Fx.Object.Classes.IUI
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public Designer_(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("path", QS.Fx.Reflection.ParameterClass.Value)] string _path)
        {
            InitializeComponent();
            QS._qss_x_.Component_.Classes_.Components_ _components = new QS._qss_x_.Component_.Classes_.Components_(_mycontext);
            _components.Dock = DockStyle.Fill;
            this.splitContainer2.Panel2.Controls.Add(_components);

            this.panel1.MouseWheel += new MouseEventHandler(this.__on_MouseWheel);
            this._ui = _mycontext.ExportedUI(this);
            this._path = _path;
            this._loader = new Loader_(QS._qss_x_.Reflection_.Library.LocalLibrary);

            if ((this._path != null) && ((this._path = this._path.Trim()).Length > 0))
            {
                QS.Fx.Reflection.Xml.Object _o;
                using (StreamReader _reader = new StreamReader(this._path))
                {
                    _o = ((QS.Fx.Reflection.Xml.Root)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Deserialize(_reader)).Object;
                }
                QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _root = _loader._LoadObject(_o, null);
                this._Initialize(_root);                
            }
        }

        #endregion

        #region Fields

        private QS.Fx.Endpoint.Internal.IExportedUI _ui;
        private string _path;
        private Loader_ _loader;
        private QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _root;
        private PointF _p, _q, _s;
        private float _z;
        private bool _moving;
        private Point _m;
        private QS._qss_x_.ObjectDesigner_.Elements_.Element_ _selected, _pointedto;
        private List<QS._qss_x_.ObjectDesigner_.Elements_.Element_> _highlighted;

        #endregion

        #region QS.Fx.Object.Classes.IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this._ui; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        private void _Initialize(QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _root)
        {
            this._root = _root;
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(this._root);
            treeView1.EndUpdate();
            this._root._Validate();
            this._root._Rebuild();
            this._root._Recalculate_1();
            this._root._P = new PointF(- this._root._S.X / 2, - this._root._S.Y / 2);
            this._root._Recalculate_2();
            Rectangle _rec = panel1.ClientRectangle;
            this._z = Math.Min(((float)_rec.Width) / this._root._S.X, ((float)_rec.Height) / this._root._S.Y) / 2;
            this._s = new PointF(((float)_rec.Width) / this._z, ((float)_rec.Height) / this._z);
            this._p = new PointF(-this._s.X / 2, -this._s.Y / 2);
            this._q = new PointF(this._s.X / 2, this._s.Y / 2);
            this._highlighted = new List<QS._qss_x_.ObjectDesigner_.Elements_.Element_>();
            this._selected = null;
            this.Refresh();
        }

        #endregion

        #region _Refresh

        private void _Refresh(bool _recalculate)
        {
            this._root._Validate();
            this._root._Rebuild();
            if (_recalculate)
            {
                this._root._Recalculate_1();
                this._root._Recalculate_2();
            }
            this.Refresh();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region __my_SelectionChanged

        private void __my_SelectionChanged(Elements_.Element_ _element)
        {
            if (this._selected != null)
            {
                this._selected._Selected = false;
                if (this._highlighted != null)
                {
                    foreach (Elements_.Element_ _e in this._highlighted)
                        _e._Highlighted = false;
                    this._highlighted.Clear();
                }
            }
            this._selected = _element;
            if (_element != null)
            {
                _element._Selected = true;
                IEnumerable<Elements_.Element_> _highlighted = _element._Highlight();
                if (_highlighted != null)
                {
                    this._highlighted.AddRange(_highlighted);
                    foreach (Elements_.Element_ _e in _highlighted)
                    {
                        _e._Highlighted = true;
                        _e.EnsureVisible();
                    }
                }
            }
        }

        #endregion

        #region __my_ZoomInOnElement

        private void __my_ZoomInOnElement(Elements_.Element_ _element)
        {
            Rectangle _rec = panel1.ClientRectangle;
            this._z = Math.Min(((float)_rec.Width) / _element._S.X, ((float)_rec.Height) / _element._S.Y) / 2;
            this._s = new PointF(((float)_rec.Width) / this._z, ((float)_rec.Height) / this._z);
            PointF _c = new PointF((_element._P.X + _element._Q.X) / 2, (_element._P.Y + _element._Q.Y) / 2);
            this._p = new PointF(_c.X - this._s.X / 2, _c.Y - this._s.Y / 2);
            this._q = new PointF(this._p.X + this._s.X, this._p.Y + this._s.Y);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region __on_Paint

        private void __on_Paint(object sender, PaintEventArgs e)
        {
            Graphics _g = e.Graphics;
            RectangleF _c = e.ClipRectangle;
            _g.FillRectangle(Brushes.White, _c);
            try
            {
                if ((_z > 0) && (this._root != null))
                {
                    _root._Draw(_g, this._p, this._q, this._z);
                }
            }
            catch (Exception _exc)
            {
                _g.DrawString(_exc.ToString(), new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular), Brushes.Red, _c);
            }
        }

        #endregion

        #region __on_Resize

        private void __on_Resize(object sender, EventArgs e)
        {
            lock (this)
            {
                Rectangle _rec = panel1.ClientRectangle;
                this._z = Math.Min(((float)_rec.Width) / this._s.X, ((float)_rec.Height) / this._s.Y);
                this._s = new PointF(((float)_rec.Width) / this._z, ((float)_rec.Height) / this._z);
                this._p = new PointF((this._p.X + this._q.X - this._s.X) / 2, (this._p.Y + this._q.Y - this._s.Y) / 2);
                this._q = new PointF(this._p.X + this._s.X, this._p.Y + this._s.Y);
                panel1.Refresh();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region __on_MouseWheel

        private void __on_MouseWheel(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                float _d = (float) Math.Pow(1.05, (((double) e.Delta) / (120.0)));
                this._z = this._z * _d;
                float _m = (1 - 1 / _d) / 2;
                this._p = new PointF(this._p.X + this._s.X * _m, this._p.Y + this._s.Y * _m);
                this._q = new PointF(this._q.X - this._s.X * _m, this._q.Y - this._s.Y * _m);
                this._s = new PointF(this._q.X - this._p.X, this._q.Y - this._p.Y);
                panel1.Refresh();
            }
        }

        #endregion

        #region __on_MouseEnter

        private void __on_MouseEnter(object sender, EventArgs e)
        {
        }

        #endregion

        #region __on_MouseDown

        private void __on_MouseDown(object sender, MouseEventArgs e)
        {
            panel1.Focus();
            lock (this)
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    this._moving = true;
                    this._m = e.Location;
                }
                if (this._root != null)
                {
                    Rectangle _rec = panel1.ClientRectangle;
                    PointF _m = new PointF(
                        this._p.X + this._s.X * ((float)(e.X - _rec.X)) / ((float)_rec.Width),
                        this._p.Y + this._s.Y * ((float)(e.Y - _rec.Y)) / ((float)_rec.Height));
                    Elements_.Element_ _element = this._root._Click(_m);
                    if (!ReferenceEquals(_element, this._selected))
                    {
                        this.__my_SelectionChanged(_element);
                        treeView1.SelectedNode = _element;
                        this.Refresh();
                    }
                }
            }
        }

        #endregion

        #region __on_MouseMove

        private void __on_MouseMove(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                Point _m = e.Location;
                if (this._moving)
                {
                    float _dx = (_m.X - this._m.X) / this._z;
                    float _dy = (_m.Y - this._m.Y) / this._z;
                    this._m = _m;
                    this._p = new PointF(this._p.X - _dx, this._p.Y - _dy);
                    this._q = new PointF(this._q.X - _dx, this._q.Y - _dy);
                    panel1.Refresh();
                }
                else
                {
                    Elements_.Element_ _element;
                    if (this._root != null)
                    {
                        Rectangle _rec = panel1.ClientRectangle;
                        PointF _mm = new PointF(
                            this._p.X + this._s.X * ((float)(e.X - _rec.X)) / ((float)_rec.Width),
                            this._p.Y + this._s.Y * ((float)(e.Y - _rec.Y)) / ((float)_rec.Height));
                        _element = this._root._Click(_mm);
                    }
                    else
                        _element = null;
                    if (_element != this._pointedto)
                    {
                        this._pointedto = _element;
                        if (_element != null)
                        {
                            StringBuilder _ss = new StringBuilder();
                            _ss.AppendLine(_element.Text);
                            string _comment = _element._CreateComment();
                            if (_comment != null)
                                _ss.AppendLine(_comment);
                            string _error = _element._Error;
                            if (_error != null)
                            {
                                _ss.AppendLine("errors:");
                                _ss.AppendLine(_error);
                            }
                            string _tooltiptext = _ss.ToString();
                            toolTip1.SetToolTip(panel1, _tooltiptext);
                        }
                        else
                        {
                            toolTip1.SetToolTip(panel1, null);
                        }
                        this.Refresh();
                    }
                }
            }
        }

        #endregion

        #region __on_MouseUp

        private void __on_MouseUp(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                this._moving = false;
            }
        }

        #endregion

        #region __on_MouseLeave

        private void __on_MouseLeave(object sender, EventArgs e)
        {
            lock (this)
            {
                this._moving = false;
            }
        }

        #endregion

        #region __on_MouseHover

        private void __on_MouseHover(object sender, EventArgs e)
        {
/*
            lock (this)
            {
                if (this._root != null)
                {
                    Rectangle _rec = panel1.ClientRectangle;
                    Point _mm = panel1.PointToClient(MousePosition);
                    PointF _m = new PointF(
                        this._p.X + this._s.X * ((float)(_mm.X - _rec.X)) / ((float)_rec.Width),
                        this._p.Y + this._s.Y * ((float)(_mm.Y - _rec.Y)) / ((float)_rec.Height));
                    Elements_.Element_ _element = this._root._Click(_m);
                    if (_element != null)
                    {
                        StringBuilder _ss = null;
                        string _comment = _element._CreateComment();
                        if (_comment != null)
                        {
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine(_comment);
                        }
                        string _error = _element._Error;
                        if (_error != null)
                        {
                            if (_ss == null)
                                _ss = new StringBuilder();
                            _ss.AppendLine("errors:");
                            _ss.AppendLine(_error);
                        }
                        if (_ss != null)
                        {
                            string _tooltiptext = _ss.ToString();
                            toolTip1.SetToolTip(panel1, _tooltiptext);
                            toolTip1.Active = true;
                        }
                        else
                        {
                            // toolTip1.Hide(panel1);
                        }
                    }
                    else
                    {
                        // toolTip1.Hide(panel1);
                    }
                }
                else
                {
                    // toolTip1.Hide(panel1);
                }
            }
*/
        }

        #endregion

        #region __on_DoubleClick

        private void __on_DoubleClick(object sender, EventArgs e)
        {
            lock (this)
            {
                if (this._selected != null)
                {
                    // __my_ZoomInOnElement(this._selected);
                    if (this._selected is Elements_.Element_Object_)
                    {
                        Elements_.Element_Object_ _o = (Elements_.Element_Object_)this._selected;
                        PointF _p1 = _o._Reference._P;
                        _o._DoubleClick();
                        this._root._Recalculate_1();
                        this._root._Recalculate_2();
                        PointF _p2 = _o._Reference._P;
                        PointF _movement = new PointF(_p2.X - _p1.X, _p2.Y - _p1.Y);
                        this._p = new PointF(this._p.X + _movement.X, this._p.Y + _movement.Y);
                        this._q = new PointF(this._q.X + _movement.X, this._q.Y + _movement.Y);
                        panel1.Refresh();
                    }
                }
            }
        }

        #endregion 

        #region __on_tree_MouseDown

        private void __on_tree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                lock (this)
                {
                    Point _p1 = new Point(e.X, e.Y);
                    Elements_.Element_ _element = (Elements_.Element_)treeView1.GetNodeAt(_p1);
                    if (_element != null)
                    {
                        treeView1.SelectedNode = _element;
                        if (_element != null)
                        {
                            IEnumerable<Elements_.Element_Action_> _actions = _element._Menu();
                            if (_actions != null)
                            {
                                Point _p2 = treeView1.PointToScreen(_p1);
                                Point _p3 = PointToClient(_p2);
                                ContextMenuStrip _menu = this.contextMenuStrip1;
                                _menu.Items.Clear();
                                foreach (Elements_.Element_Action_ _action in _actions)
                                    _menu.Items.Add(new MyMenuItem_(this, _action));
                                _menu.Show(this, _p3);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region __on_tree_AfterSelect

        private void __on_tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lock (this)
            {
                Elements_.Element_ _element = (Elements_.Element_)treeView1.SelectedNode;
                if (!ReferenceEquals(_element, this._selected))
                {
                    this.__my_SelectionChanged(_element);
                    this.Refresh();
                }
            }
        }

        #endregion

        #region __on_tree_DoubleClick

        private void __on_tree_DoubleClick(object sender, EventArgs e)
        {
        }

        #endregion

        #region __on_tree_NodeMouseHover

        private void __on_tree_NodeMouseHover(object sender, TreeNodeMouseHoverEventArgs e)
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region __on_DragEnter

        private void __on_DragEnter(object sender, DragEventArgs e)
        {
            __on_DragOver(sender, e);
        }

        #endregion

        #region __on_DragOver

        private void __on_DragOver(object sender, DragEventArgs e)
        {
            lock (this)
            {
                Elements_.Element_ _element = null;
                if (this._root != null)
                {
                    Point ee = panel1.PointToClient(new Point(e.X, e.Y));
                    Rectangle _rec = panel1.ClientRectangle;
                    PointF _m = new PointF(
                        this._p.X + this._s.X * ((float)(ee.X - _rec.X)) / ((float)_rec.Width),
                        this._p.Y + this._s.Y * ((float)(ee.Y - _rec.Y)) / ((float)_rec.Height));
                    _element = this._root._Click(_m);
                }
                else
                    _element = null;
                if (this._DropOk(_element, e.Data))
                    e.Effect = DragDropEffects.All;
                else
                {
                    _element = null;
                    e.Effect = DragDropEffects.None;
                }
                if (!ReferenceEquals(_element, this._selected))
                {
                    this.__my_SelectionChanged(_element);
                    treeView1.SelectedNode = _element;
                    this.Refresh();
                }
            }
        }

        #endregion

        #region __on_DragDrop

        private void __on_DragDrop(object sender, DragEventArgs e)
        {
            lock (this)
            {
                this._Drop(this._selected, e.Data);
            }
        }

        #endregion

        #region __on_DragLeave

        private void __on_DragLeave(object sender, EventArgs e)
        {
            lock (this)
            {
                if (!ReferenceEquals(null, this._selected))
                {
                    this.__my_SelectionChanged(null);
                    treeView1.SelectedNode = null;
                    this.Refresh();
                }
            }
        }

        #endregion

        #region __on_tree_DragEnter

        private void __on_tree_DragEnter(object sender, DragEventArgs e)
        {
            __on_tree_DragOver(sender, e);
        }

        #endregion

        #region __on_tree_DragOver

        private void __on_tree_DragOver(object sender, DragEventArgs e)
        {
            lock (this)
            {
                Point ee = treeView1.PointToClient(new Point(e.X, e.Y));
                Elements_.Element_ _element = (Elements_.Element_) treeView1.GetNodeAt(ee.X, ee.Y);
                if (this._DropOk(_element, e.Data))
                    e.Effect = DragDropEffects.All;
                else
                {
                    _element = null;
                    e.Effect = DragDropEffects.None;
                }
                if (!ReferenceEquals(_element, this._selected))
                {
                    this.__my_SelectionChanged(_element);
                    this.Refresh();
                }
            }
        }

        #endregion

        #region __on_tree_DragDrop

        private void __on_tree_DragDrop(object sender, DragEventArgs e)
        {
            lock (this)
            {
                this._Drop(this._selected, e.Data);
            }      
        }

        #endregion

        #region __on_tree_DragLeave

        private void __on_tree_DragLeave(object sender, EventArgs e)
        {
            lock (this)
            {
                if (!ReferenceEquals(null, this._selected))
                {
                    this.__my_SelectionChanged(null);
                    this.Refresh();
                }
            }
        }

        #endregion

        #region __on_tree_ItemDrag

        private void __on_tree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                Elements_.Element_ _element = (Elements_.Element_)e.Item;
                if ((_element != null) && _element._Correct)
                {
                    if (_element is Elements_.Element_Object_)
                    {
                        Elements_.Element_Object_ _element_object = (Elements_.Element_Object_)_element;
                        QS.Fx.Reflection.Xml.Object _xml_object = _element_object._Serialize();
                        QS.Fx.Reflection.Xml.Root _xml_root = new QS.Fx.Reflection.Xml.Root(_xml_object);
                        StringBuilder _ss = new StringBuilder();
                        using (StringWriter _writer = new StringWriter(_ss))
                        {
                            (new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Serialize(_writer, _xml_root);
                        }
                        string _objectxml = _ss.ToString();
                        DataObject _dataobject = new DataObject();
                        _dataobject.SetData(DataFormats.Text, _objectxml);
                        _dataobject.SetData(DataFormats.UnicodeText, _objectxml);
                        string _name_1;
                        QS.Fx.Attributes.IAttribute _nameattribute;
                        if ((_element_object._Attributes != null) && (_element_object._Attributes.Get(QS.Fx.Attributes.AttributeClasses.CLASS_name, out _nameattribute)))
                            _name_1 = _nameattribute.Value;
                        else if (_element_object._ID != null)
                            _name_1 = _element_object._ID;
                        else
                            _name_1 = string.Empty;
                        StringBuilder _name_2 = new StringBuilder();
                        for (int _k = 0; _k < _name_1.Length; _k++)
                        {
                            char c = _name_1[_k];
                            if (char.IsLetterOrDigit(c))
                                _name_2.Append(c);
                            else
                                _name_2.Append("_");
                        }
                        string _name_3 = _name_2.ToString();
                        string _tempfile;
                        do
                        {
                            _tempfile = Path.GetTempFileName();
                            _tempfile = Path.GetDirectoryName(_tempfile) + Path.DirectorySeparatorChar + _name_3 + "_" +
                                Path.GetFileNameWithoutExtension(_tempfile) + ".liveobject";

                        }
                        while (File.Exists(_tempfile));
                        using (StreamWriter writer = new StreamWriter(_tempfile, false))
                        {
                            writer.WriteLine(_objectxml);
                        }
                        System.Collections.Specialized.StringCollection _c = new System.Collections.Specialized.StringCollection();
                        _c.Add(_tempfile);
                        _dataobject.SetFileDropList(_c);
                        DoDragDrop(_dataobject, DragDropEffects.Move);
                    }
                }
            }
            catch (Exception _exc)
            {
                (new QS._qss_x_.Base1_.ExceptionForm(_exc)).ShowDialog();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _DropOk

        private bool _DropOk(Elements_.Element_ _element, IDataObject _dataobject)
        {
            if (_dataobject.GetDataPresent(DataFormats.FileDrop, false) ||
                _dataobject.GetDataPresent(DataFormats.Text, false) ||
                _dataobject.GetDataPresent(DataFormats.UnicodeText, false))
            {
                if (_element != null)
                    return _element._DropOk(Elements_.Category_.Object_);
                else
                    return true;
            }
            else if (_dataobject.GetDataPresent(typeof(QS._qss_x_.Reflection_.draggable_library_object_)))
            {
                QS._qss_x_.Reflection_.draggable_library_object_ _o =
                    (QS._qss_x_.Reflection_.draggable_library_object_)_dataobject.GetData(
                        typeof(QS._qss_x_.Reflection_.draggable_library_object_));
                if (_element != null)
                {
                    switch (_o._Category)
                    {
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.valueclass_:
                            return _element._DropOk(Elements_.Category_.ValueClass_);
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.interfaceclass_:
                            return _element._DropOk(Elements_.Category_.InterfaceClass_);
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.endpointclass_:
                            return _element._DropOk(Elements_.Category_.EndpointClass_);
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.objectclass_:
                            return _element._DropOk(Elements_.Category_.ObjectClass_);
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.object_:
                            return _element._DropOk(Elements_.Category_.Object_);
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.ordinaryobject_:
                            return _element._DropOk(Elements_.Category_.OrdinaryObject_);
                        default:
                            throw new NotImplementedException();
                    }                    
                }
                else
                    return (_o._Category == QS._qss_x_.Reflection_.draggable_library_object_.category_.object_);
            }
            else
                return false;
        }

        #endregion

        #region _Drop

        private void _Drop(Elements_.Element_ _element, IDataObject _dataobject)
        {            
            try
            {
                if (_dataobject.GetDataPresent(typeof(QS._qss_x_.Reflection_.draggable_library_object_)))
                {
                    QS._qss_x_.Reflection_.draggable_library_object_ _o =
                        (QS._qss_x_.Reflection_.draggable_library_object_) _dataobject.GetData(
                            typeof(QS._qss_x_.Reflection_.draggable_library_object_));
                    Elements_.Category_ _category;
                    Elements_.Element_ _dropped;
                    bool _recalculate = false;
                    switch (_o._Category)
                    {
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.valueclass_:
                            {
                                QS.Fx.Reflection.IValueClass _valueclass = (QS.Fx.Reflection.IValueClass) _o._Object;
                                _category = QS._qss_x_.ObjectDesigner_.Elements_.Category_.ValueClass_;
                                _dropped = this._loader._LoadValueClass(_valueclass, null, false);
                            }
                            break;
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.interfaceclass_:
                            {
                                QS.Fx.Reflection.IInterfaceClass _interfaceclass = (QS.Fx.Reflection.IInterfaceClass)_o._Object;
                                _category = QS._qss_x_.ObjectDesigner_.Elements_.Category_.InterfaceClass_;
                                _dropped = this._loader._LoadInterfaceClass(_interfaceclass, null, false);
                            }
                            break;
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.endpointclass_:
                            {
                                QS.Fx.Reflection.IEndpointClass _endpointclass = (QS.Fx.Reflection.IEndpointClass)_o._Object;
                                _category = QS._qss_x_.ObjectDesigner_.Elements_.Category_.EndpointClass_;
                                _dropped = this._loader._LoadEndpointClass(_endpointclass, null, false);
                            }
                            break;
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.objectclass_:
                            {
                                QS.Fx.Reflection.IObjectClass _objectclass = (QS.Fx.Reflection.IObjectClass)_o._Object;
                                _category = QS._qss_x_.ObjectDesigner_.Elements_.Category_.ObjectClass_;
                                _dropped = this._loader._LoadObjectClass(_objectclass, null, false);
                            }
                            break;
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.object_:
                            {
                                QS.Fx.Reflection.IObject _object = (QS.Fx.Reflection.IObject)_o._Object;
                                _category = QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_;
                                _dropped = this._loader._LoadObject(_object);
                                _recalculate = true;
                            }
                            break;
                        case QS._qss_x_.Reflection_.draggable_library_object_.category_.ordinaryobject_:
                        default:
                            throw new NotImplementedException();
                    }
                    if (_element != null)
                    {
                        _element._Drop(_category, _dropped);
                        this._Refresh(_recalculate);
                    }
                    else if (_category == QS._qss_x_.ObjectDesigner_.Elements_.Category_.Object_)
                    {
                        _root = (QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_)_dropped;
                        this._Initialize(_root);
                    }
                }
                else
                {
                    string _text = null;
                    if (_dataobject.GetDataPresent(DataFormats.FileDrop, false))
                    {
                        string[] _filenames = (string[]) _dataobject.GetData(DataFormats.FileDrop);
                        if (_filenames.Length == 1)
                        {
                            using (StreamReader _streamreader = new StreamReader(_filenames[0]))
                            {
                                _text = _streamreader.ReadToEnd();
                            }
                        }
                    }
                    else if (_dataobject.GetDataPresent(DataFormats.Text, false))
                    {
                        _text = (string)_dataobject.GetData(DataFormats.Text);
                    }
                    else if (_dataobject.GetDataPresent(DataFormats.UnicodeText, false))
                    {
                        _text = (string)_dataobject.GetData(DataFormats.UnicodeText);
                    }
                    if (_text != null)
                    {
                        QS.Fx.Reflection.Xml.Object _x;
                        using (StringReader _reader = new StringReader(_text))
                        {
                            _x = ((QS.Fx.Reflection.Xml.Root)(new XmlSerializer(typeof(QS.Fx.Reflection.Xml.Root))).Deserialize(_reader)).Object;
                        }
                        QS._qss_x_.ObjectDesigner_.Elements_.Element_Object_ _dropped = _loader._LoadObject(_x, null);
                        if (_element != null)
                        {
                            _element._Drop(Elements_.Category_.Object_, _dropped);
                            this._Refresh(true);
                        }
                        else
                        {
                            _root = _dropped;
                            this._Initialize(_root);
                        }
                    }
                    else
                        throw new Exception("The drag and drop operation cannot continue because the data is not in a recognized format.");
                }
            }
            catch (Exception _exc)
            {
                (new QS._qss_x_.Base1_.ExceptionForm(_exc)).ShowDialog();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class MyMenuItem_

        private sealed class MyMenuItem_ : ToolStripMenuItem
        {
            public MyMenuItem_(Designer_ _owner, Elements_.Element_Action_ _action) : base(_action._ID)
            {
                this._owner = _owner;
                this._action = _action;
                this.Click += new EventHandler(_MyMenuItem_Click);
            }

            void _MyMenuItem_Click(object sender, EventArgs e)
            {
                _owner._ActionCallback(_action);
            }

            private Designer_ _owner;
            private Elements_.Element_Action_ _action;
        }

        #endregion

        #region _ActionCallback

        private void _ActionCallback(Elements_.Element_Action_ _action)
        {
            lock (this)
            {
                _action._Callback(_action._Context);
                this.Refresh();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
