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

#define VERBOSE
//#define PROFILE

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass("2CD5FE8B9A9848d48429B28968F5A558", "MCexp8")]
    public class MCexp8
        : QS._qss_x_.Properties_.Component_.Base_,
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region Constructor

        public MCexp8
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>
                _channel,
            [QS.Fx.Reflection.Parameter("batch size", QS.Fx.Reflection.ParameterClass.Value)] int _batch_size,
            [QS.Fx.Reflection.Parameter("batchs per second", QS.Fx.Reflection.ParameterClass.Value)] int _batch_rate,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] bool _debug
        )
            : base(_mycontext, _debug)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_."+_component_name+".Constructor");
#endif
            this._channelendpoint = _mycontext.DualInterface<
               QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
               QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);


            this._channelendpoint.OnConnected +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Connect))); });
            this._channelendpoint.OnDisconnect +=
                new QS.Fx.Base.Callback(
                    delegate { this._Enqueue(new QS._qss_x_.Properties_.Base_.Event_(new QS._qss_x_.Properties_.Base_.EventCallback_(this._Disconnect))); });

            if (_channel != null)
                this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint)this._channelendpoint).Connect(_channel.Dereference(_mycontext).Channel);

            this._rate_update = new Timer();
            this._rate_update.Interval = 1000;
            this._rate_update.AutoReset = true;
            this._rate_update.Elapsed += new ElapsedEventHandler(this._update_rate);
            this._rate = 0;
            this._msgRecvd = 0;

            this._batch_callback = new Timer();
            this._batch_callback.Interval = 1000 / _batch_rate;
            this._batch_callback.AutoReset = true;
            this._batch_callback.Elapsed += new ElapsedEventHandler(this._send_batch);

            this._batch_rate = _batch_rate;
            this._batch_size = _batch_size;
        }

        #endregion

        #region Fields

        

        [QS.Fx.Base.Inspectable]
        private const string _component_name = "MCexp8";

        [QS.Fx.Base.Inspectable]
        private int _batch_size;
        [QS.Fx.Base.Inspectable]
        private int _batch_rate;

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _channelendpoint;
        [QS.Fx.Base.Inspectable("channelconnection")]
        private QS.Fx.Endpoint.IConnection _channelconnection;
        [QS.Fx.Base.Inspectable]
        private bool _initialized;
        [QS.Fx.Base.Inspectable]
        private bool _notified;
        [QS.Fx.Base.Inspectable]
        private int _msgRecvd;
        [QS.Fx.Base.Inspectable]
        private Timer _batch_callback;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Value.Classes.IText _currMsg;
        [QS.Fx.Base.Inspectable]
        private double _startTime;
        [QS.Fx.Base.Inspectable]
        private double _rate;
        [QS.Fx.Base.Inspectable]
        private Timer _rate_update;

#if PROFILE
        [QS.Fx.Base.Inspectable]

        private QS._qss_c_.Statistics_.Samples2D _statistics_recvtimes = new QS._qss_c_.Statistics_.Samples2D(
            "msg recvd times", "times messsages are received on the cc level", "msg", "", "msg #", "time", "s", "time in seconds");
        //[QS.Fx.Base.Inspectable]
        //private QS._qss_c_.Statistics_.Samples2D _statistics_dequeue = new QS._qss_c_.Statistics_.Samples2D(
        //    "enqueue times", "times it takes to enqueue", "time", "s", "time in seconds", "time", "s", "time in seconds");
#endif


        #endregion

        #region Timer Callbacks
        private void _update_rate(object s, ElapsedEventArgs e)
        {
            this._rate = this._msgRecvd / (_platform.Clock.Time - _startTime);
        }

        private void _send_batch(object s, ElapsedEventArgs e)
        {
            for (int i = 0; i < this._batch_size; i++)
            {
                this._channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText(i.ToString()));
            }
        }

        #endregion
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Initialize

        protected override void _Initialize()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Initialize");
#endif

            base._Initialize();
        }

        #endregion

        #region _Dispose

        protected override void _Dispose()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Dispose");
#endif

            lock (this)
            {
                if (this._channelendpoint.IsConnected)
                    this._channelendpoint.Disconnect();


                base._Dispose();
            }
        }

        #endregion

        #region _Start

        protected override void _Start()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Start");
#endif

            base._Start();
        }

        protected void _Start(QS.Fx.Base.Address _address)
        {
            lock (this)
            {
                //this._address = _address;
                //this._initialized = true;
                //if (this._transport_endpoint.IsConnected)
                //{
                //    this._notified = true;
                //    this._transport_endpoint.Interface.Address(_address);
                //}
                //this._ConnectChannel();


                //setup timer
            }
        }

        #endregion

        #region _Stop

        protected override void _Stop()
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Stop");
#endif

            lock (this)
            {
                this._initialized = false;

                base._Stop();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        #region _Connect

        private void _Connect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Connect ");
#endif

            lock (this)
            {
                this._batch_callback.Start();
            }

            // start sending
        }

        #endregion


        #region _Disconnect

        private void _Disconnect(QS._qss_x_.Properties_.Base_.IEvent_ _event)
        {
#if VERBOSE
            if (this._logger != null)
                this._logger.Log("Component_." + _component_name + "._Disconnect");
#endif

            lock (this)
            {
                this._notified = false;
            }
        }

        #endregion












        #region ICheckpointedCommunicationChannelClient<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(QS.Fx.Value.Classes.IText _message)
        {
#if PROFILE
            this._statistics_recvtimes.Add(this._msgRecvd, _platform.Clock.Time);
#endif

            if (_msgRecvd == 1)
            {
                while (this._startTime == 0)
                    this._startTime = _platform.Clock.Time;
                this._rate_update.Start();
            }
            this._msgRecvd++;
            this._currMsg = _message;
            
        }

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(QS.Fx.Value.Classes.IText _checkpoint)
        {
            this._msgRecvd++;
            
            //this._currMsg = (MessageClass)_checkpoint;
        }

        QS.Fx.Value.Classes.IText QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            return new QS.Fx.Value.UnicodeText("chkpt");
        }

        #endregion
    }
}
