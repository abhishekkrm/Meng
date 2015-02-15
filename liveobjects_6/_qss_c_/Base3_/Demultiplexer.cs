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
    public class Demultiplexer : QS.Fx.Inspection.Inspectable, IDemultiplexer
    {
        public Demultiplexer(QS.Fx.Logging.ILogger logger, QS.Fx.Logging.IEventLogger eventLogger)
        {
            this.eventLogger = eventLogger;
            this.logger = logger;
            mappings = new QS._core_c_.Collections.Hashtable(100);
        }

        private QS.Fx.Logging.ILogger logger;
        private QS.Fx.Logging.IEventLogger eventLogger;
        private QS._core_c_.Collections.Hashtable mappings;

        #region IDemultiplexer Members

        void IDemultiplexer.register(uint localObjectID, ReceiveCallback receiveCallback)
        {
            mappings[localObjectID] = receiveCallback;
        }

        void IDemultiplexer.unregister(uint localObjectID)
        {
            mappings.remove(localObjectID);
        }

        QS.Fx.Serialization.ISerializable IDemultiplexer.dispatch(uint destinationLOID, QS._core_c_.Base3.InstanceID sourceAddress, QS.Fx.Serialization.ISerializable receivedObject)
        {
            try
            {
                return ((ReceiveCallback)mappings[destinationLOID])(sourceAddress, receivedObject);
            }
            catch (Exception exc)
            {
				string destinationString;
				try
				{
					destinationString = ((ReservedObjectID)destinationLOID).ToString();
				}
				catch (Exception)
				{
					destinationString = destinationLOID.ToString();
				}

				logger.Log("Cannot dispatch message of type { " + receivedObject.GetType().ToString() + 
                    " } from { " + sourceAddress.ToString() + " } to { "+ destinationString + " }.\n" + exc.ToString());

                if (eventLogger.Enabled)
                    eventLogger.Log(new Logging_1_.Events.ExceptionCaught(double.NaN, null, exc));
            }

            return null;
        }

        #endregion
    }
}
