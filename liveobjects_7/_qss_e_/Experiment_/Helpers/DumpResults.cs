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
    public static class DumpResults
    {
        public static void Dump(QS.Fx.Logging.ILogger logger, QS._core_c_.Components.AttributeSet attributes, string root, string key)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("\nRegistered Sample Collectors\n{");
            foreach (QS._qss_c_.Statistics_.Samples1D samples in QS._qss_c_.Statistics_.Samples1D.Registered)
                s.AppendLine("    1D : " + samples.ToString());
            foreach (QS._qss_c_.Statistics_.Samples2D samples in QS._qss_c_.Statistics_.Samples2D.Registered)
                s.AppendLine("    2D : " + samples.ToString());
            s.AppendLine("}");
            logger.Log(null, s.ToString());

            logger.Log(null, "\nCollected Attributes\n" + QS._core_c_.Helpers.ToString.Attributes(attributes));

            System.IO.Directory.CreateDirectory(root);
            QS._core_e_.Repository.IRepository repository = new QS._qss_e_.Repository_.Repository(root);
            repository.Add(key, attributes);
        }
    }
}
