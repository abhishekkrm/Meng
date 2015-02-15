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

namespace QS._qss_c_.Native_
{
    public unsafe static class Win32
    {
        #region Imported from Kernel32.dll
        
        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long lpFrequency);

        [DllImport("kernel32.dll")]
        public static extern UInt32 GetCurrentProcessorNumber();

        [DllImport("kernel32.dll")]
        public static extern UInt32 SetThreadIdealProcessor(IntPtr hThread, UInt32 dwIdealProcessor);

        public delegate int ThreadProc(uint* parameter);

        public enum ProcessCreationFlags : uint
        {
            CREATE_SUSPENDED = 4
        }

        [DllImport("kernel32.dll")]
        public static extern IntPtr CreateThread(uint* lpThreadAttributes,
            int dwStackSize, ThreadProc lpStartAddress, uint* lpParameter, uint dwCreationFlags, uint* lpThreadId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll")]
        public static extern uint* SetThreadAffinityMask(IntPtr hThread, uint* dwThreadAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern int GetProcessAffinityMask(IntPtr hProcess,
            out ulong lpProcessAffinityMask, out ulong lpSystemAffinityMask);

        [DllImport("kernel32.dll")]
        public static extern int SetProcessAffinityMask(IntPtr hProcess, ulong dwProcessAffinityMask);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemInfo
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [DllImport("kernel32.dll")]
        public static extern void GetSystemInfo(out SystemInfo lpSystemInfo);

        #endregion

        #region Processor-Related

        public static int NumberOfProcessors
        {
            get
            {
                SystemInfo info;
                GetSystemInfo(out info);
                return (int) info.dwNumberOfProcessors;
            }
        }

        public static int CurrentProcessorNumber
        {
            get { return (int)GetCurrentProcessorNumber(); }
        }

        #endregion

        #region Class FixedThread

        public class FixedThread : System.IDisposable
        {
            public delegate void Callback(FixedThread thread, object argument);

            public FixedThread(int processorNumber, Callback callback, object argument)
            {
                this.processorNumber = processorNumber;
                this.callback = callback;
                this.argument = argument;
                
                uint id;                
                handle = CreateThread(null, 0, new ThreadProc(this.MyCallback), null, 
                    (uint)Native_.Win32.ProcessCreationFlags.CREATE_SUSPENDED, &id);

                uint mask = (uint)(1 << processorNumber);
                SetThreadAffinityMask(handle, &mask);
                if (mask == 0)
                    throw new Exception("Could set thread affinity mask.");
                unchecked
                {
                    if (SetThreadIdealProcessor(handle, (uint)processorNumber) == ((uint)(-1)))
                        throw new Exception("Could not set thread ideal processor.");
                }

                ResumeThread(handle);
                if (!ready.WaitOne())
                    throw new Exception("Could not resume thread.");
            }

            private int processorNumber;
            private IntPtr handle;
            private System.Threading.ManualResetEvent ready = new System.Threading.ManualResetEvent(false);
            private Callback callback;
            private object argument;

            #region Accessors

            public int ProcessorNumber
            {
                get { return processorNumber; }
            }

            #endregion

            #region MyCallback

            private int MyCallback(uint* lpParam)
            {
                while (GetCurrentProcessorNumber() != processorNumber)
                    System.Threading.Thread.Sleep(0);
                ready.Set();

                callback(this, argument);

                return 0;
            }
            
            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                CloseHandle(handle);
            }

            #endregion
        }

        #endregion
    }
}
