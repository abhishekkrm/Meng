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

#define DEBUG_LogGenerously

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_x_.Filesystem_
{
    public class VirtualDisk : IVirtualDisk
    {
        public VirtualDisk(QS.Fx.Logging.ILogger logger, QS.Fx.Clock.IAlarmClock alarmClock)
        {
            this.logger = logger;
            this.alarmClock = alarmClock;

            completionCallback = new QS.Fx.Clock.AlarmCallback(this.CompletionCallback);
        }

        private const double latency = 0.1;
        private const double speed = 1000;
        private const int reorder_top = 20;

        private static Random random = new Random();

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Clock.IAlarmClock alarmClock;
        private QS.Fx.Clock.AlarmCallback completionCallback;
        private QS.Fx.Clock.IAlarm alarm;
        private Queue<Request> requests_tail = new Queue<Request>();
        private Request current_request;
        private Request[] requests_head = new Request[reorder_top];
        private int requests_head_count;
        private bool processing;

        #region Structure Request

        private struct Request
        {
            public Request(IVirtualFile file, int position, int count, QS.Fx.Base.ContextCallback callback, object context)
            {
                this.file = file;
                this.position = position;
                this.count = count;
                this.callback = callback;
                this.context = context;
            }

            public IVirtualFile file;
            public int position, count;
            public QS.Fx.Base.ContextCallback callback;
            public object context;
        }

        #endregion

        #region Generating delay

        private double _GenerateDelay(int nbytes)
        {
            return (latency + nbytes * 8 / speed) * (1 + random.NextDouble()) / 2;
        }

        #endregion

        #region CompletionCallback

        private void CompletionCallback(QS.Fx.Clock.IAlarm alarm)
        {
            Nullable<Request> request;

            lock (this)
            {
                if (processing)
                {
                    request = current_request;

                    if (requests_head_count > 0)
                    {
                        if (requests_head_count < requests_head.Length && requests_tail.Count > 0)
                            throw new Exception("Internal error: head or the request queue is not full, yet the tail is nonempty.");

                        if (requests_head_count > 1)
                        {
                            int request_index = random.Next(0, requests_head_count);    
                        
                            current_request = requests_head[request_index];
                            if (request_index != (requests_head_count - 1))
                                requests_head[request_index] = requests_head[requests_head_count - 1];
                            requests_head_count--;

                            while (requests_head_count < requests_head.Length && requests_tail.Count > 0)
                                requests_head[requests_head_count++] = requests_tail.Dequeue();
                        }
                        else
                        {
                            current_request = requests_head[0];
                            requests_head_count = 0;
                        }

                        alarm.Reschedule(_GenerateDelay(current_request.count));
                    }
                    else
                    {
                        processing = false;

                        if (requests_tail.Count > 0)
                            throw new Exception("Internal error: no requests are queued at head, but there are requests at tail.");
                    }
                }
                else
                    request = null;
            }

            if (request.HasValue && request.Value.callback != null)
                request.Value.callback(request.Value.context);
        }

        #endregion

        #region IVirtualDisk Members

        void IVirtualDisk.Schedule(IVirtualFile file, int position, int count, QS.Fx.Base.ContextCallback callback, object context)
        {
            lock (this)
            {
                Request request = new Request(file, position, count, callback, context);

                if (processing)
                {
                    if (requests_head_count < requests_head.Length)
                        requests_head[requests_head_count++] = request;
                    else
                        requests_tail.Enqueue(request);
                }
                else
                {
                    if (requests_head_count > 0 || requests_tail.Count > 0)
                        throw new Exception("Internal error: there are requests enqueued, but the disk is not processing them.");

                    current_request = request;

                    if (alarm == null)
                        alarm = alarmClock.Schedule(_GenerateDelay(count), completionCallback, null);
                    else
                        alarm.Reschedule(_GenerateDelay(count));

                    processing = true;
                }
            }
        }

        void IVirtualDisk.Reset()
        {
#if DEBUG_LogGenerously
            logger.Log("Resetting filesystem.");
#endif

            lock (this)
            {
#if DEBUG_LogGenerously
                if (processing)
                    logger.Log("Canceling request: " + QS.Fx.Printing.Printable.ToString(current_request));

                for (int ind = 0; ind < requests_head_count; ind++)
                    logger.Log("Canceling request: " + QS.Fx.Printing.Printable.ToString(requests_head[ind]));

                foreach (Request request in requests_tail)
                    logger.Log("Canceling request: " + QS.Fx.Printing.Printable.ToString(request));
#endif

                current_request = new Request();

                Array.Clear(requests_head, 0, requests_head.Length);
                requests_head_count = 0;
                
                requests_tail.Clear();

                processing = false;

                if (alarm != null)
                {
                    try
                    {
                        if (!alarm.Cancelled)
                            alarm.Cancel();
                    }
                    catch (Exception)
                    {
                    }

                    alarm = null;
                }
            }
        }

        #endregion
    }
}
