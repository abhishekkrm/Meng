/*

Copyright (c) 2004-2009 Saad Sami. All rights reserved.

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

namespace liveobjects_8.Components_
{
    [QS.Fx.Reflection.ComponentClass("B8415197AA56491380FAC98872950090", "AppAzureBacking", "")]
    public sealed class AppAzureBacking<
        [QS.Fx.Reflection.Parameter("MessageClass", QS.Fx.Reflection.ParameterClass.ValueClass)] MessageClass,
        [QS.Fx.Reflection.Parameter("CheckpointClass", QS.Fx.Reflection.ParameterClass.ValueClass)] CheckpointClass>
         : QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>
        where MessageClass : class, QS.Fx.Serialization.ISerializable
        where CheckpointClass : class, QS.Fx.Serialization.ISerializable
    {
        #region Constructor
        public AppAzureBacking(QS.Fx.Object.IContext _mycontext,
            [QS.Fx.Reflection.Parameter("AccountName", QS.Fx.Reflection.ParameterClass.Value)] string _AccountName,
            [QS.Fx.Reflection.Parameter("AccountSharedKey", QS.Fx.Reflection.ParameterClass.Value)] string _AccountSharedKey,
            [QS.Fx.Reflection.Parameter("StorageEndpoint", QS.Fx.Reflection.ParameterClass.Value)] string _StorageEndpoint,
            [QS.Fx.Reflection.Parameter("timer interval", QS.Fx.Reflection.ParameterClass.Value)] int _interval,
            [QS.Fx.Reflection.Parameter("debugging", QS.Fx.Reflection.ParameterClass.Value)] bool _debugging)
        {
            this._mycontext = _mycontext;
            this._AccountName = _AccountName;
            this._AccountSharedKey = _AccountSharedKey;
            this._StorageEndpoint = _StorageEndpoint;
            this._debugging = _debugging;
            this._interval = _interval;

            // Set up Azure Parameters
            this._BaseUri = new Uri(this._StorageEndpoint);
            this._usePathStyleUris = null;
            this._allowIncompleteSettings = false;

            // Create Azure Storage Account Info
            //StorageAccountInfo accounInfo = new StorageAccountInfo(_BaseUri, _usePathStyleUris, _AccountName, _AccountSharedKey, _allowIncompleteSettings);
        }
        #endregion

        #region Fields

        private QS.Fx.Object.IContext _mycontext;
        private int interval;

        // Azure Account Info Fields
        private string _AccountName;
        private string _AccountSharedKey;
        private string _StorageEndpoint;
        private bool? _usePathStyleUris;
        private bool _allowIncompleteSettings;
        private Uri _BaseUri;
        
        [QS.Fx.Base.Inspectable("endpoint")]
        private QS.Fx.Endpoint.Internal.IDualInterface<
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>,
            QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> _clientEndpoint;

        private bool _debugging;
        private int _interval;

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        QS.Fx.Endpoint.Classes.IDualInterface<QS.Fx.Interface.Classes.ICheckpointedCommunicationChannelClient<MessageClass, CheckpointClass>, QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>> QS.Fx.Object.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Channel
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ICheckpointedCommunicationChannel<MessageClass,CheckpointClass> Members

        void QS.Fx.Interface.Classes.ICheckpointedCommunicationChannel<MessageClass, CheckpointClass>.Send(MessageClass _message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
