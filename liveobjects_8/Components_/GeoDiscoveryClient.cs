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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.Net.Sockets;
using System.IO;


//using com.amazon.s3;

namespace MapLibrary
{
#if XNA
    [QS.Fx.Reflection.ComponentClass("198A24C4D9FF4bba999A24A1C97C15D6", "GeoDiscoveryClient", "A discovery client that uses Amazon S3 to store object XML")]
    public sealed class GeoDiscoveryClient: QS.Fx.Object.Classes.IObject, IGeoDiscoveryClient, IGeoDiscoveryClientOps, IDisposable
    {
        #region Fields

        private string awsAccessKeyId;
        private string awsSecretAccessKey;
        private string bucketName;

        [QS.Fx.Base.Inspectable("xnawindowendpoint")]
        QS.Fx.Endpoint.Internal.IExportedInterface<IGeoDiscoveryClientOps> _xnawindowendpoint;

        private Dictionary<String, String> objCache;
        private List<String> ownObjects;
        //private AWSAuthConnection conn;
        private AutoResetEvent xmlDataDownloaded;
        private int numObjectsDownloaded;
        private int numObjectsToDownload;
        private object numObjectsDownloadedLock;

        private Socket discServerUpdate;
        private Socket discServerQuery;
        private Socket discServerRegister;
        private Socket discServerXML;

        private bool useS3;
        private string gdsAddress;

        #endregion

        #region Constructor

        public GeoDiscoveryClient(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("GeoDiscoveryServer Address", QS.Fx.Reflection.ParameterClass.Value)]
            String gdsAddress,
            [QS.Fx.Reflection.Parameter("UseS3?", QS.Fx.Reflection.ParameterClass.Value)]
            bool useS3,
            [QS.Fx.Reflection.Parameter("AWSAccessKeyID", QS.Fx.Reflection.ParameterClass.Value)]
            String awsAccessKeyId,
            [QS.Fx.Reflection.Parameter("AWSSecretAccessKey", QS.Fx.Reflection.ParameterClass.Value)]
            String awsSecretAccessKey,
            [QS.Fx.Reflection.Parameter("XMLBucketName", QS.Fx.Reflection.ParameterClass.Value)]
            String xmlBucketName)
        {
            this.gdsAddress = gdsAddress;
            this.useS3 = useS3;
            if (useS3)
            {
                throw new Exception("Use of S3 currently not supported");
                //if (awsAccessKeyId == null || awsSecretAccessKey == null || xmlBucketName == null)
                //    throw new Exception("An essential S3 parameter of GeoDiscoveryClient is missing!");
                //this.awsAccessKeyId = awsAccessKeyId;
                //this.awsSecretAccessKey = awsSecretAccessKey;
                //this.bucketName = xmlBucketName;
            }

            //else
                //throw new Exception("Non-S3 client not implemented");

            if (gdsAddress == null)
                throw new Exception("Must supply IP address of GeoDiscovery Server!");

            this._xnawindowendpoint = _mycontext.ExportedInterface<IGeoDiscoveryClientOps>(this);

            //this.conn = new AWSAuthConnection(awsAccessKeyId, awsSecretAccessKey);
            this.objCache = new Dictionary<string, string>();
            this.ownObjects = new List<string>();
            this.xmlDataDownloaded = new AutoResetEvent(false);
            this.numObjectsDownloadedLock = new object();
            //if (useS3)
                //this.ClearBuckets();

            discServerUpdate = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            discServerQuery = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            discServerRegister = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint discServerEP = new IPEndPoint(IPAddress.Parse(gdsAddress), 6464);
            discServerUpdate.Connect(discServerEP);

            discServerQuery.Connect(discServerEP);
            discServerRegister.Connect(discServerEP);

            discServerXML = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint discServerEP = new IPEndPoint(IPAddress.Parse(gdsAddress), 6464);
            discServerXML.Connect(discServerEP);
        }

        #endregion

        //private void ClearBuckets()
        //{
        //    ListBucketResponse lbr = null;
        //    while (lbr == null || lbr.Status != HttpStatusCode.OK)
        //    {
        //        if (lbr != null)
        //            lbr.Dispose();
        //        lbr = conn.listBucket(bucketName, null, null, 0, null);
        //    }

        //    ArrayList keys = lbr.Entries;
        //    foreach (ListEntry key in keys)
        //    {
        //        Response r = null;
        //        while (r == null || r.Status != HttpStatusCode.NoContent)
        //        {
        //            if (r != null)
        //                r.Dispose();
        //            try
        //            {
        //                r = conn.delete(bucketName, key.Key, null);
        //            }
        //            catch (System.Net.WebException e) { if (r != null) r.Dispose(); r = null; }
        //        }
        //        r.Dispose();
        //    }
        //    lbr.Dispose();
        //}

        #region IGeoDiscoveryClient Members

        QS.Fx.Endpoint.Classes.IExportedInterface<IGeoDiscoveryClientOps> IGeoDiscoveryClient.GeoDiscoveryClient
        {
            get { return this._xnawindowendpoint; }
        }

        #endregion

        #region IGeoDiscoveryClientOps Members

        private void RegisterAsync(string key, string objXML)
        {
            if (useS3)
            {
                //S3Object obj = new S3Object(objXML, null);
                //Response r = null;
                //while (r == null || r.Status != HttpStatusCode.OK)
                //{
                //    if (r != null)
                //        r.Dispose();
                //    try
                //    {
                //        r = conn.put(bucketName, key, obj, null);
                //    }
                //    catch (System.Net.WebException e) { if (r != null) r.Dispose(); r = null; }
                //}
                //r.Dispose();
            }

            else
            {
                string toSend = "%Register%" + key + "%" + objXML;
                byte[] toSendB = System.Text.Encoding.ASCII.GetBytes(toSend);
                byte[] lenBytes = BitConverter.GetBytes(toSendB.Length);
                lock (discServerRegister)
                {
                    discServerRegister.Send(lenBytes);
                    discServerRegister.Send(toSendB);
                }
            }
        }

        void IGeoDiscoveryClientOps.Register(string key, string objXML)
        {
            lock (ownObjects)
                if (!ownObjects.Contains(key))
                    ownObjects.Add(key);
            RegisterAsyncDel del = new RegisterAsyncDel(RegisterAsync);
            AsyncCallback cb = new AsyncCallback(RegisterAsyncEnd);
            del.BeginInvoke(key, objXML, cb, null);
        }

        private delegate void RegisterAsyncDel(string key, string objXML);

        private void RegisterAsyncEnd(IAsyncResult res)
        {
            RegisterAsyncDel ul = (RegisterAsyncDel)((AsyncResult)res).AsyncDelegate;
            ul.EndInvoke(res);
        }

        void IGeoDiscoveryClientOps.UpdateLocation(string key, Location loc, float minZoom, float maxZoom)
        {
            lock (ownObjects)
            {
                if (!discServerUpdate.Connected || !ownObjects.Contains(key))
                    return;
            }

            string toSend = "%UpdateLocation" + "%" + key + "%" + loc.Longitude + "%" + loc.Latitude + "%" + loc.Altitude + "%" + minZoom + "%" + maxZoom;
            byte[] toSendB = System.Text.Encoding.ASCII.GetBytes(toSend);
            byte[] lenBytes = BitConverter.GetBytes(toSendB.Length);
            lock (discServerUpdate)
            {
                discServerUpdate.Send(lenBytes);
                discServerUpdate.Send(toSendB);
            }
        }

        private delegate void UpdateLocationDel(string key, Location loc, float minZoom, float maxZoom);

        private void UpdateLocationEnd(IAsyncResult res)
        {
            UpdateLocationDel ul = (UpdateLocationDel)((AsyncResult)res).AsyncDelegate;
            ul.EndInvoke(res);
        }

        GeoDiscoveryObjects IGeoDiscoveryClientOps.GetObjectKeys(float top, float bottom, float left, float right, float zoomLevel)
        {
            if (!discServerQuery.Connected)
                return null;

            Dictionary<string, string> objects = new Dictionary<string, string>();

            string toSend = "%GetObjectKeys" + "%" + top + "%" + bottom + "%" + left + "%" + right + "%" + zoomLevel;
            byte[] toSendB = System.Text.Encoding.ASCII.GetBytes(toSend);
            byte[] lenBytes = BitConverter.GetBytes(toSendB.Length);
            discServerQuery.Send(lenBytes);
            discServerQuery.Send(toSendB);

            //byte[] rec = new byte[100000];
            byte[] lenBuf = new byte[4];
            int count = ReceiveNBytes(discServerQuery, 4, lenBuf);
            if (count != 4)
                throw new Exception("Error in client receiving length of next message: " + count);
            int readLength = BitConverter.ToInt32(lenBuf, 0);
            byte[] rec = new byte[readLength];
            int len = ReceiveNBytes(discServerQuery, readLength, rec);
            char[] chars = new char[len];
            System.Text.Decoder d = System.Text.Encoding.ASCII.GetDecoder();
            int charLen = d.GetChars(rec, 0, len, chars, 0);
            string message = new string(chars);

            string[] messageParts = message.Split(new char[] { '%' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> keys = new List<string>();

            for (int i = 0; i < messageParts.Length; i++)
                lock (ownObjects)
                    if (!ownObjects.Contains(messageParts[i]) && !messageParts[i].Equals(""))
                        keys.Add(messageParts[i]);


            numObjectsToDownload = keys.Count;
            numObjectsDownloaded = 0;

            if (numObjectsToDownload > 0)
            {
                foreach (string key in keys)
                {
                    //DownloadXMLDataDel del = new DownloadXMLDataDel(DownloadXMLData);
                    //AsyncCallback cb = new AsyncCallback(DownloadXMLDataEnd);
                    //del.BeginInvoke(key, objects, cb, null);
                    this.DownloadXMLData(key, objects);
                }
                //xmlDataDownloaded.WaitOne();
            }

            return new GeoDiscoveryObjects(objects);
        }

        private delegate void DownloadXMLDataDel(string key, Dictionary<string, string> objects);

        private void DownloadXMLData(string key, Dictionary<string, string> objects)
        {
            String objXml = null;
            bool objCacheLocked = true;
            Monitor.Enter(objCache);
            if (!objCache.ContainsKey(key))
            {
                objCacheLocked = false;
                Monitor.Exit(objCache);

                if (useS3)
                {
                    //GetResponse r = null;

                    //try
                    //{
                    //    r = conn.get(bucketName, key, null);
                    //}
                    //catch (System.Net.WebException e) { if (r != null) r.Dispose(); r = null; }

                    //if (r != null)
                    //{
                    //    if (r.Status == HttpStatusCode.OK)
                    //        objXml = r.Object.Data;
                    //    r.Dispose();
                    //}
                }

                else
                {
                    //Socket discServerXML = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    //IPEndPoint discServerEP = new IPEndPoint(IPAddress.Parse(gdsAddress), 6464);
                    //discServerXML.Connect(discServerEP);

                    byte[] toSendB = System.Text.Encoding.ASCII.GetBytes("%GetObjectXML%" + key);
                    byte[] lenBytes = BitConverter.GetBytes(toSendB.Length);
                    discServerXML.Send(lenBytes);
                    discServerXML.Send(toSendB);

                    byte[] lenBuf = new byte[4];
                    int count = ReceiveNBytes(discServerXML, 4, lenBuf);
                    if (count != 4)
                        throw new Exception("Error in client receiving length of next message: " + count);
                    int readLength = BitConverter.ToInt32(lenBuf, 0);
                    if (readLength != 0)
                    {
                        byte[] rec = new byte[readLength];
                        int len = ReceiveNBytes(discServerXML, readLength, rec);
                        char[] chars = new char[len];
                        System.Text.Decoder d = System.Text.Encoding.ASCII.GetDecoder();
                        int charLen = d.GetChars(rec, 0, len, chars, 0);
                        objXml = new string(chars);
                    }

                    //discServerXML.Shutdown(SocketShutdown.Both);
                    //discServerXML.Close();
                }

                if (objXml != null)
                {
                    lock (objCache)
                        objCache.Add(key, objXml);
                }
            }
            if (objCacheLocked)
                Monitor.Exit(objCache);

            lock (ownObjects)
            {
                if (!ownObjects.Contains(key) && objCache.ContainsKey(key) && !objects.ContainsKey(key))
                {
                    objXml = objCache[key];
                    objects.Add(key, objXml);
                }
            }
            /*
            lock (numObjectsDownloadedLock)
                if (++numObjectsDownloaded == numObjectsToDownload)
                    xmlDataDownloaded.Set();
             * */
        }

        private void DownloadXMLDataEnd(IAsyncResult res)
        {
            DownloadXMLDataDel ul = (DownloadXMLDataDel)((AsyncResult)res).AsyncDelegate;
            ul.EndInvoke(res);
        }


        void IGeoDiscoveryClientOps.Delete(string key)
        {
            lock (ownObjects)
            {
                if (!discServerUpdate.Connected || !ownObjects.Contains(key))
                    return;
            }

            string toSend = "%Delete" + "%" + key;
            byte[] toSendB = System.Text.Encoding.ASCII.GetBytes(toSend);
            byte[] lenBytes = BitConverter.GetBytes(toSendB.Length);
            lock (discServerUpdate)
            {
                discServerUpdate.Send(lenBytes);
                discServerUpdate.Send(toSendB);
            }

            lock (ownObjects)
                ownObjects.Remove(key);
        }

        private delegate void DeleteDel(string key);

        private void DeleteEnd(IAsyncResult res)
        {
            DeleteDel ul = (DeleteDel)((AsyncResult)res).AsyncDelegate;
            ul.EndInvoke(res);
        }

        #endregion

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

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            discServerQuery.Shutdown(SocketShutdown.Both);
            discServerQuery.Close();

            discServerUpdate.Shutdown(SocketShutdown.Both);
            discServerUpdate.Close();

            discServerRegister.Shutdown(SocketShutdown.Both);
            discServerRegister.Close();

            discServerXML.Shutdown(SocketShutdown.Both);
            discServerXML.Close();
        }

        #endregion
    }
#endif
}
