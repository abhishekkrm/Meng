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

// #define DEBUG_RootSender
#define DEBUG_RootSender_MembershipChanges
// #define Calculate_Statistics
// #define DEBUG_LogIncomingPacketTimes
#define DEBUG_LogOversizePacketErrorMessages
#define DEBUG_LogAllSendingExceptions

// #define DEBUG_EnableLoggingOfReceivedPackets

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    [QS._core_c_.Diagnostics.ComponentContainer]
    [QS.Fx.Base.Inspectable]
    public class RootSender : SenderClass<ISerializableSender>, Devices_3_.IReceiver, System.IDisposable, 
		Devices_3_.IMembershipController, QS._qss_e_.Base_1_.IStatisticsCollector
    {
        // private int rate = 0;
        public double ReceiveRate
        {
            get 
            {
                double rate = rateCalculator.Rate;
                // logger.Log(this, "__ReceiveRate.Get : Rate = " + rate.ToString());
                // return (double) System.Threading.Interlocked.Increment(ref rate);
                return rate;
            }
        }

        // [Diagnostics.Component]
        private FlowControl3.IRateCalculator rateCalculator;

        public RootSender(QS.Fx.Logging.IEventLogger eventLogger, QS.Fx.Logging.ILogger logger, 
            Devices_3_.ICommunicationsDevice underlyingDevice, int portno, IDemultiplexer demultiplexer, QS.Fx.Clock.IClock clock,
            bool nolistener)
            : this(eventLogger, QS._core_c_.Base3.Incarnation.Current, logger, underlyingDevice, portno, demultiplexer, clock, nolistener)
		{
		}

        public RootSender(QS.Fx.Logging.IEventLogger eventLogger, QS._core_c_.Base3.Incarnation incarnation, QS.Fx.Logging.ILogger logger, 
			Devices_3_.ICommunicationsDevice underlyingDevice, int portno, IDemultiplexer demultiplexer, QS.Fx.Clock.IClock clock, 
            bool nolistener)
        {
            this.nolistener = nolistener;
            this.eventLogger = eventLogger;
            this.logger = logger;
            this.clock = clock;
            this.underlyingDevice = underlyingDevice;
            this.localAddress = new QS.Fx.Network.NetworkAddress(underlyingDevice.Address, portno);
            this.demultiplexer = demultiplexer;
			this.incarnation = incarnation;

            if (!nolistener)
                listener = underlyingDevice.ListenAt(localAddress, this);

			this.instanceID = new QS._core_c_.Base3.InstanceID(localAddress, incarnation);

            rateCalculator = new FlowControl3.RateCalculator3(clock);
        }

        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IClock clock;
        private Devices_3_.ICommunicationsDevice underlyingDevice;
        private QS.Fx.Network.NetworkAddress localAddress;
		private QS._core_c_.Base3.Incarnation incarnation;
		private QS._core_c_.Base3.InstanceID instanceID;
        [QS._core_c_.Diagnostics.Component]
        private Devices_3_.IListener listener;
        private IDemultiplexer demultiplexer;
        private bool nolistener;

#if DEBUG_LogIncomingPacketTimes
        [TMS.Inspection.Inspectable]
        private Base.Logger incomingPacketLog = new Base.Logger(true, null, true, string.Empty);
#endif

		public QS._core_c_.Base3.InstanceID InstanceID
		{
			get { return instanceID; }
		}

        private const int header_overhead = 4 * sizeof(uint) + sizeof(ushort);

#if Calculate_Statistics
        [QS.CMS.Diagnostics.Component("Received Packet Size Samples")]
		Statistics.SamplesXY received_packetSize_samples = new QS.CMS.Statistics.SamplesXY();

		public TMS.Data.IDataSet ReceivedPacketSizes
		{
			get { return received_packetSize_samples.DataSet; }
		}
#endif

		protected override QS._qss_c_.Base3_.ISerializableSender createSender(QS.Fx.Network.NetworkAddress destinationAddress)
		{
            return new Sender(this, incarnation, logger, underlyingDevice.GetSender(destinationAddress), (uint)localAddress.PortNumber);
        }

        public QS.Fx.Network.NetworkAddress Address
        {
            get
            {
                return localAddress;
            }
        }

        #region Devices3.IReceiver Members

        public unsafe void receive(QS.Fx.Network.NetworkAddress sourceAddress, ArraySegment<byte> packet)
        {
            try
            {
                double current_time = clock.Time;

#if DEBUG_RootSender
                logger.Log(this, "__received : packet contains " + packet.Count + " bytes");
#endif

#if Calculate_Statistics
				lock (this)
				{
                    if (received_packetSize_samples.Enabled)
                        received_packetSize_samples.addSample(current_time, packet.Count);
				}
#endif

                QS._core_c_.Base3.InstanceID senderInstanceID;
                uint destinationLOID;
                QS.Fx.Serialization.ISerializable receivedObject;

                Root.Decode(sourceAddress.HostIPAddress, new QS.Fx.Base.Block(packet), 
                    out senderInstanceID, out destinationLOID, out receivedObject);

#if DEBUG_RootSender
				logger.Log(this, "__received : Deserialized into " + 
					Helpers.ToString.ReceivedObject(responseAddress, receivedObject));
#endif

#if DEBUG_RootSender
                logger.Log(this, "__received : demultiplexing");
#endif

#if DEBUG_LogIncomingPacketTimes
                incomingPacketLog.logMessage(null, "(" + senderInstanceID.ToString() + " to " + destinationLOID.ToString() + ") " +
                    Helpers.ToString.Object(receivedObject));
#endif

#if DEBUG_EnableLoggingOfReceivedPackets
                if (eventLogger.Enabled)
                    eventLogger.Log(new Logging.Events.PacketArrival(current_time, this,
                        senderInstanceID, instanceID, new Message(destinationLOID, receivedObject))); 
#endif

                demultiplexer.dispatch(destinationLOID, senderInstanceID, receivedObject);
            }
            catch (Exception exc)
            {
                logger.Log(this, "__Receive : " + exc.ToString());
            }

            rateCalculator.sample();
        }

        #endregion

        #region Class Sender

        [QS.Fx.Base.Inspectable]
        [QS._core_c_.Diagnostics.ComponentContainer]
		public class Sender : QS.Fx.Inspection.Inspectable, ISerializableSender, Senders4.IPassiveMessageSink, QS._qss_e_.Base_1_.IStatisticsCollector
		{
            public Sender(RootSender owner, QS._core_c_.Base3.Incarnation incarnation, QS.Fx.Logging.ILogger logger, Devices_3_.ISender underlyingSender, uint responsePortNo)
            {
                this.owner = owner;
				this.incarnation = incarnation;
                this.logger = logger;
                this.underlyingSender = underlyingSender;
                this.responsePortNo = responsePortNo;
            }

            private RootSender owner;
            private QS.Fx.Logging.ILogger logger;
            private Devices_3_.ISender underlyingSender;
            private uint responsePortNo;
			private QS._core_c_.Base3.Incarnation incarnation;

#if Calculate_Statistics
            [QS.CMS.Diagnostics.Component("Outgoing Packet Size Samples")]
            private CMS.Statistics.SamplesXY outgoing_packetSize_samples = new QS.CMS.Statistics.SamplesXY();
#endif

			#region Base3.IPassiveMessageSink Members

			void Senders4.IPassiveMessageSink.Send(QS._core_c_.Base3.Message message)
			{
				((ISerializableSender)this).send(message.destinationLOID, message.transmittedObject);
			}

			IAsyncResult Senders4.IPassiveMessageSink.BeginSend(
				QS._core_c_.Base3.Message message, AsyncCallback sendCallback, object asynchronousState)
			{
				((ISerializableSender)this).send(message.destinationLOID, message.transmittedObject);
				return new SynchronousResult(asynchronousState);
			}

			void Senders4.IPassiveMessageSink.EndSend(IAsyncResult asynchronousResult)
			{
				SynchronousResult synchronousResult = asynchronousResult as SynchronousResult;
				((IDisposable)synchronousResult).Dispose();
			}

			int Senders4.IPassiveMessageSink.MTU
			{
				get { return ((ISerializableSender)this).MTU; }
			}

			#endregion

            #region ISerializableSender Members

            public QS.Fx.Network.NetworkAddress Address
            {
                get { return underlyingSender.Address; }
            }

            public unsafe void send(uint destinationLOID, QS.Fx.Serialization.ISerializable data)
            {
                try
                {
                    QS.Fx.Serialization.SerializableInfo info = data.SerializableInfo;

#if DEBUG_RootSender
                logger.Log(this, "__send : " + info.Size.ToString() + " bytes");
#endif

                    int outgoing_size = info.Size + header_overhead;
                    int max_outgoing_size = underlyingSender.CommunicationsDevice.MTU;

                    if (outgoing_size > max_outgoing_size)
                    {
                        try
                        {
                            string error_message = QS._core_c_.Helpers.ToString.Strings("When sending " + data.ToString() + " to " + 
                                this.Address.ToString() + ":" + destinationLOID.ToString() + ", size of the assembled packet (",
                                (info.Size + header_overhead).ToString(), 
                                ") is larger than the MTU supported by the underlying communication device (",
                                underlyingSender.CommunicationsDevice.MTU.ToString(), ").");
#if DEBUG_LogOversizePacketErrorMessages
                            logger.Log(this, error_message);
#endif
                            throw new Exception(error_message);
                        }
                        catch (Exception)
                        {
                        }
                    }

#if Calculate_Statistics
                    outgoing_packetSize_samples.addSample(owner.clock.Time, outgoing_size);
#endif

                    QS.Fx.Base.ConsumableBlock header = new QS.Fx.Base.ConsumableBlock((uint)(info.HeaderSize + header_overhead));
                    System.Collections.Generic.IList<QS.Fx.Base.Block> buffers = new List<QS.Fx.Base.Block>(info.NumberOfBuffers + 1);
                    buffers.Add(header.Block);

                    fixed (byte* headerptr = header.Array)
                    {
                        *((uint*)headerptr) = responsePortNo;
                        *((uint*)(headerptr + sizeof(uint))) = destinationLOID;
                        *((ushort*)(headerptr + 2 * sizeof(uint))) = info.ClassID;
                        *((uint*)(headerptr + 2 * sizeof(uint) + sizeof(ushort))) = (uint) info.HeaderSize;
                        *((uint*)(headerptr + 3 * sizeof(uint) + sizeof(ushort))) = incarnation.SeqNo;
                    }
                    header.consume(header_overhead);

#if DEBUG_RootSender
                logger.Log(this, "__send : serializing data");
#endif

                    data.SerializeTo(ref header, ref buffers);

#if DEBUG_RootSender
                logger.Log(this, "__send : sending");
#endif

                    underlyingSender.send(buffers);

#if DEBUG_RootSender
                logger.Log(this, "__send : send completed successfully");
#endif
                }
                catch (Exception exc)
                {
                    string error_message = QS._core_c_.Helpers.ToString.Strings("Could not send to ", this.Address.ToString(), ":",
                        destinationLOID.ToString(), " message ", ((data != null) ? data.ToString() : "(null)"), ".");

#if DEBUG_LogAllSendingExceptions
                    logger.Log(this, QS._core_c_.Helpers.ToString.Strings(error_message, " Caught exception: ", exc.ToString()));
#endif

                    throw new Exception(error_message, exc);
                }
            }

            public int MTU
            {
                get { return underlyingSender.CommunicationsDevice.MTU - header_overhead; }
            }

            #endregion

            #region IComparable Members

            public int CompareTo(object obj)
            {
                QS.Fx.Network.NetworkAddress destinationAddress = underlyingSender.Address;
                QS.Fx.Network.NetworkAddress address = obj as QS.Fx.Network.NetworkAddress;
                if (address != null)
                    return destinationAddress.CompareTo(address);
                else
                {
                    ISerializableSender sender = obj as ISerializableSender;
                    if (sender != null)
                        return destinationAddress.CompareTo(sender.Address);
                    else
                        throw new ArgumentException();
                }
            }

            #endregion

            #region System.Object Overrides

            public override bool Equals(object obj)
            {
                QS.Fx.Network.NetworkAddress destinationAddress = underlyingSender.Address;
                QS.Fx.Network.NetworkAddress address = obj as QS.Fx.Network.NetworkAddress;
                if (address != null)
                    return destinationAddress.Equals(address);
                else
                {
                    ISerializableSender sender = obj as ISerializableSender;
                    if (sender != null)
                        return destinationAddress.Equals(sender.Address);
                    else
                        throw new ArgumentException();
                }
            }

            public override int GetHashCode()
            {
                return underlyingSender.Address.GetHashCode();
            }

            public override string ToString()
            {
                return "RootSender(" + underlyingSender.Address.ToString() + ")";
            }

            #endregion

            #region IStatisticsCollector Members

            IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
            {
                get 
                {
                    List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
#if Calculate_Statistics
                    statistics.Add(new Components.Attribute("Outgoing Packet Sizes", outgoing_packetSize_samples.DataSet)); 
#endif
                    return statistics;
                }
            }

            #endregion
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            logger.Log(this, "__Dispose: Recycling address " + listener.Address.ToString());
            listener.Dispose();
        }

        #endregion

        #region Devices3.IMembershipController Members

        QS._qss_c_.Devices_3_.IListener QS._qss_c_.Devices_3_.IMembershipController.Join(QS.Fx.Network.NetworkAddress multicastAddress)
        {
#if DEBUG_RootSender_MembershipChanges
            logger.Log(this, "Joining " + multicastAddress.ToString());
#endif

            return underlyingDevice.ListenAt(multicastAddress, this);
        }

        #endregion

		#region IStatisticsCollector Members

		IList<QS._core_c_.Components.Attribute> QS._qss_e_.Base_1_.IStatisticsCollector.Statistics
		{
			get 
			{
                List<QS._core_c_.Components.Attribute> statistics = new List<QS._core_c_.Components.Attribute>();
                statistics.Add(new QS._core_c_.Components.Attribute("Senders", new QS._core_c_.Components.AttributeSet(base.Statistics)));
#if Calculate_Statistics
				statistics.Add(new Components.Attribute("Received Packet Sizes", ReceivedPacketSizes));
#endif
                return statistics;
			}
		}

		#endregion

        public override string ToString()
        {
            return "Root";
        }
    }
}
