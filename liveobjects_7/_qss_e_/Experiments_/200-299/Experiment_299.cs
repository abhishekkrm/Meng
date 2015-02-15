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
using System.Threading;
using System.Net;

using QS._qss_e_.Parameters_.Specifications;

namespace QS._qss_e_.Experiments_
{
    public class Experiment_299 : Experiment_200
    {
        protected override void experimentWork(QS._core_c_.Components.IAttributeSet results)
        {
        }

        protected new class Application : Experiment_200.Application
        {
            public Application(QS._qss_c_.Platform_.IPlatform platform, QS._core_c_.Components.AttributeSet args)
                : base(platform, args)
            {
                Microsoft.Win32.RegistryKey key1 = 
                    Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Policies\Microsoft\Windows", true);
                if (key1 == null)
                    throw new Exception("Cannot open HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows");

                Microsoft.Win32.RegistryKey key2 = key1.OpenSubKey("WindowsUpdate", true);
                if (key2 == null)
                    key2 = key1.CreateSubKey("WindowsUpdate");

                Microsoft.Win32.RegistryKey key3 = key2.OpenSubKey("AU", true);
                if (key3 == null)
                    key3 = key2.CreateSubKey("AU");

                key3.SetValue("NoAutoUpdate", 1, Microsoft.Win32.RegistryValueKind.DWord);
            }

            public override void TerminateApplication(bool smoothly)
            {
            }

            public override void Dispose()
            {
            }
        }

        public Experiment_299()
        {
        }

        protected override Type ApplicationClass
        {
            get { return typeof(Application); }
        }
    }
}
