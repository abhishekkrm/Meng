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

namespace QS._qss_e_.Data_.Converters_
{
    public class IdentifyLosses : QS._core_e_.Data.IConverter
    {
        public IdentifyLosses()
        {
        }

        #region IConverter Members

        string QS._core_e_.Data.IConverter.Name
        {
            get { return "Identify Losses"; }
        }

        KeyValuePair<string, QS._core_e_.Data.IDataSet>[] QS._core_e_.Data.IConverter.Convert(KeyValuePair<string, QS._core_e_.Data.IDataSet>[] arguments)
        {
            if (arguments.Length != 1 || !(arguments[0].Value is QS._core_e_.Data.DataSeries))
                throw new Exception("Bad arguments");

            QS._core_e_.Data.DataSeries receiveTimes = (QS._core_e_.Data.DataSeries)arguments[0].Value;

            QS._core_e_.Data.XYSeries indicator;
            QS._core_e_.Data.DataSeries losses;
            QS._qss_e_.Postprocessing_.Losses.Calculate(receiveTimes, out indicator, out losses);

            return new KeyValuePair<string, QS._core_e_.Data.IDataSet>[] 
            {
                new KeyValuePair<string, QS._core_e_.Data.IDataSet>("LossIndicator(" + arguments[0].Key + ")", indicator),
                new KeyValuePair<string, QS._core_e_.Data.IDataSet>("Losses(" + arguments[0].Key + ")", losses)
            };
        }

        #endregion
    }
}
