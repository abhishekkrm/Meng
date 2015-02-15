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

using System.Xml.Serialization;

#if XNA
using Demo.Xna;



namespace Demo
{
    [QS.Fx.Reflection.ValueClass(
        "53417B82864647559AF91BC559D741F0", "UpdateCameraEvent",
        "A UI update event.  This event holds information about the state of the XNA Window")]
    public class UpdateWindowEvent
    {
        [XmlElement]
        private Vector3 cameraPosition;
        [XmlElement]
        private Vector3 cameraReference;
        [XmlElement]
        private Vector3 cameraUp;
        [XmlAttribute]
        private bool showMouse;
        [XmlAttribute]
        private Double myId;

        public Double Id { get { return myId; } }

        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        public Vector3 CameraReference
        {
            get { return cameraReference; }
            set { cameraReference = value; }
        }

        public Vector3 CameraUp
        {
            get { return cameraUp; }
            set { cameraUp = value; }
        }

        public bool ShowMouse
        {
            get { return showMouse; }
            set { showMouse = value; }
        }

        public UpdateWindowEvent(Double myId, Vector3 pos, Vector3 reference, Vector3 up, bool showMouse)
        {
            this.cameraPosition = pos;
            this.cameraReference = reference;
            this.cameraUp = up;
            this.showMouse = showMouse;
        }

        public UpdateWindowEvent() { }
    }
}
#endif
