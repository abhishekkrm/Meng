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
using System.Reflection;

namespace QS._qss_c_.Infrastructure2.Components.Types
{
    public sealed class GroupTypeLibrary: Interfaces.Types.IGroupTypeLibrary
    {
        public GroupTypeLibrary()
        {
            RegisterAllKnownTypes();
        }

        private IDictionary<string, Interfaces.Types.IGroupType> registeredTypes = new Dictionary<string, Interfaces.Types.IGroupType>();

        private void RegisterAllKnownTypes()
        {
            lock (this)
            {
                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        if (typeof(Interfaces.Types.IGroupType).IsAssignableFrom(type))
                        {
                            ConstructorInfo constructorInfo = type.GetConstructor(Type.EmptyTypes);
                            if (constructorInfo != null)
                            {
                                Interfaces.Types.IGroupType groupType = (Interfaces.Types.IGroupType) constructorInfo.Invoke(new object[] {});
                                registeredTypes.Add(groupType.Name, groupType);
                            }
                        }
                    }
                }
            }
        }

        public void Register(string type_name, QS._qss_c_.Infrastructure2.Interfaces.Types.IGroupType groupType)
        {
            lock (this)
            {
                registeredTypes.Add(type_name, groupType);
            }
        }

        #region IGroupTypeLibrary Members

        bool QS._qss_c_.Infrastructure2.Interfaces.Types.IGroupTypeLibrary.GetType(
        string type_name, out QS._qss_c_.Infrastructure2.Interfaces.Types.IGroupType groupType)
        {
            lock (this)
            {
                return registeredTypes.TryGetValue(type_name, out groupType);
            }
        }

        #endregion
    }
}
