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

namespace QS._qss_x_.Administrator_
{
    [XmlType("Configuration")]
    public sealed class Configuration
    {
        #region Constructor

        public Configuration()
        {
        }

        #endregion

        #region Fields

        private string filename, subnet, port, rootfolder, authentication, certificate;
        private bool verbose, exec;

        #endregion

        #region Accessors

        [XmlElement("Subnet")]
        public string Subnet
        {
            get { return subnet; }
            set { subnet = value; }
        }

        [XmlElement("Port")]
        public string Port
        {
            get { return port; }
            set { port = value; }
        }

        [XmlElement("Verbose")]
        public bool Verbose
        {
            get { return verbose; }
            set { verbose = value; }
        }

        [XmlElement("Root")]
        public string Root
        {
            get { return rootfolder; }
            set { rootfolder = value; }
        }

        [XmlElement("Exec")]
        public bool Exec
        {
            get { return exec; }
            set { exec = value; }
        }

        [XmlElement("Authentication")]
        public string Authentication
        {
            get { return authentication; }
            set { authentication = value; }
        }

        [XmlElement("Certificate")]
        public string Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }

        #endregion

        #region Load

        public static Configuration Load(string filename)
        {
            using (StreamReader reader = new StreamReader(filename))
            {
                Configuration configuration = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
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
