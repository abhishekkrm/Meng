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
        QS.Fx.Reflection.ComponentClasses.Color, "Color", "Color of a 3-dimensional object.")]
    public sealed class Color
        : QS.Fx.Inspection.Inspectable, 
        QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>,
        QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>, 
        IDisposable
    {
        #region Constructor

        public Color(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("R", QS.Fx.Reflection.ParameterClass.Value)] byte _R,
            [QS.Fx.Reflection.Parameter("G", QS.Fx.Reflection.ParameterClass.Value)] byte _G,
            [QS.Fx.Reflection.Parameter("B", QS.Fx.Reflection.ParameterClass.Value)] byte _B,
            [QS.Fx.Reflection.Parameter("A", QS.Fx.Reflection.ParameterClass.Value)] byte _A)
        {
            this._color = new QS._qss_x_.Channel_.Message_.Color(_R, _G, _B, _A);

            this._colorendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>,
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>>(this);

            this._colorendpoint.OnConnect += new QS.Fx.Base.Callback(this._ColorConnectCallback);
            this._colorendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ColorDisconnectCallback);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>,
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>> _colorendpoint;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Channel_.Message_.IColor _color;

        #endregion

        #region _ColorConnectCallback

        private void _ColorConnectCallback()
        {
            lock (this)
            {
                this._colorendpoint.Interface.Set(this._color);
            }
        }

        #endregion

        #region _ColorDisconnectCallback

        private void _ColorDisconnectCallback()
        {
        }

        #endregion

        #region IValue<IColor> Members

        QS._qss_x_.Channel_.Message_.IColor QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>.Get()
        {
            return this._color;
        }

        void QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>.Set(QS._qss_x_.Channel_.Message_.IColor _value)
        {
            this._color = _value;
            if (this._colorendpoint.IsConnected)
                this._colorendpoint.Interface.Set(_value);
        }

        #endregion

        #region IValue<IColor> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>,
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>>
                QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>.Endpoint
        {
            get { return this._colorendpoint; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}
