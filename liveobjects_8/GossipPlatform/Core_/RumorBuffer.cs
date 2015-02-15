/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Linq;
using System.Text;
using System.Threading;
using GOTransport.Common;
using System.Diagnostics;
using System.Collections;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// Represents pending Rumors (i.e. yet to be gossiped to other nodes)
    /// </summary>
    public class RumorBuffer : MarshalByRefObject
    {
        #region private fields

        // private List<IGossip> rumorBuffer;
        PriorityQueue<IGossip, DateTime> rumorBuffer;
        private object syncObj;
        private PlatformWorkerThreadHelper platformWorkerThreadHelper;
        private AutoResetEvent rumorDispatchEvent;
        private int timeout;

        #endregion

        #region constructors

        public RumorBuffer()
        {
            rumorDispatchEvent = new AutoResetEvent(false);
            syncObj = new object();
            //rumorBuffer = new List<IGossip>();
            rumorBuffer = new PriorityQueue<IGossip, DateTime>();
        }

        #endregion

        #region public methods

        /// <summary>
        /// add a rumor to the rumor buffer
        /// </summary>
        /// <param name="_rumor"></param>
        public void Add(IGossip _rumor)
        {
            try
            {
                lock (syncObj)
                {
                    Rumor rumor = _rumor as Rumor;

                    if (rumor.ReceivedTimestamp == -1 || rumor.ReceivedTimestamp == 0)
                    {
                        rumor.StampTheTime();
                        rumor.StartTTLTicks(new TimerCallback(this.Cleanup), this.timeout);
                    }

                    //Trace.WriteLine("\r\n" + DateTime.Now.Ticks
                    //              + "[insert]\t" + Utils.GetLocalIPAddress() + "\t" + rumor.Id + "\t" + rumor.ReceivedTimestamp);

                    Console.WriteLine("[Insert], " + _rumor.Id);
                    // rumorBuffer.Add(rumor);
                    rumorBuffer.Enqueue(new PriorityQueueItem<IGossip,DateTime>(rumor as Rumor, DateTime.Now));
                }

                this.rumorDispatchEvent.Set();
            }
            catch (Exception e)
            {
                throw new UseCaseException("600", "RumorBuffer::Add " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// cleanup a rumor from rumor buffer
        /// </summary>
        /// <param name="_r">rumor to remove from the buffer</param>
        public void Cleanup(object _rumor)
        {
            try
            {
                IGossip r = (_rumor as IGossip);

                lock (syncObj)
                {
                    if (rumorBuffer.Contains(r))
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose, "RumorBuffer::Cleanup: acquired lock on syncObj, removing rumor: " + r.Id);
                        rumorBuffer.Remove(r);
                    }
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("620", "RumorBuffer::Cleanup " + e + ", " + e.StackTrace);
            }
        }
        
        /// <summary>
        /// remove a rumor
        /// </summary>
        /// <param name="_r">rumor to be removed</param>
        public void Remove(object _rumor)
        {
            try
            {
                Rumor r = (_rumor as Rumor);
                lock (syncObj)
                {
                    // rumorBuffer.Remove(_rumor as IGossip);
                    rumorBuffer.Remove(r);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("640", "RumorBuffer::Remove " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// get all rumors in rumor buffer
        /// </summary>
        /// <returns>list of rumors in buffer</returns>
        //public List<IGossip> GetAll()
        public PriorityQueue<IGossip, DateTime> GetAll()
        {   
            lock (syncObj)
            {
                return rumorBuffer;
            }
        }

        public bool DiscardOldest(IGraphElement destination)
        {
            if (rumorBuffer.Count == 0)
            {
                return false;
            }

            PriorityQueueItem<IGossip, DateTime> item = rumorBuffer.Last();
            if (item.Value != null && platformWorkerThreadHelper.IsSent(destination, item.Value)) 
            {
                rumorBuffer.Remove(item.Value);
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// return the number of rumors in the buffer
        /// </summary>
        /// <returns>count of number of rumors in the buffer</returns>
        public int GetCount()
        {
            return rumorBuffer.Count();
        }

        /// <summary>
        /// the lock per platform instance, since RumorBuffer's access should be synchronized
        /// </summary>
        /// <param name="syncObj">object used for locking</param>
        public void SetLocker(object syncObj)
        {
            this.syncObj = syncObj;
        }

        public bool HasRumor(Rumor rumor)
        {
            return rumorBuffer.Contains(rumor as IGossip);
        }

        /// <summary>
        /// helper object?
        /// </summary>
        /// <param name="_platformWorkerThreadHelper"></param>
        public void SetPlatformWorkerThreadHelper(PlatformWorkerThreadHelper _platformWorkerThreadHelper)
        {
            this.platformWorkerThreadHelper = _platformWorkerThreadHelper;
        }

        /// <summary>
        /// when called, blocks for a rumor to arrive into the RumorBuffer
        /// </summary>
        public void WaitForRumorDispatchEvent()
        {
            this.rumorDispatchEvent.WaitOne();
        }

        /// <summary>
        /// when called, resets the event used to indicate that rumor arrived in the RumorBuffer
        /// </summary>
        public void ResetRumorDispatchEvent()
        {
            this.rumorDispatchEvent.Reset();
        }

        /// <summary>
        /// when called, signals that a rumor arrived in the RumorBuffer
        /// </summary>
        public void SetRumorDispatchEvent()
        {
            this.rumorDispatchEvent.Set();
        }

        /// <summary>
        /// indicates the maximum time interval for which a rumor could stay in the RumorBuffer
        /// </summary>
        /// <param name="rumorTimeout">timeout in milliseconds</param>
        public void SetTimeout(int _rumorTimeout)
        {
            timeout = _rumorTimeout;
        }

        #endregion
    }
}
