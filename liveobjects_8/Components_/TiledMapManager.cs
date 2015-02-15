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
using System.Collections;
using System.Drawing;

using System.Threading;
using System.IO;

using System.Windows.Forms;

#if XNA

namespace MapLibrary
{
    [QS.Fx.Reflection.ComponentClass("B6A0CCB1D93B4ac2B5F544EEB59C27B7", "Tiled Map Manager", "Manages a map broken up into square tiles")]
    public sealed class TiledMapManager : IMapManager, IMapManagerOps, IDisposable
    {
        public TiledMapManager(
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("MapSource", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<IMapSource> mapSource,
            [QS.Fx.Reflection.Parameter("Cache", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<ICache> cache)
        {
            this.mapSourceEndpoint = _mycontext.DualInterface<IMapSourceOps, IMapManagerOps>(this);
            this.mapSourceConnection = mapSourceEndpoint.Connect(mapSource.Dereference(_mycontext).MapManager);

            this.cacheEndpoint = _mycontext.ImportedInterface<ICacheOps>();

            if (cache != null)
                this.cacheConnection = cacheEndpoint.Connect(cache.Dereference(_mycontext).Cache);

            this.contentRenderer = _mycontext.DualInterface<IContentRendererOps, IMapManagerOps>(this);

            myLoc = new Location(-100000000f, -1000000000f, -1f);
            myLocLock = new object();

            tileWidth = 500f;
            tileHeight = 500f;
            worldHeight = worldWidth = 131072f;
            numTilesStored = 0;
            maxAlt = 300000f;
            myAltitude = maxAlt;
            zoomScaleRatio = 2.0f;
            zoomedIn = zoomedOut = false;

            tileBuffer = new Hashtable();

            gridTrans = new GridTranslator();

            locsToProcess = new Stack();
            getImageryT = new Thread(new ThreadStart(this.GetImageryDel));
            running = true;
            //t.Priority = ThreadPriority.Highest;
            
        }

        #region Private Fields

        private QS.Fx.Endpoint.Internal.IDualInterface<IMapSourceOps, IMapManagerOps> mapSourceEndpoint;
        private QS.Fx.Endpoint.IConnection mapSourceConnection;

        private QS.Fx.Endpoint.Internal.IImportedInterface<ICacheOps> cacheEndpoint;
        private QS.Fx.Endpoint.IConnection cacheConnection;

        private QS.Fx.Endpoint.Internal.IDualInterface<IContentRendererOps, IMapManagerOps> contentRenderer;

        private Location myLoc;
        private float myAltitude;
        private object myLocLock;   // Used to lock current location

        private float tileWidth;
        private float tileHeight;
        private float maxAlt;
        private float zoomScaleRatio;
        private float worldHeight;
        private float worldWidth;
        private Boolean flushOnNext;
        private bool zoomedIn;
        private bool zoomedOut;

        private int numTilesStored;

        private Hashtable tileBuffer = new Hashtable();

        private GridTranslator gridTrans;

        private Stack locsToProcess;
        private Thread getImageryT;
        private AutoResetEvent worktodo = new AutoResetEvent(false);
        private int threadworking;

        #endregion

        #region IMapManager Members

        QS.Fx.Endpoint.Classes.IDualInterface<IMapSourceOps, IMapManagerOps> IMapManager.MapSource
        {
            get { return this.mapSourceEndpoint; }
        }

        QS.Fx.Endpoint.Classes.IDualInterface<IContentRendererOps, IMapManagerOps> IMapManager.ContentRenderer
        {
            get { return this.contentRenderer; }
        }

        #endregion

        #region IMapManagerOps Members

        void IMapManagerOps.ImageLoadedCallback(Location loc, byte[] tile)
        {
            lock (myLocLock)
            {
                loc = gridTrans.LatLonToPixel(loc);

                string key = GetKey(loc);
                if (tileBuffer[key] == null)
                {
                    numTilesStored++;
                    lock (tileBuffer)
                        tileBuffer[key] = tile;

                    if (cacheEndpoint.IsConnected && !cacheEndpoint.Interface.Contains(key))
                        cacheEndpoint.Interface.Add(key, tile);

                    //TODO: check current location to see if it still makes sense to send over this image

                    SendToRenderer(loc);
                }
            }
        }

        List<String> processedKeys = new List<String>();

        void IMapManagerOps.CurrentLocation(Location currLoc)
        {
 //           MessageBox.Show("TiledMapManager: CurrentLocation(): " + currLoc.Longitude + " " + currLoc.Latitude + " " + currLoc.Altitude);
            
            if (!isInvalidCoord(currLoc))
            {
                float alt = currLoc.Altitude;
                float factor = maxAlt / alt;
                int zLevel = (int)(Math.Ceiling(Math.Log((double)factor, (double)zoomScaleRatio)));

                currLoc = new Location(currLoc.Latitude, currLoc.Longitude, zLevel);

                lock (processedKeys)
                {
                    if (!processedKeys.Contains(GetKey(currLoc)))
                    {
                        processedKeys.Add(GetKey(currLoc));

                        lock (myLocLock)
                        {
                            //MessageBox.Show("Key wasn't equal!" + lastKey + ":" + GetKey(currLoc));

                            if (myLoc.Altitude != zLevel)
                            {
                                myLoc.Altitude = zLevel;
                                processedKeys.Clear();
                                processedKeys.Add(GetKey(currLoc));
                                this.flushOnNext = true;
                                int factor2 = ((int)(Math.Pow(2.0, (double)myLoc.Altitude - 1)));
                                this.tileHeight = worldHeight / factor2;
                                this.tileWidth = worldWidth / factor2;

                            }
                            myLoc.Latitude = currLoc.Latitude;
                            myLoc.Longitude = currLoc.Longitude;
                            myLoc.Latitude = (float)(Math.Floor((double)((myLoc.Latitude) / tileHeight))) * tileHeight + tileHeight / 2.0f;
                            myLoc.Longitude = (float)(Math.Floor((double)((myLoc.Longitude) / tileWidth))) * tileWidth + tileWidth / 2.0f;
                            zoomedOut = zoomedIn = false;

                            //MessageBox.Show("thread is " + getImageryT.ThreadState.ToString());

                            if (!flushOnNext)
                            {
                                lock (locsToProcess)
                                {
                                    //MessageBox.Show("processing new location");
                                    locsToProcess.Push(new Location(myLoc.Latitude, myLoc.Longitude, myLoc.Altitude));
                                    if (Interlocked.CompareExchange(ref this.threadworking, 1, 0) == 0)
                                        getImageryT.Start();
                                    //if (getImageryT.ThreadState == ThreadState.Suspended)
                                    //    getImageryT.Resume();
                                    worktodo.Set();
                                }
                            }

                            else
                                GetImagery(false);
                        }
                    }
                    else if (myAltitude > alt)
                    {
                        myAltitude = alt;
                        if (!zoomedIn)
                        {
                            zoomedIn = true;

                            Location zoomed = myLoc;
                            zoomed.Altitude += 1;
                            zoomed = GetKeyedLoc(zoomed);
                            zoomed.Latitude += tileHeight / 4;
                            zoomed.Longitude += tileWidth / 4;
                            //processedKeys.Add(GetKey(zoomed));

                            lock (locsToProcess)
                            {
                                //MessageBox.Show("processing new location");
                                locsToProcess.Push(new Location(zoomed.Latitude, zoomed.Longitude, zoomed.Altitude));
                                if (Interlocked.CompareExchange(ref this.threadworking, 1, 0) == 0)
                                    getImageryT.Start();
                                //if (getImageryT.ThreadState == ThreadState.Suspended)
                                //    getImageryT.Resume();
                                worktodo.Set();
                            }
                        }
                    }
                    else if (myAltitude < alt)
                    {
                        myAltitude = alt;
                        if (!zoomedOut)
                        {
                            zoomedOut = true;

                            Location zoomed = myLoc;
                            zoomed.Altitude -= 1;
                            zoomed = GetKeyedLoc(zoomed);
                            zoomed.Latitude += tileHeight;
                            zoomed.Longitude += tileWidth;
                            LoadTileAt(zoomed);
                        }
                    }
                }
            }
            else
            {
                //throw new Exception("Invalid Coords: " + currLoc.Latitude + "," + currLoc.Latitude + "," + currLoc.Altitude);
            }
        }

        #endregion

        private void SendToRenderer(Location loc)
        {
            if (flushOnNext)
            {
                this.contentRenderer.Interface.FlushContent();
                flushOnNext = false;
            }

            lock (myLocLock)
            {
                if (myLoc.Altitude == loc.Altitude)
                {
                    byte[] tile = null;
                    lock (tileBuffer)
                        tile = (byte[])tileBuffer[GetKey(loc)];
                    Stream s = new MemoryStream(tile);
                    Location newLoc = new Location();
                    int factor = ((int)(Math.Pow(2.0, (double)loc.Altitude - 1)));
                    float loctileHeight = worldHeight / factor;
                    float loctileWidth = worldWidth / factor;
                    newLoc.Latitude = (float)(Math.Floor((double)((loc.Latitude) / loctileHeight))) * loctileHeight;
                    newLoc.Longitude = (float)(Math.Floor((double)((loc.Longitude) / loctileWidth))) * loctileWidth;

                    newLoc.Altitude = 0f;
                    Bitmap b = new Bitmap(s);
                    contentRenderer.Interface.RenderContent(new Content(b), newLoc, this.tileWidth, this.tileHeight);
                }
            }
        }

        private void addToCache()
        {

        }
/*
        private Location toLatLon(Location loc)
        {
            return new Location((180.0f / (float)Math.PI) * (2f * (float)(Math.Atan(Math.Exp(Math.PI * (1.0 - 2.0 * (double)(loc.Latitude)/ (double)worldHeight)))) - .5f * (float)Math.PI),
                                loc.Longitude * 360.0f / worldWidth - 180.0f,
                                loc.Altitude);
        }

        private Location toCoords(Location loc)
        {
            return new Location(worldWidth * 0.5f * (1f - (float)(Math.Log(Math.Tan((double)loc.Latitude*Math.PI/180.0 * 0.5 + Math.PI * 0.25), Math.E)) / (float)Math.PI),
                                (loc.Longitude + 180.0f) * worldWidth / 360.0f,
                                loc.Altitude);
        }
*/
        private bool isInvalidCoord(Location loc)
        {

            if (loc.Latitude <= 0f || loc.Latitude >= worldHeight ||
                loc.Longitude <= 0f || loc.Longitude >= worldWidth)
            {
                return true;
            }
            return false;
        }
        
        private String GetKey(Location loc)
        {
            Location l = GetKeyedLoc(loc);
            String s = l.Latitude + "," + l.Longitude + "," + l.Altitude;

            return s;
        }

        private Location GetKeyedLoc(Location loc)
        {
            int factor = ((int)(Math.Pow(2.0, (double)loc.Altitude - 1)));
            float loctileHeight = worldHeight / factor;
            float loctileWidth = worldWidth / factor;
            float down = (int)((float)(Math.Floor((double)((loc.Latitude) / loctileHeight))) * loctileHeight);
            float across = (int)((float)(Math.Floor((double)((loc.Longitude) / loctileWidth))) * loctileWidth);
            Location l = new Location(down, across, loc.Altitude);
            return l;
        }

        private void LoadNeighbors()
        {
            //this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new GetImageryDel(this.GetImagery), false);
        }

        private void LoadTileAt(Location loc)
        {
            string key = GetKey(loc);

            if (tileBuffer[key] != null)
            {
                SendToRenderer(loc);
            }
            else if (cacheEndpoint.IsConnected && cacheEndpoint.Interface.Contains(key))
            {
                //MessageBox.Show("Loading from cache.");
                lock (tileBuffer)
                    if (!tileBuffer.Contains(key))
                        tileBuffer.Add(key, cacheEndpoint.Interface.Get(key));
                SendToRenderer(loc);
            }
            else if (!tileBuffer.Contains(key) && !isInvalidCoord(loc))
            {
                lock (tileBuffer)
                    if (!tileBuffer.Contains(key))
                        tileBuffer.Add(key, null);
              //  MessageBox.Show("Crap! Going to Internet to Load Image.");
                mapSourceEndpoint.Interface.LoadImage(gridTrans.PixelToLatLon(loc), true);
            }
        }

        private void GetImageryDel()
        {   
            GetImagery(true);
        }

        private void GetImagery(bool septhread)
        {
            Location loc = new Location();
            //MessageBox.Show("started getimagery thread");
            bool sleep;
            while (running)
            {
                sleep = false;
                if (septhread)
                {
                    lock (locsToProcess)
                    {
                        if (locsToProcess.Count > 0)
                            loc = (Location)(locsToProcess.Pop());
                        else
                            sleep = true;
                    }
                    if (sleep)
                    {
                        //MessageBox.Show("running is " + running.ToString() + ", going to sleep");
                        worktodo.WaitOne();
                        //MessageBox.Show("running is " + running.ToString() + ", resuming");
                        continue;
                    }
                }
                else
                    loc = myLoc;

                // calculate tileHeight and tileWidth for this zoom level
                int factor = ((int)(Math.Pow(2.0, (double)loc.Altitude - 1)));
                float currTileHeight = worldHeight / factor;
                float currTileWidth = worldWidth / factor;

                if (!isInvalidCoord(loc))
                {
                    LoadTileAt(loc);
                }

                if (true)//(!justMyLoc)
                {
                    // prefetch neighboring images at same zoom level

                    for (int i = 1; i <= 9; i++)
                    {
                        float latDelta = -1;
                        float lonDelta = -1;
                        float lat = -1;
                        float lon = -1;
                        // set the delta for longitude
                        if (i % 3 == 1)
                        {
                            lon = loc.Longitude - currTileWidth;
                            lonDelta = -currTileWidth;
                        }
                        else if (i % 3 == 2)
                        {
                            lon = loc.Longitude;
                            lonDelta = 0;
                        }
                        else if (i % 3 == 0)
                        {
                            lon = loc.Longitude + currTileWidth;
                            lonDelta = currTileWidth;
                        }

                        // set the delta for latitude
                        if (i <= 3)
                        {
                            lat = loc.Latitude + currTileHeight;
                            latDelta = currTileHeight;
                        }
                        else if (i > 3 && i <= 6)
                        {
                            lat = loc.Latitude;
                            latDelta = 0;
                        }
                        else if (i > 6 && i <= 9)
                        {
                            lat = loc.Latitude - currTileHeight;
                            latDelta = -currTileHeight;
                        }

                        Location newLoc = new Location(lat, lon, loc.Altitude);
                        if (!isInvalidCoord(newLoc))
                        {
                            LoadTileAt(newLoc);
                        }

                    }
                }

                if (!septhread)
                    break;
            }
        }

        ~TiledMapManager()
        {
            this._Dispose(false);
        }

        private int _disposed;

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private bool running;

        private void _Dispose(bool _disposemanagedresources)
        {
            if (Interlocked.CompareExchange(ref this._disposed, 1, 0) == 0)
            {
                if (_disposemanagedresources)
                {
                    running = false;
                    //if (getImageryT.ThreadState == ThreadState.Suspended)
                    //    getImageryT.Resume();
                    //ThreadState t = getImageryT.ThreadState;
                    //MessageBox.Show("calling wordtodo.set, thread: " + t.ToString());
                    worktodo.Set();
                    //while (getImageryT.ThreadState == t)
                    //{ }
                    /*
                    if (Interlocked.CompareExchange(ref this.threadworking, 2, 1) == 1)
                        if (!getImageryT.Join(100))
                            getImageryT.Abort();
                    */
                }
            }
        }
    }
}

#endif
