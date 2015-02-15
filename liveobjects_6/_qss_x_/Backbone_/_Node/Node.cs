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

namespace QS._qss_x_.Backbone_._Node
{
    /// <summary>
    /// Represents a non-replicated node in the backbone hierarchy, a singleton that manages its scope alone. 
    /// It can represent a leaf-node client or a non-replicated server.
    /// </summary>
    public sealed class Node : INode, IDisposable
    {
        #region Constructor

        public Node(string scopename, QS.Fx.Base.ID scopeid, QS.Fx.Logging.ILogger logger)
        {
            this.scopename = scopename;
            this.scopeid = scopeid;
            this.logger = logger;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        private string scopename;
        private QS.Fx.Base.ID scopeid;
        private QS.Fx.Logging.ILogger logger;

        private IDictionary<string, ScopeConnection> scopes = new Dictionary<string, ScopeConnection>();

        #endregion

        // INode Members

        #region INode.ScopeConnections

        IEnumerable<IScopeConnection> INode.ScopeConnections
        {
            get 
            {
                lock (scopes)
                {
                    List<IScopeConnection> result = new List<IScopeConnection>(scopes.Count);
                    foreach (ScopeConnection connection in scopes.Values)
                        result.Add(connection);
                    return result;
                }
            }
        }

        #endregion

        #region INode.BeginConnect

        IAsyncResult INode.BeginConnect(
            string scopeName, AsyncCallback callback, object cookie, IEnumerable<QS._qss_x_.Base1_.Address> addresses)
        {
            lock (scopes)
            {
                ScopeConnection connection;

                if (!scopes.TryGetValue(scopeName, out connection))
                {
                    connection = new ScopeConnection(scopeName);
                    scopes.Add(scopeName, connection);

                    ((Connections_2_.IConnectionControl) connection).ReleaseCallback =
                        new QS.Fx.Base.ContextCallback<Connections_2_.IConnectionControl>(this._ReleaseScopeCallback);
                }

                ((IScopeConnectionControl) connection).AddAddresses(addresses);

                bool connect;
                Connections_2_.Request request = 
                    Connections_2_.Request.Create(connection, callback, cookie, out connect);

                if (connect)
                    _StartConnectionToScope(connection);

                return request;
            }
        }

        private void _ReleaseScopeCallback(Connections_2_.IConnectionControl connection)
        {
            _StopConnectionToScope((ScopeConnection) connection);
        }

        #endregion

        #region INode.EndConnect

        IScopeConnection INode.EndConnect(IAsyncResult result)
        {
            return Connections_2_.Request.GetConnection<ScopeConnection>(result);
        }

        #endregion

        #region [ NOT IMPLEMENTED ] INode.TopicConnections

/*
        IEnumerable<ITopicConnection> INode.TopicConnections
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
*/

        #endregion

        // Managing scope connections

        #region [ NOT IMPLEMENTED ] _StartConnectionToScope

        private void _StartConnectionToScope(ScopeConnection connection)
        {
            System.Diagnostics.Debug.Assert(false, "Not Implemented");
            // ................................................................................................................................................................
        }

        #endregion

        #region [ NOT IMPLEMENTED ] _StopConnectionToScope

        private void _StopConnectionToScope(ScopeConnection connection)
        {
            System.Diagnostics.Debug.Assert(false, "Not Implemented");
            // ................................................................................................................................................................
        }

        #endregion
    }
}
