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

namespace QS._qss_x_.Component_.Classes_
{
#if XNA
    [QS.Fx.Reflection.ComponentClass(
        QS.Fx.Reflection.ComponentClasses.Image_3D, "Image_3D", "Implements a 3-dimensional image displayable in XNA windows.")]
    public sealed class Image_3D
        : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IUI_X
    {
        #region Constructor

        public Image_3D(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter(
                "imagename",
                "The identifier of the resource that contains the texture for the model.",
                QS.Fx.Reflection.ParameterClass.Value)] string _imagename)
        {
            this._imagename = _imagename;

            this._uiendpoint = _mycontext.ExportedUI_X(
                new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback), 
                new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback), 
                new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this._uiendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this._uiendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedUI_X _uiendpoint;
        [QS.Fx.Base.Inspectable]
        private string _imagename;
        private Microsoft.Xna.Framework.Graphics.Effect _effect;
        private QS.Fx.Xna.IContent _content;
        private Microsoft.Xna.Framework.Graphics.Texture2D _texture;
        private Microsoft.Xna.Framework.Matrix _modelmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

        #endregion

        #region _EFFECT_SOURCE

        private const string _effect_source =
@"
struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
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
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


//------- Technique: Textured --------

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
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
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords)*clamp(PSIn.LightingFactor + xAmbient,0,1);

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

        private static readonly Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] _vertices = _MAKE_VERTICES();
        private const float _SIZE = 18000.0f;
        private static Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] _MAKE_VERTICES()
        {
            Microsoft.Xna.Framework.Graphics.VertexPositionTexture[] _vertices = new Microsoft.Xna.Framework.Graphics.VertexPositionTexture[6];

            _vertices[0].Position = new Microsoft.Xna.Framework.Vector3(-_SIZE, +_SIZE, 0f);
            _vertices[0].TextureCoordinate.X = 0;
            _vertices[0].TextureCoordinate.Y = 0;

            _vertices[1].Position = new Microsoft.Xna.Framework.Vector3(+_SIZE, -_SIZE, 0f);
            _vertices[1].TextureCoordinate.X = 1;
            _vertices[1].TextureCoordinate.Y = 1;

            _vertices[2].Position = new Microsoft.Xna.Framework.Vector3(-_SIZE, -_SIZE, 0f);
            _vertices[2].TextureCoordinate.X = 0;
            _vertices[2].TextureCoordinate.Y = 1;

            _vertices[3].Position = new Microsoft.Xna.Framework.Vector3(+_SIZE, -_SIZE, 0f);
            _vertices[3].TextureCoordinate.X = 1;
            _vertices[3].TextureCoordinate.Y = 1;

            _vertices[4].Position = new Microsoft.Xna.Framework.Vector3(-_SIZE, +_SIZE, 0f);
            _vertices[4].TextureCoordinate.X = 0;
            _vertices[4].TextureCoordinate.Y = 0;

            _vertices[5].Position = new Microsoft.Xna.Framework.Vector3(+_SIZE, +_SIZE, 0f);
            _vertices[5].TextureCoordinate.X = 1;
            _vertices[5].TextureCoordinate.Y = 0;

            return _vertices;
        }

        #endregion

        #region IUI_X Members

        QS.Fx.Endpoint.Classes.IExportedUI_X QS.Fx.Object.Classes.IUI_X.UI
        {
            get { return this._uiendpoint; }
        }

        #endregion

        #region _UIConnectCallback

        private void _UIConnectCallback()
        {
            /*Microsoft.Xna.Framework.Graphics.CompiledEffect _compiledeffect =
                Microsoft.Xna.Framework.Graphics.Effect.CompileEffectFromSource(
                    _effect_source, null, null,
                    Microsoft.Xna.Framework.Graphics.CompilerOptions.None,
                    Microsoft.Xna.Framework.TargetPlatform.Windows);*/
            
            this._effect = new Microsoft.Xna.Framework.Graphics.Effect(
                this._uiendpoint.GraphicsDevice.GraphicsDevice,null);
            this._effect.CurrentTechnique = this._effect.Techniques["Textured"];

            this._content = this._uiendpoint.Content(new QS._qss_x_.Xna_.ContentRef(QS.Fx.Xna.ContentClass.Texture2D, _imagename));
            this._texture = (Microsoft.Xna.Framework.Graphics.Texture2D) this._content.Content;
            this._effect.Parameters["xTexture"].SetValue(this._texture);

            this._modelmatrix = Microsoft.Xna.Framework.Matrix.CreateRotationX(0f);
            this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion

        #region _RepositionCallback

        private void _RepositionCallback
        (
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix
        )
        {
            lock (this)
            {
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

        #region _DrawCallback

        private void _DrawCallback(
            Microsoft.Xna.Framework.GameTime _time)
        {
            lock (this)
            {
                if (this._texture != null)
                {
                    // this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);
                    //this._effect.Begin();
                    foreach (Microsoft.Xna.Framework.Graphics.EffectPass _pass in this._effect.CurrentTechnique.Passes)
                    {
                        //_pass.Begin();
                        /*this._uiendpoint.GraphicsDevice.GraphicsDevice.VertexDeclaration =
                            new Microsoft.Xna.Framework.Graphics.VertexDeclaration(
                                this._uiendpoint.GraphicsDevice.GraphicsDevice, Microsoft.Xna.Framework.Graphics.VertexPositionTexture.VertexElements);*/
                        this._uiendpoint.GraphicsDevice.GraphicsDevice.DrawUserPrimitives(
                            Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, _vertices, 0, 2);
                        //_pass.End();
                    }
                    //this._effect.End();
                }
            }
        }

        #endregion
    }
#endif
}
