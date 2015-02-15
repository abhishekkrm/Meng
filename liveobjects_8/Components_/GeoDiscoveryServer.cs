/*

Copyright (c) 2004-2009 Petko Nikolov. All rights reserved.

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

#define VERBOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.IO;

#if XNA
using Demo.Xna;
#endif

namespace MapLibrary
{
#if XNA
    [QS.Fx.Reflection.ComponentClass("2D226186C516421a92A93CECA45EDB68", "GeoDiscoveryServer", "A discovery server that stores object location information")]
    public sealed class GeoDiscoveryServer : QS.Fx.Object.Classes.IObject, 
        IGeoDiscoveryServerOps, IDisposable, 
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GeoDiscoveryEvent, GeoDiscoveryEvent>
    {
        #region Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                GeoDiscoveryEvent, GeoDiscoveryEvent>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                GeoDiscoveryEvent, GeoDiscoveryEvent>> channelendpoint;
        private QS.Fx.Endpoint.IConnection channelconnection;

        private Dictionary<string, string> objXML;

        private ObjectTree objTree;

        private Socket clientListener;

        private AutoResetEvent newConn = new AutoResetEvent(false);

        [QS.Fx.Base.Inspectable]
        protected QS.Fx.Logging.ILogger _logger;


        #endregion

        #region Constructor

        public GeoDiscoveryServer(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<
                QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<
                    GeoDiscoveryEvent, GeoDiscoveryEvent>> channel)
        {
            this._logger = _mycontext.Platform.Logger;

            if (channel != null)
            {
                channelendpoint = _mycontext.DualInterface<
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<
                        GeoDiscoveryEvent, GeoDiscoveryEvent>,
                    QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<
                        GeoDiscoveryEvent, GeoDiscoveryEvent>>(this);
                channelconnection = channelendpoint.Connect(channel.Dereference(_mycontext).Channel);
            }
            this.objXML = new Dictionary<string, string>();

            this.objTree = new ObjectTree(20);

            clientListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint local = new IPEndPoint(IPAddress.Any, 6464);
            clientListener.Bind(local);
            clientListener.Listen(1000);

            StartListeningDel sl = new StartListeningDel(StartListening);
            sl.BeginInvoke(new AsyncCallback(StartListeningEnd), null);
        }

        private delegate void StartListeningDel();

        private void StartListeningEnd(IAsyncResult res)
        {
            StartListeningDel ul = (StartListeningDel)((AsyncResult)res).AsyncDelegate;
            ul.EndInvoke(res);
        }

        private void StartListening()
        {
            while (true)
            {
                newConn.Reset();
                clientListener.BeginAccept(new AsyncCallback(OnClientConnect), null);
                newConn.WaitOne();
            }
        }

        private int ReceiveNBytes(Socket client, int readLength, byte[] buf)
        {
            int total = 0;

            while (true)
            {
                int numRead = client.Receive(buf, total, readLength - total, SocketFlags.None);
                total += numRead;
                if (total < readLength)
                    Thread.Sleep(100);
                else
                    break;
            }

            return total;
        }

        private void OnClientConnect(IAsyncResult res)
        {
            newConn.Set();
            Socket client = clientListener.EndAccept(res);
            byte[] lenBuf = new byte[4];
            byte[] buf;
            string leftover = "";
            int count;

            while (client.Connected)
            {
                int len;
                try
                {
                    count = ReceiveNBytes(client, 4, lenBuf);
                    if (count == 0)
                        break;
                    if (count != 4)
                        throw new Exception("Error in server receiving length of next message: " + count);
                    int readLength = BitConverter.ToInt32(lenBuf, 0);
                    buf = new byte[readLength];
                    len = ReceiveNBytes(client, readLength, buf);
                }
                catch (SocketException se) { break; }

                char[] chars = new char[len];
                System.Text.Decoder d = System.Text.Encoding.ASCII.GetDecoder();
                int charLen = d.GetChars(buf, 0, len, chars, 0);
                string message = leftover + new string(chars);
                leftover = "";

                string[] messageParts = message.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);
                int i = 0;

                //while (i < messageParts.Length)
                //{
                    if (messageParts[i].Equals("UpdateLocation"))
                    {

                        string key;
                        float lon;
                        float lat;
                        float alt;
                        float minZoom;
                        float maxZoom;
                        Vector3 l;
                        try
                        {
                            key = messageParts[i + 1];
                            lon = float.Parse(messageParts[i + 2]);
                            lat = float.Parse(messageParts[i + 3]);
                            alt = float.Parse(messageParts[i + 4]);
                            minZoom = float.Parse(messageParts[i + 5]);
                            maxZoom = float.Parse(messageParts[i + 6]);
                            l = new Vector3(lon, lat, alt);
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message + " " + message);
                        }
                        channelendpoint.Interface.Send(new GeoDiscoveryEvent(GeoDiscoveryEvent.EventType.UpdatePosition, key, l, minZoom, maxZoom));
                        i += 7;
                    }
                    else if (messageParts[i].Equals("Delete"))
                    {

                        string key = messageParts[i + 1];
                        channelendpoint.Interface.Send(new GeoDiscoveryEvent(GeoDiscoveryEvent.EventType.Delete, key, null, 0f, 0f));
                        i += 2;
                    }
                    else if (messageParts[i].Equals("GetObjectKeys"))
                    {

                        float top = float.Parse(messageParts[i + 1]);
                        float bottom = float.Parse(messageParts[i + 2]);
                        float left = float.Parse(messageParts[i + 3]);
                        float right = float.Parse(messageParts[i + 4]);
                        float zoomLevel = float.Parse(messageParts[i + 5]);
                        List<String> objects = objTree.GetNearbyObjects(top, bottom, left, right, zoomLevel);
                        StringBuilder sb = new StringBuilder("%");
                        foreach (string key in objects)
                            sb.Append(key + "%");

                        byte[] sendbuf = System.Text.Encoding.ASCII.GetBytes(sb.ToString());
                        byte[] lenBytes = BitConverter.GetBytes(sendbuf.Length);
                        client.Send(lenBytes);
                        client.Send(sendbuf);
#if VERBOSE
                        if (this._logger != null)
                            if (objects.Count != 0)
                                this._logger.Log("Sent " + objects.Count + " relevant keys to " + client.Handle.ToString());
#endif
                        i += 6;
                    }
                    else if (messageParts[i].Equals("GetObjectXML"))
                    {
                        string key = messageParts[i + 1];
                        string xml = null;
                        lock (objXML)
                            if (objXML.ContainsKey(key))
                                xml = objXML[key];

                        byte[] lenBytes;
                        if (xml != null)
                        {
                            //throw new Exception(xml);
                            byte[] toSendB = System.Text.Encoding.ASCII.GetBytes(xml);
                            lenBytes = BitConverter.GetBytes(toSendB.Length);
                            client.Send(lenBytes);
                            client.Send(toSendB);
                        }
                        else
                        {
                            lenBytes = BitConverter.GetBytes(0);
                            client.Send(lenBytes);
                        }
#if VERBOSE
                        if (this._logger != null)
                            this._logger.Log("Sent xml of key " + key + " to " + client.Handle.ToString());
#endif

                        i += 2;
                    }
                    else if (messageParts[i].Equals("Register"))
                    {
                        string key = messageParts[i + 1];
                        string xml = messageParts[i + 2];

                        lock (objXML)
                        {
                            try
                            {
                                objXML.Add(key, xml);
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Registered key " + key);
#endif
                            }
                            catch (ArgumentException e)
                            {
#if VERBOSE
                                if (this._logger != null)
                                    this._logger.Log("Key " + key + " already added!");
#endif
                            }
                        }

                        i += 3;
                    }
                    else if (i == messageParts.Length - 1)
                        leftover = messageParts[i];
                    else
                        throw new Exception("Invalid operation received from client: " + messageParts[i] + ": " + message);
                //}

                //buf = new byte[10000];
            }
        }

        #endregion

        #region IGeoDiscoveryServerOps Members

        void  IGeoDiscoveryServerOps.UpdateLocation(string key, Location loc, float minZoom, float maxZoom)
        {
            Vector3 l = new Vector3(loc.Longitude, loc.Latitude, loc.Altitude);
            channelendpoint.Interface.Send(new GeoDiscoveryEvent(GeoDiscoveryEvent.EventType.UpdatePosition,key, l, minZoom, maxZoom));
        }

        GeoDiscoveryObjects IGeoDiscoveryServerOps.GetObjectKeys(float top, float bottom, float left, float right, float zoomLevel)
        {
            List<String> objects = objTree.GetNearbyObjects(top, bottom, left, right, zoomLevel);
            return new GeoDiscoveryObjects(objects);
        }

        void IGeoDiscoveryServerOps.Delete(string key)
        {
            channelendpoint.Interface.Send(new GeoDiscoveryEvent(GeoDiscoveryEvent.EventType.Delete, key, null, 0f, 0f));
        }

        #endregion
    
        #region ICheckpointedCommunicationChannelClient<GeoDiscoveryEvent,GeoDiscoveryEvent> Members

        void  QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GeoDiscoveryEvent,GeoDiscoveryEvent>.Receive            (GeoDiscoveryEvent _message)
        {
            if (_message != null)
            {
                if (_message.myEvent == GeoDiscoveryEvent.EventType.UpdatePosition)
                {
                    Location loc = new Location(_message.loc.Y, _message.loc.X, _message.loc.Z);
                    ObjectInfo objInfo = new ObjectInfo(_message.key, loc, _message.minZoom, _message.maxZoom);
                    objTree.Insert(objInfo);
                }
                else if (_message.myEvent == GeoDiscoveryEvent.EventType.Delete)
                {
                    objTree.Delete(_message.key);
                    lock (objXML)
                        objXML.Remove(_message.key);
                }
            }
        }

        void  QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GeoDiscoveryEvent,GeoDiscoveryEvent>.Initialize(GeoDiscoveryEvent _checkpoint)
        {
 	        //throw new NotImplementedException();
        }

        GeoDiscoveryEvent  QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<GeoDiscoveryEvent,GeoDiscoveryEvent>.Checkpoint()
        {
            return null;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {

        }

        #endregion
    }
#endif
}
