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

namespace QS._qss_c_.FailureDetection_
{
    public class DummyFD : IFailureDetector
    {
        public DummyFD()
        {
        }

        public void Crashed(QS._core_c_.Base3.InstanceID deadAddress)
        {
            Crashed(new QS._core_c_.Base3.InstanceID[] { deadAddress });
        }

        public void Crashed(QS._core_c_.Base3.InstanceID[] deadAddresses)
        {
            Change[] changes = null;
            ChangeCallback toCall = null;

            lock (this)
            {
                cachedDeadAddresses.AddRange(deadAddresses);
                if (onChange != null)
                {
                    toCall = onChange;
                    changes = new Change[deadAddresses.Length];
                    for (int ind = 0; ind < deadAddresses.Length; ind++)
                        changes[ind] = new Change(deadAddresses[ind], Action.CRASHED);
                }
            }

            if (toCall != null)
                toCall(changes);
        }

        private event ChangeCallback onChange;
        private List<QS._core_c_.Base3.InstanceID> cachedDeadAddresses = new List<QS._core_c_.Base3.InstanceID>();

        #region IFailureDetector Members

        event ChangeCallback IFailureDetector.OnChange
        {
            add 
            {
                Change[] changes = null;
                ChangeCallback toCall = null;

                lock (this)
                {
                    onChange += value;

                    if (cachedDeadAddresses.Count > 0)
                    {
                        toCall = value;
                        changes = new Change[cachedDeadAddresses.Count];
                        int ind = 0;
                        foreach (QS._core_c_.Base3.InstanceID deadAddress in cachedDeadAddresses)
                            changes[ind++] = new Change(deadAddress, Action.CRASHED);
                    }
                }

                if (toCall != null)
                    toCall(changes);
            }

            remove { onChange -= value; }
        }

        #endregion
    }
}
