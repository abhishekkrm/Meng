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

using MapLibrary;

namespace MapLibrary
{
    class GridTranslator
    {
        WorldConfiguration world;
        
        public GridTranslator()
        {
            // Until the world (XNAWindow) is actually setup to retrieve this info, 
            // it will be hard-coded here.
            float width_Pixel = 131072f;
            float height_Pixel = 131072f;
            WorldConfiguration.CoordinateSystem coordinates = WorldConfiguration.CoordinateSystem.MERIDIAN;

            WorldConfiguration world = new WorldConfiguration(width_Pixel, height_Pixel, coordinates);

            this.world = world;
        }

        public GridTranslator(WorldConfiguration _world)
        {
            this.world = _world;
        }

        public Location PixelToLatLon(float lat, float lon, float alt)
        {
            return new Location((180.0f / (float)Math.PI) * (2f * (float)(Math.Atan(Math.Exp(Math.PI * (1.0 - 2.0 * (double)(world.PixelHeight-lat) / (double)world.PixelHeight)))) - .5f * (float)Math.PI),
                                lon * 360.0f / world.PixelWidth - 180.0f,
                                alt);
        }

        public Location PixelToLatLon(Location loc)
        {
            return PixelToLatLon(loc.Latitude, loc.Longitude, loc.Altitude);
        }

        public Location LatLonToPixel(float lat, float lon, float alt)
        {
            return new Location(world.PixelHeight * 0.5f * (1f - (float)(Math.Log(Math.Tan((0-(double)lat) * Math.PI / 180.0 * 0.5 + Math.PI * 0.25), Math.E)) / (float)Math.PI),
                    (lon + 180.0f) * world.PixelWidth / 360.0f,
                    alt);
        }

        public Location LatLonToPixel(Location loc)
        {
            return LatLonToPixel(loc.Latitude, loc.Longitude, loc.Altitude);
        }

        public Location MatixToViewPoint(Microsoft.Xna.Framework.Matrix _cameramatrix, Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            Matrix inv = Matrix.Invert(_cameramatrix);
            Vector3 pointRotationAnchor = new Vector3(0, 0, 0);

            // Sets the rotation anchor as the point directly in the middle of view
            Vector3 normalReference = new Vector3(inv.M31, inv.M32, inv.M33);
            float heightStep = Math.Abs(inv.M43 / normalReference.Z);
            pointRotationAnchor.X = inv.M41 - normalReference.X * heightStep;
            pointRotationAnchor.Y = inv.M42 - normalReference.Y * heightStep;

            float dist = Vector3.Distance(new Vector3(inv.M41, inv.M42, inv.M43), pointRotationAnchor);

            //return new Location(pointRotationAnchor.Y, pointRotationAnchor.X, inv.M43);
            return new Location(pointRotationAnchor.Y, pointRotationAnchor.X, dist);
        }

        public Location MatixToPosition(Microsoft.Xna.Framework.Matrix _cameramatrix, Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            Matrix inv = Matrix.Invert(_cameramatrix);
            return new Location(inv.M42, inv.M41, inv.M43);
        }

        public Location GetTopLeftCornerOfEarthFromMatrix(Microsoft.Xna.Framework.Matrix _cameramatrix, Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            Matrix inv = Matrix.Invert(_cameramatrix);
            Vector3 pointRotationAnchor = new Vector3(0, 0, 0);

            // Sets the rotation anchor as the point directly in the middle of view
            Vector3 normalReference = new Vector3(inv.M31, inv.M32, inv.M33);
            normalReference = Vector3.Transform(normalReference, Matrix.CreateRotationX((float)Math.PI / 8f) * Matrix.CreateRotationY((float)Math.PI / 8f));
            float heightStep = Math.Abs(inv.M43 / normalReference.Z);
            pointRotationAnchor.X = inv.M41 - normalReference.X * heightStep;
            pointRotationAnchor.Y = inv.M42 - normalReference.Y * heightStep;

            return new Location(pointRotationAnchor.Y, pointRotationAnchor.X, inv.M43);
        }

        public Location GetBottomRightCornerOfEarthFromMatrix(Microsoft.Xna.Framework.Matrix _cameramatrix, Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            Matrix inv = Matrix.Invert(_cameramatrix);
            Vector3 pointRotationAnchor = new Vector3(0, 0, 0);

            // Sets the rotation anchor as the point directly in the middle of view
            Vector3 normalReference = new Vector3(inv.M31, inv.M32, inv.M33);
            normalReference = Vector3.Transform(normalReference, Matrix.CreateRotationX(-1f * (float)Math.PI / 8f) * Matrix.CreateRotationY(-1f * (float)Math.PI / 8f));
            float heightStep = Math.Abs(inv.M43 / normalReference.Z);
            pointRotationAnchor.X = inv.M41 - normalReference.X * heightStep;
            pointRotationAnchor.Y = inv.M42 - normalReference.Y * heightStep;

            return new Location(pointRotationAnchor.Y, pointRotationAnchor.X, inv.M43);
        }
    }
}

#endif
