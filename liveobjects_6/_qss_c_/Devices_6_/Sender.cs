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

#define OPTION_DisableScatterGatherForMonoCompatibility

// #define UseEnhancedRateControl

// #define DEBUG_AllowCollectingOfStatistics
// #define DEBUG_SanityChecks

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace QS._qss_c_.Devices_6_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class Sender : QS.Fx.Inspection.Inspectable, ISender<Base6_.Asynchronous<Block>>
    {
        public Sender(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress destinationAddress, ISenderController senderController)
            : this(interfaceAddress, destinationAddress, senderController, 10)
        {
        }

        public Sender(IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            ISenderController senderController, int maximumConcurrency)
            : this(QS._core_c_.Base2.PreciseClock.Clock, interfaceAddress, destinationAddress, senderController, maximumConcurrency)
        {
        }

        public Sender(QS.Fx.Clock.IClock clock, IPAddress interfaceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
            ISenderController senderController, int maximumConcurrency)
        {
            this.clock = clock;
            this.interfaceAddress = interfaceAddress;
            this.destinationAddress = destinationAddress;
            this.senderController = senderController;
            this.maximumConcurrency = maximumConcurrency;
        }

        private QS.Fx.Clock.IClock clock;
        private IPAddress interfaceAddress;
        private QS.Fx.Network.NetworkAddress destinationAddress;
        private int concurrency, maximumConcurrency;
        private bool registered;
        private ISenderController senderController;
        private Queue<QS._core_c_.Base6.GetObjectsCallback<Base6_.Asynchronous<Block>>> callbackQueue = 
            new Queue<QS._core_c_.Base6.GetObjectsCallback<Base6_.Asynchronous<Block>>>();
        private Queue<Base6_.Asynchronous<Block>> pendingQueue = new Queue<Base6_.Asynchronous<Block>>();
        private Queue<SendOperation> sendOperations = new Queue<SendOperation>();
        private Queue<Base6_.Asynchronous<Block>> transmissionQueue = new Queue<Base6_.Asynchronous<Block>>();

#if DEBUG_AllowCollectingOfStatistics
        [QS.CMS.Diagnostics.Component("Concurrent Sends (X = time, Y = count)")]
        private QS.CMS.Statistics.SamplesXY timeSeries_concurrentSends = new QS.CMS.Statistics.SamplesXY();
        [QS.CMS.Diagnostics.Component("Send Times")]
        private QS.CMS.Statistics.Samples timeSeries_sendTimes = new QS.CMS.Statistics.Samples();
#endif

        #region Adjusting Configuration

        public int MaximumConcurrency
        {
            get { return maximumConcurrency; }
            set { maximumConcurrency = value; }
        }

        #endregion

        #region ProcessOutgoing

        private void ProcessOutgoing()
        {
            while (transmissionQueue.Count > 0 && concurrency < maximumConcurrency)
            {
                SendOperation sendOperation;
                if (sendOperations.Count > 0)
                    sendOperation = sendOperations.Dequeue();
                else
                    sendOperation = new SendOperation(Sockets.CreateSenderUDPSocket(interfaceAddress, destinationAddress));

                Base6_.Asynchronous<Block> toSend = transmissionQueue.Dequeue();
                sendOperation.Argument = toSend;

                concurrency++;

#if DEBUG_AllowCollectingOfStatistics
                if (timeSeries_sendTimes.Enabled)
                    timeSeries_sendTimes.addSample(clock.Time);                    
#endif

#if OPTION_DisableScatterGatherForMonoCompatibility
                byte[] flattened_segments = Base3_.BufferHelper.FlattenBuffers(toSend.Argument.Segments);
                IAsyncResult asynchronousResult = sendOperation.Socket.BeginSend(
                    flattened_segments, 0, flattened_segments.Length, 
                    SocketFlags.None, new AsyncCallback(this.SendCallback), sendOperation);
#else
                IAsyncResult asynchronousResult = sendOperation.Socket.BeginSend(
                    toSend.Argument.Segments, SocketFlags.None, new AsyncCallback(this.SendCallback), sendOperation);
#endif

                if (asynchronousResult.CompletedSynchronously)
                    this.Completed(asynchronousResult, sendOperation);
            }

#if DEBUG_AllowCollectingOfStatistics
            if (timeSeries_concurrentSends.Enabled)
                timeSeries_concurrentSends.addSample(clock.Time, concurrency);
#endif
        }

        #endregion

        #region Completed

        private void Completed(IAsyncResult asynchronousResult, SendOperation sendOperation)
        {
            System.Exception exception = null;
            bool succeeded = true;
            try
            {
                if (sendOperation.Socket.EndSend(asynchronousResult) <= 0)
                    throw new Exception("No bytes sent.");
            }
            catch (Exception exc)
            {
                exception = exc;
                succeeded = false;
            }

            if (sendOperation.Argument.CompletionCallback != null)
            {
                Monitor.Exit(this);
                try
                {
                    sendOperation.Argument.CompletionCallback(succeeded, exception, sendOperation.Argument.Context);
                }
                finally
                {
                    Monitor.Enter(this);
                }
            }
            else
            {
                // We might want to log the error message somewhere...........................
            }

            sendOperation.Argument = default(Base6_.Asynchronous<Block>);
            sendOperations.Enqueue(sendOperation);
            concurrency--;
        }

        #endregion

        #region SendCallback

        private void SendCallback(IAsyncResult asynchronousResult)
        {
            if (!asynchronousResult.CompletedSynchronously)
            {
                SendOperation sendOperation = (SendOperation)asynchronousResult.AsyncState;
                senderController.Release(sendOperation.Argument.Argument.Size, 1);

                lock (this)
                {
                    this.Completed(asynchronousResult, sendOperation);
                    this.ProcessOutgoing();
                }
            }
        }

        #endregion

        #region Class SendOperation

        private class SendOperation
        {
            public SendOperation(Socket socket)
            {
                this.socket = socket;
            }

            public Socket Socket
            {
                get { return socket; }
            }

            public Base6_.Asynchronous<Block> Argument
            {
                set { argument = value; }
                get { return argument; }
            }

            private Socket socket;
            private Base6_.Asynchronous<Block> argument;
        }

        #endregion

        #region IControlledSender Members

        void IControlledSender.Consume(
            int maximumSends, int maximumBytes, out int consumedSends, out int consumedBytes, out bool moreToGo)
        {
            lock (this)
            {
#if DEBUG_SanityChecks
                if (!registered)
                    throw new Exception("__Consume: Not registered.");
#endif

                moreToGo = true;
                consumedBytes = consumedSends = 0;
                while (true)
                {
                    if (pendingQueue.Count > 0)
                    {
                        if (consumedSends < maximumSends)
                        {
                            if (pendingQueue.Peek().Argument.Size > maximumBytes - consumedBytes)
                                break;
                            else
                            {
                                Base6_.Asynchronous<Block> element = pendingQueue.Dequeue();

                                consumedSends++;
                                consumedBytes += element.Argument.Size;

                                transmissionQueue.Enqueue(element);
                            }
                        }
                        else
                            break;
                    }
                    else
                    {
                        while (true)
                        {
                            if (callbackQueue.Count > 0)
                            {
                                if (consumedBytes < maximumBytes && consumedSends < maximumSends)
                                {
                                    QS._core_c_.Base6.GetObjectsCallback<Base6_.Asynchronous<Block>> getCallback = callbackQueue.Dequeue();

                                    // TODO: Implement enhanced rate control

                                    int numberOfObjects;
#if UseEnhancedRateControl
                                    int numberOfBytes;
#endif
                                    bool moreAvailable;
                                    getCallback(pendingQueue, maximumSends - consumedSends, 
#if UseEnhancedRateControl
                                        int.MaxValue, // maximumBytes - consumedBytes,
#endif
                                        out numberOfObjects, 
#if UseEnhancedRateControl                                        
                                        out numberOfBytes, 
#endif                                        
                                        out moreAvailable);
                                    if (moreAvailable)
                                        callbackQueue.Enqueue(getCallback);

                                    if (numberOfObjects > 0)
                                    {
                                        foreach (Base6_.Asynchronous<Block> element in pendingQueue)
                                        {
                                            if (consumedSends < maximumSends && element.Argument.Size < maximumBytes - consumedBytes)
                                            {
                                                consumedSends++;
                                                consumedBytes += element.Argument.Size;

                                                transmissionQueue.Enqueue(element);
                                            }
                                            else
                                                break;
                                        }

                                        if (pendingQueue.Count > 0)
                                            break;
                                    }
                                }
                                else
                                    break;
                            }
                            else
                            {
                                moreToGo = registered = false;
                                break;
                            }
                        }

                        break;
                    }
                }

                this.ProcessOutgoing();
            }
        }

        #endregion

        #region ISink<Base6.Asynchronous<Block>> Members

        int QS._core_c_.Base6.ISink<Base6_.Asynchronous<Block>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        void QS._core_c_.Base6.ISink<Base6_.Asynchronous<Block>>.Send(
            QS._core_c_.Base6.GetObjectsCallback<Base6_.Asynchronous<Block>> getObjectsCallback)
        {
            bool signal_now = false;
            lock (this)
            {
                callbackQueue.Enqueue(getObjectsCallback);
                if (!registered)
                {
                    registered = true;
                    signal_now = true;
                }
            }

            if (signal_now)
                senderController.Register(this);
        }

        #endregion
    }
}
