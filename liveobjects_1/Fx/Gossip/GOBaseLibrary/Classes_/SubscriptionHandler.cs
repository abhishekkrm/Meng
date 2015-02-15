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
using GOBaseLibrary.Interfaces;

namespace GOBaseLibrary.Common
{
    public delegate void RumorSeekEventHandler(object sender, EventArgs e);

    public class SubscriptionHandler
    {
        private int totalMessageRate;
        private List<ISubscription> subscriptionList;
        private Dictionary<string, string> loConfig;

        public SubscriptionHandler()
        {
            subscriptionList = new List<ISubscription>();
        }

        /// <summary>
        /// subscribe an application as a user of the platform
        /// </summary>
        /// <param name="_subscription">subscription containing service requirements</param>
        /// <returns>true if subsciption was successful, false otherwise</returns>
        public bool Subscribe(ISubscription _subscription)
        {
            try
            {
                int _messageRate = Int32.Parse(_subscription.Get(SubscriptionPropertyKey.MAX_MESSAGE_RATE).ToString());
                int totalSoFar = _messageRate;

                foreach (ISubscription current in subscriptionList)
                {
                    int rate = Int32.Parse(current.Get(SubscriptionPropertyKey.MAX_MESSAGE_RATE).ToString());
                    rate += totalSoFar;
                }

                if (totalSoFar > totalMessageRate)
                {
                    return false;
                }

                subscriptionList.Add(_subscription);
                return true;
            }
            catch (Exception e)
            {
                throw new UseCaseException("200", "SubscriptionHandler::Subscribe " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// unsubscribe an application from the list of users of platform
        /// </summary>
        /// <param name="_subscription"></param>
        public void Unsubscribe(ISubscription _subscription)
        {
            try
            {
                if (subscriptionList.Contains(_subscription))
                {
                    subscriptionList.Remove(_subscription);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("220", "SubscriptionHandler::Unsubscribe " + e + ", " + e.StackTrace);
            }
        }

        /// <summary>
        /// set the configuration (key/value pairs)
        /// </summary>
        /// <param name="_loConfig">dictionary object containing the key/value pairs</param>
        public void SetConfig(Dictionary<string, string> _loConfig)
        {
            this.loConfig = _loConfig;

            try
            {
                if (loConfig.ContainsKey("MAX_MESSAGE_RATE"))
                {
                    totalMessageRate = Int32.Parse(loConfig["MAX_MESSAGE_RATE"]);
                }
                else
                {
                    totalMessageRate = Constants.DEFAULT_MAX_MESSAGE_RATE;
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("240", "SubscriptionHandler::SetConfig " + e + ", " + e.StackTrace);
            }
        }
    }
}
