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

namespace QS._qss_d_.Monitoring_
{
    public static class Analyzer
    {
        public static IEnumerable<IAnalyzerClass> GetAnalyzers()
        {
            List<IAnalyzerClass> analyzers = new List<IAnalyzerClass>();
            foreach (Type type in System.Reflection.Assembly.GetAssembly(typeof(IAnalyzer)).GetTypes())
            {
                if (typeof(IAnalyzer).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    object[] attributes = type.GetCustomAttributes(typeof(AnalyzerAttribute), true);
                    if (attributes.Length > 0)
                    {
                        string name = ((AnalyzerAttribute) attributes[0]).Name;
                        if (name == null)
                            name = type.Name;

                        analyzers.Add(new AnalyzerClass(name, type));
                    }
                }
            }
            return analyzers;
        }

        #region Class AnalyzerClass

        private class AnalyzerClass : IAnalyzerClass
        {
            public AnalyzerClass(string name, Type type)
            {
                this.name = name;
                this.type = type;
            }

            private string name;
            private Type type;

            public override string ToString()
            {
                return name;
            }

            #region IAnalyzerClass Members

            IAnalyzer IAnalyzerClass.GetAnalyzer()
            {
                return (IAnalyzer)(type.GetConstructor(Type.EmptyTypes)).Invoke(new object[] { });
            }

            #endregion
        }

        #endregion
    }
}
