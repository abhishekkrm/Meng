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

namespace QS._qss_e_.Launchers_
{
    [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Expanded, QS.Fx.Printing.SelectionOption.Implicit)]
    [Serializable]
    [XmlType("experiment")]
    public class ExperimentSpecification
    {
        public static ExperimentSpecification Load(string filename)
        {
            ExperimentSpecification specification;
            using (StreamReader reader = new StreamReader(filename))
            {
                specification = (ExperimentSpecification)(new XmlSerializer(typeof(ExperimentSpecification))).Deserialize(reader);
            }
            return specification;
        }

        public ExperimentSpecification(
            string experimentClass, Argument[] arguments, string[] subnets, string experimentNode, string[] workerNodes, 
            int appsPerNode, string cryptographicKey, string path, string executablePath, string[] files, string user, string pass, string domain,
            string mailaccount, string mailpassword, string maildomain, string mailserver, string mailfrom, string mailto, string mailtoname)
        {
            this.experimentClass = experimentClass;
            this.arguments = arguments;
            this.control_subnets = subnets;
            this.experimental_subnets = new string[0];
            this.experimentNode = experimentNode;
            this.workerNodes = workerNodes;
            this.applicationsPerNode = appsPerNode;
            this.cryptographicKey = cryptographicKey;
            this.path = path;
            this.executablePath = executablePath;
            this.files = files;
            this.user = user;
            this.pass = pass;
            this.domain = domain;
            this.mailaccount = mailaccount;
            this.mailpassword = mailpassword;
            this.maildomain = maildomain;
            this.mailserver = mailserver;
            this.mailfrom = mailfrom;
            this.mailto = mailto;
            this.mailtoname = mailtoname;
        }

        public ExperimentSpecification()
        {
        }

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                (new XmlSerializer(typeof(ExperimentSpecification))).Serialize(writer, this);
            }
        }

        #region Class Argument

        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact, QS.Fx.Printing.SelectionOption.Explicit)]
        [Serializable]
        [XmlType("argument")]
        public class Argument
        {
            public Argument()
            {
            }

            public Argument(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact)]
            private string name, value;

            [XmlAttribute("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [XmlAttribute("value")]
            public string Value
            {
                get { return value; }
                set { this.value = value; }
            }
        }

        #endregion

        private string experimentClass, experimentNode, cryptographicKey, path, executablePath, user, pass, domain, machinesfile;
        private string[] workerNodes, files, control_subnets, experimental_subnets;
        private int applicationsPerNode;
        [QS.Fx.Printing.Printable(QS.Fx.Printing.PrintingStyle.Compact)]
        private Argument[] arguments;
        private string mailaccount, mailpassword, maildomain, mailserver, mailfrom, mailto, mailtoname, profilednodes;
        private bool restart, upload, debug, profile, profilememory;

        [XmlElement("class")]
        public string Class
        {
            get { return experimentClass; }
            set { experimentClass = value; }
        }

        [XmlElement("argument")]
        public Argument[] Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        [XmlElement("key")]
        public string CryptographicKey
        {
            get { return cryptographicKey; }
            set { cryptographicKey = value; }
        }

        [XmlElement("control_subnet")]
        public string[] Control_Subnets
        {
            get { return control_subnets; }
            set { control_subnets = value; }
        }

        [XmlElement("experimental_subnet")]
        public string[] Experimental_Subnets
        {
            get { return experimental_subnets; }
            set { experimental_subnets = value; }
        }

        [XmlElement("controller")]
        public string Controller
        {
            get { return experimentNode; }
            set { experimentNode = value; }
        }

        [XmlElement("restart")]
        public bool Restart
        {
            get { return restart; }
            set { restart = value; }
        }

        [XmlElement("machines")]
        public string Machines
        {
            get { return machinesfile; }
            set { machinesfile = value; }
        }

        [XmlElement("worker")]
        public string[] Workers
        {
            get { return workerNodes; }
            set { workerNodes = value; }
        }

        [XmlElement("concurrency")]
        public int Concurrency
        {
            get { return applicationsPerNode; }
            set { applicationsPerNode = value; }
        }

        [XmlElement("upload")]
        public bool Upload
        {
            get { return upload; }
            set { upload = value; }
        }

        [XmlElement("file")]
        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }

        [XmlElement("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        [XmlElement("executable")]
        public string Executable
        {
            get { return executablePath; }
            set { executablePath = value; }
        }

        [XmlElement("debug")]
        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        [XmlElement("profile")]
        public bool Profile
        {
            get { return profile; }
            set { profile = value; }
        }

        [XmlElement("profilememory")]
        public bool ProfileMemory
        {
            get { return profilememory; }
            set { profilememory = value; }
        }

        [XmlElement("profilednodes")]
        public string ProfiledNodes
        {
            get { return profilednodes; }
            set { profilednodes = value; }
        }

        [XmlElement("user")]
        public string User
        {
            get { return user; }
            set { user = value; }
        }

        [XmlElement("password")]
        public string Password
        {
            get { return pass; }
            set { pass = value; }
        }

        [XmlElement("domain")]
        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }

        [XmlElement("mailaccount")]
        public string MailAccount
        {
            get { return mailaccount; }
            set { mailaccount = value; }
        }
            
        [XmlElement("mailpassword")]
        public string MailPassword
        {
            get { return mailpassword; }
            set { mailpassword = value; }
        }
            
        [XmlElement("maildomain")]
        public string MailDomain
        {
            get { return maildomain; }
            set { maildomain = value; }
        }
            
        [XmlElement("mailserver")]
        public string MailServer
        {
            get { return mailserver; }
            set { mailserver = value; }
        }
            
        [XmlElement("mailfrom")]
        public string MailFrom
        {
            get { return mailfrom; }
            set { mailfrom = value; }
        }
            
        [XmlElement("mailto")]
        public string MailTo
        {
            get { return mailto; }
            set { mailto = value; }
        }

        [XmlElement("mailtoname")]
        public string MailToName
        {
            get { return mailtoname; }
            set { mailtoname = value; }
        }
    }
}
