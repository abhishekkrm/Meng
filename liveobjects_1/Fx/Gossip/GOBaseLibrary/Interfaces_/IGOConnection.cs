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
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOBaseLibrary.Interfaces
{
    public interface IGOConnection
    {
        bool Subscribe(ISubscription _subscription, RumorFeedEventHandler handler);

        void Unsubscribe(ISubscription _subscription);

        // set which platform instance to work on
        void SetWorkingContext(string port);

        // indicate that the initialization is complete for the instance        
        void InitializationComplete();

        // add a new node to a group
        void AddNode(Group _group, Node _node);
       
        // add a list of nodes to a group
        void AddNodeList(IGraphElement group, NodeList nodeList);

        // connect two nodes
        void Connect(Node node1, Node node2, double cost);

        // disconnect two nodes
        void DisConnect(Node node1, Node node2);
        
        // remove a node from a group
        void RemoveNode(IGraphElement group, IGraphElement node);
        
        // remove a list of nodes from a group
        void RemoveNodeList(IGraphElement group, NodeList nodeList);
  
        // send a rumor
        void Send(Rumor _rumor) ;
    
        // receive a rumor
        IGossip Receive();
       
        // check if the platform is ready
        Boolean IsReady();

        // close the connection
        void Close(string port);

        // close the connection
        void Clear();

        // fetch rumors received from other nodes
        Rumor BeginToFetch();

        // set which platform instance to receive the rumors from
        void SetReceiveContext(String _context, 
            out AutoResetEvent _ae, 
            out AutoResetEvent _fec, 
            out AutoResetEvent _fes,
            out List<Rumor> _fedRumor);

        void SetFedRumor(ref List<Rumor> rumor);

        event RumorFeedEventHandler AppCallbackEvent;
    }
}
