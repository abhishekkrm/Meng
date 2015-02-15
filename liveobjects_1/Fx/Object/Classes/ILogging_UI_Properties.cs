/*

Copyright (c) 2009 Revant Kapoor, Yilin Qin, Krzysztof Ostrowski. All rights reserved.

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
using System.Linq;
using System.Text;

namespace QS.Fx.Object.Classes
{
    [QS.Fx.Reflection.ObjectClass(QS.Fx.Reflection.ObjectClasses.Logging_UI_Properties, "Logging UI Properties", "A class of objects with a 2-dimensional graphical user interface that may log messages and exposes properties.")]
    public interface ILogging_UI_Properties : IObject
    {
        [QS.Fx.Reflection.Endpoint("UI")]
        QS.Fx.Endpoint.Classes.IExportedUI UI
        {
            get;
        }

        [QS.Fx.Reflection.Endpoint("Logging")]
        QS.Fx.Endpoint.Classes.IImportedInterface<QS.Fx.Interface.Classes.ILog> Logging
        {
            get;
        }

        [QS.Fx.Reflection.Endpoint("Properties")]
        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IMetadataClient,
                                              QS.Fx.Interface.Classes.IMetadata> Properties
        {
            get;
        }

    }
}
