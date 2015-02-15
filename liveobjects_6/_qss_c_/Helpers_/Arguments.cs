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

namespace QS._qss_c_.Helpers_
{
    public static class Arguments
    {
        public static C GetValue<C>(IDictionary<string, object> arguments, string name)
        {
            object value;
            if (!arguments.TryGetValue(name, out value))
                throw new Exception("Argument \"" + name + "\" not defined.");

            if (value is C)
                return (C)value;
            else
                throw new Exception("Argument \"" + name + "\" is not of type \"" + typeof(C).Name + "\".");
        }

        public static C GetValue<C>(IDictionary<string, object> arguments, string name, C defaultValue)
        {
            object value;
            return (arguments.TryGetValue(name, out value) && value is C) ? ((C) value) : defaultValue;
        }

        public static bool GetValue<C>(IDictionary<string, object> arguments, string name, out C value)
        {
            object obj;
            if (arguments.TryGetValue(name, out obj) && obj is C)
            {
                value = (C) obj;
                return true;
            }
            else
            {
                value = default(C);
                return false;
            }
        }
    }
}
