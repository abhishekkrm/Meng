/* Copyright (c) 2004-2009 Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Properties_.Value_
{
    [QS.Fx.Printing.Printable("Round", QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [QS.Fx.Serialization.ClassID(QS.ClassID.Round_)]
    [QS.Fx.Reflection.ValueClass("53417B82864687558AF91BC359D741F0", "Round_", "")]
    public sealed class Round_
        : QS.Fx.Inspection.Inspectable, QS.Fx.Serialization.ISerializable
    {
        #region Constructor

        public Round_
        (
            QS.Fx.Base.Incarnation _incarnation,
            QS.Fx.Base.Index _index
        )
        {
            this._incarnation = _incarnation;
            this._index = _index;
        }

        public Round_()
        {
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Incarnation _incarnation;
        [QS.Fx.Printing.Printable]
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Base.Index _index;

        #endregion

        #region Accessors

        public QS.Fx.Base.Incarnation Incarnation
        {
            get { return this._incarnation; }
        }

        public QS.Fx.Base.Index Index
        {
            get { return this._index; }
        }

        #endregion

        #region ISerializable Members


        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get
            {
                QS.Fx.Serialization.SerializableInfo _info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Round_);
                _info.AddAnother(this._incarnation.SerializableInfo);
                _info.AddAnother(this._index.SerializableInfo);
                return _info;
            }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock _header, ref IList<QS.Fx.Base.Block> _data)
        {
            this._incarnation.SerializeTo(ref _header, ref _data);
            this._index.SerializeTo(ref _header, ref _data);
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock _header, ref QS.Fx.Base.ConsumableBlock _data)
        {
            this._incarnation = new QS.Fx.Base.Incarnation();
            this._incarnation.DeserializeFrom(ref _header, ref _data);
            this._index = new QS.Fx.Base.Index();
            this._index.DeserializeFrom(ref _header, ref _data);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Round_))
                return false;

            Round_ _r = (Round_)obj;

            return _r._incarnation.Equals(this._incarnation) && _r._index.Equals(this._index);
        }

        #endregion

    }
}
