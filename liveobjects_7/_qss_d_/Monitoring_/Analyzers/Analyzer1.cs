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

// #define DEBUG_CollectStatistics

using System;
using System.Collections.Generic;
using System.Text;

namespace QS._qss_d_.Monitoring_.Analyzers
{
    [Analyzer("Analyzer extracting message seqno from Multicasting5.MessageRV messages.")]
    public class Analyzer1 : QS.Fx.Inspection.Inspectable, IAnalyzer
    {
        public Analyzer1()
        {
        }

        private IListener dataSource;

#if DEBUG_CollectStatistics
        [CMS.Diagnostics.Component("Receive Times (X = time, Y = seqno)")]
        private CMS.Statistics.SamplesXY sequenceNumbers = new QS.CMS.Statistics.SamplesXY();

        [CMS.Diagnostics.Component("Receive Times (X = seqno, Y = time)")]
        private CMS.Statistics.Samples earliestReceiveTimes = 
            new QS.CMS.Statistics.Samples(CMS.Statistics.Samples.LoggingOption.First);        
#endif

        private void analyze()
        {            
            foreach (IPacket packet in dataSource.Received)
            {
                if (packet.Channel == (uint) ReservedObjectID.Rings6_SenderController1_DataChannel && 
                    packet.Contents is QS._qss_c_.Multicasting5.MessageRV)
                {
                    QS._qss_c_.Multicasting5.MessageRV message = (QS._qss_c_.Multicasting5.MessageRV) packet.Contents;
#if DEBUG_CollectStatistics
                    sequenceNumbers.addSample(packet.Time, message.SeqNo);
                    earliestReceiveTimes.addSample((int) message.SeqNo, packet.Time);
#endif
                }
            }
        }   

        #region IAnalyzer Members

        IListener IAnalyzer.DataSource
        {
            get { return dataSource; }
            set 
            {
                lock (this)
                {
                    if (value != dataSource)
                    {                     
                        dataSource = value;
                        this.analyze();
                    }
                }
            }
        }

        #endregion
    }
}
