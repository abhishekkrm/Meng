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

#if RELEASE3

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
        "26`1", "PilotCameraManager",
        "Translate Map Events into appropriate actions for a pilot camera.")]
    class PilotCameraManager : ICameraManager, ICameraManagerOps
    {

        private QS.Fx.Endpoint.Internal.IExportedInterface<ICameraManagerOps> windowendpoint;

        private QS.Fx.Endpoint.Internal.IImportedInterface<ICameraOps> cameraendpoint;
        private QS.Fx.Endpoint.IConnection cameraconnection;

        bool master = false;
        private InputState inputState;

        // State transtion variables
        private float lastMouseX = 0.0f;
        private float lastMouseY = 0.0f;
        private float lastScrollWheel = 0.0f;
        private bool showMouse = true;

        // Constraints
        private const float MIN_Z = 100.0f;
        private const float MAX_Z = 15000.0f;

        // Configuration Parameters
        private float SPEED = 10f;
        private float SPEED_RATE = 1f;
        private float TURN_RATE = 0.025f;

        Vector3 cameraReference = Vector3.Normalize(new Vector3(0, 1, -4));
        Vector3 cameraPosition = new Vector3(8000, 8000, 20000);
        Vector3 cameraUp = Vector3.Normalize(new Vector3(0, 1, 9));

        public PilotCameraManager(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Camera", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<ICamera> camera,
            [QS.Fx.Reflection.Parameter("master", QS.Fx.Reflection.ParameterClass.Value)]
            bool master)
        {
            this.master = master;
            this.windowendpoint = _mycontext.ExportedInterface<ICameraManagerOps>(this);

            this.cameraendpoint = _mycontext.ImportedInterface<ICameraOps>();
            this.cameraconnection = this.cameraendpoint.Connect(camera.Dereference(_mycontext).Camera);
        }

        private UpdateWindowEvent Update()
        {
            UpdateZoom();
            UpdateTurn();

            lastMouseX = inputState.mouseState.X;
            lastMouseY = inputState.mouseState.Y;
            lastScrollWheel = inputState.mouseState.ScrollWheelValue;

            UpdateWindowEvent uwe = this.cameraendpoint.Interface.GetState();

            cameraPosition = uwe.CameraPosition.Vector3_;
            cameraReference = uwe.CameraReference.Vector3_;

            this.cameraendpoint.Interface.BroadcastState();

            return uwe;
        }

        void UpdateZoom()
        {
            if (inputState.keyboardState.IsKeyDown(Keys.U))
                SPEED += SPEED_RATE;
            if (inputState.keyboardState.IsKeyDown(Keys.D))
                SPEED -= SPEED_RATE;
            
            this.cameraendpoint.Interface.ZoomToReference(SPEED);
        }

        void UpdateTurn()
        {
            if (inputState.keyboardState.IsKeyDown(Keys.Up))
            {
                this.cameraendpoint.Interface.Pitch(TURN_RATE);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Down))
            {
                this.cameraendpoint.Interface.Pitch(-TURN_RATE);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Left))
            {
                this.cameraendpoint.Interface.Roll(-TURN_RATE);
            }
            if (inputState.keyboardState.IsKeyDown(Keys.Right))
            {
                this.cameraendpoint.Interface.Roll(TURN_RATE);
            }
        }



        #region ICameraManagerOps Members

        UpdateWindowEvent ICameraManagerOps.UpdateCamera(Window_XStateEvent e)
        {
            inputState = e.InputState;
            return Update();
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

#endif
