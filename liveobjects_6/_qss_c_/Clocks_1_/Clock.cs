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
using System.Runtime.InteropServices;

namespace QS._qss_c_.Clocks_1_
{
/*
    public unsafe static class Clock
    {
        #region Calculating offsets

        public static Base3.CPUClock[] SynchronizeClocks()
        {
            int nprocessors = Native.Win32.NumberOfProcessors;
            Console.WriteLine("Number of Processors: " + nprocessors.ToString());

            Base3.CPUClock cpuClock = new QS.CMS.Base3.CPUClock();

            ThreadContext[] threads = new ThreadContext[nprocessors];
            for (int ind = 0; ind < threads.Length; ind++)
                threads[ind] = new ThreadContext((uint)ind, threads);

            threads[0].Calculate();

            Base3.CPUClock[] result = new QS.CMS.Base3.CPUClock[threads.Length];
            for (int ind = 0; ind < threads.Length; ind++)
                result[ind] = threads[ind].Clock;

            foreach (ThreadContext thread in threads)
                thread.Dispose();

            return result;
        }

        #region Class ThreadContext

        private class ThreadContext : IDisposable
        {
            public ThreadContext(uint processorNumber, ThreadContext[] threads)
            {
                this.processorNumber = processorNumber;
                this.threads = threads;
                coordinator = (processorNumber == 0);

                uint id, mask = (uint)(1 << ((int) processorNumber));
                handle = Native.Win32.CreateThread(null, 0, new Native.Win32.ThreadProc(this.MyCallback), null,
                    (uint)Native.Win32.ProcessCreationFlags.CREATE_SUSPENDED, &id);

                Native.Win32.SetThreadIdealProcessor(handle, processorNumber);
                Native.Win32.SetThreadAffinityMask(handle, &mask);

                Native.Win32.ResumeThread(handle);
                ready.WaitOne();
            }

            private uint processorNumber;
            private Base3.CPUClock clock;
            private ThreadContext[] threads;
            private IntPtr handle;
            private bool coordinator, finishing = false;
            private System.Threading.ManualResetEvent ready = new System.Threading.ManualResetEvent(false);
            private System.Threading.AutoResetEvent check = new System.Threading.AutoResetEvent(false);
            private System.Threading.AutoResetEvent checkComplete = new System.Threading.AutoResetEvent(false);
            private double time;

            #region Callback

            private int MyCallback(uint* lpParam)
            {
                while (Native.Win32.GetCurrentProcessorNumber() != processorNumber)
                    System.Threading.Thread.Sleep(0);

                this.clock = new QS.CMS.Base3.CPUClock();

                Console.WriteLine("Thread(" + processorNumber.ToString() + ") ready, time = " + Internal_GetTime().ToString());

                ready.Set();

                while (true)
                {
                    check.WaitOne();
                    if (finishing)
                        break;

                    if (coordinator)
                    {
                        for (int ind = 1; ind < threads.Length; ind++)
                        {
                            ThreadContext otherContext = threads[ind];

                            int nsamples = 10000;
                            int nadjustments = 10;

                            double min_d1, min_d2;

                            for (int ind3 = 0; ind3 < nadjustments; ind3++)
                            {
                                min_d1 = min_d2 = double.MaxValue;

                                for (int ind2 = 0; ind2 < nsamples; ind2++)
                                {
                                    double t1 = Internal_GetTime();
                                    double t2 = otherContext.GetTime();
                                    double t3 = Internal_GetTime();
                                    double d1 = t2 - t1;
                                    double d2 = t3 - t2;

                                    if (d1 < min_d1)
                                        min_d1 = d1;
                                    if (d2 < min_d2)
                                        min_d2 = d2;
                                }

                                double calculated_offset = (min_d1 - min_d2) / 2;

                                Console.WriteLine("Offset " + processorNumber.ToString() + "-" + ind.ToString() + " = " +
                                    calculated_offset.ToString("000000.000000000000") + "(" +
                                    min_d1.ToString("000000.000000000000") + ", " +
                                    min_d2.ToString("000000.000000000000") + ")");

                                otherContext.Clock.Adjust(-calculated_offset);
                            }
                        }

                        checkComplete.Set();
                    }
                    else
                    {
                        Internal_GetTime();
                        checkComplete.Set();
                    }
                }

                Console.WriteLine("Thread(" + processorNumber.ToString() + ") stops, time = " + Internal_GetTime().ToString());

                return 0;
            } 

            #endregion

            public double GetTime()
            {                
                check.Set();
                if (!checkComplete.WaitOne(1000, false))
                    throw new Exception("Failed.");
                return time;
            }

            private double Internal_GetTime()
            {
                do
                {
                    while (Native.Win32.GetCurrentProcessorNumber() != processorNumber)
                        System.Threading.Thread.Sleep(0);
                    time = clock.Time;
                }
                while (Native.Win32.GetCurrentProcessorNumber() != processorNumber);
                return time;
            }

            public void Calculate()
            {
                check.Set();
                checkComplete.WaitOne();
            }

            public Base3.CPUClock Clock
            {
                get { return clock; }
            }

            public uint ProcessorNumber
            {
                get { return processorNumber; }
            }

            #region IDisposable Members

            public void Dispose()
            {
                finishing = true;
                check.Set();
                Native.Win32.CloseHandle(handle);
            }

            #endregion
        }

        #endregion

        #endregion
    }
*/ 
}
