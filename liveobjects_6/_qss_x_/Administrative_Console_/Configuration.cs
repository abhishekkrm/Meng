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
using System.Xml.Serialization;
using System.IO;

namespace QS._qss_x_.Administrative_Console_
{
    [XmlType("Configuration")]
    public class Configuration
    {
        #region Constructor

        public Configuration(Connection[] connections)
        {
            this.connections = connections;
        }

        public Configuration()
        {
        }

        #endregion

        #region Fields

        private string filename;
        private Connection[] connections;

        #endregion

        #region Accessors

        [XmlElement("Connection")]
        public Connection[] Connections
        {
            get { return connections; }
            set { connections = value; }
        }

        #endregion

        #region Class Connection

        [XmlType("Connection")]
        public class Connection
        {
            public Connection(string name, string address)
            {
                this.name = name;
                this.address = address;
            }

            public Connection()
            {
            }

            private string name, address;

            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [XmlAttribute("address")]
            public string Address
            {
                get { return address; }
                set { address = value; }
            }
        }

        #endregion

        #region Load

        public static Configuration Load(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                Configuration configuration = (Configuration) (new XmlSerializer(typeof(Configuration))).Deserialize(reader); 
                configuration.filename = filename;
                return configuration;
            }
        }

        #endregion

        #region Save

        public void Save()
        {
            Save(null);
        }

        public void Save(string filename)
        {
            if (filename != null)
                this.filename = filename;

            using (StreamWriter writer = new StreamWriter(this.filename))
            {
                (new XmlSerializer(typeof(Configuration))).Serialize(writer, this);
            }
        }

        #endregion
    }
}
