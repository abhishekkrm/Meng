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
using System.Threading;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Color_UI, "Color_UI", "UI for choosing the color of a 3-dimensional object.")]
    public sealed partial class Color_UI : QS.Fx.Component.Classes.UI
    {
        #region Constructor

        public Color_UI(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        QS._qss_x_.Channel_.Message_.IColor, QS._qss_x_.Channel_.Message_.IColor>> _channelobjectref
        )
            : base(_mycontext)
        {
            this.InitializeComponent();

            if (_channelobjectref == null)
                throw new Exception("Channel is null.");

            this._colorobject = new Color(_mycontext, 0, 0, 0, 0);

            this._publisherobject = 
                new Publisher<QS._qss_x_.Channel_.Message_.IColor>
                (
                    _mycontext,
                    QS._qss_x_.Object_.Reference<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>>.Create
                    (
                        this._colorobject,
                        "color",
                        QS._qss_x_.Reflection_.Library.ObjectClassOf
                        (
                            typeof(QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>)
                        )
                    ),
                    _channelobjectref
                );
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Component_.Classes_.Color _colorobject;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Component_.Classes_.Publisher<QS._qss_x_.Channel_.Message_.IColor> _publisherobject;

        #endregion

        #region _ValueChanged

        private void _ValueChanged(object sender, EventArgs e)
        {
            QS._qss_x_.Channel_.Message_.IColor _color;
            lock (this)
            {
                _color = 
                    new QS._qss_x_.Channel_.Message_.Color(
                        (byte)this.trackBar1.Value,
                        (byte)this.trackBar2.Value,
                        (byte)this.trackBar3.Value,
                        (byte)this.trackBar4.Value);
            }

            ((QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>)_colorobject).Set(_color);
        }

        #endregion
    }
}
