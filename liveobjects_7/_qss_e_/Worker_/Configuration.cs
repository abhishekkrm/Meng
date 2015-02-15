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

namespace QS._qss_e_.Worker_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Explicit)]
    [Serializable]
    public class Configuration
    {
        #region Default

        public static Configuration Default
        {
            get
            {
                return new Configuration(1, QS._qss_c_.Base1_.Subnet.Any);
            }
        }

        #endregion

        #region Constructors

        public Configuration(int concurrency, QS._qss_c_.Base1_.Subnet subnet)
        {
            this.concurrency = concurrency;
            this.subnet = subnet;
        }

        public Configuration()
        {
        }

        #endregion

        [QS.Fx.Printing.Printable]
        private int concurrency;
        [QS.Fx.Printing.Printable]
        private QS._qss_c_.Base1_.Subnet subnet;

        #region Accessors

        public int Concurrency
        {
            get { return concurrency; }
            set { concurrency = value; }
        }

        public QS._qss_c_.Base1_.Subnet Subnet
        {
            get { return subnet; }
            set { subnet = value; }
        }

        #endregion

        #region Loading and Saving

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                (new XmlSerializer(typeof(Configuration))).Serialize(writer, this);
            }
        }

        public static Configuration Load(string filename)
        {
            Configuration configuration;
            using (StreamReader reader = new StreamReader(filename))
            {
                configuration = (Configuration)(new XmlSerializer(typeof(Configuration))).Deserialize(reader);
            }
            return configuration;
        }

        #endregion
    }
}
