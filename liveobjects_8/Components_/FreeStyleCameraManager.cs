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
using System.IO;

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
        "AB1EF1C9BF92474790F8FFA9D65EFE0F", "FreeStyleCameraManager",
        "Translates Map Events into appropriate actions for the camera.")]
    class FreeStyleCameraManager : ICameraManager, ICameraManagerOps
    {

        private QS.Fx.Endpoint.Internal.IExportedInterface<ICameraManagerOps> windowendpoint;

        private QS.Fx.Endpoint.Internal.IImportedInterface<ICameraOps> cameraendpoint;
        private QS.Fx.Endpoint.IConnection cameraconnection;

        private InputState inputState;

        // State transtion variables
        private float lastMouseX = 0.0f;
        private float lastMouseY = 0.0f;
        private float lastScrollWheel = 0.0f;
        private Vector3 pointRotationAnchor = Vector3.Zero;
        private bool showMouse = true;

        // Constraints
        private const float MIN_Z = 1.0f;
        private const float MAX_Z = 75000.0f;

        // Configuration Parameters
        private float ZOOM_RATE = .4f;
        private int SEND_RATE = 1;
        private int sendCount = 0;

        Vector3 cameraReference;
        Vector3 cameraPosition;
        Vector3 cameraUp;

        public FreeStyleCameraManager(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Camera", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<ICamera> camera)
        {
            this.windowendpoint = _mycontext.ExportedInterface<ICameraManagerOps>(this);

            this.cameraendpoint = _mycontext.ImportedInterface<ICameraOps>();
            this.cameraconnection = this.cameraendpoint.Connect(camera.Dereference(_mycontext).Camera);
        }

        private UpdateWindowEvent Update()
        {
            UpdateZoom();
            UpdateTurn();
            UpdateDrag();
            UpdateAnchorRotation();

            lastMouseX = inputState.mouseState.X;
            lastMouseY = inputState.mouseState.Y;
            lastScrollWheel = inputState.mouseState.ScrollWheelValue;

            UpdateWindowEvent uwe = this.cameraendpoint.Interface.GetState();

            cameraPosition = uwe.CameraPosition.Vector3_;
            cameraReference = uwe.CameraReference.Vector3_;
            cameraUp = uwe.CameraUp.Vector3_;

            if (sendCount++ % SEND_RATE == 0)
                this.cameraendpoint.Interface.BroadcastState();

            return uwe;
        }

        void UpdateZoom()
        {
            float delta;

            if (inputState.mouseWheelDelta != 0.0f)
            {
                delta = inputState.mouseWheelDelta;
            }
            else
            {
                delta = -1.0f * (lastScrollWheel - inputState.mouseState.ScrollWheelValue);
            }

            delta *= ZOOM_RATE * cameraPosition.Z/1000;

            this.cameraendpoint.Interface.ZoomToReference(delta);
        }

        void UpdateTurn()
        {
            if (inputState.keyboardState.IsKeyDown(Keys.Up))
            {
                this.cameraendpoint.Interface.MoveForward(100f);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Down))
            {
                this.cameraendpoint.Interface.MoveForward(-100f);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Left))
            {
                this.cameraendpoint.Interface.MoveSideways(-100f);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Right))
            {
                this.cameraendpoint.Interface.MoveSideways(100f);
            }
        }

        bool wasPressed = false;

        void UpdateDrag()
        {
            if (inputState.mouseState.LeftButton == ButtonState.Pressed)
            {
                if (!wasPressed)
                {
                    wasPressed = true;
                }
                else
                {
                    float deltaX = inputState.mouseState.X - lastMouseX;
                    float deltaY = inputState.mouseState.Y - lastMouseY;

                    //Vector3 referenceNormalXY = Vector3.Normalize(new Vector3(cameraReference.X, cameraReference.Y, 0));
                    Vector3 referenceNormal = Vector3.Normalize(cameraReference);

                    // move more land the higher up and the shallower the viewing angle
                    float zFactor = cameraPosition.Z * .0015f / Math.Abs(referenceNormal.Z);

                    this.cameraendpoint.Interface.MoveForward(deltaY * zFactor);
                    this.cameraendpoint.Interface.MoveSideways(-deltaX * zFactor);
                }
            }
            else
            {
                wasPressed = false;
            }
        }

        void UpdateAnchorRotation()
        {
            if (inputState.mouseState.RightButton == ButtonState.Pressed)
            {
                showMouse = false;

                this.cameraendpoint.Interface.VerticalRotationAroundReference(-(inputState.mouseState.Y - lastMouseY) * 0.005f);
                this.cameraendpoint.Interface.HorizontalRotationAroundReference(-(inputState.mouseState.X - lastMouseX) * 0.005f);
            }
            else
            {
                showMouse = true;
            }
        }


        #region ICameraManagerOps Members

        UpdateWindowEvent ICameraManagerOps.UpdateCamera(Window_XStateEvent e)
        {
            lock (this)
            {
                inputState = e.InputState;
                return Update();
            }
        }

        #endregion

        #region ICameraManager Members

        QS.Fx.Endpoint.Classes.IExportedInterface<ICameraManagerOps> ICameraManager.CameraManager
        {
            get { return this.windowendpoint; }
        }

        #endregion


    }
}
#endif
