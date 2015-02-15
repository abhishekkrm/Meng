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

namespace QS._qss_x_.Runtime_1_
{
    [QS._qss_x_.Language_.Structure_.ValueType("BOOL", QS._qss_x_.Language_.Structure_.PredefinedType.BOOL)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    // [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_Runtime_Bool)]
    public struct BOOL : IValue<BOOL>, QS.Fx.Serialization.ISerializable
    {
        #region Constructors

        public BOOL(bool _isdefined, bool _value)
        {
            this._isdefined = _isdefined;
            this._value = _value;
        }

        // [QS.Fx.Language.Structure.Constructor]
        // [QS.Fx.Language.Structure.Operator("Bool", QS.Fx.Language.Structure.PredefinedOperator.None)]
        [QS._qss_x_.Language_.Structure_.Operator(QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, true)]
        public BOOL(bool _value) : this(true, _value)
        {
        }

//        public BOOL()
//        {
//        }

        private static readonly BOOL _undefined = new BOOL(false, false);
        private static readonly BOOL _false = new BOOL(true, false);
        private static readonly BOOL _true = new BOOL(true, true);

        [QS._qss_x_.Language_.Structure_.Operator("Undefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.Undefined, true)]
        public static BOOL Undefined
        {
            get { return _undefined; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("False", QS._qss_x_.Language_.Structure_.PredefinedOperator.False, true)]
        public static BOOL False
        {
            get { return _false; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("True", QS._qss_x_.Language_.Structure_.PredefinedOperator.True, true)]
        public static BOOL True
        {
            get { return _true; }
        }

        #endregion

        #region Fields

        private bool _isdefined, _value;

        #endregion

        #region Accessors

        [QS._qss_x_.Language_.Structure_.Operator("Value", new QS._qss_x_.Language_.Structure_.PredefinedOperator[] { 
            QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, QS._qss_x_.Language_.Structure_.PredefinedOperator.Value }, true)]
        public bool Value
        {
            get 
            {
                if (!_isdefined)
                    throw new Exception("Value not defined");
                return _value; 
            }

            set 
            {
                _value = value;
                _isdefined = true;
            }
        }

        #endregion

        #region ISerializable Members

        public unsafe QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get { return new QS.Fx.Serialization.SerializableInfo((ushort) QS.ClassID.Fx_Runtime_Bool, 2 * sizeof(bool)); }
        }

        public unsafe void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            fixed (byte* _pheader = header.Array)
            {
                bool* pheader = (bool*)(_pheader + header.Offset);
                pheader[0] = _isdefined;
                pheader[1] = _value;
            }
            header.consume(2 * sizeof(bool));
        }

        public unsafe void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            fixed (byte* _pheader = header.Array)
            {
                bool* pheader = (bool*)(_pheader + header.Offset);
                _isdefined = pheader[0];
                _value = pheader[1];
            }
            header.consume(2 * sizeof(bool));
        }

        #endregion

        #region ToString

        public override string ToString()
        {
            if (_isdefined)
                return _value.ToString();
            else
                return "undefined";
        }

        #endregion

        #region IValue<Bool> Members

        [QS._qss_x_.Language_.Structure_.Operator("IsDefined", QS._qss_x_.Language_.Structure_.PredefinedOperator.IsDefined, true)]
        public bool IsDefined
        {
            get { return _isdefined; }
        }

        public void SetTo(BOOL value)
        {
            _isdefined = value._isdefined;
            _value = value._value;
        }

        public BOOL Clone()
        {
            return new BOOL(_isdefined, _value);
        }

        public BOOL Erase()
        {
            throw new NotImplementedException();
        }

        public bool IsLess(BOOL other)
        {
            return !_isdefined || (other._isdefined && (!_value || other._value));
        }

        #endregion
    }
}
