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
using System.Net;

namespace QS._qss_x_.Applications_
{
    public class Application_003 : Platform_.IApplication
    {
        public Application_003()
        {
        }

        private QS.Fx.Platform.IPlatform platform;
        private QS._qss_x_.Platform_.IApplicationContext context;
        private QS._qss_x_.Persistence_.IPersistent<IState, State, State.Operation> persistent;
        private Random random = new Random();

        #region Class State

        public interface IState
        {
            int Num
            {
                get;
            }
        }

        [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_App3_State)]
        [Serializable]
        public class State : IState, QS.Fx.Serialization.ISerializable
        {
            public State()
            {
            }

            private int num;

            #region IState Members

            int IState.Num
            {
                get { return num; }
            }

            #endregion

            #region Class Operation

            [QS.Fx.Serialization.ClassID(QS.ClassID.Fx_App3_State_Operation)]
            [Serializable]
            public class Operation : QS._qss_x_.Persistence_.IOperation<State>, QS.Fx.Serialization.ISerializable
            {
                public Operation(int num)
                {
                    this.num = num;
                }

                public Operation()
                {
                }

                private int num;

                #region QS.Fx.Persistence.IOperation<State> Members

                void QS._qss_x_.Persistence_.IOperation<State>.Execute(State state)
                {
                    // Console.WriteLine("Adding " + num.ToString());
                    lock (state)
                    {
                        state.num += num;
                    }
                }

                #endregion

                #region QS.Fx.Serialization.ISerializable Members

                unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
                {
                    get { return new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.Fx_App3_State_Operation), sizeof(int)); }
                }

                unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                    ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
                {
                    fixed (byte* parray = header.Array)
                    {
                        *((int*)(parray + header.Offset)) = num;
                    }
                }

                unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
                    ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
                {
                    fixed (byte* parray = header.Array)
                    {
                        num = *((int*)(parray + header.Offset));
                    }
                }

                #endregion
            }

            #endregion

            #region QS.Fx.Serialization.ISerializable Members

            unsafe QS.Fx.Serialization.SerializableInfo QS.Fx.Serialization.ISerializable.SerializableInfo
            {
                get { return new QS.Fx.Serialization.SerializableInfo((ushort)(QS.ClassID.Fx_App3_State), sizeof(int)); }
            }

            unsafe void QS.Fx.Serialization.ISerializable.SerializeTo(
                ref QS.Fx.Base.ConsumableBlock header, ref IList<QS.Fx.Base.Block> data)
            {
                fixed (byte* parray = header.Array)
                {
                    *((int*)(parray + header.Offset)) = num;
                }
            }

            unsafe void QS.Fx.Serialization.ISerializable.DeserializeFrom(
                ref QS.Fx.Base.ConsumableBlock header, ref QS.Fx.Base.ConsumableBlock data)
            {
                fixed (byte* parray = header.Array)
                {
                    num = *((int*)(parray + header.Offset));
                }
            }

            #endregion
        }

        #endregion

        #region IApplication Members

        #region IApplication.Start

        void QS._qss_x_.Platform_.IApplication.Start(QS.Fx.Platform.IPlatform platform, QS._qss_x_.Platform_.IApplicationContext context)
        {
            platform.Logger.Log("Application : Starting");

            this.platform = platform;
            this.context = context;

            persistent = new QS._qss_x_.Persistence_.Persistent<QS.Fx.Serialization.ISerializable, IState, State, State.Operation>(
                platform.Filesystem.Root, platform.AlarmClock, QS._core_c_.Base3.Serializer.Global, platform.Logger);

            persistent.OnReady += new QS.Fx.Base.Callback(
                delegate
                {
                    platform.Logger.Log("_____Ready : Num = " + persistent.State.Num.ToString());
                    
                    platform.AlarmClock.Schedule(1, new QS.Fx.Clock.AlarmCallback(
                        delegate(QS.Fx.Clock.IAlarm alarm)
                        {
                            for (int ind = 0; ind < 3; ind++)
                            {
                                int val = random.Next(5);
                                platform.Logger.Log("_____Submitting(" + val.ToString() + ")");

                                persistent.Submit(new State.Operation(val), null);
                            }

                            alarm.Reschedule();
                        }), null);
                });

            persistent.OnChange += new QS.Fx.Base.ContextCallback<State.Operation>(
                delegate(State.Operation operation)
                {
                    platform.Logger.Log("_____Change : Num = " + persistent.State.Num.ToString());
                });

            platform.Logger.Log("Application : Started");
        }

        #endregion

        #region IApplication.Start

        void QS._qss_x_.Platform_.IApplication.Stop()
        {
            platform.Logger.Log("Application : Stopping");

            // .......................................................................................................................................................................................................

            platform.Logger.Log("Application : Stopped");
        }

        #endregion

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            platform.Logger.Log("Application : Disposing");

            // .......................................................................................................................................................................................................

            platform.Logger.Log("Application : Disposed");
        }

        #endregion
    }
}
