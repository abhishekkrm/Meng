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

// #define DEBUG_EnableLogging

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Backbone_.Node
{
    public sealed class Connection : Connections_2_.Connection, IConnection, IConnectionControl, IControllerConnectionContext, IChannel
    {
        #region Constructor

        public Connection(bool isoutgoing, string name, Base1_.Address address, QS._qss_c_.Base3_.ISerializableSender sender,
            QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink, IConnectionControlContext controlcontext) : base()
        {
            this.isoutgoing = isoutgoing;
            this.name = name;
            this.address = address;
            this.sender = sender;
            this.sink = sink;
            this.controlcontext = controlcontext;
        }

        #endregion

        #region Constants

        private const double RetransmissionTimeout = 1;
        private const int WindowSize = 200;
        private const double AcknowledgementTimeout = 0.01;
        private const int MaximumRangesPerAck = 20;

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable] private bool isoutgoing;
        [QS.Fx.Base.Inspectable] private string name;
        [QS.Fx.Base.Inspectable] private Base1_.Address address;
        [QS.Fx.Base.Inspectable] private QS._qss_c_.Base3_.ISerializableSender sender;
        [QS.Fx.Base.Inspectable] private QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> sink;
        [QS.Fx.Base.Inspectable] private QS.Fx.Base.ID id;
        [QS.Fx.Base.Inspectable] private ulong endpoint1, endpoint2;
        [QS.Fx.Base.Inspectable] private Phase phase = Phase.Negotiating;
        [QS.Fx.Base.Inspectable] private QS.Fx.Clock.IAlarm alarm;
        [QS.Fx.Base.Inspectable] private IControllerConnection controllerConnection;

        private IConnectionControlContext controlcontext;

        [QS.Fx.Base.Inspectable] private bool registered, registeredack, waitingtoack;
        [QS.Fx.Base.Inspectable] private double nextack;
        [QS.Fx.Base.Inspectable] private QS.Fx.Clock.IAlarm ackalarm;
        [QS.Fx.Base.Inspectable] private uint sequenceno;
        [QS.Fx.Base.Inspectable] private Queue<Outgoing> totransmit, toretransmit;
        [QS.Fx.Base.Inspectable] private QS._qss_c_.Receivers5.IPendingCollection<Outgoing> requests;
        [QS.Fx.Base.Inspectable] private IDictionary<uint, Outgoing> followup;
        [QS.Fx.Base.Inspectable] private QS._qss_c_.Receivers4.IAckCollection ackcollection;

        #endregion

        #region IControllerConnectionContext Members

        IChannel IControllerConnectionContext.RequestChannel
        {
            get { return this; }
        }

        string IControllerConnectionContext.Name
        {
            get { return name; }
        }

        QS.Fx.Base.ID IControllerConnectionContext.ID
        {
            get { return id; }
        }

        bool IControllerConnectionContext.IsSuper
        {
            get { return isoutgoing; }
        }

        #endregion

        #region IConnection Members

        string IConnection.Name
        {
            get { return name; }
        }

        bool IConnection.IsOutgoing
        {
            get { return isoutgoing; }
        }

        #endregion

        #region IConnectionControl Members

        Base1_.Address IConnectionControl.Address
        {
            get { return address; }
        }

        QS._qss_c_.Base3_.ISerializableSender IConnectionControl.UnreliableSender
        {
            get { return sender; }
        }

        QS._core_c_.Base6.ISink<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> IConnectionControl.UnreliableSink
        {
            get { return sink; }
        }

        QS.Fx.Base.ID IConnectionControl.ID
        {
            get { return id; }
            set { id = value; }
        }

        ulong IConnectionControl.Endpoint1
        {
            get { return endpoint1; }
            set { endpoint1 = value; }
        }

        ulong IConnectionControl.Endpoint2
        {
            get { return endpoint2; }
            set { endpoint2 = value; }
        }

        Phase IConnectionControl.Phase
        {
            get { return phase; }
            set { phase = value; }
        }

        QS.Fx.Clock.IAlarm IConnectionControl.Alarm
        {
            get { return alarm; }
            set { alarm = value; }
        }

        IControllerConnection IConnectionControl.ControllerConnection
        {
            get { return controllerConnection; }
            set { controllerConnection = value; }
        }

        IControllerConnectionContext IConnectionControl.ControllerConnectionContext
        {
            get { return this; }
        }

        void IConnectionControl.Initialize()
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\").Initialize");
#endif

            totransmit = new Queue<Outgoing>();
            toretransmit = new Queue<Outgoing>();
            requests = new QS._qss_c_.Receivers5.PendingCollection<Outgoing>();
            followup = new Dictionary<uint, Outgoing>();
            sequenceno = 0;
            ackcollection = new QS._qss_c_.Receivers4.AckCollection1(controlcontext.Clock);
        }

        void IConnectionControl.Cleanup()
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\").Cleanup");
#endif

            // TODO: Could iterate over messages and call back that they're failed......

            ackcollection = null;
            followup = null;
            requests = null;
            toretransmit = null;
            totransmit = null;
        }

        void IConnectionControl.Receive(Incoming message)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\").Receive : \n" + QS.Fx.Printing.Printable.ToString(message));
#endif

            bool handle_now = false, acknowledge_now = false;

            lock (this)
            {
                if (message.Endpoint2.Equals(endpoint1) && message.Endpoint1.Equals(endpoint2))
                {
                    double time = controlcontext.Clock.Time;

                    handle_now = ackcollection.Add(message.Number);

                    if (!waitingtoack)
                    {
                        waitingtoack = true;
                        if (time >= nextack)
                            acknowledge_now = true;
                        else
                        {
                            double timeout = nextack - time;
                            if (ackalarm != null)
                                ackalarm.Reschedule(timeout);
                            else
                                ackalarm = controlcontext.AlarmClock.Schedule(timeout,
                                    new QS.Fx.Clock.AlarmCallback(this._AcknowledgementCallback), null);
                        }
                    }
                }
            }

            if (acknowledge_now)
                _Acknowledge();

            if (handle_now)
                _Handle(message);
        }

        void IConnectionControl.Receive(Acknowledgement acknowledgement)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\").Receive : \n" + QS.Fx.Printing.Printable.ToString(acknowledgement));
#endif

            IEnumerable<Outgoing> justacked = null;
            lock (this)
            {
                if (acknowledgement.Endpoint2.Equals(endpoint1) && acknowledgement.Endpoint1.Equals(endpoint2))
                    requests.Remove(acknowledgement.Numbers, out justacked);
            }

            if (justacked != null)
            {
                foreach (Outgoing request in justacked)
                    _Acknowledged(request);
            }
        }

        #endregion

        #region IChannel Members

        IOutgoing IChannel.Submit(
            QS.Fx.Serialization.ISerializable body, MessageOptions options, QS.Fx.Base.ContextCallback<IOutgoing> callback, object context)
        {
            return _Submit(0, body, options, callback, context);
        }

        #endregion

        #region _Submit

        private IOutgoing _Submit(uint cookie, QS.Fx.Serialization.ISerializable body, MessageOptions options,
            QS.Fx.Base.ContextCallback<IOutgoing> callback, object context)
        {
            Outgoing request;
            bool register_now;

            lock (this)
            {
                request = new Outgoing(controlcontext.ID, endpoint1, id, endpoint2, 0, cookie, body, options, callback, context);

#if DEBUG_EnableLogging
                controlcontext.Logger.Log("Connection(\"" + name + "\")._Submit : \n" + QS.Fx.Printing.Printable.ToString(request));
#endif

                totransmit.Enqueue(request);
                register_now = !registered;
                registered = true;
            }

            if (register_now)
                sink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this._OutgoingCallback));

            return request;
        }

        #endregion

        #region _OutgoingCallback

        private void _OutgoingCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> transmitting, int maxcount, out int count, out bool hasmore)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._OutgoingCallback");
#endif

            Queue<Outgoing> transmitted = new Queue<Outgoing>();

            lock (this)
            {
                count = 0;
                hasmore = true;

                while (count < maxcount && toretransmit.Count > 0)
                {
                    Outgoing request = toretransmit.Dequeue();
                    if ((request.State & MessageState.Completed) != MessageState.Completed)
                    {
#if DEBUG_EnableLogging
                        controlcontext.Logger.Log("Connection(\"" + name + "\")._OutgoingCallback, Retransmitting : \n" + 
                            QS.Fx.Printing.Printable.ToString(request));
#endif

                        transmitting.Enqueue(request);
                        transmitted.Enqueue(request);
                        count++;
                    }
                }

                if (toretransmit.Count == 0)
                {
                    while (true)
                    {
                        if (totransmit.Count > 0)
                        {
                            if (count < maxcount)
                            {
                                Outgoing request = totransmit.Dequeue();
                                request.Number = ++sequenceno;
                                requests.Add(request.Number, request);
                                if ((request.Options & MessageOptions.Respond) == MessageOptions.Respond)
                                    followup.Add(request.Number, request);

#if DEBUG_EnableLogging
                                controlcontext.Logger.Log("Connection(\"" + name + "\")._OutgoingCallback, Transmitting : \n" +
                                    QS.Fx.Printing.Printable.ToString(request));
#endif

                                transmitting.Enqueue(request);
                                transmitted.Enqueue(request);
                            }
                            else
                                break;
                        }
                        else
                        {
                            hasmore = false;
                            registered = false;
                            break;
                        }
                    }
                }
            }

            foreach (Outgoing request in transmitted)
            {
                if ((request.State & MessageState.Completed) != MessageState.Completed)
                {
                    request.Retransmitted = false;
                    request.Alarm = controlcontext.AlarmClock.Schedule(RetransmissionTimeout, 
                        new QS.Fx.Clock.AlarmCallback(this._RetransmissionCallback), request);
                }
            }
        }

        #endregion

        #region _RetransmissionCallback

        private void _RetransmissionCallback(QS.Fx.Clock.IAlarm alarm)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._RetransmissionCallback");
#endif

            Outgoing request = (Outgoing) alarm.Context;
            bool retransmit_now;
            lock (request)
            {
                retransmit_now = ((request.State & MessageState.Completed) != MessageState.Completed) && !request.Retransmitted;
                if (retransmit_now)
                {
                    request.Retransmitted = true;

                    if (request.Alarm != null)
                    {
                        try { request.Alarm.Cancel(); } catch (Exception) { }
                    }
                    request.Alarm = null;
                }
            }

            if (retransmit_now)
            {
                bool register_now;
                lock (this)
                {
                    toretransmit.Enqueue(request);
                    register_now = !registered;
                    registered = true;
                }

                if (register_now)
                    sink.Send(new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(this._OutgoingCallback));
            }
        }

        #endregion

        #region _AcknowledgementCallback

        private void _AcknowledgementCallback(QS.Fx.Clock.IAlarm alarmRef)
        {
            lock (this)
            {
                _Acknowledge();
            }
        }

        #endregion

        #region _Acknowledge

        private void _Acknowledge()
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._Acknowledge");
#endif

            bool register_now;
            lock (this)
            {
                register_now = !registeredack;
                registeredack = true;
            }

            if (register_now)
                sink.Send(
                    new QS._core_c_.Base6.GetObjectsCallback<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>>(
                        this._OutgoingAcknowledgementCallback));
        }

        #endregion

        #region _OutgoingAcknowledgementCallback

        private void _OutgoingAcknowledgementCallback(
            Queue<QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>> transmitting, int maxcount, out int count, out bool hasmore)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._OutgoingAcknowledgementCallback");
#endif

            lock (this)
            {
                IList<QS._qss_c_.Base1_.Range<uint>> numbers;
                uint covered;
                QS._qss_c_.Receivers5.AckHelper.AckCollection2Acks(ackcollection, MaximumRangesPerAck, out numbers, out covered);

                transmitting.Enqueue(
                    new Acknowledgement(controlcontext.ID, endpoint1, id, endpoint2, numbers, covered));

                double time = controlcontext.Clock.Time;
                waitingtoack = false;
                nextack = time + AcknowledgementTimeout;
                count = 1;
                hasmore = false;
                registeredack = false;
            }
        }

        #endregion

        #region _Acknowledged

        private void _Acknowledged(Outgoing request)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._Acknowledged : \n" + QS.Fx.Printing.Printable.ToString(request));
#endif

            bool acknowledged_now = false;
            QS.Fx.Base.ContextCallback<IOutgoing> callback = null;

            lock (request)
            {
                if (request.Alarm != null)
                {
                    try { request.Alarm.Cancel(); }
                    catch (Exception) { }
                }
                request.Alarm = null;

                if ((request.State & MessageState.Acknowledged) != MessageState.Acknowledged)
                {
                    request.State = request.State | MessageState.Acknowledged;
                    
                    if ((request.Options & MessageOptions.Respond) != MessageOptions.Respond)
                        request.State = request.State | MessageState.Completed | MessageState.Succeeded;

                    acknowledged_now = true;
                    callback = request.Callback;
                }
            }

            if (acknowledged_now && callback != null &&
                ((request.Options & MessageOptions.Acknowledge) == MessageOptions.Acknowledge))
            {
                callback(request);
            }
        }

        #endregion

        #region _Handle

        private void _Handle(Incoming message)
        {
#if DEBUG_EnableLogging
            controlcontext.Logger.Log("Connection(\"" + name + "\")._Handle : \n" + QS.Fx.Printing.Printable.ToString(message));
#endif

            message.ResponseCallback = new ResponseCallback(this._ResponseCallback);

            if (message.Cookie > 0)
            {
                Outgoing outgoing = null;

                lock (this)
                {
                    if (followup.TryGetValue(message.Cookie, out outgoing))
                        followup.Remove(message.Cookie);
                    else
                        outgoing = null;
                }

                if (outgoing != null)
                {
                    QS.Fx.Base.ContextCallback<IOutgoing> callback;

                    lock (outgoing)
                    {
                        outgoing.Response = message;
                        outgoing.State = outgoing.State | MessageState.Responded | MessageState.Completed | MessageState.Succeeded;
                        callback = outgoing.Callback;
                    }

                    if (callback != null)
                        callback(outgoing);
                }
            }
            else
            {
                controllerConnection.Handle(message);
            }
        }

        #endregion

        #region _ResponseCallback

        private IOutgoing _ResponseCallback(Incoming request, QS.Fx.Serialization.ISerializable response, 
            MessageOptions options, QS.Fx.Base.ContextCallback<IOutgoing> callback, object context)
        {
            return _Submit(request.Number, response, options, callback, context); 
        }

        #endregion
    }
}
