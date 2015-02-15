/*

Copyright (c) 2004-2009 Krzysztof Ostrowski. All rights reserved.

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

// #define DEBUG_AllowCollectingOfStatistics

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace QS._qss_c_.Devices_7_
{
    [QS.Fx.Base.Inspectable]
    [QS._core_c_.Diagnostics.ComponentContainer]
    public class Listener : QS.Fx.Inspection.Inspectable, IListener, IDisposable
    {
        public Listener(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress listeningAddress, int mtu, 
            QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Clock.IClock clock)
        {
            this.interfaceAddress = interfaceAddress;
            this.listeningAddress = listeningAddress;
            this.mtu = mtu;
            this.buffer = new ArraySegment<byte>(new byte[mtu], 0, mtu);
            this.eventLogger = eventLogger;
            this.clock = clock;

            this.socket = QS._qss_c_.Devices_6_.Sockets.CreateReceiverUDPSocket(interfaceAddress, ref this.listeningAddress);
            this.receiveAsyncCallback = new AsyncCallback(this.ReceiveCallback);
            this.mylocation = interfaceAddress.ToString();
            this.mysource = "Listener(" + listeningAddress.ToString() + ")";

            this.enumerator = new MyEnumerator(this);
        }

        private static EndPoint remote_endpoint = new IPEndPoint(IPAddress.Any, 0);

        private string mylocation, mysource;
        private QS.Fx.Clock.IClock clock;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress listeningAddress;
        private Socket socket;
        private ArraySegment<byte> buffer;
        private int mtu, nreceived;
        private EndPoint endpoint;
        // private SocketFlags flags = SocketFlags.None;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private AsyncCallback receiveAsyncCallback;
        private IAsyncResult asynchronousResult;
        private ListenerCallback listenerCallback;
        private bool waiting, available, started;
        private MyEnumerator enumerator;

#if DEBUG_AllowCollectingOfStatistics
        [Diagnostics.Component("Callback Times")]
        private Statistics.Samples timeSeries_callbackTimes = new QS.CMS.Statistics.Samples();
        [Diagnostics.Component("Wait Times")]
        private Statistics.Samples timeSeries_waitTimes = new QS.CMS.Statistics.Samples();
        [Diagnostics.Component("Wait and Callback Times")]
        private TMS.Data.MultiSeries CallbackAndWaitTimes
        {
            get
            {
                TMS.Data.MultiSeries series = new QS.TMS.Data.MultiSeries();
                series.Series.Add("wait", (TMS.Data.DataSeries) timeSeries_waitTimes.DataSet);
                series.Series.Add("callback", (TMS.Data.DataSeries) timeSeries_callbackTimes.DataSet);
                return series;
            }
        }
#endif

        #region Error

        private void Error(string s)
        {
            try
            {
                if (eventLogger.Enabled)
                    eventLogger.Log(new Logging_1_.Events.Error(clock.Time, mylocation, mysource, s));
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region IListener Members

        IPAddress IListener.NIC
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.NetworkAddress IListener.Address
        {
            get { return listeningAddress; }
        }

        event ListenerCallback IListener.OnArrival
        {
            add 
            {
                lock (this)
                {
                    if (listenerCallback != null)
                        throw new Exception("Only one listener callback is allowed.");
                    listenerCallback += value;
                }                
            }

            remove 
            {
                lock (this)
                {
                    listenerCallback -= value;
                }
            }
        }

        void IListener.Start()
        {
            this.Wait();
        }

        void IListener.Stop()
        {
        }

        IEnumerable<IPacket> IListener.Received
        {
            get { return enumerator;  }
        }

        #endregion

        #region Wait

        private void Wait()
        {
            if (waiting)
                throw new Exception("Already started.");

#if DEBUG_AllowCollectingOfStatistics
            if (timeSeries_waitTimes.Enabled)
                timeSeries_waitTimes.addSample(clock.Time);
#endif

            do
            {
                try
                {
                    endpoint = remote_endpoint;
                    waiting = true;
                    asynchronousResult = socket.BeginReceiveFrom(
                        buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, ref endpoint, receiveAsyncCallback, null);
                }
                catch (SocketException exc)
                {
                    waiting = false;
                    this.Error("Could not initiate asynchronous receive: " + exc.SocketErrorCode.ToString());
                }
            }
            while (!waiting);
        }

        #endregion

        #region ReceiveCallback

        private void ReceiveCallback(IAsyncResult asynchronousResult)
        {
            waiting = false;

            try
            {
                if (asynchronousResult != null)
                {
                    if ((nreceived = socket.EndReceiveFrom(asynchronousResult, ref endpoint)) > 0)
                    {
                        available = true;
                    }
                    else
                    {
                        this.Error("Could not finalize asynchronous receive.");

                        available = this.Get();
                    }
                }
                else
                    this.Error("Receive callback invoked on an empty request.");

                if (available)
                {
                    started = false;

#if DEBUG_AllowCollectingOfStatistics
                    if (timeSeries_callbackTimes.Enabled)
                        timeSeries_callbackTimes.addSample(clock.Time);
#endif

                    try
                    {
                        if (this.listenerCallback != null)
                            this.listenerCallback(this);
                        else
                            throw new Exception("No listener.");
                    }
                    catch (Exception exc)
                    {
                        try
                        {
                            if (eventLogger.Enabled)
                                eventLogger.Log(new Logging_1_.Events.ExceptionCaught(clock.Time, mylocation, exc));
                        }
                        catch (Exception)
                        {
                        }

                        this.Wait();
                    }
                }
                else
                    this.Wait();
            }
            catch (Exception exc)
            {
                this.Error(exc.ToString());
                this.Wait();
            }
        }

        #endregion

        #region Get

        private bool Get()
        {
            while (socket.Available > 0)
            {
                try
                {
                    endpoint = remote_endpoint;
                    // flags = SocketFlags.None;

                    if ((nreceived = socket.ReceiveFrom(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None, ref endpoint)) > 0)
                    {
                        return true;
                    }
                    else
                        this.Error("Could not complete synchronous receive.");
                }
                catch (SocketException exc)
                {
                    this.Error("Could not complete synchronous receive: " + exc.SocketErrorCode.ToString());
                }
            }

            return false;
        }

        #endregion    

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            ((IListener) this).Stop();

            try
            {
                socket.Close();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Class MyEnumerator

        private class MyEnumerator : IEnumerable<IPacket>, IEnumerator<IPacket>, IPacket
        {
            public MyEnumerator(Listener owner)
            {
                this.owner = owner;
            }

            private Listener owner;

            #region IPacket Members

            IPEndPoint IPacket.Origin
            {
                get { return (IPEndPoint) (owner.endpoint); }
            }

            ArraySegment<byte> IPacket.Data
            {
                get { return new ArraySegment<byte>(owner.buffer.Array, owner.buffer.Offset, owner.nreceived); }
            }

            #endregion

            #region IEnumerator<IPacket> Members

            IPacket IEnumerator<IPacket>.Current
            {
                get 
                {
                    if (owner.started && owner.available)
                        return this;
                    else
                        throw new Exception("Either not started or no data available.");
                }
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            bool System.Collections.IEnumerator.MoveNext()
            {
                if (owner.started)
                {
                    owner.available = owner.Get();
                    return owner.available;
                }
                else
                {
                    owner.started = true;
                    return true;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return ((IEnumerator<IPacket>)this).Current; }
            }

            void System.Collections.IEnumerator.Reset()
            {
                throw new Exception("Attempting to reset a read-once enumerator.");
            }

            #endregion

            #region IEnumerable<IPacket> Members

            IEnumerator<IPacket> IEnumerable<IPacket>.GetEnumerator()
            {
                return this;
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this;
            }

            #endregion
        }

        #endregion
    }
}
