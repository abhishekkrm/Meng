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

namespace QS._qss_d_.Scheduler_3_
{
    [Serializable]
    [XmlType("task")]
    public class TaskSpecification
    {
        public static TaskSpecification Load(string filename)
        {
            TaskSpecification task;
            using (StreamReader reader = new StreamReader(filename))
            {
                task = (TaskSpecification)(new XmlSerializer(typeof(TaskSpecification))).Deserialize(reader);
            }
            return task;
        }

        public TaskSpecification(
            string name, string description, string executable, string arguments, DateTime time, double priority, IEnumerable<File> files)
        {
            this.name = name;
            this.description = description;
            this.executable = executable;
            this.arguments = arguments;
            this.time = time;
            this.priority = priority;
            this.files = (new List<File>(files)).ToArray();
        }

        public TaskSpecification()
        {
        }

        private string name, description, executable, arguments;
        private DateTime time;
        private double priority;
        private File[] files;

        public void Save(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                (new XmlSerializer(typeof(TaskSpecification))).Serialize(writer, this);
            }
        }

        public ITask CreateTask(IScheduler scheduler)
        {
            ITask task = scheduler.CreateTask(name, description, time, priority, executable, arguments);
            if (files != null)
            {
                foreach (File file in files)
                {
                    byte[] contents = null;
                    using (FileStream stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                    {
                        contents = new byte[stream.Length];
                        if (stream.Read(contents, 0, (int)stream.Length) != stream.Length)
                            throw new Exception("Could not read the whole file.");
                    }
                    task.Upload(file.Name, contents);
                }
            }
            return task;
        }

        #region Class File

        [Serializable]
        [XmlType("file")]
        public class File
        {
            public File(string name, string path)
            {
                this.name = name;
                this.path = path;
            }

            public File()
            {
            }

            private string name, path;

            [XmlElement("name")]
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            [XmlElement("path")]
            public string Path
            {
                get { return path; }
                set { path = value; }
            }
        }

        #endregion

        [XmlElement("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlElement("description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [XmlElement("executable")]
        public string Executable
        {
            get { return executable; }
            set { executable = value; }
        }

        [XmlElement("arguments")]
        public string Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        [XmlElement("time")]
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        [XmlElement("priority")]
        public double Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        [XmlElement("file")]
        public File[] Files
        {
            get { return files; }
            set { files = value; }
        }
    }
}
