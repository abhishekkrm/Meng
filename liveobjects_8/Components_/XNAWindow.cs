/*

Copyright (c) 2007-2009 Jared Cantwell (jmc279@cornell.edu), Petko Nikolov (pn42@cornell.edu), Krzysztof Ostrowski (krzys@cs.cornell.edu). All rights reserved.

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
using System.IO;
using System.Xml.Serialization;
using System.Text;
using System.Threading;

#if XNA
using Microsoft.Xna.Framework;

using System.Windows.Forms;
using MapLibrary;

namespace Demo
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    [QS.Fx.Reflection.ComponentClass(
        "C312BA6E943345baA586A9263E840CC6", "XNAWindow", "Implements an XNA Window")]
    public class XnaWindow : Microsoft.Xna.Framework.Game, QS.Fx.Object.Classes.IWindow_X
    {
        #region Fields

        private XnaWindowHandler handler;

        #endregion

        #region Constructor

        public XnaWindow(
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
            GraphicsDeviceManager graphics = new GraphicsDeviceManager(this);
            Control form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);

            handler = new XnaWindowHandler(graphics, form, this.Services, _mycontext, _folder, cameraManager, _loader, _discClient, 
                resizeable, nearClip, farClip, windowWidth, windowHeight);

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = resizeable;

            if (windowWidth <= 0) windowWidth = 600;
            graphics.PreferredBackBufferWidth = windowWidth;

            if (windowHeight <= 0) windowHeight = 600;
            graphics.PreferredBackBufferHeight = windowHeight;
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
        protected override void LoadContent()
        {
            handler.LoadContent();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
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

        #region OnExiting
        protected override void OnExiting(object sender, EventArgs args)
        {
            handler.Cleanup();
            base.OnExiting(sender, args);
        }
        #endregion
    }

}

#endif

