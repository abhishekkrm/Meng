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

namespace QS._qss_x_.Channel_.Message_
{
    /// <summary>
    /// Describes the position, velocity, rotation and angular velocity of an object, as measured at a specified point in time.
    /// The rotation is given with respect to the default position, where the object's front is directed towards the positive half 
    /// of the "X" axis, the left side of the object is directed towards the positive half of the "Y" axis, and the top of the object 
    /// is directed towards the positive half of the "Z" axis, and the rotation is always increasing when going clockwise, as in 
    /// the "rule of thumb". The coordinates are always given in the metric system, in meters, meter/s, radians, and radians/s.
    /// </summary>
    [QS.Fx.Reflection.ValueClass(QS.Fx.Reflection.ValueClasses.Coordinates, "Coordinates",
        "A value that represents coordinates, speed and orientation of a nobject in a 3-dimensional space. ")]
    public interface ICoordinates : QS.Fx.Serialization.ISerializable
    {
        /// <summary>
        /// The time when the coordinates have been measured.
        /// </summary>
        float TM
        {
            get;
            set;
        }

        /// <summary>
        /// The "X" coordinate of the object's position in meters.
        /// </summary>
        float PX
        {
            get;
            set;
        }

        /// <summary>
        /// The "Y" coordinate of the object's position in meters.
        /// </summary>
        float PY
        {
            get;
            set;
        }

        /// <summary>
        /// The "Z" coordinate of the object's position in meters.
        /// </summary>
        float PZ
        {
            get;
            set;
        }

        /// <summary>
        /// The object's speed along the "X" axis in meter per second.
        /// </summary>
        float DX
        {
            get;
            set;
        }

        /// <summary>
        /// The object's speed along the "Y" axis in meters per second.
        /// </summary>
        float DY
        {
            get;
            set;
        }

        /// <summary>
        /// The object's speed along the "Z" axis in meters per second.
        /// </summary>
        float DZ
        {
            get;
            set;
        }

        /// <summary>
        /// The object's clockwise rotation along the "X" axis in radians.
        /// </summary>
        float RX
        {
            get;
            set;
        }

        /// <summary>
        /// The object's clockwise rotation along the "Y" axis in radians.
        /// </summary>
        float RY
        {
            get;
            set;
        }

        /// <summary>
        /// The object's clockwise rotation along the "Z" axis in radians.
        /// </summary>
        float RZ
        {
            get;
            set;
        }

        /// <summary>
        /// The angular velocity in radians per second of the object's rotation along the "X" axis.
        /// </summary>
        float AX
        {
            get;
            set;
        }

        /// <summary>
        /// The angular velocity in radians per second of the object's rotation along the "Y" axis.
        /// </summary>
        float AY
        {
            get;
            set;
        }

        /// <summary>
        /// The angular velocity in radians per second of the object's rotation along the "Z" axis.
        /// </summary>
        float AZ
        {
            get;
            set;
        }
    }
}
