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

namespace QS.Fx.Value.Classes.Desktop
{
    [QS.Fx.Printing.Printable("State", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Channel_Message_Desktop_State)]
    public sealed class DesktopState : IDesktopState
    {
        #region Constructors

        public DesktopState(IEnumerable<DesktopObject> _objects)
        {
            this._objects = new List<DesktopObject>(_objects);
        }

        public DesktopState()
        {
        }

        #endregion

        #region Fields

        private List<DesktopObject> _objects;

        #endregion

        #region IState Members

        IEnumerable<DesktopObject> IDesktopState.Objects
        {
            get { return _objects; }
            set { _objects = new List<DesktopObject>(value); }
        }

        #endregion

        #region ISerializable Members

        unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info =
                    new QS.Fx.Serialization.SerializableInfo(
                        (ushort)QS.ClassID.Fx_Channel_Message_Desktop_State, sizeof(uint));
                foreach (DesktopObject _object in this._objects)
                    _info.AddAnother(((QS.Fx.Serialization.ISerializable)_object).SerializableInfo);
                return _info;
            }
        }

        unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
            ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* ph = header.Array)
            {
                *((uint*)(ph + header.Offset)) = (uint)this._objects.Count;
            }
            header.consume(sizeof(uint));
            foreach (IDesktopObject _object in this._objects)
                ((QS.Fx.Serialization.ISerializable)_object).SerializeTo(ref header, ref data);
        }

        unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
            ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            int count;
            fixed (byte* ph = header.Array)
            {
                count = (int)(*((uint*)(ph + header.Offset)));
            }
            header.consume(sizeof(uint));
            this._objects = new List<DesktopObject>(count);
            while (count-- > 0)
            {
                DesktopObject _object = new DesktopObject();
                ((QS.Fx.Serialization.ISerializable)_object).DeserializeFrom(ref header, ref data);
                this._objects.Add(_object);
            }
        }

        #endregion
    }
}
