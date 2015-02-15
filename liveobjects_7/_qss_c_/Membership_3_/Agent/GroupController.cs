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

// #define UseEnhancedRateControl

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_c_.Membership_3_.Agent
{
/*
    public class GroupController : Base8.ISender
    {
        // TODO: Finish installing a new context and disabling and flushing the old one
        // TODO: Add code to clients so that they invoke this class's GetObjects call to retrieve pending messages

        public GroupController(QS.Fx.Logging.ILogger logger)
        {
            this.logger = logger;
        }

        private QS.Fx.Logging.ILogger logger;
        private bool waiting;
        private Queue<Base6.IAsynchronous<Base3.Message>> incomingObjects =
            new Queue<Base6.IAsynchronous<Base3.Message>>();
        private Queue<Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>>> incomingCallbacks =
            new Queue<Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>>>();
        private ClientContext clientContext;

        #region Interface used by the membership service to update this object

        public void NewContext(ClientContext context)
        {
/-*
            ClientContext old_context, new_context = context;
            lock (this)
            {
                old_context = clientContext;
                clientContext = context;
            }
            
            old_context.

*-/

            ClientContext to_signal = null;
            lock (this)
            {
                clientContext = context;
                if (waiting)
                    to_signal = context;
            }

            if (to_signal != null)
                to_signal.Signal();
        }

        #endregion

        #region GetObjects

        public void GetObjects(Queue<Base6.IAsynchronous<Base3.Message>> destinationQueue,
            int maximumNumberOfObjectsToReturn, int maximumNumberOfBytesToReturn, 
            out int numberOfObjectsReturned, out int numberOfBytesReturned, out bool moreObjectsAvailable)
        {
            // TODO: Implement enhanced rate control

            numberOfObjectsReturned = numberOfBytesReturned = 0;
            moreObjectsAvailable = true;

            lock (this)
            {
                while (numberOfObjectsReturned < maximumNumberOfObjectsToReturn) // && numberOfBytesReturned < maximumNumberOfBytesToReturn)
                {
                    if (incomingObjects.Count > 0)
                    {
                        destinationQueue.Enqueue(incomingObjects.Dequeue());
                        numberOfObjectsReturned++;
                        // numberOfBytesReturned += ......................................................................................HERE
                    }
                    else if (incomingCallbacks.Count > 0)
                    {
                        Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>> callback = incomingCallbacks.Dequeue();

                        int nreturned;
#if UseEnhancedRateControl
                        int nbytesreturned;
#endif
                        bool more;
                        callback(incomingObjects, 
                            maximumNumberOfObjectsToReturn - numberOfObjectsReturned, 
#if UseEnhancedRateControl
                            int.MaxValue, // maximumNumberOfBytesToReturn - numberOfBytesReturned,
#endif                            
                            out nreturned, 
#if UseEnhancedRateControl                            
                            out nbytesreturned, 
#endif                            
                            out more);

                        if (more)
                        {
                            incomingCallbacks.Enqueue(callback);
                            if (nreturned == 0)
                            {
                                logger.Log(this, 
                                    "Internal problem: the callback has more objects, but did not return anything.");

                                moreObjectsAvailable = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        moreObjectsAvailable = false;
                        break;
                    }
                }
            }
        }

        #endregion

        #region ISender Members

        void QS.CMS.Base8.ISender.Send(QS.CMS.Base3.Message message)
        {
            ((Base8.ISender)this).BeginSend(message, null, null);
        }

        IAsyncResult QS.CMS.Base8.ISender.BeginSend(
            QS.CMS.Base3.Message message, AsyncCallback callback, object context)
        {
            Base6.AsynchronousRequest request = new Base6.AsynchronousRequest(message, callback, context);
            ClientContext to_signal = null;
            lock (this)
            {                
                incomingObjects.Enqueue(request);
                if (clientContext != null && !waiting)
                {
                    waiting = true;
                    to_signal = clientContext;
                }
            }

            if (to_signal != null)
                to_signal.Signal();

            return request;
        }

        void QS.CMS.Base8.ISender.EndSend(IAsyncResult operation)
        {
            ((Base6.AsynchronousRequest)operation).ConsumeResult();
        }

        #endregion

        #region ISink<IAsynchronous<Message>> Members

        void QS.CMS.Base6.ISink<Base6.IAsynchronous<Base3.Message>>.Send(
            Base6.GetObjectsCallback<Base6.IAsynchronous<Base3.Message>> getObjectsCallback)
        {
            ClientContext to_signal = null;
            lock (this)
            {
                incomingCallbacks.Enqueue(getObjectsCallback);
                if (clientContext != null && !waiting)
                {
                    waiting = true;
                    to_signal = clientContext;
                }
            }

            if (to_signal != null)
                to_signal.Signal();
        }

        int Base6.ISink<Base6.IAsynchronous<Base3.Message>>.MTU
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
*/ 
}
