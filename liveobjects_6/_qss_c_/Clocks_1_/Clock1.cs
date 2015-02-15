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

namespace QS._qss_c_.Clocks_1_
{
/*
    public unsafe static class Clock1
    {
        public static void GetSamples(int nsamples, out double[] samples1, out double[] samples2)
        {
            samples1 = new double[nsamples];
            samples2 = new double[nsamples];
            for (int ind = 0; ind < nsamples; ind++)
            {
                ulong t1, t2;
                uint cpuid;
                QS.CMS.Base3.CPUClock.GetCpuNoTSC(&t1, &t2, &cpuid);
                samples1[ind] = (double) (t2 - t1);
                samples2[ind] = (double) cpuid;
                System.Threading.Thread.Sleep(0);
            }

            // Native.Win32.FixedThread thread1; // , thread2;
            // thread1 = new QS.CMS.Native.Win32.FixedThread(0, new QS.CMS.Native.Win32.FixedThread.Callback(ThreadCallback1), null);
            // thread2 = new QS.CMS.Native.Win32.FixedThread(1, new QS.CMS.Native.Win32.FixedThread.Callback(ThreadCallback2), null);
        }

        private static void ThreadCallback1(Native.Win32.FixedThread thread, object argument)
        {
//            long time1;
            
        }


//        private static void ThreadCallback2(Native.Win32.FixedThread thread, object argument)
//        {
//        } 
    }
*/ 
}
