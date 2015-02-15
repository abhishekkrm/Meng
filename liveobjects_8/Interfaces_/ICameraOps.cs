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

#if XNA
using Demo.Xna;

namespace Demo
{
    [QS.Fx.Reflection.InterfaceClass("23`1", "Interface for interacting with an Xna camera")]
    public interface ICameraOps : QS.Fx.Interface.Classes.IInterface
    {
        [QS.Fx.Reflection.Operation("LookVertical")]
        void LookVertical (float radians, bool allowCrossOver);

        [QS.Fx.Reflection.Operation("LookSideways")]
        void LookSideways (float radians);

        [QS.Fx.Reflection.Operation("ChangeAltitude")]
        void ChangeAltitute(float units, bool fixedReference);

        [QS.Fx.Reflection.Operation("SetAltitude")]
        void SetAltitude(float altitude, bool fixedReference);

        [QS.Fx.Reflection.Operation("SetPosition")]
        void SetPosition(Vector3 point);

        [QS.Fx.Reflection.Operation("SetForwardVector")]
        void SetForwardVector(Vector3 v);

        [QS.Fx.Reflection.Operation("SetUpVector")]
        void SetUpVector(Vector3 v);

        // Not sure if this can be general
        [QS.Fx.Reflection.Operation("VerticalRotationAroundPoint")]
        void VerticalRotationAroundPoint(Vector3 point, float radians);

        [QS.Fx.Reflection.Operation("HorizontalRotationAroundPoint")]
        void HorizontalRotationAroundPoint(Vector3 point, float radians);

        // Not sure if this can be general
        [QS.Fx.Reflection.Operation("VerticalRotationAroundReference")]
        void VerticalRotationAroundReference(float radians);

        [QS.Fx.Reflection.Operation("HorizontalRotationAroundReference")]
        void HorizontalRotationAroundReference(float radians);

        [QS.Fx.Reflection.Operation("ZoomToReference")]
        void ZoomToReference(float units);
        
        // dunno about this
        [QS.Fx.Reflection.Operation("ZoomToPoint")]
        void ZoomToPoint(Vector3 point, float units);

        // Could there be a better interface for things like dragging?
        // Right now its not quite like this interface
        [QS.Fx.Reflection.Operation("MoveForward")]
        void MoveForward(float units);

        [QS.Fx.Reflection.Operation("MoveSideways")]
        void MoveSideways(float units);

        [QS.Fx.Reflection.Operation("Pitch")]
        void Pitch(float units);

        [QS.Fx.Reflection.Operation("Yaw")]
        void Yaw(float units);

        [QS.Fx.Reflection.Operation("Roll")]
        void Roll(float units);

        [QS.Fx.Reflection.Operation("TranslateAbsloute")]
        void TranslateAbsolute(Vector3 translation);

        [QS.Fx.Reflection.Operation("GetState")]
        UpdateWindowEvent GetState();

        [QS.Fx.Reflection.Operation("BroadcastState")]
        void BroadcastState();
    }
}
#endif
