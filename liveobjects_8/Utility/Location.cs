/* Copyright (c) 2009 Jared Cantwell, Petko Nikolov. All rights reserved.

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

using System.Text;

namespace MapLibrary
{
    [QS.Fx.Reflection.ValueClass("805A7752903E486a9B98B37F94CE9BF3", "Location", "Specifies a map location by a latitude, longitude, and zoom level")]
    public struct Location
    {
        public Location(float lat, float lon, float alt)
        {
            Latitude = lat;
            Longitude = lon;
            Altitude = alt;
        }

        public static bool operator ==(Location x, Location y)
        {
            return ((x.Latitude == y.Latitude) &&
                    (x.Latitude == y.Longitude) &&
                    (x.Altitude == y.Altitude));
        }

        public static bool operator !=(Location x, Location y)
        {
            return ((x.Latitude != y.Latitude) ||
                    (x.Latitude != y.Longitude) ||
                    (x.Altitude != y.Altitude));
        }

        public String toString()
        {
            return Latitude + " " + Longitude + " " + Altitude;
        }

        public float Latitude;
        public float Longitude;
        public float Altitude;
    }
}
