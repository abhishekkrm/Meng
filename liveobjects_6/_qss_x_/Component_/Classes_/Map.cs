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
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Map, "Map", "A map.")]
    public sealed partial class Map
        : Viewfinder, 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Map.IOperation, 
                QS._qss_x_.Channel_.Message_.Map.IState>
    {
        #region Constructor

        public Map(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS._qss_x_.Channel_.Message_.Map.IOperation,
                        QS._qss_x_.Channel_.Message_.Map.IState>>
                    _channel,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IService<
                        QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> 
                    _loader)
            : base(_mycontext, _loader)
        {
            this._mycontext = _mycontext;

            InitializeComponent();

            if (_channel == null)
                throw new Exception("Folder view cannot run without the attached channel.");

            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                    QS._qss_x_.Channel_.Message_.Map.IOperation,
                    QS._qss_x_.Channel_.Message_.Map.IState>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                    QS._qss_x_.Channel_.Message_.Map.IOperation,
                    QS._qss_x_.Channel_.Message_.Map.IState>>(this);

            lock (this)
            {
                this._channelconnection =
                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);
            }

            this._image = new System.Drawing.Bitmap("C:\\Users\\krzys\\Work\\QuickSilver\\@Content\\Images\\Map_1.jpg");

            this.BackColor = System.Drawing.Color.Empty;
            this._ContainerControl.BackColor = System.Drawing.Color.Empty;

            ((DrawingPanel) this._ContainerControl).PaintCallback = this._PaintCallback;

            this._timer = new System.Windows.Forms.Timer();
            this._timer.Interval = 30;
            this._timer.Tick += new EventHandler(this._TimeoutCallback);
            this._timer.Enabled = true;
            this._lastdraw = DateTime.Now;

            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true); 
        }

        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                QS._qss_x_.Channel_.Message_.Map.IOperation,
                QS._qss_x_.Channel_.Message_.Map.IState>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Map.IOperation,
                QS._qss_x_.Channel_.Message_.Map.IState>>
            _channelendpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channelconnection;

        [QS.Fx.Base.Inspectable]
        private ICollection<_EmbeddedObject> _embeddedobjects =
            new System.Collections.ObjectModel.Collection<_EmbeddedObject>();

        private System.Drawing.Image _image;
        private DateTime _lastdraw;
        private System.Windows.Forms.Timer _timer;

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Class _EmbeddedObject

        private sealed class _EmbeddedObject
        {
            #region Constructor

            public _EmbeddedObject(Map _map,
/*
                double _x1, double _y1, double _x2, double _y2, 
*/ 
                string _label, string _objectxml,
                QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IMap> _objectref)
            {
                this._map = _map;
/*
                this._x1 = _x1;
                this._y1 = _y1;
                this._x2 = _x2;
                this._y2 = _y2;
*/ 
                this._label = _label;
                this._objectxml = _objectxml;
                this._objectref = _objectref;
                this._object = null;
            }

            #endregion

            #region Fields

            private Map _map;
/*
            private double _x1, _y1, _x2, _y2;
*/ 
            private string _label, _objectxml;
            private QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IMap> _objectref;
            private QS._qss_x_.Object_.Classes_.IMap _object;
/*
            private int _physical_x1, _physical_y1, _physical_x2, _physical_y2;
            private System.Windows.Forms.Label _labelcontrol;

            [QS.TMS.Inspection.Inspectable("uiendpoint")]
            private QS.Fx.Endpoint.Internal.IImportedUI _uiendpoint;

            [QS.TMS.Inspection.Inspectable("uiconnection")]
            private QS.Fx.Endpoint.IConnection _uiconnection;
*/

            #endregion

            #region _Load

            public void _Load()
            {
                if (_object != null)
                    throw new Exception("The object is already loaded.");

                this._object = _objectref.Dereference(_map._mycontext);
                
/*
                this._uiendpoint = _mycontext.ImportedUI(this);
                this._uiconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._uiendpoint).Connect(_object.UI);
                
                this._uiendpoint.Control.Location = new Point(_BORDER_WIDTH, _BORDER_WIDTH);

                this._labelcontrol = new System.Windows.Forms.Label();
                this._labelcontrol.BackColor = Color.Ivory;
                this._labelcontrol.Text = this._label;

                this.BackColor = Color.Black;
                _desktop._ControlCollection.Add(this);
                _desktop._ControlCollection.Add(this._labelcontrol);
                
                _MovePhysical();
*/ 
            }

            #endregion
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Initialize

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Map.IOperation,
            QS._qss_x_.Channel_.Message_.Map.IState>.Initialize(
                QS._qss_x_.Channel_.Message_.Map.IState _checkpoint)
        {
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Receive

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Map.IOperation,
            QS._qss_x_.Channel_.Message_.Map.IState>.Receive(
                QS._qss_x_.Channel_.Message_.Map.IOperation _operation)
        {
            try
            {
                switch (_operation.OperationType)
                {
                    case QS._qss_x_.Channel_.Message_.Map.OperationType.AddObject:
                        this._Operation_AddObject((QS._qss_x_.Channel_.Message_.Map.Operation_AddObject)_operation);
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception _exc)
            {
                this._Exception(null, _exc);
            }
        }

        #endregion

        #region ICheckpointedCommunicationChannelClient<IOperation,IState>.Checkpoint

        QS._qss_x_.Channel_.Message_.Map.IState 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Map.IOperation,
                QS._qss_x_.Channel_.Message_.Map.IState>.Checkpoint()
        {
            return null;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Operation_AddObject

        private void _Operation_AddObject(QS._qss_x_.Channel_.Message_.Map.Operation_AddObject _operation_addobject)
        {
            lock (this)
            {
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                if (this._LoadObject(_operation_addobject.ObjectXml, out _objectref))
                {
                    QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IMap> _mapobjectref = _objectref.CastTo<QS._qss_x_.Object_.Classes_.IMap>();

                    _EmbeddedObject _embeddedobject = 
                        new _EmbeddedObject(
                            this, _operation_addobject.Label, _operation_addobject.ObjectXml, _mapobjectref);

                    _embeddedobjects.Add(_embeddedobject);
                    _embeddedobject._Load();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _PaintCallback

        private DateTime _start = DateTime.Now;

        private void _PaintCallback(PaintEventArgs e)
        {
            double _logical_dx = ((double)_image.Width) / 2;
            double _logical_dy = ((double)_image.Height) / 2;
            int _d_x1, _d_y1, _d_x2, _d_y2;
            _LogicalToPhysical(-_logical_dx, _logical_dy, out _d_x1, out _d_y1);
            _LogicalToPhysical(_logical_dx, -_logical_dy, out _d_x2, out _d_y2);

            Rectangle _s_rectangle = new Rectangle(new Point(0,0), this._image.Size);
            Rectangle _d_rectangle = new Rectangle(_d_x1, _d_y1, _d_x2 - _d_x1, _d_y2 - _d_y1);

            e.Graphics.DrawImage(this._image, _d_rectangle, _s_rectangle, GraphicsUnit.Pixel);

            double t = (DateTime.Now - _start).TotalSeconds;
            Brush[] _colors = new Brush[] { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Orange };
            int _xx, _yy;
            for (int n = 1; n <= 5; n++)
            {
                double s = Math.Pow(-1, (double) n);
                for (int k = 0; k < 10; k++)
                {
                    double tt = s * n * (t - 0.1 * k);
                    _LogicalToPhysical(50 * n * Math.Cos(tt), 50 * n * Math.Sin(tt), out _xx, out _yy);
                    e.Graphics.FillEllipse(_colors[n - 1], _xx, _yy, 10, 10);
                }
            }

            this._lastdraw = DateTime.Now;
        }

        #endregion

        #region _TimeoutCallback

        private void _TimeoutCallback(object _o, EventArgs _e)
        {
            if ((DateTime.Now - _lastdraw).TotalMilliseconds > 20)
                this._ContainerControl.Refresh();
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CoordinatesChanged

        protected override void _CoordinatesChanged()
        {
            this._ContainerControl.Refresh();
/*
            foreach (_EmbeddedObject _embeddedobject in this._embeddedobjects)
                _embeddedobject._CalculateCoordinates_LogicalToPhysical();
*/ 
        }

        #endregion

        #region _DropObject

        protected override void _DropObject(
            double _xc, double _yc, string _objectxml, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
        {
            QS.Fx.Object.IReference<QS._qss_x_.Object_.Classes_.IMap> _mapobjectref = _objectref.CastTo<QS._qss_x_.Object_.Classes_.IMap>();
            string _label = QS.Fx.Attributes.Attribute.ValueOf(
                _objectref.Attributes, QS.Fx.Attributes.AttributeClasses.CLASS_name, _objectref.ID);

            QS._qss_x_.Channel_.Message_.Map.Operation_AddObject _operation_addobject =
                new QS._qss_x_.Channel_.Message_.Map.Operation_AddObject(_label, _objectxml);

            this._channelendpoint.Interface.Send(_operation_addobject);
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
