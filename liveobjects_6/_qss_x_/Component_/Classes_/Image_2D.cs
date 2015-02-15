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
using System.Drawing.Imaging;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Image_2D, "Image_2D", "A simple 2D image.")]
    public sealed partial class Image_2D
        : QS.Fx.Component.Classes.UI, 
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Video.IFrame, 
            QS._qss_x_.Channel_.Message_.Video.IFrame>
    {
        #region Constructor

        public Image_2D(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS._qss_x_.Channel_.Message_.Video.IFrame, 
                        QS._qss_x_.Channel_.Message_.Video.IFrame>>
                            _channel)
            : base(_mycontext)
        {
            InitializeComponent();

            this._channelendpoint = 
                _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                        QS._qss_x_.Channel_.Message_.Video.IFrame,
                        QS._qss_x_.Channel_.Message_.Video.IFrame>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                        QS._qss_x_.Channel_.Message_.Video.IFrame,
                        QS._qss_x_.Channel_.Message_.Video.IFrame>>(this);

            if (_channel != null)
                this._channelconnection = 
                    ((QS.Fx.Endpoint.Classes.IEndpoint) this._channelendpoint).Connect(
                        _channel.Dereference(_mycontext).Channel);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable("channelendpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                QS._qss_x_.Channel_.Message_.Video.IFrame,
                QS._qss_x_.Channel_.Message_.Video.IFrame>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Video.IFrame,
                QS._qss_x_.Channel_.Message_.Video.IFrame>>
                    _channelendpoint;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection 
            _channelconnection;

        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS._qss_x_.Channel_.Message_.Video.IFrame _frame;

        #endregion

        #region ICheckpointedCommunicationChannelClient<IFrame,IFrame> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Video.IFrame, 
            QS._qss_x_.Channel_.Message_.Video.IFrame>.Receive(
                QS._qss_x_.Channel_.Message_.Video.IFrame _message)
        {
            lock (this)
            {
                this._frame = _message;
            }
            this._Refresh();
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
            QS._qss_x_.Channel_.Message_.Video.IFrame, 
            QS._qss_x_.Channel_.Message_.Video.IFrame>.Initialize(
                QS._qss_x_.Channel_.Message_.Video.IFrame _checkpoint)
        {
            lock (this)
            {
                this._frame = _checkpoint;
            }
            this._Refresh();
        }

        QS._qss_x_.Channel_.Message_.Video.IFrame 
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                QS._qss_x_.Channel_.Message_.Video.IFrame, 
            QS._qss_x_.Channel_.Message_.Video.IFrame>.Checkpoint()
        {
            lock (this)
            {
                return this._frame;
            }
        }

        #endregion

        #region _Refresh

        private void _Refresh()
        {
            if (this.InvokeRequired)
                this.BeginInvoke(new QS.Fx.Base.Callback(this._Refresh), new object[0]);
            else
            {
                lock (this)
                {
                    Bitmap _bitmap = null;
                    if (this._frame != null)
                    {
                        MemoryStream _stream = new MemoryStream(this._frame.Data);
                        _bitmap = new Bitmap(_stream);
                        // _bitmap.Size = new Size(this._frame.Width, this._frame.Height);
                    }
                    pictureBox1.Image = _bitmap;
                    this._Resize();
                }
            }
        }

        #endregion

        #region _DragEnter

        private void _DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) || e.Data.GetDataPresent(DataFormats.Bitmap, true))
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
                    string[] _filenames = (string[]) e.Data.GetData(DataFormats.FileDrop);
                    if (_filenames.Length == 1)
                        this._Drop(new Bitmap(_filenames[0]));
                }
                else if (e.Data.GetDataPresent(DataFormats.Bitmap, true))
                {
                    this._Drop((Bitmap) e.Data.GetData(DataFormats.Bitmap));
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

        private void _Drop(Bitmap _bitmap)
        {
            MemoryStream _stream = new MemoryStream();
            _bitmap.Save(_stream, ImageFormat.Jpeg);

            QS._qss_x_.Channel_.Message_.Video.IFrame _frame = 
                new QS._qss_x_.Channel_.Message_.Video.Frame(0, _bitmap.Width, _bitmap.Height, _stream.ToArray());
            
            this._channelendpoint.Interface.Send(_frame);
        }

        #endregion

        #region _SizeChanged

        private void _SizeChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                this._Resize();
            }
        }

        #endregion

        #region _Resize

        private void _Resize()
        {
        }

        #endregion
    }
}
