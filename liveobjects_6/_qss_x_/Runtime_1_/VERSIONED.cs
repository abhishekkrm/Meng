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
    [QS._qss_x_.Language_.Structure_.ValueTypeTemplate("VERSIONED", QS._qss_x_.Language_.Structure_.PredefinedTypeTemplate.VERSIONED)]
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Native)]
    public struct VERSIONED<C> : IVersioned<C>, IValue<VERSIONED<C>> where C : IValue<C>, new()
    {
        #region Constructor

        public VERSIONED(VERSION version, C value)
        {
            this.version = version;
            this.value = value;
        }

        #endregion

        #region Fields

        [QS.Fx.Printing.Printable]
        private VERSION version;
        [QS.Fx.Printing.Printable]
        private C value;

        #endregion

        #region IVersioned<C> Members

        [QS._qss_x_.Language_.Structure_.Operator("Version", QS._qss_x_.Language_.Structure_.PredefinedOperator.Version, true)]
        public VERSION Version
        {
            get { return version; }
            set { version = value; }
        }

        [QS._qss_x_.Language_.Structure_.Operator("Value", new QS._qss_x_.Language_.Structure_.PredefinedOperator[] { 
            QS._qss_x_.Language_.Structure_.PredefinedOperator.Create, QS._qss_x_.Language_.Structure_.PredefinedOperator.Value }, true)]
        public C Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region ISerializable Members

        public QS.Fx.Serialization.SerializableInfo SerializableInfo
        {
            get 
            {
                QS.Fx.Serialization.SerializableInfo info = new QS.Fx.Serialization.SerializableInfo(QS.ClassID.Fx_Runtime_Versioned);
                info.AddAnother(version.SerializableInfo);
                if (version.IsDefined)
                    info.AddAnother(value.SerializableInfo);
                return info;
                
            }
        }

        public void SerializeTo(ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
        {
            version.SerializeTo(ref header, ref data);
            if (version.IsDefined)
                value.SerializeTo(ref header, ref data);
        }

        public void DeserializeFrom(ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
        {
            version.DeserializeFrom(ref header, ref data);
            if (version.IsDefined)
            {
                value = new C();
                value.DeserializeFrom(ref header, ref data);
            }
            else
                value = default(C);
        }

        #endregion

        #region Clone

        public VERSIONED<C> Clone()
        {
            throw new NotImplementedException();
            // return new VERSIONED<C>(version, (value != null) ? value.Clone() : null);
        }

        #endregion

        #region UpdateTo

        public bool UpdateTo(VERSIONED<C> other)
        {
            if (other.version.CompareTo(this.version) > 0)
            {
                if (this.value != null)
                    this.value.SetTo(other.value);
                else
                    this.value = other.value.Clone();
                this.version = other.version;
                return true;
            }
            else
                return false;
        }

        #endregion

        #region System.Object Overrides

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion

        #region MonotonicCheck

        public static void MonotonicCheck(IVersioned<C> x, IVersioned<C> y)
        {
            if ((x.Version.CompareTo(y.Version) <= 0) && !x.Value.IsLess(y.Value))
                System.Diagnostics.Debug.Assert(false, "MonotonicCheck : Failed\nvalue1 = " + x.ToString() + "\nvalue2 = " + y.ToString());
        }

        #endregion

        #region IValue<VERSIONED<C>> Members

        bool IValue<VERSIONED<C>>.IsDefined
        {
            get { throw new NotImplementedException(); }
        }

        void IValue<VERSIONED<C>>.SetTo(VERSIONED<C> value)
        {
            throw new NotImplementedException();
        }

        VERSIONED<C> IValue<VERSIONED<C>>.Clone()
        {
            throw new NotImplementedException();
        }

        VERSIONED<C> IValue<VERSIONED<C>>.Erase()
        {
            throw new NotImplementedException();
        }

        bool IValue<VERSIONED<C>>.IsLess(VERSIONED<C> other)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
