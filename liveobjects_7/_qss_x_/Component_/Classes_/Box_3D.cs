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
        QS.Fx.Reflection.ComponentClasses.Box_3D, "Box_3D", "Implements a 3-dimensional box displayable in XNA windows.")]
    public sealed class Box_3D : 
        QS.Fx.Inspection.Inspectable, 
        QS.Fx.Object.Classes.IUI_X,
        QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>,
        QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>
    {
        #region Constructor

        public Box_3D(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("SX", QS.Fx.Reflection.ParameterClass.Value)] float _sx,
            [QS.Fx.Reflection.Parameter("SY", QS.Fx.Reflection.ParameterClass.Value)] float _sy,
            [QS.Fx.Reflection.Parameter("SZ", QS.Fx.Reflection.ParameterClass.Value)] float _sz,
            [QS.Fx.Reflection.Parameter("coordinates", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinates,
            [QS.Fx.Reflection.Parameter("color", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>> _color)
        {
            this._rx = _sx;
            this._ry = _sy;
            this._rz = _sz;

            this._color = new QS._qss_x_.Channel_.Message_.Color(0, 0, 0, 0);
            this._RecalculateColor();

            this._uiendpoint = _mycontext.ExportedUI_X(
                new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback),
                new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback),
                new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this._uiendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this._uiendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            this._coordinatesendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>>(this);

            this._coordinatesendpoint.OnConnect += new QS.Fx.Base.Callback(this._CoordinatesConnectCallback);
            this._coordinatesendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._CoordinatesDisconnectCallback);

            this._coordinatesconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._coordinatesendpoint).Connect(_coordinates.Dereference(_mycontext).Endpoint);

            this._colorendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>,
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>>(this);

            this._colorendpoint.OnConnect += new QS.Fx.Base.Callback(this._ColorConnectCallback);
            this._colorendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._ColorDisconnectCallback);

            this._colorconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._colorendpoint).Connect(_color.Dereference(_mycontext).Endpoint);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedUI_X _uiendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinatesendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.IColor>,
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>> _colorendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _coordinatesconnection;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _colorconnection;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Channel_.Message_.ICoordinates _coordinates;
        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Channel_.Message_.IColor _color;        
        [QS.Fx.Base.Inspectable]
        private float _rx, _ry, _rz;

        private Microsoft.Xna.Framework.Graphics.Effect _effect;
        private Microsoft.Xna.Framework.Graphics.VertexBuffer _vertexbuffer;
        private bool _ready;
        private Microsoft.Xna.Framework.Matrix _modelmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

        #endregion

        #region _EFFECT_SOURCE

        private const string _effect_source =
@"
struct VertexToPixel
{
    float4 Position : POSITION;
    float4 Color : COLOR0;
};

 struct PixelToFrame
 {
    float4 Color : COLOR0;
 };

float4x4 xViewProjection;
float4x4 xWorld;
float4 xColor0;
float4 xColor1;

VertexToPixel FooVertexShader(float4 inPos : POSITION, float inMyIndex : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel) 0;
    Output.Position = mul(mul(inPos, xWorld), xViewProjection);
    if (inMyIndex > 0)
    {
        Output.Color = xColor1;
    }
    else
    {
        Output.Color = xColor0;
    }
    return Output;
}

PixelToFrame FooPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame) 0;
    Output.Color = PSIn.Color; 
    return Output;
}

technique Foo
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 FooVertexShader();
        PixelShader = compile ps_1_1 FooPixelShader();
    }
}
";

        #endregion

        #region Struct FooVertexFormat

        private struct FooVertexFormat
        {
            private Microsoft.Xna.Framework.Vector3 position;
            private float myindex;

            public FooVertexFormat(Microsoft.Xna.Framework.Vector3 position, float myindex)
            {
                this.position = position;
                this.myindex = myindex;
            }

            public static Microsoft.Xna.Framework.Graphics.VertexElement[] Elements =
            {
                new Microsoft.Xna.Framework.Graphics.VertexElement(
                    0, 
                    Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3, 
                    Microsoft.Xna.Framework.Graphics.VertexElementUsage.Position, 
                    0),
                new Microsoft.Xna.Framework.Graphics.VertexElement(
                    3 * sizeof(float), 
                    Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single, 
                    Microsoft.Xna.Framework.Graphics.VertexElementUsage.TextureCoordinate, 
                    0)
            };

            public static int SizeInBytes = 4 * sizeof(float);
        }

        #endregion

        #region _VERTICES

        private static readonly FooVertexFormat[] _vertices = 
            new FooVertexFormat[]
            {
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    1  ),   1),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    1  ),   1),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    1  ),   1),
            
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    1  ),   1),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    1  ),   1),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    1  ),   1),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    1  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    0  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    0  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    1  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    1  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,    1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(   -1,   -1,    0  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    1  ),   0),

                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    1  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,    1,    0  ),   0),
                new FooVertexFormat(new Microsoft.Xna.Framework.Vector3(    1,   -1,    0  ),   0)
            };

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
                    Microsoft.Xna.Framework.TargetPlatform.Windows);

            if (!_compiledeffect.Success)
                throw new Exception("Could not compile the effect.\n" + _compiledeffect.ErrorsAndWarnings);*/

            this._effect = new Microsoft.Xna.Framework.Graphics.Effect(
                this._uiendpoint.GraphicsDevice.GraphicsDevice, null);

            this._effect.CurrentTechnique = this._effect.Techniques["Foo"];

            /*this._vertexbuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer(
                this._uiendpoint.GraphicsDevice.GraphicsDevice, FooVertexFormat.SizeInBytes * _vertices.Length,
                Microsoft.Xna.Framework.Graphics.BufferUsage.WriteOnly);
            this._vertexbuffer.SetData(_vertices);*/

            if (this._modelmatrix != null)
                this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);

            this._RecalculateColor();
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion

        #region _CoordinatesConnectCallback

        private void _CoordinatesConnectCallback()
        {
            lock (this)
            {
                this._coordinates = this._coordinatesendpoint.Interface.Get();
                if (this._coordinates != null)
                    this._RecalculateCoordinates();
            }
        }

        #endregion

        #region _CoordinatesDisconnectCallback

        private void _CoordinatesDisconnectCallback()
        {
        }

        #endregion

        #region _ColorConnectCallback

        private void _ColorConnectCallback()
        {
            lock (this)
            {
                QS._qss_x_.Channel_.Message_.IColor _color = this._colorendpoint.Interface.Get();
                if (_color != null)
                {
                    this._color = _color;
                    this._RecalculateColor();
                }
            }
        }

        #endregion

        #region _ColorDisconnectCallback

        private void _ColorDisconnectCallback()
        {
        }

        #endregion

        #region _RecalculateCoordinates

        private void _RecalculateCoordinates()
        {
            this._modelmatrix = 
                Microsoft.Xna.Framework.Matrix.CreateScale(this._rx, this._ry, this._rz) *                
                Microsoft.Xna.Framework.Matrix.CreateRotationX(this._coordinates.RX) *
                Microsoft.Xna.Framework.Matrix.CreateRotationY(this._coordinates.RY) *
                Microsoft.Xna.Framework.Matrix.CreateRotationZ(this._coordinates.RZ) *
                Microsoft.Xna.Framework.Matrix.CreateTranslation(
                    new Microsoft.Xna.Framework.Vector3(this._coordinates.PX, this._coordinates.PY, this._coordinates.PZ));

            if (this._effect != null)
                this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);
        }

        #endregion

        #region _RecalculateColor

        private void _RecalculateColor()
        {
            if (this._effect != null && this._color != null)
            {
                this._effect.Parameters["xColor0"].SetValue(Microsoft.Xna.Framework.Color.WhiteSmoke.ToVector4());
                this._effect.Parameters["xColor1"].SetValue(
                    new Microsoft.Xna.Framework.Vector4(
                        (float)this._color.R / 255.0f, 
                        (float)this._color.G / 255.0f, 
                        (float) this._color.B / 255.0f,
                        (float)this._color.A / 255.0f));
            }
        }

        #endregion

        #region _RepositionCallback

        private void _RepositionCallback(
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            lock (this)
            {
                this._cameramatrix = _cameramatrix;
                this._projectionmatrix = _projectionmatrix;
                if (this._effect != null)
                    this._effect.Parameters["xViewProjection"].SetValue(this._cameramatrix * this._projectionmatrix);
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
                if (this._vertexbuffer != null && this._coordinates != null && this._color != null)
                {
                    /*this._effect.Begin();
                    foreach (Microsoft.Xna.Framework.Graphics.EffectPass _pass in this._effect.CurrentTechnique.Passes)
                    {
                        _pass.Begin();
                        this._uiendpoint.GraphicsDevice.GraphicsDevice.VertexDeclaration =
                            new Microsoft.Xna.Framework.Graphics.VertexDeclaration(
                                this._uiendpoint.GraphicsDevice.GraphicsDevice, FooVertexFormat.Elements);
                        this._uiendpoint.GraphicsDevice.GraphicsDevice.Vertices[0].SetSource(this._vertexbuffer, 0, FooVertexFormat.SizeInBytes);
                        this._uiendpoint.GraphicsDevice.GraphicsDevice.DrawPrimitives(
                            Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList, 0, _vertices.Length / 3);
                        _pass.End();
                    }
                    this._effect.End();*/
                }
            }
        }

        #endregion

        #region IValueClient<ICoordinates> Members

        void QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>.Set(QS._qss_x_.Channel_.Message_.ICoordinates _value)
        {
            lock (this)
            {
                if ((_value != null) && ((this._coordinates == null) || (_value.TM > this._coordinates.TM)))
                {
                    this._coordinates = _value;
                    this._RecalculateCoordinates();
                }
            }
        }

        #endregion

        #region IValueClient<IColor> Members

        void QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.IColor>.Set(QS._qss_x_.Channel_.Message_.IColor _value)
        {
            lock (this)
            {
                if (_value != null)
                {
                    this._color = _value;
                    this._RecalculateColor();
                }
            }
        }

        #endregion
    }
#endif
}
