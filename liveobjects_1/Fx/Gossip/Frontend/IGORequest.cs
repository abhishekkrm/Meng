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
using GOBaseLibrary.Interfaces;
using GOBaseLibrary.Common;

namespace GOTransport.Frontend
{
    [QS.Fx.Reflection.InterfaceClass("1`1", "IGORequest")]
    public interface IGORequest : QS.Fx.Interface.Classes.IInterface
    {
        #region IGORequest members

        [QS.Fx.Reflection.Operation("Ready")]
        bool Ready();

        [QS.Fx.Reflection.Operation("Subscribe")]
        void Subscribe(ISubscription _subscriptionParameterList);

        [QS.Fx.Reflection.Operation("AddNode")]
        void AddNode(IGraphElement _group, IGraphElement _node);

        [QS.Fx.Reflection.Operation("AddNodeList")]
        void AddNodeList(IGraphElement _group, NodeList _nodeList);

        [QS.Fx.Reflection.Operation("RemoveNode")]
        void RemoveNode(IGraphElement _group, IGraphElement _node);

        [QS.Fx.Reflection.Operation("RemoveNodeList")]
        void RemoveNodeList(IGraphElement _group, NodeList _nodeList);

        [QS.Fx.Reflection.Operation("Connect")]
        void Connect(IGraphElement _node1, IGraphElement _node2, double _cost);

        [QS.Fx.Reflection.Operation("DisConnect")]
        void DisConnect(IGraphElement _node1, IGraphElement _node2);

        [QS.Fx.Reflection.Operation("Send")]
        void Send(IGossip _gossip);

        [QS.Fx.Reflection.Operation("Receive")]
        IGossip Receive();

        [QS.Fx.Reflection.Operation("IsReady")]
        Boolean IsReady();

        [QS.Fx.Reflection.Operation("HasGraphData")]
        Boolean HasGraphData();

        [QS.Fx.Reflection.Operation("SetHasGraphData")]
        void SetHasGraphData(Boolean _flag);

        [QS.Fx.Reflection.Operation("Close")]
        void Clear();

        [QS.Fx.Reflection.Operation("SetWorkingContext")]
        void SetWorkingContext(String port);

        [QS.Fx.Reflection.Operation("InitializationComplete")]
        void InitializationComplete();

        [QS.Fx.Reflection.Operation("BeginToFetch")]
        Rumor BeginToFetch();

        #endregion
    }
}
