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

namespace QS._qss_d_.Scheduler_3_
{
    [Serializable]
    public class Task : IComparable<Task>
    {
        public Task()
        {
        }

        public Task(string name, string description, DateTime time, double priority, string executable, string arguments)
        {
            this.name = name;
            this.description = description;
            this.time = time;
            this.priority = priority;
            this.executable = executable;
            this.arguments = arguments;
            this.status = TaskStatus.Disabled;
        }

        private string name, description, executable, arguments;
        private DateTime time;
        private double priority;
        private TaskStatus status;
        private List<string> files;

        #region Accessors

        public string[] Files
        {
            get { return (files != null) ? files.ToArray() : null; }
            set 
            {
                if (value != null)
                    files = new List<string>(value);
                else
                    files = new List<string>();
            }
        }

        public TaskStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Executable
        {
            get { return executable; }
            set { executable = value; }
        }

        public string Arguments
        {
            get { return arguments; }
            set { arguments = value; }
        }

        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        public double Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        #endregion

        public void AddFile(string name)
        {
            if (files == null)
                files = new List<string>();
            if (!files.Contains(name))
                files.Add(name);
        }

        #region IComparable<Task> Members

        int IComparable<Task>.CompareTo(Task other)
        {
            int result = status.CompareTo(other.status);
            if (result != 0)
                return result;
            result = priority.CompareTo(other.priority);
            if (result != 0)
                return result;
            return time.CompareTo(other.time);
        }

        #endregion
    }
}
