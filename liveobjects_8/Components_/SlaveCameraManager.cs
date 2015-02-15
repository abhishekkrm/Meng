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
        "A04EB4CDF06B44e990651B13737E1FF1", "SlaveCameraManager",
        "Translates no mouse or keyboard input.  Simply relays positions from the camera.")]
    class SlaveCameraManager : ICameraManager, ICameraManagerOps
    {

        private QS.Fx.Endpoint.Internal.IExportedInterface<ICameraManagerOps> windowendpoint;

        private QS.Fx.Endpoint.Internal.IImportedInterface<ICameraOps> cameraendpoint;
        private QS.Fx.Endpoint.IConnection cameraconnection;

        public SlaveCameraManager(
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
            UpdateWindowEvent uwe = this.cameraendpoint.Interface.GetState();
            return uwe;
        }


        #region ICameraManagerOps Members

        UpdateWindowEvent ICameraManagerOps.UpdateCamera(Window_XStateEvent e)
        {
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
