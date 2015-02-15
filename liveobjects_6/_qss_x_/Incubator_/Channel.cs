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

// #define DEBUG_LogOutgoingMessages
// #define DEBUG_LogControlObjectStateUponMessageSending
// #define DEBUG_LogMessageDeliveryRelatedEvents

#define DEBUG_CollectDetailedProfilingInformation

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Incubator_
{
    public sealed class Channel : QS.Fx.Inspection.Inspectable, QS._qss_x_.Agents_.Base.IMember
    {
#if DEBUG_CollectDetailedProfilingInformation
        public static QS.Fx.Clock.IClock ProfilingClock;
        public static double sum_serialize;
        public static int num_serialize;
#endif

        internal Channel(Incubator incubator, Member localmember, Member remotemember)
        {
            this.incubator = incubator;
            this.localmember = localmember;
            this.remotemember = remotemember;
        }

        internal Incubator incubator;

        [QS.Fx.Base.Inspectable]
        internal Member localmember, remotemember;
        [QS.Fx.Base.Inspectable]
        internal double lasttime;

//        [TMS.Inspection.Inspectable]
//        internal bool isdead;

        #region IMember Members

        QS._qss_x_.Base1_.AgentID QS._qss_x_.Agents_.Base.IMember.ID
        {
            get { return remotemember.agent.id; }
        }

        QS._qss_x_.Agents_.Base.MemberCategory QS._qss_x_.Agents_.Base.IMember.Category
        {
            get { return remotemember.category; }
        }

        void QS._qss_x_.Agents_.Base.IMember.Send(QS.Fx.Serialization.ISerializable message)
        {
#if DEBUG_LogOutgoingMessages
#if DEBUG_LogControlObjectStateUponMessageSending
            StringBuilder s = new StringBuilder();
            foreach (QS.Fx.Agents.Base.Session _session in ((QS.Fx.Agents.Base.Agent) localmember.agent.agent).sessions.Values)
                s.AppendLine("Session(" + _session.id.ToString() + ") :\n" + QS.Fx.Printing.Printable.ToString(_session.controlobject));
#endif
            localmember.agent.logger.Log("{ " + localmember.agent.id.ToString() + " # " + localmember.agent.incarnation.ToString() +
                " } sending a message to { " + remotemember.agent.id.ToString() + " # " + remotemember.agent.incarnation.ToString() + " : \n" +
                QS.Fx.Printing.Printable.ToString(message)
#if DEBUG_LogControlObjectStateUponMessageSending
                + "\n\nSTATE:\n\n" + s.ToString()
#endif
                );
#endif

            // serialize and deserialize to create an entirely independent copy of the object

#if DEBUG_CollectDetailedProfilingInformation
            double __time1 = ProfilingClock.Time;
#endif

            IList<QS.Fx.Base.Block> blocks = QS._core_c_.Base3.Serializer.ToSegments(message);
            int count = 0;
            foreach (QS.Fx.Base.Block block in blocks)
                count += (int) block.size;
            byte[] bytes = new byte[count];
            int offset = 0;
            foreach (QS.Fx.Base.Block block in blocks)
            {
                if ((block.type & QS.Fx.Base.Block.Type.Managed) == QS.Fx.Base.Block.Type.Managed && block.buffer != null)
                    Buffer.BlockCopy(block.buffer, (int) block.offset, bytes, offset, (int) block.size);
                else
                    throw new Exception("Unmanaged memory is not supported here.");
                offset += (int) block.size;
            }

            Message _message = new Message(remotemember.agent.id, remotemember.agent.incarnation, localmember.cluster.incarnation, 
                QS._core_c_.Base3.Serializer.FromSegment(new ArraySegment<byte>(bytes)));

#if DEBUG_CollectDetailedProfilingInformation
            double __time2 = ProfilingClock.Time;
            sum_serialize += __time2 - __time1;
            num_serialize++;
#endif

            // schedule delivery

            double now = ((QS.Fx.Clock.IClock) incubator.simulatedclock).Time;
            double latency;
            // latency = incubator.minlatency + (incubator.minlatency - incubator.avglatency) * Math.Log(incubator.random.NextDouble());
            latency = incubator.minlatency + 2 * (incubator.avglatency - incubator.minlatency) * incubator.random.NextDouble();
            lasttime = Math.Max(lasttime + 0.000001, now + latency);

            incubator._messagesizes.Add(lasttime, (double) count);

            ((QS.Fx.Clock.IAlarmClock) incubator.simulatedclock).Schedule(
                lasttime - now, new QS.Fx.Clock.AlarmCallback(this._AlarmCallback), _message);
        }

        #endregion

        #region _AlarmCallback

        private void _AlarmCallback(QS.Fx.Clock.IAlarm alarm)
        {
            Message message = (Message) alarm.Context;

            if (/* !isdead && */ message.incarnation.Equals(remotemember.agent.incarnation))
            {
                if (message.configuration_incarnation.Equals(remotemember.agent.member.cluster.incarnation))
                {
                    double t1 = incubator.physicalclock.Time;
                    remotemember.agent.agent.Receive(localmember.cluster.incarnation, localmember.index, message.message);
                    double t2 = incubator.physicalclock.Time;
                    double overhead = t2 - t1;
                    incubator._receivingtimes.Add(alarm.Time, overhead);
                }
                else
                {
#if DEBUG_LogMessageDeliveryRelatedEvents
                    incubator.mainlogger.Log("Suppressing delivery of message sent in a different cluster incarnation, from " +
                        localmember.agent.id.ToString() + " # " + localmember.agent.incarnation.ToString() + " in cluster " +
                        localmember.cluster.domain.id.ToString() + " incarnation " + message.configuration_incarnation.ToString() + " to " +
                        message.id.ToString() + " # " + message.incarnation.ToString() + " in cluster " + remotemember.cluster.domain.id.ToString() +
                        " incarnation " + remotemember.cluster.incarnation.ToString() + ".");
#endif
                }
            }
            else
            {
#if DEBUG_LogMessageDeliveryRelatedEvents
                incubator.mainlogger.Log("Suppressing delivery on dead channel from " +
                    localmember.agent.id.ToString() + " # " + localmember.agent.incarnation.ToString() + " to " +
                    message.id.ToString() + " # " + message.incarnation.ToString() + ", the channel was rerouted to " +
                    remotemember.agent.id.ToString() + " # " + remotemember.agent.incarnation.ToString() + ".");
#endif
            }
        }

        #endregion
    }
}
