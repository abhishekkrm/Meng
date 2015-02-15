/* Copyright (c) 2009 Jared Cantwell, Petko Nikolov. All rights reserved.

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
using System.Net;
using System.Threading;

//using liveobjects_8.VEStagingToken;
using liveobjects_8.VEGeocodingService;
using liveobjects_8.VEImageryService;
using liveobjects_8.VERoutingService;
using liveobjects_8.VESearchService;

using System.Drawing;
using System.IO;

using System.Windows.Forms;

namespace MapLibrary
{
    [QS.Fx.Reflection.ComponentClass("3A96A1A73BB9461285011A7F07D9D314", "MSN Virtual Earth", "A map source")]
    public sealed class MSNVEarth : IMapSource, IMapSourceOps
    {
        public MSNVEarth(
            QS.Fx.Object.IContext _mycontext, 
            [QS.Fx.Reflection.Parameter("MapType: 1-Aerial,2-Road,3-Traffic", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 whichMap,
            [QS.Fx.Reflection.Parameter("Do not change (default: 7)", QS.Fx.Reflection.ParameterClass.Value)]
            Int32 magicNumber,
            [QS.Fx.Reflection.Parameter("Subnet", QS.Fx.Reflection.ParameterClass.Value)]
            String subnet,
            [QS.Fx.Reflection.Parameter("Common Serice URL (default: https://staging.common.virtualearth.net/find-30/common.asmx", QS.Fx.Reflection.ParameterClass.Value)]
            String commonServiceUrl,
            [QS.Fx.Reflection.Parameter("MSN VEarth Username", QS.Fx.Reflection.ParameterClass.Value)]
            String userName,
            [QS.Fx.Reflection.Parameter("MSN VEarth Password", QS.Fx.Reflection.ParameterClass.Value)]
            String password
            )
        {
            this.whichMap = whichMap;
            this.magicNumber = magicNumber;
            this.commonServiceUrl = commonServiceUrl;
            this.userName = userName;
            this.password = password;

            if (commonServiceUrl == null || userName == null || password == null || subnet == null)
                MessageBox.Show("Caution: One or more essential parameters of MSN VEarth component are missing!");

            this.myendpoint = _mycontext.DualInterface<IMapManagerOps, IMapSourceOps>(this);
            //myendpoint.OnConnect += new QS.Fx.Base.Callback(myendpoint_OnConnect);
            
            // Get the local interface
            if (subnet == null)
                subnet = "0.0.0.0/0";
            this._subnet = new QS._qss_c_.Base1_.Subnet(subnet);
            QS.Fx.Platform.IPlatform _platform = _mycontext.Platform;
            String _hostname = _platform.Network.GetHostName();
            this._networkinterface = null;

            foreach (QS.Fx.Network.INetworkInterface _nic in _platform.Network.Interfaces)
            {
                if (_nic.InterfaceAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (this._subnet.contains(_nic.InterfaceAddress))
                    {
                        this._networkinterface = _nic;
                        break;
                    }
                }
            }
            if (this._networkinterface == null)
                throw new Exception("Could not locate any network adapter on the requested subnet " + this._subnet.ToString() + ".");

            if (this._networkinterface.InterfaceAddress == null)
                throw new Exception("null InterfaceAddress on requested subnet.");

            _myIp = this._networkinterface.InterfaceAddress;
            
            
            SetToken();
            imageWidth = 512; //max is 900
            imageHeight = 512; //max is 834

            //locs = new Queue<Location>();
        }

        

        void myendpoint_OnConnect()
        {
            SetToken();
        }

        #region Private Fields

        //////////// Saved parameters

        private Int32 whichMap;
        private Int32 magicNumber;
        private string userName;
        private string password;
        private string commonServiceUrl;

        ////////////

        private bool internet;

        private Queue<Location> locs;
        private QS.Fx.Endpoint.Internal.IDualInterface<IMapManagerOps, IMapSourceOps> myendpoint;

        private IPAddress _myIp;
        private string clientToken;
        private int imageHeight;
        private int imageWidth;

        private QS._qss_c_.Base1_.Subnet _subnet;
        private QS.Fx.Network.INetworkInterface _networkinterface;

        #endregion

        #region IMapSource Members

        QS.Fx.Endpoint.Classes.IDualInterface<IMapManagerOps, IMapSourceOps> IMapSource.MapManager
        {
            get { return this.myendpoint; }
        }

        #endregion

        #region IMapSourceOps Members

        void IMapSourceOps.LoadImage(Location l, bool async)
        {
            if (!internet)
                return;
            
            Thread imageDownloaderT = new Thread(new ParameterizedThreadStart(this.DownloadImageObj));
            //MyNewThread8.Priority = ThreadPriority.Highest;
            
            imageDownloaderT.Start(l);
        }

        #endregion

        private void ImageThreadFunc()
        {
            Location loc = new Location();
            
            lock (locs)
            {
                loc = locs.Dequeue();    
            }

            //DownloadImage
            return;
            
            
            bool go = true;

            while (true)
            {

                while (go)
                {
                    go = false;
                    lock (locs)
                    {
                        if (locs.Count > 0)
                        {
                            loc = locs.Dequeue();
                            go = true;
                        }
                    }

                    if (go)
                        DownloadImage(loc);
                }

                go = false;
                Thread.Sleep(100);
            }
        }

        private void DownloadImageObj(Object loc)
        {
            try
            {
                Location l = (Location)loc;
                DownloadImage((Location)loc);
            }
            catch (Exception e) {
                /*
                lock (this)
                {
                    TextWriter tw = new StreamWriter("c:\\debug.txt", true);
                    tw.WriteLine(e.ToString());
                    tw.Close();
                }
                 * */
            }
        }

        private void DownloadImage(Location loc)
        {
            bool async = true;
            //VEImageryService.Location loc = new MapLibrary.VEImageryService.Location();
            //loc.Latitude = (double)(l.Latitude);
            //loc.Latitude = (double)(l.Latitude);
            //int zLevel = (int)(l.Altitude);
            //file.WriteLine("DownloadImage ENTER: " + DateTime.Now.Second + "," + DateTime.Now.Millisecond);
            //file.Flush();
            MapUriRequest mapUriRequest = new MapUriRequest();

            // Set credentials using a valid Virtual Earth Token
            mapUriRequest.Credentials = new liveobjects_8.VEImageryService.Credentials();
            mapUriRequest.Credentials.ApplicationId = "AoCladQBfA8MKxe0JQG4XOm0SLA0KaYMDT47byTjhDEaAFGdtq42VLncEBoLKY8V";

            // Set the location of the requested image
            mapUriRequest.Center = new liveobjects_8.VEImageryService.Location();
            //string[] digits = locationString.Split(',');
            mapUriRequest.Center.Latitude = (double)(loc.Latitude);
            mapUriRequest.Center.Longitude = (double)(loc.Longitude);

            // Set the map style and zoom level
            MapUriOptions mapUriOptions = new MapUriOptions();
            if (whichMap == 1)
                mapUriOptions.Style = liveobjects_8.VEImageryService.MapStyle.AerialWithLabels;
            else if (whichMap == 2)
            {
                mapUriOptions.Style = liveobjects_8.VEImageryService.MapStyle.Road;
                //mapUriOptions.DisplayLayers = new string[] { "TrafficFlow" };
            }
            else if (whichMap == 3)
            {
                mapUriOptions.Style = liveobjects_8.VEImageryService.MapStyle.Road;
                mapUriOptions.DisplayLayers = new string[] { "TrafficFlow" };
            }
            else
            {
                // Invalid map option => THROW EXCEPTION?
                return;
            }

            //mapUriOptions.DisplayLayers = new string[] { "TrafficFlow" };
            mapUriOptions.ZoomLevel = (int)(loc.Altitude);

            // Set the size of the requested image in pixels
            mapUriOptions.ImageSize = new liveobjects_8.VEImageryService.SizeOfint();
            mapUriOptions.ImageSize.Height = imageHeight;
            mapUriOptions.ImageSize.Width = imageWidth;
            mapUriRequest.Options = mapUriOptions;

            ImageryServiceClient imageryService = new ImageryServiceClient("BasicHttpBinding_IImageryService");
            MapUriResponse mapUriResponse;
            try
            {
                mapUriResponse = imageryService.GetMapUri(mapUriRequest);
            }
            catch (Exception e)
            {
                MessageBox.Show("Catch start" + e.Data + " " + e.Message + " " + e.Source+ "  " +e.StackTrace);
                throw e;
                imageryService.Close();
                return;
            }
            String mapUri = "http://" + mapUriResponse.Uri.Substring(magicNumber);

            WebClient client = new WebClient();

            if (async)
            {
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
                client.DownloadDataAsync(new Uri(mapUri), loc);
            }
            else
            {
                byte[] img = client.DownloadData(mapUri);
                client.Dispose();
            }

            imageryService.Close();
        }

        private void SetToken()
        {
            //CommonService commonService = null;
            try
            {
                /*// Create a reference to the web service
                commonService = new CommonService();
                commonService.Url = commonServiceUrl;

                //Use your developer credentials
                commonService.Credentials =
                  new System.Net.NetworkCredential(userName, password);

                // Create the TokenSpecification object to pass to GetClientToken.
                TokenSpecification tokenSpec = new TokenSpecification();

                //Insert the client IP address
                //string myHost = System.Net.Dns.GetHostName();
                //string myIP = System.Net.Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
                tokenSpec.ClientIPAddress = _myIp.ToString();

                // The maximum allowable token duration is 480 minutes (8 hours).
                // The minimum allowable duration is 15 minutes.
                tokenSpec.TokenValidityDurationMinutes = 480;
                */
                internet = true;
            
                //clientToken = commonService.GetClientToken(tokenSpec);
                clientToken = "AoCladQBfA8MKxe0JQG4XOm0SLA0KaYMDT47byTjhDEaAFGdtq42VLncEBoLKY8V";
            }
            catch (Exception e)
            {
                internet = false;
            }
            //commonService.Dispose();
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                ((WebClient)sender).Dispose();
                byte[] img = (byte[])(e.Result);

                Location loc = (Location)(e.UserState);

                myendpoint.Interface.ImageLoadedCallback(loc, img);
            }
            catch (Exception ex) { }
        }
    }
}
