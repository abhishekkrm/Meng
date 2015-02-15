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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

using System.Net;

namespace QS._qss_c_.Devices_3_
{
    public class CompatibilityNetworkWrapper : Devices_3_.INetwork
    {
        public CompatibilityNetworkWrapper(IPAddress[] localAddresses, Devices_2_.ICommunicationsDevice underlyingDevice)
        {
            networkInterfaces = new INetworkInterface[localAddresses.Length];
            for (int ind = 0; ind < localAddresses.Length; ind++)
                networkInterfaces[ind] = new NetworkInterfaceWrapper(localAddresses[ind], underlyingDevice);
        }

        private INetworkInterface[] networkInterfaces;

        private class NetworkInterfaceWrapper : Devices_3_.INetworkInterface, Devices_3_.ICommunicationsDevice
        {
            public NetworkInterfaceWrapper(IPAddress address, Devices_2_.ICommunicationsDevice underlyingDevice)
            {
                this.address = address;
                this.underlyingDevice = underlyingDevice;
            }

            private IPAddress address;
            private Devices_2_.ICommunicationsDevice underlyingDevice;

            #region INetworkInterface Members

            IPAddress INetworkInterface.Address
            {
                get { return address; }
            }

            ICommunicationsDevice INetworkInterface.this[CommunicationsDevice.Class deviceClass]
            {
                get { return this; }
            }

            #endregion

            #region ICommunicationsDevice Members

            IPAddress ICommunicationsDevice.Address
            {
                get { return address; }
            }

            INetworkInterface ICommunicationsDevice.NetworkInterface
            {
                get { return this; }
            }

            ISender ICommunicationsDevice.GetSender(QS.Fx.Network.NetworkAddress destinationAddress)
            {
                return new SenderWrapper(destinationAddress, this);
            }

            #region SenderWrapper

            private class SenderWrapper : ISender
            {
                public SenderWrapper(QS.Fx.Network.NetworkAddress destinationAddress, NetworkInterfaceWrapper wrapper)
                {
                    this.destinationAddress = destinationAddress;
                    this.wrapper = wrapper;
                }

                private QS.Fx.Network.NetworkAddress destinationAddress;
                private NetworkInterfaceWrapper wrapper;

                #region ISender Members

                ICommunicationsDevice ISender.CommunicationsDevice
                {
                    get { return wrapper; }
                }

                QS.Fx.Network.NetworkAddress ISender.Address
                {
                    get { return destinationAddress; }
                }

                void ISender.send(IList<QS.Fx.Base.Block> buffers)
                {
                    int ind, size;
                    
                    size = 0;
                    for (ind = 0; ind < buffers.Count; ind++)
                        size += (int) buffers[ind].size;
                    byte[] bytes = new byte[size];
                    
                    size = 0;
                    for (ind = 0; ind < buffers.Count; ind++)
                    {
                        if ((buffers[ind].type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && buffers[ind].buffer != null)
                            Buffer.BlockCopy(buffers[ind].buffer, (int) buffers[ind].offset, bytes, size, (int) buffers[ind].size);
                        else
                            throw new Exception("Unmanaged memory not supported here.");

                        size += (int) buffers[ind].size;
                    }

                    wrapper.underlyingDevice.sendto(new QS.Fx.Network.NetworkAddress(wrapper.address, 0), destinationAddress,
                        new QS._core_c_.Base2.BlockOfData(bytes));
                }

                IAsyncResult ISender.BeginSend(IList<QS.Fx.Base.Block> buffers, AsyncCallback callback, object state)
                {
                    int ind, size;

                    size = 0;
                    for (ind = 0; ind < buffers.Count; ind++)
                        size += (int) buffers[ind].size;
                    byte[] bytes = new byte[size];

                    size = 0;
                    for (ind = 0; ind < buffers.Count; ind++)
                    {
                        if ((buffers[ind].type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && buffers[ind].buffer != null)
                            Buffer.BlockCopy(buffers[ind].buffer, (int)buffers[ind].offset, bytes, size, (int)buffers[ind].size);
                        else
                            throw new Exception("Unmanaged memory not supported here.");

                        size += (int) buffers[ind].size;
                    }

                    return wrapper.underlyingDevice.BeginSendTo(new QS.Fx.Network.NetworkAddress(wrapper.address, 0), destinationAddress,
                        new QS._core_c_.Base2.BlockOfData(bytes), callback, state);
                }

                #endregion
            }

            #endregion

            IListener ICommunicationsDevice.ListenAt(QS.Fx.Network.NetworkAddress receivingAddress, IReceiver asynchronousReceiver)
            {
                ReceiverWrapper receiverWrapper = new ReceiverWrapper(asynchronousReceiver);
                return new ListenerWrapper(receiverWrapper, underlyingDevice.listenAt(address, receivingAddress,
                    new Devices_2_.OnReceiveCallback(receiverWrapper.receiveCallback)));
            }

            #region ReceiverWrapper

            private class ReceiverWrapper
            {
                public ReceiverWrapper(IReceiver asynchronousReceiver)
                {
                    this.asynchronousReceiver = asynchronousReceiver;
                }

                private IReceiver asynchronousReceiver;

                public void receiveCallback(QS.Fx.Network.NetworkAddress sourceAddress, QS.Fx.Network.NetworkAddress destinationAddress,
                    QS._core_c_.Base2.IBlockOfData blockOfData)
                {
                    asynchronousReceiver.receive(sourceAddress,
                        new ArraySegment<byte>(blockOfData.Buffer, (int)blockOfData.OffsetWithinBuffer, (int)blockOfData.SizeOfData));
                }
            }

            #endregion

            #region ListenerWrapper

            private class ListenerWrapper : IListener
            {
                public ListenerWrapper(ReceiverWrapper receiverWrapper, Devices_2_.IListener listener)
                {
                    this.receiverWrapper = receiverWrapper;
                    this.listener = listener;
                }

                private ReceiverWrapper receiverWrapper;
                private Devices_2_.IListener listener;

                #region IDisposable Members

                void IDisposable.Dispose()
                {
                    listener.shutdown();
                }

                #endregion

                #region IListener Members

                QS.Fx.Network.NetworkAddress QS._qss_c_.Devices_2_.IListener.Address
                {
                    get { return listener.Address; }
                }

                void QS._qss_c_.Devices_2_.IListener.shutdown()
                {
                    listener.shutdown();
                }

                #endregion
            }

            #endregion

            int ICommunicationsDevice.MTU
            {
                get { return (int) underlyingDevice.MTU; }
                set { throw new Exception("Operation not permitted in this context."); }
            }

            CommunicationsDevice.Class ICommunicationsDevice.Class
            {
                get { return CommunicationsDevice.Class.UDP; }
            }

            #endregion
        }

        #region INetwork Members

        INetworkInterface[] INetwork.NICs
        {
            get { return networkInterfaces; }
        }

        public INetworkInterface this[IPAddress address]
        {
            get
            {
                foreach (INetworkInterface netinf in networkInterfaces)
                {
                    if (netinf.Address.Equals(address))
                        return netinf;
                }

                throw new Exception("No such address.");
            }
        }

        #endregion
    }
}
