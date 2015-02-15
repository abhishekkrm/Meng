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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Network_
{
    public sealed class VirtualSender : QS._qss_x_.Base1_.Parametrized, QS.Fx.Network.ISender, IVirtualSender, QS._core_c_.FlowControl3.IRateControlled
    {
        public VirtualSender(System.Net.IPAddress interfaceAddress, int portno, QS.Fx.Network.NetworkAddress address, 
            IVirtualSenderController senderController, QS._core_c_.RateControl.IRateController rateController,
            QS.Fx.Logging.ILogger logger)
        {
            this.interfaceAddress = interfaceAddress;
            this.portno = portno;
            this.address = address;
            this.senderController = senderController;
            this.rateController = rateController;
            this.logger = logger;
        }

        private IVirtualSenderController senderController;
        private QS.Fx.Logging.ILogger logger;

        [QS.Fx.Base.Inspectable]
        private System.Net.IPAddress interfaceAddress;
        [QS.Fx.Base.Inspectable]
        private int portno;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Network.NetworkAddress address;
        [QS.Fx.Base.Inspectable]
        private QS._core_c_.RateControl.IRateController rateController;
        [QS.Fx.Base.Parameter(QS._core_c_.Core.SenderInfo.Parameters.MTU)]
        [QS.Fx.Base.Inspectable]
        private int mtu = 60000;
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Network.ISource> sources = new Queue<QS.Fx.Network.ISource>();
        [QS.Fx.Base.Inspectable]
        private Queue<QS.Fx.Network.AsynchronousSend> requests = new Queue<QS.Fx.Network.AsynchronousSend>();
        [QS.Fx.Base.Inspectable]
        private bool dataWaiting, registeredToSend;

        #region QS.CMS.FlowControl3.IRateControlled Members

        [QS.Fx.Base.Parameter(QS._core_c_.Core.SenderInfo.Parameters.MaximumRate)]
        double  QS._core_c_.FlowControl3.IRateControlled.MaximumRate
        {
            get { return rateController.MaximumRate; }
	        set { rateController.MaximumRate = value; }
        }

        #endregion

        #region _SignalSend

        private void _SignalSend()
        {
            if (!dataWaiting)
            {
                dataWaiting = true;
                if (!registeredToSend && rateController.Ready)
                {
                    registeredToSend = true;
                    senderController.ScheduleSender(this);
                }
            }
        }

        #endregion

        #region ISender Members

        void QS.Fx.Network.ISender.Send(QS.Fx.Network.Data data)
        {
            requests.Enqueue(new QS.Fx.Network.AsynchronousSend(data, null, null));
            _SignalSend();
        }

        void QS.Fx.Network.ISender.Send(QS.Fx.Network.Data data, QS.Fx.Base.ContextCallback completionCallback, object context)
        {
            // sanity check
            // data._CheckSize();

            requests.Enqueue(new QS.Fx.Network.AsynchronousSend(data, completionCallback, context));
            _SignalSend();
        }

        void QS.Fx.Network.ISender.Send(QS.Fx.Network.ISource source)
        {
            sources.Enqueue(source);
            _SignalSend();
        }

        System.Net.IPAddress QS.Fx.Network.ISender.InterfaceAddress
        {
            get { return interfaceAddress; }
        }

        QS.Fx.Network.NetworkAddress QS.Fx.Network.ISender.Address
        {
            get { return address; }
        }

        #endregion

        #region IVirtualSender Members

        QS.Fx.Network.NetworkAddress IVirtualSender.SourceAddress
        {
            get { return new QS.Fx.Network.NetworkAddress(interfaceAddress, portno); }
        }

        QS.Fx.Network.NetworkAddress IVirtualSender.DestinationAddress
        {
            get { return address; }
        }

        void IVirtualSender.TransmissionCallback(
            Queue<QS.Fx.Network.AsynchronousSend> tosend, int maximumNumberOfPackets, int maximumNumberOfBytes,
            out int numberOfPacketsConsumed, out int numberOfBytesConsumed, out bool moreAvailable)
        {
            numberOfPacketsConsumed = 0;
            numberOfBytesConsumed = 0;
            moreAvailable = true;

            while (numberOfPacketsConsumed < maximumNumberOfPackets && numberOfBytesConsumed < maximumNumberOfBytes)
            {
                int maximumBytes = maximumNumberOfBytes - numberOfBytesConsumed;
                if (mtu < maximumBytes)
                    maximumBytes = mtu;

                dataWaiting = requests.Count > 0 || sources.Count > 0;

                if (rateController.Ready && dataWaiting)
                {
                    if (requests.Count > 0)
                    {
                        if (requests.Peek().Data.Size <= maximumBytes)
                        {
                            QS.Fx.Network.AsynchronousSend request = requests.Dequeue();

                            numberOfPacketsConsumed++;
                            numberOfBytesConsumed += request.Data.Size;

                            tosend.Enqueue(request);
                            rateController.Consume();
                        }
                        else
                            break;
                    }
                    else
                    {
                        QS.Fx.Network.ISource source = sources.Dequeue();

                        QS.Fx.Network.Data data;
                        QS.Fx.Base.ContextCallback callback;
                        object context;
                        bool more;
                        bool returned = source.Get(maximumBytes, out data, out callback, out context, out more);

                        if (more)
                        {
                            sources.Enqueue(source);
                            if (!returned)
                                break;
                        }

                        if (returned)
                        {
                            // sanity check
                            // data._CheckSize();

                            numberOfPacketsConsumed++;
                            numberOfBytesConsumed += data.Size;

                            tosend.Enqueue(new QS.Fx.Network.AsynchronousSend(data, callback, context));
                            rateController.Consume();
                        }
                    }
                }
                else
                {
                    moreAvailable = false;
                    registeredToSend = false;
                    break;
                }
            }
        }

        #endregion

        #region Reset

        public void Reset()
        {
            lock (this)
            {
#if DEBUG_LogGenerously
                logger.Log("Resetting sender " + address.ToString() + " at interface " + interfaceAddress.ToString() + ".");
#endif

                dataWaiting = false;
                registeredToSend = false;
                sources.Clear();
                requests.Clear();

                senderController = null;
            }
        }

        #endregion
    }
}
