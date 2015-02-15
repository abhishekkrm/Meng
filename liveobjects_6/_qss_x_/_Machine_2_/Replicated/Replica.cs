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
using System.Diagnostics;

namespace QS._qss_x_._Machine_2_.Replicated
{
    [QS._qss_x_.Base1_.SynchronizationClass(QS._qss_x_.Base1_.SynchronizationOption.Asynchronous)]
    public sealed class Replica : IDisposable
    {
        #region Constructor

        public Replica(QS.Fx.Platform.IPlatform platform, string root) : this(platform, root, BootOption.None, null)
        {
        }

        public Replica(QS.Fx.Platform.IPlatform platform, string root, BootOption bootOption) : this(platform, root, bootOption, null)
        {
        }

        public Replica(QS.Fx.Platform.IPlatform platform, string root, BootOption bootoption, QS.Fx.Base.ID machineID)
        {
            this.platform = platform;
            if (root != null) 
            {
                QS.Fx.Filesystem.IFilesystemObject _root = platform.Filesystem[root];
                if (_root.Type != QS.Fx.Filesystem.FilesystemObjectType.Folder)
                    throw new Exception("Cannot initialize, the provided root path does not point to a folder in the filesystem.");
                this.root = (QS.Fx.Filesystem.IFolder) _root;
            }
            else
                this.root = platform.Filesystem.Root;
            this.bootoption = bootoption;
            this.machineID = machineID;
            this.persisted = 
                new QS._qss_x_.Persistence_.Persistent<QS.Fx.Serialization.ISerializable, IPersisted, Persisted, Persisted.Operation>(
                    this.root, platform.AlarmClock, QS._core_c_.Base3.Serializer.Global, platform.Logger);
            persisted.OnReady += new QS.Fx.Base.Callback(this._LoadingCallback_1);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion

        #region Fields

        private QS.Fx.Platform.IPlatform platform;
        private QS.Fx.Filesystem.IFolder root;
        private QS._qss_x_.Persistence_.IPersistent<IPersisted, Persisted, Persisted.Operation> persisted;
        private QS.Fx.Base.ID machineID;
        private BootOption bootoption;

        #endregion

        #region _LoadingCallback_1

        private void _LoadingCallback_1()
        {
            lock (this)
            {
                if (!persisted.Ready)
                    throw new Exception("Persistent state not ready upon entering _LoadingCallback");

                Persisted.Operation operation = new Persisted.Operation();

                if (((IEquatable<QS.Fx.Base.ID>) persisted.State.MachineID).Equals(QS.Fx.Base.ID.Undefined))
                {
                    if ((machineID == null) || ((IEquatable<QS.Fx.Base.ID>)machineID).Equals(QS.Fx.Base.ID.Undefined))
                    {
                        if ((bootoption & BootOption.Master) != BootOption.Master)
                            throw new Exception("Machine identity unknown.");
                        else
                            machineID = QS.Fx.Base.ID.NewID();
                    }

                    operation.Add(new Persisted.Operation.SetMachineID(machineID));                        
                }
                else
                {
                    if ((machineID != null) && !((IEquatable<QS.Fx.Base.ID>) machineID).Equals(QS.Fx.Base.ID.Undefined))
                    {
                        if (!((IEquatable<QS.Fx.Base.ID>)machineID).Equals(persisted.State.MachineID))
                            throw new Exception("Machine identity mismatch.");
                    }
                    else
                        machineID = persisted.State.MachineID;
                }

                platform.Logger.Log("Machine ID : " + machineID.ToString());

                // ...............................

//                if ((bootoption & BootOption.Master) == BootOption.Master)
//                {
//                    platform.Logger.Log("Acting as a master");
//
//                    // ...............................
//                }

                persisted.Submit(operation, new QS.Fx.Base.ContextCallback<Persisted.Operation>(this._LoadingCallback_2));
            }
        }

        #endregion

        #region _LoadingCallback_2

        private void _LoadingCallback_2(Persisted.Operation operation)
        {
            lock (this)
            {
                // .......
            }
        }

        #endregion
    }
}
