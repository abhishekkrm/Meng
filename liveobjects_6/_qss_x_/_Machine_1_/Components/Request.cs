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

namespace QS._qss_x_._Machine_1_.Components
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    public sealed class Request : QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message>
    {
        #region Constructors

        public Request(string machineName, QS._qss_x_.Base1_.Address requestorAddress, uint machineIncarnation, uint viewSeqNo, 
            uint messageSeqNo, IList<Submission> submissions, QS._core_c_.Base6.CompletionCallback<object> transmissionCompletionCallback)
        {
            this.machineName = machineName;
            this.requestorAddress = requestorAddress;
            this.machineIncarnation = machineIncarnation;
            this.viewSeqNo = viewSeqNo;
            this.messageSeqNo = messageSeqNo;
            this.submissions = submissions;
            this.transmissionCompletionCallback = transmissionCompletionCallback;
        }

        #endregion

        #region Fields

        private string machineName;
        private uint machineIncarnation, viewSeqNo, messageSeqNo;
        private IList<Submission> submissions;
        private QS._core_c_.Base6.CompletionCallback<object> transmissionCompletionCallback;
        private QS._qss_x_.Base1_.Address requestorAddress;

        #endregion

        #region Accessors

        [QS.Fx.Printing.Printable]
        public string MachineName
        {
            get { return machineName; }
        }

        [QS.Fx.Printing.Printable]
        public QS._qss_x_.Base1_.Address RequestorAddress
        {
            get { return requestorAddress; }
            set { requestorAddress = value; }
        }

        [QS.Fx.Printing.Printable]
        public uint MachineIncarnation
        {
            get { return machineIncarnation; }
        }

        [QS.Fx.Printing.Printable]
        public uint ViewSeqNo
        {
            get { return viewSeqNo; }
        }

        [QS.Fx.Printing.Printable]
        public uint MessageSeqNo
        {
            get { return messageSeqNo; }
        }

        [QS.Fx.Printing.Printable]
        public IList<Submission> Submissions
        {
            get { return submissions; }
        }

        #endregion

        #region IAsynchronous<Message,object> Members

        QS._core_c_.Base3.Message QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Argument
        {
            get 
            {
                List<ServiceControl.IServiceControllerOperation> operations =
                    new List<QS._qss_x_._Machine_1_.ServiceControl.IServiceControllerOperation>(submissions.Count);
                foreach (Submission submission in submissions)
                    operations.Add(submission.Operation);
                return new QS._core_c_.Base3.Message((uint)QS.ReservedObjectID.Fx_Machine_Components_Replica,
                    new Append(machineName, requestorAddress, machineIncarnation, viewSeqNo, messageSeqNo, operations));
            }
        }

        object QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.Context
        {
            get { return this; }
        }

        QS._core_c_.Base6.CompletionCallback<object> QS._core_c_.Base6.IAsynchronous<QS._core_c_.Base3.Message, object>.CompletionCallback
        {
            get { return transmissionCompletionCallback; }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return QS.Fx.Printing.Printable.ToString(this);
        }

        #endregion
    }
}
