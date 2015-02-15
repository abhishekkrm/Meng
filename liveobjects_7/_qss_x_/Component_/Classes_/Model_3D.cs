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
        QS.Fx.Reflection.ComponentClasses.Model_3D, 
        "Model_3D", 
        "Implements a 3-dimensional model displayable in XNA windows.")]
    public sealed class Model_3D
        : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IUI_X, QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>, IDisposable
    {
        #region Constructor

        public Model_3D(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("modelname",
                "The name of the resource containing the texture of the object.",
                QS.Fx.Reflection.ParameterClass.Value)] string _modelname,
            [QS.Fx.Reflection.Parameter("coordinates", 
                "The stream of the object's coordinates in 3-dimensional space.",
                QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<QS.Fx.Object.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinates)
        {
            this._modelname = _modelname;

            this._adjustmatrix =
                Microsoft.Xna.Framework.Matrix.CreateScale(0.1f) *
                Microsoft.Xna.Framework.Matrix.CreateRotationX(- Microsoft.Xna.Framework.MathHelper.PiOver2) *
                Microsoft.Xna.Framework.Matrix.CreateRotationY(+ Microsoft.Xna.Framework.MathHelper.Pi) *
                Microsoft.Xna.Framework.Matrix.CreateRotationZ(+ Microsoft.Xna.Framework.MathHelper.Pi);

            this._uiendpoint = _mycontext.ExportedUI_X(
                new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback), 
                new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback), 
                new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this._coordinatesendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>, 
                QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>>(this);

            this._uiendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this._uiendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            this._coordinatesendpoint.OnConnect += new QS.Fx.Base.Callback(this._CoordinatesConnectCallback);
            this._coordinatesendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._CoordinatesDisconnectCallback);

            this._coordinatesconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._coordinatesendpoint).Connect(_coordinates.Dereference(_mycontext).Endpoint);
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IExportedUI_X _uiendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<QS.Fx.Interface.Classes.IValue<QS._qss_x_.Channel_.Message_.ICoordinates>,
            QS.Fx.Interface.Classes.IValueClient<QS._qss_x_.Channel_.Message_.ICoordinates>> _coordinatesendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _coordinatesconnection;
        [QS.Fx.Base.Inspectable]
        private string _modelname;
        private QS.Fx.Xna.IContent _content;
        private Microsoft.Xna.Framework.Graphics.Model _model;

        [QS.Fx.Base.Inspectable]
        private QS._qss_x_.Channel_.Message_.ICoordinates _coordinates;
        private float _aspectratio;
        private Microsoft.Xna.Framework.Matrix _adjustmatrix, _modelmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

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
            this._content = this._uiendpoint.Content(new QS.Fx.Xna.ContentRef(QS.Fx.Xna.ContentClass.Model, _modelname));
            this._model = (Microsoft.Xna.Framework.Graphics.Model) this._content.Content;
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

        #region _RepositionCallback

        private void _RepositionCallback(
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            lock (this)
            {
                this._cameramatrix = _cameramatrix;
                this._projectionmatrix = _projectionmatrix;
            }
        }

        #endregion

        #region _UpdateCallback

        private void _UpdateCallback(Microsoft.Xna.Framework.GameTime _time)
        {
            lock (this)
            {
                if (this._coordinatesendpoint.IsConnected)
                {
                    this._coordinates = this._coordinatesendpoint.Interface.Get();
                    this._RecalculateCoordinates();
                }
            }
        }

        #endregion

        #region _DrawCallback

        private void _DrawCallback(Microsoft.Xna.Framework.GameTime _time)
        {
            lock (this)
            {
                if (this._model != null && this._coordinates != null)
                {
                    // _RecalculateCoordinates();

                    Microsoft.Xna.Framework.Matrix[] _transforms = new Microsoft.Xna.Framework.Matrix[this._model.Bones.Count];
                    this._model.CopyAbsoluteBoneTransformsTo(_transforms);

                    foreach (Microsoft.Xna.Framework.Graphics.ModelMesh _mesh in this._model.Meshes)
                    {
                        foreach (Microsoft.Xna.Framework.Graphics.BasicEffect _effect in _mesh.Effects)
                        {
                            _effect.EnableDefaultLighting();
                            _effect.World = _transforms[_mesh.ParentBone.Index] * this._modelmatrix;
                            _effect.View = this._cameramatrix;
                            _effect.Projection = this._projectionmatrix;
                        }
                        _mesh.Draw();
                    }
                }
            }
        }

        #endregion

        #region _RecalculateCoordinates

        private void _RecalculateCoordinates()
        {
            if (this._coordinates != null)
            {
                this._modelmatrix = this._adjustmatrix *
                    Microsoft.Xna.Framework.Matrix.CreateRotationX(this._coordinates.RX) *
                    Microsoft.Xna.Framework.Matrix.CreateRotationY(this._coordinates.RY) *
                    Microsoft.Xna.Framework.Matrix.CreateRotationZ(this._coordinates.RZ) *
                    Microsoft.Xna.Framework.Matrix.CreateTranslation(
                        new Microsoft.Xna.Framework.Vector3(this._coordinates.PX, this._coordinates.PY, this._coordinates.PZ));
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (this._coordinatesconnection != null)
                    this._coordinatesconnection.Dispose();
                this._coordinatesconnection = null;
                if (this._coordinates is IDisposable)
                    ((IDisposable)this._coordinates).Dispose();
            }
        }

        #endregion
    }
#endif
}
