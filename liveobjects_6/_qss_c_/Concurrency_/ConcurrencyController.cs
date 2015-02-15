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
using System.Threading;

namespace QS._qss_c_.Concurrency_
{
    public class ConcurrencyController : IConcurrencyController
    {
        public ConcurrencyController(double minimumCredit, double maximumCredit)
        {
            this.minimumCredit = minimumCredit;
            this.maximumCredit = maximumCredit;
        }

        private double minimumCredit, maximumCredit, consumedCredit;
        private Queue<IControlled> controlledQueue = new Queue<IControlled>();

        #region Internal Processing

        private void ProcessQueue()
        {
            while (controlledQueue.Count > 0)
            {
                double availableCredit = maximumCredit - consumedCredit;
                if (availableCredit > minimumCredit)
                {
                    IControlled controlled = controlledQueue.Dequeue();

                    double actuallyConsumed;
                    bool moreToGo;

                    Monitor.Exit(this);
                    try
                    {
                        controlled.Consume(availableCredit, out actuallyConsumed, out moreToGo);
                    }
                    finally
                    {
                        Monitor.Enter(this);
                    }

                    consumedCredit += actuallyConsumed;
                    if (moreToGo)
                        controlledQueue.Enqueue(controlled);
                }
                else
                    break;
            }
        }

        #endregion

        #region IConcurrencyController Members

        void IConcurrencyController.Register(IControlled controlled)
        {
            lock (this)
            {
                controlledQueue.Enqueue(controlled);
                ProcessQueue();
            }
        }

        void IConcurrencyController.Release(double credit)
        {
            lock (this)
            {
                consumedCredit -= credit;
                ProcessQueue();
            }
        }

        double IConcurrencyController.MaximumCredit
        {
            get { return maximumCredit; }
            set 
            {
                maximumCredit = value;
                ProcessQueue();
            }
        }

        #endregion
    }
}
