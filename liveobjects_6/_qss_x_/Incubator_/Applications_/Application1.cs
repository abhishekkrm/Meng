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

// #define DEBUG_LogTransactionOutcomes
// #define DEBUG_CheckDecisionCorrectness
#define DEBUG_DoNotRememberLocalDecisions
#define DEBUG_DoNotInitializeWithOldTransactions
// #define DEBUG_LogTransactionTimes

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Incubator_.Applications_
{
    public sealed class Application1 : QS.Fx.Inspection.Inspectable, IApplication
    {
        #region Constructor

        public Application1() : this(DefaultTransactionsPerSecond, DefaultProbabilityOfCommitting)
        {
        }

        public Application1(double tps, double globalpcommit)
        {
            this.tps = tps;
            this.globalpcommit = globalpcommit;
        }

        #endregion

        #region Constants

        private const double DefaultTransactionsPerSecond = 1000;
        private const double DefaultProbabilityOfCommitting = 0.95;

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
        internal uint transactioncount;
        [QS.Fx.Base.Inspectable]
        internal double pcommit, globalpcommit;
#if DEBUG_CheckDecisionCorrectness
        [QS.TMS.Inspection.Inspectable]
        internal bool[] transactions;
#endif
#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        internal double[] transactiontimes;
#endif
        [QS.Fx.Base.Inspectable]
        internal double tps;

        internal System.Random random = new System.Random();

        #endregion

        #region _TransactionTimes

#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        private QS.TMS.Data.Data1D _TransactionTimes
        {
            get
            {
                return new QS.TMS.Data.Data1D("Transaction Times", transactiontimes, "transaction starting times", 
                    "transaction no", "", "transaction number", "time", "s", "time the transaction has started");
            }
        }
#endif

        #endregion

        #region _TransactionLatencies_Minimum

#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        private QS.TMS.Data.Data1D _TransactionLatencies_Minimum
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].transactioncount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = double.MaxValue;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value = Math.Min(_value, clients[ind].transactionlatencies[_ind]);
                    _latencies[_ind] = _value;
                }

                return new QS.TMS.Data.Data1D("Transaction Latencies (Minimum)", _latencies, "minimum transaction latencies",
                    "transaction no", "", "transaction number", "latency", "s", "minimum transaction latency");
            }
        }
#endif

        #endregion

        #region _TransactionLatencies_Maximum

#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        private QS.TMS.Data.Data1D _TransactionLatencies_Maximum
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].transactioncount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = 0;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value = Math.Max(_value, clients[ind].transactionlatencies[_ind]);
                    _latencies[_ind] = _value;
                }

                return new QS.TMS.Data.Data1D("Transaction Latencies (Maximum)", _latencies, "maximum transaction latencies",
                    "transaction no", "", "transaction number", "latency", "s", "maximum transaction latency");
            }
        }
#endif

        #endregion

        #region _TransactionLatencies_Average

#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        private QS.TMS.Data.Data1D _TransactionLatencies_Average
        {
            get
            {
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].transactioncount);
                double[] _latencies = new double[_count];
                for (int _ind = 0; _ind < _count; _ind++)
                {
                    double _value = 0;
                    for (int ind = 0; ind < clients.Length; ind++)
                        _value += clients[ind].transactionlatencies[_ind];
                    _latencies[_ind] = _value / ((double) clients.Length);
                }

                return new QS.TMS.Data.Data1D("Transaction Latencies (Average)", _latencies, "average transaction latencies",
                    "transaction no", "", "transaction number", "latency", "s", "average transaction latency");
            }
        }
#endif

        #endregion

        #region _ReportTransactionProblems

#if DEBUG_LogTransactionTimes
        [QS.TMS.Inspection.Inspectable]
        private string _ReportTransactionProblems
        {
            get
            {
                StringBuilder _message = new StringBuilder();
                uint _count = uint.MaxValue;
                for (int ind = 0; ind < clients.Length; ind++)
                    _count = Math.Min(_count, clients[ind].transactioncount);
                bool isbad = false;
                for (int _ind = 0; !isbad && _ind < _count; _ind++)
                {
                    for (int ind = 0; !isbad && ind < clients.Length; ind++)
                    {
                        if (clients[ind].transactionlatencies[_ind] == 0)
                        {
                            _message.AppendLine("Client " + clients[ind].name + " has no timestamp for transaction " + (_ind + 1).ToString() + ".");
                            isbad = true;
                        }
                    }
                }
                return _message.ToString();
            }
        }
#endif

        #endregion

        #region IApplication Members

        void IApplication.Initialize(string name, int count, IApplicationContext context)
        {
            this.context = context;
            this.name = name;
            
            clients = new Client[count];
            for (int ind = 0; ind < clients.Length; ind++)
                clients[ind] = new Client(this, ind);

            pcommit = Math.Pow(globalpcommit, 1.0 / ((double) count));

            alarm = context.AlarmClock.Schedule(((double) 1) / tps, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), null);
        }

        IClient[] IApplication.Clients
        {
            get { return clients; }
        }

        QS._qss_x_.Agents_.Base.IProtocol IApplication.Protocol
        {
            get { return QS._qss_x_.Protocols_.Simple2PC.Protocol; }
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
                uint transactionno = ++transactioncount;
                
                bool commit = true;
                foreach (Client client in clients)
                {
                    bool localcommit = (random.NextDouble() < pcommit);
                    commit &= localcommit;

#if DEBUG_LogTransactionOutcomes
                    if (!localcommit)
                        context.Logger.Log("[ " + name + "." + client.name + "] : local reject ( " + transactionno.ToString() + ")");
#endif

                    client.NewTransaction(transactionno, localcommit);
                }

#if DEBUG_CheckDecisionCorrectness
                if (transactions == null)
                    transactions = new bool[transactioncount];
                else if (transactioncount > transactions.Length)
                {
                    bool[] _transactions = new bool[Math.Max(2 * transactions.Length, transactioncount)];
                    Array.Copy(transactions, _transactions, transactions.Length);
                    transactions = _transactions;
                }
                transactions[transactionno - 1] = commit;
#endif

#if DEBUG_LogTransactionTimes
                if (transactiontimes == null)
                    transactiontimes = new double[transactioncount];
                else if (transactioncount > transactiontimes.Length)
                {
                    double[] _transactiontimes = new double[Math.Max(2 * transactiontimes.Length, transactioncount)];
                    Array.Copy(transactiontimes, _transactiontimes, transactiontimes.Length);
                    transactiontimes = _transactiontimes;
                }
                transactiontimes[transactionno - 1] = context.Clock.Time;
#endif

#if DEBUG_LogTransactionOutcomes
                context.Logger.Log("[ " + name + "] : TRANSACTION (" +
                        transactionno.ToString() + ", " + transactiontimes[transactionno - 1].ToString() + ", " + (commit ? "ACCEPT" : "REJECT") + ")");
#endif
            }

            alarm.Reschedule();
        }

        #endregion

        #region Class Client

        internal sealed class Client 
            : QS.Fx.Inspection.Inspectable, IClient, QS._qss_x_.Agents_.Base.IClient, QS._qss_x_.Protocols_.Simple2PC.IEndpoint
        {
            #region Constructor

            internal Client(Application1 application, int index)
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
            internal Application1 application;
            [QS.Fx.Base.Inspectable]
            internal string name;
            [QS.Fx.Base.Inspectable]
            internal IClientContext context;
            [QS.Fx.Base.Inspectable]
            internal int index;
            [QS.Fx.Base.Inspectable]
            internal bool running;
            [QS.Fx.Base.Inspectable]
            internal uint transactioncount;
#if !DEBUG_DoNotRememberLocalDecisions
            [QS.TMS.Inspection.Inspectable]
            internal bool[] transactions;
#endif
#if DEBUG_LogTransactionTimes
            [QS.TMS.Inspection.Inspectable]
            internal double[] transactionlatencies;
#endif
            [QS.Fx.Base.Inspectable]
            internal QS._qss_x_.Agents_.Base.IConnection connection;

            private event QS._qss_x_.Protocols_.Simple2PC.CommittableCallback onCommittable;
            private event QS._qss_x_.Protocols_.Simple2PC.AbortableCallback onAbortable;

            #endregion

            #region _TransactionLatencies

#if DEBUG_LogTransactionTimes
            [QS.TMS.Inspection.Inspectable]
            private QS.TMS.Data.Data1D _TransactionLatencies
            {
                get
                {
                    return new QS.TMS.Data.Data1D("Transaction Latencies", transactionlatencies, "transaction latencies",
                        "transaction no", "", "transaction number", "latency", "s", "the time the transaction waited to be committed or aborted");
                }
            }
#endif 

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

            #region NewTransaction

            internal void NewTransaction(uint transactionno, bool acceptcommit)
            {
                lock (this)
                {
#if DEBUG_LogTransactionOutcomes
                        context.Logger.Log("[" + application.name + "] : NewTransaction (" +
                            transactionno.ToString() + ", " + (acceptcommit ? "ACCEPT" : "REJECT") + ")");
#endif

                    transactioncount++;
                    System.Diagnostics.Debug.Assert(transactionno == transactioncount);

#if !DEBUG_DoNotRememberLocalDecisions
                    if (transactions == null)
                        transactions = new bool[transactioncount];
                    else if (transactioncount > transactions.Length)
                    {
                        bool[] _transactions = new bool[Math.Max(2 * transactions.Length, transactioncount)];
                        Array.Copy(transactions, _transactions, transactions.Length);
                        transactions = _transactions;
                    }
                    transactions[transactionno - 1] = acceptcommit;
#endif

#if DEBUG_LogTransactionTimes
                    if (transactionlatencies == null)
                        transactionlatencies = new double[transactioncount];
                    else if (transactioncount > transactionlatencies.Length)
                    {
                        double[] _transactionlatencies = new double[Math.Max(2 * transactionlatencies.Length, transactioncount)];
                        Array.Copy(transactionlatencies, _transactionlatencies, transactionlatencies.Length);
                        transactionlatencies = _transactionlatencies;
                    }
#endif

                    if (running)
                    {
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
                    }
                }
            }

            #endregion

            #region IEndpoint.OnCommittable

            event QS._qss_x_.Protocols_.Simple2PC.CommittableCallback QS._qss_x_.Protocols_.Simple2PC.IEndpoint.OnCommittable
            {
                add 
                {
                    lock (this)
                    {
                        onCommittable += value;
#if !DEBUG_DoNotInitializeWithOldTransactions
                        for (int ind = 0; ind < transactioncount; ind++)
                            if (transactions[ind])
                                onCommittable((uint) (ind + 1));
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

            event QS._qss_x_.Protocols_.Simple2PC.AbortableCallback QS._qss_x_.Protocols_.Simple2PC.IEndpoint.OnAbortable
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

            #region IEndpoint.Commit

            void QS._qss_x_.Protocols_.Simple2PC.IEndpoint.Commit(uint m)
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

#if DEBUG_LogTransactionTimes
                    transactionlatencies[index] = application.context.Clock.Time - application.transactiontimes[index];
#endif

#if DEBUG_LogTransactionOutcomes
                    context.Logger.Log("[" + application.name + "] : COMMIT (" + m.ToString() + " )");
#endif
                }
            }

            #endregion

            #region IEndpoint.Abort

            void QS._qss_x_.Protocols_.Simple2PC.IEndpoint.Abort(uint m)
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

#if DEBUG_LogTransactionTimes
                    transactionlatencies[index] = application.context.Clock.Time - application.transactiontimes[index];
#endif

#if DEBUG_LogTransactionOutcomes
                    context.Logger.Log("[" + application.name + "] : ABORT (" + m.ToString() + " )");
#endif
                }
            }

            #endregion

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
