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

#define PROFILE

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Component_.Classes_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Counter, "Counter", "(some stuff used internally for testing and debugging)")]
    [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
    public sealed class Counter_ : QS.Fx.Inspection.Inspectable, QS.Fx.Object.Classes.IObject, 
        QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>
    {
        #region Constructor

        public Counter_
        (
            QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)] 
            double _rate,
            [QS.Fx.Reflection.Parameter("channel", QS.Fx.Reflection.ParameterClass.Value)] 
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _channel,
            [QS.Fx.Reflection.Parameter("debug", QS.Fx.Reflection.ParameterClass.Value)] 
            bool _debug
        ) 
        {
            this._mycontext = _mycontext;
            this._debug = _debug;
            if (_rate <= 0)
                throw new ArgumentException();
            this._rate = _rate;
            this._channelendpoint = _mycontext.DualInterface<
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
                QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>>(this);
            if (_channel == null)
                throw new ArgumentException();
            this._channelobject = _channel.Dereference(_mycontext);
            this._channelconnection = ((QS.Fx.Endpoint.Classes.IEndpoint) this._channelendpoint).Connect(this._channelobject.Channel);
            this._timer = new System.Threading.Timer(
                new System.Threading.TimerCallback(this._TimerCallback), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000.0 / this._rate));
        }

        #endregion

        #region Fields

        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.IContext _mycontext;
        [QS.Fx.Base.Inspectable]
        private bool _debug;
        [QS.Fx.Base.Inspectable]
        private double _rate;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>> _channelendpoint;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText> _channelobject;
        [QS.Fx.Base.Inspectable]
        private QS.Fx.Endpoint.IConnection _channelconnection;
        [QS.Fx.Base.Inspectable]
        private System.Threading.Timer _timer;
        [QS.Fx.Base.Inspectable]
        private int _count;
#if PROFILE
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_send = new QS._qss_c_.Statistics_.Samples2D(
            "sends", "send times and counts", "time", "s", "time in seconds", "count", "", "");
        [QS.Fx.Base.Inspectable]
        private QS._qss_c_.Statistics_.Samples2D _statistics_receive = new QS._qss_c_.Statistics_.Samples2D(
            "receives", "receive times and counts", "time", "s", "time in seconds", "count", "", "");
#endif

        #endregion

        #region ICheckpointedCommunicationChannelClient<QS.Fx.Channel.Message.UnicodeText, QS.Fx.Channel.Message.UnicodeText> Members

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Initialize(
            QS.Fx.Value.Classes.IText _checkpoint)
        {
            lock (this)
            {
                if (_checkpoint != null)
                {
                    int _count = Convert.ToInt32(_checkpoint.Text);
                    if (_count > this._count)
                    {
                        this._count = _count;
#if PROFILE
                        this._statistics_receive.Add(this._mycontext.Platform.Clock.Time, _count);
#endif
                        if (this._debug)
                            this._mycontext.Platform.Logger.Log("Counter ( " + this._count.ToString() + " )");
                    }
                }
            }
        }

        [QS.Fx.Base.Synchronization(QS.Fx.Base.SynchronizationOption.Multithreaded)]
        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Receive(
            QS.Fx.Value.Classes.IText _message)
        {
            lock (this)
            {
                if (_message != null)
                {
                    int _count = Convert.ToInt32(_message.Text);
                    if (_count > this._count)
                    {
                        this._count = _count;
#if PROFILE
                        this._statistics_receive.Add(this._mycontext.Platform.Clock.Time, _count);
#endif
                        if (this._debug)
                            this._mycontext.Platform.Logger.Log("Counter ( " + this._count.ToString() + " )");
                    }
                }
            }
        }

        QS.Fx.Value.Classes.IText
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<QS.Fx.Value.Classes.IText, QS.Fx.Value.Classes.IText>.Checkpoint()
        {
            lock (this)
            {
                return new QS.Fx.Value.UnicodeText(this._count.ToString());
            }
        }

        #endregion

        #region _TimerCallback

        private void _TimerCallback(object _o)
        {
            lock (this)
            {
                try
                {
                    if (_channelendpoint.IsConnected)
                    {
                        int _count = this._count + 1;
#if PROFILE
                        this._statistics_send.Add(this._mycontext.Platform.Clock.Time, _count);
#endif
                        _channelendpoint.Interface.Send(new QS.Fx.Value.UnicodeText(_count.ToString()));
                    }
                }
                catch
                {
                }
            }
        }

        #endregion
    }
}
