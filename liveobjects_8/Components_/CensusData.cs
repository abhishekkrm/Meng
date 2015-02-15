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
using System.Net;
using System.IO;
using System.Threading;

#if XNA

using MapLibrary;
using Microsoft.Xna.Framework.Graphics;

namespace Demo
{
    [QS.Fx.Reflection.ComponentClass(
    "1E685CEA13D24591821A15B671A7816E", "CensusData", "Provide Census data pulled from a census website.")]
    class CensusData : ITextRendererClient, ITextRendererClientOps
    {
        private Dictionary<String, String> zipDict;

        private QS.Fx.Endpoint.Internal.IDualInterface<ITextRendererOps, ITextRendererClientOps> clientendpoint;

        private QS.Fx.Endpoint.Internal.IImportedInterface<ICacheOps> cacheEndpoint;
        private QS.Fx.Endpoint.IConnection cacheConnection;

        private ASCIIEncoding ascii;

        public CensusData(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("Cache", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<ICache> cache)
        {
            this.clientendpoint = _mycontext.DualInterface<ITextRendererOps, ITextRendererClientOps>(this);

            this.cacheEndpoint = _mycontext.ImportedInterface<ICacheOps>();

            if (cache != null)
                this.cacheConnection = cacheEndpoint.Connect(cache.Dereference(_mycontext).Cache);

            ascii = new System.Text.ASCIIEncoding();

            zipDict = new Dictionary<string, string>();
        }




        public string fromZipcode(string zip, string attribute, out String number, out String percent)
        {
            String page;

            if (cacheEndpoint.IsConnected)
            {
                if (cacheEndpoint.Interface.Contains("nyc"))
                    page = ascii.GetString(cacheEndpoint.Interface.Get("nyc"));
                else
                {
                    page = _Content(url(zip), Encoding.UTF8);
                    cacheEndpoint.Interface.Add("nyc", ascii.GetBytes(page));
                }
            } else 
                page = _Content(url(zip), Encoding.UTF8);
            
              
            //Console.WriteLine(page);
            page = page.Replace("\n", "").Replace("\r", "");

            number = "";
            percent = "";

            int _i = 0;
            string _s = ">" + attribute + "</p>";
            _i = page.IndexOf(_s);
            if (_i < 0)
                return "_i < 0";

            if (page.Substring(_i - 6, 4).Equals("bold"))
                _s = "<p style=\"text-align:right;font-weight:bold;\">";
            else
                _s = "<p style=\"text-align:right;\">";
            _i = page.IndexOf(_s, _i);
            if (_i < 0)
                return "_i < 0 (2)";
            _i += _s.Length;
            int _j = page.IndexOf("</p>", _i);
            if (_j < 0)
                return "_j < 0";
            number = page.Substring(_i, _j - _i);

            _i = page.IndexOf(_s, _i);
            if (_i < 0)
                return "_i < 0 (2x)";
            _i += _s.Length;
            _j = page.IndexOf("</p>", _i);
            if (_j < 0)
                return "_j < 0 x";
            percent = page.Substring(_i, _j - _i);

            return "";
        }

        public void fromLatLon(float lat, float lon)
        {
        }

        public String url(string zipcode)
        {
                return "http://factfinder.census.gov/servlet/SAFFFacts?_event=Search&geo_id=04000US34&_geoContext=01000US%7C04000US34&_street=&_county=New+York+City&_cityTown=New+York+City&_state=04000US36&_zip=&_lang=en&_sse=on&ActiveGeoDiv=geoSelect&_useEV=&pctxt=fph&pgsl=040&_submenuId=factsheet_1&ds_name=DEC_2000_SAFF&_ci_nbr=null&qr_name=null&reg=null%3Anull&_keyword=&_industry=";
            //return "http://factfinder.census.gov/servlet/QTTable?_bm=y&-geo_id=86000US" + zipcode + "&-qr_name=DEC_2000_SF1_U_DP1&-ds_name=DEC_2000_SF1_U&-_lang=en&-_sse=on";
        }

        static string _Content(string _address, Encoding _encoding)
        {
            WebRequest _request = WebRequest.Create(_address);
            _request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse _response = (HttpWebResponse)_request.GetResponse();
            Stream _stream = _response.GetResponseStream();
            StreamReader _reader = new StreamReader(_stream, _encoding);
            return _reader.ReadToEnd();
        }

        #region ITextRendererClientOps Members

        void ITextRendererClientOps.CurrentLocation(MapLibrary.Location loc, MapLibrary.Location topLeft, MapLibrary.Location bottomRight)
        {
            String zipcode = "kkkk";

            lock (zipDict)
            {
                if (!zipDict.ContainsKey(zipcode))
                {
                    zipDict.Add(zipcode, "");
                    Thread t = new Thread(new ParameterizedThreadStart(this.process));
                    t.Start(loc);
                }
            }


        }

        void process(Object obj)
        {
            //Location loc = (Location)obj;
            String pop, percent;
            String male, malePercent;
            String female, femalePercent;
            String zip = "00083";
            Location loc = new Location(40.77f, -73.99f, 0f);
   
            fromZipcode(zip, "Total population", out pop, out percent);
            fromZipcode(zip, "Male", out male, out malePercent);
            fromZipcode(zip, "Female", out female, out femalePercent);
            
            this.clientendpoint.Interface.DrawText(new Demo.Xna.Vector3(loc.Longitude, loc.Latitude, loc.Altitude),
                new String[]{"Census Data - NYC","Population: " + pop, "Male: " + male, "Female: " + female});//, Color.Red);
        }

        #endregion

        #region ITextRendererClient Members

        QS.Fx.Endpoint.Classes.IDualInterface<ITextRendererOps, ITextRendererClientOps> ITextRendererClient.TextRendererClient
        {
            get { return this.clientendpoint; }
        }

        #endregion
    }
}


#endif
