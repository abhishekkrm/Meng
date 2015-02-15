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

// #define DEBUG_Unwrapper
// #define Calculate_Statistics

#region Using directives

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace QS._qss_c_.Buffering_3_
{
    public class Unwrapper
    {
        public Unwrapper(uint loid, Base3_.IDemultiplexer demultiplexer, IControllerClass controllerClass, QS.Fx.Logging.ILogger logger)
        {
            this.controllerClass = controllerClass;
            this.myReceiveCallback = new QS._qss_c_.Base3_.ReceiveCallback(receiveCallback);
            this.demultiplexer = demultiplexer;
            this.logger = logger;

            demultiplexer.register(loid, this.ReceiveCallback);
        }

        private Base3_.IDemultiplexer demultiplexer;
        private IControllerClass controllerClass;
        private Base3_.ReceiveCallback myReceiveCallback;
        private QS.Fx.Logging.ILogger logger;

#if Calculate_Statistics
		private Statistics.Samples messageCountSamples = new QS.CMS.Statistics.Samples();
//		private System.Collections.Generic.List<TMS.Data.XY> samples = new List<QS.TMS.Data.XY>();
//
		public TMS.Data.IDataSet MessageCountStatistics
		{
			get { return messageCountSamples.DataSet; }
		}
#endif

		public Base3_.ReceiveCallback ReceiveCallback
        {
            get
            {
                return myReceiveCallback;
            }
        }

		private QS.Fx.Serialization.ISerializable receiveCallback(QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
// #if DEBUG_Unwrapper
//            logger.Log(this, "__________ReceiveCallback : Enter");
// #endif

// #if Calculate_Statistics
// 			int nmessages_wrapped = 0;
// #endif

//			IMessageCollection messageCollection = receivedObject as IMessageCollection;
//			if (messageCollection == null)
//				throw new Exception("Wrong received object type: Unwrapper was expecting IMessageCollection.");
//
// #if Calculate_Statistics
//			messageCountSamples.addSample(messageCollection.Count);
// #endif

#if DEBUG_Unwrapper
			int nmessages = 0;
#endif
			foreach (QS._core_c_.Base3.Message message in controllerClass.GetMessages(receivedObject))
			{
#if DEBUG_Unwrapper
				nmessages++;
#endif
				demultiplexer.dispatch(message.destinationLOID, sourceAddress, message.transmittedObject);
			}

#if DEBUG_Unwrapper
			logger.Log(this, "__________Unwrapped : " + nmessages.ToString());
#endif


// #if Calculate_Statistics
//			samples.addSample(new TMS.Data.XY());
// #endif

			return null;
        }
    }
}
