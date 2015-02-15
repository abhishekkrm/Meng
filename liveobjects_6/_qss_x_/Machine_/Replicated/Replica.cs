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

namespace QS._qss_x_.Machine_.Replicated_
{
    public sealed class Replica : IReplica
    {
        #region Constructors

        public Replica()
        {
        }

        #endregion

        #region Fields

        private IReplicaContext context;
        private State state = new State();

        #endregion

        #region IReplica.Handle

        void IReplica.Handle(IEvent e)
        {
            switch (e.Type)
            {
                case EventType.Initialization:
                    _OnInitialization((OnInitialization) e);
                    break;

                case EventType.Appended:
                    _OnAppended((OnAppended) e);
                    break;

                case EventType.Initialized:
                case EventType.Append:
                    throw new NotSupportedException();

                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // events
        // ................................................................................................................................................................................................................................

        #region _OnInitialization

        private void _OnInitialization(OnInitialization e)
        {
            this.context = e.context;
            
            state.localaddress = e.localaddress;

            if (e.operations != null)
            {
                foreach (IOperation o in e.operations)
                {
                    switch (o.Type)
                    {
                        case OperationType.Checkpoint:
                            _DoCheckpoint((DoCheckpoint)o);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            _MyCheckOnInitialization();

            state.localincarnation++;

            _MyGenerateCheckpoint();
        }

        #endregion

        #region _OnAppended

        private void _OnAppended(OnAppended e)
        {
            foreach (IOperation o in e.operations)
            {
                switch (o.Type)
                {
                    case OperationType.Checkpoint:
                        _MyCheckpointComplete();
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // operations
        // ................................................................................................................................................................................................................................

        #region _DoCheckpoint

        private void _DoCheckpoint(DoCheckpoint o)
        {
            state.localid = o.localid;
            state.localincarnation = o.localincarnation;
            state.machineid = o.machineid;
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // internal
        // ................................................................................................................................................................................................................................

        #region _MyCheckOnInitialization

        private void _MyCheckOnInitialization()
        {
            if (state.localid == null)
                throw new Exception("Local id is null.");

            if (state.machineid == null)
                throw new Exception("Machine id is null.");
        }

        #endregion

        #region _MyGenerateCheckpoint

        private void _MyGenerateCheckpoint()
        {
            DoCheckpoint checkpoint = new DoCheckpoint();

            checkpoint.localid = state.localid;
            checkpoint.localincarnation = state.localincarnation;
            checkpoint.machineid = state.machineid;

            context.Handle(new OnAppend(new IOperation[] { checkpoint }));
        }

        #endregion

        #region _MyCheckpointComplete

        private void _MyCheckpointComplete()
        {
            if (!state.initialized)
            {
                // ................................
            }
        }

        #endregion

        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    }
}
