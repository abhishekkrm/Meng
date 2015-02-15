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
using System.IO;
using System.Collections;

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using System.Windows.Forms;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
        "601119857F9D4499AEA9224A355E2302", "Camera",
        "An XNA camera that can be shared.")]
    class Camera : ICamera, ICameraOps, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>
    {
        private QS.Fx.Endpoint.Classes.IExportedInterface<ICameraOps> managerendpoint;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                UpdateWindowEvent, UpdateWindowEvent>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                UpdateWindowEvent, UpdateWindowEvent>> channelendpoint;
        private QS.Fx.Endpoint.IConnection channelconnection;

        private ICameraOps _self;
        private bool sharePosition;
        private bool shareReference;
        private bool processPositions;
        private bool processReferences;

        // State transition variables
        private Vector3 pointRotationAnchor = Vector3.Zero;
        private bool showMouse = true;

        // Constraints
        private const float MIN_Z = 1.0f;
        private const float MAX_Z = 300000.0f;

        Vector3 cameraReference;
        Vector3 cameraPosition;
        Vector3 cameraUp;

        Double myRandomId;

        public Camera(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                    UpdateWindowEvent, UpdateWindowEvent>> channel,
            [QS.Fx.Reflection.Parameter("Share Position?", QS.Fx.Reflection.ParameterClass.Value)]
            bool sharePosition,
            [QS.Fx.Reflection.Parameter("Share Reference?", QS.Fx.Reflection.ParameterClass.Value)]
            bool shareReference,
            [QS.Fx.Reflection.Parameter("Process Positions?", QS.Fx.Reflection.ParameterClass.Value)]
            bool processPositions,
            [QS.Fx.Reflection.Parameter("Process References?", QS.Fx.Reflection.ParameterClass.Value)]
            bool processReferences,
            [QS.Fx.Reflection.Parameter("PositionX", QS.Fx.Reflection.ParameterClass.Value)]
            Single positionX,
            [QS.Fx.Reflection.Parameter("PositionY", QS.Fx.Reflection.ParameterClass.Value)]
            Single positionY,
            [QS.Fx.Reflection.Parameter("PositionZ", QS.Fx.Reflection.ParameterClass.Value)]
            Single positionZ,
            [QS.Fx.Reflection.Parameter("ForwardX", QS.Fx.Reflection.ParameterClass.Value)]
            Single forwardX,
            [QS.Fx.Reflection.Parameter("ForwardY", QS.Fx.Reflection.ParameterClass.Value)]
            Single forwardY,
            [QS.Fx.Reflection.Parameter("ForwardZ", QS.Fx.Reflection.ParameterClass.Value)]
            Single forwardZ,
            [QS.Fx.Reflection.Parameter("UpX", QS.Fx.Reflection.ParameterClass.Value)]
            Single upX,
            [QS.Fx.Reflection.Parameter("UpY", QS.Fx.Reflection.ParameterClass.Value)]
            Single upY,
            [QS.Fx.Reflection.Parameter("UpZ", QS.Fx.Reflection.ParameterClass.Value)]
            Single upZ)
            //[QS.Fx.Reflection.Parameter("UpZ", QS.Fx.Reflection.ParameterClass.Value)]
            //Single minZ,
            //[QS.Fx.Reflection.Parameter("UpZ", QS.Fx.Reflection.ParameterClass.Value)]
            //Single maxZ,)
        {
            cameraPosition = new Vector3(positionX, positionY, positionZ);
            if(cameraPosition.Equals(Vector3.Zero))
                cameraPosition = new Vector3(80000, 80000, 70000);

            cameraReference = new Vector3(forwardX, forwardY, forwardZ);
            if (cameraReference.Equals(Vector3.Zero) || Single.IsNaN(forwardX) || Single.IsNaN(forwardY) ||Single.IsNaN(forwardZ))
                cameraReference = new Vector3(0, 0, -1);
            cameraReference.Normalize();

            cameraUp = new Vector3(upX, upY, upZ);
            if (cameraUp.Equals(Vector3.Zero) || Single.IsNaN(upX) || Single.IsNaN(upY) || Single.IsNaN(upZ))
                cameraUp = new Vector3(0, 1, 0);
            cameraUp.Normalize();

            myRandomId = new Random().NextDouble();

            this._self = (ICameraOps)this;
            
            this.sharePosition = sharePosition;
            this.shareReference = shareReference;
            this.processPositions = processPositions;
            this.processReferences = processReferences;

            managerendpoint = _mycontext.ExportedInterface<ICameraOps>(this);

            if (channel != null)
            {
                channelendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                        UpdateWindowEvent, UpdateWindowEvent>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                        UpdateWindowEvent, UpdateWindowEvent>>(this);
                channelconnection = channelendpoint.Connect(channel.Dereference(_mycontext).Channel);
            }
        }


        #region ICameraOps Members

        void ICameraOps.LookVertical(float radians, bool allowCrossOver)
        {
            Vector3 normalReference = Vector3.Normalize(cameraReference);
                
            // Rotates the reference vector up/down (i.e. perpendicular to the axis going in one ear and out the other)
            Matrix transform = Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, normalReference), radians);
            Vector3 temp = Vector3.Transform(cameraReference, transform);
            Vector3 temp2 = Vector3.Transform(cameraUp, transform);
            
            // if their product is > 0 then we didn't cross the axis (i.e. look between our legs)          
            if (allowCrossOver || (temp.X * cameraReference.X >= 0 && temp.Y * cameraReference.Y >= 0))
            {
                cameraReference = temp;
                cameraUp = temp2;
            }
            broadcastInfo();
        }

        void ICameraOps.LookSideways(float radians)
        {
            cameraReference = Vector3.Transform(cameraReference, Matrix.CreateRotationZ(radians));
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateRotationZ(radians));
            broadcastInfo();
        }

        void ICameraOps.ChangeAltitute(float units, bool fixedReference)
        {
            if (fixedReference)
            {
                throw new NotImplementedException();
            }
            else
            {
                cameraPosition.Z += units;
                if (cameraPosition.Z > MAX_Z)
                    cameraPosition.Z = MAX_Z;
                if (cameraPosition.Z < MIN_Z)
                    cameraPosition.Z = MIN_Z;
            }
            broadcastInfo();
        }

        void ICameraOps.SetAltitude(float altitude, bool fixedReference)
        {
            if (fixedReference)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (altitude > MAX_Z)
                    cameraPosition.Z = MAX_Z;
                else if (altitude < MIN_Z)
                    cameraPosition.Z = MIN_Z;
                else
                    cameraPosition.Z = altitude;
            }

            broadcastInfo();
        }

        void ICameraOps.VerticalRotationAroundPoint(Demo.Xna.Vector3 point, float radians)
        {
            throw new NotImplementedException();
        }

        void ICameraOps.HorizontalRotationAroundPoint(Demo.Xna.Vector3 point, float radians)
        {
            throw new NotImplementedException();
        }

        void ICameraOps.VerticalRotationAroundReference(float radians)
        {
            pointRotationAnchor = new Vector3(0, 0, 0);

            // Sets the rotation anchor as the point directly in the middle of view
            Vector3 normalReference = Vector3.Normalize(cameraReference);
            float heightStep = Math.Abs(cameraPosition.Z / normalReference.Z);
            pointRotationAnchor.X = cameraPosition.X + normalReference.X * heightStep;
            pointRotationAnchor.Y = cameraPosition.Y + normalReference.Y * heightStep;

            // Rotates the reference vector up/down (i.e. perpendicular to the axis going in one ear and out the other)
            Matrix transform = Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, normalReference), radians);
            Vector3 temp = Vector3.Transform(cameraReference, transform);
            Vector3 temp2 = Vector3.Transform(cameraUp, transform);

            // if their product is > 0 then we didn't cross the axis (i.e. look between our legs)
            if (temp.X * cameraReference.X >= 0 && temp.Y * cameraReference.Y >= 0)
            {
                cameraReference = temp;
                cameraUp = temp2;
            }

            // Backtrace the camera position using the anchor and new reference vector
            float distance = Vector3.Distance(cameraPosition, pointRotationAnchor);
            Vector3 ray = Vector3.Multiply(Vector3.Normalize(cameraReference), -1.0f);
            cameraPosition = pointRotationAnchor + Vector3.Multiply(ray, distance);

            bool changed = false;
            if (cameraPosition.Z > MAX_Z)
            {
                cameraPosition.Z = MAX_Z;
                changed = true;
            }
            if (cameraPosition.Z < MIN_Z)
            {
                cameraPosition.Z = MIN_Z;
                changed = true;
            }

            // Need to recalculate reference if the Z coordinate was changed
            if (changed)
                cameraReference = Vector3.Normalize(pointRotationAnchor - cameraPosition);

            broadcastInfo();
        }

        void ICameraOps.HorizontalRotationAroundReference(float radians)
        {
            pointRotationAnchor = new Vector3(0, 0, 0);

            // Sets the rotation anchor as the point directly in the middle of view
            Vector3 normalReference = Vector3.Normalize(cameraReference);
            float heightStep = Math.Abs(cameraPosition.Z / normalReference.Z);
            pointRotationAnchor.X = cameraPosition.X + normalReference.X * heightStep;
            pointRotationAnchor.Y = cameraPosition.Y + normalReference.Y * heightStep;

            // Spin the reference vector (so that when it gets backtraced it will look like camera spun around anchor)
            cameraReference = Vector3.Transform(cameraReference, Matrix.CreateRotationZ(radians));
            cameraUp = Vector3.Transform(cameraUp, Matrix.CreateRotationZ(radians));

            // Backtrace the camera position using the anchor and new reference vector
            float distance = Vector3.Distance(cameraPosition, pointRotationAnchor);
            Vector3 ray = Vector3.Multiply(Vector3.Normalize(cameraReference), -1.0f);
            cameraPosition = pointRotationAnchor + Vector3.Multiply(ray, distance);

            bool changed = false;
            if (cameraPosition.Z > MAX_Z)
            {
                cameraPosition.Z = MAX_Z;
                changed = true;
            }
            if (cameraPosition.Z < MIN_Z)
            {
                cameraPosition.Z = MIN_Z;
                changed = true;
            }

            // Need to recalculate reference if the Z coordinate was changed
            if (changed)
                cameraReference = Vector3.Normalize(pointRotationAnchor - cameraPosition);

            broadcastInfo();
        }

        void ICameraOps.ZoomToReference(float units)
        {
            Vector3 referenceNormal = Vector3.Normalize(cameraReference);

            // if we will move past max zoom, only move to max zoom
            if (cameraPosition.Z + units * referenceNormal.Z >= MAX_Z)
                units = (MAX_Z - cameraPosition.Z) / referenceNormal.Z;

            // if we will move past min zoom, only move to min zoom
            if (cameraPosition.Z + units * referenceNormal.Z <= MIN_Z)
                units = (MIN_Z - cameraPosition.Z) / referenceNormal.Z;

            cameraPosition += (Vector3.Multiply(referenceNormal, units));
            broadcastInfo();
        }

        void ICameraOps.ZoomToPoint(Demo.Xna.Vector3 point, float units)
        {
            throw new NotImplementedException();
        }

        void ICameraOps.MoveForward(float units)
        {
            Vector3 v;
            if(cameraReference.X ==0 && cameraReference.Y == 0)
                v = Vector3.Normalize(new Vector3(cameraUp.X, cameraUp.Y, 0));
            else
                v = Vector3.Normalize(new Vector3(cameraReference.X, cameraReference.Y, 0));

            cameraPosition += Vector3.Multiply(v, units);
            broadcastInfo();
        }

        void ICameraOps.MoveSideways(float units)
        {
            Vector3 v = Vector3.Normalize(Vector3.Cross(cameraReference, cameraUp));
            cameraPosition += Vector3.Multiply(v, units);
            broadcastInfo();
        }

        void ICameraOps.TranslateAbsolute(Demo.Xna.Vector3 translation)
        {
            cameraPosition += translation.Vector3_;
            broadcastInfo();
        }

        UpdateWindowEvent ICameraOps.GetState()
        {
            UpdateWindowEvent uwe = new UpdateWindowEvent(myRandomId,new Demo.Xna.Vector3(cameraPosition),
                                              new Demo.Xna.Vector3(cameraReference),
                                              new Demo.Xna.Vector3(cameraUp),
                                              showMouse);
            return uwe;
        }

        #endregion

        #region ICamera Members

        QS.Fx.Endpoint.Classes.IExportedInterface<ICameraOps> ICamera.Camera
        {
            get { return this.managerendpoint; }
        }

        #endregion


        #region ICameraOps Members


        void ICameraOps.Pitch(float radians)
        {
            _self.LookVertical(radians, true);
        }

        void ICameraOps.Yaw(float radians)
        {
            throw new NotImplementedException();

            broadcastInfo();
        }

        void ICameraOps.Roll(float radians)
        {
            Matrix transform = Matrix.CreateFromAxisAngle(cameraReference, radians);
            cameraReference = Vector3.Transform(cameraReference, transform);
            cameraUp = Vector3.Transform(cameraUp, transform);

            broadcastInfo();
        }

        #endregion

        void broadcastInfo()
        {
            //this.channelendpoint.Interface.Send(getUpdateCameraEvent());
        }

        UpdateWindowEvent getUpdateCameraEvent()
        {
            if (sharePosition && shareReference)
                return new UpdateWindowEvent(myRandomId, new Demo.Xna.Vector3(cameraPosition),
                                              new Demo.Xna.Vector3(cameraReference),
                                              new Demo.Xna.Vector3(cameraUp),
                                              showMouse);
            else if (sharePosition)
                return new UpdateWindowEvent(myRandomId, new Demo.Xna.Vector3(cameraPosition),
                                              null,
                                              null,
                                              showMouse);
            else if (shareReference)
                return new UpdateWindowEvent(myRandomId, null,
                                              new Demo.Xna.Vector3(cameraReference),
                                              new Demo.Xna.Vector3(cameraUp),
                                              showMouse);

            else
                return new UpdateWindowEvent();
        }
        #region ICheckpointedCommunicationChannelClient<UpdateWindowEvent,UpdateWindowEvent> Members

        UpdateWindowEvent QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Checkpoint()
        {
            return getUpdateCameraEvent();
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Initialize(UpdateWindowEvent _checkpoint)
        {
            if (_checkpoint == null) return;
            if (_checkpoint.Id == myRandomId) return;

            if (processPositions && _checkpoint.CameraPosition != null)
                cameraPosition = _checkpoint.CameraPosition.Vector3_;
            if (processReferences && _checkpoint.CameraReference != null)
                cameraReference = _checkpoint.CameraReference.Vector3_;
            if (processReferences && _checkpoint.CameraUp != null)
                cameraReference = _checkpoint.CameraUp.Vector3_;

            showMouse = _checkpoint.ShowMouse;
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Receive(UpdateWindowEvent _message)
        {
            //return;
            if (_message == null) return;
            if (_message.Id == myRandomId) return;

            if (processPositions && _message.CameraPosition != null)
                cameraPosition = _message.CameraPosition.Vector3_;
            if (processReferences && _message.CameraReference != null)
                cameraReference = _message.CameraReference.Vector3_;
            if (processReferences && _message.CameraUp != null)
                cameraUp = _message.CameraUp.Vector3_;

            showMouse = _message.ShowMouse;
        }

        #endregion

        #region ICameraOps Members


        void ICameraOps.BroadcastState()
        {
            if(this.channelendpoint != null)
                this.channelendpoint.Interface.Send(getUpdateCameraEvent());
        }

        #endregion

        #region ICameraOps Members


        void ICameraOps.SetPosition(Demo.Xna.Vector3 point)
        {
            cameraPosition = point.Vector3_;
        }

        void ICameraOps.SetForwardVector(Demo.Xna.Vector3 v)
        {
            cameraReference = v.Vector3_;
        }

        void ICameraOps.SetUpVector(Demo.Xna.Vector3 v)
        {
            cameraUp = v.Vector3_;
        }

        #endregion

    }
}
#endif
