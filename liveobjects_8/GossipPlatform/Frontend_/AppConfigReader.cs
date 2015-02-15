/*

Copyright (c) 2004-2009 Deepak Nataraj. All rights reserved.

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
using System.Configuration;

namespace GOTransport.Frontend
{
    /// <summary>
    /// read and store configuration values set in the config file
    /// </summary>
    public class AppConfigReader
    {
        #region private fields

        private Dictionary<String, String> configKV;

        #endregion

        #region constructor

        /// <summary>
        /// read the values from configuration file and save them in a list of InstanceContainer
        /// </summary>
        public AppConfigReader()
        {
            string _numberOfRumors = ConfigurationManager.AppSettings["NUMBER_OF_RUMORS"];
            string destination = ConfigurationManager.AppSettings["RUMOR_DESTINATION_NODE"];
            string _sendingInterval = ConfigurationManager.AppSettings["SENDING_INTERVAL"];
            string _ttl = ConfigurationManager.AppSettings["HOP_COUNT"];

            string [] allConfigKeys = ConfigurationManager.AppSettings.AllKeys;

            configKV = new Dictionary<string, string>();
            foreach (string key in allConfigKeys)
            {
                configKV.Add(key, ConfigurationManager.AppSettings[key]);
            }
        }

        #endregion

        #region public methods

        public int ReadIntConfig(String _input)
        {
            if (configKV.ContainsKey(_input))
            {
                return Int32.Parse(configKV[_input]);
            }

            throw new Exception("AppConfigReader::ReadConfig: Cannot find config value for " + _input);
        }

        public String ReadStrConfig(String _input)
        {
            if (configKV.ContainsKey(_input))
            {
                return configKV[_input];
            }

            return null;
        }

        public String[] AllKeys()
        {
            return ConfigurationManager.AppSettings.AllKeys;
        }

        #endregion
    }
}

