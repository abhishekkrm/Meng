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
using System.Linq;
using System.Text;

namespace QS._qss_x_.Experiment_.Interface_
{
    [QS.Fx.Reflection.InterfaceClass("E8F2B293AD634686BA3A9A77BF7FDEF2")]
    [QS._qss_x_.Reflection_.Internal]
    public interface IPriorityQueue_ : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Warmup")]
        [QS.Fx.Serialization.Serializable]
        void _Warmup(int _count);

        [QS.Fx.Reflection.Operation("Enqueue")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time);

        [QS.Fx.Reflection.Operation("Enqueue2")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2);

        [QS.Fx.Reflection.Operation("Enqueue4")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2, double _time3, double _time4);

        [QS.Fx.Reflection.Operation("Enqueue8")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8);

        [QS.Fx.Reflection.Operation("Enqueue16")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8, 
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16);

        [QS.Fx.Reflection.Operation("Enqueue32")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16,
            double _time17, double _time18, double _time19, double _time20, double _time21, double _time22, double _time23, double _time24,
            double _time25, double _time26, double _time27, double _time28, double _time29, double _time30, double _time31, double _time32);

        [QS.Fx.Reflection.Operation("Enqueue64")]
        [QS.Fx.Serialization.Serializable]
        void _Enqueue(double _time1, double _time2, double _time3, double _time4, double _time5, double _time6, double _time7, double _time8,
            double _time9, double _time10, double _time11, double _time12, double _time13, double _time14, double _time15, double _time16,
            double _time17, double _time18, double _time19, double _time20, double _time21, double _time22, double _time23, double _time24,
            double _time25, double _time26, double _time27, double _time28, double _time29, double _time30, double _time31, double _time32,
            double _time33, double _time34, double _time35, double _time36, double _time37, double _time38, double _time39, double _time40,
            double _time41, double _time42, double _time43, double _time44, double _time45, double _time46, double _time47, double _time48,
            double _time49, double _time50, double _time51, double _time52, double _time53, double _time54, double _time55, double _time56,
            double _time57, double _time58, double _time59, double _time60, double _time61, double _time62, double _time63, double _time64);

        [QS.Fx.Reflection.Operation("Dequeue")]
        [QS.Fx.Serialization.Serializable]
        void _Dequeue(int _count);

        [QS.Fx.Reflection.Operation("Done")]
        [QS.Fx.Serialization.Serializable]
        void _Done();
    }
}
