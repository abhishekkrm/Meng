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
using Microsoft.Win32;
using System.Reflection;

namespace QS._qss_x_.ActiveX_
{
    public static class Registration
    {
        private const string TypeLib = "{ce90f8eb-ef21-4fdf-a31e-9ac12e26f52d}";
        private const string Version = "1.0";

        public static void Register(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);
            if (k != null)
            {
                RegistryKey ctrl = k.CreateSubKey("Control");
                ctrl.Close();
                RegistryKey insertable = k.CreateSubKey("Insertable");
                insertable.Close();
                RegistryKey typelib = k.CreateSubKey("TypeLib");
                typelib.SetValue(null, QS._qss_x_.ActiveX_.Registration.TypeLib);
                typelib.Close();
                RegistryKey version = k.CreateSubKey("Version");
                version.SetValue(null, QS._qss_x_.ActiveX_.Registration.Version);
                version.Close();
                RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
                inprocServer32.SetValue("CodeBase", Assembly.GetExecutingAssembly().CodeBase);
                inprocServer32.Close();
                k.Close();
            }
        }

        public static void Unregister(string key)
        {
            StringBuilder sb = new StringBuilder(key);
            sb.Replace(@"HKEY_CLASSES_ROOT\", "");
            RegistryKey k = Registry.ClassesRoot.OpenSubKey(sb.ToString(), true);
            if (k != null)
            {
                k.DeleteSubKey("Control", false);
                k.DeleteSubKey("Insertable");
                k.DeleteSubKey("TypeLib");
                k.DeleteSubKey("Version");
                RegistryKey inprocServer32 = k.OpenSubKey("InprocServer32", true);
                k.DeleteSubKey("CodeBase", false);
                k.Close();
            }
        }
    }
}
