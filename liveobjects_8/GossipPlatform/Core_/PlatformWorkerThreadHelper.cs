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
using GOTransport.Core;
using GOTransport.GossipTransport;
using System.Diagnostics;
using System.Threading;
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Debugging;
using GOBaseLibrary.Common;

namespace GOTransport.Core
{
    public class PlatformWorkerThreadHelper
    {
        #region private fields

        private Dictionary<string, List<string>> SentRumorRecorder;
        private GroupObjectGraph groupObjectGraph;
        private Node currentNode;
        private object syncObj;
        private IDictionary<Node, double> neighbors;
        private DebuggingContext debuggingContext;

        #endregion

        #region constructors

        public PlatformWorkerThreadHelper(GroupObjectGraph _groupObjectGraph,
                                            IGraphElement _currentNode,
                                            object _syncObj,
                                            DebuggingContext _debuggingContext)
        {
            SentRumorRecorder = new Dictionary<string, List<string>>();
            groupObjectGraph = _groupObjectGraph;
            currentNode = _currentNode as Node;
            syncObj = _syncObj;
            debuggingContext = _debuggingContext;

            if (this.currentNode != null)
            {
                neighbors = this.currentNode.getNeighborList();
            }
        }

        #endregion

        #region public methods

        public void Record(IGossip _rumor, IGraphElement _node)
        {
            try
            {
                lock (syncObj)
                {
                    if (_rumor == null || _node == null)
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose, debuggingContext + "PlatformWorkerThreadHelper::Record, nothing to record, exiting");
                    }

                    if (!SentRumorRecorder.ContainsKey(_node.GetID()))
                    {
                        Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                            debuggingContext
                                            + "PlatformWorkerThreadHelper::Record, adding node to track, node [" + _node.GetID());
                        SentRumorRecorder.Add(_node.GetID(), new List<string>());
                    }

                    Debug.WriteLineIf(Utils.debugSwitch.Verbose,
                                        debuggingContext
                                        + "PlatformWorkerThreadHelper::Record, adding rumor to track, rumor [" + _rumor.Id + "]");
                    SentRumorRecorder[_node.GetID()].Add(_rumor.Id);
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("820", "PlatformWorkerThreadHelper::Record " + e + ", " + e.StackTrace);
            }
        }

        public bool IsSent(IGraphElement _node, IGossip _rumor)
        {
            try
            {
                lock (syncObj)
                {
                    if (_rumor == null || _node == null || !SentRumorRecorder.ContainsKey(_node.GetID()))
                    {
                        return false;
                    }

                    List<string> rumorsSent = SentRumorRecorder[_node.GetID()];

                    bool retValue = rumorsSent.Contains(_rumor.Id);
                    return retValue;
                }
            }
            catch (Exception e)
            {
                throw new UseCaseException("840", "PlatformWorkerThreadHelper::IsSent " + e + ", " + e.StackTrace);
            }
        }

        #endregion
    }
}
