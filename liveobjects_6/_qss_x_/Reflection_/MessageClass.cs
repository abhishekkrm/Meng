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

#define DEBUG_INCLUDE_INSPECTION_CODE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Reflection_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class MessageClass : QS.Fx.Inspection.Inspectable, QS.Fx.Reflection.IMessageClass
    {
        #region Constructor

        public MessageClass(IDictionary<string, QS.Fx.Reflection.IValue> _values)
        {
            this._values = _values;
        }

        #endregion

        #region Fields

#if DEBUG_INCLUDE_INSPECTION_CODE
        [QS.Fx.Printing.Printable("values")]
#endif
        private IDictionary<string, QS.Fx.Reflection.IValue> _values;

        #endregion

        #region Inspection

#if DEBUG_INCLUDE_INSPECTION_CODE

        [QS.Fx.Base.Inspectable("values")]
        private QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IValue> __inspectable_values
        {
            get
            {
                return new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IValue>("values", _values,
                    new QS._qss_e_.Inspection_.DictionaryWrapper1<string, QS.Fx.Reflection.IValue>.ConversionCallback(
                        delegate(string s) { return s; }));
            }
        }

#endif

        #endregion

        #region IMessageClass Members

        IDictionary<string, QS.Fx.Reflection.IValue> QS.Fx.Reflection.IMessageClass.Values
        {
            get { return new QS._qss_x_.Base1_.ReadonlyDictionaryOf<string, QS.Fx.Reflection.IValue>(_values); }
        }

        QS.Fx.Reflection.IMessageClass QS.Fx.Reflection.IMessageClass.Instantiate(IEnumerable<QS.Fx.Reflection.IParameter> _parameters)
        {
            Dictionary<string, QS.Fx.Reflection.IValue> _new_values = new Dictionary<string, QS.Fx.Reflection.IValue>();
            foreach (QS.Fx.Reflection.IValue _value in this._values.Values)
                _new_values.Add(_value.ID, _value.Instantiate(_parameters));
            return new MessageClass(_new_values);
        }

        bool QS.Fx.Reflection.IMessageClass.IsSubtypeOf(QS.Fx.Reflection.IMessageClass other)
        {
            if (ReferenceEquals(this, other))
                return true;
            foreach (KeyValuePair<string, QS.Fx.Reflection.IValue> element in other.Values)
            {
                QS.Fx.Reflection.IValue myvalue, othervalue;
                if (!this._values.TryGetValue(element.Key, out myvalue))
                    return false;
                othervalue = element.Value;
                if (!myvalue.ValueClass.IsSubtypeOf(othervalue.ValueClass))
                    return false;
            }
            return true;
        }

        #endregion
    }
}
