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

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Incubator_.Applications_
{
    internal sealed class _Application : QS.Fx.Inspection.Inspectable, IApplication
    {
        #region Constructor

        internal _Application(double messagerate)
        {
            this.messagerate = messagerate;
        }

        #endregion

        #region Constants

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        internal string name;
        [QS.Fx.Base.Inspectable]
        internal Client[] clients;
        [QS.Fx.Base.Inspectable]
        internal IApplicationContext context;
        [QS.Fx.Base.Inspectable]
        internal QS.Fx.Clock.IAlarm alarm;
        [QS.Fx.Base.Inspectable]
        internal uint messagecount;
        [QS.Fx.Base.Inspectable]
        internal double[] messagetimes;
        [QS.Fx.Base.Inspectable]
        internal double messagerate;

        internal System.Random random = new System.Random();

        #endregion

        #region _MessageTimes

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _MessageTimes
        {
            get
            {
                return new QS._core_e_.Data.Data1D("Message Times", messagetimes, "message sending times",
                    "message no", "", "message number", "time", "s", "time the message was sent");
            }
        }

        #endregion

        #region _MessageLatencies_Minimum

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _MessageLatencies_Minimum
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].messagecount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = double.MaxValue;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value = Math.Min(_value, clients[ind].messagelatencies[_ind]);
                    _latencies[_ind] = _value;
                }

                return new QS._core_e_.Data.Data1D("Message Latencies (Minimum)", _latencies, "minimum message latencies",
                    "message no", "", "message number", "latency", "s", "minimum message latency");
            }
        }

        #endregion

        #region _MessageLatencies_Maximum

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _MessageLatencies_Maximum
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].messagecount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = 0;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value = Math.Max(_value, clients[ind].messagelatencies[_ind]);
                    _latencies[_ind] = _value;
                }

                return new QS._core_e_.Data.Data1D("Message Latencies (Maximum)", _latencies, "maximum message latencies",
                    "message no", "", "message number", "latency", "s", "maximum message latency");
            }
        }

        #endregion

        #region _MessageLatencies_Average

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _MessageLatencies_Average
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].messagecount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = 0;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value += clients[ind].messagelatencies[_ind];
                    _latencies[_ind] = _value / ((double)clients.Length);
                }

                return new QS._core_e_.Data.Data1D("Message Latencies (Average)", _latencies, "average message latencies",
                    "message no", "", "message number", "latency", "s", "average message latency");
            }
        }

        #endregion

        #region _ReportMessageProblems

        [QS.Fx.Base.Inspectable]
        private string _ReportMessageProblems
        {
            get
            {
                StringBuilder _message = new StringBuilder();
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].messagecount);
                bool isbad = false;
                for (int _ind = 0; !isbad && _ind < _count; _ind++)
                {
                    for (int ind = 0; !isbad && ind < clients.Length; ind++)
                    {
                        if (clients[ind].messagelatencies[_ind] == 0)
                        {
                            _message.AppendLine("Client " + clients[ind].name + " has no timestamp for message " + (_ind + 1).ToString() + ".");
                            isbad = true;
                        }
                    }
                }
                return _message.ToString();
            }
        }

        #endregion

        #region IApplication Members

        void IApplication.Initialize(string name, int count, IApplicationContext context)
        {
            this.context = context;
            this.name = name;

            clients = new Client[count];
            for (int ind = 0; ind < clients.Length; ind++)
                clients[ind] = new Client(this, ind);

            alarm = context.AlarmClock.Schedule(((double)1) / messagerate, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
        }

        IClient[] IApplication.Clients
        {
            get { return clients; }
        }

        QS._qss_x_.Agents_.Base.IProtocol IApplication.Protocol
        {
            get { return null; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (alarm != null)
            {
                if (!alarm.Cancelled)
                    alarm.Cancel();
            }
        }

        #endregion

        #region _AlarmCallback

        private void _AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            lock (this)
            {
                uint messageno = ++messagecount;
                foreach (Client client in clients)
                    client.NewMessage(messageno);

                if (messagetimes == null)
                    messagetimes = new double[messagecount];
                else if (messagecount > messagetimes.Length)
                {
                    double[] _messagetimes = new double[Math.Max(2 * messagetimes.Length, messagecount)];
                    Array.Copy(messagetimes, _messagetimes, messagetimes.Length);
                    messagetimes = _messagetimes;
                }
                messagetimes[messageno - 1] = context.Clock.Time;
            }

            alarm.Reschedule();
        }

        #endregion

        #region Class Client

        internal sealed class Client : QS.Fx.Inspection.Inspectable, IClient, QS._qss_x_.Agents_.Base.IClient, QS._qss_x_.Protocols_._Protocol.IEndpoint
        {
            #region Constructor

            internal Client(_Application application, int index)
            {
                this.application = application;
                this.index = index;
            }

            #endregion

            #region IDisposable Members

            void IDisposable.Dispose()
            {
            }

            #endregion

            #region Fields

            [QS.Fx.Base.Inspectable]
            internal _Application application;
            [QS.Fx.Base.Inspectable]
            internal string name;
            [QS.Fx.Base.Inspectable]
            internal IClientContext context;
            [QS.Fx.Base.Inspectable]
            internal int index;
            [QS.Fx.Base.Inspectable]
            internal bool running;
            [QS.Fx.Base.Inspectable]
            internal uint messagecount;
            [QS.Fx.Base.Inspectable]
            internal double[] messagelatencies;
            [QS.Fx.Base.Inspectable]
            internal QS._qss_x_.Agents_.Base.IConnection connection;

            /*
                        private event QS.Fx.Protocols.Simple2PC.CommittableCallback onCommittable;
                        private event QS.Fx.Protocols.Simple2PC.AbortableCallback onAbortable;
            */

            #endregion

            #region _MessageLatencies

            [QS.Fx.Base.Inspectable]
            private QS._core_e_.Data.Data1D _MessageLatencies
            {
                get
                {
                    return new QS._core_e_.Data.Data1D("Message Latencies", messagelatencies, "message latencies",
                        "message no", "", "message number", "latency", "s", "the time the message waited to be cleaned up");
                }
            }

            #endregion

            #region IClient.Initialize

            void IClient.Initialize(string name, IClientContext context)
            {
                this.name = name;
                this.context = context;
            }

            #endregion

            #region IClient.Start

            void IClient.Start(QS._qss_x_.Agents_.Base.IConnection connection)
            {
                context.Logger.Log("[" + application.name + "] : Start");
                running = true;
                this.connection = connection;
            }

            #endregion

            #region IClient.Stop

            void IClient.Stop()
            {
                context.Logger.Log("[" + application.name + "] : Stop");
                running = false;
                // connection.Dispose();
            }

            #endregion

            #region NewMessage

            internal void NewMessage(uint messageno)
            {
                lock (this)
                {
                    messagecount++;
                    System.Diagnostics.Debug.Assert(messageno == messagecount);

                    if (messagelatencies == null)
                        messagelatencies = new double[messagecount];
                    else if (messagecount > messagelatencies.Length)
                    {
                        double[] _messagelatencies = new double[Math.Max(2 * messagelatencies.Length, messagecount)];
                        Array.Copy(messagelatencies, _messagelatencies, messagelatencies.Length);
                        messagelatencies = _messagelatencies;
                    }

                    if (running)
                    {
                        /*
                                                if (acceptcommit)
                                                {
                                                    if (onCommittable != null)
                                                        onCommittable(transactionno);
                                                }
                                                else
                                                {
                                                    if (onAbortable != null)
                                                        onAbortable(transactionno);
                                                }
                        */
                    }
                }
            }

            #endregion

            /*
            #region IEndpoint.OnCommittable

            event QS.Fx.Protocols.Simple2PC.CommittableCallback QS.Fx.Protocols.Simple2PC.IEndpoint.OnCommittable
            {
                add
                {
                    lock (this)
                    {
                        onCommittable += value;
#if !DEBUG_DoNotInitializeWithOldTransactions
                        for (int ind = 0; ind < transactioncount; ind++)
                            if (transactions[ind])
                                onCommittable((uint)(ind + 1));
#endif
                    }
                }

                remove
                {
                    lock (this)
                    {
                        onCommittable -= value;
                    }
                }
            }

            #endregion

            #region IEndpoint.OnAbortable

            event QS.Fx.Protocols.Simple2PC.AbortableCallback QS.Fx.Protocols.Simple2PC.IEndpoint.OnAbortable
            {
                add
                {
                    lock (this)
                    {
                        onAbortable += value;
#if !DEBUG_DoNotInitializeWithOldTransactions
                        for (int ind = 0; ind < transactioncount; ind++)
                            if (!transactions[ind])
                                onAbortable((uint)(ind + 1));
#endif
                    }
                }

                remove
                {
                    lock (this)
                    {
                        onAbortable -= value;
                    }
                }
            }

            #endregion
*/

            /*
                        #region IEndpoint.Commit

                        void QS.Fx.Protocols.Cleanup.IEndpoint.Commit(uint m)
                        {
                            lock (this)
                            {
                                int index = (int)(m - 1);

                                System.Diagnostics.Debug.Assert(index < transactioncount);
            #if DEBUG_CheckDecisionCorrectness
                                System.Diagnostics.Debug.Assert(transactions[index], 
                                    this.name + " is committing transaction" + m.ToString() + " even though it locally wanted to reject it");
            #endif
                                System.Diagnostics.Debug.Assert(index < application.transactioncount);
            #if DEBUG_CheckDecisionCorrectness
                                System.Diagnostics.Debug.Assert(application.transactions[index],
                                    this.name + " is committing transaction" + m.ToString() + " even though it should have been globally rejected");
            #endif

                                transactionlatencies[index] = application.context.Clock.Time - application.transactiontimes[index];

            #if DEBUG_LogTransactionOutcomes
                                context.Logger.Log("[" + application.name + "] : COMMIT (" + m.ToString() + " )");
            #endif
                            }
                        }

                        #endregion

                        #region IEndpoint.Abort

                        void QS.Fx.Protocols.Simple2PC.IEndpoint.Abort(uint m)
                        {
                            lock (this)
                            {
                                int index = (int)(m - 1);

                                System.Diagnostics.Debug.Assert(index < transactioncount);
                                System.Diagnostics.Debug.Assert(index < application.transactioncount);
            #if DEBUG_CheckDecisionCorrectness
                                System.Diagnostics.Debug.Assert(!application.transactions[index],
                                    this.name + " is aborting transaction" + m.ToString() + " even though it should have been globally accepted");
            #endif

                                transactionlatencies[index] = application.context.Clock.Time - application.transactiontimes[index];

            #if DEBUG_LogTransactionOutcomes
                                context.Logger.Log("[" + application.name + "] : ABORT (" + m.ToString() + " )");
            #endif
                            }
                        }

                        #endregion
            */

            #region IClient Members

            object QS._qss_x_.Agents_.Base.IClient.Endpoint
            {
                get { return this; }
            }

            #endregion
        }

        #endregion
    }
}
