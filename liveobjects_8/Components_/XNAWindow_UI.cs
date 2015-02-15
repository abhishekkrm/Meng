/* Copyright (c) 2009 Jared Cantwell. All rights reserved.

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

#define NUCLEX_IS_UNAVAILABLEe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

#if XNA
using Microsoft.Xna.Framework;
using MapLibrary;

namespace Demo
{
#if NUCLEX_IS_UNAVAILABLE
#else
    [QS.Fx.Reflection.ComponentClass(
        "D5821C252A3642edB46E09DD9BFE1CE2", "XNAWindow_UI", "Implements an XNA Window in a UI component, not an XNA Window.")]
    public partial class XNAWindow_UI : Nuclex.GameControl, QS.Fx.Object.Classes.IUI
    {
        #region Fields

        private XnaWindowHandler handler;

        private QS.Fx.Endpoint.Internal.IExportedUI myendpoint;

        #endregion

        #region Constructor

        public XNAWindow_UI(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Dictionary", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.IDictionary<
                        String, QS.Fx.Object.Classes.IObject>> _folder,
            [QS.Fx.Reflection.Parameter("CameraManager", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<ICameraManager> cameraManager,
            [QS.Fx.Reflection.Parameter("loader", QS.Fx.Reflection.ParameterClass.Value)] 
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IService<QS.Fx.Interface.Classes.ILoader<QS.Fx.Object.Classes.IObject>>> _loader,
            [QS.Fx.Reflection.Parameter("GeoDiscoveryService", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<IGeoDiscoveryClient> _discClient,
            [QS.Fx.Reflection.Parameter("Resizeable", QS.Fx.Reflection.ParameterClass.Value)]
            bool resizeable,
            [QS.Fx.Reflection.Parameter("NearClip", QS.Fx.Reflection.ParameterClass.Value)]
            Single nearClip,
            [QS.Fx.Reflection.Parameter("FarClip", QS.Fx.Reflection.ParameterClass.Value)]
            Single farClip,
            [QS.Fx.Reflection.Parameter("WindowWidth", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 windowWidth,
            [QS.Fx.Reflection.Parameter("WindowHeight", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 windowHeight) 
        {
            this.myendpoint = _mycontext.ExportedUI(this);
            Control form = this;

            handler = new XnaWindowHandler(graphics, form, this.Services, _mycontext, _folder, cameraManager, _loader, _discClient, 
                resizeable, nearClip, farClip, windowWidth, windowHeight);

            this.Disposed += new EventHandler(XNAWindow_UI_Disposed);
            this.MouseWheel += new MouseEventHandler(XNAWindow_UI_MouseWheel);
        }

        void XNAWindow_UI_MouseWheel(object sender, MouseEventArgs e)
        {
            handler.SetMouseWheelDelta((float)e.Delta);
        }

        #endregion

        #region Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            handler.Initialize();
            base.Initialize();
        }
        #endregion

        #region Load/Unload Content
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            handler.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadGraphicsContent(bool loadAllContent)
        {
            handler.UnloadContent();
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            handler.Update(gameTime);
            base.Update(gameTime);
        }

        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            handler.Draw(gameTime);

            base.Draw(gameTime);
        }
        #endregion

        #region Form Close Event

        void XNAWindow_UI_Disposed(object sender, EventArgs e)
        {
         	handler.Cleanup();
        }
        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return myendpoint; }
        }

        #endregion
    }
#endif    
}

#endif
