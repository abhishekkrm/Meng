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
        QS.Fx.Reflection.ComponentClasses.Coordinates_1, "Coordinates_1", "Coordinates of a moving object.")]
    public sealed class Coordinates_1
        : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
        QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>
    {
        #region Constructor

        public Coordinates_1(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("PX", QS.Fx.Reflection.ParameterClass.Value)] float _PX,
            [QS.Fx.Reflection.Parameter("PY", QS.Fx.Reflection.ParameterClass.Value)] float _PY,
            [QS.Fx.Reflection.Parameter("PZ", QS.Fx.Reflection.ParameterClass.Value)] float _PZ,
            [QS.Fx.Reflection.Parameter("RX", QS.Fx.Reflection.ParameterClass.Value)] float _RX,
            [QS.Fx.Reflection.Parameter("RY", QS.Fx.Reflection.ParameterClass.Value)] float _RY,
            [QS.Fx.Reflection.Parameter("RZ", QS.Fx.Reflection.ParameterClass.Value)] float _RZ)
        {
            this._coordinates = new QS._qss_x_.Channel_.Message_.Coordinates(0, _PX, _PY, _PZ, 0f, 0f, 0f, _RX, _RY, _RZ, 0f, 0f, 0f);

            this._coordinatesendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>>(this);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinatesendpoint;
        private QS._qss_x_.Channel_.Message_.Coordinates _coordinates;

        #endregion

        #region IValue<ICoordinates> Members

        QS._qss_x_.Channel_.Message_.ICoordinates QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Get()
        {
            return this._coordinates;
        }

        void QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Set(QS._qss_x_.Channel_.Message_.ICoordinates _value)
        {
            throw new NotSupportedException();
        }

        #endregion

        #region IValue<ICoordinates> Members

        QS.Fx.Endpoint.Classes.IDualInterface<
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>>
                QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>.Endpoint
        {
            get { return this._coordinatesendpoint; }
        }

        #endregion
    }
}
