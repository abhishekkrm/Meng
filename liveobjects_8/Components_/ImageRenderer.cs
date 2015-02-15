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
using System.Drawing;
using System.Net;

using MapLibrary;

using System.Windows.Forms;
using System.Threading;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
        "17FDC74B86F54737BFB4C051BFD2F1D4", "ImageRenderer", "Implements an XNA renderer for images on an earthly plane.")]
    public class ImageRenderer : QS.Fx.Object.Classes.IUI_X, IContentRendererOps, IDisposable
    {

        #region Fields
        // For displaying the image
        private Microsoft.Xna.Framework.Graphics.Effect _effect;
        private Microsoft.Xna.Framework.Matrix _modelmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

        GraphicsDevice graphics;
        List<ImageData> providerData;
        private bool disposingEffect = false;

        //****** LiveObjects variables******
        private QS.Fx.Endpoint.Internal.IExportedUI_X windowendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<IMapManagerOps, IContentRendererOps> providerendpoint;
        private QS.Fx.Endpoint.IConnection providerconnection;

        //private System.IO.StreamWriter file;
        private GridTranslator gridTrans;

        private IMapManager providerRef;

        private bool deviceResetting;

        #endregion

        #region Constructor

        public ImageRenderer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("ImageProvider", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<IMapManager> provider)
        {
            //file = new System.IO.StreamWriter("c:\\debug.txt", true);

            this.windowendpoint = _mycontext.ExportedUI_X(
                new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback),
                new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback),
                new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this.windowendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this.windowendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            if (provider != null)
            {
                providerRef = provider.Dereference(_mycontext);

                this.providerendpoint = _mycontext.DualInterface<IMapManagerOps, IContentRendererOps>(this);
                this.providerconnection = this.providerendpoint.Connect(providerRef.ContentRenderer);
            }
            

            providerData = new List<ImageData>();
            gridTrans = new GridTranslator();
        }

        #endregion

        #region _EFFECT_SOURCE

        private const string _effect_source =
@"
struct VertexToPixel
{
    float4 Position    : POSITION;
    float4 Color  : COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- Constants --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter =
LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV =
mirror;};


//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL,
float2 inTexCoords: TEXCOORD0)
{
 VertexToPixel Output = (VertexToPixel)0;
 float4x4 preViewProjection = mul (xView, xProjection);
 float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);

 Output.Position = mul(inPos, preWorldViewProjection);
 Output.TextureCoords = inTexCoords;

 float3 Normal = normalize(mul(normalize(inNormal), xWorld));
 Output.LightingFactor = 1;
 if (xEnableLighting)
  Output.LightingFactor = dot(Normal, -xLightDirection);

 return Output;
}

PixelToFrame TexturedPS(VertexToPixel PSIn)
{
 PixelToFrame Output = (PixelToFrame)0;

 Output.Color = tex2D(TextureSampler,
PSIn.TextureCoords)*clamp(PSIn.LightingFactor + xAmbient,0,1);

 return Output;
}

technique Textured
{
 pass Pass0
    {
     VertexShader = compile vs_1_1 TexturedVS();
        PixelShader  = compile ps_1_1 TexturedPS();
    }
}
";

        #endregion

        #region _VERTICES

        private static Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] _MAKE_VERTICES(float x, float y, float w, float h)
        {
            Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] _vertices = new Microsoft.Xna.Framework.Graphics.VertexPositionTexture[6];
            _vertices[0].Position = new Microsoft.Xna.Framework.Vector3(x, y + h, 0f);
            _vertices[0].TextureCoordinate.X = 0;
            _vertices[0].TextureCoordinate.Y = 0;

            _vertices[1].Position = new Microsoft.Xna.Framework.Vector3(x + w, y, 0f);
            _vertices[1].TextureCoordinate.X = 1;
            _vertices[1].TextureCoordinate.Y = 1;

            _vertices[2].Position = new Microsoft.Xna.Framework.Vector3(x, y, 0f);
            _vertices[2].TextureCoordinate.X = 0;
            _vertices[2].TextureCoordinate.Y = 1;

            _vertices[3].Position = new Microsoft.Xna.Framework.Vector3(x + w, y, 0f);
            _vertices[3].TextureCoordinate.X = 1;
            _vertices[3].TextureCoordinate.Y = 1;

            _vertices[4].Position = new Microsoft.Xna.Framework.Vector3(x, y + h, 0f);
            _vertices[4].TextureCoordinate.X = 0;
            _vertices[4].TextureCoordinate.Y = 0;

            _vertices[5].Position = new Microsoft.Xna.Framework.Vector3(x + w, y + h, 0f);
            _vertices[5].TextureCoordinate.X = 1;
            _vertices[5].TextureCoordinate.Y = 0;

            return _vertices;
        }

        #endregion

        #region _UIConnectCallback

        public void _UIConnectCallback()
        {
            graphics = this.windowendpoint.GraphicsDevice.GraphicsDevice;
            graphics.DeviceResetting += new EventHandler<EventArgs>(graphics_DeviceResetting);
            graphics.DeviceReset += new EventHandler<EventArgs>(graphics_DeviceReset); 
            graphics_DeviceReset(null, null);
        }

        void graphics_DeviceReset(object sender, EventArgs e)
        {
            lock (this)
            {
                /*Microsoft.Xna.Framework.Content.Pipeline.Graphics. _compiledeffect =
                    Microsoft.Xna.Framework.Graphics.Effect.CompileEffectFromSource(
                    _effect_source, null, null,
                    Microsoft.Xna.Framework.Graphics.CompilerOptions.None,
                    Microsoft.Xna.Framework.TargetPlatform.Windows);*/

                this._effect = new Microsoft.Xna.Framework.Graphics.Effect(
                    graphics, null);
                this._effect.CurrentTechnique = this._effect.Techniques["Textured"];
                this._effect.Disposing += new EventHandler<EventArgs>(delegate(object _sender, EventArgs _e)
                    { disposingEffect = true; });

                this._modelmatrix = Microsoft.Xna.Framework.Matrix.CreateRotationX(0f);
                this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);
                deviceResetting = false;
            }
        }

        void graphics_DeviceResetting(object sender, EventArgs e)
        {
            lock (this)
            {
                deviceResetting = true;
            }
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion

        #region _RepositionCallback

        int count = 0;
        Microsoft.Xna.Framework.Matrix _lastCameramatrix;
        Microsoft.Xna.Framework.Matrix _lastProjectionmatrix;


        public void _RepositionCallback(
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            // if the camera hasn't changed, don't bother notifying anyone
            //if (_lastCameramatrix.Equals(_cameramatrix) && _lastProjectionmatrix.Equals(_projectionmatrix))
                //return;

            //_lastCameramatrix = _cameramatrix;
            //_lastProjectionmatrix = _projectionmatrix;

            //MessageBox.Show("ImageRenderer: RepositionCallback()");

            lock (this)
            {

                if (count++ % 5 == 0)
                {
                    this.providerendpoint.Interface.CurrentLocation(gridTrans.MatixToViewPoint(_cameramatrix, _projectionmatrix));
                }

                this._cameramatrix = _cameramatrix;
                this._projectionmatrix = _projectionmatrix;
                this._effect.Parameters["xView"].SetValue(this._cameramatrix);
                this._effect.Parameters["xProjection"].SetValue(this._projectionmatrix);
            }
        }

        #endregion

        #region _UpdateCallback

        private void _UpdateCallback(Microsoft.Xna.Framework.GameTime _time)
        {
        }

        #endregion

        
        void IContentRendererOps.RenderContent(Content content, Location whereToPlace, float stretchWidth, float stretchHeight)
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\debug.txt", true);
            //file.WriteLine("RenderContent - Start: " + DateTime.Now.Second + "," + DateTime.Now.Millisecond);



            if (content.content.GetType().Equals(typeof(Bitmap)))
            {
                Bitmap b = (Bitmap)content.content;

                Texture tx = null;
                using (MemoryStream s = new MemoryStream())
                {
                    b.Save(s, System.Drawing.Imaging.ImageFormat.Png);
                    s.Seek(0, SeekOrigin.Begin); //must do this, or error is thrown in next line
                    tx = Texture2D.FromStream(graphics, s, b.Width,b.Height,false);
                }
                //info += whereToPlace.Latitude + " " + whereToPlace.Longitude + Environment.NewLine;
                ImageData temp = new ImageData(tx, _MAKE_VERTICES(whereToPlace.Longitude, whereToPlace.Latitude, stretchWidth, stretchHeight));


                lock (providerData)
                {
                    if (!providerData.Contains(temp))
                        providerData.Add(temp);
                }
            }
            else
            {
                throw new Exception("Renderer does not support this type.");
            }
           // file.WriteLine("RenderContent - End: " + DateTime.Now.Second + "," + DateTime.Now.Millisecond);
           // file.Flush();
            //file.Close();
        
        

           
             
        }


        void IContentRendererOps.FlushContent()
        {
            lock (providerData)
            {
                providerData.Clear();
            }
        }
    

        #region _DrawCallback

        public void _DrawCallback(
            Microsoft.Xna.Framework.GameTime _time)
        {
            lock (this)
            {
                if (!deviceResetting && !this._effect.IsDisposed && !disposingEffect)
                {
                    //this._effect.Begin();

                    foreach (Microsoft.Xna.Framework.Graphics.EffectPass _pass in this._effect.CurrentTechnique.Passes)
                    {
                        /*graphics.VertexDeclaration =
                            new Microsoft.Xna.Framework.Graphics.VertexDeclaration(
                                graphics, Microsoft.Xna.Framework.Graphics.VertexPositionTexture.VertexElements);*/

                        lock (providerData)
                        {
                            foreach (ImageData i in providerData)
                            {
                                this._effect.Parameters["xTexture"].SetValue(i.texture);

                                //_pass.Begin();
                                graphics.SetVertexBuffers(new VertexBuffer(graphics, typeof(VertexPositionTexture), i.vertices.Length, BufferUsage.None));
                                graphics.DrawUserPrimitives(
                                        Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, i.vertices, 0, 2);
                                //_pass.End();
                            }
                        }
                    }

                    //this._effect.End();
                }
            }
        }

        #endregion

        public struct ImageData
        {
            public Texture texture;
            public Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] vertices;

            public ImageData(Texture t, Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] v)
            {
                texture = t;
                vertices = v;
            }

            public override bool Equals(object obj)
            {
                ImageData d = (ImageData) obj;

                for (int i = 0; i < 6; i++)
                    if (!vertices[i].Equals(d.vertices[i]))
                        return false;

                return true;
            }
        }

        #region IUI_X Members

        QS.Fx.Endpoint.Classes.IExportedUI_X QS.Fx.Object.Classes.IUI_X.UI
        {
            get { return this.windowendpoint; }
        }

        #endregion

        ~ImageRenderer()
        {
            this._Dispose(false);
        }

        private int _disposed;

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                    if (providerRef != null && providerRef is IDisposable)
                        ((IDisposable)providerRef).Dispose();

                }
            }
        }
    }
}
#endif
