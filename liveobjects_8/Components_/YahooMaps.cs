/*

Copyright (c) 2007-2009 Jared Cantwell (jmc279@cornell.edu), Petko Nikolov (pn42@cornell.edu). All rights reserved.

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
using System.Net;
using System.Text.RegularExpressions;

//using liveobjects_8.VEStagingToken;
using liveobjects_8.VEGeocodingService;
using liveobjects_8.VEImageryService;
using liveobjects_8.VERoutingService;
using liveobjects_8.VESearchService;

using System.Drawing;
using System.IO;

namespace MapLibrary
{
    [QS.Fx.Reflection.ComponentClass("54`1", "YahooMaps", "A map source")]
    public sealed class YahooMaps : IMapSource, IMapSourceOps
    {
        public YahooMaps(QS.Fx.Object.IContext _mycontext)
        {
            //throw new Exception("yoooo");
            this.myendpoint = _mycontext.DualInterface<IMapManagerOps, IMapSourceOps>(this);
            //myendpoint.OnConnect += new QS.Fx.Base.Callback(myendpoint_OnConnect);
            imageWidth = 512; //max is 900
            imageHeight = 512; //max is 834
            API_KEY = "IcZc4nbV34FcB2NdxVULz6WLmL9te9rwfUww5T48vMzTLunbiiGWPROX4yTkYAewvoEjr4s-";
        }

        void myendpoint_OnConnect()
        {
        }

        #region Private Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<IMapManagerOps, IMapSourceOps> myendpoint;

        private string clientToken;
        private int imageHeight;
        private int imageWidth;
        String API_KEY;

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
            if (l.Latitude == 0f)
                l.Latitude = 0.01f;
            if (l.Longitude == 0f)
                l.Longitude = 0.01f;

            DownloadImage(l, async);
        }

        #endregion

        private void DownloadImage(Location loc, bool async)
        {
            WebClient client = new WebClient();
 
            String URL = "http://local.yahooapis.com/MapsService/V1/mapImage?appid=" + API_KEY;
            URL += "&latitude=" + loc.Latitude + "&longitude=" + loc.Longitude +
                    "&zoom=12&image_height=" + imageHeight + "&image_width=" + imageWidth;

            //throw new Exception(URL);

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            //String result = "<?xml version=\"1.0\"?><Result xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">http://gws.maps.yahoo.com/mapimage?MAPDATA=npokt.d6wXVVDqdt67vSzL9Vx_ZCvCXkOxEKLGI9tvbJxgGMo63vnk7erSw1E5_eQcmErFAnKBrEGpYNR7KI.wL9j.nQn3dIML87FoBOOCHdqA--&amp;mvt=m?cltype=onnetwork&amp;.intl=us</Result><!-- ws06.search.re2.yahoo.com compressed/chunked Tue Nov 25 09:52:33 PST 2008 -->";
            Regex exp = new Regex(@"<Result[^>]*>([^<]+)<", RegexOptions.IgnoreCase);
            MatchCollection MatchList = exp.Matches(result);
            String imageURI = MatchList[0].Groups[1].Value;

            //return new Bitmap(WebRequest.Create(imageURI).GetResponse().GetResponseStream());

            if (async)
            {
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(client_DownloadDataCompleted);
                client.DownloadDataAsync(new Uri(imageURI), loc);
            }
            else
            {
                //byte[] img = client.DownloadData(pimpMyMap(mapUriResponse.Uri));
                //client.Dispose();
            }
        }

        void client_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            ((WebClient)sender).Dispose();
            byte[] img = (byte[])(e.Result);
            Location loc = (Location)(e.UserState);
            myendpoint.Interface.ImageLoadedCallback(loc, img);
        }

    }
}

