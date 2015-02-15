/*

Copyright (c) 2009, Jared Cantwell, Petko Nikolov. All rights reserved.

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
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using System.Net;

#if RELEASE3

using liveobjects_8.VEStagingToken;
using liveobjects_8.VEGeocodingService;


namespace Demo
{
#if XNA 
    
    [QS.Fx.Reflection.ComponentClass(
    "91`1", "LocationJumper", "Connects to a camera and enables location jumping.")]
    public partial class LocationJumper : UserControl, QS.Fx.Object.Classes.IUI, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>
    {
        private float worldWidth;
        private float worldHeight;

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                UpdateWindowEvent, UpdateWindowEvent>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                UpdateWindowEvent, UpdateWindowEvent>> channelendpoint;
        private QS.Fx.Endpoint.IConnection channelconnection;

        private QS.Fx.Endpoint.Internal.IExportedUI myendpoint;

        public LocationJumper(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
                QS.Fx.Object.IReference<
                    QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                        UpdateWindowEvent, UpdateWindowEvent>> channel)
        {
            InitializeComponent();

            this.myendpoint = _mycontext.ExportedUI(this);

            if (channel != null)
            {
                channelendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                        UpdateWindowEvent, UpdateWindowEvent>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                        UpdateWindowEvent, UpdateWindowEvent>>(this);
                channelconnection = channelendpoint.Connect(channel.Dereference(_mycontext).Channel);
            }

            worldHeight = worldWidth = 131072f;
        }

        private void latLongButton_Click(object sender, EventArgs e)
        {
            if (latitude.Text.Equals(""))
                latitude.Text = y.Text;
            if (longitude.Text.Equals(""))
                longitude.Text = x.Text;
            if (altitude.Text.Equals(""))
                altitude.Text = z.Text;


            UpdateWindowEvent uwe = new UpdateWindowEvent(-1, new Demo.Xna.Vector3((float)Convert.ToDouble(longitude.Text), (float)Convert.ToDouble(latitude.Text), Convert.ToSingle(altitude.Text)),
                                                            null,
                                                            null,
                                                            true);

            this.channelendpoint.Interface.Send(uwe);
        }

        #region ICheckpointedCommunicationChannelClient<UpdateWindowEvent,UpdateWindowEvent> Members

        UpdateWindowEvent QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Checkpoint()
        {
            return new UpdateWindowEvent();
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Initialize(UpdateWindowEvent _checkpoint)
        {
            if (_checkpoint != null && _checkpoint.CameraPosition != null)
            {
                x.Text = _checkpoint.CameraPosition.X.ToString();
                y.Text = _checkpoint.CameraPosition.Y.ToString();
                z.Text = _checkpoint.CameraPosition.Z.ToString();
            }
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<UpdateWindowEvent, UpdateWindowEvent>.Receive(UpdateWindowEvent _message)
        {
            if (_message != null && _message.CameraPosition != null)
            {
                x.Text = _message.CameraPosition.X.ToString();
                y.Text = _message.CameraPosition.Y.ToString();
                z.Text = _message.CameraPosition.Z.ToString();
            }
        }

        #endregion

        #region IUI Members

        QS.Fx.Endpoint.Classes.IExportedUI QS.Fx.Object.Classes.IUI.UI
        {
            get { return this.myendpoint; }
        }

        #endregion

        private void LocationJumper_Load(object sender, EventArgs e)
        {

        }

        private void locButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Get a Virtual Earth token before making a request
                string token = GetToken();

                GeocodeRequest geocodeRequest = new GeocodeRequest();

                // Set the credentials using a valid Virtual Earth token
                geocodeRequest.Credentials = new Credentials();
                geocodeRequest.Credentials.ApplicationId = "AoCladQBfA8MKxe0JQG4XOm0SLA0KaYMDT47byTjhDEaAFGdtq42VLncEBoLKY8V";

                // Set the full address query
                geocodeRequest.Query = geocodeLoc.Text;

                // Set the options to only return high confidence results 
                ConfidenceFilter[] filters = new ConfidenceFilter[1];
                filters[0] = new ConfidenceFilter();
                filters[0].MinimumConfidence = Confidence.High;

                GeocodeOptions geocodeOptions = new GeocodeOptions();
                geocodeOptions.Filters = filters;

                geocodeRequest.Options = geocodeOptions;

                // Make the geocode request
                GeocodeServiceClient geocodeService = new GeocodeServiceClient("BasicHttpBinding_IGeocodeService");
                GeocodeResponse geocodeResponse = geocodeService.Geocode(geocodeRequest);

                MapLibrary.Location loc = toCoords((float)geocodeResponse.Results[0].Locations[0].Longitude, (float)-geocodeResponse.Results[0].Locations[0].Latitude);

                UpdateWindowEvent uwe = new UpdateWindowEvent(-1, new Demo.Xna.Vector3((float)loc.Longitude, (float)loc.Latitude, 100),
                                                null,
                                                null,
                                                true);
                this.channelendpoint.Interface.Send(uwe);

            }
            catch (Exception ex)
            {
                notes.Text = "An exception occurred: " + ex.Message;

            }
        }

        private MapLibrary.Location toCoords(float lon, float lat)
        {
            return new MapLibrary.Location(worldWidth * 0.5f * (1f - (float)(Math.Log(Math.Tan((double)lat * Math.PI / 180.0 * 0.5 + Math.PI * 0.25), Math.E)) / (float)Math.PI),
                                (lon + 180.0f) * worldWidth / 360.0f,
                                0);
        }

        private string GetToken()
        {
            // Set Virtual Earth Platform Developer Account credentials to access the Token Service
            CommonService commonService = new CommonService();
            commonService.Credentials = new System.Net.NetworkCredential("136513", "LiveObjects08!");

            // Set the token specification properties
            TokenSpecification tokenSpec = new TokenSpecification();
            //Insert the client IP address
            string myHost = System.Net.Dns.GetHostName();
            string myIP = System.Net.Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();
            tokenSpec.ClientIPAddress = myIP;
            tokenSpec.TokenValidityDurationMinutes = 480;

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
    }
#endif
}

#endif
