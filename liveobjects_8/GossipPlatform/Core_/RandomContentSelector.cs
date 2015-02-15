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
using System.Text;
using GOTransport.Common;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    /// <summary>
    /// selector for randomly selecting gossip messages for propagation
    /// </summary>
    public class RandomContentSelector : IContentSelector
    {
        #region private fields

        private RumorBuffer rumorBuffer;
        private PlatformWorkerThreadHelper platformWorkerThreadHelper;
        private object syncObj;

        #endregion

        #region constructors

        public RandomContentSelector(ref RumorBuffer _rumorBuffer, ref PlatformWorkerThreadHelper _platformWorkerThreadHelper)
        {
            rumorBuffer = _rumorBuffer;
            platformWorkerThreadHelper = _platformWorkerThreadHelper;
            syncObj = new object();
        }

        #endregion

        #region IContentSelector Members

        IGossip IContentSelector.Select(IGraphElement _destination)
        {
            // List<IGossip> rumors = rumorBuffer.GetAll();
            PriorityQueue<IGossip, DateTime> rumors = rumorBuffer.GetAll();

            if (rumors == null || rumors.Count == 0)
            {
                return null;
            }

            int rand = new Random().Next(0, rumors.Count);
            //return rumors[rand];
            PriorityQueueItem<IGossip, DateTime> item = rumors.Peek();
            List<Rumor> rumorList = new List<Rumor>();
            foreach (PriorityQueueItem<IGossip, DateTime> _item in rumors)
            {
                rumorList.Add(_item.Value as Rumor);
            }

            return rumorList[rand];
        }

        #endregion
    }
}
