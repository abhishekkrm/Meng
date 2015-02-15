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

namespace QS._qss_e_.Experiment_.Helpers
{
    public static class Args
    {
        public static bool BoolOf(QS._core_c_.Components.IAttributeSet args, string name)
        {
            return BoolOf(args, name, false);
        }

        public static bool BoolOf(QS._core_c_.Components.IAttributeSet args, string name, bool defaultValue)
        {
            if (args.contains(name))
            {
                object valueObject = args[name];
                if (valueObject == null)
                    return true;
                else
                {
                    if (valueObject.Equals("yes") || valueObject.Equals("enabled") || valueObject.Equals("true") || valueObject.Equals("on"))
                        return true;
                    else if (valueObject.Equals("no") || valueObject.Equals("disabled") || valueObject.Equals("false") || valueObject.Equals("off"))
                        return false;
                    else
                        throw new Exception("Unrecognized boolean parameter \"" + name + "\": " +
                            QS._core_c_.Helpers.ToString.Object(valueObject));
                }
            }
            else
                return defaultValue;
        }

        public static int Int32Of(QS._core_c_.Components.IAttributeSet args, string name, int defaultValue)
        {
            return args.contains(name) ? Convert.ToInt32((string) args[name]) : defaultValue;
        }

        public static double DoubleOf(QS._core_c_.Components.IAttributeSet args, string name, double defaultValue)
        {
            return args.contains(name) ? Convert.ToDouble((string)args[name]) : defaultValue;
        }
    }
}
