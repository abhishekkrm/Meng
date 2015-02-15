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
using System.Net;
using System.Xml;
using System.Threading;

using MapLibrary;

namespace WeatherLibrary
{
    [QS.Fx.Reflection.ComponentClass("105`1", "GoogleWeather", "A weather source")]
    public sealed class GoogleWeather: IWeatherSource, IWeatherSourceOps
    {
        //public GoogleWeather()
        //{
        //    this.myendpoint = _mycontext.DualInterface<IWeatherRendererOps, IWeatherSourceOps>(this);
        //}

        public GoogleWeather(QS.Fx.Object.IContext _mycontext)
        {
            this.myendpoint = _mycontext.DualInterface<IWeatherRendererOps, IWeatherSourceOps>(this);
            w = new Weather();
        }

        #region Field
        
        private QS.Fx.Endpoint.Internal.IDualInterface<IWeatherRendererOps, IWeatherSourceOps> myendpoint;

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int defaultTimeout = 60 * 1000; // 1 minutes timeout

        Weather w;

        private class RequestState
        {
            public Location loc;
            public HttpWebRequest request;
            public HttpWebResponse response;
            public QS.Fx.Endpoint.Internal.IDualInterface<IWeatherRendererOps, IWeatherSourceOps> endpoint;
            public RequestState(Location loc)
            {
                request = null;
                this.loc = loc;
                response = null;
            }
        }

        #endregion

        #region IWeatherSource Members

        QS.Fx.Endpoint.Classes.IDualInterface<IWeatherRendererOps, IWeatherSourceOps> IWeatherSource.WeatherSource
        {
            get { return this.myendpoint; }
        }

        #endregion

        #region IWeatherSourceOps Members

        void IWeatherSourceOps.LoadWeather(Location l, bool async)
        {
            if (l.Latitude == 0f)
                l.Latitude = 0.01f;
            if (l.Longitude == 0f)
                l.Longitude = 0.01f;

            DownloadWeather(l, async);
        }

        #endregion

        #region AsyncCallbacks

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        private static void RespCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                HttpWebResponse myHttpWebResponse = myRequestState.response;
                Location loc = myRequestState.loc;

                myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                XmlDocument googleweather = new XmlDocument();
                googleweather.Load(myHttpWebResponse.GetResponseStream());

                myHttpWebResponse.Close();

                Weather w = ParseCurrent(googleweather.SelectNodes("xml_api_reply/weather/current_conditions"), loc);
                
                if (w != null)
                    myRequestState.endpoint.Interface.ReportWeather(loc, w);

                return;
            }
            catch (WebException e)
            {
                Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            //allDone.Set();
        }

        private static Weather ParseCurrent(XmlNodeList current, Location l)
        {
            if (current.Count > 0)
            {
                Weather w = new Weather();
                w.loc = l;
                w.condition = current[0].SelectSingleNode("condition").Attributes["data"].InnerText;
                w.temp_c = current[0].SelectSingleNode("temp_c").Attributes["data"].InnerText;
                w.temp_f = current[0].SelectSingleNode("temp_f").Attributes["data"].InnerText;
                w.humidity = current[0].SelectSingleNode("humidity").Attributes["data"].InnerText;
                w.wind = current[0].SelectSingleNode("wind_condition").Attributes["data"].InnerText;
                
                w.picture = current[0].SelectSingleNode("icon").Attributes["data"].InnerText;
                string[] results = w.picture.Split('/');
                string[] results2 = results.ElementAt(results.Count() - 1).Split('.');
                w.picture = results2[0];
 
                return w;
            }
            else
            {
                return null;
            }
        }

        #endregion

        private void DownloadWeather(Location loc, bool async)
        {
            int Latitude = (int)(loc.Latitude * 1000000);
            int Longitude = (int)(loc.Longitude * 1000000);

            String URL = "http://www.google.com/ig/api?hl=en-us&weather=,,," + Latitude + "," + Longitude;

            
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
            myRequest.Method = "GET";
            myRequest.UserAgent = @"Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.4) Gecko/20070515 Firefox/2.0.0.4";

            try
            {
                RequestState myRequestState = new RequestState(loc);
                myRequestState.request = myRequest;
                myRequestState.endpoint = myendpoint;
                IAsyncResult result = 
                    (IAsyncResult)myRequest.BeginGetResponse(
                    new AsyncCallback(RespCallback), myRequestState);
                
                ////Set timeout
                //ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle,
                //    new WaitOrTimerCallback(TimeoutCallback), myRequest, defaultTimeout, true);

                //// The response came in the allowed time. The work processing will happen in the 
                //// callback function.
                //allDone.WaitOne();

                //// Release the HttpWebResponse resource.
                //myRequestState.response.Close();
            }
            catch (WebException e)
            {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
                //Console.WriteLine("Press any key to continue..........");
            }
            catch (Exception e)
            {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("Source :{0} ", e.Source);
                Console.WriteLine("Message :{0} ", e.Message);
                //Console.WriteLine("Press any key to continue..........");
                //Console.Read();
            }

        }

    }
}
