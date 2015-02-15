/* Copyright (c) 2009 Petko Nikolov. All rights reserved.

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
SUCH DAMAGE. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MapLibrary
{
#if XNA
    [QS.Fx.Reflection.InterfaceClass("D210EE24CAF948a0976719232C8B7D69", "IGeoDiscoveryClientOps")]
    public interface IGeoDiscoveryClientOps : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("Register")]
        void Register(String key, String objXML);

        [QS.Fx.Reflection.Operation("UpdateLocation")]
        void UpdateLocation(String key, Location loc, float minZoom, float maxZoom);

        [QS.Fx.Reflection.Operation("GetObjectKeys")]
        GeoDiscoveryObjects GetObjectKeys(float top, float bottom, float left, float right, float zoomLevel);

        [QS.Fx.Reflection.Operation("Delete")]
        void Delete(String key);
    }
#endif
}
