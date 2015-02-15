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

// #define OPTION_AUTOMATICALLY_REGISTER_ALL_POSSIBLE_COMPONENTS

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace QS._qss_x_.Base1_
{
    public static class Assemblies
    {
        // Optional components to load should be in folder ..\components, relative to the executable.
        public static readonly string _COMPONENTS_ROOT =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ + Path.DirectorySeparatorChar + "components";

        private static readonly string _MESSAGES_FILE =
            QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_ +
                Path.DirectorySeparatorChar + "logs" + Path.DirectorySeparatorChar + "QuickSilver.Message_Log.txt";

        private static List<Assembly> _assemblies = new List<Assembly>();
        private static bool _registered = false;

        public static IEnumerable<Assembly> RegisteredAssemblies
        {
            get 
            {
                lock (typeof(Assemblies))
                {
                    if (!_registered)
                    {
//                        StringBuilder _assemblies_report = new StringBuilder();

                        foreach (Assembly _assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            string _name = _assembly.GetName().Name;
                            bool _isok = 
                                _name.Equals("liveobjects_1") ||
                                _name.Equals("liveobjects_4") ||
                                _name.Equals("liveobjects_5") ||
                                _name.Equals("liveobjects_6") ||
                                _name.Equals("liveobjects_7") ||
                                _name.Equals("liveobjects_8") ||
                                _name.Equals("liveobjects_9");
                            if (_isok)
                                _assemblies.Add(_assembly);
//                            _assemblies_report.AppendLine((_isok ? "+ " : "- ") + _name);
                        }
//                        System.Windows.Forms.MessageBox.Show(_assemblies_report.ToString());

#if OPTION_AUTOMATICALLY_REGISTER_ALL_POSSIBLE_COMPONENTS
                        if (Directory.Exists(_COMPONENTS_ROOT))
                        {
                            foreach (string _filename in Directory.GetFiles(_COMPONENTS_ROOT, "*.dll"))
                            {
                                try
                                {
                                    _assemblies.Add(Assembly.LoadFrom(_filename));
                                }
                                catch (Exception _exc)
                                {
                                    throw new Exception("Cannot load components from \"" + _filename + "\".", _exc);
                                }
                            }
                        }
#endif

                        try
                        {
                            using (StreamWriter _w = new StreamWriter(_MESSAGES_FILE))
                            {
                                foreach (Assembly _assembly in _assemblies)
                                {
                                    _w.WriteLine("Assembly: \"" + _assembly.FullName + "\"");
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        _registered = true;
                    }
                    _assemblies.Sort(
                        new Comparison<Assembly>(
                            delegate(Assembly x, Assembly y)
                            {
                                return x.FullName.CompareTo(y.FullName);
                            }));
                    return _assemblies;
                }
            }
        }

        public static void RegisterAssembly(System.Reflection.Assembly _assembly)
        {
            lock (typeof(Assemblies))
            {
                if (!_registered)
                {
                    _assemblies.Add(_assembly);
                }
                else
                    throw new Exception("Cannot register the assembly at this time through this interface, it should have been done earlier.");
            }
        }
    }
}
