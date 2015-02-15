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

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Base3_
{
    public abstract class AsynchronousOperation : Base3_.IAsynchronousOperation
    {
        public AsynchronousOperation(Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
        {
            this.completionCallback = completionCallback;
            this.asynchronousState = asynchronousState;

            completed = cancelled = false;
            completionEvent = new System.Threading.ManualResetEvent(false);
            ignoreCallback = completionCallback == null;
        }

        private Base3_.AsynchronousOperationCallback completionCallback;
        private object asynchronousState;
        private bool completed, cancelled, ignoreCallback;
        private System.Threading.ManualResetEvent completionEvent;

        public abstract void Unregister();

        #region IAsynchronousOperation Members

		[QS.Fx.Base.Inspectable]
		public bool Cancelled
        {
            get { return cancelled; }
        }

        public void Cancel()
        {
            lock (this)
            {
                if (!completed && !cancelled)
                {
                    cancelled = true;
                    this.Unregister();
                }
            }
        }

        public void Ignore()
        {
            ignoreCallback = true;
        }

        #endregion

        #region IAsyncResult Members

		[QS.Fx.Base.Inspectable]
		public object AsyncState
        {
            get { return asynchronousState; }
        }

        public System.Threading.WaitHandle AsyncWaitHandle
        {
            get { return completionEvent; }
        }

		[QS.Fx.Base.Inspectable]
		public bool CompletedSynchronously
        {
            get { return false; }
        }

		[QS.Fx.Base.Inspectable(QS.Fx.Base.AttributeAccess.ReadOnly)]
		public bool IsCompleted
        {
            get { return completed; }
            
            set
            {
                lock (this)
                {
                    if (!completed && !cancelled)
                    {
                        completed = true;
                        completionEvent.Set();

                        if (!ignoreCallback)
                            this.completionCallback(this);

                        this.Unregister();
                    }
                }
            }
        }

        #endregion

		#region Class Inspectable

		public abstract class Inspectable : AsynchronousOperation, QS.Fx.Inspection.IInspectable
		{
			public Inspectable(Base3_.AsynchronousOperationCallback completionCallback, object asynchronousState)
				: base(completionCallback, asynchronousState)
			{
			}

			#region IInspectable Members

			private QS.Fx.Inspection.IAttributeCollection inspectableCollection = null;
			QS.Fx.Inspection.IAttributeCollection QS.Fx.Inspection.IInspectable.Attributes
			{
				get 
				{ 
					if (inspectableCollection == null)
					{
						lock (this)
						{
							if (inspectableCollection == null)
								inspectableCollection = new QS.Fx.Inspection.AttributesOf(this);
						}
					}

					return inspectableCollection;
				}
			}

			#endregion
		}

		#endregion
	}
}
