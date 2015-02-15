/*

Copyright (c) 2004-2009 Jared Cantwell, Petko Nikolov, Qi Huang. All rights reserved.

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
using System.Linq;
using System.Text;

#if XNA

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Drawing;
using System.Net;
using MapLibrary;
using System.Xml;

namespace WeatherLibrary
{
    [QS.Fx.Reflection.ComponentClass(
        "104`1", "WeatherRenderer", "Implements an XNA renderer for weather text on an earthly plane.")]
    public class WeatherRenderer : QS.Fx.Object.Classes.IUI_X, IWeatherRendererOps
    {
        #region Field

        /// IUI_X endpoint
        private QS.Fx.Endpoint.Internal.IExportedUI_X windowendpoint;

        /// WeatherSource liveobject endpoint
        private QS.Fx.Endpoint.Internal.IDualInterface<IWeatherSourceOps, IWeatherRendererOps> weathersourceendpoint;
        private QS.Fx.Endpoint.IConnection weathersourceconnection;

        /// XNA effect
        private Microsoft.Xna.Framework.Graphics.Effect _effect;

        /// XNA matrix
        private Microsoft.Xna.Framework.Matrix _modelmatrix;
        private Microsoft.Xna.Framework.Matrix _cameramatrix, _projectionmatrix;

        //private Microsoft.Xna.Framework.Matrix _worldmatrix;

        /// XNA device
        GraphicsDevice device;
        IGraphicsDeviceService graphics;


        // For 2D image
        SpriteBatch spriteBatch;
        Texture2D weatherImage;
        SpriteFont spriteFont;

        // For calculate control
        private bool isMoved = false;
        private bool opAfterMoved = false;
        private int waitForStill = 0;

        // Bool for internet access
        private bool isinternet;

        // For pixel and latlon translator
        private GridTranslator grid;

        // For watch weather list
        private string file;

        // For weather information
        private int showdiv = 0;
        Dictionary<string, WeatherShowText> weatherdict;
        List<Location> weatherList;
        List<Location> watchList;
        List<string> zipList;

        private class WeatherShowText
        {
            public WeatherShowText(Location Pos, Weather W)
            {
                pos = Pos;
                this.text = W.temp_f + "F;";// +W.condition;
                pic = W.picture;
            }

            public void Projection(GraphicsDevice device, Matrix projetion, Matrix view, Matrix world)
            {
                Vector3 proj = device.Viewport.Project(new Vector3(pos.Longitude, pos.Latitude, 0f), projetion, view, world);
                X = (int)(proj.X);
                Y = (int)(proj.Y);
            }

            public Location pos;
            public int X, Y;
            public string text;
            public string pic;
        }

        #endregion

        #region Construct

        public WeatherRenderer(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("WeatherSource", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<IWeatherSource> weathersource,
            [QS.Fx.Reflection.Parameter("Grid division divx x divx", QS.Fx.Reflection.ParameterClass.Value)]
                int divx,
            [QS.Fx.Reflection.Parameter("Watch Weather List XML file", QS.Fx.Reflection.ParameterClass.Value)]
                string filename,
            [QS.Fx.Reflection.Parameter("Enable Internet", QS.Fx.Reflection.ParameterClass.Value)]
                bool isinternet
            )
        {
            this.isinternet = isinternet;

            /// Weather Source endpoint
            /// 
            if (weathersource != null)
            {
                this.weathersourceendpoint = _mycontext.DualInterface<IWeatherSourceOps, IWeatherRendererOps>(this);
                this.weathersourceconnection = this.weathersourceendpoint.Connect(weathersource.Dereference(_mycontext).WeatherSource);
            }


            /// XNA window endpoint
            /// 
            this.windowendpoint = _mycontext.ExportedUI_X(
                new QS.Fx.Endpoint.Internal.Xna.RepositionCallback(this._RepositionCallback),
                new QS.Fx.Endpoint.Internal.Xna.UpdateCallback(this._UpdateCallback),
                new QS.Fx.Endpoint.Internal.Xna.DrawCallback(this._DrawCallback));

            this.windowendpoint.OnConnect += new QS.Fx.Base.Callback(this._UIConnectCallback);
            this.windowendpoint.OnDisconnect += new QS.Fx.Base.Callback(this._UIDisconnectCallback);

            // Dictionary of weather info for lat/lon
            if (divx >= 0)
                this.showdiv = divx;
            else
                this.showdiv = 3;

            weatherdict = new Dictionary<string, WeatherShowText>();
            weatherList = new List<Location>(showdiv * showdiv);
            zipList = new List<string>(showdiv * showdiv);

            // Fow watch
            if (filename != null)
                file = filename;
            else
                file = "";
            watchList = new List<Location>();

            grid = new GridTranslator();
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

        #region _UIConnectCallback

        public void _UIConnectCallback()
        {
            //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\debug.txt", true);
            //file.WriteLine("ConnectCallback: " + DateTime.Now.Second + "," + DateTime.Now.Millisecond);

            graphics = this.windowendpoint.GraphicsDevice;
            device = this.windowendpoint.GraphicsDevice.GraphicsDevice;

            /*Microsoft.Xna.Framework.Graphics.CompiledEffect _compiledeffect =
                Microsoft.Xna.Framework.Graphics.Effect.CompileEffectFromSource(
                    _effect_source, null, null,
                    Microsoft.Xna.Framework.Graphics.CompilerOptions.None,
                    Microsoft.Xna.Framework.TargetPlatform.Windows);

            this._effect = new Microsoft.Xna.Framework.Graphics.Effect(
                device, _compiledeffect.GetEffectCode(),
                Microsoft.Xna.Framework.Graphics.CompilerOptions.None, null);
            this._effect.CurrentTechnique = this._effect.Techniques["Textured"];*/

            this._modelmatrix = Microsoft.Xna.Framework.Matrix.CreateRotationX(0f);
            this._effect.Parameters["xWorld"].SetValue(this._modelmatrix);

            spriteFont = (SpriteFont)this.windowendpoint.Content(new QS.Fx.Xna.ContentRef(QS.Fx.Xna.ContentClass.SpriteFont, "C312BA6E943345baA586A9263E840CC6`1:Kootenay12.xnb")).Content;

            // For image
            spriteBatch = new SpriteBatch(device);
            //weatherImage = Texture2D.FromFile(device, "C:\\liveobjects\\bin\\images\\weather\\cloudy.png");
        }

        #endregion

        #region _UIDisconnectCallback

        private void _UIDisconnectCallback()
        {
        }

        #endregion

        #region _RepositionCallback

        Microsoft.Xna.Framework.Matrix _lastCameramatrix;
        Microsoft.Xna.Framework.Matrix _lastProjectionmatrix;


        public void _RepositionCallback(
            Microsoft.Xna.Framework.Matrix _cameramatrix,
            Microsoft.Xna.Framework.Matrix _projectionmatrix)
        {
            // if the camera hasn't changed, don't bother notifying anyone
            if (_lastCameramatrix.Equals(_cameramatrix) && _lastProjectionmatrix.Equals(_projectionmatrix))
            {
                isMoved = false;
                waitForStill++;
            }
            else
            {
                _lastCameramatrix = _cameramatrix;
                _lastProjectionmatrix = _projectionmatrix;
                isMoved = true;
                opAfterMoved = false;
                waitForStill = 0;

                this._cameramatrix = _cameramatrix;
                this._projectionmatrix = _projectionmatrix;

                //// Recalculate the weather position
                //lock (weatherdict)
                //{
                //    foreach (KeyValuePair<string, WeatherShowText> kvp in weatherdict)
                //    {
                //        WeatherShowText show = kvp.Value;
                //        show.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);
                //    }
                //}
            }

            // After finished a moving, operate once (do not move for half seconds)
            if (isMoved == false && opAfterMoved == false && (waitForStill % 30 == 0))
            {
                opAfterMoved = true;
                CalculateGrid();
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
            
            lock (this)
            {
                spriteBatch.Begin();
                if (!(this.isinternet))
                {
                    spriteBatch.DrawString(spriteFont, "Internet Problem: Incorrect Weather", new Vector2(10f, 10f),
                                   Microsoft.Xna.Framework.Color.Red);
                }

                #region Test

                //Location ithaca = new Location(42.44f, -76.5f, 0.0f);
                //Location ithacapos = grid.LatLonToPixel(ithaca);
                //Vector3 ithacapoint = device.Viewport.Project(
                //    new Vector3(ithacapos.Longitude, ithacapos.Latitude, 0f),
                //    _projectionmatrix, _cameramatrix, _modelmatrix);

                //int ithacaX = (int)(ithacapoint.X);
                //int ithacaY = (int)(ithacapoint.Y);

                //string posstring = "Ithaca 3D:" + ithacapos.Latitude + "," + ithacapos.Longitude + "," + ithacapos.Altitude;
                #endregion

                #region ShowGrid in WeatherList
                lock (weatherList)
                {


                    foreach (Location latlon in weatherList)
                    {
                        string key = latlon.Latitude + ":" + latlon.Longitude;
                        Location pos = grid.LatLonToPixel(latlon);
                        WeatherShowText test = new WeatherShowText(pos, new Weather());
                        test.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);

                        lock (weatherdict)
                        {
                            WeatherShowText show;
                            if (weatherdict.TryGetValue(key, out show))
                            {
                                spriteBatch.DrawString(spriteFont, show.text, new Vector2(show.X, show.Y),
                                    Microsoft.Xna.Framework.Color.Yellow);

                                if (show.pic != null && show.pic != "")
                                 {
                                //String[] pics = new String[] { "rain", "cloudy", "partly_cloudy", "sunny" };
                                //String pic = pics[new Random().Next(0, 4)];

                                     try
                                     {
                                         string file = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, "images\\weather\\");
                                         file += show.pic + ".bmp";
                                         FileStream fcc = new FileStream(file, FileMode.Open);
                                         weatherImage = Texture2D.FromStream(device, fcc);
                                         //spriteBatch.Begin();
                                         spriteBatch.Draw(weatherImage, new Vector2(show.X + 40, show.Y), null, Microsoft.Xna.Framework.Color.White, 0f, new Vector2(20, 20), 0.5f, SpriteEffects.None, 1);
                                         //spriteBatch.End();
                                     }
                                     catch (Exception e) {
                                         spriteBatch.DrawString(spriteFont, show.pic, new Vector2(show.X, show.Y + 15), Microsoft.Xna.Framework.Color.Yellow);   
                                     }
                                }
                            }
                            else
                            {
                                string[] results = key.Split(':');
                                string[] results2 = results[0].Split('.');
                                string[] results3 = results[1].Split('.');
                                string latlonstring = results2[0] + ":" + results3[0];
                                spriteBatch.DrawString(spriteFont, latlonstring, new Vector2(test.X, test.Y),
                                Microsoft.Xna.Framework.Color.Red);
                            }
                        }
                    }
                }
                #endregion

                lock (watchList)
                {
                    foreach (Location latlon in watchList)
                    {
                        string key = latlon.Latitude + ":" + latlon.Longitude;
                        Location pos = grid.LatLonToPixel(latlon);
                        WeatherShowText test = new WeatherShowText(pos, new Weather());
                        test.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);

                        lock (weatherdict)
                        {
                            WeatherShowText show;
                            if (weatherdict.TryGetValue(key, out show))
                            {
                                show.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);
                                spriteBatch.DrawString(spriteFont, show.text, new Vector2(show.X, show.Y),
                                    Microsoft.Xna.Framework.Color.Yellow);


                                if (show.pic != null && show.pic != "")
                                {
                                //String[] pics = new String[] { "rainy", "cloudy", "partly_cloudy", "sunny"}; 
                                //String pic = pics[new Random().Next(0, 4)];
                                    try
                                    {
                                        string file = System.IO.Path.Combine(QS._qss_x_.Reflection_.Library._LIVEOBJECTS_ROOT_, "images\\weather\\");
                                        file += show.pic + ".bmp";
                                        FileStream fv = new FileStream(file, FileMode.Open);
                                        weatherImage = Texture2D.FromStream(device, fv);
                                        //spriteBatch.Begin();
                                        spriteBatch.Draw(weatherImage, new Vector2(show.X + 40, show.Y), null, Microsoft.Xna.Framework.Color.White, 0f, new Vector2(20, 20), 0.5f, SpriteEffects.None, 1);
                                        //spriteBatch.End();
                                    }
                                    catch (Exception e) {
                                        spriteBatch.DrawString(spriteFont, show.pic, new Vector2(show.X, show.Y + 15), Microsoft.Xna.Framework.Color.Yellow);  
                                    }
                                }
                            }
                            else
                            {
                                string[] results = key.Split(':');
                                string[] results2 = results[0].Split('.');
                                string[] results3 = results[1].Split('.');
                                string latlonstring = results2[0] + ":" + results3[0];
                                spriteBatch.DrawString(spriteFont, latlonstring, new Vector2(test.X, test.Y), 
                                Microsoft.Xna.Framework.Color.Red);
                            }
                        }
                    }
                }

                //lock (zipList)
                //{
                //    int count = 0;
                //    foreach (string zipcode in zipList)
                //    {
                //        m_font.DrawString(10, 40 + 15 * count++,
                //                Microsoft.Xna.Framework.Graphics.Color.Red, zipcode);
                //    }
                //}

                #region comment debug
                //System.IO.StreamWriter file = new System.IO.StreamWriter("c:\\debug.txt", true);
                //file.WriteLine("Draw text " + center.X + "," + center.Y);
                //file.Flush();
                //file.Close();
                #endregion
                spriteBatch.End();
            }
        }

        #endregion

        #region IUI_X Members

        QS.Fx.Endpoint.Classes.IExportedUI_X QS.Fx.Object.Classes.IUI_X.UI
        {
            get { return this.windowendpoint; }
        }

        #endregion

        #region IWeatherRendererOps Members

        void IWeatherRendererOps.ReportWeather(Location l, Weather w)
        {
            lock (weatherdict)
            {
                string latlong = l.Latitude + ":" + l.Longitude;

                Location d3 = grid.LatLonToPixel(w.loc);
                WeatherShowText show = new WeatherShowText(d3, w);
                show.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);
                weatherdict.Add(latlong, show);
            }
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Based on the matrix, calculate the lat/lon need to fetch weather
        /// </summary>
        private void CalculateGrid()
        {
            lock (zipList)
            {
                zipList.Clear();
            }

            lock (weatherList)
            {
                // Clear old list to show
                weatherList.Clear();

                // Current range of map
                Location topleft = grid.GetTopLeftCornerOfEarthFromMatrix(this._cameramatrix, this._projectionmatrix);
                Location topleftlanlon = grid.PixelToLatLon(topleft);

                Location downright = grid.GetBottomRightCornerOfEarthFromMatrix(this._cameramatrix, this._projectionmatrix);
                Location downrightlanlon = grid.PixelToLatLon(downright);

                // Grid generation
                for (int i = 1; i <= showdiv; i++)
                {
                    for (int j = 1; j <= showdiv; j++)
                    {
                        Location itemlatlon = new Location(
                            (downrightlanlon.Latitude - topleftlanlon.Latitude) / (showdiv + 1) * i + topleftlanlon.Latitude,
                            (downrightlanlon.Longitude - topleftlanlon.Longitude) / (showdiv + 1) * j + topleftlanlon.Longitude,
                            0f);

                        if (itemlatlon.Longitude < -180 || itemlatlon.Longitude > 180
                            || itemlatlon.Latitude < -90 || itemlatlon.Latitude > 90)
                            continue;

                        weatherList.Add(itemlatlon);

                        //// For zipcode
                        //InvokeFunction invoke = LatLonToZip;
                        //invoke.BeginInvoke(itemlatlon,
                        //    delegate(IAsyncResult ar)
                        //    {
                        //        invoke.EndInvoke(ar);
                        //    }, null);
                    }
                }
            }

            lock (watchList)
            {
                watchList.Clear();

                if (!file.Equals("") && File.Exists(file))
                {
                    XmlDocument watchfile = new XmlDocument();

                    try
                    {
                        watchfile.Load(file);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(e.Message + file);
                    }

                    XmlNodeList nodelist = watchfile.SelectNodes("watch/pos");

                    foreach (XmlNode pos in nodelist)
                    {
                        string data = pos.Attributes["data"].InnerText;

                        string[] results = data.Split(':');

                        Location loc = new Location(
                            float.Parse(results[0]),
                            float.Parse(results[1]),
                            0.0f);
                        watchList.Add(loc);
                    }
                }
                else
                {
                    // test
                    /*
                    Location ithaca = new Location(42.44f, -76.5f, 0.0f);
                    Location neworleans = new Location(29.98f, -90.25f, 0.0f);

                    watchList.Add(ithaca);
                    watchList.Add(neworleans);
                     * */
                }
            }

            lock (weatherdict)
            {
                #region test
                //if (!weatherdict.ContainsKey(42.44f + ":" + -76.5f))
                //{

                //    this.weathersourceendpoint.Interface.LoadWeather(ithaca, true);
                //}
                //if (!weatherdict.ContainsKey(29.98f + ":" + -90.25f))
                //{

                //    this.weathersourceendpoint.Interface.LoadWeather(neworland, true);
                //}
                #endregion

                Random ra = new Random();
                //String[] pics = new String[] { "chance_of_snow", 
                //    "fog", "rain", "cloudy", "partly_cloudy", "sunny" };
                //String pic = pics[new Random().Next(0, 4)];

                lock (weatherList)
                {                    

                    foreach (Location latlon in weatherList)
                    {
                        if (!weatherdict.ContainsKey(latlon.Latitude + ":" + latlon.Longitude))
                        {
                            if (this.isinternet)
                            {
                                this.weathersourceendpoint.Interface.LoadWeather(latlon, true);
                            }
                            else
                            {
                                string latlong = latlon.Latitude + ":" + latlon.Longitude;
                                Weather w = new Weather();
                                w.loc = latlon;
                                w.temp_f = ra.Next(100).ToString();
                                int pic = ra.Next(1, 9);
                                switch (pic)
                                {
                                    case 1: w.picture = "chance_of_snow";
                                        break;
                                    case 2: w.picture = "cloudy";
                                        break;
                                    case 3: w.picture = "fog";
                                        break;
                                    case 4: w.picture = "haze";
                                        break;
                                    case 5: w.picture = "icy";
                                        break;
                                    case 6: w.picture = "mostly_cloudy";
                                        break;
                                    case 7: w.picture = "rain";
                                        break;
                                    case 8: w.picture = "snow";
                                        break;
                                    default: w.picture = "sunny";
                                        break;
                                }
                                Location d3 = grid.LatLonToPixel(w.loc);
                                WeatherShowText show = new WeatherShowText(d3, w);
                                show.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);
                                weatherdict.Add(latlong, show);
                            }
                        }
                    }
                }

                lock (watchList)
                {
                    foreach (Location latlon in watchList)
                    {
                        if (!weatherdict.ContainsKey(latlon.Latitude + ":" + latlon.Longitude))
                        {
                            if (this.isinternet)
                            {
                                this.weathersourceendpoint.Interface.LoadWeather(latlon, true);
                            }
                            else
                            {
                                string latlong = latlon.Latitude + ":" + latlon.Longitude;
                                Weather w = new Weather();
                                w.loc = latlon;
                                w.temp_f = ra.Next(100).ToString();                                
                                int pic = ra.Next(1,9);
                                switch (pic)
                                {
                                    case 1: w.picture = "chance_of_snow";
                                        break;
                                    case 2: w.picture = "cloudy";
                                        break;
                                    case 3: w.picture = "fog";
                                        break;
                                    case 4: w.picture = "haze";
                                        break;
                                    case 5: w.picture = "icy";
                                        break;
                                    case 6: w.picture = "mostly_cloudy";
                                        break;
                                    case 7: w.picture = "rain";
                                        break;
                                    case 8: w.picture = "snow";
                                        break;
                                    default: w.picture = "sunny";
                                        break;
                                }
                                Location d3 = grid.LatLonToPixel(w.loc);
                                WeatherShowText show = new WeatherShowText(d3, w);
                                show.Projection(device, _projectionmatrix, _cameramatrix, _modelmatrix);
                                weatherdict.Add(latlong, show);
                            }
                        }
                    }
                }
            }

        }

        private void ClearWeather()
        {
            lock (weatherdict)
            {
                weatherdict.Clear();
            }
        }

        #endregion

        #region MSNVirtual Earth Operations

        private delegate void InvokeFunction(Location loc);

        private void LatLonToZip(Location loc)
        {
            try
            {
                // Get a Virtual Earth token before making a request
                string token = GetToken();

                liveobjects_8.VEGeocodingService.ReverseGeocodeRequest reverseGeocodeRequest = new liveobjects_8.VEGeocodingService.ReverseGeocodeRequest();

                // Set the credentials using a valid Virtual Earth token
                reverseGeocodeRequest.Credentials = new liveobjects_8.VEGeocodingService.Credentials();
                reverseGeocodeRequest.Credentials.ApplicationId = "AoCladQBfA8MKxe0JQG4XOm0SLA0KaYMDT47byTjhDEaAFGdtq42VLncEBoLKY8V";

                // Set the GeoCodeLocation by lat/lon
                liveobjects_8.VEGeocodingService.GeocodeLocation geoLoc = new liveobjects_8.VEGeocodingService.GeocodeLocation();
                geoLoc.Latitude = loc.Latitude;
                geoLoc.Longitude = loc.Longitude;
                geoLoc.Altitude = loc.Altitude;
                reverseGeocodeRequest.Location = geoLoc;

                // Make the reverse geocode request
                liveobjects_8.VEGeocodingService.GeocodeServiceClient geocodeService = new liveobjects_8.VEGeocodingService.GeocodeServiceClient("BasicHttpBinding_IGeocodeService");

                liveobjects_8.VEGeocodingService.GeocodeResponse geocodeResponse = geocodeService.ReverseGeocode(reverseGeocodeRequest);

                if (geocodeResponse.Results.Count() > 0)
                {
                    lock (zipList)
                    {
                        zipList.Add(geocodeResponse.Results[0].Address.PostalCode);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occurred: " + ex.Message);
            }
        }

        // The GetToken function needs to be called once before any web service request is made. Once
        //    the token is retrieved, it can be cached for use in subsequent web service requests.

        private string GetToken()
        {
            /* Set Virtual Earth Platform Developer Account credentials to access the Token Service
            liveobjects_8.VEStagingToken.CommonService commonService = new liveobjects_8.VEStagingToken.CommonService();
            commonService.Credentials = new System.Net.NetworkCredential("136513", "LiveObjects08!");

            // Set the token specification properties
            liveobjects_8.VEStagingToken.TokenSpecification tokenSpec = new liveobjects_8.VEStagingToken.TokenSpecification();
            string myHost = System.Net.Dns.GetHostName();
            string myIP = System.Net.Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            tokenSpec.ClientIPAddress = myIP;
            tokenSpec.TokenValidityDurationMinutes = 60;
            */
            string token = "AoCladQBfA8MKxe0JQG4XOm0SLA0KaYMDT47byTjhDEaAFGdtq42VLncEBoLKY8V";

            // Get a token
            try
            {
                //token = commonService.GetClientToken(tokenSpec);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return token;
        }

        #endregion
    }
}

#endif
