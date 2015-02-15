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

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Viewfinder, 
        "Viewfinder", "This component implements a zoomable and scrollable view into a virtual plane.")]
    public partial class Viewfinder : QS.Fx.Component.Classes.UI
    {
        #region Constructor

        public Viewfinder(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("loader",
                "The component to use to load objects from XML descriptions.",
                QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IService<
                        QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> 
                    _loader) : base(_mycontext)
        {
            InitializeComponent();

            if (_loader == null)
                throw new Exception("Folder view cannot run without the attached loader.");

            this._loaderendpoint = _mycontext.ImportedInterface<
                QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>();

            lock (this)
            {
                this._loaderconnection =
                    ((QS.Fx.Endpoint.Classes.IEndpoint)this._loaderendpoint).Connect(_loader.Dereference(_mycontext).Endpoint);
            }

            this._desktop_x = 0;
            this._desktop_y = 0;
            this._zoom_level = 1;

            this._CalculateCoordinates_PhysicalToLogical();

            this.MouseWheel += new MouseEventHandler(Panel_MouseWheel);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IImportedInterface<
            QS.Fx.Interface.Classes.ILoader<
                QS.Fx.Object.Classes.IObject>>
            _loaderendpoint;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _loaderconnection;

        [QS.Fx.Base.Inspectable]
        private double _desktop_x, _desktop_y, _zoom_level,
            _desktop_dx, _desktop_dy, _desktop_x1, _desktop_y1, _desktop_x2, _desktop_y2,
            _desktop_xp, _desktop_yp, _desktop_xp0, _desktop_yp0;

        [QS.Fx.Base.Inspectable]
        private bool _desktop_moving;

        #endregion

        #region _BackgroundColor

        protected System.Drawing.Color _BackgroundColor
        {
            get { return Panel.BackColor; }
            set { Panel.BackColor = value; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Load

        protected bool _LoadObject(string _objectxml, out QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
        {
            try
            {
                _objectref = this._loaderendpoint.Interface.Load(_objectxml);
                return true;
            }
            catch (Exception _exc)
            {
                _Exception("Cannot create object because the XML specification is malformed.\n", _exc);
                _objectref = null;
                return false;
            }
        }

        #endregion

        #region _Exception

        protected void _Exception(string _msg, Exception _exc)
        {
            StringBuilder sb = new StringBuilder();
            if (_msg != null)
                sb.AppendLine(_msg);
            if (_exc != null)
                sb.AppendLine(_exc.ToString());
            System.Windows.Forms.MessageBox.Show(sb.ToString(), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        #region _LogicalToPhysical

        protected void _LogicalToPhysical(double _logical_x, double _logical_y, out int _physical_x, out int _physical_y)
        {
            Rectangle _rectangle = this.Panel.ClientRectangle;
            _physical_x = _rectangle.Left + (int) Math.Round(((_logical_x - _desktop_x1) / (2 * _desktop_dx)) * (double)_rectangle.Width);
            _physical_y = _rectangle.Bottom - (int) Math.Round(((_logical_y - _desktop_y1) / (2 * _desktop_dy)) * (double)_rectangle.Height);
        }

        #endregion

        #region _PhysicalToLogical

        protected void _PhysicalToLogical(int _physical_x, int _physical_y, out double _logical_x, out double _logical_y)
        {
            Rectangle _rectangle = this.Panel.ClientRectangle;
            _logical_x  = _desktop_x1 + 2 * _desktop_dx * ((double)(_physical_x - _rectangle.Left)) / ((double)_rectangle.Width);
            _logical_y = _desktop_y1 + 2 * _desktop_dy * ((double)(_rectangle.Bottom - _physical_y)) / ((double)_rectangle.Height);
        }

        #endregion

        #region _Zoom_X

        /// <summary>
        ///  The value of this property represents the number of physical pixels that correspond to one unit in the logical coordinates in the horizontal direction.
        /// </summary>
        protected double _Zoom_X
        {
            get { return this._zoom_level; }
        }

        #endregion

        #region _Zoom_Y

        /// <summary>
        ///  The value of this property represents the number of physical pixels that correspond to one unit in the logical coordinates in the vertical direction.
        /// </summary>
        protected double _Zoom_Y
        {
            get { return this._zoom_level; }
        }

        #endregion

        #region _ControlCollection

        protected System.Windows.Forms.Control _ContainerControl
        {
            get { return this.Panel; }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CoordinatesChanged

        protected virtual void _CoordinatesChanged()
        {
        }

        #endregion

        #region _DropObject

        protected virtual void _DropObject(
            double _xc, double _yc, string _objectxml, QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref)
        {
        }

        #endregion

        #region _DeselectObjects

        protected virtual void _DeselectObjects()
        {
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Drop

        private void _Drop(string _textdata, Point _position, int _keystate)
        {
            lock (this)
            {
                Rectangle _rectangle = this.Panel.ClientRectangle;

                double _xn = ((double)(_position.X - _rectangle.Left)) / ((double)_rectangle.Width);
                double _yn = ((double)(_rectangle.Bottom - _position.Y)) / ((double)_rectangle.Height);
                double _xc = _desktop_x1 + 2 * _desktop_dx * _xn;
                double _yc = _desktop_y1 + 2 * _desktop_dy * _yn;

                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IObject> _objectref;
                if (this._LoadObject(_textdata, out _objectref))
                    this._DropObject(_xc, _yc, _textdata, _objectref);
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _CalculateCoordinates_PhysicalToLogical

        private void _CalculateCoordinates_PhysicalToLogical()
        {
            Rectangle _rectangle = this.Panel.ClientRectangle;

            double _physical_dx = (((double)_rectangle.Right) - ((double)_rectangle.Left)) / 2;
            double _physical_dy = (((double)_rectangle.Bottom) - ((double)_rectangle.Top)) / 2;

            this._desktop_dx = _physical_dx / this._zoom_level;
            this._desktop_dy = _physical_dy / this._zoom_level;

            // double _physical_x = (((double)_rectangle.Left) + ((double)_rectangle.Right)) / 2;
            // double _physical_y = (((double)_rectangle.Top) + ((double)_rectangle.Bottom)) / 2;

            this._desktop_x1 = this._desktop_x - this._desktop_dx;
            this._desktop_y1 = this._desktop_y - this._desktop_dy;
            this._desktop_x2 = this._desktop_x + this._desktop_dx;
            this._desktop_y2 = this._desktop_y + this._desktop_dy;

            this.label_Xrange.Text = "(" + this._desktop_x1.ToString("0.00") + "," + this._desktop_x2.ToString("0.00") + ")";
            this.label_Yrange.Text = "(" + this._desktop_y1.ToString("0.00") + "," + this._desktop_y2.ToString("0.00") + ")";
        }

        #endregion

        #region _CalculateCoordinates_PhysicalToLogical_OnResize

        private void _CalculateCoordinates_PhysicalToLogical_OnResize()
        {
            Rectangle _rectangle = this.Panel.ClientRectangle;

            double _physical_dx = (((double)_rectangle.Right) - ((double)_rectangle.Left)) / 2;
            double _physical_dy = (((double)_rectangle.Bottom) - ((double)_rectangle.Top)) / 2;

            this._zoom_level = _physical_dx / this._desktop_dx;

            this._desktop_dy = this._desktop_dx * _physical_dy / _physical_dx;

            this._desktop_x1 = this._desktop_x - this._desktop_dx;
            this._desktop_y1 = this._desktop_y - this._desktop_dy;
            this._desktop_x2 = this._desktop_x + this._desktop_dx;
            this._desktop_y2 = this._desktop_y + this._desktop_dy;

            this.label_Xrange.Text = "(" + this._desktop_x1.ToString("0.00") + "," + this._desktop_x2.ToString("0.00") + ")";
            this.label_Yrange.Text = "(" + this._desktop_y1.ToString("0.00") + "," + this._desktop_y2.ToString("0.00") + ")";
        }

        #endregion

        #region _CalculateCoordinates_MouseLogical

        private void _CalculateCoordinates_MouseLogical(int mouse_x, int mouse_y)
        {
            Rectangle _rectangle = this.Panel.ClientRectangle;

            this._desktop_xp = this._desktop_x1 + 2 * this._desktop_dx * (((double)(mouse_x - _rectangle.Left)) / ((double)_rectangle.Width));
            this._desktop_yp = this._desktop_y2 - 2 * this._desktop_dy * (((double)(mouse_y - _rectangle.Top)) / ((double)_rectangle.Height));

            this.label_X.Text = this._desktop_xp.ToString("0.00");
            this.label_Y.Text = this._desktop_yp.ToString("0.00");
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Panel_DragEnter

        private void Panel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) ||
                e.Data.GetDataPresent(DataFormats.Text, false) || e.Data.GetDataPresent(DataFormats.UnicodeText, false))
            {
                e.Effect = DragDropEffects.All;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        #region Panel_DragDrop

        private void Panel_DragDrop(object sender, DragEventArgs e)
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
                        this._Drop(_text, this.PointToClient(new Point(e.X, e.Y)), e.KeyState);
                    }
                }
                else if (e.Data.GetDataPresent(DataFormats.Text, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.Text);
                    this._Drop(_text, this.PointToClient(new Point(e.X, e.Y)), e.KeyState);
                }
                else if (e.Data.GetDataPresent(DataFormats.UnicodeText, false))
                {
                    string _text = (string)e.Data.GetData(DataFormats.UnicodeText);
                    this._Drop(_text, this.PointToClient(new Point(e.X, e.Y)), e.KeyState);
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

        #region Panel_DragOver

        private void Panel_DragOver(object sender, DragEventArgs e)
        {
            Point _position = this.PointToClient(new Point(e.X, e.Y));
            lock (this)
            {
                this._CalculateCoordinates_MouseLogical(_position.X, _position.Y);
            }
        }

        #endregion

        #region Panel_Resize

        private void Panel_Resize(object sender, EventArgs e)
        {
            lock (this)
            {
                this._CalculateCoordinates_PhysicalToLogical_OnResize();
                this._CoordinatesChanged();
            }
        }

        #endregion

        #region Panel_MouseDown

        private void Panel_MouseDown(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                this._desktop_xp0 = this._desktop_xp;
                this._desktop_yp0 = this._desktop_yp;
                this.Panel.Focus();
                this._DeselectObjects();
                this._desktop_moving = true;
            }
        }

        #endregion

        #region Panel_MouseMove

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (_desktop_moving)
                {
                    Rectangle _rectangle = this.Panel.ClientRectangle;
                    double _wouldbe_desktop_xp =
                        this._desktop_x1 + 2 * this._desktop_dx * (((double)(e.X - _rectangle.Left)) / ((double)_rectangle.Width));
                    double _wouldbe_desktop_yp =
                        this._desktop_y2 - 2 * this._desktop_dy * (((double)(e.Y - _rectangle.Top)) / ((double)_rectangle.Height));
                    double _movement_dx = _wouldbe_desktop_xp - _desktop_xp;
                    double _movement_dy = _wouldbe_desktop_yp - _desktop_yp;
                    this._desktop_x -= _movement_dx;
                    this._desktop_y -= _movement_dy;
                    this._CalculateCoordinates_PhysicalToLogical();
                    this._CoordinatesChanged();
                }
                else
                {
                    this._CalculateCoordinates_MouseLogical(e.X, e.Y);
                }
            }
        }

        #endregion

        #region Panel_MouseUp

        private void Panel_MouseUp(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                _desktop_moving = false;
            }
        }

        #endregion

        #region Panel_MouseWheel

        private void Panel_MouseWheel(object sender, MouseEventArgs e)
        {
            lock (this)
            {
                if (this.Panel.Focused)
                {
                    double _relative_zoom = Math.Pow(1.05, (((double)e.Delta) / (120.0)));
                    this._zoom_level *= _relative_zoom;
                    this._desktop_x = this._desktop_xp + (this._desktop_x - this._desktop_xp) / _relative_zoom;
                    this._desktop_y = this._desktop_yp + (this._desktop_y - this._desktop_yp) / _relative_zoom;
                    this._CalculateCoordinates_PhysicalToLogical();
                    this._CoordinatesChanged();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
