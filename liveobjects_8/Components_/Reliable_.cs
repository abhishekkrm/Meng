/*

Copyright (c) 2004-2009 Colin Barth. All rights reserved.

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
using System.Collections;

namespace QS._qss_x_.Properties_.Component_
{
    [QS.Fx.Reflection.ComponentClass(QS.Fx.Reflection.ComponentClasses.Reliable_, "Reliable", "Updates a component with missing messages.")]
    public sealed class Reliable_<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass> :
        QS._qss_x_.Properties_.Component_.Tree_<QS._qss_x_.Properties_.Value_.ReliableMsgSetToken>,
        QS.Fx.Object.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>,
        QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
    {

        public Reliable_(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("group", QS.Fx.Reflection.ParameterClass.Value)]
            QS.Fx.Object.IReference<QS.Fx.Object.Classes.IGroup<
            QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name, QS.Fx.Serialization.ISerializable>> group,
            [QS.Fx.Reflection.Parameter("fanout", QS.Fx.Reflection.ParameterClass.Value)]
            int _fanout,
            [QS.Fx.Reflection.Parameter("rate", QS.Fx.Reflection.ParameterClass.Value)]
            double _rate,
            [QS.Fx.Reflection.Parameter("MTTA", QS.Fx.Reflection.ParameterClass.Value)]
            double _mtta,
            [QS.Fx.Reflection.Parameter("MTTB", QS.Fx.Reflection.ParameterClass.Value)]
            double _mttb,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
            : base(_mycontext, group, _fanout, _rate, _mtta, _mttb, _debugging)
        {
            this.reliableEndpoint = _mycontext.DualInterface<
            QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>,
            QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>>(this);
            
            lastCompletedEpoch = null;

            cached = new Dictionary<QS.Fx.Base.Index, HashSet<QS.Fx.Base.MessageIdentifier>>();
            pending = new Dictionary<QS.Fx.Base.MessageIdentifier, HashSet<QS.Fx.Base.Identifier>>();

            this._debugging = _debugging;
            if (_debugging)
            {
                this._form = new System.Windows.Forms.Form();
                this._textbox = new System.Windows.Forms.RichTextBox();
                this._textbox.Dock = System.Windows.Forms.DockStyle.Fill;
                this._textbox.ReadOnly = true;
                this._form.Controls.Add(this._textbox);
                this._form.Show();
            }
        }

        #region Fields

        private QS.Fx.Base.Index lastCompletedEpoch;
        private Dictionary<QS.Fx.Base.Index, HashSet<QS.Fx.Base.MessageIdentifier>> cached;
        private Dictionary<QS.Fx.Base.MessageIdentifier, HashSet<QS.Fx.Base.Identifier>> pending;
        private bool _debugging;
        private System.Windows.Forms.Form _form;
        private System.Windows.Forms.RichTextBox _textbox;
        private QS.Fx.Object.Classes.IReliableClient<
            QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass> reliableClient;
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>,
            QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>> reliableEndpoint;

        #endregion

        #region IReliable<Index,MessageIdentifier,MessageClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.IReliableClient<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>, QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>> QS.Fx.Object.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Reliable
        {
            get { return this.reliableEndpoint; }
        }

        #endregion

        #region IReliable<EpochClass,IdentifierClass,MessageClass> Members

        void QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Cached(
            QS.Fx.Base.Index epoch, QS.Fx.Base.MessageIdentifier identifier)
        {
            lock (cached)
            {
                if (!cached.ContainsKey(epoch))
                    cached.Add(epoch, new HashSet<QS.Fx.Base.MessageIdentifier>());
                HashSet<QS.Fx.Base.MessageIdentifier> ids;
                cached.TryGetValue(epoch, out ids);
                ids.Add(identifier);
            }
            QS._qss_x_.Properties_.Value_.ReliableMsgSetToken msgSet = this.cached2MsgSet();
            this.disseminateMsg(msgSet);
        }

        void QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Message(
            QS.Fx.Base.MessageIdentifier identifier, MessageClass message)
        {
            lock (pending)
            {
                HashSet<QS.Fx.Base.Identifier> ids;
                if (pending.ContainsKey(identifier))
                {
                    pending.TryGetValue(identifier, out ids);
                    foreach (QS.Fx.Base.Identifier id in ids)
                    {
                        List<QS.Fx.Serialization.ISerializable> updates = new List<QS.Fx.Serialization.ISerializable>();
                        updates.Add(new QS._qss_x_.Properties_.Value_.IdToken_(identifier, message));
                        QS._qss_x_.Properties_.Value_.MsgsForEpochToken subToken = new QS._qss_x_.Properties_.Value_.MsgsForEpochToken(null, updates, true);
                        List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken> tokenList = new List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken>();
                        tokenList.Add(subToken);
                        QS._qss_x_.Properties_.Value_.ReliableMsgSetToken token = new QS._qss_x_.Properties_.Value_.ReliableMsgSetToken(this._Identifier, this._Identifier, tokenList); 
                        this._Tree_Outgoing(id, token);
                    }
                    pending.Remove(identifier);
                }
            }
        }

        void QS.Fx.Interface.Classes.IReliable<QS.Fx.Base.Index, QS.Fx.Base.MessageIdentifier, MessageClass>.Completed(QS.Fx.Base.Index epoch)
        {
            lock(cached)
            {
                cached.Remove(epoch);
                this.removePendingEpoch(epoch);
                lastCompletedEpoch = epoch;
            }
        }

        #endregion

        protected override void _Tree_Incoming(QS.Fx.Base.Identifier _identifier, QS._qss_x_.Properties_.Value_.ReliableMsgSetToken msg)
        {
            List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken> msgs = msg.Messages;
            foreach (QS._qss_x_.Properties_.Value_.MsgsForEpochToken t in msgs)
            {
                if (t.ContainsData)
                {
                    foreach (QS.Fx.Serialization.ISerializable s in t.Messages)
                    {
                        QS._qss_x_.Properties_.Value_.IdToken_ i = (QS._qss_x_.Properties_.Value_.IdToken_)s;
                        MessageClass mc = (MessageClass)i.Payload;
                        this.reliableEndpoint.Interface.Receive(t.Epoch, i.Id, mc);
                    }
                }
                else
                {
                    lock (cached)
                    {
                        HashSet<QS.Fx.Base.MessageIdentifier> thisCached;
                        this.cached.TryGetValue(t.Epoch, out thisCached);
                        HashSet<QS.Fx.Base.MessageIdentifier> msgSet = new HashSet<QS.Fx.Base.MessageIdentifier>();
                        foreach (QS.Fx.Serialization.ISerializable s in t.Messages)
                        {
                            msgSet.Add((QS.Fx.Base.MessageIdentifier)s);
                        }
                        HashSet<QS.Fx.Base.MessageIdentifier> diff = this.difference(thisCached, msgSet);
                        foreach (QS.Fx.Base.MessageIdentifier mid in diff)
                        {
                            this.updatePending(mid, msg.OriginalId);
                            this.reliableEndpoint.Interface.Fetch(mid);
                        }
                        this.disseminateMsg(msg);
                    }
                }
            }
        }

        private void updatePending(QS.Fx.Base.MessageIdentifier mid, QS.Fx.Base.Identifier id)
        {
            lock (pending)
            {
                HashSet<QS.Fx.Base.Identifier> ids;
                if (!pending.ContainsKey(mid))
                {
                    ids = new HashSet<QS.Fx.Base.Identifier>();
                    pending.Add(mid, ids);
                }
                else
                {
                    pending.TryGetValue(mid, out ids);
                }
                ids.Add(id);
            }
        }

        private void removePendingEpoch(QS.Fx.Base.Index epoch)
        {
           /* lock (pending)
            {
                foreach (Dictionary<QS.Fx.Base.Index, HashSet<QS.Fx.Base.MessageIdentifier>> msgs in pending.Values)
                {
                    msgs.Remove(epoch);
                }
            }
            */
        }

        private HashSet<QS.Fx.Base.MessageIdentifier> difference(HashSet<QS.Fx.Base.MessageIdentifier> h1,
            HashSet<QS.Fx.Base.MessageIdentifier> h2)
        {
            HashSet<QS.Fx.Base.MessageIdentifier> result = new HashSet<QS.Fx.Base.MessageIdentifier>(h1);
            foreach (QS.Fx.Base.MessageIdentifier mi in h2)
            {
                result.Remove(mi);
            }
            return result;
        }

        private QS._qss_x_.Properties_.Value_.ReliableMsgSetToken cached2MsgSet()
        {
            List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken> msgSets = new List<QS._qss_x_.Properties_.Value_.MsgsForEpochToken>();
            lock (cached)
            {
                foreach (QS.Fx.Base.Index epoch in cached.Keys)
                {
                    HashSet<QS.Fx.Base.MessageIdentifier> msgs;
                    cached.TryGetValue(epoch, out msgs);
                    List<QS.Fx.Serialization.ISerializable> msgList = new List<QS.Fx.Serialization.ISerializable>();
                    foreach(QS.Fx.Base.MessageIdentifier mi in msgs)
                    {
                        msgList.Add((QS.Fx.Serialization.ISerializable)mi);
                    }
                    QS._qss_x_.Properties_.Value_.MsgsForEpochToken t = new QS._qss_x_.Properties_.Value_.MsgsForEpochToken(epoch,
                        msgList, false);
                    msgSets.Add(t);
                }
            }
            return new QS._qss_x_.Properties_.Value_.ReliableMsgSetToken(this._Identifier, this._Identifier, msgSets);
        }

        private void disseminateMsg(QS._qss_x_.Properties_.Value_.ReliableMsgSetToken msg)
        {
            if (!this._IsRoot)
            {
                sendToParent(msg);
            }
            if (!this._IsLeaf)
            {
                sendToChildren(msg);
            }
        }

        private void sendToChildren(QS._qss_x_.Properties_.Value_.ReliableMsgSetToken msg)
        {
            foreach (QS.Fx.Value.Classes.IMember<QS.Fx.Base.Identifier, QS.Fx.Base.Incarnation, QS.Fx.Base.Name> _child in this._Children)
            {
                if(!msg.SenderId.Equals(_child.Identifier) && !msg.OriginalId.Equals(_child.Identifier))
                    this._Tree_Outgoing(_child.Identifier, msg);
            }
        }

        private void sendToParent(QS._qss_x_.Properties_.Value_.ReliableMsgSetToken msg)
        {
            if (!msg.SenderId.Equals(this._Parent.Identifier) && !msg.OriginalId.Equals(this._Parent.Identifier))
                this._Tree_Outgoing(this._Parent.Identifier, msg);
        }

        private void printDebug(object msg)
        {
            if (this._debugging)
            {
                lock (this)
                {
                    string smsg = (string)msg;
                    if (_form.InvokeRequired)
                        _form.BeginInvoke(new QS.Fx.Base.ContextCallback(this.printDebug), new object[] { msg });
                    else
                        this._textbox.AppendText(smsg + "\r\n");
                }
            }
        }
    }
}
