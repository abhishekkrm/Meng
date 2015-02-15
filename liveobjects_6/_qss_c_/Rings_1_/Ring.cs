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

#define DEBUG_Ring

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Rings_1_
{
    public class Ring : IRing
    {
        public Ring(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IClock clock, QS.Fx.Clock.IAlarmClock alarmClock, QS._core_c_.Base3.InstanceID localAddress,
            Base3_.IAddressCollection addressCollection, double tokenCirculationRate)
        {
            this.logger = logger;
            this.clock = clock;
            this.alarmClock = alarmClock;
            this.localAddress = localAddress;
            this.addressCollection = addressCollection;
            this.tokenCirculationRate = tokenCirculationRate;

            lock (this)
            {
                memberAddresses = new List<QS._core_c_.Base3.InstanceID>(addressCollection.RegisterCallback(this.CrashCallback));
                UpdateAddresses();
            }

#if DEBUG_Ring
            StringBuilder s = new StringBuilder();
            foreach (QS._core_c_.Base3.InstanceID address in memberAddresses)
                s.AppendLine(address.ToString());
            logger.Log(this, "__Constructor\n" + s.ToString());
#endif
        }

        private const double RandomnessCoefficient = 0.1;

        private System.Random random = new System.Random();
        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarm recheckingAlarm;
        private double tokenCirculationRate, interval;
        private QS.Fx.Clock.IClock clock;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS._core_c_.Base3.InstanceID localAddress;
        private Base3_.IAddressCollection addressCollection;
        private List<QS._core_c_.Base3.InstanceID> memberAddresses;
        private QS._core_c_.Base3.InstanceID leaderAddress, predecessorAddress, successorAddress;
        private ICollection<Connection> connections = new System.Collections.ObjectModel.Collection<Connection>();

        #region Internal Processing

        private void RecheckingCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                bool somebodyActive = false;
                List<QS._core_c_.Base3.Message> messages = new List<QS._core_c_.Base3.Message>();
                foreach (Connection connection in connections)
                {
                    if (connection.Client.Active)
                    {
                        somebodyActive = true;

                        QS.Fx.Serialization.ISerializable forwardGoing;
                        connection.Client.Process(out forwardGoing);
                        if (forwardGoing != null)
                            messages.Add(new QS._core_c_.Base3.Message(connection.Client.ID, forwardGoing));
                    }
                }

                if (somebodyActive)
                {


                    // .....................





                }

                if (recheckingAlarm != null)
                    alarmRef.Reschedule(this.Interval);
            }
        }

        // TODO: We should add some way of stopping the callbacks if ring is inactive, so we're not interrupted for no reason.....

        private double Interval
        {
            get { return interval * (1 + RandomnessCoefficient * (2 * random.NextDouble() - 1)); }
        }

        #endregion

        #region Managing Membership

        private void UpdateAddresses()
        {
            leaderAddress = predecessorAddress = successorAddress = localAddress;
            foreach (QS._core_c_.Base3.InstanceID address in memberAddresses)
            {
                if (((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(leaderAddress) < 0)
                    leaderAddress = address;

                if (((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(localAddress) > 0 &&
                    ((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(successorAddress) < 0)
                    successorAddress = address;

                if (((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(localAddress) < 0 &&
                    ((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(predecessorAddress) > 0)
                    predecessorAddress = address;
            }
            if (successorAddress.Equals(localAddress))
                successorAddress = leaderAddress;
            if (predecessorAddress.Equals(localAddress))
            {
                foreach (QS._core_c_.Base3.InstanceID address in memberAddresses)
                    if (((IComparable<QS._core_c_.Base3.InstanceID>)address).CompareTo(predecessorAddress) > 0)
                        predecessorAddress = address;
            }

#if DEBUG_Ring           
            logger.Log(this, "__UpdateAddresses : Leader = " + leaderAddress.ToString() + ", Predecessor = " +
                predecessorAddress.ToString() + ", Successor = " + successorAddress.ToString());
#endif

            interval = 1 / (tokenCirculationRate * memberAddresses.Count);
        }

        private void CrashCallback(IEnumerable<QS._core_c_.Base3.InstanceID> crashedAddresses)
        {
            lock (this)
            {
                foreach (QS._core_c_.Base3.InstanceID address in crashedAddresses)
                    memberAddresses.Remove(address);

                UpdateAddresses();
            }
        }

        #endregion

        #region Managing Client Connections

        private void Connect(Connection connection)
        {
            lock (this)
            {
#if DEBUG_Ring
                logger.Log(this, "__Connect : " + connection.ToString());
#endif

                if (recheckingAlarm == null)
                {
                    recheckingAlarm = alarmClock.Schedule(this.Interval, 
                        new QS.Fx.Clock.AlarmCallback(this.RecheckingCallback), null);
                }

                connections.Add(connection);
            }
        }

        private void Disconnect(Connection connection)
        {
            lock (this)
            {
#if DEBUG_Ring
                logger.Log(this, "__Disconnect : " + connection.ToString());
#endif

                connections.Remove(connection);

                if (connections.Count == 0 && recheckingAlarm != null)
                {
                    recheckingAlarm.Cancel();
                    recheckingAlarm = null;
                }
            }
        }

        #endregion

        #region Class Connection

        private class Connection : IRingConnection
        {
            public Connection(Ring owner, IRingClient client)
            {
                this.owner = owner;
                this.client = client;

                owner.Connect(this);
            }

            private Ring owner;
            private IRingClient client;

            public IRingClient Client
            {
                get { return client; }
            }

            #region IDisposable Members

            void IDisposable.Dispose()
            {
                owner.Disconnect(this);
            }

            #endregion

            public override string ToString()
            {
                return client.ToString();
            }
        }

        #endregion

        #region IRing Members

        IRingConnection IRing.Register(IRingClient client)
        {
            return new Connection(this, client);
        }

        #endregion
    }
}
