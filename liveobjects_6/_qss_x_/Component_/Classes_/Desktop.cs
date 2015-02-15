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
using Isis;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Desktop, "Desktop", "A shared desktop.")]
    public sealed partial class Desktop
        : Viewfinder, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                QS.Fx.Value.Classes.Desktop.IDesktopState>
    {
        #region Constructor
        
        delegate void stringArg(string who, string val);
        Group myGroup;
        int UPDATE = 1;
        QS.Fx.Base.ID _new_id;
        bool exists;

        public Desktop(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("background_color", 
                QS.Fx.Reflection.ParameterClass.Value)] string _background_color, 
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                        QS.Fx.Value.Classes.Desktop.IDesktopState>>
                    _channel,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IService<
                        QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> 
                    _loader)
            : base(_mycontext, _loader)
        {
            IsisSystem.Start();

            myGroup = Isis.Group.Lookup("Circuit");
            if (myGroup == null)
                myGroup = new Group("Circuit");

            myGroup.Handlers[UPDATE] += (stringArg)delegate(string name, string val)
            {
                if (val.StartsWith("newx"))
                {
                    string _objectxml = "";
                    _objectxml = val.Substring(4);
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_n;
                    
                    if (_LoadObject(_objectxml, out _objectref_n))
                    {
                        string _label_n = QS.Fx.Attributes.Attribute.ValueOf(
                _objectref_n.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref_n.ID);
                        exists = false;
                        foreach (_EmbeddedObject obj in this._embeddedobjects.Values)
                        {
                            if (obj.Label.CompareTo(_label_n) == 0)
                            {
                                exists = true;
                                if (_ContainerControl.Controls[_label_n].Visible)
                                {
                                    _ContainerControl.Controls[_label_n].Hide();
                                }
                                else
                                {
                                    _ContainerControl.Controls[_label_n].Show();
                                }
                            }
                        }
                        if (!exists)
                        {
                            _DropObject(108, 108, _objectxml, _objectref_n);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error loading object!");
                    }
                }
                if (val.StartsWith("newf"))
                {
                    string _objectxml = "";
                    string fname = val.Substring(4);
                    using (StreamReader _streamreader = new StreamReader(fname))
                    {
                        _objectxml = _streamreader.ReadToEnd();
                    }
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_n;
                    if (_LoadObject(_objectxml, out _objectref_n))
                    {
                        _DropObject(108, 108, _objectxml, _objectref_n);
                    }
                    else
                    {
                        MessageBox.Show("Error loading object!");
                    }
                }
                /*if (val.CompareTo("remove") == 0)
                {
                    r = true;
                    string _objectxml = "";
                    using (StreamReader _streamreader = new StreamReader("C:\\liveobjects\\examples\\image_1.liveobject"))
                    {
                        _objectxml = _streamreader.ReadToEnd();
                    }
                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref_n;
                    if (_LoadObject(_objectxml, out _objectref_n))
                    {
                        _DropObject(108, 108, _objectxml, _objectref_n);
                    }
                    else
                    {
                        MessageBox.Show("NULL");
                    }
                }*/
            };
            myGroup.Join();

            InitializeComponent();

            this._mycontext = _mycontext;
            this._BackgroundColor = System.Drawing.Color.FromName(_background_color);

            if (_channel == null)
                throw new Exception("Folder view cannot run without the attached channel.");

            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                    QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                    QS.Fx.Value.Classes.Desktop.IDesktopState>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                    QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                    QS.Fx.Value.Classes.Desktop.IDesktopState>>(this);

            lock (this)
            {
                this._channelconnection =
                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);
            }

            
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                QS.Fx.Value.Classes.Desktop.IDesktopState>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                QS.Fx.Value.Classes.Desktop.IDesktopState>>
            _channelendpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        [QS.Fx.Base.Inspectable]
        private IDictionary<QS.Fx.Base.ID, _EmbeddedObject> _embeddedobjects = new Dictionary<QS.Fx.Base.ID, _EmbeddedObject>();

        [QS.Fx.Base.Inspectable]
        private _EmbeddedObject _selected_embeddedobject;

        private Queue<object> _queue = new Queue<object>();

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _DeselectObjects

        protected override void _DeselectObjects()
        {
            lock (this)
            {
                if (this._selected_embeddedobject != null)
                    this._selected_embeddedobject._Deselect();
                this._selected_embeddedobject = null;
            }
        }

        #endregion

        #region _SelectObject

        private void _SelectObject(_EmbeddedObject _embeddedobject)
        {
            lock (this)
            {
                if (_embeddedobject != this._selected_embeddedobject)
                {
                    if (this._selected_embeddedobject != null)
                        this._selected_embeddedobject._Deselect();
                    this._selected_embeddedobject = _embeddedobject;
                    this._selected_embeddedobject._Select();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class _EmbeddedObject

        private sealed class _EmbeddedObject : UserControl
        {
            #region Constructor

            public _EmbeddedObject(
                QS.Fx.Object.IContext _mycontext,
                Desktop _desktop,
                double _x1, double _y1, double _x2, double _y2, QS.Fx.Base.ID _id, string _label, string _xmlspecification,
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _objectref,String name="Circuit")
            {
                this._mycontext = _mycontext;
                this._desktop = _desktop;
                this._x1 = _x1;
                this._y1 = _y1;
                this._x2 = _x2;
                this._y2 = _y2;
                this._id = _id;
                this._label = _label;
                this._xmlspecification = _xmlspecification;
                this._objectref = _objectref;
                this._object = null;
                this.Name = name;
            }

            #endregion

            #region Fields

            private QS.Fx.Object.IContext _mycontext;
            private Desktop _desktop;
            private QS.Fx.Base.ID _id;
            private double _x1, _y1, _x2, _y2;
            private string _label, _xmlspecification;
            private QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _objectref;
            private QS.Fx.Object.Classes.IUI _object;
            private int _physical_x1, _physical_y1, _physical_x2, _physical_y2;
            private System.Windows.Forms.Label _labelcontrol;
            private int _border_width;
            private _Resizing_Mode _resizing_mode;
            private int _mouse_x0, _mouse_y0;

            [QS.Fx.Base.Inspectable("uiendpoint")]
            public QS.Fx.Endpoint.Internal.IImportedUI _uiendpoint;

            [QS.Fx.Base.Inspectable("uiconnection")]
            public QS.Fx.Endpoint.IConnection _uiconnection;

            #endregion

            #region Enum _Resizing_Mode

            [Flags]
            private enum _Resizing_Mode
            {
                None = 0x0, Left = 0x1, Right = 0x2, Top = 0x4, Bottom = 0x8
            }

            #endregion

            #region Constants

            private const int _BORDER_WIDTH_1 = 1;
            private const int _BORDER_WIDTH_2 = 10;

            #endregion

            #region Accessors

            public double X1
            {
                get { return _x1; }
                set { _x1 = value; }
            }

            public double Y1
            {
                get { return _y1; }
                set { _y1 = value; }
            }

            public double X2
            {
                get { return _x2; }
                set { _x2 = value; }
            }

            public double Y2
            {
                get { return _y2; }
                set { _y2 = value; }
            }

            public QS.Fx.Base.ID ID
            {
                get { return _id; }
                set { _id = value; }
            }

            public string Label
            {
                get { return _label; }
                set { _label = value; }
            }

            public string ObjectXml
            {
                get { return _xmlspecification; }
                set { _xmlspecification = value; }
            }

            public QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> ObjectRef
            {
                get { return _objectref; }
                set { _objectref = value; }
            }

            public QS.Fx.Object.Classes.IUI Object
            {
                get { return _object; }
                set { _object = value; }
            }

            #endregion

            #region _CalculateCoordinates_LogicalToPhysical

            public void _CalculateCoordinates_LogicalToPhysical()
            {
                _desktop._LogicalToPhysical(_x1, _y2, out _physical_x1, out _physical_y1);
                _desktop._LogicalToPhysical(_x2, _y1, out _physical_x2, out _physical_y2);
                if (_object != null)
                    _MovePhysical();
            }

            #endregion

            #region _Load

            public void _Load()
            {
                lock (this._desktop)
                {
                    if (_object != null)
                        throw new Exception("The object is already loaded."); 
                    this._object = _objectref.Dereference(_mycontext);
                    this._uiendpoint = _mycontext.ImportedUI(this);
                    this._uiconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._uiendpoint).Connect(_object.UI);
                    this._border_width = _BORDER_WIDTH_1;
                    this.BackColor = System.Drawing.Color.Black;
                    this._labelcontrol = new System.Windows.Forms.Label();
                    this._labelcontrol.BackColor = System.Drawing.Color.Ivory;
                    this._labelcontrol.Name = "label";
                    this._labelcontrol.Text = this._label;
                    this.Name = this._label;
                    this._labelcontrol.MouseDown += new MouseEventHandler(this._Label_MouseDown);
                    this._labelcontrol.MouseUp += new MouseEventHandler(this._Label_MouseUp);
                    this._labelcontrol.MouseMove += new MouseEventHandler(this._Label_MouseMove);
                    this._labelcontrol.Hide();
                    _desktop._ContainerControl.Controls.Add(this);
                    _desktop._ContainerControl.Controls.Add(this._labelcontrol);
                    _MovePhysical();
                    this.MouseDown += new MouseEventHandler(this._MouseDown);
                    this.MouseUp += new MouseEventHandler(this._MouseUp);
                    this.MouseMove += new MouseEventHandler(this._MouseMove);
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Select

            public void _Select()
            {
                this._border_width = _BORDER_WIDTH_2;
                this._MovePhysical();
            }

            #endregion

            #region _Deselect

            public void _Deselect()
            {
                this._border_width = _BORDER_WIDTH_1;
                this._MovePhysical();
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Label_MouseDown

            private void _Label_MouseDown(object sender, EventArgs e)
            {
                _desktop._SelectObject(this);
            }

            #endregion

            #region _Label_MouseMove

            private void _Label_MouseMove(object sender, MouseEventArgs e)
            {
            }

            #endregion

            #region _Label_MouseUp

            private void _Label_MouseUp(object sender, MouseEventArgs e)
            {
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _MouseDown

            private void _MouseDown(object sender, MouseEventArgs e)
            {
                _desktop._SelectObject(this);

                lock (this)
                {
                    this._resizing_mode = _Resizing_Mode.None;
                    
                    int _distance = this._border_width;
                    
                    if (e.X < _distance)
                        this._resizing_mode |= _Resizing_Mode.Left;
                    
                    if (e.Y < _distance)
                        this._resizing_mode |= _Resizing_Mode.Top;
                        _distance = e.Y;
                    
                    if (this.Width - e.X < _distance)
                        this._resizing_mode |= _Resizing_Mode.Right;
                    
                    if (this.Height - this._labelcontrol.Height - e.Y < _distance)
                        this._resizing_mode |= _Resizing_Mode.Bottom;
                    
                    if (this._resizing_mode != _Resizing_Mode.None)
                    {
                        this._mouse_x0 = e.X;
                        this._mouse_y0 = e.Y;
                    }
                }
            }

            #endregion

            #region _MouseMove

            private void _MouseMove(object sender, MouseEventArgs e)
            {
                lock (this)
                {
                    if (this._resizing_mode != _Resizing_Mode.None)
                    {
                        int _dx = e.X - this._mouse_x0;
                        int _dy = e.Y - this._mouse_y0;

                        this._mouse_x0 = e.X;
                        this._mouse_y0 = e.Y;

                        if ((this._resizing_mode & _Resizing_Mode.Left) == _Resizing_Mode.Left)
                            this._x1 += _dx / _desktop._Zoom_X;

                        if ((this._resizing_mode & _Resizing_Mode.Right) == _Resizing_Mode.Right)
                            this._x2 += _dx / _desktop._Zoom_X;

                        if ((this._resizing_mode & _Resizing_Mode.Top) == _Resizing_Mode.Top)
                            this._y2 -= _dy / _desktop._Zoom_Y;

                        if ((this._resizing_mode & _Resizing_Mode.Bottom) == _Resizing_Mode.Bottom)
                            this._y1 -= _dy / _desktop._Zoom_Y;

                        _CalculateCoordinates_LogicalToPhysical();
                        this._MovePhysical();
                    }
                }
            }

            #endregion

            #region _MouseUp

            private void _MouseUp(object sender, MouseEventArgs e)
            {
                lock (this)
                {
                    this._resizing_mode = _Resizing_Mode.None;
                    this._desktop._MoveObject(this);
                }
            }

            #endregion

            // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            #region _Drag

            private void _Drag()
            {
                lock (this)
                {
                    DataObject _dataobject = new DataObject();
                    _dataobject.SetData(DataFormats.Text, "dragging... " + this._objectref.ID);
                    _desktop.DoDragDrop(_dataobject, DragDropEffects.Copy);
                }
            }

            #endregion

            #region _MovePhysical

            private void _MovePhysical()
            {
                this.Location = new Point(this._physical_x1, this._physical_y1);
                this.Size = new Size(this._physical_x2 - this._physical_x1, this._physical_y2 - this._physical_y1);

                this._uiendpoint.UI.Location = new Point(this._border_width, this._border_width);
                this._uiendpoint.UI.Size = new Size(this.Width - 2 * this._border_width, this.Height - 2 * this._border_width);

                this._labelcontrol.Left = this.Left;
                this._labelcontrol.Top = this.Bottom;
                this._labelcontrol.Width = this.Width;
            }

            #endregion

            #region _MoveLogical

            public void _MoveLogical(double _dx, double _dy)
            {
                this._x1 += _dx;
                this._x2 += _dx;
                this._y1 += _dy;
                this._y2 += _dy;
                _CalculateCoordinates_LogicalToPhysical();
                _MovePhysical();
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Initialize

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS.Fx.Value.Classes.Desktop.IDesktopOperation,
            QS.Fx.Value.Classes.Desktop.IDesktopState>.Initialize(
                QS.Fx.Value.Classes.Desktop.IDesktopState _checkpoint)
        {
            lock (this)
            {
                this._queue.Enqueue(_checkpoint);
                _Work();
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Receive

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS.Fx.Value.Classes.Desktop.IDesktopOperation,
            QS.Fx.Value.Classes.Desktop.IDesktopState>.Receive(
                QS.Fx.Value.Classes.Desktop.IDesktopOperation _operation)
        {
            lock (this)
            {
                this._queue.Enqueue(_operation);
                _Work();
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Checkpoint

        QS.Fx.Value.Classes.Desktop.IDesktopState 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS.Fx.Value.Classes.Desktop.IDesktopOperation,
                QS.Fx.Value.Classes.Desktop.IDesktopState>.Checkpoint()
        {
            lock (this)
            {
                List<QS.Fx.Value.Classes.Desktop.DesktopObject> _o = new List<QS.Fx.Value.Classes.Desktop.DesktopObject>();
                foreach (_EmbeddedObject _embeddedobject in this._embeddedobjects.Values)
                {
                    _o.Add
                    (
                        new QS.Fx.Value.Classes.Desktop.DesktopObject
                        (
                            _embeddedobject.ID,
                            _embeddedobject.Label,
                            _embeddedobject.X1, _embeddedobject.Y1,
                            _embeddedobject.X2, _embeddedobject.Y2,
                            _embeddedobject.ObjectXml)
                    );
                }
                return new QS.Fx.Value.Classes.Desktop.DesktopState(_o);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Work

        private void _Work()
        {
            lock (this)
            {
                if (this.InvokeRequired)
                    this.BeginInvoke(new QS.Fx.Base.Callback(this._Work), new object[0]);
                else
                {
                    while (this._queue.Count > 0)
                    {
                        object _e = this._queue.Dequeue();
                        try
                        {
                            if (_e is QS.Fx.Value.Classes.Desktop.IDesktopState)
                            {
                                QS.Fx.Value.Classes.Desktop.IDesktopState _checkpoint = (QS.Fx.Value.Classes.Desktop.IDesktopState)_e;
                                foreach (_EmbeddedObject _embeddedobject in this._embeddedobjects.Values)
                                {
                                    // should unload
                                }
                                this._embeddedobjects.Clear();
                                foreach (QS.Fx.Value.Classes.Desktop.IDesktopObject _o in _checkpoint.Objects)
                                {
                                    QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                                    if (this._LoadObject(_o.ObjectXml, out _objectref))
                                    {
                                        QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _uiobjectref =
                                            _objectref.CastTo<QS.Fx.Object.Classes.IUI>();

                                        _EmbeddedObject _embeddedobject =
                                            new _EmbeddedObject(
                                                _mycontext,
                                                this,
                                                _o.X1, _o.Y1,
                                                _o.X2, _o.Y2,
                                                _o.ID,
                                                _o.Label,
                                                _o.ObjectXml,
                                                _uiobjectref);

                                        _embeddedobjects.Add(_embeddedobject.ID, _embeddedobject);
                                        _embeddedobject._CalculateCoordinates_LogicalToPhysical();

                                        _embeddedobject._Load();
                                    }
                                }
                            }
                            else if (_e is QS.Fx.Value.Classes.Desktop.IDesktopOperation)
                            {
                                QS.Fx.Value.Classes.Desktop.IDesktopOperation _operation = (QS.Fx.Value.Classes.Desktop.IDesktopOperation)_e;
                                switch (_operation.OperationType)
                                {
                                    case QS.Fx.Value.Classes.Desktop.DesktopOperationType.Add:
                                        this._Operation_Add((QS.Fx.Value.Classes.Desktop.DesktopOperation_Add) _operation);
                                        break;

                                    case QS.Fx.Value.Classes.Desktop.DesktopOperationType.Move:
                                        this._Operation_Move((QS.Fx.Value.Classes.Desktop.DesktopOperation_Move) _operation);
                                        break;

                                    default:
                                        throw new NotImplementedException();
                                }
                            }
                        }
                        catch (Exception _exc)
                        {
                            this._Exception(null, _exc);
                        }
                    }
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Operation_Add

        private void _Operation_Add(QS.Fx.Value.Classes.Desktop.DesktopOperation_Add _operation_add)
        {
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
            if (this._LoadObject(_operation_add.ObjectXml, out _objectref))
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _uiobjectref = 
                    _objectref.CastTo<QS.Fx.Object.Classes.IUI>();

                _EmbeddedObject _embeddedobject = 
                    new _EmbeddedObject(
                        _mycontext,
                        this,
                        _operation_add.X1, _operation_add.Y1, 
                        _operation_add.X2, _operation_add.Y2,
                        _operation_add.ID, 
                        _operation_add.Label,
                        _operation_add.ObjectXml, 
                        _uiobjectref);

                _embeddedobjects.Add(_embeddedobject.ID, _embeddedobject);
                _embeddedobject._CalculateCoordinates_LogicalToPhysical();
                _embeddedobject._Load();
            }
        }

        #endregion

        #region _Operation_Move

        private void _Operation_Move(QS.Fx.Value.Classes.Desktop.DesktopOperation_Move _operation_move)
        {
            _EmbeddedObject _embeddedobject;
            if (this._embeddedobjects.TryGetValue(_operation_move.ID, out _embeddedobject))
            {
                _embeddedobject.X1 = _operation_move.X1;
                _embeddedobject.Y1 = _operation_move.Y1;
                _embeddedobject.X2 = _operation_move.X2;
                _embeddedobject.Y2 = _operation_move.Y2;
                _embeddedobject._CalculateCoordinates_LogicalToPhysical();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CoordinatesChanged

        protected override void _CoordinatesChanged()
        {
            foreach (_EmbeddedObject _embeddedobject in this._embeddedobjects.Values)
                _embeddedobject._CalculateCoordinates_LogicalToPhysical();
        }

        #endregion

        #region _DropObject

        protected override void _DropObject(
            double _xc, double _yc, string _objectxml, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
        {
            
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IUI> _uiobjectref = _objectref.CastTo<QS.Fx.Object.Classes.IUI>();

            double _xs = 200;
            double _ys = 200;
            double _x1 = _xc - _xs / 2;
            double _y1 = _yc - _ys / 2;
            double _x2 = _xc + _xs / 2;
            double _y2 = _yc + _ys / 2;

            string _label = QS.Fx.Attributes.Attribute.ValueOf(
                _objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);
            
            foreach(_EmbeddedObject obj in this._embeddedobjects.Values)
            {
                if(obj.Label.CompareTo(_label) == 0)
                {
                    MessageBox.Show("Object already exists!");
                    return;
                }
            }

            if (_label.Contains("Network") || _label.Contains("Circuit"))
            {
                _x1 = -330;
                _y1 = -140;
                _x2 = 100;
                _y2 = 150;
            }
            if (_label.Contains("Graph"))
            {
                _x1 = 108;
                _y1 = 0;
                _x2 = 330;
                _y2 = 150;
            }
            if (_label.Contains("Log"))
            {
                _x1 = 108;
                _y1 = -140;
                _x2 = 330;
                _y2 = -15;
            }

            do
            {
                _new_id = QS.Fx.Base.ID.NewID();
            }
            while (this._embeddedobjects.ContainsKey(_new_id));
            QS.Fx.Value.Classes.Desktop.DesktopOperation_Add _operation_add =
                new QS.Fx.Value.Classes.Desktop.DesktopOperation_Add(_new_id, _label, _x1, _y1, _x2, _y2, _objectxml);
            this._channelendpoint.Interface.Send(_operation_add);
            
            /*
                _EmbeddedObject o = _embeddedobjects[_new_id];
                _embeddedobjects.Remove(_new_id);                
                //o.Hide();
                o._uiendpoint.Disconnect();
                //_mycontext.ExportedUI(o);
                this._ContainerControl.Controls.RemoveByKey("Circuit");
                this._ContainerControl.Controls.RemoveByKey("label");
                r = false;
            */
        }

        #endregion

        #region _MoveObject

        private void _MoveObject(_EmbeddedObject _embeddedobject)
        {
            QS.Fx.Value.Classes.Desktop.DesktopOperation_Move _operation_move =
                new QS.Fx.Value.Classes.Desktop.DesktopOperation_Move
                (
                    _embeddedobject.ID, 
                    _embeddedobject.X1, _embeddedobject.Y1, 
                    _embeddedobject.X2, _embeddedobject.Y2
                );

            this._channelendpoint.Interface.Send(_operation_move);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
