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

// #define DEBUG_LogOnEnteringPhases

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Incubator_.Applications_
{
    // Coordinated phases
    public sealed class Application2 : QS.Fx.Inspection.Inspectable, IApplication
    {
        #region Constructor

        public Application2()
        {
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

        internal System.Random random = new System.Random();

        #endregion

        #region _Phases

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data2D _Phases
        {
            get
            {
                List<QS._core_e_.Data.XY> _maxphases = new List<QS._core_e_.Data.XY>();
                for (int ind = 0; ind < clients.Length; ind++)
                    if (clients[ind].running)
                        _maxphases.Add(new QS._core_e_.Data.XY(ind, clients[ind].maxphase));

                return new QS._core_e_.Data.Data2D("Phases", _maxphases.ToArray(), "phases started on clients",
                    "client index", "", "zero-based client index", "phase", "", "the latest phase that was started on the client");
            }
        }

        #endregion

        #region _PhaseTimes_Minimum

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _PhaseTimes_Minimum
        {
            get
            {
                uint _maxphase = 0;
                for (int ind = 0; ind < clients.Length; ind++)
                    _maxphase = Math.Max(_maxphase, clients[ind].maxphase);
                double[] _times = new double[_maxphase + 1];
                for (int _ind = 0; _ind <= _maxphase; _ind++)
                {
                    double _mintime = double.MaxValue;
                    bool _found = false;
                    for (int ind = 0; ind < clients.Length; ind++)
                    {
                        if (_ind <= clients[ind].maxphase)
                        {
                            double _time = clients[ind].phasetimes[_ind];
                            if (_time > 0)
                            {
                                _found = true;
                                _mintime = Math.Min(_mintime, _time);
                            }
                        }
                    }
                    _times[_ind] = _found ? _mintime : 0;
                }

                return new QS._core_e_.Data.Data1D("Phase Times (Minimum)", _times, "times when phases first started on any node",
                    "phase no", "", "phase number", "time", "s", "the earliest time phase has started on any node");
            }
        }

        #endregion

        #region _PhaseTimes_Distance

        [QS.Fx.Base.Inspectable]
        private QS._core_e_.Data.Data1D _PhaseTimes_Distance
        {
            get
            {
                uint _maxphase = 0;
                for (int ind = 0; ind < clients.Length; ind++)
                    _maxphase = Math.Max(_maxphase, clients[ind].maxphase);
                double[] _times = new double[_maxphase + 1];
                for (int _ind = 0; _ind <= _maxphase; _ind++)
                {
                    double _mintime = double.MaxValue, _maxtime = double.MinValue;
                    bool _found = false;
                    for (int ind = 0; ind < clients.Length; ind++)
                    {
                        if (_ind <= clients[ind].maxphase)
                        {
                            double _time = clients[ind].phasetimes[_ind];
                            if (_time > 0)
                            {
                                _found = true;
                                _mintime = Math.Min(_mintime, _time);
                                _maxtime = Math.Max(_maxtime, _time);
                            }
                        }
                    }
                    _times[_ind] = _found ? (_maxtime - _mintime) : 0;
                }

                return new QS._core_e_.Data.Data1D("Phase Times (Distance)", _times, "maximum distance between phase starting times",
                    "phase no", "", "phase number", "distance", "s", "the maximum distance between the times phase has started on different nodes");
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
        }

        IClient[] IApplication.Clients
        {
            get { return clients; }
        }

        QS._qss_x_.Agents_.Base.IProtocol IApplication.Protocol
        {
            get { return QS._qss_x_.Protocols_.CoordinatedPhases.Protocol; }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Class Client

        internal sealed class Client : QS.Fx.Inspection.Inspectable, IClient, QS._qss_x_.Agents_.Base.IClient, QS._qss_x_.Protocols_.CoordinatedPhases.IEndpoint
        {
            #region Constructor

            internal Client(Application2 application, int index)
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
            internal Application2 application;
            [QS.Fx.Base.Inspectable]
            internal string name;
            [QS.Fx.Base.Inspectable]
            internal IClientContext context;
            [QS.Fx.Base.Inspectable]
            internal int index;
            [QS.Fx.Base.Inspectable]
            internal bool running;
            [QS.Fx.Base.Inspectable]
            internal uint maxphase;
            [QS.Fx.Base.Inspectable]
            internal double[] phasetimes = new double[1];
            [QS.Fx.Base.Inspectable]
            internal QS._qss_x_.Agents_.Base.IConnection connection;

            #endregion

            #region _PhaseTimes

            [QS.Fx.Base.Inspectable]
            private QS._core_e_.Data.Data1D _PhaseTimes
            {
                get
                {
                    return new QS._core_e_.Data.Data1D("PhaseTimes", phasetimes, "phase times",
                        "phase no", "", "phase number", "time", "s", "the time the phase started");
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

            #region IEndpoint.Phase

            void QS._qss_x_.Protocols_.CoordinatedPhases.IEndpoint.Phase(uint k)
            {
#if DEBUG_LogOnEnteringPhases
                context.Logger.Log("Component on " + name + " entering phase (" + k.ToString() + ").");
#endif

                lock (this)
                {
                    System.Diagnostics.Debug.Assert(k >= maxphase);
                    if (phasetimes != null)
                    {
                        if (k >= phasetimes.Length)
                        {
                            double[] _phasetimes = new double[2 * Math.Max(k + 1, phasetimes.Length)];
                            Array.Copy(phasetimes, _phasetimes, phasetimes.Length);
                            phasetimes = _phasetimes;
                        }
                    }
                    else
                        phasetimes = new double[k + 1];
                    phasetimes[k] = application.context.Clock.Time;
                    maxphase = k;
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
