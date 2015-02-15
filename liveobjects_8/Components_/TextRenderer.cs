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

using System;
using System.Collections.Generic;
using System.Text;

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Net;

using MapLibrary;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
        "7EF14D00333343da8E09A71AB6C4491C", "TextRenderer", "Implements an XNA renderer for text")]
    public class TextRenderer : QS.Fx.Object.Classes.IUI_X, ITextRendererOps
    {
        #region Constructor
        public TextRenderer(
            QS.Fx.Object.IContext _mycontext,
             [QS.Fx.Reflection.Parameter("TextProvider", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<ITextRendererClient> client)
        {
            this.windowendpoint = _mycontext.ExportedUI_X(
                    new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback),
                    new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback),
                    new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this.windowendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this.windowendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            if (client != null)
            {
                this.clientendpoint = _mycontext.DualInterface<ITextRendererClientOps, ITextRendererOps>(this);
                this.clientconnection = this.clientendpoint.Connect(client.Dereference(_mycontext).TextRendererClient);
            }

            gridTrans = new GridTranslator();
            textList = new List<Text>();
        }
        #endregion

        #region Fields


        // LiveObjects variables
        private QS.Fx.Endpoint.Internal.IExportedUI_X windowendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<ITextRendererClientOps, ITextRendererOps> clientendpoint;
        private QS.Fx.Endpoint.IConnection clientconnection;

        private Microsoft.Xna.Framework.Matrix _lastCameramatrix;
        private Microsoft.Xna.Framework.Matrix _lastProjectionmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix, _worldmatrix;

        // Drawing objects
        GraphicsDevice graphics;
        GridTranslator gridTrans;
        SpriteBatch spriteBatch;
        Texture2D bgTexture;
        Texture2D pinpoint;
        bool usePinpoint = false;

        /// XNAExtra BitmapFont
        SpriteFont spriteFont;

        //List of all text to draw on the screen
        private List<Text> textList;


        #endregion

        #region Text Class
        private struct Text
        {
            public SpriteFont font;
            public String[] textRows;
            public Vector3 location;
            public Color textColor;
            public Color backgroundColor;
            private int maxWidth;
            private bool widthDone;
            public int MaxWidth
            {
                get {
                    if (widthDone)
                        return maxWidth;

                    String temp;
                    int w;

                    for (int i = 0; i < textRows.Length; i++)
                    {
                        temp = textRows[i];
                        w = (int) font.MeasureString(textRows[i]).X;
                        if (w > maxWidth)
                            maxWidth = w;
                    }

                    widthDone = true;
                    return maxWidth;
                }
            }
        }

        #endregion


        #region _UIConnectCallback

        public void _UIConnectCallback()
        {
            graphics = this.windowendpoint.GraphicsDevice.GraphicsDevice;
            spriteBatch = new SpriteBatch(graphics);

            spriteFont = (SpriteFont)this.windowendpoint.Content(new QS.Fx.Xna.ContentRef(QS.Fx.Xna.ContentClass.SpriteFont, "C312BA6E943345baA586A9263E840CC6`1:Kootenay12.xnb")).Content;

            try
            {
                String file = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, "images\\pinpoint.png");
                FileStream s = new FileStream(file, FileMode.Open);
                pinpoint = Texture2D.FromStream(graphics, s);
                usePinpoint = true;
            }
            catch (Exception ex)
            {
            }

            _worldmatrix = Matrix.CreateRotationX(0f);

            float w = 200;
            float h = spriteFont.MeasureString(" ").Y + 5;

            bgTexture = new Texture2D(graphics, (int)w, (int)h);
            Color[] texture = new Color[(int)w * (int)h];
            for (int i = 0; i < texture.Length; ++i)
            {
                texture[i] = Color.White;
            }
            bgTexture.SetData(texture);
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion

        #region _RepositionCallback

        public void _RepositionCallback(
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            // if the camera hasn't changed, don't bother notifying anyone
            if (_lastCameramatrix.Equals(_cameramatrix) && _lastProjectionmatrix.Equals(_projectionmatrix))
                return;

            _lastCameramatrix = _cameramatrix;
            _lastProjectionmatrix = _projectionmatrix;

            lock (this)
            {
                this._cameramatrix = _cameramatrix;
                this._projectionmatrix = _projectionmatrix;

                Location tl = gridTrans.GetTopLeftCornerOfEarthFromMatrix(_cameramatrix, _projectionmatrix);
                Location br = gridTrans.GetBottomRightCornerOfEarthFromMatrix(_cameramatrix, _projectionmatrix);

                if(this.clientendpoint != null && this.clientendpoint.IsConnected)
                    this.clientendpoint.Interface.CurrentLocation(gridTrans.MatixToViewPoint(_cameramatrix, _projectionmatrix), tl, br);
            }
        }

        #endregion

        #region _UpdateCallback

        private void _UpdateCallback(Microsoft.Xna.Framework.GameTime _time)
        {

        }

        #endregion

        #region _DrawCallback

        public void _DrawCallback(
            Microsoft.Xna.Framework.GameTime _time)
        {
            Vector3 d2;

            lock (textList)
            {
                spriteBatch.Begin();
                // The backgrounds need drawn first and then the batch closed, 
                // otherwise the text won't display on top of the background
                for (int i = 0; i < 2; i++)
                {

                    foreach (Text t in textList)
                    {
                        d2 = graphics.Viewport.Project(
                                    new Vector3(t.location.X, t.location.Y, 0f),
                                    _projectionmatrix, _cameramatrix, _worldmatrix);

                        int offsetX = -t.MaxWidth / 2;
                        int pinHeight = usePinpoint ? pinpoint.Height : 0;
                        int offsetY = -(bgTexture.Height * t.textRows.Length + pinHeight);

                        if (i == 0)
                        {
                            Vector2 pos = new Vector2(d2.X + offsetX, d2.Y + offsetY);
                  
                            Color newColor = new Color(1f,1f, 1f);
                            Color borderColor = new Color(0f,0f,0f);
                            int thickness = 3;
                            

                            spriteBatch.Draw(bgTexture, pos + new Vector2((float)-thickness, (float)-thickness), new Rectangle(0, 0, t.MaxWidth + 2 * thickness, bgTexture.Height * t.textRows.Length + 2 * thickness), borderColor, 0f,
                                    Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(bgTexture, pos, new Rectangle(0, 0, t.MaxWidth, bgTexture.Height * t.textRows.Length), newColor, 0f,
                                    Vector2.Zero, 1.0f, SpriteEffects.None, 1f);
                            if(usePinpoint)
                                spriteBatch.Draw(pinpoint, pos + new Vector2((t.MaxWidth / 2) - (pinpoint.Width / 2), bgTexture.Height * t.textRows.Length), null, Color.White, 0f, new Vector2(-pinpoint.Width / 2, 0), 1f, SpriteEffects.None, 1f);

                        }
                        else
                        {
                            for(int j = 0; j < t.textRows.Length; j++)
                                spriteBatch.DrawString(spriteFont, t.textRows[j], new Vector2(d2.X + offsetX, d2.Y + bgTexture.Height * j + offsetY), t.textColor);
                        }
                    }
                        
                }
            
                spriteBatch.End();
            }
        }

        #endregion

        #region ITextRendererOps Members

        void  ITextRendererOps.DrawText(Demo.Xna.Vector3 location, String[] text)//, Microsoft.Xna.Framework.Graphics.Color textColor)
        {
            Text t = new Text();
            t.font = spriteFont;
            t.textRows = text;
            Location l = gridTrans.LatLonToPixel(location.Y, location.X, location.Z);
            t.location = new Vector3(l.Longitude, l.Latitude, l.Altitude);

            t.textColor = Color.Black;
            t.backgroundColor = Color.White;

            lock (textList)
            {
                textList.Add(t);
            }
        }
        /*        
                void  ITextRendererOps.DrawText(Demo.Xna.Vector3 location, String text, Microsoft.Xna.Framework.Graphics.Color textColor, Microsoft.Xna.Framework.Graphics.Color backgroundColor)
                {
                    throw new NotImplementedException();
            
                    Text t = new Text();
                    t.text = text;
                    t.location = gridTrans.LatLonToPixel(location.Y, location.X, location.Z);
                    t.textColor = textColor;
                    t.backgroundColor = backgroundColor;

                    lock (textList)
                    {
                        textList.Add(t);
                    }
             
                }

         */
        void ITextRendererOps.FlushContent()
        {
            textList.Clear();
        }

        #endregion

        #region IUI_X Members

        QS.Fx.Endpoint.Classes.IExportedUI_X QS.Fx.Object.Classes.IUI_X.UI
        {
            get { return windowendpoint; }
        }

        #endregion
    }
}

#endif
