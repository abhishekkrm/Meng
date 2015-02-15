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
using CovarianceRequests_ = QS._qss_x_.Experiment_.Component_.PCA_.CovarianceRequests_;
namespace QS._qss_x_.Experiment_.Interface_
{
    [QS.Fx.Reflection.InterfaceClass("983EC48B7D5C4488BDC4DC6D6175013C")]
    public interface IPCA_ : QS.Fx.Interface.Classes.IInterface
    {
        //[QS.Fx.Reflection.Operation("Work")]
        //[QS.Fx.Serialization.Serializable]
        //void _Work(PCAStep_ _step, double[][] _rows, double[] _means);

        [QS.Fx.Reflection.Operation("Mean")]
        [QS.Fx.Serialization.Serializable]
        void _Work_Mean(double[] _rows, int _start, int _size);

        [QS.Fx.Reflection.Operation("Covariance")]
        [QS.Fx.Serialization.Serializable]
        void _Work_Covariance(CovarianceRequests_ _cov_reqests);

        [QS.Fx.Reflection.Operation("Done with Covariance")]
        void _Done_Covariance();

        [QS.Fx.Reflection.Operation("Done with Mean")]
        void _Done_Mean();
    }
}
