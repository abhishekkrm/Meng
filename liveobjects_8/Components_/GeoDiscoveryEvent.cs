/*

Copyright (c) 2004-2009 Petko Nikolov. All rights reserved.

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
using System.Xml.Serialization;

#if XNA
using Demo.Xna;
#endif

namespace MapLibrary
{
#if XNA
    

    [QS.Fx.Reflection.ValueClass("FD7D5B7A3D054e5a8166143F8F01C9B6", "GeoDiscoveryEvent", "An event passed between GeoDiscovery servers")]
    public class GeoDiscoveryEvent
    {
        public enum EventType { UpdatePosition, Delete };

        [XmlElement]
        public EventType myEvent;

        [XmlElement]
        public String key;

        [XmlElement]
        public Vector3 loc;

        [XmlAttribute]
        public float minZoom;

        [XmlAttribute]
        public float maxZoom;

        public GeoDiscoveryEvent(EventType e, String key, Vector3 loc, float minZoom, float maxZoom)
        {
            this.myEvent = e;
            this.key = key;
            this.loc = loc;
            this.minZoom = minZoom;
            this.maxZoom = maxZoom;
        }
        
        public GeoDiscoveryEvent()
        {
        }
    }

#endif
}
